using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class FBRPostModel
    {
        public string? invoiceType { get; set; }
        public string? invoiceDate { get; set; }

        public string? sellerNTNCNIC { get; set; }
        public string? sellerBusinessName { get; set; }
        public string? sellerProvince { get; set; }
        public string? sellerAddress { get; set; }

        public string? buyerNTNCNIC { get; set; }
        public string? buyerBusinessName { get; set; }
        public string? buyerProvince { get; set; }
        public string? buyerAddress { get; set; }
        public string? buyerRegistrationType { get; set; }

        public string? invoiceRefNo { get; set; }
        public string? scenarioId { get; set; }

        public List<FBRItemModel>? items { get; set; }

        //public string? fbrUrl { get; set; }
    }

    public class FBRItemModel
    {
        public string? hsCode { get; set; }
        public string? productDescription { get; set; }
        public string? rate { get; set; }
        public string? uoM { get; set; }
        public int? quantity { get; set; }
        public decimal? totalValues { get; set; }
        public decimal? valueSalesExcludingST { get; set; }
        public decimal? fixedNotifiedValueOrRetailPrice { get; set; }
        public decimal? salesTaxApplicable { get; set; }
        public decimal? salesTaxWithheldAtSource { get; set; }
        public decimal? extraTax { get; set; }
        public decimal? furtherTax { get; set; }
        public string? sroScheduleNo { get; set; }
        public decimal? fedPayable { get; set; }
        public decimal? discount { get; set; }
        public string? saleType { get; set; }
        public string? sroItemSerialNo { get; set; }
    }

    public class FBRUrlToken
    {
        public string Url { get; set; }
        public string Token { get; set; }
    }

    public class FBRModel
    {
        public FBRPostModel FBRPostModel { get; set; }
        public FBRUrlToken FBRUrlToken { get; set; }
    }

}
