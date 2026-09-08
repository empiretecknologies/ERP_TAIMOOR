using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMpoTrimsRepository
    {
        MyHttpResponseMessage QuickSearch(Common common, Menu menu);
        MyHttpResponseMessage Save(CustomMpoTrims model, Common common, Menu menu);
        MyHttpResponseMessage GetMpoTrimsByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetMpoTrimsDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetBatchDetailByProcess(string process, Common common);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteMpoTrimsDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(MpoTrimsRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}