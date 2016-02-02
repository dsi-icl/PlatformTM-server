﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Study : Identifiable<int>
    {
        //public string OID { get; set; }
        public string Accession { get; set; }
        public string Name { get; set; }
        public string Site { get; set; }
        public string Description { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        //public ICollection<Activity> Activities { get; set; }
        public ICollection<Observation> Observations { get; set; }
        public ICollection<Dataset> Datasets { get; set; }
        public ICollection<Visit> Visits { get; set; }
    }
}