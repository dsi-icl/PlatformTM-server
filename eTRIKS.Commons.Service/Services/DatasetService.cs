using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Permissions;
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
        private readonly IServiceUoW _dataServiceUnit;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<DomainTemplate, string> _domainRepository;
        private readonly IRepository<Activity, int> _activityRepository;
        private readonly IRepository<VariableDefinition, int> _variableDefinitionRepository;
        private readonly IRepository<DataFile, int> _dataFileRepository;
        private readonly IRepository<HumanSubject, string> _humanSubjectRepository;
        private readonly IRepository<Biosample, int> _bioSampleRepository;
        private readonly IRepository<SdtmEntity, Guid> _sdtmRepository;
        private readonly IRepository<MongoDocument, Guid> _mongoDocRepository;
       // private FileService _fileService;

        public DatasetService(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            //_templateRepository = uoW.GetRepository<DomainTemplate, string>();
            //_templateVariableRepository = uoW.GetRepository<DomainTemplateVariable,string>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _domainRepository = uoW.GetRepository<DomainTemplate, string>();
            _variableDefinitionRepository = uoW.GetRepository<VariableDefinition, int>();
            _dataFileRepository = uoW.GetRepository<DataFile, int>();
            _bioSampleRepository = uoW.GetRepository<Biosample, int>();
            _activityRepository = uoW.GetRepository<Activity, int>();
            _sdtmRepository = uoW.GetRepository<SdtmEntity, Guid>();
            _mongoDocRepository = uoW.GetRepository<MongoDocument, Guid>();
            _humanSubjectRepository = uoW.GetRepository<HumanSubject, string>();

            //_fileService = new FileService(_dataServiceUnit);
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
                    d => d.Activity,
                    d => d.DataFiles,
                    d => d.Activity.Project,
                    d => d.Domain
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
            var dataset = CreateDataset(datasetDTO);

            _datasetRepository.Insert(dataset);
            if (_dataServiceUnit.Save().Equals("CREATED"))
            {
                return dataset;
            }
            return null;
        }

        public Dataset CreateDataset(DatasetDTO datasetDTO)
        {
            var dataset = new Dataset {ActivityId = datasetDTO.ActivityId, DomainId = datasetDTO.DomainId};

            //var activity = _activityRepository.Get(dataset.ActivityId);

            // Get any exisiting variable definitions for that study
            List<VariableDefinition> variableDefsOfStudy = getVariableDefinitionsOfStudy(datasetDTO.ProjectId).ToList();

            foreach (var variableDto in datasetDTO.variables.Where(variableDto => variableDto.isSelected))
            {
                ;
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
        
        public string updateDataset(DatasetDTO datasetDTO)
        {
            Dataset datasetToUpdate = GetActivityDataset(datasetDTO.Id);

            //TODO: Do this using some DTOmapping library
            // datasetDTO woll only have one DataFile unlike Class Dataset since the DTO reflect one assignment of a datafile to a dataset
            //check the FileDTO sent back in this dataset
            //


            if (datasetDTO.DataFileDTO != null && !datasetToUpdate.DataFiles.Any(d => d.Id.Equals(datasetDTO.DataFileDTO.DataFileId)))
            {
                //Adding a new datafile to this dataset
                var datafile = _dataFileRepository.FindSingle(d=>d.Id.Equals(datasetDTO.DataFileDTO.DataFileId));
                datasetToUpdate.DataFiles.Add(datafile);
            }
           
                //update status of datafile

           // datasetToUpdate.StandardDataFile = datasetDTO.StandardDataFile;
            datasetToUpdate.State = datasetDTO.State;
            
            List<VariableDefinition> variableDefsOfStudy = getVariableDefinitionsOfStudy(datasetDTO.ProjectId).ToList();

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
                            varDef.ProjectId = datasetDTO.ProjectId;
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

        public IEnumerable<VariableDefinition> getVariableDefinitionsOfStudy(int studyId)
        {
            return _variableDefinitionRepository.FindAll(d => d.ProjectId.Equals(studyId));
        }

        public DataTemplateMap GetTemplateMaps(int datasetId)
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

        public Hashtable getDatasetPreview(int datasetId, int fileId)
        {
            var dataset = GetActivityDataset(datasetId);
            var dataFile = dataset.DataFiles.SingleOrDefault(df => df.Id.Equals(fileId));
            //var studyId = dataset.Activity.StudyId;
            var filePath = dataFile.Path + "\\" + dataFile.FileName;

            var fileService = new FileService(_dataServiceUnit);
            //TEMP usage of dataset.state
            var dataTable = fileService.ReadOriginalFile(filePath);// : fileService.readStandardFile(studyId, fileName);
            var ht = getHashtable(dataTable);
            return ht;
        }

        public int? mapToTemplate(int datasetId, int fileId, DataTemplateMap map)
        {
            var dataset = GetActivityDataset(datasetId);
            var studyId = dataset.Activity.ProjectId;
            var projectAcc = dataset.Activity.Project.Accession;
            var dataFile = dataset.DataFiles.SingleOrDefault(df => df.Id.Equals(fileId));
            var filePath = dataFile.Path + "\\" + dataFile.FileName;

            FileService fileService = new FileService(_dataServiceUnit);
            DataTable inputDataTable = fileService.ReadOriginalFile(filePath);
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
                var fileInfo = fileService.writeDataFile(dataFile.Path, sdtmTable);
                standardFile = fileService.addOrUpdateFile(projectAcc, fileInfo);
                //var file = _dataFileRepository.Get(4);
                //Update dataset
                dataset.DataFiles.Add(standardFile);//.StandardDataFile = dsName + ".csv";
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

        public string SaveDataFile(int datasetId, int fileId)
        {
            var dataset = GetActivityDataset(datasetId);
            var dataFile = dataset.DataFiles.SingleOrDefault(df => df.Id.Equals(fileId));
            
            var filePath = dataFile.Path + "\\" + dataFile.FileName;

            //if (!dataFile.State.ToLower().Equals("new"))
            //{
            //    /**
            //     * Replacing previously loaded file
            //     * Remove file from collection before reloading it
            //     */
            //    var filterFields = new List<object>();
            //    var filterField1 = new Dictionary<string, object>();
            //    filterField1.Add("DBDATASETID", datasetId);
            //    filterFields.Add(filterField1);
            //    var filterField2 = new Dictionary<string, object>();
            //    filterField2.Add("DBDATAFILEID", fileId);
            //    filterFields.Add(filterField2);
            //    _mongoDocRepository.DeleteManyAsync(filterFields);
            //    Debug.WriteLine("RECORD(s) SUCCESSFULLY DELETED FOR DATASET:" + datasetId + " ,DATAFILE:" + fileId);
            //}
            
            //var studyId = dataset.Activity.StudyId;

            var fileService = new FileService(_dataServiceUnit);
            var dataTable = fileService.ReadOriginalFile(filePath);// : fileService.readStandardFile(studyId, fileName);

            if (dataset.Domain.Code.Equals("AE"))
            {
                //dataTable.Columns.Add("AEOCCUR");
                var newColumn = new DataColumn("AEOCCUR", typeof(String));
                newColumn.DefaultValue = "Y";
                dataTable.Columns.Add(newColumn);

                //ADD all remaining subjects in  project
                var tableStudyIds = dataTable.AsEnumerable().Select(s => s.Field<string>("STUDYID")).Distinct().ToList();
                var subjects = _humanSubjectRepository
                    .FindAll(s => tableStudyIds.Contains(s.Study.Name),
                    new List<Expression<Func<HumanSubject, object>>>()
                    {
                        s=>s.Study
                    }).ToList();
                
                var tableSubjectIds = dataTable.AsEnumerable().Select(s => s.Field<string>("USUBJID")).Distinct().ToList();

                var O3s = dataTable.AsEnumerable().Select(s => s.Field<string>("AEDECOD")).Distinct().ToList();
                var remainingSubjects = subjects.FindAll(s => !tableSubjectIds.Contains(s.UniqueSubjectId));
                
                //Foreach subject add occur = N for each adverse event
                foreach (var subject in remainingSubjects)
                {
                    foreach (var o3 in O3s)
                    {
                        DataRow dr = dataTable.NewRow();
                        dr["USUBJID"] = subject.UniqueSubjectId;
                        dr["AEOCCUR"] = "N";
                        dr["AEDECOD"] = o3;
                        dr["STUDYID"] = subject.Study.Name;
                        dr["DOMAIN"] = "AE";
                        dataTable.Rows.Add(dr);
                    }
                    
                }
            }
            //return "";
            //var dataTable = fileService.readStandardFile(studyId,fileName);

            var ms = new MongoDbDataRepository();

            int count = 0, i = 0;
            var records = new List<MongoDocument>();
            //var columns = dataTable.Columns.Cast<String>();
            foreach (DataRow row in dataTable.Rows)
            {
                var record = new MongoDocument();
                i++;
                foreach (DataColumn column in dataTable.Columns)
                {
                    var recordItem = new MongoField { Name = column.ColumnName, value = row[column].ToString() };
                    //TODO: consider putting this back
                    //if (recordItem.value != "")
                    record.fields.Add(recordItem);
                }

                //ADD internal fields to link SQL recordss with mongo datasets
                var recorditem = new MongoField { Name = "DBPROJECTACC", value = dataset.Activity.Project.Accession };
                record.fields.Add(recorditem);
                recorditem = new MongoField { Name = "DBPROJECTID", value = dataset.Activity.ProjectId };
                record.fields.Add(recorditem);
                //recorditem = new MongoField { Name = "DBSTUDYID", value = dataset.Activity.Project };
                //record.fields.Add(recorditem);
                recorditem = new MongoField { Name = "DBDATASETID", value = dataset.Id };
                record.fields.Add(recorditem);
                recorditem = new MongoField { Name = "DBACTIVITYID", value = dataset.ActivityId };
                record.fields.Add(recorditem);
                recorditem = new MongoField { Name = "DBDATAFILEID", value = fileId };
                record.fields.Add(recorditem);

                
                

                records.Add(record);

                if (i % 500 == 0)
                {
                    ms.loadDataGeneric(records);
                    //_mongoDocRepository.InsertManyAsync(records);
                    records.Clear();
                    Debug.WriteLine(i + " RECORD(s) SUCCESSFULLY INSERTED");
                }
                count = i + 1;
            }


            ms.loadDataGeneric(records);
            //_mongoDocRepository.InsertManyAsync(records);

            Debug.WriteLine(count + " RECORD(s) SUCCESSFULLY INSERTED");
            dataFile.State = "LOADED";
            _dataFileRepository.Update(dataFile);
            _dataServiceUnit.Save();

            const string status = "CREATED";
            return status;
        }

        public async Task<bool> LoadDataset(int datasetId, int fileId)
        {
            Dataset dataset = _datasetRepository
                .FindSingle(ds => ds.Id.Equals(datasetId),
                 new List<Expression<Func<Dataset, object>>>(){
                        d => d.Domain, 
                        d => d.Variables,
                        d => d.Activity,
                        d => d.Activity.Project.Studies,
                        d=>d.Variables.Select(k=>k.VariableDefinition)
                });

            var sdtmEntityDescriptor = GetSdtmEntityDescriptor(dataset);
            _dataServiceUnit.setSDTMentityDescriptor(sdtmEntityDescriptor);


            List<SdtmEntity> sdtmData = await _sdtmRepository.FindAllAsync(
                    dm => dm.DatasetId.Equals(datasetId) && dm.DatafileId.Equals(fileId));

            var observationService = new ObservationService(this._dataServiceUnit);
            
            if (dataset.Domain.Code.Equals("DM"))
            {
                var subjectService = new SubjectService(_dataServiceUnit);
                await subjectService.LoadSubjects(sdtmData,datasetId);
            }
            else if (dataset.Domain.Code.Equals("BS"))
            {
                var sampleService = new BioSampleService(_dataServiceUnit);
                await sampleService.LoadBioSamples(sdtmData, datasetId);
            }
            else if (dataset.Domain.Code.Equals("CY") || dataset.Domain.Code.Equals("HD"))
            {
                var hdDataService = new HDdataService(_dataServiceUnit);
                await hdDataService.LoadHDdata(sdtmData, datasetId);
            }
                //else if (dataset.Domain.Code.Equals("SC"))
                //{
                
                //}
            else
            {
                //observationService.LoadObsDescriptors(dataset, );
                await observationService.loadDatasetObservations(dataset, fileId);
            }
                
            const string status = "LOADED";
            return true;
        }

        public void UnloadDataset(int datasetId, int fileId)
        {
            var filterFields = new List<object>();
            var filterField1 = new Dictionary<string, object>();
            filterField1.Add("DBDATASETID", datasetId);
            filterFields.Add(filterField1);
            var filterField2 = new Dictionary<string, object>();
            filterField2.Add("DBDATAFILEID", fileId);
            filterFields.Add(filterField2);
            _mongoDocRepository.DeleteMany(filterFields);
            Debug.WriteLine("RECORD(s) SUCCESSFULLY DELETED FOR DATASET:" + datasetId + " ,DATAFILE:" + fileId);


        }
        
        //public async Task<bool> loadSubjects(int datasetId)
        //{
        //    List<SdtmEntity> subjectData = await _sdtmRepository.FindAllAsync(
        //            dm => dm.DatasetId.Equals(datasetId));

        //    var subjectService = new SubjectService(_dataServiceUnit);
        //    return subjectService.LoadSubjects(subjectData);
        //}

        //public async Task<bool> loadBioSamples(int datasetId)
        //{
        //    //var dataset = _datasetRepository
        //    //    .FindSingle(ds => ds.Id.Equals(datasetId),
        //    //     new List<Expression<Func<Dataset, object>>>(){
        //    //            d => d.Domain, 
        //    //            d=> d.Variables,
        //    //            d => d.Activity,
        //    //            d => d.Activity.Study,
        //    //            d=>d.Variables.Select(k=>k.VariableDefinition)
        //    //    });

        //    List<SdtmEntity> sampleData = await _sdtmRepository.FindAllAsync(
        //            bs => bs.DatasetId.Equals(datasetId));

        //    var sampleService = new BioSampleService(_dataServiceUnit);
        //    return sampleService.LoadBioSamples(sampleData);

            

        //}


        /**
         * Each dataset will have a set of different variables describing its observations
         * SDTMEntityDescriptor uses the SDTM standard convention to identify the different types of variables 
         * available for this dataset
         */
        private SdtmEntityDescriptor GetSdtmEntityDescriptor(Dataset dataset)
        {
            var sdtmEntityDescriptor = new SdtmEntityDescriptor();

            //TODO: change all these strings to ENUMS

            sdtmEntityDescriptor.QualifierVariables = dataset.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-3")
                        .ToList();
            //sdtmEntityDescriptor.GroupDescriptors = dataset.Variables
            //            .Select(l => l.VariableDefinition)
            //            .Where(v => v.RoleId == "CL-Role-T-7")
            //            .ToList();
            sdtmEntityDescriptor.SynonymVariables = dataset.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-4")
                        .ToList();
            sdtmEntityDescriptor.VariableQualifierVariables = dataset.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-5")
                        .ToList();
            sdtmEntityDescriptor.ResultVariables = dataset.Variables
                        .Select(l => l.VariableDefinition)
                        .Where(v => v.RoleId == "CL-Role-T-8")
                        .ToList();

            //Domain Class
            sdtmEntityDescriptor.Class = dataset.Domain.Class;


            //O3Descriptor
            sdtmEntityDescriptor.O3Variable = dataset.Variables
                        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.RoleId == "CL-Role-T-2");

            //O3 CVTerm
            sdtmEntityDescriptor.O3CVterm = dataset.Variables
                        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == dataset.Domain.Code + "DECOD");

            if (dataset.Domain.Class.ToLower().Equals("findings"))
                sdtmEntityDescriptor.O3CVterm = dataset.Variables
                    .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == dataset.Domain.Code + "LOINC");

            //O3 Synonym (VSTEST)
            sdtmEntityDescriptor.O3SynoymVariable = dataset.Variables
                    .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == dataset.Domain.Code + "MODIFY");

            if (dataset.Domain.Class.ToLower().Equals("findings"))
                sdtmEntityDescriptor.O3SynoymVariable = dataset.Variables
                    .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == dataset.Domain.Code + "TEST");

            //O3 Category
            sdtmEntityDescriptor.GroupVariable = dataset.Variables
                        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == dataset.Domain.Code + "CAT");

            //O3 Subcategory
            sdtmEntityDescriptor.SubgroupVariable = dataset.Variables
                       .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == dataset.Domain.Code + "SCAT");

            //O3 DefaultQualifier
            //if (dataset.Domain.Class.ToLower().Equals("findings"))
            //    sdtmEntityDescriptor.DefaultQualifier = dataset.Variables
            //        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == dataset.Domain.Code + "ORRES");

            //if (dataset.Domain.Class.ToLower().Equals("events"))
            //    sdtmEntityDescriptor.DefaultQualifier = dataset.Variables
            //        .Select(l => l.VariableDefinition).FirstOrDefault(v => v.Name == dataset.Domain.Code + "OCCUR");



            return sdtmEntityDescriptor;
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

        //public List<Dictionary<string, string>> getFileColHeaders(int datasetId)
        //{
        //    var dataset = GetActivityDataset(datasetId);
        //    var fileName = dataset.DataFile;
        //    var studyId = dataset.Activity.StudyId;

        //    var fileService = new FileService(_dataServiceUnit);
        //    return fileService.getFileColHeaders(studyId, fileName);
        //}

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

        public FileDTO getDatasetFileInfo(int datasetId, int fileId)
        {
            var dataset = GetActivityDataset(datasetId);
            var dataFile = dataset.DataFiles.SingleOrDefault(df => df.Id.Equals(fileId));
            var studyId = dataset.Activity.ProjectId;
            var filePath = dataFile.Path + "\\" + dataFile.FileName;
            var fileService = new FileService(_dataServiceUnit);
            var colHeaders = fileService.getFileColHeaders(filePath);
            
            FileDTO fileDto = new FileDTO();
            fileDto.FileName = dataFile.FileName;
            fileDto.columnHeaders = colHeaders;
            fileDto.DataFileId = dataFile.Id;
            
            var varNames = dataset.Variables.Select(v => v.VariableDefinition.Name).ToList();
            var headers = colHeaders.Select(d => d["colName"]).ToList<string>();

            fileDto.templateMatched = headers.All(header => varNames.Contains(header));
            if (fileDto.templateMatched)
            {
                //dataset.StandardDataFile = filePath;
               //TODO: TEMP usage of dataset state
                //dataset.State = "standard";
                fileDto.IsStandard = true;
                dataFile.IsStandard = true;
                _dataFileRepository.Update(dataFile);
                _dataServiceUnit.Save();
            }
                
            return fileDto;
        }

        public List<DatasetDTO> GetAssayFeatureTemplates()
        {
            var ds = GetTemplateDataset("D-ASSAY-FEAT");
            return new List<DatasetDTO>(){ds};
        }

        public List<DatasetDTO> GetAssaySampleTemplates()
        {
            var ds =  GetTemplateDataset("D-SDTM-BS");
            return new List<DatasetDTO> {ds};
        }

        public List<DatasetDTO> GetAssayDataTemplates()
        {
            var ds =  GetTemplateDataset("D-CUST-HD");
            return new List<DatasetDTO>(){ds};
        }
    }

}
