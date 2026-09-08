using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class ShipmentService : IShipmentService
    {
        public IShipmentRepository _shipmentRepository { get; set; }
        public ShipmentService(IShipmentRepository materialRequisitionRepository)
        {
            _shipmentRepository = materialRequisitionRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            return _shipmentRepository.QuickSearch(common);
        }

        public MyHttpResponseMessage GetShipmentByCode(int code, Common common)
        {
            return _shipmentRepository.GetShipmentByCode(code, common);
        }

        public MyHttpResponseMessage GetShipmentDetailByCode(int code, Common common)
        {
            return _shipmentRepository.GetShipmentDetailByCode(code, common);
        }

        public MyHttpResponseMessage Save(CustomShipment modelRecord, Common common)
        {
            return _shipmentRepository.Save(modelRecord, common);
        }

		public MyHttpResponseMessage Delete(int code, Common common)
		{
			return _shipmentRepository.Delete(code, common);
		}

        public MyHttpResponseMessage DeleteShipmentDetailByCode(int code, Common common)
        {
            return _shipmentRepository.DeleteShipmentDetailByCode(code, common);
        }

        public MyHttpResponseMessage GetSodaBookFeedingDetail(Common common)
        {
            return _shipmentRepository.GetSodaBookFeedingDetail(common);
        }
    }
}