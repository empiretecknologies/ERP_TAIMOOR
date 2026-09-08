using System;
using System.Collections.Generic;

namespace Empire_ERP.Core.Entities
{
    public class ClosingMasterModel
    {
        public string? FromDate { get; set; }      // Nullable
        public string? ToDate { get; set; }        // Nullable
        public decimal? closingBalance { get; set; } // Nullable
    }

    public class ClosingDetailModel
    {
        public string? acT_NAME { get; set; }
        public string? postype { get; set; }
        public decimal? balanced { get; set; }
        public string? qty { get; set; }
        public string? rate { get; set; }
        // Add more nullable fields as needed
    }

    public class SaveClosingRequest
    {
        public ClosingMasterModel? Master { get; set; }
        public List<ClosingDetailModel>? Detail { get; set; }
    }
}
