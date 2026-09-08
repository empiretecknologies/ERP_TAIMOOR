using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class DatabaseBackupService : IDatabaseBackupService
    {
        public IDatabaseBackupRepository _databaseBackupRepository { get; set; }
        public DatabaseBackupService(IDatabaseBackupRepository databaseBackupRepository)
        {
            _databaseBackupRepository = databaseBackupRepository;
        }

        public MyHttpResponseMessage GetDatabaseInformation()
        {
            return _databaseBackupRepository.GetDatabaseInformation();
        }

        public MyHttpResponseMessage GenerateDatabaseBackup(string rootPath, Common common)
        {
            return _databaseBackupRepository.GenerateDatabaseBackup(rootPath, common);
        }
    }
}