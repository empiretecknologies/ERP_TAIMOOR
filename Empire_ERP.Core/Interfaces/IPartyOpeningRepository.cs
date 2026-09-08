using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPartyOpeningRepository
	{
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetPartyOpeningDetails(int code, Common common);
        MyHttpResponseMessage GetPartyOpeningDetailByCode(int code, Common common);
		MyHttpResponseMessage Save(PartyOpening partyOpening, Common common);
		MyHttpResponseMessage GetPartyOpeningByCode(int code, Common common);
		MyHttpResponseMessage Delete(int code, Common common);
	}
}