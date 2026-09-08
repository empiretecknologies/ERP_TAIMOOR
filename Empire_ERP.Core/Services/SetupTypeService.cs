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
    public class SetupTypeService : ISetupTypeService
    {
        public ISetupTypeRepository _setupTypeRepository { get; set; }
        public SetupTypeService(ISetupTypeRepository setupTypeRepository)
        {
            _setupTypeRepository = setupTypeRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _setupTypeRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(SetupType model, Common common)
        {
            return _setupTypeRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _setupTypeRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetSetupTypeById(int id, Common common)
        {
            return _setupTypeRepository.GetSetupTypeById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _setupTypeRepository.Delete(id, common);
        }
    }
}