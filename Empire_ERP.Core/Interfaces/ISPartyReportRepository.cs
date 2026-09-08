using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface ISPartyReportRepository
    {
        MyHttpResponseMessage GetReportTypes(int menuID, string roleType, int? roleId);
        MyHttpResponseMessage GetReportData(SPartyReport report, Common common);
        MyHttpResponseMessage UpdateSodeBookFeedingReport(SodePartyReport report, Common common);
    }
}