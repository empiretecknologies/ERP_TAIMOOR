namespace Empire_ERP.Core.Entities
{
    public class Period
    {
        public int PID { get; set; }
        public DateTime? START_D { get; set; }
        public DateTime? START_E { get; set; }
        public string? DESCR { get; set; }
        public int? BCODE { get; set; }
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
        public int? CLOSING { get; set; }
        public string? DLT { get; set; }
        public string? ASTATUS { get; set; }
    }
}