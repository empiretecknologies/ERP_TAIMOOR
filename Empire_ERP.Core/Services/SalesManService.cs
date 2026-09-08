using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class SalesManService : ISalesManService
    {
        
        public ISalesManRepository _ISalesManRepository { get; set; }
        public SalesManService(ISalesManRepository salesManRepository)
        {
            _ISalesManRepository = salesManRepository;
        }

        public MyHttpResponseMessage GetAllSalesMan(int menuid)
        {
            return _ISalesManRepository.GetAllSalesMan(menuid);
        }
    }
}
