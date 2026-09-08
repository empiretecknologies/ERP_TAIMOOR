using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public RoleRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
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
                        string query = @"SELECT MIN(ROLE_ID) AS ROLE_ID, ROLE_NAME, CASE WHEN MIN(ROLE_TYPE) = 'U' THEN 'USER' ELSE 'MANAGER' END AS ROLE_TYPE FROM TBL_ROLE WHERE DLT = 'T' AND ASTATUS = 'Y' GROUP BY ROLE_NAME ORDER BY ROLE_ID DESC;";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ROLE_ID = Convert.ToString(reader["ROLE_ID"]),
                                ROLE_NAME = Convert.ToString(reader["ROLE_NAME"]),
                                ROLE_TYPE = Convert.ToString(reader["ROLE_TYPE"])
                            };
                            jsonDataResult.Add(row);
                        }

                        reader.Close();
                    }

                    response.data = jsonDataResult;
                    response.msg = "";
                    response.msgType = 1;
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

        public MyHttpResponseMessage GetMainMenue(Common common)
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
                        string query = $"SELECT ID, MENU_NAME, MENU_GRCODE FROM TBL_MENU_BUILDER WHERE MENU_TYPE = 1 AND ASTATUS = 'Y' AND DLT = 'T' ORDER BY MENU_GRCODE";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                MENU_GRCODE = Convert.ToString(reader["MENU_GRCODE"]),
                                MENU_NAME = Convert.ToString(reader["MENU_NAME"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();

                        response.data = jsonDataResult;
                        response.msg = "";
                        response.msgType = 1;
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

        public MyHttpResponseMessage GetAllPermissions(Common common)
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
                        //string query = $"SELECT ITEM_CODE AS CODE, 0 AS ACT_CODE, '1' AS GRCODE, 0 AS PARENT_CODE, ITEM_NAME AS NAME, 'I' AS TYPE, 4 AS GROUP_CODE FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y' " +
                        //            $"UNION ALL " +
                        //            $"SELECT ID AS CODE, 0 AS ACT_CODE, CAST(MENU_GRCODE AS VARCHAR(255)) AS GRCODE, MENU_PARENT_CODE AS PARENT_CODE, MENU_NAME AS NAME, MTYPE AS TYPE, 1 AS GROUP_CODE FROM TBL_MENU_BUILDER WHERE ASTATUS = 'Y' AND DLT = 'T' " +
                        //            $"UNION ALL " +
                        //            $"SELECT R_ID AS CODE, 0 AS ACT_CODE, '1' AS GRCODE, 0 AS PARENT_CODE, REPORT_NAME AS NAME, 'R' AS TYPE, 2 AS GROUP_CODE FROM TBL_REPORT_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' " +
                        //            $"UNION ALL " +
                        //            $"SELECT PARTY_CODE AS CODE, ACT_CODE, '1' AS GRCODE, 0 AS PARENT_CODE, PARTY_NAME AS NAME, 'P' AS TYPE, 6 AS GROUP_CODE  FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' " +
                        //            $"UNION ALL SELECT ACT_CODE AS CODE, 0 AS ACT_CODE, '1' AS GRCODE, ACT_PARENT_CODE AS PARENT_CODE, ACT_NAME AS NAME, ACT_TYPE AS TYPE, 3 AS GROUP_CODE FROM TBL_CHART WHERE DLT = 'T'";

                        string query = $@" SELECT ITEM_CODE AS CODE, 0 AS ACT_CODE, '1' AS GRCODE, 0 AS PARENT_CODE, ITEM_NAME AS NAME, 'I' AS TYPE, 4 AS GROUP_CODE 
                                             FROM TBL_ITEMSMASTER 
                                             WHERE DLT = 'T' AND ASTATUS = 'Y' 
                                             UNION ALL 
                                             SELECT ID AS CODE, 0 AS ACT_CODE, CAST(MENU_GRCODE AS VARCHAR(255)) AS GRCODE, MENU_PARENT_CODE AS PARENT_CODE, MENU_NAME AS NAME, MTYPE AS TYPE, 1 AS GROUP_CODE 
                                             FROM TBL_MENU_BUILDER 
                                             WHERE ASTATUS = 'Y' AND DLT = 'T' 
                                             UNION ALL 
                                             SELECT R_ID AS CODE, 0 AS ACT_CODE, '1' AS GRCODE, 0 AS PARENT_CODE, REPORT_NAME AS NAME, 'R' AS TYPE, 2 AS GROUP_CODE 
                                             FROM TBL_REPORT_TYPES 
                                             WHERE DLT = 'T' AND ASTATUS = 'Y' 
                                             UNION ALL 
                                             SELECT PARTY_CODE AS CODE, ACT_CODE, '1' AS GRCODE, 0 AS PARENT_CODE, PARTY_NAME AS NAME, 'P' AS TYPE, 6 AS GROUP_CODE  
                                             FROM TBL_PARTY_TYPES 
                                             WHERE DLT = 'T' AND ASTATUS = 'Y' 
                                             UNION ALL 
                                             SELECT ACT_CODE AS CODE, 0 AS ACT_CODE, '1' AS GRCODE, ACT_PARENT_CODE AS PARENT_CODE, ACT_NAME AS NAME, ACT_TYPE AS TYPE, 3 AS GROUP_CODE 
                                             FROM TBL_CHART
                                             WHERE DLT = 'T'
                                             UNION ALL
                                             SELECT ID AS CODE, 0 AS ACT_CODE, CAST(MENU_GRCODE AS VARCHAR(255)) AS GRCODE, MENU_PARENT_CODE AS PARENT_CODE, MENU_NAME AS NAME, MTYPE AS TYPE, 5 AS GROUP_CODE 
                                             FROM TBL_MENU_BUILDER 
                                             WHERE ASTATUS = 'Y' AND DLT = 'T' ";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = Convert.ToInt32(reader["CODE"]),
                                ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                                MENU_NAME = Convert.ToString(reader["NAME"]),
                                MENU_PARENT_CODE = Convert.ToInt32(reader["PARENT_CODE"]),
                                GRCODE = Convert.ToString(reader["GRCODE"]),
                                MTYPE = Convert.ToString(reader["TYPE"]),
                                MODULE_ID = Convert.ToInt32(reader["GROUP_CODE"]),
                                ROWSELECTION = false,
                                ADD = false,
                                EDIT = false,
                                VIEW = false,
                                DELETE = false,
                                PRINT = false,
                                COPY = false
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();

                        response.data = jsonDataResult;
                        response.msg = "";
                        response.msgType = 1;
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

        public MyHttpResponseMessage Save(CustomRole modelRecord, Common common)
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
                    var roleName = modelRecord.ROLE_NAME;
                    var showSelected = modelRecord.SHOW_SELECTED;
                    var roleType = modelRecord.ROLE_TYPE;
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
                            if (modelRecord.ROLE_ID == null || modelRecord.ROLE_ID == 0)
                            {
                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE ROLE_NAME = '" + roleName + "' AND MENU_ID = '" + common.MenuID + "' AND R_BCODE = '" + common.Branch + "' AND DLT = 'T'";
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count == 0)
                                {
                                    var roleId = GenerateNextId(common);
                                    var dtCode = GenerateNextDtId(common, 0);
                                    StringBuilder sb = new StringBuilder();
                                    sb.Append("INSERT INTO " + table + " " +
                                              "(ROLE_ID, ACT_CODE, ROLE_NAME, ROLE_TYPE, SHOW_SELECTED, MODULE_ID, DT_CODE, " +
                                              "R_ADD, R_EDIT, R_DLT, R_PRINT, R_VIEW, R_COPY, R_BCODE, RMENU_ID, " +
                                              "ADD_USER_ID, ADD_DATE, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, " +
                                              "EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, " +
                                              "ADD_COMPUTER_NAME, MENU_ID, DLT) VALUES ");

                                    List<string> valuesList = new List<string>();
                                    foreach (var perm in modelRecord.PERMISSIONS)
                                    {
                                        var add = perm.ADD ? 1 : 0;
                                        var edit = perm.EDIT ? 1 : 0;
                                        var dlt = perm.DELETE ? 1 : 0;
                                        var view = perm.VIEW ? 1 : 0;
                                        var print = perm.PRINT ? 1 : 0;
                                        var copy = perm.COPY ? 1 : 0;

                                        foreach (var branch in modelRecord.BRANCH)
                                        {
                                            string values = $"('{roleId}','{perm.ACT_CODE}','{roleName}','{roleType}','{showSelected}',{perm.MODULE_ID},{dtCode}," +
                                                            $"{add},{edit},{dlt},{print},{view},{copy}," +
                                                            $"'{branch}','{perm.ID}','{userid}','{CommonService.GetDateTime("Pakistan Standard Time")}'," +
                                                            $"'{Ip}','{userid}','{CommonService.GetDateTime("Pakistan Standard Time")}','{Computer}'," +
                                                            $"'{Ip}','{Postal}','{Postal}','{modelRecord.ASTATUS}'," +
                                                            $"'{Computer}','{common.MenuID}','T')";
                                            valuesList.Add(values);

                                            dtCode = (Convert.ToInt32(dtCode) + 1).ToString();
                                        }
                                    }

                                    sb.Append(string.Join(",", valuesList));
                                    command.CommandText = sb.ToString();
                                    command.ExecuteNonQuery();

                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.data = roleId;
                                    response.msg = "Record Added Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msg = "Role name Already Exist !....";
                                    response.msgType = 2;
                                }
                            }
                            else
                            {
                                var roleId = modelRecord.ROLE_ID;
                                var dtCode = GenerateNextDtId(common, roleId);

                                // Delete existing records for this role ID
                                string deleteQuery = $"DELETE FROM {table} WHERE ROLE_ID = {roleId}";
                                command.CommandText = deleteQuery;
                                command.ExecuteScalar();

                                int batchSize = 1000;
                                int counter = 0;

                                StringBuilder sb = new StringBuilder();
                                sb.Append("INSERT INTO " + table + " " +
                                          "(ROLE_ID, ACT_CODE, ROLE_NAME, SHOW_SELECTED, ROLE_TYPE, MODULE_ID, DT_CODE, " +
                                          "R_ADD, R_EDIT, R_DLT, R_PRINT, R_VIEW, R_COPY, R_BCODE, RMENU_ID, " +
                                          "ADD_USER_ID, ADD_DATE, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, " +
                                          "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, " +
                                          "ASTATUS, ADD_COMPUTER_NAME, MENU_ID, DLT) VALUES ");

                                List<string> valuesList = new List<string>();

                                foreach (var perm in modelRecord.PERMISSIONS)
                                {
                                    var add = perm.ADD ? 1 : 0;
                                    var edit = perm.EDIT ? 1 : 0;
                                    var dlt = perm.DELETE ? 1 : 0;
                                    var view = perm.VIEW ? 1 : 0;
                                    var print = perm.PRINT ? 1 : 0;
                                    var copy = perm.COPY ? 1 : 0;

                                    foreach (var branch in modelRecord.BRANCH)
                                    {
                                        string values = $"('{roleId}', '{perm.ACT_CODE}', '{roleName}', '{showSelected}', '{roleType}', {perm.MODULE_ID}, {dtCode}, " +
                                                        $"{add}, {edit}, {dlt}, {print}, {view}, {copy}, " +
                                                        $"'{branch}', '{perm.ID}', '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                        $"'{Ip}', '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', " +
                                                        $"'{Ip}', '{Postal}', '{Postal}', '{modelRecord.ASTATUS}', " +
                                                        $"'{Computer}', '{common.MenuID}', 'T')";
                                        valuesList.Add(values);

                                        dtCode = (Convert.ToInt32(dtCode) + 1).ToString();
                                        counter++;

                                        // If we reach the batch size, execute the current batch
                                        if (counter % batchSize == 0)
                                        {
                                            sb.Append(string.Join(",", valuesList));
                                            command.CommandText = sb.ToString();
                                            command.ExecuteNonQuery();

                                            // Reset the StringBuilder and values list for the next batch
                                            sb.Clear();
                                            sb.Append("INSERT INTO " + table + " " +
                                                      "(ROLE_ID, ACT_CODE, ROLE_NAME, ROLE_TYPE, MODULE_ID, DT_CODE, " +
                                                      "R_ADD, R_EDIT, R_DLT, R_PRINT, R_VIEW, R_COPY, R_BCODE, RMENU_ID, " +
                                                      "ADD_USER_ID, ADD_DATE, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, " +
                                                      "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, " +
                                                      "ASTATUS, ADD_COMPUTER_NAME, MENU_ID, DLT) VALUES ");
                                            valuesList.Clear();
                                        }
                                    }
                                }

                                // Insert any remaining records if they are less than the batch size
                                if (valuesList.Count > 0)
                                {
                                    sb.Append(string.Join(",", valuesList));
                                    command.CommandText = sb.ToString();
                                    command.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                response.msgType = 1;
                                response.data = roleId;
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

        public string GenerateNextId(Common common)
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
                    string maxIdQuery = "SELECT ISNULL(MAX(ROLE_ID), 0) + 1 FROM " + table;
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

        public string GenerateNextDtId(Common common, int? roleId)
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
                    string maxIdQuery = "";
                    if (roleId > 0)
                    {
                        maxIdQuery = "SELECT ISNULL(MAX(DT_CODE), 0) + 1 FROM " + table + " WHERE ROLE_ID != " + roleId;
                    }
                    else
                    {
                        maxIdQuery = "SELECT ISNULL(MAX(DT_CODE), 0) + 1 FROM " + table;
                    }
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

        public MyHttpResponseMessage GetRoleById(int id, Common common)
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
                        string query = @"SELECT * FROM TBL_ROLE WHERE DLT = 'T' AND ROLE_ID = " + id;

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ROLE_ID = reader["ROLE_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ROLE_ID"]),
                                ROLE_NAME = reader["ROLE_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ROLE_NAME"]),
                                ROLE_TYPE = reader["ROLE_TYPE"] == DBNull.Value ? "" : Convert.ToString(reader["ROLE_TYPE"]),
                                MODULE_ID = reader["MODULE_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["MODULE_ID"]),
                                ADD = reader["R_ADD"] == DBNull.Value ? false : Convert.ToBoolean(reader["R_ADD"]),
                                EDIT = reader["R_EDIT"] == DBNull.Value ? false : Convert.ToBoolean(reader["R_EDIT"]),
                                VIEW = reader["R_VIEW"] == DBNull.Value ? false : Convert.ToBoolean(reader["R_VIEW"]),
                                DELETE = reader["R_DLT"] == DBNull.Value ? false : Convert.ToBoolean(reader["R_DLT"]),
                                PRINT = reader["R_PRINT"] == DBNull.Value ? false : Convert.ToBoolean(reader["R_PRINT"]),
                                COPY = reader["R_COPY"] == DBNull.Value ? false : Convert.ToBoolean(reader["R_COPY"]),
                                ROWSELECTION = Convert.ToBoolean(reader["R_ADD"]) && Convert.ToBoolean(reader["R_EDIT"]) && Convert.ToBoolean(reader["R_DLT"]) && Convert.ToBoolean(reader["R_VIEW"]) && Convert.ToBoolean(reader["R_PRINT"]) && Convert.ToBoolean(reader["R_COPY"]),
                                BRANCH = reader["R_BCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["R_BCODE"]),
                                ID = reader["RMENU_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RMENU_ID"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                SHOW_SELECTED = reader["SHOW_SELECTED"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SHOW_SELECTED"])
                            };
                            jsonDataResult.Add(row);
                        }

                        reader.Close();
                    }

                    response.data = jsonDataResult;
                    response.msg = "";
                    response.msgType = 1;
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

        public MyHttpResponseMessage Delete(int id, Common common)
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE ROLE_ID = '" + id + "'";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            response.msg = "Record Deleted Successfully";
                            response.msgType = 1;
                        }
                    }
                }
                else
                {
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