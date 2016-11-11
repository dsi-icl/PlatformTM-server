﻿using System;
using System.Collections.Generic;
using eTRIKS.Commons.Core.Domain.Model.Base;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Core.Domain.Model.DatasetModel
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
        public string VariableTypeStr { get; set; } //TEMP until OLS is setup
        public CVterm VariableType { get; set; }
        public string VariableTypeId { get; set; }
        public CVterm Role { get; set; }
        public string RoleId { get; set; }
        public Project Project { get; set; }
        public int ProjectId { get; set; }
        public List<VariableDefinition> VariableQualifiers { get; set; }

        public bool? IsComputed { get; set; }
        public string ComputedVarExpression { get; set; }
        //public List<VariableDefinition> Synonyms { get; set; } 

        //public DerivedMethod DerivedVariableProperties { get; set; }
        //public string DerivedVariablePropertiesId { get; set; }



        //public Data.DescriptorType DescriptorType { get; set; }
    }
}