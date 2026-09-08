using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System.Data;

namespace Empire_ERP.Core.Services
{
    public class POSMappingService : IPOSMappingService
    {

        public IPOSMappingRepository _POSMappingRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public POSMappingService(IPOSMappingRepository chartOfAccountRepository, IMenuService menuService, ICompanyService companyService)
        {
            _POSMappingRepository = chartOfAccountRepository;
            _menuService = menuService;
            _companyService = companyService;
        }

        public MyHttpResponseMessage GetPOSMappings(Common common)
        {
            return _POSMappingRepository.GetPOSMappings(common);
        }
        public MyHttpResponseMessage GetMapping(Common common)
        {
            return _POSMappingRepository.GetMapping(common);
        }

        public MyHttpResponseMessage GetAccountsForTreeView(Common common)
        {
            return _POSMappingRepository.GetAccountsForTreeView(common);
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _POSMappingRepository.QuickSearch(common);
        }

        //public MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null)
        //{
        //    return _POSMappingRepository.QuickSearch(common, skip, take, filter, group);
        //}

        public MyHttpResponseMessage Save(POSMapping model, Common common)
        {
            return _POSMappingRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _POSMappingRepository.GenerateNextId(common);
        }

        public string GenerateGrCode(string ParentId, Common common)
        {
            return _POSMappingRepository.GenerateGrCode(ParentId, common);
        }

        public MyHttpResponseMessage GetPOSMappingById(int id, Common common)
        {
            return _POSMappingRepository.GetPOSMappingById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _POSMappingRepository.Delete(id, common);
        }

        public MyHttpResponseMessage GetDataForReport(POSMappingReport modelRecord, DataTable dataTable, Common common)
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
                    menuDetail = menuData.Where(m => m.MD_ID == modelRecord.MD_ID).FirstOrDefault();
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

                return _POSMappingRepository.GetDataForReport(modelRecord, dataTable, menuDetail, currentCompany, common);
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
