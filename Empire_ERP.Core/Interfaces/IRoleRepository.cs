using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IRoleRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetAllPermissions(Common common);
        MyHttpResponseMessage GetMainMenue(Common common);
        MyHttpResponseMessage GetRoleById(int id, Common common);
        MyHttpResponseMessage Save(CustomRole model, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
