using System.Data;
using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IImportManifestService
    {
        MyHttpResponseMessage GetChartOfAccounts(Common common);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(CustomImportManifest model, Common common);
        MyHttpResponseMessage GetImportManifestByCode(int code, Common common);
        MyHttpResponseMessage GetImportManifestDetailsByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteImportManifestDetailByCode(int tranID, int code, Common common);
        MyHttpResponseMessage GetDataForReport(ListPrintReport modelRecord, DataTable details, Common common);
    }
}
