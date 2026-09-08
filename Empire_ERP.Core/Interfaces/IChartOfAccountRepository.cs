using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IChartOfAccountRepository
    {
        MyHttpResponseMessage GetChartOfAccounts(Common common);
        MyHttpResponseMessage GetAccountsForTreeView(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage GetChartOfAccountById(int id, Common common);
        MyHttpResponseMessage Save(ChartOfAccount model, Common common);
        string GenerateNextId(Common common);
        string GenerateGrCode(string ParentId, Common common);
        MyHttpResponseMessage Delete(int id, string startDate, string endDate, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
    }
}
