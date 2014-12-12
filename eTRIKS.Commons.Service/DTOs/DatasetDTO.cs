/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/********             DTO for Dataset              **********/
/************************************************************/
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
        public string Activity { get; set; }
        public string Domain { get; set; }
    }

}
