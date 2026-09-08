using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class CurrencyService : ICurrencyService
    {
        public ICurrencyRepository _currencyRepository { get; set; }
        public CurrencyService(ICurrencyRepository currencyRepository)
        {
            _currencyRepository = currencyRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _currencyRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(Currency model, Common common)
        {
            return _currencyRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _currencyRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetCurrencyById(int id, Common common)
        {
            return _currencyRepository.GetCurrencyById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _currencyRepository.Delete(id, common);
        }
    }
}