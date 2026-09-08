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
    public class EmpMasterInfoService : IEmpMasterInfoService
    {
        public IEmpMasterInfoRepository _EmpMasterInfoRepository { get; set; }
        public EmpMasterInfoService(IEmpMasterInfoRepository EmpMasterInfoRepository)
        {
            _EmpMasterInfoRepository = EmpMasterInfoRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common , string TableName)
        {
            return _EmpMasterInfoRepository.QuickSearch(common , TableName);
        }

        public MyHttpResponseMessage Save(EmpMasterInfo model, Common common)
        {
            return _EmpMasterInfoRepository.Save(model, common);
        }

        public string GenerateNextId(Common common , string TableName)
        {
            return _EmpMasterInfoRepository.GenerateNextId(common , TableName);
        }

        public MyHttpResponseMessage GetEmpMasterInfoById(int id , string TableName, Common common)
        {
            return _EmpMasterInfoRepository.GetEmpMasterInfoById(id,TableName, common);
        }

        public MyHttpResponseMessage Delete(int id, string TableName, Common common)
        {
            return _EmpMasterInfoRepository.Delete(id,TableName, common);
        }
    }
}