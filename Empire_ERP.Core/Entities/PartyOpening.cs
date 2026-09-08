using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
	public class PartyOpening
	{
		public int? OP_ID { get; set; }
		public string? BTYPE { get; set; }
		public string? BILL_NO { get; set; }
		public DateTime? BILL_DATE { get; set; }
		public string? DC_TYPE { get; set; }
		public double? AMOUNT { get; set; }
		public double? STAX_AMT { get; set; }
		public double? TAMT { get; set; }
		public double? COMM_AMT { get; set; }
		public int? TERMS { get; set; }
		public int? SALES_CODE { get; set; }
		public int? SALESACT_CODE { get; set; }
		public string? DDESC { get; set; }
		public int? PARTY_CODE { get; set; }
		public int? ACT_CODE { get; set; }
		public int? PARTYTYPE_CODE { get; set; }
		public int? CURR_CODE { get; set; }
		public double? CRATE { get; set; }
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
