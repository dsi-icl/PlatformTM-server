using System.Collections.Generic;

namespace eTRIKS.Commons.Service.DTOs.Explorer
{
    public class ObservationRequestDTO
    {
        public string QueryFor { get; set; }
        public string QueryBy { get; set; }
        public string QueryWhereProperty { get; set; }
        public string QuerySelectProperty { get; set; }

        public string O3 { get; set; }
        public int O3id { get; set; }
        public string O3code { get; set; }
        public string Id => O3id + (QO2id!=0 ? "_"+QO2id : "");
        public string Name => (O3code + (QO2 != null ? "[" + QO2 + "]" : "")).ToLower();

        public int ProjectId { get; set; }
        public string O3variable { get; set; }
        public bool IsMultipleObservations { get; set; }
        public List<int> TermIds { get; set; }

        public List<ObservationRequestDTO> GroupedObservations;

        
        public string OntologyEntryCategoryName { get; set; }
        public string OntologyEntryValue { get; set; }
        
        public string QO2 { get; set; }
        public int QO2id { get; set; }
        public string DataType { get; set; }
        public string QO2_label { get; set; }

        //TYPES
        public bool IsEvent { get; set; }
        public bool IsFinding { get; set; }
        public bool IsOntologyEntry { get; set; }
        public bool IsSubjectCharacteristics { get; set; }
        public bool IsClinicalObservations { get; set; }
        public bool IsDesignElement { get; set; }
        public bool IsFeature { get; set; }
        public bool IsSampleCharacteristic { get; set; }
        
        public string DesignElementType { get; set; }

        //FILTERS
        public List<string> FilterExactValues { get; set; } //the set of values selected by the user
        public float FilterRangeFrom { get; set; } //the from value selected by user
        public float FilterRangeTo { get; set; } //the to value selected by user
        public bool IsFiltered { get; set; }
        public string Group { get; set; }

        public ObservationRequestDTO()
        {
            TermIds = new List<int>();
        }
    }

    
}
