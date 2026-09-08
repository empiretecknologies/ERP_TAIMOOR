using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMpoTrimsService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(CustomMpoTrims model, Common common);
        MyHttpResponseMessage GetMpoTrimsByCode(int code, Common common);
        MyHttpResponseMessage GetMpoTrimsDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteMpoTrimsDetailByCode(int code, Common common);
        MyHttpResponseMessage GetBatchDetailByProcess(string process, Common common);
        MyHttpResponseMessage GetDataForReport(MpoTrimsRDLCReport modelRecord, DataTable details, Common common);
    }
}