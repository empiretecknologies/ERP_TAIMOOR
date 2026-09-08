using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IDailyProductionRepository
    {
        MyHttpResponseMessage QuickSearch(Common common, Menu menu);
        MyHttpResponseMessage Save(CustomDailyProduction model, Common common, Menu menu);
        MyHttpResponseMessage GetDailyProductionByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetDailyProductionDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetBatchDetailByProcess(int process, Common common);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteDailyProductionDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(DailyProductionRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}