using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Service.DTOs
{
    public class DatasetDTO
    {
        public string OID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
