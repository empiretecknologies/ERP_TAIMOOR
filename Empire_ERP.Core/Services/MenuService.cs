using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class MenuService : IMenuService
    {
        public IMenuRepository _menuRepository { get; set; }
        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetMenu()
        {
            return _menuRepository.GetMenu();
        }

        public MyHttpResponseMessage GetMenuByRole(int? id)
        {
            return _menuRepository.GetMenuByRole(id);
        }

        public MyHttpResponseMessage GetMenu(int id)
        {
            return _menuRepository.GetMenu(id);
        }

        public MyHttpResponseMessage GetMenuCustomDetails(int menuID)
        {
            return _menuRepository.GetMenuCustomDetails(menuID);
        }

        public MyHttpResponseMessage GetMenuDetails(int menuID)
        {
            return _menuRepository.GetMenuDetails(menuID);
        }
    }
}