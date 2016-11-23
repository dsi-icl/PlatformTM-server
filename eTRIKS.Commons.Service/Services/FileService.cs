using System.Collections;
using System.Diagnostics;
using CsvHelper;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using Microsoft.Extensions.Options;
using eTRIKS.Commons.Service.Configuration;

namespace eTRIKS.Commons.Service.Services
{
    public class FileService
    {
        private IServiceUoW _dataServiceUnit;
        private IRepository<DataFile, int> _fileRepository;
        private IRepository<Project, int> _projectRepository;
        private FileStorageSettings ConfigSettings { get; set; }
        private string uploadedFilesDirectory;
        private string stdFilesDirecotry;

        public FileService(IServiceUoW uoW, IOptions<FileStorageSettings> settings)
        {
            _dataServiceUnit = uoW;
            _fileRepository = uoW.GetRepository<DataFile, int>();
            _projectRepository = uoW.GetRepository<Project, int>();
            ConfigSettings = settings.Value;
            uploadedFilesDirectory = ConfigSettings.FileDirectory;//ConfigurationManager.AppSettings["FileDirectory"];
            stdFilesDirecotry = uploadedFilesDirectory + "\\Mapped";
        }
        public List<FileDTO> getUploadedFiles(int projectId,string path)
        {
            List<FileDTO> fileDTOs = new List<FileDTO>();
            


            var files = _fileRepository.FindAll(f => f.ProjectId == projectId && f.Path.Equals(path));
            //DirectoryInfo f = new DirectoryInfo(path);
            foreach (var file in files)
            {
                FileDTO fileDTO = new FileDTO();
                fileDTO.FileName = file.FileName;
                fileDTO.dateAdded = file.DateAdded;//.LastWriteTime.ToLongDateString();
                fileDTO.dateLastModified = file.LastModified ?? file.DateAdded;
                fileDTO.icon = "";
                fileDTO.IsDirectory = file.IsDirectory;
                fileDTO.selected = false;
                fileDTOs.Add(fileDTO);
                fileDTO.state = file.State;
                fileDTO.DataFileId = file.Id;
                fileDTO.path = file.Path;
            }
            return fileDTOs;
        }

        public DirectoryInfo addDirectory(int projectId, string newDir)
        {
            if (Directory.Exists(newDir))
                return new DirectoryInfo(newDir);

            
            var di = Directory.CreateDirectory(newDir);
                //_fileService.addDirectory(projectId, new DirectoryInfo(newDir), projectId);
            
           
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

        public DataFile addOrUpdateFile(int projectId, FileInfo fi)
        {
            //TODO: projectId
            DataFile file;
            if (fi == null)
                return null;
            string filePath = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf("P-"+projectId));
            file = _fileRepository.FindSingle(d => d.FileName.Equals(fi.Name) && d.Path.Equals(filePath) && d.ProjectId == projectId);
            //TODO: add property isLoadedToDB to the file and only change status to modified if not loadedToDB
            if (file == null)
            {
                var project = _projectRepository.FindSingle(p => p.Id == projectId);
                if (project == null)
                    return null;
                file = new DataFile();
                file.FileName = fi.Name;
                file.DateAdded = fi.CreationTime.ToString("d") + " " + fi.CreationTime.ToString("t");
                file.State = "NEW";
                file.Path = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf("P-"+projectId));
                file.IsDirectory = false;
                _fileRepository.Insert(file);
                file.ProjectId = project.Id;
            }
            else
            {
                file.LastModified = fi.LastWriteTime.ToString("d") + " " + fi.LastWriteTime.ToString("t");
                if (file.LoadedToDB)
                    file.State = "UPDATED";
                _fileRepository.Update(file);

            }
            return _dataServiceUnit.Save().Equals("CREATED") ? file : null;
        }

        #region IO methods

        public DataTable ReadOriginalFile(string filePath)
        {
            string PATH = uploadedFilesDirectory + filePath;
            return readDataFile(PATH);
        }

