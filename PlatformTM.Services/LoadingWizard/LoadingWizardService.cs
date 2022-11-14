using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.BMO;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;
using PlatformTM.Models.Configuration;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services;
using PlatformTM.Services.LoadingWizard.DTO;

namespace PlatformTM.Services.LoadingWizard
{
    public class LoadingWizardService
    {
        private IServiceUoW _loadingDBContext;
        private readonly IRepository<Study, int> _studyRepository;
        private readonly IRepository<Project, int> _projectRepository;
        private readonly IRepository<DataFile, int> _fileRepository;
        private readonly IRepository<PrimaryDataset, int> _pdsRepository;
        private readonly IRepository<ObservablePhenomenon, int> _phenoRepository;
        private readonly IRepository<ObservationDatasetDescriptor, Guid> _datasetDescriptorRepository;
        private readonly IRepository<ObservationProperty, int> _observationStudyPropertyRepository;
        private readonly IRepository<Feature, int> _featureRepository;
        private readonly IRepository<Assessment, int> _assessmentRepository;
        private readonly IRepository<Core.Domain.Model.BMO.Observation, Guid> _observationRepository;
        private readonly FileService FileService;
        private FileStorageSettings ConfigSettings { get; set; }
        private readonly string dataFilesDirectory;


        public LoadingWizardService(IServiceUoW uoW, FileService fileService, IOptions<FileStorageSettings> settings)
        {
            _loadingDBContext = uoW;
            _studyRepository = uoW.GetRepository<Study, int>();
            _projectRepository = uoW.GetRepository<Project, int>();
            _fileRepository = uoW.GetRepository<DataFile, int>();
            _pdsRepository = uoW.GetRepository<PrimaryDataset, int>();
            _datasetDescriptorRepository = uoW.GetRepository<ObservationDatasetDescriptor, Guid>();
            _phenoRepository = uoW.GetRepository<ObservablePhenomenon, int>();
            _featureRepository = uoW.GetRepository<Feature, int>();
            _assessmentRepository = uoW.GetRepository<Assessment, int>();
            _observationStudyPropertyRepository = uoW.GetRepository<ObservationProperty, int>();
            _observationRepository = uoW.GetRepository<Core.Domain.Model.BMO.Observation, Guid>();

            FileService = fileService;
            ConfigSettings = settings.Value;
            dataFilesDirectory = ConfigSettings.UploadFileDirectory;

        }

        public List<StudyDatasetsDTO> GetProjectAssessments(int projectId)
        {

            var studies = _studyRepository.FindAll(s => s.ProjectId == projectId, new List<string> { "Assessments.Datasets" }).ToList();

            if (studies == null)
                return null;

            var studyAssessmentsDTO = studies.Select(s => new StudyDatasetsDTO()
            {
                StudyName = s.Name,
                StudyTitle = s.Description,
                StudyAssessments = s.Assessments.Select(a => new DTOs.AssessmentDTO()
                {
                     Id = a.Id,
                     Name = a.Name,
                     AssociatedDatasets = a.Datasets.Select(d => new DTOs.AssessmentDatasetDTO()
                     {
                          Id = d.Id,
                          Title = d.Title,
                          Acronym = d.Acronym
                     }).ToList()
                }).ToList()
            }).ToList();

            return studyAssessmentsDTO;
        }

