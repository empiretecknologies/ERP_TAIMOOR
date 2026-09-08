using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class ApprovalService : IApprovalService
    {
        public IApprovalRepository _ApprovalRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ApprovalService(IApprovalRepository ApprovalRepository, IMenuService menuService)
        {
            _ApprovalRepository = ApprovalRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage GetApprovalSetup(int Branch, Common common)
        {
            return _ApprovalRepository.GetApprovalSetup(Branch, common);
        }
        public MyHttpResponseMessage GetApprovals(int Branch, Common common)
        {
            return _ApprovalRepository.GetApprovals(Branch, common);
        }

        public MyHttpResponseMessage Save(List<Approval> modelRecord, Common common)
        {
            return _ApprovalRepository.Save(modelRecord, common);
        }
        public MyHttpResponseMessage SaveSetup(List<Approval> modelRecord, Common common)
        {
            return _ApprovalRepository.SaveSetup(modelRecord, common);
        }


    }
}