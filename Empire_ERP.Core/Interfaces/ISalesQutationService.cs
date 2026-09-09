using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface ISalesQutationService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetSalesQutationByCode(int code, Common common);
        MyHttpResponseMessage GetSalesQutationDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomSalesQutation modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteSalesQutationDetailByCode(int code, Common common);
        MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable details, Common common);
    }
}
