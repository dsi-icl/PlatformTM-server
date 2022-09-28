using System;
using System.Data;
using System.Globalization;
using CsvHelper;

namespace Loader
{
    public class TabularDataIO
    {
        

        public static DataTable ReadDataTable(string filePath)
        {
            DataTable dt = new ();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                using var dr = new CsvDataReader(csv);
                dt.Load(dr);
            }

            return dt;
        }

        public static void WriteDataFile(string filePath, DataTable dt)
        {

            using (var textWriter = File.CreateText(filePath))
            using (var csv = new CsvWriter(textWriter, CultureInfo.InvariantCulture))
            {
                // Write columns
                foreach (DataColumn column in dt.Columns)
                {
                    csv.WriteField(column.ColumnName);
                }
                csv.NextRecord();

                // Write row values
                foreach (DataRow row in dt.Rows)
                {
                    for (var i = 0; i < dt.Columns.Count; i++)
                    {
                        csv.WriteField(row[i]);
                    }
                    csv.NextRecord();
                }
            }

            //var dirPath = Path.Combine(_downloadFileDirectory, path);
            //var di = Directory.CreateDirectory(dirPath);
            //if (!di.Exists) return null;
            //var filePath = Path.Combine(dirPath, dt.TableName + ".csv");


            //StreamWriter writer = File.CreateText(filePath);

            //var headerValues = dt.Columns.Cast<DataColumn>()
            //    .Select(column => QuoteValue(column.ColumnName));

            //writer.WriteLine(string.Join(",", headerValues));

            //foreach (DataRow row in dt.Rows)
            //{
            //    var items = row.ItemArray.Cast<object>().Select(o => QuoteValue(o.ToString()));
            //    writer.WriteLine(string.Join(",", items));
            //}
            //writer.Flush();
            //writer.Dispose();
            //return new FileInfo(filePath);
        }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }
    }
}

