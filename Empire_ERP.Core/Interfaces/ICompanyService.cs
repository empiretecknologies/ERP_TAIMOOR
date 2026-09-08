using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface ICompanyService
    {
        MyHttpResponseMessage GetCompanies();
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetCompanyById(int id, Common common);
        MyHttpResponseMessage Save(Company model, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
        MyHttpResponseMessage GetCompanyByCode(int code);
    }
}
