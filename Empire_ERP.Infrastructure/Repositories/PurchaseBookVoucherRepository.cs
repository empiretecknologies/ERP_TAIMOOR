using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json.Nodes;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class PurchaseBookVoucherRepository : IPurchaseBookVoucherRepository
    {
        public ICommonRepository _commonRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public IMenuRepository _menuRepository { get; set; }
        public PurchaseBookVoucherRepository(IBranchRepository branchRepository, ICommonRepository commonRepository, IMenuRepository menuRepository)
        {
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetChartOfAccounts(int? pType)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<KeyValuePair<int, string?>> dropdown = new List<KeyValuePair<int, string?>>();
                using (SqlConnection db = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "SELECT ACT_CODE,ACT_NAME FROM TBL_CHART WHERE ACT_NATURE = '" + pType + "' AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                    using (SqlCommand command = new SqlCommand(query, db))
                    {
                        db.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    int code = Convert.ToInt32(reader["ACT_CODE"]);
                                    string? name = Convert.ToString(reader["ACT_NAME"]);
                                    dropdown.Add(new KeyValuePair<int, string?>(code, name));
                                }
                            }
                        }
                    }
                }
                response.data = dropdown;
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

        public MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common)
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
                
                    string query = "SELECT * FROM (" +
                                   "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, C.DESCR AS CURR_CODE, A.CRATE, A.AMT, " +
                                   "CH.ACT_NAME AS BOOK_TYPE, A.REMARKS, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME," +
                                   "A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME,PT.PARTY_NAME, " +
                                   "A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' WHEN A.ASTATUS = 'N' THEN 'In-Active' ELSE '' END ASTATUS," +
                                   "ROW_NUMBER() OVER (PARTITION BY A.TRAN_ID ORDER BY A.ADD_DATE) AS ROW_NUM  " +
                                   "FROM " + table + " A " +
                                   "LEFT OUTER JOIN TBL_CURRENCY C " +
                                   "ON C.CODE = A.CURR_CODE " +
                                   "LEFT OUTER JOIN TBL_CHART CH " +
                                   "ON CH.ACT_CODE = A.BOOK_TYPE " +
                                   "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.ACT_CODE = A.ACT_CODE AND PT.PARTY_CODE = A.PARTY_CODE " +
                                   "WHERE A.DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "') T ORDER BY TRAN_ID DESC";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                            VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                            PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]),
                            CURR_CODE = Convert.ToString(reader["CURR_CODE"]),
                            CRATE = Convert.ToString(reader["CRATE"]),
                            AMT = Convert.ToDecimal(reader["AMT"]),
                            BOOK_TYPE = Convert.ToString(reader["BOOK_TYPE"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                            ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
                            ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                            ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                            EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                            EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
                            EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                            EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                            ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                            EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                            //DETAIL = GetCashReceiptDetails(Convert.ToString(reader["TRAN_ID"]), common, Menu)
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

        //public MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common, int skip, int take, string filter = null, string group = null)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        string? table = string.Empty;
        //        int? pType;
        //        var menu = (Menu)Menu.data;
        //        table = menu.TABLE1;
        //        pType = menu.PTYPE;
        //        List<object> jsonDataResult = new List<object>();
        //        int totalCount = 0;

        //        using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //        {
        //            string filterCondition = string.Empty;
        //            if (!string.IsNullOrEmpty(filter))
        //            {
        //                JsonNode jsonNode = JsonNode.Parse(filter);
        //                JsonArray jsonArray = jsonNode.AsArray();
        //                if (jsonArray.Count >= 15)
        //                {
        //                    jsonArray.RemoveAt(1);
        //                    jsonArray.RemoveAt(0);
        //                }
        //                filterCondition = _commonRepository.BuildFilterCondition(jsonArray);
        //                filterCondition = filterCondition
        //                    .Replace("iteM_CODE", "IM.ITEM_NAME")
        //                    //.Replace("partY_NAME", "PT.PARTY_NAME")
        //                    .Replace("qty", "D.QTY");
        //            }

        //            string query = "SELECT COUNT(*) FROM (" +
        //                           "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, C.DESCR AS CURR_CODE, A.CRATE, A.AMT, " +
        //                           "CH.ACT_NAME AS BOOK_TYPE, A.REMARKS, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME," +
        //                           "A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, PT.PARTY_NAME, " +
        //                           "A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' WHEN A.ASTATUS = 'N' THEN 'In-Active' ELSE '' END ASTATUS," +
        //                           "ROW_NUMBER() OVER (PARTITION BY A.TRAN_ID ORDER BY A.ADD_DATE) AS ROW_NUM  " +
        //                           "FROM " + table + " A " +
        //                           "LEFT OUTER JOIN TBL_CURRENCY C " +
        //                           "ON C.CODE = A.CURR_CODE " +
        //                           "LEFT OUTER JOIN TBL_CHART CH " +
        //                           "ON CH.ACT_CODE = A.BOOK_TYPE " +
        //                           "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.ACT_CODE = A.ACT_CODE AND PT.PARTY_CODE = A.PARTY_CODE " +
        //                           "WHERE A.DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "') T WHERE ROW_NUM = 1 " + filterCondition + "";

        //            SqlCommand countCommand = new SqlCommand(query, connection);
        //            connection.Open();
        //            totalCount = (int)countCommand.ExecuteScalar();
        //            connection.Close();

        //            if (string.IsNullOrEmpty(group))
        //            {
        //                query = "SELECT * FROM (" +
        //                           "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, C.DESCR AS CURR_CODE, A.CRATE, A.AMT, " +
        //                           "CH.ACT_NAME AS BOOK_TYPE, A.REMARKS, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME," +
        //                           "A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME,PT.PARTY_NAME, " +
        //                           "A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' WHEN A.ASTATUS = 'N' THEN 'In-Active' ELSE '' END ASTATUS," +
        //                           "ROW_NUMBER() OVER (PARTITION BY A.TRAN_ID ORDER BY A.ADD_DATE) AS ROW_NUM  " +
        //                           "FROM " + table + " A " +
        //                           "LEFT OUTER JOIN TBL_CURRENCY C " +
        //                           "ON C.CODE = A.CURR_CODE " +
        //                           "LEFT OUTER JOIN TBL_CHART CH " +
        //                           "ON CH.ACT_CODE = A.BOOK_TYPE " +
        //                           "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.ACT_CODE = A.ACT_CODE AND PT.PARTY_CODE = A.PARTY_CODE " +
        //                           "WHERE A.DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "') T WHERE ROW_NUM = 1 " + filterCondition + " ORDER BY TRAN_ID DESC OFFSET " + skip + " ROWS FETCH NEXT " + take + " ROWS ONLY ";
        //            }else
        //            {
        //                query = "SELECT * FROM (" +
        //                           "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, C.DESCR AS CURR_CODE, A.CRATE, A.AMT, " +
        //                           "CH.ACT_NAME AS BOOK_TYPE, A.REMARKS, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME," +
        //                           "A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME,PT.PARTY_NAME, " +
        //                           "A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' WHEN A.ASTATUS = 'N' THEN 'In-Active' ELSE '' END ASTATUS," +
        //                           "ROW_NUMBER() OVER (PARTITION BY A.TRAN_ID ORDER BY A.ADD_DATE) AS ROW_NUM  " +
        //                           "FROM " + table + " A " +
        //                           "LEFT OUTER JOIN TBL_CURRENCY C " +
        //                           "ON C.CODE = A.CURR_CODE " +
        //                           "LEFT OUTER JOIN TBL_CHART CH " +
        //                           "ON CH.ACT_CODE = A.BOOK_TYPE " +
        //                           "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.ACT_CODE = A.ACT_CODE AND PT.PARTY_CODE = A.PARTY_CODE " +
        //                           "WHERE A.DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "') T WHERE ROW_NUM = 1 " + filterCondition + " ORDER BY TRAN_ID DESC";
        //            }

        //            SqlCommand command = new SqlCommand(query, connection);
        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                var row = new
        //                {
        //                    TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
        //                    ASTATUS = Convert.ToString(reader["ASTATUS"]),
        //                    V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
        //                    VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
        //                    PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]),
        //                    CURR_CODE = Convert.ToString(reader["CURR_CODE"]),
        //                    CRATE = Convert.ToString(reader["CRATE"]),
        //                    AMT = Convert.ToString(reader["AMT"]),
        //                    BOOK_TYPE = Convert.ToString(reader["BOOK_TYPE"]),
        //                    REMARKS = Convert.ToString(reader["REMARKS"]),
        //                    ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
        //                    ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
        //                    ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
        //                    ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
        //                    EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
        //                    EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
        //                    EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
        //                    EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
        //                    ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
        //                    EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
        //                    //DETAIL = GetCashReceiptDetails(Convert.ToString(reader["TRAN_ID"]), common, Menu)
        //                };
        //                jsonDataResult.Add(row);
        //            }

        //            reader.Close();
        //        }

        //        if (!string.IsNullOrEmpty(group))
        //        {

        //            JsonNode groupNode = JsonNode.Parse(group);
        //            JsonArray groupArray = groupNode.AsArray();
        //            var groupSelectors = groupArray.Select(g => g["selector"].ToString());
        //            string groupByClause = string.Join("", groupSelectors.Select(selector => selector));

        //            var groupedData = new List<object>();
        //            var grouped = jsonDataResult.GroupBy(d => _commonRepository.GetPropertyValue(d, groupByClause.ToUpper())).Select(g => new
        //            {
        //                key = g.Key,
        //                items = g.ToList()
        //            });
        //            groupedData.AddRange(grouped);
        //            response.data = new
        //            {
        //                data = groupedData,
        //                totalCount = totalCount,
        //            };
        //        }
        //        else
        //        {
        //            response.data = new
        //            {
        //                data = jsonDataResult,
        //                totalCount = totalCount
        //            };
        //        }
        //        response.msg = "";
        //        response.msgType = 1;
        //        return response;
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        response.msg = _catchMessage;
        //        response.msgType = 2;
        //    }
        //    return response;
        //}

        private List<object> GetCashReceiptDetails(string tranID, Common common, MyHttpResponseMessage menuData)
        {
            List<object> jsonDataResult = new List<object>();
            try
            {
                string? detailTable = string.Empty;
                if (menuData.data != null)
                {
                    var menu = (Menu)menuData.data;
                    detailTable = menu.TABLE1;
                }

                if (!String.IsNullOrWhiteSpace(detailTable))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT A.DT_CODE, BK.ACT_NAME AS BOOK_TYPE, CASE WHEN A.PARTY_CODE = 0 THEN B.ACT_NAME ELSE C.PARTY_NAME END AS ACT_CODE, A.PARTY_CODE, CASE WHEN A.DC_TYPE = 'D' THEN 'DEBIT' WHEN A.DC_TYPE = 'C' THEN 'CREDIT' ELSE '' END AS DC_TYPE, " +
                                       "A.AMT, A.CHQ_NO, A.CHQ_DATE, A.DT_DESC " +
                                       $"FROM {detailTable} A " +
                                       $"LEFT OUTER JOIN TBL_PARTY_TYPES C ON C.PARTY_CODE = A.PARTY_CODE AND C.ACT_CODE = A.ACT_CODE " +
                                       $"LEFT OUTER JOIN TBL_CHART B ON B.ACT_CODE = A.ACT_CODE " +
                                       $"LEFT OUTER JOIN TBL_CHART BK ON BK.ACT_CODE = A.BOOK_TYPE " +
                                       $"WHERE A.DLT = 'T' AND A.TRAN_ID = '{tranID}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                ACT_CODE = Convert.ToString(reader["ACT_CODE"]),
                                BOOK_TYPE = Convert.ToString(reader["BOOK_TYPE"]),
                                PARTY_CODE = Convert.ToString(reader["PARTY_CODE"]),
                                DC_TYPE = Convert.ToString(reader["DC_TYPE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                CHQ_NO = Convert.ToString(reader["CHQ_NO"]),
                                CHQ_DATE = Convert.ToString(reader["CHQ_DATE"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
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
            }
            return jsonDataResult;
        }

        private int GenerateNextId(Common common, SqlCommand command, Menu menu)
        {
            try
            {
                string? table = menu.TABLE1;
                string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM {table} WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                command.CommandText = maxIdQuery;
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        private string GenerateVoucherNo(Common common, int code, string vDate, Menu menu)
        {
            try
            {
                string? prefix = menu.PERFIX, shortName = string.Empty;
                int voucherLength = Convert.ToInt32(menu.VOUCHER_LEN);
                var branchData = _branchRepository.GetBranchByCode(common.Branch);
                if (branchData.data != null)
                {
                    var branch = (Branch)branchData.data;
                    shortName = branch.B_SHORT_NAME;
                }

                if (!String.IsNullOrWhiteSpace(shortName) && !String.IsNullOrWhiteSpace(prefix) && voucherLength > 0 && code > 0)
                {
                    string paddedVoucherValue = code.ToString().PadLeft(voucherLength, '0');
                    return $"{shortName}/{prefix}/{Convert.ToDateTime(vDate).ToString("yy-MM")}/{paddedVoucherValue}";
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        private int GenerateNextDetailId(SqlCommand command, Menu menu)
        {
            try
            {
                string? table = menu.TABLE1;
                string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) + 1 FROM {table}";
                command.CommandText = maxIdQuery;
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public MyHttpResponseMessage Save(List<PurchaseBookVoucher> modelRecord, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                var ip = common.IPAddress;
                var computerName = common.ComputerName;
                var postalCode = common.PostalCode;
                var userid = common.Username;
                var branch = common.Branch;
                var periodID = common.Period;
                var menuID = common.MenuID;
                int? ACT_CODE = 0;
                int? PARTY_CODE = 0;
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
                        string query = "", voucherNo = string.Empty;
                        bool IsMasterAdded = true, IsDetailAdded = true, IsNew = false;
                        int code = 0;
                        PurchaseBookVoucher party = modelRecord.FirstOrDefault();
                        var accountInformation = partiesData.ToList().Where(p => p.customizedKey == party.PARTY_CODE.ToString()).FirstOrDefault();
                        if (accountInformation != null)
                        {
                            PARTY_CODE = accountInformation.key;
                            ACT_CODE = accountInformation.accountCode;
                        }
                        else
                        {
                            PARTY_CODE = party.PARTY_CODE;
                            ACT_CODE = party.ACT_CODE;
                        }
                        foreach (var item in modelRecord)
                        {
                            try
                            {
                                if (item.CHQ_DATE == null || string.IsNullOrWhiteSpace(item.CHQ_DATE.ToString()))
                                {
                                    item.CHQ_DATE = item.V_DATE;
                                }
                                if (item.TRAN_ID == null || item.TRAN_ID == 0)
                                {
                                    IsNew = true;
                                    if (code == 0)
                                    {
                                        code = GenerateNextId(common, command, menu);
                                        if (code <= 0)
                                        {
                                            IsMasterAdded = false;
                                        }
                                        voucherNo = GenerateVoucherNo(common, code, CommonService.GetDateTime("Pakistan Standard Time"), menu);
                                        if (String.IsNullOrWhiteSpace(voucherNo))
                                        {
                                            IsMasterAdded = false;
                                        }
                                    }

                                    item.DT_CODE = GenerateNextDetailId(command, menu);
                                    if (item.DT_CODE > 0)
                                    {
                                        query = $"INSERT INTO {table} " +
                                                "(TRAN_ID, V_DATE, QTY, RATE, VOUCHER_NO, DOC, CURR_CODE, CRATE, BOOK_TYPE, REMARKS, BCODE, PERIOD_ID, " +
                                                "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, " +
                                                "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, " +
                                                "DLT, DT_CODE, ACT_CODE, PARTY_CODE, DC_TYPE, AMT, CHQ_NO, CHQ_DATE, DT_DESC) " +
                                                $"VALUES " +
                                                $"('{code}', '{item.V_DATE}', '{item.QTY}', '{item.RATE}', '{voucherNo}', '{item.DOC}', '{item.CURR_CODE}', " +
                                                $"'{item.CRATE}', '{item.BOOK_TYPE}', '{item.REMARKS}', '{branch}', " +
                                                $"'{periodID}', '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                $"'{computerName}', '{ip}', " +
                                                $"'{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{computerName}', " +
                                                $"'{ip}', '{postalCode}', '{postalCode}', " +
                                                $"'{item.ASTATUS}', '{menuID}', 'T', '{item.DT_CODE}', " +
                                                $"'{ACT_CODE}', '{PARTY_CODE}', '{item.DC_TYPE}', " +
                                                $"'{item.AMT}', '{item.CHQ_NO}', '{item.CHQ_DATE}', '{item.DT_DESC}')";
                                        command.CommandText = query;
                                        command.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        IsDetailAdded = false;
                                    }
                                }
                                else
                                {
                                    if (item.DT_CODE == null || item.DT_CODE == 0)
                                    {
                                        item.DT_CODE = GenerateNextDetailId(command, menu);
                                        if (item.DT_CODE > 0)
                                        {
                                            query = $"INSERT INTO {table} " +
                                                    "(TRAN_ID, V_DATE, QTY, RATE, VOUCHER_NO, DOC, CURR_CODE, CRATE, BOOK_TYPE, REMARKS, BCODE, PERIOD_ID, " +
                                                    "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, " +
                                                    "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, " +
                                                    "DLT, DT_CODE, ACT_CODE, PARTY_CODE, DC_TYPE, AMT, CHQ_NO, CHQ_DATE, DT_DESC) " +
                                                    $"VALUES " +
                                                    $"('{item.TRAN_ID}', '{item.V_DATE}', '{item.QTY}', '{item.RATE}', '{item.VOUCHER_NO}', '{item.DOC}', '{item.CURR_CODE}', " +
                                                    $"'{item.CRATE}', '{item.BOOK_TYPE}', '{item.REMARKS}', '{branch}', " +
                                                    $"'{periodID}', '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                    $"'{computerName}', '{ip}', " +
                                                    $"'{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{computerName}', " +
                                                    $"'{ip}', '{postalCode}', '{postalCode}', " +
                                                    $"'{item.ASTATUS}', '{menuID}', 'T', '{item.DT_CODE}', " +
                                                    $"'{ACT_CODE}', '{PARTY_CODE}', '{item.DC_TYPE}', " +
                                                    $"'{item.AMT}', '{item.CHQ_NO}', '{item.CHQ_DATE}', '{item.DT_DESC}')";
                                            command.CommandText = query;
                                            command.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            IsDetailAdded = false;
                                        }
                                    }
                                    else
                                    {
                                        query = $"UPDATE {table} SET DLT = 'F' " +
                                                $"WHERE TRAN_ID = '{item.TRAN_ID}' AND DT_CODE = '{item.DT_CODE}' AND BCODE = '{branch}' AND PERIOD_ID = '{periodID}'";
                                        command.CommandText = query;
                                        command.ExecuteNonQuery();

                                        query = $"UPDATE {table} SET " +
                                                $"V_DATE = '{item.V_DATE}', " +
                                                $"QTY = '{item.QTY}', " +
                                                $"RATE = '{item.RATE}', " +
                                                $"CURR_CODE = {item.CURR_CODE}, " +
                                                $"CRATE = {item.CRATE}, " +
                                                $"BOOK_TYPE = {item.BOOK_TYPE}, " +
                                                $"REMARKS = '{item.REMARKS}', " +
                                                $"DOC = '{item.DOC}', " +
                                                $"EDIT_USER_ID = '{userid}', " +
                                                $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                $"EDIT_COMPUTER_NAME = '{computerName}', " +
                                                $"EDIT_IP_ADDRESS = '{ip}', " +
                                                $"EDIT_POSTALCODE = '{postalCode}', " +
                                                $"ASTATUS = '{item.ASTATUS}', " +
                                                $"DLT = 'T', " +
                                                $"ACT_CODE = {ACT_CODE}, " +
                                                $"PARTY_CODE = {PARTY_CODE}, " +
                                                $"DC_TYPE = '{item.DC_TYPE}', " +
                                                $"AMT = {item.AMT}, " +
                                                $"CHQ_NO = '{item.CHQ_NO}', " +
                                                $"CHQ_DATE = '{item.CHQ_DATE}', " +
                                                $"DT_DESC = '{item.DT_DESC}' " +
                                                $"WHERE TRAN_ID = {item.TRAN_ID} AND DT_CODE = '{item.DT_CODE}' AND BCODE = '{branch}' AND PERIOD_ID = '{periodID}'";

                                        command.CommandText = query;
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                IsDetailAdded = false;
                            }
                        }

                        if (IsMasterAdded && IsDetailAdded)
                        {
                            transaction.Commit();
                            response.data = new
                            {
                                code = IsNew ? code : 0,
                                voucherNo = IsNew ? voucherNo : "",
                            };
                            response.msgType = 1;
                            response.msg = IsNew ? "Record Added Successfully" : "Record Updated Successfully";
                        }
                        else
                        {
                            transaction.Rollback();
                            response.data = "";
                            response.msg = "Something went wrong! please try again later.";
                            response.msgType = 2;
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

        public MyHttpResponseMessage GetPurchaseBookVoucherByCode(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "SELECT TOP 1 TRAN_ID, DOC, V_DATE, VOUCHER_NO, CURR_CODE, CRATE, " +
                                   "BOOK_TYPE, REMARKS, ASTATUS, ACT_CODE, PARTY_CODE " +
                                   "FROM " + table + " " +
                                   "WHERE DLT = 'T' AND TRAN_ID = '" + code + "' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "' ORDER BY DT_CODE";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                            V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                            VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                            CURR_CODE = Convert.ToInt32(reader["CURR_CODE"]),
                            CRATE = Convert.ToDecimal(reader["CRATE"]),
                            BOOK_TYPE = Convert.ToInt32(reader["BOOK_TYPE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            PARTY_CODE = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            DOC = Convert.ToString(reader["DOC"]),
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

        public MyHttpResponseMessage GetPurchaseBookVoucherDetailsByCode(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "SELECT DT_CODE, ACT_CODE, PARTY_CODE, RATE, QTY, DC_TYPE, BOOK_TYPE, " +
                                   "AMT, CHQ_NO, CHQ_DATE, DT_DESC " +
                                   $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                   $"AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            DT_CODE = Convert.ToString(reader["DT_CODE"]),
                            ACT_CODE = Convert.ToString(reader["ACT_CODE"]),
                            BOOK_TYPE = Convert.ToString(reader["BOOK_TYPE"]),
                            PARTY_CODE = Convert.ToString(reader["PARTY_CODE"]),
                            CUSTOM_ACT_CODE = Convert.ToString(reader["ACT_CODE"]) + "0123456789" + Convert.ToString(reader["PARTY_CODE"]),
                            DC_TYPE = Convert.ToString(reader["DC_TYPE"]),
                            AMT = Convert.ToString(reader["AMT"]),
                            CHQ_NO = Convert.ToString(reader["CHQ_NO"]),
                            CHQ_DATE = reader["CHQ_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd-MM-yyyy"),
                            DT_DESC = Convert.ToString(reader["DT_DESC"]),
                            QTY = Convert.ToString(reader["QTY"]),
                            RATE = Convert.ToString(reader["RATE"]),
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
                    string query = $"UPDATE {table} SET DLT = 'F' WHERE TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
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

        public MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string? table = menu.TABLE1;
                //string? table2 = menu.TABLE2;
                string connectionString = new SQLService().getconnstring();
                List<PurchaseBookVoucher> CashReceiptVoucherList = new List<PurchaseBookVoucher>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    //string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new PurchaseBookVoucher
                        {
                            TRAN_ID = 0,
                            V_DATE = record.V_DATE,
                            CURR_CODE = Convert.ToInt32(reader["CURR_CODE"]),
                            CRATE = Convert.ToDouble(reader["CRATE"]),
                            BOOK_TYPE = Convert.ToInt32(reader["BOOK_TYPE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            DOC = Convert.ToString(reader["DOC"]),
                            DT_CODE = 0,
                            DC_TYPE = Convert.ToString(reader["DC_TYPE"]),
                            AMT = Convert.ToInt32(reader["AMT"]),
                            CHQ_NO = Convert.ToString(reader["CHQ_NO"]),
                            CHQ_DATE = Convert.ToDateTime(reader["CHQ_DATE"]),
                            DT_DESC = Convert.ToString(reader["DT_DESC"]),
                            QTY = Convert.ToInt32(reader["QTY"]),
                            RATE = Convert.ToInt32(reader["RATE"]),
                        };
                        CashReceiptVoucherList.Add(row);
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(CashReceiptVoucherList, common, menu);
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

        public MyHttpResponseMessage DeletePurchaseBookVoucherDetailByCode(int tranID, int code, Common common, Menu menu)
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
                    string query = $"UPDATE {table} SET DLT = 'F' " +
                                   $"WHERE TRAN_ID = '{tranID}' AND DT_CODE = '{code}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
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

        public MyHttpResponseMessage GetDataForReport(CashReceiptRDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            CashReceiptRDLCReport masterData = new CashReceiptRDLCReport();
            CustomCashReceiptForPrintReport reportData = new CustomCashReceiptForPrintReport();
            var Menu = _menuRepository.GetMenu(common.MenuID);
            string? table = string.Empty, detailTable = string.Empty;
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                table = menu.TABLE1;
                detailTable = menu.TABLE2;
            }
            try
            {
                string topQuery = "", query = "";
                if (menuDetails.MD_ID == 38)
                {
                    topQuery = @$"SELECT 
                                A.VOUCHER_NO, PT.PARTY_NAME,A.V_DATE,A.REMARKS AS COMMENT, A.EDIT_USER_ID AS USER_NAME, 
                                CASE WHEN A.ASTATUS = 'Y' THEN 'Active' else 'Inactive' END AS ASTATUS 
                                FROM  {table} A 
                                LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                ON PT.ACT_CODE = A.ACT_CODE  AND PT.PARTY_CODE = A.PARTY_CODE
                                Where  a.BCODE =  '{common.Branch}' And a.PERIOD_ID =  '{common.Period}'   AND A.DLT = 'T' AND A.TRAN_ID = '{modelRecord.TRAN_ID}'";

                    query = @$"SELECT 
                            PT.ACT_NAME AS ACT_NAME ,A.DT_DESC AS DESCRIPTION1,
                            A.AMT
                            AS AMT,
                            A.QTY, A.RATE,
                            A.CHQ_NO ,A.CHQ_DATE 
                            FROM  {table} A
                            LEFT OUTER JOIN TBL_CHART C
                            ON C.ACT_CODE = A.ACT_CODE 
                            LEFT OUTER JOIN TBL_CHART PT 
                            ON PT.ACT_CODE = A.BOOK_TYPE
                            Where  a.BCODE =  '{common.Branch}' And a.PERIOD_ID =  '{common.Period}'   AND A.DLT = 'T' AND A.TRAN_ID = '{modelRecord.TRAN_ID}'";
                }

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(topQuery, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        masterData.INVOICE_NUMBER = Convert.ToString(reader["VOUCHER_NO"]);
                        masterData.DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                        masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                        masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                        masterData.SIG1 = $"{menuDetails.MENU_SIG1}";
                        masterData.SIG2 = $"{menuDetails.MENU_SIG2}";
                        masterData.SIG3 = $"{menuDetails.MENU_SIG3}";
                        masterData.SIG4 = $"{menuDetails.MENU_SIG4}";
                        masterData.COMMENT = Convert.ToString(reader["COMMENT"]);
                        masterData.STATUS = Convert.ToString(reader["ASTATUS"]);
                        masterData.COMPANY_NAME = currentCompany.C_NAME;
                        masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                        masterData.COMPANY_PHONE = currentCompany.C_TEL;
                        masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                        masterData.USER = reader["USER_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["USER_NAME"]);
                        masterData.PARTY_NAME = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]);
                    }
                    reader.Close();
                }

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (menuDetails.MD_ID == 38)
                    {
                        while (reader.Read())
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["ActName"] = Convert.ToString(reader["ACT_NAME"]);
                            dataRow["Desc"] = Convert.ToString(reader["DESCRIPTION1"]);
                            dataRow["ChqNo"] = Convert.ToString(reader["CHQ_NO"]);
                            dataRow["ChqDate"] = reader["CHQ_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["CHQ_DATE"]);
                            dataRow["Qty"] = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]);
                            dataRow["Rate"] = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]);
                            dataRow["Amt"] = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);
                            dataTable.Rows.Add(dataRow);
                        }
                    }

                    reader.Close();
                }

                reportData.Master = masterData;
                reportData.Detail = dataTable;
                response.data = reportData;
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
