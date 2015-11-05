using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class DatasetFile : Identifiable<int>
    {
        public string FileName { get; set; }
        public string dataType { get; set; }
        public List<string> tags { get; set; }
        public string dateAdded { get; set; }
        public string icon { get; set; }
        public bool selected { get; set; }
        public string state { get; set; }
        public string fileType { get; set; }
        public char delimiter { get; set; }
        public bool isStandard { get; set; }
        public bool isMapped { get; set; }
    }
}
