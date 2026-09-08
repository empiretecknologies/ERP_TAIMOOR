using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class MachineInfoRepository : IMachineInfoRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public MachineInfoRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonRepository commonRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
        }

        public MyHttpResponseMessage GetMachineInfos(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<MachineInfo> chartOfAccounts = new List<MachineInfo>();
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
                            MachineInfo chartOfAccount = new MachineInfo();
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
                List<MachineInfo> chartOfAccounts = new List<MachineInfo>();
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
                            MachineInfo chartOfAccount = new MachineInfo();
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
                        string query = @$"SELECT [CODE],[MCODE],[MACHINE_NAME],[BCODE],[IP_ADDRESS],[IP_PORT],[SERVER_NAME],[S_USER_ID]
                                ,[S_PASSWORD],[MDB_NAME],[QUERY] FROM {table} A " +
                            "WHERE A.DLT = 'T' " +
                            "ORDER BY CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRANID = reader["CODE"].ToString(),
                                MachineCode = reader["MCODE"].ToString(),
                                MachineName = reader["MACHINE_NAME"].ToString(),
                                Branch = reader["BCODE"].ToString(),
                                IpAddress = reader["IP_ADDRESS"].ToString(),
                                Ipport = reader["IP_PORT"].ToString(),
                                ServerName = reader["SERVER_NAME"].ToString(),
                                //Branch= reader["BCODE"].ToString(),
                                SUserId = reader["S_USER_ID"],
                                SPassword = reader["S_PASSWORD"].ToString(),
                                DbName = reader["MDB_NAME"].ToString(),
                                Query = reader["QUERY"].ToString(),
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(CODE), 0) + 1 FROM {table}";
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
        public MyHttpResponseMessage Save(MachineInfo modelRecord, Core.Entities.Common common)
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

                                query = $"INSERT INTO {table} " +
                                         " ([CODE], [MCODE], [MACHINE_NAME], [BCODE], [IP_ADDRESS], [IP_PORT], [SERVER_NAME], [S_USER_ID], [S_PASSWORD], [MDB_NAME], " +
                                         " [QUERY], [ADD_USER_ID], [ADD_DATE], [ADD_COMPUTER_NAME], [ADD_IP_ADDRESS], [EDIT_USER_ID], [EDIT_DATE], [EDIT_COMPUTER_NAME], " +
                                         " [EDIT_IP_ADDRESS], [MENU_ID], [ADD_POSTALCODE], [EDIT_POSTALCODE], [DLT], [ASTATUS]) VALUES " +
                                         " ('" + code + "', '" + modelRecord.Machine_Code + "', '" + modelRecord.Machine_Name + "', '" + modelRecord.BRANCH + "', '" + modelRecord.IP_ADDRESS + "', '" + modelRecord.IP_PORT + "', " +
                                         " '" + modelRecord.SERVER_NAME + "', '" + modelRecord.S_USER_ID + "', '" + modelRecord.S_PASSWORD + "', '" + modelRecord.DB_NAME + "', '" + modelRecord.QUERY + "', " +
                                         " '" + userid + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "', '" + Ip + "', " +
                                         " '" + userid + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "', '" + Ip + "', " +
                                         " '" + common.MenuID + "', '" + Postal + "', '" + Postal + "', 'T', 'Y')";


                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                response.data = code;
                                response.msgType = 1;
                                response.msg = "Record Added Successfully";

                            }
                            else
                            {
                                query = $"UPDATE {table} SET " +
                                         " [MCODE] = '" + modelRecord.Machine_Code + "', " +
                                         " [MACHINE_NAME] = '" + modelRecord.Machine_Name + "', " +
                                         " [BCODE] = '" + modelRecord.BRANCH + "', " +
                                         " [IP_ADDRESS] = '" + modelRecord.IP_ADDRESS + "', " +
                                         " [IP_PORT] = '" + modelRecord.IP_PORT + "', " +
                                         " [SERVER_NAME] = '" + modelRecord.SERVER_NAME + "', " +
                                         " [S_USER_ID] = '" + modelRecord.S_USER_ID + "', " +
                                         " [S_PASSWORD] = '" + modelRecord.S_PASSWORD + "', " +
                                         " [MDB_NAME] = '" + modelRecord.DB_NAME + "', " +
                                         " [QUERY] = '" + modelRecord.QUERY + "', " +
                                         " [EDIT_USER_ID] = '" + userid + "', " +
                                         " [EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + "', " +
                                         " [EDIT_COMPUTER_NAME] = '" + Computer + "', " +
                                         " [EDIT_IP_ADDRESS] = '" + Ip + "', " +
                                         " [EDIT_POSTALCODE] = '" + Postal + "', " +
                                         " [MENU_ID] = '" + common.MenuID + "', " +
                                         " [ASTATUS] = 'Y', " +
                                         " [DLT] = 'T' " + 
                                         " WHERE [CODE] = '" + modelRecord.TRAN_ID + "'";


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

        public MyHttpResponseMessage GetMachineInfoById(int id, Core.Entities.Common common)
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
                        string query = @$"SELECT [CODE],[MCODE],[MACHINE_NAME],[BCODE],[IP_ADDRESS],[IP_PORT],[SERVER_NAME],[S_USER_ID]
                                ,[S_PASSWORD],[MDB_NAME],[QUERY] FROM {table} A " +
                            $"WHERE CODE = {id} AND A.DLT = 'T' " +
                            "ORDER BY CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRANID = reader["CODE"].ToString(),
                                MachineCode = reader["MCODE"].ToString(),
                                MachineName = reader["MACHINE_NAME"].ToString(),
                                Branch = reader["BCODE"].ToString(),
                                IpAddress = reader["IP_ADDRESS"].ToString(),
                                Ipport = reader["IP_PORT"].ToString(),
                                ServerName = reader["SERVER_NAME"].ToString(),
                                //Branch= reader["BCODE"].ToString(),
                                SUserId = reader["S_USER_ID"],
                                SPassword = reader["S_PASSWORD"].ToString(),
                                DbName = reader["MDB_NAME"].ToString(),
                                Query = reader["QUERY"].ToString(),
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE CODE = '" + id + @"'";
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

        //public MyHttpResponseMessage GetDataForReport(MachineInfoReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    MachineInfoReport masterData = new MachineInfoReport();
        //    CustomDeliveryFeedingForPrintReport reportData = new CustomDeliveryFeedingForPrintReport();
        //    var Menu = _menuRepository.GetMenu(common.MenuID);
        //    string? table = string.Empty, detailTable = string.Empty;
        //    if (Menu.data != null)
        //    {
        //        var menu = (Menu)Menu.data;
        //        table = menu.TABLE1;
        //    }
        //    try
        //    {
        //        string topQuery = "";
        //        if (menuDetails.MD_ID == 31)
        //        {
        //            topQuery = @"SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO,  P.PARTY_NAME AS PARTY_CODE, L.LOT_NO, IM.ITEM_NAME AS ITEM_CODE, UN.GROUP_NAME AS UNIT, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, A.DUE_NO, A.DRIVER, A.VEHICLE, A.QTY " +
        //                            $"FROM {table} A " +
        //                            "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE " +
        //                            "LEFT OUTER JOIN TBL_LOT L ON A.LOT = L.CODE " +
        //                            "LEFT OUTER JOIN TBL_UNIT UN ON A.UNIT = UN.GROUP_CODE " +
        //                            "LEFT OUTER JOIN TBL_PARTY_TYPES P ON A.PARTY_CODE = P.PARTY_CODE AND P.ACT_CODE = A.ACT_CODE " +
        //                            $"WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' AND A.TRAN_ID = '{modelRecord.TRAN_ID}' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}'";
        //        }

        //        using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //        {
        //            SqlCommand command = new SqlCommand(topQuery, connection);
        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                masterData.COMPANY_NAME = currentCompany.C_NAME;
        //                masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
        //                masterData.COMPANY_PHONE = currentCompany.C_TEL;
        //                masterData.COMPANY_LOGO = currentCompany.C_LOGO;
        //                masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
        //                masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
        //                masterData.MENU_SIG1 = $"{menuDetails.MENU_SIG1}";
        //                masterData.MENU_SIG2 = $"{menuDetails.MENU_SIG2}";
        //                masterData.MENU_SIG3 = $"{menuDetails.MENU_SIG3}";
        //                masterData.MENU_SIG4 = $"{menuDetails.MENU_SIG4}";
        //                masterData.MENU_TERMS = $"{menuDetails.MENU_TERMS}";
        //                masterData.V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
        //                masterData.VOUCHER_NO = reader["VOUCHER_NO"].ToString();
        //                masterData.PARTY_CODE = reader["PARTY_CODE"].ToString();
        //                masterData.LOT_NO = reader["LOT_NO"].ToString();
        //                masterData.ITEM_CODE = reader["ITEM_CODE"].ToString();
        //                masterData.UNIT = reader["UNIT"].ToString();
        //                masterData.DUE_NO = reader["UNIT"].ToString();
        //                masterData.DRIVER = reader["UNIT"].ToString();
        //                masterData.VEHICLE = reader["UNIT"].ToString();
        //                masterData.QUANTITY = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]);
        //            }
        //            reader.Close();
        //        }

        //        response.data = masterData;
        //        response.msg = "";
        //        response.msgType = 1;
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
    }
}