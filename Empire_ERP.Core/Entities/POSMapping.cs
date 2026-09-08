using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics.Metrics;

namespace Empire_ERP.Core.Entities
{
    public class POSMapping
    {
        public int? TRAN_ID { get; set; }
        public string? PAY_COPIES { get; set; }
        public string? PRE_COPIES { get; set; }
        public string? DUP_COPIES { get; set; }
        public int? PAY_MODAL { get; set; }
        public int? PRE_MODAL { get; set; }
        public int? DUP_MODAL { get; set; }
        public string? GROUP_NAME { get; set; }
        public string? CASH_ACCOUNT { get; set; }
        public string? CASH_TAX { get; set; }
        public string? BANK_ACCOUNT { get; set; }
        public string? BANK_TAX { get; set; }
        public string? BANK_CHARGES { get; set; }
        public string? BRANCH { get; set; }
        public string? PAY_ACCOUNT { get; set; }
        public string? PARTY_TAX { get; set; }
        public string? SRB_NAME { get; set; }
        public string? SRB_NTN { get; set; }
        public string? POS_USER { get; set; }
        public string? POS_PASS { get; set; }
        public string? SRB_ID { get; set; }
        public string? SRB_URL { get; set; }
        public string? Group_IMG { get; set; }
        public string? Item_IMG { get; set; }
        public string? Table_IMG { get; set; }
        public string? Waiter_IMG { get; set; }
        public string? cashcheck { get; set; }
        public string? P_PRINTER { get; set; }
        public string? advancecheck { get; set; }
        public string? bankcheck { get; set; }
        public string? partycheck { get; set; }
        public string? splitcheck { get; set; }
        public string? advancebtn { get; set; }
        public string? kotbtn { get; set; }
        public string? salesmanReq { get; set; }
        public string? srbcheck { get; set; }
        public string? RATE { get; set; }
        public string? FB_LINK { get; set; }
        public string? INSTA_LINK { get; set; }
        public string? WEB_LINK { get; set; }
        public string? TIKTOK_LINK { get; set; }
        public string? YOUTUBE_LINK { get; set; }
        public string? WIFI_NAME { get; set; }
        public string? WIFI_PASSWORD { get; set; }
        public string? WHATSAPP_URL { get; set; }
        public string? WHATSAPP_TOKEN { get; set; }
        public string? WHATSAPP_MSG { get; set; }
        public string? WHATSAPP_CC { get; set; }
        public string? WHATSAPP_RTN { get; set; }
        public string? WHATSAPP_ADV { get; set; }
        public string? WHT_ADV_COM { get; set; }
        public string? WHT_PARTY_MSG { get; set; }
        public string? SER_CHARGES { get; set; }
        public string? S_IMG { get; set; }
        public string? QR_CODE { get; set; }
        public string? P_WINDOW { get; set; }
        public string? LOC_SNAME { get; set; }
        public string? LOGO_IMG { get; set; }


    }

    public class POSMappingReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? COMPANY_LOGO { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? PARTY_CODE { get; set; }
        public string? LOT_NO { get; set; }
        public string? ITEM_CODE { get; set; }
        public string? DUE_NO { get; set; }
        public string? DRIVER { get; set; }
        public string? VEHICLE { get; set; }
        public decimal? QUANTITY { get; set; }
        public string? UNIT { get; set; }
        public string? MENU_SIG1 { get; set; }
        public string? MENU_SIG2 { get; set; }
        public string? MENU_SIG3 { get; set; }
        public string? MENU_SIG4 { get; set; }
        public string? MENU_TERMS { get; set; }
    }
}