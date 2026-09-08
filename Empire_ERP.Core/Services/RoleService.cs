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
    public class RoleService : IRoleService
    {
        public IRoleRepository _roleRepository { get; set; }
        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _roleRepository.QuickSearch(common);
        }
      
        public MyHttpResponseMessage GetAllPermissions(Common common)
        {
            return _roleRepository.GetAllPermissions(common);
        }

        public MyHttpResponseMessage GetMainMenue(Common common)
        {
            return _roleRepository.GetMainMenue(common);
        }
        public MyHttpResponseMessage Save(CustomRole model, Common common)
        {
            return _roleRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _roleRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetRoleById(int id, Common common)
        {
            return _roleRepository.GetRoleById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _roleRepository.Delete(id, common);
        }
    }
}