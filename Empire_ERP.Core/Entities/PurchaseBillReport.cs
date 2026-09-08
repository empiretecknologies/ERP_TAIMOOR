namespace Empire_ERP.Core.Entities
{
    public class PurchaseBillReport
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ReportID { get; set; }
        public string? BuyerControlCode { get; set; }
        public string? SellerControlCode { get; set; }
        public int? BuyerRegionCode { get; set; }
        public int? SellerRegionCode { get; set; }
        public int? BuyerPartyCode { get; set; }
        public int? SellerPartyCode { get; set; }
        public string? ArivalStatus { get; set; }
        public string? Astatus { get; set; }
        public int? Group { get; set; }
        public int? Item { get; set; }
    }

    public class CustomPurchaseBillReport
    {
        public string? VoucherDate { get; set; }
        public string? VoucherNo { get; set; }
        public string? ArivalStatus { get; set; }
        public string? BuyerName { get; set; }
        public string? SellerName { get; set; }
        public string? ItemName { get; set; }
        public string? Remarks { get; set; }
        public string? QTY { get; set; }
        public string? QTY2 { get; set; }
        public string? BAL_QTY { get; set; }
        public string? UNIT { get; set; }
        public string? BrokerName { get; set; }
        public string? COND { get; set; }
        public string? RATE { get; set; }
        public string? RT_TYPE { get; set; }
        public string? AMT { get; set; }
        public string? INS { get; set; }
        public string? REF { get; set; }
        public string? DISC { get; set; }
        public string? DISC_AMT { get; set; }
        public string? TAX { get; set; }
        public string? TAX_AMT { get; set; }
        public string? NET_AMT { get; set; }
        public string? LINK { get; set; }
        public string? MTRAN_ID { get; set; }

        
    }
}