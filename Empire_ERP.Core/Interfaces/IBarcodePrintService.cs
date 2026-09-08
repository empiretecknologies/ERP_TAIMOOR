using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IBarcodePrintService
    {
		MyHttpResponseMessage GetData(string sdate,string edate);
        string GenerateBarcode(string content, string path);
    }
}