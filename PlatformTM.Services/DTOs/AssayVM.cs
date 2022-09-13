using System;
using System.Collections.Generic;

namespace PlatformTM.Models.DTOs
{
    public class AssayVM
    {
        public int Id { get; set; }
        public string Platform { get; set; }
        public string Technology { get; set; }
        public string Type { get; set; }
        public string Design { get; set; }
        public string Name { get; set; }
        public List<DatasetVM> Datasets {get;set;}
        public AssayVM()
        {
            Datasets = new List<DatasetVM>();
        }
    }
}
