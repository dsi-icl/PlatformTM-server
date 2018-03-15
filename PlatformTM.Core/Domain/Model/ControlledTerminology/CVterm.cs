using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.ControlledTerminology
{
    public class CVterm : Identifiable<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Definition { get; set; }
        //public int? OrdinalValue { get; set; }
        public bool? IsUserSpecified { get; set; }

        /// <summary>
        /// A comma-separated list of synonyms
        /// </summary>
        public string Synonyms { get; set; }
        
        /// <summary>
        /// An instance of reference to an equivalent concept in an external database
        /// </summary>
        public virtual Dbxref Xref { get; set; }
        public string XrefId { get; set; }

        /// <summary>
        /// The collection of terms that this term belongs to
        /// Allegedly this dictionary is associated to a template field its values are controlled by it
        /// </summary>
        public virtual Dictionary Dictionary { get; private set; }
        public string DictionaryId { get; set; }

    }
}