using System.Data;

namespace Empire_ERP.Core.Entities
{
    public class ListPrintReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? COMPANY_LOGO { get; set; }
    }

    public class ListMultiBillPrintReport
    {
        public int[] TRAN_IDS { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? COMPANY_LOGO { get; set; }
    }

    public class CustomMultiBillPrintReport
    {
        public ListPrintReport? Master { get; set; }
        public DataTable?[]? Detail { get; set; }
    }

    public class CustomPrintReport
    {
        public ListPrintReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
}
