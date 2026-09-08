using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IJournalVoucherRepository
    {
        MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common);
        //MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage Save(List<JournalVoucher> model, Common common, Menu menu);
        MyHttpResponseMessage GetJournalVoucherByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetJournalVoucherDetailsByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteJournalVoucherDetailByCode(int tranID, int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(JournalVoucherRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Common common);
    }
}