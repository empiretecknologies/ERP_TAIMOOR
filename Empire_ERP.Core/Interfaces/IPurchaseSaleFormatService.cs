using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPurchaseSaleFormatService
    {
        MyHttpResponseMessage GetChartOfAccounts(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage Save(CustomPurchaseSaleFormat model, Common common);
        MyHttpResponseMessage GetPurchaseSaleFormatByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseSaleFormatDetailsByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeletePurchaseSaleFormatDetailByCode(int tranID, int code, Common common);
        MyHttpResponseMessage GetDataForReport(ListPrintReport modelRecord, DataTable details, Common common);
        MyHttpResponseMessage GetDataForMultiBillReport(ListMultiBillPrintReport modelRecord, DataTable details, Common common);
    }
}
