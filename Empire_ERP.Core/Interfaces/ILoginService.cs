using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface ILoginService
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
