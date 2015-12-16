using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs
{
    public class ExportRequestDTO
    {
        public Criteria SubjectCriteria { get; set; }
        public Criteria ClinicalCriteria { get; set; }
        public Criteria SampleCriteria { get; set; }

        public class Criteria
        {
            public List<FilterExact> ExactFilters { get; set; }
            public List<FilterRange> RangeFilters { get; set; }
            public List<string> Projection { get; set; }

            public Criteria()
            {
                ExactFilters = new List<FilterExact>();
                RangeFilters = new List<FilterRange>();
                Projection = new List<string>();
            }

           
        }

        [KnownType(typeof(FilterRange))]
        [KnownType(typeof(FilterExact))]
        public class Filter 
        {
            public string Field { set; get; }
        }

        
        public class FilterRange : Filter
        {
            public Range Range { get; set; }
        }
        public class FilterExact : Filter
        {
            public List<string> Values { get; set; }

            public FilterExact()
            {
                Values = new List<string>();
            }
        }

        public class Range
        {
            public int Upperbound { get; set; }
            public int Lowerbound { get; set; }
        }
    }
}
