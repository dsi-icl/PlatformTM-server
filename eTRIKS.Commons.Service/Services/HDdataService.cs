using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.Data.SDTM;

namespace eTRIKS.Commons.Service.Services
{
    public class HDdataService
    {
        private IRepository<Project, int> _projectRepository;
        private IServiceUoW _hdObsServiceUnit;

        public HDdataService(IServiceUoW uoW)
        {
            _hdObsServiceUnit = uoW;
            _projectRepository = uoW.GetRepository<Project, int>();
        }

        internal Task LoadHDdata(List<SdtmRow> sdtmData, int datasetId)
        {
            throw new NotImplementedException();
        }
    }
}
