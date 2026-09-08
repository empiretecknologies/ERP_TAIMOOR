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
    public class GradeService : IGradeService
    {
        public IGradeRepository _GradeRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public GradeService(IGradeRepository GradeRepository, IMenuService menuService)
        {
            _GradeRepository = GradeRepository;
            _menuService = menuService;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _GradeRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(Grade model, Common common)
        {
            return _GradeRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _GradeRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetGradeById(int id, Common common)
        {
            return _GradeRepository.GetGradeById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _GradeRepository.Delete(id, common);
        }

        public MyHttpResponseMessage CopyRecord(CopyRecord record, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                MyHttpResponseMessage Menu = _menuService.GetMenu(common.MenuID);
                string table = string.Empty;
                Menu menu = new Menu();
                if (Menu.data != null)
                {
                    menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                }
                if (!string.IsNullOrWhiteSpace(table))
                {
                    if (record.TRAN_ID == 0)
                    {
                        response.msg = "ID is not in numeric format";
                        response.msgType = 2;
                    }
                    else
                    {
                        response = _GradeRepository.CopyRecord(record, common, menu);
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
                    _catchMessage = _catchMessage + "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }
    }
}