namespace eTRIKS.Commons.Service.DTOs
{
    public class ObservationRequestDTO
    {
        private string _id;
        private string _O3code;
        public string Id
        {
            get { return _O3code+ (QO2!=null? " ["+QO2+"]":"");}
            set { _id = value; }
        }

        public string O3 { get; set; }
        public int O3id { get; set; }
        public string O3variable { get; set; }

        public string O3code
        {
            get { return _O3code; } 
            set { _O3code = value.ToLower(); } 
        }

        public string QO2 { get; set; }
        public int? QO2id { get; set; }

    }


}
