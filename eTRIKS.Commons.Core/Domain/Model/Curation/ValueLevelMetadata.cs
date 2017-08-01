using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.Curation
{
    public class ValueLevelMetadata
    {
        public string ValueListOid { get; set; }    // The unique identifier of the valuelevel metadata set.
        public string ValueName { get; set; }       // The specific valuelevel item to which the valuelevel metadata applies.
        public string Type { get; set; }            // This should be text, integer, float, datetime, date or time
        public string Length { get; set; }
        public string Lable { get; set; }           // The label of the valuelevel item
        public string SignificantDigits { get; set; }  // If the type=float, then this should be populated with the integer representing the number of decimal places for the item.
        public string Origin { get; set; }             // This text should describe where the item comes from (for example, CRF page 5, Central laboratory data, or even derived.
        public string Comment { get; set; }            // This is a free text field where you can put any comment about the item.
        public string DisplayFormat { get; set; }      // This is the display format for numeric items
        public string CodelistName { get; set; }
        public string Mandatory { get; set; }
        
    }
}
