using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class DeliveryFormatRepository : IDeliveryFormatRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public DeliveryFormatRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonRepository commonRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
        }

        public MyHttpResponseMessage GetDeliveryFormats(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<DeliveryFormat> chartOfAccounts = new List<DeliveryFormat>();
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
                            DeliveryFormat chartOfAccount = new DeliveryFormat();
                            chartOfAccount.ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]);
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
                List<DeliveryFormat> chartOfAccounts = new List<DeliveryFormat>();
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
                            DeliveryFormat chartOfAccount = new DeliveryFormat();
                            string actName = Convert.ToString(reader["ACT_NAME"]);
                            int actCode = Convert.ToInt32(reader["ACT_CODE"]);
                            int actParentCode = Convert.ToInt32(reader["ACT_PARENT_CODE"]);

                            //chartOfAccount.ACT_NAME = actName;
                            chartOfAccount.ACT_CODE = actCode;
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
                        string query = @"SELECT COUNT(*) OVER() AS COUNT, A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, P.PARTY_NAME AS PARTY_CODE, A.ACT_CODE, A.S_DATE, A.S_NO, B.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE, A.GODOWN," +
                            " A.KANTA, A.COND, A.C_NAME, A.CELL, A.LOT_NO, A.ORIGIN, IM.ITEM_NAME AS ITEM_CODE, A.TBAG, A.BCODE, A.PERIOD_ID, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, A.MENU_ID, A.DLT " +
                            " FROM " + table + " A " +
                            "LEFT OUTER JOIN TBL_PARTY_TYPES P ON A.PARTY_CODE = P.PARTY_CODE AND P.ACT_CODE = A.ACT_CODE " +
                            "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BROKER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BD_ACT_CODE " +
                            "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE " +
                            "WHERE A.DLT = 'T' " +
                            "ORDER BY TRAN_ID DESC";

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
                                ACT_CODE = reader["ACT_CODE"].ToString(),
                                S_DATE = reader["S_DATE"].ToString(),
                                S_NO = reader["S_NO"].ToString(),
                                BROKER_CODE = reader["BROKER_CODE"].ToString(),
                                BD_ACT_CODE = reader["BD_ACT_CODE"].ToString(),
                                GODOWN = reader["GODOWN"].ToString(),
                                KANTA = reader["KANTA"].ToString(),
                                COND = reader["COND"].ToString(),
                                C_NAME = reader["C_NAME"].ToString(),
                                CELL = reader["CELL"].ToString(),
                                LOT_NO = reader["LOT_NO"].ToString(),
                                ORIGIN = reader["ORIGIN"].ToString(),
                                ITEM_CODE = reader["ITEM_CODE"].ToString(),
                                TBAG = reader["TBAG"].ToString(),
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

        //public MyHttpResponseMessage QuickSearch(Core.Entities.Common common, int skip, int take, string filter = null, string group = null)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
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
        //                    if (jsonArray.Count >= 15)
        //                    {
        //                        jsonArray.RemoveAt(1);
        //                        jsonArray.RemoveAt(0);
        //                    }
        //                    filterCondition = _commonRepository.BuildFilterCondition(jsonArray);
        //                    filterCondition = filterCondition
        //                        .Replace("iteM_CODE", "IM.ITEM_NAME")
        //                        .Replace("astatus", "A.ASTATUS")
        //                        .Replace("partY_CODE", "P.PARTY_NAME")
        //                        .Replace("cell", "A.CELL")
        //                        .Replace("A.ASTATUS LIKE '%Active%'", "A.ASTATUS LIKE '%Y%'")
        //                        .Replace("A.ASTATUS LIKE '%In-Active%'", "A.ASTATUS LIKE '%N%'");
        //                }

        //                string query = "";

        //                if (string.IsNullOrEmpty(group))
        //                {
        //                    query = @"SELECT COUNT(*) OVER() AS COUNT, A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, P.PARTY_NAME AS PARTY_CODE, A.ACT_CODE, A.S_DATE, A.S_NO, B.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE, A.GODOWN," +
        //                    " A.KANTA, A.COND, A.C_NAME, A.CELL, A.LOT_NO, A.ORIGIN, IM.ITEM_NAME AS ITEM_CODE, A.TBAG, A.BCODE, A.PERIOD_ID, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, A.MENU_ID, A.DLT " +
        //                    " FROM " + table + " A " +
        //                    "LEFT OUTER JOIN TBL_PARTY_TYPES P ON A.PARTY_CODE = P.PARTY_CODE AND P.ACT_CODE = A.ACT_CODE " +
        //                    "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BROKER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BD_ACT_CODE " +
        //                    "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE " +
        //                    "WHERE A.DLT = 'T' " + filterCondition + " " +
        //                    "ORDER BY TRAN_ID OFFSET " + skip + " ROWS FETCH NEXT " + take + " ROWS ONLY";
        //                }
        //                else
        //                {
        //                    query = @"SELECT COUNT(*) OVER() AS COUNT, A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, P.PARTY_NAME AS PARTY_CODE, A.ACT_CODE, A.S_DATE, A.S_NO, B.PARTY_NAME AS BROKER_CODE, A.BD_ACT_CODE, A.GODOWN," +
        //                    " A.KANTA, A.COND, A.C_NAME, A.CELL, A.LOT_NO, A.ORIGIN, IM.ITEM_NAME AS ITEM_CODE, A.TBAG, A.BCODE, A.PERIOD_ID, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, A.MENU_ID, A.DLT " +
        //                    " FROM " + table + " A " +
        //                    "LEFT OUTER JOIN TBL_PARTY_TYPES P ON A.PARTY_CODE = P.PARTY_CODE AND P.ACT_CODE = A.ACT_CODE " +
        //                    "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BROKER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BD_ACT_CODE " +
        //                    "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE " +
        //                    "WHERE A.DLT = 'T' " + filterCondition + " " +
        //                    "ORDER BY TRAN_ID";
        //                }
        //                SqlCommand countCommand = new SqlCommand(query, connection);
        //                connection.Open();
        //                SqlDataReader readerCommand = countCommand.ExecuteReader();
        //                if (readerCommand.Read())
        //                {
        //                    totalCount = readerCommand.GetInt32(readerCommand.GetOrdinal("COUNT"));
        //                }
        //                readerCommand.Close();
        //                connection.Close();

        //                SqlCommand command = new SqlCommand(query, connection);
        //                connection.Open();
        //                SqlDataReader reader = command.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    var row = new
        //                    {
        //                        TRAN_ID = reader["TRAN_ID"].ToString(),
        //                        V_DATE = reader["V_DATE"].ToString(),
        //                        VOUCHER_NO = reader["VOUCHER_NO"].ToString(),
        //                        PARTY_CODE = reader["PARTY_CODE"].ToString(),
        //                        ACT_CODE = reader["ACT_CODE"].ToString(),
        //                        S_DATE = reader["S_DATE"].ToString(),
        //                        S_NO = reader["S_NO"].ToString(),
        //                        BROKER_CODE = reader["BROKER_CODE"].ToString(),
        //                        BD_ACT_CODE = reader["BD_ACT_CODE"].ToString(),
        //                        GODOWN = reader["GODOWN"].ToString(),
        //                        KANTA = reader["KANTA"].ToString(),
        //                        COND = reader["COND"].ToString(),
        //                        C_NAME = reader["C_NAME"].ToString(),
        //                        CELL = reader["CELL"].ToString(),
        //                        LOT_NO = reader["LOT_NO"].ToString(),
        //                        ORIGIN = reader["ORIGIN"].ToString(),
        //                        ITEM_CODE = reader["ITEM_CODE"].ToString(),
        //                        TBAG = reader["TBAG"].ToString(),
        //                        BCODE = reader["BCODE"].ToString(),
        //                        PERIOD_ID = reader["PERIOD_ID"].ToString(),
        //                        ASTATUS = reader["ASTATUS"].ToString(),
        //                        MENU_ID = reader["MENU_ID"].ToString()
        //                    };
        //                    jsonDataResult.Add(row);
        //                }
        //                reader.Close();

        //                if (!string.IsNullOrEmpty(group))
        //                {

        //                    JsonNode groupNode = JsonNode.Parse(group);
        //                    JsonArray groupArray = groupNode.AsArray();
        //                    var groupSelectors = groupArray.Select(g => g["selector"].ToString());
        //                    string groupByClause = string.Join("", groupSelectors.Select(selector => selector));

        //                    var groupedData = new List<object>();
        //                    var grouped = jsonDataResult.GroupBy(d => _commonRepository.GetPropertyValue(d, groupByClause.ToUpper())).Select(g => new
        //                    {
        //                        key = g.Key,
        //                        items = g.ToList()
        //                    });
        //                    groupedData.AddRange(grouped);
        //                    response.data = new
        //                    {
        //                        data = groupedData,
        //                        totalCount = totalCount,
        //                    };
        //                }
        //                else
        //                {
        //                    response.data = new
        //                    {
        //                        data = jsonDataResult,
        //                        totalCount = totalCount
        //                    };
        //                }
        //                response.msg = "";
        //                response.msgType = 1;
        //                return response;
        //            }
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

        public MyHttpResponseMessage Save(DeliveryFormat modelRecord, Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                int? pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    pType = menu.PTYPE;
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
                    List<CustomPartyType> partiesData = DropdownService.CustomPartyTypeDropdownWithAccountCodeAndPType(common.RoleID, common.RoleType, pType);
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
                            var partyInformation = partiesData.ToList().Where(p => p.customizedKey == modelRecord.PARTY_CODE).FirstOrDefault();
                            var brokerInformation = partiesData.ToList().Where(p => p.customizedKey == modelRecord.BROKER_CODE).FirstOrDefault();

                            if (partyInformation != null)
                            {
                                modelRecord.PARTY_CODE = Convert.ToString(partyInformation.key);
                                modelRecord.ACT_CODE = partyInformation.accountCode;
                            }

                            if (brokerInformation != null)
                            {
                                modelRecord.BROKER_CODE = Convert.ToString(brokerInformation.key);
                                modelRecord.BD_ACT_CODE = brokerInformation.accountCode;
                            }
                            if (modelRecord.TRAN_ID == 0 || modelRecord.TRAN_ID is null)
                            {

                                code = GenerateNextId(common, command);
                                modelRecord.TRAN_ID = code;
                                voucherNo = GenerateVoucherNo(common, code, CommonService.GetDateTime("Pakistan Standard Time"));

                                query = "INSERT INTO " + table + " " +
                                        "([TRAN_ID],[V_DATE],[VOUCHER_NO],[PARTY_CODE],[ACT_CODE],[S_DATE],[S_NO],[BROKER_CODE],[BD_ACT_CODE],[GODOWN]," +
                                        "[KANTA],[COND],[C_NAME],[CELL],[LOT_NO],[ORIGIN],[ITEM_CODE],[TBAG],[BCODE],[PERIOD_ID]," +
                                        "[ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[ADD_POSTALCODE]," +
                                        "[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME],[EDIT_IP_ADDRESS],[EDIT_POSTALCODE],[MENU_ID],[ASTATUS],[UNIT],[DLT])" +
                                        "VALUES " +
                                        "('" + GenerateNextId(common) + "','" + modelRecord.V_DATE + "','" + voucherNo + "','" + modelRecord.PARTY_CODE + "','" + modelRecord.ACT_CODE + "','" + modelRecord.S_DATE + "','" + modelRecord.S_NO + "','" + modelRecord.BROKER_CODE + "','" + modelRecord.BD_ACT_CODE + "','" + modelRecord.GODOWN + "'," +
                                        "'" + modelRecord.KANTA + "','" + modelRecord.COND + "','" + modelRecord.C_NAME + "','" + modelRecord.CELL + "','" + modelRecord.LOT_NO + "','" + modelRecord.ORIGIN + "','" + modelRecord.ITEM_CODE + "','" + modelRecord.TBAG + "','" + common.Branch + "','" + common.Period + "'," +
                                        "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + Postal + "'," +
                                        "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + Postal + "','" + common.MenuID + "','" + modelRecord.ASTATUS + "','" + modelRecord.UNIT + "','T')";

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
                                        " [V_DATE] = '" + modelRecord.V_DATE + @"',
                                        [PARTY_CODE] = '" + modelRecord.PARTY_CODE + @"',
                                        [ACT_CODE] = '" + modelRecord.ACT_CODE + @"',
                                        [S_DATE] = '" + modelRecord.S_DATE + @"',
                                        [S_NO] = '" + modelRecord.S_NO + @"',
                                        [BROKER_CODE] = '" + modelRecord.BROKER_CODE + @"',
                                        [BD_ACT_CODE] = '" + modelRecord.BD_ACT_CODE + @"',
                                        [GODOWN] = '" + modelRecord.GODOWN + @"',
                                        [KANTA] = '" + modelRecord.KANTA + @"',
                                        [COND] = '" + modelRecord.COND + @"',
                                        [C_NAME] = '" + modelRecord.C_NAME + @"',
                                        [CELL] = '" + modelRecord.CELL + @"',
                                        [LOT_NO] = '" + modelRecord.LOT_NO + @"',
                                        [ORIGIN] = '" + modelRecord.ORIGIN + @"',
                                        [ITEM_CODE] = '" + modelRecord.ITEM_CODE + @"',
                                        [TBAG] = '" + modelRecord.TBAG + @"',
		                                [EDIT_USER_ID] = '" + userid + @"',
		                                [EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
		                                [EDIT_COMPUTER_NAME] = '" + Computer + @"',
		                                [EDIT_IP_ADDRESS] = '" + Ip + @"',
		                                [MENU_ID] = '" + common.MenuID + @"',
		                                [EDIT_POSTALCODE] = '" + Postal + @"',
		                                [UNIT] = '" + modelRecord.UNIT + @"',
		                                [ASTATUS] = '" + modelRecord.ASTATUS + @"',
		                                [DLT] = 'T' 
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

        public MyHttpResponseMessage GetDeliveryFormatById(int id, Core.Entities.Common common)
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
                        string query = "SELECT [TRAN_ID],[V_DATE],[VOUCHER_NO],[PARTY_CODE],[ACT_CODE],[S_DATE],[S_NO],[BROKER_CODE],[BD_ACT_CODE],[GODOWN],[KANTA]," +
                            "[COND],[C_NAME],[CELL],[LOT_NO],[ORIGIN],[ITEM_CODE],[TBAG],[BCODE],[PERIOD_ID],[ADD_USER_ID],[ADD_DATE]," +
                            "[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME],[EDIT_IP_ADDRESS],[ADD_POSTALCODE],[EDIT_POSTALCODE]," +
                            "[ASTATUS],[MENU_ID],[UNIT],[DLT] FROM " + table + " WHERE TRAN_ID = @Id";

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
                                VOUCHER_NO = reader["VOUCHER_NO"],
                                PARTY_CODE = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",
                                ACT_CODE = reader["ACT_CODE"],
                                S_DATE = reader["S_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["S_DATE"]).ToString("yyyy-MM-dd"),
                                S_NO = reader["S_NO"],
                                BROKER_CODE = $"{Convert.ToString(reader["BROKER_CODE"])}{Convert.ToString(reader["BD_ACT_CODE"])}",
                                BD_ACT_CODE = reader["BD_ACT_CODE"],
                                GODOWN = reader["GODOWN"],
                                KANTA = reader["KANTA"],
                                COND = reader["COND"],
                                C_NAME = reader["C_NAME"],
                                CELL = reader["CELL"],
                                LOT_NO = reader["LOT_NO"],
                                ORIGIN = reader["ORIGIN"],
                                ITEM_CODE = reader["ITEM_CODE"],
                                TBAG = reader["TBAG"],
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
                                UNIT = reader["UNIT"],
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
                DeliveryFormat DeliveryFormat = new DeliveryFormat();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    //string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        DeliveryFormat = new DeliveryFormat
                        {
                            TRAN_ID = 0,
                            V_DATE = record.V_DATE,
                            PARTY_CODE = Convert.ToString(reader["PARTY_CODE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            S_DATE = Convert.ToDateTime(reader["S_DATE"]),
                            S_NO = Convert.ToString(reader["S_NO"]),
                            BROKER_CODE = Convert.ToString(reader["BROKER_CODE"]),
                            BD_ACT_CODE = Convert.ToInt32(reader["BD_ACT_CODE"]),
                            GODOWN = Convert.ToString(reader["GODOWN"]),
                            KANTA = Convert.ToString(reader["KANTA"]),
                            COND = Convert.ToString(reader["COND"]),
                            C_NAME = Convert.ToString(reader["C_NAME"]),
                            CELL = Convert.ToString(reader["CELL"]),
                            LOT_NO = Convert.ToString(reader["LOT_NO"]),
                            ORIGIN = Convert.ToString(reader["ORIGIN"]),
                            ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            TBAG = Convert.ToDouble(reader["TBAG"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            UNIT = Convert.ToInt32(reader["UNIT"]),
                        };
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(DeliveryFormat, common);
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

        public MyHttpResponseMessage GetDataForReport(DeliveryFormatReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            DeliveryFormatReport masterData = new DeliveryFormatReport();
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