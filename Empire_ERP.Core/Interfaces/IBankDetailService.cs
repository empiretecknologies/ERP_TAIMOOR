using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IBankDetailService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetChartOfAccounts(Common common);
        MyHttpResponseMessage GetBankDetailByCode(int code, Common common);
        MyHttpResponseMessage GetBankDetailDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomBankDetail modelRecord, Common common);
		MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteBankDetailDetailByCode(int code, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetail(Common common);
    }
}