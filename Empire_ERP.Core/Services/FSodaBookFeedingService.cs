using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class FSodaBookFeedingService : IFSodaBookFeedingService
    {
        public IFSodaBookFeedingRepository _sodaBookFeedingRepository { get; set; }

        public IMenuService _menuService { get; set; }
        public FSodaBookFeedingService(IFSodaBookFeedingRepository sodaBookFeedingRepository, IMenuService menuService)
        {
            _sodaBookFeedingRepository = sodaBookFeedingRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _sodaBookFeedingRepository.QuickSearch(common);
        }

        //public MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null)
        //{
        //    return _sodaBookFeedingRepository.QuickSearch(common, skip, take, filter, group);
        //}

        public MyHttpResponseMessage Save(CustomFSodaBookFeeding model, Common common)
        {
            return _sodaBookFeedingRepository.Save(model, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingByCode(int code, Common common)
        {
            return _sodaBookFeedingRepository.GetSodaBookFeedingByCode(code, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingDetailByCode(int code, Common common)
        {
            return _sodaBookFeedingRepository.GetSodaBookFeedingDetailByCode(code, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _sodaBookFeedingRepository.Delete(code, common);
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
                        response = _sodaBookFeedingRepository.CopyRecord(record, common, menu);
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

        public MyHttpResponseMessage DeleteSodaBookFeedingDetailByCode(int code, Common common)
        {
            return _sodaBookFeedingRepository.DeleteSodaBookFeedingDetailByCode(code, common);
        }
    }
}