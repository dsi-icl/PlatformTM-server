using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services.Loading.SDTM
{
    public class SDTMloader
    {
        private readonly IServiceUoW _dataServiceUnit;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<DataFile, int> _dataFileRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;
        private readonly FileService _fileService;

        public SDTMloader(IServiceUoW uoW, FileService fileService)
        {
            _dataServiceUnit = uoW;
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _dataFileRepository = uoW.GetRepository<DataFile, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();

            _fileService = fileService;
        }

        public bool LoadSDTM(int datasetId, int fileId)
        {
            var dataset = _datasetRepository.FindSingle(d=>d.Id == datasetId, new List<string>(){"Variables.VariableDefinition", "Template" });
            var dataFile = _dataFileRepository.Get(fileId);

            if (!dataFile.State.ToLower().Equals("new"))
            {
                /**
                 * Replacing previously loaded file
                 * Remove file from collection before reloading it
                 */
                _sdtmRepository.DeleteMany(s => s.DatafileId == fileId && s.DatasetId == datasetId);
                Debug.WriteLine("RECORD(s) SUCCESSFULLY DELETED FOR DATASET:" + datasetId + " ,DATAFILE:" + fileId);
            }

            var filePath = dataFile.Path + "\\" + dataFile.FileName;
            var dataTable = _fileService.ReadOriginalFile(filePath);

            var sdtmRowDescriptor = SdtmRowDescriptor.GetSdtmRowDescriptor(dataset);
            var SDTM = new List<SdtmRow>();
            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    var sdtmRow = SDTMreader.readSDTMrow(row, dataTable, sdtmRowDescriptor);

                    sdtmRow.Id = Guid.NewGuid();
                    sdtmRow.DatasetId = datasetId;
                    sdtmRow.ActivityId = dataset.ActivityId;
                    sdtmRow.DatafileId = fileId;
                    sdtmRow.ProjectId = dataset.Activity.ProjectId;
                    sdtmRow.ProjectAccession = dataset.Activity.Project.Accession;

                    SDTM.Add(sdtmRow);
                }
                _sdtmRepository.InsertMany(SDTM);
                Debug.WriteLine(dataTable.Rows.Count + " RECORD(s) SUCCESSFULLY ADDED FOR DATASET:" + datasetId + " ,DATAFILE:" + fileId);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }


            dataFile.State = "SAVED";
            _dataFileRepository.Update(dataFile);

            _dataServiceUnit.Save();
            return true;
        }
    }
}
