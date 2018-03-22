using System;
using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Base;

namespace eTRIKS.Commons.Core.Domain.Model
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
