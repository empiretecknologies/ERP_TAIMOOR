using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class FamilyMemberRepository : IFamilyMemberRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public FamilyMemberRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage QuickSearch(int employeeId, Common common)
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
                        string query = $@"SELECT FM.TRAN_ID, FM.EMP_ID, FM.F_NAME, FM.CNIC, FM.DOB, R.GROUP_NAME RELATION,
                                        CASE WHEN FM.ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS
                                        FROM {table} FM 
                                        LEFT OUTER JOIN TBL_RELATION R ON R.GROUP_CODE = FM.RELATION 
                                        WHERE FM.DLT = 'T' AND FM.ASTATUS = 'Y' AND FM.EMP_ID = {employeeId}";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new FamilyMember
                            {
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                EMP_ID = Convert.ToInt32(reader["EMP_ID"]),
                                F_NAME = Convert.ToString(reader["F_NAME"]),
                                GENDER = Convert.ToString(reader["RELATION"]),
                                DOB = Convert.ToDateTime(reader["DOB"]),
                                CNIC = Convert.ToString(reader["CNIC"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
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

        public MyHttpResponseMessage Save(FamilyMember modelRecord, Common common)
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
                            if (modelRecord.TRAN_ID == null || modelRecord.TRAN_ID == 0)
                            {
                                string code = GenerateNextId(common);
                                query = "INSERT INTO " + table + " " +
                                            "([TRAN_ID], [EMP_ID], [F_NAME], [RELATION], [DOB], [GENDER], [CNIC], [CNIC_EXP], [MSTATUS], [INSTITUTE], [EDUCATION], [REMARKS], " +
                                            "[ADD_USER_ID], [ADD_DATE], [ADD_COMPUTER_NAME], [ADD_IP_ADDRESS], [ADD_POSTALCODE], [ASTATUS], [MENU_ID], [DLT])" +
                                            "VALUES" +
                                            "('" + code + "','" + modelRecord.EMP_ID + "','" + modelRecord.F_NAME + "','" + modelRecord.RELATION + "','" + modelRecord.DOB + "','" + modelRecord.GENDER + "'," +
                                            "'" + modelRecord.CNIC + "','" + modelRecord.CNIC_EXP + "','" + modelRecord.MSTATUS + "','" + modelRecord.INSTITUTE + "','" + modelRecord.EDUCATION + "','" + modelRecord.REMARKS + "'," +
                                            "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                            "'" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                            "'" + common.MenuID + "','T')";
                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                response.msgType = 1;
                                response.msg = "Family Member Added Successfully";
                                response.data = new
                                {
                                    code = code
                                };
                                response.data = GenerateNextId(common);
                            }
                            else
                            {
                                query = "UPDATE " + table + " SET " +
                                    "F_NAME = '" + modelRecord.F_NAME + @"',
                                    RELATION = '" + modelRecord.RELATION + @"',
                                    DOB = '" + modelRecord.DOB + @"',
                                    GENDER = '" + modelRecord.GENDER + @"',
                                    CNIC = '" + modelRecord.CNIC + @"',
                                    CNIC_EXP = '" + modelRecord.CNIC_EXP + @"',
                                    MSTATUS = '" + modelRecord.MSTATUS + @"',
                                    INSTITUTE = '" + modelRecord.INSTITUTE + @"',
                                    EDUCATION = '" + modelRecord.EDUCATION + @"',
                                    REMARKS = '" + modelRecord.REMARKS + @"',
                                    EDIT_USER_ID = '" + userid + @"',
                                    EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                    EDIT_COMPUTER_NAME = '" + Computer + @"',
                                    EDIT_IP_ADDRESS = '" + Ip + @"',
                                    EDIT_POSTALCODE = '" + Postal + @"',
                                    ASTATUS = '" + modelRecord.ASTATUS + @"' 
                                    WHERE TRAN_ID = '" + modelRecord.TRAN_ID + "' AND MENU_ID = '" + common.MenuID + "'";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                response.msgType = 1;
                                response.msg = "Family Member Updated Successfully";
                                response.data = new
                                {
                                    code = modelRecord.TRAN_ID
                                };
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
                    string maxIdQuery = "SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM " + table;
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

        public MyHttpResponseMessage GetFamilyMemberById(int id, Common common)
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
                        string query = $@"SELECT * FROM {table} WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' AND TRAN_ID = '" + id + "'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var user = new 
                            {
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                EMP_ID = Convert.ToInt32(reader["EMP_ID"]),
                                F_NAME = Convert.ToString(reader["F_NAME"]),
                                RELATION = Convert.ToInt32(reader["RELATION"]),
                                DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd"),
                                GENDER = Convert.ToString(reader["GENDER"]),
                                CNIC = Convert.ToString(reader["CNIC"]),
                                CNIC_EXP = reader["CNIC_EXP"] == DBNull.Value ? null : Convert.ToDateTime(reader["CNIC_EXP"]).ToString("yyyy-MM-dd"),
                                MSTATUS = Convert.ToString(reader["MSTATUS"]),
                                INSTITUTE = Convert.ToString(reader["INSTITUTE"]),
                                EDUCATION = Convert.ToString(reader["EDUCATION"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE TRAN_ID = '" + id + "'";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            response.msg = "Family Member Deleted Successfully";
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

        public MyHttpResponseMessage SaveFamilyMemberOTP(OTPFamilyMember user)
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

        public MyHttpResponseMessage OTPVerification(OTPFamilyMember user)
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
                            OTPFamilyMember otpFamilyMember = new OTPFamilyMember();
                            otpFamilyMember.U_ID = user.U_ID;
                            otpFamilyMember.STATUS = "EXPIRED";
                            UpdateFamilyMemberOTPStatus(otpFamilyMember);
                            response.msg = "OTP has been expired!";
                            response.msgType = 2;
                        }
                        else
                        {
                            if (user.OTP == OTP)
                            {
                                OTPFamilyMember otpFamilyMember = new OTPFamilyMember();
                                otpFamilyMember.U_ID = user.U_ID;
                                otpFamilyMember.STATUS = "APPROVED";
                                UpdateFamilyMemberOTPStatus(otpFamilyMember);
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

        public MyHttpResponseMessage UpdateFamilyMemberOTPStatus(OTPFamilyMember user)
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