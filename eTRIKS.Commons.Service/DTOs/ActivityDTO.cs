/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/********             DTO for Activity             **********/
/************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs
{
    public class ActivityDTO
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectAcc { get; set; }
        public ICollection<DatasetDTO> datasets { get; set; }

        //Assay properties
        public bool isAssay { get; set; }
        public string AssayDesignType { get; set; }
        public string AssayTechnology { get; set; }
        public string AssayTechnologyPlatform { get; set; }
        public string AssayMeasurementType { get; set; }
        public ActivityDTO()
        {
            datasets = new List<DatasetDTO>();
        }
    }

    public class DatasetBriefDTO
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string DomainId {get; set; }
        
    }
}
