using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IUploadItemImagesService
    {
		MyHttpResponseMessage GetItemMaster( string? SDate,string? currentDate , Common common);
        MyHttpResponseMessage Save(List<UploadItemImages> modelRecord, Common common);
    }
}