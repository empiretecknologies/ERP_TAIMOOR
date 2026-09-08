using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPurchaseBookVoucherService
    {
        MyHttpResponseMessage GetChartOfAccounts(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(List<PurchaseBookVoucher> model, Common common);
        MyHttpResponseMessage GetPurchaseBookVoucherByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseBookVoucherDetailsByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeletePurchaseBookVoucherDetailByCode(int tranID, int code, Common common);
        MyHttpResponseMessage GetDataForReport(CashReceiptRDLCReport modelRecord, DataTable details, Common common);
    }
}
