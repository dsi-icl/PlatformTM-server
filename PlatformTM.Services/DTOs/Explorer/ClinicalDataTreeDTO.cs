using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PlatformTM.Models.DTOs.Explorer
{
    public class ClinicalExplorerDTO
    {
        public Guid Id;
        public List<ClinicalDataTreeDTO> Classes { get; set; }
        public int ProjectId { get; set; }

        public ClinicalExplorerDTO()
        {
            Classes = new List<ClinicalDataTreeDTO>();
        }
    }
    public class ClinicalDataTreeDTO
    {
        public string Class;
        //public ICollection<GroupNode> Activities { get; set; }
        public ICollection<GroupNode> Domains { get; set; }

        public ClinicalDataTreeDTO()
        {
            //Activities = new List<GroupNode>();
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
    //[KnownType(typeof(ObservationGroupNode))]
    public class GenericNode
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string Code { get; set; }
        public bool IsSelectable { get; set; }
    }

    public class ObservationNode : GenericNode
    {
        public ObservationRequestDTO DefaultObservation { get; set; }
        public List<ObservationRequestDTO> Qualifiers { get; set; }
    }

    //public class ObservationGroupNode : GenericNode
    //{
    //    public ObsRequestGroupDTO DefaultObservation;
    //    public List<ObsRequestGroupDTO> Qualifiers;
    //}

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
        public ObservationRequestDTO DefaultObservation { get; set; }
        //public List<ObservationRequestDTO> Qualifiers { get; internal set; }
        //public string Group { get; internal set; }

        public MedDRAGroupNode()
        {
            Terms = new List<GenericNode>();
            IsSelectable = true;
        }
    }



    //public class MedDRATermNode : GenericNode
    //{
    //    public ObservationRequestDTO DefaultObservation;
    //    public List<ObservationRequestDTO> Qualifiers;
    //    //public string Variable { get; set; }
    //}

}
