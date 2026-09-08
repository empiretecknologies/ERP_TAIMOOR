namespace Empire_ERP.Core.Entities
{
    public class Company
    {
        public int CCODE { get; set; }
        public string? C_NAME { get; set; }
        public string? C_ADDRESS { get; set; }
        public string? C_TEL { get; set; }
        public string? C_GST { get; set; }
        public string? C_NTN { get; set; }
        public int? BUS_NATURE { get; set; }
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
        public string? DLT { get; set; }
        public string? C_LOGO { get; set; }
        public string? RC_LOGO { get; set; }
        public string? C_WATER { get; set; }
    }
}