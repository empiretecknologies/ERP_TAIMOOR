using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class StockTransfer
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? REF { get; set; }
        public string? AM { get; set; }
        public string? REMARKS { get; set; }
        public int? BCODE { get; set; }
        public int? TBCODE { get; set; }
        public int? PERIOD_ID { get; set; }
        public int? TPERIOD_ID { get; set; }
        public string? ADD_USER_ID { get; set; }
        public DateTime? ADD_DATE { get; set; }
        public string? ADD_COMPUTER_NAME { get; set; }
        public string? ADD_IP_ADDRESS { get; set; }
        public string? EDIT_USER_ID { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string? EDIT_COMPUTER_NAME { get; set; }
        public string? EDIT_IP_ADDRESS { get; set; }
        public string? ADD_POSTALCODE { get; set; }
        public string? EDIT_POSTALCODE { get; set; }
        public string? ASTATUS { get; set; }
        public int? MENU_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? DLT { get; set; }
    }

    public class CustomStockTransfer
    {
        public StockTransfer? Master { get; set; }
        public List<StockTransferDetail>? Detail { get; set; }
    }

    public class StockTransferStickerPrint
    {
        public int? ITEM_CODE { get; set; }
        public string? ITEM_NAME { get; set; }
        public string? ITEM_ID { get; set; }
        public string? BLABEL { get; set; }
        public string? SIZES { get; set; }
        public string? COLORS { get; set; }
        public int? QTY { get; set; }
        public string? CATEGORIES { get; set; }
        public string? SUBCATEGORIES { get; set; }
        public string? BARCODE_TYPE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public int? FBCODE { get; set; }
        public int? TBCODE { get; set; }
    }

    public class StockTransferForPrint
    {
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? REF { get; set; }
        public string? REMARKS { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? COMPANY_LOGO { get; set; }
        public string? BRANCH_FROM_NAME { get; set; }
        public string? BRANCH_FROM_ADDRESS { get; set; }
        public string? BRANCH_TO_NAME { get; set; }
        public string? BRANCH_TO_ADDRESS { get; set; }
        public string? ASTATUS { get; set; }
        public string? STOCK_TYPE { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? REPORT_NAME { get; set; }
        public bool? MENU_SIG1 { get; set; }
        public bool? MENU_SIG2 { get; set; }
        public bool? MENU_SIG3 { get; set; }
        public bool? MENU_SIG4 { get; set; }
    }

    public class CustomStockTransferForPrintReport
    {
        public StockTransferForPrint? Master { get; set; }
        public DataTable Detail { get; set; }
    }
}
