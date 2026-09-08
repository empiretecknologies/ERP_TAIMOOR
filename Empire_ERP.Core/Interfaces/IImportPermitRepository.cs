using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IImportPermitRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetImportPermitByCode(int code, Common common);
        MyHttpResponseMessage GetImportPermitDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomImportPermit modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteImportPermitDetailByCode(int code, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetail(Common common);
    }
}