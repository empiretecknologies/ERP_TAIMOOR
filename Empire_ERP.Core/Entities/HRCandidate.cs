namespace Empire_ERP.Core.Entities
{
    public class HRCandidate
    {
        public int? TRAN_ID { get; set; }
        public string? ASTATUS { get; set; }
        public DateTime? V_DATE { get; set; }
        public DateTime? APPLY_DATE { get; set; }
        public int? JOB { get; set; }
        public string? NAME { get; set; }
        public string? EMAIL { get; set; }
        public string? PHONE { get; set; }
        public string? GENDER { get; set; }
        public DateTime? DOB { get; set; }
        public decimal? EXP_YEAR { get; set; }
        public int? EDUCATION { get; set; }
        public string? ADDRESS { get; set; }
        public string? REMARKS { get; set; }
        public string? DOC { get; set; }
    }

    public class HRCandidateReport
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