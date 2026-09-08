using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface ILoginRepository
    {
        MyHttpResponseMessage CheckCredentials(string username, string password, bool isremember);
        MyHttpResponseMessage GetUserByUsername(string username);
        Info GetBackGroundAndLogo();
        MyHttpResponseMessage SaveUserOTP(OTPUser user);
        MyHttpResponseMessage UpdateUserOTPStatus(OTPUser user);
        MyHttpResponseMessage OTPVerification(OTPUser user);
        MyHttpResponseMessage UpdatePassword(User user);
        MyHttpResponseMessage GetUserByUserID(int userID);
        MyHttpResponseMessage GetAllDDL(Common common);
    }
}
