using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class ItemGroupService: IItemGroupService
    {
        public IItemGroupRepository _itemGroupRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ItemGroupService(IItemGroupRepository itemGroupRepository, IMenuService menuService)
        {
            _itemGroupRepository = itemGroupRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage GetItemGroupsForTreeView(Common common)
        {
            return _itemGroupRepository.GetItemGroupsForTreeView(common);
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _itemGroupRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(ItemGroup model, Common common)
        {
            return _itemGroupRepository.Save(model, common);
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
                        response = _itemGroupRepository.CopyRecord(record, common, menu);
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

        public string GenerateNextId(Common common)
        {
            return _itemGroupRepository.GenerateNextId(common);
        }

        public string GenerateGrCode(string ParentId, Common common)
        {
            return _itemGroupRepository.GenerateGrCode(ParentId, common);
        }

        public MyHttpResponseMessage GetItemGroupById(int id, Common common)
        {
            return _itemGroupRepository.GetItemGroupById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _itemGroupRepository.Delete(id, common);
        }
    }
}
