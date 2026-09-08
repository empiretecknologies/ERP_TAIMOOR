using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class BarcodePrint
    {
        public int? ITEM_CODE { get; set; }

        public string? ITEM_NAME { get; set; }
        public string? RATE { get; set; }
        public string? ITEM_GROUP { get; set; }
        public string? ITEM { get; set; }
        public string? REMARKS { get; set; }
        public string? BARCODE { get; set; }
        public string? BARCODE_TEXT { get; set; }
        public string? SIZE_NAME { get; set; }
        public string? COLOR_NAME { get; set; }
        public double? SRATE { get; set; }
        public double? WSALE { get; set; }
        public double? RRATE { get; set; }
        public int? QTY { get; set; }
        public bool? ISSALE { get; set; }
        public bool? ISWHOLESALE { get; set; }
        public bool? ISRETAIL { get; set; }
        public int? BLABEL { get; set; }
        public string? CAT_CODE { get; set; }
        public string? ITEM_ID { get; set; }
    }

    public class BarcodePrintForRDLC
    {
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public List<BarcodePrint> data { get; set; }
    }
}