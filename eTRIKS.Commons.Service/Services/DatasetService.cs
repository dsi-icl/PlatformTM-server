using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.Service.DTOs;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.Timing;
using eTRIKS.Commons.Core.JoinEntities;

namespace eTRIKS.Commons.Service.Services
{
    public class DatasetService
    {
        private readonly IServiceUoW _dataServiceUnit;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<VariableDefinition, int> _variableDefinitionRepository;
        private readonly IRepository<DataFile, int> _dataFileRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;
        private readonly IRepository<Observation, int> _observationRepository;
        private readonly FileService _fileService;

        public DatasetService(IServiceUoW uoW, FileService fileService)
        {
            _dataServiceUnit = uoW;
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _variableDefinitionRepository = uoW.GetRepository<VariableDefinition, int>();
            _dataFileRepository = uoW.GetRepository<DataFile, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();
            _observationRepository = uoW.GetRepository<Observation, int>();

            _fileService = fileService;
        }

       

        /// <summary>
        /// Retrieves dataset for selected activity including Variable_Defs
        /// </summary>
        /// <param name="datasetId"></param>
        private Dataset GetActivityDataset(int datasetId)
        {
            Dataset ds = _datasetRepository.FindSingle(
                d => d.Id.Equals(datasetId),
                new List<string>()
                {
                    "Variables.VariableDefinition",
                    "DataFiles",
                    "Activity.Project",
                    "Domain"
                });
            return ds;
        }

        /// <summary>
        /// Retrieves datasetDTO for selected activity including a union of VarDefs and TemplateVariables from the relevant Domain
        /// //TODO: This method should change once users are allowed to add their own Variables
        //          In this case VarDEFs should take precedence and the issue of adding CVterms to VarDEFs shuold be settled
        /// </summary>
        /// <param name="datasetId"></param>
        /// <returns></returns>
        public DatasetDTO GetActivityDatasetDTO(int datasetId)
        {
            DatasetDTO dto = new DatasetDTO();
            Dataset ds = _datasetRepository.FindSingle(
                d => d.Id.Equals(datasetId),
                new List<string>()
                {
                    "Variables.VariableDefinition",
                    "Domain.Variables.controlledTerminology.Xref.DB",
                    "Activity"
                });

            dto.Id = ds.Id; //Set DatasetDTO id to Dataset.Id (int)
            dto.Class = ds.Domain.Class;
            dto.Description = ds.Domain.Description;
            dto.Name = ds.Domain.Name;
            dto.DomainId = ds.Domain.Id;
            dto.Structure = ds.Domain.Structure;
            dto.Code = ds.Domain.Code;
            dto.ProjectId = ds.Activity.ProjectId;

           

            foreach (DomainVariableTemplate vt in ds.Domain.Variables.OrderBy(v => v.Order))
                //foreach (var vt in ds.Variables)
            {
                DatasetVariableDTO dv = new DatasetVariableDTO();
                dv.Name = vt.Name;
                dv.Description = vt.Description;
                dv.Label = vt.Label;
                dv.Accession = vt.Id;
                dv.DataType = vt.DataType;
                dv.IsCurated = true;
                dv.RoleId = vt.RoleId;

                if (vt.controlledTerminology != null)
                {
                    dv.DictionaryName = vt.controlledTerminology.Name;
                    dv.DictionaryDefinition = vt.controlledTerminology.Definition;
                    dv.DictionaryXrefURL = vt.controlledTerminology.Xref.DB.UrlPrefix +
                                           vt.controlledTerminology.Xref.Accession;
                }

                var vr = ds.Variables.SingleOrDefault(v => v.VariableDefinition.Name.Equals(vt.Name));
                if (vr != null)
                {
                     
                    dv.IsRequired = vr.IsRequired;
                    dv.KeySequence = vr.KeySequence;
                    dv.OrderNumber = vr.OrderNumber;
                    dv.Id = vr.VariableDefinitionId;
                    dv.isSelected = true;
                }
                dto.variables.Add(dv);
            }

            
            foreach (VariableReference vr in ds.Variables)
            {
                
                if (!dto.variables.Exists(v => v.Accession.Equals(vr.VariableDefinition.Accession)))
                {
                    DatasetVariableDTO dv = new DatasetVariableDTO();
                    dv.Name = vr.VariableDefinition.Name;
                    dv.Description = vr.VariableDefinition.Description;
                    dv.Label = vr.VariableDefinition.Label;
                    dv.Accession = vr.VariableDefinition.Accession;
                    dv.DataType = vr.VariableDefinition.DataType;
                    dv.IsCurated = vr.VariableDefinition.IsCurated;
                    dv.IsComputed = vr.VariableDefinition.IsComputed ?? false;
                    dv.RoleId = vr.VariableDefinition.RoleId;

                    dv.IsRequired = vr.IsRequired;
                    dv.KeySequence = vr.KeySequence;
                    dv.OrderNumber = vr.OrderNumber;
                    dv.Id = vr.VariableDefinitionId;
                    dv.isSelected = true;

                    dto.variables.Add(dv);
                }
                

            }

            return dto;
        }

