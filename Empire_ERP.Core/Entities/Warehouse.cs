using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class Warehouse
    {
        public int? CODE { get; set; }
        public string? DESCR { get; set; }
        public string? GROUP_TYPE { get; set; }
        public int? PARENT_CODE { get; set; }
        public string? CONTACT_PERSON { get; set; }
        public string? TELL { get; set; }
        public string? CELL { get; set; }
        public string? ADDR { get; set; }
        public int? BCODE { get; set; }
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
        public string? GR_CODE { get; set; }
        public string? DLT { get; set; }
    }
}
