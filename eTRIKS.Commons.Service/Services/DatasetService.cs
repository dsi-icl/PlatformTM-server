using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.DataAccess.MongoDB;
using eTRIKS.Commons.Service.DTOs;
using System.Data;

namespace eTRIKS.Commons.Service.Services
{
    public class DatasetService
    {
        private IServiceUoW _dataServiceUnit;
        private IRepository<Dataset, int> _datasetRepository;
        private IRepository<DomainTemplate, string> _domainRepository;
        private IRepository<Activity, string> _activityRepository;
        private IRepository<VariableDefinition, int> _variableDefinitionRepository;

        public DatasetService(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            //_templateRepository = uoW.GetRepository<DomainTemplate, string>();
            //_templateVariableRepository = uoW.GetRepository<DomainTemplateVariable,string>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _domainRepository = uoW.GetRepository<DomainTemplate, string>();
            _variableDefinitionRepository = uoW.GetRepository<VariableDefinition, int>();
        }

        /// <summary>
        /// Retrieves Domain dataset from Template Tables for "New" datasets
        /// </summary>
        /// <param name="domainId"></param>
        public DatasetDTO GetTemplateDataset(string domainId)
        {
            DomainTemplate domainTemplate = _domainRepository.FindAll(
                d => d.Id.Equals(domainId),
                new List<Expression<Func<DomainTemplate, object>>>()
                {
                    d => d.Variables.Select(c => c.controlledTerminology.Xref.DB)
                }
                ).FirstOrDefault<DomainTemplate>();


            //TODO: USE AutoMapper instead of this manual mapping

            DatasetDTO dto = new DatasetDTO();

            dto.Class = domainTemplate.Class;
            dto.Description = domainTemplate.Description;
            dto.Name = domainTemplate.Name;
            dto.DomainId = domainTemplate.Id;
            dto.Structure = domainTemplate.Structure;

            foreach (DomainVariableTemplate vt in domainTemplate.Variables.OrderBy(c => c.Order))
            {
                DatasetVariableDTO dv = new DatasetVariableDTO();
                dv.Name = vt.Name;
                dv.Description = vt.Description;
                dv.Label = vt.Label;
                dv.Accession = vt.Id;
                dv.RoleId = vt.RoleId;
                dv.DataType = vt.DataType;
                dv.UsageId = vt.UsageId;
                if (vt.controlledTerminology != null)
                {
                    dv.DictionaryName = vt.controlledTerminology.Name;
                    dv.DictionaryDefinition = vt.controlledTerminology.Definition;
                    dv.DictionaryXrefURL = vt.controlledTerminology.Xref.DB.UrlPrefix +
                                           vt.controlledTerminology.Xref.Accession;
                }
                dv.IsRequired = null;
                dv.KeySequence = null;
                dv.OrderNumber = null;
                dv.IsCurated = true;

                if (dv.UsageId.Equals("CL-Compliance-T-1") || dv.UsageId.Equals("CL-Compliance-T-2"))
                {
                    dv.isSelected = true;
                    dv.IsRequired = true;
                }

                dto.variables.Add(dv);
            }

            return dto;
        }

        public List<DatasetDTO> GetAllDomainTemplates()
        {
            //leaky abstraction
            //TODO: create a method in Generic Repository that takes an expression as a parameter
            // and uses it in a select method.
            return _domainRepository.GetAll().Select(AsDatasetDto).ToList();
        }

