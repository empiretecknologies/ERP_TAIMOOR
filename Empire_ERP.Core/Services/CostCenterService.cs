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
    public class CostCenterService : ICostCenterService
    {
        public ICostCenterRepository _costCenterRepository { get; set; }
        public CostCenterService(ICostCenterRepository costCenterRepository)
        {
            _costCenterRepository = costCenterRepository;
        }

        public MyHttpResponseMessage QuickSearch(CostCenter model)
        {
            return _costCenterRepository.QuickSearch(model);
        }

        public MyHttpResponseMessage Save(CostCenter model, Common common)
        {
            return _costCenterRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _costCenterRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetCostCenterByID(int id, Common common)
        {
            return _costCenterRepository.GetCostCenterByID(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _costCenterRepository.Delete(id, common);
        }
    }
}