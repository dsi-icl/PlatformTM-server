using eTRIKS.Commons.Core.Domain.Model.Base;
using System.Collections.Generic;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class DataFile : Identifiable<int>
    {
        public string FileName { get; set; }
        public string DataType { get; set; }
        public List<string> Tags { get; set; }
        public string DateAdded { get; set; }
        public string LastModified { get; set; }
        public string State { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public ICollection<Dataset> Datasets { get; set; }
        public bool IsStandard { get; set; } 
        public string Path { get; set; }
        public bool IsDirectory { get; set; }

        public bool LoadedToDB { get; set; }
    }
}
