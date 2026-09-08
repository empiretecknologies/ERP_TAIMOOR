namespace Empire_ERP.Core.Entities
{
    public class MembershipCard
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? FIRST_NAME { get; set; }
        public string? LAST_NAME { get; set; }
        public string? CNIC_NO { get; set; }
        public string? PHONE_NO { get; set; }
        public string? EMAIL { get; set; }
        public string? COMMENT { get; set; }
        public DateTime? START_D { get; set; }
        public DateTime? END_D { get; set; }
        public int? DISCOUNT { get; set; }
        public int? CLOSED { get; set; }
        public int? PointStartValue { get; set; }
        public int? PointRate { get; set; }
        public int? MaxPointsBeforeDiscount { get; set; }
        public string? CARD_TYPE { get; set; }
        public string? CARD_NO { get; set; }
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

    public class MembershipCardReport
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
        public string? S_DATE { get; set; }
        public string? S_NO { get; set; }
        public string? BROKER_CODE { get; set; }
        public string? GODOWN { get; set; }
        public string? KANTA { get; set; }
        public string? COND { get; set; }
        public string? C_NAME { get; set; }
        public string? CELL { get; set; }
        public string? LOT_NO { get; set; }
        public string? ORIGIN { get; set; }
        public string? ITEM_CODE { get; set; }
        public decimal? TBAG { get; set; }
        public string? UNIT { get; set; }
    }
}