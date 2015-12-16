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
        public ICollection<DomainNode> Activities { get; set; }
        public ICollection<DomainNode> Domains { get; set; }

        public ClinicalDataTreeDTO()
        {
            Activities = new List<DomainNode>();
            Domains = new List<DomainNode>();
        }
    }

    public class DomainNode
    {
        public string Name;
        public string Domain;
        public string code;
        public List<GenericNode> Terms;
        public int Count;
        public DomainNode()
        {
            Terms = new List<GenericNode>();
        }
    }

    [KnownType(typeof(ObservationNode))]
    [KnownType(typeof(GroupNode))]
    [KnownType(typeof(MedDRAGroupNode))]
    [KnownType(typeof(MedDRATermNode))]
    public class GenericNode
    {
        public string Name {get;set;}
        public int Id { get; set; }
        public string Code { get; set; }

        public override bool Equals(Object obj)
        {
            GenericNode nodeObj = obj as GenericNode;
            if (nodeObj == null)
                return false;
            else
                return Name.Equals(nodeObj.Name);
        }
    }

    public class ObservationNode : GenericNode
    {
        //public string DomainCode;
        //public string KeyVariable;
        //public string ValueVariable;
        public ObservationRequestDTO DefaultObservation;
        public List<ObservationRequestDTO> Qualifiers;

        //public string DefaultQualifier { get; set; }
    }

    public class GroupNode : GenericNode
    {

        public List<GenericNode> Terms;

        public GroupNode()
        {
            Terms = new List<GenericNode>();
        }
    }

    public class MedDRAGroupNode : GenericNode
    {
         public string Variable {get;set;}
         public string GroupTerm {get;set;}
         public int Count {get;set;}
         public List<int> TermIds { get; set; }
         public IList<GenericNode> Terms { get; set; }

         public MedDRAGroupNode()
         {
             Terms = new List<GenericNode>();
         }
    }

    public class MedDRATermNode : GenericNode
    {
        public ObservationRequestDTO DefaultObservation;
        public List<ObservationRequestDTO> Qualifiers;
        public string Variable { get; set; }
    }

}
