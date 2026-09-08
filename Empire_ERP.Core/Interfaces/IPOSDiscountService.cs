using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPOSDiscountService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(POSDiscount modelRecord, Common common);
        MyHttpResponseMessage GetPOSDiscountByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
    }
}
