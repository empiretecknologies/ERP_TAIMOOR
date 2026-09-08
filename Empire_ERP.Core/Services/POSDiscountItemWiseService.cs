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
    public class POSDiscountItemWiseService : IPOSDiscountItemWiseService
    {

        public IPOSDiscountItemWiseRepository _POSDiscountItemWiseRepository { get; set; }
        public POSDiscountItemWiseService(IPOSDiscountItemWiseRepository POSDiscountItemWiseRepository)
        {
            _POSDiscountItemWiseRepository = POSDiscountItemWiseRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _POSDiscountItemWiseRepository.QuickSearch(common);
        }
        public MyHttpResponseMessage detailGrid(Common common)
        {
            return _POSDiscountItemWiseRepository.detailGrid(common);
        }

        public MyHttpResponseMessage Save(POSDiscountItemWise modelRecord, Common common)
        {
            return _POSDiscountItemWiseRepository.Save(modelRecord, common);
        }

        public MyHttpResponseMessage GetPOSDiscountItemWiseByCode(int code, Common common)
        {
            return _POSDiscountItemWiseRepository.GetPOSDiscountItemWiseByCode(code, common);
        }
        public MyHttpResponseMessage GetPOSDiscountItemWiseDetailByCode(int code, Common common)
        {
            return _POSDiscountItemWiseRepository.GetPOSDiscountItemWiseDetailByCode(code, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _POSDiscountItemWiseRepository.Delete(code, common);
        }
    }
}
