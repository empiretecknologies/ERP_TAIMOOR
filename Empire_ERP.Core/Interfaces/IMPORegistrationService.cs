using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMPORegistrationService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetMPORegistrationById(int id, Common common);
        MyHttpResponseMessage Save(MPORegistration model, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
