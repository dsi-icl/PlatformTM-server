using System.Collections.Generic;
using PlatformTM.Services.DTOs;

namespace PlatformTM.Services.LoadingWizard.DTO
{
    public class StudyDatasetsDTO
    {
        public string StudyName { get; set; }
        public string StudyTitle { get; set; }


        public List<AssessmentDTO> StudyAssessments { get; set; }


    }
}