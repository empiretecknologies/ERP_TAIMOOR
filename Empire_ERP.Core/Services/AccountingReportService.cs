using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class AccountingReportService : IAccountingReportService
    {
        public IAccountingReportRepository _accountingReportRepository { get; set; }
        public AccountingReportService(IAccountingReportRepository accountingReportRepository)
        {
            _accountingReportRepository = accountingReportRepository;
        }

        public MyHttpResponseMessage GetReportTypes(Common common)
        {
            return _accountingReportRepository.GetReportTypes(common.MenuID, common.RoleType, common.RoleID);
        }

        public MyHttpResponseMessage GetReportData(AccountingReport report, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {

                if (report.ReportID == 3 || report.ReportID == 4 || report.ReportID == 6 || report.ReportID == 7 || report.ReportID == 37 || report.ReportID == 92 || report.ReportID == 101 || report.ReportID == 103 || (report.ReportID >= 106 && report.ReportID <= 116) || report.ReportID == 119 || report.ReportID == 121)
                {
                    response = _accountingReportRepository.GetReportData(report, common);
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