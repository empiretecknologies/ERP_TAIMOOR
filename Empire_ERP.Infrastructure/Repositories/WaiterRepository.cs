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
    public class WaiterRepository : IWaiterRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public WaiterRepository(IMenuRepository menuRepository)
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
                        string query = "SELECT GROUP_CODE,GROUP_NAME,GPIC,CASE WHEN T.ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS , B.B_NAME AS bcode " +
                                       "FROM " + table + " T " +
                                       "LEFT JOIN TBL_BRANCH B ON T.BCODE = B.BCODE " +
                                       "WHERE T.MENU_ID = '" + common.MenuID + "' AND T.DLT = 'T' ORDER BY GROUP_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                BCODE = Convert.ToString(reader["BCODE"]),
                                GPIC = Convert.ToString(reader["GPIC"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"])
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

        public MyHttpResponseMessage Save(Waiter modelRecord, Common common)
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
                            if (modelRecord.GROUP_CODE == 0)
                            {
                                query = "INSERT INTO " + table + " " +
                                            "(GROUP_CODE,GROUP_NAME,GPIC,ADD_USER_ID,ADD_DATE," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                            "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                            "ADD_COMPUTER_NAME,MENU_ID,DLT,BCODE)" +
                                            "VALUES" +
                                            "('" + GenerateNextId(common) + "','" + modelRecord.GROUP_NAME + "','" + modelRecord.GPIC + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                            "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                            "'" + Computer + "','" + common.MenuID + $"','T','{modelRecord.BCODE}')";
                                //SqlCommand command = new SqlCommand(query, connection);
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE GROUP_NAME = '" + modelRecord.GROUP_NAME + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
                                //SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
                                command.CommandText = Duplicationquery;
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
                            else
                            {
                                query = "UPDATE " + table + " SET GROUP_NAME = '" + modelRecord.GROUP_NAME + @"',
                                            EDIT_USER_ID = '" + userid + @"',
                                            GPIC = '" + modelRecord.GPIC + @"',
                                            EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                            EDIT_COMPUTER_NAME = '" + Computer + @"',
                                            EDIT_IP_ADDRESS = '" + Ip + @"',
                                            EDIT_POSTALCODE = '" + Postal + @"',
                                            ASTATUS = '" + modelRecord.ASTATUS + @"',
                                            BCODE = '" + modelRecord.BCODE + @"'
                                            WHERE GROUP_CODE = '" + modelRecord.GROUP_CODE + "' AND MENU_ID = '" + common.MenuID + "'";
                                //SqlCommand command = new SqlCommand(query, connection);
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE GROUP_NAME = '" + modelRecord.GROUP_NAME + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
                                //SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
                                command.CommandText = Duplicationquery;
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

        public MyHttpResponseMessage GetWaiterById(int id, Common common)
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
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT GROUP_CODE,GROUP_NAME,GPIC,ASTATUS , BCODE " +
                                       "FROM " + table + " " +
                                       "WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' AND GROUP_CODE = '" + id + "'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var Table = new Table
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                BCODE = Convert.ToInt32(reader["BCODE"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                GPIC = Convert.ToString(reader["GPIC"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"])
                            };

                            response.msg = "";
                            response.msgType = 1;
                            response.data = Table;
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