using Azure;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode.Internal;

namespace Empire_ERP.Core.Services
{
    public class POSTransactionService : IPOSTransactionService
    {
        public IPOSTransactionRepository _posTransactionRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public IBranchService _branchService { get; set; }
        public IPOSMappingService _mappingService { get; set; }
        public ILoginService _loginService { get; set; }
        public POSTransactionService(IPOSTransactionRepository deliverFeedingRepository, IMenuService menuService, ICompanyService companyService, IBranchService branchService,IPOSMappingService mappingService , ILoginService loginService)
        {
            _posTransactionRepository = deliverFeedingRepository;
            _menuService = menuService;
            _companyService = companyService;
            _branchService = branchService;
            _loginService = loginService;
            _mappingService = mappingService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _posTransactionRepository.QuickSearch(common);
        }
        public MyHttpResponseMessage GetSyncData(Common common)
        {
            return _posTransactionRepository.GetSyncData(common);
        }
        public MyHttpResponseMessage GetPendingRecords(DateTime FromDate , DateTime ToDate , Common common)
        {
            return _posTransactionRepository.GetPendingRecords(FromDate , ToDate ,common);
        }
        public MyHttpResponseMessage GetPayQuickSearch(Common common)
        {
            return _posTransactionRepository.GetPayQuickSearch(common);
        }
        public MyHttpResponseMessage AdvanceBookingRecords(Common common)
        {
            return _posTransactionRepository.AdvanceBookingRecords(common);
        }
        public MyHttpResponseMessage GetCustomerHistory(Common common , string CstNumber)
        {
            return _posTransactionRepository.GetCustomerHistory(common , CstNumber);
        }
        public MyHttpResponseMessage GetMapData(Common common)
        {
            return _posTransactionRepository.GetMapData(common);
        }
        public MyHttpResponseMessage SRBApi_Status(string voucherno, string invoiceId, Common common)
        {
            return _posTransactionRepository.SRBApi_Status(voucherno , invoiceId, common);
        }

