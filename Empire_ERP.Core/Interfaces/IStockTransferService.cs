using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IStockTransferService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetStockTransferByCode(int code, Common common);
        MyHttpResponseMessage GetStockTransferDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPrintData(int code, Common common);
        MyHttpResponseMessage Save(CustomStockTransfer modelRecord, Common common);
		MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteStockTransferDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForCartonSticker(List<StockTransferStickerPrint> stockData);
        MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable details, Common common);
        MyHttpResponseMessage GetStockTransferDetailByItem(int code, int qty, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, int branch, Common common);
        MyHttpResponseMessage UpdatePrintStatus(string codes, Common common);
        MyHttpResponseMessage GetBarcodeList();
    }
}