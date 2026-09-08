using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IEmpLeavesService
    {
        MyHttpResponseMessage QuickSearch(Common common , string TableName);
        MyHttpResponseMessage GetEmpLeavesRecord(int id,Common common);
        MyHttpResponseMessage Save(EmpLeaves model, Common common);
        //string GenerateNextId(Common common, string TableName);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
