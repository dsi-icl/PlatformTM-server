using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.DataParser.DataUtility
{
    class Auxiliary
    {
        public string processDictionaryIdForTemplateVariable(string input)
        {
            if (input.Contains('('))
            {
                if (!input.Contains(':')) //to filter out (Fsdtm-1-3:Classifier.RequiredVariable)
                {
                    char[] removeParanthesis = { '(', ')' };
                    return "CL-" + input.Replace("\nISO 3166", "").Trim(removeParanthesis);
                }
            }
            return null;
        }

        public string getIdOfCVTerm(string field)
        {
            if (field.Length < 1)
                return null;
            //IGenericDataService<CVterm_TBL> chkExist = new DataManagementService<CVterm_TBL>();

            //CVterm_TBL Cvterm = chkExist.GetSingleOptimised(o => o.Name.Equals(field));

            ////CVterm_TBL Cvterm = chkExist.GetSingle(o => o.Name.Equals(field));
            //if (Cvterm != null)
            //{
            //    return Cvterm.OID.ToString();
            //}
            return "Record Not Found";
        }
    }
}
