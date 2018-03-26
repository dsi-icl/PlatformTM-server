using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.Extensions.Options;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Services.Configuration;
using PlatformTM.Services.DTOs;
using PlatformTM.Services.Services.Loading.AssayData;
using PlatformTM.Services.Services.Loading.SDTM;

namespace PlatformTM.Services.Services
{
    public class FileService
    {
        private readonly IServiceUoW _dataServiceUnit;
        private readonly IRepository<DataFile, int> _fileRepository;
        private readonly IRepository<Project, int> _projectRepository;
        private readonly IRepository<Dataset, int> _datasetRepository;
        
        private FileStorageSettings ConfigSettings { get; set; }
        private readonly string _uploadFileDirectory;
        private readonly string _downloadFileDirectory;


        //private readonly IRepository<SdtmRow, Guid> _sdtmRepository;
        private readonly IRepository<Observation, int> _observationRepository;

        public FileService(IServiceUoW uoW, IOptions<FileStorageSettings> settings)
        {
            _dataServiceUnit = uoW;
            _fileRepository = uoW.GetRepository<DataFile, int>();
            _projectRepository = uoW.GetRepository<Project, int>();
            _datasetRepository = uoW.GetRepository<Dataset, int>();

            ConfigSettings = settings.Value;
            _uploadFileDirectory = ConfigSettings.UploadFileDirectory;
            _downloadFileDirectory = ConfigSettings.DownloadFileDirectory;
            _observationRepository = uoW.GetRepository<Observation, int>();
        }

        public bool LoadFile(int fileId, int datasetId)
        {
            
            var file = _fileRepository.Get(fileId);
            string filePath;
            DataTable dataTable;
            bool success;
            filePath = Path.Combine(file.Path, file.FileName);
            dataTable = ReadOriginalFile(filePath);
            switch (file.Format)
            {
                case "SDTM":
                    var sdtmLoader = new SDTMloader(_dataServiceUnit);
                    success = sdtmLoader.LoadSDTM(datasetId, fileId, dataTable);
                    return success;
                case "ADTM":
                    var hdDataloader = new HDloader(_dataServiceUnit);
                    success = hdDataloader.LoadHDdata(datasetId, fileId, dataTable);
                    return success;
            }
            return false;
        }

        public void GetLoadingProgress(int fileId, int datasetId)
        {
            var file = _fileRepository.Get(fileId);
            
        }

        public List<FileDTO> GetUploadedFiles(int projectId,string path)
        {
            var files = _fileRepository.FindAll(f => f.ProjectId == projectId && f.Path.Equals(path));
            return files.Select(file => new FileDTO
            {
                FileName = file.FileName,
                dateAdded = file.DateAdded,
                dateLastModified = file.LastModified ?? file.DateAdded,
                icon = "",
                IsDirectory = file.IsDirectory,
                IsLoaded = file.IsLoadedToDB,
                selected = false,
                state = file.State,
                DataFileId = file.Id,
                path = file.Path
            }).ToList();
        }

        public DirectoryInfo AddDirectory(int projectId, string newDir)
        {
            if (Directory.Exists(newDir))
                return new DirectoryInfo(newDir);

            var di = Directory.CreateDirectory(newDir);

            var project = _projectRepository.FindSingle(p => p.Id == projectId);
            if(project ==null)
                return null;

            var file = new DataFile();
            file.FileName = di.Name;
            //file.Path = di.FullName.Substring(di.FullName.IndexOf(projectId));
            file.Path = di.Parent.FullName.Substring(di.Parent.FullName.IndexOf("P-"+projectId));//file.Path.Substring(0,file.Path.LastIndexOf("\\\"))
            file.DateAdded = di.CreationTime.ToString("D");
            file.IsDirectory = true;
            file.ProjectId = project.Id;

            _fileRepository.Insert(file);
            return _dataServiceUnit.Save().Equals("CREATED") ? di : null;
        }

        public void DeleteFile(int fileId)
        {
            var selectFile = _fileRepository.FindSingle(f => f.Id == fileId, new List<string> { "Datasets" });

            //bool success = false;
            //if (selectFile.State == "LOADED")
            //{ 
            //    success = UnloadFile(fileId);
            //}

            //TODO:IF selectFile.State != LOADED, success is still false. Will not be able to delete file
            //if (success)
            //{
       
            string path = Path.Combine(_uploadFileDirectory, selectFile.Path, selectFile.FileName);
            try
            {
                File.Delete(path);
                _fileRepository.Remove(selectFile);
                _dataServiceUnit.Save();
            }
            catch (Exception)
            {
                
                throw;
            }
                        
        }

