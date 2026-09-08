namespace Empire_ERP.Core.Entities
{
    public class Role
    {
        public int? GROUP_CODE { get; set; }
        public string? GROUP_NAME { get; set; }
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
        public double? QTY { get; set; }
        public int? SHOW_SELECTED { get; set; }
    }

    public class CustomRole{
        public int? ROLE_ID { get; set; }
        public string? ROLE_NAME { get; set; }
        public string? ROLE_TYPE { get; set; }
        public string? ASTATUS { get; set; }
        public string? SHOW_SELECTED { get; set; }
        public int[]? BRANCH { get; set; }
        public List<PermissionList>? PERMISSIONS { get; set; }
        public List<MENU_SELECTIONS>? MENU_SELECTIONS { get; set; }
    }
    public class MENU_SELECTIONS
    {
        public int? isSelected { get; set; }
        public string? MenuId { get; set; }
        public string? Module_Id { get; set; }


    }
    public class PermissionList
    {
        public int? ID { get; set; }
        public int? Menu_Id { get; set; }
        public bool IsSelected { get; set; }

        public int? MENU_PARENT_CODE { get; set; }
        public string? GRCODE { get; set; }
        public string? MENU_NAME { get; set; }
        public string? MTYPE { get; set; }
        public int MODULE_ID { get; set; }
        public int ACT_CODE { get; set; }
        public bool ROWSELECTION { get; set; }
        public bool ADD { get; set; }
        public bool EDIT { get; set; }
        public bool VIEW { get; set; }
        public bool DELETE { get; set; }
        public bool PRINT { get; set; }
        public bool COPY { get; set; }
    }
}
