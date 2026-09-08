using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class HROfferLetterRepository : IHROfferLetterRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public HROfferLetterRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonRepository commonRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
        }

        public MyHttpResponseMessage GetHRJobPosts(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<HRCandidate> chartOfAccounts = new List<HRCandidate>();
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
                        string query = $"SELECT A.ACT_CODE, A.ACT_NAME,CASE WHEN  C.ACT_NAME IS NULL THEN '' ELSE C.ACT_NAME END AS CONRTOL_NAME" +
                            $" FROM {table} A" +
                            $" LEFT OUTER JOIN {table} B ON A.ACT_GR_CODE LIKE CONCAT('', B.ACT_GR_CODE, '%') AND B.ASTATUS <> 'Y'" +
                            $" LEFT OUTER JOIN {table} C ON C.ACT_CODE = A.ACT_PARENT_CODE" +
                            $" WHERE A.DLT = 'T' AND A.ACT_TYPE = 'C' AND B.ASTATUS IS NULL" +
                            $" GROUP BY A.ACT_CODE, A.ACT_NAME,A.ASTATUS,A.ACT_GR_CODE,B.ASTATUS,C.ACT_NAME";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            HRCandidate chartOfAccount = new HRCandidate();
                            //chartOfAccount.ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]);
                            //chartOfAccount.ACT_NAME = Convert.ToString(reader["ACT_NAME"]);
                            //chartOfAccount.ACT_SNAME = Convert.ToString(reader["CONRTOL_NAME"]);
                            chartOfAccounts.Add(chartOfAccount);
                        }
                        reader.Close();
                    }

                    response.data = chartOfAccounts;
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

        public MyHttpResponseMessage GetAccountsForTreeView(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<HRCandidate> chartOfAccounts = new List<HRCandidate>();
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
                        string query = "SELECT A.ACT_GR_CODE + '-' + A.ACT_NAME AS ACT_NAME, A.ACT_CODE, A.ACT_PARENT_CODE " +
                                       "FROM " + table + " A WHERE A.DLT = 'T' " +
                                       "ORDER BY A.ACT_GR_CODE";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            HRCandidate chartOfAccount = new HRCandidate();
                            string actName = Convert.ToString(reader["ACT_NAME"]);
                            int actCode = Convert.ToInt32(reader["ACT_CODE"]);
                            int actParentCode = Convert.ToInt32(reader["ACT_PARENT_CODE"]);

                            //chartOfAccount.ACT_NAME = actName;
                            //chartOfAccount.ACT_CODE = actCode;
                            // chartOfAccount.ACT_PARENT_CODE = actParentCode;
                            chartOfAccounts.Add(chartOfAccount);
                        }
                        reader.Close();
                    }
                    response.data = chartOfAccounts;
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

        public MyHttpResponseMessage QuickSearch(Core.Entities.Common common)
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

                        string query = $@"SELECT OL.TRAN_ID 
                                        ,CAN.FULL_NAME AS CAN_NAME
                                        ,OL.JOB_TITLE
                                        ,EMP.DESCR AS DEP_NAME
                                        ,OL.BASIC_SALARY
                                        ,OL.ALLOWANCES
                                        ,OL.TOTAL_PACKAGE
                                        ,OL.JOINING_DATE
                                        ,OL.OFFER_DATE
                                        ,OL.DUTY_START
                                        ,OL.DUTY_END
                                        ,OL.WEEKLY_OFF
                                        ,OL.WEEKLY_DESC
                                        ,OL.ASTATUS
                                        FROM TBL_OFFER_LETTER OL
                                        LEFT OUTER JOIN TBL_INTERVIEW_SCH INTS ON INTS.TRAN_ID = OL.INT_ID
                                        LEFT OUTER JOIN TBL_CANDIDATES CAN ON CAN.TRAN_ID = INTS.CON_ID
                                        LEFT OUTER JOIN TBL_ACT_GROUP EMP ON EMP.CODE = OL.DEP_ID
                                        WHERE OL.DLT = 'T' ORDER BY TRAN_ID DESC";



                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = reader["TRAN_ID"].ToString(),
                                CAN_NAME = reader["CAN_NAME"].ToString(),
                                JOB_TITLE = reader["JOB_TITLE"].ToString(),
                                DEP_NAME = reader["DEP_NAME"].ToString(),
                                BASIC_SALARY = Convert.ToDecimal(reader["BASIC_SALARY"]),
                                ALLOWANCES = Convert.ToDecimal(reader["ALLOWANCES"]),
                                TOTAL_PACKAGE = Convert.ToDecimal(reader["TOTAL_PACKAGE"]),
                                JOINING_DATE = reader["JOINING_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["JOINING_DATE"]).ToString("dd-MM-yyyy"),
                                OFFER_DATE = reader["OFFER_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["OFFER_DATE"]).ToString("dd-MM-yyyy"),
                                DUTY_START = reader["DUTY_START"] == DBNull.Value ? null : ((TimeSpan)reader["DUTY_START"]).ToString(@"hh\:mm"),
                                DUTY_END = reader["DUTY_END"] == DBNull.Value ? null : ((TimeSpan)reader["DUTY_END"]).ToString(@"hh\:mm"),
                                WEEKLY_OFF = reader["WEEKLY_OFF"].ToString(),
                                WEEKLY_DESC = reader["WEEKLY_DESC"].ToString(),
                                ASTATUS = reader["ASTATUS"].ToString(),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();

                        response.data = jsonDataResult;
                        response.msg = "";
                        response.msgType = 1;
                        return response;
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM {table} ";
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

        public string GenerateCardNo(Common common)
        {
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? prefix = string.Empty, shortName = string.Empty;
                int voucherLength = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    voucherLength = Convert.ToInt32(menu.VOUCHER_LEN);
                    prefix = menu.PERFIX;
                }

                var branchData = _branchRepository.GetBranchByCode(common.Branch);
                if (branchData.data != null)
                {
                    var branch = (Branch)branchData.data;
                    shortName = branch.B_SHORT_NAME;
                }

                if (!String.IsNullOrWhiteSpace(shortName))
                {
                    return $"{shortName}-{GenerateRandomNumber(voucherLength)}";
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        public string GenerateRandomNumber(int length)
        {
            if (length <= 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than 0.");

            Random random = new Random();
            char[] digits = new char[length];

            digits[0] = (char)('1' + random.Next(0, 9));

            for (int i = 1; i < length; i++)
            {
                digits[i] = (char)('0' + random.Next(0, 10));
            }

            return new string(digits);
        }

        public MyHttpResponseMessage Save(HROfferLetter modelRecord, Core.Entities.Common common)
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
                    int code = 0;
                    string connectionString = new SQLService().getconnstring();
                    //List<CustomPartyType> partiesData = DropdownService.CustomPartyTypeDropdownWithAccountCode(common.RoleID, common.RoleType);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;
                        try
                        {
                            string query = "";
                            //string Duplicationquery = "";
                            if (modelRecord.TRAN_ID == 0 || modelRecord.TRAN_ID is null)
                            {

                                code = GenerateNextId(common, command);
                                modelRecord.TRAN_ID = code;

                                query = "INSERT INTO " + table + " " +
                                                "([TRAN_ID],INT_ID, JOB_TITLE, DEP_ID, BASIC_SALARY, ALLOWANCES, TOTAL_PACKAGE, JOINING_DATE, OFFER_DATE, DUTY_START, DUTY_END, WEEKLY_OFF, WEEKLY_DESC, DOC, ASTATUS," +
                                                "[ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[ADD_POSTALCODE], " +
                                                "[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME],[EDIT_IP_ADDRESS],[EDIT_POSTALCODE],[MENU_ID],[DLT]) " +
                                                "VALUES " +
                                                "('" + modelRecord.TRAN_ID + "','"
                                                + modelRecord.INT_ID + "','"
                                                + modelRecord.JOB_TITLE + "','"
                                                + modelRecord.DEP_ID + "','"
                                                + modelRecord.BASIC_SALARY + "','"
                                                + modelRecord.ALLOWANCES + "','"
                                                + modelRecord.TOTAL_PACKAGE + "','"
                                                + modelRecord.JOINING_DATE + "','"
                                                + modelRecord.OFFER_DATE + "','"
                                                + modelRecord.DUTY_START + "','"
                                                + modelRecord.DUTY_END + "','"
                                                + modelRecord.WEEKLY_OFF + "','"
                                                + modelRecord.WEEKLY_DESC + "','"
                                                + modelRecord.DOC + "','"
                                                + modelRecord.ASTATUS + "','"
                                                + userid + "','"
                                                + CommonService.GetDateTime("Pakistan Standard Time") + "','"
                                                + Computer + "','"
                                                + Ip + "','"
                                                + Postal + "','"
                                                + userid + "','"
                                                + CommonService.GetDateTime("Pakistan Standard Time") + "','"
                                                + Computer + "','"
                                                + Ip + "','"
                                                + Postal + "','"
                                                + common.MenuID + "','T')";


                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();

                                response.data = code;
                                response.msgType = 1;
                                response.msg = "Record Added Successfully";

                            }
                            else
                            {
                                query = "UPDATE " + table + " SET " +
                                        "[INT_ID] = '" + modelRecord.INT_ID + "', " +
                                        "[JOB_TITLE] = '" + modelRecord.JOB_TITLE + "', " +
                                        "[DEP_ID] = '" + modelRecord.DEP_ID + "', " +
                                        "[BASIC_SALARY] = '" + modelRecord.BASIC_SALARY + "', " +
                                        "[ALLOWANCES] = '" + modelRecord.ALLOWANCES + "', " +
                                        "[TOTAL_PACKAGE] = '" + modelRecord.TOTAL_PACKAGE + "', " +
                                        "[JOINING_DATE] = '" + modelRecord.JOINING_DATE + "', " +
                                        "[OFFER_DATE] = '" + modelRecord.OFFER_DATE + "', " +
                                        "[DUTY_START] = '" + modelRecord.DUTY_START + "', " +
                                        "[DUTY_END] = '" + modelRecord.DUTY_END + "', " +
                                        "[WEEKLY_OFF] = '" + modelRecord.WEEKLY_OFF + "', " +
                                        "[WEEKLY_DESC] = '" + modelRecord.WEEKLY_DESC + "', " +
                                        "[DOC] = '" + modelRecord.DOC + "', " +
                                        "[ASTATUS] = '" + modelRecord.ASTATUS + "', " +
                                        "[EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + "', " +
                                        "[EDIT_COMPUTER_NAME] = '" + Computer + "', " +
                                        "[EDIT_IP_ADDRESS] = '" + Ip + "', " +
                                        "[EDIT_POSTALCODE] = '" + Postal + "', " +
                                        "[MENU_ID] = '" + common.MenuID + "'" +
                                        "WHERE [TRAN_ID] = '" + modelRecord.TRAN_ID + "'";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                response.data = modelRecord.TRAN_ID;
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

        public string GenerateNextId(Core.Entities.Common common)
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
                    string maxIdQuery = "SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM " + table + "";
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

        public string GenerateGrCode(string ParentId, Core.Entities.Common common)
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
                    string maxIdQuery = "SELECT CASE WHEN (select max(convert(bigint,ACT_GR_CODE))+1 from " + table + " where DLT = 'T' )   IS NULL THEN '01' " +
                    "WHEN(SELECT COUNT(ACT_PARENT_CODE)FROM " + table + " where ACT_PARENT_CODE= '" + ParentId + "' AND DLT = 'T' ) = 0 THEN '0'+CONVERT(NVARCHAR(100)," +
                    "(select (max(CONVERT(BIGINT,(ACT_GR_CODE)+'01'))) as act_groupCode from " + table + "  where ACT_CODE= '" + ParentId + "' AND DLT = 'T') )" +
                    " ELSE '0'+CONVERT(NVARCHAR(100),(select (max(CONVERT(BIGINT,(ACT_GR_CODE+1)))) as act_groupCode" +
                    " from " + table + " where ACT_PARENT_CODE= '" + ParentId + "' AND DLT = 'T' ) ) END As ACCOUNT_GROUP_CODE";

                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(maxIdQuery, connection);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        return Convert.ToString(Convert.ToInt64(result));
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

        public MyHttpResponseMessage GetHRJobPostById(int id, Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                object json = null;
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
                        string query = $@"SELECT 
                                            TRAN_ID,
                                            INT_ID,
                                            JOB_TITLE,
                                            DEP_ID,
                                            BASIC_SALARY,
                                            ALLOWANCES,
                                            TOTAL_PACKAGE,
                                            JOINING_DATE,
                                            OFFER_DATE,
                                            DUTY_START,
                                            DUTY_END,
                                            WEEKLY_OFF,
                                            WEEKLY_DESC,
                                            DOC,
                                            ASTATUS 
                                            FROM {table} WHERE TRAN_ID = @Id";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new
                            {
                                TRAN_ID = reader["TRAN_ID"].ToString(),
                                INT_ID = Convert.ToInt32(reader["INT_ID"]),
                                JOB_TITLE = reader["JOB_TITLE"].ToString(),
                                DEP_ID = Convert.ToInt32(reader["DEP_ID"]),
                                BASIC_SALARY = Convert.ToDecimal(reader["BASIC_SALARY"]),
                                ALLOWANCES = Convert.ToDecimal(reader["ALLOWANCES"]),
                                TOTAL_PACKAGE = Convert.ToDecimal(reader["TOTAL_PACKAGE"]),
                                JOINING_DATE = reader["JOINING_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["JOINING_DATE"]).ToString("yyyy-MM-dd"),
                                OFFER_DATE = reader["OFFER_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["OFFER_DATE"]).ToString("yyyy-MM-dd"),
                                DUTY_START = reader["DUTY_START"] == DBNull.Value ? null : ((TimeSpan)reader["DUTY_START"]).ToString(@"hh\:mm"),
                                DUTY_END = reader["DUTY_END"] == DBNull.Value ? null : ((TimeSpan)reader["DUTY_END"]).ToString(@"hh\:mm"),
                                WEEKLY_OFF = reader["WEEKLY_OFF"].ToString(),
                                WEEKLY_DESC = reader["WEEKLY_DESC"].ToString(),
                                DOC = reader["DOC"].ToString(),
                                ASTATUS = reader["ASTATUS"].ToString(),
                            };
                            json = jsonDataResult;
                        }
                        reader.Close();
                    }

                    response.msg = "";
                    response.msgType = 1;
                    response.data = json;
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

        public MyHttpResponseMessage GetEmpMails(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                object json = null;

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = @"SELECT EMP_CODE, EMP_ID, ENAME, EMAIL FROM TBL_EMP_REG WHERE ASTATUS = 'Y' AND DLT = 'T' AND MSTATUS = 'Y'";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    var jsonDataList = new List<object>();

                    while (reader.Read())
                    {
                        jsonDataList.Add(new
                        {
                            EMP_CODE = Convert.ToInt32(reader["EMP_CODE"]),
                            EMP_ID = reader["EMP_ID"].ToString(),
                            ENAME = reader["ENAME"].ToString(),
                            EMAIL = reader["EMAIL"].ToString(),
                        });
                    }

                    json = jsonDataList;
                    reader.Close();
                }

                response.msg = "";
                response.msgType = 1;
                response.data = json;
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

        public MyHttpResponseMessage Delete(int id, Core.Entities.Common common)
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE TRAN_ID = '" + id + @"'";
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

        public MyHttpResponseMessage GetDataForReport(HROfferLetterReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            HROfferLetterReport masterData = new HROfferLetterReport();
            CustomHROfferLetterForPrintReport reportData = new CustomHROfferLetterForPrintReport();
            var Menu = _menuRepository.GetMenu(common.MenuID);
            string? table = string.Empty, detailTable = string.Empty;
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                table = menu.TABLE1;
            }
            try
            {
                string topQuery = "";
                    topQuery = $@"SELECT 
                                    OL.TRAN_ID ,
                                    CAN.FULL_NAME AS CAN_NAME,
                                    OL.JOB_TITLE,
                                    EMP.DESCR AS DEP_NAME,
                                    OL.BASIC_SALARY,                  
                                    OL.ALLOWANCES,
                                    OL.TOTAL_PACKAGE,
                                    OL.JOINING_DATE,
                                    OL.OFFER_DATE,
                                    OL.DUTY_START,
                                    OL.DUTY_END,
                                    OL.WEEKLY_OFF,                             
                                    OL.WEEKLY_DESC,
                                    OL.ASTATUS, 
                                    OL.MENU_ID,
                                    OL.DLT
                                    FROM {table} OL
                                    LEFT OUTER JOIN TBL_INTERVIEW_SCH INTS ON INTS.TRAN_ID = OL.INT_ID
                                    LEFT OUTER JOIN TBL_CANDIDATES CAN ON CAN.TRAN_ID = INTS.CON_ID
                                    LEFT OUTER JOIN TBL_ACT_GROUP EMP ON EMP.CODE = OL.DEP_ID
                                    WHERE OL.DLT = 'T' AND OL.TRAN_ID = {modelRecord.TRAN_ID} ";

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(topQuery, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        masterData.COMPANY_NAME = currentCompany.C_NAME;
                        masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                        masterData.COMPANY_PHONE = currentCompany.C_TEL;
                        masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                        masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                        masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                        masterData.SIG1 = $"{menuDetails.MENU_SIG1}";
                        masterData.SIG2 = $"{menuDetails.MENU_SIG2}";
                        masterData.SIG3 = $"{menuDetails.MENU_SIG3}";
                        masterData.SIG4 = $"{menuDetails.MENU_SIG4}";
                        masterData.CAN_NAME = reader["CAN_NAME"].ToString();
                        masterData.JOB_TITLE = reader["JOB_TITLE"].ToString();
                        masterData.DEP_NAME = reader["DEP_NAME"].ToString();
                        masterData.BASIC_SALARY = reader["BASIC_SALARY"].ToString();
                        masterData.ALLOWANCES = reader["ALLOWANCES"].ToString();
                        masterData.TOTAL_PACKAGE = reader["TOTAL_PACKAGE"].ToString();
                        masterData.JOINING_DATE = reader["JOINING_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["JOINING_DATE"]).ToString("dd-MM-yyyy");
                        masterData.OFFER_DATE = reader["OFFER_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["OFFER_DATE"]).ToString("dd-MM-yyyy");
                        masterData.DUTY_START = reader["DUTY_START"].ToString();
                        masterData.DUTY_END = reader["DUTY_END"].ToString();
                        masterData.WEEKLY_OFF = reader["WEEKLY_OFF"].ToString();
                        masterData.WEEKLY_DESC = reader["WEEKLY_DESC"].ToString();
                    }
                    reader.Close();
                }

                reportData.Master = masterData;
                reportData.Detail = dataTable;
                response.data = masterData;
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
    }
}