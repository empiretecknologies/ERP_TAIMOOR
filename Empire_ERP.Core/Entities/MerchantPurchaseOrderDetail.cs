using System.Data;

namespace Empire_ERP.Core.Entities
{
    public class MerchantPurchaseOrderDetail
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? REF { get; set; }
        public string? CLIENT_PO { get; set; }
        public int? PARTY_CODE { get; set; }
        public int? ACT_CODE { get; set; }
        public int? SPARTY_CODE { get; set; }
        public int? FABRIC { get; set; }
        public int? GSM { get; set; }
        public int? SACT_CODE { get; set; }
        public string? REMARKS { get; set; }
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
        public string? JOB_NO { get; set; }
        public string? DLT { get; set; }
        public int? PICK_ID { get; set; }
        public double? COMM { get; set; }
        public double? COMM_VAL { get; set; }
        public string? COMM_AMT { get; set; }
    }

    public class MerchantPurchaseOrderDetailDetail
    {
        public int? TRAN_ID { get; set; }
        public int? DT_CODE { get; set; }
        public int? ITEM_CODE { get; set; } // D
        public string? BATCH { get; set; } // D
        public double? QTY2 { get; set; } // D
        public double? BAL_QTY { get; set; } // D
        public string? ORDER_NO { get; set; }
        public string? COMM_TYPE { get; set; }
        public decimal? COMM_RATE { get; set; }
        public decimal? COMM_AMT { get; set; }
        public int? INTAKE { get; set; }
        public int? S_NO { get; set; }
        public string? REV_STATUS { get; set; }
        public int? REV_REF { get; set; }
        public int? COLOR { get; set; }
        public int? SENTITY_CODE { get; set; }
        public int? DCHANNEL_CODE { get; set; }
        public int? GRADE_CODE { get; set; }
        public int? SEASON_CODE { get; set; }
        public int? SIZE { get; set; }
        public double? QTY { get; set; }
        public int? UNIT { get; set; }
        public double? RATE { get; set; }
        public double? AMT { get; set; }
        public string? PORT { get; set; }
        public DateTime? SHIP_DATE { get; set; }
        public DateTime? BOOKING_DATE { get; set; }
        public DateTime? HANDOVER_DATE { get; set; }
        public string? DOC { get; set; }
        public string? DT_DESC { get; set; }
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
        public bool? REV_TOGGLE { get; set; }

    }

    public class CustomMerchantPurchaseOrderDetail
    {
        public MerchantPurchaseOrderDetail? Master { get; set; }
        public List<MerchantPurchaseOrderDetailDetail>? Detail { get; set; }
    }

    public class MerchantPurchaseOrderDetailRDLCReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? INVOICE_NUMBER { get; set; }
        public string? DATE { get; set; }
        public string? MITEM_NAME { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? BRANCH_ADDRESS { get; set; }
        public string? BRANCH_PHONE { get; set; }
        public string? COMPANY_LOGO { get; set; }
        public string? COMPANY_WATER { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? REF { get; set; }
        public string? PROCESS { get; set; }
        public string? REMARKS { get; set; }
        public string? ORDER_QTY { get; set; }
        public string? BQTY { get; set; }
        public string? COST { get; set; }
        public string? LOSS { get; set; }
        public string? SIG1 { get; set; }
        public string? SIG2 { get; set; }
        public string? SIG3 { get; set; }
        public string? SIG4 { get; set; }
        public string? REFERENCENO { get; set; }
        public string? TERM { get; set; }
        public string? EDIT_USER_ID { get; set; }
        public string? MENU_TERMS { get; set; }
        public string? TERMS { get; set; }
        public string? SUP_NAME { get; set; }
        public string? CLIENT_NAME { get; set; }
        public string? CLIENT_PO { get; set; }
        public string? CURRENCY { get; set; }
        public string? CURR{ get; set; }
        public string? USER { get; set; }
    }

    public class CustomMerchantPurchaseOrderDetailForPrintReport
    {
        public MerchantPurchaseOrderDetailRDLCReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
}