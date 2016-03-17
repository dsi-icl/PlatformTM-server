using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs
{
    public class AssayDTO
    {
        public int AssayId { get; set; }
        public string Platform { get; set; }
        public string Technology { get; set; }
        public string Measurement { get; set; }
        public string Name { get; set; }
    }
}
