using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class ItemGroup
    {
        public int GROUP_CODE { get; set; }
        public string? GROUP_NAME { get; set; }
        public string? GROUP_NAME2 { get; set; }
        public string? GROUP_TYPE { get; set; }
        public string? K_PRINTER { get; set; }
        public int ASETUP { get; set; }
        public int ITEM_TYPE { get; set; }
        public int SALES_TAX { get; set; }
        public string? IPIC { get; set; }
        public string? ADD_USER_ID { get; set; }
        public DateTime? ADD_DATE { get; set; }
        public string? ADD_COMPUTER_NAME { get; set; }
        public string? ADD_IP_ADDRESS { get; set; }
        public string? EDIT_USER_ID { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string? EDIT_COMPUTER_NAME { get; set; }
        public string? EDIT_IP_ADDRESS { get; set; }
        public int MENU_ID { get; set; }
        public int ADD_POSTALCODE { get; set; }
        public int EDIT_POSTALCODE { get; set; }
        public string? ASTATUS { get; set; }
        public bool DLT { get; set; }
        public string? GR_CODE { get; set; }
        public string? PARENT_CODE { get; set; }
        public string? STICKER { get; set; }
        public string? STK_PRINTER { get; set; }


    }
}