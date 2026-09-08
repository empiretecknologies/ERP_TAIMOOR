using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMaterialRequisitionRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetMaterialRequisitionByCode(int code, Common common);
        MyHttpResponseMessage GetMaterialRequisitionDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomMaterialRequisition modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteMaterialRequisitionDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(MaterialRequisitionRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);

    }
}   