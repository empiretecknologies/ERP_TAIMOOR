using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class BatchIssue
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public int? ITEM_CODE { get; set; }
        public string? REF { get; set; }
        public string? BATCH_NAME { get; set; }
        public double? COST { get; set; }
        public int? PROCESS { get; set; }
        public int? UNIT { get; set; }
        public double? BQTY { get; set; }
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
        public string? BATCHNO { get; set; }
        public DateTime? MFGDATE { get; set; }
        public DateTime? EXPDATE { get; set; }
    }

    public class CustomBatchIssue
    {
        public BatchIssue? Master { get; set; }
        public List<BatchIssueDetail>? Detail { get; set; }
    }

    public class BatchIssueRDLCReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? INVOICE_NUMBER { get; set; }
        public string? DATE { get; set; }
        public string? MFG_DATE { get; set; }
        public string? EXP_DATE { get; set; }
        public string? MITEM_NAME { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? B_NAME { get; set; }
        public string? B_ADDRESS { get; set; }
        public string? B_TEL { get; set; }
        public string? STRN { get; set; }
        public string? B_NTN { get; set; }
        public string? B_TERMS { get; set; }
        public string? COMPANY_LOGO { get; set; }
        public string? COMPANY_WATER { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? REF { get; set; }
        public string? M_ITEM { get; set; }
        public string? BATCH_NO { get; set; }
        public string? M_UNIT { get; set; }
        public string? PROCESS { get; set; }
        public string? REMARKS { get; set; }
        public string? BQTY { get; set; }
        public string? COST { get; set; }
        public string? LOSS { get; set; }
        public string? SIG1 { get; set; }
        public string? SIG2 { get; set; }
        public string? SIG3 { get; set; }
        public string? SIG4 { get; set; }
        public string? B_WEBSITE { get; set; }
        public string? EMAIL { get; set; }
        public string? REFERENCENO { get; set; }
        public string? TERM { get; set; }
        public string? EDIT_USER_ID { get; set; }
        public string? MENU_TERMS { get; set; }
        public string? USER_NAME { get; set; }
    }

    public class CustomBatchIssueForPrintReport
    {
        public BatchIssueRDLCReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
}