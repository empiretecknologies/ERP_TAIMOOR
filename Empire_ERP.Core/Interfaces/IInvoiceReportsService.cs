using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IInvoiceReportsService
    {
        MyHttpResponseMessage GetReportTypes(Common common);
        MyHttpResponseMessage GetReportData(InvoiceReports report, Common common);
    }
}