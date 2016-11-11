using System.Collections.Generic;

namespace eTRIKS.Commons.Service.DTOs
{
    public class ProjectDTO
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Accession { get; set; }
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public List<string> Users { get; set; }
        public string Type { get; set; }
        public string Desc { get; set; }

        public IList<StudyDTO> Studies { get; set; }
        public IList<ActivityDTO> Activities { get; set; }
        public int SubjectCount { get; set; }
        public int CohortCount { get; set; }
        public int StudyCount { get; set; }
    }
}