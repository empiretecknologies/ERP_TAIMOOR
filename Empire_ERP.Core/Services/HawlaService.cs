using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class HawlaService : IHawlaService
    {
        public IHawlaRepository _HawlaRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public HawlaService(IHawlaRepository HawlaRepository, IMenuService menuService)
        {
            _HawlaRepository = HawlaRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage GetHawlas(int Branch, Common common)
        {
            return _HawlaRepository.GetHawlas(Branch, common);
        }

        public MyHttpResponseMessage Save(Hawla modelRecord, Common common)
        {
            return _HawlaRepository.Save(modelRecord, common);
        }

        public MyHttpResponseMessage GetDataForReport(int amount, int branch, DataTable dataTable, Common common)
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
                    menuDetail = menuData.Where(m => m.MD_ID == 46).FirstOrDefault();


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

                return _HawlaRepository.GetDataForReport(amount, branch, menuDetail, common);
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

        public MyHttpResponseMessage GetTJVRecord(int code, Common common)
        {
            return _HawlaRepository.GetTJVRecord(code, common);
        }
    }
}