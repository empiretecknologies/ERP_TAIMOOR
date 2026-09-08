using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class ImportGeneralManifestService : IImportGeneralManifestService
    {
        public IImportGeneralManifestRepository _sodaBookFeedingRepository { get; set; }
        public ImportGeneralManifestService(IImportGeneralManifestRepository sodaBookFeedingRepository)
        {
            _sodaBookFeedingRepository = sodaBookFeedingRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _sodaBookFeedingRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage Save(CustomImportGeneralManifest model, Common common)
        {
            return _sodaBookFeedingRepository.Save(model, common);
        }

        public MyHttpResponseMessage GetImportGeneralManifestByCode(int code, Common common)
        {
            return _sodaBookFeedingRepository.GetImportGeneralManifestByCode(code, common);
        }

        public MyHttpResponseMessage GetImportGeneralManifestDetailByCode(int code, Common common)
        {
            return _sodaBookFeedingRepository.GetImportGeneralManifestDetailByCode(code, common);
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            return _sodaBookFeedingRepository.Delete(code, common);
        }

        public MyHttpResponseMessage DeleteImportGeneralManifestDetailByCode(int code, Common common)
        {
            return _sodaBookFeedingRepository.DeleteImportGeneralManifestDetailByCode(code, common);
        }
    }
}