/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/********             DTO for VarDef               **********/
/************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTRIKS.Commons.Service.DTOs
{
    public class VariableDefinitionDTO
    {
        public string OID { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public Nullable<bool> IsCurated { get; set; }
        public string VariableType { get; set; }
        public string VariableTypeId { get; set; }
        public string Role { get; set; }
        public string RoleId { get; set; }
        public string Study { get; set; }
        public string StudyId { get; set; }
    }
}
