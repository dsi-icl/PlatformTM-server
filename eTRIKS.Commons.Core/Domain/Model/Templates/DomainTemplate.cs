using System;
using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Base;
using System.Runtime.Serialization;


namespace eTRIKS.Commons.Core.Domain.Model.Templates
{

    //[KnownType(typeof(DomainTemplateVariable))]
    //[DataContract]
    public class DomainDataset : Identifiable<string>
    {

        //public string OID { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Structure { get; set; }
        public Boolean IsRepeating { get; set; }


        public virtual ICollection<DomainTemplateVariable> Variables { get; set; }
    }

}