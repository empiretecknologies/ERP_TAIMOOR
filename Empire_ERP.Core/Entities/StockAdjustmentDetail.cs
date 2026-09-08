namespace Empire_ERP.Core.Entities
{
    public class StockAdjustmentDetail
    {
        public int? TRAN_ID { get; set; }
        public int? DT_CODE { get; set; }
        public int? ITEM_CODE { get; set; }
        public int? PICK_ID { get; set; }
        public double? QTY { get; set; }
        public int? UNIT { get; set; }
        public int? RATE { get; set; }
        public double? QTY2 { get; set; }
        public double? AMT { get; set; }
        public double? BAL_QTY { get; set; }
        public string? DT_DESC { get; set; }
        public int? COLOR { get; set; }
        public int? SIZE { get; set; }
        public int? GRADE { get; set; }
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
    }

    public class StockAdjustmentDetailForPrint
    {
        public string? ITEM_CODE { get; set; }
        public string? ITEM_NAME { get; set; }
        public string? ITEM_ID { get; set; }
        public double? QTY { get; set; }
        public string? UNIT { get; set; }
        public double? QTY2 { get; set; }
        public double? BAL_QTY { get; set; }
        public string? DT_DESC { get; set; }
        public string? COLOR { get; set; }
        public string? SIZE { get; set; }
        public string? GRADE { get; set; }
    }
}