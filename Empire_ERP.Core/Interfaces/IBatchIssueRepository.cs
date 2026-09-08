using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IBatchIssueRepository
    {
        MyHttpResponseMessage QuickSearch(Common common, Menu menu);
        MyHttpResponseMessage Save(CustomBatchIssue model, Common common, Menu menu);
        MyHttpResponseMessage BatchUpdate(BatchIssue model, Common common, Menu menu);
        MyHttpResponseMessage GetBatchIssueByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetBatchIssueDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteBatchIssueDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(BatchIssueRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}