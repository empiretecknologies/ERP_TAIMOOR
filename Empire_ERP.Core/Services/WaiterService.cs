using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class WaiterService : IWaiterService
    {
        public IWaiterRepository _WaiterRepository { get; set; }
        public WaiterService(IWaiterRepository WaiterRepository)
        {
            _WaiterRepository = WaiterRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _WaiterRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(Waiter model, Common common)
        {
            return _WaiterRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _WaiterRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetWaiterById(int id, Common common)
        {
            return _WaiterRepository.GetWaiterById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _WaiterRepository.Delete(id, common);
        }
    }
}