using System.Data;

namespace Empire_ERP.Core.Entities
{
    public class MpoTrims
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? ASTATUS { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? REMARKS { get; set; }
        public string? JOB_NO { get; set; }
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
        public int? PICK_ID { get; set; }

    }

    public class MpoTrimsDetail
    {

        public int? TRAN_ID { get; set; }
        public int? DT_CODE { get; set; }
        public string? REV_STATUS { get; set; }
        public bool? REV_TOGGLE { get; set; }
        public int? REV_REF { get; set; }




        public DateTime? SHEET_REC_DATE { get; set; }
        public string? SHEET_REC_DOC { get; set; }
        public DateTime? PLAN_SUBM_DATE { get; set; }
        public DateTime? ACT_SUBD_DATE { get; set; }
        public string? ACT_SUBD_DOC { get; set; }
        public DateTime? ACT_APPR_DATE { get; set; }
        public DateTime? LABEL_DATE { get; set; }
        public DateTime? SNAP_DATE { get; set; }
        public DateTime? INLAY_DATE { get; set; }
        public DateTime? POLY_DATE { get; set; }
        public DateTime? STIFNER_DATE { get; set; }
        public DateTime? PLYBAG_DATE { get; set; }
        public DateTime? CART_STKR_DATE { get; set; }
        public DateTime? CART_INH_DATE { get; set; }



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

    }

    public class CustomMpoTrims
    {
        public MpoTrims? Master { get; set; }
        public List<MpoTrimsDetail>? Detail { get; set; }
    }

    public class MpoTrimsRDLCReport
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

    public class CustomMpoTrimsForPrintReport
    {
        public MpoTrimsRDLCReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
}