        public MyHttpResponseMessage Save(CustomPOSTransaction model, Common common)
        {
            CustomMenuDetail menuDetail = new CustomMenuDetail();
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {

                var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
                if (currentCompanyResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }

                var currentCompany = (Company)currentCompanyResponse.data;

                var currentBranchResponse = _branchService.GetBranchByCode(common.Branch);
                if (currentBranchResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }

                var currentBranch = (Branch)currentBranchResponse.data;

                var currentLableResponse = _loginService.GetBackGroundAndLogo();
                var currentLabel = (Info)currentLableResponse;

                var menuResponse = _menuService.GetMenuDetails(common.MenuID);
                if (menuResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }
                var menuData = (List<CustomMenuDetail>)menuResponse.data;
                if (menuData.Count > 0)
                {
                    if (model.Master.MD_ID != 0)
                    {
                        menuDetail = menuData.Where(m => m.MD_ID == model.Master.MD_ID).FirstOrDefault();
                    }
                    else if (model.Master.MD_ID == 27)
                    {
                        menuDetail = menuData.Where(m => m.MD_ID == 27).FirstOrDefault();
                    }
                    else
                    {
                        menuDetail = menuData.FirstOrDefault();
                    }


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




                return _posTransactionRepository.Save(model, menuDetail, common, currentCompany, currentBranch, currentLabel);
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
        
        public MyHttpResponseMessage UpdateTablesAndWaiter(CustomPOSTransaction model, Common common)
        {
            return _posTransactionRepository.UpdateTablesAndWaiter(model, common);
        }

        public MyHttpResponseMessage GetPOSTransactionByCode(int code, string voucher, Common common)
        {
            return _posTransactionRepository.GetPOSTransactionByCode(code, voucher, common);
        }
        public MyHttpResponseMessage GetExpenseByCode(int code, Common common)
        {
            return _posTransactionRepository.GetExpenseByCode(code, common);
        }

        public MyHttpResponseMessage GetPOSTransactionDetailByCode(int? code, Common common)
        {
            return _posTransactionRepository.GetPOSTransactionDetailByCode(code, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _posTransactionRepository.Delete(code, common);
        }
        public MyHttpResponseMessage DeleteExpenseRow(int code, Common common)
        {
            return _posTransactionRepository.DeleteExpenseRow(code, common);
        }
        public MyHttpResponseMessage GetBarcodeList(int ItemId, Common common)
        {
            return _posTransactionRepository.GetBarcodeList(ItemId, common);
        }
        public MyHttpResponseMessage GetAllBarcodeList(Common common)
        {
            return _posTransactionRepository.GetAllBarcodeList(common);
        }

        public MyHttpResponseMessage DeletePOSTransactionDetailByCode(CustomPOSTransaction modelRecord, int code, int ItemId, string dtcode, Common common)
        {
            return _posTransactionRepository.DeletePOSTransactionDetailByCode(modelRecord, code, ItemId, dtcode, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, Common common)
        {
            return _posTransactionRepository.GetSodaBookFeedingDetailBySodaDate(sodaDate, common);
        }
        public MyHttpResponseMessage GetItemsGroup(Common common)
        {
            return _posTransactionRepository.GetItemsGroup(common);
        }
        public MyHttpResponseMessage GetItemsMasterByGroup(int groupId, Common common)
        {
            return _posTransactionRepository.GetItemsMasterByGroup(groupId, common);
        }
        public MyHttpResponseMessage GetItemsMasterByCode(int itemId, string barcode, int Qty, int TranId, Common common)
        {
            return _posTransactionRepository.GetItemsMasterByCode(itemId, barcode, Qty, TranId, common);
        }
        public MyHttpResponseMessage GetAllDiscount(Common common)
        {
            return _posTransactionRepository.GetAllDiscount(common);
        }
        public MyHttpResponseMessage GetAllWaiter(Common common)
        {
            return _posTransactionRepository.GetAllWaiter(common);
        }
        public MyHttpResponseMessage GetAllTables(Common common, string Tran_Id)
        {
            return _posTransactionRepository.GetAllTables(common, Tran_Id);
        }
        public MyHttpResponseMessage ExpenseRecord(Common common)
        {
            return _posTransactionRepository.ExpenseRecord(common);
        }
        public MyHttpResponseMessage ExpenseRecordSave(ExpensePOSTransaction modelRecord, Common common)
        {
            return _posTransactionRepository.ExpenseRecordSave(modelRecord, common);
        }

        public MyHttpResponseMessage GetDynamicIcon()
        {
            return _posTransactionRepository.GetDynamicIcon();
        }

        public MyHttpResponseMessage GetDataForReport(POSPrint_Model modelRecord, DataTable dataTable, Common common)
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
                    if (menuData.Any(m => m.MD_ID == 36) && modelRecord.BillStatus == "K")
                    {
                        menuDetail = menuData.FirstOrDefault(m => m.MD_ID == 36) ?? menuData.FirstOrDefault();
                    }
                    else if (menuData.Any(m => m.MD_ID == 27))
                    {
                        menuDetail = menuData.FirstOrDefault(m => m.MD_ID == 27) ?? menuData.FirstOrDefault();
                    }
                    else if (menuData.Any(m => m.MD_ID == 28))
                    {
                        menuDetail = menuData.FirstOrDefault(m => m.MD_ID == 28) ?? menuData.FirstOrDefault();
                    }
                    else if (menuData.Any(m => m.MD_ID == 47))
                    {
                        menuDetail = menuData.FirstOrDefault(m => m.MD_ID == 47) ?? menuData.FirstOrDefault();
                    }
                    else if (menuData.Any(m => m.MD_ID == 51))
                    {
                        menuDetail = menuData.FirstOrDefault(m => m.MD_ID == 51) ?? menuData.FirstOrDefault();
                    }
                    else if (menuData.Any(m => m.MD_ID == 63))
                    {
                        menuDetail = menuData.FirstOrDefault(m => m.MD_ID == 63) ?? menuData.FirstOrDefault();
                    }

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

                var currentBranchResponse = _branchService.GetBranchByCode(common.Branch);
                if (currentBranchResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }

                var currentBranch = (Branch)currentBranchResponse.data;
                
                var currentMappingResponse = _mappingService.GetMapping(common);
                if (currentMappingResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }

                var currentMapping = (POSMapping)currentMappingResponse.data;

                var currentLableResponse = _loginService.GetBackGroundAndLogo();
                var currentLabel = (Info)currentLableResponse;

                return _posTransactionRepository.GetDataForReport(modelRecord, menuDetail, currentLabel, currentBranch, currentCompany, currentMapping, common);
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

        public MyHttpResponseMessage ExpensePrintReport(List<ExpensePrint> expenses, Common common)
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
                var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
                if (currentCompanyResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }

                var currentCompany = (Company)currentCompanyResponse.data;

                var currentBranchResponse = _branchService.GetBranchByCode(common.Branch);
                if (currentBranchResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }

                var currentBranch = (Branch)currentBranchResponse.data;

                var currentLableResponse = _loginService.GetBackGroundAndLogo();
                var currentLabel = (Info)currentLableResponse;

                return _posTransactionRepository.ExpensePrintReport(expenses, currentLabel, currentBranch, currentCompany, common);
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