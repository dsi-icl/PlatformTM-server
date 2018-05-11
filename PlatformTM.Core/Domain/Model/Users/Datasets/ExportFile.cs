using System;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.Users.Datasets
{
	public class ExportFile : Identifiable<Guid>
    {
		public string Name { get; set; }
		public string OwnerId { get; set; }
		public string ContentType { get; set; }
		public string QueryId { get; set; }
        public int SubjectCount { get; set; }
        public int SampleCount { get; set; }
		public int ProjectId { get; set; }
        public int FileStatus { get; set; }
        public string ExportFileURI { get; set; }

        public ExportFile()
        {
        }
    }
}
