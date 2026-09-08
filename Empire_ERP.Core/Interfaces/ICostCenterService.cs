using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface ICostCenterService
    {
        MyHttpResponseMessage QuickSearch(CostCenter model);
        MyHttpResponseMessage GetCostCenterByID(int id, Common common);
        MyHttpResponseMessage Save(CostCenter model, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
