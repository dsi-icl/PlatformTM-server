using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.ControlledTerminology;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Core.Domain.Model.DatasetModel
{
    public class VariableDefinition : Identifiable<int>
    {
       
        public string Accession { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public bool? IsCurated { get; set; }
        public bool IsGeneric { get; set; }
        public string NameQualifier { get; set; }
        //if field is a generic field with square brackets then what is in the brackets gets 
        //replace with the name
        //the type (what's between the brackets) has no effect on the VALUES for this variable
        //when the user is 'instantiating' a 
        //Characteristics[<Name>] or a ParamterValue[<Name>] or a Factor[<Name>] or a Comment[<Name>]

        //the <Name> parameter can be a free text string or a controlled vocabulary
        //in the case of Factor[<Name>] and Parameter[<Name>]
        //The <Name> paramter is referring to a previously defined term in IDF or Investigation
        //that can in itself be a free text term or a CVterm

            //in case of Reporter Database Entry[<Name>]
            //Name parameter gets its value from the form
            //CVtermerefencesource gets set also

            //

        public string VariableTypeStr { get; set; } //TEMP until OLS is setup
        public bool? IsComputed { get; set; } //TEMP until OLS is setup
        public CVterm VariableType { get; set; } //STANDARD | SUBMITTED | DERIVED
        public string VariableTypeId { get; set; }

        /// <example> MAIN | TABLE | HEADER </example>
        public string Section { get; set; }
        public bool AllowMultipleValues { get; set; }

        /// <summary>
        /// A collection of permissible values for this variable 
        /// Used when a defined list of controlled vocabulary terms are predefined (e.g. SDTM codelists)
        /// </summary>
        public Dictionary CVtermDictionary { get; set; }
        public string CVtermDictionaryId { get; set; }

        /// <summary>
        /// An external reference to a database or an ontology that values of this variable should reference
        /// Used when the values of a variable are ontology terms (EFO) or external database cross references (e.g. genbank) 
        /// </summary>
        /// <remarks>Could be updated to a list if values can come from different ontologies</remarks>
        public DB CVTermReferenceSource { get; set; }
        public string CVTermReferenceSourceId { get; set; }

        /// <summary>
        /// A role determines the type of information conveyed by the variable
        /// </summary>
        /// <example> Identifier | Topic | Timing | GroupingQualifier | RecordQualifier | Synonym Qualifier | Variable Qualifier</example>
        public CVterm Role { get; set; }
        public string RoleId { get; set; }

        public Project Project { get; set; }
        public int ProjectId { get; set; }

        /// <summary>
        /// Other Fields that qualify information in this field
        /// </summary>
        /// <example>Value Unit | Value High Range | Value Low Range | </example>
        public List<VariableQualifier> VariableQualifiers { get; set; }

        public string ComputedVarExpression { get; set; }

        //public Data.DescriptorType DescriptorType { get; set; }
    }
}