        public Dataset addDataset(DatasetDTO datasetDTO)
        {
            //1. Fields for Dataset
            var dataset = CreateDataset(datasetDTO);

            _datasetRepository.Insert(dataset);
            return _dataServiceUnit.Save().Equals("CREATED") ? dataset : null;
        }

        public Dataset CreateDataset(DatasetDTO datasetDTO)
        {
            if (datasetDTO == null)
                return null;
            var dataset = new Dataset {ActivityId = datasetDTO.ActivityId, DomainId = datasetDTO.DomainId};


            // Get any exisiting variable definitions for that study
            List<VariableDefinition> variableDefsOfStudy = getVariableDefinitionsOfStudy(datasetDTO.ProjectId).ToList();

            foreach (var variableDto in datasetDTO.variables.Where(variableDto => variableDto.isSelected))
            {
                
                //Compare newly added Variable to previously added VarDefs using accession string 
                //since no Id for Variable created yet
                var varDef = variableDefsOfStudy.SingleOrDefault(
                    d => d.Accession.Equals(variableDto.Accession));
                if (varDef == null)
                {
                    varDef = new VariableDefinition();
                    varDef.Accession = variableDto.Accession;
                    varDef.Name = variableDto.Name;
                    varDef.Label = variableDto.Label;
                    varDef.Description = variableDto.Description;
                    varDef.DataType = variableDto.DataType;
                    varDef.IsCurated = variableDto.IsCurated;
                    varDef.RoleId = variableDto.RoleId;
                    varDef.ProjectId = datasetDTO.ProjectId;

                    varDef.VariableTypeStr = variableDto.varType;

                    if (variableDto.IsComputed)
                    {
                        varDef.IsComputed = true;
                        varDef.ComputedVarExpression = variableDto.ExpressionList.Select(t => t.val).Aggregate((i, j) => i + ',' + j);
                    }

                }
                //3. Fields for varRefList
                VariableReference varRef = new VariableReference();
                varRef.OrderNumber = variableDto.OrderNumber;
                varRef.IsRequired = variableDto.IsRequired;
                varRef.KeySequence = variableDto.KeySequence;
                varRef.VariableDefinition = varDef;
                dataset.Variables.Add(varRef);
            }
            return dataset;
        }
        
