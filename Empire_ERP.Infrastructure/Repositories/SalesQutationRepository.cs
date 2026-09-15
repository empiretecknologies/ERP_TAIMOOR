using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class SalesQutationRepository : ISalesQutationRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }

        public SalesQutationRepository(IMenuRepository menuRepository, IBranchRepository branchRepository)
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
                        string query = "SELECT M.TRAN_ID,M.V_DATE,M.VOUCHER_NO," +
									   "M.PARTY_CODE,M.ACT_CODE,M.REMARKS,M.BCODE,M.PERIOD_ID," +
									   "M.ADD_USER_ID,M.ADD_DATE,M.ADD_COMPUTER_NAME,M.ADD_IP_ADDRESS," +
									   "M.EDIT_USER_ID,M.EDIT_DATE,M.EDIT_COMPUTER_NAME,M.EDIT_IP_ADDRESS," +
									   "M.ADD_POSTALCODE,M.EDIT_POSTALCODE,M.ASTATUS,PT.PARTY_NAME " +
									   $"FROM {table} M " +
                                       $"LEFT OUTER JOIN TBL_PARTY_TYPES PT " +
                                       $"ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE " +
                                       $"WHERE M.DLT = 'T' AND M.BCODE = '" + common.Branch + "' AND M.PERIOD_ID = '" + common.Period + "' ORDER BY M.TRAN_ID DESC";
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
                                PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]),
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

        public MyHttpResponseMessage GetSalesQutationByCode(int code, Common common)
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
                        string query = "SELECT TRAN_ID,V_DATE,VOUCHER_NO," +
                                       "PARTY_CODE,ACT_CODE,REMARKS,ASTATUS " +
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
                                PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
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

        public MyHttpResponseMessage GetSalesQutationDetailByCode(int code, Common common)
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
                        string query = "SELECT DT_CODE,ITEM_CODE,QTY,RATE,BARCODE " +
                                       $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            double qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["QTY"]);
                            double rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["RATE"]);
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                RATE = reader["RATE"] == DBNull.Value ? "" : Convert.ToString(reader["RATE"]),
                                BARCODE = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                AMT = qty * rate,
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

        public MyHttpResponseMessage GetPartyLastItemRates(int partyCode, int actCode, Common common)
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

                List<object> jsonDataResult = new List<object>();
                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable) && partyCode > 0)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"SELECT ITEM_CODE, RATE, BARCODE
                                        FROM (
                                            SELECT D.ITEM_CODE, D.RATE, D.BARCODE,
                                                   ROW_NUMBER() OVER (PARTITION BY D.ITEM_CODE ORDER BY M.TRAN_ID DESC, D.DT_CODE DESC) AS RN
                                            FROM {detailTable} D
                                            INNER JOIN {table} M
                                                ON M.TRAN_ID = D.TRAN_ID
                                               AND M.BCODE = D.BCODE
                                               AND M.PERIOD_ID = D.PERIOD_ID
                                            WHERE M.DLT = 'T'
                                              AND D.DLT = 'T'
                                              AND M.PARTY_CODE = '{partyCode}'
                                              AND M.ACT_CODE = '{actCode}'
                                              AND M.BCODE = '{common.Branch}'
                                              AND M.PERIOD_ID = '{common.Period}'
                                        ) X
                                        WHERE RN = 1";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                RATE = reader["RATE"] == DBNull.Value ? "" : Convert.ToString(reader["RATE"]),
                                BARCODE = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
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

        public MyHttpResponseMessage Save(CustomSalesQutation modelRecord, Common common)
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
                                        "(TRAN_ID,V_DATE,VOUCHER_NO,PARTY_CODE," +
                                        "ACT_CODE,REMARKS,BCODE,PERIOD_ID," +
                                        "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                        "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                        "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                                        "EDIT_POSTALCODE,ASTATUS,MENU_ID,DLT)" +
                                        "VALUES" +
                                        "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.PARTY_CODE + "'," +
                                        "'" + modelRecord.Master.ACT_CODE + "','" + modelRecord.Master.REMARKS + "','" + branch + "','" + period + "'," +
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
                                                ACT_CODE = '" + modelRecord.Master.ACT_CODE + @"',
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
                                                           "(TRAN_ID,DT_CODE,ITEM_CODE,QTY,RATE,BARCODE," +
                                                           "BCODE,PERIOD_ID,ADD_USER_ID,ADD_DATE," +
                                                           "ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                                                           "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                                           "ADD_POSTALCODE,EDIT_POSTALCODE," +
                                                           "MENU_ID,DLT)" +
                                                           "VALUES" +
                                                           "('" + modelRecord.Master.TRAN_ID + "','" + detailCode + "','" + item.ITEM_CODE + "','" + item.QTY + "','" + item.RATE + "','" + item.BARCODE + "'," +
                                                           "'" + branch + "','" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
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
                                        detailQuery = $"UPDATE {detailTable} SET ITEM_CODE = '" + item.ITEM_CODE + @"',
                                                        QTY = '" + item.QTY + @"',
                                                        RATE = '" + item.RATE + @"',
                                                        BARCODE = '" + item.BARCODE + @"',
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

        public MyHttpResponseMessage GetPartyBranches(int partyCode, int actCode)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $@"SELECT BRANCH_NAME
                                      FROM TBL_PARTY_BRANCHES
                                      WHERE DLT = 'T'
                                      AND PARTY_CODE = '{partyCode}'
                                      AND ACT_CODE = '{actCode}'
                                      ORDER BY BRANCH_NAME";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            branchName = Convert.ToString(reader["BRANCH_NAME"])
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

        public MyHttpResponseMessage GetExcelItemLookup()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = @"SELECT ITEM_CODE, ITEM_ID, ITEM_NAME
                                     FROM TBL_ITEMSMASTER
                                     WHERE DLT = 'T' AND ASTATUS = 'Y'";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            itemCode = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                            itemId = Convert.ToString(reader["ITEM_ID"]),
                            itemName = Convert.ToString(reader["ITEM_NAME"])
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

        private HashSet<int> GetExistingItemCodes(IEnumerable<int> itemCodes)
        {
            HashSet<int> existing = new HashSet<int>();
            var codes = itemCodes.Where(c => c > 0).Distinct().ToList();
            if (codes.Count == 0)
            {
                return existing;
            }

            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                connection.Open();
                for (int i = 0; i < codes.Count; i += 500)
                {
                    var chunk = codes.Skip(i).Take(500).ToList();
                    string inList = string.Join(",", chunk);
                    string query = $"SELECT ITEM_CODE FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y' AND ITEM_CODE IN ({inList})";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["ITEM_CODE"] != DBNull.Value)
                        {
                            existing.Add(Convert.ToInt32(reader["ITEM_CODE"]));
                        }
                    }
                    reader.Close();
                }
            }
            return existing;
        }

        public MyHttpResponseMessage SaveExcelBatch(List<CustomSalesQutation> modelRecords, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                if (modelRecords == null || modelRecords.Count == 0)
                {
                    response.msg = "No valid Master Records to save.";
                    response.msgType = 2;
                    return response;
                }

                var preparedRecords = new List<CustomSalesQutation>();
                foreach (var record in modelRecords)
                {
                    if (record == null || record.Master == null)
                    {
                        continue;
                    }
                    if (record.Master.PARTY_CODE == null || record.Master.PARTY_CODE == 0)
                    {
                        response.msg = "Please select Party Type.";
                        response.msgType = 2;
                        return response;
                    }
                    if (record.Master.V_DATE == null)
                    {
                        response.msg = "Transaction date is required.";
                        response.msgType = 2;
                        return response;
                    }

                    var validDetails = new List<SalesQutationDetail>();
                    foreach (var item in record.Detail ?? new List<SalesQutationDetail>())
                    {
                        if (item == null || item.ITEM_CODE == null || item.ITEM_CODE == 0)
                        {
                            continue;
                        }
                        if (item.QTY == null || item.QTY <= 0)
                        {
                            continue;
                        }
                        if (item.RATE == null || item.RATE < 0)
                        {
                            response.msg = "Please enter correct item rate.";
                            response.msgType = 2;
                            return response;
                        }
                        validDetails.Add(item);
                    }

                    if (validDetails.Count == 0)
                    {
                        continue;
                    }

                    record.Master.TRAN_ID = null;
                    record.Detail = validDetails;
                    preparedRecords.Add(record);
                }

                if (preparedRecords.Count == 0)
                {
                    response.msg = "No valid Master Records to save.";
                    response.msgType = 2;
                    return response;
                }

                var itemCodes = preparedRecords
                    .SelectMany(r => r.Detail.Select(d => d.ITEM_CODE ?? 0))
                    .Where(c => c > 0)
                    .Distinct()
                    .ToList();
                var existingItemCodes = GetExistingItemCodes(itemCodes);
                var missingItem = itemCodes.FirstOrDefault(c => !existingItemCodes.Contains(c));
                if (missingItem > 0)
                {
                    response.msg = "Invalid ItemCode: " + missingItem + ".";
                    response.msgType = 2;
                    return response;
                }

                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                }

                if (String.IsNullOrWhiteSpace(table) || String.IsNullOrWhiteSpace(detailTable))
                {
                    response.data = "";
                    response.msg = "Something went wrong! please try again later.";
                    response.msgType = 2;
                    return response;
                }

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
                        int nextMasterId = GenerateNextId(common, command);
                        int nextDetailId = GenerateNextDetailId(common, command);
                        if (nextMasterId <= 0 || nextDetailId <= 0)
                        {
                            transaction.Rollback();
                            response.msg = "Something went wrong! please try again later.";
                            response.msgType = 2;
                            return response;
                        }

                        int savedCount = 0;
                        foreach (var modelRecord in preparedRecords)
                        {
                            int code = nextMasterId;
                            nextMasterId += 1;
                            string voucherNo = GenerateVoucherNo(common, code, CommonService.GetDateTime("Pakistan Standard Time"));
                            if (String.IsNullOrWhiteSpace(voucherNo))
                            {
                                transaction.Rollback();
                                response.msg = "Something went wrong! please try again later.";
                                response.msgType = 2;
                                return response;
                            }

                            string query = $"INSERT INTO {table}" +
                                    "(TRAN_ID,V_DATE,VOUCHER_NO,PARTY_CODE," +
                                    "ACT_CODE,REMARKS,BCODE,PERIOD_ID," +
                                    "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                    "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                    "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                                    "EDIT_POSTALCODE,ASTATUS,MENU_ID,DLT)" +
                                    "VALUES" +
                                    "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.PARTY_CODE + "'," +
                                    "'" + modelRecord.Master.ACT_CODE + "','" + modelRecord.Master.REMARKS + "','" + branch + "','" + period + "'," +
                                    "'" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                    "'" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                    "'" + Computer + "','" + Ip + "','" + Postal + "','" + Postal + "','" + (string.IsNullOrWhiteSpace(modelRecord.Master.ASTATUS) ? "Y" : modelRecord.Master.ASTATUS) + "','" + menuID + "','T')";
                            command.CommandText = query;
                            command.ExecuteNonQuery();

                            foreach (var item in modelRecord.Detail)
                            {
                                int detailCode = nextDetailId;
                                nextDetailId += 1;
                                string detailQuery = $"INSERT INTO {detailTable}" +
                                               "(TRAN_ID,DT_CODE,ITEM_CODE,QTY,RATE,BARCODE," +
                                               "BCODE,PERIOD_ID,ADD_USER_ID,ADD_DATE," +
                                               "ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                                               "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                               "ADD_POSTALCODE,EDIT_POSTALCODE," +
                                               "MENU_ID,DLT)" +
                                               "VALUES" +
                                               "('" + code + "','" + detailCode + "','" + item.ITEM_CODE + "','" + item.QTY + "','" + item.RATE + "','" + (item.BARCODE ?? "") + "'," +
                                               "'" + branch + "','" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                               "'" + Computer + "','" + Ip + "','" + username + "'," +
                                               "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                               "'" + Postal + "','" + Postal + "','" + menuID + "','T')";
                                command.CommandText = detailQuery;
                                command.ExecuteNonQuery();
                            }
                            savedCount += 1;
                        }

                        transaction.Commit();
                        response.data = new { savedCount = savedCount };
                        response.msgType = 1;
                        response.msg = savedCount == 1
                            ? "Record Added Successfully"
                            : savedCount + " Master Records added successfully.";
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
				string? table = string.Empty, detailTable = string.Empty;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE1;
                    detailTable = menu.TABLE2;
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
                            SqlTransaction transaction = connection.BeginTransaction();
                            SqlCommand command = connection.CreateCommand();
                            command.Transaction = transaction;
                            try
                            {
                                string query = $"UPDATE {table} SET DLT = 'F' WHERE TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                if (!String.IsNullOrWhiteSpace(detailTable))
                                {
                                    string detailQuery = $"UPDATE {detailTable} SET DLT = 'F' WHERE TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                                    command.CommandText = detailQuery;
                                    command.ExecuteNonQuery();
                                }
                                transaction.Commit();
                                response.msgType = 1;
                                response.msg = "Record Deleted Successfully";
                            }
                            catch (Exception)
                            {
                                transaction.Rollback();
                                throw;
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

        public MyHttpResponseMessage DeleteSalesQutationDetailByCode(int code, Common common)
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

        public MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Branch currentBranch, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            SalesQutationForPrint masterData = new SalesQutationForPrint();
            CustomSalesQutationForPrintReport reportData = new CustomSalesQutationForPrintReport();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? tableMaster = string.Empty;
                string? tableDetail = string.Empty;
                string? menuName = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    tableMaster = menu.TABLE1;
                    tableDetail = menu.TABLE2;
                    menuName = menu.MENU_NAME;
                }
                masterData.COMPANY_NAME = currentCompany.C_NAME;
                masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                masterData.COMPANY_PHONE = currentCompany.C_TEL;
                masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                masterData.HEADER_NAME = !String.IsNullOrWhiteSpace(menuDetails.MD_NAME) ? menuDetails.MD_NAME : (!String.IsNullOrWhiteSpace(menuName) ? menuName : "Sales Quotation");
                masterData.REPORT_NAME = !String.IsNullOrWhiteSpace(menuDetails.REPORT_NAME) ? menuDetails.REPORT_NAME : "SalesQutationPrintReport";
                masterData.MENU_SIG1 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG1) ? true : false;
                masterData.MENU_SIG2 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG2) ? true : false;
                masterData.MENU_SIG3 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG3) ? true : false;
                masterData.MENU_SIG4 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG4) ? true : false;

                if (!String.IsNullOrWhiteSpace(tableMaster))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT M.V_DATE, M.VOUCHER_NO, M.REMARKS, PT.PARTY_NAME " +
                                       $"FROM {tableMaster} M " +
                                       $"LEFT OUTER JOIN TBL_PARTY_TYPES PT " +
                                       $"ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE " +
                                       $"WHERE M.DLT = 'T' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}' AND M.TRAN_ID = {modelRecord.TRAN_ID}";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            masterData.V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]);
                            masterData.VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]);
                            masterData.REMARKS = Convert.ToString(reader["REMARKS"]);
                            masterData.PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]);
                        }
                        reader.Close();
                    }
                }

                if (!String.IsNullOrWhiteSpace(tableDetail))
                {
                    if (!dataTable.Columns.Contains("ItemName"))
                    {
                        dataTable.Columns.Add("ItemName", typeof(string));
                        dataTable.Columns.Add("Qty", typeof(decimal));
                        dataTable.Columns.Add("Rate", typeof(decimal));
                        dataTable.Columns.Add("Amount", typeof(decimal));
                    }
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT IM.ITEM_NAME, D.QTY, D.RATE " +
                                       $"FROM {tableDetail} D " +
                                       $"LEFT JOIN TBL_ITEMSMASTER IM WITH (NOLOCK) ON D.ITEM_CODE = IM.ITEM_CODE " +
                                       $"WHERE D.DLT = 'T' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}' AND D.TRAN_ID = {modelRecord.TRAN_ID} " +
                                       $"ORDER BY D.DT_CODE";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            decimal qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]);
                            decimal rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]);
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["ItemName"] = Convert.ToString(reader["ITEM_NAME"]);
                            dataRow["Qty"] = qty;
                            dataRow["Rate"] = rate;
                            dataRow["Amount"] = qty * rate;
                            dataTable.Rows.Add(dataRow);
                        }
                        reader.Close();
                    }
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
