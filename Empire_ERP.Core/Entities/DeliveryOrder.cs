using System.Data;

namespace Empire_ERP.Core.Entities
{
    public class DeliveryOrder
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public int? PARTY_CODE { get; set; }
        public int? ACT_CODE { get; set; }
        public int? SCODE { get; set; }
        public int? SACODE { get; set; }
        public int? DEL_CODE { get; set; }
        public int? DEL_ACODE { get; set; }
        public string? REF { get; set; }
        public string? REMARKS { get; set; }
        public string? BATCH_NO { get; set; }
        public string? DRIVER_NAME { get; set; }
        public string? BTYPE { get; set; }
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

    public class CustomDeliveryOrder
    {
        public DeliveryOrder? Master { get; set; }
        public List<DeliveryOrderDetail>? Detail { get; set; }
    }

    public class DeliveryOrderRDLCReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? INVOICE_NUMBER { get; set; }
        public string? DATE { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? COMMENT { get; set; }
        public string? PARTY { get; set; }
        public string? SALESMAN { get; set; }
        public string? DELIVERYMAN { get; set; }
        public string? REFERENCE { get; set; }
        public string? DRIVER_NAME { get; set; }
        public string? BATCH_NO { get; set; }
        public string? SIG1 { get; set; }
        public string? SIG2 { get; set; }
        public string? SIG3 { get; set; }
        public string? SIG4 { get; set; }
        public string? USER { get; set; }

    }

    public class CustomDeliveryOrderForPrintReport
    {
        public DeliveryOrderRDLCReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
}
