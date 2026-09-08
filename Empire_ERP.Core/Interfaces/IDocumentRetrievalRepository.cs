using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IDocumentRetrievalRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetDocumentRetrievalByCode(int code, Common common);
        MyHttpResponseMessage GetDocumentRetrievalDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomDocumentRetrieval modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteDocumentRetrievalDetailByCode(int code, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetail(Common common);
    }
}