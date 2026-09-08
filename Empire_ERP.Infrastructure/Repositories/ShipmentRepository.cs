using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class ShipmentRepository : IShipmentRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }

        public ShipmentRepository(IMenuRepository menuRepository, IBranchRepository branchRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
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
                            string query = $@"SELECT M.TRAN_ID,M.V_DATE,M.VOUCHER_NO, M.REF, CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS ,M.REMARKS, M.SHIPMENT_DOC, M.ADD_USER_ID, 
                                                M.ADD_DATE, M.ADD_COMPUTER_NAME, M.ADD_IP_ADDRESS, M.EDIT_USER_ID, M.EDIT_DATE, M.EDIT_COMPUTER_NAME, M.EDIT_IP_ADDRESS, M.ADD_POSTALCODE, M.EDIT_POSTALCODE ,
                                                IP_M.TRAN_ID AS PICK_ID,IP_M.VOUCHER_NO AS PVOUCHER_NO, MB.ID AS MENU_ID ,MB.MENU_PAGE,MB.MENU_PARENT_CODE
                                                FROM {table} M 
                                                LEFT OUTER JOIN TBL_SHIP_DETAIL D ON D.TRAN_ID = M.TRAN_ID
												LEFT OUTER JOIN TBL_IP_DETAIL IP_D ON IP_D.DT_CODE = D.PICK_ID
                                                LEFT OUTER JOIN TBL_IP_MASTER IP_M ON IP_M.TRAN_ID = IP_D.TRAN_ID
                                                LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = IP_M.MENU_ID
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
                                    SHIPMENT_DOC = Convert.ToString(reader["SHIPMENT_DOC"]),
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
                                    PICK_ID = reader["PICK_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PICK_ID"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                };
							    jsonDataResult.Add(row);
						    }
						    reader.Close();
                        }
                        else
                        {
                            string query = $@"SELECT M.TRAN_ID, M.V_DATE, M.VOUCHER_NO, M.REF, M.SHIPMENT_DOC, CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, M.REMARKS, M.ADD_USER_ID, 
                                                M.ADD_DATE, M.ADD_COMPUTER_NAME, M.ADD_IP_ADDRESS, M.EDIT_USER_ID, M.EDIT_DATE, M.EDIT_COMPUTER_NAME, M.EDIT_IP_ADDRESS, M.ADD_POSTALCODE, M.EDIT_POSTALCODE,
                                                D.QTY, U.GROUP_NAME AS UNIT, D.CQTY, D.SHIP_DATE, D.BL_NO, D.DT_DESC, PM.VOUCHER_NO AS PICK_DATA  ,
                                                PM.TRAN_ID AS PTRAN_ID,  MB.ID AS MENU_ID ,MB.MENU_PAGE,MB.MENU_PARENT_CODE
                                                FROM {table} M 
                                                LEFT OUTER JOIN {table2} D ON D.TRAN_ID = M.TRAN_ID AND D.PERIOD_ID = M.PERIOD_ID AND D.BCODE = M.BCODE 
                                                LEFT OUTER JOIN {tablePick2} PD
                                                ON PD.DT_CODE = D.PICK_ID AND PD.BCODE = D.BCODE AND PD.PERIOD_ID = D.PERIOD_ID
                                                LEFT OUTER JOIN {tablePick} PM
                                                ON PM.TRAN_ID = PD.TRAN_ID AND PM.BCODE = PD.BCODE AND PM.PERIOD_ID = PD.PERIOD_ID
                                                LEFT OUTER JOIN TBL_UNIT U
                                                ON U.GROUP_CODE = D.UNIT  
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
                                    ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                    V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                    REF = Convert.ToString(reader["REF"]),
                                    REMARKS = Convert.ToString(reader["REMARKS"]),
                                    SHIPMENT_DOC = Convert.ToString(reader["SHIPMENT_DOC"]),
                                    QTY = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["QTY"]),
                                    //AMT = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["AMT"]),
                                    //RATE = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["RATE"]),
                                    UNIT = reader["UNIT"] == DBNull.Value ? "" : Convert.ToString(reader["UNIT"]),
                                    CQTY = reader["CQTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["CQTY"]),
                                    SHIP_DATE = reader["SHIP_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM-dd"),
                                    BL_NO = reader["BL_NO"] == DBNull.Value ? "" : Convert.ToString(reader["BL_NO"]),
                                    DESC = Convert.ToString(reader["DT_DESC"]),
                                    PICK_DATA = Convert.ToString(reader["PICK_DATA"]),
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

        public MyHttpResponseMessage GetShipmentByCode(int code, Common common)
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
                        string query = "SELECT TRAN_ID,V_DATE,VOUCHER_NO,REF,REMARKS,SHIPMENT_DOC,PARTY_CODE,ACT_CODE,ASTATUS " +
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
                                SHIPMENT_DOC = Convert.ToString(reader["SHIPMENT_DOC"]),
                                PARTY_CODE = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",
                                ACT_CODE = reader["ACT_CODE"].ToString(),
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

        public MyHttpResponseMessage GetShipmentDetailByCode(int code, Common common)
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
                        string query = $@"SELECT D.*,PM.TRAN_ID AS PTRAN_ID, PM.VOUCHER_NO AS PICK_DATA, MB.ID AS MENU_ID ,MB.MENU_PAGE,MB.MENU_PARENT_CODE FROM {table} D 
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
                                QTY = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["QTY"]),
                                AMT = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["AMT"]),
                                RATE = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["RATE"]),
                                UNIT = reader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UNIT"]),
                                CQTY = reader["CQTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["CQTY"]),
                                SHIP_DATE = reader["SHIP_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM-dd"),
                                SHIPMENT_DATE = reader["SHIP_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM-dd"),
                                ALLOW = Convert.ToString(reader["ALLOW"]),
                                BL_NO = Convert.ToString(reader["BL_NO"]),
                                VOUCHER_NO = Convert.ToString(reader["PICK_DATA"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
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

        public MyHttpResponseMessage Save(CustomShipment modelRecord, Common common)
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
                    var Ip = common.IPAddress;
                    var Computer = common.ComputerName;
                    var Postal = common.PostalCode;
                    var username = common.Username;
                    var branch = common.Branch;
                    var period = common.Period;
                    var menuID = common.MenuID;
                    string connectionString = new SQLService().getconnstring();
                    List<CustomPartyType> partiesData = DropdownService.CustomPartyTypeDropdownWithAccountCode(common.RoleID, common.RoleType);

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;
                        try
                        {
                            var partyInformation = partiesData.ToList().Where(p => p.customizedKey == modelRecord.Master.PARTY_CODE).FirstOrDefault();

                            if (partyInformation != null)
                            {
                                modelRecord.Master.PARTY_CODE = Convert.ToString(partyInformation.key);
                                modelRecord.Master.ACT_CODE = partyInformation.accountCode;
                            }

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
                                        "(TRAN_ID, V_DATE, VOUCHER_NO, REF, REMARKS, SHIPMENT_DOC, BCODE, PARTY_CODE, ACT_CODE, PERIOD_ID," +
                                        "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME," +
                                        "ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE," +
                                        "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE," +
                                        "EDIT_POSTALCODE, ASTATUS, MENU_ID, DLT)" +
                                        "VALUES" +
                                        "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.REF + "','" + modelRecord.Master.REMARKS + "','" + modelRecord.Master.SHIPMENT_DOC + "','" + branch + "','" + modelRecord.Master.PARTY_CODE + "','" + modelRecord.Master.ACT_CODE + "','" + period + "'," +
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
                                                PARTY_CODE = '" + modelRecord.Master.PARTY_CODE + @"',
                                                ACT_CODE = '" + modelRecord.Master.ACT_CODE + @"',
                                                REMARKS = '" + modelRecord.Master.REMARKS + @"',
                                                SHIPMENT_DOC = '" + modelRecord.Master.SHIPMENT_DOC + @"',
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
                                                           "(TRAN_ID, DT_CODE, QTY, RATE, AMT, ALLOW, UNIT, CQTY, SHIP_DATE, BL_NO, DT_DESC, " +
                                                           "PICK_ID, BCODE," +
                                                           "PERIOD_ID,ADD_USER_ID,ADD_DATE," +
                                                           "ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                                                           "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                                           "ADD_POSTALCODE,EDIT_POSTALCODE," +
                                                           "MENU_ID,DLT)" +
                                                           "VALUES" +
                                                           "('" + modelRecord.Master.TRAN_ID + "','" + detailCode + "','" + item.QTY + "','" + item.RATE + "','" + item.AMT + "','" + item.ALLOW + "','" + item.UNIT + "','" + item.CQTY + "','" + item.SHIP_DATE + "','" + item.BL_NO + "','" + item.DT_DESC + "'," +
                                                           "'" + item.PICK_ID + "','" + branch + "'," +
                                                           "'" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                                           "'" + Computer + "','" + Ip + "','" + username + "'," +
                                                           "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                                           "'" + Postal + "','" + Postal + "','" + menuID + "','T')";
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
                                        detailQuery = $"UPDATE {detailTable} SET QTY = '" + item.QTY + @"',
                                                        UNIT = '" + item.UNIT + @"',
                                                        RATE = '" + item.RATE + @"',
                                                        AMT = '" + item.AMT + @"',
                                                        ALLOW = '" + item.ALLOW + @"',
                                                        CQTY = '" + item.CQTY + @"',
                                                        SHIP_DATE = '" + item.SHIP_DATE + @"',
                                                        BL_NO = '" + item.BL_NO + @"',
                                                        DT_DESC = '" + item.DT_DESC + @"',
                                                        EDIT_USER_ID = '" + username + @"',
                                                        EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                                        EDIT_COMPUTER_NAME = '" + Computer + @"',
                                                        EDIT_IP_ADDRESS = '" + Ip + @"',
                                                        EDIT_POSTALCODE = '" + Postal + @"',
                                                        DLT = 'T'
                                                        WHERE TRAN_ID = '" + modelRecord.Master.TRAN_ID + "' AND DT_CODE = '" + item.DT_CODE + "' AND BCODE = '" + branch + "' AND PERIOD_ID = '" + period + "'";
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

        public MyHttpResponseMessage DeleteShipmentDetailByCode(int code, Common common)
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
                        //string query = @$"SELECT SD.IMPORT_PERMIT, SD.SDOC, SD.IMPORT_PERMIT_DOC, SD.TQTY, SD.IQTY, SD.ISSUE_DATE, SD.EXP_DATE, PT.PARTY_NAME, SD.DT_DESC, SM.V_DATE, SM.VOUCHER_NO, SM.REMARKS,
                        //                SD.DT_CODE
                        //                FROM {pickMasterTable} SM
                        //                LEFT OUTER JOIN {pickDetailTable} SD
                        //                ON SD.TRAN_ID = SM.TRAN_ID AND SD.BCODE = SM.BCODE AND SD.PERIOD_ID = SM.PERIOD_ID
                        //                LEFT OUTER JOIN {detailTable} DFD
                        //                ON SD.DT_CODE = DFD.PICK_ID AND SD.BCODE = DFD.BCODE AND SD.PERIOD_ID = DFD.PERIOD_ID
                        //                AND DFD.DLT = 'T'
                        //                LEFT OUTER JOIN TBL_PARTY_TYPES PT 
                        //                ON PT.PARTY_CODE = SD.PARTY_CODE AND PT.ACT_CODE = SD.ACT_CODE 
                        //                WHERE SM.DLT = 'T' AND SM.ASTATUS = 'Y' AND SD.DLT = 'T'
                        //                AND DFD.PICK_ID IS NULL";
                        string query = @$"SELECT SD.IMPORT_PERMIT, SD.SDOC, SD.IMPORT_PERMIT_DOC, SD.TQTY, SD.IQTY, SD.ISSUE_DATE, SD.EXP_DATE, PT.PARTY_NAME, SD.DT_DESC, SM.V_DATE, SM.VOUCHER_NO, SM.REMARKS,
                                        SD.DT_CODE, SBFD.RATE, SBFM.SHIP_DATE
                                        FROM {pickMasterTable} SM
                                        LEFT OUTER JOIN {pickDetailTable} SD
                                        ON SD.TRAN_ID = SM.TRAN_ID AND SD.BCODE = SM.BCODE AND SD.PERIOD_ID = SM.PERIOD_ID
                                        LEFT OUTER JOIN {detailTable} DFD
                                        ON SD.DT_CODE = DFD.PICK_ID AND SD.BCODE = DFD.BCODE AND SD.PERIOD_ID = DFD.PERIOD_ID
                                        AND DFD.DLT = 'T'
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT 
                                        ON PT.PARTY_CODE = SD.PARTY_CODE AND PT.ACT_CODE = SD.ACT_CODE
                                        LEFT OUTER JOIN TBL_ISC_DETAIL ISDD
                                        ON ISDD.DT_CODE = SD.PICK_ID AND ISDD.BCODE = SD.BCODE AND ISDD.PERIOD_ID = SD.PERIOD_ID
                                        LEFT OUTER JOIN TBL_SBF_DETAIL SBFD
                                        ON SBFD.DT_CODE = ISDD.PICK_ID AND ISDD.BCODE = SBFD.BCODE AND ISDD.PERIOD_ID = SBFD.PERIOD_ID
                                        LEFT OUTER JOIN TBL_SBF_MASTER SBFM
                                        ON SBFM.TRAN_ID = SBFD.TRAN_ID AND SBFM.BCODE = SBFD.BCODE AND SBFM.PERIOD_ID = SBFD.PERIOD_ID
                                        WHERE SM.DLT = 'T' AND SM.ASTATUS = 'Y' AND SD.DLT = 'T'
                                        AND DFD.PICK_ID IS NULL";
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
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                IMPORT_PERMIT = Convert.ToString(reader["IMPORT_PERMIT"]),
                                SDOC = Convert.ToString(reader["SDOC"]),
                                IMPORT_PERMIT_DOC = Convert.ToString(reader["IMPORT_PERMIT_DOC"]),
                                TQTY = reader["TQTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["TQTY"]),
                                IQTY = reader["IQTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["IQTY"]),
                                RATE = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["RATE"]),
                                ISSUE_DATE = reader["ISSUE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ISSUE_DATE"]).ToString("yyyy-MM-dd"),
                                EXP_DATE = reader["EXP_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EXP_DATE"]).ToString("yyyy-MM-dd"),
                                SHIPMENT_DATE = reader["SHIP_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM-dd"),
                                PARTY_NAME = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
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