using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface ICashBookVoucherRepository
    {
        MyHttpResponseMessage GetChartOfAccounts(int? pType, Common common);
        MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common);
        MyHttpResponseMessage Save(List<CashBookVoucher> model, Common common, Menu menu);
        MyHttpResponseMessage GetCashBookVoucherByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetCashBookVoucherDetailsByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteCashBookVoucherDetailByCode(int tranID, int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(CashBookRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}
