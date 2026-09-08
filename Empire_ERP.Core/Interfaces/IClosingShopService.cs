using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IClosingShopService
    {
        MyHttpResponseMessage GetClosingData(DateTime FromDate , DateTime toDate, Common common);
        MyHttpResponseMessage UpdateClosedData(DateTime FromDate , DateTime toDate, Common common);
        MyHttpResponseMessage GetDataForReport(ClosingShop modelRecord, DataTable details, Common common);
        MyHttpResponseMessage SaveClosingData(SaveClosingRequest saveClosingRequest, Common common);
    }
}