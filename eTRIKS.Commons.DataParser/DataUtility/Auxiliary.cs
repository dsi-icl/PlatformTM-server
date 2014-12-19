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

      
    }
}
