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

        public IList<StudyDTO> Studies { get; set; }
        public IList<ActivityDTO> Activities { get; set; }
    }
}