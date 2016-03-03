using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Core.Domain.Model.Users.Datasets
{
    public class UserDataset
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Tags { get; set; }
        public string Owner { get; set; }
        public string ProjectAccession { get; set; }
        public List<Criterion> ExportCriteria { get; set; }

    }

    public class Query
    {
        public List<FilterExact> ExactFilters { get; set; } //will contain the topic variable filter+any other qo2 e.g. aesev
        public List<FilterRange> RangeFilters { get; set; } //a qo2 with numeric values e.g. VSORRES
    }

    public class Criterion
    {
        public string O3 { get; set; }
        public List<FilterExact> ExactFilters { get; set; } //will contain the topic variable filter+any other qo2 e.g. aesev
        public List<FilterRange> RangeFilters { get; set; } //a qo2 with numeric values e.g. VSORRES
        public List<string> Projection { get; set; }

        public Criterion()
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
