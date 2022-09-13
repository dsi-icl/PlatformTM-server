/************************************************************/
/********   Created By: Dilshan Silva 12-12-2014   **********/
/********             DTO for Dataset              **********/
/************************************************************/

using System;
using System.Collections.Generic;

namespace PlatformTM.Models.DTOs
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
        public int ProjectId { get; set; }
        public string ProjectStrAcc { get; set; }
        public int ActivityId { get; set; }
        public FileDTO DataFileDTO { get; set; }

        //public List<FileDTO> DataFileDtos { get; set; } 
        public string StandardDataFile { get; set; }
        public string State { get; set; }
        public List<DatasetVariableDTO> Variables { get; set; }
        public List<DatasetVariableDTO> GenericFields { get; set; }
        public List<DatasetVariableDTO> HeaderFields { get; set; }
        public bool IsHeaderIncluded { get; set; }
        public bool IsNew { get; set; }
        public bool HasHeader { get; set; }

        public DatasetDTO()
        {
            Variables = new List<DatasetVariableDTO>();
            GenericFields = new List<DatasetVariableDTO>();
            HeaderFields = new List<DatasetVariableDTO>();
        }
    }

    public class DatasetVariableDTO
    {
        public string varType;
        public int Id { get; set; }
        public string Accession { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public bool? IsCurated { get; set; }
      
        public string RoleId { get; set; }
        public Nullable<int> OrderNumber { get; set; }
        public Nullable<bool> IsRequired { get; set; }
        public Nullable<int> KeySequence { get; set; }
        public bool isSelected { get; set; }
        public string DictionaryName { get; set; }
        public string DictionaryDefinition { get; set; }
        public string DictionaryXrefURL { get; set; }
        //public DerivedMethod DerivedVariableProperties { get; set; }
        //public string DerivedVariablePropertiesId { get; set; }
        public bool IsComputed { get; set; }
        public List<ExpressionElement> ExpressionList { get; set; }
        public string UsageId { get; set; }
        public bool IsGeneric { get; set; }
        public string Section { get; set; }
        public List<string> AllowedQualifiers { get; set; }
        public string GenericFieldQualifier { get; set; }
    }
    public class ExpressionElement
    {
        public string Name { get; set; }
        public string Type { get; set; } //syntax / variable / term
        public string val { get; set; }
    }
}
