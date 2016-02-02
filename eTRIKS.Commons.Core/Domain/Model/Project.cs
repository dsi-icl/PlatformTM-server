using eTRIKS.Commons.Core.Domain.Model.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Project : Identifiable<int>
    {
        public string Name { get; set;}
        public string Description { get; set;}
        public string Accession { get; set;}
        public ICollection<Study> Studies { get; set;}
        public ICollection<DataFile> DataFiles { get; set; } 

        public Project()
        {
            Studies = new List<Study>();
        }

        public ICollection<Activity> Activities { get; set; }
    }
}
