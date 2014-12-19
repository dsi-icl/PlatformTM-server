using eTRIKS.Commons.Core.Domain.Model.Templates;
using eTRIKS.Commons.DataParser.DataUtility;
using eTRIKS.Commons.DataParser.MongoDBAccess;
using eTRIKS.Commons.Service.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.DataParser.IOFileManagement
{
    public class FileHandler
    {
        private TemplateService _templateService;

        public FileHandler(TemplateService templateService)
        {
            _templateService = templateService;
        }
        

        public string loadDataFromFile(string dataSource, DataSet ds)
        {
            string status = string.Empty;
            string currentOID = string.Empty;
            List<string> successfullyInsertedRecords = new List<string>();

            if (dataSource == "DomainTemplate")
            {
                //Remove Column Headers
                DataRow row = ds.Tables[0].Rows[0];
                ds.Tables[0].Rows.Remove(row);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    try
                    {
                        currentOID = ds.Tables[0].Rows[i][0].ToString().Trim();
                        DomainTemplate dt = new DomainTemplate();
                        dt.OID = ds.Tables[0].Rows[i][0].ToString().Trim();
                        dt.Name = ds.Tables[0].Rows[i][4].ToString().Trim();
                        dt.Class = ds.Tables[0].Rows[i][1].ToString().Trim();
                        dt.Description = ds.Tables[0].Rows[i][6].ToString().Trim();
                        dt.Code = ds.Tables[0].Rows[i][5].ToString().Trim();
                        dt.Structure = ds.Tables[0].Rows[i][7].ToString().Trim();
                        _templateService.addDomainTemplate(dt);

                        successfullyInsertedRecords.Add(ds.Tables[0].Rows[i][0].ToString().Trim());
                    }
                    catch(Exception e)
                    { 
                        status = "Line: " + (i + 1) + ", OID: " + currentOID + ":" + e.InnerException.Message;
                        // CLEAN UP: Delete the successfully inserted records to preserve Atomicity
                        cleanUpPartialInserts(successfullyInsertedRecords, dataSource);
                        return status;
                    }
                }
            }
            else if (dataSource == "DomainVariableTemplate")
            {
                //Remove Column Headers
                DataRow row = ds.Tables[0].Rows[0];
                ds.Tables[0].Rows.Remove(row);
                //Check for FKs
                //checkTemplateDataFK(ds);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)   
                {
                    try
                    {
                        currentOID = ds.Tables[0].Rows[i][0].ToString().Trim();
                        DomainVariableTemplate dvt = new DomainVariableTemplate();
                        dvt.OID = currentOID;
                        dvt.Name = ds.Tables[0].Rows[i][1].ToString().Trim();
                        dvt.Label = ds.Tables[0].Rows[i][2].ToString().Trim();
                        dvt.Description = ds.Tables[0].Rows[i][3].ToString().Trim();
                        dvt.DataType = ds.Tables[0].Rows[i][4].ToString().Trim();
                        dvt.RoleId = _templateService.getOIDOfCVterm(ds.Tables[0].Rows[i][5].ToString().Trim());
                        dvt.UsageId = _templateService.getOIDOfCVterm(ds.Tables[0].Rows[i][6].ToString().Trim());
                        dvt.controlledTerminologyId = ds.Tables[0].Rows[i][7].ToString().Trim();
                        dvt.DomainId = ds.Tables[0].Rows[i][8].ToString().Trim();

                        successfullyInsertedRecords.Add(ds.Tables[0].Rows[i][0].ToString().Trim());
                    }
                    catch (Exception e)
                    {
                        status = "Line: " + (i + 1) + ", OID: " + currentOID + ":" + status;
                        // CLEAN UP: Delete the successfully inserted records to preserve Atomicity
                        cleanUpPartialInserts(successfullyInsertedRecords, dataSource);
                        return status;
                    }
                }
            }
           
            else if (dataSource == "NOSQL")
            {
                // undo the remove first line
                MongoDbDataServices ms = new MongoDbDataServices();
                NoSQLRecord record = new NoSQLRecord();

                //string[] headers = ds.Tables[0].Rows[0][0].ToString().Split(new Char[] { ' ', ',', '.', ':', '\t' });
                string[] headers = ds.Tables[0].Rows[0][0].ToString().Split(new Char[] { '\t' });
                for (int i = 1; i < ds.Tables[0].Rows.Count; i++)
                {
                    record.recordItem.Clear();
                    string[] rowElements = ds.Tables[0].Rows[i][0].ToString().Split(new Char[] { '\t' });

                    for (int j = 0; j < headers.Length; j++)
                    {
                        RecordItem recordItem = new RecordItem();
                        recordItem.fieldName = headers[j];
                        recordItem.value = rowElements[j];
                        record.recordItem.Add(recordItem);
                    }
                    ms.loadDataGeneric(record);
                }
            }
            return status;
        }

        //  CLEAN UP
        public void cleanUpPartialInserts(List<string> successfullyInsertedRecords, string dataSource)
        {
            if (dataSource == "DomainTemplate")
            {
                for (int j = 0; j < successfullyInsertedRecords.Count; j++)
                {
                    //daAPI.deleteTemplateDomain(successfullyInsertedRecords[j]);
                }
            }
            else if (dataSource == "DomainVariableTemplate")
            {
                for (int j = 0; j < successfullyInsertedRecords.Count; j++)
                {
                    //daAPI.deleteTemplateDomainVariable(successfullyInsertedRecords[j]);
                }
            }
        }
    }
}
