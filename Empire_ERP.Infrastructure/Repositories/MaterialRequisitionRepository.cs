using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class MaterialRequisitionRepository : IMaterialRequisitionRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }

        public MaterialRequisitionRepository(IMenuRepository menuRepository, IBranchRepository branchRepository)
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
                        string query = "SELECT TRAN_ID,V_DATE,VOUCHER_NO," +
									   "DEP_ID,REF,REMARKS,EMP_ID,BCODE,PERIOD_ID," +
									   "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
									   "EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
									   "ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS " +
									   $"FROM {table} WHERE DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "' ORDER BY TRAN_ID DESC";
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
                                DEP_ID = Convert.ToString(reader["DEP_ID"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                EMP_ID = Convert.ToString(reader["EMP_ID"]),
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

        public MyHttpResponseMessage GetMaterialRequisitionByCode(int code, Common common)
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
                                       "DEP_ID,REF,REMARKS,EMP_ID,ASTATUS " +
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
                                DEP_ID = Convert.ToString(reader["DEP_ID"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                EMP_ID = Convert.ToString(reader["EMP_ID"]),
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

        public MyHttpResponseMessage GetMaterialRequisitionDetailByCode(int code, Common common)
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
                        string query = "SELECT DT_CODE,ITEM_CODE,QTY," +
                                       "UNIT,QTY2,BAL_QTY,DT_DESC," +
                                       "COLOR, SIZE, GRADE, WAREHOUSE," +
                                       "PRIOIRTY, DEL_DATE, REQ_TYPE," +
                                       "BCODE, PERIOD_ID, CHK " +
                                       $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToInt32(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                COLOR = Convert.ToInt32(reader["COLOR"]),
                                SIZE = Convert.ToInt32(reader["SIZE"]),
                                GRADE = Convert.ToInt32(reader["GRADE"]),
                                WAREHOUSE = Convert.ToInt32(reader["WAREHOUSE"]),
                                PRIORITY = Convert.ToString(reader["PRIOIRTY"]),
                                DEL_DATE = reader["DEL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DEL_DATE"]).ToString("dd-MM-yyyy"),
                                REQ_TYPE = Convert.ToString(reader["REQ_TYPE"]),
                                CHK = Convert.ToString(reader["CHK"]),
                                CHK1 = Convert.ToString(reader["CHK"]) == "1" ? true : false,
                                
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

        public MyHttpResponseMessage Save(CustomMaterialRequisition modelRecord, Common common)
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
                                        "(TRAN_ID,V_DATE,VOUCHER_NO,DEP_ID," +
                                        "REF,REMARKS,EMP_ID,BCODE,PERIOD_ID," +
                                        "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                        "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                        "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                                        "EDIT_POSTALCODE,ASTATUS,MENU_ID,DLT)" +
                                        "VALUES" +
                                        "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.DEP_ID + "'," +
                                        "'" + modelRecord.Master.REF + "','" + modelRecord.Master.REMARKS + "','" + modelRecord.Master.EMP_ID + "','" + branch + "','" + period + "'," +
                                        "'" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                        "'" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                        "'" + Computer + "','" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.Master.ASTATUS + "','" + menuID + "','T')";
                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                query = $"UPDATE {table} SET V_DATE = '" + modelRecord.Master.V_DATE + @"',
                                                DEP_ID = '" + modelRecord.Master.DEP_ID + @"',
                                                REF = '" + modelRecord.Master.REF + @"',
                                                REMARKS = '" + modelRecord.Master.REMARKS + @"',
                                                EMP_ID = '" + modelRecord.Master.EMP_ID + @"',
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
                            var vdate = modelRecord.Master.V_DATE;

                            foreach (var item in modelRecord.Detail.ToList())
                            {
                                if (item.DEL_DATE == null || string.IsNullOrWhiteSpace(item.DEL_DATE.ToString()))
                                {
                                    item.DEL_DATE = vdate;
                                }
                                try
                                {
                                    if (item.DT_CODE == null || item.DT_CODE == 0)
                                    {
                                        int detailCode = GenerateNextDetailId(common, command);
                                        if (detailCode > 0)
                                        {
                                            detailQuery = $"INSERT INTO {detailTable}" +
                                                           "(TRAN_ID,DT_CODE,ITEM_CODE,QTY," +
                                                           "UNIT,QTY2,BAL_QTY,DT_DESC," +
                                                           "COLOR,SIZE,GRADE,WAREHOUSE," +
                                                           "PRIOIRTY,DEL_DATE,REQ_TYPE,BCODE," +
                                                           "PERIOD_ID,ADD_USER_ID,ADD_DATE," +
                                                           "ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                                                           "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                                           "ADD_POSTALCODE,EDIT_POSTALCODE," +
                                                           "MENU_ID,DLT,CHK)" +
                                                           "VALUES" +
                                                           "('" + modelRecord.Master.TRAN_ID + "','" + detailCode + "','" + item.ITEM_CODE + "','" + item.QTY + "'," +
                                                           "'" + item.UNIT + "','" + item.QTY2 + "','" + item.BAL_QTY + "','" + item.DT_DESC + "'," +
                                                           "'" + item.COLOR + "','" + item.SIZE + "','" + item.GRADE + "','" + item.WAREHOUSE + "'," +
                                                           "'" + item.PRIORITY + "','" + item.DEL_DATE + "','" + item.REQ_TYPE + "','" + branch + "'," +
                                                           "'" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                                           "'" + Computer + "','" + Ip + "','" + username + "'," +
                                                           "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                                           "'" + Postal + "','" + Postal + "','" + menuID + "','T','" + item.CHK + "')";
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
                                                        UNIT = '" + item.UNIT + @"',
                                                        QTY2 = '" + item.QTY2 + @"',
                                                        BAL_QTY = '" + item.BAL_QTY + @"',
                                                        DT_DESC = '" + item.DT_DESC + @"',
                                                        COLOR = '" + item.COLOR + @"',
                                                        SIZE = '" + item.SIZE + @"',
                                                        GRADE = '" + item.GRADE + @"',
                                                        WAREHOUSE = '" + item.WAREHOUSE + @"',
                                                        PRIOIRTY = '" + item.PRIORITY + @"',
                                                        DEL_DATE = '" + item.DEL_DATE + @"',
                                                        REQ_TYPE = '" + item.REQ_TYPE + @"',
                                                        CHK = '" + item.CHK + @"',
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
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE1;
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

                MaterialRequisition MaterialRequisition = new MaterialRequisition();
                List<MaterialRequisitionDetail> MaterialRequisitionDetailList = new List<MaterialRequisitionDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        MaterialRequisition = new MaterialRequisition
                        {
                            EMP_ID = Convert.ToString(reader["EMP_ID"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            REF = Convert.ToString(reader["REF"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            DEP_ID = Convert.ToInt32(reader["DEP_ID"]),
                            V_DATE = Convert.ToDateTime(reader["V_DATE"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new MaterialRequisitionDetail
                        {
                            ITEM_CODE = Convert.ToInt32(detail_Reader["ITEM_CODE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                            QTY2 = Convert.ToDouble(detail_Reader["QTY2"]),
                            BAL_QTY = Convert.ToDouble(detail_Reader["BAL_QTY"]),
                            UNIT = Convert.ToInt32(detail_Reader["UNIT"]),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            COLOR = Convert.ToInt32(detail_Reader["COLOR"]),
                            SIZE = Convert.ToInt32(detail_Reader["SIZE"]),
                            GRADE = Convert.ToInt32(detail_Reader["GRADE"]),
                            WAREHOUSE = Convert.ToInt32(detail_Reader["WAREHOUSE"]),
                            PRIORITY = Convert.ToString(detail_Reader["PRIOIRTY"]),
                            REQ_TYPE = Convert.ToString(detail_Reader["REQ_TYPE"]),
                            DEL_DATE = Convert.ToDateTime(detail_Reader["DEL_DATE"]),
                            CHK = detail_Reader["CHK"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["CHK"])
                        };
                        MaterialRequisitionDetailList.Add(row);
                    }
                    
                    detail_Reader.Close();
                    connection.Close();
                }

                var customRequisition = new CustomMaterialRequisition
                {
                    Master = MaterialRequisition,
                    Detail = MaterialRequisitionDetailList
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

        public MyHttpResponseMessage DeleteMaterialRequisitionDetailByCode(int code, Common common)
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

        public MyHttpResponseMessage GetDataForReport(MaterialRequisitionRDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            MaterialRequisitionRDLCReport masterData = new MaterialRequisitionRDLCReport();
            CustomMaterialRequisitionForPrintReport reportData = new CustomMaterialRequisitionForPrintReport();
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
                if (menuDetails.MD_ID == 41)
                {
                    //query = @$"SELECT 
                    //            M.V_DATE,m.VOUCHER_NO,PT.PARTY_NAME,M.ORDER_TYPE,CONVERT(NVARCHAR(20),M.TERMS)+' Days'  as Terms,
                    //            M.REF,M.DEL_DATE,M.REMARKS,
                    //            IM.ITEM_NAME,D.BAL_QTY,U.GROUP_NAME AS UNIT,D.RATE,D.AMT,D.DISC,D.DISC_AMT,D.TAX,D.TAX_AMT,D.NET_AMT
                    //            FROM {table} M
                    //            LEFT OUTER JOIN {detailTable} D
                    //            ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
                    //            LEFT OUTER JOIN TBL_ITEMSMASTER IM
                    //            ON IM.ITEM_CODE = D.ITEM_CODE
                    //            LEFT OUTER JOIN TBL_UNIT U
                    //            ON U.GROUP_CODE = D.UNIT
                    //            LEFT OUTER JOIN TBL_PARTY_TYPES PT
                    //            ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE
                    //            LEFT OUTER JOIN TBL_CURRENCY CR
                    //            ON CR.CODE = M.CURR_CODE
                    //            WHERE M.TRAN_ID = {modelRecord.TRAN_ID} AND M.BCODE = {common.Branch} AND M.PERIOD_ID = {common.Period}
                    //            AND M.ASTATUS = 'Y' AND M.DLT = 'T' AND D.DLT = 'T'";

                    query = $@"SELECT
                            M.V_DATE,M.VOUCHER_NO,M.ASTATUS,
                            AG.DESCR AS DEP_NAME,M.EMP_ID,M.REMARKS,
                            IM.ITEM_NAME,D.BAL_QTY ,D.DT_DESC,
                            CASE
                            WHEN D.PRIOIRTY = 'N' THEN 'Normal'
                            WHEN D.PRIOIRTY = 'U' THEN 'Urgent'
                            WHEN D.PRIOIRTY = 'M' THEN 'Most Urgent'
                            ELSE 'N/A'
                            END AS PRIOIRTY
                            FROM {table} M
                            LEFT OUTER JOIN {detailTable} D
                            ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
                            LEFT OUTER JOIN TBL_ACT_GROUP AG
                            ON AG.CODE = M.DEP_ID
                            LEFT OUTER JOIN TBL_ITEMSMASTER IM
                            ON IM.ITEM_CODE = D.ITEM_CODE
                            WHERE M.DLT = 'T' AND D.DLT = 'T'
                            AND M.TRAN_ID = {modelRecord.TRAN_ID} AND M.BCODE = {common.Branch} AND M.PERIOD_ID = {common.Period}";
                }

                masterData.COMPANY_NAME = currentCompany.C_NAME;
                masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                masterData.COMPANY_PHONE = currentCompany.C_TEL;
                masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                masterData.SIG1 = $"{menuDetails.MENU_SIG1}";
                masterData.SIG2 = $"{menuDetails.MENU_SIG2}";
                masterData.SIG3 = $"{menuDetails.MENU_SIG3}";
                masterData.SIG4 = $"{menuDetails.MENU_SIG4}";
                masterData.MENU_TERMS = $"{menuDetails.MENU_TERMS}";

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow["Item"] = Convert.ToString(reader["ITEM_NAME"]);
                        dataRow["Qty"] = reader["BAL_QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BAL_QTY"]);
                        dataRow["Voucher"] = Convert.ToString(reader["VOUCHER_NO"]);
                        dataRow["Status"] = Convert.ToString(reader["ASTATUS"]) == "Y" ? "Active" : "InActive";
                        dataRow["Descr"] = Convert.ToString(reader["DT_DESC"]);
                        dataRow["Department"] = Convert.ToString(reader["DEP_NAME"]);
                        dataRow["Employee"] = Convert.ToString(reader["EMP_ID"]);
                        dataRow["Date"] = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                        dataRow["Remarks"] = Convert.ToString(reader["REMARKS"]);
                        dataRow["Prioirty"] = Convert.ToString(reader["PRIOIRTY"]);
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