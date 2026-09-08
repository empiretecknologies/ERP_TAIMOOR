using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPurchaseOrderRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetPurchaseOrderByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomPurchaseOrder modelRecord, Common common);
        MyHttpResponseMessage GetPickDataByParty(int partyCode, int actCode, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeletePurchaseOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(PurchaseOrderRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}