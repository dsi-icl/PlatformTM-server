using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace eTRIKS.Commons.Service.DTOs
{
    public class ClinicalDataTreeDTO
    {
        public string Class;
        public ICollection<GroupNode> Activities { get; set; }
        public ICollection<GroupNode> Domains { get; set; }

        public ClinicalDataTreeDTO()
        {
            Activities = new List<GroupNode>();
            Domains = new List<GroupNode>();
        }
    }

    //public class DomainNode:GenericNode
    //{
    //    //public string Name;
    //    public string Domain;
    //    //public string code;
    //    public List<GenericNode> Terms;
    //    public int Count;
    //    public DomainNode()
    //    {
    //        Terms = new List<GenericNode>();
    //    }
    //}

    [KnownType(typeof(ObservationNode))]
    [KnownType(typeof(GroupNode))]
    [KnownType(typeof(MedDRATermNode))]
    public class GenericNode
    {
        public string Name { get; set; }
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
        public ObservationRequestDTO DefaultObservation;
        public List<ObservationRequestDTO> Qualifiers;
    }

    [KnownType(typeof(MedDRAGroupNode))]
    public class GroupNode : GenericNode
    {
        public int Count { get; internal set; }
        public bool IsDomain { get; internal set; }

        public List<GenericNode> Terms;
        public GroupNode()
        {
            Terms = new List<GenericNode>();
        }


    }

    public class MedDRAGroupNode : GroupNode
    {
        public string Variable { get; set; }
        public string GroupTerm { get; set; }
        public List<int> TermIds { get; set; }
        public bool IsSelectable { get; set; }
        public ObservationRequestDTO DefaultObservation { get; set; }
        public List<string> TermNames { get; internal set; }
        public string Group { get; internal set; }
        public List<ObservationRequestDTO> Qualifiers { get; internal set; }

        public MedDRAGroupNode()
        {
            Terms = new List<GenericNode>();
            IsSelectable = true;
        }
    }

    public class MedDRATermNode : GenericNode
    {
        public ObservationRequestDTO DefaultObservation;
        public List<ObservationRequestDTO> Qualifiers;
        public string Variable { get; set; }
    }

}
