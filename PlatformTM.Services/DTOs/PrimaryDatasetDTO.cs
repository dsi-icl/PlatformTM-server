using System;
namespace PlatformTM.Services.DTOs
{
    public class PrimaryDatasetDTO
    {
        public PrimaryDatasetDTO()
        {
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string ProjectName { get; set; }
        public string StudyName { get; set; }
        public string StudyAccronym { get; set; }
        public string DatasetType { get; set; }
    }
}

