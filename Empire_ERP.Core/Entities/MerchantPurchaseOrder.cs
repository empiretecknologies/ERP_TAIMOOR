namespace Empire_ERP.Core.Entities
{
    public class MerchantPurchaseOrder
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? PARTY_CODE { get; set; }
        public int? ACT_CODE { get; set; }
        public DateTime? SHIP_DATE { get; set; }
        public string? REF { get; set; }
        public string? COMM_AMT { get; set; }
        public double? AMT { get; set; }
        public double? COMM { get; set; }
        public double? COMM_VAL { get; set; }
        public string? SPARTY_CODE { get; set; }
        public int? SACT_CODE { get; set; }
        public string? CLIENT_PO { get; set; }
        public string? QTY { get; set; }
        public double? RATE { get; set; }
        public int? ITEM_CODE { get; set; }
        public int? EMP { get; set; }
        public int? BCODE { get; set; }
        public int? PERIOD_ID { get; set; }
        public int? TERMS { get; set; }
        public int? DEP_ID { get; set; }
        public int? GRADE { get; set; }
        //public int? FABRIC { get; set; }
        //public int? GSM { get; set; }
        public int? JOB_NO { get; set; }
        public int? CURR_CODE { get; set; }
        public double? CRATE { get; set; }
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
        public string? REMARKS { get; set; }
        public int? MENU_ID { get; set; }
        public string? DLT { get; set; }
        public string? UNIT { get; set; }
        public string? DOC { get; set; }
    }

    public class MerchantPurchaseOrderReport
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
        public string? REF { get; set; }
        public string? CLIENT_PO { get; set; }

        public string? CLIENT_NAME { get; set; }
        public string? DEP { get; set; }
        public string? SUPPLIER_NAME { get; set; }
        public string? JOB_NO { get; set; }
        public string? EMP_NAME { get; set; }
        public string? TERMS_NAME { get; set; }
        public string? CURR { get; set; }
        public decimal? CURR_RATE { get; set; }
        public string? ITEM_NAME { get; set; }
        public string? BRAND { get; set; }
        public decimal? QTY { get; set; }
        public string? UNIT_NAME { get; set; }
        public decimal? RATE { get; set; }
        public decimal? AMT { get; set; }
        public string? COMM_UNIT { get; set; }
        public decimal? COMM { get; set; }
        public decimal? COMM_VAL { get; set; }
        public string? REMARKS { get; set; }
        public string? SIG1 { get; set; }
        public string? SIG2 { get; set; }
        public string? SIG3 { get; set; }
        public string? SIG4 { get; set; }
        public string? USER { get; set; }




        //public string? PARTY_CODE { get; set; }
        //public string? S_DATE { get; set; }
        //public string? S_NO { get; set; }
        //public string? BROKER_CODE { get; set; }
        //public string? GODOWN { get; set; }
        //public string? KANTA { get; set; }
        //public string? COND { get; set; }
        //public string? C_NAME { get; set; }
        //public string? CELL { get; set; }
        //public string? LOT_NO { get; set; }
        //public string? ORIGIN { get; set; }
        //public string? ITEM_CODE { get; set; }
        //public decimal? TBAG { get; set; }
        //public string? UNIT { get; set; }
    }
}