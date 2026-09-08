using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface ISaleTaxInvoiceService
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetSaleTaxInvoiceByCode(int code, Common common);
        MyHttpResponseMessage GetSaleTaxInvoiceDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomSaleTaxInvoice modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage GetDataForApi(int code, Common common);
        List<SaleTaxInvoiceDetail> GetDetailDataForApi(int code, Common common);
        MyHttpResponseMessage FBRApi_Status(string code, string apiResponce, Common common);
        MyHttpResponseMessage DeleteSaleTaxInvoiceDetailByCode(int code, Common common);
        //MyHttpResponseMessage GetDataForReport(CashReceiptRDLCReport modelRecord, DataTable details, DataTable taxDetails, DataTable inspectionServiceChargesDetails, Common common);
        MyHttpResponseMessage GetDataForReport(SaleTaxInvoiceRDLCReport modelRecord, DataTable details, Common common);
    }
}