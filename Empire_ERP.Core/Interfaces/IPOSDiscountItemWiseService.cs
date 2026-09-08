using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPOSDiscountItemWiseService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage detailGrid(Common common);
        MyHttpResponseMessage Save(POSDiscountItemWise modelRecord, Common common);
        MyHttpResponseMessage GetPOSDiscountItemWiseByCode(int code, Common common);
        MyHttpResponseMessage GetPOSDiscountItemWiseDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
    }
}
