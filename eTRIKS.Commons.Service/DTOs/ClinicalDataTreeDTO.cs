using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

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
        public ICollection<ClinicalDataTreeObs> Observations;
        public ClinicalDataTreeActivityDTO()
        {
            Observations = new List<ClinicalDataTreeObs>();
        }
    }

    [KnownType(typeof(Observation))]
    [KnownType(typeof(ObservationGroup))]
    public class ClinicalDataTreeObs
    {
        public string Name {get;set;}
        public string Code { get; set; }
    }

    public class Observation : ClinicalDataTreeObs
    {
        public string DomainCode;
        public string KeyVariable;
        public string ValueVariable;
    }

    public class ObservationGroup : ClinicalDataTreeObs
    {

        public ICollection<Observation> Observations;

        public ObservationGroup()
        {
            Observations = new List<Observation>();
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
