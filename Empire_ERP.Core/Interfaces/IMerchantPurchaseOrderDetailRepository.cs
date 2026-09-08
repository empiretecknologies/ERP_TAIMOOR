using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMerchantPurchaseOrderDetailRepository
    {
        MyHttpResponseMessage QuickSearch(Common common, Menu menu);
        MyHttpResponseMessage Save(CustomMerchantPurchaseOrderDetail model, Common common, Menu menu);
        MyHttpResponseMessage GetMerchantPurchaseOrderDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetMerchantPurchaseOrderDetailDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetBatchDetailByProcess(string process, Common common);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteMerchantPurchaseOrderDetailDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(MerchantPurchaseOrderDetailRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}