        public string UpdateDataset(DatasetDTO datasetDTO)
        {
            Dataset datasetToUpdate = GetActivityDataset(datasetDTO.Id);
            datasetToUpdate.State = datasetDTO.State;
            
            List<VariableDefinition> variableDefsOfStudy = getVariableDefinitionsOfStudy(datasetDTO.ProjectId).ToList();

            var datasetVarsToUpdate = new HashSet<string>(
                datasetToUpdate.Variables.Select(c => c.VariableDefinition.Name));

            foreach (var variableDto in datasetDTO.variables)
            {
                if (variableDto.isSelected)
                {
                    //TODO: NOTE NO UPDATE IS ASSUMED other than adding or removing the whole variable
                    //WILL HAVE tO CHANGE WHEN UPDATING CONTROLLED VOCAB
                    if (!datasetVarsToUpdate.Contains(variableDto.Name))
                    {
                        //add VarDEF and addVarREF
                        VariableDefinition varDef;
                        varDef = variableDefsOfStudy.SingleOrDefault(d => d.Accession.Equals(variableDto.Accession));
                        if (varDef == null)
                        {
                            varDef = new VariableDefinition();
                            varDef.Accession = variableDto.Accession;
                            varDef.Name = variableDto.Name;
                            varDef.Label = variableDto.Label;
                            varDef.Description = variableDto.Description;
                            varDef.IsComputed = variableDto.IsComputed;
                            varDef.DataType = variableDto.DataType;
                            varDef.IsCurated = variableDto.IsCurated;
                            varDef.RoleId = variableDto.RoleId;
                            varDef.ProjectId = datasetDTO.ProjectId;
                            varDef.VariableTypeStr = variableDto.varType;

                            if (variableDto.IsComputed)
                            {
                                varDef.IsComputed = true;
                               
                                varDef.ComputedVarExpression = variableDto.ExpressionList.Select(t => t.val).Aggregate((i, j) => i + ',' + j);
                            }
                        }

                        VariableReference varRef = new VariableReference();
                        varRef.OrderNumber = variableDto.OrderNumber;
                        varRef.IsRequired = variableDto.IsRequired;
                        varRef.KeySequence = variableDto.KeySequence;
                        varRef.VariableDefinition = varDef;
                        datasetToUpdate.Variables.Add(varRef);
                    }
                }
                else if (datasetVarsToUpdate.Contains(variableDto.Name))
                {
                    //remove variable from dataset
                    VariableReference var =
                        datasetToUpdate.Variables.Single(v => v.VariableDefinitionId.Equals(variableDto.Id));
                    datasetToUpdate.Variables.Remove(var);
                }
            }

            _datasetRepository.Update(datasetToUpdate);

            return _dataServiceUnit.Save();
        }

        //TODO: move to Repository 
        public IEnumerable<VariableDefinition> getVariableDefinitionsOfStudy(int studyId)
        {
            return _variableDefinitionRepository.FindAll(d => d.ProjectId.Equals(studyId));
        }

        public DataTemplateMap GetTemplateMaps(int datasetId)
        {
            var map = new DataTemplateMap();

            var ds = _datasetRepository.FindSingle(
                d => d.Id.Equals(datasetId),
                new List<string>()
                {
                    "Variables.VariableDefinition.Role",
                    "Domain",
                    "Activity"
                });
            //when querying for variables exclude synonym and variable qualifiers as these will be retrieved from their associated main variables
            //CL-Role-4 & CL-Role 5
            //Add a property in vardef and templatevar to reference a list of synonyms and a list of qualifier variables

            map.Domain = ds.Domain.Name;
            map.TopicColumns = new List<string>();
            map.ObservationName = ds.Domain.Name.Substring(0, ds.Domain.Name.Length - 1);
            //map.VarTypes = new List<Dictionary<string, List<DataTemplateMap.VariableMap>>>();
            map.VarTypes = new List<DataTemplateMap.VariableType>();
            var ignoredRoles = new List<string>() {"SynonymQualifier", "GroupingQualifier", "Rule"};
            var observationRoles = new List<string>()
            {
                "Topic",
                "RecordQualifier",
                "ResultQualifier",
                "VariableQualifier"
            };

            foreach (var vtg in ds.Variables
                .OrderBy(o => o.VariableDefinition.RoleId) //should be variable.order
                .GroupBy(v => v.VariableDefinition.Role.Name))
            {
                //if (vtg.Key.Equals("SynonymQualifier") || vtg.Key.Equals("GroupingQualifier")) continue;
                if (ignoredRoles.Any(s => s.Equals(vtg.Key))) continue;

                DataTemplateMap.VariableType varType = null;
                if (vtg.Key.Equals("Identifier"))
                {
                    //map.VarTypes.Add("Identifiers", varList = new List<DataTemplateMap.VariableMap>());
                    map.VarTypes.Add(varType = new DataTemplateMap.VariableType()
                    {
                        name = "Identifiers",
                        vars = new List<DataTemplateMap.VariableMap>()
                    });
                }
                if (vtg.Key.Equals("Timing"))
                {
                    //map.VarTypes.Add("Timing Descriptors", varList = new List<DataTemplateMap.VariableMap>());

                    map.VarTypes.Add(varType = new DataTemplateMap.VariableType()
                    {
                        name = "Timing Descriptors",
                        vars = new List<DataTemplateMap.VariableMap>()
                    });
                }

                if (observationRoles.Any(s => s.Equals(vtg.Key)))
                {
                    if (!map.VarTypes.Exists(v => v.name.Equals("Observation Descriptors")))

                        map.VarTypes.Add(varType = new DataTemplateMap.VariableType()
                        {
                            name = "Observation Descriptors",
                            vars = new List<DataTemplateMap.VariableMap>()
                        });
                    else
                    {
                        varType = map.VarTypes.Find(vt => vt.name.Equals("Observation Descriptors"));
                    }
                }

                foreach (var var in vtg.OrderBy(o => o.VariableDefinition.Id)) //orderby orderNo
                {
                    var vgmap = new DataTemplateMap.VariableMap
                    {
                        Label = var.VariableDefinition.Label,
                        ShortName = var.VariableDefinition.Name,
                        Description = var.VariableDefinition.Description,
                        DataType = var.VariableDefinition.DataType,
                        MapToStringValueList = new List<string>(),
                        MapToColList = new List<ColHeaderDTO>()
                    };
                    varType.vars.Add(vgmap);
                }
            }


            return map;
        }

