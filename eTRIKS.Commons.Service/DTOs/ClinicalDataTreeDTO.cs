using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs
{
    public class ClinicalDataTreeDTO
    {
        public string Class;
        public ICollection<ClinicalDataTreeActivityDTO> Activities { get; set; }

        public ClinicalDataTreeDTO()
        {
            Activities = new List<ClinicalDataTreeActivityDTO>();
        }
    }

    public class ClinicalDataTreeActivityDTO
    {
        public string Name;
        public string Domain;
        public string code;
        public ICollection<string> Observations;
        public ClinicalDataTreeActivityDTO()
        {
            Observations = new List<string>();
        }
    }

    public class ClinicalDataTreeRecordSummary
    {
        public string Class;
        public string Name;
        public string Domain;
        public string code;
        public string variableDefinition;
    }
}
