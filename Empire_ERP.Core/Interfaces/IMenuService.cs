using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IMenuService
    {
        MyHttpResponseMessage GetMenu();
        MyHttpResponseMessage GetMenuByRole(int? id);
        MyHttpResponseMessage GetMenu(int id);
        MyHttpResponseMessage GetMenuCustomDetails(int menuID);
        MyHttpResponseMessage GetMenuDetails(int menuID);
    }
}