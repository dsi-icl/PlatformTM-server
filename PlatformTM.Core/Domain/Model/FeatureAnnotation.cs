using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model
{
    public class FeatureAnnotation : Identifiable<Guid>
    {
        public string FeatureId { get; set; }
        public string Name { get; set; }
        public ICollection<NV> Properties { get; set; }

        public FeatureAnnotation()
        {
            Properties = new List<NV>();
        }
    }

    public class NV
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}

