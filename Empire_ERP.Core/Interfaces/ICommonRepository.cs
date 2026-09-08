using Empire_ERP.Core.Entities;
using System.Text.Json.Nodes;

namespace Empire_ERP.Core.Interfaces
{
    public interface ICommonRepository
    {
        MyHttpResponseMessage GenerateQRCode(string content, string path);
        MyHttpResponseMessage GenerateBarCode(string content, string path);
        bool CreateZipFromSpecificFile(string sourceFilePath, string destinationZipFilePath, string password);
        string ToAccountingFormat(decimal value);
        string BuildFilterCondition(JsonArray filterArray);
        string BuildSingleCondition(JsonArray filter);
        object GetPropertyValue(dynamic obj, string propertyName);
    }
}