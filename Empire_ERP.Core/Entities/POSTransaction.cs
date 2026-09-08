using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class POSTransaction
    {
        public decimal? TRAN_ID { get; set; }
        public string? V_DATE { get; set; }
        public int MD_ID { get; set; }
        public int TaxPer { get; set; }
        public int TaxVal { get; set; }


        public string? VOUCHER_NO { get; set; }
        public string? SRB_VN { get; set; }
        public string? BOOK_TYPE { get; set; }
        public int? BCODE { get; set; }
        public int? PERIOD_ID { get; set; }
        public string? CNAME { get; set; }
        public string? CMOB { get; set; }
        public string? INV_STATUS { get; set; }
        public float TOTAL { get; set; }
        public float? DISC { get; set; }
        public float? SETT { get; set; }
        public string? SETTL_SIGN { get; set; }
        public float NET_TOTAL { get; set; }
        public float CASH { get; set; }
        public float ADVANCE { get; set; }
        public float ADV_BANK { get; set; }
        public int CASH_BACK { get; set; }
        public int? CACT_CODE { get; set; }
        public float RECV { get; set; }
        public float DEL { get; set; }
        public float DEL_CHARGES { get; set; }
        public float DISC_AMT { get; set; }
        public float ITEM_DISCOUNT { get; set; }
        public float TAX { get; set; }
        public float TAX_PER { get; set; }
        public float TAX_AMT { get; set; }
        public string? SALESMAN { get; set; }
        public string? SACT_CODE { get; set; }
        public string? SALESMANNAME { get; set; }
        public string? USERNAME { get; set; }
        public float? COMMISION { get; set; }
        public float CASHTAX_AMT { get; set; }
        public float BANKTAX_AMT { get; set; }
        public float PARTYTAX_AMT { get; set; }
        public string? BILL_STATUS { get; set; }
        public string? CADD { get; set; }
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
        public int? BACT_CODE { get; set; }
        public int? ADV_BOOK_TYPE { get; set; }
        public float? PARTY { get; set; }
        public float? BANK { get; set; }
        public int? PARTY_CODE { get; set; }
        public string? PARTY_NAME { get; set; }
        public string? PARTY_NUMBER { get; set; }
        public int? ACT_CODE { get; set; }
        public string? DUE_DATE { get; set; }
        public string? DEL_DATE { get; set; }
        public string? BCHARGES { get; set; }
        public string? CASH_TAX { get; set; }
        public string? BANK_TAX { get; set; }
        public string? PARTY_TAX { get; set; }
        public string? SRBInvoiceId { get; set; }
        public string? SRBSTATUS { get; set; }
        public string? FBRInvoiceId { get; set; }
        public string? REMARK { get; set; }
        public string? WAITERNAME { get; set; }
        public string? TABLENUM { get; set; }
        public string? BILLMODE { get; set; }
        public string? CARD_NO { get; set; }
        //ADD POINTS
        public int? PREV_POINTS { get; set; }
        public int? CURR_POINTS { get; set; }

        public int? WAITER { get; set; }
        public int? TABLE { get; set; }
        public int? COMPLETE { get; set; }
        public int? SER_CHARGES { get; set; }
        public int? CardDiscValue { get; set; }
        public int PRINT_COPIES { get; set; }

        public int? PWINDOW { get; set; }
        public string? PAY_TYPE { get; set; }
        public bool? Return { get; set; }
    }
    public class CustomPOSTransaction
    {
        public POSTransaction? Master { get; set; }
        public List<POSTransactionDetail>? Detail { get; set; }
    }
    public class POSTransactionReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? INVOICE_NUMBER { get; set; }
        public string? SODA_DATE { get; set; }
        public string? DELIVERY_DATE { get; set; }
        public string? CONDITION { get; set; }
        public string? PARTY_NAME { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? COMPANY_ADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? COMPANY_LOGO { get; set; }
        public string? HEADER_NAME { get; set; }
        public decimal? WT_AMOUNT { get; set; }
        public decimal? BR_AMOUNT { get; set; }
        public decimal? NET_AMOUNT { get; set; }
        public string? TRUCK_NO { get; set; }
    }

    public class GetPOSTransactionById
    {
        public int TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? CNAME { get; set; }
        public string? CMOB { get; set; }
        public string? CADD { get; set; }
        public string? INV_STATUS { get; set; }
        public string? BILL_STATUS { get; set; }
        public float TOTAL { get; set; }
        public float? DISC { get; set; }
        public float DISC_AMT { get; set; }
        public float NET_TOTAL { get; set; }
        public float CASH { get; set; }
        public string? BANK { get; set; }
        public string? PARTY { get; set; }
        public int? PARTY_CODE { get; set; }
        public int? BCODE { get; set; }
        public float DEL { get; set; }
        public float DEL_CHARGES { get; set; }
        public int? ACT_CODE { get; set; }
    }

    public class CustomPOSTransactionForPrintReport
    {
        public POSTransactionReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
    public class Item
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
    }

    public class POSPrint_Model
    {
        public decimal? tranId { get; set; }
        public string? BillStatus { get; set; }
        public string? DT_Code { get; set; }
    }
}
