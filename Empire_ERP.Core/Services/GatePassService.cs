using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System.Data;

namespace Empire_ERP.Core.Services
{
    public class GatePassService : IGatePassService
    {

        public IGatePassRepository _gatePassRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public GatePassService(IGatePassRepository chartOfAccountRepository, IMenuService menuService, ICompanyService companyService)
        {
            _gatePassRepository = chartOfAccountRepository;
            _menuService = menuService;
            _companyService = companyService;
        }

        public MyHttpResponseMessage GetGatePasss(Common common)
        {
            return _gatePassRepository.GetGatePasss(common);
        }

        public MyHttpResponseMessage GetAccountsForTreeView(Common common)
        {
            return _gatePassRepository.GetAccountsForTreeView(common);
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _gatePassRepository.QuickSearch(common);
        }

        //public MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null)
        //{
        //    return _gatePassRepository.QuickSearch(common, skip, take, filter, group);
        //}

        public MyHttpResponseMessage Save(GatePass model, Common common)
        {
            return _gatePassRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _gatePassRepository.GenerateNextId(common);
        }

        public string GenerateGrCode(string ParentId, Common common)
        {
            return _gatePassRepository.GenerateGrCode(ParentId, common);
        }

        public MyHttpResponseMessage GetGatePassById(int id, Common common)
        {
            return _gatePassRepository.GetGatePassById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _gatePassRepository.Delete(id, common);
        }

        public MyHttpResponseMessage CopyRecord(CopyRecord record, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                var Menu = _menuService.GetMenu(common.MenuID);
                string? table = string.Empty;
                Menu menu = new Menu();
                if (Menu.data != null)
                {
                    menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    if (record.TRAN_ID == 0)
                    {
                        response.msg = "ID is not in numeric format";
                        response.msgType = 2;
                    }
                    else
                    {
                        response = _gatePassRepository.CopyRecord(record, common, menu);
                    }
                }
                else
                {
                    response.data = "";
                    response.msg = "Something went wrong! please try again later.";
                    response.msgType = 2;
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

        public MyHttpResponseMessage GetDataForReport(GatePassReport modelRecord, DataTable dataTable, Common common)
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

                return _gatePassRepository.GetDataForReport(modelRecord, dataTable, menuDetail, currentCompany, common);
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
