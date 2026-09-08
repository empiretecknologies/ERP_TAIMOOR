using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Empire_ERP.Core.Services
{
    public class InvoiceReportsService : IInvoiceReportsService
    {
        public IInvoiceReportsRepository _InvoiceReportsRepository { get; set; }
        public InvoiceReportsService(IInvoiceReportsRepository InvoiceReportsRepository)
        {
            _InvoiceReportsRepository = InvoiceReportsRepository;
        }

        public MyHttpResponseMessage GetReportTypes(Common common)
        {
            return _InvoiceReportsRepository.GetReportTypes(common.MenuID, common.RoleType, common.RoleID);
        }

        public MyHttpResponseMessage GetReportData(InvoiceReports report, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                if ((report.ReportID >= 18 && report.ReportID <= 36) || (report.ReportID >= 41 && report.ReportID <= 44) || (report.ReportID >= 45 && report.ReportID <= 67) || (report.ReportID >= 74 && report.ReportID <= 76) || (report.ReportID >= 87 && report.ReportID <= 90) || report.ReportID == 94 || report.ReportID == 95 || report.ReportID == 117 || report.ReportID == 118 || report.ReportID == 132 || report.ReportID == 133 || report.ReportID == 134 || report.ReportID == 135 || report.ReportID == 137 || (report.ReportID == 100) || (report.ReportID >= 125 && report.ReportID <= 130) || report.ReportID == 138 || report.ReportID == 140 || report.ReportID == 141 || report.ReportID == 91 || report.ReportID == 144)
                {
                    response = _InvoiceReportsRepository.GetReportData(report, common);
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