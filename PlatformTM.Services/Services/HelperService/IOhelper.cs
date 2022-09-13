using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlatformTM.Models.DTOs;

namespace PlatformTM.Models.Services.HelperService
{
    public static class IOhelper
    {
        public static void WriteDataTable(DataTable sourceTable, TextWriter writer, bool includeHeaders)
        {
            if (includeHeaders)
            {
                IEnumerable<String> headerValues = sourceTable.Columns
                    .OfType<DataColumn>()
                    .Select(column => QuoteValue(column.ColumnName));

                writer.WriteLine(String.Join(",", headerValues));
            }

            IEnumerable<String> items = null;

            foreach (DataRow row in sourceTable.Rows)
            {
                 items = row.Values.Cast<object>().Select(o => QuoteValue(o.ToString()));
                writer.WriteLine(string.Join(",", items));
                //items = row.ItemArray.Select(o => QuoteValue(o?.ToString() ?? String.Empty));
                //writer.WriteLine(String.Join(",", items));
            }

            writer.Flush();
        }

        private static string QuoteValue(string value)
        {
            return String.Concat("\"",
            value.Replace("\"", "\"\""), "\"");
        }
    }
}
