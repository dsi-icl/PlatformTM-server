using System;
using System.IO;
using System.Text.Json;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS.DatasetDescriptorTypes;
using PlatformTM.Models.Configuration;

namespace PlatformTM.Services.Services.Loading.PDS
{
    public class PDSLoader
    {


        private readonly IServiceUoW _dataServiceUnit;

        private readonly IRepository<PrimaryDataset, int> _pdsRepository;
        private readonly IRepository<DataFile, int> _fileRepository;
        private readonly IRepository<ObservationDatasetDescriptor, Guid> _DatasetDescriptorRepository;

        private FileStorageSettings ConfigSettings { get; set; }

        public PDSLoader(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            _pdsRepository = uoW.GetRepository<PrimaryDataset, int>();
            _fileRepository = uoW.GetRepository<DataFile, int>();
            _DatasetDescriptorRepository = uoW.GetRepository<ObservationDatasetDescriptor, Guid>();
        }

        //public void loadPDS(PrimaryDataset primaryDataset)
        //{

        //}

        public bool LoadPDS(int datasetId, int fileId, string descriptorId, int studyId)
        {
            var PDS = _pdsRepository.FindSingle(p => p.Id == datasetId);

            //ADD data file to PDS data files
            var file = _fileRepository.Get(fileId);
            if (file != null)
                PDS.DataFiles.Add(file);


            //ADD data records from file
            var _uploadFileDirectory = ConfigSettings.UploadFileDirectory;
            string fullpath = Path.Combine(_uploadFileDirectory, "P-142", file.FileName);
            string jsonString = File.ReadAllText(fullpath);
            // var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10,  IgnoreNullValues= true };
            PrimaryDataset pDSdata = JsonSerializer.Deserialize<PrimaryDataset>(jsonString)!;

            LoadContents(pDSdata);

            PDS.Modified = DateTime.Now.ToString("D");
            _pdsRepository.Update(PDS);
            return _dataServiceUnit.Save().Equals("CREATED");
        }

        private void LoadContents(PrimaryDataset pds)
        {
            var dd = _DatasetDescriptorRepository.FindSingle(d => d.Id == pds.DescriptorId);

        }
        private void UpdateLoadingStatus(DataFile _dataFile, string state)
        {
            _dataFile.State = state;
            _dataFile.IsLoadedToDB = state == "SAVED";
            _fileRepository.Update(_dataFile);
            _dataServiceUnit.Save();
        }
    }
}

