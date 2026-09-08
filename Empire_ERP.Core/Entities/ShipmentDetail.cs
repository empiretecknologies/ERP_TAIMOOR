namespace Empire_ERP.Core.Entities
{
    public class ShipmentDetail
    {
        public int? TRAN_ID { get; set; }
        public int? DT_CODE { get; set; }
        public double? QTY { get; set; }
        public double? RATE { get; set; }
        public double? AMT { get; set; }
        public int? UNIT { get; set; }
        public double? CQTY { get; set; }
        public DateTime? SHIP_DATE { get; set; }
        public string? BL_NO { get; set; }
        public string? DT_DESC { get; set; }
        public int? PICK_ID { get; set; }
        public int? BCODE { get; set; }
        public int? PERIOD_ID { get; set; }
        public int? ALLOW { get; set; }
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
    }
}
