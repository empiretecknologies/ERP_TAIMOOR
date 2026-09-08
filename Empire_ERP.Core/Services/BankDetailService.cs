using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class BankDetailService : IBankDetailService
    {
        public IBankDetailRepository _BankDetailRepository { get; set; }
        public IMenuService _menuService { get; set; }

        public BankDetailService(IBankDetailRepository materialRequisitionRepository, IMenuService menuService)
        {
            _BankDetailRepository = materialRequisitionRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _BankDetailRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetChartOfAccounts(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuService.GetMenu(common.MenuID);
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                }
                return _BankDetailRepository.GetChartOfAccounts(common);
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

        public MyHttpResponseMessage GetBankDetailByCode(int code, Common common)
        {
            return _BankDetailRepository.GetBankDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetBankDetailDetailByCode(int code, Common common)
        {
            return _BankDetailRepository.GetBankDetailDetailByCode(code, common);
        }

        public MyHttpResponseMessage Save(CustomBankDetail modelRecord, Common common)
        {
            return _BankDetailRepository.Save(modelRecord, common);
        }

		public MyHttpResponseMessage Delete(int code, Common common)
		{
			return _BankDetailRepository.Delete(code, common);
		}

        public MyHttpResponseMessage DeleteBankDetailDetailByCode(int code, Common common)
        {
            return _BankDetailRepository.DeleteBankDetailDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingDetail(Common common)
        {
            return _BankDetailRepository.GetSodaBookFeedingDetail(common);
        }
    }
}