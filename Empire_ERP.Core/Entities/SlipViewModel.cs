using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Empire_ERP.Core.Entities
{
    public class SlipViewModel
    {
        public string? Clogo { get; set; }
        public string? BName { get; set; }
        public string? BAddress { get; set; }
        public string? BPhone { get; set; }
        public string? BNTN { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? DelDate { get; set; }
        public string? CustomerName { get; set; }
        public string? ContactNumber { get; set; }
        public string? SalesmanName { get; set; }
        public string? UserName { get; set; }
        public string? CADD { get; set; }
        public string? Waiter { get; set; }
        public string? Table { get; set; }
        public double? RoundingDiscount { get; set; }
        public double? DiscountPercentage { get; set; }
        public double? TotalValue { get; set; }
        public double? Sett { get; set; }
        public double? Payments { get; set; }
        public double? CashPaid { get; set; }
        public double? Advance { get; set; }
        public double? TotalAdvance { get; set; }
        public double? ADVBANK { get; set; }
        public double? Balance { get; set; }
        public double? Bank { get; set; }
        public double? Cash { get; set; }
        public double? Party { get; set; }
        public double? CashBack { get; set; }
        public int? Complete { get; set; }
        public int? QRCode { get; set; }
        public int? PWindow { get; set; }
        public string? FName { get; set; }
        public string? FTEL { get; set; }
        public string? FWebsite { get; set; }
        public string? KotPrinter { get; set; }
        public string? P_PRINTER { get; set; }

        public string? StickerPrinter { get; set; }
        public double? TAX_CASH { get; set; }
        public double? BANK_TAX { get; set; }
        public double? PARTY_TAX { get; set; }
        public double? TAXCASH_AMT { get; set; }
        public double? CASHTAX_AMT { get; set; }
        public double? BANKTAX_AMT { get; set; }
        public double? PARTYTAX_AMT { get; set; }
        public float? TotalTax { get; set; }
        public float? ItemTax { get; set; }
        public float? TAX { get; set; }
        public int? TotalAmount { get; set; }
        public double? TotalQuantity { get; set; }
        public int? NoOfItems { get; set; }
        public string? TotalAmountFormatted { get; set; }
        public string? PARTY_MODE { get; set; }
        public string? CashFormatted { get; set; }
        public string? BankFormatted { get; set; }
        public string? PartyFormatted { get; set; }
        public string? MENUTERMS { get; set; }
        public string? SRBInvoiceId { get; set; }
        public string? FBRInvoiceId { get; set; }
        public string? Remarks { get; set; }
        public string? billmode { get; set; }
        public string? billStatus { get; set; }
        public string? Type { get; set; }
        public string? Waitername { get; set; }
        public string? TableNum { get; set; }
        public string? FBLINK { get; set; }
        public string? INSTALINK { get; set; }
        public string? WEBLINK { get; set; }
        public string? TIKTOKLINK { get; set; }
        public string? YOUTUBELINK { get; set; }
        public string? WIFIPASS { get; set; }
        public string? WIFINAME { get; set; }
        public string? SRBSTATUS { get; set; }
        public string? StickerLogo { get; set; }
        public string? LocationShortName { get; set; }
        public string? PAYTYPE { get; set; }
        public double? SER_CHARGES { get; set; }
        public double? Delivery { get; set; }
        public double? Del_Charges { get; set; }
        public string? CPC_CODE { get; set; }
        public string? JobName { get; set; }
        public string? PrintModeLabel { get; set; }

        public double? Amount { get; set; }
        public double? GrossTotal { get; set; }
        public double? itemDiscount { get; set; }

        public List<SlipItemViewModel>? Items { get; set; }
        public List<ExpenseViewModel>? Expense { get; set; }
    }
    public class SlipItemViewModel
    {

        public float? ItemTax { get; set; }

        public string? Description { get; set; }
        public string? KotPrinter { get; set; }
        public string? P_PRINTER { get; set; }

        public string? StickerPrinter { get; set; }
        public string? JobName { get; set; }
        public int? ItemCode { get; set; }
        public string? Remarks { get; set; }
        public string? TotalPrice { get; set; }
        public string? Disc_Per { get; set; }
        public double? Quantity { get; set; }
        public double? Price { get; set; }
        public double? Discount { get; set; }
        public double? Disc_AMt { get; set; }
        public double? Amount { get; set; }
        public double? TAX_Per { get; set; }
        public double? TAX_AMT { get; set; }
        public string? BARCODE { get; set; }

    }
    public class ExpenseViewModel
    {
        public string? Description { get; set; }
        public double? Amount { get; set; }

    }
}
