using System.Collections.Generic;

namespace PlatformTM.Services.DTOs
{
    public class FileDTO
    {
        public string FileName { get; set; }
        public string dataType { get; set; }
        public List<string> tags { get; set; }
        public string dateAdded { get; set;}
        public string icon { get; set; }
        public bool selected { get; set; }
        public string state { get; set; }

        public List<Dictionary<string, string>> columnHeaders { get; set; }

        public bool templateMatched { get; set; }

        public bool IsDirectory { get; set; }
        public int DataFileId { get; set; }
        public bool IsStandard { get; set; }

        public string dateLastModified { get; set; }

        public string path { get; set; }
        public bool templateAlmostMatched { get; internal set; }
        public int percentMatched { get; internal set; }
        public List<string> unmappedCols { get; internal set; }
        public bool IsLoaded { get; set; }
        public int PercentLoaded { get; set; }
    }
}
