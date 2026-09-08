using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System.Linq;

namespace Empire_ERP.Core.Services
{
    public class PartyReportService : IPartyReportService
    {
        public IPartyReportRepository _partyReportRepository { get; set; }
        public PartyReportService(IPartyReportRepository partyReportRepository)
        {
            _partyReportRepository = partyReportRepository;
        }

        public MyHttpResponseMessage GetReportTypes(Common common)
        {
            return _partyReportRepository.GetReportTypes(common.MenuID, common.RoleType, common.RoleID);
        }

        public async Task<MyHttpResponseMessage> GetReportData(PartyReport report, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var validReportIds = new HashSet<int?> { 10, 11, 38, 39, 51, 52, 58, 70, 97, 98, 99 , 102, 136};

                if (validReportIds.Contains(report.ReportID))
                {
                    response = await _partyReportRepository.GetReportData(report, common);
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