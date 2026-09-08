using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IHRSetupService
    {
        MyHttpResponseMessage Delete(int code, int actCode, Common common);
        MyHttpResponseMessage Save(HRSetup HRSetup, Common common);
        MyHttpResponseMessage QuickSearchHRSetup(Common common);
        MyHttpResponseMessage GetHolidayRecords(Common common);
        MyHttpResponseMessage GetHRSetupByHRSetupCode(int code, Common common);
        MyHttpResponseMessage GetHRSetupByHRSetupCode(int code);
    }
}