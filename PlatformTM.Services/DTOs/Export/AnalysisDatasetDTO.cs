using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;
using PlatformTM.Core.Domain.Model.Users.Datasets;

namespace PlatformTM.Models.DTOs.Export
{
	public class AnalysisDatasetDTO : Identifiable<Guid>
    {
		public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public string OwnerId { get; set; }
        public int ProjectId { get; set; }

		public string QueryId { get; set; }
		public List<ExportFileDTO> Files { get; set; }
		public AnalysisDatasetDTO()
        {
			Files = new List<ExportFileDTO>();
        }
    }
}
