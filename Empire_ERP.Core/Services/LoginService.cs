using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class LoginService : ILoginService
    {
        public ILoginRepository _loginInterface { get; set; }
        public LoginService(ILoginRepository loginInterface)
        {
            _loginInterface = loginInterface;
        }
        public MyHttpResponseMessage CheckCredentials(string username, string password, bool isremember)
        {
            return _loginInterface.CheckCredentials(username, password, isremember);    
        }

        public MyHttpResponseMessage GetUserByUsername(string username)
        {
            return _loginInterface.GetUserByUsername(username);
        }

        public Info GetBackGroundAndLogo()
        {
            return _loginInterface.GetBackGroundAndLogo();
        }

        public MyHttpResponseMessage SaveUserOTP(OTPUser user)
        {
            return _loginInterface.SaveUserOTP(user);
        }

        public MyHttpResponseMessage UpdateUserOTPStatus(OTPUser user)
        {
            return _loginInterface.UpdateUserOTPStatus(user);
        }

        public MyHttpResponseMessage OTPVerification(OTPUser user)
        {
            return _loginInterface.OTPVerification(user);
        }

        public MyHttpResponseMessage UpdatePassword(User user)
        {
            return _loginInterface.UpdatePassword(user);
        }

        public MyHttpResponseMessage GetUserByUserID(int userID)
        {
            return _loginInterface.GetUserByUserID(userID);
        }
        public MyHttpResponseMessage GetAllDDL(Common common)
        {
            return _loginInterface.GetAllDDL(common);
        }
    }
}