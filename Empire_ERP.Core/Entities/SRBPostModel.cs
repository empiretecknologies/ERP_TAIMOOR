using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class SRBPostModel
    {
        public string? posId { get; set; }
        public string? name { get; set; }
        public string? ntn { get; set; }
        public string? invoiceID { get; set; }
        public string? invoiceDateTime { get; set; }
        public int? invoiceType { get; set; }
        public decimal? rateValue { get; set; }
        public decimal? saleValue { get; set; }
        public decimal? taxAmount { get; set; }
        public string? consumerName { get; set; }
        public string? consumerNTN { get; set; }
        public string? address { get; set; }
        public string? tariffCode { get; set; }
        public string? extraInf { get; set; }
        public string? pos_user { get; set; }
        public string? pos_pass { get; set; }
        public string? SrbUrl { get; set; }

        [JsonIgnore]
        public string? BillVoucher { get; set; }
    }
}
