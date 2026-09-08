using Azure;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class CostCenterRepository : ICostCenterRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        private string table = "TBL_CC_DETAIL";
        public CostCenterRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage QuickSearch(CostCenter model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                //var Menu = _menuRepository.GetMenu(common.MenuID);
                //string? table = string.Empty;
                //if (Menu.data != null)
                //{
                //    var menu = (Menu)Menu.data;
                //    table = menu.TABLE1;
                //}

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"SELECT A.GROUP_CODE,CC.DESCR CC_NAME,A.AMOUNT,A.DESCR,A.PTRAN_ID,A.PICK_ID,A.PMENU_ID,CASE WHEN A.ASTATUS='Y' THEN 'Active' ELSE 'In-Active' END AS STATUS 
                                        FROM TBL_CC_DETAIL A LEFT OUTER JOIN TBL_COST_CENTER CC ON CC.CODE = A.COST_CENTER_ID
                                        WHERE A.PTRAN_ID = '{model.PTRAN_ID}' AND A.PICK_ID = '{model.PICK_ID}' AND A.DLT = 'T' ORDER BY GROUP_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CostCenter
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                CC_NAME = Convert.ToString(reader["CC_NAME"]),
                                AMOUNT = Convert.ToDecimal(reader["AMOUNT"]),
                                DESCR = Convert.ToString(reader["DESCR"]),
                                PTRAN_ID = Convert.ToInt32(reader["PTRAN_ID"]),
                                PICK_ID = Convert.ToInt32(reader["PICK_ID"]),
                                PMENU_ID = Convert.ToInt32(reader["PMENU_ID"]),
                                ASTATUS = Convert.ToString(reader["STATUS"]),
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

        public MyHttpResponseMessage Save(CostCenter modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                //var Menu = _menuRepository.GetMenu(common.MenuID);
                //string? table = string.Empty;
                //if (Menu.data != null)
                //{
                //    var menu = (Menu)Menu.data;
                //    table = menu.TABLE1;
                //}

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
                            if (modelRecord.GROUP_CODE == 0)
                            {
                                //if (modelRecord.PTRAN_ID == 0 || modelRecord.PICK_ID == 0)
                                //{
                                //    response.data = "";
                                //    response.msg = "Something went wrong! please try again later.";
                                //    response.msgType = 2;
                                //}
                                query = "INSERT INTO " + table + " " +
                                            "(GROUP_CODE,COST_CENTER_ID,AMOUNT,DESCR,PTRAN_ID,PICK_ID,PMENU_ID,ADD_USER_ID,ADD_DATE," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                            "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                            "ADD_COMPUTER_NAME,DLT)" +
                                            "VALUES" +
                                            "('" + GenerateNextId(common) + "','" + modelRecord.COST_CENTER_ID + "','" + modelRecord.AMOUNT + "','" + modelRecord.DESCR + "','" + modelRecord.PTRAN_ID + "','" + modelRecord.PICK_ID + "'," +
                                            "'" + common.MenuID + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                            "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                            "'" + Computer + "','T')";
                                //SqlCommand command = new SqlCommand(query, connection);
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                transaction.Commit();
                                response.msgType = 1;
                                response.msg = "Record Added Successfully";

                            }
                            else
                            {
                                query = "UPDATE " + table + " SET COST_CENTER_ID = '" + modelRecord.COST_CENTER_ID + @"',
                                            AMOUNT = '" + modelRecord.AMOUNT + @"',
                                            DESCR = '" + modelRecord.DESCR + @"',
                                            PTRAN_ID = '" + modelRecord.PTRAN_ID + @"',
                                            PICK_ID = '" + modelRecord.PICK_ID + @"',
                                            EDIT_USER_ID = '" + userid + @"',
                                            EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                            EDIT_COMPUTER_NAME = '" + Computer + @"',
                                            EDIT_IP_ADDRESS = '" + Ip + @"',
                                            EDIT_POSTALCODE = '" + Postal + @"',
                                            ASTATUS = '" + modelRecord.ASTATUS + @"'
                                            WHERE GROUP_CODE = '" + modelRecord.GROUP_CODE + "'";
                                //SqlCommand command = new SqlCommand(query, connection);
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                transaction.Commit();
                                response.msgType = 1;
                                response.msg = "Record Updated Successfully";

                                //Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE GROUP_NAME = '" + modelRecord.GROUP_NAME + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
                                ////SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
                                //command.CommandText = Duplicationquery;
                                //int count = (int)command.ExecuteScalar();
                                //if (count == 1)
                                //{
                                //    transaction.Commit();
                                //    response.msgType = 1;
                                //    response.msg = "Record Updated Successfully";
                                //}
                                //else
                                //{
                                //    transaction.Rollback();
                                //    response.msg = "Name Already Exist !....";
                                //    response.msgType = 2;
                                //}
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
                //var Menu = _menuRepository.GetMenu(common.MenuID);
                //string? table = string.Empty;
                //if (Menu.data != null)
                //{
                //    var menu = (Menu)Menu.data;
                //    table = menu.TABLE1;
                //}

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

        public MyHttpResponseMessage GetCostCenterByID(int id, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                //var Menu = _menuRepository.GetMenu(common.MenuID);
                //string? table = string.Empty;
                //if (Menu.data != null)
                //{
                //    var menu = (Menu)Menu.data;
                //    table = menu.TABLE1;
                //}

                if (!String.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT GROUP_CODE,COST_CENTER_ID,AMOUNT,DESCR,PTRAN_ID,PICK_ID,PMENU_ID,ASTATUS " +
                                       "FROM " + table + " " +
                                       "WHERE GROUP_CODE = '" + id + "' AND DLT = 'T'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var costCenter = new CostCenter
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                COST_CENTER_ID = Convert.ToInt32(reader["COST_CENTER_ID"]),
                                AMOUNT = Convert.ToDecimal(reader["AMOUNT"]),
                                DESCR = Convert.ToString(reader["DESCR"]),
                                PTRAN_ID = Convert.ToInt32(reader["PTRAN_ID"]),
                                PICK_ID = Convert.ToInt32(reader["PICK_ID"]),
                                PMENU_ID = Convert.ToInt32(reader["PMENU_ID"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            };

                            response.msg = "";
                            response.msgType = 1;
                            response.data = costCenter;
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
                //var Menu = _menuRepository.GetMenu(common.MenuID);
                //string? table = string.Empty;
                //if (Menu.data != null)
                //{
                //    var menu = (Menu)Menu.data;
                //    table = menu.TABLE1;
                //}

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