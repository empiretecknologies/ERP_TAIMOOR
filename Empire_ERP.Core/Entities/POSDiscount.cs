using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class POSDiscount
    {
        public int? CODE { get; set; }
        public string? DESCR { get; set; }
        public int? BCODE { get; set; }
        public string? SELECTEDBRANCHES { get; set; }
        public DateTime? FDATE { get; set; }
        public DateTime? TDATE { get; set; }
        public double? DISC { get; set; }
        public int? DISC_EXP { get; set; }
        public int? ITEM_CODE { get; set; }
        public string? SELECTEDITEMS { get; set; }
        public int? ITEM_GROUP { get; set; }
        public string? SELECTEDITEMGROUPS { get; set; }
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
        public int? MENU_ID { get; set; }
        public string? ASTATUS { get; set; }
        public string? DLT { get; set; }
    }
}
