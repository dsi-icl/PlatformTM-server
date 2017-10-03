using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Timing;

namespace eTRIKS.Commons.Core.Domain.Model.Curation
{
    public class ColumnMapper
    {
        public string SourceHeader { get; set; }
        public int DataFileId { get; set; }
        public string Type { get; set; }   // string, date, time

        // info from the questions  ******************************************************************************

        // identifiers
        public bool IsIdentifier { get; set; }
        public bool IsStudyId { get; set; }
        public bool IsSubjectId { get; set; }
        
        // date and time
        public bool IsDate { get; set; }
        public bool IsTime { get; set; }
        public bool IsCollectionDateTime { get; set; } //Date of Collection 
        public bool IsCollectionStudyDay { get; set; } //--DY
        public bool IsCollectionStudyTimePoint { get; set; } //--TPT 
        
        //VISIT varaibles
        public string IsVisitName { get; set; } 
        public int IsVisitNum { get; set; }
        public int IsVisitPlannedStudyDay { get; set; }
         
        // link to other columns
        public bool IsLinked { get; set; } 
        public bool IsUnitRequired { get; set; }
        public string AssociatedUnitColumn { get; set; }
        public string UserDefinedUnit { get; set; }
        public List<string>LinkedColumns { get; set; } 

        // info from the codelist query filled by CurationDTO ******************************************************************************
        public bool IsMapped { get; set; }
        public string MappedDomain { get; set; }
        public string MappedVariable { get; set; }
        public string MappedVariableSynonym { get; set; }
        public string MappedVariablePreferedTerm { get; set; }
        public string RoleId { get; set; }
        public string Class { get; set; }
        public string CodeList { get; set; }

    }
}
