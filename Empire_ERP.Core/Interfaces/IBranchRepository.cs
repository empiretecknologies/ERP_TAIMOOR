using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IBranchRepository
    {
        MyHttpResponseMessage GetBranchByCompany(int id);
        MyHttpResponseMessage GetBranchByCompanyWithRole(int id, int? roleId);
        MyHttpResponseMessage GetBranchByCode(string? code);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetBranchById(int id, Common common);
        MyHttpResponseMessage Save(Branch model, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
