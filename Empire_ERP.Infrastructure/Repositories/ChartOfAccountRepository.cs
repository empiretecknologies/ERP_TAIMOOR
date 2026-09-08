using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using ZXing.QrCode.Internal;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class ChartOfAccountRepository : IChartOfAccountRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public ChartOfAccountRepository(IMenuRepository menuRepository, ICommonRepository commonRepository)
        {
            _menuRepository = menuRepository;
            _commonRepository = commonRepository;
        }

        public MyHttpResponseMessage GetChartOfAccounts(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<ChartOfAccount> chartOfAccounts = new List<ChartOfAccount>();
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
                        string query = $"SELECT A.ACT_CODE, A.ACT_NAME,CASE WHEN  C.ACT_NAME IS NULL THEN '' ELSE C.ACT_NAME END AS CONRTOL_NAME, CT.TNAME AS CHART_TYPE" +
                            $" FROM {table} A" +
                            $" LEFT OUTER JOIN {table} B ON A.ACT_GR_CODE LIKE CONCAT('', B.ACT_GR_CODE, '%') AND B.ASTATUS <> 'Y'" +
                            $" LEFT OUTER JOIN {table} C ON C.ACT_CODE = A.ACT_PARENT_CODE" +
                            $" LEFT OUTER JOIN TBL_CHART_TYPE CT ON CT.ID = A.CHART_TYPE" +
                            $" WHERE A.DLT = 'T' AND A.ACT_TYPE = 'C' AND B.ASTATUS IS NULL" +
                            $" GROUP BY A.ACT_CODE, A.ACT_NAME, CT.TNAME, A.ASTATUS,A.ACT_GR_CODE,B.ASTATUS,C.ACT_NAME";

                   

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {

                            ChartOfAccount chartOfAccount = new ChartOfAccount();
                            chartOfAccount.ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]);

                            chartOfAccount.ACT_NAME = Convert.ToString(reader["ACT_NAME"]);
                            chartOfAccount.ACT_SNAME = Convert.ToString(reader["CONRTOL_NAME"]);
                            chartOfAccount.CHART_NAME = Convert.ToString(reader["CHART_TYPE"]);


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
                List<ChartOfAccount> chartOfAccounts = new List<ChartOfAccount>();
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
                            ChartOfAccount chartOfAccount = new ChartOfAccount();
                            string actName = Convert.ToString(reader["ACT_NAME"]);
                            int actCode = Convert.ToInt32(reader["ACT_CODE"]);
                            int actParentCode = Convert.ToInt32(reader["ACT_PARENT_CODE"]);

                            chartOfAccount.ACT_NAME = actName;
                            chartOfAccount.ACT_CODE = actCode;
                            chartOfAccount.ACT_PARENT_CODE = actParentCode;
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
                        string query = @"SELECT A.ACT_CODE,A.ACT_NAME,A.ACT_SNAME,  " +
                            " CASE WHEN A.ACT_TYPE = 'C' THEN 'Control' WHEN A.ACT_TYPE = 'S' THEN 'Subsidiary' END AS ACCOUNT_TYPE, " +
                            " CASE WHEN B.ACT_NAME IS NULL THEN '' ELSE B.ACT_NAME END  AS PARENT_NAME, " +
                            " A.ACT_GR_CODE,GR.DESCR AS ACT_GROUP_NAME,AN.GROUP_NAME AS ACT_NATURE_NAME, " +
                            " A.COSTCENTER,A.PREFIX,A.ACCOUNT_NO,A.TITTLE,A.SWIFT,CHQ.GROUP_NAME CHEQUEFORMAT,C.DESCR CURRENCY, " +
                            " A.ADD_USER_ID,A.ADD_DATE,A.ADD_COMPUTER_NAME,A.ADD_IP_ADDRESS,A.ADD_POSTALCODE, " +
                             " CT.TNAME AS CHART_TYPE," +
                            " A.EDIT_USER_ID,A.EDIT_DATE,A.EDIT_COMPUTER_NAME,A.EDIT_IP_ADDRESS,A.EDIT_POSTALCODE, " +
                            " CASE WHEN A.ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ACT_STATUS " +
                            " FROM " + table + " A " +
                            " LEFT OUTER JOIN " + table + " B" +
                            " ON B.ACT_CODE = A.ACT_PARENT_CODE" +
                            " LEFT OUTER JOIN TBL_ACT_GROUP GR" +
                            " ON GR.CODE = A.ACT_GROUP" +
                            " LEFT OUTER JOIN TBL_ACT_NATURE AN" +
                            " ON AN.GROUP_CODE = A.ACT_NATURE" +
                            " LEFT OUTER JOIN TBL_CHQ_FORAMT CHQ" +
                            " ON CHQ.GROUP_CODE = A.CHQ_ID" +
                            " LEFT OUTER JOIN TBL_CURRENCY C" +

                            " ON C.CODE = A.CURRENCY" +
                            " LEFT OUTER JOIN TBL_CHART_TYPE CT ON CT.ID = A.CHART_TYPE" +
                            " WHERE A.DLT = 'T'" +
                            " ORDER BY A.ACT_GR_CODE OPTION(FAST 50)";


                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = reader["ACT_CODE"],
                                ACT_NAME = reader["ACT_NAME"].ToString(),
                                ACT_SNAME = reader["ACT_SNAME"].ToString(),
                                ACCOUNT_TYPE = reader["ACCOUNT_TYPE"].ToString(),
                                PARENT_NAME = reader["PARENT_NAME"].ToString(),
                                ACT_GR_CODE = reader["ACT_GR_CODE"].ToString(),
                                ACT_GROUP_NAME = reader["ACT_GROUP_NAME"].ToString(),
                                ACT_NATURE_NAME = reader["ACT_NATURE_NAME"].ToString(),
                                COSTCENTER = reader["COSTCENTER"].ToString(),
                                PREFIX = reader["PREFIX"].ToString(),
                                ACCOUNT_NO = reader["ACCOUNT_NO"].ToString(),
                                TITTLE = reader["TITTLE"].ToString(),
                                SWIFT = reader["SWIFT"].ToString(),
                                CHQ_ID = reader["CHEQUEFORMAT"].ToString(),
                                CURRENCY = reader["CURRENCY"].ToString(),
                                CHART_TYPE = reader["CHART_TYPE"].ToString(),
                                ADD_DATE = reader["ADD_DATE"].ToString(),
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"].ToString(),
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"].ToString(),
                                EDIT_USER_ID = reader["EDIT_USER_ID"].ToString(),
                                EDIT_DATE = reader["EDIT_DATE"].ToString(),
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"].ToString(),
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"].ToString(),
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"].ToString(),
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"].ToString(),
                                ADD_USER_ID = reader["ADD_USER_ID"].ToString(),
                                ACT_STATUS = reader["ACT_STATUS"].ToString()
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

   

        public MyHttpResponseMessage Save(ChartOfAccount modelRecord, Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                modelRecord.PASS = modelRecord.PASS == null ? "" : modelRecord.PASS;
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
                    var password = modelRecord.PASS.Length >= 8 ? CommonService.EncryptString(modelRecord.PASS) : null;
                    var actCode = string.Empty;
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
                            if (modelRecord.ACT_CODE == 0)
                            {
                                var Parent = modelRecord.ACT_PARENT_CODE;
                                var GrCode = "0" + GenerateGrCode(Convert.ToString(Parent), common);
                                actCode = GenerateNextId(common);
                                query = "INSERT INTO " + table + " " +
                                        "([ACT_CODE],[ACT_NAME],[PASS],[ACT_SNAME],[ACT_TYPE]," +
                                        "[ACT_PARENT_CODE],[ACT_GR_CODE],[ACT_GROUP],[ACT_NATURE]," +
                                        "[COSTCENTER],[PREFIX],[ACCOUNT_NO],[TITTLE],[SWIFT],[CHQ_ID]," +
                                        "[CURRENCY],[CHART_TYPE],[ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME]," +
                                        "[ADD_IP_ADDRESS],[ADD_POSTALCODE]," +
                                        "[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME]," +
                                        "[EDIT_IP_ADDRESS],[EDIT_POSTALCODE],[MENU_ID]," +
                                        "[ASTATUS],[DLT]) " +
                                        "VALUES (" +
                                        "'" + actCode + "'," +
                                        "'" + modelRecord.ACT_NAME + "'," +
                                        "'" + password + "'," +
                                        "'" + modelRecord.ACT_SNAME + "'," +
                                        "'" + modelRecord.ACT_TYPE + "'," +
                                        "'" + modelRecord.ACT_PARENT_CODE + "'," +
                                        "'" + GrCode + "'," +
                                        "'" + modelRecord.ACT_GROUP + "'," +
                                        "'" + modelRecord.ACT_NATURE + "'," +
                                        "'" + modelRecord.COSTCENTER + "'," +
                                        "'" + modelRecord.PREFIX + "'," +
                                        "'" + modelRecord.ACCOUNT_NO + "'," +
                                        "'" + modelRecord.TITTLE + "'," +
                                        "'" + modelRecord.SWIFT + "'," +
                                        "'" + modelRecord.CHQ_ID + "'," +
                                        "'" + modelRecord.CURRENCY + "'," +
                                        "'" + modelRecord.CHART_TYPE + "'," +
                                        "'" + userid + "'," +
                                        "'" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                        "'" + Computer + "'," +
                                        "'" + Ip + "'," +
                                        "'" + Postal + "'," +
                                        "'" + userid + "'," +
                                        "'" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                        "'" + Computer + "'," +
                                        "'" + Ip + "'," +
                                        "'" + Postal + "'," +
                                        "'" + common.MenuID + "'," +
                                        "'" + modelRecord.ASTATUS + "'," +
                                        "'T')";

                                //SqlCommand command = new SqlCommand(query, connection);
                                //command.ExecuteNonQuery();
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE ACT_NAME = '" + modelRecord.ACT_NAME + "' AND DLT = 'T'";
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                //SqlCommand Ncommand = new SqlCommand(query, connection);
                                //connection.Open();
                                //int count = (int)Ncommand.ExecuteScalar();
                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.msg = "Record Added Successfully";
                                    response.data = new
                                    {
                                        ACT_CODE = actCode,
                                    };
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msgType = 2;
                                    response.msg = "Name Already Exist !....";
                                }
                            }
                            else
                            {
                                query = "UPDATE " + table + " SET [ACT_NAME] = '" + modelRecord.ACT_NAME + @"',
		                                                [ACT_SNAME] = '" + modelRecord.ACT_SNAME + @"',
		                                                [PASS] = '" + password + @"',
		                                                [ACT_TYPE] = '" + modelRecord.ACT_TYPE + @"',
		                                                [ACT_PARENT_CODE] = '" + modelRecord.ACT_PARENT_CODE + @"',
		                                                [ACT_GROUP] = '" + modelRecord.ACT_GROUP + @"',
		                                                [ACT_NATURE] = '" + modelRecord.ACT_NATURE + @"',
		                                                [COSTCENTER] = '" + modelRecord.COSTCENTER + @"',
		                                                [PREFIX] = '" + modelRecord.PREFIX + @"',
		                                                [ACCOUNT_NO] = '" + modelRecord.ACCOUNT_NO + @"',
		                                                [TITTLE] = '" + modelRecord.TITTLE + @"',
		                                                [SWIFT] = '" + modelRecord.SWIFT + @"',
		                                                [CHQ_ID] = '" + modelRecord.CHQ_ID + @"',
		                                                [CURRENCY] = '" + modelRecord.CURRENCY + @"',
                                                        
		                                                [EDIT_USER_ID] = '" + userid + @"',
		                                                [EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
		                                                [EDIT_COMPUTER_NAME] = '" + Computer + @"',
		                                                [EDIT_IP_ADDRESS] = '" + Ip + @"',
		                                                [MENU_ID] = '" + common.MenuID + @"',
		                                                [EDIT_POSTALCODE] = '" + Postal + @"',
		                                                [ASTATUS] = '" + modelRecord.ASTATUS + @"',
		                                                [DLT] = 'T'
                                                    WHERE[ACT_CODE] = '" + modelRecord.ACT_CODE + @"'";

                                //SqlCommand command = new SqlCommand(query, connection);
                                //command.ExecuteNonQuery();
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE ACT_NAME = '" + modelRecord.ACT_NAME + "' AND DLT = 'T'";
                                //SqlCommand Ncommand = new SqlCommand(Nquery, Nconnection);
                                //Nconnection.Open();
                                //int count = (int)Ncommand.ExecuteScalar();
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.msg = "Record Updated Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msgType = 2;
                                    response.msg = "Name Already Exist !....";
                                }
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
                    string maxIdQuery = "SELECT ISNULL(MAX(ACT_CODE), 0) + 1 FROM " + table + "";
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

        public MyHttpResponseMessage GetChartOfAccountById(int id, Core.Entities.Common common)
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
                        string query = "SELECT ACT_CODE, PASS, ACT_NAME, ACT_SNAME, ACT_TYPE, ACT_PARENT_CODE, " +
                            "ACT_GR_CODE, ACT_GROUP, ACT_NATURE, COSTCENTER, PREFIX, ACCOUNT_NO, TITTLE, SWIFT, " +
                            "CHQ_ID, CURRENCY, ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, " +
                            "EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, MENU_ID, ADD_POSTALCODE, " +
                            "EDIT_POSTALCODE, ASTATUS, DLT, CHART_TYPE FROM " + table + " WHERE ACT_CODE = @Id";

                      



                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new
                            {
                                ID = reader["ACT_CODE"],
                                ACT_NAME = reader["ACT_NAME"],
                                PASS = CommonService.DecryptString(reader["PASS"].ToString()),
                                ACT_SNAME = reader["ACT_SNAME"],
                                ACT_TYPE = reader["ACT_TYPE"],
                                ACT_PARENT_CODE = reader["ACT_PARENT_CODE"],
                                ACT_GR_CODE = reader["ACT_GR_CODE"],
                                ACT_GROUP = reader["ACT_GROUP"],
                                ACT_NATURE = reader["ACT_NATURE"],
                                COSTCENTER = reader["COSTCENTER"],
                                PREFIX = reader["PREFIX"],
                                ACCOUNT_NO = reader["ACCOUNT_NO"],
                                TITTLE = reader["TITTLE"],
                                SWIFT = reader["SWIFT"],
                                CHQ_ID = reader["CHQ_ID"].ToString(),
                                CURRENCY = reader["CURRENCY"].ToString(),
                                //chart type add
                                CHART_TYPE = reader["CHART_TYPE"],
                                ASTATUS = reader["ASTATUS"],
                                ADD_USER_ID = reader["ADD_USER_ID"],
                                ADD_DATE = reader["ADD_DATE"],
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"],
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"],
                                EDIT_USER_ID = reader["EDIT_USER_ID"],
                                EDIT_DATE = reader["EDIT_DATE"],
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"],
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"],
                                MENU_ID = reader["MENU_ID"],
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"],
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"]
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

        //public MyHttpResponseMessage Delete(int id, string startDate, string endDate, Core.Entities.Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    response.msg = "Data not found in our records";
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }
        //        bool childExists = false;
        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            if (id == 0)
        //            {
        //                response.msg = "ID is not in numeric format";
        //                response.msgType = 2;
        //            }
        //            else
        //            {
        //                string checkQuerry = new SQLService().getconnstring();
        //                using (SqlConnection connection = new SqlConnection(checkQuerry))
        //                {
        //                    connection.Open();
        //                    string query = "SELECT ACT_NAME FROM " + table + " WHERE DLT = 'T' AND ACT_PARENT_CODE = @ParentCode";

        //                    using (SqlCommand command = new SqlCommand(query, connection))
        //                    {
        //                        command.Parameters.AddWithValue("@ParentCode", id);

        //                        using (SqlDataReader reader = command.ExecuteReader())
        //                        {
        //                            List<string> descriptions = new List<string>();
        //                            while (reader.Read())
        //                            {
        //                                descriptions.Add(reader["ACT_NAME"].ToString());
        //                            }

        //                            if (descriptions.Count > 0)
        //                            {
        //                                childExists = true;
        //                                response.msgType = 2;
        //                                response.msg = "The following child exist and must be deleted first:       " +
        //                                               string.Join(", ", descriptions);
        //                            }
        //                        }
        //                    }
        //                }

        //                if (!childExists)
        //                {
        //                    string connectionString = new SQLService().getconnstring();
        //                    using (SqlConnection connection = new SqlConnection(connectionString))
        //                    {
        //                        connection.Open();
        //                        string query = "UPDATE " + table + " SET DLT = 'F' WHERE ACT_CODE = '" + id + @"'";
        //                        SqlCommand command = new SqlCommand(query, connection);
        //                        command.ExecuteNonQuery();
        //                        if (query != null)
        //                        {
        //                            response.msg = "Record Deleted Successfully";
        //                            response.msgType = 1;
        //                        }
        //                    }
        //                }
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

        public MyHttpResponseMessage Delete(int id, string startDate, string endDate, Core.Entities.Common common)
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

                bool childExists = false;
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

                        // 1. Check Child Records
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            string query = "SELECT ACT_NAME FROM " + table + " WHERE DLT = 'T' AND ACT_PARENT_CODE = @ParentCode";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ParentCode", id);

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    List<string> descriptions = new List<string>();
                                    while (reader.Read())
                                    {
                                        descriptions.Add(reader["ACT_NAME"].ToString());
                                    }

                                    if (descriptions.Count > 0)
                                    {
                                        childExists = true;
                                        response.msgType = 2;
                                        response.msg = "The following child exist and must be deleted first: " + string.Join(", ", descriptions);
                                    }
                                }
                            }
                        }

                        // 2. Agar child exist nahi karta, to Stored Procedure check karein
                        if (!childExists)
                        {
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();

                                string countQuery = $@"EXEC APROC '3','{startDate}','{endDate}','','{id}','{common.Branch}','{common.Period}'";
                                bool hasRecords = false;

                                using (SqlCommand countCommand = new SqlCommand(countQuery, connection))
                                {
                                    using (SqlDataReader procReader = countCommand.ExecuteReader())
                                    {
                                        if (procReader.Read())
                                        {
                                            hasRecords = true;
                                        }
                                    }
                                }

                                if (hasRecords)
                                {
                                    response.msg = "Cannot delete: Account used in vouchers.";
                                    response.msgType = 2;
                                    return response;
                                }

                                string updateQuery = "UPDATE " + table + " SET DLT = 'F' WHERE ACT_CODE = '" + id + "'";
                                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                                {
                                    int rowsAffected = command.ExecuteNonQuery();

                                    if (rowsAffected > 0)
                                    {
                                        response.msg = "Record Deleted Successfully";
                                        response.msgType = 1;
                                    }
                                    else
                                    {
                                        response.msg = "No record was updated.";
                                        response.msgType = 2;
                                    }
                                }
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
                ChartOfAccount chartOfAccountList = new ChartOfAccount();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE ACT_CODE = {record.TRAN_ID} AND DLT = 'T'";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        chartOfAccountList = new ChartOfAccount
                        {
                            ACT_CODE = 0,
                            ACT_NAME = record.ACT_NAME,
                            ACT_SNAME = Convert.ToString(reader["ACT_SNAME"]),
                            ACT_TYPE = Convert.ToString(reader["ACT_TYPE"]),
                            ACT_PARENT_CODE = Convert.ToInt32(reader["ACT_PARENT_CODE"]),
                            ACT_GROUP = Convert.ToInt32(reader["ACT_GROUP"]),
                            ACT_NATURE = Convert.ToInt32(reader["ACT_NATURE"]),
                            COSTCENTER = Convert.ToInt32(reader["COSTCENTER"]),
                            PREFIX = Convert.ToString(reader["PREFIX"]),
                            ACCOUNT_NO = Convert.ToString(reader["ACCOUNT_NO"]),
                            TITTLE = Convert.ToString(reader["TITTLE"]),
                            SWIFT = Convert.ToString(reader["SWIFT"]),
                            CHQ_ID = Convert.ToInt32(reader["CHQ_ID"]),
                            CURRENCY = Convert.ToInt32(reader["CURRENCY"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                        };
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(chartOfAccountList, common);
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
    }
}