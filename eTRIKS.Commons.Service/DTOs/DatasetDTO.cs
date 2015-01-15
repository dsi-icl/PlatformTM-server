/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/********             DTO for Dataset              **********/
/************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.ControlledTerminology;

namespace eTRIKS.Commons.Service.DTOs
{
    public class DatasetDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Structure { get; set; }
        public string DomainId { get; set; }
        public string StudyId { get; set; }
        public int ActivityId { get; set; }
        public List<DatasetVariableDTO> variables { get; set; }

        public DatasetDTO()
        {
            variables = new List<DatasetVariableDTO>();
        }
    }

    public class DatasetVariableDTO
    {
        public int Id { get; set; }
        public string Accession { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        //public Nullable<bool> IsCurated { get; set; }
        //public CVterm VariableType { get; set; }
        //public string VariableTypeId { get; set; }
        //public CVterm Role { get; set; }
        //public string RoleId { get; set; }
        //public Study study { get; set; }
        //public string StudyId { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public Nullable<bool> IsRequired { get; set; }
        public Nullable<int> KeySequence { get; set; }
        public bool isSelected { get; set; }
        public string DictionaryName { get; set; }
        public string DictionaryDefinition { get; set; }
        public string DictionaryXrefURL { get; set; }
        //public DerivedMethod DerivedVariableProperties { get; set; }
        //public string DerivedVariablePropertiesId { get; set; }
    }
}
