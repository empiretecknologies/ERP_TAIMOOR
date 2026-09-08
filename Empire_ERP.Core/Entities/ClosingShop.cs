using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class ClosingShop
    {
        public ClosingshopMaster? Master { get; set; }
        public List<DetailClosingShop>? Detail { get; set; }
    }
    public class ClosingshopMaster
    {
        public int? MD_ID { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public double? ClosingBalance { get; set; }
    }
    public class DetailClosingShop
    {
        public string? acT_NAME { get; set; }
        public float? balanced { get; set; }
        public float? qty { get; set; }
        public float? Rate { get; set; }
        public float? disc { get; set; }
        public string? postype { get; set; }
    }
}
