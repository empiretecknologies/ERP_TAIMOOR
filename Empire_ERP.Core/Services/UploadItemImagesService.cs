using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class UploadItemImagesService : IUploadItemImagesService
    {
        public IUploadItemImagesRepository _UploadItemImagesRepository { get; set; }
        public ISalesQutationService _salesQutationService { get; set; }
        public IMenuService _menuService { get; set; }
        public UploadItemImagesService(IUploadItemImagesRepository UploadItemImagesRepository, ISalesQutationService salesQutationService, IMenuService menuService)
        {
            _UploadItemImagesRepository = UploadItemImagesRepository;
            _salesQutationService = salesQutationService;
            _menuService = menuService;
        }

        public MyHttpResponseMessage GetItemMaster(string? sDate, string? currentDate, Common common)
        {
            return _UploadItemImagesRepository.GetItemMaster(sDate,currentDate,common);
        }

        public MyHttpResponseMessage GetPartyBranches(int partyCode, int actCode)
        {
            return _UploadItemImagesRepository.GetPartyBranches(partyCode, actCode);
        }

        public MyHttpResponseMessage GetPartyLastItemRates(int partyCode, int actCode, Common common)
        {
            var menus = _menuService.GetMenu().Menu;
            var sqMenu = menus?.FirstOrDefault(m =>
                m.DLT == "T" &&
                !string.IsNullOrWhiteSpace(m.TABLE1) &&
                !string.IsNullOrWhiteSpace(m.TABLE2) &&
                !string.IsNullOrWhiteSpace(m.MENU_PAGE) &&
                m.MENU_PAGE.IndexOf("SalesQutation", StringComparison.OrdinalIgnoreCase) >= 0);
            if (sqMenu != null)
            {
                common.MenuID = sqMenu.ID;
            }
            return _salesQutationService.GetPartyLastItemRates(partyCode, actCode, common);
        }

        public MyHttpResponseMessage Save(List<UploadItemImages> modelRecord, Common common)
        {
            return _UploadItemImagesRepository.Save(modelRecord, common);
        }
    }
}