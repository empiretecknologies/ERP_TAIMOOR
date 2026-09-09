using System.Data;

namespace Empire_ERP.Core.Entities
{
    public class SalesQutation
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public int? PARTY_CODE { get; set; }
        public int? ACT_CODE { get; set; }
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
    }

    public class CustomSalesQutation
    {
        public SalesQutation? Master { get; set; }
        public List<SalesQutationDetail>? Detail { get; set; }
    }

    public class SalesQutationForPrint
    {
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? REMARKS { get; set; }
        public string? PARTY_NAME { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? COMPANY_LOGO { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? REPORT_NAME { get; set; }
        public bool? MENU_SIG1 { get; set; }
        public bool? MENU_SIG2 { get; set; }
        public bool? MENU_SIG3 { get; set; }
        public bool? MENU_SIG4 { get; set; }
    }

    public class CustomSalesQutationForPrintReport
    {
        public SalesQutationForPrint? Master { get; set; }
        public DataTable Detail { get; set; }
    }
}
