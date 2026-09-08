using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IDeliveryOrderRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetDeliveryOrderByCode(int code, Common common);
        MyHttpResponseMessage GetDeliveryOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDeliveryOrderPickDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPickDataByParty(int partyCode, int actCode, Common common);
        MyHttpResponseMessage GetBarcodeList();
        MyHttpResponseMessage GetDeliveryOrderDetailByItem(int code, int qty, Common common);
        MyHttpResponseMessage Save(CustomDeliveryOrder modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteDeliveryOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(DeliveryOrderRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Common common);
        Dictionary<int?, double?> PreviousStockInBill(string table, int? TRAN_ID, string period, string branch);
    }
}