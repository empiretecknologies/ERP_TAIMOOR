using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IBarcodePrintRepository
    {
        MyHttpResponseMessage GetData(string sdate,string fdate);
        string GenerateBarcode(string content, string path);
    }
}