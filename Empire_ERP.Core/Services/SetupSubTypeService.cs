using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class SetupSubTypeService : ISetupSubTypeService
    {
        public ISetupSubTypeRepository _setupSubTypeRepository { get; set; }
        public SetupSubTypeService(ISetupSubTypeRepository setupSubTypeRepository)
        {
            _setupSubTypeRepository = setupSubTypeRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _setupSubTypeRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(SetupSubType model, Common common)
        {
            return _setupSubTypeRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _setupSubTypeRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetSetupSubTypeById(int id, Common common)
        {
            return _setupSubTypeRepository.GetSetupSubTypeById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _setupSubTypeRepository.Delete(id, common);
        }
    }
}