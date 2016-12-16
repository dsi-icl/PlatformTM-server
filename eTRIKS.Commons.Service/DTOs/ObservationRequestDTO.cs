using System.Collections.Generic;

namespace eTRIKS.Commons.Service.DTOs
{
    public class ObservationRequestDTO
    {
        private string _id;
        private string _O3code;
        public string Id
        {
            get { return O3id.ToString() + (QO2id!=null ? "_"+QO2id.ToString():""); }
            //get { return _O3code+ (QO2!=null? "["+QO2+"]":"");}
            set { _id = value; }
        }
        public string Name
        {
            //get { return (O3id + QO2id ?? QO2id).ToString(); }
            get { return _O3code + (QO2 != null ? "[" + QO2 + "]" : ""); }
            set { _id = value; }
        }

        public string O3 { get; set; }
        public int O3id { get; set; }
        public string O3variable { get; set; }
        public bool IsMultipleObservations { get; set; }
        public List<int> TermIds { get; set; }
        public bool IsEvent { get; set; }
        public bool IsFinding { get; set; }
        public bool IsOntologyEntry { get; set; }
        public string O3code
        {
            get { return _O3code; } 
            set { _O3code = value.ToLower(); } 
        }
        public string QO2 { get; set; }
        public int? QO2id { get; set; }
        public string DataType { get; set; }
        public string QO2_label { get; internal set; }



        public bool IsSubjectCharacteristics { get; set; }
        public bool IsClinicalObservations { get; set; }
        public List<string> FilterExactValues { get; set; } //the set of values selected by the user
        public float FilterRangeFrom { get; set; } //the from value selected by user
        public float FilterRangeTo { get; set; } //the to value selected by user
        public bool IsFiltered { get; set; }

        //public bool IsAssayParameters { get; set; }
    }
}
