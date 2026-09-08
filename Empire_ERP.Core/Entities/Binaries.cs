namespace Empire_ERP.Core.Entities
{
    public class Binaries
    {
        public string? S_SERVER_HOST { get; set; }
        public string? S_AUTH { get; set; }
        public string? S_USERNAME { get; set; }
        public string? S_PASS { get; set; }
        public string? S_DATABASE { get; set; }
        public string? DB_TYPE { get; set; }
        public string? D_SERVER_HOST { get; set; }
        public string? D_AUTH { get; set; }
        public string? D_USERNAME { get; set; }
        public string? D_PASS { get; set; }
        public string? D_DATABASE { get; set; }

        //public int BCODE { get; set; }
        //public string? B_NAME { get; set; }
        //public string? B_SHORT_NAME { get; set; }
        //public string? B_ADDRESS { get; set; }
        //public string? B_TEL { get; set; }
        //public string? B_GST { get; set; }
        //public string? B_NTN { get; set; }
        //public string? EMAIL { get; set; }
        //public int CCODE { get; set; }
        //public string? TIME_IN { get; set; }
        //public string? TIME_OUT { get; set; }
        //public string? CONTACT_NAME1 { get; set; }
        //public string? CONTACT_NO1 { get; set; }
        //public string? CONTACT_NAME2 { get; set; }
        //public string? CONTACT_NO2 { get; set; }
        //public string? CONTACT_NAME3 { get; set; }
        //public string? CONTACT_NO3 { get; set; }
        //public string? B_LOGO { get; set; }
        //public string? ADD_USER_ID { get; set; }
        //public DateTime? ADD_DATE { get; set; }
        //public string? ADD_COMPUTER_NAME { get; set; }
        //public string? ADD_IP_ADDRESS { get; set; }
        //public string? EDIT_USER_ID { get; set; }
        //public DateTime? EDIT_DATE { get; set; }
        //public string? EDIT_COMPUTER_NAME { get; set; }
        //public string? EDIT_IP_ADDRESS { get; set; }
        //public int MENU_ID { get; set; }
        //public string? ADD_POSTALCODE { get; set; }
        //public string? EDIT_POSTALCODE { get; set; }
        //public string? ASTATUS { get; set; }
        //public string? BARCODE { get; set; }
        //public string? RT_TYPE { get; set; }
        //public string? B_WEBSITE { get; set; }
        //public string? B_TYPE { get; set; }

    }

    public class TreeNode
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public List<TreeNode> children { get; set; } = new List<TreeNode>();
    }
}