        public bool InitLoading(int fileId)
        {
            try
            {
                var _dataFile = _fileRepository.Get(fileId);
                _dataFile.State = "LOADING";
                _dataFile.IsLoadedToDB = false;
                _fileRepository.Update(_dataFile);
                _loadingDBContext.Save();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public void LoadFile(int fileId, int datasetId, int assessmentId)
        {
            var file = _fileRepository.Get(fileId);

            var assessment = _assessmentRepository.Get(assessmentId);

            if (file == null)
                return;

            //GET DATASET
            var primaryDataset = _pdsRepository.FindSingle(d => d.Id == datasetId);

            if (primaryDataset == null)
                return;

            //GET DSCRIPTOR
            var dd = _datasetDescriptorRepository.FindSingle(d => d.Id == primaryDataset.DescriptorId);


            //READ DATASET RECORDS
            string fullpath = Path.Combine(dataFilesDirectory, file.Path,file.FileName);
            string jsonString = File.ReadAllText(fullpath);
            // var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10,  IgnoreNullValues= true };
            PrimaryDataset pdsData = JsonSerializer.Deserialize<PrimaryDataset>(jsonString)!;


            //Get frorm the descriptor the feature fields

            //REMEMBER DOMAIN/CATEGORY/SUBCATEGROY
            var groupedByFeatures = pdsData.DataRecords.GroupBy(r => new
            {
                domain = r[dd.ClassifierFields.Find(c => c.Order == 1).Name],
                cat = r[dd.ClassifierFields.Find(c => c.Order == 2).Name],
                subcat = r[dd.ClassifierFields.Find(c => c.Order == 3).Name],
                feat = r[dd.FeatureNameField.Name]

            }).Distinct().ToList();


            //Dataset previously created phenomena
            var createdObsPhenomena = _phenoRepository
                .FindAll(f => f.DatasetId == datasetId, new List<string>() { "ObservedFeature", "ObservedProperty"}).ToList();

            var createdStudyProperties = _observationStudyPropertyRepository
                .FindAll(op => op.StudyId == assessment.StudyId).ToList();



            List<ObservablePhenomenon> phenomenonsToAdd = new();
            List<ObservationProperty> obPropertiesToAdd = new();
            List<Core.Domain.Model.BMO.Observation> observationsToAdd = new();


            List<Property> createdProperties = createdObsPhenomena.Select(p => p.ObservedProperty).ToList();
            List<Feature> createdFeatures = createdObsPhenomena.Select(p => p.ObservedFeature).ToList();

            Feature feature=null;
            foreach (var group in groupedByFeatures)
            {

                ObservablePhenomenon phenomenon;

                foreach (var obsPropertyField in dd.ObservedPropertyValueFields)
                {
                    if (!group.First().ContainsKey(obsPropertyField.Name))
                        continue;
                    if (group.All(v => v[obsPropertyField.Name] == ""))
                        continue;


                    //Create or find phenomenon
                    phenomenon = (createdObsPhenomena.Find(
                        p => p.ObservedFeature.Category == group.Key.cat
                        && p.ObservedFeature.Subcategory == group.Key.subcat
                        && p.ObservedFeature.Name == group.Key.feat
                        && p.ObservedProperty.Name == obsPropertyField.Name));

                    if (phenomenon == null)
                    {

                        //Create or find Feature
                        feature = createdFeatures.Find(
                            f => f.Category == group.Key.cat
                            && f.Subcategory == group.Key.subcat
                            && f.Name == group.Key.feat);
                        if (feature == null)
                        {
                            feature = new Feature
                            {
                                Name = group.Key.feat,
                                Domain = group.Key.domain,
                                Category = group.Key.cat,
                                Subcategory = group.Key.subcat,
                                DatasetId = datasetId,
                            };
                            createdFeatures.Add(feature);
                        }
   

                        //Create or find Property
                        Property property;
                        if (createdProperties.Exists(p => p.Name == obsPropertyField.Name))
                            property = createdProperties.Find(p => p.Name == obsPropertyField.Name);
                        else
                        {
                            property = new Property() { Name = obsPropertyField.Name };
                            createdProperties.Add(property);
                        }


                        //Create new phenomenon
                        phenomenon = new ObservablePhenomenon()
                        {
                            ObservedFeature = feature,
                            ObservedProperty = property,
                            DatasetId = datasetId
                        };

                        phenomenonsToAdd.Add(phenomenon);
                    }
                    feature = phenomenon.ObservedFeature;
                    
                }

                if (dd.FeaturePropertyNameField != null && !group.All(v => v[dd.FeaturePropertyNameField.Name] == ""))
                {
                    var featname = group.First(r => r[dd.FeaturePropertyNameField.Name] != "")[dd.FeaturePropertyNameField.Name];
                    feature.FeatureProperties.Add(new Property() { Name = featname });
                }

            }
            primaryDataset.ObservedFeatures = phenomenonsToAdd.Select(ph => ph.ObservedFeature).ToList();
            _pdsRepository.Update(primaryDataset);

            _phenoRepository.InsertMany(phenomenonsToAdd);

            foreach(var opField in dd.ObservationPropertyFields)
            {
                if(!createdStudyProperties.Exists(sp=>sp.Name == opField.Name)){
                    var op = new ObservationProperty();
                    op.StudyId = assessment.StudyId;
                    op.Name = opField.Name;
                    obPropertiesToAdd.Add(op);
                }
            }

            _observationStudyPropertyRepository.InsertMany(obPropertiesToAdd);

            _loadingDBContext.Save();
            //NOW LOAD OBSERVATIONS TO MONGO


            var savedFeatures = _featureRepository
                .FindAll(f => f.DatasetId == datasetId, new List<string>() { "ObservablePhenomena", "FeatureProperties" }).ToList();

            var savedObsProperties = _observationStudyPropertyRepository
                .FindAll(op => op.StudyId == assessment.StudyId).ToList();

            foreach(var sfeature in savedFeatures)
            {
                var recs = groupedByFeatures
                    .Where(g => g.Key.domain == sfeature.Domain
                    && g.Key.cat == sfeature.Category
                    && g.Key.subcat == sfeature.Subcategory
                    && g.Key.feat == sfeature.Name).First();

                foreach (var rec in recs)
                {
                    
                    var observation = new Core.Domain.Model.BMO.Observation();
                    observation.SubjectId = rec[dd.SubjectIdentifierField.Name];
                    observation.FeatureOfInterestId = sfeature.Id;
                    observation.DatasetId = datasetId;
                    observation.Id = Guid.NewGuid();

                    //FEATURE PROPERTIES (observation ABOUT the observed feature)(E.G. TYPE OF RASH, SOUND OF WHEEZE)
                    foreach (var p in sfeature.FeatureProperties)
                    {
                        if (rec.ContainsKey(p.Name))
                            observation.ObservedFeatureProperties.Add(new NominalObsResult()
                            {
                                ObservedPhenomenonId = p.Id,
                                Value = rec[p.Name]
                            });
                    }

                    //OBSERVED PHENOMENA
                    foreach (var p in sfeature.ObservablePhenomena)
                    {
                        if(rec.ContainsKey(p.ObservedProperty.Name))
                            observation.ObservedPhenomena.Add(new NominalObsResult()
                            {
                                ObservedPhenomenonId = p.Id,
                                Value = rec[p.ObservedProperty.Name]
                            });
                    }

                    //OBSERVATION PROPERTIES (E.G. VISIT, EPOCH, DOV)
                    foreach(var op in savedObsProperties)
                    {
                        //somewhere here I need to reference the observation properties e.g. visit epoch ..etc
                        if (rec.ContainsKey(op.Name))
                        {
                            observation.ObservationProperties.Add(new NominalObsResult()
                            {
                                ObservedPhenomenonId = op.Id,
                                Value = rec[op.Name]
                            });
                        }

                    }

                    observationsToAdd.Add(observation);
                }
            }
            _observationRepository.InsertMany(observationsToAdd);

            

        }


        

        public FileDTO GetFileProgress(int fileId)
        {
            var file = _fileRepository.Get(fileId);
            int ploaded;

            var dto = FileService.GetDTO(file);
            if (file.State == "LOADED" || file.State == "SAVED")
                dto.PercentLoaded = 100;
            else if (int.TryParse(file.State, out ploaded))
                dto.PercentLoaded = ploaded;
            return dto;
        }

        public FileDTO MatchFileToTemplate(int datasetId, int fileId)
        {
            var file = _fileRepository.Get(fileId);
            var filePath = FileService.GetFullPath(file.ProjectId);
            var colHeaders = getFileColHeaders(Path.Combine(filePath, file.FileName));


            //var dataset = _datasetRepository.FindSingle(d => d.Id == datasetId,
            //    new List<string>() { "Variables.VariableDefinition" });

            //var varNames = dataset.Variables.Select(v => v.VariableDefinition.Name).ToList();
            //var headers = colHeaders.Select(d => d["colName"]).ToList<string>();

            var fileDto = new FileDTO();
            //{
            //    FileName = file.FileName,
            //    columnHeaders = colHeaders,
            //    DataFileId = file.Id,
            //    templateMatched = headers.All(header => varNames.Contains(header))
            //};

            //if (!fileDto.templateMatched)
            //{
            //    var mappedVars = headers.FindAll(h => varNames.Contains(h));
            //    if (mappedVars.Any())
            //    {
            //        float p = ((float)mappedVars.Count / headers.Count) * 100;
            //        if (p >= 50)
            //        {
            //            fileDto.unmappedCols = headers.FindAll(h => !varNames.Contains(h));
            //            fileDto.percentMatched = (int)p;
            //        }
            //    }
            //}

            //if (fileDto.templateMatched)
            //{
            //    fileDto.percentMatched = 100;
            //    fileDto.IsStandard = true;
            //    //file.IsStandard = true;
            //    //TODO: depending on the dataset the format for the file should be set
            //    file.Format = "SDTM";
            //    _fileRepository.Update(file);
            //    _dataServiceUnit.Save();
            //}

            return fileDto;
        }

        public List<Dictionary<string, string>> getFileColHeaders(string filePath)
        {

            StreamReader reader = File.OpenText(filePath);
            string firstline = reader.ReadLine();

            string[] header = null;
            //var parser = new CsvParser(reader);
            if (firstline.Contains("\t"))
                header = firstline.Split('\t');
            else if (firstline.Contains(","))
                header = firstline.Split(',');

            var res = new List<Dictionary<string, string>>();
            for (int i = 0; i < header.Length; i++)
            {
                var r = new Dictionary<string, string>();
                r.Add("colName", header[i].Replace("\"", ""));
                r.Add("pos", i.ToString());
                res.Add(r);
            }
            reader.Dispose();

            return res;
        }

        public int? mapToTemplate(int datasetId, int fileId, DataTemplateMap map)
        {
            //var dataset = GetActivityDataset(datasetId);
            //var projectId = dataset.Activity.ProjectId;

            var dataFile = _fileRepository.Get(fileId);
            var filePath = Path.Combine(dataFile.Path, dataFile.FileName);

            //FileService fileService = new FileService(_dataServiceUnit);
            DataTable inputDataTable = FileService.ReadOriginalFile(filePath);
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
            //var dataset = _datasetRepository.FindSingle(d => d.Id == datasetId, new List<string>() { "Activity", "DataFiles" });
            //if (sdtmTable.Rows.Count != 0)
            //{
            //    string dsName = dataset.Activity.Name + "_" + dataset.TemplateId;
            //    sdtmTable.TableName = dsName;
            //    //Write new transformed to file 
            //    var fileInfo = WriteDataFile(dataFile.Path, sdtmTable);
            //    //standardFile = AddOrUpdateFile(dataFile.ProjectId, fileInfo);

            //    //Update dataset
            //    //STUPID EF 1.1
            //    //dataset.DataFiles.Add(standardFile);
            //    dataset.DataFiles.Add(new DatasetDatafile() { DatasetId = datasetId, DatafileId = dataFile.Id });
            //    /////////////////////////////////////

            //    _datasetRepository.Update(dataset);
            //    _dataServiceUnit.Save();
            //}
            //dataset.State = "mapped";

            return standardFile?.Id;
        }

    }
}

