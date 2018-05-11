using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.Users.Datasets
{
	public class AnalysisDataset : Identifiable<Guid>
    {
		public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public string OwnerId { get; set; }
        public int ProjectId { get; set; }
		public List<string> FileIds { get; set; }
        public AnalysisDataset()
        {
			FileIds = new List<string>();
        }
    }
}
