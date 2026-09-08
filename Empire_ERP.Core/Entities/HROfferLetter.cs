using System.Data;

namespace Empire_ERP.Core.Entities
{
    public class HROfferLetter
    {
        public int? TRAN_ID { get; set; }
        public int? INT_ID { get; set; }
        public string? JOB_TITLE { get; set; }
        public int? DEP_ID { get; set; }
        public decimal? BASIC_SALARY { get; set; }
        public decimal? ALLOWANCES { get; set; }
        public decimal? TOTAL_PACKAGE { get; set; }
        public DateTime? JOINING_DATE { get; set; }
        public DateTime? OFFER_DATE { get; set; }
        public TimeSpan? DUTY_START { get; set; }
        public TimeSpan? DUTY_END { get; set; }
        public string? WEEKLY_OFF { get; set; }
        public string? WEEKLY_DESC { get; set; }
        public string? DOC { get; set; }
        public string? ASTATUS { get; set; }
    }

    public class HROfferLetterReport
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
        public string? CAN_NAME { get; set; }
        public string? JOB_TITLE { get; set; }
        public string? DEP_NAME { get; set; }
        public string? BASIC_SALARY { get; set; }
        public string? ALLOWANCES { get; set; }
        public string? TOTAL_PACKAGE { get; set; }
        public string? JOINING_DATE { get; set; }
        public string? OFFER_DATE { get; set; }
        public string? DUTY_START { get; set; }
        public string? DUTY_END { get; set; }
        public string? WEEKLY_OFF { get; set; }
        public string? WEEKLY_DESC { get; set; }
        public string? SIG1 { get; set; }
        public string? SIG2 { get; set; }
        public string? SIG3 { get; set; }
        public string? SIG4 { get; set; }
    }

    public class CustomHROfferLetterForPrintReport
    {
        public HROfferLetterReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
}