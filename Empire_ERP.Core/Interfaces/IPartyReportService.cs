using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPartyReportService
    {
        MyHttpResponseMessage GetReportTypes(Common common);
        Task<MyHttpResponseMessage> GetReportData(PartyReport report, Common common);
    }
}