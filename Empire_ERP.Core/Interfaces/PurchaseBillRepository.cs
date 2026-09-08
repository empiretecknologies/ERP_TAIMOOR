using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPurchaseBillRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null, string sort = null);
        MyHttpResponseMessage GetPurchaseBillByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseBillDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPurchaseBillCommissionByCode(int code, Common common);
        MyHttpResponseMessage GetPrintData(int code, Common common);
        MyHttpResponseMessage GetPurchaseBillPickDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPickDataByParty(int partyCode, int actCode, int region, Common common);
        MyHttpResponseMessage GetBarcodeList();
        MyHttpResponseMessage GetPurchaseBillDetailByItem(int code, int qty, Common common);
        MyHttpResponseMessage Save(CustomPurchaseBill modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeletePurchaseBillDetailByCode(int code, Common common);
        MyHttpResponseMessage DeleteCommDetailByCode(int code, Common common);
        MyHttpResponseMessage GetLastRateByBarcode(CustomPurchaseBill model, string? dcType, Common common);

        MyHttpResponseMessage GetDataForReport(PurchaseBillRDLCReport modelRecord, DataTable details, DataTable taxDetails, DataTable inspectionServiceChargesDetails, DataTable reportDetailsDDJ, CustomMenuDetail menuDetails, Company currentCompany, Common common);
        Dictionary<int?, double?> PreviousStockInBill(string table, int? TRAN_ID, string period, string branch);
    }
}