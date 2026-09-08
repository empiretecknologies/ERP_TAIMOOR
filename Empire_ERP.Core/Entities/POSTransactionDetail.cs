using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class POSTransactionDetail
    {
        public int? TRAN_ID { get; set; }
        public int? DT_CODE { get; set; }
        public int? PICK_ID { get; set; }
        public int? ITEM_CODE { get; set; }
        public string? ITEM_NAME { get; set; }
        public double? QTY { get; set; }
        public int? UNIT { get; set; }
        public int? ISKOTPRINT { get; set; }
        public double? RATE { get; set; }
        public double? AMT { get; set; }
        public float? TAX { get; set; }
        public float? TAX_PER { get; set; }
        public int? TAX_AMT { get; set; }
        public double? DISC { get; set; }
        public double? DISC_AMT { get; set; }
        public double? NET_AMT { get; set; }
        public string? BARCODE { get; set; }
        public string? REMARKS { get; set; }
        public int? RITEM { get; set; }
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
        public string? COLOR { get; set; }
        public string? SIZE { get; set; }
        public int? MENU_ID { get; set; }
        public string? DLT { get; set; }
    }
}