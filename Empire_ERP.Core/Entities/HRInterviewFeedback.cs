namespace Empire_ERP.Core.Entities
{
    public class HRInterviewFeedback
    {
        public int? TRAN_ID { get; set; }
        public string? ASTATUS { get; set; }
        public int? EMP_ID { get; set; }
        public int? CON_ID { get; set; }
        public int? INT_ID { get; set; }
        public string? COMM_SKILL { get; set; }
        public string? TECH_SKILL { get; set; }
        public string? PROBLEM_SOLVED { get; set; }
        public string? CONF_SKILL { get; set; }
        public string? CUL_FIT { get; set; }
        public int? HR_EMAIL { get; set; }
        public string? OVERALL_REMARKS { get; set; }
        public string? FINAL_RECOM { get; set; }
    }

    public class HRInterviewFeedbackReport
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