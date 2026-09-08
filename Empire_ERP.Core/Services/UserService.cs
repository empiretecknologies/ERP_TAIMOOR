using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class UserService : IUserService
    {
        public IUserRepository _userRepository { get; set; }
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _userRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(User model, Common common)
        {
            return _userRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _userRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetUserById(int id, Common common)
        {
            return _userRepository.GetUserById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _userRepository.Delete(id, common);
        }

        public MyHttpResponseMessage SaveUserOTP(OTPUser user)
        {
            return _userRepository.SaveUserOTP(user);
        }

        public MyHttpResponseMessage OTPVerification(OTPUser user)
        {
            return _userRepository.OTPVerification(user);
        }
    }
}