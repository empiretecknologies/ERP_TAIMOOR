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
    public class EmpLeavesService : IEmpLeavesService
    {
        public IEmpLeavesRepository _EmpLeavesRepository { get; set; }
        public EmpLeavesService(IEmpLeavesRepository EmpLeavesRepository)
        {
            _EmpLeavesRepository = EmpLeavesRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common , string TableName)
        {
            return _EmpLeavesRepository.QuickSearch(common , TableName);
        }

        public MyHttpResponseMessage Save(EmpLeaves model, Common common)
        {
            return _EmpLeavesRepository.Save(model, common);
        }

        //public string GenerateNextId(Common common , string TableName)
        //{
        //    return _EmpLeavesRepository.GenerateNextId(common , TableName);
        //}

        public MyHttpResponseMessage GetEmpLeavesRecord(int id , Common common)
        {
            return _EmpLeavesRepository.GetEmpLeavesRecord(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _EmpLeavesRepository.Delete(id, common);
        }
    }
}