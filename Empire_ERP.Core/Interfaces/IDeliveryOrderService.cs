using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IDeliveryOrderService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetDeliveryOrderByCode(int code, Common common);
        MyHttpResponseMessage GetDeliveryOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPickDataByParty(int partyCode, int actCode, Common common);
        MyHttpResponseMessage GetBarcodeList();
        MyHttpResponseMessage GetDeliveryOrderPickDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDeliveryOrderDetailByItem(int code, int qty, Common common);
        MyHttpResponseMessage Save(CustomDeliveryOrder modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteDeliveryOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(DeliveryOrderRDLCReport modelRecord, DataTable details, Common common);
    }
}