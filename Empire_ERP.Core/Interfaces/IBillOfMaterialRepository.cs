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
    public interface IBillOfMaterialRepository
    {
        MyHttpResponseMessage QuickSearch(Common common, Menu menu);
        MyHttpResponseMessage Save(CustomBillOfMaterial model, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage GetBillOfMaterialByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetBillOfMaterialDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage DeleteBillOfMaterialDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(BillOfMaterialRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}