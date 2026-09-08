using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface ISodaBookFeedingService
    {
		MyHttpResponseMessage QuickSearch(Common common);
		//MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage Save(CustomSodaBookFeeding model, Common common);
        MyHttpResponseMessage GetSodaBookFeedingByCode(int code, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteSodaBookFeedingDetailByCode(int code, Common common);
    }
}