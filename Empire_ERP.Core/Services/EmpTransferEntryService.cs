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
    public class EmpTransferEntryService : IEmpTransferEntryService
    {
        public IEmpTransferEntryRepository _EmpTransferEntryRepository { get; set; }
        public EmpTransferEntryService(IEmpTransferEntryRepository EmpTransferEntryRepository)
        {
            _EmpTransferEntryRepository = EmpTransferEntryRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common , string TableName)
        {
            return _EmpTransferEntryRepository.QuickSearch(common , TableName);
        }

        public MyHttpResponseMessage Save(EmpTransferEntry model, Common common)
        {
            return _EmpTransferEntryRepository.Save(model, common);
        }

        //public string GenerateNextId(Common common , string TableName)
        //{
        //    return _EmpTransferEntryRepository.GenerateNextId(common , TableName);
        //}

        public MyHttpResponseMessage GetEmpTransferEntryRecord(int id , Common common)
        {
            return _EmpTransferEntryRepository.GetEmpTransferEntryRecord(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _EmpTransferEntryRepository.Delete(id, common);
        }
    }
}