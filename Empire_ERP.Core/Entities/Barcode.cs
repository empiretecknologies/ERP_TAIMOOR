using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class Barcode
    {
        public int? CODE { get; set; }
        public string? BARCODE { get; set; }
        public int? BARCODE_TYPE { get; set; }
        public int? COLOR { get; set; }
        public string? SELECTEDCOLORS { get; set; }
        public int? SIZE { get; set; }
        public string? SELECTEDSIZES { get; set; }
        public double? PRATE { get; set; }
        public double? SRATE { get; set; }
        public double? WSALE { get; set; }
        public double? RRATE { get; set; }
        public double? DRATE { get; set; }
        public int? ITEM_CODE { get; set; }
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
        public int? BLABEL { get; set; }
    }
}