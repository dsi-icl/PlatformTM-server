using System;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.Users.Datasets
{
	public class ExportFileDTO
    {
		public string Id { get; set; }
		public string Name { get; set; }
		public string OwnerId { get; set; }
		public string ContentType { get; set; }
		public string QueryId { get; set; }
        public int SubjectCount { get; set; }
        public int SampleCount { get; set; }
		public int ProjectId { get; set; }
        public int FileStatus { get; set; }
    }
}
