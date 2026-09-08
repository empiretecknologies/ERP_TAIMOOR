using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface ITexSalesInvoiceRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null, string sort = null);
        MyHttpResponseMessage GetPurchaseBillByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseBillDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseBillPickDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPickDataBySupplier(int pCode, int actCode, Common common);
        MyHttpResponseMessage GetBarcodeList();
        MyHttpResponseMessage GetPurchaseBillDetailByItem(int code, int qty, Common common);
        MyHttpResponseMessage Save(CustomTexSalesInvoice modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeletePurchaseBillDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForReport(TexSalesInvoiceRDLCReport modelRecord, DataTable inspectionServiceChargesDetails, Company currentCompany, Common common);
        Dictionary<int?, double?> PreviousStockInBill(string table, int? TRAN_ID, string period, string branch);
    }
}