        public int? mapToTemplate(int datasetId, int fileId, DataTemplateMap map)
        {
            var dataset = GetActivityDataset(datasetId);
            var projectId = dataset.Activity.ProjectId;
            //var projectAcc = dataset.Activity.Project.Accession;
            //var dataFile = dataset.DataFiles.SingleOrDefault(df => df.Id.Equals(fileId));

            var dataFile = _dataFileRepository.Get(fileId);
            var filePath = dataFile.Path + "\\" + dataFile.FileName;

            //FileService fileService = new FileService(_dataServiceUnit);
            DataTable inputDataTable = _fileService.ReadOriginalFile(filePath);
            DataTable sdtmTable = new DataTable();

            //var varMaps = new List<DataTemplateMap.VariableMap>();
            foreach (
                var varMap in
                    map.VarTypes.SelectMany(variableType => variableType.vars.Where(varMap => varMap.DataType != null)))
            {
                sdtmTable.Columns.Add(varMap.ShortName); //,Type.GetType(varMap.DataType)
                // varMaps.Add(varMap);
            }


            for (int i = 0; i < map.TopicColumns.Count; i++)
                foreach (DataRow inputRow in inputDataTable.Rows) // Loop over the rows.
                {
                    DataRow sdtmRow = sdtmTable.NewRow();

                    //Identifiers
                    foreach (var varMap in map.VarTypes.Find(vt => vt.name.Equals("Identifiers")).vars)
                    {
                        if (varMap.MapToStringValueList.Count == 0 && varMap.MapToColList.Count == 0)
                            continue;
                        if (varMap.MapToStringValueList[0] != null && varMap.MapToStringValueList[0] != string.Empty)
                        {
                            sdtmRow[varMap.ShortName] = varMap.MapToStringValueList[0];
                        }

                        else if (varMap.MapToColList[0] != null)
                        {
                            var colName = varMap.MapToColList[0].colName;
                            sdtmRow[varMap.ShortName] = inputRow[colName];
                        }
                        //if (varMap.ShortName.Equals("STUDYID"))
                        //    studyId = sdtmRow[varMap.ShortName].ToString();
                    }

                    //Observation Topic & Qualifiers
                    if (map.VarTypes.Exists(vt => vt.name.Equals("Observation Descriptors")))
                    foreach (var varMap in map.VarTypes.Find(vt => vt.name.Equals("Observation Descriptors")).vars)
                    {
                        if (varMap.MapToStringValueList.Count == 0 && varMap.MapToColList.Count == 0)
                            continue;
                        if (varMap.MapToStringValueList[i] != null && varMap.MapToStringValueList[i] != string.Empty)
                        {
                            sdtmRow[varMap.ShortName] = varMap.MapToStringValueList[i];
                        }
                        else if (varMap.MapToColList[i] != null)
                        {
                            var colName = varMap.MapToColList[i].colName;
                            sdtmRow[varMap.ShortName] = inputRow[colName];
                        }
                        else
                        {
                            sdtmRow[varMap.ShortName] = null;
                        }
                    }

                    //Timings
                    if (map.VarTypes.Exists(vt => vt.name.Equals("Timing Descriptors")))
                    foreach (var varMap in map.VarTypes.Find(vt => vt.name.Equals("Timing Descriptors")).vars)
                    {
                        if (varMap.MapToStringValueList.Count == 0 && varMap.MapToColList.Count == 0)
                            continue;
                        if (varMap.MapToStringValueList[0] != null && varMap.MapToStringValueList[0] != string.Empty)
                        {
                            sdtmRow[varMap.ShortName] = varMap.MapToStringValueList[0];
                        }

                        else if (varMap.MapToColList[0] != null)
                        {
                            var colName = varMap.MapToColList[0].colName;
                            sdtmRow[varMap.ShortName] = inputRow[colName];
                        }
                    }
                    sdtmTable.Rows.Add(sdtmRow);
                }

            DataFile standardFile = null;
            if (sdtmTable.Rows.Count != 0)
            {
                string dsName = dataset.Activity.Name+"_" +dataset.DomainId;
                sdtmTable.TableName = dsName;
                //Write new transformed to file 
                var fileInfo = _fileService.writeDataFile(projectId, dataFile.Path, sdtmTable);
                standardFile = _fileService.addOrUpdateFile(projectId, fileInfo);
                //var file = _dataFileRepository.Get(4);
                
                //Update dataset
                //STUPID EF 1.1
                //dataset.DataFiles.Add(standardFile);
                dataset.DataFiles.Add(new DatasetDatafile() {DatasetId = datasetId,DatafileId = dataFile.Id});
                /////////////////////////////////////

                _datasetRepository.Update(dataset);
                _dataServiceUnit.Save();
            }
            dataset.State = "mapped";
            //var ht = new Hashtable();
            //var headerList = new List<Dictionary<string, string>>();
            //foreach (var col in sdtmTable.Columns.Cast<DataColumn>())
            //{
            //    var header = new Dictionary<string, string>
            //    {
            //        {"data", col.ColumnName.ToLower()},
            //        {"title", col.ColumnName}
            //    };
            //    headerList.Add(header);
            //}
            //ht.Add("header", headerList);
            //ht.Add("data", sdtmTable);
            //return ht;
            if (standardFile != null)
                return standardFile.Id;
            else return null;
        }

