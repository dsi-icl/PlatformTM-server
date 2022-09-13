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

            try
            {
                var filepath = Path.Combine(SourceFilePath, SourceFileName);
                DataTable dt = TabularDataIO.ReadDataTable(filepath);
                ImportSourceData(dt);
            }catch(IOException _e)
            {

            }
           
        }

        public  void ImportSourceData(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                var subjectDataRow = new SubjectSrcDataRow();
                subjectDataRow.SubjectId = row[SubjectIdVariableName].ToString();
                dataTable.Columns.

                foreach (var v in sourceFile.DataVariables)
                {
                    if (v.IsDerived) continue;
                    var tempColName = v.ColumnName.Split('_')[1];
                    subjectDataRow.DataRecord[v.Identifier] = row[tempColName].ToString();
                }
                sourceFile.SubjectDataRows.Add(subjectDataRow);
            }
        }
    }
}

