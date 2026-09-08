using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode.Internal;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class ApprovalRepository : IApprovalRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }

        public ApprovalRepository(IMenuRepository menuRepository, IBranchRepository branchRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
        }

        public MyHttpResponseMessage GetApprovals(int Branch, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table1 = string.Empty;
                string? table2 = string.Empty;
                string? pickMaster = string.Empty;
                string? pickDetail = string.Empty;
                string? menuPage = string.Empty;
                var menu = (Menu)Menu.data;
                table1 = menu.TABLE1;
                table2 = menu.TABLE2;
                pickMaster = menu.PICK_TABLE_MASTER;
                pickDetail = menu.PICK_TABLE_DETAIL;
                menuPage = menu.MENU_PAGE;
                if (!String.IsNullOrWhiteSpace(menuPage))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"EXEC [APPROVAL] {common.Branch},{common.Period}";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                                VOUCHER_DATE = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                BOOK_TYPE = Convert.ToString(reader["BOOK_TYPE"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                ACT_NAME = Convert.ToString(reader["ACT_NAME"]),
                                AMT = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                BCODE = Convert.ToString(reader["BCODE"]),
                                PERIOD_ID = Convert.ToString(reader["PERIOD_ID"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                TABLER = Convert.ToString(reader["TABLE1"]),
                                MENU_ID = Convert.ToString(reader["MENU_ID"]),

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
        public MyHttpResponseMessage GetApprovalSetup(int Branch, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table1 = string.Empty;
                string? table2 = string.Empty;
                string? pickMaster = string.Empty;
                string? pickDetail = string.Empty;
                string? menuPage = string.Empty;
                var menu = (Menu)Menu.data;
                table1 = menu.TABLE1;
                table2 = menu.TABLE2;
                pickMaster = menu.PICK_TABLE_MASTER;
                pickDetail = menu.PICK_TABLE_DETAIL;
                menuPage = menu.MENU_PAGE;
                if (!String.IsNullOrWhiteSpace(menuPage))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"EXEC [APPROVAL_SETUP]";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var row = new
                            {
                                GROUP_CODE = reader["GROUP_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GROUP_CODE"]),
                                //VOUCHER_DATE = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                MENU_ID = Convert.ToString(reader["MENU_ID"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                MENU_NAME = Convert.ToString(reader["MENU_NAME"]),
                                MENU_PAGE = Convert.ToString(reader["MENU_PAGE"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                MENU_P_CODE = Convert.ToString(reader["MENU_PARENT_CODE"]),
                                TABLER = Convert.ToString(reader["TABLE1"]),

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
        public MyHttpResponseMessage Save(List<Approval> modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, prefix = string.Empty, query = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    prefix = menu.PERFIX;
                }

                var Ip = common.IPAddress;
                var Computer = common.ComputerName;
                var Postal = common.PostalCode;
                var username = common.Username;
                string connectionString = new SQLService().getconnstring();

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;
                        try
                        {
                        foreach (var item in modelRecord)
                        {
                            query = $@"UPDATE {item.TABLER} SET ASTATUS = 'Y' WHERE MENU_ID = {item.MENU_ID} AND PERIOD_ID = {item.PERIOD_ID} 
                                      AND BCODE = {item.BCODE} AND TRAN_ID = {item.TRAN_ID}";

                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }


                            command.CommandText = query;
                            command.ExecuteNonQuery();

                            transaction.Commit();
                            response.msgType = 1;
                            response.msg ="Record Updated Successfully";

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
        public MyHttpResponseMessage SaveSetup(List<Approval> modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, prefix = string.Empty, query = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    prefix = menu.PERFIX;
                }

                var Ip = common.IPAddress;
                var Computer = common.ComputerName;
                var Postal = common.PostalCode;
                var username = common.Username;
                string connectionString = new SQLService().getconnstring();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = connection.CreateCommand();
                    command.Transaction = transaction;
                    try
                    {
                        foreach (var item in modelRecord)
                        {
                            query = $@"UPDATE {item.TABLER} SET ASTATUS = 'Y' WHERE MENU_ID = {item.MENU_ID} AND GROUP_CODE = {item.GROUP_CODE}";

                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }


                        command.CommandText = query;
                        command.ExecuteNonQuery();

                        transaction.Commit();
                        response.msgType = 1;
                        response.msg = "Record Updated Successfully";

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
                    string maxIdQuery = "SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM " + table;
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
    }
}