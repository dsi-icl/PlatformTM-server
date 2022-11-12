using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Microsoft.Extensions.Options;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.JoinEntities;
using PlatformTM.Models.Configuration;
using PlatformTM.Models.DTOs;
using PlatformTM.Models.Services.Loading.AssayData;
using PlatformTM.Models.Services.Loading.SDTM;
using PlatformTM.Models.ViewModels;

namespace PlatformTM.Models.Services
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

        public bool InitLoading(int fileId, int datasetId)
        {
            try
            {
                var _dataFile = _fileRepository.Get(fileId);
                _dataFile.State = "LOADING";
                _dataFile.IsLoadedToDB = false;
                _fileRepository.Update(_dataFile);
                _dataServiceUnit.Save();
            }
            catch (Exception)
            {
                return false;
            }
         
            return true;
        }



        public bool LoadFile(int fileId, int datasetId)
        {
            
            var file = _fileRepository.Get(fileId);
            string filePath;
            DataTable dataTable;
            bool success;
            filePath = GetFullPath(file.ProjectId);
            dataTable = readDataFile(Path.Combine(filePath, file.FileName));
            //datasetId descriptor will decide the type of the dataset
            //loader is by type of dataset not by format


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

        public DriveVM GetFiles(int projectId, int folderId)
        {
            List<DataFile> contents;
            DataFile folder;

            if (folderId == 0)
            {
                contents = _fileRepository.FindAll(f => f.ProjectId == projectId && f.FolderId == null,
                                                new List<string>() { "Folder" }).ToList();
                folder = null;

            }

            else
            {
                contents = _fileRepository.FindAll(f => f.ProjectId == projectId && f.FolderId == folderId).ToList();
                folder = _fileRepository.FindSingle(f => f.ProjectId == projectId && f.Id == folderId,
                                                    new List<string>() { "Folder" });
            }

            var folders = new List<DataFile>();

            while (folder != null){
                folders.Add(folder);
                folder = folder.Folder; 
            }

        
            folders.Reverse();
            var dirsDTO = folders.Select(d => new FileDTO()
            {
                FileName = d.FileName,
                DataFileId = d.Id,
                IsDirectory = d.IsDirectory
            }).ToList();


            var filesDTO =  contents.Select(GetDTO).ToList();
            return new DriveVM() {Files = filesDTO, folders= dirsDTO };
        }

        public DataFile CreateFolder(int projectId, DirectoryDTO folder)
        {
            //if (Directory.Exists(newDir))
                //return new DirectoryInfo(newDir);

            //var di = Directory.CreateDirectory(newDir);

            var project = _projectRepository.FindSingle(p => p.Id == projectId);
            if(project ==null || folder.Name=="" || folder.Name==null)
                return null;

            var file = new DataFile();
            file.FileName = folder.Name;
         
            file.Created = DateTime.Now.ToString("D");
            file.IsDirectory = true;
            file.ProjectId = project.Id;
            if (folder.ParentFolderId != 0) file.FolderId = folder.ParentFolderId;

            _fileRepository.Insert(file);
            return _dataServiceUnit.Save().Equals("CREATED") ? file : null;
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

            //string path = Path.Combine(_uploadFileDirectory, selectFile.Path, selectFile.FileName);
            var filepath = Path.Combine(GetFullPath(selectFile.ProjectId), selectFile.FileName);
            try
            {
                File.Delete(filepath);
                _fileRepository.Remove(selectFile);
                _dataServiceUnit.Save();
            }
            catch (Exception)
            {
                
                throw;
            }
                        
        }

        public DataFile AddOrUpdateFile(int projectId, FileInfo fi, int dirId)
        {
            if (fi == null)
                return null;
            DataFile file;
            //var filePath = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf("P-"+projectId));
            //var file = _fileRepository.FindSingle(d => d.FileName.Equals(fi.Name) && d.Path.Equals(filePath) && d.ProjectId == projectId);
            if(dirId==0){
                file = _fileRepository.FindSingle(d => d.FileName.Equals(fi.Name) && d.ProjectId == projectId && d.FolderId == null);
            }
            else

                file = _fileRepository.FindSingle(d => d.FileName.Equals(fi.Name) && d.ProjectId == projectId && d.FolderId == dirId);

            if (file == null)
            {
                var project = _projectRepository.FindSingle(p => p.Id == projectId);
                if (project == null)
                    return null;
                file = new DataFile
                {
                    FileName = fi.Name,
                    Created = fi.CreationTime.ToString("d") + " " + fi.CreationTime.ToString("t"),
                    State = "NEW",
                    //Path = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf("P-" + projectId)),
                    IsDirectory = false,
                    ProjectId = project.Id,
                    FolderId = dirId
                };
                _fileRepository.Insert(file);
            }
            else
            {
                file.Modified = fi.LastWriteTime.ToString("d") + " " + fi.LastWriteTime.ToString("t");
                if (file.IsLoadedToDB.Value || file.State == "FAILED TO LOAD")
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

        public FileContentVM GetFilePreview(int fileId)
        {

            var file = _fileRepository.FindSingle(f=>f.Id == fileId, new List<string>() { "Folder" });

            var folder = file.Folder;

            var folders = new List<DataFile>();
            while (folder != null)
            {
                folders.Add(folder);
                folder = folder.Folder;
            }
            folders.Reverse();
            var foldersDTO = folders.Select(GetDTO).ToList();


            var filePath = GetFullPath(file.ProjectId);
            var dataTable = readDataFile(Path.Combine(filePath,file.FileName));

            if (dataTable.Rows.Count > 1000)
                dataTable.Rows.RemoveRange(100, dataTable.Rows.Count - 100);

            if (dataTable.Columns.Count > 40)
                dataTable.Columns.RemoveRange(100, dataTable.Columns.Count - 100);
            dataTable.TableName = file.FileName;

            var vM = new FileContentVM
            {
                Folders = foldersDTO,
                Data = dataTable,
                File = GetDTO(file)
            };

            return vM;
        }

        public FileDTO MatchFileToTemplate(int datasetId, int fileId)
        {
            var file = _fileRepository.Get(fileId);
            var filePath = GetFullPath(file.ProjectId);
            var colHeaders = getFileColHeaders(Path.Combine(filePath, file.FileName));

           
            var dataset = _datasetRepository.FindSingle(d => d.Id == datasetId,
                new List<string>() {"Variables.VariableDefinition"});

            var varNames = dataset.Variables.Select(v => v.VariableDefinition.Name).ToList();
            var headers = colHeaders.Select(d => d["colName"]).ToList<string>();

            var fileDto = new FileDTO
            {
                FileName = file.FileName,
                columnHeaders = colHeaders,
                DataFileId = file.Id,
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
                //file.IsStandard = true;
                //TODO: depending on the dataset the format for the file should be set
                file.Format = "SDTM";
                _fileRepository.Update(file);
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
                //standardFile = AddOrUpdateFile(dataFile.ProjectId, fileInfo);

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
            DataTable _dt = new DataTable();



            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                // Do any configuration to `CsvReader` before creating CsvDataReader.
                using (var dreader = new CsvDataReader(csv))
                {
                    var dt = new System.Data.DataTable();
                    dt.Load(dreader);

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        _dt.Columns.Add(dt.Columns[i].ColumnName,typeof(string));
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var srcDr = dt.Rows[i];
                        DataRow dr = _dt.NewRow();

                        for (int j = 0; j < srcDr.ItemArray.Length; j++)
                        {
                            dr[j] = srcDr[j];
                        }
                                
                        _dt.Rows.Add(dr);

                    }
                }
            }

            return _dt;
           

            //StreamReader reader = File.OpenText(filePath);
            //var parser = new CsvParser(reader);
            //string[] header = parser.Read();
            //if (!(header.Count() > 1))
            //{
            //    if (header[0].Contains("\t"))
            //    {
            //        parser.Configuration.Delimiter = "\t";
            //        header = header[0].Split('\t');
            //    }
            //}


            //foreach (string field in header)
            //{
            //    dt.Columns.Add(field.Replace("\"", "").Replace(" ", ""), typeof(string));
            //}

            //while (true)
            //{
            //    try
            //    {
            //        var row = parser.Read();
            //        if (row == null)
            //            break;

            //        DataRow dr = dt.NewRow();
            //        if (row.Length == 0 || row.Length != dt.Columns.Count)
            //        {
            //            Debug.WriteLine(row.Length + " " + dt.Columns.Count);
            //            return null;
            //        }

            //        for (int i = 0; i < row.Length; i++)
            //        {
            //            if (row[i] == null)
            //                Debug.WriteLine(row);
            //            dr[i] = row[i];
            //        }
            //        dt.Rows.Add(dr);
            //    }
            //    catch (System.NullReferenceException e)
            //    {
            //        Debug.WriteLine(e.Message);
            //        throw;
            //    }


        
            //parser.Dispose();
            //reader.Dispose();

            //return dt;
        }

        //private DataTable readDataFile(string filePath)
        //{
        //    System.Data.DataTable dt = new System.Data.DataTable();
        //    using (var reader = new StreamReader(filePath))
        //    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        //    {
        //        // Do any configuration to `CsvReader` before creating CsvDataReader.
        //        using (var dr = new CsvDataReader(csv))
        //        {

        //            dt.Load(dr);
        //        }
        //    }
        //    return dt;
        //}
        public List<Dictionary<string, string>> getFileColHeaders(string filePath)
        {
           
            StreamReader reader = File.OpenText(filePath);
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
       

        public FileDTO GetFile(int fileId)
        {
            var file = _fileRepository.Get(fileId);
            int ploaded;

            var dto = GetDTO(file);
            if (file.State == "LOADED" || file.State == "SAVED")
                dto.PercentLoaded = 100;
            else if (int.TryParse(file.State, out ploaded))
                dto.PercentLoaded = ploaded;
            return dto;
        }

        private FileDTO GetDTO(DataFile file){
            var dto = new FileDTO()
            {
                FileName = file.FileName,
                dateAdded = file.Created,
                dateLastModified = file.Modified ?? file.Created,
                icon = "",
                IsDirectory = file.IsDirectory,
                selected = false,
                state = file.State,
                DataFileId = file.Id,
                FolderId = file.FolderId,
                IsLoaded = file.IsLoadedToDB.Value
            };
            return dto;
        }

        public string GetFullPath(int projectId)
        {
            return Path.Combine(_uploadFileDirectory, "P-" + projectId);
        }

        //TODO: to update...to be tested
        public FileStream GetFile(int fileId, out string filename)
        {
            var file = _fileRepository.Get(fileId);
            var filePath = Path.Combine(GetFullPath(file.ProjectId), file.FileName);
            filename = file.FileName;
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
            return fileStream;
        }
    }
}
