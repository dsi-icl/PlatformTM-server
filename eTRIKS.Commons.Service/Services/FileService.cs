using System.Collections;
using System.Configuration;
using System.Net;
using AutoMapper;
using AutoMapper.Internal;
using CsvHelper;
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
        private string rawFilesDirectory;
        private string stdFilesDirecotry;

        public FileService()
        {
            rawFilesDirectory = ConfigurationManager.AppSettings["FileDirectory"];
            stdFilesDirecotry = rawFilesDirectory + "\\Mapped";
        }
        public List<FileDTO> getUploadedFiles(string path)
        {
            List<FileDTO> fileDTOs = new List<FileDTO>();
            //string PATH = HttpContext.Current.Server.MapPath("~/App_Data");

            //List<string> files = Directory.GetFiles(path).ToList<string>();
            DirectoryInfo f = new DirectoryInfo(path);
            foreach (var Finfo in f.GetFiles())
            {
                FileDTO fileDTO = new FileDTO();
                fileDTO.FileName = Finfo.Name;
                fileDTO.dateAdded = Finfo.LastWriteTime.ToLongDateString();
                fileDTO.icon = "";
                fileDTO.selected = false;
                fileDTOs.Add(fileDTO);
                fileDTO.state = "NEW";
            }
            return fileDTOs;
        }

        //public FileDTO getDatasetFileInfo(int datasetId)
        //{
        //    FileDTO fileDTO = new FileDTO();

        //}

        public DataTable readStandardFile(string studyId, string fileName)
        {
            string filePath = rawFilesDirectory + studyId + "\\Mapped\\" + fileName; 
            //string filePath = stdFilesDirecotry + "\\" + fileName;
            return readDataFile(filePath);
        }

        public DataTable ReadOriginalFile(string studyId, string fileName)
        {
            string filePath = rawFilesDirectory + studyId + "\\" + fileName;
            return readDataFile(filePath);
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

        public List<Dictionary<string, string>> getFileColHeaders(string studyId, string fileName)
        {
            //Parse header of the file
            string delim = ",";
            string PATH = rawFilesDirectory + studyId + "\\" + fileName;
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


        
    }
}
