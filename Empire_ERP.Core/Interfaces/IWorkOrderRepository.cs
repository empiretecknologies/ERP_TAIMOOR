using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IWorkOrderRepository
    {
        MyHttpResponseMessage QuickSearch(Common common, Menu menu);
        MyHttpResponseMessage Save(CustomWorkOrder model, Common common, Menu menu);
        MyHttpResponseMessage GetWorkOrderByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetWorkOrderDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetBatchDetailByProcess(int process, Common common);
        MyHttpResponseMessage Delete(int code, Common common, Menu menu);
        MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu);
        MyHttpResponseMessage DeleteWorkOrderDetailByCode(int code, Common common, Menu menu);
        MyHttpResponseMessage GetDataForReport(WorkOrderRDLCReport modelRecord, DataTable details, CustomMenuDetail menuDetails, Company currentCompany, Common common);
    }
}