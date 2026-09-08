using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IEmpMasterInfoRepository
    {
        MyHttpResponseMessage QuickSearch(Common common , string TableName);
        MyHttpResponseMessage GetEmpMasterInfoById(int id, string TableName, Common common);
        MyHttpResponseMessage Save(EmpMasterInfo model, Common common);
        string GenerateNextId(Common common , string TableName);
        MyHttpResponseMessage Delete(int id, string TableName, Common common);
    }
}
