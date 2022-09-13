using System;
using System.Collections.Generic;

namespace PlatformTM.Core.Domain.Model.BMO
{
    public class Feature : Observable
    {
        public string Name { get; set; }
        public List<Property> FeatureProperties { get; set; }
        public Feature()
        {
        }
    }
}

