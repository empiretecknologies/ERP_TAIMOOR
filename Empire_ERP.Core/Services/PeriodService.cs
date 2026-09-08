using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class PeriodService : IPeriodService
    {
        public IPeriodRepository _periodRepository { get; set; }
        public PeriodService(IPeriodRepository IPeriodRepository)
        {
            _periodRepository = IPeriodRepository;
        }
        public MyHttpResponseMessage GetPeriodsByBranch(int id)
        {
            return _periodRepository.GetPeriodsByBranch(id);    
        }
        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _periodRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(Period model, Common common)
        {
            return _periodRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _periodRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetPeriodById(int id, Common common)
        {
            return _periodRepository.GetPeriodById(id, common);
        }

        public MyHttpResponseMessage GetPeriodById(int periodID)
        {
            return _periodRepository.GetPeriodById(periodID);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _periodRepository.Delete(id, common);
        }
    }
}