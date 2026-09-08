using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class AccountNatureService : IAccountNatureService
    {

        public IAccountNatureRepository _accountNatureRepository { get; set; }
        public AccountNatureService(IAccountNatureRepository accountNaturerepository)
        {
            _accountNatureRepository = accountNaturerepository;
        }



        public MyHttpResponseMessage GetAccountNature()
        {
            return _accountNatureRepository.GetAccountNature();
        }
    }
}
