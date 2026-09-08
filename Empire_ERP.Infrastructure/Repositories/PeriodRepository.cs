using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class PeriodRepository : IPeriodRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public PeriodRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
        public MyHttpResponseMessage GetPeriodsByBranch(int branchid)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var periods = FetchPeriodsByBranch(branchid);
                List<Period> periodList = new List<Period>();
                if (periods.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in periods.Tables[0].Rows)
                    {
                        Period period = new Period();
                        period.PID = Convert.ToInt32(Row["PID"]);
                        period.DESCR = Convert.ToString(Row["DESCR"]);
                        periodList.Add(period);
                    }
                }
                response.data = periodList;
                response.msg = "";
                response.msgType = 1;
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataSet FetchPeriodsByBranch(int branchid)
        {
            string query = "select * from [dbo].[TBL_PERIOD] WHERE DLT = 'T' AND BCODE =" + branchid + " AND ASTATUS = 'Y' ORDER BY 1 DESC";
            DataSet data = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
            return data;
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
                        string query = "SELECT PID,START_D," +
                            " START_E,DESCR,BCODE,CLOSING," +
                            " CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS," +
                            " ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                            " ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                            " EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                            " ADD_POSTALCODE,EDIT_POSTALCODE" +
                            " FROM " + table + "" +
                            " WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' ORDER BY PID DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new Period
                            {
                                PID = Convert.ToInt32(reader["PID"]),
                                BCODE = Convert.ToInt32(reader["BCODE"]),
                                START_E = Convert.ToDateTime(reader["START_E"]),
                                START_D = Convert.ToDateTime(reader["START_D"]),
                                DESCR = Convert.ToString(reader["DESCR"]),
                                CLOSING = Convert.ToInt32(reader["CLOSING"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]),
                                ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]),
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
        public MyHttpResponseMessage Save(Period modelRecord, Common common)
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
                            if (modelRecord.PID == null || modelRecord.PID == 0)
                            {
                                query = "INSERT INTO " + table + "" +
                                    "(PID,START_D,START_E,DESCR," +
                                    "BCODE,CLOSING,ASTATUS," +
                                    "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                    "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                    "MENU_ID,ADD_POSTALCODE,EDIT_POSTALCODE,DLT)" +
                                    "VALUES" +
                                    "('" + GenerateNextId(common) + "','" + modelRecord.START_D + "','" + modelRecord.START_E + "','" + modelRecord.DESCR + "'," +
                                    "'" + modelRecord.BCODE + "','" + modelRecord.CLOSING + "','" + modelRecord.ASTATUS + "'," +
                                    "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                    "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                    "'" + common.MenuID + "','" + Postal + "','" + Postal + "','T')";
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE DESCR = '" + modelRecord.DESCR + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
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
                                query = "UPDATE " + table + " SET DESCR = '" + modelRecord.DESCR + @"',
                                        START_D = '" + modelRecord.START_D + @"',
                                        START_E = '" + modelRecord.START_E + @"',
                                        BCODE = '" + modelRecord.BCODE + @"',
                                        CLOSING = '" + modelRecord.CLOSING + @"',
                                        ASTATUS = '" + modelRecord.ASTATUS + @"',
                                        EDIT_USER_ID = '" + userid + @"',
                                        EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                        EDIT_COMPUTER_NAME = '" + Computer + @"',
                                        EDIT_IP_ADDRESS = '" + Ip + @"',
                                        MENU_ID = '" + common.MenuID + @"',
                                        EDIT_POSTALCODE = '" + Postal + @"'
                                        WHERE PID = '" + modelRecord.PID + @"'";
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE DESCR = '" + modelRecord.DESCR + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
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
                    string maxIdQuery = "SELECT ISNULL(MAX(PID), 0) + 1 FROM " + table;
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

        public MyHttpResponseMessage GetPeriodById(int id, Common common)
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
                        string query = "SELECT PID,START_D," +
                            " START_E,DESCR,BCODE,CLOSING," +
                            " ASTATUS," +
                            " EDIT_USER_ID,EDIT_DATE," +
                            " EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                            " EDIT_POSTALCODE" +
                            " FROM " + table + "" +
                            " WHERE MENU_ID = '" + common.MenuID + "' AND PID = '" + id + "' AND DLT = 'T'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var Period = new Period
                            {
                                PID = Convert.ToInt32(reader["PID"]),
                                //START_D = Convert.ToDateTime(reader["START_D"]),
                                //START_E = Convert.ToDateTime(reader["START_E"]),
                                START_D = Convert.ToDateTime(reader["START_D"] == DBNull.Value ? null : Convert.ToDateTime(reader["START_D"]).ToString("yyyy-MM-dd")),
                                START_E = Convert.ToDateTime(reader["START_E"] == DBNull.Value ? null : Convert.ToDateTime(reader["START_E"]).ToString("yyyy-MM-dd")),
                                //START_D = reader["START_D"] == DBNull.Value ? (DateTime?)null : ((DateTime)reader["START_D"]).ToString("dd-MMM-yyyy"),
                                //START_E = reader["START_E"] == DBNull.Value ? (DateTime?)null : ((DateTime)reader["START_E"]).Date,
                                DESCR = Convert.ToString(reader["DESCR"]),
                                BCODE = Convert.ToInt32(reader["BCODE"]),
                                CLOSING = Convert.ToInt32(reader["CLOSING"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                            };

                            response.msg = "";
                            response.msgType = 1;
                            response.data = Period;
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE PID = '" + id + "'";
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

        public MyHttpResponseMessage GetPeriodById(int periodID)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "SELECT PID,START_D," +
                                   " START_E,DESCR,BCODE,CLOSING," +
                                   " ASTATUS," +
                                   " EDIT_USER_ID,EDIT_DATE," +
                                   " EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                   " EDIT_POSTALCODE" +
                                   " FROM TBL_PERIOD" +
                                   $" WHERE PID = '{periodID}' AND DLT = 'T'";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        var Period = new Period
                        {
                            START_D = Convert.ToDateTime(reader["START_D"] == DBNull.Value ? null : Convert.ToDateTime(reader["START_D"]).ToString("yyyy-MM-dd")),
                            START_E = Convert.ToDateTime(reader["START_E"] == DBNull.Value ? null : Convert.ToDateTime(reader["START_E"]).ToString("yyyy-MM-dd")),
                            CLOSING = Convert.ToInt32(reader["CLOSING"]),
                        };

                        response.msg = "";
                        response.msgType = 1;
                        response.data = Period;
                    }
                    reader.Close();
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