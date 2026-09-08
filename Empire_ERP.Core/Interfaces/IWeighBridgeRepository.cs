using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IWeighBridgeRepository
    {
        MyHttpResponseMessage GetGrid(Common common);
    }
}