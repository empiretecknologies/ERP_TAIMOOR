using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text.Json.Nodes;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class FSodaBookFeedingRepository : IFSodaBookFeedingRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public IPartyRepository _partyRepository { get; set; }
        public FSodaBookFeedingRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, IPartyRepository partyRepository, ICommonRepository commonRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _partyRepository = partyRepository;
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
                        string query = "SELECT D.DT_CODE AS TRAN_ID, A.TRAN_ID AS CODE, A.V_DATE, MU.GROUP_NAME AS MASTERUNIT, A.VOUCHER_NO, " +
                                       "S.PARTY_NAME AS SELLER_CODE, A.SACT_CODE, B.PARTY_NAME AS BUYER_CODE, A.BACT_CODE, " +
                                       "A.REF, A.REMARKS, CASE WHEN A.COND = 'Adv' THEN 'Advance' WHEN A.COND = 'Cash' THEN 'Cash' " +
                                       "WHEN A.COND = 'Cr' THEN 'Credit Days' WHEN A.COND = 'CrD' THEN 'Credit Date' ELSE '' END AS COND, " +
                                       "BR.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE, " +
                                       "A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, " +
                                       "A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, " +
                                       "A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, CREDIT_DAYS, DUE_DATE, " +
                                       "IM.ITEM_NAME AS ITEM_CODE, D.QTY, U.GROUP_NAME AS UNIT, D.QTY2, D.BAL_QTY, D.RATE, " +
                                       "RTRIM(LTRIM(D.RT_TYPE)) + ' kg' as RT_TYPE, D.AMT, D.DT_DESC,CH.ACT_NAME AS COA " +
                                       $"FROM {table} A " +
                                       "LEFT OUTER JOIN TBL_CHART CH ON CH.ACT_CODE = A.COA  " +
                                       "LEFT OUTER JOIN TBL_PARTY_TYPES S ON A.SELLER_CODE = S.PARTY_CODE AND S.ACT_CODE = A.SACT_CODE " +
                                       "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BUYER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BACT_CODE " +
                                       "LEFT OUTER JOIN TBL_PARTY_TYPES BR ON A.BROKER_CODE = BR.PARTY_CODE AND BR.ACT_CODE = A.BD_ACT_CODE " +
                                       $"LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}' " +
                                       "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON D.ITEM_CODE = IM.ITEM_CODE " +
                                       "LEFT OUTER JOIN TBL_UNIT U ON D.UNIT = U.GROUP_CODE " +
                                       "LEFT OUTER JOIN TBL_UNIT MU ON A.UNIT = MU.GROUP_CODE " +
                                       $"WHERE D.DT_CODE is not null AND A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' " +
                                       $"ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                CODE = Convert.ToInt32(reader["CODE"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                SELLER_CODE = Convert.ToString(reader["SELLER_CODE"]),
                                SACT_CODE = Convert.ToString(reader["SACT_CODE"]),
                                BUYER_CODE = Convert.ToString(reader["BUYER_CODE"]),
                                BACT_CODE = Convert.ToString(reader["BACT_CODE"]),
                                COA = Convert.ToString(reader["COA"]),

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
                                CREDIT_DAYS = Convert.ToString(reader["CREDIT_DAYS"]),
                                DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),
                                //DETAIL = GetSodaBookDetails(Convert.ToString(reader["TRAN_ID"]), Menu),
                                ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                MASTERUNIT = Convert.ToString(reader["MASTERUNIT"]),
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
        //                        .Replace("sacT_CODE", "A.SACT_CODE")
        //                        .Replace("remarks", "A.REMARKS")
        //                        .Replace("qty", "D.QTY");
        //                }

        //                string query = "SELECT COUNT(*) OVER() AS COUNT, A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, " +
        //                               "S.PARTY_NAME AS SELLER_CODE, A.SACT_CODE, B.PARTY_NAME AS BUYER_CODE, A.BACT_CODE, " +
        //                               "A.REF, A.REMARKS, CASE WHEN A.COND = 'Adv' THEN 'Advance' WHEN A.COND = 'Cash' THEN 'Cash' " +
        //                               "WHEN A.COND = 'Cr' THEN 'Credit Days' WHEN A.COND = 'CrD' THEN 'Credit Date' ELSE '' END AS COND, " +
        //                               "BR.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE, " +
        //                               "A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, " +
        //                               "A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, " +
        //                               "A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, CREDIT_DAYS, DUE_DATE, " +
        //                               "IM.ITEM_NAME AS ITEM_CODE, D.QTY, U.GROUP_NAME AS UNIT, D.QTY2, D.BAL_QTY, D.RATE, " +
        //                               "RTRIM(LTRIM(D.RT_TYPE)) + ' kg' as RT_TYPE, D.AMT, D.DT_DESC " +
        //                               $"FROM {table} A " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES S ON A.SELLER_CODE = S.PARTY_CODE AND S.ACT_CODE = A.SACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BUYER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES BR ON A.BROKER_CODE = BR.PARTY_CODE AND BR.ACT_CODE = A.BD_ACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_SBF_DETAIL D ON A.TRAN_ID = D.TRAN_ID " +
        //                               "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON D.ITEM_CODE = IM.ITEM_CODE " +
        //                               "LEFT OUTER JOIN TBL_UNIT U ON D.UNIT = U.GROUP_CODE " +
        //                               $"WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' {filterCondition} " +
        //                               "ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC";

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
        //                    query = "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, " +
        //                               "S.PARTY_NAME AS SELLER_CODE, A.SACT_CODE, B.PARTY_NAME AS BUYER_CODE, A.BACT_CODE, " +
        //                               "A.REF, A.REMARKS, CASE WHEN A.COND = 'Adv' THEN 'Advance' WHEN A.COND = 'Cash' THEN 'Cash' " +
        //                               "WHEN A.COND = 'Cr' THEN 'Credit Days' WHEN A.COND = 'CrD' THEN 'Credit Date' ELSE '' END AS COND, " +
        //                               "BR.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE, " +
        //                               "A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, " +
        //                               "A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, " +
        //                               "A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, CREDIT_DAYS, DUE_DATE, " +
        //                               "IM.ITEM_NAME AS ITEM_CODE, D.QTY, U.GROUP_NAME AS UNIT, D.QTY2, D.BAL_QTY, D.RATE, " +
        //                               "RTRIM(LTRIM(D.RT_TYPE)) + ' kg' as RT_TYPE, D.AMT, D.DT_DESC " +
        //                               $"FROM {table} A " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES S ON A.SELLER_CODE = S.PARTY_CODE AND S.ACT_CODE = A.SACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BUYER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES BR ON A.BROKER_CODE = BR.PARTY_CODE AND BR.ACT_CODE = A.BD_ACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_SBF_DETAIL D ON A.TRAN_ID = D.TRAN_ID " +
        //                               "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON D.ITEM_CODE = IM.ITEM_CODE " +
        //                               "LEFT OUTER JOIN TBL_UNIT U ON D.UNIT = U.GROUP_CODE " +
        //                               $"WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' {filterCondition} " +
        //                               $"ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        //                }else
        //                {
        //                    query = "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, " +
        //                               "S.PARTY_NAME AS SELLER_CODE, A.SACT_CODE, B.PARTY_NAME AS BUYER_CODE, A.BACT_CODE, " +
        //                               "A.REF, A.REMARKS, CASE WHEN A.COND = 'Adv' THEN 'Advance' WHEN A.COND = 'Cash' THEN 'Cash' " +
        //                               "WHEN A.COND = 'Cr' THEN 'Credit Days' WHEN A.COND = 'CrD' THEN 'Credit Date' ELSE '' END AS COND, " +
        //                               "BR.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE, " +
        //                               "A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, " +
        //                               "A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, " +
        //                               "A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, CREDIT_DAYS, DUE_DATE, " +
        //                               "IM.ITEM_NAME AS ITEM_CODE, D.QTY, U.GROUP_NAME AS UNIT, D.QTY2, D.BAL_QTY, D.RATE, " +
        //                               "RTRIM(LTRIM(D.RT_TYPE)) + ' kg' as RT_TYPE, D.AMT, D.DT_DESC " +
        //                               $"FROM {table} A " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES S ON A.SELLER_CODE = S.PARTY_CODE AND S.ACT_CODE = A.SACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BUYER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_PARTY_TYPES BR ON A.BROKER_CODE = BR.PARTY_CODE AND BR.ACT_CODE = A.BD_ACT_CODE " +
        //                               "LEFT OUTER JOIN TBL_SBF_DETAIL D ON A.TRAN_ID = D.TRAN_ID " +
        //                               "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON D.ITEM_CODE = IM.ITEM_CODE " +
        //                               "LEFT OUTER JOIN TBL_UNIT U ON D.UNIT = U.GROUP_CODE " +
        //                               $"WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' {filterCondition} " +
        //                               $"ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC";
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
        //                        CREDIT_DAYS = Convert.ToString(reader["CREDIT_DAYS"]),
        //                        DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),
        //                        //DETAIL = GetSodaBookDetails(Convert.ToString(reader["TRAN_ID"]), Menu),
        //                        ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
        //                        QTY = Convert.ToString(reader["QTY"]),
        //                        UNIT = Convert.ToString(reader["UNIT"]),
        //                        QTY2 = Convert.ToString(reader["QTY2"]),
        //                        BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
        //                        RATE = Convert.ToString(reader["RATE"]),
        //                        RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
        //                        AMT = Convert.ToString(reader["AMT"]),
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

        public MyHttpResponseMessage Save(CustomFSodaBookFeeding modelRecord, Common common)
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

                List<CustomPartyType> partiesData = DropdownService.CustomPartyTypeDropdownWithAccountCode(common.RoleID, common.RoleType);

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
                            //var buyerInformation = partiesData.ToList().Where(p => p.customizedKey == modelRecord.Master.BUYER_CODE).FirstOrDefault();

                            if (sellerInformation != null)
                            {
                                modelRecord.Master.SELLER_CODE = Convert.ToString(sellerInformation.key);
                                modelRecord.Master.SACT_CODE = sellerInformation.accountCode;
                            }

                            //if (buyerInformation != null)
                            //{
                            //    modelRecord.Master.BUYER_CODE = Convert.ToString(buyerInformation.key);
                            //    modelRecord.Master.BACT_CODE = buyerInformation.accountCode;
                            //}

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
                                        "(TRAN_ID, V_DATE, VOUCHER_NO, SELLER_CODE, " +
                                        "SACT_CODE, REF, " +
                                        "REMARKS, COND, BROKER_CODE, BD_ACT_CODE, " +
                                        "BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                        "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                        "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                        "ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, " +
                                        "DLT, CREDIT_DAYS, DUE_DATE, SBF_TYPE, CURR_CODE, UNIT, CRATE, SHIP_DATE, SHIP_STATUS, P_SHIP, TRANS_PS, SODA_TYPE, COB_CODE, COB_ACODE,COA)" +
                                        "VALUES" +
                                        "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.SELLER_CODE + "'," +
                                        "'" + modelRecord.Master.SACT_CODE + "','" + modelRecord.Master.REF + "'," +
                                        "'" + modelRecord.Master.REMARKS + "','" + modelRecord.Master.COND + "','" + modelRecord.Master.BROKER_CODE + "','" + modelRecord.Master.BD_ACT_CODE + "'," +
                                        "'" + branch + "','" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                        "'" + computer + "','" + ip + "','" + username + "'," +
                                        "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + computer + "','" + ip + "'," +
                                        "'" + postal + "','" + postal + "','" + modelRecord.Master.ASTATUS + "','" + menuID + "','T','" + modelRecord.Master.CREDIT_DAYS + "'," +
                                        "'" + modelRecord.Master.DUE_DATE + "','" + modelRecord.Master.SBF_TYPE + "','" + modelRecord.Master.CURR_CODE + "','" + modelRecord.Master.UNIT + "'," +
                                        "'" + modelRecord.Master.CRATE + "','" + modelRecord.Master.SHIP_DATE + "','" + modelRecord.Master.SHIP_STATUS + "','" + modelRecord.Master.P_SHIP + "','" + modelRecord.Master.TRANS_PS + "'," +
                                        "'" + modelRecord.Master.SODA_TYPE + "','" + modelRecord.Master.COB_CODE + "','" + modelRecord.Master.COB_ACODE + "', '" + modelRecord.Master.COA + "')";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                query = $"UPDATE {table} SET " +
                                        $"V_DATE = '{modelRecord.Master.V_DATE}', " +
                                        $"SELLER_CODE = '{modelRecord.Master.SELLER_CODE}', " +
                                        $"SACT_CODE = '{modelRecord.Master.SACT_CODE}', " +
                                        $"REF = '{modelRecord.Master.REF}', " +
                                        $"REMARKS = '{modelRecord.Master.REMARKS}', " +
                                        $"COND = '{modelRecord.Master.COND}', " +
                                        $"BROKER_CODE = '{modelRecord.Master.BROKER_CODE}', " +
                                        $"BD_ACT_CODE = '{modelRecord.Master.BD_ACT_CODE}', " +
                                        $"COB_CODE = '{modelRecord.Master.COB_CODE}', " +
                                        $"COA = '{modelRecord.Master.COA}', " +

                                        $"COB_ACODE = '{modelRecord.Master.COB_ACODE}', " +
                                        $"EDIT_USER_ID = '{username}', " +
                                        $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                        $"EDIT_COMPUTER_NAME = '{computer}', " +
                                        $"EDIT_IP_ADDRESS = '{ip}', " +
                                        $"EDIT_POSTALCODE = '{postal}', " +
                                        $"ASTATUS = '{modelRecord.Master.ASTATUS}', " +
                                        $"CREDIT_DAYS = '{modelRecord.Master.CREDIT_DAYS}', " +
                                        $"DUE_DATE = '{modelRecord.Master.DUE_DATE}', " +
                                        $"SBF_TYPE = '{modelRecord.Master.SBF_TYPE}', " +
                                        $"CURR_CODE = '{modelRecord.Master.CURR_CODE}', " +
                                        $"UNIT = '{modelRecord.Master.UNIT}', " +
                                        $"CRATE = '{modelRecord.Master.CRATE}', " +
                                        $"SHIP_STATUS = '{modelRecord.Master.SHIP_STATUS}', " +
                                        $"P_SHIP = '{modelRecord.Master.P_SHIP}', " +
                                        $"TRANS_PS = '{modelRecord.Master.TRANS_PS}', " +
                                        $"SODA_TYPE = '{modelRecord.Master.SODA_TYPE}', " +
                                        $"SHIP_DATE = '{modelRecord.Master.SHIP_DATE}' " +
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
                                                           "(TRAN_ID, DT_CODE, ITEM_CODE, QTY, " +
                                                           "UNIT, QTY2, BAL_QTY, RATE, " +
                                                           "AMT, DT_DESC, BCODE, " +
                                                           "PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                                           "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                                           "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                                           "ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT, CHK, RT_TYPE, SCOMP)" +
                                                           "VALUES" +
                                                           "('" + modelRecord.Master.TRAN_ID + "','" + detailCode + "','" + item.ITEM_CODE + "','" + item.QTY + "'," +
                                                           "'" + item.UNIT + "','" + item.QTY2 + "','" + item.BAL_QTY + "','" + item.RATE + "'," +
                                                           "'" + item.AMT + "','" + item.DT_DESC + "','" + branch + "'," +
                                                           "'" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                                           "'" + computer + "','" + ip + "','" + username + "'," +
                                                           "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + computer + "','" + ip + "'," +
                                                           "'" + postal + "','" + postal + "','" + menuID + "','T','" + item.CHK + "','" + item.RT_TYPE + "',0)";
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
                                                      $"QTY = '{item.QTY}', " +
                                                      $"UNIT = '{item.UNIT}', " +
                                                      $"QTY2 = '{item.QTY2}', " +
                                                      $"BAL_QTY = '{item.BAL_QTY}', " +
                                                      $"RATE = '{item.RATE}', " +
                                                      $"AMT = '{item.AMT}', " +
                                                      $"DT_DESC = '{item.DT_DESC}', " +
                                                      $"EDIT_USER_ID = '{username}', " +
                                                      $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                      $"EDIT_COMPUTER_NAME = '{computer}', " +
                                                      $"EDIT_IP_ADDRESS = '{ip}', " +
                                                      $"EDIT_POSTALCODE = '{postal}', " +
                                                      $"RT_TYPE = '{item.RT_TYPE}', " +
                                                      $"CHK = '{item.CHK}', " +
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

        public MyHttpResponseMessage GetSodaBookFeedingByCode(int code, Common common)
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
                        string query = "SELECT TRAN_ID, P_S, P_SHIP, TRANS_PS, SODA_TYPE, V_DATE, VOUCHER_NO," +
                                       "SELLER_CODE, SACT_CODE, BUYER_CODE, BACT_CODE, REF, REMARKS, COND, BROKER_CODE, BD_ACT_CODE, ASTATUS, CREDIT_DAYS, DUE_DATE, SBF_TYPE, CURR_CODE, UNIT, CRATE, SHIP_DATE, SHIP_STATUS,COA " +
                                       $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
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
                                SELLER_CODE = $"{Convert.ToString(reader["SELLER_CODE"])}{Convert.ToString(reader["SACT_CODE"])}",
                                BUYER_CODE = $"{Convert.ToString(reader["BUYER_CODE"])}{Convert.ToString(reader["BACT_CODE"])}",
                                BROKER_CODE = $"{Convert.ToString(reader["BROKER_CODE"])}{Convert.ToString(reader["BD_ACT_CODE"])}",
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                SODA_TYPE = Convert.ToString(reader["SODA_TYPE"]),
                                P_S = Convert.ToString(reader["P_S"]),
                                P_SHIP = Convert.ToString(reader["P_SHIP"]),
                                TRANS_PS = Convert.ToString(reader["TRANS_PS"]),
                                COND = Convert.ToString(reader["COND"]),
                                CREDIT_DAYS = Convert.ToString(reader["CREDIT_DAYS"]),
                                DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),
                                SBF_TYPE = Convert.ToString(reader["SBF_TYPE"]),
                                CURR_CODE = reader["CURR_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CURR_CODE"]),
                                UNIT = reader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UNIT"]),
                                CRATE = reader["CRATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CRATE"]),
                                SHIP_STATUS = reader["SHIP_STATUS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SHIP_STATUS"]),
                                COA = reader["COA"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COA"]),
                                SHIP_DATE = reader["SHIP_DATE"] == DBNull.Value ? null : Convert.ToInt32(reader["SHIP_STATUS"]) == 1 ? Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM") : Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM-dd"),
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

        public MyHttpResponseMessage GetSodaBookFeedingDetailByCode(int code, Common common)
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
                        //string query = "SELECT TRAN_ID,DT_CODE, ITEM_CODE, QTY," +
                        //               "UNIT, QTY2, BAL_QTY, RATE, AMT, DT_DESC, CHK, RT_TYPE " +
                        //               $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                        //               $"AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";

                        string query = $@"SELECT SBF.TRAN_ID, SBF.DT_CODE, SBF.ITEM_CODE, SBF.QTY, SBF.UNIT, SBF.QTY2, SBF.BAL_QTY, SBF.RATE, SBF.AMT, SBF.DT_DESC, SBF.CHK, SBF.RT_TYPE,
									CASE WHEN (SELECT SUM(D.AMOUNT) FROM TBL_CC_DETAIL D WHERE D.PTRAN_ID = SBF.TRAN_ID AND D.PICK_ID = SBF.DT_CODE AND D.DLT='T') IS NULL THEN 0
                                    WHEN ISNULL((SELECT SUM(D.AMOUNT) FROM TBL_CC_DETAIL D WHERE D.PTRAN_ID = SBF.TRAN_ID AND D.PICK_ID = SBF.DT_CODE AND D.DLT='T'),0) >= ISNULL(SBF.AMT,0) THEN 0
                                    ELSE 1 END AS COST_CENTER_STATUS
									FROM {table} SBF WHERE SBF.DLT = 'T' AND SBF.TRAN_ID = '{code}' AND SBF.BCODE = '{common.Branch}' AND SBF.PERIOD_ID = '{common.Period}' ORDER BY SBF.DT_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToInt32(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                CHK = Convert.ToString(reader["CHK"]),
                                CHK1 = Convert.ToString(reader["CHK"]) == "1" ? true : false,
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
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
                        if (!IsSodaDeliveryAvailable(code, true, common))
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
                        else
                        {
                            response.data = "";
                            response.msg = "Delivery is available against this soda. please delete the delivery first.";
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

                FSodaBookFeeding sodaBookFeeding = new FSodaBookFeeding();
                List<FSodaBookFeedingDetail> sodaBookFeedingDetailList = new List<FSodaBookFeedingDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        sodaBookFeeding = new FSodaBookFeeding
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
                            BCODE = Convert.ToInt32(reader["BCODE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            CREDIT_DAYS = Convert.ToDouble(reader["CREDIT_DAYS"]),
                            DUE_DATE = Convert.ToDateTime(reader["DUE_DATE"]),
                            CURR_CODE = Convert.ToInt32(reader["CURR_CODE"]),
                            CRATE = Convert.ToDecimal(reader["CRATE"]),
                            SHIP_DATE = Convert.ToDateTime(reader["SHIP_DATE"]),
                            SBF_TYPE = Convert.ToString(reader["SBF_TYPE"]),
                            UNIT = Convert.ToInt32(reader["UNIT"]),
                            SHIP_STATUS = Convert.ToInt32(reader["SHIP_STATUS"]),
                            SODA_TYPE = Convert.ToString(reader["SODA_TYPE"]),
                            P_S = Convert.ToString(reader["P_S"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new FSodaBookFeedingDetail
                        {
                            ITEM_CODE = Convert.ToInt32(detail_Reader["ITEM_CODE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                            UNIT = Convert.ToInt32(detail_Reader["UNIT"]),
                            QTY2 = Convert.ToDouble(detail_Reader["QTY2"]),
                            BAL_QTY = Convert.ToDouble(detail_Reader["BAL_QTY"]),
                            RATE = Convert.ToDouble(detail_Reader["RATE"]),
                            AMT = Convert.ToDouble(detail_Reader["AMT"]),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            CHK = detail_Reader["CHK"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["CHK"]),
                            RT_TYPE = Convert.ToDouble(detail_Reader["RT_TYPE"]),
                        };
                        sodaBookFeedingDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customRequisition = new CustomFSodaBookFeeding
                {
                    Master = sodaBookFeeding,
                    Detail = sodaBookFeedingDetailList
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

        public MyHttpResponseMessage DeleteSodaBookFeedingDetailByCode(int code, Common common)
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
                        if (!IsSodaDeliveryAvailable(code, false, common))
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
                        else
                        {
                            response.data = "";
                            response.msg = "Delivery is available against this soda. please delete the delivery first.";
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

        public bool IsSodaDeliveryAvailable(int code, bool IsMaster, Common common)
        {
            bool IsAvailable = false;
            try
            {
                var branch = common.Branch;
                var period = common.Period;
                if (IsMaster)
                {
                    string connectionString = new SQLService().getconnstring();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = $"SELECT 1 FROM TBL_DF_DETAIL " +
                                       $"WHERE PICK_ID IN (SELECT DT_CODE FROM TBL_SBF_DETAIL WHERE TRAN_ID = '{code}' AND DLT = 'T') " +
                                       $"AND DLT = 'T' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            IsAvailable = true;
                        }
                    }
                }
                else
                {
                    string connectionString = new SQLService().getconnstring();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = $"SELECT 1 FROM TBL_DF_DETAIL WHERE PICK_ID = '{code}' AND DLT = 'T' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            IsAvailable = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsAvailable = true;
            }
            return IsAvailable;
        }

        private List<object> GetSodaBookDetails(string tranID, MyHttpResponseMessage menuData)
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
                                       "U.GROUP_NAME AS UNIT, QTY2, BAL_QTY, RATE, RTRIM(LTRIM(RT_TYPE)) + ' kg' as RT_TYPE, AMT, DT_DESC " +
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
    }
}