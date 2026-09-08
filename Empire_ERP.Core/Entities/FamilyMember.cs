namespace Empire_ERP.Core.Entities
{
    public class FamilyMember
    {
        public int TRAN_ID { get; set; }
        public int EMP_ID { get; set; }
        public string F_NAME { get; set; }
        public int? RELATION { get; set; }
        public DateTime? DOB { get; set; }
        public string GENDER { get; set; }
        public string CNIC { get; set; }
        public DateTime? CNIC_EXP { get; set; }
        public string MSTATUS { get; set; }
        public string INSTITUTE { get; set; }
        public string EDUCATION { get; set; }
        public string REMARKS { get; set; }
        public string ADD_USER_ID { get; set; }
        public DateTime? ADD_DATE { get; set; }
        public string ADD_COMPUTER_NAME { get; set; }
        public string ADD_IP_ADDRESS { get; set; }
        public string EDIT_USER_ID { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string EDIT_COMPUTER_NAME { get; set; }
        public string EDIT_IP_ADDRESS { get; set; }
        public string ADD_POSTALCODE { get; set; }
        public string EDIT_POSTALCODE { get; set; }
        public int MENU_ID { get; set; }
        public string DLT { get; set; }
        public string ASTATUS { get; set; }
    }

    public class OTPFamilyMember
    {
        public int? U_ID { get; set; }
        public string? EMAIL { get; set; }
        public int? OTP { get; set; }
        public string? STATUS { get; set; }
    }
}