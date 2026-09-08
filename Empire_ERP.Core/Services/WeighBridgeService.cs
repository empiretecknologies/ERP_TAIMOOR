using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class WeighBridgeService : IWeighBridgeService
    {
        public IWeighBridgeRepository _weighBridgeRepository { get; set; }
        public WeighBridgeService(IWeighBridgeRepository weighBridgeRepository)
        {
            _weighBridgeRepository = weighBridgeRepository;
        }

        public MyHttpResponseMessage GetGrid(Common common)
        {
          return _weighBridgeRepository.GetGrid(common);
        }
    }
}