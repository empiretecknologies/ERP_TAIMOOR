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
        public UploadItemImagesService(IUploadItemImagesRepository UploadItemImagesRepository)
        {
            _UploadItemImagesRepository = UploadItemImagesRepository;
        }

        public MyHttpResponseMessage GetItemMaster(string? sDate, string? currentDate, Common common)
        {
            return _UploadItemImagesRepository.GetItemMaster(sDate,currentDate,common);
        }

        public MyHttpResponseMessage Save(List<UploadItemImages> modelRecord, Common common)
        {
            return _UploadItemImagesRepository.Save(modelRecord, common);
        }
    }
}