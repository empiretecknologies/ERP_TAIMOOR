using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class SPartyReportService : ISPartyReportService
    {
        public ISPartyReportRepository _partyReportRepository { get; set; }
        public SPartyReportService(ISPartyReportRepository partyReportRepository)
        {
            _partyReportRepository = partyReportRepository;
        }

        public MyHttpResponseMessage GetReportTypes(Common common)
        {
            return _partyReportRepository.GetReportTypes(common.MenuID, common.RoleType, common.RoleID);
        }
        public MyHttpResponseMessage UpdateSodeBookFeedingReport(SodePartyReport modelrecord , Common common)
        {
            return _partyReportRepository.UpdateSodeBookFeedingReport(modelrecord , common);
        }

        public MyHttpResponseMessage GetReportData(SPartyReport report, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var validReportIds = new HashSet<int?> { 13, 14, 15, 17, 40, 50, 68, 69, 72, 77, 78, 79, 80, 81, 82, 83, 84, 123, 124 };

                if (validReportIds.Contains(report.ReportID))
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