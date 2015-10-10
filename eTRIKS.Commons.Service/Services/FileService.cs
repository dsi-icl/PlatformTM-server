using System.Net;
using AutoMapper.Internal;
using CsvHelper;
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
                fileDTO.dateAdded = Finfo.CreationTime.ToLongDateString();
                fileDTO.icon = "";
                fileDTO.selected = false;
                fileDTOs.Add(fileDTO);
            }
            return fileDTOs;
        }


        public string transformFile(string filePath, DataTemplateMap map)
        {
            DataTable inputDataTable = readDataFile(filePath);
            DataTable sdtmTable = new DataTable();

            var varMaps = new List<DataTemplateMap.VariableMap>();

            foreach (var varMap in map.VarTypes.SelectMany(variableType => variableType.Value.Where(varMap => varMap.DataType != null)))
            {
                sdtmTable.Columns.Add(varMap.ShortName, Type.GetType(varMap.DataType));
                varMaps.Add(varMap);
            }


            for(int i=0;i<map.TopicColumns.Count;i++)
                 

            foreach (DataRow inputRow in inputDataTable.Rows) // Loop over the rows.
            {
                DataRow sdtmRow = sdtmTable.NewRow();
                foreach (var varMap in varMaps)
                {
                    
                    if (varMap.MapToStringValueList != null && varMap.MapToStringValueList[i] != null)
                        sdtmRow[varMap.ShortName] = varMap.MapToStringValueList[i];
                    else if (varMap.MapToColList != null && varMap.MapToColList[i] != null)
                    {
                        var colName = varMap.MapToColList[i].colName;
                        sdtmRow[varMap.ShortName] = inputRow[colName];
                    }
                    else
                    {
                        sdtmRow[varMap.ShortName] = sdtmRow[varMap.ShortName].ToNullSafeString();
                    }
                }
                sdtmTable.Rows.Add(sdtmRow);
            }
            return "";
        }

        private DataTable readDataFile(string filePath)
        {
            StreamReader reader = File.OpenText(filePath);
            //var csv = new CsvReader(reader);

            
            var parser = new CsvParser(reader);
            
            string[] header = parser.Read();
            DataTable dt = new DataTable();
            foreach (string field in header)
            {
                dt.Columns.Add(field, typeof(string));
            }  

            while (true)
            {
                var row = parser.Read();

                if (row == null)
                    break;
                
                DataRow dr = dt.NewRow();
                if (row.Length == 0 || row.Length != dt.Columns.Count) return null;

                for (int i = 0; i < row.Length; i++){
                    dr[i] = row[i];
                }

                dt.Rows.Add(dr);  
            }

            return dt;
        }

        public class Myclass
        {
            
        }


        public string[] LinkFile(string filePath, string activityId)
        {

            return null;

        }

        public List<Dictionary<string, string>> getFileColHeaders(string filePath)
        {
            //Parse the header of the file

            StreamReader reader = File.OpenText(filePath);

            var parser = new CsvParser(reader);
            string[] header = parser.Read();
            List<Dictionary<string,string>> res = new List<Dictionary<string, string>>();
            for (int i = 0; i < header.Length; i++)
            {
                var r = new Dictionary<string, string>();
                r.Add("colName",header[i]);
                r.Add("pos",i.ToString());
                res.Add(r);
            }



            return res;

        }


    }
}
