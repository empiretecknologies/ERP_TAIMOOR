using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class AccountOpeningService : IAccountOpeningService
    {
        public IAccountOpeningRepository _accountOpeningRepository { get; set; }
        public AccountOpeningService(IAccountOpeningRepository accountOpeningRepository)
        {
            _accountOpeningRepository = accountOpeningRepository;
        }

        public MyHttpResponseMessage GetAccountOpenings(Common common)
        {
            return _accountOpeningRepository.GetAccountOpenings(common);
        }

        public MyHttpResponseMessage Save(List<AccountOpening> modelRecord, Common common)
        {
            return _accountOpeningRepository.Save(modelRecord, common);
        }
    }
}