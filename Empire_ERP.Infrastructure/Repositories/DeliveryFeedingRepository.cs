using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using SkiaSharp;
using System.Data;
using System.Text.Json.Nodes;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class DeliveryFeedingRepository : IDeliveryFeedingRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public ICommonService _commonService { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public IPartyRepository _partyRepository { get; set; }
        public DeliveryFeedingRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, IPartyRepository partyRepository, ICommonService commonService, ICommonRepository commonRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _partyRepository = partyRepository;
            _commonService = commonService;
            _commonRepository = commonRepository;
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
                        string query = "SELECT A.TRAN_ID AS CODE, SBFM.TRAN_ID AS PTRAN_ID, SBFM.VOUCHER_NO AS SODA_VOUCHER,D.DT_CODE AS TRAN_ID, A.BGOD_CHARGES, A.SGOD_CHARGES, A.BLABOUR, A.SLABOUR, A.BFRIEGHT, A.SFRIEGHT, A.BFUMIGATION, A.SFUMIGATION, A.SBARDANA, A.BARDANA, A.BR_AMOUNT_BUYER, A.BR_AMOUNT_SELLER, A.WT_AMOUNT_SELLER, A.WT_AMOUNT_BUYER, A.S_NET_AMOUNT, A.B_NET_AMOUNT, A.V_DATE, A.VOUCHER_NO, A.DUE_DATE, " +
                                  "S.PARTY_NAME AS SELLER_CODE, A.SACT_CODE, B.PARTY_NAME AS BUYER_CODE, A.BACT_CODE," +
                                  "A.REF, A.REMARKS, CASE WHEN A.COND = 'Adv' THEN 'Advance' WHEN A.COND = 'Cash' THEN 'Cash'" +
                                  "WHEN A.COND = 'Cr' THEN 'Credit Days' WHEN A.COND = 'CrD' THEN 'Credit Date' ELSE '' END AS COND,BR.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE," +
                                  "A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS," +
                                  "A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS," +
                                  "A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, A.SDATE, A.CREDIT_DAYS, " +
                                  "IM.ITEM_NAME AS ITEM_CODE, D.QTY, U.GROUP_NAME AS UNIT, D.QTY2, D.BAL_QTY, D.RATE, " +
                                  "RTRIM(LTRIM(D.RT_TYPE)) + ' kg' as RT_TYPE, D.AMT, D.DT_DESC, MB.ID AS MENU_ID, MB.MENU_PAGE, MB.MENU_PARENT_CODE " +
                                  $"FROM {table} A " +
                                  "LEFT OUTER JOIN TBL_PARTY_TYPES S ON A.SELLER_CODE = S.PARTY_CODE AND S.ACT_CODE = A.SACT_CODE " +
                                  "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BUYER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BACT_CODE " +
                                  "LEFT OUTER JOIN TBL_PARTY_TYPES BR ON A.BROKER_CODE = BR.PARTY_CODE AND BR.ACT_CODE = A.BD_ACT_CODE " +
                                  $"LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID AND A.PERIOD_ID = D.PERIOD_ID AND A.BCODE = D.BCODE " +
                                  $"LEFT OUTER JOIN TBL_SBF_DETAIL SBFD ON D.PICK_ID = SBFD.DT_CODE AND SBFD.PERIOD_ID = D.PERIOD_ID AND SBFD.BCODE = D.BCODE " +
                                  $"LEFT OUTER JOIN TBL_SBF_MASTER SBFM ON SBFD.TRAN_ID = SBFM.TRAN_ID AND SBFD.PERIOD_ID = SBFM.PERIOD_ID AND SBFD.BCODE = SBFM.BCODE " +
                                  "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON D.ITEM_CODE = IM.ITEM_CODE " +
                                  "LEFT OUTER JOIN TBL_UNIT U ON D.UNIT = U.GROUP_CODE " +
                                  "LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = SBFM.MENU_ID " +
                                  $"WHERE A.DLT = 'T' AND D.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' " +
                                  "ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC ";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                PTRAN_ID = reader["PTRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PTRAN_ID"]),
                                CODE = Convert.ToInt32(reader["CODE"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                SODA_VOUCHER = Convert.ToString(reader["SODA_VOUCHER"]),
                                DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),
                                SELLER_CODE = Convert.ToString(reader["SELLER_CODE"]),
                                SACT_CODE = Convert.ToString(reader["SACT_CODE"]),
                                BUYER_CODE = Convert.ToString(reader["BUYER_CODE"]),
                                BACT_CODE = Convert.ToString(reader["BACT_CODE"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                COND = Convert.ToString(reader["COND"]),
                                BROKER_CODE = Convert.ToString(reader["BROKER_CODE"]),
                                BD_ACT_CODE = Convert.ToString(reader["BD_ACT_CODE"]),
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
                                SDATE = reader["SDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SDATE"]).ToString("yyyy-MM-dd"),
                                CREDIT_DAYS = Convert.ToString(reader["CREDIT_DAYS"]),
                                //DETAIL = GetDeliveryDetails(Convert.ToString(reader["TRAN_ID"]), Menu),
                                ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                BR_AMOUNT_BUYER = Convert.ToString(reader["BR_AMOUNT_BUYER"]),
                                BR_AMOUNT_SELLER = Convert.ToString(reader["BR_AMOUNT_SELLER"]),
                                WT_AMOUNT_SELLER = Convert.ToString(reader["WT_AMOUNT_SELLER"]),
                                WT_AMOUNT_BUYER = Convert.ToString(reader["WT_AMOUNT_BUYER"]),
                                S_NET_AMOUNT = Convert.ToString(reader["S_NET_AMOUNT"]),
                                B_NET_AMOUNT = Convert.ToString(reader["B_NET_AMOUNT"]),
                                BARDANA = Convert.ToString(reader["BARDANA"]),
                                SBARDANA = Convert.ToString(reader["SBARDANA"]),
                                BGOD_CHARGES = Convert.ToString(reader["BGOD_CHARGES"]),
                                SGOD_CHARGES = Convert.ToString(reader["SGOD_CHARGES"]),
                                BLABOUR = Convert.ToString(reader["BLABOUR"]),
                                SLABOUR = Convert.ToString(reader["SLABOUR"]),
                                BFRIEGHT = Convert.ToString(reader["BFRIEGHT"]),
                                SFRIEGHT = Convert.ToString(reader["SFRIEGHT"]),
                                BFUMIGATION = Convert.ToString(reader["BFUMIGATION"]),
                                SFUMIGATION = Convert.ToString(reader["SFUMIGATION"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
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

        //public MyHttpResponseMessage QuickSearchLazyLoad(Common common, int skip, int take, string filter = null, string group = null)
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
        //                    if (jsonArray.Count > 30)
        //                    {
        //                        jsonArray.RemoveAt(1);
        //                        jsonArray.RemoveAt(0);
        //                    }
        //                    filterCondition = _commonRepository.BuildFilterCondition(jsonArray);
        //                    filterCondition = filterCondition
        //                        .Replace("traN_ID", "A.TRAN_ID")
        //                        .Replace("astatus", "A.ASTATUS")
        //                        .Replace("sacT_CODE", "A.SACT_CODE")
        //                        .Replace("remarks", "A.REMARKS")
        //                        .Replace("iteM_CODE", "IM.ITEM_NAME")
        //                        .Replace("qty", "D.QTY");
        //                }


        //                string query = $"SELECT COUNT(*) FROM {table} A " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES S ON A.SELLER_CODE = S.PARTY_CODE AND S.ACT_CODE = A.SACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BUYER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES BR ON A.BROKER_CODE = BR.PARTY_CODE AND BR.ACT_CODE = A.BD_ACT_CODE " +
        //                               $"LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID " +
        //                               "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON D.ITEM_CODE = IM.ITEM_CODE " +
        //                               "LEFT OUTER JOIN TBL_UNIT U ON D.UNIT = U.GROUP_CODE " +
        //                               $"WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' {filterCondition}";

        //                SqlCommand countCommand = new SqlCommand(query, connection);
        //                connection.Open();
        //                totalCount = (int)countCommand.ExecuteScalar();
        //                connection.Close();

        //                if (string.IsNullOrEmpty(group))
        //                {
        //                    query = "SELECT A.TRAN_ID, A.BARDANA, A.BR_AMOUNT_BUYER, A.BR_AMOUNT_SELLER, A.WT_AMOUNT_SELLER, A.WT_AMOUNT_BUYER, A.S_NET_AMOUNT, A.B_NET_AMOUNT, A.V_DATE, A.VOUCHER_NO, A.DUE_DATE, " +
        //                          "S.PARTY_NAME AS SELLER_CODE, A.SACT_CODE, B.PARTY_NAME AS BUYER_CODE, A.BACT_CODE," +
        //                          "A.REF, A.REMARKS, CASE WHEN A.COND = 'Adv' THEN 'Advance' WHEN A.COND = 'Cash' THEN 'Cash'" +
        //                          "WHEN A.COND = 'Cr' THEN 'Credit Days' WHEN A.COND = 'CrD' THEN 'Credit Date' ELSE '' END AS COND,BR.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE," +
        //                          "A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS," +
        //                          "A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS," +
        //                          "A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, A.SDATE, A.CREDIT_DAYS, " +
        //                          "IM.ITEM_NAME AS ITEM_CODE, D.QTY, U.GROUP_NAME AS UNIT, D.QTY2, D.BAL_QTY, D.RATE, " +
        //                          "RTRIM(LTRIM(D.RT_TYPE)) + ' kg' as RT_TYPE, D.AMT, D.DT_DESC " +
        //                          $"FROM {table} A " +
        //                          "LEFT OUTER JOIN TBL_PARTY_TYPES S ON A.SELLER_CODE = S.PARTY_CODE AND S.ACT_CODE = A.SACT_CODE " +
        //                          "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BUYER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BACT_CODE " +
        //                          "LEFT OUTER JOIN TBL_PARTY_TYPES BR ON A.BROKER_CODE = BR.PARTY_CODE AND BR.ACT_CODE = A.BD_ACT_CODE " +
        //                          $"LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID AND A.PERIOD_ID = D.PERIOD_ID AND A.BCODE = D.BCODE " +
        //                          "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON D.ITEM_CODE = IM.ITEM_CODE " +
        //                          "LEFT OUTER JOIN TBL_UNIT U ON D.UNIT = U.GROUP_CODE " +
        //                          $"WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' {filterCondition}" +
        //                          "ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC " +
        //                          $"OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        //                }
        //                else
        //                {
        //                    query = "SELECT A.TRAN_ID, A.BARDANA, A.BR_AMOUNT_BUYER, A.BR_AMOUNT_SELLER, A.WT_AMOUNT_SELLER, A.WT_AMOUNT_BUYER, A.S_NET_AMOUNT, A.B_NET_AMOUNT, A.V_DATE, A.VOUCHER_NO, A.DUE_DATE, " +
        //                          "S.PARTY_NAME AS SELLER_CODE, A.SACT_CODE, B.PARTY_NAME AS BUYER_CODE, A.BACT_CODE," +
        //                          "A.REF, A.REMARKS, CASE WHEN A.COND = 'Adv' THEN 'Advance' WHEN A.COND = 'Cash' THEN 'Cash'" +
        //                          "WHEN A.COND = 'Cr' THEN 'Credit Days' WHEN A.COND = 'CrD' THEN 'Credit Date' ELSE '' END AS COND,BR.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE," +
        //                          "A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS," +
        //                          "A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS," +
        //                          "A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, A.SDATE, A.CREDIT_DAYS, " +
        //                          "IM.ITEM_NAME AS ITEM_CODE, D.QTY, U.GROUP_NAME AS UNIT, D.QTY2, D.BAL_QTY, D.RATE, " +
        //                          "RTRIM(LTRIM(D.RT_TYPE)) + ' kg' as RT_TYPE, D.AMT, D.DT_DESC " +
        //                          $"FROM {table} A " +
        //                          "LEFT OUTER JOIN TBL_PARTY_TYPES S ON A.SELLER_CODE = S.PARTY_CODE AND S.ACT_CODE = A.SACT_CODE " +
        //                          "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BUYER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BACT_CODE " +
        //                          "LEFT OUTER JOIN TBL_PARTY_TYPES BR ON A.BROKER_CODE = BR.PARTY_CODE AND BR.ACT_CODE = A.BD_ACT_CODE " +
        //                          $"LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID AND A.PERIOD_ID = D.PERIOD_ID AND A.BCODE = D.BCODE " +
        //                          "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON D.ITEM_CODE = IM.ITEM_CODE " +
        //                          "LEFT OUTER JOIN TBL_UNIT U ON D.UNIT = U.GROUP_CODE " +
        //                          $"WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' {filterCondition}" +
        //                          "ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC";
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
        //                        DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),
        //                        SELLER_CODE = Convert.ToString(reader["SELLER_CODE"]),
        //                        SACT_CODE = Convert.ToString(reader["SACT_CODE"]),
        //                        BUYER_CODE = Convert.ToString(reader["BUYER_CODE"]),
        //                        BACT_CODE = Convert.ToString(reader["BACT_CODE"]),
        //                        REF = Convert.ToString(reader["REF"]),
        //                        REMARKS = Convert.ToString(reader["REMARKS"]),
        //                        COND = Convert.ToString(reader["COND"]),
        //                        BROKER_CODE = Convert.ToString(reader["BROKER_CODE"]),
        //                        BD_ACT_CODE = Convert.ToString(reader["BD_ACT_CODE"]),
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
        //                        SDATE = reader["SDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SDATE"]).ToString("yyyy-MM-dd"),
        //                        CREDIT_DAYS = Convert.ToString(reader["CREDIT_DAYS"]),
        //                        //DETAIL = GetDeliveryDetails(Convert.ToString(reader["TRAN_ID"]), Menu),
        //                        ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
        //                        QTY = Convert.ToString(reader["QTY"]),
        //                        UNIT = Convert.ToString(reader["UNIT"]),
        //                        QTY2 = Convert.ToString(reader["QTY2"]),
        //                        BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
        //                        RATE = Convert.ToString(reader["RATE"]),
        //                        RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
        //                        AMT = Convert.ToString(reader["AMT"]),
        //                        BR_AMOUNT_BUYER = Convert.ToString(reader["BR_AMOUNT_BUYER"]),
        //                        BR_AMOUNT_SELLER = Convert.ToString(reader["BR_AMOUNT_SELLER"]),
        //                        WT_AMOUNT_SELLER = Convert.ToString(reader["WT_AMOUNT_SELLER"]),
        //                        WT_AMOUNT_BUYER = Convert.ToString(reader["WT_AMOUNT_BUYER"]),
        //                        S_NET_AMOUNT = Convert.ToString(reader["S_NET_AMOUNT"]),
        //                        B_NET_AMOUNT = Convert.ToString(reader["B_NET_AMOUNT"]),
        //                        BARDANA = Convert.ToString(reader["BARDANA"]),
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

        public MyHttpResponseMessage Save(CustomDeliveryFeeding modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty, pickTable = string.Empty;
                int? pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickTable = menu.PICK_TABLE_DETAIL;
                    pType = menu.PTYPE;
                }
                List<CustomPartyType> partiesData = DropdownService.CustomPartyTypeDropdownWithAccountCodeAndPType(common.RoleID, common.RoleType, pType);

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable) && partiesData != null && partiesData.Count > 0)
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
                            var sellerInformation = partiesData.ToList().Where(p => p.customizedKey == modelRecord.Master.SELLER_CODE).FirstOrDefault();
                            var buyerInformation = partiesData.ToList().Where(p => p.customizedKey == modelRecord.Master.BUYER_CODE).FirstOrDefault();

                            if (sellerInformation != null)
                            {
                                modelRecord.Master.SELLER_CODE = Convert.ToString(sellerInformation.key);
                                modelRecord.Master.SACT_CODE = sellerInformation.accountCode;
                            }

                            if (buyerInformation != null)
                            {
                                modelRecord.Master.BUYER_CODE = Convert.ToString(buyerInformation.key);
                                modelRecord.Master.BACT_CODE = buyerInformation.accountCode;
                            }

                            if (modelRecord.Master.BROKER_CODE != null)
                            {
                                var brokerInformation = partiesData.Where(p => p.customizedKey == modelRecord.Master.BROKER_CODE).FirstOrDefault();
                                if (brokerInformation != null)
                                {
                                    modelRecord.Master.BROKER_CODE = Convert.ToString(brokerInformation.key);
                                    modelRecord.Master.BD_ACT_CODE = brokerInformation.accountCode;
                                }
                            }

                            if (modelRecord.Master.COB_CODE != null)
                            {
                                var brokerInformation = partiesData.Where(p => p.customizedKey == modelRecord.Master.COB_CODE).FirstOrDefault();
                                if (brokerInformation != null)
                                {
                                    modelRecord.Master.COB_CODE = Convert.ToString(brokerInformation.key);
                                    modelRecord.Master.COB_ACODE = brokerInformation.accountCode;
                                }
                            }

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
                                        "(TRAN_ID, V_DATE, D_NAME, D_NUM, VOUCHER_NO, SELLER_CODE, " +
                                        "SACT_CODE, BUYER_CODE, BACT_CODE, REF, " +
                                        "REMARKS, COND, BROKER_CODE, BD_ACT_CODE, " +
                                        "BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                        "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                        "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                        "ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, DLT, SDATE," +
                                        "DUE_DATE, CREDIT_DAYS, BR_AMOUNT_BUYER, BR_AMOUNT_SELLER, " +
                                        "BGOD_CHARGES, SGOD_CHARGES, BLABOUR, SLABOUR, BFRIEGHT, SFRIEGHT, BFUMIGATION, SFUMIGATION, WT_AMOUNT_SELLER, WT_AMOUNT_BUYER, B_NET_AMOUNT, S_NET_AMOUNT, BARDANA, SBARDANA, " +
                                        "BGOD_CHARGES_ST, SGOD_CHARGES_ST, BLABOUR_ST, SLABOUR_ST, BFRIEGHT_ST, SFRIEGHT_ST, BFUMIGATION_ST, SFUMIGATION_ST, WT_AMOUNT_SELLER_ST, WT_AMOUNT_BUYER_ST, BARDANA_ST, SBARDANA_ST, " +
                                        "BR_AMOUNT_BUYER_ST, BR_AMOUNT_SELLER_ST, COB_CODE, COB_ACODE, CURR_CODE, CRATE, SHIP_DATE, SHIP_STATUS, UNIT, SODA_TYPE, SBF_TYPE )" +
                                        "VALUES" +
                                        "('" + code + "','" + modelRecord.Master.V_DATE + "','" + modelRecord.Master.D_NAME + "','" + modelRecord.Master.D_NUM + "','" + voucherNo + "','" + modelRecord.Master.SELLER_CODE + "'," +
                                        "'" + modelRecord.Master.SACT_CODE + "','" + modelRecord.Master.BUYER_CODE + "','" + modelRecord.Master.BACT_CODE + "','" + modelRecord.Master.REF + "'," +
                                        "'" + modelRecord.Master.REMARKS + "','" + modelRecord.Master.COND + "','" + modelRecord.Master.BROKER_CODE + "','" + modelRecord.Master.BD_ACT_CODE + "'," +
                                        "'" + branch + "','" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                        "'" + computer + "','" + ip + "','" + username + "'," +
                                        "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + computer + "','" + ip + "'," +
                                        "'" + postal + "','" + postal + "','" + modelRecord.Master.ASTATUS + "','" + menuID + "','T','" + modelRecord.Master.SDATE + "'," +
                                        "'" + modelRecord.Master.DUE_DATE + "','" + modelRecord.Master.CREDIT_DAYS + "','" + modelRecord.Master.BR_AMOUNT_BUYER + "','" + modelRecord.Master.BR_AMOUNT_SELLER + "'," +
                                        "'" + modelRecord.Master.BGOD_CHARGES + "','" + modelRecord.Master.SGOD_CHARGES + "','" + modelRecord.Master.BLABOUR + "','" + modelRecord.Master.SLABOUR + "','" + modelRecord.Master.BFRIEGHT + "','" + modelRecord.Master.SFRIEGHT + "','" + modelRecord.Master.BFUMIGATION + "','" + modelRecord.Master.SFUMIGATION + "',"+
                                        "'" + modelRecord.Master.WT_AMOUNT_SELLER + "','" + modelRecord.Master.WT_AMOUNT_BUYER + "','" + modelRecord.Master.B_NET_AMOUNT + "','" + modelRecord.Master.S_NET_AMOUNT + "','" + modelRecord.Master.BARDANA + "','" + modelRecord.Master.SBARDANA + "',"+
                                        "'" + modelRecord.Master.BGOD_CHARGES_ST + "','" + modelRecord.Master.SGOD_CHARGES_ST + "','" + modelRecord.Master.BLABOUR_ST + "','" + modelRecord.Master.SLABOUR_ST + "','" + modelRecord.Master.BFRIEGHT_ST + "','" + modelRecord.Master.SFRIEGHT_ST + "','" + modelRecord.Master.BFUMIGATION_ST + "','" + modelRecord.Master.SFUMIGATION_ST + "',"+
                                        "'" + modelRecord.Master.WT_AMOUNT_SELLER_ST + "','" + modelRecord.Master.WT_AMOUNT_BUYER_ST + "','" + modelRecord.Master.BARDANA_ST + "','" + modelRecord.Master.SBARDANA_ST + "','" + modelRecord.Master.BR_AMOUNT_BUYER_ST + "','" + modelRecord.Master.BR_AMOUNT_SELLER_ST + "'," +
                                        "'" + modelRecord.Master.COB_CODE + "','" + modelRecord.Master.COB_ACODE + "','" + modelRecord.Master.CURR_CODE + "','" + modelRecord.Master.CRATE + "','" + modelRecord.Master.SHIP_DATE + "','" + modelRecord.Master.SHIP_STATUS + "','" + modelRecord.Master.UNIT + "','" + modelRecord.Master.SODA_TYPE + "','" + modelRecord.Master.SBF_TYPE + "')";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                query = $"UPDATE {table} SET " +
                                        $"V_DATE = '{modelRecord.Master.V_DATE}', " +
                                        $"D_NAME = '{modelRecord.Master.D_NAME}', " +
                                        $"D_NUM = '{modelRecord.Master.D_NUM}', " +
                                        $"SELLER_CODE = '{modelRecord.Master.SELLER_CODE}', " +
                                        $"SACT_CODE = '{modelRecord.Master.SACT_CODE}', " +
                                        $"BUYER_CODE = '{modelRecord.Master.BUYER_CODE}', " +
                                        $"BACT_CODE = '{modelRecord.Master.BACT_CODE}', " +
                                        $"REF = '{modelRecord.Master.REF}', " +
                                        $"REMARKS = '{modelRecord.Master.REMARKS}', " +
                                        $"COND = '{modelRecord.Master.COND}', " +
                                        $"BROKER_CODE = '{modelRecord.Master.BROKER_CODE}', " +
                                        $"BD_ACT_CODE = '{modelRecord.Master.BD_ACT_CODE}', " +
                                        $"EDIT_USER_ID = '{username}', " +
                                        $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                        $"EDIT_COMPUTER_NAME = '{computer}', " +
                                        $"EDIT_IP_ADDRESS = '{ip}', " +
                                        $"EDIT_POSTALCODE = '{postal}', " +
                                        $"ASTATUS = '{modelRecord.Master.ASTATUS}', " +
                                        $"SDATE = '{modelRecord.Master.SDATE}', " +
                                        $"DUE_DATE = '{modelRecord.Master.DUE_DATE}', " +
                                        $"CREDIT_DAYS = '{modelRecord.Master.CREDIT_DAYS}', " +
                                        $"BR_AMOUNT_BUYER_ST = '{modelRecord.Master.BR_AMOUNT_BUYER_ST}', " +
                                        $"BR_AMOUNT_SELLER_ST = '{modelRecord.Master.BR_AMOUNT_SELLER_ST}', " +
                                        $"WT_AMOUNT_BUYER_ST = '{modelRecord.Master.WT_AMOUNT_BUYER_ST}', " +
                                        $"WT_AMOUNT_SELLER_ST = '{modelRecord.Master.WT_AMOUNT_SELLER_ST}', " +
                                        $"BARDANA_ST = '{modelRecord.Master.BARDANA_ST}', " +
                                        $"SBARDANA_ST = '{modelRecord.Master.SBARDANA_ST}', " +
                                        $"BGOD_CHARGES_ST = '{modelRecord.Master.BGOD_CHARGES_ST}', " +
                                        $"SGOD_CHARGES_ST = '{modelRecord.Master.SGOD_CHARGES_ST}', " +
                                        $"BLABOUR_ST = '{modelRecord.Master.BLABOUR_ST}', " +
                                        $"SLABOUR_ST = '{modelRecord.Master.SLABOUR_ST}', " +
                                        $"BFRIEGHT_ST = '{modelRecord.Master.BFRIEGHT_ST}', " +
                                        $"SFRIEGHT_ST = '{modelRecord.Master.SFRIEGHT_ST}', " +
                                        $"BFUMIGATION_ST = '{modelRecord.Master.BFUMIGATION_ST}', " +
                                        $"SFUMIGATION_ST = '{modelRecord.Master.SFUMIGATION_ST}', " +
                                        $"BR_AMOUNT_BUYER = '{modelRecord.Master.BR_AMOUNT_BUYER}', " +
                                        $"BR_AMOUNT_SELLER = '{modelRecord.Master.BR_AMOUNT_SELLER}', " +
                                        $"WT_AMOUNT_SELLER = '{modelRecord.Master.WT_AMOUNT_SELLER}', " +
                                        $"WT_AMOUNT_BUYER = '{modelRecord.Master.WT_AMOUNT_BUYER}', " +
                                        $"B_NET_AMOUNT = '{modelRecord.Master.B_NET_AMOUNT}', " +
                                        $"BARDANA = '{modelRecord.Master.BARDANA}', " +
                                        $"SBARDANA = '{modelRecord.Master.SBARDANA}', " +
                                        $"BGOD_CHARGES = '{modelRecord.Master.BGOD_CHARGES}', " +
                                        $"SGOD_CHARGES = '{modelRecord.Master.SGOD_CHARGES}', " +
                                        $"BLABOUR = '{modelRecord.Master.BLABOUR}', " +
                                        $"SLABOUR = '{modelRecord.Master.SLABOUR}', " +
                                        $"BFRIEGHT = '{modelRecord.Master.BFRIEGHT}', " +
                                        $"SFRIEGHT = '{modelRecord.Master.SFRIEGHT}', " +
                                        $"BFUMIGATION = '{modelRecord.Master.BFUMIGATION}', " +
                                        $"SFUMIGATION = '{modelRecord.Master.SFUMIGATION}', " +
                                        $"S_NET_AMOUNT = '{modelRecord.Master.S_NET_AMOUNT}', " +
                                        $"COB_CODE = '{modelRecord.Master.COB_CODE}', " +
                                        $"COB_ACODE = '{modelRecord.Master.COB_ACODE}', " +
                                        $"CURR_CODE = '{modelRecord.Master.CURR_CODE}', " +
                                        $"CRATE = '{modelRecord.Master.CRATE}', " +
                                        $"SHIP_DATE = '{modelRecord.Master.SHIP_DATE}', " +
                                        $"SHIP_STATUS = '{modelRecord.Master.SHIP_STATUS}', " +
                                        $"UNIT = '{modelRecord.Master.UNIT}', " +
                                        $"SODA_TYPE = '{modelRecord.Master.SODA_TYPE}', " +
                                        $"SBF_TYPE = '{modelRecord.Master.SBF_TYPE}' " +
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
                                    item.SCOMP = item.SCOMP == null ? 0 : item.SCOMP;
                                    if (item.DT_CODE == null || item.DT_CODE == 0)
                                    {
                                        int detailCode = GenerateNextDetailId(common, command);
                                        if (detailCode > 0)
                                        {
                                            detailQuery = $"INSERT INTO {detailTable}" +
                                                           "(TRAN_ID, DT_CODE, ITEM_CODE, WAREHOUSE, QTY, " +
                                                           "UNIT, QTY2, BAL_QTY, RATE, CRATE, " +
                                                           "AMT, DT_DESC, TRUCK_NO, CONT_NO, SCOMP, BCODE, " +
                                                           "PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                                           "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                                           "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                                           "ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT, CHK, PICK_ID, " +
                                                           "RT_TYPE, INS, LOT_NO)" +
                                                           "VALUES" +
                                                           "('" + modelRecord.Master.TRAN_ID + "','" + detailCode + "','" + item.ITEM_CODE + "','" + item.WAREHOUSE + "','" + item.QTY + "'," +
                                                           "'" + item.UNIT + "','" + item.QTY2 + "','" + item.BAL_QTY + "','" + item.RATE + "','" + item.CRATE + "'," +
                                                           "'" + item.AMT + "','" + item.DT_DESC + "'," +
                                                           "'" + item.TRUCK_NO + "','" + item.CONT_NO + "','" + item.SCOMP + "','" + branch + "'," +
                                                           "'" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                                           "'" + computer + "','" + ip + "','" + username + "'," +
                                                           "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + computer + "','" + ip + "'," +
                                                           "'" + postal + "','" + postal + "','" + menuID + "','T','" + item.CHK + "','" + item.PICK_ID + "','" + item.RT_TYPE + "','" + item.INS + "','" + item.LOT_NO + "')";

                                            if (item.PICK_ID > 0)
                                            {
                                                detailQuery += $" UPDATE {pickTable} SET SCOMP = {item.SCOMP}" +
                                                $" WHERE DT_CODE = '{item.PICK_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                                            }

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
                                                      $"ITEM_CODE = '{item.ITEM_CODE}', " +
                                                      $"WAREHOUSE = '{item.WAREHOUSE}', " +
                                                      $"LOT_NO = '{item.LOT_NO}', " +
                                                      $"QTY = '{item.QTY}', " +
                                                      $"UNIT = '{item.UNIT}', " +
                                                      $"QTY2 = '{item.QTY2}', " +
                                                      $"BAL_QTY = '{item.BAL_QTY}', " +
                                                      $"RATE = '{item.RATE}', " +
                                                      $"CRATE = '{item.CRATE}', " +
                                                      $"AMT = '{item.AMT}', " +
                                                      $"DT_DESC = '{item.DT_DESC}', " +
                                                      $"TRUCK_NO = '{item.TRUCK_NO}', " +
                                                      $"CONT_NO = '{item.CONT_NO}', " +
                                                      $"SCOMP = '{item.SCOMP}', " +
                                                      $"EDIT_USER_ID = '{username}', " +
                                                      $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                      $"EDIT_COMPUTER_NAME = '{computer}', " +
                                                      $"EDIT_IP_ADDRESS = '{ip}', " +
                                                      $"EDIT_POSTALCODE = '{postal}', " +
                                                      $"CHK = '{item.CHK}', " +
                                                      $"PICK_ID = '{item.PICK_ID}', " +
                                                      $"RT_TYPE = '{item.RT_TYPE}', " +
                                                      $"INS = '{item.INS}', " +
                                                      $"DLT = 'T' " +
                                                      $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND DT_CODE = '{item.DT_CODE}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";

                                        if (item.PICK_ID > 0)
                                        {
                                            detailQuery += $" UPDATE {pickTable} SET SCOMP = {item.SCOMP}" +
                                            $" WHERE DT_CODE = '{item.PICK_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                                        }

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

        public MyHttpResponseMessage GetDeliveryFeedingByCode(int code, Common common)
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
                        string query = $"SELECT * FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                D_NAME = Convert.ToString(reader["D_NAME"]),
                                D_NUM = Convert.ToString(reader["D_NUM"]),
                                BR_AMOUNT_BUYER = Convert.ToString(reader["BR_AMOUNT_BUYER"]),
                                BR_AMOUNT_SELLER = Convert.ToString(reader["BR_AMOUNT_SELLER"]),
                                WT_AMOUNT_SELLER = Convert.ToString(reader["WT_AMOUNT_SELLER"]),
                                WT_AMOUNT_BUYER = Convert.ToString(reader["WT_AMOUNT_BUYER"]),
                                B_NET_AMOUNT = Convert.ToString(reader["B_NET_AMOUNT"]),
                                S_NET_AMOUNT = Convert.ToString(reader["S_NET_AMOUNT"]),
                                BARDANA = Convert.ToString(reader["BARDANA"]),
                                SBARDANA = Convert.ToString(reader["SBARDANA"]),
                                BGOD_CHARGES = Convert.ToString(reader["BGOD_CHARGES"]),
                                SGOD_CHARGES = Convert.ToString(reader["SGOD_CHARGES"]),
                                SLABOUR = Convert.ToString(reader["SLABOUR"]),
                                BLABOUR = Convert.ToString(reader["BLABOUR"]),
                                BFRIEGHT = Convert.ToString(reader["BFRIEGHT"]),
                                SFRIEGHT = Convert.ToString(reader["SFRIEGHT"]),
                                BFUMIGATION = Convert.ToString(reader["BFUMIGATION"]),
                                SFUMIGATION = Convert.ToString(reader["SFUMIGATION"]),
                                BR_AMOUNT_BUYER_ST = Convert.ToString(reader["BR_AMOUNT_BUYER_ST"]),
                                BR_AMOUNT_SELLER_ST = Convert.ToString(reader["BR_AMOUNT_SELLER_ST"]),
                                WT_AMOUNT_BUYER_ST = Convert.ToString(reader["WT_AMOUNT_BUYER_ST"]),
                                WT_AMOUNT_SELLER_ST = Convert.ToString(reader["WT_AMOUNT_SELLER_ST"]),
                                BARDANA_ST = Convert.ToString(reader["BARDANA_ST"]),
                                SBARDANA_ST = Convert.ToString(reader["SBARDANA_ST"]),
                                BGOD_CHARGES_ST = Convert.ToString(reader["BGOD_CHARGES_ST"]),
                                SGOD_CHARGES_ST = Convert.ToString(reader["SGOD_CHARGES_ST"]),
                                BLABOUR_ST = Convert.ToString(reader["BLABOUR_ST"]),
                                SLABOUR_ST = Convert.ToString(reader["SLABOUR_ST"]),
                                BFRIEGHT_ST = Convert.ToString(reader["BFRIEGHT_ST"]),
                                SFRIEGHT_ST = Convert.ToString(reader["SFRIEGHT_ST"]),
                                BFUMIGATION_ST = Convert.ToString(reader["BFUMIGATION_ST"]),
                                SFUMIGATION_ST = Convert.ToString(reader["SFUMIGATION_ST"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                SELLER_CODE = $"{Convert.ToString(reader["SELLER_CODE"])}{Convert.ToString(reader["SACT_CODE"])}",
                                BUYER_CODE = $"{Convert.ToString(reader["BUYER_CODE"])}{Convert.ToString(reader["BACT_CODE"])}",
                                BROKER_CODE = $"{Convert.ToString(reader["BROKER_CODE"])}{Convert.ToString(reader["BD_ACT_CODE"])}",
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                COND = Convert.ToString(reader["COND"]),
                                SDATE = reader["SDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SDATE"]).ToString("yyyy-MM-dd"),
                                CREDIT_DAYS = Convert.ToString(reader["CREDIT_DAYS"]),
                                DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),
                                COB_CODE = $"{Convert.ToString(reader["COB_CODE"])}{Convert.ToString(reader["COB_ACODE"])}",
                                CURR_CODE = reader["CURR_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CURR_CODE"]),
                                CRATE = Convert.ToString(reader["CRATE"]),
                                SHIP_STATUS = reader["SHIP_STATUS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SHIP_STATUS"]),
                                SHIP_DATE = reader["SHIP_DATE"] == DBNull.Value ? null : Convert.ToInt32(reader["SHIP_STATUS"]) == 1 ? Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM") : Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM-dd"),
                                UNIT = reader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UNIT"]),
                                SODA_TYPE = Convert.ToString(reader["SODA_TYPE"]),
                                SBF_TYPE = Convert.ToString(reader["SBF_TYPE"]),
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

        public MyHttpResponseMessage GetDeliveryFeedingDefaultOperators(Common common)
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
                        string query = $"SELECT TOP 1 * FROM {table} WHERE DLT = 'T' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}' ORDER BY TRAN_ID DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                BR_AMOUNT_BUYER_ST = Convert.ToString(reader["BR_AMOUNT_BUYER_ST"]),
                                BR_AMOUNT_SELLER_ST = Convert.ToString(reader["BR_AMOUNT_SELLER_ST"]),
                                WT_AMOUNT_BUYER_ST = Convert.ToString(reader["WT_AMOUNT_BUYER_ST"]),
                                WT_AMOUNT_SELLER_ST = Convert.ToString(reader["WT_AMOUNT_SELLER_ST"]),
                                BARDANA_ST = Convert.ToString(reader["BARDANA_ST"]),
                                SBARDANA_ST = Convert.ToString(reader["SBARDANA_ST"]),
                                BGOD_CHARGES_ST = Convert.ToString(reader["BGOD_CHARGES_ST"]),
                                SGOD_CHARGES_ST = Convert.ToString(reader["SGOD_CHARGES_ST"]),
                                BLABOUR_ST = Convert.ToString(reader["BLABOUR_ST"]),
                                SLABOUR_ST = Convert.ToString(reader["SLABOUR_ST"]),
                                BFRIEGHT_ST = Convert.ToString(reader["BFRIEGHT_ST"]),
                                SFRIEGHT_ST = Convert.ToString(reader["SFRIEGHT_ST"]),
                                BFUMIGATION_ST = Convert.ToString(reader["BFUMIGATION_ST"]),
                                SFUMIGATION_ST = Convert.ToString(reader["SFUMIGATION_ST"]),
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

        public MyHttpResponseMessage GetDeliveryFeedingDetailByCode(int code, Common common)
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
                        string query = @$"SELECT D.DT_CODE, D.ITEM_CODE, D.WAREHOUSE, D.QTY,D.UNIT, D.QTY2, D.BAL_QTY, D.RATE, D.CRATE, D.AMT, D.DT_DESC,D.TRUCK_NO, 
                                            D.CONT_NO,D.SCOMP, D.CHK, D.PICK_ID,D.RT_TYPE, D.INS, D.LOT_NO, SBFM.VOUCHER_NO AS VOUCHER_NO,MB.ID AS MENU_ID ,MB.MENU_PAGE,
                                            MB.MENU_PARENT_CODE,SBFM.TRAN_ID,SBFM.BCODE,SBFM.PERIOD_ID 
                                            FROM {table} D
                                            LEFT OUTER JOIN TBL_SBF_DETAIL SBF ON SBF.DT_CODE = D.PICK_ID AND SBF.PERIOD_ID = D.PERIOD_ID AND SBF.BCODE = D.BCODE
                                            LEFT OUTER JOIN TBL_SBF_MASTER SBFM ON SBF.TRAN_ID = SBFM.TRAN_ID AND SBF.PERIOD_ID = SBFM.PERIOD_ID AND SBF.BCODE = SBFM.BCODE
                                            LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = SBFM.MENU_ID
                                            WHERE
                                            D.DLT = 'T'
                                            AND D.TRAN_ID = '{code}'
                                            AND D.BCODE = '{common.Branch}'
                                            AND D.PERIOD_ID = '{common.Period}'
                                            ORDER BY
                                            D.DT_CODE DESC";


                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                WAREHOUSE = reader["WAREHOUSE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["WAREHOUSE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToInt32(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                LOT_NO = Convert.ToString(reader["LOT_NO"]),
                                CRATE = Convert.ToString(reader["CRATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                TRUCK_NO = Convert.ToString(reader["TRUCK_NO"]),
                                CONT_NO = Convert.ToString(reader["CONT_NO"]),
                                SCOMP = Convert.ToString(reader["SCOMP"]),
                                PICK_ID = Convert.ToString(reader["PICK_ID"]),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                CHK = Convert.ToString(reader["CHK"]),
                                CHK1 = Convert.ToString(reader["CHK"]) == "1" ? true : false,
                                SCOMP1 = Convert.ToString(reader["SCOMP"]) == "1" ? true : false,
                                //BR_AMOUNT_BUYER = Convert.ToString(reader["BR_AMOUNT_BUYER"]),
                                //BR_AMOUNT_SELLER = Convert.ToString(reader["BR_AMOUNT_SELLER"]),
                                //WT_AMOUNT_BUYER = Convert.ToString(reader["WT_AMOUNT_BUYER"]),
                                //WT_AMOUNT_SELLER = Convert.ToString(reader["WT_AMOUNT_SELLER"]),
                                //NET_AMT = Convert.ToString(reader["NET_AMT"]),
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
                                INS = Convert.ToString(reader["INS"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
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

        public MyHttpResponseMessage Delete(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? table2 = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    table2 = menu.TABLE2;
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
                            string query = $"UPDATE {table} SET DLT = 'F' WHERE TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'" +
                                $"UPDATE {table2} SET DLT = 'F' WHERE TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
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

                DeliveryFeeding deliveryFeeding = new DeliveryFeeding();
                List<DeliveryFeedingDetail> deliveryFeedingDetailList = new List<DeliveryFeedingDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        deliveryFeeding = new DeliveryFeeding
                        {
                            V_DATE = Convert.ToDateTime(reader["V_DATE"]),
                            SELLER_CODE = Convert.ToString(reader["SELLER_CODE"]),
                            SACT_CODE = Convert.ToInt32(reader["SACT_CODE"]),
                            BUYER_CODE = Convert.ToString(reader["BUYER_CODE"]),
                            BACT_CODE = Convert.ToInt32(reader["BACT_CODE"]),
                            REF = Convert.ToString(reader["REF"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            COND = Convert.ToString(reader["COND"]),
                            BROKER_CODE = Convert.ToString(reader["BROKER_CODE"]),
                            BD_ACT_CODE = Convert.ToInt32(reader["BD_ACT_CODE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            SDATE = Convert.ToDateTime(reader["SDATE"]),
                            CREDIT_DAYS = Convert.ToDouble(reader["CREDIT_DAYS"]),
                            DUE_DATE = Convert.ToDateTime(reader["DUE_DATE"]),
                            BR_AMOUNT_BUYER = Convert.ToDouble(reader["BR_AMOUNT_BUYER"]),
                            BR_AMOUNT_SELLER = Convert.ToDouble(reader["BR_AMOUNT_SELLER"]),
                            WT_AMOUNT_BUYER = Convert.ToDouble(reader["WT_AMOUNT_BUYER"]),
                            WT_AMOUNT_SELLER = Convert.ToDouble(reader["WT_AMOUNT_SELLER"]),
                            B_NET_AMOUNT = Convert.ToDouble(reader["B_NET_AMOUNT"]),
                            S_NET_AMOUNT = Convert.ToDouble(reader["S_NET_AMOUNT"]),
                            BARDANA = Convert.ToDouble(reader["BARDANA"]),
                            D_NAME = Convert.ToString(reader["D_NAME"]),
                            D_NUM = Convert.ToString(reader["D_NUM"]),
                            SBARDANA = Convert.ToDouble(reader["SBARDANA"]),
                            BR_AMOUNT_BUYER_ST = Convert.ToString(reader["BR_AMOUNT_BUYER_ST"]),
                            BR_AMOUNT_SELLER_ST = Convert.ToString(reader["BR_AMOUNT_SELLER_ST"]),
                            WT_AMOUNT_BUYER_ST = Convert.ToString(reader["WT_AMOUNT_BUYER_ST"]),
                            WT_AMOUNT_SELLER_ST = Convert.ToString(reader["WT_AMOUNT_SELLER_ST"]),
                            BARDANA_ST = Convert.ToString(reader["BARDANA_ST"]),
                            SBARDANA_ST = Convert.ToString(reader["SBARDANA_ST"]),
                            SGOD_CHARGES = Convert.ToDouble(reader["SGOD_CHARGES"]),
                            SGOD_CHARGES_ST = Convert.ToString(reader["SGOD_CHARGES_ST"]),
                            SLABOUR = Convert.ToDouble(reader["SLABOUR"]),
                            SLABOUR_ST = Convert.ToString(reader["SLABOUR_ST"]),
                            SFRIEGHT = Convert.ToDouble(reader["SFRIEGHT"]),
                            SFRIEGHT_ST = Convert.ToString(reader["SFRIEGHT_ST"]),
                            SFUMIGATION = Convert.ToDouble(reader["SFUMIGATION"]),
                            SFUMIGATION_ST = Convert.ToString(reader["SFUMIGATION_ST"]),
                            BGOD_CHARGES = Convert.ToDouble(reader["BGOD_CHARGES"]),
                            BGOD_CHARGES_ST = Convert.ToString(reader["BGOD_CHARGES_ST"]),
                            BLABOUR = Convert.ToDouble(reader["BLABOUR"]),
                            BLABOUR_ST = Convert.ToString(reader["BLABOUR_ST"]),
                            BFRIEGHT = Convert.ToDouble(reader["BFRIEGHT"]),
                            BFRIEGHT_ST = Convert.ToString(reader["BFRIEGHT_ST"]),
                            BFUMIGATION = Convert.ToDouble(reader["BFUMIGATION"]),
                            BFUMIGATION_ST = Convert.ToString(reader["BFUMIGATION_ST"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new DeliveryFeedingDetail
                        {
                            ITEM_CODE = Convert.ToInt32(detail_Reader["ITEM_CODE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                            UNIT = Convert.ToInt32(detail_Reader["UNIT"]),
                            QTY2 = Convert.ToDouble(detail_Reader["QTY2"]),
                            BAL_QTY = Convert.ToDouble(detail_Reader["BAL_QTY"]),
                            RATE = Convert.ToDouble(detail_Reader["RATE"]),
                            AMT = Convert.ToDouble(detail_Reader["AMT"]),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            TRUCK_NO = Convert.ToString(detail_Reader["TRUCK_NO"]),
                            CONT_NO = Convert.ToString(detail_Reader["CONT_NO"]),
                            SCOMP = Convert.ToInt32(detail_Reader["SCOMP"]),
                            PICK_ID = Convert.ToInt32(detail_Reader["PICK_ID"]),
                            CHK = detail_Reader["CHK"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["CHK"]),
                            RT_TYPE = Convert.ToDouble(detail_Reader["RT_TYPE"]),
                            INS = Convert.ToDouble(detail_Reader["INS"]),
                            CRATE = Convert.ToDouble(detail_Reader["CRATE"]),
                            WAREHOUSE = Convert.ToInt32(detail_Reader["WAREHOUSE"]),
                            LOT_NO = Convert.ToString(detail_Reader["LOT_NO"]),
                        };
                        deliveryFeedingDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customRequisition = new CustomDeliveryFeeding
                {
                    Master = deliveryFeeding,
                    Detail = deliveryFeedingDetailList
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

        public MyHttpResponseMessage DeleteDeliveryFeedingDetailByCode(int code, Common common)
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

        public MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty, pickMasterTable = string.Empty, pickDetailTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickMasterTable = menu.PICK_TABLE_MASTER;
                    pickDetailTable = menu.PICK_TABLE_DETAIL;
                }

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable) && !String.IsNullOrWhiteSpace(pickMasterTable) && !String.IsNullOrWhiteSpace(pickDetailTable))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"SELECT SM.TRAN_ID , SM.V_DATE, SM.VOUCHER_NO, UN.GROUP_NAME AS UNIT_NAME, PTS.PARTY_NAME AS SELLER, PTB.PARTY_NAME AS BUYER, PTBR.PARTY_NAME AS BROKER
                                        ,SD.ITEM_CODE, IM.ITEM_NAME,
                                        (ISNULL((SELECT SUM(ISNULL(BAL_QTY,0)) FROM TBL_SBF_DETAIL
                                        WHERE SD.DT_CODE = DT_CODE AND SD.PERIOD_ID = PERIOD_ID AND SD.BCODE = BCODE),0))  AS SQTY,
                                        ISNULL((SELECT SUM(ISNULL(BAL_QTY, 0)) FROM TBL_DF_DETAIL
                                        WHERE DFD.PICK_ID = PICK_ID AND DFD.PERIOD_ID = PERIOD_ID AND DFD.BCODE = BCODE),0) AS DQTY,
                                        (ISNULL((SELECT SUM(ISNULL(BAL_QTY, 0)) FROM TBL_SBF_DETAIL
                                        WHERE SD.DT_CODE = DT_CODE AND SD.PERIOD_ID = PERIOD_ID AND SD.BCODE = BCODE), 0) -
                                        ISNULL((SELECT SUM(ISNULL(BAL_QTY,0)) FROM TBL_DF_DETAIL
                                        WHERE DFD.PICK_ID = PICK_ID AND DFD.PERIOD_ID = PERIOD_ID AND DFD.BCODE = BCODE),0))
                                        AS BAL_QTY, '' AS QTY,
                                        SD.RATE,SD.AMT,SD.DT_CODE as PICK_ID, SD.UNIT, SM.SELLER_CODE, SM.SACT_CODE, SM.BUYER_CODE, SM.BACT_CODE, SM.BROKER_CODE, SM.BD_ACT_CODE, 
                                        SM.COND AS COND, SM.REF AS REF, SM.REMARKS AS REMARKS
                                        , SM.CREDIT_DAYS, SM.DUE_DATE, SD.RT_TYPE,
                                        CRR.CODE AS CURRENCY_CODE , CRR.DESCR AS CURRENCY_NAME,
										SM.SODA_TYPE,SM.SHIP_DATE,SM.SHIP_STATUS,SM.COB_CODE,SM.COB_ACODE,COPT.PARTY_NAME AS CO_BROKER,SM.SBF_TYPE,CASE 
                                        WHEN SHIP_DATE = '1900-01-01 00:00:00.000' THEN '' ELSE CASE 
                                        WHEN SHIP_STATUS = 1 THEN FORMAT(SHIP_DATE, 'MM-yyyy')
                                        WHEN SHIP_STATUS = 0 THEN FORMAT(SHIP_DATE, 'dd-MM-yyyy')
                                        ELSE CONVERT(varchar, SHIP_DATE, 120)  -- Optional: fallback formatting
                                        END END AS FSHIP_DATE,MB.ID AS MENU_ID,MB.MENU_PAGE,MB.MENU_PARENT_CODE
                                        FROM TBL_SBF_MASTER SM
                                        LEFT OUTER JOIN TBL_SBF_DETAIL SD
                                        ON SD.TRAN_ID = SM.TRAN_ID AND SD.BCODE = SM.BCODE AND SD.PERIOD_ID = SM.PERIOD_ID
                                        LEFT OUTER JOIN TBL_DF_DETAIL DFD
                                        ON SD.DT_CODE = DFD.PICK_ID AND SD.BCODE = DFD.BCODE AND SD.PERIOD_ID = DFD.PERIOD_ID
                                        AND DFD.DLT = 'T'
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PTS
                                        ON PTS.PARTY_CODE = SM.SELLER_CODE AND PTS.ACT_CODE = SM.SACT_CODE
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PTB
                                        ON PTB.PARTY_CODE = SM.BUYER_CODE AND PTB.ACT_CODE = SM.BACT_CODE
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PTBR
                                        ON PTBR.PARTY_CODE = SM.BROKER_CODE AND PTBR.ACT_CODE = SM.BD_ACT_CODE
                                        LEFT OUTER JOIN TBL_ITEMSMASTER IM
                                        ON IM.ITEM_CODE = SD.ITEM_CODE
                                        LEFT OUTER JOIN TBL_CURRENCY CRR
                                        ON CRR.CODE = SM.CURR_CODE
										 LEFT OUTER JOIN TBL_PARTY_TYPES COPT
                                        ON COPT.PARTY_CODE = SM.COB_CODE AND COPT.ACT_CODE = SM.COB_ACODE
										LEFT OUTER JOIN TBL_MENU_BUILDER MB
										ON MB.ID = SM.MENU_ID 
                                        LEFT OUTER JOIN TBL_UNIT UN
                                        ON SD.UNIT = UN.GROUP_CODE 
                                        WHERE SM.DLT = 'T' AND SM.ASTATUS = 'Y' AND SD.DLT = 'T'
                                        AND SM.V_DATE = '{sodaDate}' AND SM.BCODE = '{common.Branch}' AND SM.PERIOD_ID = '{common.Period}' 
                                        AND (SD.SCOMP = 0 OR SD.SCOMP IS NULL)
                                        AND IM.ITEM_CODE IN (
                                            SELECT M.ITEM_CODE
                                            FROM TBL_ROLE R 
                                            LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE 
                                            LEFT OUTER JOIN TBL_ITEMSGROUP IG ON M.GROUP_CODE = IG.GROUP_CODE 
                                            WHERE R.ROLE_TYPE = '{common.RoleType}'
                                              AND R.MODULE_ID = 4 
                                              AND IG.GROUP_TYPE = 'S' 
                                              AND IG.DLT = 'T' 
                                              AND IG.ASTATUS = 'Y'
                                              AND R.ROLE_ID = {common.RoleID}
                                            GROUP BY M.ITEM_CODE
                                        
                                            UNION ALL
                                        
                                            SELECT M.ITEM_CODE 
                                            FROM TBL_ITEMSMASTER M 
                                            WHERE '{common.RoleType}' = 'A'   
                                              AND M.DLT = 'T' 
                                              AND M.ASTATUS = 'Y'
                                        
                                            UNION ALL
                                        
                                            SELECT M.ITEM_CODE 
                                            FROM TBL_ITEMSMASTER M 
                                            WHERE '{common.RoleType}' = 'U'  
                                              AND M.DLT = 'T' 
                                              AND M.ASTATUS = 'Y' 
                                              AND (
                                                SELECT COUNT(R.MODULE_ID) 
                                                FROM TBL_ROLE R 
                                                LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE 
                                                LEFT OUTER JOIN TBL_ITEMSGROUP IG ON M.GROUP_CODE = IG.GROUP_CODE 
                                                WHERE R.ROLE_TYPE = '{common.RoleType}'
                                                  AND R.MODULE_ID = 4 
                                                  AND IG.GROUP_TYPE = 'S' 
                                                  AND IG.DLT = 'T' 
                                                  AND IG.ASTATUS = 'Y'
                                                  AND R.ROLE_ID = {common.RoleID}
                                              ) < 1
                                        )
                                        
                                        
                                        
                                        
                                        GROUP BY
                                        SM.TRAN_ID , SM.V_DATE, SM.VOUCHER_NO,SM.REF,SM.REMARKS, PTS.PARTY_NAME , PTB.PARTY_NAME, PTBR.PARTY_NAME 
                                        ,SD.ITEM_CODE, IM.ITEM_NAME, SD.RATE, SD.AMT, SD.DT_CODE, SD.PERIOD_ID, SD.BCODE, DFD.PICK_ID,
                                        DFD.BCODE, DFD.PERIOD_ID, SD.UNIT, SM.SELLER_CODE, SM.SACT_CODE, UN.GROUP_NAME, SM.BUYER_CODE, SM.BACT_CODE, SM.BROKER_CODE, SM.BD_ACT_CODE, SM.COND, SM.CREDIT_DAYS, SM.DUE_DATE, SD.RT_TYPE,UN.QTY,
                                        CRR.CODE,CRR.DESCR,SM.SODA_TYPE,SM.SHIP_DATE,SM.SHIP_STATUS,SM.COB_CODE,SM.COB_ACODE,COPT.PARTY_NAME,SM.SBF_TYPE,MB.ID ,MB.MENU_PAGE,MB.MENU_PARENT_CODE";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                V_DATE = Convert.ToString(reader["V_DATE"]),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                SELLER = Convert.ToString(reader["SELLER"]),
                                BUYER = Convert.ToString(reader["BUYER"]),
                                BROKER = Convert.ToString(reader["BROKER"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                                QTY = Convert.ToString(reader["SQTY"]),
                                SQTY = Convert.ToString(reader["SQTY"]),
                                DQTY = Convert.ToString(reader["DQTY"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                PICK_ID = Convert.ToString(reader["PICK_ID"]),
                                SELLER_CODE = $"{Convert.ToString(reader["SELLER_CODE"])}{Convert.ToString(reader["SACT_CODE"])}",
                                BUYER_CODE = $"{Convert.ToString(reader["BUYER_CODE"])}{Convert.ToString(reader["BACT_CODE"])}",
                                BROKER_CODE = $"{Convert.ToString(reader["BROKER_CODE"])}{Convert.ToString(reader["BD_ACT_CODE"])}",
                                UNIT = Convert.ToInt32(reader["UNIT"]),
                                UNIT_NAME = Convert.ToString(reader["UNIT_NAME"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                COND = Convert.ToString(reader["COND"]),
                                CREDIT_DAYS = Convert.ToString(reader["CREDIT_DAYS"]),
                                DUE_DATE = Convert.ToString(reader["DUE_DATE"]),
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
                                SBF_TYPE = Convert.ToString(reader["SBF_TYPE"]),
                                COB_CODE = $"{Convert.ToString(reader["COB_CODE"])}{Convert.ToString(reader["COB_ACODE"])}",
                                CO_BROKER = Convert.ToString(reader["SHIP_DATE"]),
                                //SHIP_DATE = Convert.ToString(reader["SHIP_DATE"]),
                                //SHIP_STATUS = Convert.ToString(reader["SHIP_STATUS"]),
                                SHIP_STATUS = reader["SHIP_STATUS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SHIP_STATUS"]),
                                SHIP_DATE = reader["SHIP_DATE"] == DBNull.Value ? null : Convert.ToInt32(reader["SHIP_STATUS"]) == 1 ? Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM") : Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM-dd"),
                                FSHIP_DATE = Convert.ToString(reader["FSHIP_DATE"]),
                                SODA_TYPE = Convert.ToString(reader["SODA_TYPE"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                CURR_CODE = reader["CURRENCY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CURRENCY_CODE"]),
                                CURR_NAME = Convert.ToString(reader["CURRENCY_NAME"]),
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

        private List<object> GetDeliveryDetails(string tranID, MyHttpResponseMessage menuData)
        {
            List<object> jsonDataResult = new List<object>();
            try
            {
                string? detailTable = string.Empty;
                if (menuData.data != null)
                {
                    var menu = (Menu)menuData.data;
                    detailTable = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(detailTable))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT A.TRAN_ID, IM.ITEM_NAME AS ITEM_CODE, A.QTY," +
                                       "U.GROUP_NAME AS UNIT, QTY2, BAL_QTY, RATE, RTRIM(LTRIM(RT_TYPE)) + ' kg' as RT_TYPE, AMT, DT_DESC, A.TRUCK_NO, " +
                                       "A.CONT_NO, A.SCOMP, A.BR_AMOUNT_BUYER, A.BR_AMOUNT_SELLER, A.WT_AMOUNT_BUYER, A.WT_AMOUNT_SELLER, A.NET_AMT " +
                                       $"FROM {detailTable} A " +
                                       "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE " +
                                       "LEFT OUTER JOIN TBL_UNIT U ON A.UNIT = U.GROUP_CODE " +
                                       $"WHERE A.DLT = 'T' AND A.TRAN_ID = '{tranID}' ORDER BY A.DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                TRUCK_NO = Convert.ToString(reader["TRUCK_NO"]),
                                CONT_NO = Convert.ToString(reader["CONT_NO"]),
                                SCOMP = Convert.ToString(reader["SCOMP"]) == "1" ? "Yes" : "No",
                                BR_AMOUNT_BUYER = Convert.ToString(reader["BR_AMOUNT_BUYER"]),
                                BR_AMOUNT_SELLER = Convert.ToString(reader["BR_AMOUNT_SELLER"]),
                                WT_AMOUNT_BUYER = Convert.ToString(reader["WT_AMOUNT_BUYER"]),
                                WT_AMOUNT_SELLER = Convert.ToString(reader["WT_AMOUNT_SELLER"]),
                                NET_AMT = Convert.ToString(reader["NET_AMT"]),
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

        public MyHttpResponseMessage GetDataForReport(DeliveryFeedingReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            DeliveryFeedingReport masterData = new DeliveryFeedingReport();
            CustomDeliveryFeedingForPrintReport reportData = new CustomDeliveryFeedingForPrintReport();
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
                if (menuDetails.MD_ID == 9)
                {
                    topQuery = @$"SELECT TOP 1 M.VOUCHER_NO AS INVOICE_NO, M.V_DATE AS DEL_DATE,
                                DPM.V_DATE AS SODA_DATE, M.REMARKS AS COMMENT, BRO.PARTY_NAME AS BROKER_NAME,

                                CASE WHEN M.COND = 'CR' THEN CONVERT(NVARCHAR(50),M.CREDIT_DAYS)+' Days' 
                                ELSE CASE WHEN M.COND  = 'Cash' THEN M.COND   
                                ELSE CASE WHEN M.COND  = 'Adv' THEN 'Advance'  
                                ELSE CASE WHEN M.COND  = 'CRD' THEN CONVERT(NVARCHAR(50), M.DUE_DATE, 105)  
                                END END END END AS COND, 
                                M.BGOD_CHARGES AS GOD_CHARGES, 
                                M.BLABOUR AS LABOUR, 
                                M.BFRIEGHT AS FRIEGHT, 
                                M.BFUMIGATION AS FUMIGATION
                                ,PT.PARTY_NAME, d.TRUCK_NO
                                ,M.BR_AMOUNT_BUYER AS BR_AMOUNT, M.WT_AMOUNT_BUYER AS WT_AMOUNT, M.BARDANA AS BARDANA,
                                M.B_NET_AMOUNT AS NET_AMOUNT
                                FROM {table} M
                                LEFT OUTER JOIN {detailTable} D
                                ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                                ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_MASTER DPM
                                ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID
                                LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                ON PT.PARTY_CODE = M.BUYER_CODE AND PT.ACT_CODE = M.BACT_CODE
                                LEFT OUTER JOIN TBL_PARTY_TYPES BRO
                                ON BRO.PARTY_CODE = M.BROKER_CODE AND BRO.ACT_CODE = M.BD_ACT_CODE
                                LEFT OUTER JOIN TBL_ITEMSMASTER IT
                                ON IT.ITEM_CODE = D.ITEM_CODE 
                                WHERE M.DLT = 'T' AND M.ASTATUS = 'Y'  AND D.DLT = 'T' AND M.TRAN_ID = '{modelRecord.TRAN_ID}' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}'";

                    query = @$"SELECT IT.ITEM_NAME,D.QTY2 AS QTY,D.QTY AS WEIGHT1,D.RATE,CONVERT(NVARCHAR(50),D.RT_TYPE)+' kg'
                               As RT_TYPE,D.AMT
                               FROM {detailTable} D 
                               LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                               ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                               LEFT OUTER JOIN TBL_SBF_MASTER DPM
                               ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID 
                               LEFT OUTER JOIN TBL_ITEMSMASTER IT
                               ON IT.ITEM_CODE = D.ITEM_CODE 
                               WHERE D.DLT = 'T' AND D.TRAN_ID = '{modelRecord.TRAN_ID}' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}'";
                }
                else if (menuDetails.MD_ID == 34)
                {
                    topQuery = @$"SELECT TOP 1 M.VOUCHER_NO AS INVOICE_NO, M.V_DATE AS DEL_DATE,
                                DPM.V_DATE AS SODA_DATE, BRO.PARTY_NAME AS BROKER_NAME,

                                CASE WHEN M.COND = 'CR' THEN CONVERT(NVARCHAR(50),M.CREDIT_DAYS)+' Days' 
                                ELSE CASE WHEN M.COND  = 'Cash' THEN M.COND   
                                ELSE CASE WHEN M.COND  = 'Adv' THEN 'Advance'  
                                ELSE CASE WHEN M.COND  = 'CRD' THEN CONVERT(NVARCHAR(50), M.DUE_DATE, 105)  
                                END END END END AS COND,
                                M.REMARKS AS COMMENT,
                                PT.PARTY_NAME, d.TRUCK_NO, 
                                CASE WHEN M.BR_AMOUNT_BUYER_ST = 'P' THEN M.BR_AMOUNT_BUYER ELSE -M.BR_AMOUNT_BUYER END AS BR_AMOUNT, 
                                CASE WHEN M.WT_AMOUNT_BUYER_ST = 'P' THEN M.WT_AMOUNT_BUYER ELSE -M.WT_AMOUNT_BUYER END AS WT_AMOUNT, 
                                CASE WHEN M.BARDANA_ST = 'P' THEN M.BARDANA ELSE -M.BARDANA END AS BARDANA,
                                CASE WHEN M.BGOD_CHARGES_ST = 'P' THEN M.BGOD_CHARGES ELSE -M.BGOD_CHARGES END AS GOD_CHARGES, 
                                CASE WHEN M.BLABOUR_ST = 'P' THEN M.BLABOUR ELSE -M.BLABOUR END AS LABOUR, 
                                CASE WHEN M.BFRIEGHT_ST = 'P' THEN M.BFRIEGHT ELSE -M.BFRIEGHT END AS FRIEGHT, 
                                CASE WHEN M.BFUMIGATION_ST = 'P' THEN M.BFUMIGATION ELSE -M.BFUMIGATION END AS FUMIGATION,
                                M.B_NET_AMOUNT AS NET_AMOUNT 

                                FROM {table} M
                                LEFT OUTER JOIN {detailTable} D
                                ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                                ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_MASTER DPM
                                ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID
                                LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                ON PT.PARTY_CODE = M.BUYER_CODE AND PT.ACT_CODE = M.BACT_CODE
                                LEFT OUTER JOIN TBL_PARTY_TYPES BRO
                                ON BRO.PARTY_CODE = M.BROKER_CODE AND BRO.ACT_CODE = M.BD_ACT_CODE
                                LEFT OUTER JOIN TBL_ITEMSMASTER IT
                                ON IT.ITEM_CODE = D.ITEM_CODE 
                                WHERE M.DLT = 'T' AND M.ASTATUS = 'Y'  AND D.DLT = 'T' AND M.TRAN_ID = '{modelRecord.TRAN_ID}' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}'";

                    query = @$"SELECT IT.ITEM_NAME,D.QTY2 AS QTY,D.QTY AS WEIGHT1,D.RATE,CONVERT(NVARCHAR(50),D.RT_TYPE)+' kg'
                               As RT_TYPE,D.AMT
                               FROM {detailTable} D 
                               LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                               ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                               LEFT OUTER JOIN TBL_SBF_MASTER DPM
                               ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID 
                               LEFT OUTER JOIN TBL_ITEMSMASTER IT
                               ON IT.ITEM_CODE = D.ITEM_CODE 
                               WHERE D.DLT = 'T' AND D.TRAN_ID = '{modelRecord.TRAN_ID}' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}'";
                }
                else if (menuDetails.MD_ID == 44)
                {
                    topQuery = @$"SELECT TOP 1 M.VOUCHER_NO AS INVOICE_NO, M.V_DATE AS DEL_DATE,
                                DPM.V_DATE AS SODA_DATE, BRO.PARTY_NAME AS BROKER_NAME,

                                CASE WHEN M.COND = 'CR' THEN CONVERT(NVARCHAR(50),M.CREDIT_DAYS)+' Days' 
                                ELSE CASE WHEN M.COND  = 'Cash' THEN M.COND   
                                ELSE CASE WHEN M.COND  = 'Adv' THEN 'Advance'  
                                ELSE CASE WHEN M.COND  = 'CRD' THEN CONVERT(NVARCHAR(50), M.DUE_DATE, 105)  
                                END END END END AS COND,
                                M.REMARKS AS COMMENT,
                                PT.PARTY_NAME, d.TRUCK_NO, 
                                CASE WHEN M.BR_AMOUNT_BUYER_ST = 'P' THEN M.BR_AMOUNT_BUYER ELSE -M.BR_AMOUNT_BUYER END AS BR_AMOUNT, 
                                CASE WHEN M.WT_AMOUNT_BUYER_ST = 'P' THEN M.WT_AMOUNT_BUYER ELSE -M.WT_AMOUNT_BUYER END AS WT_AMOUNT, 
                                CASE WHEN M.BARDANA_ST = 'P' THEN M.BARDANA ELSE -M.BARDANA END AS BARDANA,
                                CASE WHEN M.BGOD_CHARGES_ST = 'P' THEN M.BGOD_CHARGES ELSE -M.BGOD_CHARGES END AS GOD_CHARGES, 
                                CASE WHEN M.BLABOUR_ST = 'P' THEN M.BLABOUR ELSE -M.BLABOUR END AS LABOUR, 
                                CASE WHEN M.BFRIEGHT_ST = 'P' THEN M.BFRIEGHT ELSE -M.BFRIEGHT END AS FRIEGHT, 
                                CASE WHEN M.BFUMIGATION_ST = 'P' THEN M.BFUMIGATION ELSE -M.BFUMIGATION END AS FUMIGATION,
                                M.B_NET_AMOUNT AS NET_AMOUNT 

                                FROM {table} M
                                LEFT OUTER JOIN {detailTable} D
                                ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                                ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_MASTER DPM
                                ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID
                                LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                ON PT.PARTY_CODE = M.BUYER_CODE AND PT.ACT_CODE = M.BACT_CODE
                                LEFT OUTER JOIN TBL_PARTY_TYPES BRO
                                ON BRO.PARTY_CODE = M.BROKER_CODE AND BRO.ACT_CODE = M.BD_ACT_CODE
                                LEFT OUTER JOIN TBL_ITEMSMASTER IT
                                ON IT.ITEM_CODE = D.ITEM_CODE 
                                WHERE M.DLT = 'T' AND M.ASTATUS = 'Y'  AND D.DLT = 'T' AND M.TRAN_ID = '{modelRecord.TRAN_ID}' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}'";

                    query = @$"SELECT IT.ITEM_NAME,D.QTY2 AS QTY,D.QTY AS WEIGHT1,D.RATE,CONVERT(NVARCHAR(50),D.RT_TYPE)+' kg'
                               As RT_TYPE,D.AMT
                               FROM {detailTable} D 
                               LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                               ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                               LEFT OUTER JOIN TBL_SBF_MASTER DPM
                               ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID 
                               LEFT OUTER JOIN TBL_ITEMSMASTER IT
                               ON IT.ITEM_CODE = D.ITEM_CODE 
                               WHERE D.DLT = 'T' AND D.TRAN_ID = '{modelRecord.TRAN_ID}' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}'";
                }
                else if (menuDetails.MD_ID == 35)
                {
                    topQuery = @$"SELECT TOP 1 M.VOUCHER_NO AS INVOICE_NO, M.V_DATE AS DEL_DATE,
                                DPM.V_DATE AS SODA_DATE, M.REMARKS AS COMMENT, BRO.PARTY_NAME AS BROKER_NAME,

                                CASE WHEN M.COND = 'CR' THEN CONVERT(NVARCHAR(50),M.CREDIT_DAYS)+' Days' 
                                ELSE CASE WHEN M.COND  = 'Cash' THEN M.COND   
                                ELSE CASE WHEN M.COND  = 'Adv' THEN 'Advance'  
                                ELSE CASE WHEN M.COND  = 'CRD' THEN CONVERT(NVARCHAR(50), M.DUE_DATE, 105)  
                                END END END END AS COND
                                
                                ,PT.PARTY_NAME, d.TRUCK_NO, 
                                CASE WHEN M.BR_AMOUNT_SELLER_ST = 'P' THEN M.BR_AMOUNT_SELLER ELSE -M.BR_AMOUNT_SELLER END AS BR_AMOUNT, 
                                CASE WHEN M.WT_AMOUNT_SELLER_ST = 'P' THEN M.WT_AMOUNT_SELLER ELSE -M.WT_AMOUNT_SELLER END AS WT_AMOUNT, 
                                CASE WHEN M.SBARDANA_ST = 'P' THEN M.SBARDANA ELSE -M.SBARDANA END AS BARDANA,
                                CASE WHEN M.SGOD_CHARGES_ST = 'P' THEN M.SGOD_CHARGES ELSE -M.SGOD_CHARGES END AS GOD_CHARGES, 
                                CASE WHEN M.SLABOUR_ST = 'P' THEN M.SLABOUR ELSE -M.SLABOUR END AS LABOUR, 
                                CASE WHEN M.SFRIEGHT_ST = 'P' THEN M.SFRIEGHT ELSE -M.SFRIEGHT END AS FRIEGHT, 
                                CASE WHEN M.SFUMIGATION_ST = 'P' THEN M.SFUMIGATION ELSE -M.SFUMIGATION END AS FUMIGATION,
                                M.S_NET_AMOUNT AS NET_AMOUNT 

                                FROM {table} M
                                LEFT OUTER JOIN {detailTable} D
                                ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                                ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_MASTER DPM
                                ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID
                                LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                ON PT.PARTY_CODE = M.SELLER_CODE AND PT.ACT_CODE = M.SACT_CODE
                                LEFT OUTER JOIN TBL_PARTY_TYPES BRO
                                ON BRO.PARTY_CODE = M.BROKER_CODE AND BRO.ACT_CODE = M.BD_ACT_CODE
                                LEFT OUTER JOIN TBL_ITEMSMASTER IT
                                ON IT.ITEM_CODE = D.ITEM_CODE 
                                WHERE M.DLT = 'T' AND M.ASTATUS = 'Y'  AND D.DLT = 'T' AND M.TRAN_ID = '{modelRecord.TRAN_ID}' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}'";

                    query = @$"SELECT IT.ITEM_NAME,D.QTY2 AS QTY,D.QTY AS WEIGHT1,D.RATE,CONVERT(NVARCHAR(50),D.RT_TYPE)+' kg'
                               As RT_TYPE,D.AMT
                               FROM {detailTable} D 
                               LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                               ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                               LEFT OUTER JOIN TBL_SBF_MASTER DPM
                               ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID 
                               LEFT OUTER JOIN TBL_ITEMSMASTER IT
                               ON IT.ITEM_CODE = D.ITEM_CODE 
                               WHERE D.DLT = 'T' AND D.TRAN_ID = '{modelRecord.TRAN_ID}' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}'";
                }
                else if (menuDetails.MD_ID == 45)
                {
                    topQuery = @$"SELECT TOP 1 M.VOUCHER_NO AS INVOICE_NO, M.V_DATE AS DEL_DATE,
                                DPM.V_DATE AS SODA_DATE, M.REMARKS AS COMMENT, BRO.PARTY_NAME AS BROKER_NAME,

                                CASE WHEN M.COND = 'CR' THEN CONVERT(NVARCHAR(50),M.CREDIT_DAYS)+' Days' 
                                ELSE CASE WHEN M.COND  = 'Cash' THEN M.COND   
                                ELSE CASE WHEN M.COND  = 'Adv' THEN 'Advance'  
                                ELSE CASE WHEN M.COND  = 'CRD' THEN CONVERT(NVARCHAR(50), M.DUE_DATE, 105)  
                                END END END END AS COND
                                
                                ,PT.PARTY_NAME, d.TRUCK_NO, 
                                CASE WHEN M.BR_AMOUNT_SELLER_ST = 'P' THEN M.BR_AMOUNT_SELLER ELSE -M.BR_AMOUNT_SELLER END AS BR_AMOUNT, 
                                CASE WHEN M.WT_AMOUNT_SELLER_ST = 'P' THEN M.WT_AMOUNT_SELLER ELSE -M.WT_AMOUNT_SELLER END AS WT_AMOUNT, 
                                CASE WHEN M.SBARDANA_ST = 'P' THEN M.SBARDANA ELSE -M.SBARDANA END AS BARDANA,
                                CASE WHEN M.SGOD_CHARGES_ST = 'P' THEN M.SGOD_CHARGES ELSE -M.SGOD_CHARGES END AS GOD_CHARGES, 
                                CASE WHEN M.SLABOUR_ST = 'P' THEN M.SLABOUR ELSE -M.SLABOUR END AS LABOUR, 
                                CASE WHEN M.SFRIEGHT_ST = 'P' THEN M.SFRIEGHT ELSE -M.SFRIEGHT END AS FRIEGHT, 
                                CASE WHEN M.SFUMIGATION_ST = 'P' THEN M.SFUMIGATION ELSE -M.SFUMIGATION END AS FUMIGATION,
                                M.S_NET_AMOUNT AS NET_AMOUNT 

                                FROM {table} M
                                LEFT OUTER JOIN {detailTable} D
                                ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                                ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_MASTER DPM
                                ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID
                                LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                ON PT.PARTY_CODE = M.SELLER_CODE AND PT.ACT_CODE = M.SACT_CODE
                                LEFT OUTER JOIN TBL_PARTY_TYPES BRO
                                ON BRO.PARTY_CODE = M.BROKER_CODE AND BRO.ACT_CODE = M.BD_ACT_CODE
                                LEFT OUTER JOIN TBL_ITEMSMASTER IT
                                ON IT.ITEM_CODE = D.ITEM_CODE 
                                WHERE M.DLT = 'T' AND M.ASTATUS = 'Y'  AND D.DLT = 'T' AND M.TRAN_ID = '{modelRecord.TRAN_ID}' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}'";

                    query = @$"SELECT IT.ITEM_NAME,D.QTY2 AS QTY,D.QTY AS WEIGHT1,D.RATE,CONVERT(NVARCHAR(50),D.RT_TYPE)+' kg'
                               As RT_TYPE,D.AMT
                               FROM {detailTable} D 
                               LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                               ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                               LEFT OUTER JOIN TBL_SBF_MASTER DPM
                               ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID 
                               LEFT OUTER JOIN TBL_ITEMSMASTER IT
                               ON IT.ITEM_CODE = D.ITEM_CODE 
                               WHERE D.DLT = 'T' AND D.TRAN_ID = '{modelRecord.TRAN_ID}' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}'";
                }
                else if (menuDetails.MD_ID == 8)
                {
                    topQuery = @$"SELECT TOP 1 M.VOUCHER_NO AS INVOICE_NO, M.V_DATE AS DEL_DATE,
                                DPM.V_DATE AS SODA_DATE, M.REMARKS AS COMMENT, BRO.PARTY_NAME AS BROKER_NAME,

                                CASE WHEN M.COND = 'CR' THEN CONVERT(NVARCHAR(50),M.CREDIT_DAYS)+' Days' 
                                ELSE CASE WHEN M.COND  = 'Cash' THEN M.COND   
                                ELSE CASE WHEN M.COND  = 'Adv' THEN 'Advance'  
                                ELSE CASE WHEN M.COND  = 'CRD' THEN CONVERT(NVARCHAR(50), M.DUE_DATE, 105)  
                                END END END END AS COND,
                                M.BGOD_CHARGES AS GOD_CHARGES,
                                M.BLABOUR AS LABOUR,
                                M.BFRIEGHT AS FRIEGHT,
                                M.BFUMIGATION AS FUMIGATION
                                ,PT.PARTY_NAME, d.TRUCK_NO
                                ,M.BR_AMOUNT_SELLER AS BR_AMOUNT, M.WT_AMOUNT_SELLER AS WT_AMOUNT, 0 AS BARDANA,
                                M.S_NET_AMOUNT AS NET_AMOUNT
                                FROM {table} M
                                LEFT OUTER JOIN {detailTable} D
                                ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                                ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                                LEFT OUTER JOIN TBL_SBF_MASTER DPM
                                ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID
                                LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                ON PT.PARTY_CODE = M.SELLER_CODE AND PT.ACT_CODE = M.SACT_CODE
                                LEFT OUTER JOIN TBL_PARTY_TYPES BRO
                                ON BRO.PARTY_CODE = M.BROKER_CODE AND BRO.ACT_CODE = M.BD_ACT_CODE
                                LEFT OUTER JOIN TBL_ITEMSMASTER IT
                                ON IT.ITEM_CODE = D.ITEM_CODE 
                                WHERE M.DLT = 'T' AND M.ASTATUS = 'Y' AND D.DLT = 'T' AND M.TRAN_ID = '{modelRecord.TRAN_ID}' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}'";

                    query = @$"SELECT IT.ITEM_NAME,D.QTY2 AS QTY,D.QTY AS WEIGHT1,D.RATE,CONVERT(NVARCHAR(50), D.RT_TYPE) + ' kg'
                               As RT_TYPE, D.AMT
                               FROM {detailTable} D 
                               LEFT OUTER JOIN TBL_SBF_DETAIL DPD
                               ON DPD.DT_CODE = D.PICK_ID AND D.BCODE = DPD.BCODE AND D.PERIOD_ID = DPD.PERIOD_ID
                               LEFT OUTER JOIN TBL_SBF_MASTER DPM
                               ON DPD.TRAN_ID = DPM.TRAN_ID AND D.BCODE = DPM.BCODE AND D.PERIOD_ID = DPM.PERIOD_ID 
                               LEFT OUTER JOIN TBL_ITEMSMASTER IT
                               ON IT.ITEM_CODE = D.ITEM_CODE
                               WHERE D.DLT = 'T' AND D.TRAN_ID = '{modelRecord.TRAN_ID}' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}'";
                }

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(topQuery, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        masterData.INVOICE_NUMBER = Convert.ToString(reader["INVOICE_NO"]);
                        masterData.SODA_DATE = reader["SODA_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SODA_DATE"]).ToString("dd-MM-yyyy");
                        masterData.DELIVERY_DATE = reader["DEL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DEL_DATE"]).ToString("dd-MM-yyyy");
                        masterData.CONDITION = Convert.ToString(reader["COND"]);
                        masterData.PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]);
                        masterData.COMPANY_NAME = currentCompany.C_NAME;
                        masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                        masterData.COMPANY_PHONE = currentCompany.C_TEL;
                        masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                        masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                        masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                        masterData.MENU_SIG1 = $"{menuDetails.MENU_SIG1}";
                        masterData.MENU_TERMS = $"{menuDetails.MENU_TERMS}";
                        masterData.TRUCK_NO = Convert.ToString(reader["TRUCK_NO"]);
                        masterData.BR_AMOUNT = reader["BR_AMOUNT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BR_AMOUNT"]);
                        masterData.WT_AMOUNT = reader["WT_AMOUNT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WT_AMOUNT"]);
                        masterData.NET_AMOUNT = reader["NET_AMOUNT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMOUNT"]);
                        masterData.BARDANA = reader["NET_AMOUNT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BARDANA"]);
                        masterData.GOD_CHARGES = reader["GOD_CHARGES"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["GOD_CHARGES"]);
                        masterData.LABOUR = reader["LABOUR"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["LABOUR"]);
                        masterData.FRIEGHT = reader["FRIEGHT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["FRIEGHT"]);
                        masterData.FUMIGATION = reader["FUMIGATION"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["FUMIGATION"]);
                        masterData.BROKER_NAME = Convert.ToString(reader["BROKER_NAME"]);
                        masterData.COMMENT = Convert.ToString(reader["COMMENT"]);
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
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["ItemName"] = Convert.ToString(reader["ITEM_NAME"]);
                        dataRow["Qty"] = Convert.ToString(reader["QTY"]);
                        dataRow["Weight"] = Convert.ToString(reader["WEIGHT1"]);
                        dataRow["Rate"] = Convert.ToString(reader["RATE"]);
                        dataRow["RTType"] = Convert.ToString(reader["RT_TYPE"]);
                        dataRow["Amount"] = reader["AMT"] == DBNull.Value ? 0 : _commonService.ToAccountingFormat(Convert.ToDecimal(reader["AMT"]));
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