using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IApprovalService
    {
		MyHttpResponseMessage GetApprovals(int Branch, Common common);
        MyHttpResponseMessage GetApprovalSetup(int Branch, Common common);
        MyHttpResponseMessage Save(List<Approval> modelRecord, Common common);
        MyHttpResponseMessage SaveSetup(List<Approval> modelRecord, Common common);

    }
}