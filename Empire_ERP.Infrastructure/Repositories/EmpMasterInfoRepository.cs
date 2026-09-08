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
    public class EmpMasterInfoRepository : IEmpMasterInfoRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public EmpMasterInfoRepository(IMenuRepository menuRepository)
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
                        if (TableName == "TBL_WORK_EXP")
                        {
                            query = "SELECT GROUP_CODE, START_D , END_D , COMPANY_NAME , DESC_S , DESC_E , INITAL_SALARY , FINAL_SALARY , CELL_NO , LREASON , WADD , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";

                            //query = "SELECT GROUP_CODE,GROUP_NAME,QTY,ADD_USER_ID," +
                            //           "ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                            //           "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                            //           "EDIT_POSTALCODE,CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                            //           "FROM " + table + " " +
                            //           "WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' ORDER BY GROUP_CODE DESC";
                        }
                        if (TableName == "TBL_EMP_EDUCATION")
                        {
                            query = @"SELECT GROUP_CODE,[EDUCATION_ID]
                                      ,[MAJOR_SUBJECT]
                                      ,[E_YEAR]
                                      ,[GROUP_BATCH]
                                      ,[INSTITUTE]
                                      ,[GRADE_CGPA] , [DOC], CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }
                        if (TableName == "TBL_EMP_WORKSHOP")
                        {
                            query = @"SELECT GROUP_CODE, [W_DATE]
                                      ,[TITLE]
                                      ,[REMARKS], [DOC] , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }
                        if (TableName == "TBL_EMP_SALARY_HISTORY")
                        {
                            query = @"SELECT GROUP_CODE,[BASIC_SALARY]
                                      ,[HOUSE_RENT]
                                      ,[UTILITY]
                                      ,[COLA]
                                      ,[EFF_DATE]
                                      ,[REMARKS] , [DOC] , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       "WHERE DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (TableName == "TBL_WORK_EXP")
                            {
                                var row = new EmpMasterInfo
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    START_D = Convert.ToDateTime(reader["START_D"]).ToShortDateString(),
                                    END_D = Convert.ToDateTime(reader["END_D"]).ToShortDateString(),
                                    CompanyName = Convert.ToString(reader["COMPANY_NAME"]),
                                    DESC_S = Convert.ToString(reader["DESC_S"]),
                                    DESC_E = Convert.ToString(reader["DESC_E"]),
                                    InitialSalary = reader["INITAL_SALARY"] == null ? 0 : Convert.ToInt32(reader["INITAL_SALARY"]),
                                    FinalSalary = reader["FINAL_SALARY"] == null ? 0 : Convert.ToInt32(reader["FINAL_SALARY"]),
                                    CellNo = Convert.ToString(reader["CELL_NO"]),
                                    WADD = Convert.ToString(reader["WADD"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_EMP_EDUCATION")
                            {
                                var row = new EmpMasterInfo
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    EducationID = reader["EDUCATION_ID"] == null ? 0 : Convert.ToInt32(reader["EDUCATION_ID"]),
                                    MSubject = Convert.ToString(reader["MAJOR_SUBJECT"]),
                                    EYear = Convert.ToString(reader["E_YEAR"]),
                                    GBatch = Convert.ToString(reader["GROUP_BATCH"]),
                                    Institute = Convert.ToString(reader["INSTITUTE"]),
                                    GCGPA = Convert.ToString(reader["GRADE_CGPA"]),
                                    EduDoc = Convert.ToString(reader["DOC"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_EMP_WORKSHOP")
                            {
                                var row = new EmpMasterInfo
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    WDate = Convert.ToDateTime(reader["W_DATE"]).ToShortDateString(),
                                    Title = Convert.ToString(reader["TITLE"]),
                                    WorkRemark = Convert.ToString(reader["REMARKS"]),
                                    WorkDoc = Convert.ToString(reader["DOC"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"])
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_EMP_SALARY_HISTORY")
                            {
                                var row = new EmpMasterInfo
                                {
                                    GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                    BasicSalary = reader["BASIC_SALARY"] == null ? 0 : Convert.ToInt32(reader["BASIC_SALARY"]),
                                    HouseRent = reader["HOUSE_RENT"] == null ? 0 : Convert.ToInt32(reader["HOUSE_RENT"]),
                                    Utility = Convert.ToString(reader["UTILITY"]),
                                    Cola = Convert.ToString(reader["COLA"]),
                                    EFFDate = Convert.ToString(reader["EFF_DATE"]),
                                    EmpRemark = Convert.ToString(reader["REMARKS"]),
                                    EmpDoc = Convert.ToString(reader["DOC"]),
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

        public MyHttpResponseMessage Save(EmpMasterInfo modelRecord, Common common)
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
                                if (modelRecord.TableName == "TBL_WORK_EXP")
                                {
                                    query = $"INSERT INTO {modelRecord.TableName} " +
                                           "(GROUP_CODE,START_D , END_D , COMPANY_NAME , DESC_S , DESC_E , INITAL_SALARY , FINAL_SALARY , CELL_NO , LREASON " +
                                           ", WADD,ADD_USER_ID,ADD_DATE," +
                                           "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                           "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                           "ADD_COMPUTER_NAME,MENU_ID,DLT,MASTER_ID,EMP_ID)" +
                                           "VALUES" +
                                           "('" + GenerateNextId(common, modelRecord.TableName) + "','" + modelRecord.START_D + "','" + modelRecord.END_D + "','" + modelRecord.CompanyName + "','" + modelRecord.DESC_S + "'" +
                                           ",'" + modelRecord.DESC_E + "','" + modelRecord.InitialSalary + "','" + modelRecord.FinalSalary + "','" + modelRecord.CellNo + "','" + modelRecord.LREASON + "','" + modelRecord.WADD + "'," +
                                           "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                           "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                           "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                           "'" + Computer + "','" + common.MenuID + $"','T',{modelRecord.MasterId},{modelRecord.EMP_ID})";
                                }
                                else if (modelRecord.TableName == "TBL_EMP_EDUCATION")
                                {
                                    query = $"INSERT INTO {modelRecord.TableName} " +
                                           "(GROUP_CODE,EDUCATION_ID,MAJOR_SUBJECT , E_YEAR , GROUP_BATCH , INSTITUTE , GRADE_CGPA ,ADD_USER_ID,ADD_DATE," +
                                           "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                           "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                           "ADD_COMPUTER_NAME,MENU_ID,DLT,MASTER_ID,EMP_ID)" +
                                           "VALUES" +
                                           "('" + GenerateNextId(common, modelRecord.TableName) + "','" + modelRecord.EducationID + "','" + modelRecord.MSubject + "'" +
                                           ",'" + modelRecord.EYear + "','" + modelRecord.GBatch + "','" + modelRecord.Institute + "','" + modelRecord.GCGPA + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                           "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                           "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                           "'" + Computer + "','" + common.MenuID + $"','T',{modelRecord.MasterId},{modelRecord.EMP_ID})";
                                }
                                else if (modelRecord.TableName == "TBL_EMP_WORKSHOP")
                                {
                                    query = $"INSERT INTO {modelRecord.TableName} " +
                                           "(GROUP_CODE,W_DATE,TITLE,REMARKS,ADD_USER_ID,ADD_DATE," +
                                           "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                           "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                           "ADD_COMPUTER_NAME,MENU_ID,DLT,MASTER_ID,EMP_ID)" +
                                           "VALUES" +
                                           "('" + GenerateNextId(common, modelRecord.TableName) + "','" + modelRecord.WDate + "','" + modelRecord.Title + "','" + modelRecord.WorkRemark + "','"
                                           + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                           "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                           "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                           "'" + Computer + "','" + common.MenuID + $"','T',{modelRecord.MasterId},{modelRecord.EMP_ID})";
                                }
                                else if (modelRecord.TableName == "TBL_EMP_SALARY_HISTORY")
                                {
                                    query = $"INSERT INTO {modelRecord.TableName} " +
                                           "(GROUP_CODE,BASIC_SALARY,HOUSE_RENT,UTILITY,COLA,EFF_DATE,REMARKS,ADD_USER_ID,ADD_DATE," +
                                           "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                           "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                           "ADD_COMPUTER_NAME,MENU_ID,DLT,MASTER_ID,EMP_ID)" +
                                           "VALUES" +
                                           "('" + GenerateNextId(common, modelRecord.TableName) + "','" + modelRecord.BasicSalary + "','" + modelRecord.HouseRent + "','" + modelRecord.Utility + "','" + modelRecord.Cola + "'," +
                                           "'" + modelRecord.EFFDate + "','" + modelRecord.EmpRemark + "','"
                                           + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                           "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                           "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                           "'" + Computer + "','" + common.MenuID + $"','T',{modelRecord.MasterId},{modelRecord.EMP_ID})";
                                }

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                IsInsert = true;
                            }
                            else
                            {
                                if (modelRecord.TableName == "TBL_WORK_EXP")
                                {
                                    query = $"UPDATE {modelRecord.TableName} SET " +
                                            $"START_D = '{modelRecord.START_D}', " +
                                            $"END_D = '{modelRecord.END_D}', " +
                                            $"COMPANY_NAME = '{modelRecord.CompanyName}', " +
                                            $"DESC_S = '{modelRecord.DESC_S}', " +
                                            $"DESC_E = '{modelRecord.DESC_E}', " +
                                            $"INITAL_SALARY = '{modelRecord.InitialSalary}', " +
                                            $"FINAL_SALARY = '{modelRecord.FinalSalary}', " +
                                            $"CELL_NO = '{modelRecord.CellNo}', " +
                                            $"LREASON = '{modelRecord.LREASON}', " +
                                            $"WADD = '{modelRecord.WADD}', " +
                                            $"EDIT_USER_ID = '{userid}', " +
                                            $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                            $"EDIT_COMPUTER_NAME = '{Computer}', " +
                                            $"EDIT_IP_ADDRESS = '{Ip}', " +
                                            $"EDIT_POSTALCODE = '{Postal}', " +
                                            $"ASTATUS = '{modelRecord.ASTATUS}' " +
                                            $"WHERE GROUP_CODE = '{modelRecord.GROUP_CODE}'";
                                }
                                else if (modelRecord.TableName == "TBL_EMP_EDUCATION")
                                {
                                    query = $"UPDATE {modelRecord.TableName} SET " +
                                            $"EDUCATION_ID = '{modelRecord.EducationID}', " +
                                            $"MAJOR_SUBJECT = '{modelRecord.MSubject}', " +
                                            $"E_YEAR = '{modelRecord.EYear}', " +
                                            $"GROUP_BATCH = '{modelRecord.GBatch}', " +
                                            $"INSTITUTE = '{modelRecord.Institute}', " +
                                            $"GRADE_CGPA = '{modelRecord.GCGPA}', " +
                                            $"EDIT_USER_ID = '{userid}', " +
                                            $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                            $"EDIT_COMPUTER_NAME = '{Computer}', " +
                                            $"EDIT_IP_ADDRESS = '{Ip}', " +
                                            $"EDIT_POSTALCODE = '{Postal}', " +
                                            $"ASTATUS = '{modelRecord.ASTATUS}' " +
                                            $"WHERE GROUP_CODE = '{modelRecord.GROUP_CODE}'";
                                }
                                else if (modelRecord.TableName == "TBL_EMP_WORKSHOP")
                                {
                                    query = $"UPDATE {modelRecord.TableName} SET " +
                                            $"W_DATE = '{modelRecord.WDate}', " +
                                            $"TITLE = '{modelRecord.Title}', " +
                                            $"REMARKS = '{modelRecord.WorkRemark}', " +
                                            $"EDIT_USER_ID = '{userid}', " +
                                            $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                            $"EDIT_COMPUTER_NAME = '{Computer}', " +
                                            $"EDIT_IP_ADDRESS = '{Ip}', " +
                                            $"EDIT_POSTALCODE = '{Postal}', " +
                                            $"ASTATUS = '{modelRecord.ASTATUS}' " +
                                            $"WHERE GROUP_CODE = '{modelRecord.GROUP_CODE}'";
                                }
                                else if (modelRecord.TableName == "TBL_EMP_SALARY_HISTORY")
                                {
                                    query = $"UPDATE {modelRecord.TableName} SET " +
                                            $"BASIC_SALARY = '{modelRecord.BasicSalary}', " +
                                            $"HOUSE_RENT = '{modelRecord.HouseRent}', " +
                                            $"UTILITY = '{modelRecord.Utility}', " +
                                            $"COLA = '{modelRecord.Cola}', " +
                                            $"EFF_DATE = '{modelRecord.EFFDate}', " +
                                            $"REMARKS = '{modelRecord.EmpRemark}', " +
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

        public MyHttpResponseMessage GetEmpMasterInfoById(int id, string TableName, Common common)
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
                        if (TableName == "TBL_WORK_EXP")
                        {
                            query = @"SELECT [GROUP_CODE]
                                      ,[START_D]
                                      ,[END_D]
                                      ,[COMPANY_NAME]
                                      ,[DESC_S]
                                      ,[DESC_E]
                                      ,[INITAL_SALARY]
                                      ,[FINAL_SALARY]
                                      ,[CELL_NO]
                                      ,[LREASON]
                                      ,[WADD] , EMP_ID , CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       $"WHERE GROUP_CODE = {id} AND DLT = 'T' ORDER BY GROUP_CODE ASC";

                        }
                        if (TableName == "TBL_EMP_EDUCATION")
                        {
                            query = @"SELECT [GROUP_CODE]
                                  ,[EDUCATION_ID]
                                  ,[MAJOR_SUBJECT]
                                  ,[E_YEAR]
                                  ,[GROUP_BATCH]
                                  ,[INSTITUTE]
                                  ,[GRADE_CGPA],EMP_ID, CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       $"WHERE GROUP_CODE = {id} AND DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }
                        if (TableName == "TBL_EMP_WORKSHOP")
                        {
                            query = @"SELECT [GROUP_CODE]
                                      ,[W_DATE]
                                      ,[TITLE]
                                      ,[REMARKS] , EMP_ID, CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       $"WHERE GROUP_CODE = {id} AND DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }
                        if (TableName == "TBL_EMP_SALARY_HISTORY")
                        {
                            query = @"SELECT [GROUP_CODE]
                                      ,[BASIC_SALARY]
                                      ,[HOUSE_RENT]
                                      ,[UTILITY]
                                      ,[COLA]
                                      ,[EFF_DATE]
                                      ,[REMARKS] ,EMP_ID ,CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       $"FROM {TableName} " +
                                       $"WHERE GROUP_CODE = {id} AND DLT = 'T' ORDER BY GROUP_CODE ASC";
                        }


                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            if (TableName == "TBL_WORK_EXP")
                            {
                                var row = new EmpMasterInfo
                                {
                                    GROUP_CODE = reader["GROUP_CODE"] != DBNull.Value ? Convert.ToInt32(reader["GROUP_CODE"]) : (int?)null,
                                    START_D = reader["START_D"] != DBNull.Value ? Convert.ToDateTime(reader["START_D"]).ToString("yyyy-MM-dd") : null,
                                    END_D = reader["END_D"] != DBNull.Value ? Convert.ToDateTime(reader["END_D"]).ToString("yyyy-MM-dd") : null,
                                    CompanyName = reader["COMPANY_NAME"] != DBNull.Value ? Convert.ToString(reader["COMPANY_NAME"]) : null,
                                    DESC_S = reader["DESC_S"] != DBNull.Value ? Convert.ToString(reader["DESC_S"]) : null,
                                    DESC_E = reader["DESC_E"] != DBNull.Value ? Convert.ToString(reader["DESC_E"]) : null,
                                    InitialSalary = reader["INITAL_SALARY"] != DBNull.Value ? Convert.ToInt32(reader["INITAL_SALARY"]) : (int?)null,
                                    FinalSalary = reader["FINAL_SALARY"] != DBNull.Value ? Convert.ToInt32(reader["FINAL_SALARY"]) : (int?)null,
                                    CellNo = reader["CELL_NO"] != DBNull.Value ? Convert.ToString(reader["CELL_NO"]) : null,
                                    LREASON = reader["LREASON"] != DBNull.Value ? Convert.ToString(reader["LREASON"]) : null,
                                    WADD = reader["WADD"] != DBNull.Value ? Convert.ToString(reader["WADD"]) : null,
                                    ASTATUS = reader["ASTATUS"] != DBNull.Value ? Convert.ToString(reader["ASTATUS"]) : null,
                                    EMP_ID = reader["EMP_ID"] != DBNull.Value ? Convert.ToInt32(reader["EMP_ID"]) : null
                                };

                                jsonDataResult.Add(row);

                            }
                            if (TableName == "TBL_EMP_EDUCATION")
                            {
                                var row = new EmpMasterInfo
                                {
                                    GROUP_CODE = reader["GROUP_CODE"] != DBNull.Value ? Convert.ToInt32(reader["GROUP_CODE"]) : (int?)null,
                                    EducationID = reader["EDUCATION_ID"] != DBNull.Value ? Convert.ToInt32(reader["EDUCATION_ID"]) : (int?)null,
                                    MSubject = reader["MAJOR_SUBJECT"] != DBNull.Value ? Convert.ToString(reader["MAJOR_SUBJECT"]) : null,
                                    EYear = reader["E_YEAR"] != DBNull.Value ? Convert.ToString(reader["E_YEAR"]) : null,
                                    GBatch = reader["GROUP_BATCH"] != DBNull.Value ? Convert.ToString(reader["GROUP_BATCH"]) : null,
                                    Institute = reader["INSTITUTE"] != DBNull.Value ? Convert.ToString(reader["INSTITUTE"]) : null,
                                    GCGPA = reader["GRADE_CGPA"] != DBNull.Value ? Convert.ToString(reader["GRADE_CGPA"]) : null,
                                    ASTATUS = reader["ASTATUS"] != DBNull.Value ? Convert.ToString(reader["ASTATUS"]) : null,
                                    EMP_ID = reader["EMP_ID"] != DBNull.Value ? Convert.ToInt32(reader["EMP_ID"]) : null
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_EMP_WORKSHOP")
                            {
                                var row = new EmpMasterInfo
                                {
                                    GROUP_CODE = reader["GROUP_CODE"] != DBNull.Value ? Convert.ToInt32(reader["GROUP_CODE"]) : (int?)null,
                                    WDate = reader["W_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["W_DATE"]).ToString("yyyy-MM-dd") : null,
                                    Title = reader["TITLE"] != DBNull.Value ? Convert.ToString(reader["TITLE"]) : null,
                                    WorkRemark = reader["REMARKS"] != DBNull.Value ? Convert.ToString(reader["REMARKS"]) : null,
                                    ASTATUS = reader["ASTATUS"] != DBNull.Value ? Convert.ToString(reader["ASTATUS"]) : null,
                                    EMP_ID = reader["EMP_ID"] != DBNull.Value ? Convert.ToInt32(reader["EMP_ID"]) : null
                                };

                                jsonDataResult.Add(row);
                            }
                            if (TableName == "TBL_EMP_SALARY_HISTORY")
                            {
                                var row = new EmpMasterInfo
                                {
                                    GROUP_CODE = reader["GROUP_CODE"] != DBNull.Value ? Convert.ToInt32(reader["GROUP_CODE"]) : (int?)null,
                                    BasicSalary = reader["BASIC_SALARY"] != DBNull.Value ? Convert.ToInt32(reader["BASIC_SALARY"]) : (int?)null,
                                    HouseRent = reader["HOUSE_RENT"] != DBNull.Value ? Convert.ToInt32(reader["HOUSE_RENT"]) : (int?)null,
                                    Utility = reader["UTILITY"] != DBNull.Value ? Convert.ToString(reader["UTILITY"]) : null,
                                    Cola = reader["COLA"] != DBNull.Value ? Convert.ToString(reader["COLA"]) : null,
                                    EFFDate = reader["EFF_DATE"] != DBNull.Value ? Convert.ToDateTime(reader["EFF_DATE"]).ToString("yyyy-MM-dd") : null,
                                    EmpRemark = reader["REMARKS"] != DBNull.Value ? Convert.ToString(reader["REMARKS"]) : null,
                                    ASTATUS = reader["ASTATUS"] != DBNull.Value ? Convert.ToString(reader["ASTATUS"]) : null,
                                    EMP_ID = reader["EMP_ID"] != DBNull.Value ? Convert.ToInt32(reader["EMP_ID"]) : null
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