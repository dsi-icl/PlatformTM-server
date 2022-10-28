using System;
using System.Data;

namespace Loader.MapperModels.SourceDataModels
{
    public class SourceDataFile
    {
        
        public List<SourceDataVariable> DataVariables { get; set; }
        public List<SubjectSrcDataRow> DataRows { get; set; }
        public string SourceFileName { get; internal set; }
        public string SourceFilePath { get; set; }
        public string SubjectIdVariableName { get; set; }

        public SourceDataFile(string _srcFileName, string _srcFilePath, string subjectIdVariable)
        {
            SourceFileName = _srcFileName;
            SourceFilePath = _srcFilePath;
            SubjectIdVariableName = subjectIdVariable;
            DataVariables = new();
            DataRows = new();

            
           
        }

        public  void ReadSourceDataFile()
        {
            try
            {
                var filepath = Path.Combine(SourceFilePath, SourceFileName);
                DataTable dt = TabularDataIO.ReadDataTable(filepath);

                foreach (DataRow row in dt.Rows)
                {
                    SubjectSrcDataRow subjectDataRow = new SubjectSrcDataRow();
                    subjectDataRow.SubjectId = row[SubjectIdVariableName].ToString();

                    foreach (var v in DataVariables)
                    {
                        if (v.IsDerived) continue;
                        //var tempColName = v.ColumnName.Split('_')[1];
                        subjectDataRow.DataRecord[v.Identifier] = row[v.Name].ToString();
                    }
                    DataRows.Add(subjectDataRow);
                }

            }
            catch (IOException _e)
            {

            }
        }

        public List<string>? GetSubjectIds()
        {
           return DataRows.Select(r => r.SubjectId)?.ToList();
        }
    }
}

