using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPartyReportRepository
    {
        MyHttpResponseMessage GetReportTypes(int menuID, string roleType, int? roleId);
        Task<MyHttpResponseMessage> GetReportData(PartyReport report, Common common);
    }
}