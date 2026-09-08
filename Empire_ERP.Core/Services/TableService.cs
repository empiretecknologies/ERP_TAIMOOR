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
    public class TableService : ITableService
    {
        public ITableRepository _TableRepository { get; set; }
        public TableService(ITableRepository TableRepository)
        {
            _TableRepository = TableRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _TableRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(Table model, Common common)
        {
            return _TableRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _TableRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetTableById(int id, Common common)
        {
            return _TableRepository.GetTableById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _TableRepository.Delete(id, common);
        }
    }
}