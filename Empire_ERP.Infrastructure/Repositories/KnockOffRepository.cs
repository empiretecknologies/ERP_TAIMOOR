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
using ZXing.QrCode.Internal;
using static Empire_ERP.Core.Entities.KnockOff;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class KnockOffRepository : IKnockOffRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        private string table = "TBL_KNOCKOFF";
        public KnockOffRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetAllSaleInvoices(KnockOff model, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? pickTableMaster = string.Empty;
                string? pickTableDetail = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    pickTableMaster = menu.PICK_TABLE_MASTER;
                    pickTableDetail = menu.PICK_TABLE_DETAIL;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {

                        string query = $@"EXEC KNOCKOFF_PICK  'PICK_DATA','{common.Branch}','{common.Period}','{model.PARTY_CODE}','{model.ACT_CODE}','',''";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {

                            var row = new
                            {
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                PICK_ID = Convert.ToString(reader["TRAN_ID"]),
                                QTY = Convert.ToInt32(reader["QTY"]),
                                AMOUNT = reader["NET_AMT"] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(reader["NET_AMT"]), 2),
                                PMENU_ID = Convert.ToString(reader["MENU_ID"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
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

        public MyHttpResponseMessage GetAllKnockOff(KnockOff model, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? pickTableMaster = string.Empty;
                string? pickTableDetail = string.Empty;
                int? pmenuId = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    pickTableMaster = menu.PICK_TABLE_MASTER;
                    pickTableDetail = menu.PICK_TABLE_DETAIL;
                    pmenuId = menu.ID;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {

                        //string qudery = $@"SELECT K.TRAN_ID,K.PICK_ID,M.V_DATE, M.VOUCHER_NO, M.REMARKS, K.AMOUNT AS KO_AMT,
                        //                    SUM(ISNULL(D.BAL_QTY,0)) as QTY, SUM(ISNULL(D.AMT,0)) as PICK_AMT , K.ASTATUS,
                        //                    MB.ID AS MENU_ID, MB.MENU_PAGE, MB.MENU_PARENT_CODE
                        //                    FROM {table} K
                        //                    LEFT OUTER JOIN {pickTableMaster} M ON K.PICK_ID = M.TRAN_ID 
                        //                    LEFT OUTER JOIN {pickTableDetail} D ON D.TRAN_ID = M.TRAN_ID 
                        //                    LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = K.PMENU_ID
                        //                    WHERE K.K_ID = {model.DT_CODE} AND K.KMENU_ID = {common.MenuID} AND K.ASTATUS = 'Y' AND K.DLT = 'T' 
                        //                    group by K.TRAN_ID,K.PICK_ID,M.V_DATE, M.VOUCHER_NO, M.REMARKS, K.AMOUNT, K.ASTATUS, MB.ID , MB.MENU_PAGE, MB.MENU_PARENT_CODE";

                        string query = $@"EXEC KNOCKOFF_PICK  'KNOCKOFF',{common.Branch},'{common.Period}','{model.PARTY_CODE}','{model.ACT_CODE}','{pmenuId}','{model.DT_CODE}'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {

                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["KNOCKOFF_TRAN_ID"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                QTY = Convert.ToInt32(reader["QTY"]),
                                PICK_AMT = Convert.ToDecimal(reader["NET_AMT"]),
                                KO_AMT = reader["RECV_AMT"] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(reader["RECV_AMT"]), 2),
                                INVOICE_VALUE = Convert.ToDecimal(reader["INVOICE_VALUE"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                PICK_ID = Convert.ToString(reader["TRAN_ID"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                WHT_AMT = reader["WHT_AMT"] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(reader["WHT_AMT"]), 2),
                                WHT_NET_AMT = reader["WHT_NET_AMT"] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(reader["WHT_NET_AMT"]), 2),
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

        public MyHttpResponseMessage Save(CustomKnockOff modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {

                if (!String.IsNullOrWhiteSpace(table))
                {
                    string Ip = common.IPAddress;
                    string Computer = common.ComputerName;
                    string Postal = common.PostalCode;
                    string userid = common.Username;
                    string bcode = common.Branch;
                    string period = common.Period;

                    string connectionString = new SQLService().getconnstring();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;

                        try
                        {
                            foreach (var item in modelRecord.Data.ToList())
                            {
                                var koAmt = Math.Round(Convert.ToDecimal(item.KO_AMT), 2, MidpointRounding.AwayFromZero);
                                var whtAmt = Math.Round(Convert.ToDecimal(item.WHT_AMT), 2, MidpointRounding.AwayFromZero);
                                var netAmt = Math.Round(Convert.ToDecimal(item.WHT_NET_AMT), 2, MidpointRounding.AwayFromZero);
                                string query = "";

                                if (item.TRAN_ID == null || item.TRAN_ID == 0)
                                {
                                    query = $@"INSERT INTO {table} (TRAN_ID, PICK_ID, PMENU_ID, K_ID, KMENU_ID, AMOUNT, WHT, WHT_AMT, WHT_NET_AMT, BCODE, PERIOD_ID, 
                                               ADD_USER_ID, EDIT_USER_ID, ADD_DATE, EDIT_DATE, ADD_COMPUTER_NAME, EDIT_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, DLT) VALUES
                                               ('{GenerateNextId(common, command)}', '{item.PICK_ID}', '{item.PMENU_ID}', '{modelRecord.K_ID}', '{common.MenuID}', '{koAmt}', '{item.WHT}', '{whtAmt}', '{netAmt}', '{bcode}', '{period}',
                                               '{userid}','{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Computer}', '{Ip}', '{Ip}', '{Postal}', '{Postal}', '{item.ASTATUS}', 'T')";
                                }
                                else
                                {
                                    query = $"UPDATE {table} " +
                                                  $"SET KMENU_ID = '{common.MenuID}', " +
                                                  $"AMOUNT = '{koAmt}', " +
                                                  $"WHT = '{item.WHT}', " +
                                                  $"WHT_AMT = '{whtAmt}', " +
                                                  $"WHT_NET_AMT = '{netAmt}', " +
                                                  $"EDIT_USER_ID = '{userid}', " +
                                                  $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                  $"EDIT_COMPUTER_NAME = '{Computer}', " +
                                                  $"EDIT_IP_ADDRESS = '{Ip}', " +
                                                  $"EDIT_POSTALCODE = '{Postal}', " +
                                                  $"ASTATUS = '{item.ASTATUS}', " +
                                                  $"DLT = 'T' " +
                                                  $"WHERE TRAN_ID = '{item.TRAN_ID}' AND BCODE = '{bcode}' AND PERIOD_ID = '{period}'";
                                }

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }

                            transaction.Commit();

                            response.msg = "Record saved successfully.";
                            response.msgType = 1;
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();

                            string _catchMessage = ex.Message;
                            if (ex.InnerException != null)
                                _catchMessage += "<br/>" + ex.InnerException.Message;

                            response.msg = _catchMessage;
                            response.msgType = 2;
                        }
                    }
                }
                else
                {
                    response.data = "";
                    response.msg = "Something went wrong! Please try again later.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                    _catchMessage += "<br/>" + ex.InnerException.Message;

                response.msg = _catchMessage;
                response.msgType = 2;
            }

            return response;
        }

        //public string GenerateNextId(Common common)
        //{
        //    try
        //    {
        //        //var Menu = _menuRepository.GetMenu(common.MenuID);
        //        //string? table = string.Empty;
        //        //if (Menu.data != null)
        //        //{
        //        //    var menu = (Menu)Menu.data;
        //        //    table = menu.TABLE1;
        //        //}

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            string maxIdQuery = "SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM " + table;
        //            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //            {
        //                SqlCommand command = new SqlCommand(maxIdQuery, connection);
        //                connection.Open();
        //                object result = command.ExecuteScalar();
        //                int nextId = Convert.ToInt32(result);
        //                return Convert.ToString(nextId);
        //            }
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return string.Empty;
        //    }
        //}

        public int GenerateNextId(Common common, SqlCommand command)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(table))
                {
                    string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM {table}";
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE TRAN_ID = '" + id + "'";
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