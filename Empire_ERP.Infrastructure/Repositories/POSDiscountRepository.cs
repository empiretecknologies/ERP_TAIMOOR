using Azure;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class POSDiscountRepository : IPOSDiscountRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public POSDiscountRepository(IMenuRepository menuRepository)
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
                        string query = @$"SELECT CODE, DESCR, BR.B_NAME, FDATE, TDATE, DISC, CASE WHEN DISC_EXP = '1' THEN 'Active' ELSE 'In-Active' END AS DISC_EXP
                                        , A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS
                                        , A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE 
                                        , A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS
                                          FROM " + table + " A " +
                                        " LEFT OUTER JOIN TBL_BRANCH BR" +
                                        " ON BR.BCODE = A.BCODE" +
                                        " WHERE A.DLT = 'T'" +
                                        " --AND BR.DLT = 'T' AND BR.ASTATUS = 'Y'" +
                                        " --AND IM.DLT = 'T' AND IM.ASTATUS = 'Y'" +
                                        " --AND IGR.DLT = 'T' AND IGR.ASTATUS = 'Y'\n" +
                                        " ORDER BY A.CODE DESC OPTION(FAST 50)";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = Convert.ToString(reader["CODE"]),
                                DESCR = Convert.ToString(reader["DESCR"]),
                                B_NAME = Convert.ToString(reader["B_NAME"]),
                                FDATE = Convert.ToString(reader["FDATE"]),
                                TDATE = Convert.ToString(reader["TDATE"]),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_EXP = Convert.ToString(reader["DISC_EXP"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                ADD_DATE = Convert.ToString(reader["ADD_DATE"]),
                                ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = Convert.ToString(reader["EDIT_DATE"]),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
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

        public string GenerateNextId(Common common, SqlCommand command)
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
                    string query = "SELECT ISNULL(MAX(CODE), 0) + 1 FROM " + table + "";
                    command.CommandText = query;
                    object result = command.ExecuteScalar();
                    int nextId = Convert.ToInt32(result);
                    return Convert.ToString(nextId);
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

        public MyHttpResponseMessage Save(POSDiscount modelRecord, Common common)
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
                    var menuID = common.MenuID;
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
                            if (modelRecord.CODE == null || modelRecord.CODE == 0)
                            {
                                char[] separators = { ',' };
                                var branches = modelRecord.SELECTEDBRANCHES.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                //string?[] items = null;
                                bool isSuccess = true, isItem = false;
                                //if (!String.IsNullOrWhiteSpace(modelRecord.SELECTEDITEMS))
                                //{
                                //    items = modelRecord.SELECTEDITEMS.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                //    isItem = true;
                                //}
                                //else
                                //{
                                //    items = modelRecord.SELECTEDITEMGROUPS.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                //}
                                
                                string color = string.Empty, size = string.Empty;
                                foreach (var branch in branches)
                                {
                                    try
                                    {
                                        var code = GenerateNextId(common, command);

                                        query = $@"INSERT INTO {table} (
                                                       [CODE], [DESCR], [BCODE], [FDATE], [TDATE], 
                                                       [DISC], [DISC_EXP], 
                                                       [ADD_USER_ID], [ADD_DATE], [ADD_COMPUTER_NAME], 
                                                       [ADD_IP_ADDRESS], [EDIT_USER_ID], [EDIT_DATE], 
                                                       [EDIT_COMPUTER_NAME], [EDIT_IP_ADDRESS], [ADD_POSTALCODE], 
                                                       [EDIT_POSTALCODE], [MENU_ID], [ASTATUS], [DLT])
                                                       VALUES (
                                                           '{code}', '{modelRecord.DESCR}', '{branch}', '{modelRecord.FDATE}', '{modelRecord.TDATE}', 
                                                           '{modelRecord.DISC}', '{modelRecord.DISC_EXP}',
                                                           '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', 
                                                           '{Ip}', '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', 
                                                           '{Computer}', '{Ip}', '{Postal}', 
                                                           '{Postal}', {menuID}, '{modelRecord.ASTATUS}', 'T'
                                                       );";
                                        command.CommandText = query;
                                        command.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        isSuccess = false;
                                        response.msg = $"{ex.Message} !....";
                                    }
                                }

                                if (isSuccess)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.msg = "Record Added Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msgType = 2;
                                }
                            }
                            else
                            {
                                query = $@"UPDATE {table}
                                           SET 
                                            [DESCR] = '{modelRecord.DESCR}', 
                                            --[BCODE] = '{modelRecord.BCODE}', 
                                            [FDATE] = '{modelRecord.FDATE}', 
                                            [TDATE] = '{modelRecord.TDATE}', 
                                            [DISC] = '{modelRecord.DISC}', 
                                            [DISC_EXP] = '{modelRecord.DISC_EXP}',
                                            [EDIT_USER_ID] = '{userid}', 
                                            [EDIT_DATE] = '{CommonService.GetDateTime("Pakistan Standard Time")}', 
                                            [EDIT_COMPUTER_NAME] = '{Computer}', 
                                            [EDIT_IP_ADDRESS] = '{Ip}', 
                                            [EDIT_POSTALCODE] = '{Postal}', 
                                            [ASTATUS] = '{modelRecord.ASTATUS}'
                                           WHERE [CODE] = '{modelRecord.CODE}';";

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

        public MyHttpResponseMessage GetPOSDiscountByCode(int code, Common common)
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
                        string query = @$"SELECT CODE, DESCR, BCODE, FDATE, TDATE, DISC, DISC_EXP, ASTATUS
                                          FROM {table} WHERE DLT = 'T' AND CODE = '{code}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                CODE = Convert.ToInt32(reader["CODE"]),
                                DESCR = Convert.ToString(reader["DESCR"]),
                                BCODE = Convert.ToInt32(reader["BCODE"]),
                                FDATE = reader["FDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["FDATE"]).ToString("yyyy-MM-dd"),
                                TDATE = reader["TDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["TDATE"]).ToString("yyyy-MM-dd"),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_EXP = Convert.ToString(reader["DISC_EXP"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"])
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

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
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
                    if (code == 0)
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
                            string query = $"UPDATE {table} SET DLT = 'F' WHERE CODE = '{code}'";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            response.msgType = 1;
                            response.msg = "Record Deleted Successfully";
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