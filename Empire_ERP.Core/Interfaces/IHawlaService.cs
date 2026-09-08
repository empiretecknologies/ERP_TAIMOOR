using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IHawlaService
    {
		MyHttpResponseMessage GetHawlas(int Branch, Common common);
        MyHttpResponseMessage Save(Hawla modelRecord, Common common);
        MyHttpResponseMessage GetDataForReport(int amount, int branch, DataTable details, Common common);
        MyHttpResponseMessage GetTJVRecord(int code, Common common);
    }
}