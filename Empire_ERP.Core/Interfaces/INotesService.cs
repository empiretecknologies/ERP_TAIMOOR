using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface INotesService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetNotesById(int id, Common common);
        MyHttpResponseMessage Save(Notes model, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}