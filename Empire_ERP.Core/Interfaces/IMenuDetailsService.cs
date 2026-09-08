using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMenuDetailsService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetMenuDetailsById(int id, Common common);
        MyHttpResponseMessage Save(MenuDetails model, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
