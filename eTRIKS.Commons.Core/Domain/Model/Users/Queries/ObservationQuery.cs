using System;
using System.Collections.Generic;

namespace eTRIKS.Commons.Core.Domain.Model.Users.Queries
{
    public abstract class Query
    {
        public string QueryObjectName { get; set; }
        public string Entity { get; set; } //Study //SubjectCharacteristics //
        public string DataType { get; set; }
        public int ProjectId { get; set; }
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
                    return string.Join(",", FilterExactValues);
                if (FilterRangeFrom != 0 && FilterRangeTo != 0)
                    return FilterRangeFrom + " -> " + FilterRangeTo;
                return "";
            }
        }  
    }
    public class ObservationQuery : Query
    {
        public string TermName { get { return base.QueryObjectName; } set { base.QueryObjectName = value; } } //O3   EntityName  Age/BMI     ObservationName INJECTION SITE PAIN
        public int TermId { get; set; } //O3id   EntityId //REUSE ID for ontology entryID (such as AEDECOD as well as DBtopicID
        public string PropertyName { get; set; } //QO2  (results/occurance)
        public string PropertyLabel { get; set; }
        public int PropertyId { get; set; } //QO2id    PropertyId
        //public string DataType { get; set; }
       // public string ObservationObjectType { get; set; } //Single //Multiple //OntologyEntry
        public string ObservationName => TermName + (PropertyName != null?" [" + PropertyName + "] ":"");

        //TEMP properties for CVterms until we have real OEs
        public bool IsOntologyEntry { get; set; }
        public string TermCategory { get; set; } //AEDECOD //AESOC AEHLG
        
        public string ObservationObjectShortName { get; set; }
        public string Group { get; set; }
    }
    public class GroupedObservationsQuery : ObservationQuery
    {
        public string GroupedObsName
        {
            get { return base.QueryObjectName; }
            set { base.QueryObjectName = value; }
        }
        public List<ObservationQuery> GroupedObservations { get; set; }
        public string Name { get; set; }
    }




}
