namespace Empire_ERP.Core.Entities
{
    public class ImportManifestDetail
    {
        public int? TRAN_ID { get; set; }
        public int? DT_CODE { get; set; }
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
        public int? MENU_ID { get; set; }
        public string? DLT { get; set; }
        public DateTime? D_DATE { get; set; }
        public string? SHIPPER_DDL { get; set; }
        public int? SHIP_CODE { get; set; }
        public int? SACT_CODE { get; set; }
        public string? IMPORTER_DDL { get; set; }
        public int? IMPORT_CODE { get; set; }
        public int? IACT_CODE { get; set; }
        public double? QTY { get; set; }
        public string? COMMENT { get; set; }
        public string? WAREHOUSE { get; set; }
        public string? LOT { get; set; }
    }
}
