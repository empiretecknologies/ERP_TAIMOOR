using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IHawlaRepository
	{
        MyHttpResponseMessage GetHawlas(int Branch, Common common);
        MyHttpResponseMessage Save(Hawla modelRecord, Common common);
        MyHttpResponseMessage GetDataForReport(int amount, int branch, CustomMenuDetail menuDetails, Common common);
        MyHttpResponseMessage GetTJVRecord(int code, Common common);

    }
}