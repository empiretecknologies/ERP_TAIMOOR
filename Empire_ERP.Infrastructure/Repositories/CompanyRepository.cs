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
    public class CompanyRepository : ICompanyRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public CompanyRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }
        public MyHttpResponseMessage GetCompanies()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var companies = FetchCompanies();
                List<Company> companyList = new List<Company>();
                if (companies.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in companies.Tables[0].Rows)
                    {
                        Company company = new Company();
                        company.CCODE = Convert.ToInt32(Row["CCODE"]);
                        company.C_NAME = Convert.ToString(Row["C_NAME"]);
                        companyList.Add(company);
                    }
                }
                response.data = companies;
                response.msg = "";
                response.msgType = 1;
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public DataSet FetchCompanies()
        {
            string query = "select * from [dbo].[TBL_COMPANY] WHERE DLT = 'T'";
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
                        string query = "SELECT CCODE,C_NAME," +
                            " C_ADDRESS,C_TEL,C_GST,C_NTN, C_LOGO, C_WATER," +
                            " BUS_NATURE,ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                            " ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                            " EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                            " ADD_POSTALCODE,EDIT_POSTALCODE" +
                            " FROM " + table + "" +
                            " WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' ORDER BY CCODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new Company
                            {
                                CCODE = Convert.ToInt32(reader["CCODE"]),
                                BUS_NATURE = Convert.ToInt32(reader["BUS_NATURE"]),
                                C_NAME = Convert.ToString(reader["C_NAME"]),
                                C_LOGO = Convert.ToString(reader["C_LOGO"]),
                                C_WATER = Convert.ToString(reader["C_WATER"]),
                                C_ADDRESS = Convert.ToString(reader["C_ADDRESS"]),
                                C_TEL = Convert.ToString(reader["C_TEL"]),
                                C_GST = Convert.ToString(reader["C_GST"]),
                                C_NTN = Convert.ToString(reader["C_NTN"]),
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
        public MyHttpResponseMessage Save(Company modelRecord, Common common)
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
                            if (modelRecord.CCODE == null || modelRecord.CCODE == 0)
                            {
                                query = "INSERT INTO " + table + "" +
                                    "(CCODE, C_LOGO, C_WATER, C_NAME,C_ADDRESS,C_TEL," +
                                    "C_GST,C_NTN,BUS_NATURE," +
                                    "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                    "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                    "MENU_ID,ADD_POSTALCODE,EDIT_POSTALCODE,DLT)" +
                                    "VALUES" +
                                    "('" + GenerateNextId(common) + "','" + modelRecord.C_LOGO + "','" + modelRecord.C_WATER + "','" + modelRecord.C_NAME + "','" + modelRecord.C_ADDRESS + "','" + modelRecord.C_TEL + "'," +
                                    "'" + modelRecord.C_GST + "','" + modelRecord.C_NTN + "','" + modelRecord.BUS_NATURE + "'," +
                                    "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                    "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                    "'" + common.MenuID + "','" + Postal + "','" + Postal + "','T')";
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE C_NAME = '" + modelRecord.C_NAME + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
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
                                query = "UPDATE " + table + " SET C_NAME = '" + modelRecord.C_NAME + @"',
                                        C_ADDRESS = '" + modelRecord.C_ADDRESS + @"',
                                        C_LOGO = '" + modelRecord.C_LOGO + @"',
                                        C_WATER = '" + modelRecord.C_WATER + @"',
                                        C_TEL = '" + modelRecord.C_TEL + @"',
                                        C_GST = '" + modelRecord.C_GST + @"',
                                        C_NTN = '" + modelRecord.C_NTN + @"',
                                        BUS_NATURE = '" + modelRecord.BUS_NATURE + @"',
                                        EDIT_USER_ID = '" + userid + @"',
                                        EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                        EDIT_COMPUTER_NAME = '" + Computer + @"',
                                        EDIT_IP_ADDRESS = '" + Ip + @"',
                                        MENU_ID = '" + common.MenuID + @"',
                                        EDIT_POSTALCODE = '" + Postal + @"'
                                        WHERE CCODE = '" + modelRecord.CCODE + @"'";
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE C_NAME = '" + modelRecord.C_NAME + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
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
                    string maxIdQuery = "SELECT ISNULL(MAX(CCODE), 0) + 1 FROM " + table;
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

        public MyHttpResponseMessage GetCompanyById(int id, Common common)
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
                        string query = "SELECT CCODE,C_NAME," +
                            " C_ADDRESS,C_TEL,C_GST,BUS_NATURE," +
                            " C_NTN,C_LOGO,C_WATER," +
                            " EDIT_USER_ID,EDIT_DATE," +
                            " EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                            " EDIT_POSTALCODE" +
                            " FROM " + table + "" +
                            " WHERE MENU_ID = '" + common.MenuID + "' AND CCODE = '" + id + "' AND DLT = 'T'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var Branch = new Company
                            {
                                CCODE = Convert.ToInt32(reader["CCODE"]),
                                C_NAME = Convert.ToString(reader["C_NAME"]),
                                C_ADDRESS = Convert.ToString(reader["C_ADDRESS"]),
                                C_TEL = Convert.ToString(reader["C_TEL"]),
                                C_GST = Convert.ToString(reader["C_GST"]),
                                BUS_NATURE = Convert.ToInt32(reader["BUS_NATURE"]),
                                C_NTN = Convert.ToString(reader["C_NTN"]),
                                C_LOGO = Convert.ToString(reader["C_LOGO"]),
                                C_WATER = Convert.ToString(reader["C_WATER"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE CCODE = '" + id + "'";
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

        public MyHttpResponseMessage GetCompanyByCode(int code)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "SELECT CCODE,C_NAME,C_LOGO,RIGHT_CLOGO,C_WATER," +
                        " C_ADDRESS,C_TEL,C_GST,BUS_NATURE,C_NTN," +
                        " EDIT_USER_ID,EDIT_DATE," +
                        " EDIT_USER_ID,EDIT_DATE," +
                        " EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,EDIT_POSTALCODE" +
                        " FROM TBL_COMPANY " +
                        $"WHERE CCODE = '{code}' AND DLT = 'T'";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        var company = new Company
                        {
                            CCODE = Convert.ToInt32(reader["CCODE"]),
                            C_NAME = Convert.ToString(reader["C_NAME"]),
                            C_ADDRESS = Convert.ToString(reader["C_ADDRESS"]),
                            C_TEL = Convert.ToString(reader["C_TEL"]),
                            C_LOGO = Convert.ToString(reader["C_LOGO"]),
                            RC_LOGO = Convert.ToString(reader["RIGHT_CLOGO"]),
                            C_WATER = Convert.ToString(reader["C_WATER"]),
                        };

                        response.msg = "";
                        response.msgType = 1;
                        response.data = company;
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
