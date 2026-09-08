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
    public class EmpTransferEntryRepository : IEmpTransferEntryRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public EmpTransferEntryRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common, string TableName)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? query = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                }

                if (!String.IsNullOrWhiteSpace(TableName))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        if (TableName != "")
                        {
                            query = $"SELECT * FROM {table} WHERE GROUP_CODE = 2 AND ASTATUS = 'Y' AND DLT = 'T'";

                        }
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {

                            var row = new TransferEntryRecords
                            {

                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]) != 0 ? Convert.ToInt32(reader["GROUP_CODE"]) : 0,
                                Date = reader["V_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["V_DATE"]) : (DateTime?)null,
                                BRANCH_FROM = Convert.ToInt32(reader["BRANCH_FROM"]) != 0 ? Convert.ToInt32(reader["BRANCH_FROM"]) : 0,
                                BRANCH_TO = Convert.ToInt32(reader["BRANCH_TO"]) != 0 ? Convert.ToInt32(reader["BRANCH_TO"]) : 0,
                                REPORT_TO = Convert.ToString(reader["REPORT_TO"]) != "" ? Convert.ToString(reader["REPORT_TO"]) : "",
                                DOC = Convert.ToString(reader["DOC"]) != "" ? Convert.ToString(reader["DOC"]) : "",
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                EMP_ID = Convert.ToInt32(reader["EMP_ID"]) != 0 ? Convert.ToInt32(reader["EMP_ID"]) : 0,
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

        public MyHttpResponseMessage Save(EmpTransferEntry modelRecord, Common common)
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
                    var username = common.Username;
                    var branch = common.Branch;
                    var period = common.Period;
                    var menuID = common.MenuID;
                    bool IsInsert = false;
                    int nextCode = 0;
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
                            
                            foreach (var item in modelRecord.Master.ToList())
                            {
                                try
                                {
                                    if (item.GROUP_CODE == null || item.GROUP_CODE == 0)
                                    {
                                        nextCode = GenerateNextDetailId(common, command);
                                        query = $@"INSERT INTO {table} 
                                                            ([GROUP_CODE], [V_DATE],[BRANCH_FROM],[BRANCH_TO],[REPORT_TO],[ADD_USER_ID], [ADD_DATE], [ADD_COMPUTER_NAME], [ADD_IP_ADDRESS], [EDIT_USER_ID], 
                                                             [EDIT_DATE], [EDIT_COMPUTER_NAME], [EDIT_IP_ADDRESS], [ADD_POSTALCODE], [EDIT_POSTALCODE], [ASTATUS],
                                                             [MENU_ID], [DLT],[DOC],[EMP_ID])
                                                            VALUES ({nextCode}, '{item.Date}',  '{item.BRANCH_FROM}',  '{item.BRANCH_TO}',  '{item.REPORT_TO}', 
                                                                    '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', 
                                                                    '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', 
                                                                    '{Postal}', '{Postal}', 'Y','{menuID}', 'T', '{item.DOC}', '{item.EMP_ID}')";
                                        command.CommandText = query;
                                        command.ExecuteNonQuery();

                                        IsInsert = true;

                                    }
                                    else
                                    {
                                        query = $@"
                                                    UPDATE {table} SET 
                                                        [V_DATE] = '{item.Date}',
                                                        [BRANCH_FROM] = '{item.BRANCH_FROM}',
                                                        [BRANCH_TO] = '{item.BRANCH_TO}',
                                                        [REPORT_TO] = '{item.REPORT_TO}',
                                                        [EDIT_USER_ID] = '{username}',
                                                        [EDIT_DATE] = '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                        [EDIT_COMPUTER_NAME] = '{Computer}',
                                                        [EDIT_IP_ADDRESS] = '{Ip}',
                                                        [EDIT_POSTALCODE] = '{Postal}',
                                                        [MENU_ID] = '{menuID}',
                                                        [DOC] = '{item.DOC}'
                                                    WHERE 
                                                        [GROUP_CODE] = {item.GROUP_CODE}";

                                        command.CommandText = query;
                                        command.ExecuteNonQuery();

                                    }
                                }
                                catch (Exception ex)
                                {
                                    response.data = "";
                                    response.msg = "Something went wrong! please try again later.";
                                    response.msgType = 2;
                                    return response;

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

                        transaction.Commit();
                        response.msgType = 1;
                        response.data = nextCode;
                        response.msg = IsInsert ? "Record Added Sucessfully" : "Record Update Sucessfully";
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

        public MyHttpResponseMessage GetEmpTransferEntryRecord(int id, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string query = string.Empty;
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
                        query = @"SELECT [GROUP_CODE]
                                  ,[V_DATE]
                                  ,[BRANCH_FROM]
                                  ,[BRANCH_TO]
                                  ,[REPORT_TO]
                                  ,[ASTATUS]
                                  ,[EMP_ID] , [DOC]" +
                                      $"FROM {table} " +
                                      $"WHERE EMP_ID = {id} AND DLT = 'T' ORDER BY EMP_ID DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]) != 0 ? Convert.ToInt32(reader["GROUP_CODE"]) : 0,
                                DATE = reader["V_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd") : "", 
                                BRANCH_FROM = Convert.ToInt32(reader["BRANCH_FROM"]) != 0 ? Convert.ToInt32(reader["BRANCH_FROM"]) : 0,
                                BRANCH_TO = Convert.ToInt32(reader["BRANCH_TO"]) != 0 ? Convert.ToInt32(reader["BRANCH_TO"]) : 0,
                                REPORT_TO = Convert.ToString(reader["REPORT_TO"]) != "" ? Convert.ToString(reader["REPORT_TO"]) : "",
                                DOC = Convert.ToString(reader["DOC"]) != "" ? Convert.ToString(reader["DOC"]) : "",
                                EMP_ID = Convert.ToInt32(reader["EMP_ID"]) != 0 ? Convert.ToInt32(reader["EMP_ID"]) : 0,
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE TRAN_ID = '" + Convert.ToInt32(id) + "'";
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
        private int GenerateNextDetailId(Common common, SqlCommand command)
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(GROUP_CODE), 0) + 1 FROM {table}";
                    command.CommandText = maxIdQuery;
                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }
    }

}