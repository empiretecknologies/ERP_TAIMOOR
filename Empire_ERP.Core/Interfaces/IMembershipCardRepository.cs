using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMembershipCardRepository
    {
        MyHttpResponseMessage GetMembershipCards(Common common);
        MyHttpResponseMessage GetAccountsForTreeView(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetMembershipCardById(int id, Common common);
        MyHttpResponseMessage Save(MembershipCard model, Common common);
        string GenerateNextId(Common common);
        string GenerateGrCode(string ParentId, Common common);
        string GenerateCardNo(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
        MyHttpResponseMessage GetDataForReport(MembershipCardReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}
