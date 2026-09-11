using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Reflection.Metadata;
using System.Text;
using ZXing;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class PurchaseBillRepository : IPurchaseBillRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public ICommonService _commonService { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public IPeriodRepository _periodRepository { get; set; }

        public PurchaseBillRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonRepository commonRepository, ICommonService commonService, IPeriodRepository periodRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
            _commonService = commonService;
            _periodRepository = periodRepository;
        }

        public MyHttpResponseMessage GetLastRateByBarcode(CustomPurchaseBill model, string? dctype, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? detailTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                }
                //var BARCODE = model.Detail.First().;
                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();

                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {

                        string query = $@"DECLARE @DC_TYPE VARCHAR(5);
                            SET @DC_TYPE = 'PB';
                            DECLARE @PARTY_CODE INT = '{model.Master.PARTY_CODE}';
                            DECLARE @ACT_CODE INT = '{model.Master.ACT_CODE}';
                            DECLARE @BARCODE INT = '{model.Master.BARCODE_ID}';

                            SELECT
                                A.ITEM_CODE AS CODE,
                                '' AS BARCODE,
                                '' AS BLABEL,
                                A.ITEM_NAME AS ITEM_ID,
                                '' AS COLOR,
                                '' AS SIZE,
                                CASE
                                    WHEN @DC_TYPE IN('PB','PR') 
                                        THEN ISNULL(NULLIF(LAST_RATE.RATE,0), A.PURCHASE_RATE)

                                    WHEN @DC_TYPE IN('SB', 'SR')
                                        THEN ISNULL(NULLIF(LAST_RATE.RATE,0), A.SALE_RATE)

                                    ELSE 0
                                END AS SRATE,

                                LAST_RATE.RATE AS LAST_RATE,

                                M.PARTY_CODE,
                                M.ACT_CODE

                            FROM TBL_ITEMSMASTER A
							                          
						   OUTER APPLY(
                                SELECT TOP 1
                                    D.RATE
                                FROM TBL_SB_DETAIL D
                                INNER JOIN TBL_SB_MASTER M2
                                    ON M2.TRAN_ID = D.TRAN_ID
                                    AND M2.BCODE = D.BCODE
                                    AND M2.PERIOD_ID = D.PERIOD_ID
                                WHERE
                                    D.ITEM_CODE = A.ITEM_CODE
                                    AND M2.PARTY_CODE = @PARTY_CODE
                                    AND M2.ACT_CODE = @ACT_CODE
                                ORDER BY M2.TRAN_ID DESC
                            ) LAST_RATE



                            OUTER APPLY(
                                SELECT TOP 1
                                    M3.PARTY_CODE,
                                    M3.ACT_CODE
                                FROM TBL_SB_MASTER M3
                                WHERE
                                    M3.PARTY_CODE = @PARTY_CODE
                                    AND M3.ACT_CODE = @ACT_CODE
                                    AND EXISTS(
                                        SELECT 1
                                        FROM TBL_SB_DETAIL D3
                                        WHERE D3.TRAN_ID = M3.TRAN_ID
                                        AND D3.ITEM_CODE = A.ITEM_CODE
                                    )
                                ORDER BY M3.TRAN_ID DESC
                            ) M

                            WHERE
                                A.DLT = 'T'
                                AND A.ASTATUS = 'Y'
                                AND A.ITEM_CODE = @BARCODE;";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                rate = Convert.ToString(reader["SRATE"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                    response.data = jsonDataResult;
                    response.msg = "";
                    response.msgType = 1;
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

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? detailTable = string.Empty;
                string? search = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    search = menu.SEARCH;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();

                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        if (search == "M")
                        {
                            string query = $@"SELECT M.TRAN_ID AS TRAN_ID, M.TRAN_ID AS CODE,M.V_DATE,M.VOUCHER_NO,M.BTYPE,M.COMM,
                                                M.PARTY_CODE,M.ACT_CODE,M.REF,M.REMARKS,M.SCODE,M.SACODE,M.RINV_NO,M.RINV_DATE,M.BCODE,
                                                M.PERIOD_ID,M.ADD_USER_ID,M.ADD_DATE,M.ADD_COMPUTER_NAME,M.ADD_IP_ADDRESS,M.EDIT_USER_ID,
                                                M.EDIT_DATE,M.EDIT_COMPUTER_NAME,M.EDIT_IP_ADDRESS,PT.PARTY_NAME,M.ADD_POSTALCODE,M.EDIT_POSTALCODE,
                                                CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS 
                                                FROM {table} M 
                                                LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE 
                                                WHERE  M.DLT = 'T' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}' ORDER BY M.TRAN_ID DESC";

                            SqlCommand command = new SqlCommand(query, connection);
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                var row = new
                                {
                                    ID = Convert.ToInt32(reader["TRAN_ID"]),
                                    CODE = Convert.ToInt32(reader["CODE"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                    V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                    PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                    ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                    REF = Convert.ToString(reader["REF"]),
                                    BTYPE = Convert.ToString(reader["BTYPE"]),
                                    REMARKS = Convert.ToString(reader["REMARKS"]),
                                    SCODE = reader["SCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SCODE"]),
                                    SACODE = reader["SACODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SACODE"]),
                                    ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                    ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
                                    ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                    ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                    EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                    EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
                                    EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                    EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                    PARTY_NAME = reader["PARTY_NAME"],
                                    ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                    EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                    COMM = Convert.ToInt32(reader["COMM"]),
                                    RINV_NO = reader["RINV_NO"] == DBNull.Value ? "" : Convert.ToString(reader["RINV_NO"]),
                                    //REGION = reader["REGION"] == DBNull.Value ? "" : Convert.ToString(reader["REGION"]),
                                    RINV_DATE = reader["RINV_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["RINV_DATE"]).ToString("yyyy-MM-dd"),
                                };
                                jsonDataResult.Add(row);
                            }
                            reader.Close();


                        }
                        else
                        {
                            var query = $@"SELECT M.TRAN_ID AS TRAN_ID, M.TRAN_ID AS CODE,M.V_DATE,M.RINV_NO,M.RINV_DATE,M.VOUCHER_NO,M.BTYPE,M.COMM,M.PARTY_CODE,M.ACT_CODE,M.REF,D.DT_DESC,
                                            ROUND(SUM(D.NET_AMT),0) AS AMT,
                                            M.SCODE,M.SACODE,M.BCODE,M.PERIOD_ID,
                                            M.ADD_USER_ID,M.ADD_DATE,M.ADD_COMPUTER_NAME,M.ADD_IP_ADDRESS,M.EDIT_USER_ID,M.EDIT_DATE,
                                            M.EDIT_COMPUTER_NAME,M.EDIT_IP_ADDRESS,PT.PARTY_NAME,M.ADD_POSTALCODE,M.EDIT_POSTALCODE, 
                                            CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS 
                                            FROM {table} M 
                                            LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE 
                                            LEFT OUTER JOIN {detailTable} D
                                            ON D.TRAN_ID = M.TRAN_ID AND D.PERIOD_ID = M.PERIOD_ID AND D.BCODE = M.BCODE
                                            WHERE  M.DLT = 'T' AND M.BCODE = {common.Branch} AND M.PERIOD_ID = {common.Period} AND  D.DLT = 'T'
                                            GROUP BY 
                                            M.TRAN_ID  , M.TRAN_ID  ,M.V_DATE,M.RINV_NO,M.RINV_DATE,M.VOUCHER_NO,M.BTYPE,M.COMM,M.PARTY_CODE,M.ACT_CODE,M.REF,D.DT_DESC,
                                            M.SCODE,M.SACODE,M.BCODE,M.PERIOD_ID,
                                            M.ADD_USER_ID,M.ADD_DATE,M.ADD_COMPUTER_NAME,M.ADD_IP_ADDRESS,M.EDIT_USER_ID,M.EDIT_DATE,
                                            M.EDIT_COMPUTER_NAME,M.EDIT_IP_ADDRESS,PT.PARTY_NAME,M.ADD_POSTALCODE,M.EDIT_POSTALCODE, M.ASTATUS 
                                            ORDER BY M.TRAN_ID DESC";
                            SqlCommand command = new SqlCommand(query, connection);
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                var row = new
                                {
                                    ID = Convert.ToInt32(reader["TRAN_ID"]),
                                    CODE = Convert.ToInt32(reader["CODE"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                    V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                    PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                    ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                    REF = Convert.ToString(reader["REF"]),
                                    REMARKS = Convert.ToString(reader["DT_DESC"]),
                                    AMT = Convert.ToString(reader["AMT"]),
                                    BTYPE = Convert.ToString(reader["BTYPE"]),
                                    SCODE = reader["SCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SCODE"]),
                                    SACODE = reader["SACODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SACODE"]),
                                    ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                    ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
                                    ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                    ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                    EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                    EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
                                    EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                    EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                    PARTY_NAME = reader["PARTY_NAME"],
                                    ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                    EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                    COMM = Convert.ToInt32(reader["COMM"]),
                                    RINV_NO = reader["RINV_NO"] == DBNull.Value ? "" : Convert.ToString(reader["RINV_NO"]),
                                    RINV_DATE = reader["RINV_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["RINV_DATE"]).ToString("yyyy-MM-dd"),
                                };
                                jsonDataResult.Add(row);
                            }
                            reader.Close();
                        }
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

        //public MyHttpResponseMessage QuickSearch(Common common, int skip, int take, string filter = null, string group = null, string sort = null)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        string sortBy;
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }

        //        if (sort is not null)
        //        {
        //            var data = JsonSerializer.Deserialize<dynamic>(sort);
        //            var selector = data[0].GetProperty("selector").ToString();
        //            var order = data[0].GetProperty("desc").ToString() == "False" ? "ASC" : "DESC";
        //            sortBy = $"{selector} {order}";
        //            sortBy = sortBy
        //                .Replace("id", "TRAN_ID");
        //        }
        //        else
        //        {
        //            sortBy = "TRAN_ID DESC";
        //        }

        //        if (!String.IsNullOrWhiteSpace(table))
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
        //                    if (jsonArray.Count > 8)
        //                    {
        //                        jsonArray.RemoveAt(1);
        //                        jsonArray.RemoveAt(0);
        //                    }
        //                    filterCondition = _commonRepository.BuildFilterCondition(jsonArray);
        //                    filterCondition = filterCondition
        //                        .Replace("id", "TRAN_ID");
        //                }

        //                string query = $"SELECT COUNT(*) FROM {table} WHERE DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + $"' {filterCondition}";

        //                SqlCommand countCommand = new SqlCommand(query, connection);
        //                connection.Open();
        //                totalCount = (int)countCommand.ExecuteScalar();
        //                connection.Close();

        //                if (string.IsNullOrEmpty(group))
        //                {
        //                    query = "SELECT TRAN_ID,V_DATE,VOUCHER_NO,BTYPE,COMM," +
        //                               "PARTY_CODE,ACT_CODE,REF,REMARKS,SCODE,SACODE,BCODE,PERIOD_ID," +
        //                               "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
        //                               "EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
        //                               "ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS " +
        //                               $"FROM {table} WHERE DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + $"' {filterCondition} ORDER BY {sortBy}" +
        //                               $" OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        //                }
        //                else
        //                {
        //                    query = "SELECT TRAN_ID,V_DATE,VOUCHER_NO,BTYPE,COMM," +
        //                               "PARTY_CODE,ACT_CODE,REF,REMARKS,SCODE,SACODE,BCODE,PERIOD_ID," +
        //                               "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
        //                               "EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
        //                               "ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS " +
        //                               $"FROM {table} WHERE DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + $"' {filterCondition} ORDER BY {sortBy}";
        //                }
        //                SqlCommand command = new SqlCommand(query, connection);
        //                connection.Open();
        //                SqlDataReader reader = command.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    var row = new
        //                    {
        //                        ID = Convert.ToString(reader["TRAN_ID"]),
        //                        ASTATUS = Convert.ToString(reader["ASTATUS"]),
        //                        V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
        //                        VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
        //                        PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
        //                        ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
        //                        REF = Convert.ToString(reader["REF"]),
        //                        BTYPE = Convert.ToString(reader["BTYPE"]),
        //                        REMARKS = Convert.ToString(reader["REMARKS"]),
        //                        SCODE = Convert.ToInt32(reader["SCODE"]),
        //                        SACODE = Convert.ToInt32(reader["SACODE"]),
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
        //                        COMM = Convert.ToInt32(reader["COMM"]),
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

        public MyHttpResponseMessage GetPurchaseBillByCode(int code, Common common)
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
                        string query = $"SELECT TRAN_ID,V_DATE,VOUCHER_NO,BTYPE, DOC,COMM,COMM_AMT,COMM_VAL,DISC,TERMS,CARTAGE," +
                                       "PARTY_CODE,ACT_CODE,REGION,CURR_CODE,CRATE,REF,RINV_NO,RINV_DATE,REMARKS,SCODE,SACODE,BCODE,PERIOD_ID," +
                                       "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
                                       "EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                       "ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS " +
                                       $"FROM {table} WHERE DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "' AND TRAN_ID = '" + code + "'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = reader["TRAN_ID"] == DBNull.Value ? "" : Convert.ToString(reader["TRAN_ID"]),
                                ASTATUS = reader["ASTATUS"] == DBNull.Value ? "" : Convert.ToString(reader["ASTATUS"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? "" : Convert.ToString(reader["VOUCHER_NO"]),
                                BTYPE = reader["BTYPE"] == DBNull.Value ? "" : Convert.ToString(reader["BTYPE"]),
                                DOC = reader["DOC"] == DBNull.Value ? "" : Convert.ToString(reader["DOC"]),
                                COMM = reader["COMM"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMM"]),
                                CARTAGE = reader["CARTAGE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["CARTAGE"]),
                                TERMS = reader["TERMS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TERMS"]),
                                COMM_AMT = reader["COMM_AMT"] == DBNull.Value ? "" : Convert.ToString(reader["COMM_AMT"]),
                                COMM_VAL = reader["COMM_VAL"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMM_VAL"]),
                                DISC = reader["DISC"] == DBNull.Value ? "" : Convert.ToString(reader["DISC"]),
                                REF = reader["REF"] == DBNull.Value ? "" : Convert.ToString(reader["REF"]),
                                SCODE = reader["SCODE"] == DBNull.Value ? "" : Convert.ToString(reader["SCODE"]),
                                SACODE = reader["SACODE"] == DBNull.Value ? "" : Convert.ToString(reader["SACODE"]),
                                CURR_CODE = reader["CURR_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CURR_CODE"]),
                                CRATE = reader["CRATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CRATE"]),
                                REMARKS = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
                                PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                RINV_NO = reader["RINV_NO"] == DBNull.Value ? "" : Convert.ToString(reader["RINV_NO"]),
                                REGION = ((reader["REGION"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["REGION"])),
                                RINV_DATE = reader["RINV_DATE"] == DBNull.Value || Convert.ToDateTime(reader["RINV_DATE"]) == new DateTime(1900, 1, 1) ? null : Convert.ToDateTime(reader["RINV_DATE"]).ToString("yyyy-MM-dd"),
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

        public MyHttpResponseMessage GetPickDataByParty(int partyCode, int actCode, int region, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                MyHttpResponseMessage Menu = this._menuRepository.GetMenu(common.MenuID);
                string table = string.Empty;
                string detailTable = string.Empty;
                string pickTable = string.Empty;
                string pickDetailTable = string.Empty;
                string pickType = string.Empty;
                string dcType = string.Empty;
                if (Menu.data != null)
                {
                    Menu menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickTable = menu.PICK_TABLE_MASTER;
                    pickDetailTable = menu.PICK_TABLE_DETAIL;
                    pickType = menu.PICK_TYPE;
                    dcType = menu.DCTYPE;
                }
                if (dcType == "SO")
                {
                    List<SalesOrderPickData> jsonDataResult = new List<SalesOrderPickData>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string pickQuery = @"
                        SELECT
                            M.TRAN_ID,
                            D.DT_CODE,
                            D.ITEM_CODE,
                            I.ITEM_NAME,
                            D.QTY,
                            D.RATE,
                            (ISNULL(D.QTY, 0) * ISNULL(CONVERT(FLOAT, D.RATE), 0)) AS AMOUNT,
                            M.V_DATE,
                            M.VOUCHER_NO,
                            M.PARTY_CODE,
                            M.ACT_CODE,
                            M.REMARKS,
                            D.BCODE,
                            I.IUNIT_CODE AS UNIT
                        FROM TBL_SQ_MASTER M
                        INNER JOIN TBL_SQ_DETAIL D
                            ON M.TRAN_ID = D.TRAN_ID
                           AND D.DLT = 'T'
                        LEFT OUTER JOIN TBL_ITEMSMASTER I
                            ON I.ITEM_CODE = D.ITEM_CODE
                        WHERE M.DLT = 'T'
                          AND M.BCODE = @BCODE
                          AND M.PERIOD_ID = @PERIOD_ID
                          AND M.ASTATUS = 'Y'
                          AND M.PARTY_CODE = @PARTY_CODE
                        ORDER BY M.TRAN_ID DESC, D.DT_CODE DESC";

                        SqlCommand command = new SqlCommand(pickQuery, connection);
                        command.Parameters.AddWithValue("@BCODE", common.Branch);
                        command.Parameters.AddWithValue("@PERIOD_ID", common.Period);
                        command.Parameters.AddWithValue("@PARTY_CODE", partyCode);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            double qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["QTY"]);
                            int rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RATE"]);
                            double amount = reader["AMOUNT"] == DBNull.Value ? (qty * rate) : Convert.ToDouble(reader["AMOUNT"]);
                            int pickTranId = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]);
                            int pickDtCode = reader["DT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DT_CODE"]);

                            jsonDataResult.Add(new SalesOrderPickData
                            {
                                ID = Convert.ToString(pickTranId) + "_" + Convert.ToString(pickDtCode),
                                TRAN_ID = pickTranId,
                                DT_CODE = pickDtCode,
                                ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                ITEM_NAME = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                QTY = qty,
                                RATE = rate,
                                AMOUNT = amount,
                                AMT = amount,
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? "" : Convert.ToString(reader["VOUCHER_NO"]),
                                PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                REMARKS = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
                                BCODE = reader["BCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BCODE"]),
                                UNIT = reader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UNIT"]),
                                NET_AMT = amount,
                                DISC = 0,
                                DISC_AMT = 0,
                                TAX = 0,
                                TAX_AMT = 0,
                                ADV = 0,
                                ADV_AMT = 0,
                                WAREHOUSE = 2,
                                CHK = "0",
                                CHK1 = false,
                                PICK_ID = pickTranId,
                                PICK_ID_D = pickDtCode,
                            });
                        }
                        reader.Close();
                    }

                    response.data = jsonDataResult;
                    response.msg = "";
                    response.msgType = 1;
                }
                else 
                {
                    if (!string.IsNullOrWhiteSpace(table))
                    {
                        List<object> jsonDataResult3 = new List<object>();
                        using (SqlConnection connection3 = new SqlConnection(new SQLService().getconnstring()))
                        {
                            string query = $@"SELECT 
                                                A.TRAN_ID, A.V_DATE AS LB_DATE, A.VOUCHER_NO, A.TERMS,
                                                A.PARTY_CODE, A.CURR_CODE, A.CRATE, I.HS_CODE, A.ACT_CODE, AC.PARTY_NAME AS PARTY_NAME, A.REF,
                                                A.BTYPE, A.DISC AS DISC_M, A.LOCAL_CHARGES, A.DOC, A.COMM_VAL, A.RINV_NO, A.RINV_DATE,


-- Issue carton
                    ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {pickDetailTable} WHERE DT_CODE = B.DT_CODE), 0) AS ISSUE_CRTN,
                    -- Received carton
                    ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {detailTable} D
                            LEFT OUTER JOIN {table} M ON D.TRAN_ID = M.TRAN_ID
                            WHERE D.PICK_ID_D = B.DT_CODE AND D.DLT = 'T' AND M.DLT = 'T'), 0) AS R_CRTN,
                    -- Balance carton
                    ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {pickDetailTable} WHERE DT_CODE = B.DT_CODE), 0) - 
                    ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {detailTable} D
                            LEFT OUTER JOIN {table} M ON D.TRAN_ID = M.TRAN_ID
                            WHERE D.PICK_ID_D = B.DT_CODE AND D.DLT = 'T' AND M.DLT = 'T'), 0) AS B_CRTN,


                                                -- Issue Qty
                                                ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {pickDetailTable} WHERE DT_CODE = B.DT_CODE), 0) AS ISSUE_QTY,
                                                -- Returned Qty
                                                ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {detailTable} D
                                                        LEFT OUTER JOIN {table} M ON D.TRAN_ID = M.TRAN_ID
                                                        WHERE D.PICK_ID_D = B.DT_CODE AND D.DLT = 'T' AND M.DLT = 'T'), 0) AS R_QTY,
                                                -- Balance Qty
                                                ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {pickDetailTable} WHERE DT_CODE = B.DT_CODE), 0) - 
                                                ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {detailTable} D
                                                        LEFT OUTER JOIN {table} M ON D.TRAN_ID = M.TRAN_ID
                                                        WHERE D.PICK_ID_D = B.DT_CODE AND D.DLT = 'T' AND M.DLT = 'T'), 0) AS B_QTY,
                                                U.GROUP_CODE AS UNIT, U.GROUP_NAME AS UNIT_NAME, B.RATE, B.AMT,
                                                B.PACK, B.QTY, B.WEIGHT, W.CODE AS WAREHOUSE_CODE, W.DESCR AS WAREHOUSE_NAME,
                                                B.DT_CODE, B.DISC, B.DISC_AMT, B.ADV, B.ADV_AMT, B.TAX, B.TAX_AMT, G.GROUP_CODE AS GRADE, G.GROUP_NAME AS GRADE_NAME, B.NET_AMT,
                                                CL.GROUP_NAME AS COLOR, SL.GROUP_NAME AS SIZE, A.COMM, A.COMM_AMT,
                                                CL.GROUP_CODE AS COLOR_ID, SL.GROUP_CODE AS SIZE_ID, A.SCODE, A.SACODE, B.ITEM_CODE, B.DEL_DATE, B.DUE_DATE, B.DUE_DAYS, I.ITEM_NAME,
                                                MB.MENU_PAGE, MB.MENU_PARENT_CODE, A.MENU_ID
                                            FROM {pickTable} A
                                            LEFT OUTER JOIN {pickDetailTable} B ON B.TRAN_ID = A.TRAN_ID AND B.DLT = 'T' AND B.BCODE = A.BCODE AND B.PERIOD_ID = A.PERIOD_ID
                                            LEFT OUTER JOIN {detailTable} VC ON VC.PICK_ID_D = B.DT_CODE AND VC.BCODE = B.BCODE AND VC.DLT = 'T' AND VC.PERIOD_ID = B.PERIOD_ID
                                            LEFT OUTER JOIN {table} SBM ON VC.TRAN_ID = SBM.TRAN_ID AND SBM.DLT = 'T' 
                                            LEFT OUTER JOIN TBL_PARTY_TYPES AC ON AC.PARTY_CODE = A.PARTY_CODE AND AC.ACT_CODE = A.ACT_CODE
                                            LEFT OUTER JOIN TBL_COLOR CL ON CL.GROUP_CODE = B.COLOR
                                            LEFT OUTER JOIN TBL_SIZE SL ON SL.GROUP_CODE = B.SIZE
                                            LEFT OUTER JOIN TBL_UNIT U ON U.GROUP_CODE = B.UNIT
                                            LEFT OUTER JOIN TBL_ITEMSMASTER I ON I.ITEM_CODE = B.ITEM_CODE
                                            LEFT OUTER JOIN TBL_WAREHOUSE W ON W.CODE = B.WAREHOUSE
                                            LEFT OUTER JOIN TBL_GRADE G ON G.GROUP_CODE = B.GRADE
                                            LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = A.MENU_ID
                                            WHERE A.BCODE = '{common.Branch}' 
                                              AND A.PERIOD_ID = '{common.Period}' 
                                              AND AC.PARTY_CODE = '{partyCode}' 
                                              AND AC.ACT_CODE = '{actCode}' 
                                              AND A.DLT = 'T' AND A.ASTATUS = 'Y' AND B.DLT = 'T'
                                            GROUP BY 
                                                A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, A.TERMS, A.PARTY_CODE, A.CURR_CODE, A.CRATE, I.HS_CODE,
                                                A.BTYPE, A.DISC, A.LOCAL_CHARGES, A.DOC, A.COMM_VAL, A.RINV_NO, A.RINV_DATE,
                                                A.ACT_CODE, AC.PARTY_NAME, A.REF, U.GROUP_CODE, U.GROUP_NAME, B.RATE, B.AMT,
                                                B.DT_CODE, B.DISC, B.DISC_AMT, B.ADV, B.ADV_AMT, B.TAX, B.TAX_AMT, G.GROUP_CODE, G.GROUP_NAME, B.NET_AMT,
                                                CL.GROUP_NAME, SL.GROUP_NAME, A.COMM, A.COMM_AMT,
                                                CL.GROUP_CODE, SL.GROUP_CODE, A.SCODE, A.SACODE, B.ITEM_CODE,
                                                B.PACK, B.QTY, B.WEIGHT, W.CODE, W.DESCR, 
                                                B.DEL_DATE, B.DUE_DATE, B.DUE_DAYS, I.ITEM_NAME, MB.MENU_PAGE, MB.MENU_PARENT_CODE, A.MENU_ID, VC.PICK_ID_D
                                            HAVING 
                                                (ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {pickDetailTable} WHERE DT_CODE = B.DT_CODE), 0) -
								 ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {detailTable} D
										 LEFT JOIN {table} M ON D.TRAN_ID = M.TRAN_ID
										 WHERE D.PICK_ID_D = B.DT_CODE AND D.DLT = 'T' AND M.DLT = 'T'), 0)) <> 0
                                                        order by DT_CODE desc";

                            SqlCommand sqlCommand3 = new SqlCommand(query, connection3);
                            connection3.Open();
                            SqlDataReader reader3 = sqlCommand3.ExecuteReader();
                            while (reader3.Read())
                            {
                                var row3 = new
                                {
                                    ID = Convert.ToString(reader3["TRAN_ID"]),
                                    LB_DATE = ((reader3["LB_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader3["LB_DATE"]).ToString("dd-MM-yyyy")),
                                    RINV_DATE = ((reader3["RINV_DATE"] == DBNull.Value || Convert.ToDateTime(reader3["RINV_DATE"]) == new DateTime(1900, 1, 1)) ? null : Convert.ToDateTime(reader3["RINV_DATE"]).ToString("yyyy-MM-dd")),
                                    DEL_DATE = ((reader3["DEL_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader3["DEL_DATE"]).ToString("yyyy-MM-dd")),
                                    DUE_DATE = ((reader3["DUE_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader3["DUE_DATE"]).ToString("yyyy-MM-dd")),
                                    VOUCHER_NO = ((reader3["VOUCHER_NO"] == DBNull.Value) ? "" : Convert.ToString(reader3["VOUCHER_NO"])),
                                    PARTY_CODE = ((reader3["PARTY_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["PARTY_CODE"])),
                                    SPARTY_CODE = ((reader3["SCODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["SCODE"])),
                                    ACT_CODE = ((reader3["ACT_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["ACT_CODE"])),
                                    SACT_CODE = ((reader3["SACODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["SACODE"])),
                                    PARTY_DDL = Convert.ToString(reader3["PARTY_CODE"]) + Convert.ToString(reader3["ACT_CODE"]),
                                    PARTY_NAME = ((reader3["PARTY_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader3["PARTY_NAME"])),
                                    REF = ((reader3["REF"] == DBNull.Value) ? "" : Convert.ToString(reader3["REF"])),
                                    RINV_NO = ((reader3["RINV_NO"] == DBNull.Value) ? "" : Convert.ToString(reader3["RINV_NO"])),
                                    HS_CODE = ((reader3["HS_CODE"] == DBNull.Value) ? "" : Convert.ToString(reader3["HS_CODE"])),
                                    IQTY = ((reader3["ISSUE_QTY"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["ISSUE_QTY"])),
                                    TOTAL_PACK = ((reader3["B_QTY"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["B_QTY"])),
                                    RQTY = ((reader3["R_QTY"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["R_QTY"])),
                                    UNIT = ((reader3["UNIT"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["UNIT"])),
                                    UNIT_NAME = ((reader3["UNIT_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader3["UNIT_NAME"])),
                                    RATE = ((reader3["RATE"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["RATE"])),
                                    LOCAL_CHARGES = ((reader3["LOCAL_CHARGES"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["LOCAL_CHARGES"])),
                                    DISC_M = ((reader3["DISC_M"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["DISC_M"])),
                                    COMM_VAL = ((reader3["COMM_VAL"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["COMM_VAL"])),
                                    DISC = ((reader3["DISC"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["DISC"])),
                                    DISC_AMT = ((reader3["DISC_AMT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["DISC_AMT"])),
                                    ADV = ((reader3["ADV"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["ADV"])),
                                    ADV_AMT = ((reader3["ADV_AMT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["ADV_AMT"])),
                                    TAX = ((reader3["TAX"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["TAX"])),
                                    TAX_AMT = ((reader3["TAX_AMT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["TAX_AMT"])),
                                    PACK = ((reader3["PACK"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["PACK"])),

                                    QTY = ((reader3["B_CRTN"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["B_CRTN"])),

                                    CURR_CODE = ((reader3["CURR_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["CURR_CODE"])),
                                    CRATE = ((reader3["CRATE"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["CRATE"])),
                                    DUE_DAYS = ((reader3["DUE_DAYS"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["DUE_DAYS"])),
                                    WEIGHT = ((reader3["WEIGHT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["WEIGHT"])),
                                    AMT = ((reader3["AMT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["AMT"])),
                                    COLOR_NAME = ((reader3["COLOR"] == DBNull.Value) ? "" : Convert.ToString(reader3["COLOR"])),
                                    SIZE_NAME = ((reader3["SIZE"] == DBNull.Value) ? "" : Convert.ToString(reader3["SIZE"])),
                                    DOC = ((reader3["DOC"] == DBNull.Value) ? "" : Convert.ToString(reader3["DOC"])),
                                    COLOR = ((reader3["COLOR_ID"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["COLOR_ID"])),
                                    SIZE = ((reader3["SIZE_ID"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["SIZE_ID"])),
                                    PICK_ID_D = ((reader3["DT_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["DT_CODE"])),
                                    ITEM_CODE = ((reader3["ITEM_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["ITEM_CODE"])),
                                    ITEM_NAME = ((reader3["ITEM_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader3["ITEM_NAME"])),
                                    WAREHOUSE = ((reader3["WAREHOUSE_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["WAREHOUSE_CODE"])),
                                    WAREHOUSE_NAME = ((reader3["WAREHOUSE_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader3["WAREHOUSE_NAME"])),
                                    TERMS = ((reader3["TERMS"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["TERMS"])),
                                    GRADE = ((reader3["GRADE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["GRADE"])),
                                    GRADE_NAME = ((reader3["GRADE_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader3["GRADE_NAME"])),
                                    COMM_AMT = ((reader3["COMM_AMT"] == DBNull.Value) ? "" : Convert.ToString(reader3["COMM_AMT"])),
                                    BTYPE = ((reader3["BTYPE"] == DBNull.Value) ? "" : Convert.ToString(reader3["BTYPE"])),
                                    COMM = ((reader3["COMM"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["COMM"])),
                                    LINK = string.Concat(new string[]
                                    {
                                        "/",
                                        Convert.ToString(reader3["MENU_PAGE"]),
                                        "?MOID=",
                                        Convert.ToString(reader3["MENU_PARENT_CODE"]),
                                        "&Code=",
                                        Convert.ToString(reader3["MENU_ID"])
                                    })
                                };
                                jsonDataResult3.Add(row3);
                            }
                            reader3.Close();
                        }

                        response.data = jsonDataResult3;
                        response.msg = "";
                        response.msgType = 1;
                    }
                }
                
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage = _catchMessage + "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }

        public MyHttpResponseMessage GetSalesQuotationPickData(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    // SalesOrder PickData - Master list from TBL_SQ_MASTER
                    string pickMasterQuery = @"
SELECT
    M.TRAN_ID,
    M.V_DATE,
    M.VOUCHER_NO,
    M.PARTY_CODE,
    M.ACT_CODE,
    PT.PARTY_NAME,
    M.REMARKS
FROM TBL_SQ_MASTER M
LEFT OUTER JOIN TBL_PARTY_TYPES PT
    ON PT.PARTY_CODE = M.PARTY_CODE
   AND PT.ACT_CODE = M.ACT_CODE
WHERE M.DLT = 'T'
  AND M.BCODE = @BCODE
  AND M.PERIOD_ID = @PERIOD_ID
  AND M.ASTATUS = 'Y'
ORDER BY M.TRAN_ID DESC";

                    SqlCommand command = new SqlCommand(pickMasterQuery, connection);
                    command.Parameters.AddWithValue("@BCODE", common.Branch);
                    command.Parameters.AddWithValue("@PERIOD_ID", common.Period);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            ID = Convert.ToString(reader["TRAN_ID"]),
                            TRAN_ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                            V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                            VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? "" : Convert.ToString(reader["VOUCHER_NO"]),
                            PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                            ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                            PARTY_NAME = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
                            REMARKS = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
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
                    _catchMessage = _catchMessage + "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }

        public MyHttpResponseMessage GetSalesQuotationPickByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> masterResult = new List<object>();
                List<object> detailResult = new List<object>();

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    connection.Open();

                    // SalesOrder PickData - selected Master from TBL_SQ_MASTER
                    string pickMasterByCodeQuery = @"
SELECT
    M.TRAN_ID,
    M.V_DATE,
    M.VOUCHER_NO,
    M.PARTY_CODE,
    M.ACT_CODE,
    PT.PARTY_NAME,
    M.REMARKS
FROM TBL_SQ_MASTER M
LEFT OUTER JOIN TBL_PARTY_TYPES PT
    ON PT.PARTY_CODE = M.PARTY_CODE
   AND PT.ACT_CODE = M.ACT_CODE
WHERE M.DLT = 'T'
  AND M.TRAN_ID = @TRAN_ID
  AND M.BCODE = @BCODE
  AND M.PERIOD_ID = @PERIOD_ID
  AND M.ASTATUS = 'Y'";

                    SqlCommand masterCommand = new SqlCommand(pickMasterByCodeQuery, connection);
                    masterCommand.Parameters.AddWithValue("@TRAN_ID", code);
                    masterCommand.Parameters.AddWithValue("@BCODE", common.Branch);
                    masterCommand.Parameters.AddWithValue("@PERIOD_ID", common.Period);
                    SqlDataReader masterReader = masterCommand.ExecuteReader();
                    while (masterReader.Read())
                    {
                        var row = new
                        {
                            ID = Convert.ToString(masterReader["TRAN_ID"]),
                            TRAN_ID = masterReader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(masterReader["TRAN_ID"]),
                            V_DATE = masterReader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(masterReader["V_DATE"]).ToString("yyyy-MM-dd"),
                            VOUCHER_NO = masterReader["VOUCHER_NO"] == DBNull.Value ? "" : Convert.ToString(masterReader["VOUCHER_NO"]),
                            PARTY_CODE = masterReader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(masterReader["PARTY_CODE"]),
                            ACT_CODE = masterReader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(masterReader["ACT_CODE"]),
                            PARTY_NAME = masterReader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(masterReader["PARTY_NAME"]),
                            REMARKS = masterReader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(masterReader["REMARKS"]),
                        };
                        masterResult.Add(row);
                    }
                    masterReader.Close();

                    // SalesOrder PickData - related Detail from TBL_SQ_DETAIL linked by TRAN_ID
                    string pickDetailByCodeQuery = @"
SELECT
    D.TRAN_ID,
    D.DT_CODE,
    D.ITEM_CODE,
    D.QTY,
    D.BCODE,
    D.RATE,
    I.ITEM_NAME,
    I.IUNIT_CODE AS UNIT
FROM TBL_SQ_DETAIL D
INNER JOIN TBL_SQ_MASTER M
    ON M.TRAN_ID = D.TRAN_ID
   AND M.BCODE = D.BCODE
   AND M.PERIOD_ID = D.PERIOD_ID
   AND M.DLT = 'T'
LEFT OUTER JOIN TBL_ITEMSMASTER I
    ON I.ITEM_CODE = D.ITEM_CODE
WHERE D.DLT = 'T'
  AND D.TRAN_ID = @TRAN_ID
  AND D.BCODE = @BCODE
  AND D.PERIOD_ID = @PERIOD_ID
ORDER BY D.DT_CODE DESC";

                    SqlCommand detailCommand = new SqlCommand(pickDetailByCodeQuery, connection);
                    detailCommand.Parameters.AddWithValue("@TRAN_ID", code);
                    detailCommand.Parameters.AddWithValue("@BCODE", common.Branch);
                    detailCommand.Parameters.AddWithValue("@PERIOD_ID", common.Period);
                    SqlDataReader detailReader = detailCommand.ExecuteReader();
                    while (detailReader.Read())
                    {
                        double qty = detailReader["QTY"] == DBNull.Value ? 0 : Convert.ToDouble(detailReader["QTY"]);
                        int rate = detailReader["RATE"] == DBNull.Value ? 0 : Convert.ToInt32(detailReader["RATE"]);
                        double amt = qty * rate;
                        int pickTranId = detailReader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(detailReader["TRAN_ID"]);
                        int pickDtCode = detailReader["DT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(detailReader["DT_CODE"]);
                        var row = new
                        {
                            TRAN_ID = pickTranId,
                            DT_CODE = 0,
                            ITEM_CODE = detailReader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(detailReader["ITEM_CODE"]),
                            ITEM_NAME = detailReader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(detailReader["ITEM_NAME"]),
                            QTY = qty,
                            BCODE = detailReader["BCODE"] == DBNull.Value ? 0 : Convert.ToInt32(detailReader["BCODE"]),
                            RATE = rate,
                            UNIT = detailReader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(detailReader["UNIT"]),
                            AMT = amt,
                            DISC = 0,
                            DISC_AMT = 0,
                            TAX = 0,
                            TAX_AMT = 0,
                            ADV = 0,
                            ADV_AMT = 0,
                            NET_AMT = amt,
                            WAREHOUSE = 2,
                            CHK = "0",
                            CHK1 = false,
                            PICK_ID = pickTranId,
                            PICK_ID_D = pickDtCode,
                        };
                        detailResult.Add(row);
                    }
                    detailReader.Close();
                }

                if (masterResult.Count == 0)
                {
                    response.data = "";
                    response.msg = "Sales Quotation not found.";
                    response.msgType = 2;
                }
                else
                {
                    response.data = masterResult;
                    response.data2 = detailResult;
                    response.msg = "";
                    response.msgType = 1;
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage = _catchMessage + "<br/>" + ex.InnerException.Message;
                }
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }
        public MyHttpResponseMessage GetBarcodeList()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $@"SELECT IM.ITEM_NAME AS ITEM_ID, IM.ITEM_CODE AS ITEM_CODE, C.GROUP_CODE AS COLOR_ID, S.GROUP_CODE AS SIZE_ID,B.CODE AS BARCODE_CODE,S.GROUP_NAME AS SIZE, C.GROUP_NAME AS COLOR, B.BARCODE, B.SRATE AS RATE FROM TBL_BARCODE B
                                    LEFT OUTER JOIN TBL_ITEMSMASTER IM
                                    ON IM.ITEM_CODE = B.ITEM_CODE
                                    LEFT OUTER JOIN TBL_SIZE S
                                    ON S.GROUP_CODE = B.SIZE
                                    LEFT OUTER JOIN TBL_COLOR C
                                    ON C.GROUP_CODE = B.COLOR
                                    WHERE B.DLT = 'T' AND IM.ASTATUS = 'Y' AND IM.ASTATUS = 'Y'
                                    ORDER BY ITEM_ID , SIZE";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            BARCODE_CODE = Convert.ToString(reader["BARCODE_CODE"]),
                            ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                            ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                            SIZE_NAME = Convert.ToString(reader["SIZE"]),
                            COLOR_NAME = Convert.ToString(reader["COLOR"]),
                            BARCODE = Convert.ToString(reader["BARCODE"]),
                            RATE = Convert.ToInt32(reader["RATE"]),
                            AMT = Convert.ToInt32(reader["RATE"]),
                            NET_AMT = Convert.ToInt32(reader["RATE"]),
                            COLOR = Convert.ToInt32(reader["COLOR_ID"]),
                            SIZE = Convert.ToInt32(reader["SIZE_ID"]),
                            QTY = 1,
                            BAL_QTY = 1,
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
        public MyHttpResponseMessage GetPrintData(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? pickDetail = string.Empty;
                string? b_i = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE2;
                    pickDetail = menu.PICK_TABLE_DETAIL;
                    b_i = menu.B_I;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        var query = $@"SELECT IM.ITEM_NAME AS ITEM,IM.BARCODE,D.QTY,D.RATE,D.DT_CODE
                                    FROM {table} D
                                    LEFT OUTER JOIN TBL_ITEMSMASTER IM 
                                    ON IM.ITEM_CODE = D.ITEM_CODE 
                                    WHERE D.DLT = 'T' AND D.BCODE = {common.Branch}  AND D.PERIOD_ID = {common.Period} AND D.TRAN_ID = {code} ORDER BY D.DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                ITEM = Convert.ToString(reader["ITEM"]),
                                BARCODE = Convert.ToString(reader["BARCODE"]),
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

        public MyHttpResponseMessage GetPurchaseBillPickDetailByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.PICK_TABLE_DETAIL;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT TRAN_ID,DT_CODE,M.ITEM_CODE, QTY,UNIT,QTY2,BAL_QTY,M.COLOR,M.SIZE, " +
                         " RATE,AMT,DISC,DISC_AMT, TAX,TAX_AMT,ADV,ADV_AMT,NET_AMT, DT_DESC, " +
                         " CL.GROUP_NAME AS COLORNAME,SL.GROUP_NAME AS SIZENAME,M.GRADE, WAREHOUSE,DEL_DATE,DUE_DATE, DUE_DAYS,VEH,BCODE,PERIOD_ID, M.ADD_USER_ID,M.ADD_DATE," +
                         " M.ADD_COMPUTER_NAME,M.ADD_IP_ADDRESS, M.EDIT_USER_ID,M.EDIT_DATE,M.EDIT_COMPUTER_NAME, " +
                         " M.EDIT_IP_ADDRESS, M.ADD_POSTALCODE,M.EDIT_POSTALCODE,M.MENU_ID,M.DLT,CHK,PICK_ID,PICK_ID_D " +
                         $" FROM {table} M LEFT OUTER JOIN TBL_BARCODE BG ON BG.CODE = M.ITEM_CODE " +
                         " LEFT OUTER JOIN TBL_SIZE SL ON SL.GROUP_CODE = BG.SIZE LEFT OUTER JOIN TBL_COLOR CL ON CL.GROUP_CODE = BG.COLOR" +
                         " WHERE  M.DLT = 'T' AND TRAN_ID = '" + code + "' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "'" +
                         " ORDER BY DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = 0,
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToInt32(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_AMT = Convert.ToString(reader["DISC_AMT"]),
                                TAX = Convert.ToString(reader["TAX"]),
                                TAX_AMT = Convert.ToString(reader["TAX_AMT"]),
                                ADV = Convert.ToString(reader["ADV"]),
                                ADV_AMT = Convert.ToString(reader["ADV_AMT"]),
                                NET_AMT = Convert.ToString(reader["NET_AMT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                COLOR = reader["COLOR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COLOR"]),
                                SIZE = reader["SIZE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE"]),
                                GRADE = Convert.ToInt32(reader["GRADE"]),
                                WAREHOUSE = Convert.ToInt32(reader["WAREHOUSE"]),
                                DEL_DATE = Convert.ToString(reader["DEL_DATE"]),
                                DUE_DATE = Convert.ToString(reader["DUE_DATE"]),
                                DUE_DAYS = Convert.ToString(reader["DUE_DAYS"]),
                                VEH = Convert.ToString(reader["VEH"]),
                                CHK = Convert.ToString(reader["CHK"]),
                                CHK1 = Convert.ToString(reader["CHK"]) == "1" ? true : false,
                                PICK_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                PICK_ID_D = Convert.ToInt32(reader["DT_CODE"]),

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

        public MyHttpResponseMessage GetPurchaseBillDetailByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);

                string? tableMaster = string.Empty;
                string? tableDetail = string.Empty;
                string? pickMaster = string.Empty;
                string? pickDetail = string.Empty;

                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    tableMaster = menu.TABLE1;
                    tableDetail = menu.TABLE2;
                    pickMaster = menu.PICK_TABLE_MASTER;
                    pickDetail = menu.PICK_TABLE_DETAIL;
                }

                //if (!String.IsNullOrWhiteSpace(tableMaster) && !String.IsNullOrWhiteSpace(tableDetail) && !String.IsNullOrWhiteSpace(pickMaster) && !String.IsNullOrWhiteSpace(pickDetail))
                if (!String.IsNullOrWhiteSpace(tableMaster) && !String.IsNullOrWhiteSpace(tableDetail) )
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        //string qufery = "SELECT TRAN_ID,PARTY_CODE,ACT_CODE,DT_CODE,ITEM_CODE,HS_CODE," +
                        //    " QTY,UNIT,QTY2,BAL_QTY," +
                        //    " RATE,WEIGHT,AMT,DISC,DISC_AMT," +
                        //    " TAX,TAX_AMT,ADV,ADV_AMT,NET_AMT," +
                        //    " DT_DESC,COLOR,SIZE,GRADE," +
                        //    " WAREHOUSE,DEL_DATE,DUE_DATE," +
                        //    " DUE_DAYS,VEH,BCODE,PERIOD_ID," +
                        //    " ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
                        //    " EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                        //    " ADD_POSTALCODE,EDIT_POSTALCODE,MENU_ID,DLT,CHK,PICK_ID,PICK_ID_D" +
                        //    $" FROM {table} " +
                        //    " WHERE  DLT = 'T' AND TRAN_ID = '" + code + "' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "'" +
                        //    " ORDER BY DT_CODE DESC";

                        string query = $@"SELECT D.TRAN_ID,D.LAST_RATE,D.BARCODE, D.PARTY_CODE, D.ACT_CODE, D.DT_CODE, D.ITEM_CODE, D.HS_CODE, D.QTY, D.UNIT, D.QTY2, D.BAL_QTY, D.RATE, D.WEIGHT, D.AMT, D.DISC, D.DISC_AMT, 
                                            D.TAX, D.TAX_AMT, D.ADV, D.ADV_AMT, D.NET_AMT, D.PACK, D.TOTAL_PACK,D.DT_DESC, D.COLOR, D.SIZE, D.GRADE, D.WAREHOUSE, D.DEL_DATE, D.DUE_DATE, D.DUE_DAYS, D.VEH, D.BCODE, D.PERIOD_ID,
                                            D.ADD_USER_ID, D.ADD_DATE, D.ADD_COMPUTER_NAME, D.ADD_IP_ADDRESS, D.EDIT_USER_ID, D.EDIT_DATE, D.EDIT_COMPUTER_NAME, D.EDIT_IP_ADDRESS, D.ADD_POSTALCODE, 
                                            D.EDIT_POSTALCODE, D.MENU_ID, D.DLT, D.CHK, D.PICK_ID, D.PICK_ID_D, PM.TRAN_ID AS PTRAN_ID, PM.VOUCHER_NO, PM.MENU_ID AS PMENU_ID, MB.MENU_PAGE AS MENU_PAGE, MB.MENU_PARENT_CODE
                                            FROM {tableDetail} D
                                            LEFT OUTER JOIN {pickDetail} PD ON PD.DT_CODE = D.PICK_ID_D AND PD.BCODE = D.BCODE AND PD.PERIOD_ID = D.PERIOD_ID
                                            LEFT OUTER JOIN {pickMaster} PM ON PM.TRAN_ID = PD.TRAN_ID AND PM.BCODE = PD.BCODE AND PM.PERIOD_ID = PD.PERIOD_ID
                                            LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = PM.MENU_ID
                                            WHERE  D.DLT = 'T' AND D.TRAN_ID = '{code}' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}' ORDER BY D.DT_CODE DESC";


                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = reader["DT_CODE"] == DBNull.Value ? "" : Convert.ToString(reader["DT_CODE"]),
                                ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                HS_CODE = reader["HS_CODE"] == DBNull.Value ? "" : Convert.ToString(reader["HS_CODE"]),
                                PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                PARTY_DDL = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",
                                QTY = reader["QTY"] == DBNull.Value ? "" : Convert.ToString(reader["QTY"]),
                                PACK = ((reader["PACK"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["PACK"])),
                                TOTAL_PACK = ((reader["TOTAL_PACK"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["TOTAL_PACK"])),
                                UNIT = reader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UNIT"]),
                                WEIGHT = reader["WEIGHT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["WEIGHT"]),
                                QTY2 = reader["QTY2"] == DBNull.Value ? "" : Convert.ToString(reader["QTY2"]),
                                BAL_QTY = reader["BAL_QTY"] == DBNull.Value ? "" : Convert.ToString(reader["BAL_QTY"]),
                                RATE = reader["RATE"] == DBNull.Value ? "" : Convert.ToString(reader["RATE"]),
                                LAST_RATE = reader["LAST_RATE"] == DBNull.Value ? "" : Convert.ToString(reader["LAST_RATE"]),
                                BARCODE = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                AMT = reader["AMT"] == DBNull.Value ? "" : Convert.ToString(reader["AMT"]),
                                DISC = reader["DISC"] == DBNull.Value ? "" : Convert.ToString(reader["DISC"]),
                                DISC_AMT = reader["DISC_AMT"] == DBNull.Value ? "" : Convert.ToString(reader["DISC_AMT"]),
                                TAX = reader["TAX"] == DBNull.Value ? "" : Convert.ToString(reader["TAX"]),
                                TAX_AMT = reader["TAX_AMT"] == DBNull.Value ? "" : Convert.ToString(reader["TAX_AMT"]),
                                ADV = reader["ADV"] == DBNull.Value ? "" : Convert.ToString(reader["ADV"]),
                                ADV_AMT = reader["ADV_AMT"] == DBNull.Value ? "" : Convert.ToString(reader["ADV_AMT"]),
                                NET_AMT = reader["NET_AMT"] == DBNull.Value ? "" : Convert.ToString(reader["NET_AMT"]),
                                DT_DESC = reader["DT_DESC"] == DBNull.Value ? "" : Convert.ToString(reader["DT_DESC"]),
                                COLOR = reader["COLOR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COLOR"]),
                                SIZE = reader["SIZE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE"]),
                                GRADE = reader["GRADE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GRADE"]),
                                WAREHOUSE = reader["WAREHOUSE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["WAREHOUSE"]),
                                DEL_DATE = reader["DEL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DEL_DATE"]).ToString("yyyy-MM-dd"),
                                DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),
                                DUE_DAYS = reader["DUE_DAYS"] == DBNull.Value ? "" : Convert.ToString(reader["DUE_DAYS"]),
                                VEH = reader["VEH"] == DBNull.Value ? "" : Convert.ToString(reader["VEH"]),
                                CHK = reader["CHK"] == DBNull.Value ? "" : Convert.ToString(reader["CHK"]),
                                VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? "" : Convert.ToString(reader["VOUCHER_NO"]),
                                CHK1 = Convert.ToString(reader["CHK"]) == "1" ? true : false,
                                PICK_ID = reader["PICK_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PICK_ID"]),
                                PICK_ID_D = reader["PICK_ID_D"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PICK_ID_D"]),
                                ID = reader["PTRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PTRAN_ID"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["PMENU_ID"]),

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
        public MyHttpResponseMessage GetPurchaseBillCommissionByCode(int code, Common common)
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
                        string query = "SELECT * FROM TBL_COMM_GEN" +
                            " WHERE  DLT = 'T' AND BILL_TRAN_ID = '" + code + "' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "'" +
                            " ORDER BY DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = reader["DT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DT_CODE"]),
                                TRAN_ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                                BILL_TRAN_ID = reader["BILL_TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BILL_TRAN_ID"]),
                                COMM_UNIT = Convert.ToString(reader["COMM_UNIT"]),
                                COMM_VALUE = Convert.ToString(reader["COMM_VALUE"]),
                                ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                SACT_CODE = reader["SACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SACT_CODE"]),
                                SALESMAN = reader["SALESMAN"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SALESMAN"]),
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

        public MyHttpResponseMessage GetPurchaseBillDetailByItem(int code, int qty, Common common)
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
                        string query = "SELECT B.CODE, B.SRATE, B.COLOR, B.SIZE, IT.ITEM_ID, IT.SALE_RATE FROM TBL_BARCODE B " +
                            $"LEFT OUTER JOIN TBL_ITEMSMASTER IT ON B.ITEM_CODE = IT.ITEM_CODE " +
                            "WHERE  B.DLT = 'T' AND B.ITEM_CODE = " + code + "";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = 0,
                                ITEM_CODE = Convert.ToInt32(reader["CODE"]),
                                ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                                QTY = qty,
                                UNIT = 0,
                                QTY2 = qty.ToString(),
                                BAL_QTY = qty.ToString(),
                                RATE = Convert.ToString(reader["SRATE"]),
                                AMT = (Convert.ToInt32(reader["SRATE"]) * qty).ToString(),
                                DISC = "",
                                DISC_AMT = "",
                                TAX = "",
                                TAX_AMT = "",
                                ADV = "",
                                ADV_AMT = "",
                                NET_AMT = (Convert.ToInt32(reader["SRATE"]) * qty).ToString(),
                                DT_DESC = "",
                                COLOR = reader["COLOR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COLOR"]),
                                SIZE = reader["SIZE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE"]),
                                GRADE = 0,
                                WAREHOUSE = 0,
                                DEL_DATE = "",
                                DUE_DATE = "",
                                DUE_DAYS = "",
                                VEH = "",
                                CHK = "",
                                CHK1 = false,
                                PICK_ID = 0
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

        private int GenerateNextId(Common common, SqlCommand command, string Commission = "")
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
                    if (Commission == "")
                    {
                        string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM {table} WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                        command.CommandText = maxIdQuery;
                        object result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM TBL_COMM_GEN WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                        command.CommandText = maxIdQuery;
                        object result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }

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

        private int GenerateNextDetailId(Common common, SqlCommand command, string Commission = "")
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
                    if (Commission == "")
                    {
                        string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) FROM {table}";
                        command.CommandText = maxIdQuery;
                        object result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) + 1 FROM TBL_COMM_GEN";
                        command.CommandText = maxIdQuery;
                        object result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public MyHttpResponseMessage Save(CustomPurchaseBill modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty, b_i = string.Empty, stk_status = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    b_i = menu.B_I;
                    stk_status = menu.STK_STATUS;
                }

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
                {
                    var Ip = common.IPAddress;
                    var Computer = common.ComputerName;
                    var Postal = common.PostalCode;
                    var username = common.Username;
                    var branch = common.Branch;
                    var period = common.Period;
                    var periodInfo = _periodRepository.GetPeriodById(Convert.ToInt32(period));
                    string startDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
                    string endDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
                    var menuID = common.MenuID;
                    bool isStockSufficient = true;
                    string InSufficientItem = "";
                    double InSufficientItemQty = 0;
                    DataTable dt = new DataTable();
                    string connectionString = new SQLService().getconnstring();
                    Dictionary<int?, double?> currentItems = new Dictionary<int?, double?>();
                    Dictionary<int?, double?> previousItems = new Dictionary<int?, double?>();
                    Dictionary<int?, double?> stockBalance = new Dictionary<int?, double?>();
                    //if (stk_status == "Y")
                    //{
                    //    if (b_i == "B")
                    //    {
                    //        foreach (var item in modelRecord.Detail.ToList())
                    //        {
                    //            if (!currentItems.ContainsKey(item.ITEM_CODE))
                    //            {
                    //                currentItems.Add(item.ITEM_CODE, item.BAL_QTY);
                    //            }
                    //            else
                    //            {
                    //                currentItems[item.ITEM_CODE] += item.BAL_QTY;
                    //            }
                    //        }

                    //        var itemCodes = string.Join(",", modelRecord.Detail.Select(x => x.ITEM_CODE).Distinct());
                    //        string query = $@"EXEC STKPROC 71,'{startDate}','{endDate}','{common.Branch}','{common.Period}','',''";
                    //        using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    //        {
                    //            SqlCommand command = new SqlCommand(query, connection);
                    //            connection.Open();
                    //            using (SqlDataReader reader = command.ExecuteReader())
                    //            {
                    //                while (reader.Read())
                    //                {
                    //                    int? itemCode = reader["ITEM_ID"] as int?;
                    //                    double? balance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["BALANCE"]);

                    //                    if (itemCode != null)
                    //                        stockBalance.Add(itemCode, balance);
                    //                }
                    //            }
                    //        }

                    //        if (modelRecord.Master.TRAN_ID > 0)
                    //        {
                    //            previousItems = PreviousStockInBill(detailTable, modelRecord.Master.TRAN_ID, period, branch);

                    //            foreach (var item in previousItems)
                    //            {
                    //                if (currentItems.ContainsKey(item.Key))
                    //                {
                    //                    currentItems[item.Key] = (currentItems[item.Key] ?? 0) - (item.Value ?? 0);

                    //                    if (currentItems[item.Key] < 0)
                    //                    {
                    //                        currentItems[item.Key] = 0;
                    //                    }
                    //                }
                    //            }
                    //        }

                    //        foreach (var item in currentItems)
                    //        {
                    //            double availableStock = stockBalance.ContainsKey(item.Key) ? stockBalance[item.Key] ?? 0 : 0;
                    //            double currentQty = item.Value ?? 0;

                    //            if (currentQty > availableStock)
                    //            {
                    //                List<CustomKeyValuPair> barcodes = DropdownService.BarcodesKeyAndValue();
                    //                var SelectedItem = barcodes.Where(b => b.key == item.Key).FirstOrDefault();
                    //                InSufficientItem = SelectedItem.value;
                    //                InSufficientItemQty = availableStock;
                    //                isStockSufficient = false;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}

                    if (isStockSufficient)
                    {
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlTransaction transaction = connection.BeginTransaction();
                            SqlCommand command = connection.CreateCommand();
                            command.Transaction = transaction;
                            try
                            {
                                if (modelRecord.Master.SCODE != 0 && modelRecord.Master.SCODE != null)
                                {
                                    string maxIdQuery1 = "SELECT SACT_CODE FROM TBL_PARTY_TYPES WHERE PARTY_CODE = '" + modelRecord.Master.PARTY_CODE + "' AND SACT_CODE IS NOT NULL";
                                    using (SqlConnection connectionNew = new SqlConnection(new SQLService().getconnstring()))
                                    {
                                        SqlCommand commandNew1 = new SqlCommand(maxIdQuery1, connectionNew);
                                        connectionNew.Open();
                                        object result1 = commandNew1.ExecuteScalar();
                                        modelRecord.Master.SACODE = result1 == DBNull.Value ? 0 : Convert.ToInt32(result1);
                                    }
                                }
                                else
                                {
                                    modelRecord.Master.SACODE = 0;
                                }

                                string query = "", commQuery = "", detailQuery = "", voucherNo = string.Empty;
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

                                    query = $"INSERT INTO {table}" +
                                            "(TRAN_ID,V_DATE,VOUCHER_NO,PARTY_CODE,ACT_CODE,DOC,RINV_NO,RINV_DATE,REGION,TERMS,CARTAGE," +
                                            "REF,CURR_CODE,CRATE,REMARKS,SCODE,SACODE,BCODE,PERIOD_ID,COMM,COMM_VAL,COMM_AMT,DISC,BTYPE," +
                                            "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                            "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                                            "EDIT_POSTALCODE,ASTATUS,MENU_ID,DLT)" +
                                            "VALUES" +
                                            "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.PARTY_CODE + "','" + modelRecord.Master.ACT_CODE + "'," +
                                            "'" + modelRecord.Master.DOC + "','" + modelRecord.Master.RINV_NO + "','" + modelRecord.Master.RINV_DATE + "','" + modelRecord.Master.REGION + "','" + modelRecord.Master.TERMS + "','" + modelRecord.Master.CARTAGE + "'," +
                                            "'" + modelRecord.Master.REF + "','" + modelRecord.Master.CURR_CODE + "','" + modelRecord.Master.CRATE + "','" + modelRecord.Master.REMARKS + "'," +
                                            "'" + modelRecord.Master.SCODE + "','" + modelRecord.Master.SACODE + "','" + branch + "','" + period + "','" + modelRecord.Master.COMM + "'," +
                                            "'" + modelRecord.Master.COMM_VAL + "','" + modelRecord.Master.COMM_AMT + "','" + modelRecord.Master.DISC + "','" + modelRecord.Master.BTYPE + "'," +
                                            "'" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                            "'" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Computer + "','" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.Master.ASTATUS + "','" + menuID + "','T')";
                                    command.CommandText = query;
                                    command.ExecuteNonQuery();
                                }
                                else
                                {
                                    query = $"UPDATE {table} SET V_DATE = '" + modelRecord.Master.V_DATE + @"',
                                                    PARTY_CODE = '" + modelRecord.Master.PARTY_CODE + @"',
                                                    TERMS = '" + modelRecord.Master.TERMS + @"',
                                                    ACT_CODE = '" + modelRecord.Master.ACT_CODE + @"',
                                                    REF = '" + modelRecord.Master.REF + @"',
                                                    CARTAGE = '" + modelRecord.Master.CARTAGE + @"',
                                                    RINV_NO = '" + modelRecord.Master.RINV_NO + @"',
                                                    RINV_DATE = '" + modelRecord.Master.RINV_DATE + @"',
                                                    CURR_CODE = '" + modelRecord.Master.CURR_CODE + @"',
                                                    REGION = '" + modelRecord.Master.REGION + @"',
                                                    CRATE = '" + modelRecord.Master.CRATE + @"',
                                                    DOC = '" + modelRecord.Master.DOC + @"',
                                                    REMARKS = '" + modelRecord.Master.REMARKS + @"',
                                                    SCODE = '" + modelRecord.Master.SCODE + @"',
                                                    SACODE = '" + modelRecord.Master.SACODE + @"',
                                                    COMM = '" + modelRecord.Master.COMM + @"',
                                                    COMM_AMT = '" + modelRecord.Master.COMM_AMT + @"',
                                                    COMM_VAL = '" + modelRecord.Master.COMM_VAL + @"',
                                                    DISC = '" + modelRecord.Master.DISC + @"',
                                                    BTYPE = '" + modelRecord.Master.BTYPE + @"',
                                                    EDIT_USER_ID = '" + username + @"',
                                                    EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                                    EDIT_COMPUTER_NAME = '" + Computer + @"',
                                                    EDIT_IP_ADDRESS = '" + Ip + @"',
                                                    EDIT_POSTALCODE = '" + Postal + @"',
                                                    ASTATUS = '" + modelRecord.Master.ASTATUS + @"'
                                                    WHERE TRAN_ID = '" + modelRecord.Master.TRAN_ID + "' AND BCODE = '" + branch + "' AND PERIOD_ID = '" + period + "'";
                                    command.CommandText = query;
                                    command.ExecuteNonQuery();
                                }

                                var isDetailAdded = true;

                                StringBuilder insertQueryBuilder = new StringBuilder();
                                StringBuilder insertCommQueryBuilder = new StringBuilder();
                                StringBuilder updateQueryBuilder = new StringBuilder();
                                StringBuilder updateCommQueryBuilder = new StringBuilder();

                                bool allowInserts = true;
                                bool hasInserts = false;
                                bool hasUpdates = false;
                                int detailCode = GenerateNextDetailId(common, command);
                                foreach (var item in modelRecord.Detail.ToList())
                                {
                                    var amt = item.QTY * item.RATE;
                                    item.AMT = amt;
                                    item.NET_AMT = amt;

                                    if (item.RATE > 0 && (item.AMT == null || item.AMT == 0))
                                    {
                                        response.msg = "Something went wrong";
                                        response.msgType = 2;
                                        return response;
                                    }

                                    try
                                    {
                                        if (item.DT_CODE == null || item.DT_CODE == 0)
                                        {
                                            detailCode++;
                                            if (detailCode > 0)
                                            {
                                                if (!hasInserts)
                                                {
                                                    hasInserts = true;
                                                }

                                                insertQueryBuilder.AppendLine(
                                                    $"INSERT INTO {detailTable} (TRAN_ID, DT_CODE, ITEM_CODE, PARTY_CODE, ACT_CODE,LAST_RATE,BARCODE, QTY, UNIT, QTY2, PACK, TOTAL_PACK, BAL_QTY, WEIGHT, RATE, AMT, DISC, DISC_AMT, TAX, TAX_AMT, ADV, ADV_AMT, NET_AMT, DT_DESC, COLOR, SIZE, GRADE,  WAREHOUSE, DEL_DATE, DUE_DATE, DUE_DAYS, VEH, BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT, CHK, PICK_ID, PICK_ID_D,HS_CODE) VALUES " +
                                                    $"('{modelRecord.Master.TRAN_ID}', '{detailCode}', '{item.ITEM_CODE}', '{item.PARTY_CODE}', '{item.ACT_CODE}', '{item.LAST_RATE}', '{item.BARCODE}','{item.QTY}', " +
                                                    $"'{item.UNIT}', '{item.QTY2}', '{item.PACK}', '{item.TOTAL_PACK}', '{item.BAL_QTY}', '{item.WEIGHT}', '{item.RATE}', '{item.AMT}', '{item.DISC}', " +
                                                    $"'{item.DISC_AMT}', '{item.TAX}', '{item.TAX_AMT}', '{item.ADV}', '{item.ADV_AMT}', '{item.NET_AMT}', '{item.DT_DESC}', " +
                                                    $"'{item.COLOR}', '{item.SIZE}', '{item.GRADE}', '{item.WAREHOUSE}', '{item.DEL_DATE}', '{item.DUE_DATE}', " +
                                                    $"'{item.DUE_DAYS}', '{item.VEH}', '{branch}', '{period}', '{username}', " +
                                                    $"'{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', '{username}', " +
                                                    $"'{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', " +
                                                    $"'{Postal}', '{Postal}', '{menuID}', 'T', '{item.CHK}', '{item.PICK_ID}', '{item.PICK_ID_D}','{item.HS_CODE}');");
                                            }
                                            else
                                            {
                                                isDetailAdded = false;
                                            }
                                        }
                                        else
                                        {
                                            if (!hasUpdates)
                                            {
                                                hasUpdates = true;
                                            }

                                            updateQueryBuilder.AppendLine(
                                                $"UPDATE {detailTable} SET " +
                                                $"ITEM_CODE = '{item.ITEM_CODE}', " +
                                                $"QTY = '{item.QTY}', " +
                                                $"PARTY_CODE = '{item.PARTY_CODE}', " +
                                                $"ACT_CODE = '{item.ACT_CODE}', " +
                                                $"LAST_RATE = '{item.LAST_RATE}', " +
                                                $"BARCODE = '{item.BARCODE}', " +
                                                $"UNIT = '{item.UNIT}', " +
                                                $"QTY2 = '{item.QTY2}', " +
                                                $"PACK = '{item.PACK}', " +
                                                $"TOTAL_PACK = '{item.TOTAL_PACK}', " +
                                                $"BAL_QTY = '{item.BAL_QTY}', " +
                                                $"WEIGHT = '{item.WEIGHT}', " +
                                                $"RATE = '{item.RATE}', " +
                                                $"AMT = '{item.AMT}', " +
                                                $"DISC = '{item.DISC}', " +
                                                $"DISC_AMT = '{item.DISC_AMT}', " +
                                                $"TAX = '{item.TAX}', " +
                                                $"TAX_AMT = '{item.TAX_AMT}', " +
                                                $"ADV = '{item.ADV}', " +
                                                $"ADV_AMT = '{item.ADV_AMT}', " +
                                                $"NET_AMT = '{item.NET_AMT}', " +
                                                $"DT_DESC = '{item.DT_DESC}', " +
                                                $"COLOR = '{item.COLOR}', " +
                                                $"SIZE = '{item.SIZE}', " +
                                                $"GRADE = '{item.GRADE}', WAREHOUSE = '{item.WAREHOUSE}', DEL_DATE = '{item.DEL_DATE}', " +
                                                $"DUE_DATE = '{item.DUE_DATE}', DUE_DAYS = '{item.DUE_DAYS}', VEH = '{item.VEH}', " +
                                                $"CHK = '{item.CHK}', EDIT_USER_ID = '{username}', " +
                                                $"HS_CODE = '{item.HS_CODE}'," +
                                                $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                $"EDIT_COMPUTER_NAME = '{Computer}', EDIT_IP_ADDRESS = '{Ip}', " +
                                                $"EDIT_POSTALCODE = '{Postal}', DLT = 'T' " +
                                                $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND DT_CODE = '{item.DT_CODE}' " +
                                                $"AND BCODE = '{branch}' AND PERIOD_ID = '{period}';");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        isDetailAdded = false;
                                    }
                                }

                                if (b_i == "I")
                                {
                                    if (modelRecord?.Commission != null && modelRecord.Commission.Count > 0 && modelRecord.Commission[0].ITEM_CODE != null)
                                    {
                                        var BILL_TRAN_ID = code == 0 ? modelRecord.Master.TRAN_ID : code;
                                        if (modelRecord.Commission.Last().SALESMAN == modelRecord.Master.SCODE)
                                        {
                                            dt = GetComm(Convert.ToInt32(modelRecord.Master.SCODE));
                                            foreach (var item in modelRecord.Commission.ToList())
                                            {
                                                try
                                                {
                                                    if (item.DT_CODE == null || item.DT_CODE == 0)
                                                    {

                                                        int CommCode = GenerateNextDetailId(common, command, "Commission");
                                                        if (CommCode > 0)
                                                        {
                                                            if (!hasInserts)
                                                            {
                                                                hasInserts = true;
                                                            }
                                                            insertCommQueryBuilder.AppendLine($@"
                                                    INSERT INTO [dbo].[TBL_COMM_GEN]
                                                    ([BILL_TRAN_ID],[BILL_MENU_ID],[BCODE],[PERIOD_ID],[TRAN_ID],[DT_CODE],[SALESMAN],[SACT_CODE],[ITEM_CODE],[COMM_UNIT],[COMM_VALUE],
                                                     [ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME],[EDIT_IP_ADDRESS],
                                                     [ADD_POSTALCODE],[EDIT_POSTALCODE],[MENU_ID],[DLT])
                                                    VALUES ('{BILL_TRAN_ID}','{common.MenuID}','{common.Branch}','{common.Period}','{item.TRAN_ID}','{CommCode}','{dt.Rows[0]["SALESMAN"]}','{dt.Rows[0]["SACT_CODE"]}',
                                                     '{item.ITEM_CODE}','{item.COMM_UNIT}','{item.COMM_VALUE}','{username}','{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                     '{Computer}','{Ip}','{username}','{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                     '{Computer}','{Ip}','{Postal}','{Postal}','{menuID}','T');");
                                                        }

                                                    }
                                                    else
                                                    {
                                                        updateCommQueryBuilder.AppendLine($@"
                                                UPDATE [dbo].[TBL_COMM_GEN] SET 
                                                 [BILL_MENU_ID]='{common.MenuID}', [BCODE]='{common.Branch}', [PERIOD_ID]='{common.Period}',
                                                 [SALESMAN]='{item.SALESMAN}', [SACT_CODE]='{item.SACT_CODE}', [ITEM_CODE]='{item.ITEM_CODE}',
                                                 [COMM_UNIT]='{item.COMM_UNIT}', [COMM_VALUE]='{item.COMM_VALUE}', [EDIT_USER_ID]='{username}',
                                                 [EDIT_DATE]='{CommonService.GetDateTime("Pakistan Standard Time")}', [EDIT_COMPUTER_NAME]='{Computer}',
                                                 [EDIT_IP_ADDRESS]='{Ip}', [EDIT_POSTALCODE]='{Postal}', [MENU_ID]='{menuID}'
                                                WHERE [TRAN_ID]='{item.TRAN_ID}' AND [DT_CODE] = '{item.DT_CODE}' AND [DLT]='T';");
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    isDetailAdded = false;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            query = @$"UPDATE TBL_COMM_GEN SET DLT = 'F' WHERE BILL_TRAN_ID = '" + BILL_TRAN_ID + "' AND BCODE = '" + branch + "' AND PERIOD_ID = '" + period + "'";
                                            command.CommandText = query;
                                            command.ExecuteNonQuery();

                                            dt = GetComm(Convert.ToInt32(modelRecord.Master.SCODE));
                                            var Tran_Id = GenerateNextId(common, command, "Commission");
                                            int CommCode = GenerateNextDetailId(common, command, "Commission");
                                            foreach (DataRow row in dt.Rows)
                                            {
                                                try
                                                {
                                                    if (CommCode > 0)
                                                    {
                                                        if (!hasInserts)
                                                        {
                                                            hasInserts = true;
                                                        }
                                                        insertCommQueryBuilder.AppendLine($@"
                                                    INSERT INTO [dbo].[TBL_COMM_GEN]
                                                    ([BILL_TRAN_ID],[BILL_MENU_ID],[BCODE],[PERIOD_ID],[TRAN_ID],[DT_CODE],[SALESMAN],[SACT_CODE],[ITEM_CODE],[COMM_UNIT],[COMM_VALUE],
                                                     [ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME],[EDIT_IP_ADDRESS],
                                                     [ADD_POSTALCODE],[EDIT_POSTALCODE],[MENU_ID],[DLT])
                                                    VALUES ('{BILL_TRAN_ID}','{common.MenuID}','{common.Branch}','{common.Period}','{Tran_Id}','{CommCode}','{row["SALESMAN"]}','{row["SACT_CODE"]}',
                                                     '{row["ITEM_CODE"]}','{row["COMM_UNIT"]}','{row["COMM_VALUE"]}','{username}','{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                     '{Computer}','{Ip}','{username}','{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                     '{Computer}','{Ip}','{Postal}','{Postal}','{menuID}','T');");
                                                    }
                                                    else
                                                    {
                                                        isDetailAdded = false;
                                                    }
                                                    CommCode++;
                                                }
                                                catch (Exception)
                                                {
                                                    isDetailAdded = false;
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        var BILL_TRAN_ID = code == 0 ? modelRecord.Master.TRAN_ID : code;

                                        query = @$"UPDATE TBL_COMM_GEN SET DLT = 'F' WHERE BILL_TRAN_ID = '" + BILL_TRAN_ID + "' AND BCODE = '" + branch + "' AND PERIOD_ID = '" + period + "'";
                                        command.CommandText = query;
                                        command.ExecuteNonQuery();

                                        dt = GetComm(Convert.ToInt32(modelRecord.Master.SCODE));
                                        var Tran_Id = GenerateNextId(common, command, "Commission");
                                        int CommCode = GenerateNextDetailId(common, command, "Commission");
                                        
                                        foreach (DataRow row in dt.Rows)
                                        {
                                            try
                                            {
                                                if (CommCode > 0)
                                                {
                                                    if (!hasInserts)
                                                    {
                                                        hasInserts = true;
                                                    }
                                                    insertCommQueryBuilder.AppendLine($@"
                                                    INSERT INTO [dbo].[TBL_COMM_GEN]
                                                    ([BILL_TRAN_ID],[BILL_MENU_ID],[BCODE],[PERIOD_ID],[TRAN_ID],[DT_CODE],[SALESMAN],[SACT_CODE],[ITEM_CODE],[COMM_UNIT],[COMM_VALUE],
                                                     [ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME],[EDIT_IP_ADDRESS],
                                                     [ADD_POSTALCODE],[EDIT_POSTALCODE],[MENU_ID],[DLT])
                                                    VALUES ('{BILL_TRAN_ID}','{common.MenuID}','{common.Branch}','{common.Period}','{Tran_Id}','{CommCode}','{row["SALESMAN"]}','{row["SACT_CODE"]}',
                                                     '{row["ITEM_CODE"]}','{row["COMM_UNIT"]}','{row["COMM_VALUE"]}','{username}','{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                     '{Computer}','{Ip}','{username}','{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                     '{Computer}','{Ip}','{Postal}','{Postal}','{menuID}','T');");
                                                }
                                                else
                                                {
                                                    isDetailAdded = false;
                                                }
                                                CommCode++;
                                            }
                                            catch (Exception)
                                            {
                                                isDetailAdded = false;
                                            }
                                        }
                                    }
                                }


                                if (hasInserts)
                                {
                                    command.CommandText = insertQueryBuilder.ToString() + ";" + insertCommQueryBuilder.ToString();
                                    command.ExecuteNonQuery();
                                }
                                if (hasUpdates)
                                {
                                    command.CommandText = updateQueryBuilder.ToString() + ";" + updateCommQueryBuilder.ToString();
                                    command.ExecuteNonQuery();
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
                        response.msg = $"{InSufficientItem} has only {InSufficientItemQty} in stock.";
                        response.msgType = 2;
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

        public Dictionary<int?, double?> PreviousStockInBill(string table, int? TRAN_ID, string period, string branch)
        {
            List<CurrentItemsInBill> jsonDataResult = new List<CurrentItemsInBill>();
            string query;
            Dictionary<int?, double?> stock = new();
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                query = $"SELECT TRAN_ID,DT_CODE,ITEM_CODE, BAL_QTY FROM {table} " +
                    " WHERE DLT = 'T' AND TRAN_ID = '" + TRAN_ID + "' AND BCODE = '" + branch + "' AND PERIOD_ID = '" + period + "'";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new CurrentItemsInBill
                    {
                        ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                        QTY = reader["BAL_QTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["BAL_QTY"])
                    };
                    jsonDataResult.Add(row);
                }
                reader.Close();
            }
            if (jsonDataResult is not null)
            {
                foreach (var item in jsonDataResult)
                {
                    if (!stock.ContainsKey(item.ITEM_CODE))
                    {
                        stock.Add(Convert.ToInt32(item.ITEM_CODE), Convert.ToDouble(item.QTY));
                    }
                    else
                    {
                        stock[item.ITEM_CODE] += Convert.ToDouble(item.QTY);
                    }
                }
            }
            return stock;
        }

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
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
                    if (code == 0)
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
                            string query = $"UPDATE {table} SET DLT = 'F' WHERE TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            response.msgType = 1;
                            response.msg = "Record Deleted Successfully";
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

                PurchaseBill purchaseBill = new PurchaseBill();
                List<PurchaseBillDetail> purchaseBillDetailList = new List<PurchaseBillDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        purchaseBill = new PurchaseBill
                        {
                            V_DATE = Convert.ToDateTime(reader["V_DATE"]),
                            PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            SCODE = Convert.ToInt32(reader["SCODE"]),
                            SACODE = Convert.ToInt32(reader["SACODE"]),
                            COMM = Convert.ToDouble(reader["COMM"]),
                            REF = Convert.ToString(reader["REF"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            BTYPE = Convert.ToString(reader["BTYPE"]),
                            BCODE = Convert.ToInt32(reader["BCODE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            DISC = Convert.ToDouble(reader["DISC"]),
                            //HS_CODE = Convert.ToString(reader["HS_CODE"]),
                            DOC = Convert.ToString(reader["DOC"]),
                            CURR_CODE = Convert.ToInt32(reader["CURR_CODE"]),
                            CRATE = Convert.ToDouble(reader["CRATE"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new PurchaseBillDetail
                        {
                            ITEM_CODE = Convert.ToInt32(detail_Reader["ITEM_CODE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                            HS_CODE = Convert.ToString(detail_Reader["HS_CODE"]),
                            UNIT = Convert.ToInt32(detail_Reader["UNIT"]),
                            QTY2 = Convert.ToDouble(detail_Reader["QTY2"]),
                            BAL_QTY = Convert.ToDouble(detail_Reader["BAL_QTY"]),
                            RATE = Convert.ToDouble(detail_Reader["RATE"]),
                            AMT = Convert.ToDouble(detail_Reader["AMT"]),
                            DISC = Convert.ToDouble(detail_Reader["DISC"]),
                            DISC_AMT = Convert.ToDouble(detail_Reader["DISC_AMT"]),
                            TAX = Convert.ToDouble(detail_Reader["TAX"]),
                            TAX_AMT = Convert.ToDouble(detail_Reader["TAX_AMT"]),
                            NET_AMT = Convert.ToDouble(detail_Reader["NET_AMT"]),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            COLOR = Convert.ToInt32(detail_Reader["COLOR"]),
                            SIZE = Convert.ToInt32(detail_Reader["SIZE"]),
                            GRADE = Convert.ToInt32(detail_Reader["GRADE"]),
                            WAREHOUSE = Convert.ToInt32(detail_Reader["WAREHOUSE"]),
                            DEL_DATE = Convert.ToDateTime(detail_Reader["DEL_DATE"]),
                            DUE_DATE = Convert.ToDateTime(detail_Reader["DUE_DATE"]),
                            DUE_DAYS = Convert.ToInt32(detail_Reader["DUE_DAYS"]),
                            VEH = Convert.ToString(detail_Reader["VEH"]),
                            CHK = detail_Reader["CHK"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["CHK"]),
                            PICK_ID = Convert.ToInt32(detail_Reader["PICK_ID"]),
                            PICK_ID_D = detail_Reader["PICK_ID_D"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["PICK_ID_D"]),
                            ADV = Convert.ToDouble(detail_Reader["ADV"]),
                            ADV_AMT = Convert.ToDouble(detail_Reader["ADV_AMT"]),
                            PARTY_CODE = Convert.ToInt32(detail_Reader["PARTY_CODE"]),
                            ACT_CODE = Convert.ToInt32(detail_Reader["ACT_CODE"]),
                        };
                        purchaseBillDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customRequisition = new CustomPurchaseBill
                {
                    Master = purchaseBill,
                    Detail = purchaseBillDetailList
                };

                response = this.Save(customRequisition, common);

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

        public MyHttpResponseMessage DeletePurchaseBillDetailByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
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
                    var branch = common.Branch;
                    var period = common.Period;
                    if (code == 0)
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
                            string query = $"UPDATE {table} SET DLT = 'F'" +
                                $" WHERE DT_CODE = '{code}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            response.msgType = 1;
                            response.msg = "Record Deleted Successfully";
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
        public MyHttpResponseMessage DeleteCommDetailByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                var branch = common.Branch;
                var period = common.Period;
                if (code == 0)
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
                        string query = $"UPDATE TBL_COMM_GEN SET DLT = 'F'" +
                            $" WHERE DT_CODE = '{code}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        response.msgType = 1;
                        response.msg = "Record Deleted Successfully";
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
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }

        public MyHttpResponseMessage GetDataForReport(PurchaseBillRDLCReport modelRecord, DataTable dataTable, DataTable taxDataTable, DataTable inspectionServiceChargesDetails, DataTable reportDetailsDDJ, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            PurchaseBillRDLCReport masterData = new PurchaseBillRDLCReport();
            CustomPurchaseBillForPrintReport reportData = new CustomPurchaseBillForPrintReport();
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
                string query = $@"EXEC PROC_PRINT '{table}','{detailTable}','','','{common.Branch}','{common.Period}','{modelRecord.TRAN_ID}','','','{menuDetails.REPORT_NAME}'";

                if (menuDetails.REPORT_NAME == "SaleInvoiceDDJ")
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                        masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                        masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                        masterData.BALANCE = modelRecord.BALANCE;
                        if (reader.Read())
                        {
                            masterData.COMPANY_NAME = reader["C_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["C_NAME"]);
                            masterData.B_NAME = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]);
                            masterData.B_TERMS = reader["B_TERMS"] == DBNull.Value ? "" : Convert.ToString(reader["B_TERMS"]);
                            masterData.COMPANY_ADDRESS = reader["B_ADDRESS"] == DBNull.Value ? "" : Convert.ToString(reader["B_ADDRESS"]);
                            masterData.COMPANY_PHONE = reader["B_TEL"] == DBNull.Value ? "" : Convert.ToString(reader["B_TEL"]);
                            masterData.B_WEBSITE = reader["B_WEBSITE"] == DBNull.Value ? "" : Convert.ToString(reader["B_WEBSITE"]);
                            masterData.EMAIL = reader["EMAIL"] == DBNull.Value ? "" : Convert.ToString(reader["EMAIL"]);
                            masterData.B_GST = reader["B_GST"] == DBNull.Value ? "" : Convert.ToString(reader["B_GST"]);
                            masterData.B_NTN = reader["B_NTN"] == DBNull.Value ? "" : Convert.ToString(reader["B_NTN"]);
                            masterData.SIG1 = reader["MENU_SIG1"] == DBNull.Value ? "" : Convert.ToString(reader["MENU_SIG1"]);
                            masterData.SIG2 = reader["MENU_SIG2"] == DBNull.Value ? "" : Convert.ToString(reader["MENU_SIG2"]); ;
                            masterData.SIG3 = reader["MENU_SIG3"] == DBNull.Value ? "" : Convert.ToString(reader["MENU_SIG3"]); ;
                            masterData.SIG4 = reader["MENU_SIG4"] == DBNull.Value ? "" : Convert.ToString(reader["MENU_SIG4"]); ;
                            masterData.MENU_TERMS = reader["MENU_TERMS"] == DBNull.Value ? "" : Convert.ToString(reader["MENU_TERMS"]); ;
                            
                            masterData.INVOICE_NUMBER = Convert.ToString(reader["VOUCHER_NO"]);
                            masterData.DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                            masterData.USER = reader["USER_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["USER_NAME"]);
                            masterData.STATUS = Convert.ToString(reader["ASTATUS"]);
                            masterData.PARTY_NAME = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]);
                            //masterData.ORDER_TYPE = reader["ORDER_TYPE"] == DBNull.Value ? "" : Convert.ToString(reader["ORDER_TYPE"]);
                            //masterData.TERMS = reader["TERMS"] == DBNull.Value ? "" : Convert.ToString(reader["TERMS"]);
                            masterData.REF = reader["REF"] == DBNull.Value ? "" : Convert.ToString(reader["REF"]);
                            masterData.COMMENT = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]);
                            //masterData.DEL_DATE = reader["DEL_DATE"] == DBNull.Value ? "" : Convert.ToString(reader["DEL_DATE"]);

                            //masterData.DC_TYPE = dCType;
                        }
                        reader.Close();
                    }
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            DataRow dataRow = reportDetailsDDJ.NewRow();
                            dataRow["Item"] = Convert.ToString(reader["ITEM"]);
                            //dataRow["Qty"] = Convert.ToString(reader["BAL_QTY"]);
                            dataRow["Unit"] = Convert.ToString(reader["UNIT"]);
                            dataRow["Rate"] = Convert.ToString(reader["RATE"]);
                            dataRow["Amt"] = Convert.ToString(reader["AMT"]);
                            dataRow["Disc"] = Convert.ToString(reader["DISC"]);
                            dataRow["DiscAmt"] = Convert.ToString(reader["DISC_AMT"]);
                            dataRow["Tax"] = Convert.ToString(reader["TAX"]);
                            dataRow["TaxAmt"] = Convert.ToString(reader["TAX_AMT"]);
                            dataRow["Adv"] = Convert.ToString(reader["ADV"]);
                            dataRow["AdvAmt"] = Convert.ToString(reader["ADV_AMT"]);
                            dataRow["NetAmt"] = Convert.ToString(reader["NET_AMT"]);
                            dataRow["Grade"] = Convert.ToString(reader["GRADE"]);
                            dataRow["DeliveryDate"] = Convert.ToDateTime(reader["DEL_DATE"]);
                            reportDetailsDDJ.Rows.Add(dataRow);
                        }
                        reader.Close();
                    }
                }
                else if (menuDetails.REPORT_NAME == "DeliveryChallan")
                {
                    using (SqlConnection connection7 = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand sqlCommand7 = new SqlCommand(query, connection7);
                        connection7.Open();
                        SqlDataReader reader5 = sqlCommand7.ExecuteReader();
                        masterData.HEADER_NAME = (menuDetails.MD_NAME ?? "");
                        masterData.REPORT_NAME = (menuDetails.REPORT_NAME ?? "");
                        masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                        masterData.RCOMPANY_LOGO = currentCompany.RC_LOGO;
                        if (reader5.Read())
                        {
                            masterData.COMPANY_NAME = ((reader5["C_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["C_NAME"]));
                            masterData.B_NAME = ((reader5["B_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_NAME"]));
                            masterData.B_TERMS = ((reader5["B_TERMS"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_TERMS"]));
                            masterData.COMPANY_ADDRESS = ((reader5["B_ADDRESS"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_ADDRESS"]));
                            masterData.COMPANY_PHONE = ((reader5["B_TEL"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_TEL"]));
                            masterData.B_WEBSITE = ((reader5["B_WEBSITE"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_WEBSITE"]));
                            masterData.EMAIL = ((reader5["EMAIL"] == DBNull.Value) ? "" : Convert.ToString(reader5["EMAIL"]));
                            masterData.B_GST = ((reader5["B_GST"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_GST"]));
                            masterData.B_NTN = ((reader5["B_NTN"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_NTN"]));
                            masterData.SIG1 = ((reader5["MENU_SIG1"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG1"]));
                            masterData.SIG2 = ((reader5["MENU_SIG2"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG2"]));
                            masterData.SIG3 = ((reader5["MENU_SIG3"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG3"]));
                            masterData.SIG4 = ((reader5["MENU_SIG4"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG4"]));
                            masterData.MENU_TERMS = ((reader5["MENU_TERMS"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_TERMS"]));
                            masterData.INVOICE_NUMBER = Convert.ToString(reader5["VOUCHER_NO"]);
                            masterData.DATE = ((reader5["V_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader5["V_DATE"]).ToString("dd-MM-yyyy"));
                            masterData.USER = common.Username;
                            masterData.STATUS = Convert.ToString(reader5["ASTATUS"]);
                            masterData.BT_CUSTOMER = ((reader5["BT_CUSTOMER"] == DBNull.Value) ? "" : Convert.ToString(reader5["BT_CUSTOMER"]));
                            masterData.PARTY_NAME = ((reader5["PARTY_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["PARTY_NAME"]));
                            masterData.PADDRESS = ((reader5["PADDRESS"] == DBNull.Value) ? "" : Convert.ToString(reader5["PADDRESS"]));
                            masterData.REF = ((reader5["REF"] == DBNull.Value) ? "" : Convert.ToString(reader5["REF"]));
                            masterData.COMMENT = ((reader5["REMARKS"] == DBNull.Value) ? "" : Convert.ToString(reader5["REMARKS"]));
                            masterData.PNTN = ((reader5["PNTN"] == DBNull.Value) ? "" : Convert.ToString(reader5["PNTN"]));
                            //masterData.RINV_NO = ((reader5["RINV_NO"] == DBNull.Value) ? "" : Convert.ToString(reader5["RINV_NO"]));
                        }
                        reader5.Close();
                    }
                    using (SqlConnection connection8 = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand sqlCommand8 = new SqlCommand(query, connection8);
                        connection8.Open();
                        SqlDataReader reader6 = sqlCommand8.ExecuteReader();
                        while (reader6.Read())
                        {
                            DataRow dataRow3 = reportDetailsDDJ.NewRow();
                            dataRow3["Item"] = reader6["ITEM"]?.ToString() ?? "";
                            dataRow3["Barcode"] = reader6["ITEM_ID"]?.ToString() ?? "";
                            dataRow3["Qty"] = reader6["QTY"]?.ToString() ?? "";
                            dataRow3["Pack"] = reader6["PACK"]?.ToString() ?? "";
                            dataRow3["TotalPack"] = reader6["TOTAL_PACK"]?.ToString() ?? "";
                            dataRow3["Unit"] = reader6["UNIT"]?.ToString() ?? "";
                            dataRow3["Weight"] = reader6["WEIGHT"]?.ToString() ?? "";
                            dataRow3["Rate"] = reader6["RATE"]?.ToString() ?? "";
                            dataRow3["Amt"] = reader6["AMT"]?.ToString() ?? "";
                            dataRow3["Disc"] = reader6["DISC"]?.ToString() ?? "";
                            dataRow3["DiscAmt"] = reader6["DISC_AMT"]?.ToString() ?? "";
                            dataRow3["Tax"] = reader6["TAX"]?.ToString() ?? "";
                            dataRow3["TaxAmt"] = reader6["TAX_AMT"]?.ToString() ?? "";
                            dataRow3["Adv"] = reader6["ADV"]?.ToString() ?? "";
                            dataRow3["AdvAmt"] = reader6["ADV_AMT"]?.ToString() ?? "";
                            dataRow3["NetAmt"] = reader6["NET_AMT"]?.ToString() ?? "";
                            dataRow3["Grade"] = reader6["COLOR"]?.ToString() ?? "";
                            dataRow3["ExStax"] = reader6["EX_STAX"]?.ToString() ?? "";
                            reportDetailsDDJ.Rows.Add(dataRow3);
                        }
                        reader6.Close();
                    }
                }
                else if (menuDetails.REPORT_NAME == "SaleOrder")
                {
                    using (SqlConnection connection7 = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand sqlCommand7 = new SqlCommand(query, connection7);
                        connection7.Open();
                        SqlDataReader reader5 = sqlCommand7.ExecuteReader();
                        masterData.HEADER_NAME = (menuDetails.MD_NAME ?? "");
                        masterData.REPORT_NAME = (menuDetails.REPORT_NAME ?? "");
                        masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                        masterData.RCOMPANY_LOGO = currentCompany.RC_LOGO;
                        if (reader5.Read())
                        {
                            masterData.COMPANY_NAME = ((reader5["C_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["C_NAME"]));
                            masterData.B_NAME = ((reader5["B_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_NAME"]));
                            masterData.B_TERMS = ((reader5["B_TERMS"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_TERMS"]));
                            masterData.COMPANY_ADDRESS = ((reader5["B_ADDRESS"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_ADDRESS"]));
                            masterData.COMPANY_PHONE = ((reader5["B_TEL"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_TEL"]));
                            masterData.B_WEBSITE = ((reader5["B_WEBSITE"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_WEBSITE"]));
                            masterData.EMAIL = ((reader5["EMAIL"] == DBNull.Value) ? "" : Convert.ToString(reader5["EMAIL"]));
                            masterData.B_GST = ((reader5["B_GST"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_GST"]));
                            masterData.B_NTN = ((reader5["B_NTN"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_NTN"]));
                            masterData.SIG1 = ((reader5["MENU_SIG1"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG1"]));
                            masterData.SIG2 = ((reader5["MENU_SIG2"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG2"]));
                            masterData.SIG3 = ((reader5["MENU_SIG3"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG3"]));
                            masterData.SIG4 = ((reader5["MENU_SIG4"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG4"]));
                            masterData.CARTAGE = ((reader5["CARTAGE"] == DBNull.Value) ? "" : Convert.ToString(reader5["CARTAGE"]));
                            masterData.MENU_TERMS = ((reader5["MENU_TERMS"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_TERMS"]));
                            masterData.INVOICE_NUMBER = Convert.ToString(reader5["VOUCHER_NO"]);
                            masterData.DATE = ((reader5["V_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader5["V_DATE"]).ToString("dd-MM-yyyy"));
                            masterData.USER = common.Username;
                            masterData.STATUS = Convert.ToString(reader5["ASTATUS"]);
                            masterData.BT_CUSTOMER = ((reader5["BT_CUSTOMER"] == DBNull.Value) ? "" : Convert.ToString(reader5["BT_CUSTOMER"]));
                            masterData.PARTY_NAME = ((reader5["PARTY_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["PARTY_NAME"]));
                            masterData.PADDRESS = ((reader5["PADDRESS"] == DBNull.Value) ? "" : Convert.ToString(reader5["PADDRESS"]));
                            masterData.REF = ((reader5["REF"] == DBNull.Value) ? "" : Convert.ToString(reader5["REF"]));
                            masterData.COMMENT = ((reader5["REMARKS"] == DBNull.Value) ? "" : Convert.ToString(reader5["REMARKS"]));
                            masterData.PNTN = ((reader5["PNTN"] == DBNull.Value) ? "" : Convert.ToString(reader5["PNTN"]));
                            masterData.DISC = reader5["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader5["DISC"]);

                            //masterData.RINV_NO = ((reader5["RINV_NO"] == DBNull.Value) ? "" : Convert.ToString(reader5["RINV_NO"]));
                        }
                        reader5.Close();
                    }
                    using (SqlConnection connection8 = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand sqlCommand8 = new SqlCommand(query, connection8);
                        connection8.Open();
                        SqlDataReader reader6 = sqlCommand8.ExecuteReader();
                        while (reader6.Read())
                        {
                            DataRow dataRow3 = reportDetailsDDJ.NewRow();
                            dataRow3["Item"] = reader6["ITEM"]?.ToString() ?? "";
                            dataRow3["Barcode"] = reader6["ITEM_ID"]?.ToString() ?? "";
                            dataRow3["Qty"] = reader6["QTY"]?.ToString() ?? "";
                            dataRow3["Pack"] = reader6["PACK"]?.ToString() ?? "";
                            dataRow3["TotalPack"] = reader6["TOTAL_PACK"]?.ToString() ?? "";
                            //dataRow3["Barcode"] = reader6["BARCODE"]?.ToString() ?? "";
                            dataRow3["Unit"] = reader6["UNIT"]?.ToString() ?? "";
                            dataRow3["Grade"] = reader6["COLOR"]?.ToString() ?? "";
                            dataRow3["Weight"] = reader6["WEIGHT"]?.ToString() ?? "";
                            dataRow3["Rate"] = reader6["RATE"]?.ToString() ?? "";
                            dataRow3["Amt"] = reader6["AMT"]?.ToString() ?? "";
                            dataRow3["Disc"] = reader6["DISC"]?.ToString() ?? "";
                            dataRow3["DiscAmt"] = reader6["DISC_AMT"]?.ToString() ?? "";
                            dataRow3["Tax"] = reader6["TAX"]?.ToString() ?? "";
                            dataRow3["TaxAmt"] = reader6["TAX_AMT"]?.ToString() ?? "";
                            dataRow3["Adv"] = reader6["ADV"]?.ToString() ?? "";
                            dataRow3["AdvAmt"] = reader6["ADV_AMT"]?.ToString() ?? "";
                            dataRow3["NetAmt"] = reader6["NET_AMT"]?.ToString() ?? "";
                            //dataRow3["Grade"] = reader6["GRADE"]?.ToString() ?? "";
                            dataRow3["ExStax"] = reader6["EX_STAX"]?.ToString() ?? "";
                            reportDetailsDDJ.Rows.Add(dataRow3);
                        }
                        reader6.Close();
                    }
                }
                else if (menuDetails.REPORT_NAME == "SalesInvoice" || menuDetails.REPORT_NAME == "SalesReturn")
                {
                    using (SqlConnection connection7 = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand sqlCommand7 = new SqlCommand(query, connection7);
                        connection7.Open();
                        SqlDataReader reader5 = sqlCommand7.ExecuteReader();
                        masterData.HEADER_NAME = (menuDetails.MD_NAME ?? "");
                        masterData.REPORT_NAME = (menuDetails.REPORT_NAME ?? "");
                        masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                        masterData.RCOMPANY_LOGO = currentCompany.RC_LOGO;
                        if (reader5.Read())
                        {
                            masterData.COMPANY_NAME = ((reader5["C_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["C_NAME"]));
                            masterData.B_NAME = ((reader5["B_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_NAME"]));
                            masterData.B_TERMS = ((reader5["B_TERMS"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_TERMS"]));
                            masterData.COMPANY_ADDRESS = ((reader5["B_ADDRESS"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_ADDRESS"]));
                            masterData.COMPANY_PHONE = ((reader5["B_TEL"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_TEL"]));
                            masterData.B_WEBSITE = ((reader5["B_WEBSITE"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_WEBSITE"]));
                            masterData.EMAIL = ((reader5["EMAIL"] == DBNull.Value) ? "" : Convert.ToString(reader5["EMAIL"]));
                            masterData.B_GST = ((reader5["B_GST"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_GST"]));
                            masterData.B_NTN = ((reader5["B_NTN"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_NTN"]));
                            masterData.SIG1 = ((reader5["MENU_SIG1"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG1"]));
                            masterData.SIG2 = ((reader5["MENU_SIG2"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG2"]));
                            masterData.SIG3 = ((reader5["MENU_SIG3"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG3"]));
                            masterData.SIG4 = ((reader5["MENU_SIG4"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG4"]));
                            masterData.CARTAGE = ((reader5["CARTAGE"] == DBNull.Value) ? "" : Convert.ToString(reader5["CARTAGE"]));
                            masterData.MENU_TERMS = ((reader5["MENU_TERMS"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_TERMS"]));
                            masterData.INVOICE_NUMBER = Convert.ToString(reader5["VOUCHER_NO"]);
                            masterData.DATE = ((reader5["V_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader5["V_DATE"]).ToString("dd-MM-yyyy"));
                            masterData.USER = common.Username;
                            masterData.STATUS = Convert.ToString(reader5["ASTATUS"]);
                            masterData.BT_CUSTOMER = ((reader5["BT_CUSTOMER"] == DBNull.Value) ? "" : Convert.ToString(reader5["BT_CUSTOMER"]));
                            masterData.PARTY_NAME = ((reader5["PARTY_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["PARTY_NAME"]));
                            masterData.PADDRESS = ((reader5["PADDRESS"] == DBNull.Value) ? "" : Convert.ToString(reader5["PADDRESS"]));
                            masterData.REF = ((reader5["REF"] == DBNull.Value) ? "" : Convert.ToString(reader5["REF"]));
                            masterData.COMMENT = ((reader5["REMARKS"] == DBNull.Value) ? "" : Convert.ToString(reader5["REMARKS"]));
                            masterData.PNTN = ((reader5["PNTN"] == DBNull.Value) ? "" : Convert.ToString(reader5["PNTN"]));
                            masterData.DISC = reader5["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader5["DISC"]);

                            //masterData.RINV_NO = ((reader5["RINV_NO"] == DBNull.Value) ? "" : Convert.ToString(reader5["RINV_NO"]));
                        }
                        reader5.Close();
                    }
                    using (SqlConnection connection8 = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand sqlCommand8 = new SqlCommand(query, connection8);
                        connection8.Open();
                        SqlDataReader reader6 = sqlCommand8.ExecuteReader();
                        while (reader6.Read())
                        {
                            DataRow dataRow3 = reportDetailsDDJ.NewRow();
                            dataRow3["Item"] = reader6["ITEM"]?.ToString() ?? "";
                            dataRow3["Barcode"] = reader6["ITEM_ID"]?.ToString() ?? "";
                            dataRow3["Qty"] = reader6["QTY"]?.ToString() ?? "";
                            dataRow3["Pack"] = reader6["PACK"]?.ToString() ?? "";
                            dataRow3["TotalPack"] = reader6["TOTAL_PACK"]?.ToString() ?? "";
                            //dataRow3["Barcode"] = reader6["BARCODE"]?.ToString() ?? "";
                            dataRow3["Unit"] = reader6["UNIT"]?.ToString() ?? "";
                            dataRow3["Grade"] = reader6["COLOR"]?.ToString() ?? "";
                            dataRow3["Weight"] = reader6["WEIGHT"]?.ToString() ?? "";
                            dataRow3["Rate"] = reader6["RATE"]?.ToString() ?? "";
                            dataRow3["Amt"] = reader6["AMT"]?.ToString() ?? "";
                            dataRow3["Disc"] = reader6["DISC"]?.ToString() ?? "";
                            dataRow3["DiscAmt"] = reader6["DISC_AMT"]?.ToString() ?? "";
                            dataRow3["Tax"] = reader6["TAX"]?.ToString() ?? "";
                            dataRow3["TaxAmt"] = reader6["TAX_AMT"]?.ToString() ?? "";
                            dataRow3["Adv"] = reader6["ADV"]?.ToString() ?? "";
                            dataRow3["AdvAmt"] = reader6["ADV_AMT"]?.ToString() ?? "";
                            dataRow3["NetAmt"] = reader6["NET_AMT"]?.ToString() ?? "";
                            //dataRow3["Grade"] = reader6["GRADE"]?.ToString() ?? "";
                            dataRow3["ExStax"] = reader6["EX_STAX"]?.ToString() ?? "";
                            reportDetailsDDJ.Rows.Add(dataRow3);
                        }
                        reader6.Close();
                    }
                }


                reportData.Master = masterData;
                
                if (menuDetails.REPORT_NAME == "SaleOrder" || menuDetails.REPORT_NAME == "DeliveryChallan" || menuDetails.REPORT_NAME == "SalesInvoice" || menuDetails.REPORT_NAME == "SalesReturn")
                {
                    reportData.Detail = reportDetailsDDJ;
                }
                else if (menuDetails.REPORT_NAME == "")
                {
                    reportData.Detail = reportDetailsDDJ;
                }
                //=FormatNumber(FormatNumber(Fields!Rate.Value * (1 - Fields!Unit.Value / 100), 2) * Fields!Qty.Value, 0)
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

        public DataTable GetComm(int Salesman)
        {
            DataTable dt = new DataTable();
            string getComm = $"Select * from TBL_SCOMM_LIST where SALESMAN = {Salesman} ANd DLT = 'T'";

            using (SqlConnection connectionNew = new SqlConnection(new SQLService().getconnstring()))
            {
                using (SqlCommand commandNew1 = new SqlCommand(getComm, connectionNew))
                {
                    using (SqlDataAdapter da = new SqlDataAdapter(commandNew1))
                    {
                        connectionNew.Open();
                        da.Fill(dt);
                    }
                }
            }
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    Console.WriteLine($"{col.ColumnName}: {row[col]}");
                }
            }
            return dt;
        }
    }
}