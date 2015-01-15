using System;
using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Base;
using System.Runtime.Serialization;


namespace eTRIKS.Commons.Core.Domain.Model.Templates
{
    public class DomainTemplate : Identifiable<string>
    {

        //public string OID { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Structure { get; set; }
        public Boolean IsRepeating { get; set; }

        public ICollection<DomainVariableTemplate> Variables { get; private set; }
    }

}