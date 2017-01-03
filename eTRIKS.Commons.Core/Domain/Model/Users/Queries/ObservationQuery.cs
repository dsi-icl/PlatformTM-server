using System;
using System.Collections.Generic;

namespace eTRIKS.Commons.Core.Domain.Model.Users.Queries
{
    public class ObservationQuery
    {
        public string ObservationObject { get; set; } //O3   EntityName  Age/BMI     ObservationName
        public int ObservationObjectId { get; set; } //O3id   EntityId
        public string ObservationQualifier { get; set; } //QO2  (results/occurance)
        public int ObservationQualifierId { get; set; } //QO2id    PropertyId
        public string DataType { get; set; }

        public List<string> FilterExactValues { get; set; } //the set of values selected by the user
        public float FilterRangeFrom { get; set; } //the from value selected by user
        public float FilterRangeTo { get; set; } //the to value selected by user
        public bool IsFiltered { get; set; }

        public string FilterText
        {
            get
            {
                if (!IsFiltered)
                    return "";
                if (DataType == "string")
                    return string.Join(",",FilterExactValues);
                if (FilterRangeFrom != 0 && FilterRangeTo != 0)
                    return FilterRangeFrom + " -> " + FilterRangeTo;
                return "";
            }
            //set { _id = value; }
        }

        public string ObservationObjectShortName { get; set; }
    }
}
