using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Empire_ERP.Core.Entities.KnockOff;

namespace Empire_ERP.Core.Interfaces
{
    public interface IKnockOffService
    {
        MyHttpResponseMessage GetAllSaleInvoices(KnockOff model, Common common);
        MyHttpResponseMessage GetAllKnockOff(KnockOff model, Common common);
        MyHttpResponseMessage GetCostCenterByID(int id, Common common);
        MyHttpResponseMessage Save(CustomKnockOff model, Common common);
        //string GenerateNextId(Common common);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
