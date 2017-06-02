using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;
using eTRIKS.Commons.Core.JoinEntities;

namespace eTRIKS.Commons.Core.Domain.Model.Templates
{
    public class DatasetTemplateField : Identifiable<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public string Label { get; set; }
        public int Order { get; set; }

        /// <example> MAIN | TABLE | HEADER </example>
        public string Section { get; set; } //TODO: change that to placement
        public bool AllowMultipleValues { get; set; }

        /// <summary>
        /// Used to indicate fields that requiring Typing
        /// Type can be a free text term, an ontology term or a reference source
        /// Type is specified at the time of instantiating an instance of this field for a dataset template instance
        /// </summary>
        /// <example>Characteristics[] | Comment[] | Reporter Database Entry[]</example>
        public bool IsGeneric { get; set; }

        /// <summary>
        /// In case of a generic field Field[NAME], there can be a list of allowable types that can be used
        /// as name qualifiers. It doesnt matter than these terms/names can be names of databaes or come from ontologies
        /// this will be established when the template is parsed via its relevant template descriptor
        /// </summary>
        /// <example>Allowable DBs for Reporter Database Entry[REFSRC]</example>
        public Dictionary QualifiersDictionary { get; set; }
        public string QualifiersDictionaryId { get; set; }



        public DatasetTemplate Template { get; set; }
        public string TemplateId { get; set; }



        /// <summary>
        /// Originally created to map to Origin in CDISC DEFINE
        /// Deemed not importatnt to model at this stage
        /// </summary>
        /// <example>CRF | DERIVED | ASSIGNED | PROTOCOL | eDT | Predecessor</example>
        //public CVterm FieldOrigin { get; set; }
        //public string OriginTypeId { get; set; }

        /// <summary>
        /// Identifier | RecordQualifier | TimingQualifier | VariableQualifier ...
        /// </summary>
        public CVterm Role { get; set; }
        public string RoleId { get; set; }

        /// <example>
        /// Required | Expected | Permissible
        /// </example>
        public CVterm Usage { get; set; }
        public string UsageId { get; set; }

        /// <summary>
        /// A collection of permissible values for this variable 
        /// Used when a defined list of controlled vocabulary terms are predefined (e.g. SDTM codelists)
        /// </summary>
        public Dictionary ControlledVocabulary { get; set; }
        public string ControlledVocabularyId { get; set; }

        /// <summary>
        /// An external reference to a database or an ontology that values of this variable should reference
        /// Used when the values of a variable are ontology terms (EFO) or external database cross references (e.g. genbank) 
        /// </summary>
        public List<TemplateFieldDB> FieldTermSources { get; set; }
        //public string TermReferenceSourceId { get; set; }

        //HOW IS THIS USED FOR TERM SOURCE NAME of ADF
        //VS
        //REPORTER DATABASE ENTRY[<RefSource>]
        //for term source instance the allowable list of dbs would be a dictionary of terms that just happen
        //to be database names, that fact will later be identified when parsing against the ADFdescriptor
        // as
        //for the case of REPORTER DATABASE ENTRY , the allowable list of dbs will be the 
        // field term sources 
    }
}