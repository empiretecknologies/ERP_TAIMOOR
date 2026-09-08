using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class BranchRepository : IBranchRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public BranchRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
        public MyHttpResponseMessage GetBranchByCompany(int id)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var brances = FetchBranchesByCompany(id);
                List<Branch> branchList = new List<Branch>();
                if (brances.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in brances.Tables[0].Rows)
                    {
                        Branch branch = new Branch();
                        branch.B_NAME = Row["B_NAME"].ToString();
                        branch.BCODE = Convert.ToInt32(Row["BCODE"]);
                        branchList.Add(branch);
                    }
                }
                response.data = branchList;
                response.msg = "";
                response.msgType = 1;
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public MyHttpResponseMessage GetBranchByCompanyWithRole(int id, int? roleId)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var brances = FetchBranchesByCompanyWithRole(id, roleId);
                List<Branch> branchList = new List<Branch>();
                if (brances.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in brances.Tables[0].Rows)
                    {
                        Branch branch = new Branch();
                        branch.B_NAME = Row["B_NAME"].ToString();
                        branch.BCODE = Convert.ToInt32(Row["BCODE"]);
                        branchList.Add(branch);
                    }
                }
                response.data = branchList;
                response.msg = "";
                response.msgType = 1;
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataSet FetchBranchesByCompany(int id)
        {
            string query = "SELECT * FROM [dbo].[TBL_BRANCH] WHERE DLT = 'T' AND ASTATUS = 'Y' AND CCODE = " + id;
            DataSet data = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
            return data;
        }
        public DataSet FetchBranchesByCompanyWithRole(int id, int? role)
        {
            string query = "SELECT * FROM [dbo].[TBL_BRANCH] WHERE DLT = 'T' AND ASTATUS = 'Y' AND BCODE IN(SELECT DISTINCT R_BCODE FROM TBL_ROLE WHERE ROLE_ID = " + role + ") AND CCODE = " + id;
            DataSet data = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
            return data;
        }
        public MyHttpResponseMessage GetBranchByCode(string? code)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            Branch branch = new Branch();
            try
            {
                string query = "SELECT * FROM TBL_BRANCH WHERE DLT = 'T' AND BCODE = " + code;
                DataSet data = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);

                if (data != null && data.Tables.Count > 0)
                {
                    foreach (DataRow Row in data.Tables[0].Rows)
                    {
                        branch.B_SHORT_NAME = Convert.ToString(Row["B_SHORT_NAME"]);
                        branch.RT_TYPE = Convert.ToString(Row["RT_TYPE"]);
                        branch.B_ADDRESS = Convert.ToString(Row["B_ADDRESS"]);
                        branch.B_NAME = Convert.ToString(Row["B_NAME"]);
                        branch.B_TEL = Convert.ToString(Row["B_TEL"]);
                        branch.B_WEBSITE = Convert.ToString(Row["B_WEBSITE"]);
                        branch.B_NTN = Convert.ToString(Row["B_NTN"]);
                        branch.EMAIL = Convert.ToString(Row["EMAIL"]);
                        branch.CPC_CODE = Row["CPC_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(Row["CPC_CODE"]);
                    }
                }
                response.data = branch;
                response.msg = "";
                response.msgType = 1;
            }
            catch (Exception ex)
            {
                response.data = branch;
                response.msg = "";
                response.msgType = 2;
            }

            return response;
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
                        string query = "SELECT BCODE,B_NAME,B_SHORT_NAME," +
                            " B_ADDRESS,B_TEL,B_GST," +
                            " B_NTN,TIME_IN,TIME_OUT," +
                            " CONTACT_NAME1,CONTACT_NO1,CONTACT_NAME2," +
                            " CONTACT_NO2,CONTACT_NAME3,CONTACT_NO3," +
                            " B_LOGO,ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                            " ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                            " EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                            " ADD_POSTALCODE,EDIT_POSTALCODE,BARCODE," +
                            " CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS, RT_TYPE" +
                            " FROM " + table + "" +
                            " WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' ORDER BY BCODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new Branch
                            {
                                BCODE = Convert.ToInt32(reader["BCODE"]),
                                B_NAME = Convert.ToString(reader["B_NAME"]),
                                B_SHORT_NAME = Convert.ToString(reader["B_SHORT_NAME"]),
                                B_ADDRESS = Convert.ToString(reader["B_ADDRESS"]),
                                B_TEL = Convert.ToString(reader["B_TEL"]),
                                B_GST = Convert.ToString(reader["B_GST"]),
                                B_NTN = Convert.ToString(reader["B_NTN"]),
                                TIME_IN = Convert.ToString(reader["TIME_IN"]),
                                TIME_OUT = Convert.ToString(reader["TIME_OUT"]),
                                CONTACT_NAME1 = Convert.ToString(reader["CONTACT_NAME1"]),
                                CONTACT_NAME2 = Convert.ToString(reader["CONTACT_NAME2"]),
                                CONTACT_NAME3 = Convert.ToString(reader["CONTACT_NAME3"]),
                                CONTACT_NO1 = Convert.ToString(reader["CONTACT_NO1"]),
                                CONTACT_NO2 = Convert.ToString(reader["CONTACT_NO2"]),
                                CONTACT_NO3 = Convert.ToString(reader["CONTACT_NO3"]),
                                B_LOGO = Convert.ToString(reader["B_LOGO"]),
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
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
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
        public MyHttpResponseMessage Save(Branch modelRecord, Common common)
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
                            if (modelRecord.BCODE == null || modelRecord.BCODE == 0)
                            {
                                query = "INSERT INTO " + table + "" +
                                    "(BCODE,B_NAME,B_SHORT_NAME,B_ADDRESS,B_TEL," +
                                    "B_GST,B_NTN,CCODE,TIME_IN,TIME_OUT," +
                                    "CONTACT_NAME1,CONTACT_NO1,CONTACT_NAME2,CONTACT_NO2,CONTACT_NAME3," +
                                    "CONTACT_NO3,B_LOGO,ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                    "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                    "MENU_ID,ADD_POSTALCODE,EDIT_POSTALCODE,BARCODE,DLT,ASTATUS,RT_TYPE)" +
                                    "VALUES" +
                                    "('" + GenerateNextId(common) + "','" + modelRecord.B_NAME + "','" + modelRecord.B_SHORT_NAME + "','" + modelRecord.B_ADDRESS + "','" + modelRecord.B_TEL + "'," +
                                    "'" + modelRecord.B_GST + "','" + modelRecord.B_NTN + "','" + modelRecord.CCODE + "','" + modelRecord.TIME_IN + "','" + modelRecord.TIME_OUT + "'," +
                                    "'" + modelRecord.CONTACT_NAME1 + "','" + modelRecord.CONTACT_NO1 + "','" + modelRecord.CONTACT_NAME2 + "','" + modelRecord.CONTACT_NO2 + "','" + modelRecord.CONTACT_NAME3 + "'," +
                                    "'" + modelRecord.CONTACT_NO3 + "','" + modelRecord.B_LOGO + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                    "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                    "'" + common.MenuID + "','" + Postal + "','" + Postal + "','" + modelRecord.BARCODE + "','T','" + modelRecord.ASTATUS + "','" + modelRecord.RT_TYPE + "')";
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE B_NAME = '" + modelRecord.B_NAME + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
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
                                query = "UPDATE " + table + " SET B_NAME = '" + modelRecord.B_NAME + @"',
                                        B_SHORT_NAME = '" + modelRecord.B_SHORT_NAME + @"',
                                        B_ADDRESS = '" + modelRecord.B_ADDRESS + @"',
                                        B_TEL = '" + modelRecord.B_TEL + @"',
                                        B_GST = '" + modelRecord.B_GST + @"',
                                        B_NTN = '" + modelRecord.B_NTN + @"',
                                        CCODE = '" + modelRecord.CCODE + @"',
                                        TIME_IN = '" + modelRecord.TIME_IN + @"',
                                        TIME_OUT = '" + modelRecord.TIME_OUT + @"',
                                        CONTACT_NAME1 = '" + modelRecord.CONTACT_NAME1 + @"',
                                        CONTACT_NO1 = '" + modelRecord.CONTACT_NO1 + @"',
                                        CONTACT_NAME2 = '" + modelRecord.CONTACT_NAME2 + @"',
                                        CONTACT_NO2 = '" + modelRecord.CONTACT_NO2 + @"',
                                        CONTACT_NAME3 = '" + modelRecord.CONTACT_NAME3 + @"',
                                        CONTACT_NO3 = '" + modelRecord.CONTACT_NO3 + @"',
                                        B_LOGO = '" + modelRecord.B_LOGO + @"',
                                        EDIT_USER_ID = '" + userid + @"',
                                        EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                        EDIT_COMPUTER_NAME = '" + Computer + @"',
                                        EDIT_IP_ADDRESS = '" + Ip + @"',
                                        MENU_ID = '" + common.MenuID + @"',
                                        EDIT_POSTALCODE = '" + Postal + @"',
                                        BARCODE = '" + modelRecord.BARCODE + @"',
                                        ASTATUS = '" + modelRecord.ASTATUS + @"',
                                        RT_TYPE = '" + modelRecord.RT_TYPE + @"'
                                        WHERE BCODE = '" + modelRecord.BCODE + @"'";
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE B_NAME = '" + modelRecord.B_NAME + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
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
                    string maxIdQuery = "SELECT ISNULL(MAX(BCODE), 0) + 1 FROM " + table;
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

        public MyHttpResponseMessage GetBranchById(int id, Common common)
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
                        string query = "SELECT BCODE,B_NAME,B_SHORT_NAME," +
                            " B_ADDRESS,B_TEL,B_GST,BARCODE,CCODE," +
                            " B_NTN,TIME_IN,TIME_OUT," +
                            " CONTACT_NAME1,CONTACT_NO1,CONTACT_NAME2," +
                            " CONTACT_NO2,CONTACT_NAME3,CONTACT_NO3," +
                            " B_LOGO,EDIT_USER_ID,EDIT_DATE," +
                            " EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                            " EDIT_POSTALCODE,BARCODE," +
                            " ASTATUS, RT_TYPE" +
                            " FROM " + table + "" +
                            " WHERE MENU_ID = '" + common.MenuID + "' AND BCODE = '" + id + "' AND DLT = 'T'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var Branch = new Branch
                            {
                                BCODE = Convert.ToInt32(reader["BCODE"]),
                                B_NAME = Convert.ToString(reader["B_NAME"]),
                                B_SHORT_NAME = Convert.ToString(reader["B_SHORT_NAME"]),
                                B_ADDRESS = Convert.ToString(reader["B_ADDRESS"]),
                                B_TEL = Convert.ToString(reader["B_TEL"]),
                                B_GST = Convert.ToString(reader["B_GST"]),
                                BARCODE = Convert.ToString(reader["BARCODE"]),
                                CCODE = Convert.ToInt32(reader["CCODE"]),
                                B_NTN = Convert.ToString(reader["B_NTN"]),
                                TIME_IN = Convert.ToString(reader["TIME_IN"]),
                                TIME_OUT = Convert.ToString(reader["TIME_OUT"]),
                                CONTACT_NAME1 = Convert.ToString(reader["CONTACT_NAME1"]),
                                CONTACT_NAME2 = Convert.ToString(reader["CONTACT_NAME2"]),
                                CONTACT_NAME3 = Convert.ToString(reader["CONTACT_NAME3"]),
                                CONTACT_NO1 = Convert.ToString(reader["CONTACT_NO1"]),
                                CONTACT_NO2 = Convert.ToString(reader["CONTACT_NO2"]),
                                CONTACT_NO3 = Convert.ToString(reader["CONTACT_NO3"]),
                                B_LOGO = Convert.ToString(reader["B_LOGO"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
                            };

                            response.msg = "";
                            response.msgType = 1;
                            response.data = Branch;
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE BCODE = '" + id + "'";
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
