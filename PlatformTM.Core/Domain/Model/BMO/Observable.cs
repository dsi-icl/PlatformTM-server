using System;
using PlatformTM.Core.Domain.Model.Base;

namespace PlatformTM.Core.Domain.Model.BMO
{
    public abstract class Observable : Identifiable<int>
    {
        public string Name { get; set; }
        public string ControlledTerm { get; set; }
        public string TermURI { get; set; }

        public Observable()
        {
        }
    }
}

