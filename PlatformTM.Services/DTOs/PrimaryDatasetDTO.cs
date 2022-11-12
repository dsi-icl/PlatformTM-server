using System;
using System.Collections.Generic;

namespace PlatformTM.Services.DTOs
{
    public class PrimaryDatasetDTO
    {
        public PrimaryDatasetDTO()
        {
        }

        public int Id { get; internal set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
        public string DatasetType { get; set; }
        public bool IsLocked { get; set; }


        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<string> StudyNames { get; set; }
        public List<string> StudyAccronyms { get; set; }

        public string Version { get; internal set; }
        public string DateCreated { get; internal set; }
        public string DateModified { get; internal set; }


        public DatasetDescriptorDTO Descriptor { get; set; }
        public string DescriptorId { get; set; }
        public string Acronym { get; set; }
    }
}

