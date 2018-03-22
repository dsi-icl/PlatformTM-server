using PlatformTM.Core.Domain.Interfaces;
using PlatformTM.Core.Domain.Model;

namespace PlatformTM.Services.Services
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
