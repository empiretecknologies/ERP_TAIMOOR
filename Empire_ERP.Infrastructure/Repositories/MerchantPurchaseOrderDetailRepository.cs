using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class MerchantPurchaseOrderDetailRepository : IMerchantPurchaseOrderDetailRepository
    {
        public IBranchRepository _branchRepository { get; set; }
        public IMenuService _menuRepository { get; set; }
        public MerchantPurchaseOrderDetailRepository(IBranchRepository branchRepository, IMenuService menuRepository)
        {
            _branchRepository = branchRepository;
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common, Menu menu)
		{
			MyHttpResponseMessage response = new MyHttpResponseMessage();
			try
			{
				string? table = menu.TABLE1;
				string? pickTableMaster = menu.PICK_TABLE_MASTER;
				List<object> jsonDataResult = new List<object>();
				using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
				{
                    string query = $@"SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, MPO.CLIENT_PO, MPO.JOB_NO, A.REF, A.REMARKS,A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, 
                                        A.ADD_IP_ADDRESS,A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS,A.ADD_POSTALCODE, A.EDIT_POSTALCODE, 
                                        CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS , FB.GROUP_NAME AS FABRIC, GSM.GROUP_NAME AS GSM
                                        FROM {table} A 
                                        LEFT OUTER JOIN {pickTableMaster} MPO ON MPO.TRAN_ID = A.JOB_NO
                                        LEFT OUTER JOIN TBL_FABRIC FB ON A.FABRIC = FB.GROUP_CODE
                                        LEFT OUTER JOIN TBL_GSM GSM ON A.GSM = GSM.GROUP_CODE
                                        WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' ORDER BY A.TRAN_ID DESC";

                    SqlCommand command = new SqlCommand(query, connection);
					connection.Open();
					SqlDataReader reader = command.ExecuteReader();
					while (reader.Read())
					{
						var row = new
						{
                            TRAN_ID = reader["TRAN_ID"] == DBNull.Value ? null : Convert.ToString(reader["TRAN_ID"]),
                            ASTATUS = reader["ASTATUS"] == DBNull.Value ? null : Convert.ToString(reader["ASTATUS"]),
                            V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                            VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? null : Convert.ToString(reader["VOUCHER_NO"]),
                            REF = reader["REF"] == DBNull.Value ? null : Convert.ToString(reader["REF"]),
                            REMARKS = reader["REMARKS"] == DBNull.Value ? null : Convert.ToString(reader["REMARKS"]),
                            ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                            ADD_DATE = Convert.ToDateTime(reader["ADD_DATE"]),
                            ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                            ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                            EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                            EDIT_DATE = Convert.ToDateTime(reader["EDIT_DATE"]),
                            EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                            EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                            ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                            EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                            CLIENT_PO = Convert.ToString(reader["CLIENT_PO"]),
                            JOB_NO = Convert.ToString(reader["JOB_NO"]),
                            FABRIC = reader["FABRIC"].ToString(),
                            GSM = reader["GSM"].ToString(),
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

        private int GenerateNextId(Common common, SqlCommand command, Menu menu)
        {
            try
            {
                string? table = menu.TABLE1;
                string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM {table} WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                command.CommandText = maxIdQuery;
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        private string GenerateVoucherNo(Common common, int code, string vDate, Menu menu)
        {
            try
            {
                string? prefix = menu.PERFIX, shortName = string.Empty;
                int voucherLength = Convert.ToInt32(menu.VOUCHER_LEN);
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

        private int GenerateNextDetailId(SqlCommand command, Menu menu)
        {
            try
            {
                string? table = menu.TABLE2;
                string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) + 1 FROM {table}";
                command.CommandText = maxIdQuery;
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public MyHttpResponseMessage Save(CustomMerchantPurchaseOrderDetail modelRecord, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1, detailTable = menu.TABLE2;
                var ip = common.IPAddress;
                var computerName = common.ComputerName;
                var postalCode = common.PostalCode;
                var username = common.Username;
                var branch = common.Branch;
                var periodID = common.Period;
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
                        string query = "", detailQuery = "", voucherNo = string.Empty;
                        bool IsMasterAdded = true, IsNew = false;
                        int code = 0;
                        var partyInformation = partiesData.FirstOrDefault(p => int.TryParse(p.customizedKey, out int key) && key == modelRecord.Master.PARTY_CODE);
                        var supplierInformation = partiesData.FirstOrDefault(p => int.TryParse(p.customizedKey, out int key) && key == modelRecord.Master.SPARTY_CODE);

                        if (partyInformation != null)
                        {
                            modelRecord.Master.PARTY_CODE = partyInformation.key;
                            modelRecord.Master.ACT_CODE = partyInformation.accountCode;
                        }

                        if (supplierInformation != null)
                        {
                            modelRecord.Master.SPARTY_CODE = supplierInformation.key;
                            modelRecord.Master.SACT_CODE = supplierInformation.accountCode;
                        }

                        if (modelRecord.Master.TRAN_ID == null || modelRecord.Master.TRAN_ID == 0)
                        {
                            IsNew = true;
                            code = GenerateNextId(common, command, menu);

                            if (code > 0)
                            {
                                modelRecord.Master.TRAN_ID = code;
                                voucherNo = GenerateVoucherNo(common, code, CommonService.GetDateTime("Pakistan Standard Time"), menu);
                                if (String.IsNullOrWhiteSpace(voucherNo))
                                {
                                    IsMasterAdded = false;
                                }
                            }
                            else
                            {
                                IsMasterAdded = false;
                            }

                            query = $"INSERT INTO {table} " +
                                    "(TRAN_ID, V_DATE, VOUCHER_NO, REF,SPARTY_CODE, SACT_CODE, REMARKS, BCODE, " +
                                    "PERIOD_ID, JOB_NO, FABRIC, GSM, ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, " +
                                    "ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, " +
                                    "EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, " +
                                    "MENU_ID, DLT, COMM, COMM_VAL, COMM_AMT) " +
                                    $"VALUES " +
                                    $"('{code}', '{modelRecord.Master.V_DATE}', '{voucherNo}'," +
                                    $"'{modelRecord.Master.REF}','{modelRecord.Master.SPARTY_CODE}','{modelRecord.Master.SACT_CODE}', " +
                                    $"'{modelRecord.Master.REMARKS}' , '{branch}', '{periodID}', '{modelRecord.Master.JOB_NO}', '{modelRecord.Master.FABRIC}', '{modelRecord.Master.GSM}'," +
                                    $"'{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{computerName}'," +
                                    $"'{ip}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}'," +
                                    $"'{computerName}', '{ip}', " +
                                    $"'{postalCode}', '{postalCode}', " +
                                    $"'{modelRecord.Master.ASTATUS}', '{menuID}', 'T', '{modelRecord.Master.COMM}', '{modelRecord.Master.COMM_VAL}', '{modelRecord.Master.COMM_AMT}')";

                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }
                        else
                        {
                            query = $"UPDATE {table} " +
                                    $"SET V_DATE = '{modelRecord.Master.V_DATE}', " +
                                    $"REF = '{modelRecord.Master.REF}', " +
                                    $"SPARTY_CODE = '{modelRecord.Master.SPARTY_CODE}', " +
                                    $"SACT_CODE = '{modelRecord.Master.SACT_CODE}', " +
                                    $"FABRIC = '{modelRecord.Master.FABRIC}', " +
                                    $"GSM = '{modelRecord.Master.GSM}', " +
                                    $"REMARKS = '{modelRecord.Master.REMARKS}', " +
                                    $"EDIT_USER_ID = '{modelRecord.Master.EDIT_USER_ID}', " +
                                    $"EDIT_DATE = '{modelRecord.Master.EDIT_DATE}', " +
                                    $"EDIT_COMPUTER_NAME = '{modelRecord.Master.EDIT_COMPUTER_NAME}', " +
                                    $"EDIT_IP_ADDRESS = '{modelRecord.Master.EDIT_IP_ADDRESS}', " +
                                    $"EDIT_POSTALCODE = '{modelRecord.Master.EDIT_POSTALCODE}', " +
                                    $"ASTATUS = '{modelRecord.Master.ASTATUS}' " +
                                    $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{periodID}'";

                            command.CommandText = query;
                            command.ExecuteNonQuery();
                        }

                        var isDetailAdded = true;

                        //if (modelRecord.Detail.Count > 0)
                        //{
                        //    detailQuery = $"UPDATE {detailTable} SET DLT = 'F'" +
                        //    $" WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{periodID}'";
                        //    command.CommandText = detailQuery;
                        //    command.ExecuteNonQuery();
                        //}
                        foreach (var item in modelRecord.Detail.ToList())
                        {
                            try
                            {

                                if (item.DT_CODE == null || item.DT_CODE == 0)
                                {
                                    int detailCode = GenerateNextDetailId(command, menu);
                                    if (detailCode > 0)
                                    {
                                        detailQuery = $@"INSERT INTO {detailTable}
                                                       (TRAN_ID, DT_CODE, ORDER_NO, COMM_TYPE, COMM_RATE, COMM_AMT, SIZE, COLOR, QTY, UNIT, PICK_ID,
                                                       RATE, AMT, LC_PORT,DOC, SHIP_DATE, BOOKING_DATE, HANDOVER_DATE, INTAKE, REV_STATUS, REV_REF, SENTITY_CODE,
                                                       DCHANNEL_CODE, GRADE_CODE, DT_DESC, ITEM_CODE, SEASON_CODE,
                                                       BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE,
                                                       ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID,
                                                       EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS,
                                                       ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT)
                                                       VALUES
                                                       ('{modelRecord.Master.TRAN_ID}', '{detailCode}', '{item.ORDER_NO}', '{item.COMM_TYPE}', '{item.COMM_RATE}', '{item.COMM_AMT}',
                                                       '{item.SIZE}','{item.COLOR}','{item.QTY}', '{item.UNIT}', '{modelRecord.Master.PICK_ID}',
                                                       '{item.RATE}', '{item.AMT}', '{item.PORT}','{item.DOC}',
                                                       '{item.SHIP_DATE}', '{item.BOOKING_DATE}', '{item.HANDOVER_DATE}', '{item.INTAKE}', '{item.REV_STATUS}', {detailCode}, '{item.SENTITY_CODE}', '{item.DCHANNEL_CODE}',
                                                       '{item.GRADE_CODE}', '{item.DT_DESC}', '{item.ITEM_CODE}', '{item.SEASON_CODE}', '{branch}', '{periodID}', '{username}',
                                                       '{CommonService.GetDateTime("Pakistan Standard Time")}', '{computerName}',
                                                       '{ip}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                       '{computerName}', '{ip}', '{postalCode}',
                                                       '{postalCode}', '{menuID}', 'T')";

                                        command.CommandText = detailQuery;
                                        command.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        isDetailAdded = false;
                                    }
                                }
                                else if ((item.DT_CODE != null || item.DT_CODE != 0) && item.REV_TOGGLE == true)
                                {
                                    int detailCode = GenerateNextDetailId(command, menu);
                                    if (detailCode > 0)
                                    {
                                        detailQuery = $@"INSERT INTO {detailTable}
                                                       (TRAN_ID, DT_CODE, ORDER_NO, COMM_TYPE, COMM_RATE, COMM_AMT, SIZE, COLOR, QTY, UNIT, PICK_ID,
                                                       RATE, AMT, LC_PORT,DOC, SHIP_DATE, BOOKING_DATE, HANDOVER_DATE, INTAKE, REV_STATUS, REV_REF, SENTITY_CODE,
                                                       DCHANNEL_CODE, GRADE_CODE, DT_DESC, ITEM_CODE, SEASON_CODE,
                                                       BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE,
                                                       ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID,
                                                       EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS,
                                                       ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT)
                                                       VALUES
                                                       ('{modelRecord.Master.TRAN_ID}', '{detailCode}', '{item.ORDER_NO}', '{item.COMM_TYPE}', '{item.COMM_RATE}', '{item.COMM_AMT}',
                                                       '{item.SIZE}','{item.COLOR}','{item.QTY}', '{item.UNIT}', '{modelRecord.Master.PICK_ID}',
                                                       '{item.RATE}', '{item.AMT}', '{item.PORT}','{item.DOC}',
                                                       '{item.SHIP_DATE}', '{item.BOOKING_DATE}', '{item.HANDOVER_DATE}', '{item.INTAKE}', '{item.REV_STATUS}', {item.REV_REF}, '{item.SENTITY_CODE}', '{item.DCHANNEL_CODE}',
                                                       '{item.GRADE_CODE}', '{item.DT_DESC}', '{item.ITEM_CODE}', '{item.SEASON_CODE}', '{branch}', '{periodID}', '{username}',
                                                       '{CommonService.GetDateTime("Pakistan Standard Time")}', '{computerName}',
                                                       '{ip}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                       '{computerName}', '{ip}', '{postalCode}',
                                                       '{postalCode}', '{menuID}', 'T')";

                                        command.CommandText = detailQuery;
                                        command.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        isDetailAdded = false;
                                    }
                                }
                                else if ((item.DT_CODE != null || item.DT_CODE != 0) && item.REV_TOGGLE != true)
                                {
                                    string revStatusCheckQuery = $@"SELECT REV_STATUS FROM TBL_MPOD_DETAIL WHERE DT_CODE = '{item.DT_CODE}'";
                                    command.CommandText = revStatusCheckQuery;
                                    object revStatus = command.ExecuteScalar();
                                    string revStatusStr = revStatus?.ToString().Trim();


                                    if (revStatusStr != item.REV_STATUS)
                                    {
                                        response.msgType = 2;
                                        response.msg = $"You Cannot change Revised entry to New entry.";
                                        return response;
                                    }

                                    detailQuery = $"UPDATE {detailTable} " +
                                                  $"SET ORDER_NO = '{item.ORDER_NO}', " +
                                                  $"PICK_ID = '{modelRecord.Master.PICK_ID}', " +
                                                  $"SIZE = '{item.SIZE}', " +
                                                  $"COLOR = '{item.COLOR}', " +
                                                  $"QTY = '{item.QTY}', " +
                                                  $"UNIT = '{item.UNIT}', " +
                                                  $"RATE = '{item.RATE}', " +
                                                  $"AMT = '{item.AMT}', " +
                                                  $"COMM_TYPE = '{item.COMM_TYPE}', " +
                                                  $"COMM_RATE = '{item.COMM_RATE}', " +
                                                  $"COMM_AMT = '{item.COMM_AMT}', " +
                                                  $"LC_PORT = '{item.PORT}', " +
                                                  $"REV_STATUS = '{item.REV_STATUS}', " +
                                                  $"DOC = '{item.DOC}', " +
                                                  $"SHIP_DATE = '{item.SHIP_DATE}', " +
                                                  $"BOOKING_DATE = '{item.BOOKING_DATE}', " +
                                                  $"HANDOVER_DATE = '{item.HANDOVER_DATE}', " +
                                                  $"GRADE_CODE = '{item.GRADE_CODE}', " +
                                                  $"SEASON_CODE = '{item.SEASON_CODE}', " +
                                                  $"INTAKE = '{item.INTAKE}', " +
                                                  $"SENTITY_CODE = '{item.SENTITY_CODE}', " +
                                                  $"DCHANNEL_CODE = '{item.DCHANNEL_CODE}', " +
                                                  $"DT_DESC = '{item.DT_DESC}', " +
                                                  $"ITEM_CODE = '{item.ITEM_CODE}', " +
                                                  $"EDIT_USER_ID = '{username}', " +
                                                  $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                  $"EDIT_COMPUTER_NAME = '{computerName}', " +
                                                  $"EDIT_IP_ADDRESS = '{ip}', " +
                                                  $"EDIT_POSTALCODE = '{postalCode}', " +
                                                  $"DLT = 'T' " +
                                                  $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND DT_CODE = '{item.DT_CODE}' AND BCODE = '{branch}' AND PERIOD_ID = '{periodID}'";

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

        public MyHttpResponseMessage GetMerchantPurchaseOrderDetailByCode(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    //string query = "SELECT TRAN_ID, V_DATE,JOB_NO , VOUCHER_NO, SPARTY_CODE, SACT_CODE, FABRIC, GSM, REMARKS, REF,ASTATUS " +
                    //               $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                    //               $"AND PERIOD_ID = '{common.Period}'";

                    string query = $@"SELECT MPOD.TRAN_ID, MPOD.V_DATE, MPOD.JOB_NO , MPOD.VOUCHER_NO, MPOD.SPARTY_CODE, MPOD.SACT_CODE, MPOD.FABRIC, MPOD.GSM, MPOD.REMARKS, MPOD.REF, MPOD.ASTATUS , MPO.QTY AS PICK_QTY
                                    FROM {table} MPOD
                                    LEFT OUTER JOIN TBL_MPO_MASTER MPO ON MPO.TRAN_ID = MPOD.JOB_NO
                                    WHERE MPOD.DLT = 'T' AND MPOD.TRAN_ID = '{code}' AND MPOD.BCODE = '{common.Branch}' AND MPOD.PERIOD_ID = '{common.Period}'";
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
                            REF = Convert.ToString(reader["REF"]),
                            JOB_NO = Convert.ToString(reader["JOB_NO"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            //PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                            //PARTY_CODE = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",
                            //ACT_CODE = Convert.ToString(reader["ACT_CODE"]),
                            SPARTY_CODE = $"{Convert.ToString(reader["SPARTY_CODE"])}{Convert.ToString(reader["SACT_CODE"])}",
                            SACT_CODE = Convert.ToString(reader["SACT_CODE"]),
                            //CLIENT_PO = reader["CLIENT_PO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CLIENT_PO"]),
                            FABRIC = reader["FABRIC"] == DBNull.Value ? 0 : Convert.ToInt32(reader["FABRIC"]),
                            GSM = reader["GSM"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GSM"]),
                            PICK_QTY = reader["PICK_QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PICK_QTY"]),

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

        public MyHttpResponseMessage GetMerchantPurchaseOrderDetailDetailByCode(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE2;
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string querRy = "SELECT PICK_ID, DT_CODE,ITEM_CODE, ORDER_NO,SIZE,COLOR, QTY, UNIT, RATE, COMM_TYPE, COMM_RATE, COMM_AMT, AMT, LC_PORT, SHIP_DATE, BOOKING_DATE, HANDOVER_DATE, " +
                                   "INTAKE, SENTITY_CODE, DCHANNEL_CODE, DOC, GRADE_CODE, DT_DESC, REV_STATUS, REV_REF, SEASON_CODE " +
                                   $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                   $"AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";

                    string query = $@"WITH GroupStatus AS
                                        (
                                            SELECT 
                                                REV_REF,
                                                HasC = MAX(CASE WHEN REV_STATUS = 'C' THEN 1 ELSE 0 END),
                                                HasR = MAX(CASE WHEN REV_STATUS = 'R' THEN 1 ELSE 0 END)
                                            FROM {table}
                                            GROUP BY REV_REF
                                        ),
                                        Final AS
                                        (
                                            SELECT t.*
                                            FROM {table} t
                                            JOIN GroupStatus g ON t.REV_REF = g.REV_REF
                                            WHERE g.HasC = 0
                                                AND ((g.HasR = 1 AND t.REV_STATUS = 'R' AND t.DT_CODE = (
                                                            SELECT MAX(DT_CODE) 
                                                            FROM {table} 
                                                            WHERE REV_REF = t.REV_REF AND REV_STATUS = 'R')) OR (g.HasR = 0 AND t.REV_STATUS = 'N')))
                                        SELECT * FROM Final WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'
                                        ORDER BY REV_REF, DT_CODE;";


                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            DT_CODE = Convert.ToString(reader["DT_CODE"]),
                            PICK_ID = Convert.ToString(reader["PICK_ID"]),
                            ORDER_NO = Convert.ToString(reader["ORDER_NO"]),
                            SIZE = Convert.ToInt32(reader["SIZE"]),
                            ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            COLOR = Convert.ToInt32(reader["COLOR"]),
                            DCHANNEL_CODE = reader["DCHANNEL_CODE"] != DBNull.Value ? Convert.ToInt32(reader["DCHANNEL_CODE"]) : 0,
                            SENTITY_CODE = reader["SENTITY_CODE"] != DBNull.Value ? Convert.ToInt32(reader["SENTITY_CODE"]) : 0,
                            GRADE_CODE = reader["GRADE_CODE"] != DBNull.Value ? Convert.ToInt32(reader["GRADE_CODE"]) : 0,
                            SEASON_CODE = reader["SEASON_CODE"] != DBNull.Value ? Convert.ToInt32(reader["SEASON_CODE"]) : 0,
                            QTY = Convert.ToString(reader["QTY"]),
                            COMM_TYPE = Convert.ToString(reader["COMM_TYPE"]),
                            COMM_RATE = reader["COMM_RATE"] != DBNull.Value ? Convert.ToDecimal(reader["COMM_RATE"]) : 0,
                            COMM_AMT = reader["COMM_AMT"] != DBNull.Value ? Convert.ToDecimal(reader["COMM_AMT"]) : 0,
                            UNIT = Convert.ToInt32(reader["UNIT"]),
                            INTAKE = reader["INTAKE"] != DBNull.Value ? Convert.ToInt32(reader["INTAKE"]) : 0,
                            //S_NO = reader["S_NO"] != DBNull.Value ? Convert.ToInt32(reader["S_NO"]) : 0,
                            RATE = Convert.ToString(reader["RATE"]),
                            AMT = Convert.ToString(reader["AMT"]),
                            PORT = Convert.ToInt32(reader["LC_PORT"]),
                            SHIP_DATE = Convert.ToString(reader["SHIP_DATE"]),
                            BOOKING_DATE = Convert.ToString(reader["BOOKING_DATE"]),
                            HANDOVER_DATE = Convert.ToString(reader["HANDOVER_DATE"]),
                            DOC = Convert.ToString(reader["DOC"]),
                            DT_DESC = Convert.ToString(reader["DT_DESC"]),
                            REV_STATUS = Convert.ToString(reader["REV_STATUS"]),
                            REV_REF = reader["REV_REF"] != DBNull.Value ? Convert.ToInt32(reader["REV_REF"]) : 0,
                            REV_COLOR = "Blue",
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

        public MyHttpResponseMessage GetBatchDetailByProcess(string process, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty, pickMasterTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickMasterTable = menu.PICK_TABLE_MASTER;
                    //pickDetailTable = menu.PICK_TABLE_DETAIL;
                }

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable) && !String.IsNullOrWhiteSpace(pickMasterTable))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"SELECT MPO.TRAN_ID,MPO.V_DATE,MPO.VOUCHER_NO, MPO.ITEM_CODE, MPO.GRADE, MPO.PARTY_CODE, MPO.ACT_CODE, MPO.REF, MPO.JOB_NO, MPO.EMP_ID, MPO.DEP_ID,
                                        MPO.SPARTY_CODE, MPO.SACT_CODE, MPO.QTY, MPO.UNIT, MPO.RATE, MPO.AMT, MPO.REMARKS ,CASE WHEN MPO.ASTATUS = 'Y' THEN 'Active' WHEN MPO.ASTATUS = 'N' THEN 'In-Active' ELSE '' END AASTATUS,
                                        PTC.PARTY_NAME AS CLIENT_NAME, PTS.PARTY_NAME AS SUPPLIER_NAME,IT.ITEM_NAME,EMP.ENAME,PT.GROUP_NAME AS TERMS_NAME, U.GROUP_NAME AS UNIT_NAME, C.DESCR AS CURRENCY_NAME, 
                                        D.DESCR AS DEP_NAME, MPO.DOC, MPO.COMM_AMT AS COMM_TYPE, MPO.COMM AS COMM_RATE, MPO.COMM_VAL AS COMM_AMT
                                        FROM {pickMasterTable} MPO
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PTC ON MPO.PARTY_CODE = PTC.PARTY_CODE AND PTC.ACT_CODE = MPO.ACT_CODE
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PTS ON MPO.SPARTY_CODE = PTS.PARTY_CODE AND PTS.ACT_CODE = MPO.SACT_CODE
                                        LEFT OUTER JOIN TBL_ITEMSMASTER IT ON MPO.ITEM_CODE = IT.ITEM_CODE
                                        LEFT OUTER JOIN TBL_EMP_REG EMP ON MPO.EMP_ID = EMP.EMP_CODE
                                        LEFT OUTER JOIN TBL_PAY_TERMS PT ON MPO.TERMS = PT.GROUP_CODE
                                        LEFT OUTER JOIN TBL_UNIT U ON MPO.UNIT = U.GROUP_CODE
                                        LEFT OUTER JOIN TBL_CURRENCY C ON MPO.CURR_CODE = C.CODE
                                        LEFT OUTER JOIN TBL_ACT_GROUP D ON MPO.DEP_ID = D.CODE
                                        WHERE MPO.TRAN_ID = '{process}' AND MPO.DLT = 'T' AND MPO.BCODE = {common.Branch} AND MPO.PERIOD_ID = {common.Period} ORDER BY MPO.TRAN_ID DESC";

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
                                ITEM_CODE = reader["ITEM_CODE"].ToString(),
                                ITEM_NAME = reader["ITEM_NAME"].ToString(),
                                GRADE_CODE = reader["GRADE"].ToString(),
                                //PARTY_CODE = reader["PARTY_CODE"].ToString(), //yaha
                                PARTY_CODE = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",
                                COMM_TYPE = Convert.ToString(reader["COMM_TYPE"]),
                                COMM_RATE = reader["COMM_RATE"] != DBNull.Value ? Convert.ToDecimal(reader["COMM_RATE"]) : 0,
                                COMM_AMT = reader["COMM_AMT"] != DBNull.Value ? Convert.ToDecimal(reader["COMM_AMT"]) : 0,
                                ACT_CODE = reader["ACT_CODE"].ToString(),
                                CLIENT_NAME = reader["CLIENT_NAME"].ToString(),

                                REF = reader["REF"].ToString(),
                                JOB_NO = reader["JOB_NO"].ToString(),
                                EMP_ID = reader["EMP_ID"].ToString(),
                                ENAME = reader["ENAME"].ToString(),

                                DEP_ID = reader["DEP_ID"].ToString(),
                                DEP_NAME = reader["DEP_NAME"].ToString(),
                                SPARTY_CODE = $"{Convert.ToString(reader["SPARTY_CODE"])}{Convert.ToString(reader["SACT_CODE"])}",
                                SACT_CODE = reader["SACT_CODE"].ToString(),
                                SUPPLIER_NAME = reader["SUPPLIER_NAME"].ToString(),

                                //QTY = reader["QTY"].ToString(),
                                QTY = reader["QTY"] != DBNull.Value ? Convert.ToDecimal(reader["QTY"]) : 0,
                                UNIT = reader["UNIT"].ToString(),
                                UNIT_NAME = reader["UNIT_NAME"].ToString(),

                                //RATE = reader["RATE"].ToString(),
                                RATE = reader["RATE"] != DBNull.Value ? Convert.ToDecimal(reader["RATE"]) : 0,
                                //AMT = reader["AMT"].ToString(),
                                AMT = reader["AMT"] != DBNull.Value ? Convert.ToDecimal(reader["AMT"]) : 0,
                                //COMM = reader["COMM"].ToString(),
                                //COMM_VAL = reader["COMM_VAL"].ToString(),
                                //COMM_AMT = reader["COMM_AMT"].ToString(),
                                REMARKS = reader["REMARKS"].ToString(),
                                PICK_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                DOC = Convert.ToString(reader["DOC"]),
                                STATUS = "N",

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

        public MyHttpResponseMessage Delete(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string? table = menu.TABLE2;
                string connectionString = new SQLService().getconnstring();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE {table} SET DLT = 'F' WHERE DT_CODE = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    response.msgType = 1;
                    response.msg = "Record Deleted Successfully";
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

                MerchantPurchaseOrderDetail MerchantPurchaseOrderDetail = new MerchantPurchaseOrderDetail();
                List<MerchantPurchaseOrderDetailDetail> MerchantPurchaseOrderDetailDetailList = new List<MerchantPurchaseOrderDetailDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        MerchantPurchaseOrderDetail = new MerchantPurchaseOrderDetail
                        {
                            V_DATE = Convert.ToDateTime(reader["V_DATE"]),
                            REF = Convert.ToString(reader["REF"]),
                            //PROCESS = Convert.ToInt32(reader["PROCESS"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new MerchantPurchaseOrderDetailDetail
                        {
                            ITEM_CODE = Convert.ToInt32(detail_Reader["ITEM_CODE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                            UNIT = Convert.ToInt32(detail_Reader["UNIT"]),
                            QTY2 = Convert.ToDouble(detail_Reader["QTY2"]),
                            BAL_QTY = Convert.ToDouble(detail_Reader["BAL_QTY"]),
                            RATE = Convert.ToInt32(detail_Reader["RATE"]),
                            AMT = Convert.ToInt32(detail_Reader["AMT"]),
                            SHIP_DATE = Convert.ToDateTime(detail_Reader["MFG_DATE"]),
                            BOOKING_DATE = Convert.ToDateTime(detail_Reader["EXP_DATE"]),
                            BATCH = Convert.ToString(detail_Reader["BATCH"]),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            CHK = detail_Reader["CHK"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["CHK"]),
                            //PICK_ID = Convert.ToInt32(detail_Reader["PICK_ID"]),
                        };
                        MerchantPurchaseOrderDetailDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customMerchantPurchaseOrderDetail = new CustomMerchantPurchaseOrderDetail
                {
                    Master = MerchantPurchaseOrderDetail,
                    Detail = MerchantPurchaseOrderDetailDetailList
                };

                response = this.Save(customMerchantPurchaseOrderDetail, common, menu);

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

        public MyHttpResponseMessage DeleteMerchantPurchaseOrderDetailDetailByCode(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE2;
                var branch = common.Branch;
                var period = common.Period;
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

        public MyHttpResponseMessage GetDataForReport(MerchantPurchaseOrderDetailRDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            MerchantPurchaseOrderDetailRDLCReport masterData = new MerchantPurchaseOrderDetailRDLCReport();
            CustomMerchantPurchaseOrderDetailForPrintReport reportData = new CustomMerchantPurchaseOrderDetailForPrintReport();
            var Menu = _menuRepository.GetMenu(common.MenuID);
            string? table = string.Empty, detailTable = string.Empty, pickTable = string.Empty;
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                table = menu.TABLE1;
                detailTable = menu.TABLE2;
                pickTable = menu.PICK_TABLE_MASTER;
            }
            try
            {
                string topQuery = "", query = "";

                topQuery = $@"SELECT MPO.CLIENT_PO, P.PARTY_NAME AS SUP_NAME, PC.PARTY_NAME AS CLIENT_NAME, MPO.V_DATE, T.GROUP_NAME AS TERMS, C.DESCR AS CURRENCY , MPO.QTY, M.ADD_USER_ID AS USER_NAME
                                FROM {table} M
                                LEFT OUTER JOIN {detailTable} D ON D.TRAN_ID = M.TRAN_ID
                                LEFT OUTER JOIN {pickTable} MPO ON MPO.TRAN_ID = M.JOB_NO
                                LEFT OUTER JOIN TBL_PAY_TERMS T ON MPO.TERMS = T.GROUP_CODE
                                LEFT OUTER JOIN TBL_CURRENCY C ON MPO.CURR_CODE = C.CODE
                                LEFT OUTER JOIN TBL_PARTY_TYPES P ON P.PARTY_CODE = MPO.SPARTY_CODE AND P.ACT_CODE = MPO.SACT_CODE
								LEFT OUTER JOIN TBL_PARTY_TYPES PC ON PC.PARTY_CODE = MPO.PARTY_CODE AND PC.ACT_CODE = MPO.ACT_CODE
                                WHERE M.BCODE = '{common.Branch}' And M.PERIOD_ID = '{common.Period}' AND M.TRAN_ID = '{modelRecord.TRAN_ID}' AND M.DLT = 'T' AND D.DLT = 'T'";

                query = $@"WITH GroupStatus AS
                            (
                                SELECT REV_REF,
                                    HasC = MAX(CASE WHEN REV_STATUS = 'C' THEN 1 ELSE 0 END),
                                    HasR = MAX(CASE WHEN REV_STATUS = 'R' THEN 1 ELSE 0 END)
                                FROM {detailTable}
	                             WHERE  DLT = 'T' AND TRAN_ID = '{modelRecord.TRAN_ID}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'
                                GROUP BY REV_REF
                            ),
                            Final AS
                            (
                                SELECT 
                                    SE.GROUP_NAME AS ENTITY, 
                                    D.ORDER_NO,
                                    P.GROUP_NAME AS LC_PORT, 
                                    D.SHIP_DATE,  
                                    D.BOOKING_DATE, 
                                    D.HANDOVER_DATE,
                                    S.GROUP_NAME AS SEASON, 
                                    I.ITEM_NAME, 
                                    C.GROUP_NAME AS COLOR, 
                                    U.GROUP_NAME AS UNIT, 
                                    D.QTY, 
                                    D.RATE AS AMT,
                                    CR.SHORT_NAME AS CURR,
                                    D.REV_REF,
                                    D.DT_CODE,
		                            DC.GROUP_NAME AS DCHANNEL,
		                            SI.GROUP_NAME AS SIZE
                                FROM {detailTable} D
                                LEFT JOIN {pickTable} MPO ON MPO.TRAN_ID = D.PICK_ID AND MPO.BCODE = D.BCODE AND MPO.PERIOD_ID = D.PERIOD_ID
                                LEFT JOIN TBL_ITEMSMASTER I ON I.ITEM_CODE = D.ITEM_CODE
                                LEFT JOIN TBL_COLOR C ON C.GROUP_CODE = D.COLOR 
                                LEFT JOIN TBL_UNIT U ON U.GROUP_CODE = D.UNIT 
                                LEFT JOIN TBL_S_ENTITY SE ON SE.GROUP_CODE = D.SENTITY_CODE 
                                LEFT JOIN TBL_PORT P ON P.GROUP_CODE = D.LC_PORT 
                                LEFT JOIN TBL_SEASON S ON S.GROUP_CODE = D.SEASON_CODE 
                                LEFT JOIN TBL_CURRENCY CR ON CR.CODE = MPO.CURR_CODE 
	                            LEFT JOIN TBL_D_CHANNEL DC ON DC.GROUP_CODE = D.DCHANNEL_CODE 
	                            LEFT JOIN TBL_SIZE SI ON SI.GROUP_CODE = D.SIZE 
                                JOIN GroupStatus g ON D.REV_REF = g.REV_REF
                                WHERE g.HasC = 0 AND D.DLT = 'T' AND D.TRAN_ID = '{modelRecord.TRAN_ID}' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}'
                                  AND 
                                  (
                                    (g.HasR = 1 AND D.REV_STATUS = 'R' AND D.DT_CODE = 
                                        (SELECT MAX(DT_CODE) FROM {detailTable} 
                                         WHERE REV_REF = D.REV_REF AND REV_STATUS = 'R' AND DLT = 'T' AND TRAN_ID = '{modelRecord.TRAN_ID}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'))
                                    OR 
                                    (g.HasR = 0 AND D.REV_STATUS = 'N')))
                            SELECT * FROM Final ORDER BY ENTITY ASC;";

                masterData.COMPANY_NAME = currentCompany.C_NAME;
                masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                masterData.COMPANY_PHONE = currentCompany.C_TEL;
                masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                masterData.COMPANY_WATER = currentCompany.C_WATER;
                masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                masterData.SIG1 = $"{menuDetails.MENU_SIG1}";
                masterData.SIG2 = $"{menuDetails.MENU_SIG2}";
                masterData.SIG3 = $"{menuDetails.MENU_SIG3}";
                masterData.SIG4 = $"{menuDetails.MENU_SIG4}";
                masterData.MENU_TERMS = $"{menuDetails.MENU_TERMS}";
                

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(topQuery, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        //masterData.DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                        //masterData.CLIENT_PO = Convert.ToString(reader["CLIENT_PO"]);
                        //masterData.TERMS = Convert.ToString(reader["TERMS"]);
                        //masterData.CURRENCY = Convert.ToString(reader["CURRENCY"]);

                        masterData.DATE = reader["V_DATE"] == DBNull.Value ? string.Empty : (Convert.ToDateTime(reader["V_DATE"]) == new DateTime(1900, 1, 1) ? string.Empty : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy"));
                        masterData.CLIENT_PO = reader["CLIENT_PO"] == DBNull.Value ? string.Empty : reader["CLIENT_PO"].ToString();
                        masterData.TERMS = reader["TERMS"] == DBNull.Value ? string.Empty : reader["TERMS"].ToString();
                        masterData.SUP_NAME = reader["SUP_NAME"] == DBNull.Value ? string.Empty : reader["SUP_NAME"].ToString();
                        masterData.CLIENT_NAME = reader["CLIENT_NAME"] == DBNull.Value ? string.Empty : reader["CLIENT_NAME"].ToString();
                        masterData.CURRENCY = reader["CURRENCY"] == DBNull.Value ? string.Empty : reader["CURRENCY"].ToString();
                        masterData.ORDER_QTY = reader["QTY"] == DBNull.Value ? string.Empty : reader["QTY"].ToString();
                        masterData.USER = reader["USER_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["USER_NAME"]);
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
                        dataRow["ENTITY"] = reader["ENTITY"] == DBNull.Value ? string.Empty : reader["ENTITY"].ToString();
                        dataRow["ORDER_NO"] = reader["ORDER_NO"] == DBNull.Value ? string.Empty : reader["ORDER_NO"].ToString();
                        dataRow["LC_PORT"] = reader["LC_PORT"] == DBNull.Value ? string.Empty : reader["LC_PORT"].ToString();
                        //dataRow["ENTITY"] = Convert.ToString(reader["ENTITY"]);
                        //dataRow["ORDER_NO"] = Convert.ToString(reader["ORDER_NO"]);
                        //dataRow["LC_PORT"] = Convert.ToString(reader["LC_PORT"]);
                        //dataRow["SHIP_DATE"] = reader["SHIP_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["SHIP_DATE"]).ToString("dd-MM-yyyy");
                        if (reader["SHIP_DATE"] == DBNull.Value)
                        {
                            dataRow["SHIP_DATE"] = null;
                        }
                        else
                        {
                            DateTime shipDate = Convert.ToDateTime(reader["SHIP_DATE"]);
                            dataRow["SHIP_DATE"] = (shipDate == new DateTime(1900, 1, 1)) ? null : shipDate.ToString("dd-MM-yyyy");
                        }

                        if (reader["BOOKING_DATE"] == DBNull.Value)
                        {
                            dataRow["BOOKING_DATE"] = null;
                        }
                        else
                        {
                            DateTime shipDate = Convert.ToDateTime(reader["BOOKING_DATE"]);
                            dataRow["BOOKING_DATE"] = (shipDate == new DateTime(1900, 1, 1)) ? null : shipDate.ToString("dd-MM-yyyy");
                        }

                        if (reader["HANDOVER_DATE"] == DBNull.Value)
                        {
                            dataRow["HANDOVER_DATE"] = null;
                        }
                        else
                        {
                            DateTime shipDate = Convert.ToDateTime(reader["HANDOVER_DATE"]);
                            dataRow["HANDOVER_DATE"] = (shipDate == new DateTime(1900, 1, 1)) ? null : shipDate.ToString("dd-MM-yyyy");
                        }

                        //dataRow["SEASON"] = Convert.ToString(reader["SEASON"]);
                        //dataRow["ITEM_NAME"] = Convert.ToString(reader["ITEM_NAME"]);
                        //dataRow["COLOR"] = Convert.ToString(reader["COLOR"]);
                        //dataRow["UNIT"] = Convert.ToString(reader["UNIT"]);
                        dataRow["SEASON"] = reader["SEASON"] == DBNull.Value ? string.Empty : reader["SEASON"].ToString();
                        dataRow["ITEM_NAME"] = reader["ITEM_NAME"] == DBNull.Value ? string.Empty : reader["ITEM_NAME"].ToString();
                        dataRow["COLOR"] = reader["COLOR"] == DBNull.Value ? string.Empty : reader["COLOR"].ToString();
                        dataRow["UNIT"] = reader["UNIT"] == DBNull.Value ? string.Empty : reader["UNIT"].ToString();
                        dataRow["CURR"] = reader["CURR"] == DBNull.Value ? string.Empty : reader["CURR"].ToString();
                        dataRow["DCHANNEL"] = reader["DCHANNEL"] == DBNull.Value ? string.Empty : reader["DCHANNEL"].ToString();
                        dataRow["SIZE"] = reader["SIZE"] == DBNull.Value ? string.Empty : reader["SIZE"].ToString();
                        dataRow["QTY"] = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]);
                        dataRow["AMT"] = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);




                        
                        

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