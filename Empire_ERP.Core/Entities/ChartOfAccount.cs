using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class ChartOfAccount
    {
        public int ACT_CODE { get; set; }
        public string? ACT_NAME { get; set; }
        public string? ACT_SNAME { get; set; }
        public string? ACT_TYPE { get; set; }
        public int? ACT_PARENT_CODE { get; set; }
        public string? ACT_GR_CODE { get; set; }
        public int? ACT_GROUP { get; set; }
        public int? ACT_NATURE { get; set; }
        public int? COSTCENTER { get; set; }
        public string? PREFIX { get; set; }
        public string? ACCOUNT_NO { get; set; }
        public string? TITTLE { get; set; }
        public string? SWIFT { get; set; }
        public int? CHQ_ID { get; set; }
        public int? CURRENCY { get; set; }
        public string? ADD_USER_ID { get; set; }
        public DateTime? ADD_DATE { get; set; }
        public string? ADD_COMPUTER_NAME { get; set; }
        public string? ADD_IP_ADDRESS { get; set; }
        public string? EDIT_USER_ID { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string? EDIT_COMPUTER_NAME { get; set; }
        public string? EDIT_IP_ADDRESS { get; set; }
        public int? MENU_ID { get; set; }
        public string? ADD_POSTALCODE { get; set; }
        public string? EDIT_POSTALCODE { get; set; }
        public string? ASTATUS { get; set; }
        public string? DLT { get; set; }
        public string? PASS { get; set; }

        public int? CHART_TYPE { get; set; }
        public string? CHART_NAME { get; set; }
    }
}