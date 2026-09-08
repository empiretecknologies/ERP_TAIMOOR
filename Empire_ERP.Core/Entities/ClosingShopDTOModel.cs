using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class ClosingShopDTOModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string SelectedBranches { get; set; }
        public string SelectedItems { get; set; }
        public string SelectedItemGroups { get; set; }
        public string DiscountPercent { get; set; }
        public string DiscountExpired { get; set; }
    }
}
