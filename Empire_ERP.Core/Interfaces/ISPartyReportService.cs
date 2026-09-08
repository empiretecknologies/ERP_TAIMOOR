using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface ISPartyReportService
    {
        MyHttpResponseMessage GetReportTypes(Common common);
        MyHttpResponseMessage GetReportData(SPartyReport report, Common common);
        MyHttpResponseMessage UpdateSodeBookFeedingReport(SodePartyReport report, Common common);
    }
}