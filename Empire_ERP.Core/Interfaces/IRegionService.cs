using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IRegionService
    {
        MyHttpResponseMessage GetRegionsDropDown(int menuid);
        MyHttpResponseMessage GetRegions(Common common);
        MyHttpResponseMessage GetAccountsForTreeView(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetRegionById(int id, Common common);
        MyHttpResponseMessage Save(Region model, Common common);
        string GenerateNextId(Common common);
        string GenerateGrCode(string ParentId, Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}