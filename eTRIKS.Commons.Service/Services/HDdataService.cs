using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;

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

        internal Task LoadHDdata(List<SdtmEntity> sdtmData, int datasetId)
        {
            throw new NotImplementedException();
        }
    }
}
