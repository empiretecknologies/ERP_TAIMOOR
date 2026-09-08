using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface ICommonService
    {
        MyHttpResponseMessage GenerateQRCode(string content, string path);
        MyHttpResponseMessage GenerateBarCode(string content, string path);
        bool CreateZipFromSpecificFile(string sourceFilePath, string destinationZipFilePath, string password);
        string ToAccountingFormat(decimal value);
    }
}
