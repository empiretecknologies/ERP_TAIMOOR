using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class PartyOpeningService : IPartyOpeningService
	{
        public IPartyOpeningRepository _partyOpeningRepository { get; set; }
        public PartyOpeningService(IPartyOpeningRepository partyOpeningRepository)
        {
			_partyOpeningRepository = partyOpeningRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _partyOpeningRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetPartyOpeningDetails(int code, Common common)
        {
            return _partyOpeningRepository.GetPartyOpeningDetails(code, common);
        }

        public MyHttpResponseMessage GetPartyOpeningDetailByCode(int code, Common common)
		{
			return _partyOpeningRepository.GetPartyOpeningDetailByCode(code, common);
		}

		public MyHttpResponseMessage Save(PartyOpening partyOpening, Common common)
        {
            return _partyOpeningRepository.Save(partyOpening, common);
        }

        public MyHttpResponseMessage GetPartyOpeningByCode(int code, Common common)
        {
            return _partyOpeningRepository.GetPartyOpeningByCode(code, common);
		}

		public MyHttpResponseMessage Delete(int code, Common common)
		{
			return _partyOpeningRepository.Delete(code, common);
		}
	}
}