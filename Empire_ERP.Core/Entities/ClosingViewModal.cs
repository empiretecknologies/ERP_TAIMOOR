using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class ClosingViewModal
    {
        public string? Clogo { get; set; }
        public string? BName { get; set; }
        public string? BAddress { get; set; }
        public string? BPhone { get; set; }
        public string? BNTN { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Date { get; set; }
        public string? HeadingName { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? Time { get; set; }
        public string? CustomerName { get; set; }
        public string? ContactNumber { get; set; }
        public string? SalesmanName { get; set; }
        public string? CADD { get; set; }
        public float? RoundingDiscount { get; set; }
        public float? DiscountPercentage { get; set; }
        public double? TotalValue { get; set; }
        public float Payments { get; set; }
        public float? CashPaid { get; set; }
        public int? Bank { get; set; }
        public int? Party { get; set; }
        public int CashBack { get; set; }
        public string? FName { get; set; }
        public string? FTEL { get; set; }
        public string? FWebsite { get; set; }
        public string? TAX_CASH { get; set; }
        public string? BANK_TAX { get; set; }
        public string? PARTY_TAX { get; set; }
        public double? TAXCASH_AMT { get; set; }
        public double? CASHTAX_AMT { get; set; }
        public double? BANKTAX_AMT { get; set; }
        public double? PARTYTAX_AMT { get; set; }
        public float? TotalTax { get; set; }
        public float? TAX { get; set; }
        public int? TotalAmount { get; set; }
        public double? TotalQuantity { get; set; }
        public string? ClosingBalance { get; set; }
        public int? NoOfItems { get; set; }
        public string? TotalAmountFormatted { get; set; }
        public string? CashFormatted { get; set; }
        public string? BankFormatted { get; set; }
        public string? PartyFormatted { get; set; }
        public string? MENUTERMS { get; set; }

        public List<ClosingItemViewModel>? Items { get; set; }
        public List<ClosingItemGroupedViewModel> GroupedItems { get; set; }
    }
    public class ClosingItemViewModel
    {
        public string? Description { get; set; }
        public float? Balance { get; set; }
        public float? Rate { get; set; }
        public float? Qty { get; set; }
        public float? Disc { get; set; }
        public string? posType { get; set; }

    }
    public class ClosingItemGroupedViewModel
    {
        public string? PosType { get; set; }
        public List<ClosingItemViewModel> Items { get; set; }
        public double? TotalBalance { get; set; }
        public float? Qty { get; set; }
    }

}
