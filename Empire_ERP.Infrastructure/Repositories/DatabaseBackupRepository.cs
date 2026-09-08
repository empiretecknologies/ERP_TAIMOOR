using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using static System.Net.Mime.MediaTypeNames;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class DatabaseBackupRepository : IDatabaseBackupRepository
    {
        public ICommonRepository _commonRepository { get; set; }
        public ILoginService _loginService { get; set; }
        public IUserService _userService { get; set; }
        public DatabaseBackupRepository(ICommonRepository commonRepository, ILoginService loginService, IUserService userService)
        {
            _commonRepository = commonRepository;
            _loginService = loginService;
            _userService = userService;
        }

        public MyHttpResponseMessage GetDatabaseInformation()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            string? databaseSize = string.Empty;
            List<object> data = new List<object>();
            try
            {
                string connectionString = new SQLService().getconnstring();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = connection.CreateCommand();
                    command.Transaction = transaction;
                    try
                    {
                        DataSet dts = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.StoredProcedure, "sp_spaceused");
                        if (dts.Tables[0].Rows.Count > 0)
                        {
                            databaseSize = Convert.ToString(dts.Tables[0].Rows[0]["database_size"]);
                        }

                        string query = @"SELECT
                                            DB_NAME() AS DatabaseName,
                                            t.name AS TableName,
                                            t.create_date AS CreationDate,
                                            t.modify_date AS LastModifiedDate,
                                            p.rows AS RowCounts
                                        FROM 
                                            sys.tables t
                                        INNER JOIN      
                                            sys.indexes i ON t.object_id = i.object_id
                                        INNER JOIN 
                                            sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
                                        INNER JOIN 
                                            sys.allocation_units a ON p.partition_id = a.container_id
                                        GROUP BY 
                                            t.name, t.create_date, t.modify_date, p.rows
                                        ORDER BY
                                            t.name;";
                        command.CommandText = query;
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DATABASE_SIZE = databaseSize,
                                DATABASE_NAME = Convert.ToString(reader["DatabaseName"]),
                                TABLE_NAME = Convert.ToString(reader["TableName"]),
                                CREATED_DATE = reader["CreationDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["CreationDate"]).ToString("yyyy-MM-dd"),
                                MODIFIED_DATE = reader["LastModifiedDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["LastModifiedDate"]).ToString("yyyy-MM-dd"),
                                ROW_COUNTS = Convert.ToString(reader["RowCounts"]),
                            };
                            data.Add(row);
                        }
                        reader.Close();


                        response.data = data;
                        response.msg = "";
                        response.msgType = 1;

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        string _catchMessage = ex.Message;
                        if (ex.InnerException != null)
                        {
                            _catchMessage += "<br/>" + ex.InnerException.Message;
                        }
                        response.msg = _catchMessage;
                        response.msgType = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msgType = 2;
                response.msg = _catchMessage;
            }
            return response;
        }

        public MyHttpResponseMessage GenerateDatabaseBackup(string rootPath, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string connectionString = new SQLService().getconnstring();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    if(!Directory.Exists(Path.Combine(rootPath, "Client", "DatabaseBackups")))
                    {
                        Directory.CreateDirectory(Path.Combine(rootPath, "Client", "DatabaseBackups"));
                    }

                    if (!Directory.Exists(Path.Combine(rootPath, "Client", "TempDatabaseBackupZips")))
                    {
                        Directory.CreateDirectory(Path.Combine(rootPath, "Client", "TempDatabaseBackupZips"));
                    }

                    var name = $"EmpireErp_{connection.Database}_{Convert.ToDateTime(CommonService.GetDateTime("Pakistan Standard Time")).ToString("dd-MM-yyyy-HH-mm")}.bak";
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = connection.CreateCommand();
                    command.Transaction = transaction;

                    //var oldPath = Path.Combine(rootPath, "Client", "DatabaseBackups", "Test.bak");
                    //var backupFilePath = Path.Combine(rootPath, "Client", "DatabaseBackups", name);
                    //File.Copy(oldPath, backupFilePath, overwrite: true);
                    try
                    {
                        var query = $"BACKUP DATABASE {connection.Database} TO DISK = '{Path.Combine(rootPath, "Client", "DatabaseBackups", name)}' WITH FORMAT, MEDIANAME = 'DBBackup', NAME = '{connection.Database}-Full Database Backup';";
                        SqlHelper.ExecuteNonQuery(new SQLService().getconnstring(), CommandType.Text, query);
                        var userResponse = _loginService.GetUserByUserID(Convert.ToInt32(common.UserID));
                        if (userResponse.msgType == 1)
                        {
                            User user = (User)userResponse.data;
                            _commonRepository.CreateZipFromSpecificFile($"{Path.Combine(rootPath, "Client", "DatabaseBackups", name)}", $"{Path.Combine(rootPath, "Client", "TempDatabaseBackupZips", name.Replace(".bak", ".zip"))}", user.UPASS);
                            //response.data = $"/Client/TempDatabaseBackupZips/{name.Replace(".bak", ".zip")}";
                            response.data = name.Replace(".bak", ".zip");
                            response.data2 = user.EMAIL;
                            response.msg = "Backup sent successfully!";
                            response.msgType = 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        //transaction.Rollback();
                        string _catchMessage = ex.Message;
                        if (ex.InnerException != null)
                        {
                            _catchMessage += "<br/>" + ex.InnerException.Message;
                        }
                        response.msg = _catchMessage;
                        response.msgType = 2;
                    }
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msgType = 2;
                response.msg = _catchMessage;
            }
            return response;
        }
    }
}