using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class POSUserRightsRepository : IPOSUserRightsRepository
    {

        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public IMenuRepository _menuRepository { get; set; }
        public POSUserRightsRepository(IBranchRepository branchRepository, ICommonRepository commonRepository, IMenuRepository menuRepository)
        {
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
            _menuRepository = menuRepository;
        }
     

        public MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = string.Empty;
                int? pType;
                var menu = (Menu)Menu.data;
                table = menu.TABLE1;
                pType = menu.PTYPE;
                List<object> jsonDataResult = new List<object>();

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {

                    string query = $@"SELECT CODE,GROUP_NAME,USERNAME FROM {table} WHERE DLT='T'  ORDER BY CODE DESC";



                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            GROUP_CODE = Convert.ToString(reader["CODE"]),
                            USERNAME = Convert.ToString(reader["USERNAME"]),
                            GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),


                        };
                        jsonDataResult.Add(row);
                    }

                    reader.Close();
                }

                response.data = jsonDataResult;
                response.msg = "";
                response.msgType = 1;
                return response;
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(CODE), 0) + 1 FROM {table}";
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
        public MyHttpResponseMessage Save(POSUserright modelRecord,Common common)
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
                    int? code = 0;
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
                            string Duplicationquery ="";
                    
                           
                            if (modelRecord.CODE is 0 || modelRecord.CODE is null)
                            {
                                Duplicationquery = $@"SELECT COUNT(1) FROM {table} WHERE DLT='T' AND USERNAME='{modelRecord.USERNAME}'";
                                command.CommandText = Duplicationquery;
                                int existingCount = (int)command.ExecuteScalar();
                                if (existingCount > 0)
                                {
                                    response.msg = "Username already exists!";
                                    response.msgType = 2;
                                    return response;
                                }

                                code = GenerateNextId(common,command);
                                modelRecord.CODE = code;

                                query = "INSERT INTO " + table + " (" +
                                          "CODE, GROUP_NAME, USERNAME, RATE, M_DISC, D_DISC, B_RETURN, I_RETURN, " +
                                          "SETT_F, SETT_T, SERVICE_CHARGES, CARD_DISCOUNT, EXPENSES, COMM, " +
                                          "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, ADD_POSTALCODE, " +
                                          "EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, EDIT_POSTALCODE, " +
                                          "MENU_ID, ASTATUS, DLT) VALUES (" +
                                          "'" + code + "'," +
                                          "'" + modelRecord.G_NAME + "'," +
                                          "'" + modelRecord.USERNAME + "'," +
                                          "'" + modelRecord.RATE + "'," +
                                          "'" + modelRecord.M_DISC + "'," +
                                          "'" + modelRecord.D_DISC + "'," +
                                          "'" + modelRecord.B_RETURN + "'," +
                                          "'" + modelRecord.I_RETURN + "'," +
                                          "'" + modelRecord.SETT_F + "'," +
                                          "'" + modelRecord.SETT_T + "'," +
                                          "'" + modelRecord.S_CHARGES + "'," +
                                          "'" + modelRecord.C_DISC + "'," +
                                          "'" + modelRecord.EXPENSE + "'," +
                                          "'" + modelRecord.COMM + "'," +

                                          "'" + userid + "'," +
                                          "'" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                          "'" + Computer + "'," +
                                          "'" + Ip + "'," +
                                          "'" + Postal + "'," +

                                          "'" + userid + "'," +
                                          "'" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                          "'" + Computer + "'," +
                                          "'" + Ip + "'," +
                                          "'" + Postal + "'," +

                                          "'" + common.MenuID + "'," +
                                          "'Y','T')";


                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                response.data = code;
                                response.msgType = 1;
                                response.msg = "Record Added Successfully";

                            }
                            else
                            {
                                Duplicationquery = $@"SELECT COUNT(1) FROM {table} WHERE DLT='T' AND USERNAME='{modelRecord.USERNAME}' AND CODE <> {modelRecord.CODE}";
                                command.CommandText = Duplicationquery;
                                int existingCount = (int)command.ExecuteScalar();
                                if (existingCount > 0)
                                {
                                    response.msg = "Username already exists!";
                                    response.msgType = 2;
                                    return response;
                                }
                                query = "UPDATE " + table + " SET " +
                                 "[GROUP_NAME] = '" + modelRecord.G_NAME + "', " +
                                 "[USERNAME] = '" + modelRecord.USERNAME + "', " +
                                 "[RATE] = '" + modelRecord.RATE + "', " +
                                 "[M_DISC] = '" + modelRecord.M_DISC + "', " +
                                 "[D_DISC] = '" + modelRecord.D_DISC + "', " +
                                 "[B_RETURN] = '" + modelRecord.B_RETURN + "', " +
                                 "[I_RETURN] = '" + modelRecord.I_RETURN + "', " +
                                 "[SETT_F] = '" + modelRecord.SETT_F + "', " +
                                 "[SETT_T] = '" + modelRecord.SETT_T + "', " +
                                 "[SERVICE_CHARGES] = '" + modelRecord.S_CHARGES + "', " +
                                 "[CARD_DISCOUNT] = '" + modelRecord.C_DISC + "', " +
                                 "[EXPENSES] = '" + modelRecord.EXPENSE + "', " +
                                 "[COMM] = '" + modelRecord.COMM + "', " +
                                 "[EDIT_USER_ID] = '" + userid + "', " +
                                 "[EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + "', " +
                                 "[EDIT_COMPUTER_NAME] = '" + Computer + "', " +
                                 "[EDIT_IP_ADDRESS] = '" + Ip + "', " +
                                 "[EDIT_POSTALCODE] = '" + Postal + "', " +
                                 "[MENU_ID] = '" + common.MenuID + "', " +
                                 "[ASTATUS] = 'Y', " +
                                 "[DLT] = 'T' " +
                                 "WHERE [CODE] = '" + modelRecord.CODE + "'";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                response.data = modelRecord.CODE;
                                response.msgType = 1;
                                response.msg = "Record Updated Successfully";
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



        public MyHttpResponseMessage GetPosUserByCode(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $@"SELECT CODE, GROUP_NAME, USERNAME,RATE,D_DISC,M_DISC,B_RETURN,I_RETURN,SETT_F,SETT_T,SERVICE_CHARGES,CARD_DISCOUNT,EXPENSES,COMM FROM {table}
                                    WHERE DLT='T' AND CODE={code}";

                    

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            CODE = Convert.ToInt32(reader["CODE"]),
                            G_NAME = Convert.ToString(reader["GROUP_NAME"]),
                            USERNAME = Convert.ToString(reader["USERNAME"]),
                            RATE = Convert.ToString(reader["RATE"]),
                            D_DISC = Convert.ToString(reader["D_DISC"]),
                            M_DISC = Convert.ToString(reader["M_DISC"]),
                            B_RETURN = Convert.ToString(reader["B_RETURN"]),
                            I_RETURN = Convert.ToString(reader["I_RETURN"]),
                            SETT_F = Convert.ToInt32(reader["SETT_F"]),
                            SETT_T = Convert.ToInt32(reader["SETT_T"]),
                            S_CHARGES = Convert.ToString(reader["SERVICE_CHARGES"]),
                            C_DISC = Convert.ToString(reader["CARD_DISCOUNT"]),
                            COMM = Convert.ToString(reader["COMM"]),
                            EXPENSES = Convert.ToString(reader["EXPENSES"]),

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

      

        public MyHttpResponseMessage Delete(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string? table = menu.TABLE1;
                string connectionString = new SQLService().getconnstring();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE {table} SET DLT = 'F' WHERE GROUP_CODE = '{code}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    response.msgType = 1;
                    response.msg = "Record Deleted Successfully";
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
     
        public MyHttpResponseMessage DeleteCommisionMapDetailByCode(int gcode, int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                var branch = common.Branch;
                var period = common.Period;
                string connectionString = new SQLService().getconnstring();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //// Step 1: Check if related data exists in TBL_CC_DETAIL
                    //string checkQuery = $"SELECT COUNT(*) FROM {table} WHERE GROUP_CODE = '{gcode}' AND DT_CODE = '{code}' AND DLT = 'T'";
                    //SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    //int relatedCount = (int)checkCommand.ExecuteScalar();

                    //if (relatedCount > 0)
                    //{
                    //    response.msgType = 2;
                    //    response.msg = "Record cannot be deleted. Related data exists in Cost Center.";
                    //    return response;
                    //}

                    // Step 2: Perform soft delete
                    string query = $"UPDATE {table} SET DLT = 'F' " +
                                   $"WHERE GROUP_CODE = '{gcode}' AND DT_CODE = '{code}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();

                    response.msgType = 1;
                    response.msg = "Record Deleted Successfully";
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
