using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class RegionService : IRegionService
    {

        public IRegionRepository _regionRepository { get; set; }
        public RegionService(IRegionRepository regionRepository)
        {
            _regionRepository = regionRepository;
        }

        public MyHttpResponseMessage GetRegionsDropDown(int menuid)
        {
            return _regionRepository.GetRegionsDropDown(menuid);
        }

        public MyHttpResponseMessage GetRegions(Common common)
        {
            return _regionRepository.GetRegions(common);
        }

        public MyHttpResponseMessage GetAccountsForTreeView(Common common)
        {
            return _regionRepository.GetAccountsForTreeView(common);
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _regionRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(Region model, Common common)
        {
            return _regionRepository.Save(model, common);
        }

        public string GenerateNextId(Common common)
        {
            return _regionRepository.GenerateNextId(common);
        }

        public string GenerateGrCode(string ParentId, Common common)
        {
            return _regionRepository.GenerateGrCode(ParentId, common);
        }

        public MyHttpResponseMessage GetRegionById(int id, Common common)
        {
            return _regionRepository.GetRegionById(id, common);
        }

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            return _regionRepository.Delete(id, common);
        }
    }
}