        public bool PersistSDTM(int datasetId, int fileId)
        {
            var dataset = GetActivityDataset(datasetId);
            var dataFile = _dataFileRepository.Get(fileId);

            if (!dataFile.State.ToLower().Equals("new"))
            {
                /**
                 * Replacing previously loaded file
                 * Remove file from collection before reloading it
                 */
                _sdtmRepository.DeleteMany(s=>s.DatafileId == fileId && s.DatasetId == datasetId);
                Debug.WriteLine("RECORD(s) SUCCESSFULLY DELETED FOR DATASET:" + datasetId + " ,DATAFILE:" + fileId);
            }

            var filePath = dataFile.Path + "\\" + dataFile.FileName;
            //var fileService = new FileService(_dataServiceUnit);
            var dataTable = _fileService.ReadOriginalFile(filePath);

            var sdtmRowDescriptor = SdtmRowDescriptor.GetSdtmRowDescriptor(dataset);
            var SDTM = new List<SdtmRow>();
            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    var sdtmRow = SDTMreader.readSDTMrow(row, dataTable, sdtmRowDescriptor);

                    sdtmRow.Id = Guid.NewGuid();
                    sdtmRow.DatasetId = datasetId;
                    sdtmRow.ActivityId = dataset.ActivityId;
                    sdtmRow.DatafileId = fileId;
                    sdtmRow.ProjectId = dataset.Activity.ProjectId;
                    sdtmRow.ProjectAccession = dataset.Activity.Project.Accession;

                    SDTM.Add(sdtmRow);
                }
                _sdtmRepository.InsertMany(SDTM);
                Debug.WriteLine(dataTable.Rows.Count + " RECORD(s) SUCCESSFULLY ADDED FOR DATASET:" + datasetId + " ,DATAFILE:" + fileId);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
            

            dataFile.State = "SAVED";
            _dataFileRepository.Update(dataFile);

            _dataServiceUnit.Save();
            return true;
        }

