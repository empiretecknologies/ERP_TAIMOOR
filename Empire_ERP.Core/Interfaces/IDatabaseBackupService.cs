using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IDatabaseBackupService
    {
		MyHttpResponseMessage GetDatabaseInformation();
        MyHttpResponseMessage GenerateDatabaseBackup(string rootPath, Common common);
    }
}