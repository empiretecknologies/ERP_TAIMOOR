using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class POSBookingItemDto
    {
        public string name { get; set; }
        public string mobile { get; set; }
        public int traN_ID { get; set; }
        public string voucherNo { get; set; }
        public string message { get; set; }
        public string WhatsappToken { get; set; }
        public string WhatsappUrl { get; set; }
    }
    public class POSBookingSummaryModel
    {
        public string VOUCHER_NO { get; set; }
        public decimal NET_TOTAL { get; set; }
        public decimal CASHTAX { get; set; }
        public decimal BANKTAX { get; set; }
        public decimal PARTYTAX { get; set; }
        public decimal ADVANCE { get; set; }
        public decimal ADVBANK { get; set; }
        public decimal PARTY { get; set; }
        public string CNAME { get; set; }
        public string MSG_DATE { get; set; }
        public string MSGDELDATE { get; set; }
        public string PAY_TYPE { get; set; }
        public decimal RETURN_AMT { get; set; }

        // Add more fields as needed later
    }

}
