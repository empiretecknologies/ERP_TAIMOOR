using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class Menu
    {
        public int ID { get; set; }
        public string? MENU_NAME { get; set; }
        public string? MENU_GRCODE { get; set; }
        public int? MENU_TYPE { get; set; }
        public int? MENU_PARENT_CODE { get; set; }
        public string? MENU_PAGE { get; set; }
        public string? TABLE1 { get; set; }
        public string? TABLE2 { get; set; }
        public string? PERFIX { get; set; }
        public int? PTYPE { get; set; }
        public string? VOUCHER_LEN { get; set; }
        public string? PICK_TABLE_MASTER { get; set; }
        public string? PICK_TABLE_DETAIL { get; set; }
        public string? DCTYPE { get; set; }
        public string? MENU_SIG1 { get; set; }
        public string? MENU_SIG2 { get; set; }
        public string? MENU_SIG3 { get; set; }
        public string? MENU_SIG4 { get; set; }
        public string? MENU_TERMS { get; set; }
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
        public string? DLT { get; set; }
        public string? MPIC { get; set; }
        public string? MTYPE { get; set; }
        public string? ASTATUS { get; set; }
        public string? SEARCH { get; set; }
        public string? B_I { get; set; }
        public string? STK_STATUS { get; set; }
        public string? PICK_TYPE { get; set; }
        public string? PICK_DATA { get; set; }
        public int? DATA_CLEAR { get; set; }
        public string? LIMIT { get; set; }
        public string? ITEM_TYPE { get; set; }

    }

    public class CustomMenuDetail
    {
        public string? MD_NAME { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MENU_SIG1 { get; set; }
        public string? MENU_SIG2 { get; set; }
        public string? MENU_SIG3 { get; set; }
        public string? MENU_SIG4 { get; set; }
        public int? MD_ID { get; set; }
        public string? MENU_TERMS { get; set; }
    }
}