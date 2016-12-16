using System.Collections.Generic;

namespace eTRIKS.Commons.Core.Domain.Model.Users.Queries
{
    public class ObservationQuery
    {
        public string ObservarionObject { get; set; } //O3   EntityName  Age/BMI     ObservationName
        public int ObservarionObjectId { get; set; } //O3id   EntityId
        public string ObservarionQualifier { get; set; } //QO2  PropertyId (results/occurance)
        public int? ObservarionQualifierId { get; set; } //QO2id    PropertyId
        public string DataType { get; set; }

        public List<string> FilterExactValues { get; set; } //the set of values selected by the user
        public float FilterRangeFrom { get; set; } //the from value selected by user
        public float FilterRangeTo { get; set; } //the to value selected by user
        public bool IsFiltered { get; set; }
    }
}
