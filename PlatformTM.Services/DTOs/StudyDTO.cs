using System.Collections.Generic;

namespace PlatformTM.Models.DTOs
{
    public class StudyDTO
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Accession { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectAcc { get; set; }
        public int ArmCount { get; set; }
        public IEnumerable<string> ArmNames { get; set; }

       
    }
}