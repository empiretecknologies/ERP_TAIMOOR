using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IBillOfMaterialService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(CustomBillOfMaterial model, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage GetBillOfMaterialByCode(int code, Common common);
        MyHttpResponseMessage GetBillOfMaterialDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteBillOfMaterialDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(BillOfMaterialRDLCReport modelRecord, DataTable details, Common common);
    }
}