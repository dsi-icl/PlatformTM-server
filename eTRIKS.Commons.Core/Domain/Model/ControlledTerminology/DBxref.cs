namespace eTRIKS.Commons.Core.Domain.Model.ControlledTerminology
{
    /// <summary>
    /// Should eventually be removed to be replaced by entries in OLS
    /// </summary>
    public class Dbxref
    {
        public string OID { get; set; }
        public string Accession { get; set; }
        public string Description { get; set; }

        public string DBId { get; set; }
        public DB DB { get; set; }
    }
}