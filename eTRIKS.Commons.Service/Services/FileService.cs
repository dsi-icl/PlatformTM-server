using System.Collections;
using System.Configuration;
using System.Net;
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
        private string rawFilesDirectory;
        private string stdFilesDirecotry;

        public FileService(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            _fileRepository = uoW.GetRepository<DataFile, int>();
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
                fileDTO.icon = "";
                fileDTO.IsDirectory = file.IsDirectory;
                fileDTO.selected = false;
                fileDTOs.Add(fileDTO);
                fileDTO.state = file.State;
                fileDTO.DataFileId = file.Id;
            }
            return fileDTOs;
        }

        public bool addDirectory(string projectId, DirectoryInfo di, string dir)
        {
            //TODO: projectId
            var file = new DataFile();
            file.FileName = di.Name;
            //file.Path = di.FullName.Substring(di.FullName.IndexOf(projectId));
            file.Path = dir;//file.Path.Substring(0,file.Path.LastIndexOf("\\\"))
            file.DateAdded = di.CreationTime.ToLongDateString();
            file.IsDirectory = true;
            file.ProjectId = 1;

            _fileRepository.Insert(file);
            return _dataServiceUnit.Save().Equals("CREATED");
        }

        public bool addOrUpdateFile(string projectId, FileInfo fi)
        {
            //TODO: projectId
            DataFile file;
            file = _fileRepository.FindSingle(d => d.FileName.Equals(fi.Name) && d.Project.Accession.Equals(projectId));
            //TODO: add property isLoadedToDB to the file and only change status to modified if not loadedToDB
            if (file == null)
            {
                file = new DataFile();
                file.FileName = fi.Name;
                file.DateAdded = fi.CreationTime.ToLongDateString();
                file.State = "New";
                file.Path = fi.DirectoryName.Substring(fi.DirectoryName.IndexOf(projectId));
                file.IsDirectory = false;
                _fileRepository.Insert(file);
                file.ProjectId = 1;
            }
            else
            {
                file.LastModified = fi.LastWriteTime.ToLongDateString();
                _fileRepository.Update(file);
                if (file.LoadedToDB)
                    file.State = "Modified";
            }
            return _dataServiceUnit.Save().Equals("CREATED");
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

            DataTable dt = new DataTable();
            foreach (string field in header)
            {
                dt.Columns.Add(field.Replace("\"","") ,typeof(string));
            }

            while (true)
            {
                var row = parser.Read();
                if (row == null)
                    break;

                DataRow dr = dt.NewRow();
                if (row.Length == 0 || row.Length != dt.Columns.Count) return null;

                for (int i = 0; i < row.Length; i++)
                {
                    dr[i] = row[i];
                }
                dt.Rows.Add(dr);
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

        public string writeDataFile(string studyId, DataTable dt)
        {
            //TODO: ADD STUDY ID as subfolder
            string path = ConfigurationManager.AppSettings["FileDirectory"];
            path += studyId;

            if (!Directory.Exists(path + "\\Mapped"))
                Directory.CreateDirectory(path + "\\Mapped");

            string strFilePath = path+"\\Mapped\\"+dt.TableName+".csv";
            
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
            return strFilePath;
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
    }
}