        private DataTable readDataFile(string filePath)
        {
            DataTable dt = new DataTable();
       
                StreamReader reader = File.OpenText(filePath);
                //var csv = new CsvReader(reader);
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
                    dt.Columns.Add(field.Replace("\"", "").ToUpper(), typeof (string));
                }

                while (true)
                {
                    try
                    {
                        var row = parser.Read();
                        if (row == null)
                            break;

                        DataRow dr = dt.NewRow();
                        if (row.Length == 0 || row.Length != dt.Columns.Count){
                            Debug.WriteLine(row.Length+" "+dt.Columns.Count);
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
                    }
                }
                parser.Dispose();
            reader.Dispose();

            return dt;
        }

        public List<Dictionary<string, string>> getFileColHeaders(string filePath)
        {
            //Parse header of the file
            string PATH = uploadedFilesDirectory + filePath;// + studyId + "\\" + fileName;
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

        public FileInfo writeDataFile(int projectId, string filePath, DataTable dt)
        {

            var PATH = stdFilesDirecotry + filePath;
            var DirInfo = addDirectory(projectId, PATH);

            string strFilePath = DirInfo.FullName + "\\" + dt.TableName + ".csv";

            StreamWriter writer = File.CreateText(strFilePath);

            IEnumerable<String> headerValues = dt.Columns.Cast<DataColumn>()
                .Select(column => QuoteValue(column.ColumnName));

            writer.WriteLine(String.Join(",", headerValues));
            IEnumerable<String> items;

            foreach (DataRow row in dt.Rows)
            {
                items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
                writer.WriteLine(String.Join(",", items));
            }
            writer.Flush();
            writer.Dispose();
            return new FileInfo(strFilePath);
        }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }

        #endregion

        public List<string> getDirectories(int projectId)
        {
           var dirs =  _fileRepository.FindAll(f => f.IsDirectory.Equals(true) && f.ProjectId == projectId);
            if (dirs == null) return null;
            return dirs.Select(d => d.FileName).ToList();
        }

        public Hashtable getFilePreview(int fileId)
        {
            //var dataset = GetActivityDataset(datasetId);
            //var dataFile = dataset.DataFiles.SingleOrDefault(df => df.Id.Equals(fileId));
            //var studyId = dataset.Activity.StudyId;
            var file = _fileRepository.Get(fileId);
            var filePath = file.Path + "\\" + file.FileName;

            //var fileService = new FileService(_dataServiceUnit);
            //TEMP usage of dataset.state
            var dataTable = ReadOriginalFile(filePath);// : fileService.readStandardFile(studyId, fileName);
            var ht = getHashtable(dataTable);
            ht.Add("fileInfo",file.FileName);
            return ht;
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
            ht.Add("data", sdtmTable.Rows);
            
            return ht;
        }

        //public void tempmethod()
        //{
        //    DataTable usubjids = ReadOriginalFile("temp/CRC305Dusubjids.csv");
        //    //DataTable cytofSamples = ReadOriginalFile("temp/CyTOFsamples.csv");
        //    DataTable Samples = ReadOriginalFile("temp/BS_ic.csv");
        //    //DataTable FACSSamples = ReadOriginalFile("temp/FACSsamples_v1.csv");
        //    //luminexSamples.TableName = "luminexSamples";

        //    //Samples.Columns.Add("USUBJID");
        //    List<string> subjidlist = new List<string>();
        //    Dictionary<string, string> idmap = new Dictionary<string, string>();

        //    foreach (DataRow row in usubjids.Rows)
        //    {
        //        string[] id = row[0].ToString().Split('-');
        //        idmap.Add(id[2],row[0].ToString());
        //    }

