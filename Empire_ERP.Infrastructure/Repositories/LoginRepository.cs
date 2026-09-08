using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.NetworkInformation;
using static Empire_ERP.Core.Entities.LoginDetail;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class LoginRepository : ILoginRepository
    {
        public MyHttpResponseMessage CheckCredentials(string username, string password, bool isremember)
        {
            return Login(username, password);
        }

        public Info GetBackGroundAndLogo()
        {
            Info logo = new Info();

            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT TOP 1 * FROM TBL_WLABEL";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    logo = new()
                    {
                        MLOGO = Convert.ToString(reader["MLOGO"]),
                        ICON = Convert.ToString(reader["TITLE"]),
                        MSCREEN = Convert.ToString(reader["MSCREEN"]),
                        PB_LOGO = Convert.ToString(reader["PB_LOGO"]),
                        C_NAME = Convert.ToString(reader["C_NAME"]),
                        COPY_RIGHTS = Convert.ToString(reader["COPY_RIGHTS"]),
                        TEL = Convert.ToString(reader["TEL"]),
                        WEBSITE = Convert.ToString(reader["WEBSITE"]),
                        ABOUT_LABEL = Convert.ToString(reader["ABOUT_LABEL"]),
                        ABOUT_LINK = Convert.ToString(reader["ABOUT_LINK"]),
                        CONTACT_LABEL = Convert.ToString(reader["CONTACT_LABEL"]),
                        CONTACT_LINK = Convert.ToString(reader["CONTACT_LINK"]),
                    };
                }
                reader.Close();
            }

            //string query = "SELECT TOP 1 * FROM TBL_WLABEL";
            //DataSet dts = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
            //if (dts.Tables[0].Rows.Count > 0)
            //{
            //    foreach (DataRow Row in dts.Tables[0].Rows)
            //    {
            //        logo.data = Convert.ToString(Row["MLOGO"]);
            //        logo.data2 = Convert.ToString(Row["MSCREEN"]);
            //    }
            //}
            return logo;
        }

        public MyHttpResponseMessage Login(string username, string password)
        {
            MyHttpResponseMessage res = new MyHttpResponseMessage();
            try
            {
                var dts = CheckLogin(username, password);
                List<User> users = new List<User>();
                if (dts.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in dts.Tables[0].Rows)
                    {
                        User user = new User();
                        user.U_ID = Convert.ToInt32(Row["U_ID"]);
                        user.USERNAME = Convert.ToString(Row["USERNAME"]);
                        user.CELL_NO = Convert.ToString(Row["CELL_NO"]);
                        user.EMAIL = Convert.ToString(Row["EMAIL"]);
                        user.UPASS = Convert.ToString(Row["UPASS"]);
                        user.CPASS = Convert.ToString(Row["CPASS"]);
                        user.USTART_DATE = Row["USTART_DATE"] == DBNull.Value ? new DateTime(0001, 1, 1) : Convert.ToDateTime(Row["USTART_DATE"]);
                        user.ESTART_DATE = Row["ESTART_DATE"] == DBNull.Value ? new DateTime(0001, 1, 1) : Convert.ToDateTime(Row["ESTART_DATE"]);
                        user.BRANCH = Convert.ToInt32(Row["BRANCH"]);
                        user.ADD_USER_ID = Convert.ToString(Row["ADD_USER_ID"]);
                        user.ADD_DATE = Row["ADD_DATE"] == DBNull.Value ? new DateTime(0001, 1, 1) : Convert.ToDateTime(Row["ADD_DATE"]);
                        user.ADD_COMPUTER_NAME = Convert.ToString(Row["ADD_COMPUTER_NAME"]);
                        user.ADD_IP_ADDRESS = Convert.ToString(Row["ADD_IP_ADDRESS"]);
                        user.EDIT_USER_ID = Convert.ToString(Row["EDIT_USER_ID"]);
                        user.EDIT_DATE = Row["EDIT_DATE"] == DBNull.Value ? new DateTime(0001, 1, 1) : Convert.ToDateTime(Row["EDIT_DATE"]);
                        user.EDIT_COMPUTER_NAME = Convert.ToString(Row["EDIT_COMPUTER_NAME"]);
                        user.EDIT_IP_ADDRESS = Convert.ToString(Row["EDIT_IP_ADDRESS"]);
                        user.FULLNAME = Convert.ToString(Row["FULLNAME"]);
                        user.ADD_POSTALCODE = Convert.ToString(Row["ADD_POSTALCODE"]);
                        user.EDIT_POSTALCODE = Convert.ToString(Row["EDIT_POSTALCODE"]);
                        user.PICTURES = Convert.ToString(Row["PICTURES"]);
                        user.MENU_ID = Convert.ToInt32(Row["MENU_ID"]);
                        user.DLT = Convert.ToString(Row["DLT"]);
                        user.ROLEID = Convert.ToInt32(Row["ROLEID"]);
                        user.SHOW_SELECTED = Row["SHOW_SELECTED"] == DBNull.Value ? 1 : Convert.ToInt32(Row["SHOW_SELECTED"]);
                        user.ROLE_TYPE = Convert.ToString(Row["ROLE_TYPE"]);
                        user.ASTATUS = Convert.ToString(Row["ASTATUS"]);
                        user.MAC_ID = Convert.ToString(Row["MAC_ID"]);
                        users.Add(user);

                    }
                    if (users[0].USTART_DATE <= DateTime.Now)
                    {
                        if (Convert.ToString(users[0].ESTART_DATE) == null)
                        {
                            res.data = users;
                            res.msg = "login successfully";
                            res.msgType = 1;
                            return res;
                        }
                        else
                        {
                            res.data = users;
                            res.msg = "login successfully";
                            res.msgType = 1;
                            return res;
                        }
                    }
                    else
                    {
                        res.data = users;
                        res.msg = "login fail.";
                        res.msgType = 2;
                        return res;
                    }
                }
                else
                {
                    res.data = "";
                    res.msg = "Your username or password does not match.";
                    res.msgType = 2;
                    return res;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataSet CheckLogin(string username, string password)
        {
            string query = "SELECT U.*, R.SHOW_SELECTED FROM [dbo].[TBL_USER] U " +
                "LEFT OUTER JOIN TBL_ROLE R " +
                "ON U.ROLEID = R.ROLE_ID " +
                "WHERE U.[USERNAME] ='" + username + "' AND" + " U.[UPASS] = '" + CommonService.EncryptString(password) + "' AND U.[ASTATUS]='Y' AND U.DLT = 'T'";
            DataSet dts = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
            return dts;
        }

        public static string GetMacAddress()
        {
            var networkInterface = NetworkInterface
                .GetAllNetworkInterfaces()
                .FirstOrDefault(nic => nic.OperationalStatus == OperationalStatus.Up &&
                                       nic.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                                       nic.GetPhysicalAddress().ToString() != "");

            return networkInterface?.GetPhysicalAddress().ToString() ?? "No MAC Address found";
        }

        public MyHttpResponseMessage GetUserByUsername(string username)
        {
            MyHttpResponseMessage res = new MyHttpResponseMessage();
            try
            {
                string query = "SELECT TOP 1 * FROM TBL_USER WHERE LOWER(USERNAME) = LOWER('" + username + "') AND ASTATUS = 'Y' AND DLT = 'T'";
                DataSet dts = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
                User user = new User();
                if (dts.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in dts.Tables[0].Rows)
                    {
                        user.U_ID = Convert.ToInt32(Row["U_ID"]);
                        user.USERNAME = Convert.ToString(Row["USERNAME"]);
                        user.CELL_NO = Convert.ToString(Row["CELL_NO"]);
                        user.EMAIL = Convert.ToString(Row["EMAIL"]);
                        user.UPASS = Convert.ToString(Row["UPASS"]);
                        user.CPASS = Convert.ToString(Row["CPASS"]);
                        user.FULLNAME = Convert.ToString(Row["FULLNAME"]);
                        user.PICTURES = Convert.ToString(Row["PICTURES"]);
                        user.ROLE_TYPE = Convert.ToString(Row["ROLE_TYPE"]);
                    }

                    res.data = user;
                    res.msgType = 1;
                    return res;
                }
                else
                {
                    res.data = "";
                    res.msg = "Username does not match.";
                    res.msgType = 2;
                    return res;
                }
            }
            catch (Exception ex)
            {
                res.data = "";
                res.msg = "Something went wrong! please try again later.";
                res.msgType = 2;
                return res;
            }
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
                                        "('" + user.U_ID + "','" + user.EMAIL + "','" + user.OTP + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', 'PENDING')";
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
                        string query = $"UPDATE TBL_FPASSWORD SET STATUS = '" + user.STATUS + "' WHERE U_ID = '" + user.U_ID + "' AND STATUS = 'PENDING'";
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
                string query = $"SELECT ADD_DATE, OTP FROM TBL_FPASSWORD WHERE ID = (SELECT MAX(ID) FROM TBL_FPASSWORD WHERE U_ID = '{user.U_ID}' AND STATUS = 'PENDING')";
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

        public MyHttpResponseMessage UpdatePassword(User user)
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
                        var encryptedPassword = CommonService.EncryptString(user.UPASS);
                        string query = $"SELECT 1 FROM TBL_USER WHERE USERNAME = '{user.USERNAME}' AND UPASS = '{encryptedPassword}'";
                        DataSet dts = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
                        if (dts.Tables[0].Rows.Count > 0)
                        {
                            transaction.Commit();
                            response.msgType = 2;
                            response.msg = "The new password you have chosen has been used previously. Please select a password that you have not used before";
                        }
                        else
                        {
                            query = $"UPDATE TBL_USER SET UPASS = '{encryptedPassword}', CPASS = '{encryptedPassword}' WHERE U_ID = '{user.U_ID}' AND ASTATUS = 'Y' AND DLT = 'T'";
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                            transaction.Commit();
                            response.msgType = 1;
                            response.msg = "Password Updated Successfully";
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

        public MyHttpResponseMessage GetUserByUserID(int userID)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string query = $"SELECT * FROM TBL_USER WHERE U_ID = '{userID}' AND DLT = 'T'";
                DataSet dts = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
                User user = new User();
                if (dts.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in dts.Tables[0].Rows)
                    {
                        user.U_ID = Convert.ToInt32(Row["U_ID"]);
                        user.USERNAME = Convert.ToString(Row["USERNAME"]);
                        user.CELL_NO = Convert.ToString(Row["CELL_NO"]);
                        user.EMAIL = Convert.ToString(Row["EMAIL"]);
                        user.UPASS = CommonService.DecryptString(Convert.ToString(Row["UPASS"]));
                        user.CPASS = CommonService.DecryptString(Convert.ToString(Row["CPASS"]));
                        user.FULLNAME = Convert.ToString(Row["FULLNAME"]);
                        user.PICTURES = Convert.ToString(Row["PICTURES"]);
                        user.ROLE_TYPE = Convert.ToString(Row["ROLE_TYPE"]);
                    }
                    response.data = user;
                    response.msgType = 1;
                }
                else
                {
                    response.msg = "Something went wrong! please try again later.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                response.data = "";
                response.msg = "Something went wrong! please try again later.";
                response.msgType = 2;
            }

            return response;
        }
        public MyHttpResponseMessage GetAllDDL(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<CompanyModel> jsonCompanyData = new List<CompanyModel>();
                List<BranchModel> jsonBranchData = new List<BranchModel>();
                List<PeriodModel> jsonPeriodData = new List<PeriodModel>();
                List<object> jsonAllPeriodData = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $@"SELECT 
                                        CM.CCODE,
                                        CM.C_NAME,
                                        BR.BCODE,
                                        BR.B_NAME,
                                        PR.PID,
                                        PR.DESCR
                                    FROM TBL_COMPANY CM
                                    JOIN TBL_BRANCH BR ON CM.CCODE = BR.CCODE
                                    JOIN TBL_PERIOD PR ON BR.BCODE = PR.BCODE
                                    WHERE CM.DLT = 'T'
                                      AND BR.DLT = 'T'
                                      AND PR.DLT = 'T'
                                      AND (
                                            '{common.RoleType}' = 'A'  
                                            OR BR.BCODE IN (        
                                                    SELECT DISTINCT R.R_BCODE
                                                    FROM TBL_USER U
                                                    JOIN TBL_ROLE R ON R.ROLE_ID = U.ROLEID
                                                    WHERE U.USERNAME = '{common.Username}'
                                            )
                                          )
                                    ORDER BY BR.BCODE ASC, PR.PID DESC;";


                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {

                        var row = new CompanyModel
                        {
                            CCODE = reader["CCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CCODE"]),
                            C_NAME = Convert.ToString(reader["C_NAME"]),
                        };
                        jsonCompanyData.Add(row);


                        var rows = new BranchModel
                        {
                            BCODE = reader["BCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BCODE"]),
                            B_NAME = Convert.ToString(reader["B_NAME"]),
                        };
                        jsonBranchData.Add(rows);



                        var row1 = new PeriodModel
                        {
                            PID = reader["PID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PID"]),
                            DESCR = Convert.ToString(reader["DESCR"]),
                        };
                        jsonPeriodData.Add(row1);

                    }
                    reader.Close();
                }
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $@"Select PID , DESCR , BCODE from TBL_PERIOD  where DLT = 'T'";


                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            PID = reader["PID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PID"]),
                            BCODE = reader["BCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BCODE"]),
                            DESCR = Convert.ToString(reader["DESCR"]),
                        };
                        jsonAllPeriodData.Add(row);

                    }
                    reader.Close();
                }
                jsonCompanyData = jsonCompanyData
                    .GroupBy(x => x.CCODE)
                    .Select(g => g.First())
                    .ToList();

                jsonBranchData = jsonBranchData
                                    .GroupBy(x => x.BCODE)
                                    .Select(g => g.First())
                                    .ToList();

                jsonPeriodData = jsonPeriodData
                                    .GroupBy(x => x.PID)
                                    .Select(g => g.First())
                                    .ToList();
                response.data = new
                {
                    Companies = jsonCompanyData,
                    Branches = jsonBranchData,
                    Periods = jsonPeriodData,
                    AllPeriods = jsonAllPeriodData
                };
                response.msg = "";
                response.msgType = 1;
            }
            catch (Exception ex)
            {
                response.data = "";
                response.msg = "Something went wrong! please try again later.";
                response.msgType = 2;
            }

            return response;
        }
    }
}
