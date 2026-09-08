using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IImportGeneralManifestRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(CustomImportGeneralManifest model, Common common);
        MyHttpResponseMessage GetImportGeneralManifestByCode(int code, Common common);
        MyHttpResponseMessage GetImportGeneralManifestDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteImportGeneralManifestDetailByCode(int code, Common common);
    }
}