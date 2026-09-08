namespace Empire_ERP.Core.Entities
{
    public class AccountingReport
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ReportID { get; set; }
        public string? ControlCode { get; set; }
        public int? AccountCode { get; set; }
        public string? BRANCH { get; set; }
    }

    public class CustomAccountingReport
    {
        public string? VoucherDate { get; set; }
        public string? chqDate { get; set; }
        public string? NatureName { get; set; }
        public decimal? AccountNature { get; set; }
        public string? bType { get; set; }
        public string? BankName { get; set; }
        public string? desc { get; set; }
        public string? VoucherNo { get; set; }
        public int? VoucherType { get; set; }
        public int? AccountCode { get; set; }
        public int? Qty { get; set; }
        public int? GroupOrder { get; set; }
        public int? BCODE { get; set; }
        public int? Rate { get; set; }
        public string? chq { get; set; }
        public int? calcAmount { get; set; }
        public string? chqNo { get; set; }
        public int? AccountId { get; set; }
        public int? AccountGrCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountType { get; set; }
        public string? GRCode { get; set; }
        public string? ParentName { get; set; }
        public string? AccountDescription { get; set; }
        public string? LINK { get; set; }
        public int TRAN_ID { get; set; }
        public int VC_TYPE { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Balance2 { get; set; }
        public decimal? Balances { get; set; }
        public decimal? Balanced { get; set; }
        public decimal? Amt { get; set; }
        public string? BILL_NO { get; set; }
        public string? BILL_DATE { get; set; }
        public string? BName { get; set; }


    }
}
