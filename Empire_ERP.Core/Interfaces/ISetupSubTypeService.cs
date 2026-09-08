using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface ISetupSubTypeService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetSetupSubTypeById(int id, Common common);
        MyHttpResponseMessage Save(SetupSubType model, Common common);
        string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
