namespace Empire_ERP.Core.Entities
{
    public class HRJobPost
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? JOB_TITLE { get; set; }
        public int? DEPARTMENT { get; set; }
        public int? JOB_TYPE { get; set; }
        public int? EDUCATION { get; set; }
        public string? JOB_LOCATION { get; set; }
        public string? ASTATUS { get; set; }
        public string? MINIMUM_EXPERIENCE { get; set; }
        public string? MAXIMUM_EXPERIENCE { get; set; }
        public string? MINIMUM_SALARY { get; set; }
        public string? MAXIMUM_SALARY { get; set; }
        public string? SKILL { get; set; }
        public string? EMP_SHARE { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public int? BCODE { get; set; }
        public string? JOB_DESCRIPTION { get; set; }
        public string? DOCUMENT { get; set; }
        public string? JOB_RESPONSIBILITY { get; set; }
        public int? WEB_PUBLISH { get; set; }
    }

    public class HRJobPostReport
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