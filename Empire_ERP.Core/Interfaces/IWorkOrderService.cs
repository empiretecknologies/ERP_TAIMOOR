using Empire_ERP.Core.Entities;
using System.Data;

namespace Empire_ERP.Core.Interfaces
{
    public interface IWorkOrderService
    {
		MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage Save(CustomWorkOrder model, Common common);
        MyHttpResponseMessage GetWorkOrderByCode(int code, Common common);
        MyHttpResponseMessage GetWorkOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage CopyRecord(CopyRecord code, Common common);
        MyHttpResponseMessage DeleteWorkOrderDetailByCode(int code, Common common);
        MyHttpResponseMessage GetBatchDetailByProcess(int process, Common common);
        MyHttpResponseMessage GetDataForReport(WorkOrderRDLCReport modelRecord, DataTable details, Common common);
    }
}