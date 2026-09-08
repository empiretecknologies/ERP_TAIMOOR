namespace Empire_ERP.Core.Entities
{
    public class User
    {
        public int U_ID { get; set; }
        public int EmailCode { get; set; }
        public string USERNAME { get; set; }
        public string CELL_NO { get; set; }
        public string EMAIL { get; set; }
        public string UPASS { get; set; }
        public string CPASS { get; set; }
        public DateTime USTART_DATE { get; set; }
        public DateTime? ESTART_DATE { get; set; }
        public int BRANCH { get; set; }
        public string ADD_USER_ID { get; set; }
        public DateTime? ADD_DATE { get; set; }
        public string ADD_COMPUTER_NAME { get; set; }
        public string ADD_IP_ADDRESS { get; set; }
        public string EDIT_USER_ID { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string EDIT_COMPUTER_NAME { get; set; }
        public string EDIT_IP_ADDRESS { get; set; }
        public string FULLNAME { get; set; }
        public string ADD_POSTALCODE { get; set; }
        public string EDIT_POSTALCODE { get; set; }
        public string PICTURES { get; set; }
        public int MENU_ID { get; set; }
        public string DLT { get; set; }
        public int ROLEID { get; set; }
        public string ROLE_TYPE { get; set; }
        public string ASTATUS { get; set; }
        public string ST_ACTIVE { get; set; }
        public string OTP { get; set; }
        public string BRANCH_NAME { get; set; }
        public string ROLE_NAME { get; set; }
        public string MAC_ID { get; set; }
        public int SHOW_SELECTED { get; set; }
        public int MULTIPLE_LOGIN { get; set; }
    }

    public class OTPUser
    {
        public int? U_ID { get; set; }
        public string? EMAIL { get; set; }
        public int? OTP { get; set; }
        public string? STATUS { get; set; }
    }
}