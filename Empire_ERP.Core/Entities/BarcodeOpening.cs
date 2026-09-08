using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class BarcodeOpening
	{
		public int? OP_ID { get; set; }
		public int? ITEM_OP_ID { get; set; }
		public int? BARCODE { get; set; }
		public double? QTY { get; set; }
		public double? RATE { get; set; }
		public int? UNIT { get; set; }
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
	}
}