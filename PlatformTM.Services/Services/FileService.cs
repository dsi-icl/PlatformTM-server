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
            var dataTable = ReadDataFile(Path.Combine(filePath,file.FileName));

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

       
        #region IO methods
        



        public DataTable ReadOriginalFile(string filePath)
        {
            return ReadDataFile(Path.Combine(_uploadFileDirectory,filePath));
        }

        public DataTable ReadDataFile(string filePath)
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

        public FileDTO GetDTO(DataFile file){
            var dto = new FileDTO()
            {
                FileName = file.FileName,
                dateAdded = file.Created,
                dateLastModified = file?.Modified ?? file.Created,
                icon = "",
                IsDirectory = file.IsDirectory,
                selected = false,
                state = file.State,
                DataFileId = file.Id,
                FolderId = file.FolderId,
                IsLoaded = file.IsLoadedToDB.HasValue ? file.IsLoadedToDB.Value : false 
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