        /// <summary>
        /// Retrieves dataset for selected activity including Variable_Defs
        /// </summary>
        /// <param name="datasetId"></param>
        private Dataset GetActivityDataset(int datasetId)
        {
            Dataset ds = _datasetRepository.FindSingle(
                d => d.Id.Equals(datasetId),
                new List<Expression<Func<Dataset, object>>>()
                {
                    d => d.Variables.Select(t => t.VariableDefinition),
                    d => d.Activity
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
                new List<Expression<Func<Dataset, object>>>()
                {
                    d => d.Variables.Select(t => t.VariableDefinition),
                    d => d.Domain.Variables.Select(t => t.controlledTerminology.Xref.DB),
                    d => d.Activity
                });

            dto.Id = ds.Id; //Set DatasetDTO id to Dataset.Id (int)
            dto.Class = ds.Domain.Class;
            dto.Description = ds.Domain.Description;
            dto.Name = ds.Domain.Name;
            dto.DomainId = ds.Domain.Id;
            dto.Structure = ds.Domain.Structure;
            dto.Code = ds.Domain.Code;
            dto.StudyId = ds.Activity.StudyId;

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
                dto.variables.Add(dv);
            }
            foreach (VariableReference vr in ds.Variables)
            {
                DatasetVariableDTO dv;
                dv = dto.variables.SingleOrDefault(v => v.Accession.Equals(vr.VariableDefinition.Accession));
                if (dv != null)
                {
                    dv.IsRequired = vr.IsRequired;
                    dv.KeySequence = vr.KeySequence;
                    dv.OrderNumber = vr.OrderNumber;
                    dv.Id = vr.VariableDefinitionId;
                    dv.isSelected = true;
                }
            }

            return dto;
        }

        public Dataset addDataset(DatasetDTO datasetDTO)
        {
            //1. Fields for Dataset
            Dataset dataset = new Dataset();
            dataset.ActivityId = datasetDTO.ActivityId;
            dataset.DomainId = datasetDTO.DomainId;

            // Get any exisiting variable definitions for that study
            List<VariableDefinition> variableDefsOfStudy = getVariableDefinitionsOfStudy(datasetDTO.StudyId).ToList();

            foreach (var VariableDTO in datasetDTO.variables)
            {
                if (VariableDTO.isSelected)
                {
                    ;
                    //Compare newly added Variable to previously added VarDefs using accession string 
                    //since no Id for Variable created yet
                    VariableDefinition varDef = variableDefsOfStudy.SingleOrDefault(
                        d => d.Accession.Equals(VariableDTO.Accession));
                    if (varDef == null)
                    {
                        varDef = new VariableDefinition();
                        varDef.Accession = VariableDTO.Accession;
                        varDef.Name = VariableDTO.Name;
                        varDef.Label = VariableDTO.Label;
                        varDef.Description = VariableDTO.Description;
                        varDef.DataType = VariableDTO.DataType;
                        varDef.IsCurated = VariableDTO.IsCurated;
                        varDef.RoleId = VariableDTO.RoleId;
                        varDef.StudyId = datasetDTO.StudyId;
                    }
                    //3. Fields for varRefList
                    VariableReference varRef = new VariableReference();
                    varRef.OrderNumber = VariableDTO.OrderNumber;
                    varRef.IsRequired = VariableDTO.IsRequired;
                    varRef.KeySequence = VariableDTO.KeySequence;
                    varRef.VariableDefinition = varDef;
                    dataset.Variables.Add(varRef);
                }
            }

            _datasetRepository.Insert(dataset);
            if (_dataServiceUnit.Save().Equals("CREATED"))
            {
                return dataset;
            }
            return null;
        }

