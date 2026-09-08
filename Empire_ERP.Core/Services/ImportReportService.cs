using System.Data;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class ImportReportService : IImportReportService
    {
        public IImportReportRepository _importReportRepository { get; set; }
        public IMenuService _menuService;
        public ICompanyService _companyService { get; set; }
        public ImportReportService(IImportReportRepository importReportRepository, IMenuService menuService, ICompanyService companyService)
        {
            _importReportRepository = importReportRepository;
            _menuService = menuService;
            _companyService = companyService;
        }

        public MyHttpResponseMessage GetReportTypes(Common common)
        {
            return _importReportRepository.GetReportTypes(common.MenuID, common.RoleType, common.RoleID);
        }

        public MyHttpResponseMessage GetReportData(ImportReport report, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {

                if (report.ReportID == 131)
                {
                    response = _importReportRepository.GetReportData(report, common);
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

        public MyHttpResponseMessage GetDataForReport(ImportReportRDLCReport modelRecord, DataTable dataTable, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            CustomMenuDetail menuDetail = new CustomMenuDetail();
            try
            {
                //var menuResponse = _menuService.GetMenuDetails(common.MenuID);
                //if (menuResponse.msgType != 1)
                //{
                //    response.msgType = 2;
                //    return response;
                //}
                //var menuData = (List<CustomMenuDetail>)menuResponse.data;
                //if (menuData.Count > 0)
                //{
                //    //menuDetail = menuData.Where(m => m.MD_ID == modelRecord.MD_ID).FirstOrDefault();
                //    menuDetail = menuData[0];
                //    if (menuDetail?.MD_ID <= 0)
                //    {
                //        response.msgType = 2;
                //        return response;
                //    }
                //}
                //else
                //{
                //    response.msgType = 2;
                //    return response;
                //}

                var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
                if (currentCompanyResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }

                var currentCompany = (Company)currentCompanyResponse.data;

                return _importReportRepository.GetDataForReport(modelRecord, dataTable, menuDetail, currentCompany, common);
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