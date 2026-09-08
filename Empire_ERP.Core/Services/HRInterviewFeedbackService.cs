using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System.Data;

namespace Empire_ERP.Core.Services
{
    public class HRInterviewFeedbackService : IHRInterviewFeedbackService
    {

        public IHRInterviewFeedbackRepository _HRInterviewFeedbackRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public HRInterviewFeedbackService(IHRInterviewFeedbackRepository chartOfAccountRepository, IMenuService menuService, ICompanyService companyService)
        {
            _HRInterviewFeedbackRepository = chartOfAccountRepository;
            _menuService = menuService;
            _companyService = companyService;
        }

        public MyHttpResponseMessage GetHRJobPosts(Common common)
        {
            return _HRInterviewFeedbackRepository.GetHRJobPosts(common);
        }

        public MyHttpResponseMessage GetAccountsForTreeView(Common common)
        {
            return _HRInterviewFeedbackRepository.GetAccountsForTreeView(common);
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _HRInterviewFeedbackRepository.QuickSearch(common);
        }

        //public MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null)
        //{
        //    return _HRInterviewFeedbackRepository.QuickSearch(common, skip, take, filter, group);
        //}

        public MyHttpResponseMessage Save(HRInterviewFeedback model, Common common)
        {
            return _HRInterviewFeedbackRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _HRInterviewFeedbackRepository.GenerateNextId(common);
        }

        public string GenerateGrCode(string ParentId, Common common)
        {
            return _HRInterviewFeedbackRepository.GenerateGrCode(ParentId, common);
        }

        public string GenerateCardNo(Common common)
        {
            return _HRInterviewFeedbackRepository.GenerateCardNo(common);
        }

        public MyHttpResponseMessage GetHRJobPostById(int id, Common common)
        {
            return _HRInterviewFeedbackRepository.GetHRJobPostById(id, common);
        }

        public MyHttpResponseMessage GetEmpMails(Common common)
        {
            return _HRInterviewFeedbackRepository.GetEmpMails(common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _HRInterviewFeedbackRepository.Delete(id, common);
        }

        public MyHttpResponseMessage GetDataForReport(HRCandidateReport modelRecord, DataTable dataTable, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            CustomMenuDetail menuDetail = new CustomMenuDetail();
            try
            {
                var menuResponse = _menuService.GetMenuDetails(common.MenuID);
                if (menuResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }
                var menuData = (List<CustomMenuDetail>)menuResponse.data;
                if (menuData.Count > 0)
                {
                    //menuDetail = menuData.Where(m => m.MD_ID == modelRecord.MD_ID).FirstOrDefault();
                    if (menuDetail?.MD_ID <= 0)
                    {
                        response.msgType = 2;
                        return response;
                    }
                }
                else
                {
                    response.msgType = 2;
                    return response;
                }

                var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
                if (currentCompanyResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }

                var currentCompany = (Company)currentCompanyResponse.data;

                return _HRInterviewFeedbackRepository.GetDataForReport(modelRecord, dataTable, menuDetail, currentCompany, common);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msgType = 2;
                response.msg = _catchMessage;
            }
            return response;
        }
    }
}
