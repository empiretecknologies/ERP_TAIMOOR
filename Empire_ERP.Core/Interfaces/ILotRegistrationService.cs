using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface ILotRegistrationService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetLotRegistrationById(int id, Common common);
        MyHttpResponseMessage Save(LotRegistration model, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
