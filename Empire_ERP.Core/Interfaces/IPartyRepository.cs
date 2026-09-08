using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPartyRepository
    {
        MyHttpResponseMessage Delete(int partyCode, int actCode, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage Save(PartyTypes partyTypes, Common common);
        MyHttpResponseMessage GetChartOfAccounts(Common common);
        MyHttpResponseMessage QuickSearch(int partyCode, Common common);
        MyHttpResponseMessage QuickSearchParty(Common common);
        //MyHttpResponseMessage QuickSearchLazyLoading(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage GetPartyTypeByPartyCode(int partyCode, Common common);
        MyHttpResponseMessage GetBranchesInfoByPartyCode(int partyCode, Common common);
        MyHttpResponseMessage SaveBranchInfo(PartyTypeBranch partyTypesBranch, Common common);
        MyHttpResponseMessage GetBranchInfoByBranchId(int branchId, int partyCode, Common common);
        MyHttpResponseMessage DeleteBranchInfo(int branchId, int partyCode, Common common);
        MyHttpResponseMessage GetPartyTypeByPartyCode(int partyCode);
    }
}