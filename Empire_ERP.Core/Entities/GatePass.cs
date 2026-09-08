namespace Empire_ERP.Core.Entities
{
    public class GatePass
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public int? LOT { get; set; }
        public string? DUE_NO { get; set; }
        public string? DRIVER { get; set; }
        public string? VEHICLE { get; set; }
        public int? ITEM_CODE { get; set; }
        public decimal? QTY { get; set; }
        public int? UNIT { get; set; }
        public string? PARTY_CODE { get; set; }
        public int? ACT_CODE { get; set; }
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
    }

    public class GatePassReport
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