        public string updateDataset(DatasetDTO datasetDTO, string datasetId)
        {
            Dataset datasetToUpdate = GetActivityDataset(datasetDTO.Id);

            //TODO: Do this using some DTOmapping library
            datasetToUpdate.DataFile = datasetDTO.DataFile;
            datasetToUpdate.StandardDataFile = datasetDTO.StandardDataFile;
            datasetToUpdate.State = datasetDTO.State;
            
            List<VariableDefinition> variableDefsOfStudy = getVariableDefinitionsOfStudy(datasetDTO.StudyId).ToList();

            var datasetVarsToUpdate = new HashSet<string>(
                datasetToUpdate.Variables.Select(c => c.VariableDefinition.Accession));

            foreach (var VariableDTO in datasetDTO.variables)
            {
                if (VariableDTO.isSelected)
                {
                    //switching to VariableDTO.Accession since for newly selected variables Id is null
                    if (!datasetVarsToUpdate.Contains(VariableDTO.Accession))
                    {
                        //add VarDEF and addVarREF
                        VariableDefinition varDef;
                        varDef = variableDefsOfStudy.SingleOrDefault(d => d.Accession.Equals(VariableDTO.Accession));
                        if (varDef == null)
                        {
                            varDef = new VariableDefinition();
                            varDef.Accession = VariableDTO.Accession;
                            varDef.Name = VariableDTO.Name;
                            varDef.Label = VariableDTO.Label;
                            varDef.Description = VariableDTO.Description;
                            varDef.DataType = VariableDTO.DataType;
                            varDef.IsCurated = VariableDTO.IsCurated;
                            varDef.RoleId = VariableDTO.RoleId;
                            varDef.StudyId = datasetDTO.StudyId;
                        }

                        VariableReference varRef = new VariableReference();
                        varRef.OrderNumber = VariableDTO.OrderNumber;
                        varRef.IsRequired = VariableDTO.IsRequired;
                        varRef.KeySequence = VariableDTO.KeySequence;
                        varRef.VariableDefinition = varDef;
                        datasetToUpdate.Variables.Add(varRef);
                    }
                }
                else if (datasetVarsToUpdate.Contains(VariableDTO.Accession))
                {
                    //remove variable from dataset
                    VariableReference var =
                        datasetToUpdate.Variables.Single(v => v.VariableDefinitionId.Equals(VariableDTO.Id));
                    datasetToUpdate.Variables.Remove(var);
                }
            }

            _datasetRepository.Update(datasetToUpdate);

            return _dataServiceUnit.Save();
        }

        public IEnumerable<VariableDefinition> getVariableDefinitionsOfStudy(string studyId)
        {
            return _variableDefinitionRepository.FindAll(d => d.StudyId.Equals(studyId));
        }

