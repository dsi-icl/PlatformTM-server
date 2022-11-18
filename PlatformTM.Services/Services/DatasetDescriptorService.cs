using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;
using PlatformTM.Core.Domain.Model.Templates;
using PlatformTM.Models.DTOs;
using PlatformTM.Services.DTOs;

namespace PlatformTM.Models.Services
{
    public class DatasetDescriptorService
    {
        private readonly IServiceUoW _dataServiceUnit;
        private readonly IRepository<ObservationDatasetDescriptor, Guid> _DatasetDescriptorRepository;

        private readonly FileService _fileService;
        public DatasetDescriptorService(IServiceUoW uoW, FileService fileService)
        {
            _dataServiceUnit = uoW;
            _DatasetDescriptorRepository = uoW.GetRepository<ObservationDatasetDescriptor, Guid>();

            _fileService = fileService;
        }

   

        public DatasetDescriptor GetUploadedDescriptor(int projectId, string filename)
        {
            string fullpath = Path.Combine(_fileService.GetFullPath(projectId), "temp", filename);

            //string fileName = "WeatherForecast.json";
            string jsonString = File.ReadAllText(fullpath);
            var guid = Guid.NewGuid();
            
            
           // var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10,  IgnoreNullValues= true };
            ObservationDatasetDescriptor oDD = JsonSerializer.Deserialize<ObservationDatasetDescriptor>(jsonString)!;

            var oDD_dto = new DatasetDescriptorDTO(oDD);

            return oDD;

        }

        public DatasetDescriptor AddDescriptor(ObservationDatasetDescriptor dd, int projectId)
        {
            
            if (dd == null)
                return null;

            dd.ProjectId = projectId;
            dd.Id = Guid.NewGuid();


            _DatasetDescriptorRepository.Insert(dd);
            return _dataServiceUnit.Save().Equals("CREATED") ? dd : null;
            
        }

        public DatasetDescriptor GetDatasetDescriptor(string descriptorId)
        {
            var dd = _DatasetDescriptorRepository.FindSingle(d => d.Id == Guid.Parse(descriptorId));
            //var dto = new DatasetDescriptorDTO(dd);
            return dd;
        }

        public DatasetDescriptorDTO GetDatasetDescriptorDTO(string descriptorId)
        {
            var dd = _DatasetDescriptorRepository.FindSingle(d => d.Id == Guid.Parse(descriptorId));
            var dto = new DatasetDescriptorDTO(dd);
            return dto;
        }


        public List<DatasetDescriptorDTO> GetDatasetDescriptors(int projectId)
        {
            List<ObservationDatasetDescriptor> descriptors = _DatasetDescriptorRepository.FindAll(
                d => d.ProjectId == projectId).ToList();
            return descriptors.Select(s=> new DatasetDescriptorDTO(s)).ToList();
        }

        public void UpdateDescriptor(ObservationDatasetDescriptor descriptor, int projectId)
        {
            var studyToUpdate = _DatasetDescriptorRepository.Get(descriptor.Id);

            //check that the owner of this dataset is the caller
            //var dataset = ReadDTO(dto);
            //var datasetToUpdate = _DatasetDescriptorRepository.FindSingle(d => d.Id == dataset.Id);
            //datasetToUpdate.LastModified = DateTime.Today.ToString("f");
            //datasetToUpdate.Description = dataset.Description;

            _DatasetDescriptorRepository.Update(descriptor);
        }


    }
}
