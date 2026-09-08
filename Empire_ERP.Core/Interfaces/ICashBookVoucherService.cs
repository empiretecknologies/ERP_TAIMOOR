using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface ICashBookVoucherService
    {
        MyHttpResponseMessage GetChartOfAccounts(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(List<CashBookVoucher> model, Common common);
        MyHttpResponseMessage GetCashBookVoucherByCode(int code, Common common);
        MyHttpResponseMessage GetCashBookVoucherDetailsByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteCashBookVoucherDetailByCode(int tranID, int code, Common common);
        MyHttpResponseMessage GetDataForReport(CashBookRDLCReport modelRecord, DataTable details, Common common);
    }
}