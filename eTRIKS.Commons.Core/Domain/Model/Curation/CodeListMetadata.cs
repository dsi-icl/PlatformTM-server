using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.Curation
{
    public class CodeListMetadata
    {
        public string CodeListName { get; set; }
        public string Rank { get; set; }
        public string CodedValue { get; set; }  
        public string Translated { get; set; }
        public string Type { get; set; }
        public string CodeListDictionary { get; set; } 
        public string CodelistVersion { get; set; } 
        public string SourceDataset { get; set; }
        public string SourceVariable { get; set; }
        public string SourceValue { get; set; }
        public string SourceType { get; set; }
    }
}
