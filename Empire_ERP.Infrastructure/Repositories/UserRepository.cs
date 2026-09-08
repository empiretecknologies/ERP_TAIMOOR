using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public UserRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
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
                        string query = "SELECT  u.U_ID, u.USERNAME, u.CELL_NO, u.EMAIL, u.UPASS, u.CPASS, u.USTART_DATE, u.ESTART_DATE, u.BRANCH, u.ADD_USER_ID," +
                                       "u.FULLNAME, u.PICTURES, u.ROLEID, u.ROLE_TYPE, " +
                                       "u.ADD_DATE,u.ADD_COMPUTER_NAME,u.ADD_IP_ADDRESS,u.EDIT_USER_ID," +
                                       "u.EDIT_DATE,u.EDIT_COMPUTER_NAME,u.EDIT_IP_ADDRESS,u.ADD_POSTALCODE, r.ROLE_NAME, b.B_NAME as BRANCH_NAME," +
                                       "u.EDIT_POSTALCODE,CASE WHEN u.ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       "FROM " + table + " u " +
                                       "LEFT OUTER JOIN (SELECT DISTINCT ROLE_ID, ROLE_NAME FROM TBL_ROLE) r ON u.ROLEID = r.ROLE_ID " +
                                       "LEFT OUTER Join TBL_BRANCH b on BRANCH = b.BCODE " +
                                       "WHERE u.MENU_ID = '" + common.MenuID + "' AND u.DLT = 'T' ORDER BY u.U_ID DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new User
                            {
                                U_ID = Convert.ToInt32(reader["U_ID"]),
                                USERNAME = Convert.ToString(reader["USERNAME"]),
                                CELL_NO = Convert.ToString(reader["CELL_NO"]),
                                EMAIL = Convert.ToString(reader["EMAIL"]),
                                UPASS = Convert.ToString(reader["UPASS"]),
                                CPASS = Convert.ToString(reader["CPASS"]),
                                USTART_DATE = Convert.ToDateTime(reader["USTART_DATE"]),
                                ESTART_DATE = Convert.ToDateTime(reader["ESTART_DATE"]),
                                BRANCH = Convert.ToInt32(reader["BRANCH"]),
                                FULLNAME = Convert.ToString(reader["FULLNAME"]),
                                PICTURES = Convert.ToString(reader["PICTURES"]),
                                ROLEID = Convert.ToInt32(reader["ROLEID"]),
                                ROLE_TYPE = Convert.ToString(reader["ROLE_TYPE"]),
                                BRANCH_NAME = Convert.ToString(reader["BRANCH_NAME"]),
                                ROLE_NAME = Convert.ToString(reader["ROLE_NAME"]),
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

        public MyHttpResponseMessage Save(User modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var password = CommonService.EncryptString(modelRecord.UPASS);
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
                    var device = modelRecord.MULTIPLE_LOGIN == 1 ? "" : common.ComputerName;
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
                            if (modelRecord.U_ID == null || modelRecord.U_ID == 0)
                            {
                                query = "INSERT INTO " + table + " " +
                                            "(U_ID, MAC_ID, USERNAME, CELL_NO, EMAIL, UPASS, CPASS, USTART_DATE, ESTART_DATE, BRANCH, FULLNAME, PICTURES, ROLEID, ROLE_TYPE," +
                                            "ADD_USER_ID, ADD_DATE, ADD_IP_ADDRESS, ADD_COMPUTER_NAME, ADD_POSTALCODE, ASTATUS, MENU_ID, DLT, MULTIPLE_LOGIN)" +
                                            "VALUES" +
                                            "('" + GenerateNextId(common) + "','" + device + "','" + modelRecord.USERNAME + "','" + modelRecord.CELL_NO + "','" + modelRecord.EMAIL + "','" + password + "'," +
                                            "'" + password + "','" + modelRecord.USTART_DATE + "','" + modelRecord.ESTART_DATE + "','" + modelRecord.BRANCH + "','" + modelRecord.FULLNAME + "'," +
                                            "'" + modelRecord.PICTURES + "','" + modelRecord.ROLEID + "','" + modelRecord.ROLE_TYPE + "'," +
                                            "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Ip + "','" + Computer + "'," +
                                            "'" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                            "'" + common.MenuID + "','T','"+modelRecord.MULTIPLE_LOGIN + "')";
                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE (EMAIL = '" + modelRecord.EMAIL + "' OR USERNAME = '" + modelRecord.USERNAME + "') AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.msg = "User Added Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msg = "Username Or Email Already Exist !....";
                                    response.msgType = 2;
                                }
                            }
                            else
                            {
                                query = "UPDATE " + table + " SET " +
                                    "CELL_NO = '" + modelRecord.CELL_NO + @"',
                                    EMAIL = '" + modelRecord.EMAIL + @"',
                                    UPASS = '" + password + @"',
                                    CPASS = '" + password + @"',
                                    USTART_DATE = '" + modelRecord.USTART_DATE + @"',
                                    ESTART_DATE = '" + modelRecord.ESTART_DATE + @"',
                                    BRANCH = '" + modelRecord.BRANCH + @"',
                                    FULLNAME = '" + modelRecord.FULLNAME + @"',
                                    PICTURES = '" + modelRecord.PICTURES + @"',
                                    ROLEID = '" + modelRecord.ROLEID + @"',
                                    ROLE_TYPE = '" + modelRecord.ROLE_TYPE + @"',
                                    EDIT_USER_ID = '" + userid + @"',
                                    EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                    EDIT_COMPUTER_NAME = '" + Computer + @"',
                                    EDIT_IP_ADDRESS = '" + Ip + @"',
                                    EDIT_POSTALCODE = '" + Postal + @"',
                                    MAC_ID = '" + device + @"',
                                    ASTATUS = '" + modelRecord.ASTATUS + @"',
                                    MULTIPLE_LOGIN = '" + modelRecord.MULTIPLE_LOGIN + @"'
                                    WHERE U_ID = '" + modelRecord.U_ID + "' AND MENU_ID = '" + common.MenuID + "'";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE EMAIL = '" + modelRecord.EMAIL + "' AND MENU_ID = '" + common.MenuID + "' AND U_ID != '" + modelRecord.U_ID + "' AND DLT = 'T'";
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count > 0)
                                {
                                    transaction.Rollback();
                                    response.msg = "Email Already Exist !....";
                                    response.msgType = 2;
                                }
                                else
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.msg = "User Updated Successfully";
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
                    string maxIdQuery = "SELECT ISNULL(MAX(U_ID), 0) + 1 FROM " + table;
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

        public MyHttpResponseMessage GetUserById(int id, Common common)
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
                        string query = "SELECT  u.U_ID, u.MAC_ID, u.MULTIPLE_LOGIN, u.USERNAME, u.CELL_NO, u.EMAIL, u.UPASS, u.CPASS, u.USTART_DATE, u.ESTART_DATE, u.BRANCH, u.ADD_USER_ID," +
                                       "u.FULLNAME, u.PICTURES, u.ROLEID, u.ROLE_TYPE, " +
                                       "u.ADD_DATE,u.ADD_COMPUTER_NAME,u.ADD_IP_ADDRESS,u.EDIT_USER_ID," +
                                       "u.EDIT_DATE,u.EDIT_COMPUTER_NAME,u.EDIT_IP_ADDRESS,u.ADD_POSTALCODE, r.ROLE_NAME, b.B_NAME as BRANCH_NAME," +
                                       "u.EDIT_POSTALCODE,CASE WHEN u.ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                                       "FROM " + table + " u " +
                                       "LEFT Join TBL_ROLE r on ROLEID = r.ROLE_ID " +
                                       "LEFT Join TBL_BRANCH b on BRANCH = b.BCODE " +
                                       "WHERE u.MENU_ID = '" + common.MenuID + "' AND u.DLT = 'T' AND U_ID = '" + id + "'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var user = new User
                            {
                                U_ID = Convert.ToInt32(reader["U_ID"]),
                                MULTIPLE_LOGIN = Convert.ToInt32(reader["MULTIPLE_LOGIN"]),
                                USERNAME = Convert.ToString(reader["USERNAME"]),
                                CELL_NO = Convert.ToString(reader["CELL_NO"]),
                                EMAIL = Convert.ToString(reader["EMAIL"]),
                                UPASS = CommonService.DecryptString(Convert.ToString(reader["UPASS"])),
                                CPASS = CommonService.DecryptString(Convert.ToString(reader["CPASS"])),
                                USTART_DATE = Convert.ToDateTime(reader["USTART_DATE"]),
                                ESTART_DATE = Convert.ToDateTime(reader["ESTART_DATE"]),
                                BRANCH = Convert.ToInt32(reader["BRANCH"]),
                                FULLNAME = Convert.ToString(reader["FULLNAME"]),
                                PICTURES = Convert.ToString(reader["PICTURES"]),
                                ROLEID = Convert.ToInt32(reader["ROLEID"]),
                                ROLE_TYPE = Convert.ToString(reader["ROLE_TYPE"]),
                                BRANCH_NAME = Convert.ToString(reader["BRANCH_NAME"]),
                                ROLE_NAME = Convert.ToString(reader["ROLE_NAME"]),
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
                                MAC_ID = Convert.ToString(reader["MAC_ID"]),
                            };

                            response.msg = "";
                            response.msgType = 1;
                            response.data = user;
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE U_ID = '" + id + "'";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            response.msg = "User Deleted Successfully";
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

        public MyHttpResponseMessage SaveUserOTP(OTPUser user)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
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
                        string query = $"INSERT INTO TBL_FPASSWORD" +
                                        "(U_ID, EMAIL, OTP, ADD_DATE, STATUS)" +
                                        "VALUES" +
                                        "('0','" + user.EMAIL + "','" + user.OTP + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', 'PENDING')";
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        response.msgType = 1;
                        response.msg = "Record Added Successfully";
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

        public MyHttpResponseMessage OTPVerification(OTPUser user)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string connectionString = new SQLService().getconnstring();
                string query = $"SELECT ADD_DATE, OTP FROM TBL_FPASSWORD WHERE ID = (SELECT MAX(ID) FROM TBL_FPASSWORD WHERE U_ID = '{user.U_ID}' AND OTP = '{user.OTP}' AND STATUS = 'PENDING')";
                DataSet dts = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
                if (dts.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in dts.Tables[0].Rows)
                    {
                        var generatedDate = Convert.ToDateTime(Row["ADD_DATE"]);
                        var OTP = Convert.ToInt32(Row["OTP"]);
                        if (generatedDate.AddSeconds(120) <= Convert.ToDateTime(CommonService.GetDateTime("Pakistan Standard Time")))
                        {
                            OTPUser otpUser = new OTPUser();
                            otpUser.U_ID = user.U_ID;
                            otpUser.STATUS = "EXPIRED";
                            UpdateUserOTPStatus(otpUser);
                            response.msg = "OTP has been expired!";
                            response.msgType = 2;
                        }
                        else
                        {
                            if (user.OTP == OTP)
                            {
                                OTPUser otpUser = new OTPUser();
                                otpUser.U_ID = user.U_ID;
                                otpUser.STATUS = "APPROVED";
                                UpdateUserOTPStatus(otpUser);
                                response.msg = "OTP successfully verified!";
                                response.msgType = 1;
                            }
                            else
                            {
                                response.msg = "Incorrect OTP!";
                                response.msgType = 2;
                            }
                        }
                    }
                }
                else
                {
                    response.msg = "Something went wrong! please try again later.";
                    response.msgType = 2;
                    return response;
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

        public MyHttpResponseMessage UpdateUserOTPStatus(OTPUser user)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
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
                        string query = $"UPDATE TBL_FPASSWORD SET STATUS = '" + user.STATUS + "' WHERE U_ID = '0' AND EMAIL = '"+user.EMAIL+"' AND STATUS = 'PENDING'";
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                        transaction.Commit();
                        response.msgType = 1;
                        response.msg = "Record Added Successfully";
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
    }
}