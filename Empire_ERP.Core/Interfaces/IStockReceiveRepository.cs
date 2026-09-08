using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IStockReceiveRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetStockReceiveByCode(int code, Common common);
        MyHttpResponseMessage GetStockReceiveDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomStockReceive modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteStockReceiveDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForCartonSticker(List<StockReceiveStickerPrint> stockData);
        MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Branch currentBranch, Common common);
        MyHttpResponseMessage UpdatePrintStatus(string codes, Common common);
        MyHttpResponseMessage GetStockReceiveDetailByItem(int code, int qty, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, int branch, Common common);
        MyHttpResponseMessage GetPickData(string pickId, Common common);
        MyHttpResponseMessage GetBarcodeList();
    }
}