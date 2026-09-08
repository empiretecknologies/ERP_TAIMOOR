using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPurchaseRequisitionService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetPurchaseRequisitionByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseRequisitionDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomPurchaseRequisition modelRecord, Common common);
		MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeletePurchaseRequisitionDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(PurchaseRequisitionRDLCReport modelRecord, DataTable details, Common common);
    }
}