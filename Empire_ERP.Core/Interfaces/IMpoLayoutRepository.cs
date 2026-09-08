using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMpoLayoutRepository
    {
        MyHttpResponseMessage QuickSearch(Common common, Menu menu);
        MyHttpResponseMessage Save(CustomMpoLayout model, Common common, Menu menu);
        MyHttpResponseMessage GetMpoLayoutByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetMpoLayoutDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetBatchDetailByProcess(string process, Common common);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteMpoLayoutDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(MpoLayoutRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}