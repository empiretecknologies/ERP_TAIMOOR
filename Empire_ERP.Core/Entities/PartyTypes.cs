namespace Empire_ERP.Core.Entities
{
    public class PartyTypes
    {
        public int PARTY_CODE { get; set; }
        public int? PARTY_TYPE_CODE { get; set; }
        public string? PARTY_NAME { get; set; }
        public string? PARTY_SHORT_NAME { get; set; }
        public int? ACT_CODE { get; set; }
        public int? CAT_CODE { get; set; }
        public string? PADDRESS { get; set; }
        public string? NTN { get; set; }
        public string? CNIC { get; set; }
        public string? CONTACT_PERSON { get; set; }
        public string? CELL { get; set; }
        public string? WB { get; set; }
        public string? TELL { get; set; }
        public string? EMAIL { get; set; }
        public string? WEBSITE { get; set; }
        public DateTime? CNIC_EXP { get; set; }
        public string? REMARKS { get; set; }
        public int? PAYMENT_TERMS { get; set; }
        public double? CREDIT_LIMIT { get; set; }
        public int? S_CODE { get; set; }
        public int? SACT_CODE { get; set; }
        public string? GST { get; set; }
        public string? SERVICE_TAX { get; set; }
        public string? F_CODE { get; set; }
        public int? ENTITY { get; set; }
        public string? ASTATUS { get; set; }
        public string? ACCOUNT_NUM { get; set; }
        public double? TAX { get; set; }
        public string? BANK_NAME { get; set; }
        public string? BRANCH_NAME { get; set; }
        public string? BANK_ADDRESS { get; set; }
        public string? DOC_PIC { get; set; }
        public string? CNIC_PIC { get; set; }
        public string? TERMS_CONDITION { get; set; }
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
        public int? REGION { get; set; }
        public string? ADD_USER_ID { get; set; }
        public double? COMM { get; set; }
        public double? DISC { get; set; }
        public string? DLT { get; set; }
        public string? T_CAT { get; set; }
        public string? WHT { get; set; }
        public DateTime? EXEMPT_DATE { get; set; }



    }

    public class CustomPartyType
    {
        public int key { get; set; }
        public string? customizedKey { get; set; }
        public string? customKey { get; set; }
        public string? value { get; set; }
        public string? regionCode { get; set; }
        public string? disc { get; set; }
        public string? scode { get; set; }
        public int accountCode { get; set; }
        public int paymentTerms { get; set; }
        public int commission { get; set; }
    }
}