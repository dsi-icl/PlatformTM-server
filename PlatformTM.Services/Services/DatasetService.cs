using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.Core.Domain.Model.Timing;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services.Loading.AssayData;
using PlatformTM.Models.Services.Loading.SDTM;

namespace PlatformTM.Models.Services
{
    public class DatasetService
    {
        private readonly IServiceUoW _dataServiceUnit;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<VariableDefinition, int> _variableDefinitionRepository;
        private readonly IRepository<DataFile, int> _dataFileRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;

        private readonly IRepository<Observation, int> _observationRepository;
        private readonly CacheService _cacheService;
        

        public DatasetService(IServiceUoW uoW, CacheService cacheService)
        {
            _dataServiceUnit = uoW;
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _dataFileRepository = uoW.GetRepository<DataFile, int>();
            _observationRepository = uoW.GetRepository<Observation, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();
            _variableDefinitionRepository = uoW.GetRepository<VariableDefinition, int>();
            _cacheService = cacheService;
        }

       
       

        public async Task<bool> LoadDataset(int datasetId, int fileId)
        {
            Dataset dataset = GetDatasetDescriptor(datasetId);
            var reload = false;
            var loaded = false;

            var dataFile = _dataFileRepository.Get(fileId);
            if (dataFile.IsLoadedToDB.Value)
                reload = true;

            try
            {
                switch (dataset.Template.Code)
                {
                    case "DM":
                        var subjectLoader = new SubjectLoader(_dataServiceUnit);
                        loaded = await subjectLoader.LoadSubjects(dataset, fileId, reload);
                        break;
                    case "BS":
                        var sampleLoader = new BioSampleLoader(_dataServiceUnit);
                        loaded = await sampleLoader.LoadBioSamples(dataset, fileId, reload);
                        break;
                    case "CY":
                    case "HD":
                        var hdDataLoader = new HDloader(_dataServiceUnit);
                        //loaded = await hdDataLoader.LoadHDdata(dataset,fileId, datasetId);
                        break;
                    case "MX":
                        break;
                    default:
                        var observationLoader = new ObservationLoader(this._dataServiceUnit);
                        loaded = await observationLoader.LoadObservations(dataset, fileId, reload);
                        if (loaded)
                        {
                            await _cacheService.GenerateAndCacheClinicalDTO(dataFile.ProjectId);
                        }
                        break;
                }


                if (loaded)
                {
                    dataFile.State = "LOADED";
                    dataFile.IsLoadedToDB = true;
                    _dataFileRepository.Update(dataFile);
                    if(!dataset.DataFiles.Select(d=>d.DatafileId).Contains(fileId))
                    {
                        //Adding a new datafile to this dataset
                        dataset.DataFiles.Add(new DatasetDatafile() { DatasetId = datasetId, DatafileId = dataFile.Id });
                        _datasetRepository.Update(dataset);
                    }
                    _dataServiceUnit.Save();
                }

                 if(!loaded && dataFile.State == "SAVED")
                    _sdtmRepository.DeleteMany(s => s.DatafileId == fileId && s.DatasetId == datasetId);

            }

            catch (Exception e)
            {
                Debug.WriteLine("Failed to load descriptors to SQL database", e.Message);
                await UnloadDataset(datasetId,fileId,"LOADING FAILED");

            }

            return loaded;
        }

