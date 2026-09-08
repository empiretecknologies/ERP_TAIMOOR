using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class ItemAttribute
    {
        public int? CODE { get; set; }
        public int? ITEM_CODE { get; set; }
        public int? CAT_CODE { get; set; }
        public int? SUB_CAT_CODE { get; set; }
        public int? FABRIC { get; set; }
        public int? SEASON { get; set; }
        public int? BRAND { get; set; }
        public int? STYLE { get; set; }
    }
}