using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Users.Queries;

namespace eTRIKS.Commons.Core.Domain.Model.Users.Datasets
{
    public class DatasetField
    {
        public Query QueryObject { get; set; }
        public string ColumnHeader { get; set; }
        public bool ColumnHeaderIsEditable { get; set; } = true;
        private string _name;
        public string FieldName
        {
            get
            {
                if (QueryObject != null)
                {
                    //if (QueryObjectType == nameof(SubjectCharacteristic))
                    //    return "SubjectCharacteristics" + QueryObject.QueryObjectName + "";
                    ////********************************************************************************************************************************************************

                    //if (QueryObjectType == nameof(SampleCharacteristic))
                    //    return "SampleCharacteristics" + QueryObject.QueryObjectName + "";
                    ////********************************************************************************************************************************************************
                    if (QueryObject is ObservationQuery)
                    {
                        return ((ObservationQuery)QueryObject).TermName + (((ObservationQuery)QueryObject).PropertyLabel != null ? "" + ((ObservationQuery)QueryObject).PropertyLabel + "" : "");

                    }
                    return QueryObject.QueryFor + "[" + QueryObject.QuerySelectProperty + "]";

                }
                    
                return _name;
            }
            set { _name = value; }
        }
        public string QueryObjectType { get; set; }

        public DatasetField()
        {
            //QueryObjects = new List<ObservationQuery>();
            ColumnHeader = FieldName;
        }
    }
}
