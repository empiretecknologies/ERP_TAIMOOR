using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class ItemMaster
    {
        public int? ITEM_CODE { get; set; }
        public string? HS_CODE { get; set; }
        public int? BITYPE { get; set; }
        public string? ITEM_ID { get; set; }
        public string? ITEM_NAME { get; set; }
        public string? BARCODE { get; set; }
        public string? ITEM_SHORT_NAME { get; set; }
        public string? REMARKS { get; set; }
        public int? GROUP_CODE { get; set; }
        public int? IUNIT_CODE { get; set; }
        public int? SIZE { get; set; }
        public int? COLOR { get; set; }
        public string? PACK { get; set; }
        public int? PUNIT_CODE { get; set; }
        public double? SALE_RATE { get; set; }
        public double? WEIGHT { get; set; }

        public double? PURCHASE_RATE { get; set; }
        public double? SALESTAX { get; set; }
        public int? ITAX_STATUS { get; set; }
        public double? ITEM_MAX { get; set; }
        public double? ITEM_MIN { get; set; }
        public string? IPIC { get; set; }
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
        public int? GRADE { get; set; }
        public string? ITEM_TYPE { get; set; }
        public int? CAT_CODE { get; set; }
        public int? SUB_CAT_CODE { get; set; }
    }
}