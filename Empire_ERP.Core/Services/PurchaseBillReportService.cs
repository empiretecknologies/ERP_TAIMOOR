using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class PurchaseBillReportService : IPurchaseBillReportService
    {
        public IPurchaseBillReportRepository _partyReportRepository { get; set; }
        public PurchaseBillReportService(IPurchaseBillReportRepository partyReportRepository)
        {
            _partyReportRepository = partyReportRepository;
        }

        public MyHttpResponseMessage GetReportTypes(Common common)
        {
            return _partyReportRepository.GetReportTypes(common.MenuID, common.RoleType, common.RoleID);
        }

        public MyHttpResponseMessage GetReportData(PurchaseBillReport report, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                if (report.ReportID == 16 || report.ReportID == 104 || report.ReportID == 105)
                {
                    response = _partyReportRepository.GetReportData(report, common);
                }
                else
                {
                    response.msgType = 2;
                    response.msg = "This report is not available yet but this will be available soon.";
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }
    }
}