using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class POSMaster_Print
    {
        // Header Fields
        public string? VOUCHER_NO { get; set; }
        public string? V_DATE { get; set; }
        public string? DEL_DATE { get; set; }
        public string? CNAME { get; set; }
        public string? CMOB { get; set; }
        public string? CellNo { get; set; }
        public string? INV_STATUS { get; set; }
        public double? TOTAL { get; set; }
        public double? DISC_Per { get; set; }
        public double? DISC_AMT { get; set; }
        public double? NET_TOTAL { get; set; }
        public double? SER_CHARGES { get; set; }
        public double? SETTLEMENT { get; set; }
        public string? Salesman { get; set; }
        public string? PartyName { get; set; }
        public string? BShortName { get; set; }
        public string? BName { get; set; }
        public string? BAddress { get; set; }
        public string? BTel { get; set; }
        public string? BNTN { get; set; }
        public string? CPC_CODE { get; set; }
        public string? MENU_TERMS { get; set; }
        public string? FB_LINK { get; set; }
        public string? INSTA_LINK { get; set; }
        public string? WEB_LINK { get; set; }
        public string? TIKTOK_LINK { get; set; }
        public string? YOUTUBE_LINK { get; set; }
        public string? WIFINAME { get; set; }
        public string? WIFIPASSWORD { get; set; }
        public string? C_LOGO { get; set; }
        public string? PRINT_MODE_LABLE { get; set; }
        public string? KOT_PRINTER { get; set; }
        public string? P_PRINTER { get; set; }

        public string? JobName { get; set; }

        public string? STICKER_PRINTER { get; set; }

        // Payment
        public string? PAY_TYPE { get; set; }
        public string? PARTY_MODE { get; set; }
        public double? CASH { get; set; }
        public double? INVOICE_VALUE { get; set; }
        public double? BANK { get; set; }
        public double? PARTY { get; set; }
        public double? CASH_TAX { get; set; }
        public double? BANK_TAX { get; set; }
        public double? PARTY_TAX { get; set; }
        public double? CASH_TAX_VALUE { get; set; }
        public double? BANK_TAX_VALUE { get; set; }
        public double? PARTY_TAX_VALUE { get; set; }
        public double? ADVANCE { get; set; }
        public double? DEL { get; set; }
        public double? DEL_CHARGES { get; set; }
        public double? ADV_BANK { get; set; }
        public double? BALANCE { get; set; }
        public double? TOTAL_ADVANCE { get; set; }
        public double? RECV { get; set; }
        public double? CashBack { get; set; }

        // Waiter / Table
        public string? Waiter { get; set; }
        public string? TABLE { get; set; }

        // Detail Fields
        public string? ITEM_NAME { get; set; }
        public double? QTY { get; set; }
        public double? RATE { get; set; }
        public double? NET_AMT { get; set; }
        public double? DISC { get; set; }
        public double? DETAIL_DISC_AMT { get; set; }

        // Additional Fields from SQL
        public string? BARCODE { get; set; }
        public string? REMARKS { get; set; }
        public string? D_REMARKS { get; set; }
        public string? BILL_STATUS { get; set; }
        public string? BILL_MODE { get; set; }
        public string? SRB_INV { get; set; }
        public string? SRB_VN { get; set; }
        public int? COMPLETE { get; set; }
        public string? API_STATUS { get; set; }
        public string? USER_NAME { get; set; }
        public string? STICKER_LOGO { get; set; }
        public string? LOCATION_SNAME { get; set; }
        public int? ITEM_CODE { get; set; }
        public int? QR_CODE { get; set; }
        public int? P_WINDOW { get; set; }
        public double? GROSS_TOTAL { get; set; }
    }


}
