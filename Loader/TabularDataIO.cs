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
    }
}

