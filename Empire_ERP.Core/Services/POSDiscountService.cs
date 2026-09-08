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
    public class POSDiscountService : IPOSDiscountService
    {

        public IPOSDiscountRepository _posDiscountRepository { get; set; }
        public POSDiscountService(IPOSDiscountRepository posDiscountRepository)
        {
            _posDiscountRepository = posDiscountRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _posDiscountRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(POSDiscount modelRecord, Common common)
        {
            return _posDiscountRepository.Save(modelRecord, common);
        }

        public MyHttpResponseMessage GetPOSDiscountByCode(int code, Common common)
        {
            return _posDiscountRepository.GetPOSDiscountByCode(code, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _posDiscountRepository.Delete(code, common);
        }
    }
}
