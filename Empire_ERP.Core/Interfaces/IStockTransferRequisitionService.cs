using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IStockTransferRequisitionService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetStockTransferRequisitionByCode(int code, Common common);
        MyHttpResponseMessage GetStockTransferRequisitionDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomStockTransferRequisition modelRecord, Common common);
		MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteStockTransferRequisitionDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForCartonSticker(List<StockTransferRequisitionStickerPrint> stockData);
        MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable details, Common common);
        MyHttpResponseMessage UpdatePrintStatus(string codes, Common common);
    }
}