using System;
using System.Collections.Generic;
using System.Diagnostics;
using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model.DatasetModel;
using PlatformTM.Core.Domain.Model.DatasetModel.SDTM;
using PlatformTM.Models.DTOs;

namespace PlatformTM.Models.Services.Loading.SDTM
{
    public class SDTMloader
    {
        private readonly IServiceUoW _dataServiceUnit;
        private readonly IRepository<Dataset, int> _datasetRepository;
        private readonly IRepository<DataFile, int> _dataFileRepository;
        private readonly IRepository<SdtmRow, Guid> _sdtmRepository;

        public SDTMloader(IServiceUoW uoW)
        {
            _dataServiceUnit = uoW;
            _datasetRepository = uoW.GetRepository<Dataset, int>();
            _dataFileRepository = uoW.GetRepository<DataFile, int>();
            _sdtmRepository = uoW.GetRepository<SdtmRow, Guid>();
        }

        private void UpdateLoadingStatus(DataFile _dataFile, string state)
        {
            _dataFile.State = state;
            _dataFile.IsLoadedToDB = state=="SAVED";
            _dataFileRepository.Update(_dataFile);
            _dataServiceUnit.Save();
        }

        public bool LoadSDTM(int datasetId, int fileId, DataTable dataTable)
        {
            var dataset = _datasetRepository.FindSingle(d=>d.Id == datasetId, new List<string>(){"Variables.VariableDefinition", "Template", "Activity.Project" });
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

            //UpdateLoadingStatus(dataFile, "LOADING");

            var sdtmRowDescriptor = SdtmRowDescriptor.GetSdtmRowDescriptor(dataset);
            var SDTM = new List<SdtmRow>();
            var totalLoaded = 0.0;
            var totalRecords = dataTable.Rows.Count;
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
                   

                    if (SDTM.Count % 500 == 0)
                    {
                        _sdtmRepository.InsertMany(SDTM);

                        totalLoaded += SDTM.Count;
                        UpdateLoadingStatus(dataFile, Math.Round(totalLoaded / totalRecords * 100).ToString("##"));
                        
                        SDTM.Clear();
                    }
                }
                _sdtmRepository.InsertMany(SDTM);

                totalLoaded += SDTM.Count;
                UpdateLoadingStatus(dataFile, Math.Round(totalLoaded / totalRecords * 100).ToString("##"));

                Debug.WriteLine(dataTable.Rows.Count + " RECORD(s) SUCCESSFULLY ADDED FOR DATASET:" + datasetId + " ,DATAFILE:" + fileId);
            }
            catch (Exception e)
            {
                UpdateLoadingStatus(dataFile, "FAILED");
         
                Debug.WriteLine(e.Message);
                return false;
            }

            UpdateLoadingStatus(dataFile, "SAVED");
            return true;
        }
    }
}
