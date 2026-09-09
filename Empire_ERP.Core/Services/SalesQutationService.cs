using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class SalesQutationService : ISalesQutationService
    {
        public ISalesQutationRepository _salesQutationRepository { get; set; }
        public SalesQutationService(ISalesQutationRepository salesQutationRepository)
        {
            _salesQutationRepository = salesQutationRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _salesQutationRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetSalesQutationByCode(int code, Common common)
        {
            return _salesQutationRepository.GetSalesQutationByCode(code, common);
        }

        public MyHttpResponseMessage GetSalesQutationDetailByCode(int code, Common common)
        {
            return _salesQutationRepository.GetSalesQutationDetailByCode(code, common);
        }

        public MyHttpResponseMessage Save(CustomSalesQutation modelRecord, Common common)
        {
            return _salesQutationRepository.Save(modelRecord, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _salesQutationRepository.Delete(code, common);
        }

        public MyHttpResponseMessage DeleteSalesQutationDetailByCode(int code, Common common)
        {
            return _salesQutationRepository.DeleteSalesQutationDetailByCode(code, common);
        }
    }
}