        public async Task<bool> LoadDataset(int datasetId, int fileId)
        {
            //Dataset dataset = _datasetRepository
            //    .FindSingle(ds => ds.Id.Equals(datasetId),
            //     new List<string>(){
            //            "Domain", 
            //            "Activity.Project.Studies",
            //            "Variables.VariableDefinition"
            //    });

            Dataset dataset = GetActivityDataset(datasetId);
            var reload = false;
            var loaded = false;

            var dataFile = _dataFileRepository.Get(fileId);
            if (dataFile.State != "New")
                reload = true;


            //TODO: if failed to load delete file from mongodb
            //Todo: exception handling if failed 

            var sdtmRowDescriptor = SdtmRowDescriptor.GetSdtmRowDescriptor(dataset);
            List<SdtmRow> sdtmData = await _sdtmRepository.FindAllAsync(
                    dm => dm.DatasetId.Equals(datasetId) && dm.DatafileId.Equals(fileId));

           
            
            try
            {
                if (dataset.Domain.Code.Equals("DM"))
                {
                    var subjectService = new SubjectService(_dataServiceUnit);
                    loaded = subjectService.LoadSubjects(sdtmData, sdtmRowDescriptor);
                }
                else if (dataset.Domain.Code.Equals("BS"))
                {
                    var sampleService = new BioSampleService(_dataServiceUnit);
                    loaded = sampleService.LoadBioSamples(sdtmData, datasetId);
                }
                else if (dataset.Domain.Code.Equals("CY") || dataset.Domain.Code.Equals("HD"))
                {
                    var hdDataService = new HDdataService(_dataServiceUnit);
                    //loaded = await hdDataService.LoadHDdata(sdtmData, datasetId);
                }
                //else if (dataset.Domain.Code.Equals("SC"))
                //{

                //}
                else
                {
                    var observationService = new ObservationService(this._dataServiceUnit);
                    loaded = observationService.LoadObservations(sdtmData, sdtmRowDescriptor,reload);
                }


                if (loaded)
                {
                    dataFile.State = "LOADED";
                    _dataFileRepository.Update(dataFile);
                    if (!dataset.DataFiles.Select(d=>d.Datafile).Any(d => d.Id.Equals(fileId)))
                    {
                        //Adding a new datafile to this dataset
                        //var datafile = _dataFileRepository.Get(fileId);
                        //dataset.DataFiles.Add(datafile);
                        dataset.DataFiles.Add(new DatasetDatafile() { DatasetId = datasetId, DatafileId = dataFile.Id });
                        _datasetRepository.Update(dataset);
                    }
                    _dataServiceUnit.Save();
                }

               

                // if(!loaded)
                //   removeFile()

            }
            catch(Exception e)
            {
                Debug.WriteLine("Failed to load descriptors to SQL database", e.Message);

            }

            return loaded;
        }

        public void UnloadDataset(int datasetId, int fileId)
        {
            _sdtmRepository.DeleteMany(s => s.DatafileId == fileId && s.DatasetId == datasetId);
            Debug.WriteLine("RECORD(s) SUCCESSFULLY DELETED FOR DATASET:" + datasetId + " ,DATAFILE:" + fileId);
            _observationRepository.DeleteMany(o => o.DatasetId == datasetId && o.DatafileId == fileId);
            _dataServiceUnit.Save();
        }

