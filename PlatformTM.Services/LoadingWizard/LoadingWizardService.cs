using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;
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
        private readonly FileService FileService;


        public LoadingWizardService(IServiceUoW uoW, FileService fileService)
        {
            _loadingDBContext = uoW;
            _studyRepository = uoW.GetRepository<Study, int>();
            _projectRepository = uoW.GetRepository<Project, int>();
            _fileRepository = uoW.GetRepository<DataFile, int>();
            FileService = fileService;

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

        //public void ImportDataToPDS(DataFile dataFile, DatasetDescriptor datasetDescriptor)
        //{

        //}
        public bool LoadFile(int fileId, int datasetId)
        {

            var file = _fileRepository.Get(fileId);
            string filePath;
            DataTable dataTable;
            bool success=true;
            filePath = FileService.GetFullPath(file.ProjectId);
            dataTable = FileService.ReadDataFile(Path.Combine(filePath, file.FileName));
            //datasetId descriptor will decide the type of the dataset
            //loader is by type of dataset not by format


            switch (file.Format)
            {
                case "SDTM":
                    //var sdtmLoader = new SDTMloader(_dataServiceUnit);
                    //success = sdtmLoader.LoadSDTM(datasetId, fileId, dataTable);
                    return success;
                case "ADTM":
                    //var hdDataloader = new HDloader(_dataServiceUnit);
                    //success = hdDataloader.LoadHDdata(datasetId, fileId, dataTable);
                    return success;
            }
            return false;
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

