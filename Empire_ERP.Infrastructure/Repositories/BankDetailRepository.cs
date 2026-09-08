using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class BankDetailRepository : IBankDetailRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }

        public BankDetailRepository(IMenuRepository menuRepository, IBranchRepository branchRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
        }

        public MyHttpResponseMessage GetChartOfAccounts(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<KeyValuePair<int, string?>> dropdown = new List<KeyValuePair<int, string?>>();
                using (SqlConnection db = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = @$" SELECT 
                                      CH.ACT_CODE,CH.ACT_NAME 
                                    FROM TBL_ROLE R 
                                    LEFT OUTER JOIN TBL_CHART CH

                                       ON (

                                          (R.SHOW_SELECTED = 1 AND CH.ACT_CODE = R.RMENU_ID)
                                          OR 
                                          (R.SHOW_SELECTED = 0 AND 
                                           CH.ACT_CODE NOT IN 
                                             (SELECT DISTINCT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = {common.RoleID} AND MODULE_ID = 3) 
      
                                          )
                                      ) 
                                      LEFT OUTER JOIN TBL_CHART PCH
                                    ON PCH.ACT_CODE = CH.ACT_PARENT_CODE
                                    WHERE R.ROLE_TYPE = '{common.RoleType}'
                                      AND R.MODULE_ID = 3 
                                      AND CH.DLT = 'T' 
                                      AND CH.ASTATUS = 'Y'
                                      AND R.ROLE_ID = {common.RoleID}
                                      AND CH.ACT_NATURE = 2 AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'

                                    GROUP BY CH.ACT_CODE,CH.ACT_NAME 


                                      UNION ALL

                                      SELECT     CH.ACT_CODE,CH.ACT_NAME 
                                      FROM TBL_CHART CH
                                      LEFT OUTER JOIN TBL_CHART PCH
                                    ON PCH.ACT_CODE = CH.ACT_PARENT_CODE

                                    WHERE 

                                   '{common.RoleType}' = 'A'  
                                    AND CH.ACT_NATURE = 2 AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'


                                    UNION ALL
                                            SELECT CH.ACT_CODE,CH.ACT_NAME 
		                                    FROM TBL_CHART CH 
                                      LEFT OUTER JOIN TBL_CHART PCH
                                    ON PCH.ACT_CODE = CH.ACT_PARENT_CODE
		                                    WHERE 
  
                                    '{common.RoleType}'

                                      = 'U'  
                                    AND CH.ACT_NATURE = 2 AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'
   
                                          AND CH.DLT = 'T' 
                                          AND CH.ASTATUS = 'Y' AND  
                                           (SELECT 
                                     COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_CHART M ON R.RMENU_ID = M.ACT_CODE 
                                     WHERE R.ROLE_TYPE = 
                                    '{common.RoleType}'
   
                                          AND R.MODULE_ID = 3 

                                          AND M.DLT = 'T' 
                                          AND M.ASTATUS = 'Y'
                                          AND R.ROLE_ID =
	  
                                      {common.RoleID}
                                        AND CH.ACT_NATURE = 2 AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'
	
                                          ) < 1 ";
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
				string? table = string.Empty;
				string? table2 = string.Empty;
				string? tablePick = string.Empty;
				string? tablePick2 = string.Empty;
                string? search = string.Empty;
                if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE1;
					table2 = menu.TABLE2;
					tablePick = menu.PICK_TABLE_MASTER;
					tablePick2 = menu.PICK_TABLE_DETAIL;
                    search = menu.SEARCH;
                }

				if (!String.IsNullOrWhiteSpace(table))
				{
					List<object> jsonDataResult = new List<object>();
					using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
					{
                        if(search == "M")
                        {
                            string query = $@"SELECT M.TRAN_ID,M.V_DATE,M.VOUCHER_NO, M.REF, CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS ,M.REMARKS, M.ADD_USER_ID, 
                                                M.ADD_DATE, M.ADD_COMPUTER_NAME, M.ADD_IP_ADDRESS, M.EDIT_USER_ID, M.EDIT_DATE, M.EDIT_COMPUTER_NAME, M.EDIT_IP_ADDRESS, M.ADD_POSTALCODE, M.EDIT_POSTALCODE, 
                                                SM.VOUCHER_NO AS PVOUCHER_NO,SM.TRAN_ID AS PTRAN_ID, MB.ID AS MENU_ID ,MB.MENU_PAGE,MB.MENU_PARENT_CODE
                                                FROM {table} M 
                                                LEFT OUTER JOIN TBL_BD_DETAIL D ON D.TRAN_ID = M.TRAN_ID
                                                LEFT OUTER JOIN TBL_SHIP_DETAIL SD ON SD.DT_CODE = D.PICK_ID
                                                LEFT OUTER JOIN TBL_SHIP_MASTER SM ON SM.TRAN_ID = SD.TRAN_ID
                                                LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = SM.MENU_ID
                                                WHERE M.DLT = 'T' AND M.BCODE = '" + common.Branch + "' AND M.PERIOD_ID = '" + common.Period + "' ORDER BY M.TRAN_ID DESC";
                            SqlCommand command = new SqlCommand(query, connection);
						    connection.Open();
						    SqlDataReader reader = command.ExecuteReader();
						    while (reader.Read())
						    {
							    var row = new
							    {
								    ID = Convert.ToString(reader["TRAN_ID"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                    V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                    REF = Convert.ToString(reader["REF"]),
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
                                    PVOUCHER_NO = Convert.ToString(reader["PVOUCHER_NO"]),
                                    PTRAN_ID = reader["PTRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PTRAN_ID"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                };
							    jsonDataResult.Add(row);
						    }
						    reader.Close();
                        }
                        else
                        {
                            string query = $@"SELECT M.TRAN_ID, M.V_DATE, M.VOUCHER_NO, M.REF, CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, M.REMARKS, M.ADD_USER_ID, 
                                                M.ADD_DATE, M.ADD_COMPUTER_NAME, M.ADD_IP_ADDRESS, M.EDIT_USER_ID, M.EDIT_DATE, M.EDIT_COMPUTER_NAME, M.EDIT_IP_ADDRESS, M.ADD_POSTALCODE, 
                                                M.EDIT_POSTALCODE, D.DHL_NO,D.ETA ,CH.ACT_NAME AS BOOK_TYPE , 
                                                CASE WHEN BANK_STATUS = 1 THEN 'Varified' when BANK_STATUS = 0 THEN 'Not-Varified' else 'Error' End as BANK_STATUS,
                                                CASE WHEN FI_REQ = 1 THEN 'Varified' when FI_REQ = 0 THEN 'Not-Varified' else 'Error' End as FI_REQ,
                                                CASE WHEN FI = 1 THEN 'Varified' when FI = 0 THEN 'Not-Varified' else 'Error' End as FI,D.DT_DESC , 
                                                PM.VOUCHER_NO AS PICK_DATA, PM.TRAN_ID AS PTRAN_ID,  MB.ID AS MENU_ID ,MB.MENU_PAGE,MB.MENU_PARENT_CODE
                                                FROM {table} M 
                                                LEFT OUTER JOIN {table2} D ON D.TRAN_ID = M.TRAN_ID AND D.PERIOD_ID = M.PERIOD_ID AND D.BCODE = M.BCODE 
                                                LEFT OUTER JOIN {tablePick2} PD ON PD.DT_CODE = D.PICK_ID AND PD.BCODE = D.BCODE AND PD.PERIOD_ID = D.PERIOD_ID
                                                LEFT OUTER JOIN {tablePick} PM ON PM.TRAN_ID = PD.TRAN_ID AND PM.BCODE = PD.BCODE AND PM.PERIOD_ID = PD.PERIOD_ID
                                                LEFT OUTER JOIN TBL_CHART CH ON CH.ACT_CODE = D.BOOK_TYPE
                                                LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = PM.MENU_ID
                                                WHERE M.DLT = 'T' AND M.BCODE = '" + common.Branch + "' AND M.PERIOD_ID = '" + common.Period + "' ORDER BY M.TRAN_ID DESC";
                            SqlCommand command = new SqlCommand(query, connection);
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                var row = new
                                {
                                    ID = Convert.ToString(reader["TRAN_ID"]),
                                    V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                    REF = Convert.ToString(reader["REF"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"]),
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
                                    DHL_NO = reader["DHL_NO"] == DBNull.Value ? "" : Convert.ToString(reader["DHL_NO"]),
                                    ETA = reader["ETA"] == DBNull.Value ? null : Convert.ToDateTime(reader["ETA"]).ToString("yyyy-MM-dd"),
                                    BOOK_TYPE = reader["BOOK_TYPE"] == DBNull.Value ? "" : Convert.ToString(reader["BOOK_TYPE"]),
                                    BANK_STATUS = reader["BANK_STATUS"] == DBNull.Value ? "" : Convert.ToString(reader["BANK_STATUS"]),
                                    FI_REQ = reader["FI_REQ"] == DBNull.Value ? "" : Convert.ToString(reader["FI_REQ"]),
                                    FI = reader["FI"] == DBNull.Value ? "" : Convert.ToString(reader["FI"]),
                                    DT_DESC = reader["DT_DESC"] == DBNull.Value ? "" : Convert.ToString(reader["DT_DESC"]),
                                    PICK_DATA = reader["PICK_DATA"] == DBNull.Value ? "" : Convert.ToString(reader["PICK_DATA"]),
                                    PTRAN_ID = reader["PTRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PTRAN_ID"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
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

        public MyHttpResponseMessage GetBankDetailByCode(int code, Common common)
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
                        string query = "SELECT TRAN_ID,V_DATE,VOUCHER_NO,REF,REMARKS,ASTATUS " +
                                       $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = Convert.ToString(reader["TRAN_ID"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
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

        public MyHttpResponseMessage GetBankDetailDetailByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? pickTableMaster = string.Empty;
                string? pickTableDetail = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE2;
                    pickTableMaster = menu.PICK_TABLE_MASTER;
                    pickTableDetail = menu.PICK_TABLE_DETAIL;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"SELECT D.DT_CODE ,D.PICK_ID ,D.ORIGNAL_DOC ,D.DHL_NO ,D.ETA ,D.BOOK_TYPE ,D.BANK_STATUS ,D.FI ,D.FI_REQ ,D.DT_DESC,
                                        PM.VOUCHER_NO AS PICK_DATA ,PM.TRAN_ID AS PTRAN_ID , MB.ID AS MENU_ID ,MB.MENU_PAGE,MB.MENU_PARENT_CODE FROM {table} D 
                                        LEFT OUTER JOIN {pickTableDetail} PD 
                                        ON PD.DT_CODE = D.PICK_ID AND PD.BCODE = D.BCODE AND PD.PERIOD_ID = D.PERIOD_ID 
                                        LEFT OUTER JOIN {pickTableMaster} PM 
                                        ON PM.TRAN_ID = PD.TRAN_ID AND PM.BCODE = PD.BCODE AND PM.PERIOD_ID = PD.PERIOD_ID 
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = PM.MENU_ID
                                        WHERE D.DLT = 'T' AND D.TRAN_ID = '{code}' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}' ORDER BY D.DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToInt32(reader["DT_CODE"]),
                                PICK_ID = Convert.ToInt32(reader["PICK_ID"]),
                                ORIGINAL_DOC = Convert.ToString(reader["ORIGNAL_DOC"]),
                                DHL_NO = Convert.ToString(reader["DHL_NO"]),
                                ETA = Convert.ToString(reader["ETA"]),
                                BOOK_TYPE = reader["BOOK_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BOOK_TYPE"]),
                                BANK = Convert.ToInt32(reader["BANK_STATUS"]),
                                FI = Convert.ToInt32(reader["FI"]),
                                FI_REQ = Convert.ToString(reader["FI_REQ"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),

                                VOUCHER_NO = Convert.ToString(reader["PICK_DATA"]),
                                PTRAN_ID = reader["PTRAN_ID"] == DBNull.Value ? null : Convert.ToString(reader["PTRAN_ID"]),
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

        public MyHttpResponseMessage Save(CustomBankDetail modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty;
                int? pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pType = menu.PTYPE;
                }

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
                {
                    var Ip = common.IPAddress;
                    var Computer = common.ComputerName;
                    var Postal = common.PostalCode;
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
                            string query = "", detailQuery= "", voucherNo = string.Empty;
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
                                        "(TRAN_ID, V_DATE, VOUCHER_NO, REF, REMARKS, BCODE, PERIOD_ID," +
                                        "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME," +
                                        "ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE," +
                                        "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE," +
                                        "EDIT_POSTALCODE, ASTATUS, MENU_ID, DLT)" +
                                        "VALUES" +
                                        "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.REF + "','" + modelRecord.Master.REMARKS + "','" + branch + "','" + period + "'," +
                                        "'" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                        "'" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                        "'" + Computer + "','" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.Master.ASTATUS + "','" + menuID + "','T')";
                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                query = $"UPDATE {table} SET V_DATE = '" + modelRecord.Master.V_DATE + @"',
                                                REF = '" + modelRecord.Master.REF + @"',
                                                REMARKS = '" + modelRecord.Master.REMARKS + @"',
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

                            if (modelRecord.Detail.Count > 0)
                            {
                                detailQuery = $"UPDATE {detailTable} SET DLT = 'F'" +
                                $" WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                                command.CommandText = detailQuery;
                                command.ExecuteNonQuery();
                            }

                            List<CustomPartyType> partiesData = DropdownService.CustomPartyTypeDropdownWithAccountCodeAndPType(common.RoleID, common.RoleType, pType);

                            foreach (var item in modelRecord.Detail.ToList())
                            {
                                try
                                {
                                    if (item.DT_CODE == null || item.DT_CODE == 0)
                                    {
                                        int detailCode = GenerateNextDetailId(common, command);
                                        if (detailCode > 0)
                                        {
                                            int bank = item.BANK == null  ? 0 : 1;
                                            int FiReq = item.FI_REQ == null  ? 0 : 1;
                                            int fi = item.FI == null  ? 0 : 1;
                                            detailQuery = $@"INSERT INTO {detailTable} 
                                                            ([TRAN_ID], [DT_CODE], [ORIGNAL_DOC], [DHL_NO], [ETA], [BOOK_TYPE], [BANK_STATUS], [FI_REQ], [FI], [DT_DESC], [BCODE], 
                                                             [PERIOD_ID], [ADD_USER_ID], [ADD_DATE], [ADD_COMPUTER_NAME], [ADD_IP_ADDRESS], [EDIT_USER_ID], 
                                                             [EDIT_DATE], [EDIT_COMPUTER_NAME], [EDIT_IP_ADDRESS], [ADD_POSTALCODE], [EDIT_POSTALCODE], 
                                                             [MENU_ID], [DLT], [PICK_ID]) 
                                                            VALUES ({modelRecord.Master.TRAN_ID}, {detailCode}, '{item.ORIGINAL_DOC}', '{item.DHL_NO}', '{item.ETA}', '{item.BOOK_TYPE}', 
                                                                    '{bank}', '{FiReq}', '{fi}', '{item.DT_DESC}', '{branch}', '{period}', 
                                                                    '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', 
                                                                    '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', 
                                                                    '{Postal}', '{Postal}', '{menuID}', 'T', {item.PICK_ID})";
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
                                        detailQuery = $@"UPDATE {detailTable} 
                                                        SET
                                                        [ORIGNAL_DOC] = '{item.ORIGINAL_DOC}', 
                                                        [DHL_NO] = '{item.DHL_NO}', 
                                                        [ETA] = '{item.ETA}', 
                                                        [BOOK_TYPE] = '{item.BOOK_TYPE}', 
                                                        [BANK_STATUS] = '{item.BANK}', 
                                                        [FI_REQ] = '{item.FI_REQ}', 
                                                        [FI] = '{item.FI}', 
                                                        [DT_DESC] = '{item.DT_DESC}', 
                                                        [BCODE] = '{branch}', 
                                                        [PERIOD_ID] = '{period}', 
                                                        [EDIT_USER_ID] = '{username}', 
                                                        [EDIT_DATE] = '{CommonService.GetDateTime("Pakistan Standard Time")}', 
                                                        [EDIT_COMPUTER_NAME] = '{Computer}', 
                                                        [EDIT_IP_ADDRESS] = '{Ip}', 
                                                        [EDIT_POSTALCODE] = '{Postal}', 
                                                        [MENU_ID] = '{menuID}', 
                                                        [DLT] = 'T'
                                                        WHERE TRAN_ID = '" + modelRecord.Master.TRAN_ID + "' AND DT_CODE = '" + item.DT_CODE + "' AND BCODE = '" + branch + "' AND PERIOD_ID = '" + period + "'";
                                        command.CommandText = detailQuery;
                                        command.ExecuteNonQuery();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    isDetailAdded = false;
                                }
                            }

                            if (IsMasterAdded && isDetailAdded)
                            {
                                transaction.Commit();
                                response.data = new {
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

				if(!String.IsNullOrWhiteSpace(table))
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
                            string query = $"UPDATE {table} SET DLT = 'F' WHERE TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}' " +
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

        public MyHttpResponseMessage DeleteBankDetailDetailByCode(int code, Common common)
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

        public MyHttpResponseMessage GetSodaBookFeedingDetail(Common common)
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
                        string query = @$"SELECT SD.DT_CODE ,SM.V_DATE, SM.VOUCHER_NO, SM.REF, SM.REMARKS, SM.SHIPMENT_DOC,
                                        SD.QTY , SD.SHIP_DATE , SD.BL_NO , SD.DT_DESC
                                        FROM {pickMasterTable} SM
                                        LEFT OUTER JOIN {pickDetailTable} SD
                                        ON SD.TRAN_ID = SM.TRAN_ID AND SD.BCODE = SM.BCODE AND SD.PERIOD_ID = SM.PERIOD_ID
                                        LEFT OUTER JOIN {detailTable} DFD
                                        ON SD.DT_CODE = DFD.PICK_ID AND SD.BCODE = DFD.BCODE AND SD.PERIOD_ID = DFD.PERIOD_ID
                                        AND DFD.DLT = 'T'
                                        WHERE SM.DLT = 'T' AND SM.ASTATUS = 'Y' AND SD.DLT = 'T'
                                        AND DFD.PICK_ID IS NULL";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                PICK_ID = Convert.ToInt32(reader["DT_CODE"]),
                                V_DATE = Convert.ToString(reader["V_DATE"]),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                SHIPMENT_DOC = Convert.ToString(reader["SHIPMENT_DOC"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                SHIP_DATE = Convert.ToString(reader["SHIP_DATE"]),
                                BL_NO = Convert.ToString(reader["BL_NO"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
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
    }
}