using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IAccountOpeningRepository
	{
        MyHttpResponseMessage GetAccountOpenings(Common common);
        MyHttpResponseMessage Save(List<AccountOpening> modelRecord, Common common);
    }
}