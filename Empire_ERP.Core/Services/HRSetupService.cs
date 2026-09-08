using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class HRSetupService : IHRSetupService
    {
        public IHRSetupRepository _HRSetupRepository { get; set; }
        public HRSetupService(IHRSetupRepository HRSetupRepository)
        {
            _HRSetupRepository = HRSetupRepository;
        }

        public MyHttpResponseMessage Save(HRSetup HRSetup, Common common)
        {
            return _HRSetupRepository.Save(HRSetup, common);
        }

        public MyHttpResponseMessage Delete(int HRSetupCode, int actCode, Common common)
        {
            return _HRSetupRepository.Delete(HRSetupCode, actCode, common);
        }

        public MyHttpResponseMessage QuickSearchHRSetup(Common common)
        {
            return _HRSetupRepository.QuickSearchHRSetup(common);
        }
        
        public MyHttpResponseMessage GetHolidayRecords(Common common)
        {
            return _HRSetupRepository.GetHolidayRecords(common);
        }

        public MyHttpResponseMessage GetHRSetupByHRSetupCode(int HRSetupCode, Common common)
        {
            return _HRSetupRepository.GetHRSetupByHRSetupCode(HRSetupCode, common);
        }

        public MyHttpResponseMessage GetHRSetupByHRSetupCode(int HRSetupCode)
        {
            return _HRSetupRepository.GetHRSetupByHRSetupCode(HRSetupCode);
        }
    }
}