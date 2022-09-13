using System;
using System.Collections.Generic;

namespace PlatformTM.Models.DTOs
{
    public class DatasetVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<FileVM> Files {get;set;}
        public DatasetVM()
        {
            Files = new List<FileVM>();
        }
    }
}
