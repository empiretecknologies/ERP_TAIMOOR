using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class BaseService :IBaseService
    {
        public IBaseRepository _baseRepository { get; set; }
        public BaseService(IBaseRepository baseRepository)
        {
            _baseRepository = baseRepository;
        }
        public MyHttpResponseMessage UpdateSettings(Base modelRecord, Common common)
        {
            return _baseRepository.UpdateSettings(modelRecord, common);
        }
        public MyHttpResponseMessage GetApproval(string? userName, Common common)
        {
            return _baseRepository.GetApproval(userName, common);
        }

    }
}
