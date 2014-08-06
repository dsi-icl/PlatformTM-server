using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eTRIKSService.Models
{
    public class ActivityDataVariable
    {
        public int orderNumber { get; set; }
        public bool mandatory { get; set; }
        public int keySequence { get; set; }
        public Activity activity { get; set; }
        public DomainVariable usedVariable { get; set; }
    }
}