using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class SalesContractService : ISalesContractService
    {
        public ISalesContractRepository _salesContractRepository { get; set; }
        public SalesContractService(ISalesContractRepository materialRequisitionRepository)
        {
            _salesContractRepository = materialRequisitionRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _salesContractRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetSalesContractByCode(int code, Common common)
        {
            return _salesContractRepository.GetSalesContractByCode(code, common);
        }

        public MyHttpResponseMessage GetSalesContractDetailByCode(int code, Common common)
        {
            return _salesContractRepository.GetSalesContractDetailByCode(code, common);
        }

        public MyHttpResponseMessage Save(CustomSalesContract modelRecord, Common common)
        {
            return _salesContractRepository.Save(modelRecord, common);
        }

		public MyHttpResponseMessage Delete(int code, Common common)
		{
			return _salesContractRepository.Delete(code, common);
		}

        public MyHttpResponseMessage DeleteSalesContractDetailByCode(int code, Common common)
        {
            return _salesContractRepository.DeleteSalesContractDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingDetail(Common common)
        {
            return _salesContractRepository.GetSodaBookFeedingDetail(common);
        }
    }
}