using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPurchaseBillReportService
    {
        MyHttpResponseMessage GetReportTypes(Common common);
        MyHttpResponseMessage GetReportData(PurchaseBillReport report, Common common);
    }
}