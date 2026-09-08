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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public GradeRepository(IMenuRepository menuRepository)
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
                        string query = $@"SELECT G.GROUP_CODE,G.GROUP_NAME,G.GPIC,I.ITEM_NAME,G.RATE,S.GROUP_NAME AS SIZE,G.EDIT_USER_ID,G.EDIT_DATE,G.EDIT_COMPUTER_NAME,
                                            G.EDIT_IP_ADDRESS,G.EDIT_POSTALCODE,CASE WHEN G.ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS 
                                            FROM {table} G
                                            LEFT OUTER JOIN TBL_SIZE S ON S.GROUP_CODE =  G.SIZE
                                            LEFT OUTER JOIN TBL_ITEMSMASTER I ON I.ITEM_CODE = G.ITEM_CODE
                                            WHERE G.MENU_ID = '{common.MenuID}' AND G.DLT = 'T' ORDER BY G.GROUP_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new Grade
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                ITEM_NAME = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                RATE = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["RATE"]),
                                GPIC = Convert.ToString(reader["GPIC"]),
                                SIZE_NAME = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                //ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                //ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]),
                                //ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                //ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                //ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
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

        public MyHttpResponseMessage Save(Grade modelRecord, Common common)
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
                                            "(GROUP_CODE,GROUP_NAME,GPIC,SIZE,ITEM_CODE,RATE,ADD_USER_ID,ADD_DATE," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                            "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                            "ADD_COMPUTER_NAME,MENU_ID,DLT)" +
                                            "VALUES" +
                                            "('" + GenerateNextId(common) + "','" + modelRecord.GROUP_NAME + "','" + modelRecord.GPIC + "','" + modelRecord.SIZE + "','" + modelRecord.ITEM_CODE + "','" + modelRecord.RATE + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                            "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                            "'" + Computer + "','" + common.MenuID + "','T')";
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
                                    response.msg = "Name Already Exist ....!";
                                    response.msgType = 2;
                                }
                            }
                            else
                            {
                                query = "UPDATE " + table + " SET GROUP_NAME = '" + modelRecord.GROUP_NAME + @"',
                                            EDIT_USER_ID = '" + userid + @"',
                                            ITEM_CODE = '" + modelRecord.ITEM_CODE + @"',
                                            GPIC = '" + modelRecord.GPIC + @"',
                                            SIZE = '" + modelRecord.SIZE + @"',
                                            RATE = '" + modelRecord.RATE + @"',
                                            EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                            EDIT_COMPUTER_NAME = '" + Computer + @"',
                                            EDIT_IP_ADDRESS = '" + Ip + @"',
                                            EDIT_POSTALCODE = '" + Postal + @"',
                                            ASTATUS = '" + modelRecord.ASTATUS + @"'
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

        public MyHttpResponseMessage GetGradeById(int id, Common common)
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
                        string query = "SELECT GROUP_CODE,GROUP_NAME,GPIC,SIZE,ITEM_CODE,RATE,ASTATUS " +
                                       "FROM " + table + " " +
                                       "WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' AND GROUP_CODE = '" + id + "'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var Grade = new Grade
                            {
                                GROUP_CODE = reader["GROUP_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GROUP_CODE"]),
                                SIZE = reader["SIZE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE"]),
                                GROUP_NAME = reader["GROUP_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["GROUP_NAME"]),
                                GPIC = reader["GPIC"] == DBNull.Value ? "" : Convert.ToString(reader["GPIC"]),
                                ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                RATE = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["RATE"]),
                                ASTATUS = reader["ASTATUS"] == DBNull.Value ? "" : Convert.ToString(reader["ASTATUS"])
                            };

                            response.msg = "";
                            response.msgType = 1;
                            response.data = Grade;
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

        public MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string table = menu.TABLE1;
                string table2 = menu.TABLE2;
                string connectionString = new SQLService().getconnstring();
                Grade grade = new Grade();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 2);
                    defaultInterpolatedStringHandler.AppendLiteral("SELECT * FROM ");
                    defaultInterpolatedStringHandler.AppendFormatted(table);
                    defaultInterpolatedStringHandler.AppendLiteral(" WHERE GROUP_CODE = ");
                    defaultInterpolatedStringHandler.AppendFormatted<int>(record.TRAN_ID);
                    defaultInterpolatedStringHandler.AppendLiteral(" AND DLT = 'T'");
                    SqlDataReader reader = new SqlCommand(defaultInterpolatedStringHandler.ToStringAndClear(), connection).ExecuteReader();
                    if (reader.Read())
                    {
                        grade = new Grade
                        {
                            GROUP_CODE = 0,
                            GROUP_NAME = record.ITEM_NAME,
                            GPIC = Convert.ToString(reader["GPIC"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            SIZE = new int?(Convert.ToInt32(reader["SIZE"])),
                            ITEM_CODE = new int?(Convert.ToInt32(reader["ITEM_CODE"])),
                            RATE = new double?(Convert.ToDouble(reader["RATE"]))
                        };
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(grade, common);
                if (response.msgType == 1)
                {
                    response.msg = "Record Copied Successfully";
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage = _catchMessage + "<br/>" + ex.InnerException.Message;
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