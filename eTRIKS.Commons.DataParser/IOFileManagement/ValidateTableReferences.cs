using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Service.Services;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.DataParser.IOFileManagement
{
    class ValidateTableReferences
    {
        private TemplateService _templateService;
        public ValidateTableReferences(TemplateService templateService)
        {
            _templateService = templateService;
        }

        public void checkTemplateDataFK(DataSet ds)
        {
            //check for the Role
            if (!checkAndInsertFKInDictionary("Role"))
            {
                int cvTermCounter = 1;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string role = ds.Tables[0].Rows[i][5].ToString().Trim();
                    cvTermCounter = checkAndInsertFKInCVTerm("CL-Role", role, cvTermCounter);
                }
            }

            //check for the Usage
            if (!checkAndInsertFKInDictionary("Compliance"))
            {
                int cvTermCounter = 1;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    string cmpliance = ds.Tables[0].Rows[i][6].ToString().Trim();
                    cvTermCounter = checkAndInsertFKInCVTerm("CL-Compliance", cmpliance, cvTermCounter);
                }
            }
        }

        public bool checkAndInsertFKInDictionary(string FK)
        {
            string ditionaryItem = _templateService.checkDictionaryItem("CL-" + FK);
            //Load Data to dictionary and CV term
            if (ditionaryItem == null)
            {
                Dictionary dictionaryItem = new Dictionary();
                dictionaryItem.OID = "CL-" + FK;
                dictionaryItem.Name = FK;
                dictionaryItem.Definition = FK;
                dictionaryItem.XrefId = null;
                _templateService.addDictionaryItem(dictionaryItem);
                return false;
            }
            return true;
        }

        public int checkAndInsertFKInCVTerm(string dictionaryId, string FK, int cvTermCounter)
        {
            string existInCvterm = _templateService.getOIDOfCVterm(FK);
            //Load Data to dictionary and CV term
            if (existInCvterm == null)
            {
                CVterm cvTerm = new CVterm();
                cvTerm.OID = dictionaryId + "-T-" + cvTermCounter;
                cvTerm.Synonyms = null;
                cvTerm.Name = FK;
                cvTerm.Definition = FK;
                cvTerm.DictionaryId = dictionaryId;
                cvTerm.XrefId = null;
                cvTerm.IsUserSpecified = false;
                _templateService.addCVTerm(cvTerm);
                cvTermCounter++;
            }
            return cvTermCounter;
        }


    }
}
