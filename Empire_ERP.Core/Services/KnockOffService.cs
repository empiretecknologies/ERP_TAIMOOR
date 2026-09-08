using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Empire_ERP.Core.Entities.KnockOff;

namespace Empire_ERP.Core.Services
{
    public class KnockOffService : IKnockOffService
    {
        public IKnockOffRepository _knockOffRepository { get; set; }
        public KnockOffService(IKnockOffRepository knockOffRepository)
        {
            _knockOffRepository = knockOffRepository;
        }

        public MyHttpResponseMessage GetAllSaleInvoices(KnockOff model, Common common)
        {
            return _knockOffRepository.GetAllSaleInvoices(model, common);
        }

        public MyHttpResponseMessage GetAllKnockOff(KnockOff model, Common common)
        {
            return _knockOffRepository.GetAllKnockOff(model, common);
        }

        public MyHttpResponseMessage Save(CustomKnockOff model, Common common)
        {
            return _knockOffRepository.Save(model, common);
        }

        //public string GenerateNextId(Common common)
        //{
        //    return _knockOffRepository.GenerateNextId(common);
        //}

        public MyHttpResponseMessage GetCostCenterByID(int id, Common common)
        {
            return _knockOffRepository.GetCostCenterByID(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _knockOffRepository.Delete(id, common);
        }
    }
}