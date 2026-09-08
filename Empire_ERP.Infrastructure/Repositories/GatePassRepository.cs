using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class GatePassRepository : IGatePassRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public GatePassRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonRepository commonRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
        }

        public MyHttpResponseMessage GetGatePasss(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<GatePass> chartOfAccounts = new List<GatePass>();
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
                            GatePass chartOfAccount = new GatePass();
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
                List<GatePass> chartOfAccounts = new List<GatePass>();
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
                            GatePass chartOfAccount = new GatePass();
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
                        string query = @"SELECT COUNT(*) OVER() AS COUNT, A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, P.PARTY_NAME AS PARTY_CODE, 
                            L.LOT_NO, A.DUE_NO, A.DRIVER, A.VEHICLE, A.QTY, U.GROUP_NAME AS UNIT, IM.ITEM_NAME AS ITEM_CODE, 
                            A.BCODE, A.PERIOD_ID, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, A.MENU_ID, A.DLT " +
                            " FROM " + table + " A " +
                            "LEFT OUTER JOIN TBL_LOT L ON L.CODE = A.LOT " +
                            "LEFT OUTER JOIN TBL_UNIT U ON A.UNIT = U.GROUP_CODE " +
                            "LEFT OUTER JOIN TBL_PARTY_TYPES P ON A.PARTY_CODE = P.PARTY_CODE AND A.ACT_CODE = P.ACT_CODE " +
                            "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE " +
                            "WHERE A.DLT = 'T' " +
                            "ORDER BY A.TRAN_ID DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = reader["TRAN_ID"].ToString(),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = reader["VOUCHER_NO"].ToString(),
                                PARTY_CODE = reader["PARTY_CODE"].ToString(),
                                LOT_NO = reader["LOT_NO"].ToString(),
                                DUE_NO = reader["DUE_NO"].ToString(),
                                DRIVER = reader["DRIVER"].ToString(),
                                VEHICLE = reader["VEHICLE"].ToString(),
                                QTY = reader["QTY"].ToString(),
                                UNIT = reader["UNIT"].ToString(),
                                ITEM_CODE = reader["ITEM_CODE"].ToString(),
                                BCODE = reader["BCODE"].ToString(),
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

        private string GenerateVoucherNo(Common common, int code, string vDate)
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

                if (!String.IsNullOrWhiteSpace(shortName) && !String.IsNullOrWhiteSpace(prefix) && voucherLength > 0 && code > 0)
                {
                    //string paddedVoucherValue = "0".ToString().PadLeft(voucherLength - 1, '0') + code;
                    string paddedVoucherValue = code.ToString().PadLeft(voucherLength, '0');
                    return $"{shortName}/{prefix}/{Convert.ToDateTime(vDate).ToString("yy-MM")}/{paddedVoucherValue}";
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        public MyHttpResponseMessage Save(GatePass modelRecord, Core.Entities.Common common)
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
                    string voucherNo = modelRecord.VOUCHER_NO;
                    string connectionString = new SQLService().getconnstring();
                    List<CustomPartyType> partiesData = DropdownService.CustomPartyTypeDropdownWithAccountCodeForGatePass(0, common.Branch, 0);
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        var partyInformation = partiesData.ToList().Where(p => p.customizedKey == modelRecord.PARTY_CODE).FirstOrDefault();
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;
                        if (partyInformation != null)
                        {
                            modelRecord.PARTY_CODE = Convert.ToString(partyInformation.key);
                            modelRecord.ACT_CODE = partyInformation.accountCode;
                        }
                        try
                        {
                            string query = "";
                            string Duplicationquery = "";
                            if (modelRecord.TRAN_ID is 0 || modelRecord.TRAN_ID is null)
                            {

                                code = GenerateNextId(common, command);
                                modelRecord.TRAN_ID = code;
                                voucherNo = GenerateVoucherNo(common, code, CommonService.GetDateTime("Pakistan Standard Time"));

                                query = "INSERT INTO " + table + " " +
                                        "([TRAN_ID],[V_DATE],[VOUCHER_NO],[LOT],[DUE_NO],[DRIVER],[VEHICLE],[ITEM_CODE],[QTY],[UNIT],[PARTY_CODE],[ACT_CODE],[BCODE],[PERIOD_ID]," +
                                        "[ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[ADD_POSTALCODE]," +
                                        "[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME],[EDIT_IP_ADDRESS],[EDIT_POSTALCODE],[MENU_ID],[ASTATUS],[DLT])" +
                                        "VALUES " +
                                        "('" + GenerateNextId(common) + "','" + modelRecord.V_DATE + "','" + voucherNo + "','" + modelRecord.LOT + "','" + modelRecord.DUE_NO + "','" + modelRecord.DRIVER + "','" + modelRecord.VEHICLE + "','" + modelRecord.ITEM_CODE + "','" + modelRecord.QTY + "','" + modelRecord.UNIT + "','" + modelRecord.PARTY_CODE + "','" + modelRecord.ACT_CODE + "'," +
                                        "'" + common.Branch + "','" + common.Period + "'," +
                                        "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + Postal + "'," +
                                        "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + Postal + "','" + common.MenuID + "','" + modelRecord.ASTATUS + "','T')";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                response.data = code;
                                response.data2 = voucherNo;
                                response.msgType = 1;
                                response.msg = "Record Added Successfully";

                            }
                            else
                            {
                                query = "UPDATE " + table + " SET " +
                                        "[V_DATE] = '" + modelRecord.V_DATE + @"',
                                        [LOT] = '" + modelRecord.LOT + @"',
                                        [DUE_NO] = '" + modelRecord.DUE_NO + @"',
                                        [DRIVER] = '" + modelRecord.DRIVER + @"',
                                        [VEHICLE] = '" + modelRecord.VEHICLE + @"',
                                        [ITEM_CODE] = '" + modelRecord.ITEM_CODE + @"',
                                        [QTY] = '" + modelRecord.QTY + @"',
		                                [UNIT] = '" + modelRecord.UNIT + @"',
		                                [PARTY_CODE] = '" + modelRecord.PARTY_CODE + @"',
		                                [ACT_CODE] = '" + modelRecord.ACT_CODE + @"',
		                                [EDIT_USER_ID] = '" + userid + @"',
		                                [EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
		                                [EDIT_COMPUTER_NAME] = '" + Computer + @"',
		                                [EDIT_IP_ADDRESS] = '" + Ip + @"',
		                                [MENU_ID] = '" + common.MenuID + @"',
		                                [EDIT_POSTALCODE] = '" + Postal + @"',
		                                [ASTATUS] = '" + modelRecord.ASTATUS + @"'
                                    WHERE [TRAN_ID] = '" + modelRecord.TRAN_ID + @"'";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                response.data = modelRecord.TRAN_ID;
                                response.data2 = voucherNo;
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

        public MyHttpResponseMessage GetGatePassById(int id, Core.Entities.Common common)
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
                        string query = "SELECT [TRAN_ID],[V_DATE],[VOUCHER_NO],[LOT],[DUE_NO],[DRIVER],[VEHICLE],[ITEM_CODE],[QTY],[UNIT],[PARTY_CODE],[ACT_CODE],[BCODE],[PERIOD_ID],[ADD_USER_ID],[ADD_DATE]," +
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
                                PARTY_CODE = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",
                                ACT_CODE = reader["ACT_CODE"],
                                VOUCHER_NO = reader["VOUCHER_NO"],
                                LOT = reader["LOT"],
                                DUE_NO = reader["DUE_NO"],
                                DRIVER = reader["DRIVER"],
                                VEHICLE = reader["VEHICLE"],
                                ITEM_CODE = reader["ITEM_CODE"],
                                QTY = reader["QTY"],
                                UNIT = reader["UNIT"],
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

        public MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string? table = menu.TABLE1;
                string? table2 = menu.TABLE2;
                string connectionString = new SQLService().getconnstring();
                GatePass gatePass = new GatePass();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    //string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        gatePass = new GatePass
                        {
                            TRAN_ID = 0,
                            V_DATE = record.V_DATE,
                            LOT = Convert.ToInt32(reader["LOT"]),
                            DUE_NO = Convert.ToString(reader["DUE_NO"]),
                            DRIVER = Convert.ToString(reader["DRIVER"]),
                            VEHICLE = Convert.ToString(reader["VEHICLE"]),
                            ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            QTY = Convert.ToDecimal(reader["QTY"]),
                            UNIT = Convert.ToInt32(reader["UNIT"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            PARTY_CODE = Convert.ToString(reader["PARTY_CODE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                        };
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(gatePass, common);
                if (response.msgType == 1)
                {
                    response.msg = "Record Copied Successfully";
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

        public MyHttpResponseMessage GetDataForReport(GatePassReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            GatePassReport masterData = new GatePassReport();
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
                if (menuDetails.MD_ID == 31)
                {
                    topQuery = @"SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO,  P.PARTY_NAME AS PARTY_CODE, L.LOT_NO, IM.ITEM_NAME AS ITEM_CODE, UN.GROUP_NAME AS UNIT, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, A.DUE_NO, A.DRIVER, A.VEHICLE, A.QTY " +
                                    $"FROM {table} A " +
                                    "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE " +
                                    "LEFT OUTER JOIN TBL_LOT L ON A.LOT = L.CODE " +
                                    "LEFT OUTER JOIN TBL_UNIT UN ON A.UNIT = UN.GROUP_CODE " +
                                    "LEFT OUTER JOIN TBL_PARTY_TYPES P ON A.PARTY_CODE = P.PARTY_CODE AND P.ACT_CODE = A.ACT_CODE " +
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
                        masterData.MENU_SIG1 = $"{menuDetails.MENU_SIG1}";
                        masterData.MENU_SIG2 = $"{menuDetails.MENU_SIG2}";
                        masterData.MENU_SIG3 = $"{menuDetails.MENU_SIG3}";
                        masterData.MENU_SIG4 = $"{menuDetails.MENU_SIG4}";
                        masterData.MENU_TERMS = $"{menuDetails.MENU_TERMS}";
                        masterData.V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                        masterData.VOUCHER_NO = reader["VOUCHER_NO"].ToString();
                        masterData.PARTY_CODE = reader["PARTY_CODE"].ToString();
                        masterData.LOT_NO = reader["LOT_NO"].ToString();
                        masterData.ITEM_CODE = reader["ITEM_CODE"].ToString();
                        masterData.UNIT = reader["UNIT"].ToString();
                        masterData.DUE_NO = reader["DUE_NO"].ToString();
                        masterData.DRIVER = reader["DRIVER"].ToString();
                        masterData.VEHICLE = reader["VEHICLE"].ToString();
                        masterData.QUANTITY = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]);
                    }
                    reader.Close();
                }

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