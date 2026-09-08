namespace Empire_ERP.Core.Entities
{
    public class FSodaBookFeeding
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? SELLER_CODE { get; set; }
        public int? SACT_CODE { get; set; }
        public string? BUYER_CODE { get; set; }
        public int? BACT_CODE { get; set; }
        public string? REF { get; set; }
        public string? REMARKS { get; set; }
        public string? COND { get; set; }
        public string? BROKER_CODE { get; set; }
        public string? COB_CODE { get; set; }
        public int? BD_ACT_CODE { get; set; }
        public int? COB_ACODE { get; set; }
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
        public double? CREDIT_DAYS { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public string? SBF_TYPE { get; set; }
        public int? CURR_CODE { get; set; }
        public int? UNIT { get; set; }
        public decimal? CRATE { get; set; }
        public DateTime? SHIP_DATE { get; set; }
        public int? SHIP_STATUS { get; set; }
        public string? P_S { get; set; }
        public string? P_SHIP { get; set; }
        public string? TRANS_PS { get; set; }
        public string? SODA_TYPE { get; set; }

        public int? COA { get; set; }
    }

    public class CustomFSodaBookFeeding
    {
        public FSodaBookFeeding? Master { get; set; }
        public List<FSodaBookFeedingDetail>? Detail { get; set; }
    }
}
