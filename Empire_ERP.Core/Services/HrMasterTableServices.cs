using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class HrMasterTableServices : IHrMasterTableServices
    {
        private readonly IHrMasterTableRepository _hrMasterTableRepository;
        public HrMasterTableServices(IHrMasterTableRepository hrMasterTableRepository)
        {
            _hrMasterTableRepository = hrMasterTableRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _hrMasterTableRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(HrMasterTable model, Common common)
        {
            return _hrMasterTableRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _hrMasterTableRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetHrTableById(int id, Common common)
        {
            return _hrMasterTableRepository.GetHrTableById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _hrMasterTableRepository.Delete(id, common);
        }
    }
}
