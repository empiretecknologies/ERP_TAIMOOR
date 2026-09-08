using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class MembershipCardRepository : IMembershipCardRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public MembershipCardRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonRepository commonRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
        }

        public MyHttpResponseMessage GetMembershipCards(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<MembershipCard> chartOfAccounts = new List<MembershipCard>();
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
                            MembershipCard chartOfAccount = new MembershipCard();
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
                List<MembershipCard> chartOfAccounts = new List<MembershipCard>();
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
                            MembershipCard chartOfAccount = new MembershipCard();
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
                        string query = @"SELECT A.TRAN_ID, A.V_DATE, FIRST_NAME, LAST_NAME, CNIC_NO, PHONE_NO, EMAIL, COMMENT, START_D, END_D, A.PERIOD_ID, 
                            TC.GROUP_NAME AS CARD_TYPE, A.CARD_NO,
                            CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, CASE WHEN A.CLOSED = 1 THEN 'Yes' ELSE 'No' END AS CLOSED, A.MENU_ID, A.DLT " +
                            " FROM " + table + " A " +
                            " LEFT JOIN TBL_CARD_TYPE TC ON TC.GROUP_CODE = A.CARD_TYPE" +
                            " WHERE A.DLT = 'T' " +
                            " ORDER BY TRAN_ID DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = reader["TRAN_ID"].ToString(),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                FIRST_NAME = reader["FIRST_NAME"].ToString(),
                                LAST_NAME = reader["LAST_NAME"].ToString(),
                                CARD_NO = reader["CARD_NO"].ToString(),
                                CNIC_NO = reader["CNIC_NO"].ToString(),
                                PHONE_NO = reader["PHONE_NO"].ToString(),
                                EMAIL = reader["EMAIL"].ToString(),
                                COMMENT = reader["COMMENT"].ToString(),
                                START_D = reader["START_D"] == DBNull.Value ? null : Convert.ToDateTime(reader["START_D"]).ToString("yyyy-MM-dd"),
                                END_D = reader["END_D"] == DBNull.Value ? null : Convert.ToDateTime(reader["END_D"]).ToString("yyyy-MM-dd"),
                                CLOSED = reader["CLOSED"].ToString(),
                                CARD_TYPE = reader["CARD_TYPE"].ToString(),
                                PERIOD_ID = reader["PERIOD_ID"].ToString(),
                                ASTATUS = reader["ASTATUS"].ToString(),
                                MENU_ID = reader["MENU_ID"].ToString()
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM {table} WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
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

        public MyHttpResponseMessage Save(MembershipCard modelRecord, Core.Entities.Common common)
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
                    List<CustomPartyType> partiesData = DropdownService.CustomPartyTypeDropdownWithAccountCode(common.RoleID, common.RoleType);
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
                            string checkCardQuery = @$"SELECT COUNT(*) 
                                                    FROM {table} 
                                                    WHERE CARD_NO = '{modelRecord.CARD_NO}' AND TRAN_ID <> '{modelRecord.TRAN_ID}' ANd DLT = 'T'";

                            SqlCommand checkCmd = new SqlCommand(checkCardQuery, connection, transaction);
                            int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (exists > 0)
                            {
                                transaction.Rollback();
                                response.data = null;
                                response.msgType = 2;
                                response.msg = "Card already added. Try new one.";
                                return response;

                            }


                            if (modelRecord.TRAN_ID == 0 || modelRecord.TRAN_ID is null)
                            {

                                code = GenerateNextId(common, command);
                                modelRecord.TRAN_ID = code;

                                query = "INSERT INTO " + table + " " +
                                        "([TRAN_ID],[V_DATE], FIRST_NAME, LAST_NAME, CNIC_NO, PHONE_NO, EMAIL, COMMENT, START_D, END_D, CLOSED, CARD_TYPE, POINT_START_VALUE , POINT_RATE , MAX_POINTS_DISC ," +
                                        "DISC , CARD_NO, [BCODE], [PERIOD_ID]," +
                                        "[ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[ADD_POSTALCODE]," +
                                        "[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME],[EDIT_IP_ADDRESS],[EDIT_POSTALCODE],[MENU_ID],[ASTATUS],[DLT])" +
                                        "VALUES " +
                                        "('" + GenerateNextId(common) + "','" + modelRecord.V_DATE + "','" + modelRecord.FIRST_NAME + "','" + modelRecord.LAST_NAME + "','" + modelRecord.CNIC_NO + "','" + modelRecord.PHONE_NO + "','" + modelRecord.EMAIL + "','" + modelRecord.COMMENT + "','" + modelRecord.START_D + "'," +
                                        "'" + modelRecord.END_D + "','" + modelRecord.CLOSED + "','" + modelRecord.CARD_TYPE + "','" + modelRecord.PointStartValue + "','" + modelRecord.PointRate + "','" + modelRecord.MaxPointsBeforeDiscount + "'," +
                                        "'" + modelRecord.DISCOUNT + "','" + modelRecord.CARD_NO + "','" + common.Branch + "','" + common.Period + "'," +
                                        "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + Postal + "'," +
                                        "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + Postal + "','" + common.MenuID + "','" + modelRecord.ASTATUS + "','T')";

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
                                        "[V_DATE] = '" + modelRecord.V_DATE + @"',
                                        [FIRST_NAME] = '" + modelRecord.FIRST_NAME + @"',
                                        [LAST_NAME] = '" + modelRecord.LAST_NAME + @"',
                                        [CNIC_NO] = '" + modelRecord.CNIC_NO + @"',
                                        [PHONE_NO] = '" + modelRecord.PHONE_NO + @"',
                                        [EMAIL] = '" + modelRecord.EMAIL + @"',
                                        [COMMENT] = '" + modelRecord.COMMENT + @"',
                                        [START_D] = '" + modelRecord.START_D + @"',
                                        [END_D] = '" + modelRecord.END_D + @"',
                                        [CLOSED] = '" + modelRecord.CLOSED + @"',
                                        [CARD_TYPE] = '" + modelRecord.CARD_TYPE + @"',
                                        [POINT_START_VALUE] = '" + modelRecord.PointStartValue + @"',
                                        [POINT_RATE] = '" + modelRecord.PointRate + @"',
                                        [MAX_POINTS_DISC] = '" + modelRecord.MaxPointsBeforeDiscount + @"',
                                        [DISC] = '" + modelRecord.DISCOUNT + @"',
                                  [EDIT_USER_ID] = '" + userid + @"',
                                  [EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                  [EDIT_COMPUTER_NAME] = '" + Computer + @"',
                                  [EDIT_IP_ADDRESS] = '" + Ip + @"',
                                  [MENU_ID] = '" + common.MenuID + @"',
                                  [EDIT_POSTALCODE] = '" + Postal + @"',
                                  [ASTATUS] = '" + modelRecord.ASTATUS + @"',
                                  [DLT] = 'T' 
                                    WHERE [TRAN_ID] = '" + modelRecord.TRAN_ID + @"'";

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

        public MyHttpResponseMessage GetMembershipCardById(int id, Core.Entities.Common common)
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
                        string query = "SELECT [TRAN_ID],[V_DATE], FIRST_NAME, LAST_NAME, CNIC_NO, PHONE_NO, EMAIL, COMMENT, CARD_NO, START_D, END_D, CLOSED, CARD_TYPE,POINT_START_VALUE,POINT_RATE,MAX_POINTS_DISC, [BCODE],[PERIOD_ID],[ADD_USER_ID],[ADD_DATE]," +
                            "[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME],[EDIT_IP_ADDRESS],[ADD_POSTALCODE],[EDIT_POSTALCODE]," +
                            "[ASTATUS],[MENU_ID],[DLT] FROM " + table + " WHERE TRAN_ID = @Id";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new
                            {
                                ID = reader["TRAN_ID"],
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                FIRST_NAME = reader["FIRST_NAME"].ToString(),
                                LAST_NAME = reader["LAST_NAME"].ToString(),
                                CNIC_NO = reader["CNIC_NO"].ToString(),
                                PHONE_NO = reader["PHONE_NO"].ToString(),
                                EMAIL = reader["EMAIL"].ToString(),
                                COMMENT = reader["COMMENT"].ToString(),
                                START_D = reader["START_D"] == DBNull.Value ? null : Convert.ToDateTime(reader["START_D"]).ToString("yyyy-MM-dd"),
                                END_D = reader["END_D"] == DBNull.Value ? null : Convert.ToDateTime(reader["END_D"]).ToString("yyyy-MM-dd"),
                                CLOSED = reader["CLOSED"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CLOSED"]),
                                CARD_TYPE = reader["CARD_TYPE"].ToString(),
                                pointStartValue = reader["POINT_START_VALUE"].ToString(),
                                pointRate = reader["POINT_RATE"].ToString(),
                                maxPointDisc = reader["MAX_POINTS_DISC"].ToString(),
                                CARD_NO = reader["CARD_NO"].ToString(),
                                BCODE = reader["BCODE"],
                                PERIOD_ID = reader["PERIOD_ID"],
                                ADD_USER_ID = reader["ADD_USER_ID"],
                                ADD_DATE = reader["ADD_DATE"],
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"],
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"],
                                EDIT_USER_ID = reader["EDIT_USER_ID"],
                                EDIT_DATE = reader["EDIT_DATE"],
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"],
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"],
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"],
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"],
                                ASTATUS = reader["ASTATUS"],
                                MENU_ID = reader["MENU_ID"],
                                DLT = reader["DLT"]
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

        public MyHttpResponseMessage GetDataForReport(MembershipCardReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            MembershipCardReport masterData = new MembershipCardReport();
            CustomDeliveryFeedingForPrintReport reportData = new CustomDeliveryFeedingForPrintReport();
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
                if (menuDetails.MD_ID == 13)
                {
                    topQuery = @"SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, P.PARTY_NAME AS PARTY_CODE, A.ACT_CODE, A.S_DATE, A.S_NO, B.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE, A.GODOWN," +
                                    " A.KANTA, A.COND, A.C_NAME, A.CELL, A.LOT_NO, A.ORIGIN, IM.ITEM_NAME AS ITEM_CODE, UN.GROUP_NAME AS UNIT,A.TBAG, A.BCODE, A.PERIOD_ID, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, A.MENU_ID, A.DLT " +
                                    $" FROM {table} A " +
                                    "LEFT OUTER JOIN TBL_PARTY_TYPES P ON A.PARTY_CODE = P.PARTY_CODE AND P.ACT_CODE = A.ACT_CODE " +
                                    "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BROKER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BD_ACT_CODE " +
                                    "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE " +
                                    "LEFT OUTER JOIN TBL_UNIT UN ON A.UNIT = UN.GROUP_CODE " +
                                    $"WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' AND A.TRAN_ID = '{modelRecord.TRAN_ID}' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}'";
                }

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
                        masterData.V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                        masterData.VOUCHER_NO = reader["VOUCHER_NO"].ToString();
                        masterData.PARTY_CODE = reader["PARTY_CODE"].ToString();
                        masterData.S_DATE = reader["S_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["S_DATE"]).ToString("dd-MM-yyyy");
                        masterData.S_NO = reader["S_NO"].ToString();
                        masterData.BROKER_CODE = reader["BROKER_CODE"].ToString();
                        masterData.GODOWN = reader["GODOWN"].ToString();
                        masterData.KANTA = reader["KANTA"].ToString();
                        masterData.COND = reader["COND"].ToString();
                        masterData.C_NAME = reader["C_NAME"].ToString();
                        masterData.CELL = reader["CELL"].ToString();
                        masterData.LOT_NO = reader["LOT_NO"].ToString();
                        masterData.ORIGIN = reader["ORIGIN"].ToString();
                        masterData.ITEM_CODE = reader["ITEM_CODE"].ToString();
                        masterData.UNIT = reader["UNIT"].ToString();
                        masterData.TBAG = reader["TBAG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TBAG"]);
                    }
                    reader.Close();
                }

                //reportData.Master = masterData;
                //reportData.Detail = dataTable;
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