using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System.Data;

namespace Empire_ERP.Core.Services
{
    public class SaleTaxInvoiceService : ISaleTaxInvoiceService
    {
        public ISaleTaxInvoiceRepository _saleTaxInvoiceRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public SaleTaxInvoiceService(ISaleTaxInvoiceRepository saleTaxInvoiceRepository, IMenuService menuService, ICompanyService companyService)
        {
            _saleTaxInvoiceRepository = saleTaxInvoiceRepository;
            _menuService = menuService;
            _companyService = companyService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _saleTaxInvoiceRepository.QuickSearch(common);
        }

        //public MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null, string sort = null)
        //{
        //    return _saleTaxInvoiceRepository.QuickSearch(common, skip, take, filter, group, sort);
        //}

        public MyHttpResponseMessage GetSaleTaxInvoiceByCode(int code, Common common)
        {
            return _saleTaxInvoiceRepository.GetSaleTaxInvoiceByCode(code, common);
        }

        public MyHttpResponseMessage GetSaleTaxInvoiceDetailByCode(int code, Common common)
        {
            return _saleTaxInvoiceRepository.GetSaleTaxInvoiceDetailByCode(code, common);
        }
        //public MyHttpResponseMessage GetPurchaseBillCommissionByCode(int code, Common common)
        //{
        //    return _saleTaxInvoiceRepository.GetPurchaseBillCommissionByCode(code, common);
        //}

        //public MyHttpResponseMessage GetPickDataByParty(int partyCode, int actCode, Common common)
        //{
        //    return _saleTaxInvoiceRepository.GetPickDataByParty(partyCode, actCode, common);
        //}

        //public MyHttpResponseMessage GetBarcodeList()
        //{
        //    return _saleTaxInvoiceRepository.GetBarcodeList();
        //}

        //public MyHttpResponseMessage GetPurchaseBillPickDetailByCode(int code, Common common)
        //{
        //    return _saleTaxInvoiceRepository.GetPurchaseBillPickDetailByCode(code, common);
        //}

        //public MyHttpResponseMessage GetPurchaseBillDetailByItem(int code, int qty, Common common)
        //{
        //    return _saleTaxInvoiceRepository.GetPurchaseBillDetailByItem(code, qty, common);
        //}

        public MyHttpResponseMessage Save(CustomSaleTaxInvoice modelRecord, Common common)
        {
            return _saleTaxInvoiceRepository.Save(modelRecord, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _saleTaxInvoiceRepository.Delete(code, common);
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
                        response = _saleTaxInvoiceRepository.CopyRecord(record, common, menu);
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
        public MyHttpResponseMessage DeleteSaleTaxInvoiceDetailByCode(int code, Common common)
        {
            return _saleTaxInvoiceRepository.DeleteSaleTaxInvoiceDetailByCode(code, common);
        }
        public MyHttpResponseMessage GetDataForApi(int code, Common common)
        {
            return _saleTaxInvoiceRepository.GetDataForApi(code, common);
        }

        public List<SaleTaxInvoiceDetail> GetDetailDataForApi(int code, Common common)
        {
            return _saleTaxInvoiceRepository.GetDetailDataForApi(code, common);
        }

        public MyHttpResponseMessage FBRApi_Status(string code, string apiResponce, Common common)
        {
            return _saleTaxInvoiceRepository.FBRApi_Status(code, apiResponce, common);
        }
        //public MyHttpResponseMessage GetDataForReport(SaleTaxInvoiceRDLCReport modelRecord, DataTable dataTable, DataTable taxDataTable, DataTable inspectionServiceChargesDetails, Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    CustomMenuDetail menuDetail = new CustomMenuDetail();
        //    try
        //    {
        //        var menuResponse = _menuService.GetMenuDetails(common.MenuID);
        //        if (menuResponse.msgType != 1)
        //        {
        //            response.msgType = 2;
        //            return response;
        //        }
        //        var menuData = (List<CustomMenuDetail>)menuResponse.data;
        //        if (menuData.Count > 0)
        //        {
        //            menuDetail = menuData.Where(m => m.MD_ID == modelRecord.MD_ID).FirstOrDefault();
        //            if (menuDetail?.MD_ID <= 0)
        //            {
        //                response.msgType = 2;
        //                return response;
        //            }
        //        }
        //        else
        //        {
        //            response.msgType = 2;
        //            return response;
        //        }

        //        var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
        //        if (currentCompanyResponse.msgType != 1)
        //        {
        //            response.msgType = 2;
        //            return response;
        //        }

        //        var currentCompany = (Company)currentCompanyResponse.data;

        //        return _saleTaxInvoiceRepository.GetDataForReport(modelRecord, dataTable, taxDataTable, inspectionServiceChargesDetails, menuDetail, currentCompany, common);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        response.msgType = 2;
        //        response.msg = _catchMessage;
        //    }
        //    return response;
        //}

        public MyHttpResponseMessage GetDataForReport(SaleTaxInvoiceRDLCReport modelRecord, DataTable dataTable, Common common)
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
                    menuDetail = menuData.Where(m => m.REPORT_NAME == modelRecord.REPORT_NAME).FirstOrDefault();
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

                return _saleTaxInvoiceRepository.GetDataForReport(modelRecord, dataTable, menuDetail, currentCompany, common);
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