        //    foreach (DataRow row in Samples.Rows)
        //    {
        //        string subjId = row["donor"].ToString();
        //        //string newsubjid = subjidlist.Find(d => d.EndsWith(subjId));
        //        if(subjId == "N/A")
        //            continue;
        //        string newsubjid = idmap[subjId];
        //        row["USUBJID"] = newsubjid;
        //    }
        //    string path = ConfigurationManager.AppSettings["FileDirectory"];
        //    StreamWriter writer = File.CreateText(path + "temp\\BS.csv");

        //    IEnumerable<String> headerValues = Samples.Columns.Cast<DataColumn>()
        //        .Select(column => QuoteValue(column.ColumnName));

        //    writer.WriteLine(String.Join(",", headerValues));
        //    IEnumerable<String> items;

        //    foreach (DataRow row in Samples.Rows)
        //    {
        //        items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
        //        writer.WriteLine(String.Join(",", items));
        //    }
        //    writer.Flush();
        //    writer.Dispose();
        //}

        /**
         *To gather one column we need the following params
         *- identifier columns (columns that will remain as is
         *- gather columns (those that will be pivoted to long format
         *- name of the key column
         *- name of the value column
         */
        //public void getLongFormat()
        //{
        //    DataTable wideDataTable = ReadOriginalFile("temp/CyTOFdata_v2.csv");
        //    DataTable longDataTable = new DataTable();

        //    List<string> ids = new List<string>() { "SAMPLEID","POP","COUNT", "PERTOT"};
        //    List<string> gatherColumns = new List<string>();
        //    int gatherColumnsFrom = 7;
        //    int gatherColumnsTo = 111;

        //    List<int> countColumns = new List<int>(){1,10,19,28};

        //    //Retrieve dataset template for the long format file
        //    //identify key column and value Column
        //    string keyColumn = "OBSMEA", valueColumn = "OBSVALUE",
        //        featureColumn = "FEAT", domainColumn = "DOMAIN";

        //    //1- Create new table from the identifier columns + the new columns
        //    longDataTable.Columns.Add(domainColumn);
        //    foreach (var idCol in ids )
        //    {
        //        longDataTable.Columns.Add(idCol);
        //    }
        //    //longDataTable.Columns.Add(popColumn);
        //    //longDataTable.Columns.Add(countColumn);
        //    longDataTable.Columns.Add(featureColumn);
        //    longDataTable.Columns.Add(keyColumn);
        //    longDataTable.Columns.Add(valueColumn);

        //    foreach (DataRow inRow in wideDataTable.Rows)
        //    {
                

        //        for (int i = gatherColumnsFrom; i <= gatherColumnsTo; i++)
        //        {
        //            DataRow longDataRow = longDataTable.NewRow();
        //            foreach (var idCol in ids)
        //            {
        //                longDataRow[idCol] = inRow[idCol];
        //            }
        //            string[] keyValue = wideDataTable.Columns[i].ToString().Split('.');
        //            longDataRow[keyColumn] = keyValue[0];
        //            longDataRow[valueColumn] = inRow[i];
        //            longDataRow[featureColumn] = keyValue[1];
        //            longDataRow[domainColumn] = "CY";

        //            longDataTable.Rows.Add(longDataRow);
        //        }
        //        //foreach (DataColumn col in inputDataTable.Columns)
        //        //{
                    
        //        //}
        //    }
        //   // var fileInfo = writeDataFile("temp/CyTOFdata_long.csv", longDataTable);
        //    string path = ConfigurationManager.AppSettings["FileDirectory"];
        //    StreamWriter writer = File.CreateText(path+"temp\\CyTOFdata_long.csv");

        //    IEnumerable<String> headerValues = longDataTable.Columns.Cast<DataColumn>()
        //        .Select(column => QuoteValue(column.ColumnName));

        //    writer.WriteLine(String.Join(",", headerValues));
        //    IEnumerable<String> items;

        //    foreach (DataRow row in longDataTable.Rows)
        //    {
        //        items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
        //        writer.WriteLine(String.Join(",", items));
        //    }
        //    writer.Flush();
        //    writer.Dispose();
        //}

