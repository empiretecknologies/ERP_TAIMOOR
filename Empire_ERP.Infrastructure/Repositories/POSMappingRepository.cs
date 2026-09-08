using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class POSMappingRepository : IPOSMappingRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public POSMappingRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonRepository commonRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
        }

        public MyHttpResponseMessage GetPOSMappings(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<POSMapping> chartOfAccounts = new List<POSMapping>();
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
                            POSMapping chartOfAccount = new POSMapping();
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
        public MyHttpResponseMessage GetMapping(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            POSMapping mapping = new POSMapping();
            try
            {
                string query = "SELECT * FROM TBL_POS_MAP WHERE DLT = 'T' AND ASTATUS = 'Y' ";
                DataSet data = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);

                if (data != null && data.Tables.Count > 0)
                {
                    foreach (DataRow Row in data.Tables[0].Rows)
                    {
                        mapping.FB_LINK = Convert.ToString(Row["FB_LINK"]);
                        mapping.INSTA_LINK = Convert.ToString(Row["INSTA_LINK"]);
                        mapping.WEB_LINK = Convert.ToString(Row["WEB_LINK"]);
                        mapping.TIKTOK_LINK = Convert.ToString(Row["TIKTOK_LINK"]);
                        mapping.YOUTUBE_LINK = Convert.ToString(Row["YOUTUBE_LINK"]);
                        mapping.WIFI_PASSWORD = Convert.ToString(Row["WIFI_PASSWORD"]);
                        mapping.WIFI_NAME = Convert.ToString(Row["WIFI_NAME"]);
                    }
                }
                response.data = mapping;
                response.msg = "";
                response.msgType = 1;
            }
            catch (Exception ex)
            {
                response.data = mapping;
                response.msg = "";
                response.msgType = 2;
            }

            return response;
        }

        public MyHttpResponseMessage GetAccountsForTreeView(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<POSMapping> chartOfAccounts = new List<POSMapping>();
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
                            POSMapping chartOfAccount = new POSMapping();
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
                        string query = @"SELECT GROUP_CODE ,POS_PIRNT_LOGO, GROUP_NAME , CASH_ACT , CASH_TAX , BANK_ACT , BANK_TAX , BCHARGES , BCODE , PARTY_TAX , 
                              PAY_ACODE , SRB_NAME , SRB_NTN , POS_USER , POS_PASS , SRB_ID , SRB_STATUS , SRB_URL  , GROUP_IMAGE, ITEM_IMAGE, WAITER_IMAGE, TABLE_IMAGE,STICKER_LOGO " +
                            " FROM " + table + " A " +
                            "WHERE A.DLT = 'T' " +
                            "ORDER BY GROUP_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = reader["GROUP_CODE"].ToString(),
                                GROUPNAME = reader["GROUP_NAME"].ToString(),
                                SIMG = reader["STICKER_LOGO"].ToString(),
                                LOGO_IMG = reader["POS_PIRNT_LOGO"].ToString(),
                                CASHACT = reader["CASH_ACT"].ToString(),
                                CASHTAX = reader["CASH_TAX"].ToString(),
                                BANKACT = reader["BANK_ACT"].ToString(),
                                BANKTAX = reader["BANK_TAX"].ToString(),
                                BANKCHAR = reader["BCHARGES"].ToString(),
                                Branch = reader["BCODE"].ToString(),
                                PARTYTAX = reader["PARTY_TAX"].ToString(),
                                PARTYACT = reader["PAY_ACODE"].ToString(),
                                SRBNAME = reader["SRB_NAME"].ToString(),
                                SRBNTN = reader["SRB_NTN"].ToString(),
                                POSUSER = reader["POS_USER"].ToString(),
                                POSPASS = reader["POS_PASS"].ToString(),
                                SRBID = reader["SRB_ID"].ToString(),
                                SRBSTATUS = reader["SRB_STATUS"].ToString(),
                                SRBURL = reader["SRB_URL"].ToString(),
                                GROUPIMG = reader["GROUP_IMAGE"].ToString(),
                                ITEMIMG = reader["ITEM_IMAGE"].ToString(),
                                WAITERIMG = reader["WAITER_IMAGE"].ToString(),
                                TABLEIMG = reader["TABLE_IMAGE"].ToString(),
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(GROUP_CODE), 0) + 1 FROM {table}";
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

        public MyHttpResponseMessage Save(POSMapping modelRecord, Core.Entities.Common common)
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
                            if (modelRecord.TRAN_ID is 0 || modelRecord.TRAN_ID is null)
                            {

                                code = GenerateNextId(common, command);
                                modelRecord.TRAN_ID = code;

                                query = "INSERT INTO " + table + " " +
                                 "([GROUP_CODE],[GROUP_NAME], FB_LINK, INSTA_LINK, WEB_LINK, TIKTOK_LINK, YOUTUBE_LINK, WIFI_NAME, WIFI_PASSWORD, [CASH_ACT],[CASH_TAX],[BANK_ACT],[BCHARGES],[BANK_TAX],[TAX_STATUS],[BCODE],QR_CODE,P_WINDOW,STICKER_LOGO,LOCATION_SNAME,POS_PIRNT_LOGO,PAY_PRINTER," +
                                 "[ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[ADD_POSTALCODE]," +
                                 "[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME],[EDIT_IP_ADDRESS],[EDIT_POSTALCODE],[MENU_ID],[ASTATUS],[DLT]," +
                                 "[PARTY_TAX]," +
                                 "[R_CASH],[R_ADVANCE],[R_CARD],[R_PARTY],[R_SPLIT],[SRB_NAME],[SRB_NTN],[POS_USER],[POS_PASS],[SRB_ID],[SRB_STATUS],[SRB_URL]," +
                                 "[GROUP_IMAGE],[ITEM_IMAGE],[TABLE_IMAGE],[WAITER_IMAGE] , [ADVANCE_BTN], [KOT_BTN] , [WHATSAPP_URL] , [WHATSAPP_TOKEN] , [WHATSAPP_MSG] , [WHATSAPP_CC_MSG] , [WHT_MSG_ADV] , [WHT_MSG_RETURN] , " +
                                 "[WHT_ADV_COM], [WHT_PARTY_MSG] , [SER_CHARGES], [SALESMAN_REQ],PAY_COPIES,PRE_COPIES,DUP_COPIES,PAY_MODAL,PRE_MODAL,DUP_MODAL)" +
                                 "VALUES " +
                                 "('" + code + "','" + modelRecord.GROUP_NAME + "','" + modelRecord.FB_LINK + "','" + modelRecord.INSTA_LINK + "','" + modelRecord.WEB_LINK + "','" + modelRecord.TIKTOK_LINK + "','" + modelRecord.YOUTUBE_LINK + "','" + modelRecord.WIFI_NAME + "','" + modelRecord.WIFI_PASSWORD + "','" + modelRecord.CASH_ACCOUNT + "','" + modelRecord.CASH_TAX + "','" + modelRecord.BANK_ACCOUNT + "','" + modelRecord.BANK_CHARGES + "','" + modelRecord.BANK_TAX + "','" + " " + "','" + modelRecord.BRANCH + "','" + modelRecord.QR_CODE + "','" + modelRecord.P_WINDOW + "','" + modelRecord.S_IMG + "','" + modelRecord.LOC_SNAME + "','" + modelRecord.LOGO_IMG + "','" + modelRecord.P_PRINTER + "'," +
                                 "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + Postal + "'," +
                                 "'" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + Postal + "','" + common.MenuID + "','" + "Y" + "','T'," +
                                 "'" + modelRecord.PARTY_TAX + "'," +
                                 "'" + modelRecord.cashcheck + "','" + modelRecord.advancecheck + "','" + modelRecord.bankcheck + "','" + modelRecord.partycheck + "','" + modelRecord.splitcheck + "','" + modelRecord.SRB_NAME + "','" + modelRecord.SRB_NTN + "','" + modelRecord.POS_USER + "','" + modelRecord.POS_PASS + "','" + modelRecord.SRB_ID + "','" + modelRecord.srbcheck + "','" + modelRecord.SRB_URL + "'," +
                                 "'" + modelRecord.Group_IMG + "','" + modelRecord.Item_IMG + "','" + modelRecord.Table_IMG + "','" + modelRecord.Waiter_IMG + "','" + modelRecord.advancebtn + "','" + modelRecord.kotbtn + "','" + modelRecord.WHATSAPP_URL + "','" + modelRecord.WHATSAPP_TOKEN + "','" + modelRecord.WHATSAPP_MSG + "','" + modelRecord.WHATSAPP_CC + "','" + modelRecord.WHATSAPP_ADV + "','" + modelRecord.WHATSAPP_RTN + "','" + modelRecord.WHT_ADV_COM + "','" + modelRecord.WHT_PARTY_MSG + "','" + modelRecord.SER_CHARGES + "'," +
                                 "'" + modelRecord.salesmanReq + "','" + modelRecord.PAY_COPIES + "','" + modelRecord.PRE_COPIES + "','" + modelRecord.DUP_COPIES + "','" + modelRecord.PAY_MODAL + "','" + modelRecord.PRE_MODAL + "','" + modelRecord.DUP_MODAL + "')";

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
                                 "[GROUP_NAME] = '" + modelRecord.GROUP_NAME + "', " +
                                 "[CASH_ACT] = '" + modelRecord.CASH_ACCOUNT + "', " +
                                 "[CASH_TAX] = '" + modelRecord.CASH_TAX + "', " +
                                 "[BANK_ACT] = '" + modelRecord.BANK_ACCOUNT + "', " +
                                 "[BCHARGES] = '" + modelRecord.BANK_CHARGES + "', " +
                                 "[BANK_TAX] = '" + modelRecord.BANK_TAX + "', " +
                                 "[TAX_STATUS] = '', " +
                                 "[BCODE] = '" + modelRecord.BRANCH + "', " +
                                 "[QR_CODE] = '" + modelRecord.QR_CODE + "', " +
                                 "[P_WINDOW] = '" + modelRecord.P_WINDOW + "', " +
                                 "[LOCATION_SNAME] = '" + modelRecord.LOC_SNAME + "', " +
                                 "[STICKER_LOGO] = '" + modelRecord.S_IMG + "', " +
                                 "[POS_PIRNT_LOGO] = '" + modelRecord.LOGO_IMG + "', " +
                                 "[EDIT_USER_ID] = '" + userid + "', " +
                                 "[EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + "', " +
                                 "[EDIT_COMPUTER_NAME] = '" + Computer + "', " +
                                 "[EDIT_IP_ADDRESS] = '" + Ip + "', " +
                                 "[EDIT_POSTALCODE] = '" + Postal + "', " +
                                 "[MENU_ID] = '" + common.MenuID + "', " +
                                 "[ASTATUS] = 'Y', " +
                                 "[DLT] = 'T', " +
                                 "[PARTY_TAX] = '" + modelRecord.PARTY_TAX + "', " +
                                 "[R_CASH] = '" + modelRecord.cashcheck + "', " +
                                 "[PAY_PRINTER] = '" + modelRecord.P_PRINTER + "', " +
                                 "[R_ADVANCE] = '" + modelRecord.advancecheck + "', " +
                                 "[R_CARD] = '" + modelRecord.bankcheck + "', " +
                                 "[R_PARTY] = '" + modelRecord.partycheck + "', " +
                                 "[R_SPLIT] = '" + modelRecord.splitcheck + "', " +
                                 "[SRB_NAME] = '" + modelRecord.SRB_NAME + "', " +
                                 "[SRB_NTN] = '" + modelRecord.SRB_NTN + "', " +
                                 "[POS_USER] = '" + modelRecord.POS_USER + "', " +
                                 "[POS_PASS] = '" + modelRecord.POS_PASS + "', " +
                                 "[SRB_ID] = '" + modelRecord.SRB_ID + "', " +
                                 "[SRB_STATUS] = '" + modelRecord.srbcheck + "', " +
                                 "[SRB_URL] = '" + modelRecord.SRB_URL + "', " +
                                 "[GROUP_IMAGE] = '" + modelRecord.Group_IMG + "', " +
                                 "[FB_LINK] = '" + modelRecord.FB_LINK + "', " +
                                 "[INSTA_LINK] = '" + modelRecord.INSTA_LINK + "', " +
                                 "[WEB_LINK] = '" + modelRecord.WEB_LINK + "', " +
                                 "[TIKTOK_LINK] = '" + modelRecord.TIKTOK_LINK + "', " +
                                 "[YOUTUBE_LINK] = '" + modelRecord.YOUTUBE_LINK + "', " +
                                 "[WIFI_NAME] = '" + modelRecord.WIFI_NAME + "', " +
                                 "[WIFI_PASSWORD] = '" + modelRecord.WIFI_PASSWORD + "', " +
                                 "[ITEM_IMAGE] = '" + modelRecord.Item_IMG + "', " +
                                 "[TABLE_IMAGE] = '" + modelRecord.Table_IMG + "', " +
                                 "[ADVANCE_BTN] = '" + modelRecord.advancebtn + "', " +
                                 "[KOT_BTN] = '" + modelRecord.kotbtn + "', " +
                                 "[WAITER_IMAGE] = '" + modelRecord.Waiter_IMG + "', " +
                                 "[WHATSAPP_URL] = '" + modelRecord.WHATSAPP_URL + "', " +
                                 "[WHATSAPP_TOKEN] = '" + modelRecord.WHATSAPP_TOKEN + "', " +
                                 "[WHATSAPP_MSG] = '" + modelRecord.WHATSAPP_MSG + "', " +
                                 "[WHATSAPP_CC_MSG] = '" + modelRecord.WHATSAPP_CC + "', " +
                                 "[WHT_MSG_ADV] = '" + modelRecord.WHATSAPP_ADV + "', " +
                                 "[WHT_MSG_RETURN] = '" + modelRecord.WHATSAPP_RTN + "', " +
                                 "[WHT_ADV_COM] = '" + modelRecord.WHT_ADV_COM + "', " +
                                 "[WHT_PARTY_MSG] = '" + modelRecord.WHT_PARTY_MSG + "', " +
                                 "[SER_CHARGES] = '" + modelRecord.SER_CHARGES + "', " +
                                 "[SALESMAN_REQ] = '" + modelRecord.salesmanReq + "', " +
                                 "[PAY_COPIES] = '" + modelRecord.PAY_COPIES + "', " +
                                  "[PRE_COPIES] = '" + modelRecord.PRE_COPIES + "', " +
                                  "[DUP_COPIES] = '" + modelRecord.DUP_COPIES + "', " +
                                  "[PAY_MODAL] = '" + modelRecord.PAY_MODAL + "', " +
                                  "[PRE_MODAL] = '" + modelRecord.PRE_MODAL + "', " +
                                  "[DUP_MODAL] = '" + modelRecord.DUP_MODAL + "' " +
                                 "WHERE [GROUP_CODE] = '" + modelRecord.TRAN_ID + "'";

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

        public MyHttpResponseMessage GetPOSMappingById(int id, Core.Entities.Common common)
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
                        string query = @"SELECT  PAY_PRINTER,POS_PIRNT_LOGO, STICKER_LOGO,LOCATION_SNAME,QR_CODE,P_WINDOW, GROUP_CODE , GROUP_NAME, FB_LINK, INSTA_LINK, WEB_LINK, TIKTOK_LINK, YOUTUBE_LINK, WIFI_NAME, WIFI_PASSWORD , CASH_ACT , 
                                         CASH_TAX , BANK_ACT , BANK_TAX , BCHARGES , BCODE , PARTY_TAX , SRB_NAME , SRB_NTN , POS_USER , POS_PASS , SRB_ID , SRB_STATUS , 
                                         SRB_URL  , GROUP_IMAGE, ITEM_IMAGE, WAITER_IMAGE, TABLE_IMAGE , R_CASH , R_CARD , R_PARTY , R_ADVANCE, R_SPLIT , ADVANCE_BTN, KOT_BTN , 
                                         WHATSAPP_URL , WHATSAPP_TOKEN , WHATSAPP_MSG , WHATSAPP_CC_MSG , WHT_MSG_ADV , WHT_MSG_RETURN, WHT_ADV_COM , WHT_PARTY_MSG, SER_CHARGES,SALESMAN_REQ,
                                         PAY_COPIES, PRE_COPIES, DUP_COPIES, PAY_MODAL, PRE_MODAL, DUP_MODAL" +
                                        " FROM " + table + " A " +
                                       $"WHERE A.DLT = 'T' AND GROUP_CODE = {id}";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var row = new
                            {
                                PPRINTER = reader["PAY_PRINTER"].ToString(),
                                PAY_COPIES = reader["PAY_COPIES"] == DBNull.Value ? "" : reader["PAY_COPIES"].ToString(),
                                PRE_COPIES = reader["PRE_COPIES"] == DBNull.Value ? "" : reader["PRE_COPIES"].ToString(),
                                DUP_COPIES = reader["DUP_COPIES"] == DBNull.Value ? "" : reader["DUP_COPIES"].ToString(),
                                PAY_MODAL = reader["PAY_MODAL"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PAY_MODAL"]),
                                PRE_MODAL = reader["PRE_MODAL"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PRE_MODAL"]),
                                DUP_MODAL = reader["DUP_MODAL"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DUP_MODAL"]),
                                QR_CODE = reader["QR_CODE"].ToString(),
                                PWINDOW = reader["P_WINDOW"].ToString(),
                                LOC_SNAME = reader["LOCATION_SNAME"].ToString(),
                                SIMG = reader["STICKER_LOGO"].ToString(),
                                LOGO_IMG = reader["POS_PIRNT_LOGO"].ToString(),
                                TRANID = reader["GROUP_CODE"].ToString(),
                                GROUPNAME = reader["GROUP_NAME"].ToString(),
                                CASHACT = Convert.ToInt32(reader["CASH_ACT"]),
                                CASHTAX = reader["CASH_TAX"].ToString(),
                                BANKACT = Convert.ToInt32(reader["BANK_ACT"]),
                                BANKTAX = reader["BANK_TAX"].ToString(),
                                BANKCHAR = reader["BCHARGES"].ToString(),
                                Branch = Convert.ToInt32(reader["BCODE"]),
                                PARTYTAX = reader["PARTY_TAX"].ToString(),
                                //PARTYACT = Convert.ToInt32(reader["PAY_ACODE"]),
                                SRBNAME = reader["SRB_NAME"].ToString(),
                                SRBNTN = reader["SRB_NTN"].ToString(),
                                POSUSER = reader["POS_USER"].ToString(),
                                POSPASS = reader["POS_PASS"].ToString(),
                                SRBID = reader["SRB_ID"].ToString(),
                                SRBSTATUS = reader["SRB_STATUS"].ToString(),
                                SRBURL = reader["SRB_URL"].ToString(),
                                GROUPIMG = reader["GROUP_IMAGE"].ToString(),
                                ITEMIMG = reader["ITEM_IMAGE"].ToString(),
                                WAITERIMG = reader["WAITER_IMAGE"].ToString(),
                                TABLEIMG = reader["TABLE_IMAGE"].ToString(),
                                RCASH = reader["R_CASH"].ToString(),
                                RADVANCE = reader["R_ADVANCE"].ToString(),
                                RCARD = reader["R_CARD"].ToString(),
                                RPARTY = reader["R_PARTY"].ToString(),
                                RSPLIT = reader["R_SPLIT"].ToString(),
                                FB_LINK = reader["FB_LINK"].ToString(),
                                INSTA_LINK = reader["INSTA_LINK"].ToString(),
                                WEB_LINK = reader["WEB_LINK"].ToString(),
                                TIKTOK_LINK = reader["TIKTOK_LINK"].ToString(),
                                YOUTUBE_LINK = reader["YOUTUBE_LINK"].ToString(),
                                WIFI_NAME = reader["WIFI_NAME"].ToString(),
                                WIFI_PASSWORD = reader["WIFI_PASSWORD"].ToString(),
                                AdvanceBtn = reader["ADVANCE_BTN"].ToString(),
                                KotBtn = reader["KOT_BTN"].ToString(),
                                SalesmanReq = reader["SALESMAN_REQ"].ToString(),
                                WHATSAPP_URL = reader["WHATSAPP_URL"].ToString(),
                                WHATSAPP_TOKEN = reader["WHATSAPP_TOKEN"].ToString(),
                                WHATSAPP_MSG = reader["WHATSAPP_MSG"].ToString(),
                                WHATSAPP_CC = reader["WHATSAPP_CC_MSG"].ToString(),
                                WHATSAPP_ADV = reader["WHT_MSG_ADV"].ToString(),
                                WHATSAPP_RTN = reader["WHT_MSG_RETURN"].ToString(),
                                WHT_ADV_COM = reader["WHT_ADV_COM"].ToString(),
                                WHT_PARTY_MSG = reader["WHT_PARTY_MSG"].ToString(),
                                SER_CHARGES = reader["SER_CHARGES"].ToString(),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                    response.msg = "";
                    response.msgType = 1;
                    response.data = jsonDataResult;
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE GROUP_CODE = '" + id + @"'";
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

        public MyHttpResponseMessage GetDataForReport(POSMappingReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            POSMappingReport masterData = new POSMappingReport();
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
                        masterData.DUE_NO = reader["UNIT"].ToString();
                        masterData.DRIVER = reader["UNIT"].ToString();
                        masterData.VEHICLE = reader["UNIT"].ToString();
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