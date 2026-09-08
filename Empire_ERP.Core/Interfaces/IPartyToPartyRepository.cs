using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IPartyToPartyRepository
    {
        MyHttpResponseMessage GetChartOfAccounts(int? pType);
        MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common);
        //MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common, int skip = 0, int take = 12, string filter = null, string group = null);
        MyHttpResponseMessage Save(List<PartyToParty> model, Common common, Menu menu);
        MyHttpResponseMessage GetPartyToPartyByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetPartyToPartyDetailsByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeletePartyToPartyDetailByCode(int tranID, int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(CashReceiptRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}