        public async Task GenerateComputeVars(int datasetId)
        {
            var dataset = GetActivityDataset(datasetId);
            var compVarList =
                dataset.Variables.Where(v => (v.VariableDefinition.IsComputed.HasValue && v.VariableDefinition.IsComputed.Value)).Select(v=>v.VariableDefinition).ToList();

            //GET THE DATASET DATA
            List<SdtmRow> observationData = await _sdtmRepository.FindAllAsync(
                  d => d.DatasetId == datasetId && d.ProjectAccession.Equals(dataset.Activity.Project.Accession));

            
           //1- Do ROW BASED VARIABLES 
            foreach (var sdtmrow in observationData)
            {
                foreach (var variable in compVarList)
                {
                    //TODO: NEED TO IDENTIFY TYPE OF COMPUTED VARIABLE (AGGREGATE vs ROWBASED)
                    if (
                        !(variable.ComputedVarExpression.StartsWith("SUM") ||
                          variable.ComputedVarExpression.StartsWith("MAX")
                          || variable.ComputedVarExpression.StartsWith("MIN") ||
                          variable.ComputedVarExpression.StartsWith("AVG")))
                    {
                        //ROW FUNCTION
                        if (sdtmrow.Qualifiers.ContainsKey(variable.Name))
                            sdtmrow.Qualifiers[variable.Name] = ComputeValue(sdtmrow, variable);
                        else
                            sdtmrow.Qualifiers.Add(variable.Name, ComputeValue(sdtmrow, variable));
                    }
                   
                    
                }
                _sdtmRepository.Update(sdtmrow);
            }

            //2- DO AGGREGATE VARIABLES 
            foreach (var variable in compVarList)
            {
                //TODO: NEED TO IDENTIFY TYPE OF COMPUTED VARIABLE (AGGREGATE vs ROWBASED)
                if (
                    variable.ComputedVarExpression.StartsWith("SUM") ||
                    variable.ComputedVarExpression.StartsWith("MAX")
                    || variable.ComputedVarExpression.StartsWith("MIN") ||
                    variable.ComputedVarExpression.StartsWith("AVG"))
                {
                    //AGGREGATE FUNCTION
                    var aggVarName = variable.Name;
                    var expressionList = variable.ComputedVarExpression.Split(',');
                    var sourcVarAcc = expressionList[1];
                    var sourceVar = _variableDefinitionRepository.FindSingle(
                        v => v.Accession == sourcVarAcc && v.ProjectId == variable.ProjectId);
                    var sourceVarName = sourceVar.Name;
                    var func = expressionList[0];
                    var groupVar = expressionList[3];
                    //var groupProp = 

                    //PropertyInfo prop = typeof(SdtmRow)
                    //              .GetProperties()
                    //              .First(x => x.Name == groupVar);
                    //TEMP
                    //hardcoded to group by subject Id
                    //////////////////
                    var rowsBySubjId = observationData.GroupBy(o => o.USubjId);
                    foreach (var group in rowsBySubjId)
                    {
                        var subjId = group.Key;
                       
                        var aggValue = ComputeAggregate(group, func, sourceVarName);
                        observationData.FindAll(s=>s.USubjId == subjId).ForEach(s=>s.Qualifiers[variable.Name] = aggValue.ToString());

                    }
                }
            }
            observationData.ForEach(s=>_sdtmRepository.Update(s));
            
            //_dataServiceUnit.Save();

        }

        private double ComputeAggregate(IGrouping<string, SdtmRow> rows, string func, string aggVar)
        {
            double n;
            switch (func)
            {
                case "AVG":
                    return rows.Average(i => double.TryParse(i.Qualifiers[aggVar], out n) ? n : 0);
                case "MAX":
                    return rows.Max(i => double.TryParse(i.Qualifiers[aggVar], out n) ? n : 0);
                case "MIN":
                    return rows.Min(i => double.TryParse(i.Qualifiers[aggVar], out n) ? n : 0);
                case "SUM":
                    return rows.Sum(i => double.TryParse(i.Qualifiers[aggVar],out n)? n : 0);
                default:
                    return 0;
            }
        }

