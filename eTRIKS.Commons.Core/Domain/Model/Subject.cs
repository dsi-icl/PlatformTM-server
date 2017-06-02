using System.Collections.Generic;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class Subject
    {
        public ICollection<Characteristic> Characteristics { get; set; }
    }
}
