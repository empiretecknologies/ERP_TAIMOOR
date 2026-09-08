using System.Data;
using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IImportManifestRepository
    {
        MyHttpResponseMessage GetChartOfAccounts(int? pType);
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(CustomImportManifest model, Common common);
        MyHttpResponseMessage GetImportManifestByCode(int code, Common common);
        MyHttpResponseMessage GetImportManifestDetailsByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteImportManifestDetailByCode(int tranID, int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(ListPrintReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}
