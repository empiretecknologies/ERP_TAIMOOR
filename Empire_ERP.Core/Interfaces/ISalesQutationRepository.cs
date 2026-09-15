using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface ISalesQutationRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetSalesQutationByCode(int code, Common common);
        MyHttpResponseMessage GetSalesQutationDetailByCode(int code, Common common);
        MyHttpResponseMessage GetPartyLastItemRates(int partyCode, int actCode, Common common);
        MyHttpResponseMessage Save(CustomSalesQutation modelRecord, Common common);
        MyHttpResponseMessage GetPartyBranches(int partyCode, int actCode);
        MyHttpResponseMessage GetExcelItemLookup();
        MyHttpResponseMessage SaveExcelBatch(List<CustomSalesQutation> modelRecords, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteSalesQutationDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Branch currentBranch, Common common);
    }
}
