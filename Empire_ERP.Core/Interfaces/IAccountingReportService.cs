using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IAccountingReportService
    {
        MyHttpResponseMessage GetReportTypes(Common common);
        MyHttpResponseMessage GetReportData(AccountingReport report, Common common);
    }
}