using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class MPORegistrationService : IMPORegistrationService
    {
        public IMPORegistrationRepository _MPORegistrationRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public MPORegistrationService(IMPORegistrationRepository setupSubTypeRepository, IMenuService menuService)
        {
            _MPORegistrationRepository = setupSubTypeRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _MPORegistrationRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(MPORegistration model, Common common)
        {
            return _MPORegistrationRepository.Save(model, common);
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
                        response = _MPORegistrationRepository.CopyRecord(record, common, menu);
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

        public string GenerateNextId(Common common)
        {
            return _MPORegistrationRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetMPORegistrationById(int id, Common common)
        {
            return _MPORegistrationRepository.GetMPORegistrationById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _MPORegistrationRepository.Delete(id, common);
        }
    }
}