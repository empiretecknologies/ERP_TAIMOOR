using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class CostCenter
    {
        public int GROUP_CODE { get; set; }
        public int COST_CENTER_ID { get; set; }
        public int PTRAN_ID { get; set; }
        public int PICK_ID { get; set; }
        public int PMENU_ID { get; set; }
        public string? DESCR { get; set; }
        public string? CC_NAME { get; set; }
        public decimal? AMOUNT { get; set; }
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
        public string? MENU_NAME { get; set; }

    }
}