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
    public class WarehouseService : IWarehouseService
    {
        public IWarehouseRepository _warehouseRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public WarehouseService(IWarehouseRepository warehouseRepository, IMenuService menuService)
        {
            _warehouseRepository = warehouseRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage GetWarehouseAccountsForTreeView(Common common)
        {
            return _warehouseRepository.GetWarehouseAccountsForTreeView(common);
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _warehouseRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(Warehouse model, Common common)
        {
            return _warehouseRepository.Save(model, common);
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
                        response = _warehouseRepository.CopyRecord(record, common, menu);
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
            return _warehouseRepository.GenerateNextId(common);
        }

        public string GenerateGrCode(string ParentId, Common common)
        {
            return _warehouseRepository.GenerateGrCode(ParentId, common);
        }

        public MyHttpResponseMessage GetWarehouseAccountById(int code, Common common)
        {
            return _warehouseRepository.GetWarehouseAccountById(code, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _warehouseRepository.Delete(code, common);
        }
    }
}