using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IStockTransferRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetStockTransferByCode(int code, Common common);
        MyHttpResponseMessage GetStockTransferDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPrintData(int code, Common common);
        MyHttpResponseMessage Save(CustomStockTransfer modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteStockTransferDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForCartonSticker(List<StockTransferStickerPrint> stockData);
        MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Branch currentBranch, Common common);
        MyHttpResponseMessage UpdatePrintStatus(string codes, Common common);
        MyHttpResponseMessage GetStockTransferDetailByItem(int code, int qty, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, int branch, Common common);

        MyHttpResponseMessage GetBarcodeList();
    }
}