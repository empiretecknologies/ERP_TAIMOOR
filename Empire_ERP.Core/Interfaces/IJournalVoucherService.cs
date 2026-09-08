using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IJournalVoucherService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage Save(List<JournalVoucher> model, Common common);
        MyHttpResponseMessage GetJournalVoucherByCode(int code, Common common);
        MyHttpResponseMessage GetJournalVoucherDetailsByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteJournalVoucherDetailByCode(int tranID, int code, Common common);
        MyHttpResponseMessage GetDataForReport(JournalVoucherRDLCReport modelRecord, DataTable details, Common common);
    }
}