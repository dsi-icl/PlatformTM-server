using System.Collections.Generic;

namespace PlatformTM.Core.Domain.Model
{
    public class Subject
    {
        public ICollection<Characteristic> Characteristics { get; set; }
    }
}
