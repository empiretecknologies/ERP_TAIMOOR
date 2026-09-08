using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class DocumentRetrievalService : IDocumentRetrievalService
    {
        public IDocumentRetrievalRepository _DocumentRetrievalRepository { get; set; }
        public DocumentRetrievalService(IDocumentRetrievalRepository materialRequisitionRepository)
        {
            _DocumentRetrievalRepository = materialRequisitionRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _DocumentRetrievalRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetDocumentRetrievalByCode(int code, Common common)
        {
            return _DocumentRetrievalRepository.GetDocumentRetrievalByCode(code, common);
        }

        public MyHttpResponseMessage GetDocumentRetrievalDetailByCode(int code, Common common)
        {
            return _DocumentRetrievalRepository.GetDocumentRetrievalDetailByCode(code, common);
        }

        public MyHttpResponseMessage Save(CustomDocumentRetrieval modelRecord, Common common)
        {
            return _DocumentRetrievalRepository.Save(modelRecord, common);
        }

		public MyHttpResponseMessage Delete(int code, Common common)
		{
			return _DocumentRetrievalRepository.Delete(code, common);
		}

        public MyHttpResponseMessage DeleteDocumentRetrievalDetailByCode(int code, Common common)
        {
            return _DocumentRetrievalRepository.DeleteDocumentRetrievalDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingDetail(Common common)
        {
            return _DocumentRetrievalRepository.GetSodaBookFeedingDetail(common);
        }
    }
}