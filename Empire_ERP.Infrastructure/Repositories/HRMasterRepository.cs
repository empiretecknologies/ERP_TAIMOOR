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
    public class HRMasterRepository : IHRMasterRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public HRMasterRepository(IMenuRepository menuRepository)
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
                        if (TableName == "TBL_DESIGNATION")
                        {
                            query = "SELECT GROUP_CODE,GROUP_NAME , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";

                            //query = "SELECT GROUP_CODE,GROUP_NAME,QTY,ADD_USER_ID," +
                            //           "ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                            //           "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                            //           "EDIT_POSTALCODE,CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                            //           "FROM " + table + " " +
                            //           "WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' ORDER BY GROUP_CODE DESC";
                        }
                        if (TableName == "TBL_LEAVES")
                        {
                            query = "SELECT GROUP_CODE,GROUP_NAME , LEAVES , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }
                        if (TableName == "TBL_SHIFT")
                        {
                            query = "SELECT GROUP_CODE,GROUP_NAME , TIME_IN , TIME_OUT , WHR , GTIME_IN , GTIME_OUT , BTIME_IN , BTIME_OUT , NIGHT_SHIFT , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }
                        if (TableName == "TBL_TRANSPORT")
                        {
                            query = "SELECT GROUP_CODE,GROUP_NAME , CAPACITY , DRIVER , VEHICLE , T_ROUTE , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }
                        if (TableName == "TBL_PAY_SCALE")
                        {
                            query = "SELECT GROUP_CODE,GROUP_NAME , BASIC_SALARY , GROSS_SALARY , ANNUAL_PCT , ANNUAL_AMT , MAXIMUM_SALARY , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (TableName == "TBL_DESIGNATION")
                            {
                                var row = new HRMaster
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                //var row = new HRMaster
                                //{
                                //    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                //    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                //    QTY = reader["QTY"] == DBNull.Value ? null : Convert.ToDouble(reader["QTY"]),
                                //    ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                //    ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                //    ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]),
                                //    ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                //    ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                //    EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                //    EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]),
                                //    EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                //    EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                //    ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                //    EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                //};

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_LEAVES")
                            {
                                var row = new HRMaster
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                    LEAVES = Convert.ToString(reader["LEAVES"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_SHIFT")
                            {
                                var row = new HRMaster
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                    TIMEIN = Convert.ToString(reader["TIME_IN"]),
                                    TIMEOUT = Convert.ToString(reader["TIME_OUT"]),
                                    HOURS = Convert.ToString(reader["WHR"]),
                                    GTIMEIN = Convert.ToString(reader["GTIME_IN"]),
                                    GTIMEOUT = Convert.ToString(reader["GTIME_OUT"]),
                                    BTIMEIN = Convert.ToString(reader["BTIME_OUT"]),
                                    BTIMEOUT = Convert.ToString(reader["BTIME_OUT"]),
                                    NIGHTSHIFT = Convert.ToString(reader["NIGHT_SHIFT"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_TRANSPORT")
                            {
                                var row = new HRMaster
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                    Capacity = Convert.ToString(reader["CAPACITY"]),
                                    Driver = Convert.ToString(reader["DRIVER"]),
                                    Vehicle = Convert.ToString(reader["VEHICLE"]),
                                    tRoute = Convert.ToString(reader["T_ROUTE"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_PAY_SCALE")
                            {
                                var row = new HRMaster
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                    BSALARY = Convert.ToString(reader["BASIC_SALARY"]),
                                    GSALARY = Convert.ToString(reader["GROSS_SALARY"]),
                                    AnnualPct = Convert.ToString(reader["ANNUAL_PCT"]),
                                    Annualamt = Convert.ToString(reader["ANNUAL_AMT"]),
                                    MaximumSalary = Convert.ToString(reader["MAXIMUM_SALARY"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }


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

        public MyHttpResponseMessage Save(HRMaster modelRecord, Common common)
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

                if (!String.IsNullOrWhiteSpace(modelRecord.TableName))
                {
                    var Ip = common.IPAddress;
                    var Computer = common.ComputerName;
                    var Postal = common.PostalCode;
                    var userid = common.Username;
                    bool IsInsert = false;
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
                            if (modelRecord.GROUP_CODE == null || modelRecord.GROUP_CODE == 0)
                            { 
                                if (modelRecord.TableName == "TBL_DESIGNATION")
                                {
                                    query = $"INSERT INTO {modelRecord.TableName} " +
                                           "(GROUP_CODE,GROUP_NAME,ADD_USER_ID,ADD_DATE," +
                                           "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                           "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                           "ADD_COMPUTER_NAME,MENU_ID,DLT,MASTER_ID)" +
                                           "VALUES" +
                                           "('" + GenerateNextId(common , modelRecord.TableName) + "','" + modelRecord.GROUP_NAME + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                           "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                           "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                           "'" + Computer + "','" + common.MenuID + $"','T',{modelRecord.MasterId})";
                                }
                                else if(modelRecord.TableName == "TBL_LEAVES")
                                {
                                    query = $"INSERT INTO {modelRecord.TableName} " +
                                           "(GROUP_CODE,GROUP_NAME,LEAVES,ADD_USER_ID,ADD_DATE," +
                                           "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                           "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                           "ADD_COMPUTER_NAME,MENU_ID,DLT,MASTER_ID)" +
                                           "VALUES" +
                                           "('" + GenerateNextId(common, modelRecord.TableName) + "','" + modelRecord.GROUP_NAME + "','" + modelRecord.LEAVES + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                           "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                           "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                           "'" + Computer + "','" + common.MenuID + $"','T',{modelRecord.MasterId})";
                                }
                                else if(modelRecord.TableName == "TBL_SHIFT")
                                {
                                    query = $"INSERT INTO {modelRecord.TableName} " +
                                           "(GROUP_CODE,GROUP_NAME,TIME_IN,TIME_OUT,WHR,GTIME_IN,GTIME_OUT,BTIME_IN,BTIME_OUT,NIGHT_SHIFT,ADD_USER_ID,ADD_DATE," +
                                           "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                           "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                           "ADD_COMPUTER_NAME,MENU_ID,DLT,MASTER_ID)" +
                                           "VALUES" +
                                           "('" + GenerateNextId(common, modelRecord.TableName) + "','" + modelRecord.GROUP_NAME + "','" + modelRecord.TIMEIN + "','" + modelRecord.TIMEOUT + "','" + modelRecord.HOURS + "'," +
                                           "'" + modelRecord.GTIMEIN + "','" + modelRecord.GTIMEOUT + "','" + modelRecord.BTIMEOUT + "','" + modelRecord.BTIMEOUT + "','" + modelRecord.NIGHTSHIFT + "','"
                                           + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                           "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                           "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                           "'" + Computer + "','" + common.MenuID + $"','T',{modelRecord.MasterId})";
                                }
                                else if(modelRecord.TableName == "TBL_TRANSPORT")
                                {
                                    query = $"INSERT INTO {modelRecord.TableName} " +
                                           "(GROUP_CODE,GROUP_NAME,Capacity,Driver,Vehicle,T_ROUTE,ADD_USER_ID,ADD_DATE," +
                                           "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                           "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                           "ADD_COMPUTER_NAME,MENU_ID,DLT,MASTER_ID)" +
                                           "VALUES" +
                                           "('" + GenerateNextId(common, modelRecord.TableName) + "','" + modelRecord.GROUP_NAME + "','" + modelRecord.Capacity + "','" + modelRecord.Driver + "','" + modelRecord.Vehicle + "'," +
                                           "'" + modelRecord.tRoute + "','"
                                           + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                           "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                           "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                           "'" + Computer + "','" + common.MenuID + $"','T',{modelRecord.MasterId})";
                                }
                                else if(modelRecord.TableName == "TBL_PAY_SCALE")
                                {
                                    query = $"INSERT INTO {modelRecord.TableName} " +
                                           "(GROUP_CODE,GROUP_NAME,BASIC_SALARY,GROSS_SALARY, ANNUAL_PCT,ANNUAL_AMT,MAXIMUM_SALARY,ADD_USER_ID,ADD_DATE," +
                                           "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                           "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                           "ADD_COMPUTER_NAME,MENU_ID,DLT,MASTER_ID)" +
                                           "VALUES" +
                                           "('" + GenerateNextId(common, modelRecord.TableName) + "','" + modelRecord.GROUP_NAME + "','" + modelRecord.BSALARY + "','" + modelRecord.GSALARY + "','" + modelRecord.AnnualPct + "'," +
                                           "'" + modelRecord.Annualamt + "','"  + modelRecord.MaximumSalary + "','"
                                           + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                           "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                           "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                           "'" + Computer + "','" + common.MenuID + $"','T',{modelRecord.MasterId})";
                                }

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                IsInsert = true;
                            }
                            else
                            {
                                if(modelRecord.TableName == "TBL_DESIGNATION")
                                {
                                    query = $"UPDATE {modelRecord.TableName} SET " +
                                                $"GROUP_NAME = '{modelRecord.GROUP_NAME}', " +
                                                $"ASTATUS = '{modelRecord.ASTATUS}', " +
                                                $"EDIT_USER_ID = '{userid}', " +
                                                $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                $"EDIT_COMPUTER_NAME = '{Computer}', " +
                                                $"EDIT_IP_ADDRESS = '{Ip}', " +
                                                $"EDIT_POSTALCODE = '{Postal}' " +
                                                $"WHERE GROUP_CODE = {modelRecord.GROUP_CODE}";
                                }
                                else if(modelRecord.TableName == "TBL_LEAVES")
                                {
                                    query = $"UPDATE {modelRecord.TableName} SET " +
                                                $"GROUP_NAME = '{modelRecord.GROUP_NAME}', " +
                                                $"LEAVES = '{modelRecord.LEAVES}', " +
                                                $"ASTATUS = '{modelRecord.ASTATUS}', " +
                                                $"EDIT_USER_ID = '{userid}', " +
                                                $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                $"EDIT_COMPUTER_NAME = '{Computer}', " +
                                                $"EDIT_IP_ADDRESS = '{Ip}', " +
                                                $"EDIT_POSTALCODE = '{Postal}' " +
                                                $"WHERE GROUP_CODE = {modelRecord.GROUP_CODE}";
                                }
                                else if(modelRecord.TableName == "TBL_SHIFT")
                                {
                                    query = $"UPDATE {modelRecord.TableName} SET " +
                                            $"GROUP_NAME = '{modelRecord.GROUP_NAME}', " +
                                            $"TIME_IN = '{modelRecord.TIMEIN}', " +
                                            $"TIME_OUT = '{modelRecord.TIMEOUT}', " +
                                            $"WHR = '{modelRecord.HOURS}', " +
                                            $"GTIME_IN = '{modelRecord.GTIMEIN}', " +
                                            $"GTIME_OUT = '{modelRecord.GTIMEOUT}', " +
                                            $"BTIME_IN = '{modelRecord.BTIMEOUT}', " +
                                            $"BTIME_OUT = '{modelRecord.BTIMEOUT}', " +
                                            $"NIGHT_SHIFT = '{modelRecord.NIGHTSHIFT}', " +
                                            $"EDIT_USER_ID = '{userid}', " +
                                            $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                            $"EDIT_COMPUTER_NAME = '{Computer}', " +
                                            $"EDIT_IP_ADDRESS = '{Ip}', " +
                                            $"EDIT_POSTALCODE = '{Postal}', " +
                                            $"ASTATUS = '{modelRecord.ASTATUS}' " +
                                            $"WHERE GROUP_CODE = '{modelRecord.GROUP_CODE}'";

                                }
                                else if(modelRecord.TableName == "TBL_TRANSPORT")
                                {
                                    query = $"UPDATE {modelRecord.TableName} SET " +
                                            $"GROUP_NAME = '{modelRecord.GROUP_NAME}', " +
                                            $"Capacity = '{modelRecord.Capacity}', " +
                                            $"Driver = '{modelRecord.Driver}', " +
                                            $"Vehicle = '{modelRecord.Vehicle}', " +
                                            $"T_ROUTE = '{modelRecord.tRoute}', " +
                                            $"EDIT_USER_ID = '{userid}', " +
                                            $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                            $"EDIT_COMPUTER_NAME = '{Computer}', " +
                                            $"EDIT_IP_ADDRESS = '{Ip}', " +
                                            $"EDIT_POSTALCODE = '{Postal}', " +
                                            $"ASTATUS = '{modelRecord.ASTATUS}' " +
                                            $"WHERE GROUP_CODE = '{modelRecord.GROUP_CODE}'";
                                }
                                else if(modelRecord.TableName == "TBL_PAY_SCALE")
                                {
                                    query = $"UPDATE {modelRecord.TableName} SET " +
                                            $"GROUP_NAME = '{modelRecord.GROUP_NAME}', " +
                                            $"BASIC_SALARY = '{modelRecord.BSALARY}', " +
                                            $"GROSS_SALARY = '{modelRecord.GSALARY}', " +
                                            $"ANNUAL_PCT = '{modelRecord.AnnualPct}', " +
                                            $"ANNUAL_AMT = '{modelRecord.Annualamt}', " +
                                            $"MAXIMUM_SALARY = '{modelRecord.MaximumSalary}', " +
                                            $"EDIT_USER_ID = '{userid}', " +
                                            $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                            $"EDIT_COMPUTER_NAME = '{Computer}', " +
                                            $"EDIT_IP_ADDRESS = '{Ip}', " +
                                            $"EDIT_POSTALCODE = '{Postal}', " +
                                            $"ASTATUS = '{modelRecord.ASTATUS}' " +
                                            $"WHERE GROUP_CODE = '{modelRecord.GROUP_CODE}'";
                                }


                                command.CommandText = query;
                                command.ExecuteNonQuery();
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



        public string GenerateNextId(Common common, string TableName)
        {
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = TableName;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    //table = menu.TABLE1;
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

        public MyHttpResponseMessage GetHRMasterById(int id, string TableName, Common common)
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

                if (!String.IsNullOrWhiteSpace(TableName))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        if (TableName == "TBL_DESIGNATION")
                        {
                            query = "SELECT GROUP_CODE,GROUP_NAME , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       $"WHERE GROUP_CODE = {id} AND DLT = 'T' ORDER BY GROUP_CODE ASC";

                            //query = "SELECT GROUP_CODE,GROUP_NAME,QTY,ADD_USER_ID," +
                            //           "ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                            //           "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                            //           "EDIT_POSTALCODE,CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                            //           "FROM " + table + " " +
                            //           "WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' ORDER BY GROUP_CODE DESC";
                        }
                        if (TableName == "TBL_LEAVES")
                        {
                            query = "SELECT GROUP_CODE,GROUP_NAME , LEAVES , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       $"WHERE GROUP_CODE = {id} AND DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }
                        if (TableName == "TBL_SHIFT")
                        {
                            query = "SELECT GROUP_CODE,GROUP_NAME , TIME_IN , TIME_OUT , WHR , GTIME_IN , GTIME_OUT , BTIME_IN , BTIME_OUT , NIGHT_SHIFT , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       $"WHERE GROUP_CODE = {id} AND DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }
                        if (TableName == "TBL_TRANSPORT")
                        {
                            query = "SELECT GROUP_CODE,GROUP_NAME , CAPACITY , DRIVER , VEHICLE , T_ROUTE , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       $"WHERE GROUP_CODE = {id} AND DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }
                        if (TableName == "TBL_PAY_SCALE")
                        {
                            query = "SELECT GROUP_CODE,GROUP_NAME , BASIC_SALARY , GROSS_SALARY , ANNUAL_PCT , ANNUAL_AMT , MAXIMUM_SALARY , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       $"WHERE GROUP_CODE = {id} AND DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }


                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (TableName == "TBL_DESIGNATION")
                            {
                                var row = new HRMaster
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                //var row = new HRMaster
                                //{
                                //    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                //    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                //    QTY = reader["QTY"] == DBNull.Value ? null : Convert.ToDouble(reader["QTY"]),
                                //    ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                //    ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                //    ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]),
                                //    ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                //    ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                //    EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                //    EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]),
                                //    EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                //    EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                //    ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                //    EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                //};

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_LEAVES")
                            {
                                var row = new HRMaster
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                    LEAVES = Convert.ToString(reader["LEAVES"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_SHIFT")
                            {
                                var row = new HRMaster
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                    TIMEIN = Convert.ToString(reader["TIME_IN"]),
                                    TIMEOUT = Convert.ToString(reader["TIME_OUT"]),
                                    HOURS = Convert.ToString(reader["WHR"]),
                                    GTIMEIN = Convert.ToString(reader["GTIME_IN"]),
                                    GTIMEOUT = Convert.ToString(reader["GTIME_OUT"]),
                                    BTIMEIN = Convert.ToString(reader["BTIME_OUT"]),
                                    BTIMEOUT = Convert.ToString(reader["BTIME_OUT"]),
                                    NIGHTSHIFT = Convert.ToString(reader["NIGHT_SHIFT"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_TRANSPORT")
                            {
                                var row = new HRMaster
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                    Capacity = Convert.ToString(reader["CAPACITY"]),
                                    Driver = Convert.ToString(reader["DRIVER"]),
                                    Vehicle = Convert.ToString(reader["VEHICLE"]),
                                    tRoute = Convert.ToString(reader["T_ROUTE"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_PAY_SCALE")
                            {
                                var row = new HRMaster
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                    BSALARY = Convert.ToString(reader["BASIC_SALARY"]),
                                    GSALARY = Convert.ToString(reader["GROSS_SALARY"]),
                                    AnnualPct = Convert.ToString(reader["ANNUAL_PCT"]),
                                    Annualamt = Convert.ToString(reader["ANNUAL_AMT"]),
                                    MaximumSalary = Convert.ToString(reader["MAXIMUM_SALARY"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }


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

        public MyHttpResponseMessage Delete(int id, string TableName, Common common)
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

                if (!String.IsNullOrWhiteSpace(TableName))
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
                            string query = "UPDATE " + TableName + " SET DLT = 'F' WHERE GROUP_CODE = '" + id + "'";
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