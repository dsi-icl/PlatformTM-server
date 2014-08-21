using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKSService.Models
{
    public class Dataset
    {
        public string datasetId { get; set; }
        public string dataFile { get; set; }
        public string activityId { get; set; }

        public virtual Activity Activity { get; set; }
    }
}