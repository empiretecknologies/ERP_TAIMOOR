using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class Approval
    {
        public int? Branch { get; set; }
        public int? CreditAccount { get; set; }
        public int? DebitAccount { get; set; }
        public int? PickId { get; set; }
        public string? RBCODE { get; set; }
        public string? RPERIOD_ID { get; set; }
        public string? REMARKS { get; set; }
        public string? PAGE_TYPE { get; set; }
        public int? TJV_TRANID { get; set; }
        public string? TABLER { get; set; }
        public string? MENU_ID { get; set; }
        public string? TRAN_ID { get; set; }
        public string? GROUP_CODE { get; set; }

        public string? ASTATUS { get; set; }
        public string? PERIOD_ID { get; set; }
        public string? ACT_NAME { get; set; }
        public string? BCODE { get; set; }
        public string? BOOK_TYPE { get; set; }
        public string? EDIT_USER_ID { get; set; }
        public string? VOUCHER_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }












    }

    public class ApprovalViewModel
    {
        public string? Amount { get; set; }
        public string? ToBranch { get; set; }
        public string? ToBranchAddress { get; set; }
        public string? FromBranch { get; set; }
        public string? FromBranchAddress { get; set; }
        public string? FromBranchPhone { get; set; }
        public string? ToBranchPhone{ get; set; }
        
    }
}
