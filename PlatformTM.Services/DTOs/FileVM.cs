using System;
using System.Collections.Generic;

namespace PlatformTM.Models.DTOs
{
    public class FileVM
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string DataType { get; set; }
        public List<string> Tags { get; set; }
        public string DateAdded { get; set; }
        public string DateLastModified { get; set; }
        public string Path { get; set; }
        //public string Icon { get; set; }
        public FileVM()
        {
            Tags = new List<string>();
        }
    }
}
