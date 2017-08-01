using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model.Curation
{
    public class SingleColumn : Identifiable<Guid> 

    {
   
    public headerElement colHeader;
    public List<object> colValues;

    public struct headerElement
    {
        public string ColumnName;  
        public int DataFileId;
        public string Type;
       // public TypeValue ColumnType; // Type of the values in the column
    }
        public enum TypeValue
        {
            String,
            Num,
            Date,
            Time
        }
    }
}
