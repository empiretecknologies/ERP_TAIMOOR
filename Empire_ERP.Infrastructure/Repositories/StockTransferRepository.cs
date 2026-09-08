using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class StockTransferRepository : IStockTransferRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public ICommonService _commonService { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public IPeriodRepository _periodRepository { get; set; }
        public StockTransferRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonService commonService, IPeriodRepository periodRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _commonService = commonService;
            _periodRepository = periodRepository;
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
                                       "CASE WHEN AM = 'A' THEN 'Auto' else 'Manual' end As AM," +
                                       "REF,REMARKS,BCODE,PERIOD_ID," +
                                       "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
                                       "EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                       "ADD_POSTALCODE,EDIT_POSTALCODE," +
                                       "CASE WHEN ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
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
                                REF = Convert.ToString(reader["REF"]),
                                AM = Convert.ToString(reader["AM"]),
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

        public MyHttpResponseMessage GetStockTransferByCode(int code, Common common)
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
                                       "AM,REF,REMARKS,BCODE,TBCODE,PERIOD_ID,TPERIOD_ID,ASTATUS " +
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
                                AM = Convert.ToString(reader["AM"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                BCODE = Convert.ToInt32(reader["BCODE"]),
                                TBCODE = Convert.ToInt32(reader["TBCODE"]),
                                PERIOD_ID = Convert.ToInt32(reader["PERIOD_ID"]),
                                TPERIOD_ID = Convert.ToInt32(reader["TPERIOD_ID"]),
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

        public MyHttpResponseMessage GetPrintData(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? pickDetail = string.Empty;
                string? b_i = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE2;
                    pickDetail = menu.PICK_TABLE_DETAIL;
                    b_i = menu.B_I;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        var query = $@"SELECT IM.ITEM_NAME AS ITEM,IM.BARCODE,D.QTY,D.RATE,D.DT_CODE
                                    FROM {table} D
                                    LEFT OUTER JOIN TBL_ITEMSMASTER IM 
                                    ON IM.ITEM_CODE = D.ITEM_CODE 
                                    WHERE D.DLT = 'T' AND D.BCODE = {common.Branch}  AND D.PERIOD_ID = {common.Period} AND D.TRAN_ID = {code} ORDER BY D.DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                ITEM = Convert.ToString(reader["ITEM"]),
                                BARCODE = Convert.ToString(reader["BARCODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
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
        public MyHttpResponseMessage GetStockTransferDetailByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? pickDetail = string.Empty;
                string? b_i = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE2;
                    pickDetail = menu.PICK_TABLE_DETAIL;
                    b_i = menu.B_I;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $"SELECT T.PACK, T.TRAN_ID,T.DT_CODE,T.CHK, T.RATE, T.AMT, T.PICK_ID, " +
                            $" T.ITEM_CODE ," +
                            $" T.QTY,T.UNIT," +
                            $" T.QTY2,T.BAL_QTY,T.DT_DESC, " +
                            $" T.COLOR,T.SIZE,T.GRADE,T.[PRINT], IM.ITEM_NAME,CASE WHEN '{b_i}' = 'B' THEN IB.ITEM_NAME ELSE IM.ITEM_NAME END AS ITEM_ID, ISNULL(T.BLABEL,0) AS BLABEL, " +
                            $" IG.GROUP_NAME FROM {table} T " +
                            $" LEFT OUTER JOIN TBL_ITEMSMASTER IM ON IM.ITEM_CODE = T.ITEM_CODE" +
                            $" LEFT OUTER JOIN TBL_BARCODE B ON B.CODE = T.ITEM_CODE" +
                            $" LEFT OUTER JOIN TBL_ITEMSMASTER IB ON B.ITEM_CODE = IB.ITEM_CODE" +
                            $" LEFT OUTER JOIN TBL_BRANCH BR ON BR.BCODE = T.BCODE" +
                            $" LEFT OUTER JOIN {pickDetail} STKR ON STKR.DT_CODE = T.PICK_ID AND STKR.BCODE = T.BCODE AND STKR.PERIOD_ID = T.PERIOD_ID" +
                            $" LEFT OUTER JOIN TBL_ITEMSGROUP IG ON IG.GROUP_CODE = STKR.ITEM_GROUP" +
                            $" WHERE T.DLT = 'T' AND T.BCODE = {common.Branch}  AND T.PERIOD_ID = {common.Period} AND T.TRAN_ID = {code} ORDER BY T.DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                PACK = Convert.ToString(reader["PACK"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                UNIT = Convert.ToInt32(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                COLOR = Convert.ToInt32(reader["COLOR"]),
                                SIZE = Convert.ToInt32(reader["SIZE"]),
                                RATE = reader["RATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RATE"]),
                                AMT = reader["AMT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AMT"]),
                                GRADE = Convert.ToInt32(reader["GRADE"]),
                                CHK = Convert.ToString(reader["CHK"]),
                                PICK_ID = Convert.ToString(reader["PICK_ID"]),
                                ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                                CHK1 = Convert.ToString(reader["CHK"]) == "1" ? true : false,
                                BLABEL = Convert.ToInt32(reader["BLABEL"]),
                                PRINT = Convert.ToString(reader["PRINT"]) == "Y" ? true : false,
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

        public MyHttpResponseMessage GetStockTransferDetailByItem(int code, int qty, Common common)
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
                        string query = "SELECT B.CODE, B.SRATE, B.COLOR, B.SIZE, IT.SALE_RATE, IT.ITEM_NAME FROM TBL_BARCODE B " +
                            $"LEFT OUTER JOIN TBL_ITEMSMASTER IT ON B.ITEM_CODE = IT.ITEM_CODE " +
                            "WHERE  B.DLT = 'T' AND B.ITEM_CODE = " + code + "";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = 0,
                                ITEM_CODE = Convert.ToInt32(reader["CODE"]),
                                ITEM_ID = Convert.ToString(reader["ITEM_NAME"]),
                                QTY = qty,
                                UNIT = 0,
                                QTY2 = qty.ToString(),
                                BAL_QTY = qty.ToString(),
                                RATE = Convert.ToString(reader["SRATE"]),
                                AMT = (Convert.ToInt32(reader["SRATE"]) * qty).ToString(),
                                DISC = "",
                                DISC_AMT = "",
                                TAX = "",
                                TAX_AMT = "",
                                NET_AMT = (Convert.ToInt32(reader["SRATE"]) * qty).ToString(),
                                DT_DESC = "",
                                COLOR = reader["COLOR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COLOR"]),
                                SIZE = reader["SIZE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE"]),
                                GRADE = 0,
                                WAREHOUSE = 0,
                                DEL_DATE = "",
                                DUE_DATE = "",
                                DUE_DAYS = "",
                                VEH = "",
                                CHK = "",
                                CHK1 = false,
                                PICK_ID = 0
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

        public MyHttpResponseMessage GetBarcodeList()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $@"SELECT IM.ITEM_NAME AS ITEM_ID, IM.ITEM_CODE AS ITEM_CODE, C.GROUP_CODE AS COLOR_ID, S.GROUP_CODE AS SIZE_ID,B.CODE AS BARCODE_CODE,S.GROUP_NAME AS SIZE, C.GROUP_NAME AS COLOR, B.BARCODE, B.WSALE AS RATE FROM TBL_BARCODE B
                                    LEFT OUTER JOIN TBL_ITEMSMASTER IM
                                    ON IM.ITEM_CODE = B.ITEM_CODE
                                    LEFT OUTER JOIN TBL_SIZE S
                                    ON S.GROUP_CODE = B.SIZE
                                    LEFT OUTER JOIN TBL_COLOR C
                                    ON C.GROUP_CODE = B.COLOR
                                    WHERE B.DLT = 'T' AND IM.ASTATUS = 'Y' AND IM.ASTATUS = 'Y'
                                    ORDER BY ITEM_ID , SIZE";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            BARCODE_CODE = Convert.ToString(reader["BARCODE_CODE"]),
                            ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                            ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                            SIZE_NAME = Convert.ToString(reader["SIZE"]),
                            COLOR_NAME = Convert.ToString(reader["COLOR"]),
                            BARCODE = Convert.ToString(reader["BARCODE"]),
                            RATE = Convert.ToInt32(reader["RATE"]),
                            AMT = Convert.ToInt32(reader["RATE"]),
                            NET_AMT = Convert.ToInt32(reader["RATE"]),
                            COLOR = Convert.ToInt32(reader["COLOR_ID"]),
                            SIZE = Convert.ToInt32(reader["SIZE_ID"]),
                            QTY = 1,
                            BAL_QTY = 1,
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) FROM {table}";
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

        public MyHttpResponseMessage Save(CustomStockTransfer modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty, b_i = string.Empty, stk_status = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    b_i = menu.B_I;
                    stk_status = menu.STK_STATUS;
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
                    var periodInfo = _periodRepository.GetPeriodById(Convert.ToInt32(period));
                    string startDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
                    string endDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
                    int periodTo = 0;
                    bool isStockSufficient = true;
                    string InSufficientItem = "";
                    double InSufficientItemQty = 0;
                    string connectionString = new SQLService().getconnstring();
                    Dictionary<int?, double?> currentItems = new Dictionary<int?, double?>();
                    Dictionary<int?, double?> previousItems = new Dictionary<int?, double?>();
                    Dictionary<int?, double?> stockBalance = new Dictionary<int?, double?>();
                    if (stk_status == "Y")
                    {
                        if (b_i == "B")
                        {
                            foreach (var item in modelRecord.Detail.ToList())
                            {
                                if (!currentItems.ContainsKey(item.ITEM_CODE))
                                {
                                    currentItems.Add(item.ITEM_CODE, item.BAL_QTY);
                                }
                                else
                                {
                                    currentItems[item.ITEM_CODE] += item.BAL_QTY;
                                }
                            }

                            var itemCodes = string.Join(",", modelRecord.Detail.Select(x => x.ITEM_CODE).Distinct());
                            string query = $@"EXEC STKPROC 71,'{startDate}','{endDate}','{common.Branch}','{common.Period}','',''";
                            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                            {
                                SqlCommand command = new SqlCommand(query, connection);
                                connection.Open();
                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int? itemCode = reader["ITEM_ID"] as int?;
                                        double? balance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["BALANCE"]);

                                        if (itemCode != null)
                                            stockBalance.Add(itemCode, balance);
                                    }
                                }
                            }

                            if (modelRecord.Master.TRAN_ID > 0)
                            {
                                previousItems = PreviousStockInBill(detailTable, modelRecord.Master.TRAN_ID, period, branch);

                                foreach (var item in previousItems)
                                {
                                    if (currentItems.ContainsKey(item.Key))
                                    {
                                        currentItems[item.Key] = (currentItems[item.Key] ?? 0) - (item.Value ?? 0);

                                        if (currentItems[item.Key] < 0)
                                        {
                                            currentItems[item.Key] = 0;
                                        }
                                    }
                                }
                            }

                            foreach (var item in currentItems)
                            {
                                double availableStock = stockBalance.ContainsKey(item.Key) ? stockBalance[item.Key] ?? 0 : 0;
                                double currentQty = item.Value ?? 0;

                                if (currentQty > availableStock)
                                {
                                    List<CustomKeyValuPair> barcodes = DropdownService.BarcodesKeyAndValue();
                                    var SelectedItem = barcodes.Where(b => b.key == item.Key).FirstOrDefault();
                                    InSufficientItem = SelectedItem.value;
                                    InSufficientItemQty = availableStock;
                                    isStockSufficient = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (isStockSufficient)
                    {
                        //    string maxIdQuery = "SELECT TOP 1 PID FROM TBL_PERIOD WHERE BCODE = '" + modelRecord.Master.TBCODE + "' AND CLOSING = 0 ORDER BY PID DESC";
                        //    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                        //    {
                        //        SqlCommand command = new SqlCommand(maxIdQuery, connection);
                        //        connection.Open();
                        //        object result = command.ExecuteScalar();
                        //        periodTo = Convert.ToInt32(result);
                        //    }
                        //    if (periodTo == 0)
                        //    {
                        //        response.data = "";
                        //        response.msg = "Please add period on this branch.";
                        //        response.msgType = 2;
                        //    }
                        //else
                        //{
                        connectionString = new SQLService().getconnstring();
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

                                    query = $"INSERT INTO {table}" +
                                            "(TRAN_ID,V_DATE,VOUCHER_NO,AM," +
                                            "REF,REMARKS,TBCODE,BCODE,TPERIOD_ID,PERIOD_ID," +
                                            "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                            "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                                            "EDIT_POSTALCODE,ASTATUS,MENU_ID,DLT )" +
                                            "VALUES" +
                                            "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.AM + "'," +
                                            "'" + modelRecord.Master.REF + "','" + modelRecord.Master.REMARKS + "','" + modelRecord.Master.TBCODE + "','" + branch + "','" + periodTo + "','" + period + "'," +
                                            "'" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                            "'" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Computer + "','" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.Master.ASTATUS + "','" + menuID + "','T')";
                                    command.CommandText = query;
                                    command.ExecuteNonQuery();
                                }
                                else
                                {
                                    query = $"UPDATE {table} SET V_DATE = '" + modelRecord.Master.V_DATE + @"',
                                                    AM = '" + modelRecord.Master.AM + @"',
                                                    REF = '" + modelRecord.Master.REF + @"',
                                                    REMARKS = '" + modelRecord.Master.REMARKS + @"',
                                                    TBCODE = '" + modelRecord.Master.TBCODE + @"',
                                                    TPERIOD_ID = '" + periodTo + @"',
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

                                StringBuilder sqlQuery = new StringBuilder();
                                StringBuilder insertQuery = new StringBuilder();
                                StringBuilder updateQuery = new StringBuilder();

                                insertQuery.Append($"INSERT INTO {detailTable} (PACK,TRAN_ID, DT_CODE, RATE, ITEM_CODE, QTY, AMT, UNIT, QTY2, BAL_QTY, DT_DESC, COLOR, SIZE, GRADE, BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT, CHK, PICK_ID, BLABEL, [PRINT]) VALUES ");
                                updateQuery.Append($"");

                                bool hasInsert = false;
                                bool hasUpdate = false;

                                int detailCode = GenerateNextDetailId(common, command);

                                foreach (var item in modelRecord.Detail)
                                {
                                    try
                                    {
                                        if (item.DT_CODE == null || item.DT_CODE == 0)
                                        {
                                            detailCode++;
                                            if (detailCode > 0)
                                            {
                                                insertQuery.Append($"('{item.PACK}','{modelRecord.Master.TRAN_ID}', {detailCode}, '{item.RATE}', '{item.ITEM_CODE}', '{item.QTY}', '{item.AMT}', '{item.UNIT}', '{item.QTY2}', '{item.BAL_QTY}', '{item.DT_DESC}', '{item.COLOR}', '{item.SIZE}', '{item.GRADE}', '{branch}', '{period}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', '{Postal}', '{Postal}', '{menuID}', 'T', '{item.CHK}', '{item.PICK_ID}', '{item.BLABEL}', 'N'),");
                                                hasInsert = true;
                                            }
                                        }
                                        else
                                        {
                                            updateQuery.Append($@"
                                                UPDATE {detailTable} SET 
                                                ITEM_CODE = '{item.ITEM_CODE}',
                                                PACK = '{item.PACK}',
                                                QTY = '{item.QTY}',
                                                UNIT = '{item.UNIT}',
                                                RATE = '{item.RATE}',
                                                AMT = '{item.AMT}',
                                                QTY2 = '{item.QTY2}',
                                                BAL_QTY = '{item.BAL_QTY}',
                                                DT_DESC = '{item.DT_DESC}',
                                                COLOR = '{item.COLOR}',
                                                SIZE = '{item.SIZE}',
                                                GRADE = '{item.GRADE}',
                                                CHK = '{item.CHK}',
                                                PICK_ID = '{item.PICK_ID}',
                                                EDIT_USER_ID = '{username}',
                                                EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                EDIT_COMPUTER_NAME = '{Computer}',
                                                EDIT_IP_ADDRESS = '{Ip}',
                                                EDIT_POSTALCODE = '{Postal}',
                                                DLT = 'T',
                                                BLABEL = '{item.BLABEL}'
                                                WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' 
                                                AND DT_CODE = '{item.DT_CODE}' 
                                                AND BCODE = '{branch}' 
                                                AND PERIOD_ID = '{period}';");
                                            hasUpdate = true;
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        isDetailAdded = false;
                                    }
                                }

                                if (hasInsert)
                                {
                                    insertQuery.Length -= 1;
                                    sqlQuery.Append(insertQuery.ToString() + ";");
                                }
                                if (hasUpdate)
                                {
                                    sqlQuery.Append(updateQuery.ToString());
                                }

                                if (hasInsert || hasUpdate)
                                {
                                    command.CommandText = sqlQuery.ToString();
                                    command.ExecuteNonQuery();
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
                        //}
                    }
                    else
                    {
                        response.data = "";
                        response.msg = $"{InSufficientItem} has only {InSufficientItemQty} in stock.";
                        response.msgType = 2;
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

        public Dictionary<int?, double?> PreviousStockInBill(string table, int? TRAN_ID, string period, string branch)
        {
            List<CurrentItemsInBill> jsonDataResult = new List<CurrentItemsInBill>();
            string query;
            Dictionary<int?, double?> stock = new();
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                query = $"SELECT TRAN_ID,DT_CODE,ITEM_CODE, BAL_QTY FROM {table} " +
                    " WHERE DLT = 'T' AND TRAN_ID = '" + TRAN_ID + "' AND BCODE = '" + branch + "' AND PERIOD_ID = '" + period + "'";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new CurrentItemsInBill
                    {
                        ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                        QTY = reader["BAL_QTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["BAL_QTY"])
                    };
                    jsonDataResult.Add(row);
                }
                reader.Close();
            }
            if (jsonDataResult is not null)
            {
                foreach (var item in jsonDataResult)
                {
                    if (!stock.ContainsKey(item.ITEM_CODE))
                    {
                        stock.Add(Convert.ToInt32(item.ITEM_CODE), Convert.ToDouble(item.QTY));
                    }
                    else
                    {
                        stock[item.ITEM_CODE] += Convert.ToDouble(item.QTY);
                    }
                }
            }
            return stock;
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

                StockTransfer stockTransfer = new StockTransfer();
                List<StockTransferDetail> stockTransferDetailList = new List<StockTransferDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        stockTransfer = new StockTransfer
                        {
                            V_DATE = Convert.ToDateTime(reader["V_DATE"]),
                            REF = Convert.ToString(reader["REF"]),
                            AM = Convert.ToString(reader["AM"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            TBCODE = Convert.ToInt32(reader["TBCODE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new StockTransferDetail
                        {
                            ITEM_CODE = Convert.ToInt32(detail_Reader["ITEM_CODE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                            UNIT = Convert.ToInt32(detail_Reader["UNIT"]),
                            QTY2 = Convert.ToDouble(detail_Reader["QTY2"]),
                            BAL_QTY = Convert.ToDouble(detail_Reader["BAL_QTY"]),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            COLOR = Convert.ToInt32(detail_Reader["COLOR"]),
                            SIZE = Convert.ToInt32(detail_Reader["SIZE"]),
                            GRADE = Convert.ToInt32(detail_Reader["GRADE"]),
                            CHK = detail_Reader["CHK"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["CHK"]),
                            BLABEL = Convert.ToInt32(detail_Reader["BLABEL"]),
                            RATE = Convert.ToInt32(detail_Reader["RATE"]),
                            PICK_ID = Convert.ToInt32(detail_Reader["PICK_ID"]),
                        };
                        stockTransferDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customStockTransfer = new CustomStockTransfer
                {
                    Master = stockTransfer,
                    Detail = stockTransferDetailList
                };

                response = this.Save(customStockTransfer, common);

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

        public MyHttpResponseMessage DeleteStockTransferDetailByCode(int code, Common common)
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

        public MyHttpResponseMessage GetDataForCartonSticker(List<StockTransferStickerPrint> data)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var codes = String.Join(',', data.Select(d => d.ITEM_CODE).ToList());
                List<StockTransferStickerPrint> jsonDataResult = new List<StockTransferStickerPrint>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $"SELECT BL.GROUP_NAME AS BLABEL, IMC.GROUP_NAME AS CATEGORY, IMSC.GROUP_NAME AS SUBCATEGORY, IM.ITEM_ID, IM.ITEM_NAME, B.BARCODE_TYPE, C.GROUP_NAME AS COLOR, " +
                                   $"S.GROUP_NAME AS SIZE FROM TBL_BARCODE B WITH (NOLOCK) " +
                                   $"LEFT JOIN TBL_BLABEL BL WITH (NOLOCK) ON B.BLABEL = BL.GROUP_CODE " +
                                   $"LEFT JOIN TBL_COLOR C WITH (NOLOCK) ON B.COLOR = C.GROUP_CODE " +
                                   $"LEFT JOIN TBL_SIZE S WITH (NOLOCK) ON B.SIZE = S.GROUP_CODE " +
                                   $"LEFT JOIN TBL_ITEMSMASTER IM WITH (NOLOCK) ON B.ITEM_CODE = IM.ITEM_CODE " +
                                   $"LEFT JOIN TBL_CATEGORY IMC WITH (NOLOCK) ON IMC.GROUP_CODE = IM.CAT_CODE " +
                                   $"LEFT JOIN TBL_SUB_CATEGORY IMSC WITH (NOLOCK) ON IMSC.GROUP_CODE = IM.SUB_CAT_CODE " +
                                   $"WHERE B.DLT = 'T' AND B.ASTATUS= 'Y' AND IM.DLT = 'T' AND B.CODE IN ({codes})";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new StockTransferStickerPrint
                        {
                            BLABEL = Convert.ToString(reader["BLABEL"]),
                            CATEGORIES = Convert.ToString(reader["CATEGORY"]),
                            SUBCATEGORIES = Convert.ToString(reader["SUBCATEGORY"]),
                            ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                            COLORS = Convert.ToString(reader["COLOR"]),
                            SIZES = Convert.ToString(reader["SIZE"]),
                            ITEM_ID = Convert.ToString(reader["ITEM_NAME"]),
                            QTY = data.Sum(d => d.QTY),
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

        public MyHttpResponseMessage GetDataForPrintReport(RDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Branch currentBranch, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            StockTransferForPrint masterData = new StockTransferForPrint();
            CustomStockTransferForPrintReport reportData = new CustomStockTransferForPrintReport();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? tableMaster = string.Empty;
                string? tableDetail = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    tableMaster = menu.TABLE1;
                    tableDetail = menu.TABLE2;
                }
                masterData.COMPANY_NAME = currentCompany.C_NAME;
                masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                masterData.COMPANY_PHONE = currentCompany.C_TEL;
                masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                masterData.HEADER_NAME = $"Shop {menuDetails.MD_NAME}";
                masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                masterData.MENU_SIG1 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG1) ? true : false;
                masterData.MENU_SIG2 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG2) ? true : false;
                masterData.MENU_SIG3 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG3) ? true : false;
                masterData.MENU_SIG4 = String.IsNullOrWhiteSpace(menuDetails.MENU_SIG4) ? true : false;

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $"SELECT M.V_DATE, M.VOUCHER_NO, M.REF, M.REMARKS, BF.B_NAME AS BranchFrom, BF.B_ADDRESS AS BranchFromAdr, " +
                        $"BT.B_NAME AS BranchTo, BT.B_ADDRESS AS BranchToAdr, CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, " +
                        $"CASE WHEN M.AM = 'A' THEN 'AUTO' ELSE 'MANUAL' END AS AM FROM {tableMaster} M " +
                        $"LEFT JOIN TBL_BRANCH BF ON BF.BCODE = M.BCODE " +
                        $"LEFT JOIN TBL_BRANCH BT ON BT.BCODE = M.TBCODE " +
                        $"WHERE M.DLT = 'T' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}' AND M.TRAN_ID = {modelRecord.TRAN_ID}";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        masterData.V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]);
                        masterData.VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]);
                        masterData.REF = Convert.ToString(reader["REF"]);
                        masterData.REMARKS = Convert.ToString(reader["REMARKS"]);
                        masterData.BRANCH_FROM_NAME = Convert.ToString(reader["BranchFrom"]);
                        masterData.BRANCH_FROM_ADDRESS = Convert.ToString(reader["BranchFromAdr"]);
                        masterData.BRANCH_TO_NAME = Convert.ToString(reader["BranchTo"]);
                        masterData.BRANCH_TO_ADDRESS = Convert.ToString(reader["BranchToAdr"]);
                        masterData.ASTATUS = Convert.ToString(reader["ASTATUS"]);
                        masterData.STOCK_TYPE = Convert.ToString(reader["AM"]);
                    }
                    reader.Close();
                }

                if (menuDetails.MD_ID == 7)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        //string query = $"SELECT D.QTY, D.QTY2, D.BAL_QTY, B.BARCODE, IM.ITEM_NAME, IM.REMARKS, C.GROUP_NAME AS COLOR, " +
                        //               $"S.GROUP_NAME AS SIZE, IMG.GROUP_NAME AS 'GROUP', IMC.GROUP_NAME AS CATEGORY, " +
                        //               $"IMSC.GROUP_NAME AS SUBCATEGORY FROM {tableDetail} D " +
                        //               $"LEFT JOIN TBL_BARCODE B WITH (NOLOCK) ON B.CODE = D.ITEM_CODE " +
                        //               $"LEFT JOIN TBL_COLOR C WITH (NOLOCK) ON B.COLOR = C.GROUP_CODE " +
                        //               $"LEFT JOIN TBL_SIZE S WITH (NOLOCK) ON B.SIZE = S.GROUP_CODE " +
                        //               $"LEFT JOIN TBL_ITEMSMASTER IM WITH (NOLOCK) ON B.ITEM_CODE = IM.ITEM_CODE " +
                        //               $"LEFT JOIN TBL_ITEMSGROUP IMG WITH (NOLOCK) ON IMG.GROUP_CODE = IM.GROUP_CODE " +
                        //               $"LEFT JOIN TBL_CATEGORY IMC WITH (NOLOCK) ON IMC.GROUP_CODE = IM.CAT_CODE " +
                        //               $"LEFT JOIN TBL_SUB_CATEGORY IMSC WITH (NOLOCK) ON IMSC.GROUP_CODE = IM.SUB_CAT_CODE " +
                        //               $"WHERE D.DLT = 'T' AND B.DLT = 'T' AND B.ASTATUS= 'Y' AND IM.DLT = 'T' AND D.BCODE = '{common.Branch}' " +
                        //               $"AND D.PERIOD_ID = '{common.Period}' AND D.TRAN_ID = {modelRecord.TRAN_ID}";

                        string query = $@"SELECT D.QTY,  IM.ITEM_NAME, D.DT_DESC AS REMARKS,  
                                    IMG.GROUP_NAME AS 'GROUP',U.GROUP_NAME AS UNIT
                                    FROM {tableDetail} D 
                                    LEFT JOIN TBL_ITEMSMASTER IM WITH (NOLOCK) ON D.ITEM_CODE = IM.ITEM_CODE 
                                    LEFT JOIN TBL_ITEMSGROUP IMG WITH (NOLOCK) ON IMG.GROUP_CODE = IM.GROUP_CODE  
                                    LEFT JOIN TBL_UNIT U ON U.GROUP_CODE =D.UNIT
                                    WHERE D.DLT = 'T' AND IM.DLT = 'T' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}' AND D.TRAN_ID = {modelRecord.TRAN_ID}";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["Qty1"] = Convert.ToString(reader["QTY"]);
                            dataRow["DesignNo"] = Convert.ToString(reader["ITEM_NAME"]);
                            dataRow["Remarks"] = Convert.ToString(reader["REMARKS"]);
                            dataRow["Group"] = Convert.ToString(reader["GROUP"]);
                            dataRow["Unit"] = Convert.ToString(reader["UNIT"]);
                            //dataRow["Size"] = Convert.ToString(reader["SIZE"]);
                            //dataRow["Color"] = Convert.ToString(reader["COLOR"]);
                            //dataRow["Qty2"] = Convert.ToString(reader["QTY2"]);
                            //dataRow["BalQty"] = Convert.ToString(reader["BAL_QTY"]);
                            //dataRow["Category"] = Convert.ToString(reader["CATEGORY"]);
                            //dataRow["SubCategory"] = Convert.ToString(reader["SUBCATEGORY"]);
                            dataTable.Rows.Add(dataRow);
                        }
                        reader.Close();
                    }
                }
                else if (menuDetails.MD_ID == 53)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $"SELECT D.QTY, D.QTY2, D.BAL_QTY, B.BARCODE, IM.ITEM_NAME, IM.REMARKS, C.GROUP_NAME AS COLOR, B.SRATE AS RATE, " +
                                       $"S.GROUP_NAME AS SIZE, IMG.GROUP_NAME AS 'GROUP', IMC.GROUP_NAME AS CATEGORY, " +
                                       $"IMSC.GROUP_NAME AS SUBCATEGORY FROM {tableDetail} D " +
                                       $"LEFT JOIN TBL_BARCODE B WITH (NOLOCK) ON B.CODE = D.ITEM_CODE " +
                                       $"LEFT JOIN TBL_COLOR C WITH (NOLOCK) ON B.COLOR = C.GROUP_CODE " +
                                       $"LEFT JOIN TBL_SIZE S WITH (NOLOCK) ON B.SIZE = S.GROUP_CODE " +
                                       $"LEFT JOIN TBL_ITEMSMASTER IM WITH (NOLOCK) ON B.ITEM_CODE = IM.ITEM_CODE " +
                                       $"LEFT JOIN TBL_ITEMSGROUP IMG WITH (NOLOCK) ON IMG.GROUP_CODE = IM.GROUP_CODE " +
                                       $"LEFT JOIN TBL_CATEGORY IMC WITH (NOLOCK) ON IMC.GROUP_CODE = IM.CAT_CODE " +
                                       $"LEFT JOIN TBL_SUB_CATEGORY IMSC WITH (NOLOCK) ON IMSC.GROUP_CODE = IM.SUB_CAT_CODE " +
                                       $"WHERE D.DLT = 'T' AND B.DLT = 'T' AND B.ASTATUS= 'Y' AND IM.DLT = 'T' AND D.BCODE = '{common.Branch}' " +
                                       $"AND D.PERIOD_ID = '{common.Period}' AND D.TRAN_ID = {modelRecord.TRAN_ID}";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["Remarks"] = Convert.ToString(reader["REMARKS"]);
                            dataRow["DesignNo"] = Convert.ToString(reader["ITEM_NAME"]);
                            dataRow["Barcode"] = Convert.ToString(reader["BARCODE"]);
                            dataRow["Size"] = Convert.ToString(reader["SIZE"]);
                            dataRow["Color"] = Convert.ToString(reader["COLOR"]);
                            dataRow["Qty1"] = Convert.ToString(reader["QTY"]);
                            dataRow["Qty2"] = Convert.ToString(reader["QTY2"]);
                            dataRow["BalQty"] = reader["BAL_QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BAL_QTY"]);
                            dataRow["Group"] = Convert.ToString(reader["GROUP"]);
                            dataRow["Category"] = Convert.ToString(reader["CATEGORY"]);
                            dataRow["SubCategory"] = Convert.ToString(reader["SUBCATEGORY"]);
                            dataRow["Rate"] = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]);
                            dataRow["Amount"] = (reader["BAL_QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BAL_QTY"])) * (reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]));
                            dataTable.Rows.Add(dataRow);
                        }
                        reader.Close();
                    }
                }
                else if (menuDetails.MD_ID == 12)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $"SELECT IM.ITEM_NAME AS Item, IM.PURCHASE_RATE AS RATE, IM.GROUP_CODE, SUM(D.BAL_QTY) AS BAL_QTY, IG.GROUP_NAME FROM {tableDetail} D " +
                            $"LEFT JOIN TBL_ITEMSMASTER IM ON IM.ITEM_CODE = D.ITEM_CODE " +
                            $"LEFT OUTER JOIN TBL_STKTR_DETAIL STKR ON STKR.DT_CODE = D.PICK_ID AND STKR.BCODE = D.BCODE AND STKR.PERIOD_ID = D.PERIOD_ID " +
                            $"LEFT OUTER JOIN TBL_ITEMSGROUP IG ON IG.GROUP_CODE = STKR.ITEM_GROUP " +
                            $"WHERE D.DLT = 'T' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}' AND D.TRAN_ID = {modelRecord.TRAN_ID} GROUP BY IM.ITEM_NAME, IM.PURCHASE_RATE, IG.GROUP_NAME, IM.GROUP_CODE";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["BalQty"] = Convert.ToInt32(reader["BAL_QTY"]);
                            dataRow["Qty1"] = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]);
                            //dataRow["Color"] = _commonService.ToAccountingFormat(Convert.ToDecimal(reader["BalQty"]));
                            dataRow["Category"] = Convert.ToString(reader["Item"]);
                            dataRow["Group"] = Convert.ToString(reader["GROUP_NAME"]);
                            dataRow["DesignNo"] = reader["GROUP_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GROUP_CODE"]);
                            dataTable.Rows.Add(dataRow);
                        }
                        reader.Close();
                    }
                }
                else
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $"SELECT IM.ITEM_NAME AS Item, IG.GROUP_CODE, SUM(D.BAL_QTY) AS BAL_QTY, IG.GROUP_NAME FROM {tableDetail} D " +
                            $"LEFT JOIN TBL_ITEMSMASTER IM ON IM.ITEM_CODE = D.ITEM_CODE " +
                            $"LEFT OUTER JOIN TBL_STKTR_DETAIL STKR ON STKR.DT_CODE = D.PICK_ID AND STKR.BCODE = D.BCODE AND STKR.PERIOD_ID = D.PERIOD_ID " +
                            $"LEFT OUTER JOIN TBL_ITEMSGROUP IG ON IG.GROUP_CODE = STKR.ITEM_GROUP " +
                            $"WHERE D.DLT = 'T' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}' AND D.TRAN_ID = {modelRecord.TRAN_ID} GROUP BY IM.ITEM_NAME, IG.GROUP_NAME, IG.GROUP_CODE";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        // Dictionary to store unique groups and their series numbers
                        Dictionary<string, int> groupSeries = new Dictionary<string, int>();
                        int seriesCounter = 1;

                        while (reader.Read())
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["BalQty"] = Convert.ToInt32(reader["BAL_QTY"]);
                            dataRow["Category"] = Convert.ToString(reader["Item"]);
                            dataRow["Group"] = Convert.ToString(reader["GROUP_NAME"]);
                            string groupName = Convert.ToString(reader["GROUP_NAME"]);
                            if (!groupSeries.ContainsKey(groupName))
                            {
                                groupSeries[groupName] = seriesCounter;
                                seriesCounter++;
                            }
                            //dataRow["DesignNo"] = Convert.ToInt32(reader["GROUP_CODE"]);
                            dataRow["DesignNo"] = groupSeries[groupName];
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

        public MyHttpResponseMessage UpdatePrintStatus(string codes, Common common)
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
                    string connectionString = new SQLService().getconnstring();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = $"UPDATE {table} SET [PRINT] = 'Y' WHERE DT_CODE IN({codes}) AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();
                        response.msgType = 1;
                        response.msg = "Record Updated Successfully";
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

        public MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, int branch, Common common)
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
                        string query = @$"SELECT SM.TRAN_ID, SD.ITEM_GROUP, SM.V_DATE, SM.VOUCHER_NO, SD.ITEM_CODE, SD.QTY2, SD.DT_DESC, SD.BAL_QTY, IM.ITEM_NAME, IM.ITEM_ID, IG.GROUP_NAME, 
                                        ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {pickDetailTable}
                                        WHERE SD.DT_CODE = DT_CODE AND SD.PERIOD_ID = PERIOD_ID AND SD.BCODE = BCODE),0) AS SQTY,
                                        ISNULL((SELECT SUM(ISNULL(QTY, 0)) FROM {detailTable}
                                        WHERE DFD.PICK_ID = PICK_ID AND DFD.PERIOD_ID = PERIOD_ID AND DFD.BCODE = BCODE),0) AS DQTY,
                                        '' AS QTY,
                                        SD.DT_CODE as PICK_ID, SD.UNIT, SD.QTY2 FROM {pickMasterTable} SM
                                        LEFT OUTER JOIN {pickDetailTable} SD
                                        ON SD.TRAN_ID = SM.TRAN_ID AND SD.BCODE = SM.BCODE AND SD.PERIOD_ID = SM.PERIOD_ID
                                        LEFT OUTER JOIN {detailTable} DFD
                                        ON SD.DT_CODE = DFD.PICK_ID AND SD.BCODE = DFD.BCODE AND SD.PERIOD_ID = DFD.PERIOD_ID
                                        AND DFD.DLT = 'T'
                                        LEFT OUTER JOIN TBL_ITEMSMASTER IM
                                        ON IM.ITEM_CODE = SD.ITEM_CODE
                                        LEFT OUTER JOIN TBL_ITEMSGROUP IG
                                        ON IG.GROUP_CODE = SD.ITEM_GROUP
                                        WHERE SM.DLT = 'T' AND SM.ASTATUS = 'Y' AND SD.DLT = 'T'
                                        AND SM.V_DATE = '{sodaDate}' AND SM.TBCODE = {branch}
                                        GROUP BY
                                        SM.TRAN_ID , SM.V_DATE, SM.VOUCHER_NO, IG.GROUP_NAME, SD.ITEM_GROUP, SD.ITEM_CODE, SD.QTY2, SD.BAL_QTY, SD.DT_DESC, IM.ITEM_NAME, IM.ITEM_ID, SD.DT_CODE, SD.PERIOD_ID, SD.BCODE, DFD.PICK_ID,
                                        DFD.BCODE, DFD.PERIOD_ID, SD.UNIT ORDER BY IG.GROUP_NAME";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                V_DATE = Convert.ToString(reader["V_DATE"]),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                                PICK_ID = Convert.ToString(reader["PICK_ID"]),
                                UNIT = Convert.ToInt32(reader["UNIT"]),
                                QTY = Convert.ToString(reader["SQTY"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                SQTY = Convert.ToString(reader["SQTY"]),
                                DQTY = Convert.ToString(reader["DQTY"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                                ITEM_GROUP = Convert.ToString(reader["ITEM_GROUP"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
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