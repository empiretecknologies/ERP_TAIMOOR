using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPurchaseOrderService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetPurchaseOrderByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPickDataByParty(int partyCode, int actCode, Common common);
        MyHttpResponseMessage Save(CustomPurchaseOrder modelRecord, Common common);
		MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeletePurchaseOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(PurchaseOrderRDLCReport modelRecord, DataTable details, Common common);
    }
}