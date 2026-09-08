using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IFamilyMemberService
    {
        MyHttpResponseMessage QuickSearch(int employeeId, Common common);
        MyHttpResponseMessage GetFamilyMemberById(int id, Common common);
        MyHttpResponseMessage Save(FamilyMember model, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
        MyHttpResponseMessage SaveFamilyMemberOTP(OTPFamilyMember user);
        MyHttpResponseMessage OTPVerification(OTPFamilyMember user);
    }
}
