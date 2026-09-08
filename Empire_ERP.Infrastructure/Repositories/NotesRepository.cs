using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class NotesRepository : INotesRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public NotesRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage QuickSearch(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @"SELECT * FROM " + table + " WHERE DLT = 'T' ORDER BY GROUP_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = reader["GROUP_CODE"],
                                GROUP_NAME = reader["GROUP_NAME"].ToString(),
                                GPIC = reader["GPIC"].ToString(),
                                ADD_USER_ID = reader["ADD_USER_ID"].ToString(),
                                ADD_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"].ToString(),
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"].ToString(),
                                EDIT_USER_ID = reader["EDIT_USER_ID"].ToString(),
                                EDIT_DATE = reader["EDIT_DATE"].ToString(),
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"].ToString(),
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"].ToString(),
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"].ToString(),
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"].ToString()
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();

                        response.data = jsonDataResult;
                        response.msg = "";
                        response.msgType = 1;
                        return response;
                    }
                }
                else
                {
                    response.data = "";
                    response.msg = "Something went wrong! please try again later.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }

        public MyHttpResponseMessage Save(Notes modelRecord, Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    var Ip = common.IPAddress;
                    var Computer = common.ComputerName;
                    var Postal = common.PostalCode;
                    var userid = common.Username;
                    string connectionString = new SQLService().getconnstring();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;
                        try
                        {
                            string query = "";
                            string Duplicationquery = "";
                            if (modelRecord.CODE == 0)
                            {
                                query = "INSERT INTO " + table + " " +
                                        "([GROUP_CODE],[GROUP_NAME],[GPIC],[V_DATE],[ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME]" +
                                        ",[ADD_IP_ADDRESS],[ADD_POSTALCODE]" +
                                        ",[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME]" +
                                        ",[EDIT_IP_ADDRESS],[EDIT_POSTALCODE],[MENU_ID],[DLT])" +
                                        "VALUES" +
                                        "('" + GenerateNextId(common) + "','" + modelRecord.GROUP_NAME + "','" + modelRecord.GPIC + "','" + modelRecord.V_DATE + "'," +
                                        "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                        "'" + Ip + "','" + Postal + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + Postal + "','" + common.MenuID + "','T')";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                response.msgType = 1;
                                response.msg = "Record Added Successfully";
                            }
                            else
                            {
                                query = "UPDATE " + table + " SET [GROUP_NAME] = '" + modelRecord.GROUP_NAME + @"',
		                                                [GPIC] = '" + modelRecord.GPIC + @"',
		                                                [V_DATE] = '" + modelRecord.V_DATE + @"',
		                                                [EDIT_USER_ID] = '" + userid + @"',
		                                                [EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
		                                                [EDIT_COMPUTER_NAME] = '" + Computer + @"',
		                                                [EDIT_IP_ADDRESS] = '" + Ip + @"',
		                                                [MENU_ID] = '" + common.MenuID + @"',
		                                                [EDIT_POSTALCODE] = '" + Postal + @"',
		                                                [DLT] = 'T'
                                                    WHERE [GROUP_CODE] = '" + modelRecord.CODE + @"'";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                response.msgType = 1;
                                response.msg = "Record Updated Successfully";
                            }
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
                else
                {
                    response.data = "";
                    response.msg = "Something went wrong! please try again later.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }

        public string GenerateNextId(Core.Entities.Common common)
        {
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    string maxIdQuery = "SELECT ISNULL(MAX(GROUP_CODE), 0) + 1 FROM " + table + "";
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(maxIdQuery, connection);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        int nextId = Convert.ToInt32(result);
                        return Convert.ToString(nextId);
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public MyHttpResponseMessage GetNotesById(int id, Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                object json = null;
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT * FROM " + table + " WHERE GROUP_CODE = @Id";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new
                            {
                                GROUP_CODE = reader["GROUP_CODE"],
                                GROUP_NAME = reader["GROUP_NAME"],
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                GPIC = reader["GPIC"]
                            };
                            json = jsonDataResult;
                        }
                        reader.Close();
                    }

                    response.msg = "";
                    response.msgType = 1;
                    response.data = json;
                }
                else
                {
                    response.data = "";
                    response.msg = "Something went wrong! please try again later.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }

        public MyHttpResponseMessage Delete(int id, Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msg = "Data not found in our records";
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    if (id == 0)
                    {
                        response.msg = "ID is not in numeric format";
                        response.msgType = 2;
                    }
                    else
                    {
                        string connectionString = new SQLService().getconnstring();
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE GROUP_CODE = '" + id + @"'";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            if (query != null)
                            {
                                response.msg = "Record Deleted Successfully";
                                response.msgType = 1;
                            }
                        }
                    }
                }
                else
                {
                    response.data = "";
                    response.msg = "Something went wrong! please try again later.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }
    }
}