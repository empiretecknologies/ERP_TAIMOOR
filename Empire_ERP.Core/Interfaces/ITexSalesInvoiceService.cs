using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface ITexSalesInvoiceService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null, string sort = null);
        MyHttpResponseMessage GetPurchaseBillByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseBillDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPickDataBySupplier(int pCode, int actCode, Common common);
        MyHttpResponseMessage GetBarcodeList();
        MyHttpResponseMessage GetPurchaseBillPickDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseBillDetailByItem(int code, int qty, Common common);
        MyHttpResponseMessage Save(CustomTexSalesInvoice modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeletePurchaseBillDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(TexSalesInvoiceRDLCReport modelRecord, DataTable inspectionServiceChargesDetails, Common common);
    }
}