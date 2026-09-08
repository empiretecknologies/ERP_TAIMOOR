using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class PartyTypeBranch
    {
        public int CODE { get; set; }
        public int? PARTY_CODE { get; set; }
        public string? BRANCH_NAME { get; set; }
        public string? PARTY_NAME { get; set; }
        public string? CONTACT_PERSON { get; set; }
        public string? EMAIL { get; set; }
        public string? CEL { get; set; }
        public string? TEL { get; set; }
        public string? PB_ADD { get; set; }
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
        public int? ACT_CODE { get; set; }
        public string? ADD_USER_ID { get; set; }
        public string? DLT { get; set; }
    }
}