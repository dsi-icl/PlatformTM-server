using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.SDTMHelper
{
    public class FileHandler
    {
        //----------------------------------------------------------READING SDTM DOMAIN FILES

        // The File Handler classes implement the transfer of data between domain files on the file level and structured data in the so-called Data Staging Area.
        // By providing methods for different file formats with the same call signature, the File Handler can easily be implemented to work with a variety of
        // underlying file formats other than SAS Transport.

        public struct SingleColumn
        {
            public headerElement colHeader;
            public List<object> colValues;
        }

        /// <summary>
        /// Generalized column header
        /// </summary>
        public struct headerElement
        {
            public string ColumnName; // Variable Name in SDTM
            public int DataFileId; // Variable Name in SDTM


            public string ColumnLabel; // Variable Label from SDTM
            public string ColumnRole; // Variable Role from SDTM                       // you aded this
            public TypeValue ColumnType;// Type of the values in the column
            public CoreUsage ColumnCore;// Value of Core column in SDTM
            public bool Controlled; // True, for controlled terminology
        }
        public enum TypeValue
        {
            Num,
            Char,
            ISO_DateTime
        }
        public enum CoreUsage
        {
            Required,
            Expected,
            Permissible
        }


        // is the following shgould be here??????????????????????????????????????????????????????????

        /// <summary>
        /// data structure for a whole dataset
        /// </summary>
        public struct dataSet
        {
            public string domainName;
            public List<SingleColumn> dataMatrix;
        }


        //Data Storage Area is created by using a list of data sets and adding the required metadata

        /// <summary>
        /// CDISC-Helper Data Staging Area class definition
        /// </summary>
        public struct DSA_Handle
        {
            public string standardName;
            public List<dataSet> DataStage;
        }
    }
}
