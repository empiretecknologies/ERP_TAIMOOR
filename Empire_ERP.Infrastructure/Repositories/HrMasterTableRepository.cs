using Azure;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class HrMasterTableRepository : IHrMasterTableRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public HrMasterTableRepository(IMenuRepository menuRepository)
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

                if (!string.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"
                SELECT 
                    t.GROUP_CODE, t.GMENU_ID, t.GROUP_NAME, t.ADD_USER_ID,
                    t.ADD_DATE, t.ADD_COMPUTER_NAME, t.ADD_IP_ADDRESS, t.EDIT_USER_ID,
                    t.EDIT_DATE, t.EDIT_COMPUTER_NAME, t.EDIT_IP_ADDRESS, t.ADD_POSTALCODE,
                    t.EDIT_POSTALCODE,
                    CASE WHEN t.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS,
                    m.MENU_NAME
                FROM {table} t
                LEFT JOIN [db_aa30fd_empireerp].[dbo].[TBL_MENU_BUILDER] m ON t.GMENU_ID = m.ID
                WHERE t.MENU_ID = @MenuId AND t.DLT = 'T'
                ORDER BY t.GROUP_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@MenuId", common.MenuID);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var row = new SetupSubType
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : (DateTime?)reader["ADD_DATE"],
                                ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : (DateTime?)reader["EDIT_DATE"],
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                MENU_NAME = Convert.ToString(reader["MENU_NAME"]) // Ensure this property exists in SetupSubType
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

            return response; // ye return statement zaroori hai
        }

        public MyHttpResponseMessage Save(HrMasterTable modelRecord, Common common) 
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

                if (!string.IsNullOrWhiteSpace(table))
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
                            string duplicationQuery = "";

                            if (modelRecord.GROUP_CODE == 0) // INSERT
                            {
                                query = @"INSERT INTO " + table + @" 
                                    (GROUP_CODE, GROUP_NAME, TABLE_NAME, GMENU_ID, ADD_USER_ID, ADD_DATE, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME,
                                    EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, ADD_COMPUTER_NAME, MENU_ID, DLT) 
                                    VALUES 
                                    (@GroupCode, @GroupName, @TableName, @GMenuId, @AddUserId, @AddDate, @AddIpAddress, @EditUserId, @EditDate, @EditComputerName,
                                    @EditIpAddress, @AddPostalCode, @EditPostalCode, @AStatus, @AddComputerName, @MenuId, 'T')";

                                command.CommandText = query;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@GroupCode", GenerateNextId(common));
                                command.Parameters.AddWithValue("@GroupName", modelRecord.GROUP_NAME);
                                command.Parameters.AddWithValue("@TableName", table);
                                command.Parameters.AddWithValue("@GMenuId", modelRecord.MENU_ID);
                                command.Parameters.AddWithValue("@AddUserId", userid);
                                command.Parameters.AddWithValue("@AddDate", CommonService.GetDateTime("Pakistan Standard Time"));
                                command.Parameters.AddWithValue("@AddIpAddress", Ip);
                                command.Parameters.AddWithValue("@EditUserId", userid);
                                command.Parameters.AddWithValue("@EditDate", CommonService.GetDateTime("Pakistan Standard Time"));
                                command.Parameters.AddWithValue("@EditComputerName", Computer);
                                command.Parameters.AddWithValue("@EditIpAddress", Ip);
                                command.Parameters.AddWithValue("@AddPostalCode", Postal);
                                command.Parameters.AddWithValue("@EditPostalCode", Postal);
                                command.Parameters.AddWithValue("@AStatus", modelRecord.ASTATUS);
                                command.Parameters.AddWithValue("@AddComputerName", Computer);
                                command.Parameters.AddWithValue("@MenuId", common.MenuID);

                                command.ExecuteNonQuery();

                                // Check duplication (should be exactly 1 if just inserted)
                                duplicationQuery = "SELECT COUNT(*) FROM " + table + " WHERE GROUP_NAME = @GroupName AND MENU_ID = @MenuId AND DLT = 'T'";
                                command.CommandText = duplicationQuery;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@GroupName", modelRecord.GROUP_NAME);
                                command.Parameters.AddWithValue("@MenuId", common.MenuID);

                                int count = (int)command.ExecuteScalar();

                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.msg = "Record Added Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msg = "Name Already Exist !....";
                                    response.msgType = 2;
                                }
                            }
                            else // UPDATE
                            {
                                query = @"UPDATE " + table + @" SET 
                                    GROUP_NAME = @GroupName,
                                    EDIT_USER_ID = @EditUserId,
                                    GMENU_ID = @GMenu_Id,
                                    EDIT_DATE = @EditDate,
                                    EDIT_COMPUTER_NAME = @EditComputerName,
                                    EDIT_IP_ADDRESS = @EditIpAddress,
                                    EDIT_POSTALCODE = @EditPostalCode,
                                    ASTATUS = @AStatus
                                  WHERE GROUP_CODE = @GroupCode AND MENU_ID = @MenuId";

                                command.CommandText = query;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@GroupName", modelRecord.GROUP_NAME);
                                command.Parameters.AddWithValue("@EditUserId", userid);
                                command.Parameters.AddWithValue("@GMenu_Id", modelRecord.MENU_ID);
                                command.Parameters.AddWithValue("@EditDate", CommonService.GetDateTime("Pakistan Standard Time"));
                                command.Parameters.AddWithValue("@EditComputerName", Computer);
                                command.Parameters.AddWithValue("@EditIpAddress", Ip);
                                command.Parameters.AddWithValue("@EditPostalCode", Postal);
                                command.Parameters.AddWithValue("@AStatus", modelRecord.ASTATUS);
                                command.Parameters.AddWithValue("@GroupCode", modelRecord.GROUP_CODE);
                                command.Parameters.AddWithValue("@MenuId", common.MenuID);

                                command.ExecuteNonQuery();

                                // Check duplication for update
                                duplicationQuery = "SELECT COUNT(*) FROM " + table + " WHERE GROUP_NAME = @GroupName AND MENU_ID = @MenuId AND DLT = 'T'";
                                command.CommandText = duplicationQuery;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@GroupName", modelRecord.GROUP_NAME);
                                command.Parameters.AddWithValue("@MenuId", common.MenuID);

                                int count = (int)command.ExecuteScalar();

                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.msg = "Record Updated Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msg = "Name Already Exist !....";
                                    response.msgType = 2;
                                }
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
                    string maxIdQuery = "SELECT ISNULL(MAX(GROUP_CODE), 0) + 1 FROM " + table;
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

        public MyHttpResponseMessage GetHrTableById(int id, Common common)
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

                if (!string.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"
                    SELECT 
                        t.GROUP_CODE,
                        t.GROUP_NAME,
                        t.ASTATUS,
                        m.MENU_NAME,
                        t.GMENU_ID
                    FROM {table} t
                    LEFT JOIN [db_aa30fd_empireerp].[dbo].[TBL_MENU_BUILDER] m 
                        ON t.GMENU_ID = m.ID
                    WHERE t.MENU_ID = @MenuId AND t.DLT = 'T' AND t.GROUP_CODE = @GroupCode";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@MenuId", common.MenuID);
                        command.Parameters.AddWithValue("@GroupCode", id);

                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var setupSubType = new SetupSubType
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                MENU_NAME = Convert.ToString(reader["MENU_NAME"]) ,// Required value
                                MENU_ID = Convert.ToInt32(reader["GMENU_ID"]) // Required value
                            };

                            response.msg = "";
                            response.msgType = 1;
                            response.data = setupSubType;
                        }
                        reader.Close();
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE GROUP_CODE = '" + id + "'";
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