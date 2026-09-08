using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class BranchService : IBranchService
    {

        public IBranchRepository _branchRepository { get; set; }
        public BranchService(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public MyHttpResponseMessage GetBranchByCompany(int id)
        {
            return _branchRepository.GetBranchByCompany(id);
        }

        public MyHttpResponseMessage GetBranchByCompanyWithRole(int id, int? roleId)
        {
            return _branchRepository.GetBranchByCompanyWithRole(id, roleId);
        }

        public MyHttpResponseMessage GetBranchByCode(string? code)
        {
            return _branchRepository.GetBranchByCode(code);
        }
        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _branchRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(Branch model, Common common)
        {
            return _branchRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _branchRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetBranchById(int id, Common common)
        {
            return _branchRepository.GetBranchById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _branchRepository.Delete(id, common);
        }
    }
}
