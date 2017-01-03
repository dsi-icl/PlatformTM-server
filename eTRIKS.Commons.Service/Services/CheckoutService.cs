using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.Users.Datasets;
using eTRIKS.Commons.Core.Domain.Model.Users.Queries;
using eTRIKS.Commons.Service.DTOs;

namespace eTRIKS.Commons.Service.Services
{
    public class CheckoutService
    {
        private IServiceUoW _dataContext;

        private readonly IRepository<CombinedQuery, Guid> _combinedQueryRepository;
        private readonly IRepository<UserDataset, Guid> _userDatasetRepository;
        private ExportService _exportService;

        public CheckoutService(IServiceUoW uoW, ExportService exportService)
        {
            _dataContext = uoW;
            _combinedQueryRepository = uoW.GetRepository<CombinedQuery, Guid>();
            _userDatasetRepository = uoW.GetRepository<UserDataset, Guid>();
            _exportService = exportService;
        }

        public List<UserDataset> CreateCheckoutDatasets(string queryIdStr, string userId)
        {
            Guid queryId;
            if (!Guid.TryParse(queryIdStr, out queryId))
                return null;
            var query = _combinedQueryRepository.Get(queryId);

            var phenoDataset = new UserDataset();
            phenoDataset.Id = Guid.NewGuid();
            phenoDataset.OwnerId = userId;
            phenoDataset.ProjectId = query.ProjectId;
            phenoDataset.Type = "PHENO";
            phenoDataset.Name = "Phenotypes";
            //CREATE DATAFIELDS
            phenoDataset.Fields.Add(new DatasetField()
            {
                FieldName = "SubjectId",
                ColumnHeader = "SubjectId",
                ColumnHeaderIsEditable = false
            });
            phenoDataset.Fields.AddRange(query.DesignElements.Select(qObj => new DatasetField()
            {
                QueryObject = qObj,
                QueryObjectType = qObj.ObservationObject, //TEMP should consider to add type to obsquery if used as generic query
                ColumnHeader = qObj.ObservationObject
            }));
            phenoDataset.Fields.AddRange(query.SubjectCharacteristics.Select(qObj => new DatasetField()
            {
                QueryObject = qObj,
                QueryObjectType = nameof(SubjectCharacteristic),
                ColumnHeader = qObj.ObservationObject
            }));
            //TEMP //SHOULD ADD SUBJECTID and UNIQUE SUBJECT ID to CHARACTERISTICS/CHARACTERISTICS_OBJ
            phenoDataset.Fields.AddRange(query.ClinicalObservations.Select(qObj => new DatasetField()
            {
                QueryObject = qObj,
                QueryObjectType = nameof(SdtmRow),
                ColumnHeader = qObj.ObservationObjectShortName+"["+qObj.ObservationQualifier+"]"
            }));

            _userDatasetRepository.Insert(phenoDataset);
            _dataContext.Save();

            return new List<UserDataset>() {phenoDataset};
        }

        public DataTable ExportDataset(int projectId, string datasetId)
        {
            var dataset = _userDatasetRepository.FindSingle(d => d.Id == Guid.Parse(datasetId));

            var exportData = _exportService.GetDatasetContent(projectId, dataset);

            //var dt = _exportService.GetDatasetTable(exportData);

            return new DataTable();
        }
    }
}
