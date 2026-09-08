using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface ICurrencyService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetCurrencyById(int id, Common common);
        MyHttpResponseMessage Save(Currency model, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
