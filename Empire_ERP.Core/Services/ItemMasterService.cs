using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class ItemMasterService : IItemMasterService
    {
        public IItemMasterRepository _itemMasterRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ItemMasterService(IItemMasterRepository itemMasterRepository, IMenuService menuService)
        {
            _itemMasterRepository = itemMasterRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _itemMasterRepository.QuickSearch(common);
        }

        //public MyHttpResponseMessage QuickSearch(Common common, int skip = 0, int take = 12, string filter = null, string group = null)
        //{
        //    return _itemMasterRepository.QuickSearch(common, skip, take, filter, group);
        //}

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _itemMasterRepository.Delete(code, common);
        }

        public MyHttpResponseMessage AttributeDelete(int code, Common common)
        {
            return _itemMasterRepository.AttributeDelete(code, common);
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
                        response = _itemMasterRepository.CopyRecord(record, common, menu);
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


        public MyHttpResponseMessage Save(ItemMaster itemMaster, Common common)
        {
            return _itemMasterRepository.Save(itemMaster, common);
        }

        public MyHttpResponseMessage GetItemMasterByCode(int code, Common common)
        {
            return _itemMasterRepository.GetItemMasterByCode(code, common);
        }

        public MyHttpResponseMessage GetBarcodeInfoByCode(int code, Common common)
        {
            return _itemMasterRepository.GetBarcodeInfoByCode(code, common);
        }

        public MyHttpResponseMessage SaveBarcodeInfo(Barcode barCode, Common common)
        {
            return _itemMasterRepository.SaveBarcodeInfo(barCode, common);
        }

        public MyHttpResponseMessage SaveAttributeInfo(ItemAttribute itemAttribute, Common common)
        {
            return _itemMasterRepository.SaveAttributeInfo(itemAttribute, common);
        }

        public MyHttpResponseMessage GetBarcodeInfoByBarcodeId(int barCodeId, int code, Common common)
        {
            return _itemMasterRepository.GetBarcodeInfoByBarcodeId(barCodeId, code, common);
        }

        public MyHttpResponseMessage DeleteBarcodeInfo(int barCodeId, int code, Common common)
        {
            return _itemMasterRepository.DeleteBarcodeInfo(barCodeId, code, common);
        }
    }
}