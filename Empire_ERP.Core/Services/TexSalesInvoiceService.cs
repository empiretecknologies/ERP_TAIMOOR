using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System.Data;

namespace Empire_ERP.Core.Services
{
    public class TexSalesInvoiceService : ITexSalesInvoiceService
    {
        public ITexSalesInvoiceRepository _texSalesInvoiceRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public TexSalesInvoiceService(ITexSalesInvoiceRepository texSalesInvoiceRepository, IMenuService menuService, ICompanyService companyService)
        {
            _texSalesInvoiceRepository = texSalesInvoiceRepository;
            _menuService = menuService;
            _companyService = companyService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _texSalesInvoiceRepository.QuickSearch(common);
        }

        //public MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null, string sort = null)
        //{
        //    return _texSalesInvoiceRepository.QuickSearch(common, skip, take, filter, group, sort);
        //}

        public MyHttpResponseMessage GetPurchaseBillByCode(int code, Common common)
        {
            return _texSalesInvoiceRepository.GetPurchaseBillByCode(code, common);
        }

        public MyHttpResponseMessage GetPurchaseBillDetailByCode(int code, Common common)
        {
            return _texSalesInvoiceRepository.GetPurchaseBillDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetPickDataBySupplier(int pCode, int actCode, Common common)
        {
            return _texSalesInvoiceRepository.GetPickDataBySupplier(pCode, actCode, common);
        }

        public MyHttpResponseMessage GetBarcodeList()
        {
            return _texSalesInvoiceRepository.GetBarcodeList();
        }

        public MyHttpResponseMessage GetPurchaseBillPickDetailByCode(int code, Common common)
        {
            return _texSalesInvoiceRepository.GetPurchaseBillPickDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetPurchaseBillDetailByItem(int code, int qty, Common common)
        {
            return _texSalesInvoiceRepository.GetPurchaseBillDetailByItem(code, qty, common);
        }

        public MyHttpResponseMessage Save(CustomTexSalesInvoice modelRecord, Common common)
        {
            return _texSalesInvoiceRepository.Save(modelRecord, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _texSalesInvoiceRepository.Delete(code, common);
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
                        response = _texSalesInvoiceRepository.CopyRecord(record, common, menu);
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

        public MyHttpResponseMessage DeletePurchaseBillDetailByCode(int code, Common common)
        {
            return _texSalesInvoiceRepository.DeletePurchaseBillDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetDataForReport(TexSalesInvoiceRDLCReport modelRecord, DataTable inspectionServiceChargesDetails, Common common)
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

                return _texSalesInvoiceRepository.GetDataForReport(modelRecord, inspectionServiceChargesDetails, currentCompany, common);
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