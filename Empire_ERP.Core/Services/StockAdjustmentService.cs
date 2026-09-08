using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System.Data;

namespace Empire_ERP.Core.Services
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        public IStockAdjustmentRepository _stockAdjustmentRepository { get; set; }
        public IBranchService _branchService { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public StockAdjustmentService(IStockAdjustmentRepository stockAdjustmentRepository, IBranchService branchService, IMenuService menuService, ICompanyService companyService)
        {
            _stockAdjustmentRepository = stockAdjustmentRepository;
            _branchService = branchService;
            _menuService = menuService;            _companyService = companyService;

        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _stockAdjustmentRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetStockAdjustmentByCode(int code, Common common)
        {
            return _stockAdjustmentRepository.GetStockAdjustmentByCode(code, common);
        }

        public MyHttpResponseMessage GetStockAdjustmentDetailByCode(int code, Common common)
        {
            return _stockAdjustmentRepository.GetStockAdjustmentDetailByCode(code, common);
        }

        public MyHttpResponseMessage Save(CustomStockAdjustment modelRecord, Common common)
        {
            return _stockAdjustmentRepository.Save(modelRecord, common);
        }

		public MyHttpResponseMessage Delete(int code, Common common)
		{
			return _stockAdjustmentRepository.Delete(code, common);
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
                        response = _stockAdjustmentRepository.CopyRecord(record, common, menu);
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

        public MyHttpResponseMessage DeleteStockAdjustmentDetailByCode(int code, Common common)
        {
            return _stockAdjustmentRepository.DeleteStockAdjustmentDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, int branch, Common common)
        {
            return _stockAdjustmentRepository.GetSodaBookFeedingDetailBySodaDate(sodaDate, branch, common);
        }

        public MyHttpResponseMessage GetDataForCartonSticker(List<StockAdjustmentStickerPrint> stockData)
        {
            return _stockAdjustmentRepository.GetDataForCartonSticker(stockData);
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

                return _stockAdjustmentRepository.GetDataForPrintReport(modelRecord, dataTable, menuDetail, currentCompany, currentBranch, common);
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
            return _stockAdjustmentRepository.UpdatePrintStatus(codes, common);
        }

        public MyHttpResponseMessage GetStockAdjustmentPickDetailByCode(int code, Common common)
        {
            return _stockAdjustmentRepository.GetStockAdjustmentPickDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetStockAdjustmentDetailByItem(int code, int qty, Common common)
        {
            return _stockAdjustmentRepository.GetStockAdjustmentDetailByItem(code, qty, common);
        }

        public MyHttpResponseMessage GetBarcodeList()
        {
            return _stockAdjustmentRepository.GetBarcodeList();
        }
    }
}