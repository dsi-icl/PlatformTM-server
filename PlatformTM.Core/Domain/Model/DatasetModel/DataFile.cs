using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Core.Domain.Model.DatasetModel
{
    public class DataFile : Identifiable<int>
    {
        public string FileName { get; set; }
        public string DataType { get; set; }
        //public List<string> Tags { get; set; }
        public string DateAdded { get; set; }
        public string LastModified { get; set; }
        public string State { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }

        public DataFile Folder { get; set; }
        public int? FolderId { get; set; }
        /*Consider putting this back when support for many-tp-many relationship is available*/
        //public ICollection<Dataset> Datasets { get; set; }

        public ICollection<DatasetDatafile> Datasets { get; set; }
        public bool IsStandard { get; set; } 
        public string Path { get; set; }
        public bool IsDirectory { get; set; }
        public string Format { get; set; }

        public bool IsLoadedToDB { get; set; }
    }
}
