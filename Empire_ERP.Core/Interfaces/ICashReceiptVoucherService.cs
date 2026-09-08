using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface ICashReceiptVoucherService
    {
        MyHttpResponseMessage GetChartOfAccounts(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null, string sort = null);
        MyHttpResponseMessage Save(List<CashReceiptVoucher> model, Common common);
        MyHttpResponseMessage SaveDetailCharges(DetailCharges model, Common common);
        MyHttpResponseMessage GetCashReceiptVoucherByCode(int code, Common common);
        MyHttpResponseMessage GetDetailCharges(int tranId, int dtCode, int partyCode, int actCode, Common common);
        MyHttpResponseMessage GetSumDetailCharges(int tranId, int dtCode, Common common);
        MyHttpResponseMessage GetCashReceiptVoucherDetailsByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteDetailCharges(DetailCharges model, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteCashReceiptVoucherDetailByCode(int tranID, int code, Common common);
        MyHttpResponseMessage GetDataForReport(CashReceiptRDLCReport modelRecord, DataTable details, Common common);
    }
}
