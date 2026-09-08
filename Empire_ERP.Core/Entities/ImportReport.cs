using System.Data;

namespace Empire_ERP.Core.Entities
{
    public class ImportReport
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ReportID { get; set; }
        public string? ControlCode { get; set; }
        public int? AccountCode { get; set; }
    }

    public class CustomImportReport
    {
        public string? VoucherDate { get; set; }
        public string? chqDate { get; set; }
        public string? NatureName { get; set; }
        public int? AccountNature { get; set; }
        public string? bType { get; set; }
        public string? BankName { get; set; }
        public string? desc { get; set; }
        public string? VoucherNo { get; set; }
        public int? VoucherType { get; set; }
        public int? AccountCode { get; set; }
        public int? Qty { get; set; }
        public int? GroupOrder { get; set; }
        public int? Rate { get; set; }
        public int? chq { get; set; }
        public int? calcAmount { get; set; }
        public string? chqNo { get; set; }
        public string? AccountName { get; set; }
        public string? ParentName { get; set; }
        public string? AccountDescription { get; set; }
        public string? LINK { get; set; }
        public int TRAN_ID { get; set; }
        public int VC_TYPE { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Amt { get; set; }
    }

    public class ImportReportRDLCReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? INVOICE_NUMBER { get; set; }
        public string? DATE { get; set; }
        public string? PARTY_NAME { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? BRANCH_ADDRESS { get; set; }
        public string? BRANCH_PHONE { get; set; }
        public string? COMPANY_LOGO { get; set; }
        public string? COMPANY_WATER { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? ACT_GRCODE { get; set; }
        public string? SIG1 { get; set; }
        public string? SIG2 { get; set; }
        public string? SIG3 { get; set; }
        public string? SIG4 { get; set; }
        public string? REFERENCENO { get; set; }
        public string? TERM { get; set; }
        public string? CURR { get; set; }
        public string? CURR_SIG { get; set; }
        public string? EDIT_USER_ID { get; set; }
        public string? MENU_TERMS { get; set; }
        public decimal DISC { get; set; }
        public DateTime FROMDATE { get; set; }
        public DateTime TODATE { get; set; }
        public DateTime HIDDENFROMDATE { get; set; }
        public DateTime HIDDENTODATE { get; set; }
        public int REPORTID { get; set; }
        public int CONTROLCODE { get; set; }
        public int SUBSIDIARYCODE { get; set; }
    }

    public class CustomImportReportForPrintReport
    {
        public ImportReportRDLCReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
}
