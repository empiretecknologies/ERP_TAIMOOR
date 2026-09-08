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
    public class EmpPenaltyService : IEmpPenaltyService
    {
        public IEmpPenaltyRepository _EmpPenaltyRepository { get; set; }
        public EmpPenaltyService(IEmpPenaltyRepository EmpPenaltyRepository)
        {
            _EmpPenaltyRepository = EmpPenaltyRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common , string TableName)
        {
            return _EmpPenaltyRepository.QuickSearch(common , TableName);
        }

        public MyHttpResponseMessage Save(EmpPenalty model, Common common)
        {
            return _EmpPenaltyRepository.Save(model, common);
        }

        //public string GenerateNextId(Common common , string TableName)
        //{
        //    return _EmpPenaltyRepository.GenerateNextId(common , TableName);
        //}

        public MyHttpResponseMessage GetEmpPenaltyRecord(int id , Common common)
        {
            return _EmpPenaltyRepository.GetEmpPenaltyRecord(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _EmpPenaltyRepository.Delete(id, common);
        }
    }
}