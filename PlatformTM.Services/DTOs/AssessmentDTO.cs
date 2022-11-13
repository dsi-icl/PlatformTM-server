using System;
using System.Collections.Generic;

namespace PlatformTM.Services.DTOs
{
    public class AssessmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int StudyId { get; set; }

        public List<AssessmentDatasetDTO> AssociatedDatasets { get; set; }

        public AssessmentDTO()
        {
            AssociatedDatasets = new();
        }
    }

    public class AssessmentDatasetDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
        public bool IsSelected { get; set; }
        public string Acronym { get; set; }
    }
}

