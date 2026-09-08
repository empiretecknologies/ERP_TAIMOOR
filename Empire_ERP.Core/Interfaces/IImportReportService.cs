using System.Data;
using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IImportReportService
    {
        MyHttpResponseMessage GetReportTypes(Common common);
        MyHttpResponseMessage GetReportData(ImportReport report, Common common);
        MyHttpResponseMessage GetDataForReport(ImportReportRDLCReport modelRecord, DataTable dataTable, Common common);
    }
}