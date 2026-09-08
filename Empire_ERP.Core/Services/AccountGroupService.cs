using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class AccountGroupService : IAccountGroupService
    {
        public IAccountGroupRepository _accountGroupRepository { get; set; }
        public AccountGroupService(IAccountGroupRepository accountGroupRepository)
        {
            _accountGroupRepository = accountGroupRepository;
        }

        public MyHttpResponseMessage GetAccountGroups()
        {
            return _accountGroupRepository.GetAccountGroups();
        }       
    }
}