using System;
using System.Collections.Generic;
using System.Linq;
using eTRIKS.Commons.Core.Domain.Interfaces;
using eTRIKS.Commons.Core.Domain.Model;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel;
using eTRIKS.Commons.Core.Domain.Model.DatasetModel.SDTM;
using eTRIKS.Commons.Core.Domain.Model.DesignElements;
using eTRIKS.Commons.Core.JoinEntities;

namespace eTRIKS.Commons.Service.Services
{
    public class SubjectService
    {
        private readonly IRepository<HumanSubject, string> _subjectRepository;
        private readonly IRepository<Study, int> _studtRepository;
        private readonly IRepository<CharacteristicFeature, int> _characteristicObjRepository;

        private readonly IServiceUoW _dataContext;

        public SubjectService(IServiceUoW uoW)
        {
            _dataContext = uoW;
            _subjectRepository = uoW.GetRepository<HumanSubject, string>();
            _studtRepository = uoW.GetRepository<Study, int>();
            _characteristicObjRepository = uoW.GetRepository<CharacteristicFeature, int>();
        }

       
    }
}
