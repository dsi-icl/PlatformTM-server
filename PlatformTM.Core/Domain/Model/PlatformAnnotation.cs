using System;
using System.Collections.Generic;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model
{
    public class PlatformAnnotation : Identifiable<Guid>
    {
        public string Name { get; set; }
        public string Accession { get; set; }
        public ICollection<FeatureAnnotation> FeatureAnnotations { get; set; }

        public PlatformAnnotation()
        {
            FeatureAnnotations = new List<FeatureAnnotation>();
        }
        
    }

    
}
