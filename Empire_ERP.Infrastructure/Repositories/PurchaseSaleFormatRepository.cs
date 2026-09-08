using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.NetworkInformation;
using System.Text.Json.Nodes;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class PurchaseSaleFormatRepository : IPurchaseSaleFormatRepository
    {
        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public IMenuRepository _menuRepository { get; set; }
        public PurchaseSaleFormatRepository(IBranchRepository branchRepository, ICommonRepository commonRepository, IMenuRepository menuRepository)
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

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, A.ADD_USER_ID, A.ADD_DATE, 
	                                        A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, 
                                            CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, IM.ITEM_NAME AS ITEM_CODE
                                        FROM {table} A
                                            LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE
                                        WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}'  
                                        ORDER BY A.TRAN_ID DESC";
                        

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }

                    response.data = jsonDataResult;
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

        //public MyHttpResponseMessage QuickSearch(Common common, int skip, int take, string filter = null, string group = null)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty, detailTable = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //            detailTable = menu.TABLE2;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
        //        {
        //            List<object> jsonDataResult = new List<object>();
        //            int totalCount = 0;
        //            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //            {
        //                string filterCondition = string.Empty;
        //                if (!string.IsNullOrEmpty(filter))
        //                {
        //                    JsonNode jsonNode = JsonNode.Parse(filter);
        //                    JsonArray jsonArray = jsonNode.AsArray();
        //                    if (jsonArray.Count > 10)
        //                    {
        //                        jsonArray.RemoveAt(1);
        //                        jsonArray.RemoveAt(0);
        //                    }
        //                    filterCondition = _commonRepository.BuildFilterCondition(jsonArray);
        //                    filterCondition = filterCondition
        //                        .Replace("iteM_CODE", "IM.ITEM_NAME")
        //                        .Replace("traN_ID", "A.TRAN_ID")
        //                        .Replace("astatus", "A.ASTATUS")
        //                        .Replace("remarks", "A.REMARKS")
        //                        .Replace("qty", "D.QTY");
        //                }

        //                string query = $@"SELECT 
        //                                    COUNT(*) OVER() AS COUNT, A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, D.SELLER, D.SODA_DATE, D.BSODA_DATE, D.SCOND, D.BCOND, D.SRATE, D.BRATE, D.SQTY, D.BQTY, D.SAMT, D.BAMT, D.BUYER, A.ADD_USER_ID, A.ADD_DATE, 
	       //                                 A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, 
        //                                    CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, IM.ITEM_NAME AS ITEM_CODE
        //                                FROM {table} A
        //                                    LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID
        //                                    LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE
        //                                WHERE A.DLT = 'T' AND D.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' {filterCondition} 
        //                                ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC";

        //                SqlCommand countCommand = new SqlCommand(query, connection);
        //                connection.Open();
        //                SqlDataReader readerCommand = countCommand.ExecuteReader();
        //                if (readerCommand.Read())
        //                {
        //                    totalCount = readerCommand.GetInt32(readerCommand.GetOrdinal("COUNT"));
        //                }
        //                readerCommand.Close();
        //                connection.Close();

        //                if (string.IsNullOrEmpty(group))
        //                {
        //                    query = $@"SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, D.SELLER, D.SODA_DATE, D.BSODA_DATE, D.SCOND, D.BCOND, D.SRATE, D.BRATE, D.SQTY, D.BQTY, D.SAMT, D.BAMT, D.BUYER, A.ADD_USER_ID, A.ADD_DATE, 
	       //                                 A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, 
        //                                    CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, IM.ITEM_NAME AS ITEM_CODE
        //                                FROM {table} A
        //                                    LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID
        //                                    LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE
        //                                WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' {filterCondition} 
        //                                ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        //                }
        //                else
        //                {
        //                    query = $@"SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, D.SELLER, D.SODA_DATE, D.BSODA_DATE, D.SCOND, D.BCOND, D.SRATE, D.BRATE, D.SQTY, D.BQTY, D.SAMT, D.BAMT, D.BUYER, A.ADD_USER_ID, A.ADD_DATE, 
	       //                                 A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, 
        //                                    CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, IM.ITEM_NAME AS ITEM_CODE
        //                                FROM {table} A
        //                                    LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID
        //                                    LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE
        //                                WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' {filterCondition} 
        //                                ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC";
        //                }

        //                SqlCommand command = new SqlCommand(query, connection);
        //                connection.Open();
        //                SqlDataReader reader = command.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    var row = new
        //                    {
        //                        TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
        //                        ASTATUS = Convert.ToString(reader["ASTATUS"]),
        //                        V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
        //                        VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
        //                        ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
        //                        SELLER = Convert.ToString(reader["SELLER"]),
        //                        BUYER = Convert.ToString(reader["BUYER"]),
        //                        ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
        //                        ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
        //                        ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
        //                        ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
        //                        EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
        //                        EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
        //                        EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
        //                        EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
        //                        ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
        //                        EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
        //                        SODA_DATE = reader["SODA_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SODA_DATE"]).ToString("yyyy-MM-dd"),
        //                        BSODA_DATE = reader["BSODA_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["BSODA_DATE"]).ToString("yyyy-MM-dd"),
        //                        SQTY = Convert.ToString(reader["SQTY"]),
        //                        BQTY = Convert.ToString(reader["BQTY"]),
        //                        SRATE = Convert.ToString(reader["SRATE"]),
        //                        BRATE = Convert.ToString(reader["BRATE"]),
        //                        SAMT = Convert.ToString(reader["SAMT"]),
        //                        BAMT = Convert.ToString(reader["BAMT"]),
        //                    };
        //                    jsonDataResult.Add(row);
        //                }
        //                reader.Close();
        //            }

        //            if (!string.IsNullOrEmpty(group))
        //            {

        //                JsonNode groupNode = JsonNode.Parse(group);
        //                JsonArray groupArray = groupNode.AsArray();
        //                var groupSelectors = groupArray.Select(g => g["selector"].ToString());
        //                string groupByClause = string.Join("", groupSelectors.Select(selector => selector));

        //                var groupedData = new List<object>();
        //                var grouped = jsonDataResult.GroupBy(d => _commonRepository.GetPropertyValue(d, groupByClause.ToUpper())).Select(g => new
        //                {
        //                    key = g.Key,
        //                    items = g.ToList()
        //                });
        //                groupedData.AddRange(grouped);
        //                response.data = new
        //                {
        //                    data = groupedData,
        //                    totalCount = totalCount,
        //                };
        //            }
        //            else
        //            {
        //                response.data = new
        //                {
        //                    data = jsonDataResult,
        //                    totalCount = totalCount
        //                };
        //            }
        //            response.msg = "";
        //            response.msgType = 1;
        //        }
        //        else
        //        {
        //            response.data = "";
        //            response.msg = "Something went wrong! please try again later.";
        //            response.msgType = 2;
        //        }
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
                    string paddedVoucherValue = code.ToString().PadLeft(voucherLength, '0');
                    return $"{shortName}/{prefix}/{Convert.ToDateTime(vDate).ToString("yy-MM")}/{paddedVoucherValue}";
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        private int GenerateNextDetailId(Common common, SqlCommand command)
        {
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) + 1 FROM {table}";
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

        public MyHttpResponseMessage Save(CustomPurchaseSaleFormat modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
                {
                    var ip = common.IPAddress;
                    var computer = common.ComputerName;
                    var postal = common.PostalCode;
                    var username = common.Username;
                    var branch = common.Branch;
                    var period = common.Period;
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
                            string query = "", detailQuery = "", voucherNo = string.Empty;
                            bool IsMasterAdded = true, IsNew = false;
                            int code = 0;

                            if (modelRecord.Master.TRAN_ID == null || modelRecord.Master.TRAN_ID == 0)
                            {
                                IsNew = true;
                                code = GenerateNextId(common, command);

                                if (code > 0)
                                {
                                    modelRecord.Master.TRAN_ID = code;
                                    voucherNo = GenerateVoucherNo(common, code, CommonService.GetDateTime("Pakistan Standard Time"));
                                    if (String.IsNullOrWhiteSpace(voucherNo))
                                    {
                                        IsMasterAdded = false;
                                    }
                                }
                                else
                                {
                                    IsMasterAdded = false;
                                }

                                var duplicationQuery = $"SELECT COUNT(*) FROM {table} WHERE ITEM_CODE = @ItemCode AND DLT = 'T'";
                                command.CommandText = duplicationQuery;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@ItemCode", modelRecord.Master.ITEM_CODE);

                                int count = (int)command.ExecuteScalar();
                                if (count > 0)
                                {
                                    response.data = "";
                                    response.msg = $"Record With Selected Item Already Exist!";
                                    response.msgType = 2;
                                    return response;
                                }

                                query = $"INSERT INTO {table}" +
                                        "(TRAN_ID, V_DATE, VOUCHER_NO, ITEM_CODE, " +
                                        "BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                        "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                        "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                        "ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, " +
                                        "DLT)" +
                                        "VALUES" +
                                        "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.ITEM_CODE + "'," +
                                        "'" + branch + "','" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                        "'" + computer + "','" + ip + "','" + username + "'," +
                                        "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + computer + "','" + ip + "'," +
                                        "'" + postal + "','" + postal + "','" + modelRecord.Master.ASTATUS + "','" + menuID + "','T')";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                query = $"UPDATE {table} SET " +
                                        $"V_DATE = '{modelRecord.Master.V_DATE}', " +
                                        $"ITEM_CODE = '{modelRecord.Master.ITEM_CODE}', " +
                                        $"EDIT_USER_ID = '{username}', " +
                                        $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                        $"EDIT_COMPUTER_NAME = '{computer}', " +
                                        $"EDIT_IP_ADDRESS = '{ip}', " +
                                        $"EDIT_POSTALCODE = '{postal}', " +
                                        $"ASTATUS = '{modelRecord.Master.ASTATUS}' " +
                                        $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }

                            var isDetailAdded = true;

                            if (modelRecord.Detail.Count > 0)
                            {
                                detailQuery = $"UPDATE {detailTable} SET DLT = 'F'" +
                                $" WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                                command.CommandText = detailQuery;
                                command.ExecuteNonQuery();
                            }
                            foreach (var item in modelRecord.Detail.ToList())
                            {
                                try
                                {
                                    if (item.DT_CODE == null || item.DT_CODE == 0)
                                    {
                                        int detailCode = GenerateNextDetailId(common, command);
                                        if (detailCode > 0)
                                        {
                                            detailQuery = $"INSERT INTO {detailTable}" +
                                                           "(TRAN_ID, DT_CODE, " +
                                                           "SELLER, COMMENT, RT_TYPE, SODA_DATE, SCOND, SRATE, SQTY, SAMT, " +
                                                           "BUYER, BSODA_DATE, BCOND, BRATE, BQTY, BAMT, " +
                                                           "BCODE,PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                                           "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                                           "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                                           "ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT)" +
                                                           "VALUES" +
                                                           "('" + modelRecord.Master.TRAN_ID + "','" + detailCode + "'," +
                                                           "'" + item.SELLER + "','" + item.COMMENT + "','" + item.RT_TYPE + "','" + item.SODA_DATE + "','" + item.SCOND + "','" + item.SRATE + "','" + item.SQTY + "','" + item.SAMT + "'," +
                                                           "'" + item.BUYER + "','" + item.BSODA_DATE + "','" + item.BCOND + "','" + item.BRATE + "','" + item.BQTY + "','" + item.BAMT + "'," +
                                                           "'" + branch + "','" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                                           "'" + computer + "','" + ip + "','" + username + "'," +
                                                           "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + computer + "','" + ip + "'," +
                                                           "'" + postal + "','" + postal + "','" + menuID + "','T')";
                                            command.CommandText = detailQuery;
                                            command.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            isDetailAdded = false;
                                        }
                                    }
                                    else
                                    {
                                        detailQuery = $"UPDATE {detailTable} SET " +
                                                      $"SELLER = '{item.SELLER}', " +
                                                      $"COMMENT = '{item.COMMENT}', " +
                                                      $"RT_TYPE = '{item.RT_TYPE}', " +
                                                      $"SODA_DATE = '{item.SODA_DATE}', " +
                                                      $"SCOND = '{item.SCOND}', " +
                                                      $"SRATE = '{item.SRATE}', " +
                                                      $"SQTY = '{item.SQTY}', " +
                                                      $"SAMT = '{item.SAMT}', " +
                                                      $"BUYER = '{item.BUYER}', " +
                                                      $"BSODA_DATE = '{item.BSODA_DATE}', " +
                                                      $"BCOND = '{item.BCOND}', " +
                                                      $"BRATE = '{item.BRATE}', " +
                                                      $"BQTY = '{item.BQTY}', " +
                                                      $"BAMT = '{item.BAMT}', " +
                                                      $"EDIT_USER_ID = '{username}', " +
                                                      $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                      $"EDIT_COMPUTER_NAME = '{computer}', " +
                                                      $"EDIT_IP_ADDRESS = '{ip}', " +
                                                      $"EDIT_POSTALCODE = '{postal}', " +
                                                      $"DLT = 'T' " +
                                                      $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND DT_CODE = '{item.DT_CODE}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                                        command.CommandText = detailQuery;
                                        command.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception)
                                {
                                    isDetailAdded = false;
                                }
                            }

                            if (IsMasterAdded && isDetailAdded)
                            {
                                transaction.Commit();
                                response.data = new
                                {
                                    code = IsNew ? code : modelRecord.Master.TRAN_ID,
                                    voucherNo = IsNew ? voucherNo : modelRecord.Master.VOUCHER_NO,
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
                response.msgType = 2;
                response.msg = _catchMessage;
            }
            return response;
        }

        public MyHttpResponseMessage GetPurchaseSaleFormatByCode(int code, Common common)
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
                        string query = $"SELECT TRAN_ID, V_DATE, VOUCHER_NO, ITEM_CODE, ASTATUS FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}'";
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
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }

                    response.data = jsonDataResult;
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

        public MyHttpResponseMessage GetPurchaseSaleFormatDetailsByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $"SELECT * FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                COMMENT = Convert.ToString(reader["COMMENT"]),
                                SELLER = Convert.ToString(reader["SELLER"]),
                                BUYER = Convert.ToString(reader["BUYER"]),
                                SODA_DATE = reader["SODA_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SODA_DATE"]).ToString("yyyy-MM-dd"),
                                BSODA_DATE = reader["BSODA_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["BSODA_DATE"]).ToString("yyyy-MM-dd"),
                                SCOND = Convert.ToString(reader["SCOND"]),
                                BCOND = Convert.ToString(reader["BCOND"]),
                                SRATE = Convert.ToDecimal(reader["SRATE"]),
                                BRATE = Convert.ToDecimal(reader["BRATE"]),
                                SQTY = Convert.ToInt32(reader["SQTY"]),
                                BQTY = Convert.ToInt32(reader["BQTY"]),
                                SAMT = Convert.ToDecimal(reader["SAMT"]),
                                BAMT = Convert.ToDecimal(reader["bAMT"]),
                                RT_TYPE = reader["RT_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RT_TYPE"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }

                    response.data = jsonDataResult;
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

        public MyHttpResponseMessage DeletePurchaseSaleFormatDetailByCode(int tranID, int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE2;
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

        public MyHttpResponseMessage GetDataForReport(ListPrintReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            ListPrintReport masterData = new ListPrintReport();
            CustomPrintReport reportData = new CustomPrintReport();
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
                string query = "";
                if (menuDetails.MD_ID == 20)
                {
                    query = $@"SELECT 
                                COUNT(*) OVER() AS COUNT, A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, D.RT_TYPE, D.COMMENT,D.SELLER, D.SODA_DATE, D.BSODA_DATE, D.SCOND, D.BCOND, D.SRATE, D.BRATE, D.SQTY, D.BQTY, D.SAMT, D.BAMT, D.BUYER, A.ADD_USER_ID, A.ADD_DATE, 
	                            A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, 
                                CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, IM.ITEM_NAME AS ITEM_CODE
                            FROM {table} A
                                LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID
                                LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE
                            WHERE A.DLT = 'T' AND D.DLT = 'T' AND A.TRAN_ID = '{modelRecord.TRAN_ID}' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' 
                            ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC";
                }

                masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                masterData.COMPANY_NAME = currentCompany.C_NAME;
                masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                masterData.COMPANY_PHONE = currentCompany.C_TEL;
                masterData.COMPANY_LOGO = currentCompany.C_LOGO;

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        DataRow dataRow = dataTable.NewRow();

                        dataRow["Date"] = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                        dataRow["Item"] = Convert.ToString(reader["ITEM_CODE"]);
                        dataRow["Seller"] = Convert.ToString(reader["SELLER"]);
                        dataRow["Buyer"] = Convert.ToString(reader["BUYER"]);
                        dataRow["SCond"] = Convert.ToString(reader["SCOND"]);
                        dataRow["BCond"] = Convert.ToString(reader["BCOND"]);
                        dataRow["SDate"] = reader["SODA_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SODA_DATE"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(reader["SODA_DATE"]).ToString("dd-MM-yyyy");
                        dataRow["BDate"] = reader["BSODA_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["BSODA_DATE"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(reader["BSODA_DATE"]).ToString("dd-MM-yyyy");
                        dataRow["SQty"] = Convert.ToInt32(reader["SQTY"]);
                        dataRow["BQty"] = Convert.ToInt32(reader["BQTY"]);
                        dataRow["RtType"] = reader["RT_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RT_TYPE"]);
                        dataRow["SRate"] = Convert.ToDecimal(reader["SRATE"]);
                        dataRow["BRate"] = Convert.ToDecimal(reader["BRATE"]);
                        dataRow["SAmt"] = reader["SAMT"] == DBNull.Value ? 0 : _commonRepository.ToAccountingFormat(Convert.ToDecimal(reader["SAMT"]));
                        dataRow["BAmt"] = reader["BAMT"] == DBNull.Value ? 0 : _commonRepository.ToAccountingFormat(Convert.ToDecimal(reader["BAMT"]));
                        dataRow["Comment"] = Convert.ToString(reader["COMMENT"]);
                        dataTable.Rows.Add(dataRow);
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

        public MyHttpResponseMessage GetDataForMultiBillReport(ListMultiBillPrintReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            ListPrintReport masterData = new ListPrintReport();
            CustomPrintReport reportData = new CustomPrintReport();
            var Menu = _menuRepository.GetMenu(common.MenuID);
            string? table = string.Empty, detailTable = string.Empty;
            var tranIds = string.Join(",", modelRecord.TRAN_IDS.Select(id => $"'{id}'"));
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                table = menu.TABLE1;
                detailTable = menu.TABLE2;
            }
            try
            {
                string query = "";
                if (menuDetails.MD_ID == 20)
                {
                    query = $@"SELECT 
                                COUNT(*) OVER() AS COUNT, A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, D.RT_TYPE, D.COMMENT,D.SELLER, D.SODA_DATE, D.BSODA_DATE, D.SCOND, D.BCOND, D.SRATE, D.BRATE, D.SQTY, D.BQTY, D.SAMT, D.BAMT, D.BUYER, A.ADD_USER_ID, A.ADD_DATE, 
	                            A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, 
                                CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, IM.ITEM_NAME AS ITEM_CODE
                            FROM {table} A
                                LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID
                                LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE
                            WHERE A.DLT = 'T' AND D.DLT = 'T' AND A.TRAN_ID IN ({tranIds}) AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' 
                            ORDER BY D.BSODA_DATE DESC, D.SODA_DATE DESC";
                }

                masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                masterData.COMPANY_NAME = currentCompany.C_NAME;
                masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                masterData.COMPANY_PHONE = currentCompany.C_TEL;
                masterData.COMPANY_LOGO = currentCompany.C_LOGO;

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        DataRow dataRow = dataTable.NewRow();

                        dataRow["Date"] = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                        dataRow["Item"] = Convert.ToString(reader["ITEM_CODE"]);
                        dataRow["Seller"] = Convert.ToString(reader["SELLER"]);
                        dataRow["Buyer"] = Convert.ToString(reader["BUYER"]);
                        dataRow["SCond"] = Convert.ToString(reader["SCOND"]);
                        dataRow["BCond"] = Convert.ToString(reader["BCOND"]);
                        dataRow["SDate"] = reader["SODA_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SODA_DATE"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(reader["SODA_DATE"]).ToString("dd-MM-yyyy");
                        dataRow["BDate"] = reader["BSODA_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["BSODA_DATE"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(reader["BSODA_DATE"]).ToString("dd-MM-yyyy");
                        dataRow["SQty"] = Convert.ToInt32(reader["SQTY"]);
                        dataRow["BQty"] = Convert.ToInt32(reader["BQTY"]);
                        dataRow["RtType"] = reader["RT_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RT_TYPE"]);
                        dataRow["SRate"] = Convert.ToDecimal(reader["SRATE"]);
                        dataRow["BRate"] = Convert.ToDecimal(reader["BRATE"]);
                        dataRow["SAmt"] = reader["SAMT"] == DBNull.Value ? 0 : _commonRepository.ToAccountingFormat(Convert.ToDecimal(reader["SAMT"]));
                        dataRow["BAmt"] = reader["BAMT"] == DBNull.Value ? 0 : _commonRepository.ToAccountingFormat(Convert.ToDecimal(reader["BAMT"]));
                        dataRow["Comment"] = Convert.ToString(reader["COMMENT"]);
                        dataRow["Voucher"] = Convert.ToString(reader["VOUCHER_NO"]);
                        dataTable.Rows.Add(dataRow);
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
