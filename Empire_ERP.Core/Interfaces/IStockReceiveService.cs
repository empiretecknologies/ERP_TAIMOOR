using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IStockReceiveService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetStockReceiveByCode(int code, Common common);
        MyHttpResponseMessage GetStockReceiveDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomStockReceive modelRecord, Common common);
		MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteStockReceiveDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForCartonSticker(List<StockReceiveStickerPrint> stockData);
        MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable details, Common common);
        MyHttpResponseMessage GetStockReceiveDetailByItem(int code, int qty, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, int branch, Common common);
        MyHttpResponseMessage GetPickData(string pickId, Common common);
        MyHttpResponseMessage UpdatePrintStatus(string codes, Common common);
        MyHttpResponseMessage GetBarcodeList();
    }
}