        public DataTemplateMap GeTemplateMaps(int datasetId)
        {
            var map = new DataTemplateMap();

            var ds = _datasetRepository.FindSingle(
                d => d.Id.Equals(datasetId),
                new List<Expression<Func<Dataset, object>>>()
                {
                    d => d.Variables.Select(t => t.VariableDefinition.Role),
                    d => d.Domain,
                    d => d.Activity
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


        public Hashtable getDatasetPreview(int datasetId)
        {
            var dataset = GetActivityDataset(datasetId);
            //This will change to retrieve FileObject
            var fileName = dataset.StandardDataFile;
            var studyId = dataset.Activity.StudyId;

            var fileService = new FileService();
            //TEMP usage of dataset.state
            var dataTable = dataset.State.Equals("standard") ? fileService.ReadOriginalFile(studyId, fileName) : fileService.readStandardFile(studyId, fileName);
            var ht = getHashtable(dataTable);
            return ht;
        }

        public bool mapToTemplate(int datasetId, DataTemplateMap map)
        {
            var dataset = GetActivityDataset(datasetId);
            var fileName = dataset.DataFile;
            var studyId = dataset.Activity.StudyId;

            FileService fileService = new FileService();
            DataTable inputDataTable = fileService.ReadOriginalFile(studyId,fileName);
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

            if (sdtmTable.Rows.Count != 0)
            {
                string dsName = studyId + "_" + dataset.DomainId;
                sdtmTable.TableName = dsName;
                //Write new transformed to file 
                string path = fileService.writeDataFile(studyId,sdtmTable);
                
                //Update dataset
                dataset.StandardDataFile = dsName+".csv";
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
            return true;
        }

        private Hashtable getHashtable(DataTable sdtmTable)
        {
            var ht = new Hashtable();
            var headerList = new List<Dictionary<string, string>>();
            foreach (var col in sdtmTable.Columns.Cast<DataColumn>())
            {
                var header = new Dictionary<string, string>
                {
                    {"data", col.ColumnName.ToLower()},
                    {"title", col.ColumnName}
                };
                headerList.Add(header);
            }
            ht.Add("header", headerList);
            ht.Add("data", sdtmTable);
            return ht;
        }

        public string loadDataset(int datasetId)
        {
            var dataset = GetActivityDataset(datasetId);
            var fileName = dataset.StandardDataFile;
            var studyId = dataset.Activity.StudyId;

            var fileService = new FileService();
            var dataTable = dataset.State.Equals("standard") ? fileService.ReadOriginalFile(studyId, fileName) : fileService.readStandardFile(studyId, fileName);

            //var dataTable = fileService.readStandardFile(studyId,fileName);

            var ms = new MongoDbDataRepository();

            int count=0,i = 0;
            var records = new List<MongoDocument>();
            //var columns = dataTable.Columns.Cast<String>();
            foreach(DataRow row in dataTable.Rows)
            {
               var record = new MongoDocument();
                i++;
                foreach(DataColumn column in dataTable.Columns)
                {
                   var recordItem = new MongoField {Name = column.ColumnName, value = row[column].ToString()};
                    //TODO: consider putting this back
                    //if (recordItem.value != "")
                        record.fields.Add(recordItem);
                }
                records.Add(record);

                if (i%500 == 0)
                {
                    ms.loadDataGeneric(records);
                    records.Clear();
                    Debug.WriteLine(i + " RECORD(s) SUCCESSFULLY INSERTED");
                }
                count = i + 1;
            }
            ms.loadDataGeneric(records);

            Debug.WriteLine(count + " RECORD(s) SUCCESSFULLY INSERTED");

            const string status = "CREATED";
            return status;
        }

        public async Task<bool> loadObservations(int datasetId)
        {
            Dataset dataset = _datasetRepository
                .FindSingle(ds => ds.Id.Equals(datasetId),
                 new List<Expression<Func<Dataset, object>>>(){
                        d => d.Domain, 
                        d=> d.Variables,
                        d => d.Activity,
                        d => d.Activity.Study,
                        d=>d.Variables.Select(k=>k.VariableDefinition)
                });

            ObservationService observationService = new ObservationService(this._dataServiceUnit);
            if(dataset.Domain.Code.Equals("DM"))
                observationService.loadSubjectCharacteristics(dataset);
            else
                await observationService.loadDatasetObservations(dataset);
            const string status = "LOADED";
            return true;
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

        public List<Dictionary<string, string>> getFileColHeaders(int datasetId)
        {
            var dataset = GetActivityDataset(datasetId);
            var fileName = dataset.DataFile;
            var studyId = dataset.Activity.StudyId;

            var fileService = new FileService();
            return fileService.getFileColHeaders(studyId, fileName);
        }

        //public bool checkTemplateMatch(int datasetId)
        //{
        //    var dataset = GetActivityDataset(datasetId);
        //    var fileName = dataset.DataFile;
        //    var studyId = dataset.Activity.StudyId;

        //    var colHeaders = getFileColHeaders(datasetId);
        //    var headers = colHeaders.Select(d => d["colName"]).ToList<string>();
        //    var varNames = dataset.Variables.Select(v => v.VariableDefinition.Name).ToList();

        //    if (headers.All(header => varNames.Contains(header)))
        //        return true;
        //    else
        //        return false;
            

        //    //TODO: getColumnheaders and get dataset vardefs if both are equivalent call preview and skip to step
        //    //TODO: getColmn headers and get dataset if headers are not 
        //}

        public FileDTO getDatasetFileInfo(int datasetId)
        {
            var dataset = GetActivityDataset(datasetId);
            var fileName = dataset.DataFile;
            var studyId = dataset.Activity.StudyId;

            var fileService = new FileService();
            var colHeaders = fileService.getFileColHeaders(studyId, fileName);
            
            FileDTO fileDto = new FileDTO();
            fileDto.FileName = dataset.DataFile;
            fileDto.columnHeaders = colHeaders;
            
            var varNames = dataset.Variables.Select(v => v.VariableDefinition.Name).ToList();
            var headers = colHeaders.Select(d => d["colName"]).ToList<string>();

            fileDto.templateMatched = headers.All(header => varNames.Contains(header));
            if (fileDto.templateMatched)
            {
               dataset.StandardDataFile = fileName;
               //TODO: TEMP usage of dataset state
                dataset.State = "standard";
            }
                
            return fileDto;
        }
    }

}
