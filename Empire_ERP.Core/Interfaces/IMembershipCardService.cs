using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMembershipCardService
    {
        MyHttpResponseMessage GetMembershipCards(Common common);
        MyHttpResponseMessage GetAccountsForTreeView(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage GetMembershipCardById(int id, Common common);
        MyHttpResponseMessage Save(MembershipCard model, Common common);
        string GenerateNextId(Common common);
        string GenerateCardNo(Common common);
        string GenerateGrCode(string ParentId, Common common);
        MyHttpResponseMessage Delete(int id, Common common);
        MyHttpResponseMessage GetDataForReport(MembershipCardReport modelRecord, DataTable details, Common common);
    }
}
