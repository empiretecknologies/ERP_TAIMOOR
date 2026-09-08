using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class PurchaseOrder
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public DateTime? RINV_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? PARTY_CODE { get; set; }
        public int? ACT_CODE { get; set; }
        public int? DEP { get; set; }
        public string? ORDER_TYPE { get; set; }
        public string? DC_NO { get; set; }
        public int? TERMS { get; set; }
        public string? SCODE { get; set; }
        public string? RINV_NO { get; set; }
        public int? SACODE { get; set; }
        public string? COMM_AMT { get; set; }
        public double? COMM { get; set; }
        public double? COMM_VAL { get; set; }
        public string? COMM_TYPE { get; set; }
        public string? REF { get; set; }
        public string? BTYPE { get; set; }
        public string? DISC { get; set; }
        public string? TRANSPORT_TYPE { get; set; }
        public DateTime? REF_DATE { get; set; }
        public int? CURR_CODE { get; set; }
        public double? CRATE { get; set; }
        public int? WAREHOUSE { get; set; }
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
        public string? DLT { get; set; }
        public string? DOC { get; set; }
    }

    public class CustomPurchaseOrder
    {
        public PurchaseOrder? Master { get; set; }
        public List<PurchaseOrderDetail>? Detail { get; set; }
    }

    public class PurchaseOrderRDLCReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? INVOICE_NUMBER { get; set; }
        public string? DATE { get; set; }
        public string? DEL_DATE { get; set; }
        public string? BT_CUSTOMER { get; set; }
        public string? PADDRESS { get; set; }
        public string? PNTN { get; set; }
        public string? RINV_NO { get; set; }

        public string? HEADER_NAME { get; set; }
        public string? COMMENT { get; set; }
        public string? STATUS { get; set; }
        public string? SIG1 { get; set; }
        public string? SIG2 { get; set; }
        public string? SIG3 { get; set; }
        public string? SIG4 { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? COMPANY_LOGO { get; set; }
        public string? RCOMPANY_LOGO { get; set; }
        public string? USER { get; set; }
        public string? MENU_TERMS { get; set; }
        public string? B_NAME { get; set; }
        public string? B_TERMS { get; set; }
        public string? TERMS { get; set; }

        public string? EMAIL { get; set; }
        public string? B_GST { get; set; }
        public string? B_NTN { get; set; }
        public string? B_WEBSITE { get; set; }
        public string? ORDER_TYPE { get; set; }
        public string? PARTY_NAME { get; set; }
        public string? REF { get; set; }









    }

    public class CustomPurchaseOrderForPrintReport
    {
        public PurchaseOrderRDLCReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
}
