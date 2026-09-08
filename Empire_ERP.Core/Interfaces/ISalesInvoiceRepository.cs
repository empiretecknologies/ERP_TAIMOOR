using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface ISalesInvoiceReportRepository
    {
        MyHttpResponseMessage GetReportTypes(int menuID, string roleType, int? roleId);
        MyHttpResponseMessage GetReportData(SalesInvoiceReport report, Common common);
    }
}