        public DataFile AddOrUpdateFile(int projectId, FileInfo fi)
        {
            if (fi == null)
                return null;
            var filePath = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf("P-"+projectId));
            var file = _fileRepository.FindSingle(d => d.FileName.Equals(fi.Name) && d.Path.Equals(filePath) && d.ProjectId == projectId);
            if (file == null)
            {
                var project = _projectRepository.FindSingle(p => p.Id == projectId);
                if (project == null)
                    return null;
                file = new DataFile
                {
                    FileName = fi.Name,
                    DateAdded = fi.CreationTime.ToString("d") + " " + fi.CreationTime.ToString("t"),
                    State = "NEW",
                    Path = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf("P-" + projectId)),
                    IsDirectory = false,
                    ProjectId = project.Id,
                };
                _fileRepository.Insert(file);
            }
            else
            {
                file.LastModified = fi.LastWriteTime.ToString("d") + " " + fi.LastWriteTime.ToString("t");
                if (file.IsLoadedToDB || file.State == "FAILED TO LOAD")
                    file.State = "UPDATED";
                _fileRepository.Update(file);
            }
            return _dataServiceUnit.Save().Equals("CREATED") ? file : null;
        }

        public List<string> GetDirectories(int projectId)
        {
            var dirs = _fileRepository.FindAll(f => f.IsDirectory.Equals(true) && f.ProjectId == projectId);
            return dirs?.Select(d => d.FileName).ToList();
        }

        public DataTable GetFilePreview(int fileId)
        {

            var file = _fileRepository.Get(fileId);
            var filePath = Path.Combine(file.Path, file.FileName);

            var dataTable = ReadOriginalFile(filePath);

            if (dataTable.Rows.Count > 1000)
                dataTable.Rows.RemoveRange(100, dataTable.Rows.Count - 100);

            if (dataTable.Columns.Count > 40)
                dataTable.Columns.RemoveRange(100, dataTable.Columns.Count - 100);
            dataTable.TableName = file.FileName;

            return dataTable;
        }

        public FileDTO MatchFileToTemplate(int datasetId, int fileId)
        {
            var dataFile = _fileRepository.Get(fileId);
            var filePath = Path.Combine(dataFile.Path, dataFile.FileName);
            var colHeaders = getFileColHeaders(filePath);

           
            var dataset = _datasetRepository.FindSingle(d => d.Id == datasetId,
                new List<string>() {"Variables.VariableDefinition"});

            var varNames = dataset.Variables.Select(v => v.VariableDefinition.Name).ToList();
            var headers = colHeaders.Select(d => d["colName"]).ToList<string>();

            var fileDto = new FileDTO
            {
                FileName = dataFile.FileName,
                columnHeaders = colHeaders,
                DataFileId = dataFile.Id,
                templateMatched = headers.All(header => varNames.Contains(header))
            };

            if (!fileDto.templateMatched)
            {
                var mappedVars = headers.FindAll(h => varNames.Contains(h));
                if (mappedVars.Any())
                {
                    float p = ((float)mappedVars.Count / headers.Count) * 100;
                    if (p >= 50)
                    {
                        fileDto.unmappedCols = headers.FindAll(h => !varNames.Contains(h));
                        fileDto.percentMatched = (int)p;
                    }
                }
            }

            if (fileDto.templateMatched)
            {
                fileDto.percentMatched = 100;
                fileDto.IsStandard = true;
                dataFile.IsStandard = true;
                //TODO: depending on the dataset the format for the file should be set
                dataFile.Format = "SDTM";
                _fileRepository.Update(dataFile);
                _dataServiceUnit.Save();
            }

            return fileDto;
        }

        public int? mapToTemplate(int datasetId, int fileId, DataTemplateMap map)
        {
            //var dataset = GetActivityDataset(datasetId);
            //var projectId = dataset.Activity.ProjectId;

            var dataFile = _fileRepository.Get(fileId);
            var filePath = Path.Combine(dataFile.Path, dataFile.FileName);

            //FileService fileService = new FileService(_dataServiceUnit);
            DataTable inputDataTable = ReadOriginalFile(filePath);
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
            var dataset = _datasetRepository.FindSingle(d=>d.Id == datasetId, new List<string>() {"Activity","DataFiles"});
            if (sdtmTable.Rows.Count != 0)
            {
                string dsName = dataset.Activity.Name + "_" + dataset.TemplateId;
                sdtmTable.TableName = dsName;
                //Write new transformed to file 
                var fileInfo = WriteDataFile(dataFile.Path, sdtmTable);
                standardFile = AddOrUpdateFile(dataFile.ProjectId, fileInfo);

                //Update dataset
                //STUPID EF 1.1
                //dataset.DataFiles.Add(standardFile);
                dataset.DataFiles.Add(new DatasetDatafile() { DatasetId = datasetId, DatafileId = dataFile.Id });
                /////////////////////////////////////

                _datasetRepository.Update(dataset);
                _dataServiceUnit.Save();
            }
            dataset.State = "mapped";

            return standardFile?.Id;
        }

        #region IO methods
        

        public DataTable ReadOriginalFile(string filePath)
        {
            return readDataFile(Path.Combine(_uploadFileDirectory,filePath));
        }

        private DataTable readDataFile(string filePath)
        {
            DataTable dt = new DataTable();

            StreamReader reader = File.OpenText(filePath);
            var parser = new CsvParser(reader);
            string[] header = parser.Read();
            if (!(header.Count() > 1))
            {
                if (header[0].Contains("\t"))
                {
                    parser.Configuration.Delimiter = "\t";
                    header = header[0].Split('\t');
                }
            }


            foreach (string field in header)
            {
                dt.Columns.Add(field.Replace("\"", ""), typeof(string));
            }

            while (true)
            {
                try
                {
                    var row = parser.Read();
                    if (row == null)
                        break;

                    DataRow dr = dt.NewRow();
                    if (row.Length == 0 || row.Length != dt.Columns.Count)
                    {
                        Debug.WriteLine(row.Length + " " + dt.Columns.Count);
                        return null;
                    }

                    for (int i = 0; i < row.Length; i++)
                    {
                        if (row[i] == null)
                            Debug.WriteLine(row);
                        dr[i] = row[i];
                    }
                    dt.Rows.Add(dr);
                }
                catch (System.NullReferenceException e)
                {
                    Debug.WriteLine(e.Message);
                    throw ;
                }
            }
            parser.Dispose();
            reader.Dispose();

            return dt;
        }

        public List<Dictionary<string, string>> getFileColHeaders(string filePath)
        {
            //Parse header of the file
            string PATH = _uploadFileDirectory + filePath;// + studyId + "\\" + fileName;
            StreamReader reader = File.OpenText(PATH);
            string firstline = reader.ReadLine();

            string[] header = null;
            //var parser = new CsvParser(reader);
            if (firstline.Contains("\t"))
                header = firstline.Split('\t');
            else if(firstline.Contains(","))
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

        public FileInfo WriteDataFile(string path, DataTable dt)
        {
            var dirPath = Path.Combine(_downloadFileDirectory, path);
            var di = Directory.CreateDirectory(dirPath);
            if(!di.Exists) return null;
            var filePath = Path.Combine(dirPath, dt.TableName + ".csv");


            StreamWriter writer = File.CreateText(filePath);

            var headerValues = dt.Columns.Cast<DataColumn>()
                .Select(column => QuoteValue(column.ColumnName));

            writer.WriteLine(string.Join(",", headerValues));

            foreach (DataRow row in dt.Rows)
            {
                var items = row.Values.Cast<object>().Select(o => QuoteValue(o.ToString()));
                writer.WriteLine(string.Join(",", items));
            }
            writer.Flush();
            writer.Dispose();
            return new FileInfo(filePath);
        }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }

        #endregion

      
        /**
         *To gather one column we need the following params
         *- identifier columns (columns that will remain as is
         *- gather columns (those that will be pivoted to long format
         *- name of the key column
         *- name of the value column
         */
       

        public FileDTO GetFileDTO(int fileId)
        {
            var file = _fileRepository.Get(fileId);
            int ploaded;

            var dto = new FileDTO()
            {
              FileName = file.FileName,
                dateAdded = file.DateAdded,
                dateLastModified = file.LastModified ?? file.DateAdded,
                icon = "",
                IsDirectory = file.IsDirectory,
                selected = false,
                state = file.State,
                DataFileId = file.Id,
                path = file.Path
        };
            if (file.State == "LOADED")
                dto.PercentLoaded = 100;
            else if (int.TryParse(file.State, out ploaded))
                dto.PercentLoaded = ploaded;


            return dto;
        }

        public string GetFullPath(string projectId, string subdir)
        {
            return Path.Combine(_uploadFileDirectory, "P-" + projectId, subdir);
        }


        public FileStream GetFile(int fileId, out string filename)
        {
            var file = _fileRepository.Get(fileId);
            var filePath = Path.Combine(_uploadFileDirectory,file.Path, file.FileName);
            filename = file.FileName;
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return fileStream;
        }
    }
}
