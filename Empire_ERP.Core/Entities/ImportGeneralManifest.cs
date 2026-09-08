using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class ImportGeneralManifest
    {
        public int? TRAN_ID { get; set; }
        public string? ARIVAL_STATUS { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? SELLER_CODE { get; set; }
        public int? SACT_CODE { get; set; }
        public string? BUYER_CODE { get; set; }
        public int? BACT_CODE { get; set; }
        public string? REF { get; set; }
        public string? REMARKS { get; set; }
        public int? BCODE { get; set; }
        public int? PERIOD_ID { get; set; }
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

    public class CustomImportGeneralManifest
    {
        public ImportGeneralManifest? Master { get; set; }
        public List<ImportGeneralManifestDetail>? Detail { get; set; }
    }
}
