using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMerchantPurchaseOrderDetailService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(CustomMerchantPurchaseOrderDetail model, Common common);
        MyHttpResponseMessage GetMerchantPurchaseOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage GetMerchantPurchaseOrderDetailDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteMerchantPurchaseOrderDetailDetailByCode(int code, Common common);
        MyHttpResponseMessage GetBatchDetailByProcess(string process, Common common);
        MyHttpResponseMessage GetDataForReport(MerchantPurchaseOrderDetailRDLCReport modelRecord, DataTable details, Common common);
    }
}