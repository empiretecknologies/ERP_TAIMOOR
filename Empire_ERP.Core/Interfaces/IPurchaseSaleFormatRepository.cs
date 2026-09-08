using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPurchaseSaleFormatRepository
    {
        MyHttpResponseMessage GetChartOfAccounts(int? pType);
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage Save(CustomPurchaseSaleFormat model, Common common);
        MyHttpResponseMessage GetPurchaseSaleFormatByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseSaleFormatDetailsByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage DeletePurchaseSaleFormatDetailByCode(int tranID, int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(ListPrintReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
        MyHttpResponseMessage GetDataForMultiBillReport(ListMultiBillPrintReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}
