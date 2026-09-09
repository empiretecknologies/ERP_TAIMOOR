using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System.Data;

namespace Empire_ERP.Core.Services
{
    public class SalesQutationService : ISalesQutationService
    {
        public ISalesQutationRepository _salesQutationRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public IBranchService _branchService { get; set; }
        public SalesQutationService(ISalesQutationRepository salesQutationRepository, IMenuService menuService, ICompanyService companyService, IBranchService branchService)
        {
            _salesQutationRepository = salesQutationRepository;
            _menuService = menuService;
            _companyService = companyService;
            _branchService = branchService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _salesQutationRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetSalesQutationByCode(int code, Common common)
        {
            return _salesQutationRepository.GetSalesQutationByCode(code, common);
        }

        public MyHttpResponseMessage GetSalesQutationDetailByCode(int code, Common common)
        {
            return _salesQutationRepository.GetSalesQutationDetailByCode(code, common);
        }

        public MyHttpResponseMessage Save(CustomSalesQutation modelRecord, Common common)
        {
            return _salesQutationRepository.Save(modelRecord, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _salesQutationRepository.Delete(code, common);
        }

        public MyHttpResponseMessage DeleteSalesQutationDetailByCode(int code, Common common)
        {
            return _salesQutationRepository.DeleteSalesQutationDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable dataTable, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            CustomMenuDetail menuDetail = new CustomMenuDetail();

            try
            {
                var menuResponse = _menuService.GetMenuDetails(common.MenuID);
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

                if (menuResponse.msgType == 1 && menuResponse.data != null)
                {
                    var menuData = (List<CustomMenuDetail>)menuResponse.data;
                    if (menuData != null && menuData.Count > 0)
                    {
                        if (modelRecord.MD_ID > 0)
                        {
                            menuDetail = menuData.Where(m => m.MD_ID == modelRecord.MD_ID).FirstOrDefault();
                        }
                        if (menuDetail == null || menuDetail.MD_ID <= 0)
                        {
                            menuDetail = menuData.FirstOrDefault();
                        }
                    }
                }

                var currentBranch = (Branch)currentBranchResponse.data;
                var currentCompany = (Company)currentCompanyResponse.data;
                if (menuDetail == null)
                {
                    menuDetail = new CustomMenuDetail();
                }

                return _salesQutationRepository.GetDataForPrintReport(modelRecord, dataTable, menuDetail, currentCompany, currentBranch, common);
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
