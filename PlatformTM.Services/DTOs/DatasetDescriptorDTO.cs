using System;
using System.Collections.Generic;
using System.Linq;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetFieldTypes;

namespace PlatformTM.Services.DTOs
{
    public class DatasetDescriptorDTO
    {
        public string Title { set; get; }
        public string Description { get; set; }
        public string Id { get; set; }
        
        public string DatasetType { get; set; }

        public List<DatasetFieldDescriptorDTO> Fields { get; set; }
        public List<IdentifierField> IdentifierFields { get; set; }
        public List<ClassifierFieldType> ClassifierField { get; set; }
        public DesignationField DesignationField { get; set; }
       

        public DatasetDescriptorDTO()
        {
            Fields = new List<DatasetFieldDescriptorDTO>();
        }

        public DatasetDescriptorDTO(ObservationDatasetDescriptor oDD)
        {
            Id = oDD.Id.ToString();
            Title = oDD.Title;
            Description = oDD.Description;
            DatasetType = oDD.DatasetType.ToString();
            Fields = oDD.GetDatasetFields().Select(f => new DatasetFieldDescriptorDTO()
            {
                Role = nameof(f),
                Name = f.Name,
                Label = f.Label,
                Description = f.Description,
                FieldType = f.GetType().Name
               
            }).ToList();

            
        }
    }

    public class DatasetFieldDescriptorDTO
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string FieldType { get; set; }
        public string Role { get; internal set; }
    }
}

