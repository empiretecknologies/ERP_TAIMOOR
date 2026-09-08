using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IDailyProductionService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(CustomDailyProduction model, Common common);
        MyHttpResponseMessage GetDailyProductionByCode(int code, Common common);
        MyHttpResponseMessage GetDailyProductionDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteDailyProductionDetailByCode(int code, Common common);
        MyHttpResponseMessage GetBatchDetailByProcess(int process, Common common);
        MyHttpResponseMessage GetDataForReport(DailyProductionRDLCReport modelRecord, DataTable details, Common common);
    }
}