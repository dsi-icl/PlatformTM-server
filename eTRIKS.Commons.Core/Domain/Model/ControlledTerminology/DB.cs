using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model.ControlledTerminology
{
    public class DB : Identifiable<string>
    {
        //public string OID { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string Url { get; set; }
        public string UrlPrefix { get; set; }
        

    }
}