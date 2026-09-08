using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class DeliveryFeeding
    {
        public int? TRAN_ID { get; set; }
        public DateTime? V_DATE { get; set; }
        public string? VOUCHER_NO { get; set; }
        public string? SELLER_CODE { get; set; }
        public int? SACT_CODE { get; set; }
        public string? BUYER_CODE { get; set; }
        public int? BACT_CODE { get; set; }
        public string? REF { get; set; }
        public string? REMARKS { get; set; }
        public string? COND { get; set; }
        public string? BROKER_CODE { get; set; }


        public string? COB_CODE { get; set; }
        public int? COB_ACODE { get; set; }
        public int? CURR_CODE { get; set; }
        public int? SHIP_STATUS { get; set; }
        public decimal? CRATE { get; set; }
        public DateTime? SHIP_DATE { get; set; }
        public int? UNIT { get; set; }
        public string? SODA_TYPE { get; set; }
        public string? SBF_TYPE { get; set; }


        public int? BD_ACT_CODE { get; set; }
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
        public string? ASTATUS { get; set; }
        public int? MENU_ID { get; set; }
        public string? DLT { get; set; }
        public DateTime? SDATE { get; set; }
        public double? CREDIT_DAYS { get; set; }
        public DateTime? DUE_DATE { get; set; }
        public DateTime? DEL_DATE { get; set; }
        public double? BR_AMOUNT_BUYER { get; set; }
        public double? BR_AMOUNT_SELLER { get; set; }
        public double? WT_AMOUNT_BUYER { get; set; }
        public double? WT_AMOUNT_SELLER { get; set; }
        public double? BARDANA {  get; set; }
        public double? SBARDANA {  get; set; }
        public double? BGOD_CHARGES {  get; set; }
        public double? SGOD_CHARGES {  get; set; }
        public double? BLABOUR {  get; set; }
        public double? SLABOUR {  get; set; }
        public double? BFRIEGHT {  get; set; }
        public double? SFRIEGHT {  get; set; }
        public double? BFUMIGATION {  get; set; }
        public double? SFUMIGATION {  get; set; }
        public string? BR_AMOUNT_BUYER_ST { get; set; }
        public string? BR_AMOUNT_SELLER_ST { get; set; }
        public string? WT_AMOUNT_BUYER_ST { get; set; }
        public string? WT_AMOUNT_SELLER_ST { get; set; }
        public string? BARDANA_ST {  get; set; }
        public string? SBARDANA_ST {  get; set; }
        public string? BGOD_CHARGES_ST {  get; set; }
        public string? SGOD_CHARGES_ST {  get; set; }
        public string? BLABOUR_ST {  get; set; }
        public string? SLABOUR_ST {  get; set; }
        public string? BFRIEGHT_ST {  get; set; }
        public string? SFRIEGHT_ST {  get; set; }
        public string? BFUMIGATION_ST {  get; set; }
        public string? SFUMIGATION_ST {  get; set; }
        public double? B_NET_AMOUNT { get; set; }
        public double? S_NET_AMOUNT {  get; set; }
        public string? D_NAME { get; set; }
        public string? D_NUM { get; set; }
    }

    public class CustomDeliveryFeeding
    {
        public DeliveryFeeding? Master { get; set; }
        public List<DeliveryFeedingDetail>? Detail { get; set; }
    }
    public class DeliveryFeedingReport
    {
        public int? TRAN_ID { get; set; }
        public int? MD_ID { get; set; }
        public string? REPORT_NAME { get; set; }
        public string? MENU_SIG1 { get; set; }
        public string? MENU_TERMS { get; set; }
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
        public decimal? BARDANA { get; set; }
        public decimal? NET_AMOUNT { get; set; }
        public decimal? GOD_CHARGES { get; set; }
        public decimal? LABOUR { get; set; }
        public decimal? FRIEGHT { get; set; }
        public decimal? FUMIGATION { get; set; }
        public string? TRUCK_NO { get; set; }
        public string? COMMENT { get; set; }
        public string? BROKER_NAME { get; set; }
    }

    public class CustomDeliveryFeedingForPrintReport
    {
        public DeliveryFeedingReport? Master { get; set; }
        public DataTable? Detail { get; set; }
    }
}
