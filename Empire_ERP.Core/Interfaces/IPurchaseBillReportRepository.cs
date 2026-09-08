using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPurchaseBillReportRepository
    {
        MyHttpResponseMessage GetReportTypes(int menuID, string roleType, int? roleId);
        MyHttpResponseMessage GetReportData(PurchaseBillReport report, Common common);
    }
}