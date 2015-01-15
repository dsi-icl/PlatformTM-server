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
        public string StudyID { get; set; }
        public ICollection<DatasetBriefDTO> datasets { get; set; }
        //public Dictionary<string,string> datasets { get; set; }
    }

    public class DatasetBriefDTO
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
