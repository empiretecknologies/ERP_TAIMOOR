using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class StockTransferService : IStockTransferService
    {
        public IStockTransferRepository _stockTransferRepository { get; set; }
        public IBranchService _branchService { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public StockTransferService(IStockTransferRepository stockTransferRepository, IBranchService branchService, IMenuService menuService, ICompanyService companyService)
        {
            _stockTransferRepository = stockTransferRepository;
            _branchService = branchService;
            _menuService = menuService;            _companyService = companyService;

        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _stockTransferRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetStockTransferByCode(int code, Common common)
        {
            return _stockTransferRepository.GetStockTransferByCode(code, common);
        }

        public MyHttpResponseMessage GetStockTransferDetailByCode(int code, Common common)
        {
            return _stockTransferRepository.GetStockTransferDetailByCode(code, common);
        }
        public MyHttpResponseMessage GetPrintData(int code, Common common)
        {
            return _stockTransferRepository.GetPrintData(code, common);
        }

        public MyHttpResponseMessage Save(CustomStockTransfer modelRecord, Common common)
        {
            return _stockTransferRepository.Save(modelRecord, common);
        }

		public MyHttpResponseMessage Delete(int code, Common common)
		{
			return _stockTransferRepository.Delete(code, common);
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
                        response = _stockTransferRepository.CopyRecord(record, common, menu);
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

        public MyHttpResponseMessage DeleteStockTransferDetailByCode(int code, Common common)
        {
            return _stockTransferRepository.DeleteStockTransferDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, int branch, Common common)
        {
            return _stockTransferRepository.GetSodaBookFeedingDetailBySodaDate(sodaDate, branch, common);
        }

        public MyHttpResponseMessage GetDataForCartonSticker(List<StockTransferStickerPrint> stockData)
        {
            return _stockTransferRepository.GetDataForCartonSticker(stockData);
        }

        public MyHttpResponseMessage GetStockTransferDetailByItem(int code, int qty, Common common)
        {
            return _stockTransferRepository.GetStockTransferDetailByItem(code, qty, common);
        }

        public MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable dataTable, Common common)
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

                var currentBranchResponse = _branchService.GetBranchByCode(common.Branch);
                if (currentBranchResponse.msgType != 1)
                {
                    response.msgType = 2;
                    return response;
                }

                //var fromBranchResponse = _branchService.GetBranchByCode(Convert.ToString(modelRecord.Master.BCODE));
                //if (fromBranchResponse.msgType != 1)
                //{
                //    response.msgType = 2;
                //    return response;
                //}
                
                //var toBranchResponse = _branchService.GetBranchByCode(Convert.ToString(modelRecord.Master.TBCODE));
                //if (toBranchResponse.msgType != 1)
                //{
                //    response.msgType = 2;
                //    return response;
                //}

                //var menuDetails = (CustomMenuDetail)menuResponse.data;
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
                var currentBranch = (Branch)currentBranchResponse.data;
                var currentCompany = (Company)currentCompanyResponse.data;
                //var fromBranch = (Branch)fromBranchResponse.data;
                //var toBranch = (Branch)toBranchResponse.data;
                //if (String.IsNullOrWhiteSpace(currentBranch.B_TYPE))
                //{
                //    response.msgType = 2;
                //    return response;
                //}

                return _stockTransferRepository.GetDataForPrintReport(modelRecord, dataTable, menuDetail, currentCompany, currentBranch, common);
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

        public MyHttpResponseMessage UpdatePrintStatus(string codes, Common common)
        {
            return _stockTransferRepository.UpdatePrintStatus(codes, common);
        }

        public MyHttpResponseMessage GetBarcodeList()
        {
            return _stockTransferRepository.GetBarcodeList();
        }
    }
}