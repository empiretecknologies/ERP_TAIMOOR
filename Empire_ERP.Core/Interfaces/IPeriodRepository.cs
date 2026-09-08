using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPeriodRepository
    {
        MyHttpResponseMessage GetPeriodsByBranch(int id);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetPeriodById(int id, Common common);
        MyHttpResponseMessage GetPeriodById(int periodID);
        MyHttpResponseMessage Save(Period model, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
