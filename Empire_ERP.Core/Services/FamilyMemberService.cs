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
    public class FamilyMemberService : IFamilyMemberService
    {
        public IFamilyMemberRepository _userRepository { get; set; }
        public FamilyMemberService(IFamilyMemberRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public MyHttpResponseMessage QuickSearch(int employeeId, Common common)
        {
            return _userRepository.QuickSearch(employeeId, common);
        }

        public MyHttpResponseMessage Save(FamilyMember model, Common common)
        {
            return _userRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _userRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetFamilyMemberById(int id, Common common)
        {
            return _userRepository.GetFamilyMemberById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _userRepository.Delete(id, common);
        }

        public MyHttpResponseMessage SaveFamilyMemberOTP(OTPFamilyMember user)
        {
            return _userRepository.SaveFamilyMemberOTP(user);
        }

        public MyHttpResponseMessage OTPVerification(OTPFamilyMember user)
        {
            return _userRepository.OTPVerification(user);
        }
    }
}