using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IShipmentRepository
    {
        MyHttpResponseMessage QuickSearch(Common common);
        MyHttpResponseMessage GetShipmentByCode(int code, Common common);
        MyHttpResponseMessage GetShipmentDetailByCode(int code, Common common);
        MyHttpResponseMessage Save(CustomShipment modelRecord, Common common);
        MyHttpResponseMessage Delete(int code, Common common);
        MyHttpResponseMessage DeleteShipmentDetailByCode(int code, Common common);
        MyHttpResponseMessage GetSodaBookFeedingDetail(Common common);
    }
}