        public async Task<bool> UnloadDataset(int datasetId, int fileId, string status="UNLOADED")
        {
            try
            {
                var dataset = _datasetRepository.FindSingle(ds=>ds.Id == datasetId, new List<string>() {"Template","Activity"});
                switch (dataset.Template.Code)
                {
                    case "DM":
                        var subjectLoader = new SubjectLoader(_dataServiceUnit);
                        subjectLoader.UnloadSubjects(datasetId, fileId);
                        break;
                    case "BS":
                        var sampleLoader = new BioSampleLoader(_dataServiceUnit);
                        sampleLoader.UnloadBioSamples(datasetId, fileId);
                        break;
                    default:
                        var observationLoader = new ObservationLoader(_dataServiceUnit);
                        observationLoader.UnloadObservations(datasetId, fileId);
                        await _cacheService.GenerateAndCacheClinicalDTO(dataset.Activity.ProjectId);
                        break;
                }
            
                Debug.WriteLine("RECORD(s) SUCCESSFULLY DELETED FOR DATAFILE:" + fileId);
            }

            // in case an error hapens it returns false for success and therefore the main file would not be deleted. (try method)
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }

            //SINCE UNLOAD FILE CAN BE CALLED INDEPENDENTLY OF REMOVING FILE, NEED TO SET STATUS
            var file = _dataFileRepository.Get(fileId);
            file.State = status;
            _dataFileRepository.Update(file);
            _dataServiceUnit.Save();
            return true;
        }

        public async Task<bool> UnloadFileDatasets(int fileId)
        {
            var file = _dataFileRepository.FindSingle(f => f.Id == fileId, new List<string>() {"Datasets"});
            bool result = true;
            //foreach (var dataset in file.)
                result = result && await UnloadDataset(file.DatasetId.Value, fileId);
            return result;
        }

        public async Task<DataTable> ConsolidateDataset(int datasetId){
            var sdtmRows = _sdtmRepository.FindAll(s => s.DatasetId == datasetId).OrderBy(s=>s.StudyId);

            var descriptor = GetDatasetDescriptor(datasetId);
            var sdtmRowDescriptor = SdtmRowDescriptor.GetSdtmRowDescriptor(descriptor);
            var dataTable = new DataTable();
            if (descriptor.Activity.GetType().Equals(typeof(Assay)))
                dataTable.TableName = descriptor.Activity.Name + "-" + descriptor.Template.Domain;
            else
                dataTable.TableName = descriptor.Template.Domain;
			
            await Task.Run(() => {
				foreach (var field in descriptor.Variables.OrderBy(vr => vr.OrderNumber).Select(v => v.VariableDefinition))
				{
					dataTable.Columns.Add(field.Name, typeof(string));
				}
                try
                {
					foreach (var sdtmrow in sdtmRows)
					{
						if (sdtmrow.DomainCode == null) continue;
						var row = SDTMreader.WriteSDTMrow(sdtmrow, dataTable, sdtmRowDescriptor);
						dataTable.Rows.Add(row);
					}
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            });

            return dataTable;
        }

        public async Task GenerateComputeVars(int datasetId)
        {
            var dataset = GetDatasetDescriptor(datasetId);
            var compVarList =
                dataset.Variables.Where(v => (v.VariableDefinition.IsComputed.HasValue && v.VariableDefinition.IsComputed.Value)).Select(v=>v.VariableDefinition).ToList();

            //GET THE DATASET DATA
            List<SdtmRow> observationData = await _sdtmRepository.FindAllAsync(
                d => d.DatasetId == datasetId && d.ProjectAccession.Equals(dataset.Activity.Project.Accession),new List<string>(){});

            
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

        /// <summary>
        /// Retrieves dataset for selected activity including Variable_Defs
        /// </summary>
        /// <param name="datasetId"></param>
        private Dataset GetDatasetDescriptor(int datasetId)
        {
            Dataset ds = _datasetRepository.FindSingle(
                d => d.Id.Equals(datasetId),
                new List<string>()
                {
                    "Variables.VariableDefinition",
                    "DataFiles",
                    "Activity.Project",
                    "Template"
                });
            return ds;
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

        private static readonly Expression<Func<DatasetTemplate, DatasetDTO>> AsDatasetDto =
            x => new DatasetDTO
            {
                Name = x.Domain,
                Class = x.Class,
                Description = x.Description,
                DomainId = x.Id,
                Structure = x.Structure,
                Code = x.Code,

            };

        
    }

}
