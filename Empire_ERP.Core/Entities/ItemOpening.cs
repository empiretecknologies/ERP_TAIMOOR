using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
	public class ItemOpening
	{
		public int? OP_ID { get; set; }
		public int? ITEM_CODE { get; set; }
		public double? RATE { get; set; }
		public string? REF { get; set; }
		public string? BATCH { get; set; }
		public string? LOT { get; set; }
		public DateTime? MFG_DATE { get; set; }
		public DateTime? EXP_DATE { get; set; }
		public DateTime? BILL_DATE { get; set; }
		public double? QTY { get; set; }
		public int? UNIT { get; set; }
		public double? QTY2 { get; set; }
		public double? BAL_QTY { get; set; }
		public int? PACK_UNIT { get; set; }
		public int? COLOR { get; set; }
		public int? SIZE { get; set; }
		public int? GRADE { get; set; }
		public int? WAREHOUSE { get; set; }
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
		public int? CHK { get; set; }
	}
}
