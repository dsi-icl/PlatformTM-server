﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class CharacteristicObject : Identifiable<int>
    {
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string Domain { get; set; }
        public CVterm ControlledTerm { get; set; }
        public string CVtermId { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }

    }
}
