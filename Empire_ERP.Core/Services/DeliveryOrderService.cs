using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System.Data;

namespace Empire_ERP.Core.Services
{
    public class DeliveryOrderService : IDeliveryOrderService
    {
        public IDeliveryOrderRepository _deliveryOrderRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public DeliveryOrderService(IDeliveryOrderRepository deliveryOrderRepository, IMenuService menuService, ICompanyService companyService)
        {
            _deliveryOrderRepository = deliveryOrderRepository;
            _menuService = menuService;
            _companyService = companyService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _deliveryOrderRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetDeliveryOrderByCode(int code, Common common)
        {
            return _deliveryOrderRepository.GetDeliveryOrderByCode(code, common);
        }

        public MyHttpResponseMessage GetDeliveryOrderDetailByCode(int code, Common common)
        {
            return _deliveryOrderRepository.GetDeliveryOrderDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetPickDataByParty(int partyCode, int actCode, Common common)
        {
            return _deliveryOrderRepository.GetPickDataByParty(partyCode, actCode, common);
        }

        public MyHttpResponseMessage GetBarcodeList()
        {
            return _deliveryOrderRepository.GetBarcodeList();
        }

        public MyHttpResponseMessage GetDeliveryOrderPickDetailByCode(int code, Common common)
        {
            return _deliveryOrderRepository.GetDeliveryOrderPickDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetDeliveryOrderDetailByItem(int code, int qty, Common common)
        {
            return _deliveryOrderRepository.GetDeliveryOrderDetailByItem(code, qty, common);
        }

        public MyHttpResponseMessage Save(CustomDeliveryOrder modelRecord, Common common)
        {
            return _deliveryOrderRepository.Save(modelRecord, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _deliveryOrderRepository.Delete(code, common);
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
                        response = _deliveryOrderRepository.CopyRecord(record, common, menu);
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

        public MyHttpResponseMessage DeleteDeliveryOrderDetailByCode(int code, Common common)
        {
            return _deliveryOrderRepository.DeleteDeliveryOrderDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetDataForReport(DeliveryOrderRDLCReport modelRecord, DataTable dataTable, Common common)
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


                return _deliveryOrderRepository.GetDataForReport(modelRecord, dataTable, menuDetail, common);
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