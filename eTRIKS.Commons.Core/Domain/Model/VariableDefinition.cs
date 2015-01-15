using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Core.Domain.Model
{
    public class VariableDefinition : Identifiable<int>
    {
       
        //public string variableId { get; set; }
        public string Accession { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public Nullable<bool> IsCurated { get; set; }
        public CVterm VariableType { get; set; }
        public string VariableTypeId { get; set; }
        public CVterm Role { get; set; }
        public string RoleId { get; set; }
        public Study Study { get; set; }
        public string StudyId { get; set; }

        //public DerivedMethod DerivedVariableProperties { get; set; }
        //public string DerivedVariablePropertiesId { get; set; }

        
    }
}