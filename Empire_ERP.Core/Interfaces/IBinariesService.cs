using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IBinariesService
    {
        MyHttpResponseMessage SourceDatabaseDDL(Binaries model);
        MyHttpResponseMessage DestinationDatabaseDDL(Binaries model);
        MyHttpResponseMessage GetDatabaseObjects(Binaries model);
        //MyHttpResponseMessage GetWarehouseAccountsForTreeView(Common common);
        //MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage GetWarehouseAccountById(int code, Common common);
        //MyHttpResponseMessage Save(Warehouse model, Common common);
        //MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        //string GenerateNextId(Common common);
        //string GenerateGrCode(string ParentId, Common common);
        //MyHttpResponseMessage Delete(int code, Common common);
    }
}
