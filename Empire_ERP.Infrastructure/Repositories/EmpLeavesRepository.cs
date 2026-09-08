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
    public class EmpLeavesRepository : IEmpLeavesRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public EmpLeavesRepository(IMenuRepository menuRepository)
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

                //if (!String.IsNullOrWhiteSpace(TableName))
                //{
                //    List<object> jsonDataResult = new List<object>();
                //    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                //    {
                //        if (TableName == "TBL_WORK_EXP")
                //        {
                //            query = "SELECT GROUP_CODE, START_D , END_D , COMPANY_NAME , DESC_S , DESC_E , INITAL_SALARY , FINAL_SALARY , CELL_NO , LREASON , WADD , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                //                       $"FROM {TableName} " +
                //                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";

                //            //query = "SELECT GROUP_CODE,GROUP_NAME,QTY,ADD_USER_ID," +
                //            //           "ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                //            //           "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                //            //           "EDIT_POSTALCODE,CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                //            //           "FROM " + table + " " +
                //            //           "WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' ORDER BY GROUP_CODE DESC";
                //        }
                //        if (TableName == "TBL_EMP_EDUCATION")
                //        {
                //            query = @"SELECT GROUP_CODE,[EDUCATION_ID]
                //                      ,[MAJOR_SUBJECT]
                //                      ,[E_YEAR]
                //                      ,[GROUP_BATCH]
                //                      ,[INSTITUTE]
                //                      ,[GRADE_CGPA] , [DOC], CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                //                       $"FROM {TableName} " +
                //                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";
                //        }
                //        if (TableName == "TBL_EMP_WORKSHOP")
                //        {
                //            query = @"SELECT GROUP_CODE, [W_DATE]
                //                      ,[TITLE]
                //                      ,[REMARKS], [DOC] , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                //                       $"FROM {TableName} " +
                //                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";
                //        }
                //        if (TableName == "TBL_EMP_SALARY_HISTORY")
                //        {
                //            query = @"SELECT GROUP_CODE,[BASIC_SALARY]
                //                      ,[HOUSE_RENT]
                //                      ,[UTILITY]
                //                      ,[COLA]
                //                      ,[EFF_DATE]
                //                      ,[REMARKS] , [DOC] , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                //                       $"FROM {TableName} " +
                //                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";
                //        }

                //        SqlCommand command = new SqlCommand(query, connection);
                //        connection.Open();
                //        SqlDataReader reader = command.ExecuteReader();
                //        while (reader.Read())
                //        {
                //            if (TableName == "TBL_WORK_EXP")
                //            {
                //                var row = new EmpLeaves
                //                {
                //                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                //                    START_D = Convert.ToDateTime(reader["START_D"]).ToShortDateString(),
                //                    END_D = Convert.ToDateTime(reader["END_D"]).ToShortDateString(),
                //                    CompanyName = Convert.ToString(reader["COMPANY_NAME"]),
                //                    DESC_S = Convert.ToString(reader["DESC_S"]),
                //                    DESC_E = Convert.ToString(reader["DESC_E"]),
                //                    InitialSalary = reader["INITAL_SALARY"] == null ? 0 : Convert.ToInt32(reader["INITAL_SALARY"]),
                //                    FinalSalary = reader["FINAL_SALARY"] == null ? 0 : Convert.ToInt32(reader["FINAL_SALARY"]),
                //                    CellNo = Convert.ToString(reader["CELL_NO"]),
                //                    WADD = Convert.ToString(reader["WADD"]),
                //                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                //                };

                //                jsonDataResult.Add(row);
                //            }
                //            if (TableName == "TBL_EMP_EDUCATION")
                //            {
                //                var row = new EmpLeaves
                //                {
                //                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                //                    EducationID = reader["EDUCATION_ID"] == null ? 0 : Convert.ToInt32(reader["EDUCATION_ID"]),
                //                    MSubject = Convert.ToString(reader["MAJOR_SUBJECT"]),
                //                    EYear = Convert.ToString(reader["E_YEAR"]),
                //                    GBatch = Convert.ToString(reader["GROUP_BATCH"]),
                //                    Institute = Convert.ToString(reader["INSTITUTE"]),
                //                    GCGPA = Convert.ToString(reader["GRADE_CGPA"]),
                //                    EduDoc = Convert.ToString(reader["DOC"]),
                //                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                //                };

                //                jsonDataResult.Add(row);
                //            }
                //            if (TableName == "TBL_EMP_WORKSHOP")
                //            {
                //                var row = new EmpLeaves
                //                {
                //                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                //                    WDate = Convert.ToDateTime(reader["W_DATE"]).ToShortDateString(),
                //                    Title = Convert.ToString(reader["TITLE"]),
                //                    WorkRemark = Convert.ToString(reader["REMARKS"]),
                //                    WorkDoc = Convert.ToString(reader["DOC"]),
                //                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                //                };

                //                jsonDataResult.Add(row);
                //            }
                //            if (TableName == "TBL_EMP_SALARY_HISTORY")
                //            {
                //                var row = new EmpLeaves
                //                {
                //                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                //                    BasicSalary = reader["BASIC_SALARY"] == null ? 0 : Convert.ToInt32(reader["BASIC_SALARY"]),
                //                    HouseRent = reader["HOUSE_RENT"] == null ? 0 : Convert.ToInt32(reader["HOUSE_RENT"]),
                //                    Utility = Convert.ToString(reader["UTILITY"]),
                //                    Cola = Convert.ToString(reader["COLA"]),
                //                    EFFDate = Convert.ToString(reader["EFF_DATE"]),
                //                    EmpRemark = Convert.ToString(reader["REMARKS"]),
                //                    EmpDoc = Convert.ToString(reader["DOC"]),
                //                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                //                };

                //                jsonDataResult.Add(row);
                //            }
                //        }
                //        reader.Close();

                //        response.data = jsonDataResult;
                //        response.msg = "";
                //        response.msgType = 1;
                //    }
                //}
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

        public MyHttpResponseMessage Save(EmpLeaves modelRecord, Common common)
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
                                    if (item.DT_CODE == null || item.DT_CODE == 0)
                                    {
                                        int detailCode = GenerateNextDetailId(common, command);
                                        query = $@"INSERT INTO {table} 
                                                            ([TRAN_ID], [V_DATE], [LEAVE_TYPE], [PURPOSE], [LFROM], [LTO], [NOL], [BCODE], 
                                                             [PERIOD_ID], [ADD_USER_ID], [ADD_DATE], [ADD_COMPUTER_NAME], [ADD_IP_ADDRESS], [EDIT_USER_ID], 
                                                             [EDIT_DATE], [EDIT_COMPUTER_NAME], [EDIT_IP_ADDRESS], [ADD_POSTALCODE], [EDIT_POSTALCODE], 
                                                             [MENU_ID], [DLT], [EMPID], [DOC]) 
                                                            VALUES ({detailCode}, '{item.Date}', '{item.LEAVE_TYPE}', '{item.PURPOSE}', '{item.LFrom}', 
                                                                    '{item.LTo}', '{item.NOL}', '{branch}', '{period}', 
                                                                    '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', 
                                                                    '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', 
                                                                    '{Postal}', '{Postal}', '{menuID}', 'T', {item.Emp_ID}, '{item.DOC}')";
                                        command.CommandText = query;
                                        command.ExecuteNonQuery();

                                        IsInsert = true;

                                    }
                                    else
                                    {
                                        query = $@"
                                                    UPDATE {table} SET 
                                                        [V_DATE] = '{item.Date}',
                                                        [LEAVE_TYPE] = '{item.LEAVE_TYPE}',
                                                        [PURPOSE] = '{item.PURPOSE}',
                                                        [LFROM] = '{item.LFrom}',
                                                        [LTO] = '{item.LTo}',
                                                        [NOL] = '{item.NOL}',
                                                        [BCODE] = '{branch}',
                                                        [PERIOD_ID] = '{period}',
                                                        [EDIT_USER_ID] = '{username}',
                                                        [EDIT_DATE] = '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                        [EDIT_COMPUTER_NAME] = '{Computer}',
                                                        [EDIT_IP_ADDRESS] = '{Ip}',
                                                        [EDIT_POSTALCODE] = '{Postal}',
                                                        [MENU_ID] = '{menuID}',
                                                        [EMPID] = {item.Emp_ID},
                                                        [DOC] = '{item.DOC}'
                                                    WHERE 
                                                        [TRAN_ID] = {item.DT_CODE}";

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

        public MyHttpResponseMessage GetEmpLeavesRecord(int id, Common common)
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
                        query = @"SELECT [TRAN_ID]
                                  ,[V_DATE]
                                  ,[LEAVE_TYPE]
                                  ,[PURPOSE]
                                  ,[LFROM]
                                  ,[LTO]
                                  ,[NOL] , [DOC]" +
                                      $"FROM {table} " +
                                      $"WHERE EmpId = {id} AND DLT = 'T' ORDER BY EmpId DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToInt32(reader["TRAN_ID"]) != 0 ? Convert.ToInt32(reader["TRAN_ID"]) : 0,
                                DATE = reader["V_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd") : "",
                                LFROM = reader["LFROM"] != DBNull.Value ? Convert.ToDateTime(reader["LFROM"]).ToString("yyyy-MM-dd") : "",
                                LTO = reader["LTO"] != DBNull.Value ? Convert.ToDateTime(reader["LTO"]).ToString("yyyy-MM-dd") : "",   // Fix here: was V_DLTOATE and incorrect column
                                LEAVE_TYPE = Convert.ToInt32(reader["LEAVE_TYPE"]) != 0 ? Convert.ToInt32(reader["LEAVE_TYPE"]) : 0,
                                PURPOSE = reader["PURPOSE"] != DBNull.Value ? Convert.ToString(reader["PURPOSE"]) : "",
                                NOL = Convert.ToInt32(reader["NOL"]) != 0 ? Convert.ToInt32(reader["NOL"]) : 0  ,// added (int?)null for nullable int
                                DOC = Convert.ToString(reader["DOC"]) != "" ? Convert.ToString(reader["DOC"]) : ""  // added (int?)null for nullable int
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
    }

}