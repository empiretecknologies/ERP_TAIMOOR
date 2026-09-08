using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json.Nodes;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class JournalVoucherRepository : IJournalVoucherRepository
    {
        public ICommonRepository _commonRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public IMenuRepository _menuRepository { get; set; }

        public JournalVoucherRepository(IBranchRepository branchRepository, ICommonRepository commonRepository, IMenuRepository menuRepository)
        {
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = string.Empty;
                string? search = string.Empty;

                var menu = (Menu)Menu.data;
                table = menu.TABLE1;
                search = menu.SEARCH;
                List<object> jsonDataResult = new List<object>();

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    if (search == "M")
                    {
                        string query = "SELECT TRAN_ID, V_DATE, VOUCHER_NO, CURR_CODE, MAX(CRATE) AS CRATE, " +
                                       "MAX(REMARKS) AS REMARKS, MAX(ADD_USER_ID) AS ADD_USER_ID, MAX(ADD_DATE) AS ADD_DATE, " +
                                       "MAX(ADD_COMPUTER_NAME) AS ADD_COMPUTER_NAME, MAX(ADD_IP_ADDRESS) AS ADD_IP_ADDRESS, " +
                                       "MAX(EDIT_USER_ID) AS EDIT_USER_ID, MAX(EDIT_DATE) AS EDIT_DATE, " +
                                       "MAX(EDIT_COMPUTER_NAME) AS EDIT_COMPUTER_NAME, MAX(EDIT_IP_ADDRESS) AS EDIT_IP_ADDRESS, " +
                                       "MAX(ADD_POSTALCODE) AS ADD_POSTALCODE, MAX(EDIT_POSTALCODE) AS EDIT_POSTALCODE, " +
                                       "CASE WHEN MAX(ASTATUS) = 'Y' THEN 'Active' WHEN MAX(ASTATUS) = 'N' THEN 'In-Active' ELSE '' END AS ASTATUS, " +
                                       "SUM(DEBIT) AS TOTAL_DEBIT, SUM(CREDIT) AS TOTAL_CREDIT, MAX(DT_DESC) AS DT_DESC, " +
                                       "MAX(CHQ_NO) AS CHQ_NO, MAX(CHQ_DATE) AS CHQ_DATE " +
                                       "FROM (" +
                                       "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, C.DESCR AS CURR_CODE, A.CRATE, " +
                                       "A.REMARKS, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, " +
                                       "A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, " +
                                       "A.ADD_POSTALCODE, A.EDIT_POSTALCODE, A.ASTATUS, A.DEBIT, A.CREDIT, A.DT_DESC, " +
                                       "A.CHQ_NO, A.CHQ_DATE " +
                                       "FROM " + table + " A " +
                                       "LEFT OUTER JOIN TBL_CURRENCY C ON C.CODE = A.CURR_CODE " +
                                       "WHERE A.DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "') T " +
                                       "GROUP BY TRAN_ID, V_DATE, VOUCHER_NO, CURR_CODE " +
                                       "ORDER BY TRAN_ID DESC";
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
                                CURR_CODE = Convert.ToString(reader["CURR_CODE"]),
                                CRATE = Convert.ToString(reader["CRATE"]),
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
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                    else
                    {
                        string query = "SELECT * FROM (" +
                                       "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, C.DESCR AS CURR_CODE, A.CRATE, " +
                                       "A.REMARKS, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME," +
                                       "A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, " +
                                       "A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' WHEN A.ASTATUS = 'N' THEN 'In-Active' ELSE '' END ASTATUS, PT.PARTY_NAME, CH.ACT_NAME, A.DEBIT, A.CREDIT, A.DT_DESC, A.CHQ_NO, A.CHQ_DATE," +
                                       "ROW_NUMBER() OVER (PARTITION BY A.TRAN_ID ORDER BY A.ADD_DATE) AS ROW_NUM  " +
                                       "FROM " + table + " A " +
                                       "LEFT OUTER JOIN TBL_CHART CH ON CH.ACT_CODE = A.ACT_CODE " +
                                       "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.ACT_CODE = A.ACT_CODE AND PT.PARTY_CODE = A.PARTY_CODE " +
                                       "LEFT OUTER JOIN TBL_CURRENCY C " +
                                       "ON C.CODE = A.CURR_CODE " +
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
                                CHQ_DATE = reader["CHQ_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("yyyy-MM-dd"),
                                CHQ_NO = Convert.ToString(reader["CHQ_NO"]),
                                PARTY_NAME = reader["PARTY_NAME"] == DBNull.Value ? Convert.ToString(reader["ACT_NAME"]) : Convert.ToString(reader["PARTY_NAME"]),
                                DEBIT = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                CREDIT = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CREDIT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                CURR_CODE = Convert.ToString(reader["CURR_CODE"]),
                                CRATE = Convert.ToString(reader["CRATE"]),
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
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
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
        //        var menu = (Menu)Menu.data;
        //        table = menu.TABLE1;
        //        List<object> jsonDataResult = new List<object>();
        //        int totalCount = 0;

        //        using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //        {
        //            string filterCondition = string.Empty;
        //            if (!string.IsNullOrEmpty(filter))
        //            {
        //                JsonNode jsonNode = JsonNode.Parse(filter);
        //                JsonArray jsonArray = jsonNode.AsArray();
        //                if (jsonArray.Count > 10)
        //                {
        //                    jsonArray.RemoveAt(1);
        //                    jsonArray.RemoveAt(0);
        //                }
        //                filterCondition = _commonRepository.BuildFilterCondition(jsonArray);
        //                filterCondition = filterCondition
        //                    .Replace("iteM_CODE", "IM.ITEM_NAME")
        //                    .Replace("qty", "D.QTY");
        //            }

        //            string query = "SELECT COUNT(*) FROM (" +
        //                           "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, C.DESCR AS CURR_CODE, A.CRATE, " +
        //                           "A.REMARKS, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME," +
        //                           "A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, " +
        //                           "A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' WHEN A.ASTATUS = 'N' THEN 'In-Active' ELSE '' END ASTATUS, PT.PARTY_NAME, CH.ACT_NAME, A.DEBIT, A.CREDIT, A.DT_DESC, A.CHQ_NO, A.CHQ_DATE," +
        //                           "ROW_NUMBER() OVER (PARTITION BY A.TRAN_ID ORDER BY A.ADD_DATE) AS ROW_NUM  " +
        //                           "FROM " + table + " A " +
        //                           "LEFT OUTER JOIN TBL_CHART CH ON CH.ACT_CODE = A.ACT_CODE " +
        //                           "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.ACT_CODE = A.ACT_CODE AND PT.PARTY_CODE = A.PARTY_CODE " +
        //                           "LEFT OUTER JOIN TBL_CURRENCY C " +
        //                           "ON C.CODE = A.CURR_CODE " +
        //                           "WHERE A.DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "') T WHERE 1 = 1 " + filterCondition + "";

        //            SqlCommand countCommand = new SqlCommand(query, connection);
        //            connection.Open();
        //            totalCount = (int)countCommand.ExecuteScalar();
        //            connection.Close();

        //            if (string.IsNullOrEmpty(group))
        //            {
        //                query = "SELECT * FROM (" +
        //                           "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, C.DESCR AS CURR_CODE, A.CRATE, " +
        //                           "A.REMARKS, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME," +
        //                           "A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, " +
        //                           "A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' WHEN A.ASTATUS = 'N' THEN 'In-Active' ELSE '' END ASTATUS, PT.PARTY_NAME, CH.ACT_NAME, A.DEBIT, A.CREDIT, A.DT_DESC, A.CHQ_NO, A.CHQ_DATE," +
        //                           "ROW_NUMBER() OVER (PARTITION BY A.TRAN_ID ORDER BY A.ADD_DATE) AS ROW_NUM  " +
        //                           "FROM " + table + " A " +
        //                           "LEFT OUTER JOIN TBL_CHART CH ON CH.ACT_CODE = A.ACT_CODE " +
        //                           "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.ACT_CODE = A.ACT_CODE AND PT.PARTY_CODE = A.PARTY_CODE " +
        //                           "LEFT OUTER JOIN TBL_CURRENCY C " +
        //                           "ON C.CODE = A.CURR_CODE " +
        //                           "WHERE A.DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "') T WHERE 1 = 1 " + filterCondition + " ORDER BY TRAN_ID DESC OFFSET " + skip + " ROWS FETCH NEXT " + take + " ROWS ONLY ";
        //            }else
        //            {
        //                query = "SELECT * FROM (" +
        //                           "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, C.DESCR AS CURR_CODE, A.CRATE, " +
        //                           "A.REMARKS, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME," +
        //                           "A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, " +
        //                           "A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' WHEN A.ASTATUS = 'N' THEN 'In-Active' ELSE '' END ASTATUS, PT.PARTY_NAME, CH.ACT_NAME, A.DEBIT, A.CREDIT, A.DT_DESC, A.CHQ_NO, A.CHQ_DATE," +
        //                           "ROW_NUMBER() OVER (PARTITION BY A.TRAN_ID ORDER BY A.ADD_DATE) AS ROW_NUM  " +
        //                           "FROM " + table + " A " +
        //                           "LEFT OUTER JOIN TBL_CHART CH ON CH.ACT_CODE = A.ACT_CODE " +
        //                           "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.ACT_CODE = A.ACT_CODE AND PT.PARTY_CODE = A.PARTY_CODE " +
        //                           "LEFT OUTER JOIN TBL_CURRENCY C " +
        //                           "ON C.CODE = A.CURR_CODE " +
        //                           "WHERE A.DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "') T WHERE 1 = 1 " + filterCondition + " ORDER BY TRAN_ID DESC";
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
        //                    CHQ_DATE = reader["CHQ_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("yyyy-MM-dd"),
        //                    CHQ_NO = Convert.ToString(reader["CHQ_NO"]),
        //                    PARTY_NAME = reader["PARTY_NAME"] == DBNull.Value ? Convert.ToString(reader["ACT_NAME"]) : Convert.ToString(reader["PARTY_NAME"]),
        //                    DEBIT = reader["DEBIT"] == DBNull.Value ? "0" : _commonRepository.ToAccountingFormat(Convert.ToDecimal(reader["DEBIT"])).ToString(),
        //                    CREDIT = reader["CREDIT"] == DBNull.Value ? "0" : _commonRepository.ToAccountingFormat(Convert.ToDecimal(reader["CREDIT"])).ToString(),
        //                    DT_DESC = Convert.ToString(reader["DT_DESC"]),
        //                    V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
        //                    VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
        //                    CURR_CODE = Convert.ToString(reader["CURR_CODE"]),
        //                    CRATE = Convert.ToString(reader["CRATE"]),
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

        private List<object> GetVoucherDetails(string tranID, Common common, MyHttpResponseMessage menuData)
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
                        string query = "SELECT A.DT_CODE, CASE WHEN A.PARTY_CODE = 0 THEN B.ACT_NAME ELSE C.PARTY_NAME END AS ACT_CODE, A.PARTY_CODE, " +
                                       "A.DEBIT, A.CREDIT, A.CHQ_NO, A.CHQ_DATE, A.DT_DESC " +
                                       $"FROM {detailTable} A " +
                                       $"LEFT OUTER JOIN TBL_PARTY_TYPES C ON C.PARTY_CODE = A.PARTY_CODE AND C.ACT_CODE = A.ACT_CODE " +
                                       $"LEFT OUTER JOIN TBL_CHART B ON B.ACT_CODE = A.ACT_CODE " +
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
                                PARTY_CODE = Convert.ToString(reader["PARTY_CODE"]),
                                CREDIT = Convert.ToString(reader["CREDIT"]),
                                DEBIT = Convert.ToString(reader["DEBIT"]),
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

        public MyHttpResponseMessage Save(List<JournalVoucher> modelRecord, Common common, Menu menu)
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
                string connectionString = new SQLService().getconnstring();
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
                        query = $"UPDATE {table} SET DLT = 'F' " +
                                $"WHERE TRAN_ID = '{modelRecord[0].TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{periodID}'";
                        command.CommandText = query;
                        command.ExecuteNonQuery();
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
                                                "(TRAN_ID, V_DATE, VOUCHER_NO, DOC, CURR_CODE, CRATE, REMARKS, BCODE, PERIOD_ID, " +
                                                "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, " +
                                                "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, " +
                                                "DLT, DT_CODE, ACT_CODE, PARTY_CODE, DEBIT, CREDIT, CHQ_NO, CHQ_DATE, DT_DESC) " +
                                                $"VALUES " +
                                                $"('{code}', '{item.V_DATE}', '{voucherNo}', '{item.DOC}', '{item.CURR_CODE}', " +
                                                $"'{item.CRATE}', '{item.REMARKS}', '{branch}', " +
                                                $"'{periodID}', '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                $"'{computerName}', '{ip}', " +
                                                $"'{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{computerName}', " +
                                                $"'{ip}', '{postalCode}', '{postalCode}', " +
                                                $"'{item.ASTATUS}', '{menuID}', 'T', '{item.DT_CODE}', " +
                                                $"'{item.ACT_CODE}', '{item.PARTY_CODE}', '{item.DEBIT}', " +
                                                $"'{item.CREDIT}', '{item.CHQ_NO}', '{item.CHQ_DATE}', '{item.DT_DESC}')";
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
                                                    "(TRAN_ID, V_DATE, VOUCHER_NO, DOC, CURR_CODE, CRATE, REMARKS, BCODE, PERIOD_ID, " +
                                                    "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, " +
                                                    "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, " +
                                                    "DLT, DT_CODE, ACT_CODE, PARTY_CODE, DEBIT, CREDIT, CHQ_NO, CHQ_DATE, DT_DESC) " +
                                                    $"VALUES " +
                                                    $"('{item.TRAN_ID}', '{item.V_DATE}', '{item.VOUCHER_NO}', '{item.DOC}', '{item.CURR_CODE}', " +
                                                    $"'{item.CRATE}', '{item.REMARKS}', '{branch}', " +
                                                    $"'{periodID}', '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                    $"'{computerName}', '{ip}', " +
                                                    $"'{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{computerName}', " +
                                                    $"'{ip}', '{postalCode}', '{postalCode}', " +
                                                    $"'{item.ASTATUS}', '{menuID}', 'T', '{item.DT_CODE}', " +
                                                    $"'{item.ACT_CODE}', '{item.PARTY_CODE}', '{item.DEBIT}', " +
                                                    $"'{item.CREDIT}', '{item.CHQ_NO}', '{item.CHQ_DATE}', '{item.DT_DESC}')";
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

                                        //getting related data
                                        string checkQuery = $"SELECT COUNT(*) FROM TBL_CC_DETAIL WHERE PTRAN_ID = '{item.TRAN_ID}' AND PICK_ID = '{item.DT_CODE}' AND PMENU_ID = {menuID} AND DLT = 'T'";
                                        command.CommandText = checkQuery;
                                        int relatedCount = Convert.ToInt32(command.ExecuteScalar());

                                        // getting amount from db 
                                        string sumQuery = $"SELECT SUM(AMOUNT) FROM TBL_CC_DETAIL WHERE PMENU_ID = {menuID} AND PTRAN_ID = {item.TRAN_ID} AND PICK_ID = {item.DT_CODE} AND DLT = 'T' AND ASTATUS = 'Y'";
                                        command.CommandText = sumQuery;
                                        object sumResult = command.ExecuteScalar();

                                        decimal amount = 0;
                                        if (sumResult != null && sumResult != DBNull.Value)
                                        {
                                            amount = Convert.ToDecimal(sumResult);
                                        }

                                        if (relatedCount > 0 && item.DEBIT < (double)sumResult)
                                        {
                                            // Related data found, do not update
                                            response.msgType = 2;
                                            response.msg = $"You Cannot decrease old record amount , related data exist in Cost Center.";
                                            return response;
                                        }
                                        else if (relatedCount > 0 && item.PARTY_CODE > 0)
                                        {
                                            response.msgType = 2;
                                            response.msg = $"You Cannot change Account into Party, related data exist in Cost Center.";
                                            return response;
                                        }
                                        else
                                        {
                                            query = $"UPDATE {table} SET " +
                                                $"V_DATE = '{item.V_DATE}', " +
                                                $"CURR_CODE = '{item.CURR_CODE}', " +
                                                $"CRATE = '{item.CRATE}', " +
                                                $"REMARKS = '{item.REMARKS}', " +
                                                $"DOC = '{item.DOC}', " +
                                                $"EDIT_USER_ID = '{userid}', " +
                                                $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                $"EDIT_COMPUTER_NAME = '{computerName}', " +
                                                $"EDIT_IP_ADDRESS = '{ip}', " +
                                                $"EDIT_POSTALCODE = '{postalCode}', " +
                                                $"ASTATUS = '{item.ASTATUS}', " +
                                                $"DLT = 'T', " +
                                                $"ACT_CODE = '{item.ACT_CODE}', " +
                                                $"PARTY_CODE = '{item.PARTY_CODE}', " +
                                                $"CREDIT = '{item.CREDIT}', " +
                                                $"DEBIT = '{item.DEBIT}', " +
                                                $"CHQ_NO = '{item.CHQ_NO}', " +
                                                $"CHQ_DATE = '{item.CHQ_DATE}', " +
                                                $"DT_DESC = '{item.DT_DESC}' " +
                                                $"WHERE TRAN_ID = '{item.TRAN_ID}' AND DT_CODE = '{item.DT_CODE}' AND BCODE = '{branch}' AND PERIOD_ID = '{periodID}'";

                                            command.CommandText = query;
                                        }
                                        
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

        public MyHttpResponseMessage GetJournalVoucherByCode(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "SELECT TOP 1 TRAN_ID, V_DATE, VOUCHER_NO, DOC, CURR_CODE, CRATE, " +
                                   "REMARKS, ASTATUS FROM " + table + " " +
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
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            DOC = Convert.ToString(reader["DOC"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
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

        public MyHttpResponseMessage GetJournalVoucherDetailsByCode(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    //string query = "SELECT TRAN_ID, DT_CODE, ACT_CODE, PARTY_CODE, DEBIT, " +
                    //               "CREDIT, CHQ_NO, CHQ_DATE, DT_DESC " +
                    //               $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";

                    string query = $@"SELECT JV.TRAN_ID, JV.DT_CODE, JV.ACT_CODE, JV.PARTY_CODE, JV.DEBIT, JV.CREDIT, JV.CHQ_NO, JV.CHQ_DATE, JV.DT_DESC,
                                        CASE WHEN (SELECT SUM(D.AMOUNT) FROM TBL_CC_DETAIL D WHERE D.PTRAN_ID = JV.TRAN_ID AND D.PICK_ID = JV.DT_CODE AND D.DLT='T') IS NULL THEN 0
                                        WHEN ISNULL((SELECT SUM(D.AMOUNT) FROM TBL_CC_DETAIL D WHERE D.PTRAN_ID = JV.TRAN_ID AND D.PICK_ID = JV.DT_CODE AND D.DLT='T'),0) >= ISNULL(JV.DEBIT,0) THEN 0
                                        ELSE 1 END AS COST_CENTER_STATUS
                                        FROM {table} JV WHERE JV.DLT = 'T' AND JV.TRAN_ID = '{code}' AND JV.BCODE = '{common.Branch}' AND JV.PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                            DT_CODE = Convert.ToString(reader["DT_CODE"]),
                            ACT_CODE = Convert.ToString(reader["ACT_CODE"]),
                            PARTY_CODE = Convert.ToString(reader["PARTY_CODE"]),
                            CUSTOM_ACT_CODE = $"{Convert.ToInt32(reader["ACT_CODE"])}0123456789{Convert.ToInt32(reader["PARTY_CODE"])}",
                            //CUSTOM_ACT_CODE = Convert.ToString(reader["ACT_CODE"]) + "" + Convert.ToString(reader["PARTY_CODE"]),
                            DEBIT = Convert.ToString(reader["DEBIT"]),
                            CREDIT = Convert.ToString(reader["CREDIT"]),
                            CHQ_NO = Convert.ToString(reader["CHQ_NO"]),
                            CHQ_DATE = reader["CHQ_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("yyyy-MM-dd"),
                            DT_DESC = Convert.ToString(reader["DT_DESC"]),
                            COST_CENTER_STATUS = Convert.ToString(reader["COST_CENTER_STATUS"]),
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
                string? table2 = menu.TABLE2;
                string connectionString = new SQLService().getconnstring();
                List<JournalVoucher> JournalVoucherList = new List<JournalVoucher>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    //string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new JournalVoucher
                        {
                            TRAN_ID = 0,
                            V_DATE = record.V_DATE,
                            CURR_CODE = Convert.ToInt32(reader["CURR_CODE"]),
                            CRATE = Convert.ToDouble(reader["CRATE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            DEBIT = Convert.ToInt32(reader["DEBIT"]),
                            CREDIT = Convert.ToInt32(reader["CREDIT"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            DOC = Convert.ToString(reader["DOC"]),
                            DT_CODE = 0,
                            CHQ_NO = Convert.ToString(reader["CHQ_NO"]),
                            CHQ_DATE = Convert.ToDateTime(reader["CHQ_DATE"]),
                            DT_DESC = Convert.ToString(reader["DT_DESC"]),
                        };
                        JournalVoucherList.Add(row);
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(JournalVoucherList, common, menu);
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

        public MyHttpResponseMessage DeleteJournalVoucherDetailByCode(int tranID, int code, Common common, Menu menu)
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

        public MyHttpResponseMessage GetDataForReport(JournalVoucherRDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            JournalVoucherRDLCReport masterData = new JournalVoucherRDLCReport();
            CustomJournalVoucherForPrintReport reportData = new CustomJournalVoucherForPrintReport();
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
                if (menuDetails.MD_ID == 30)
                {
                    topQuery = @$"SELECT 
                                A.VOUCHER_NO , A.V_DATE, A.REMARKS AS COMMENT, A.EDIT_USER_ID AS USER_NAME, 
                                CASE WHEN A.ASTATUS = 'Y' THEN 'Active' else 'Inactive' END AS ASTATUS 
                                FROM  {table} A
                                Where  a.BCODE =  '{common.Branch}' And a.PERIOD_ID =  '{common.Period}'   AND A.DLT = 'T' AND A.TRAN_ID = '{modelRecord.TRAN_ID}'";

                    query = @$"SELECT 
                            CASE WHEN A.PARTY_CODE = 0 THEN
                            C.ACT_NAME
                            Else
                            PT.PARTY_NAME
                            End
                            AS ACT_NAME, A.DT_DESC AS DESCRIPTION1,
                            A.DEBIT, A.CREDIT,
                            A.CHQ_NO, A.CHQ_DATE 
                            FROM {table} A
                            LEFT OUTER JOIN TBL_CHART C
                            ON C.ACT_CODE = A.ACT_CODE 
                            LEFT OUTER JOIN TBL_PARTY_TYPES PT
                            ON PT.ACT_CODE = A.ACT_CODE AND PT.PARTY_CODE = A.PARTY_CODE  
                            Where a.BCODE = '{common.Branch}' And a.PERIOD_ID = '{common.Period}' AND A.DLT = 'T' AND A.TRAN_ID = '{modelRecord.TRAN_ID}'";
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
                        masterData.USER = reader["USER_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["USER_NAME"]);
                    }
                    reader.Close();
                }

                decimal totalDebit = 0;
                decimal totalCredit = 0;

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (menuDetails.MD_ID == 30)
                    {
                        while (reader.Read())
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["ActName"] = Convert.ToString(reader["ACT_NAME"]);
                            dataRow["Desc"] = Convert.ToString(reader["DESCRIPTION1"]);
                            dataRow["ChqNo"] = Convert.ToString(reader["CHQ_NO"]);
                            dataRow["ChqDate"] = reader["CHQ_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd-MM-yyyy");
                            decimal debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]);
                            decimal credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CREDIT"]);

                            dataRow["Debit"] = debit;
                            dataRow["Credit"] = credit;

                            totalDebit += debit;
                            totalCredit += credit;

                            dataTable.Rows.Add(dataRow);
                        }
                    }
                    reader.Close();
                }

                if (totalDebit == totalCredit)
                {
                    reportData.Master = masterData;
                    reportData.Detail = dataTable;
                    response.data = reportData;
                    response.msg = "";
                    response.msgType = 1;
                }
                else
                {
                    reportData.Master = masterData;
                    reportData.Detail = dataTable;
                    response.data = reportData;
                    response.msg = "Debit amount must be equal to credit amount";
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