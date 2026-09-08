using System.Data;
using System.Text.Json.Nodes;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class ImportManifestRepository : IImportManifestRepository
    {
        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public IMenuRepository _menuRepository { get; set; }
        public ImportManifestRepository(IBranchRepository branchRepository, ICommonRepository commonRepository, IMenuRepository menuRepository)
        {
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetChartOfAccounts(int? pType)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<KeyValuePair<int, string?>> dropdown = new List<KeyValuePair<int, string?>>();
                using (SqlConnection db = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "SELECT ACT_CODE,ACT_NAME FROM TBL_CHART WHERE ACT_NATURE = '" + pType + "' AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
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
                        string query = $@"SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, A.ADD_USER_ID, A.ADD_DATE, 
	                                        A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, 
                                            CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, IM.ITEM_NAME AS ITEM_CODE  
                                        FROM {table} A
                                            LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE
                                        WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}'  
                                        ORDER BY A.TRAN_ID DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                //ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                //V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                //VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
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

        public MyHttpResponseMessage Save(CustomImportManifest modelRecord, Common common)
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

                                var duplicationQuery = $"SELECT COUNT(*) FROM {table} WHERE ITEM_CODE = @ItemCode AND DLT = 'T'";
                                command.CommandText = duplicationQuery;
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("@ItemCode", modelRecord.Master.ITEM_CODE);

                                int count = (int)command.ExecuteScalar();
                                if (count > 0)
                                {
                                    response.data = "";
                                    response.msg = $"Record With Selected Item Already Exist!";
                                    response.msgType = 2;
                                    return response;
                                }

                                query = $"INSERT INTO {table}" +
                                        "(TRAN_ID, V_DATE, VOUCHER_NO, ITEM_CODE, " +
                                        "BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                        "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                        "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                        "ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, " +
                                        "DLT)" +
                                        "VALUES" +
                                        "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.ITEM_CODE + "'," +
                                        "'" + branch + "','" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                        "'" + computer + "','" + ip + "','" + username + "'," +
                                        "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + computer + "','" + ip + "'," +
                                        "'" + postal + "','" + postal + "','" + modelRecord.Master.ASTATUS + "','" + menuID + "','T')";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                query = $"UPDATE {table} SET " +
                                        $"V_DATE = '{modelRecord.Master.V_DATE}', " +
                                        $"ITEM_CODE = '{modelRecord.Master.ITEM_CODE}', " +
                                        $"EDIT_USER_ID = '{username}', " +
                                        $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                        $"EDIT_COMPUTER_NAME = '{computer}', " +
                                        $"EDIT_IP_ADDRESS = '{ip}', " +
                                        $"EDIT_POSTALCODE = '{postal}', " +
                                        $"ASTATUS = '{modelRecord.Master.ASTATUS}' " +
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
                                                           "(TRAN_ID, DT_CODE, " +
                                                           "SHIP_CODE, SACT_CODE, D_DATE, IMPORT_CODE, IACT_CODE, QTY, COMMENT, WAREHOUSE, LOT, " +
                                                           "BCODE,PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                                           "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                                           "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                                           "ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT)" +
                                                           "VALUES" +
                                                           "('" + modelRecord.Master.TRAN_ID + "','" + detailCode + "'," +
                                                           "'" + item.SHIP_CODE + "','" + item.SACT_CODE + "','" + item.D_DATE + "','" + item.IMPORT_CODE + "','" + item.IACT_CODE + "','" + item.QTY + "','" + item.COMMENT + "','" + item.WAREHOUSE + "','" + item.LOT + "'," +
                                                           "'" + branch + "','" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                                           "'" + computer + "','" + ip + "','" + username + "'," +
                                                           "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + computer + "','" + ip + "'," +
                                                           "'" + postal + "','" + postal + "','" + menuID + "','T')";
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
                                                      $"D_DATE = '{item.D_DATE}', " +
                                                      $"SHIP_CODE = '{item.SHIP_CODE}', " +
                                                      $"SACT_CODE = '{item.SACT_CODE}', " +
                                                      $"IMPORT_CODE = '{item.IMPORT_CODE}', " +
                                                      $"IACT_CODE = '{item.IACT_CODE}', " +
                                                      $"QTY = '{item.QTY}', " +
                                                      $"COMMENT = '{item.COMMENT}', " +
                                                      $"WAREHOUSE = '{item.WAREHOUSE}', " +
                                                      $"LOT = '{item.LOT}', " +
                                                      $"EDIT_USER_ID = '{username}', " +
                                                      $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                      $"EDIT_COMPUTER_NAME = '{computer}', " +
                                                      $"EDIT_IP_ADDRESS = '{ip}', " +
                                                      $"EDIT_POSTALCODE = '{postal}', " +
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

        public MyHttpResponseMessage GetImportManifestByCode(int code, Common common)
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
                        string query = $"SELECT TRAN_ID, V_DATE, VOUCHER_NO, ITEM_CODE, ASTATUS FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
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
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
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

        public MyHttpResponseMessage GetImportManifestDetailsByCode(int code, Common common)
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
                        string query = $"SELECT * FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                SHIPPER_DDL = $"{Convert.ToString(reader["SHIP_CODE"])}{Convert.ToString(reader["SACT_CODE"])}",
                                IMPORTER_DDL = $"{Convert.ToString(reader["IMPORT_CODE"])}{Convert.ToString(reader["IACT_CODE"])}",
                                SHIP_CODE = Convert.ToInt32(reader["SHIP_CODE"]),
                                IMPORT_CODE = Convert.ToInt32(reader["IMPORT_CODE"]),
                                SACT_CODE = Convert.ToInt32(reader["SACT_CODE"]),
                                IACT_CODE = Convert.ToInt32(reader["IACT_CODE"]),
                                D_DATE = reader["D_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["D_DATE"]).ToString("yyyy-MM-dd"),
                                QTY = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                COMMENT = Convert.ToString(reader["COMMENT"]),
                                WAREHOUSE = reader["WAREHOUSE"] == null ? "" : Convert.ToString(reader["WAREHOUSE"]),
                                LOT = reader["LOT"] == null ? "" : Convert.ToString(reader["LOT"]),
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
                string? table = menu.TABLE1;
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

                ImportManifest importManifest = new ImportManifest();
                List<ImportManifestDetail> importManifestDetailList = new List<ImportManifestDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        importManifest = new ImportManifest
                        {
                            V_DATE = Convert.ToDateTime(reader["V_DATE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new ImportManifestDetail
                        {
                            WAREHOUSE = Convert.ToString(detail_Reader["WAREHOUSE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                        };
                        importManifestDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customImportManifest = new CustomImportManifest
                {
                    Master = importManifest,
                    Detail = importManifestDetailList
                };

                response = this.Save(customImportManifest, common);

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

        public MyHttpResponseMessage DeleteImportManifestDetailByCode(int tranID, int code, Common common, Menu menu)
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
                    string query = $"UPDATE {table} SET DLT = 'F' " +
                                   $"WHERE TRAN_ID = '{tranID}' AND DT_CODE = '{code}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
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

        public MyHttpResponseMessage GetDataForReport(ListPrintReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            ListPrintReport masterData = new ListPrintReport();
            CustomPrintReport reportData = new CustomPrintReport();
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
                string query = "";
                if (menuDetails.MD_ID == 26)
                {
                    query = $@"SELECT 
                                COUNT(*) OVER() AS COUNT, A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, IM.ITEM_NAME AS ITEM_CODE, D.D_DATE, S.PARTY_NAME AS SHIPPER, I.PARTY_NAME AS IMPORTER, D.QTY, D.LOT, D.COMMENT, D.WAREHOUSE, A.ADD_USER_ID, A.ADD_DATE, 
	                            A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, 
                                CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS 
                            FROM {table} A
                                LEFT OUTER JOIN {detailTable} D ON A.TRAN_ID = D.TRAN_ID
                                LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE
                                LEFT OUTER JOIN TBL_PARTY_TYPES S ON D.SHIP_CODE = S.PARTY_CODE AND S.ACT_CODE = D.SACT_CODE
                                LEFT OUTER JOIN TBL_PARTY_TYPES I ON D.IMPORT_CODE = I.PARTY_CODE AND I.ACT_CODE = D.IACT_CODE
                            WHERE A.DLT = 'T' AND D.DLT = 'T' AND A.TRAN_ID = '{modelRecord.TRAN_ID}' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' 
                            ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC";
                }

                masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                masterData.COMPANY_NAME = currentCompany.C_NAME;
                masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                masterData.COMPANY_PHONE = currentCompany.C_TEL;
                masterData.COMPANY_LOGO = currentCompany.C_LOGO;

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        DataRow dataRow = dataTable.NewRow();

                        dataRow["Date"] = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                        dataRow["Item"] = Convert.ToString(reader["ITEM_CODE"]);
                        dataRow["DDate"] = reader["D_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["D_DATE"]).ToString("dd-MM-yyyy") == "01-01-1900" ? "" : Convert.ToDateTime(reader["D_DATE"]).ToString("dd-MM-yyyy");
                        dataRow["Shipper"] = Convert.ToString(reader["SHIPPER"]);
                        dataRow["Importer"] = Convert.ToString(reader["IMPORTER"]);
                        dataRow["Qty"] = Convert.ToInt32(reader["QTY"]);
                        dataRow["Comment"] = Convert.ToString(reader["COMMENT"]);
                        dataRow["Warehouse"] = Convert.ToString(reader["WAREHOUSE"]);
                        dataRow["Lot"] = Convert.ToString(reader["LOT"]);
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
