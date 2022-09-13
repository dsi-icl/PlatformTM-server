using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Models.DTOs.Export
{
	public class ExportFileDefinition
    {
        public string Name { get; set; }
        public string OwnerId { get; set; }
		public int ProjectId { get; set; }
        public string ContentType { get; set; }
        //public int SubjectCount { get; set; }
        //public int SampleCount { get; set; }
        public string ExportFileURI { get; set; }
        // 0 is not prepared; 1 is preparing, 2 prepared
        public int FileStatus { get; set; }
        public bool IsSaved { get; set; }

        public Guid QueryId { get; set; }

		public List<DatasetField> Fields { get; set; }

        public ExportFileDefinition()
        {
			Fields = new List<DatasetField>();
        }
    }
}
