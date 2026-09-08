namespace Empire_ERP.Core.Entities
{
    public class PartyReport
    {
        public string? Pass { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ReportID { get; set; }
        public string? ControlCode { get; set; }
        public string? RegionCode { get; set; }
        public string? BRANCH { get; set; }
        public string? PERIOD { get; set; }
        public int? PartyCode { get; set; }
        public int? CAT_CODE { get; set; }
        public int? Nature { get; set; }
    }

    public class CustomPartyReport
    {

        public string? ACT_NAME { get; set; }
        public string? PARTY_NAME { get; set; }
        public string? CATEGORY { get; set; }
        public decimal Aging_0_15 { get; set; }
        public decimal Aging_16_30 { get; set; }
        public decimal Aging_31_45 { get; set; }
        public decimal Aging_46_60 { get; set; }
        public decimal Aging_31_60 { get; set; }
        public decimal Aging_61_90 { get; set; }
        public decimal Aging_91_120 { get; set; }
        public decimal Aging_120_Plus { get; set; }
        public decimal BalanceWithTotal { get; set; }
        public decimal Balance2 { get; set; }
        public int TRAN_ID { get; set; }
        public int BCODE { get; set; }
        public string? VoucherDate { get; set; }
        public string? VoucherNo { get; set; }
        public int? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? BName { get; set; }
        public string? AccountNature { get; set; }
        public string? PartyName { get; set; }
        public string? AccountDescription { get; set; }
        public decimal? Debit { get; set; }
        public decimal? Credit { get; set; }
        public decimal? Balance { get; set; }
        public decimal? Amount { get; set; }
        public decimal? DueAmount { get; set; }
        public decimal? LastAmount { get; set; }
        public decimal? Qty { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amt { get; set; }
        public decimal? Disc { get; set; }
        public int? DueYear { get; set; }
        public int? DueMonth { get; set; }
        public int? DueDay { get; set; }
        public string? LINK { get; set; }
        public string? DueDate { get; set; }
        public string? LastDate { get; set; }
        public string? BillType { get; set; }
        public string? ChqNo { get; set; }
        public string? ChqDate { get; set; }

    }
}