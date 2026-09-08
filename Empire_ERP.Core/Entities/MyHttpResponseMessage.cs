namespace Empire_ERP.Core.Entities
{
    public class MyHttpResponseMessage
    {
        public int msgType { get; set; }
        public int tranId { get; set; }
        public int? copyItemCode { get; set; }
        public int? PWindow { get; set; }
        public string msg { get; set; }
        public string msgError { get; set; }
        public string voucherNo { get; set; }
        public string stackTrace { get; set; }
        public string? KotPrinter { get; set; }
        public string? StickerPrinter { get; set; }
        public string? SlipHtml { get; set; }
        public string? SlipFilePath { get; set; }
        public string? BarFilePath { get; set; }
        public object data { get; set; }
        public SaveData SaveData { get; set; }
        public object Users { get; set; }
        public object viewModel { get; set; }
        public List<User> userlist { get; set; }
        public List<Menu> Menu{ get; set; }
        public object data2 { get; set; }
        public object data3 { get; set; }
        public bool isSmsExist { get; set; }
        public bool? requireChangePassword { get; set; }
        public int activityLogId { get; set; }
        public dynamic dataTable { get; set; }
    }

    public class SaveData
    {
        public decimal? code { get; set; }
        public string voucherNo { get; set; }
        public string srbInvoiceDate { get; set; }
        public string srbNoucherNo { get; set; }
        public string dt_codes { get; set; }
    }

    public class MyHttpResponseMessageHr
    {
        public int msgType { get; set; }
        public int tranId { get; set; }
        public string msg { get; set; }
        public string voucherNo { get; set; }
        public string stackTrace { get; set; }
        public string? SlipHtml { get; set; }
        public object data { get; set; }
        public object Users { get; set; }
        public object viewModel { get; set; }
        public object data2 { get; set; }
        public bool isSmsExist { get; set; }
        public bool? requireChangePassword { get; set; }
        public int activityLogId { get; set; }
        public dynamic dataTable { get; set; }
    }
}
