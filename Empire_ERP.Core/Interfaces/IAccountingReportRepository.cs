using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IAccountingReportRepository
    {
        MyHttpResponseMessage GetReportTypes(int menuID, string roleType, int? roleId);
        MyHttpResponseMessage GetReportData(AccountingReport report, Common common);
    }
}