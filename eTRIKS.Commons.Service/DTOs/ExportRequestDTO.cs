using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;

namespace eTRIKS.Commons.Service.DTOs
{
    public class ExportRequestDTO
    {
        public Criterion SubjectCriteria { get; set; }
        public Criterion ClinicalCriteria { get; set; }
        public Criterion SampleCriteria { get; set; }

        

        
    }
}
