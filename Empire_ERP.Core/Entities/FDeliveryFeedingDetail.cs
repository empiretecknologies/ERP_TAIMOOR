namespace Empire_ERP.Core.Entities
{
    public class FDeliveryFeedingDetail
    {
        public int? TRAN_ID { get; set; }
        public int? DT_CODE { get; set; }
        public int? ITEM_CODE { get; set; }
        public double? QTY { get; set; }
        public int? UNIT { get; set; }
        public double? QTY2 { get; set; }
        public double? BAL_QTY { get; set; }
        public double? RATE { get; set; }
        public double? CRATE { get; set; }
        public double? AMT { get; set; }
        public string? DT_DESC { get; set; }
        public string? TRUCK_NO { get; set; }
        public string? CONT_NO { get; set; }
        public int? SCOMP { get; set; }
        public int? WAREHOUSE { get; set; }
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
        public int? MENU_ID { get; set; }
        public string? DLT { get; set; }
        public int? CHK { get; set; }
        public int? PICK_ID { get; set; }
        public double? BR_AMOUNT_BUYER { get; set; }
        public double? BR_AMOUNT_SELLER { get; set; }
        public double? WT_AMOUNT_BUYER { get; set; }
        public double? WT_AMOUNT_SELLER { get; set; }
        public double? NET_AMT { get; set; }
        public double? RT_TYPE { get; set; }
        public double? INS { get; set; }
        public string? LOT_NO { get; set; }
    }
}