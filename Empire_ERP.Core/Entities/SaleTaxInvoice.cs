using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class SaleTaxInvoice
    {
        public int? TRAN_ID { get; set; }
        public string? ASTATUS { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? V_DATE_STRING { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? REF { get; set; }
        public int? PARTY_CODE { get; set; }
        public int? ACT_CODE { get; set; }
        public string? BTYPE { get; set; }
        public string? DOC { get; set; }
        public string? REMARKS { get; set; }
        public string? FBR_NO { get; set; }

        /// //fbr prop

        public string INVOICE_DATE { get; set; }
        public string SELLER_NTN { get; set; }
        public string SELLER_BNAME { get; set; }
        public string SELLER_PROVINCE { get; set; }
        public string SELLER_ADDRESS { get; set; }
        public string BUYER_NTN { get; set; }
        public string BUYER_BNAME { get; set; }
        public string BUYER_PROVINCE { get; set; }
        public string BUYER_ADDRESS { get; set; }
        public string BUYER_REG { get; set; }
        public string SCENARIO_ID { get; set; }
        public string BUYER_REG_TYPE { get; set; }

    }

    public class CustomSaleTaxInvoice
    {
        public SaleTaxInvoice? Master { get; set; }
        public List<SaleTaxInvoiceDetail>? Detail { get; set; }
    }


    public class SaleTaxInvoiceRDLCReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MD_NAME { get; set; }
        public string? INVOICE_NUMBER { get; set; }
        public string? DATE { get; set; }
        public string? HEADER_NAME { get; set; }
        public string? PARTY_NAME { get; set; }
        public string? ACCOUNT_NAME { get; set; }
        public string? MENU_TERMS { get; set; }
        public string? SIG1 { get; set; }
        public string? SIG2 { get; set; }
        public string? SIG3 { get; set; }
        public string? SIG4 { get; set; }
        public string? COMPANY_NAME { get; set; }
        public string? B_NAME { get; set; }
        public string? B_TERMS { get; set; }
        public string? PADDRESS { get; set; }
        public string? COMPANY_PHONE { get; set; }
        public string? B_WEBSITE { get; set; }
        public string? EMAIL { get; set; }
        public string? B_GST { get; set; }
        public string? B_NTN { get; set; }
        public string? COMPANY_LOGO { get; set; }
        public string? USER { get; set; }
        public string? DC_TYPE { get; set; }
        public string? BTYPE { get; set; }
        public string? TELL { get; set; }
        public string? PT_NTN { get; set; }
        public string? C_NAME { get; set; }
        public string? B_ADDRESS { get; set; }
        public string? B_TEL { get; set; }
        public string? STRN { get; set; }
        public string? FBR_NO { get; set; }
    }

    public class CustomSaleTaxInvoiceForPrintReport
    {
        public SaleTaxInvoiceRDLCReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }

    public class SaleTaxInvoiceDetail
    {
        public int? TRAN_ID { get; set; }
        public int? DT_CODE { get; set; }
        public int? ITEM_CODE { get; set; }
        public double? QTY { get; set; }
        public int? UNIT { get; set; }
        public double? RATE { get; set; }
        public double? AMT { get; set; }
        public double? DISC { get; set; }
        public double? DISC_AMT { get; set; }
        public double? TAX { get; set; }
        public double? TAX_AMT { get; set; }
        public double? NET_AMT { get; set; }
        public string? DT_DESC { get; set; }



        public string? HS_CODE { get; set; }
        public string? ITEM_NAME { get; set; }
        public string? UOM { get; set; }
        public string? SRO_SCH_NO { get; set; }
        public string? S_NAME { get; set; }
        public double? TOTAL_VALUES { get; set; }
        public double? VALUE_SALES_EXCLUDING { get; set; }
        public double? FIXEDVALUE_RETAILPRICE { get; set; }
        public double? ST_APPLICABLE { get; set; }
        public int? FBR_TYPE { get; set; }
        public string? ITEM_SNO { get; set; }
        public string? SCHEDULE_NO { get; set; }
        public int? SERIAL_NO { get; set; }
        
    }

    public class FBRPostResponse
    {
        public string invoiceNumber { get; set; }
        public ValidationResponse validationResponse { get; set; }
    }

    public class ValidationResponse
    {
        public string error { get; set; }
    }


}
