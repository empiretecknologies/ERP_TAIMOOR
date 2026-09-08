using Empire_ERP.Core.Entities;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Empire_ERP.Core.Entities.KnockOff;

namespace Empire_ERP.Core.Interfaces
{
    public interface IKnockOffRepository
    {
        MyHttpResponseMessage GetAllSaleInvoices(KnockOff model, Common common);
        MyHttpResponseMessage GetAllKnockOff(KnockOff model, Common common);
        MyHttpResponseMessage GetCostCenterByID(int id, Common common);
        MyHttpResponseMessage Save(CustomKnockOff model, Common common);
        int GenerateNextId(Common common, SqlCommand command);
        MyHttpResponseMessage Delete(int id, Common common);
    }
}
