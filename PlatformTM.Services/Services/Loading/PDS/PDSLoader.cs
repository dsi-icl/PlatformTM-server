using System;
using System.IO;
using System.Text.Json;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.PDS;
using PlatformTM.Models.Configuration;

namespace PlatformTM.Services.Services.Loading.PDS
{
    public class PDSLoader
    {


        private readonly IServiceUoW _dataServiceUnit;

        private readonly IRepository<PrimaryDataset, int> _pdsRepository;
        private readonly IRepository<DataFile, int> _fileRepository;
        private FileStorageSettings ConfigSettings { get; set; }

        public PDSLoader(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            _pdsRepository = uoW.GetRepository<PrimaryDataset, int>();
            _fileRepository = uoW.GetRepository<DataFile, int>();
        }

        //public void loadPDS(PrimaryDataset primaryDataset)
        //{

        //}

        public bool LoadPDS(int fileId, string descriptorId, int studyId)
        {

            var file = _fileRepository.Get(fileId);
            string filePath;

            var _uploadFileDirectory = ConfigSettings.UploadFileDirectory;

            string fullpath = Path.Combine(_uploadFileDirectory, "P-142", file.FileName);

            //string fileName = "WeatherForecast.json";
            string jsonString = File.ReadAllText(fullpath);



            // var options = new JsonSerializerOptions { WriteIndented = true, MaxDepth = 10,  IgnoreNullValues= true };
            PrimaryDataset pDS = JsonSerializer.Deserialize<PrimaryDataset>(jsonString)!;

            pDS.DescriptorId = descriptorId;
            pDS.StudyId = studyId;

            //pDS.Id = Guid.NewGuid();


            _pdsRepository.Insert(pDS);
            return _dataServiceUnit.Save().Equals("CREATED");


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

