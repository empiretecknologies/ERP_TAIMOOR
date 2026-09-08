using System.Data;

namespace Empire_ERP.Core.Entities
{
    public class CashBookVoucher
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public int? CURR_CODE { get; set; }
        public double? CRATE { get; set; }
        public int? BOOK_TYPE { get; set; }
        public string? REMARKS { get; set; }
        public int? BCODE { get; set; }
        public int? PERIOD_ID { get; set; }
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
        public string? DLT { get; set; }
        public int? DT_CODE { get; set; }
        public int? ACT_CODE { get; set; }
        public int? PARTY_CODE { get; set; }
        public string? DC_TYPE { get; set; }
        public double? AMT { get; set; }
        public string? CHQ_NO { get; set; }
        public DateTime? CHQ_DATE { get; set; }
        public string? DT_DESC { get; set; }
        public string? DOC { get; set; }
    }

    public class CashBookRDLCReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? INVOICE_NUMBER { get; set; }
        public string? DATE { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? COMMENT { get; set; }
        public string? PARTY_NAME { get; set; }
        public string? STATUS { get; set; }
        public string? SIG1 { get; set; }
        public string? SIG2 { get; set; }
        public string? SIG3 { get; set; }
        public string? SIG4 { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? COMPANY_LOGO { get; set; }
        public string? USER { get; set; }

    }

    public class CustomCashBookForPrintReport
    {
        public CashBookRDLCReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
}
