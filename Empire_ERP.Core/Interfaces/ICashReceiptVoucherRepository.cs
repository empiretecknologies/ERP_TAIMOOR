using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface ICashReceiptVoucherRepository
    {
        MyHttpResponseMessage GetChartOfAccounts(int? pType, Common common);
        MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common);
        //MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common, int skip = 0, int take = 12, string filter = null, string group = null, string sort = null);
        MyHttpResponseMessage Save(List<CashReceiptVoucher> model, Common common, Menu menu);
        MyHttpResponseMessage SaveDetailCharges(DetailCharges model, Common common, Menu menu);
        MyHttpResponseMessage GetCashReceiptVoucherByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetDetailCharges(int tranId, int dtCode, int partyCode, int actCode, Common common, Menu menu);
        MyHttpResponseMessage GetSumDetailCharges(int tranId, int dtCode, Common common);
        MyHttpResponseMessage GetCashReceiptVoucherDetailsByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage DeleteDetailCharges(DetailCharges model, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteCashReceiptVoucherDetailByCode(int tranID, int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(CashReceiptRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}
