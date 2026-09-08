using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPOSTransactionService
    {
		MyHttpResponseMessage QuickSearch(Common common);
		MyHttpResponseMessage GetPayQuickSearch(Common common);
		MyHttpResponseMessage AdvanceBookingRecords(Common common);
		MyHttpResponseMessage GetCustomerHistory(Common common , string CstNumber);
		MyHttpResponseMessage GetMapData(Common common);
		MyHttpResponseMessage GetSyncData(Common common);
		MyHttpResponseMessage SRBApi_Status(string voucherno, string invoiceId, Common common);
        MyHttpResponseMessage Save(CustomPOSTransaction model, Common common);
        MyHttpResponseMessage UpdateTablesAndWaiter(CustomPOSTransaction model, Common common);
        MyHttpResponseMessage GetPOSTransactionByCode(int code , string voucher, Common common);
        MyHttpResponseMessage GetExpenseByCode(int code, Common common);
        MyHttpResponseMessage GetPOSTransactionDetailByCode(int? code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteExpenseRow(int code, Common common);
        MyHttpResponseMessage GetBarcodeList(int ItemId, Common common);
        MyHttpResponseMessage GetAllBarcodeList(Common common);
        MyHttpResponseMessage DeletePOSTransactionDetailByCode(CustomPOSTransaction modelRecord, int code, int ItemId, string dtcode, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, Common common);
        MyHttpResponseMessage GetItemsGroup(Common common);
        MyHttpResponseMessage GetItemsMasterByGroup(int groupId , Common common);
        MyHttpResponseMessage GetItemsMasterByCode(int itemId , string barcode, int Qty, int TranId, Common common);
        MyHttpResponseMessage GetAllDiscount(Common common);
        MyHttpResponseMessage GetAllWaiter(Common common);
        MyHttpResponseMessage GetAllTables(Common common, string Tran_Id);
        MyHttpResponseMessage ExpenseRecord(Common common);
        MyHttpResponseMessage ExpenseRecordSave(ExpensePOSTransaction modelRecord,Common common);
        MyHttpResponseMessage GetDynamicIcon();
        MyHttpResponseMessage GetDataForReport(POSPrint_Model modelRecord, DataTable details, Common common);
        MyHttpResponseMessage ExpensePrintReport(List<ExpensePrint> expenses, Common common);
        MyHttpResponseMessage GetPendingRecords(DateTime FromDate , DateTime ToDate, Common common);
    }
}