using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class MenuDetailsService : IMenuDetailsService
    {
        public IMenuDetailsRepository _menuDetailsRepository { get; set; }
        public MenuDetailsService(IMenuDetailsRepository menuDetailsRepository)
        {
            _menuDetailsRepository = menuDetailsRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _menuDetailsRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(MenuDetails model, Common common)
        {
            return _menuDetailsRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _menuDetailsRepository.GenerateNextId(common);
        }

        public MyHttpResponseMessage GetMenuDetailsById(int id, Common common)
        {
            return _menuDetailsRepository.GetMenuDetailsById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _menuDetailsRepository.Delete(id, common);
        }
    }
}