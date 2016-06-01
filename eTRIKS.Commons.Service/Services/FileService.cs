using System.Collections;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Security.Permissions;
using AutoMapper;
using AutoMapper.Internal;
using CsvHelper;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace eTRIKS.Commons.Service.Services
{
    public class FileService
    {
        private IServiceUoW _dataServiceUnit;
        private IRepository<DataFile, int> _fileRepository;
        private IRepository<Project, int> _projectRepository;
        private string rawFilesDirectory;
        private string stdFilesDirecotry;

        public FileService(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            _fileRepository = uoW.GetRepository<DataFile, int>();
            _projectRepository = uoW.GetRepository<Project, int>();
            rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
            stdFilesDirecotry = rawFilesDirectory + "\\Mapped";
        }
        public List<FileDTO> getUploadedFiles(string projectId,string path)
        {
            List<FileDTO> fileDTOs = new List<FileDTO>();
            //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");

            //List<string> files = Directory.GetFiles(path).ToList<string>();
            var files = _fileRepository.FindAll(f => f.Project.Accession.Equals(projectId) && f.Path.Equals(path));
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

        public DirectoryInfo addDirectory(string projectId, string newDir)
        {
            if (Directory.Exists(newDir))
                return new DirectoryInfo(newDir);

            
            var di = Directory.CreateDirectory(newDir);
                //_fileService.addDirectory(projectId, new DirectoryInfo(newDir), projectId);
            
           
            var project = _projectRepository.FindSingle(p => p.Accession.Equals(projectId));
            if(project ==null)
                return null;

            var file = new DataFile();
            file.FileName = di.Name;
            //file.Path = di.FullName.Substring(di.FullName.IndexOf(projectId));
            file.Path = di.Parent.FullName.Substring(di.Parent.FullName.IndexOf(projectId));//file.Path.Substring(0,file.Path.LastIndexOf("\\\"))
            file.DateAdded = di.CreationTime.ToLongDateString();
            file.IsDirectory = true;
            file.ProjectId = project.Id;

            _fileRepository.Insert(file);
            return _dataServiceUnit.Save().Equals("CREATED") ? di : null;
        }

        public DataFile addOrUpdateFile(string projectId, FileInfo fi)
        {
            //TODO: projectId
            DataFile file;
            if (fi == null)
                return null;
            string filePath = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf(projectId));
            file = _fileRepository.FindSingle(d => d.FileName.Equals(fi.Name) && d.Path.Equals(filePath) && d.Project.Accession.Equals(projectId));
            //TODO: add property isLoadedToDB to the file and only change status to modified if not loadedToDB
            if (file == null)
            {
                var project = _projectRepository.FindSingle(p => p.Accession.Equals(projectId));
                if (project == null)
                    return null;
                file = new DataFile();
                file.FileName = fi.Name;
                file.DateAdded = fi.CreationTime.ToShortDateString() + " " + fi.CreationTime.ToShortTimeString();
                file.State = "New";
                file.Path = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf(projectId));
                file.IsDirectory = false;
                _fileRepository.Insert(file);
                file.ProjectId = project.Id;
            }
            else
            {
                file.LastModified = fi.LastWriteTime.ToShortDateString() + " " + fi.LastWriteTime.ToShortTimeString();
                _fileRepository.Update(file);
                if (file.LoadedToDB)
                    file.State = "Modified";
            }
            return _dataServiceUnit.Save().Equals("CREATED") ? file : null;
        }

        #region IO methods
       
        public DataTable readStandardFile(string studyId, string fileName)
        {
            string filePath = rawFilesDirectory + studyId + "\\Mapped\\" + fileName; 
            //string filePath = stdFilesDirecotry + "\\" + fileName;
            return readDataFile(filePath);
        }

        public DataTable ReadOriginalFile(string filePath)
        {
            //string filePath = rawFilesDirectory + studyId + "\\" + fileName;
            string PATH = rawFilesDirectory + filePath;
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

            return dt;
        }

        public List<Dictionary<string, string>> getFileColHeaders(string filePath)
        {
            //Parse header of the file
            string delim = ",";
            string PATH = rawFilesDirectory + filePath;// + studyId + "\\" + fileName;
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
            reader.Close();

            return res;
        }

        public FileInfo writeDataFile(string filePath, DataTable dt)
        {
            //TODO: ADD STUDY ID as subfolder
            string path = ConfigurationManager.AppSettings["FileDirectory"];
            path +=filePath + "\\Mapped";
            string projectId = "P-BVS";
            var DirInfo = addDirectory(projectId, path);

           

            string strFilePath = DirInfo.FullName+"\\"+dt.TableName+".csv";
            
            StreamWriter writer = File.CreateText(strFilePath);

            IEnumerable<String> headerValues = dt.Columns.Cast<DataColumn>()
                .Select(column => QuoteValue(column.ColumnName));
                
            writer.WriteLine(String.Join(",", headerValues));
            IEnumerable<String> items;

            foreach (DataRow row in dt.Rows) {
                items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
                writer.WriteLine(String.Join(",", items));
            }
            writer.Flush();
            writer.Close();

            //addOrUpdateFile(studyId,new FileInfo(strFilePath));
            return new FileInfo(strFilePath);
        }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }

        #endregion

        public List<string> getDirectories(string projectId)
        {
           var dirs =  _fileRepository.FindAll(f => f.IsDirectory.Equals(true) && f.Project.Accession.Equals(projectId));
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
            ht.Add("data", sdtmTable);
            
            return ht;
        }

        public void tempmethod()
        {
            DataTable usubjids = ReadOriginalFile("temp/CRC305Dusubjids.csv");
            //DataTable cytofSamples = ReadOriginalFile("temp/CyTOFsamples.csv");
            DataTable luminexSamples = ReadOriginalFile("temp/GhentLuminexSamples_v1.csv");
            //DataTable FACSSamples = ReadOriginalFile("temp/FACSsamples_v1.csv");
            //luminexSamples.TableName = "luminexSamples";

            luminexSamples.Columns.Add("USUBJID");
            List<string> subjidlist = new List<string>();

            foreach (DataRow row in usubjids.Rows)
            {
                subjidlist.Add(row[0].ToString());
            }

            foreach (DataRow row in luminexSamples.Rows)
            {
                string subjId = row["SubjId"].ToString();
                string newsubjid = subjidlist.Find(d => d.EndsWith(subjId));
                row["USUBJID"] = newsubjid;
            }
            string path = ConfigurationManager.AppSettings["FileDirectory"];
            StreamWriter writer = File.CreateText(path + "temp\\GhentLuminexSamples_v2.csv");

            IEnumerable<String> headerValues = luminexSamples.Columns.Cast<DataColumn>()
                .Select(column => QuoteValue(column.ColumnName));

            writer.WriteLine(String.Join(",", headerValues));
            IEnumerable<String> items;

            foreach (DataRow row in luminexSamples.Rows)
            {
                items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
                writer.WriteLine(String.Join(",", items));
            }
            writer.Flush();
            writer.Close();
        }

        /**
         *To gather one column we need the following params
         *- identifier columns (columns that will remain as is
         *- gather columns (those that will be pivoted to long format
         *- name of the key column
         *- name of the value column
         */
        public void getLongFormat()
        {
            DataTable wideDataTable = ReadOriginalFile("temp/CyTOFdata_v2.csv");
            DataTable longDataTable = new DataTable();

            List<string> ids = new List<string>() { "SAMPLEID","POP","COUNT", "PERTOT"};
            List<string> gatherColumns = new List<string>();
            int gatherColumnsFrom = 7;
            int gatherColumnsTo = 111;

            List<int> countColumns = new List<int>(){1,10,19,28};

            //Retrieve dataset template for the long format file
            //identify key column and value Column
            string keyColumn = "OBSMEA", valueColumn = "OBSVALUE",
                featureColumn = "FEAT", domainColumn = "DOMAIN";

            //1- Create new table from the identifier columns + the new columns
            longDataTable.Columns.Add(domainColumn);
            foreach (var idCol in ids )
            {
                longDataTable.Columns.Add(idCol);
            }
            //longDataTable.Columns.Add(popColumn);
            //longDataTable.Columns.Add(countColumn);
            longDataTable.Columns.Add(featureColumn);
            longDataTable.Columns.Add(keyColumn);
            longDataTable.Columns.Add(valueColumn);

            foreach (DataRow inRow in wideDataTable.Rows)
            {
                

                for (int i = gatherColumnsFrom; i <= gatherColumnsTo; i++)
                {
                    DataRow longDataRow = longDataTable.NewRow();
                    foreach (var idCol in ids)
                    {
                        longDataRow[idCol] = inRow[idCol];
                    }
                    string[] keyValue = wideDataTable.Columns[i].ToString().Split('.');
                    longDataRow[keyColumn] = keyValue[0];
                    longDataRow[valueColumn] = inRow[i];
                    longDataRow[featureColumn] = keyValue[1];
                    longDataRow[domainColumn] = "CY";

                    longDataTable.Rows.Add(longDataRow);
                }
                //foreach (DataColumn col in inputDataTable.Columns)
                //{
                    
                //}
            }
           // var fileInfo = writeDataFile("temp/CyTOFdata_long.csv", longDataTable);
            string path = ConfigurationManager.AppSettings["FileDirectory"];
            StreamWriter writer = File.CreateText(path+"temp\\CyTOFdata_long.csv");

            IEnumerable<String> headerValues = longDataTable.Columns.Cast<DataColumn>()
                .Select(column => QuoteValue(column.ColumnName));

            writer.WriteLine(String.Join(",", headerValues));
            IEnumerable<String> items;

            foreach (DataRow row in longDataTable.Rows)
            {
                items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
                writer.WriteLine(String.Join(",", items));
            }
            writer.Flush();
            writer.Close();
        }

        public void getLongFormat2()
        {
            DataTable wideDataTable = ReadOriginalFile("temp/FACSdata_v2.csv");
            DataTable longDataTable = new DataTable();

            //List<string> ids = new List<string>() { "SAMPLEID","POP","COUNT", "PERTOT"};
            List<string> ids = new List<string>() { "SAMPLEID" };
            List<string> gatherColumns = new List<string>();
            int gatherColumnsFrom = 7;
            int gatherColumnsTo = 111;



            List<int> countColumns = new List<int>() { 1, 10, 19, 28 };

            //Retrieve dataset template for the long format file
            //identify key column and value Column
            string countColumn = "COUNT", keyColumn = "OBSMEA", valueColumn = "OBSVALUE",
                featureColumn = "FEAT", domainColumn = "DOMAIN", popColumn = "POPULATION";



            //1- Create new table from the identifier columns + the new columns
            longDataTable.Columns.Add(domainColumn);
            foreach (var idCol in ids)
            {
                longDataTable.Columns.Add(idCol);
            }
            longDataTable.Columns.Add(popColumn);
            longDataTable.Columns.Add(countColumn);
            longDataTable.Columns.Add(featureColumn);
            longDataTable.Columns.Add(keyColumn);
            longDataTable.Columns.Add(valueColumn);

            foreach (DataRow inRow in wideDataTable.Rows)
            {
                for(int k=0; k<countColumns.Count;k++)
                {
                    for (int i = countColumns[k] + 1; k+1==countColumns.Count?i<inRow.ItemArray.Length:i < countColumns[k+1]; i++)
                    {
                        DataRow longDataRow = longDataTable.NewRow();
                        foreach (var idCol in ids)
                        {
                            longDataRow[idCol] = inRow[idCol];
                        }
                        string[] popCountKeyValue = wideDataTable.Columns[countColumns[k]].ToString().Split('.');

                        longDataRow[popColumn] = popCountKeyValue[0];
                        longDataRow[countColumn] = inRow[countColumns[k]];

                        string[] keyValue = wideDataTable.Columns[i].ToString().Split('.');
                        longDataRow[keyColumn] = keyValue[1];
                        longDataRow[valueColumn] = inRow[i];
                        longDataRow[featureColumn] = keyValue[2];
                        longDataRow[domainColumn] = "CY";

                        longDataTable.Rows.Add(longDataRow);
                    }
                }

                
                //foreach (DataColumn col in inputDataTable.Columns)
                //{

                //}
            }
            // var fileInfo = writeDataFile("temp/CyTOFdata_long.csv", longDataTable);
            string path = ConfigurationManager.AppSettings["FileDirectory"];
            StreamWriter writer = File.CreateText(path + "temp\\FACSdata_long.csv");

            IEnumerable<String> headerValues = longDataTable.Columns.Cast<DataColumn>()
                .Select(column => QuoteValue(column.ColumnName));

            writer.WriteLine(String.Join(",", headerValues));
            IEnumerable<String> items;

            foreach (DataRow row in longDataTable.Rows)
            {
                items = row.ItemArray.Select(o => QuoteValue(o.ToString()));
                writer.WriteLine(String.Join(",", items));
            }
            writer.Flush();
            writer.Close();
        }
    }
}
