using System.Data;
using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IImportReportRepository
    {
        MyHttpResponseMessage GetReportTypes(int menuID, string roleType, int? roleId);
        MyHttpResponseMessage GetReportData(ImportReport report, Common common);
        MyHttpResponseMessage GetDataForReport(ImportReportRDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}