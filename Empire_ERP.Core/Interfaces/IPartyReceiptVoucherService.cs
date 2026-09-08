using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPartyReceiptVoucherService
    {
        MyHttpResponseMessage GetChartOfAccounts(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage Save(List<PartyReceiptVoucher> model, Common common);
        MyHttpResponseMessage GetPartyReceiptVoucherByCode(int code, Common common);
        MyHttpResponseMessage GetPartyReceiptVoucherDetailsByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeletePartyReceiptVoucherDetailByCode(int tranID, int code, Common common);
        MyHttpResponseMessage GetDataForReport(CashReceiptRDLCReport modelRecord, DataTable details, Common common);
    }
}