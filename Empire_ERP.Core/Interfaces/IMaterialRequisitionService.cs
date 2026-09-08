using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMaterialRequisitionService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetMaterialRequisitionByCode(int code, Common common);
        MyHttpResponseMessage GetMaterialRequisitionDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomMaterialRequisition modelRecord, Common common);
		MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteMaterialRequisitionDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(MaterialRequisitionRDLCReport modelRecord, DataTable details, Common common);
    }
}