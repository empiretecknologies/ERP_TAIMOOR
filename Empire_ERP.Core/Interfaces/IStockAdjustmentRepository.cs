using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IStockAdjustmentRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetStockAdjustmentByCode(int code, Common common);
        MyHttpResponseMessage GetStockAdjustmentDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomStockAdjustment modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteStockAdjustmentDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForCartonSticker(List<StockAdjustmentStickerPrint> stockData);
        MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Branch currentBranch, Common common);
        MyHttpResponseMessage UpdatePrintStatus(string codes, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, int branch, Common common);
        MyHttpResponseMessage GetStockAdjustmentDetailByItem(int code, int qty, Common common);
        MyHttpResponseMessage GetStockAdjustmentPickDetailByCode(int code, Common common);
        MyHttpResponseMessage GetBarcodeList();
    }
}