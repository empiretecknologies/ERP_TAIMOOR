using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class BarcodePrintService : IBarcodePrintService
    {
        public IBarcodePrintRepository _barcodePrintRepository { get; set; }
        public BarcodePrintService(IBarcodePrintRepository barcodePrintRepository)
        {
            _barcodePrintRepository = barcodePrintRepository;
        }

        public MyHttpResponseMessage GetData(string sdate,string edate)
        {
            return _barcodePrintRepository.GetData(sdate,edate);
        }

        public string GenerateBarcode(string content, string path)
        {
            return _barcodePrintRepository.GenerateBarcode(content, path);
        }
    }
}