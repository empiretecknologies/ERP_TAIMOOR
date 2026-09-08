using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class ChartOfAccountsService : IChartOfAccountService
    {

        public IChartOfAccountRepository _chartOfAccountRepository { get; set; }

        public IMenuService _menuService { get; set; }
        public ChartOfAccountsService(IChartOfAccountRepository chartOfAccountRepository, IMenuService menuService)
        {
            _chartOfAccountRepository = chartOfAccountRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage GetChartOfAccounts(Common common)
        {
            return _chartOfAccountRepository.GetChartOfAccounts(common);
        }

        public MyHttpResponseMessage GetAccountsForTreeView(Common common)
        {
            return _chartOfAccountRepository.GetAccountsForTreeView(common);
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _chartOfAccountRepository.QuickSearch(common);
        }

        //public MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null)
        //{
        //    return _chartOfAccountRepository.QuickSearch(common, skip, take, filter, group);
        //}

        public MyHttpResponseMessage Save(ChartOfAccount model, Common common)
        {
            return _chartOfAccountRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _chartOfAccountRepository.GenerateNextId(common);
        }

        public string GenerateGrCode(string ParentId, Common common)
        {
            return _chartOfAccountRepository.GenerateGrCode(ParentId, common);
        }

        public MyHttpResponseMessage GetChartOfAccountById(int id, Common common)
        {
            return _chartOfAccountRepository.GetChartOfAccountById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, string startDate, string endDate, Common common)
        {
            return _chartOfAccountRepository.Delete(id, startDate, endDate, common);
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
                        response = _chartOfAccountRepository.CopyRecord(record, common, menu);
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
    }
}