        //public void getLongFormat2()
        //{
        //    DataTable wideDataTable = ReadOriginalFile("temp/FACSdata_v2.csv");
        //    DataTable longDataTable = new DataTable();

        //    //List<string> ids = new List<string>() { "SAMPLEID","POP","COUNT", "PERTOT"};
        //    List<string> ids = new List<string>() { "SAMPLEID" };
        //    List<string> gatherColumns = new List<string>();
        //    int gatherColumnsFrom = 7;
        //    int gatherColumnsTo = 111;



        //    List<int> countColumns = new List<int>() { 1, 10, 19, 28 };

        //    //Retrieve dataset template for the long format file
        //    //identify key column and value Column
        //    string countColumn = "COUNT", keyColumn = "OBSMEA", valueColumn = "OBSVALUE",
        //        featureColumn = "FEAT", domainColumn = "DOMAIN", popColumn = "POPULATION";



        //    //1- Create new table from the identifier columns + the new columns
        //    longDataTable.Columns.Add(domainColumn);
        //    foreach (var idCol in ids)
        //    {
        //        longDataTable.Columns.Add(idCol);
        //    }
        //    longDataTable.Columns.Add(popColumn);
        //    longDataTable.Columns.Add(countColumn);
        //    longDataTable.Columns.Add(featureColumn);
        //    longDataTable.Columns.Add(keyColumn);
        //    longDataTable.Columns.Add(valueColumn);

        //    foreach (DataRow inRow in wideDataTable.Rows)
        //    {
        //        for(int k=0; k<countColumns.Count;k++)
        //        {
        //            for (int i = countColumns[k] + 1; k+1==countColumns.Count?i<inRow.ItemArray.Length:i < countColumns[k+1]; i++)
        //            {
        //                DataRow longDataRow = longDataTable.NewRow();
        //                foreach (var idCol in ids)
        //                {
        //                    longDataRow[idCol] = inRow[idCol];
        //                }
        //                string[] popCountKeyValue = wideDataTable.Columns[countColumns[k]].ToString().Split('.');

        //                longDataRow[popColumn] = popCountKeyValue[0];
        //                longDataRow[countColumn] = inRow[countColumns[k]];

        //                string[] keyValue = wideDataTable.Columns[i].ToString().Split('.');
        //                longDataRow[keyColumn] = keyValue[1];
        //                longDataRow[valueColumn] = inRow[i];
        //                longDataRow[featureColumn] = keyValue[2];
        //                longDataRow[domainColumn] = "CY";

        //                longDataTable.Rows.Add(longDataRow);
        //            }
        //        }

                
        //        //foreach (DataColumn col in inputDataTable.Columns)
        //        //{

        //        //}
        //    }
        //    // var fileInfo = writeDataFile("temp/CyTOFdata_long.csv", longDataTable);
        //    string path = ConfigurationManager.AppSettings["FileDirectory"];
        //    StreamWriter writer = File.CreateText(path + "temp\\FACSdata_long.csv");

        //    IEnumerable<String> headerValues = longDataTable.Columns.Cast<DataColumn>()
        //        .Select(column => QuoteValue(column.ColumnName));

        //    writer.WriteLine(String.Join(",", headerValues));
        //    IEnumerable<String> items;

        //    foreach (DataRow row in longDataTable.Rows)
        //    {
        //        items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
        //        writer.WriteLine(String.Join(",", items));
        //    }
        //    writer.Flush();
        //    writer.Dispose();
        //}

        public FileDTO GetFileDTO(int fileId)
        {
            var file = _fileRepository.Get(fileId);

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
            return dto;
        }

        public string GetFullPath(string projectId, string subdir)
        {
            //string fileDir = ConfigurationManager.AppSettings["FileDirectory"];
            string projDir = uploadedFilesDirectory + "P-" + projectId;
            string newDir = projDir + "/" + subdir;

            return Path.Combine(uploadedFilesDirectory, "P-" + projectId, subdir);
        }
    }
}
