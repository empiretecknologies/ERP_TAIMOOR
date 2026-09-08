using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMpoLayoutService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(CustomMpoLayout model, Common common);
        MyHttpResponseMessage GetMpoLayoutByCode(int code, Common common);
        MyHttpResponseMessage GetMpoLayoutDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteMpoLayoutDetailByCode(int code, Common common);
        MyHttpResponseMessage GetBatchDetailByProcess(string process, Common common);
        MyHttpResponseMessage GetDataForReport(MpoLayoutRDLCReport modelRecord, DataTable details, Common common);
    }
}