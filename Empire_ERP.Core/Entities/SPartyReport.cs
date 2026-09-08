namespace Empire_ERP.Core.Entities
{
    public class SPartyReport
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ReportID { get; set; }
        public int? dT_CODE { get; set; }
        public string? BuyerControlCode { get; set; }
        public string? SellerControlCode { get; set; }
        public int? BuyerRegionCode { get; set; }
        public int? SellerRegionCode { get; set; }
        public int? BuyerPartyCode { get; set; }
        public int? SellerPartyCode { get; set; }
        public string? ArivalStatus { get; set; }
        public int? Group { get; set; }
        public int? Item { get; set; }
        public string? SbfType { get; set; }
        public string? BuyerRegionGr { get; set; }
        public string? SellerRegionGr { get; set; }
        public bool Insurance { get; set; }
    }

    public class CustomSPartyReport
    {
        public int? TRAN_ID { get; set; }
        public int? DT_CODE { get; set; }
        public string? VoucherDate { get; set; }
        public string? LINK { get; set; }
        public string? VoucherNo { get; set; }
        public string? ArivalStatus { get; set; }
        public string? BuyerName { get; set; }
        public string? SellerName { get; set; }
        public string? ItemName { get; set; }
        public string? ItemNameWithAvg { get; set; }
        public string? Remarks { get; set; }
        public string? QTY { get; set; }
        public string? SQTY { get; set; }
        public string? DQTY { get; set; }
        public string? OQTY { get; set; }
        public string? IQTY { get; set; }
        public string? BQTY { get; set; }
        public string? UNIT { get; set; }
        public string? BrokerName { get; set; }
        public string? COND { get; set; }
        public string? DUE_DATE { get; set; }
        public int? CREDIT_DAYS { get; set; }
        public string? RATE { get; set; }
        public string? RATE1 { get; set; }
        public string? RT_TYPE { get; set; }
        public string? AMT { get; set; }
        public string? AVG { get; set; }
        public string? INS { get; set; }
        public string? TOTAL_STOCK_IN { get; set; }
        public string? TOTAL_STOCK_OUT { get; set; }
        public string? AMT_CREDIT { get; set; }
        public string? AMT_DEBIT { get; set; }
    }

    public class SodePartyReport
    {
        public SPartyReport? Master { get; set; }
        public List<SPartyReportDetail>? Detail { get; set; }
    }

    public class SPartyReportDetail
    {
        public int? dT_CODE { get; set; }
        public string? voucherDate { get; set; }
        public string? voucherNo { get; set; }
        public string? buyerName { get; set; }
        public string? sellerName { get; set; }
        public string? itemName { get; set; }
        public string? qty { get; set; }
        public string? sqty { get; set; }
        public string? dqty { get; set; }
        public string? unit { get; set; }
        public string? rate { get; set; }
    }
}