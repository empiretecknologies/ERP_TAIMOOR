using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class DeliveryFeedingService : IDeliveryFeedingService
    {
        public IDeliveryFeedingRepository _deliverFeedingRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public DeliveryFeedingService(IDeliveryFeedingRepository deliverFeedingRepository, IMenuService menuService, ICompanyService companyService)
        {
            _deliverFeedingRepository = deliverFeedingRepository;
            _menuService = menuService;
            _companyService = companyService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _deliverFeedingRepository.QuickSearch(common);
        }

        //public MyHttpResponseMessage QuickSearchLazyLoading(Common common, int skip = 0, int take = 12, string filter = null, string group = null)
        //{
        //    return _deliverFeedingRepository.QuickSearchLazyLoad(common, skip, take, filter, group);
        //}
        
        public MyHttpResponseMessage Save(CustomDeliveryFeeding model, Common common)
        {
            return _deliverFeedingRepository.Save(model, common);
        }

        public MyHttpResponseMessage GetDeliveryFeedingByCode(int code, Common common)
        {
            return _deliverFeedingRepository.GetDeliveryFeedingByCode(code, common);
        }

        public MyHttpResponseMessage GetDeliveryFeedingDetailByCode(int code, Common common)
        {
            return _deliverFeedingRepository.GetDeliveryFeedingDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetDeliveryFeedingDefaultOperators(Common common)
        {
            return _deliverFeedingRepository.GetDeliveryFeedingDefaultOperators(common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _deliverFeedingRepository.Delete(code, common);
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
                        response = _deliverFeedingRepository.CopyRecord(record, common, menu);
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

        public MyHttpResponseMessage DeleteDeliveryFeedingDetailByCode(int code, Common common)
        {
            return _deliverFeedingRepository.DeleteDeliveryFeedingDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, Common common)
        {
            return _deliverFeedingRepository.GetSodaBookFeedingDetailBySodaDate(sodaDate, common);
        }

        public MyHttpResponseMessage GetDataForReport(DeliveryFeedingReport modelRecord, DataTable dataTable, Common common)
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
                
                return _deliverFeedingRepository.GetDataForReport(modelRecord, dataTable, menuDetail, currentCompany, common);
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