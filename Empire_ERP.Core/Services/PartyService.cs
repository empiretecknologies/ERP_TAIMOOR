using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class PartyService : IPartyService
    {
        public IPartyRepository _partyRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public PartyService(IPartyRepository partyRepository, IMenuService menuService)
        {
            _partyRepository = partyRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage GetChartOfAccounts(Common common)
        {
            return _partyRepository.GetChartOfAccounts(common);
        }

        public MyHttpResponseMessage Save(PartyTypes partyTypes, Common common)
        {
            return _partyRepository.Save(partyTypes, common);
        }

        public MyHttpResponseMessage Delete(int partyCode, int actCode, Common common)
        {
            return _partyRepository.Delete(partyCode, actCode, common);
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
                        response = _partyRepository.CopyRecord(record, common, menu);
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

        public MyHttpResponseMessage QuickSearch(int partyCode, Common common)
        {
            return _partyRepository.QuickSearch(partyCode, common);
        }

        public MyHttpResponseMessage QuickSearchParty(Common common)
        {
            return _partyRepository.QuickSearchParty(common);
        }

        //public MyHttpResponseMessage QuickSearchLazyLoading(Common common, int skip = 0, int take = 12, string filter = null, string group = null)
        //{
        //    return _partyRepository.QuickSearchLazyLoading(common, skip, take, filter, group);
        //}

        public MyHttpResponseMessage GetPartyTypeByPartyCode(int partyCode, Common common)
        {
            return _partyRepository.GetPartyTypeByPartyCode(partyCode, common);
        }

        public MyHttpResponseMessage GetBranchesInfoByPartyCode(int partyCode, Common common)
        {
            return _partyRepository.GetBranchesInfoByPartyCode(partyCode, common);
        }

        public MyHttpResponseMessage SaveBranchInfo(PartyTypeBranch partyTypesBranch, Common common)
        {
            return _partyRepository.SaveBranchInfo(partyTypesBranch, common);
        }

        public MyHttpResponseMessage GetBranchInfoByBranchId(int branchId, int partyCode, Common common)
        {
            return _partyRepository.GetBranchInfoByBranchId(branchId, partyCode, common);
        }

        public MyHttpResponseMessage DeleteBranchInfo(int branchId, int partyCode, Common common)
        {
            return _partyRepository.DeleteBranchInfo(branchId, partyCode, common);
        }

        public MyHttpResponseMessage GetPartyTypeByPartyCode(int partyCode)
        {
            return _partyRepository.GetPartyTypeByPartyCode(partyCode);
        }
    }
}