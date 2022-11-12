using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;
using PlatformTM.Core.JoinEntities;

namespace PlatformTM.Core.Domain.Model.DatasetModel
{
    public class DataFile : Versionable<int>
    {
        public string FileName { get; set; }
        public string Type { get; set; }
        public string Format { get; set; }
        public string Size { get; set; }

        //public string DateAdded { get; set; }
        //public string LastModified { get; set; }
        public string State { get; set; }
        public string Path { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int? FolderId { get; set; }
        public DataFile Folder { get; set; }
        
        public int DatasetId;
        public PrimaryDataset Dataset { get; set; }

        public int AssessmentId { get; set; }
        public Assessment Assessment { get; set; }
        

        //public ICollection<DatasetDatafile> Datasets { get; set; }
        //public bool IsStandard { get; set; } 
        
        public bool IsDirectory { get; set; }
        public bool IsLoadedToDB { get; set; }
    }
}