        private string ComputeValue(SdtmRow obs,VariableDefinition var)
        {
            //NO VALIDATIONS AND NO CHECKING WATSOEVA!
            var expressionList = var.ComputedVarExpression.Split(',');
            if(expressionList[0] == "CASE")
            {
                var sourceVarAcc = expressionList[1];
                var varDef = _variableDefinitionRepository.FindSingle(v => v.Accession == sourceVarAcc && v.ProjectId == var.ProjectId);
                var oriVal = obs.Qualifiers[varDef.Name];

                var mappings = new Dictionary<string, string>();
                var i = 2;
                //IT's assumed that the expression has already been validated
                while (i< expressionList.Length-1)
                {
                    mappings.Add(expressionList[i+1], expressionList[i + 3]);
                    i += 4;
                }
                
                string mappedVal=null;
                if (mappings.TryGetValue(oriVal, out mappedVal))
                    return mappedVal;
            }

            //VERY TEMP ...need to define variable type (rpw vs qggregate
            else if (expressionList.Contains("-") || expressionList.Contains("+") || expressionList.Contains("\\") || expressionList.Contains("*"))
            {
                var left = expressionList[0];
                var right = expressionList[2];
                var leftOperandVar = _variableDefinitionRepository.FindSingle(
                        v => v.Accession == left && v.ProjectId == var.ProjectId); 
                var rightOperandVar = _variableDefinitionRepository.FindSingle(
                        v => v.Accession == right && v.ProjectId == var.ProjectId);
                var op =expressionList[1];

                //FOR DATES
                if (leftOperandVar.Name.EndsWith("ENDTC")   && rightOperandVar.Name.EndsWith("STDTC") && op == "-")
                {
                    TimeSpan ?res = ((AbsoluteTimePoint) obs.DateTimeInterval?.End)?.DateTime -
                              ((AbsoluteTimePoint) obs.DateTimeInterval?.Start)?.DateTime;
                    return res != null ? Math.Round(((TimeSpan)res).TotalDays).ToString() : "";
                }

                var leftOperandValStr = obs.Qualifiers[leftOperandVar?.Name];
                var rightOperandValStr = obs.Qualifiers[rightOperandVar?.Name];

                int leftOperandVal, rightOperandVal;

                if (int.TryParse(leftOperandValStr,out leftOperandVal) && int.TryParse(rightOperandValStr, out rightOperandVal))
                    return Evaluate(leftOperandVal, rightOperandVal, op).ToString();
            }
            else if (expressionList[0] == "SUM")
            {
                

            }
            else if (expressionList[0] == "MAX")
            {
                
            }

            //if case
            //then parse the expression 
            //get the source variable
            //for each when then create a dictionary where the key is the when and the value is the then
            //retrieve the value of the source variable from the qualifiers of subjobservation, compare it to dictionary key, if found add to qualifiers new var with value from dictionary value
            return "";
        }

        private double Evaluate(double a, double b, string op)
        {
            switch (op)
            {
                case "+":
                    return a + b;
                case "-":
                    return a - b;
                case "*":
                    return a * b;
                case "/":
                    return a / b;
                case "^":
                    return Math.Pow(a, b);
                default:
                    return 0;
            }
        }

        private static readonly Expression<Func<DomainTemplate, DatasetDTO>> AsDatasetDto =
            x => new DatasetDTO
            {
                Name = x.Name,
                Class = x.Class,
                Description = x.Description,
                DomainId = x.Id,
                Structure = x.Structure,
                Code = x.Code,

            };

        public FileDTO CheckFileTemplateMatch(int datasetId, int fileId)
        {
            var dataset = GetActivityDataset(datasetId);
            //var dataFile = dataset.DataFiles.SingleOrDefault(df => df.Id.Equals(fileId));

            var dataFile = _dataFileRepository.Get(fileId);
            var studyId = dataset.Activity.ProjectId;
            var filePath = dataFile.Path + "\\" + dataFile.FileName;
            //var fileService = new FileService(_dataServiceUnit);
            var colHeaders = _fileService.getFileColHeaders(filePath);
            
            FileDTO fileDto = new FileDTO();
            fileDto.FileName = dataFile.FileName;
            fileDto.columnHeaders = colHeaders;
            fileDto.DataFileId = dataFile.Id;
            
            var varNames = dataset.Variables.Select(v => v.VariableDefinition.Name).ToList();
            var headers = colHeaders.Select(d => d["colName"]).ToList<string>();

            fileDto.templateMatched = headers.All(header => varNames.Contains(header));
            if (!fileDto.templateMatched)
            {
                var mappedVars = headers.FindAll(h => varNames.Contains(h));
                if (mappedVars.Count() > 0)
                {
                    float p = ((float)mappedVars.Count / headers.Count)*100;
                    if(p >= 50)
                    {
                        fileDto.unmappedCols = headers.FindAll(h => !varNames.Contains(h));
                        fileDto.percentMatched = (int)p;
                    }
                    
                }
                    
            }
            if (fileDto.templateMatched)
            {
                //dataset.StandardDataFile = filePath;
                //TODO: TEMP usage of dataset state
                //dataset.State = "standard";
                fileDto.percentMatched = 100;
                fileDto.IsStandard = true;
                dataFile.IsStandard = true;
                _dataFileRepository.Update(dataFile);
                _dataServiceUnit.Save();
            }
                
            return fileDto;
        }

        
    }

}
