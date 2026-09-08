using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class StockTransferRequisitionRepository : IStockTransferRequisitionRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public StockTransferRequisitionRepository(IMenuRepository menuRepository, IBranchRepository branchRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
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
                        string query = "SELECT TRAN_ID,V_DATE,VOUCHER_NO," +
                                       "REF,REMARKS,BCODE,PERIOD_ID," +
                                       "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
                                       "EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                       "ADD_POSTALCODE,EDIT_POSTALCODE," +
                                       "CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {table} WHERE DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "' ORDER BY TRAN_ID DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = Convert.ToString(reader["TRAN_ID"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
                                ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
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

        public MyHttpResponseMessage GetStockTransferRequisitionByCode(int code, Common common)
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
                        string query = "SELECT TRAN_ID,V_DATE,VOUCHER_NO," +
                                       "REF,REMARKS,BCODE,TBCODE,PERIOD_ID,TPERIOD_ID,ASTATUS " +
                                       $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = Convert.ToString(reader["TRAN_ID"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                BCODE = Convert.ToInt32(reader["BCODE"]),
                                TBCODE = Convert.ToInt32(reader["TBCODE"]),
                                PERIOD_ID = Convert.ToInt32(reader["PERIOD_ID"]),
                                TPERIOD_ID = Convert.ToInt32(reader["TPERIOD_ID"]),
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

        public MyHttpResponseMessage GetStockTransferRequisitionDetailByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $"SELECT T.TRAN_ID,T.DT_CODE,T.CHK, " +
                            $" T.ITEM_CODE , T.ITEM_GROUP ," +
                            $" T.QTY,T.UNIT," +
                            $" T.QTY2,T.BAL_QTY,T.DT_DESC, " +
                            $" CASE WHEN IM.ITEM_ID IS NOT NULL AND IM.ITEM_ID <> '' THEN IM.ITEM_ID ELSE IB.ITEM_ID END AS ITEM_ID" +
                            $" FROM TBL_STKTR_DETAIL T " +
                            $" LEFT OUTER JOIN TBL_ITEMSMASTER IM ON IM.ITEM_CODE = T.ITEM_CODE" +
                            $" LEFT OUTER JOIN TBL_BARCODE B ON B.CODE = T.ITEM_CODE" +
                            $" LEFT OUTER JOIN TBL_ITEMSMASTER IB ON B.ITEM_CODE = IB.ITEM_CODE" +
                            $" LEFT OUTER JOIN TBL_BRANCH BR ON BR.BCODE = T.BCODE" +
                            $" WHERE T.DLT = 'T' AND T.BCODE = {common.Branch}  AND T.PERIOD_ID = {common.Period} AND T.TRAN_ID = {code} ORDER BY T.DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                ITEM_GROUP = Convert.ToInt32(reader["ITEM_GROUP"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToInt32(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                CHK = Convert.ToString(reader["CHK"]),
                                ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                                CHK1 = Convert.ToString(reader["CHK"]) == "1" ? true : false
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

        private int GenerateNextId(Common common, SqlCommand command)
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM {table} WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
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

        private string GenerateVoucherNo(Common common, int code, string vDate)
        {
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? prefix = string.Empty, shortName = string.Empty;
                int voucherLength = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    voucherLength = Convert.ToInt32(menu.VOUCHER_LEN);
                    prefix = menu.PERFIX;
                }

                var branchData = _branchRepository.GetBranchByCode(common.Branch);
                if (branchData.data != null)
                {
                    var branch = (Branch)branchData.data;
                    shortName = branch.B_SHORT_NAME;
                }

                if (!String.IsNullOrWhiteSpace(shortName) && !String.IsNullOrWhiteSpace(prefix) && voucherLength > 0 && code > 0)
                {
                    //string paddedVoucherValue = "0".ToString().PadLeft(voucherLength - 1, '0') + code;
                    string paddedVoucherValue = code.ToString().PadLeft(voucherLength, '0');
                    return $"{shortName}/{prefix}/{Convert.ToDateTime(vDate).ToString("yy-MM")}/{paddedVoucherValue}";
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
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
                    table = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) + 1 FROM {table}";
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

        public MyHttpResponseMessage Save(CustomStockTransferRequisition modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
                {
                    var Ip = common.IPAddress;
                    var Computer = common.ComputerName;
                    var Postal = common.PostalCode;
                    var username = common.Username;
                    var branch = common.Branch;
                    var period = common.Period;
                    var menuID = common.MenuID;
                    int periodTo = 0;
                    string maxIdQuery = "SELECT TOP 1 PID FROM TBL_PERIOD WHERE BCODE = '" + modelRecord.Master.TBCODE + "' AND CLOSING = 0 ORDER BY PID DESC";
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(maxIdQuery, connection);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        periodTo = Convert.ToInt32(result);
                    }
                    if (periodTo == 0)
                    {
                        response.data = "";
                        response.msg = "Please add period on this branch.";
                        response.msgType = 2;
                    }
                    else
                    {
                        string connectionString = new SQLService().getconnstring();
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlTransaction transaction = connection.BeginTransaction();
                            SqlCommand command = connection.CreateCommand();
                            command.Transaction = transaction;
                            try
                            {
                                string query = "", detailQuery = "", voucherNo = string.Empty;
                                bool IsMasterAdded = true, IsNew = false;
                                int code = 0;
                                if (modelRecord.Master.TRAN_ID == null || modelRecord.Master.TRAN_ID == 0)
                                {
                                    IsNew = true;
                                    code = GenerateNextId(common, command);

                                    if (code > 0)
                                    {
                                        modelRecord.Master.TRAN_ID = code;
                                        voucherNo = GenerateVoucherNo(common, code, CommonService.GetDateTime("Pakistan Standard Time"));
                                        if (String.IsNullOrWhiteSpace(voucherNo))
                                        {
                                            IsMasterAdded = false;
                                        }
                                    }
                                    else
                                    {
                                        IsMasterAdded = false;
                                    }

                                    query = $"INSERT INTO {table}" +
                                            "(TRAN_ID,V_DATE,VOUCHER_NO," +
                                            "REF,REMARKS,TBCODE,BCODE,TPERIOD_ID,PERIOD_ID," +
                                            "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                            "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                                            "EDIT_POSTALCODE,ASTATUS,MENU_ID,DLT)" +
                                            "VALUES" +
                                            "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "'," +
                                            "'" + modelRecord.Master.REF + "','" + modelRecord.Master.REMARKS + "','" + modelRecord.Master.TBCODE + "','" + branch + "','" + periodTo + "','" + period + "'," +
                                            "'" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                            "'" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Computer + "','" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.Master.ASTATUS + "','" + menuID + "','T')";
                                    command.CommandText = query;
                                    command.ExecuteNonQuery();
                                }
                                else
                                {
                                    query = $"UPDATE {table} SET V_DATE = '" + modelRecord.Master.V_DATE + @"',
                                                REF = '" + modelRecord.Master.REF + @"',
                                                REMARKS = '" + modelRecord.Master.REMARKS + @"',
                                                TBCODE = '" + modelRecord.Master.TBCODE + @"',
                                                TPERIOD_ID = '" + periodTo + @"',
                                                EDIT_USER_ID = '" + username + @"',
                                                EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                                EDIT_COMPUTER_NAME = '" + Computer + @"',
                                                EDIT_IP_ADDRESS = '" + Ip + @"',
                                                EDIT_POSTALCODE = '" + Postal + @"',
                                                ASTATUS = '" + modelRecord.Master.ASTATUS + @"'
                                                WHERE TRAN_ID = '" + modelRecord.Master.TRAN_ID + "' AND BCODE = '" + branch + "' AND PERIOD_ID = '" + period + "'";
                                    command.CommandText = query;
                                    command.ExecuteNonQuery();
                                }

                                var isDetailAdded = true;

                                if (modelRecord.Detail.Count > 0)
                                {
                                    detailQuery = $"UPDATE {detailTable} SET DLT = 'F'" +
                                    $" WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                                    command.CommandText = detailQuery;
                                    command.ExecuteNonQuery();
                                }
                                foreach (var item in modelRecord.Detail.ToList())
                                {
                                    try
                                    {
                                        if (item.DT_CODE == null || item.DT_CODE == 0)
                                        {
                                            int detailCode = GenerateNextDetailId(common, command);
                                            if (detailCode > 0)
                                            {
                                                detailQuery = $"INSERT INTO {detailTable}" +
                                                               "(TRAN_ID,DT_CODE,ITEM_GROUP,ITEM_CODE,QTY," +
                                                               "UNIT,QTY2,BAL_QTY,DT_DESC," +
                                                               "BCODE," +
                                                               "PERIOD_ID,ADD_USER_ID,ADD_DATE," +
                                                               "ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                                                               "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                                               "ADD_POSTALCODE,EDIT_POSTALCODE," +
                                                               "MENU_ID,DLT,CHK)" +
                                                               "VALUES" +
                                                               "('" + modelRecord.Master.TRAN_ID + "','" + detailCode + "','" + item.ITEM_GROUP + "','" + item.ITEM_CODE + "','" + item.QTY + "'," +
                                                               "'" + item.UNIT + "','" + item.QTY2 + "','" + item.BAL_QTY + "','" + item.DT_DESC + "'," +
                                                               "'" + branch + "'," +
                                                               "'" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                                               "'" + Computer + "','" + Ip + "','" + username + "'," +
                                                               "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                                               "'" + Postal + "','" + Postal + "','" + menuID + "','T','" + item.CHK + "')";
                                                command.CommandText = detailQuery;
                                                command.ExecuteNonQuery();
                                            }
                                            else
                                            {
                                                isDetailAdded = false;
                                            }
                                        }
                                        else
                                        {
                                            detailQuery = $"UPDATE {detailTable} SET ITEM_CODE = '" + item.ITEM_CODE + @"',
                                                        ITEM_GROUP = '" + item.ITEM_GROUP + @"',
                                                        QTY = '" + item.QTY + @"',
                                                        UNIT = '" + item.UNIT + @"',
                                                        QTY2 = '" + item.QTY2 + @"',
                                                        BAL_QTY = '" + item.BAL_QTY + @"',
                                                        DT_DESC = '" + item.DT_DESC + @"',
                                                        CHK = '" + item.CHK + @"',
                                                        EDIT_USER_ID = '" + username + @"',
                                                        EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                                        EDIT_COMPUTER_NAME = '" + Computer + @"',
                                                        EDIT_IP_ADDRESS = '" + Ip + @"',
                                                        EDIT_POSTALCODE = '" + Postal + @"',
                                                        DLT = 'T' 
                                                        WHERE TRAN_ID = '" + modelRecord.Master.TRAN_ID + "' AND DT_CODE = '" + item.DT_CODE + "' AND BCODE = '" + branch + "' AND PERIOD_ID = '" + period + "'";
                                            command.CommandText = detailQuery;
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        isDetailAdded = false;
                                    }
                                }

                                if (IsMasterAdded && isDetailAdded)
                                {
                                    transaction.Commit();
                                    response.data = new
                                    {
                                        code = IsNew ? code : modelRecord.Master.TRAN_ID,
                                        voucherNo = IsNew ? voucherNo : modelRecord.Master.VOUCHER_NO,
                                    };
                                    response.msgType = 1;
                                    response.msg = IsNew ? "Record Added Successfully" : "Record Updated Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.data = "";
                                    response.msg = "Something went wrong! please try again later.";
                                    response.msgType = 2;
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
                response.msgType = 2;
                response.msg = _catchMessage;
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
                            string query = $"UPDATE {table} SET DLT = 'F' WHERE TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
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

        public MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string? table = menu.TABLE1;
                string? table2 = menu.TABLE2;
                string connectionString = new SQLService().getconnstring();

                StockTransferRequisition stockTransferRequisition = new StockTransferRequisition();
                List<StockTransferRequisitionDetail> stockTransferRequisitionDetailList = new List<StockTransferRequisitionDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        stockTransferRequisition = new StockTransferRequisition
                        {
                            V_DATE = Convert.ToDateTime(reader["V_DATE"]),
                            REF = Convert.ToString(reader["REF"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            TBCODE = Convert.ToInt32(reader["TBCODE"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new StockTransferRequisitionDetail
                        {
                            ITEM_GROUP = Convert.ToInt32(detail_Reader["ITEM_GROUP"]),
                            ITEM_CODE = Convert.ToInt32(detail_Reader["ITEM_CODE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                            UNIT = Convert.ToInt32(detail_Reader["UNIT"]),
                            QTY2 = Convert.ToDouble(detail_Reader["QTY2"]),
                            BAL_QTY = Convert.ToDouble(detail_Reader["BAL_QTY"]),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            CHK = detail_Reader["CHK"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["CHK"]),
                        };
                        stockTransferRequisitionDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customRequisition = new CustomStockTransferRequisition
                {
                    Master = stockTransferRequisition,
                    Detail = stockTransferRequisitionDetailList
                };

                response = this.Save(customRequisition, common);

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
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }

        public MyHttpResponseMessage DeleteStockTransferRequisitionDetailByCode(int code, Common common)
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
                    table = menu.TABLE2;
                }
                if (!String.IsNullOrWhiteSpace(table))
                {
                    var branch = common.Branch;
                    var period = common.Period;
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
                            string query = $"UPDATE {table} SET DLT = 'F'" +
                                $" WHERE DT_CODE = '{code}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
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

        public MyHttpResponseMessage GetDataForCartonSticker(List<StockTransferRequisitionStickerPrint> data)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var codes = String.Join(',', data.Select(d => d.ITEM_CODE).ToList());
                List<StockTransferRequisitionStickerPrint> jsonDataResult = new List<StockTransferRequisitionStickerPrint>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $"SELECT BL.GROUP_NAME AS BLABEL, IMC.GROUP_NAME AS CATEGORY, IMSC.GROUP_NAME AS SUBCATEGORY, IM.ITEM_ID, IM.ITEM_NAME, B.BARCODE_TYPE, C.GROUP_NAME AS COLOR, " +
                                   $"S.GROUP_NAME AS SIZE FROM TBL_BARCODE B WITH (NOLOCK) " +
                                   $"LEFT JOIN TBL_BLABEL BL WITH (NOLOCK) ON B.BLABEL = BL.GROUP_CODE " +
                                   $"LEFT JOIN TBL_COLOR C WITH (NOLOCK) ON B.COLOR = C.GROUP_CODE " +
                                   $"LEFT JOIN TBL_SIZE S WITH (NOLOCK) ON B.SIZE = S.GROUP_CODE " +
                                   $"LEFT JOIN TBL_ITEMSMASTER IM WITH (NOLOCK) ON B.ITEM_CODE = IM.ITEM_CODE " +
                                   $"LEFT JOIN TBL_CATEGORY IMC WITH (NOLOCK) ON IMC.GROUP_CODE = IM.CAT_CODE " +
                                   $"LEFT JOIN TBL_SUB_CATEGORY IMSC WITH (NOLOCK) ON IMSC.GROUP_CODE = IM.SUB_CAT_CODE " +
                                   $"WHERE B.DLT = 'T' AND B.ASTATUS= 'Y' AND IM.DLT = 'T' AND B.CODE IN ({codes})";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new StockTransferRequisitionStickerPrint
                        {
                            BLABEL = Convert.ToString(reader["BLABEL"]),
                            CATEGORIES = Convert.ToString(reader["CATEGORY"]),
                            SUBCATEGORIES = Convert.ToString(reader["SUBCATEGORY"]),
                            ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                            COLORS = Convert.ToString(reader["COLOR"]),
                            SIZES = Convert.ToString(reader["SIZE"]),
                            ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                            QTY = data.Sum(d => d.QTY),
                        };
                        jsonDataResult.Add(row);
                    }
                    reader.Close();
                }

                response.data = jsonDataResult;
                response.msg = "";
                response.msgType = 1;
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

        public MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Branch currentBranch, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            StockTransferRequisitionForPrint masterData = new StockTransferRequisitionForPrint();
            CustomStockTransferRequisitionForPrintReport reportData = new CustomStockTransferRequisitionForPrintReport();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? tableMaster = string.Empty;
                string? tableDetail = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    tableMaster = menu.TABLE1;
                    tableDetail = menu.TABLE2;
                }
                masterData.COMPANY_PHONE = currentCompany.C_TEL;
                masterData.COMPANY_NAME = currentCompany.C_NAME;
                masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                masterData.MENU_SIG1 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG1) ? true : false;
                masterData.MENU_SIG2 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG2) ? true : false;
                masterData.MENU_SIG3 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG3) ? true : false;
                masterData.MENU_SIG4 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG4) ? true : false;
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $"SELECT M.V_DATE, M.VOUCHER_NO, M.REF, M.REMARKS, BF.B_NAME AS BranchFrom, BF.B_ADDRESS AS BranchFromAdr, BT.B_NAME AS BranchTo, BT.B_ADDRESS AS BranchToAdr, " +
                        $"CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS FROM {tableMaster} M " +
                        $"LEFT JOIN TBL_BRANCH BF ON BF.BCODE = M.BCODE " +
                        $"LEFT JOIN TBL_BRANCH BT ON BT.BCODE = M.TBCODE " +
                        $"WHERE M.DLT = 'T' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}' AND M.TRAN_ID = {modelRecord.TRAN_ID}";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        masterData.V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]);
                        masterData.VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]);
                        masterData.REF = Convert.ToString(reader["REF"]);
                        masterData.REMARKS = Convert.ToString(reader["REMARKS"]);
                        masterData.BRANCH_FROM_NAME = Convert.ToString(reader["BranchFrom"]);
                        masterData.BRANCH_FROM_ADDRESS = Convert.ToString(reader["BranchFromAdr"]);
                        masterData.BRANCH_TO_NAME = Convert.ToString(reader["BranchTo"]);
                        masterData.BRANCH_TO_ADDRESS = Convert.ToString(reader["BranchToAdr"]);
                        masterData.ASTATUS = Convert.ToString(reader["ASTATUS"]);
                    }
                    reader.Close();
                }
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $"SELECT IM.ITEM_NAME AS Item, IMG.GROUP_NAME AS Groupname, D.BAL_QTY, D.ITEM_GROUP FROM {tableDetail} D " +
                                   $"LEFT JOIN TBL_ITEMSMASTER IM ON IM.ITEM_CODE = D.ITEM_CODE " +
                                   $"LEFT JOIN TBL_ITEMSGROUP IMG ON IMG.GROUP_CODE = D.ITEM_GROUP " +
                                   $"WHERE D.DLT = 'T' AND D.TRAN_ID = {modelRecord.TRAN_ID}";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    Dictionary<string, int> groupSeries = new Dictionary<string, int>();
                    int seriesCounter = 1;

                    while (reader.Read())
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["BalQty"] = Convert.ToInt32(reader["BAL_QTY"]);
                        dataRow["Group"] = Convert.ToString(reader["Groupname"]);
                        dataRow["Category"] = Convert.ToString(reader["Item"]);
                        string groupName = Convert.ToString(reader["Groupname"]);
                        if (!groupSeries.ContainsKey(groupName))
                        {
                            groupSeries[groupName] = seriesCounter;
                            seriesCounter++;
                        }
                        dataRow["DesignNo"] = groupSeries[groupName];
                        dataTable.Rows.Add(dataRow);
                    }
                    reader.Close();
                }
                reportData.Master = masterData;
                reportData.Detail = dataTable;
                response.data = reportData;
                response.msg = "";
                response.msgType = 1;
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

        public MyHttpResponseMessage UpdatePrintStatus(string codes, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    string connectionString = new SQLService().getconnstring();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = $"UPDATE {table} SET [PRINT] = 'Y' WHERE DT_CODE IN({codes}) AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        response.msgType = 1;
                        response.msg = "Record Updated Successfully";
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