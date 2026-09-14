using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IUploadItemImagesRepository
	{
        MyHttpResponseMessage GetItemMaster(string? sDate, string? currentDate,Common common);
        MyHttpResponseMessage GetPartyBranches(int partyCode, int actCode);
        MyHttpResponseMessage Save(List<UploadItemImages> modelRecord, Common common);
    }
}