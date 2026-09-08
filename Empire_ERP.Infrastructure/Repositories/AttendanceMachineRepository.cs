using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Reflection;
using System.Security.Policy;
using ZXing.QrCode.Internal;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class AttendanceMachineRepository : IAttendanceMachineRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public ICommonService _commonService { get; set; }
        public IPartyRepository _partyRepository { get; set; }
        public AttendanceMachineRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, IPartyRepository partyRepository, ICommonService commonService)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _partyRepository = partyRepository;
            _commonService = commonService;
        }

        public MyHttpResponseMessage GetAttendanceMachineData(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {

                var Menu = _menuRepository.GetMenu(common.MenuID);
                var table = string.Empty;
                var picktable = string.Empty;
                int rowsAffected = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    picktable = menu.PICK_TABLE_MASTER;
                }
                if (!string.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(picktable))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection zkconnection = new SqlConnection(new SQLService().get_zkconnstring()))
                    {
                        using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                        {
                            string query = $@"INSERT INTO {table} (
                                            USERID, CHECKTIME, CHECKTYPE, VERIFYCODE, SENSORID, sn, 
                                            BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, 
                                            EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, 
                                            ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT, ATT_DOWN
                                        )
                                        SELECT 
                                            Z.USERID, Z.CHECKTIME, Z.CHECKTYPE, Z.VERIFYCODE, Z.SENSORID, Z.sn, 
                                            '{common.Branch}' AS BCODE, '{common.Period}' AS PERIOD_ID, '{common.Username}' AS ADD_USER_ID, '{CommonService.GetDateTime("Pakistan Standard Time")}' AS ADD_DATE, 
                                            '{common.ComputerName}' AS ADD_COMPUTER_NAME, 
                                            '{common.IPAddress}' AS ADD_IP_ADDRESS, '{common.Username}' AS EDIT_USER_ID, '{CommonService.GetDateTime("Pakistan Standard Time")}' AS EDIT_DATE, 
                                            '{common.ComputerName}' AS EDIT_COMPUTER_NAME, '{common.IPAddress}' AS EDIT_IP_ADDRESS, 
                                            '{common.PostalCode}' AS ADD_POSTALCODE, '{common.PostalCode}' AS EDIT_POSTALCODE, 
                                            '{common.MenuID}' AS MENU_ID, 'T' AS DLT, 0 AS ATT_DOWN
                                        FROM {zkconnection.Database}.dbo.{picktable} Z
                                        WHERE NOT EXISTS (
                                            SELECT 1 
                                            FROM {table} T 
                                            WHERE Z.CHECKTIME = T.CHECKTIME
                                        ); 
                            ";

                            SqlCommand command = new SqlCommand(query, connection);
                            connection.Open();
                            rowsAffected = command.ExecuteNonQuery();
                            connection.Close();

                            string Pickquery = $@"SELECT EM.MACHINE_CODE,CASE WHEN CH.CHECKTYPE = 'I' THEN 'In' Else 'Out' End as CHECKTYPE,
                                                CONVERT(DATE, CH.CHECKTIME) AS CHK_DATE,
												FORMAT(CH.CHECKTIME, 'hh:mm:ss tt')  CHK_TIME,
                                                EM.EMP_ID,EM.ENAME,EM.FATHER_NAME,AG.DESCR AS DEP_NAME,
                                                SFT.GROUP_NAME AS SHIFT_T,EM.EMAIL,EM.CELL_NO,R.GROUP_NAME AS REG,EM.JOIN_DATE,EM.PARM_DATE,
                                                CASE WHEN EM.SALARY_HOLD = 'Y' THEN  'Freeze' Else 'Active' End AS SALARY_HOLD,
                                                CASE WHEN EM.MSTATUS = 'Y' THEN  'Management' else 'Non-Management' end  AS MSTATUS
                                                FROM {table} CH
                                                LEFT OUTER JOIN TBL_EMP_REG EM
                                                ON EM.MACHINE_CODE = CH.USERID
                                                LEFT OUTER JOIN TBL_ACT_GROUP AG
                                                ON AG.CODE = EM.DEP_ID
                                                LEFT OUTER JOIN TBL_SHIFT SFT
                                                ON SFT.GROUP_CODE = EM.SHIFT_T
                                                LEFT OUTER JOIN TBL_RELIGION R
                                                ON R.GROUP_CODE = EM.REG
                                                WHERE CH.ATT_DOWN = 0 AND CH.DLT = 'T'";
                            command = new SqlCommand(Pickquery, connection);
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                var row = new
                                {
                                    MACHINECODE = Convert.ToString(reader["MACHINE_CODE"]),
                                    CHECKTYPE = Convert.ToString(reader["CHECKTYPE"]),
                                    DATE = Convert.ToString(reader["CHK_DATE"]),
                                    Time = Convert.ToString(reader["CHK_TIME"]),
                                    EMPID = Convert.ToString(reader["EMP_ID"]),
                                    ENAME = Convert.ToString(reader["ENAME"]),
                                    FATHERNAME = Convert.ToString(reader["FATHER_NAME"]),
                                    DEPNAME = Convert.ToString(reader["DEP_NAME"]),
                                    SHIFTT = Convert.ToString(reader["SHIFT_T"]),
                                    EMAIL = Convert.ToString(reader["EMAIL"]),
                                    CELL_NO = Convert.ToString(reader["CELL_NO"]),
                                    REG = Convert.ToString(reader["REG"]),
                                    JOINDATE = Convert.ToString(reader["JOIN_DATE"]),
                                    PARMDATE = Convert.ToString(reader["PARM_DATE"]),
                                    SALARYHOLD = Convert.ToString(reader["SALARY_HOLD"]),
                                    MSTATUS = Convert.ToString(reader["MSTATUS"]),

                                };
                                jsonDataResult.Add(row);
                            }
                            reader.Close();

                            string updatequery = $@"UPDATE {table} SET ATT_DOWN = 1";

                            command = new SqlCommand(updatequery, connection);
                            rowsAffected = command.ExecuteNonQuery();
                            connection.Close();
                        }
                    }


                    response.data = jsonDataResult;
                    response.msg = $"{rowsAffected} rows inserted successfully.";
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

        public MyHttpResponseMessage GetAllAttendance(Attendance attendance, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                var Menu = _menuRepository.GetMenu(common.MenuID);
                var table = string.Empty;
                var picktable = string.Empty;
                int rowsAffected = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    picktable = menu.PICK_TABLE_MASTER;
                }
                string query = string.Empty;
                var fDate = attendance.FromDate.ToString("yyyy-MM-dd");
                var tDate = attendance.ToDate.ToString("yyyy-MM-dd");
                var time = attendance.Time != null ? attendance.Time : "";

                if (!string.IsNullOrWhiteSpace(fDate) && !String.IsNullOrWhiteSpace(tDate))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        if (attendance.Employee == null && attendance.EmpId == null)
                        {
                            query = $"SELECT EM.MACHINE_CODE,CASE WHEN CH.CHECKTYPE = 'I' THEN 'In' Else 'Out' End as CHECKTYPE," +
                           $"CONVERT(DATE, CH.CHECKTIME) AS CHK_DATE,FORMAT(CH.CHECKTIME, 'hh:mm:ss tt')  CHK_TIME," +
                           $"EM.EMP_ID,EM.ENAME,EM.FATHER_NAME,AG.DESCR AS DEP_NAME," +
                           $"SFT.GROUP_NAME AS SHIFT_T,EM.EMAIL,EM.CELL_NO,R.GROUP_NAME AS REG,EM.JOIN_DATE,EM.PARM_DATE,CASE WHEN EM.SALARY_HOLD = 'Y' THEN  'Freeze' Else 'Active' End AS SALARY_HOLD," +
                           $"CASE WHEN EM.MSTATUS = 'Y' THEN  'Management' else 'Non-Management' end  AS MSTATUS FROM {table} CH LEFT OUTER JOIN TBL_EMP_REG EM ON EM.MACHINE_CODE = CH.USERID " +
                           $"LEFT OUTER JOIN TBL_ACT_GROUP AG ON AG.CODE = EM.DEP_ID LEFT OUTER JOIN TBL_SHIFT SFT ON SFT.GROUP_CODE = EM.SHIFT_T LEFT OUTER JOIN TBL_RELIGION R ON R.GROUP_CODE = EM.REG " +
                       $"WHERE CH.ATT_DOWN = 1 AND CH.DLT = 'T' AND CONVERT(DATE, CH.CHECKTIME) BETWEEN '{fDate}' AND '{tDate}' AND CONVERT(time(0), CH.CHECKTIME) > '{time}'";
                        }
                        else
                        {
                            query = $"SELECT EM.MACHINE_CODE,CASE WHEN CH.CHECKTYPE = 'I' THEN 'In' Else 'Out' End as CHECKTYPE," +
                          $"CONVERT(DATE, CH.CHECKTIME) AS CHK_DATE,FORMAT(CH.CHECKTIME, 'hh:mm:ss tt')  CHK_TIME," +
                          $"EM.EMP_ID,EM.ENAME,EM.FATHER_NAME,AG.DESCR AS DEP_NAME," +
                          $"SFT.GROUP_NAME AS SHIFT_T,EM.EMAIL,EM.CELL_NO,R.GROUP_NAME AS REG,EM.JOIN_DATE,EM.PARM_DATE,CASE WHEN EM.SALARY_HOLD = 'Y' THEN  'Freeze' Else 'Active' End AS SALARY_HOLD," +
                          $"CASE WHEN EM.MSTATUS = 'Y' THEN  'Management' else 'Non-Management' end  AS MSTATUS FROM {table} CH LEFT OUTER JOIN TBL_EMP_REG EM ON EM.MACHINE_CODE = CH.USERID " +
                          $"LEFT OUTER JOIN TBL_ACT_GROUP AG ON AG.CODE = EM.DEP_ID LEFT OUTER JOIN TBL_SHIFT SFT ON SFT.GROUP_CODE = EM.SHIFT_T LEFT OUTER JOIN TBL_RELIGION R ON R.GROUP_CODE = EM.REG " +
                      $"WHERE CH.ATT_DOWN = 1 AND CH.DLT = 'T' AND CONVERT(DATE, CH.CHECKTIME) BETWEEN '{fDate}' AND '{tDate}' AND CONVERT(time(0), CH.CHECKTIME) > '{time}' AND EM.EMP_CODE = {attendance.Employee}";
                        }

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                MACHINECODE = Convert.ToString(reader["MACHINE_CODE"]),
                                CHECKTYPE = Convert.ToString(reader["CHECKTYPE"]),
                                DATE = Convert.ToString(reader["CHK_DATE"]),
                                Time = Convert.ToString(reader["CHK_TIME"]),
                                EMPID = Convert.ToString(reader["EMP_ID"]),
                                ENAME = Convert.ToString(reader["ENAME"]),
                                FATHERNAME = Convert.ToString(reader["FATHER_NAME"]),
                                DEPNAME = Convert.ToString(reader["DEP_NAME"]),
                                SHIFTT = Convert.ToString(reader["SHIFT_T"]),
                                EMAIL = Convert.ToString(reader["EMAIL"]),
                                CELL_NO = Convert.ToString(reader["CELL_NO"]),
                                REG = Convert.ToString(reader["REG"]),
                                JOINDATE = Convert.ToString(reader["JOIN_DATE"]),
                                PARMDATE = Convert.ToString(reader["PARM_DATE"]),
                                SALARYHOLD = Convert.ToString(reader["SALARY_HOLD"]),
                                MSTATUS = Convert.ToString(reader["MSTATUS"]),
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

        public MyHttpResponseMessage DeleteattendanceByTime(string Time, string Date, Common common)
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
                    if (Time == "")
                    {
                        response.msg = "Time is not in assign";
                        response.msgType = 2;
                    }
                    else
                    {
                        DateTime parsedDate = DateTime.ParseExact(Date, "dd-MMM-yy hh:mm:ss tt", CultureInfo.InvariantCulture);
                        DateTime parsedTime = DateTime.ParseExact(Time, "hh:mm:ss tt", CultureInfo.InvariantCulture);

                        // Merge Date with new Time
                        DateTime finalDateTime = new DateTime(
                            parsedDate.Year,
                            parsedDate.Month,
                            parsedDate.Day,
                            parsedTime.Hour,
                            parsedTime.Minute,
                            parsedTime.Second
                        );

                        // Convert to required format
                        string DateNTime = finalDateTime.ToString("dd-MMM-yy hh:mm:ss tt");
                        string connectionString = new SQLService().getconnstring();
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE CHECKTIME = '" + DateNTime + @"'";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            if (query != null)
                            {
                                response.msg = "Record Deleted Successfully";
                                response.msgType = 1;
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
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }
    }
}