using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IBatchIssueService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(CustomBatchIssue model, Common common);
        MyHttpResponseMessage BatchUpdate(BatchIssue model, Common common);
        MyHttpResponseMessage GetBatchIssueByCode(int code, Common common);
        MyHttpResponseMessage GetBatchIssueDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteBatchIssueDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(BatchIssueRDLCReport modelRecord, DataTable details, Common common);
    }
}