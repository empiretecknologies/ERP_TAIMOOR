using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class ImportPermitService : IImportPermitService
    {
        public IImportPermitRepository _importPermitRepository { get; set; }
        public ImportPermitService(IImportPermitRepository materialRequisitionRepository)
        {
            _importPermitRepository = materialRequisitionRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _importPermitRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetImportPermitByCode(int code, Common common)
        {
            return _importPermitRepository.GetImportPermitByCode(code, common);
        }

        public MyHttpResponseMessage GetImportPermitDetailByCode(int code, Common common)
        {
            return _importPermitRepository.GetImportPermitDetailByCode(code, common);
        }

        public MyHttpResponseMessage Save(CustomImportPermit modelRecord, Common common)
        {
            return _importPermitRepository.Save(modelRecord, common);
        }

		public MyHttpResponseMessage Delete(int code, Common common)
		{
			return _importPermitRepository.Delete(code, common);
		}

        public MyHttpResponseMessage DeleteImportPermitDetailByCode(int code, Common common)
        {
            return _importPermitRepository.DeleteImportPermitDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingDetail(Common common)
        {
            return _importPermitRepository.GetSodaBookFeedingDetail(common);
        }
    }
}