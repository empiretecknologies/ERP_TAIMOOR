using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IDeliveryFeedingService
    {
		MyHttpResponseMessage QuickSearch(Common common);
		//MyHttpResponseMessage QuickSearchLazyLoading(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage Save(CustomDeliveryFeeding model, Common common);
        MyHttpResponseMessage GetDeliveryFeedingByCode(int code, Common common);
        MyHttpResponseMessage GetDeliveryFeedingDefaultOperators(Common common);
        MyHttpResponseMessage GetDeliveryFeedingDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteDeliveryFeedingDetailByCode(int code, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, Common common);
        MyHttpResponseMessage GetDataForReport(DeliveryFeedingReport modelRecord, DataTable details, Common common);
    }
}