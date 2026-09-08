using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IItemGroupService
    {
        MyHttpResponseMessage GetItemGroupsForTreeView(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetItemGroupById(int id, Common common);
        MyHttpResponseMessage Save(ItemGroup model, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        string GenerateNextId(Common common);
        string GenerateGrCode(string ParentId, Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
