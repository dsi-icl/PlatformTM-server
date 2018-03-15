using System.Collections.Generic;
using PlatformTM.Services.DTOs;

namespace PlatformTM.Services.Services.HelperService
{
    public class Formatter
    {
        private FileService _fileService;
        public Formatter(FileService fileService)
        {
            _fileService = fileService;
        }

        public void getLongFormat()
        {
            DataTable wideDataTable = _fileService.ReadOriginalFile("temp/CyTOFdata_v2.csv");
            DataTable longDataTable = new DataTable();

            List<string> ids = new List<string>() { "SAMPLEID", "POP", "COUNT", "PERTOT" };
            List<string> gatherColumns = new List<string>();
            int gatherColumnsFrom = 7;
            int gatherColumnsTo = 111;

            List<int> countColumns = new List<int>() { 1, 10, 19, 28 };

            //Retrieve dataset template for the long format file
            //identify key column and value Column
            string keyColumn = "OBSMEA", valueColumn = "OBSVALUE",
                featureColumn = "FEAT", domainColumn = "DOMAIN";

            //1- Create new table from the identifier columns + the new columns
            longDataTable.Columns.Add(domainColumn);
            foreach (var idCol in ids)
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
           
        }

        public void getLongFormat2()
        {
            DataTable wideDataTable = _fileService.ReadOriginalFile("temp/FACSdata_v2.csv");
            DataTable longDataTable = new DataTable();

            //List<string> ids = new List<string>() { "SAMPLEID","POP","COUNT", "PERTOT"};
            List<string> ids = new List<string>() { "SAMPLEID" };
            //List<string> gatherColumns = new List<string>();
            //int gatherColumnsFrom = 7;
            //int gatherColumnsTo = 111;



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
                for (int k = 0; k < countColumns.Count; k++)
                {
                    for (int i = countColumns[k] + 1; k + 1 == countColumns.Count ? i < inRow.ItemArray.Length : i < countColumns[k + 1]; i++)
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
           
        }

        public void TransformToHD(string filename)
        {
            var headers = new List<string>() {"SPECIMEN","DOMAIN","FEAT","OBSMEA","OBSVAL","OBSVALU"};
            DataTable oriTable = _fileService.ReadOriginalFile(filename);
            DataTable newTable = new DataTable();
            newTable.TableName = "CRC305ABC_LUMINEX_DATA";
            int gatherColumnsFrom = 8, gatherColumnsTo = 28;

            //int studyIdcol = 0, sampleId = 1; 

            foreach (var header in headers)
            {
                newTable.Columns.Add(header);
            }

            var oriHeaderRow = oriTable.Columns;
            foreach (DataRow oriRow in oriTable.Rows)
            {


                for (int i = gatherColumnsFrom; i <= gatherColumnsTo; i++)
                {
                    DataRow newTablerow = newTable.NewRow();

                    newTablerow["SPECIMEN"] = oriRow[1];
                    newTablerow["DOMAIN"] = "HD";
                    newTablerow["FEAT"] = oriHeaderRow[i].Label;
                    newTablerow["OBSMEA"] = "Concentration";
                    newTablerow["OBSVAL"] = oriRow[i] ?? "";
                    newTablerow["OBSVALU"] = oriHeaderRow[i].ToString() == "CCL5" ?   "ng/ml" : "pg/ml";

                    newTable.Rows.Add(newTablerow);
                }
            }
            _fileService.WriteDataFile("", newTable);
        }
    }
}
