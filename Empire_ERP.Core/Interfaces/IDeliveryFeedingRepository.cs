using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IDeliveryFeedingRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        //MyHttpResponseMessage QuickSearchLazyLoad(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage Save(CustomDeliveryFeeding model, Common common);
        MyHttpResponseMessage GetDeliveryFeedingByCode(int code, Common common);
        MyHttpResponseMessage GetDeliveryFeedingDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDeliveryFeedingDefaultOperators(Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteDeliveryFeedingDetailByCode(int code, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, Common common);
        MyHttpResponseMessage GetDataForReport(DeliveryFeedingReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}