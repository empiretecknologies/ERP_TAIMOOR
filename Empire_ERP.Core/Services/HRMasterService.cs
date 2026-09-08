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
    public class HRMasterService : IHRMasterService
    {
        public IHRMasterRepository _HRMasterRepository { get; set; }
        public HRMasterService(IHRMasterRepository HRMasterRepository)
        {
            _HRMasterRepository = HRMasterRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common , string TableName)
        {
            return _HRMasterRepository.QuickSearch(common , TableName);
        }

        public MyHttpResponseMessage Save(HRMaster model, Common common)
        {
            return _HRMasterRepository.Save(model, common);
        }

        public string GenerateNextId(Common common , string TableName)
        {
            return _HRMasterRepository.GenerateNextId(common , TableName);
        }

        public MyHttpResponseMessage GetHRMasterById(int id , string TableName, Common common)
        {
            return _HRMasterRepository.GetHRMasterById(id,TableName, common);
        }

        public MyHttpResponseMessage Delete(int id, string TableName, Common common)
        {
            return _HRMasterRepository.Delete(id,TableName, common);
        }
    }
}