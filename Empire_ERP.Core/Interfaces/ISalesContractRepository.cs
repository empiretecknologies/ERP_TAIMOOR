using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface ISalesContractRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetSalesContractByCode(int code, Common common);
        MyHttpResponseMessage GetSalesContractDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomSalesContract modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteSalesContractDetailByCode(int code, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetail(Common common);
    }
}