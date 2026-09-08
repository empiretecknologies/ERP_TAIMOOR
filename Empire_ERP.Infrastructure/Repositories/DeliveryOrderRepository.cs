using System.Data;
using System.Text;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class DeliveryOrderRepository : IDeliveryOrderRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public ICommonService _commonService { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public IPeriodRepository _periodRepository { get; set; }

        public DeliveryOrderRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonRepository commonRepository, ICommonService commonService, IPeriodRepository periodRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
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
                string? detailTable = string.Empty;
                string? search = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    search = menu.SEARCH;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();

                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        if (search == "M")
                        {
                            string query = "SELECT M.TRAN_ID AS TRAN_ID, M.TRAN_ID AS CODE,M.V_DATE,M.VOUCHER_NO,M.BTYPE," +
                                    "M.PARTY_CODE,M.ACT_CODE,M.REF,M.REMARKS,M.SCODE,M.SACODE,M.BCODE,M.PERIOD_ID," +
                                        "M.ADD_USER_ID,M.ADD_DATE,M.ADD_COMPUTER_NAME,M.ADD_IP_ADDRESS," +
                                        "M.EDIT_USER_ID,M.EDIT_DATE,M.EDIT_COMPUTER_NAME,M.EDIT_IP_ADDRESS,PT.PARTY_NAME," +
                                        "M.ADD_POSTALCODE,M.EDIT_POSTALCODE, CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS " +
                                        $"FROM {table} M " +
                                        $"LEFT OUTER JOIN TBL_PARTY_TYPES PT " +
                                        $"ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE " +
                                        $"WHERE  M.DLT = 'T' AND M.BCODE = '" + common.Branch + "' AND M.PERIOD_ID = '" + common.Period + "' ORDER BY M.TRAN_ID DESC";

                            SqlCommand command = new SqlCommand(query, connection);
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                var row = new
                                {
                                    ID = Convert.ToInt32(reader["TRAN_ID"]),
                                    CODE = Convert.ToInt32(reader["CODE"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                    V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                    PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                    ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                    REF = Convert.ToString(reader["REF"]),
                                    BTYPE = Convert.ToString(reader["BTYPE"]),
                                    REMARKS = Convert.ToString(reader["REMARKS"]),
                                    SCODE = reader["SCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SCODE"]),
                                    SACODE = reader["SACODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SACODE"]),
                                    ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                    ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
                                    ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                    ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                    EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                    EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
                                    EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                    EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                    PARTY_NAME = reader["PARTY_NAME"],
                                    ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                    EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                };
                                jsonDataResult.Add(row);
                            }
                            reader.Close();


                        }
                        else
                        {
                            var query = $@"SELECT M.TRAN_ID AS TRAN_ID, M.TRAN_ID AS CODE,M.V_DATE,M.VOUCHER_NO,M.BTYPE,M.PARTY_CODE,M.ACT_CODE,M.REF,D.DT_DESC,
                                            ROUND(SUM(D.NET_AMT),0) AS AMT,
                                            M.SCODE,M.SACODE,M.BCODE,M.PERIOD_ID,
                                            M.ADD_USER_ID,M.ADD_DATE,M.ADD_COMPUTER_NAME,M.ADD_IP_ADDRESS,M.EDIT_USER_ID,M.EDIT_DATE,
                                            M.EDIT_COMPUTER_NAME,M.EDIT_IP_ADDRESS,PT.PARTY_NAME,M.ADD_POSTALCODE,M.EDIT_POSTALCODE, 
                                            CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS 
                                            FROM {table} M 
                                            LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE 
                                            LEFT OUTER JOIN {detailTable} D
                                            ON D.TRAN_ID = M.TRAN_ID AND D.PERIOD_ID = M.PERIOD_ID AND D.BCODE = M.BCODE
                                            WHERE  M.DLT = 'T' AND M.BCODE = {common.Branch} AND M.PERIOD_ID = {common.Period} AND  D.DLT = 'T'
                                            GROUP BY 
                                            M.TRAN_ID  , M.TRAN_ID  ,M.V_DATE,M.VOUCHER_NO,M.BTYPE,M.PARTY_CODE,M.ACT_CODE,M.REF,D.DT_DESC,
                                            M.SCODE,M.SACODE,M.BCODE,M.PERIOD_ID,
                                            M.ADD_USER_ID,M.ADD_DATE,M.ADD_COMPUTER_NAME,M.ADD_IP_ADDRESS,M.EDIT_USER_ID,M.EDIT_DATE,
                                            M.EDIT_COMPUTER_NAME,M.EDIT_IP_ADDRESS,PT.PARTY_NAME,M.ADD_POSTALCODE,M.EDIT_POSTALCODE, M.ASTATUS 
                                            ORDER BY M.TRAN_ID DESC";
                            SqlCommand command = new SqlCommand(query, connection);
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                var row = new
                                {
                                    ID = Convert.ToInt32(reader["TRAN_ID"]),
                                    CODE = Convert.ToInt32(reader["CODE"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                    V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                    PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                    ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                    REF = Convert.ToString(reader["REF"]),
                                    REMARKS = Convert.ToString(reader["DT_DESC"]),
                                    AMT = Convert.ToString(reader["AMT"]),
                                    BTYPE = Convert.ToString(reader["BTYPE"]),
                                    SCODE = reader["SCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SCODE"]),
                                    SACODE = reader["SACODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SACODE"]),
                                    ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                    ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
                                    ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                    ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                    EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                    EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
                                    EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                    EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                    PARTY_NAME = reader["PARTY_NAME"],
                                    ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                    EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                    COMM = Convert.ToInt32(reader["COMM"]),
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

        public MyHttpResponseMessage GetDeliveryOrderByCode(int code, Common common)
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
                        string query = $"SELECT TRAN_ID, V_DATE,VOUCHER_NO,BTYPE, PARTY_CODE, ACT_CODE, REF, REMARKS, SCODE, SACODE, DEL_CODE, DEL_ACODE, BATCH_NO, DRIVER_NAME, BCODE, PERIOD_ID," +
                                       "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS," +
                                       "EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS," +
                                       "ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS " +
                                       $"FROM {table} WHERE DLT = 'T' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "' AND TRAN_ID = '" + code + "'";
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
                                BTYPE = Convert.ToString(reader["BTYPE"]),
                                REF = Convert.ToString(reader["REF"]),
                                SCODE = Convert.ToString(reader["SCODE"]),
                                SACODE = Convert.ToString(reader["SACODE"]),
                                DEL_CODE = Convert.ToString(reader["DEL_CODE"]),
                                DEL_ACODE = Convert.ToString(reader["DEL_ACODE"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                BATCH_NO = Convert.ToString(reader["BATCH_NO"]),
                                DRIVER_NAME = Convert.ToString(reader["DRIVER_NAME"]),
                                PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
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

        public MyHttpResponseMessage GetPickDataByParty(int partyCode, int actCode, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? detailTable = string.Empty;
                string? pickTable = string.Empty;
                string? pickDetailTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickTable = menu.PICK_TABLE_MASTER;
                    pickDetailTable = menu.PICK_TABLE_DETAIL;
                }

                if (!String.IsNullOrWhiteSpace(pickTable))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                         string query = $@"SELECT B.DT_CODE, A.V_DATE, A.VOUCHER_NO, AC.PARTY_NAME AS PARTY_NAME, A.REF, IT.ITEM_NAME, B.ITEM_CODE, B.UNIT, U.GROUP_NAME AS UNIT_NAME, B.BAL_QTY, B.RATE, B.AMT, B.DT_DESC 
                                        FROM {pickTable} A
                                        LEFT OUTER JOIN {pickDetailTable} B
                                        ON B.TRAN_ID = A.TRAN_ID AND B.BCODE = A.BCODE AND B.PERIOD_ID = A.PERIOD_ID
                                        LEFT OUTER JOIN TBL_PARTY_TYPES AC
                                        ON AC.PARTY_CODE = A.PARTY_CODE AND AC.ACT_CODE = A.ACT_CODE
										LEFT OUTER JOIN TBL_ITEMSMASTER IT
                                        ON IT.ITEM_CODE = B.ITEM_CODE
										LEFT OUTER JOIN TBL_UNIT U
										ON U.GROUP_CODE = B.UNIT
                                         WHERE A.BCODE = {common.Branch} And A.PERIOD_ID = {common.Period} AND A.DLT = 'T' AND A.ASTATUS = 'Y' 
                                         And A.PARTY_CODE = {partyCode} And A.ACT_CODE = {actCode}
                                         Group By
                                         A.TRAN_ID,
                                         A.V_DATE,
                                         A.VOUCHER_NO,
										 AC.PARTY_NAME,
										 A.REF,
										 B.DT_CODE,
										 B.ITEM_CODE, 
										 B.BAL_QTY, 
										 B.RATE, 
										 B.AMT, 
										 B.DT_DESC,
										 IT.ITEM_NAME,
										 B.UNIT,
										 U.GROUP_NAME,
                                         A.MENU_ID
                                         ORDER BY B.DT_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                PICK_ID = Convert.ToString(reader["DT_CODE"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]),
                                REF = Convert.ToString(reader["REF"]),
                                ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["BAL_QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                UNIT_NAME = Convert.ToString(reader["UNIT_NAME"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"])
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
                    string query = $@"SELECT IM.ITEM_NAME AS ITEM_ID, IM.ITEM_CODE AS ITEM_CODE, C.GROUP_CODE AS COLOR_ID, S.GROUP_CODE AS SIZE_ID,B.CODE AS BARCODE_CODE,S.GROUP_NAME AS SIZE, C.GROUP_NAME AS COLOR, B.BARCODE, B.SRATE AS RATE FROM TBL_BARCODE B
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

        public MyHttpResponseMessage GetDeliveryOrderPickDetailByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.PICK_TABLE_DETAIL;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT TRAN_ID,DT_CODE,M.ITEM_CODE, QTY,UNIT,QTY2,BAL_QTY,M.COLOR,M.SIZE, " +
                         " RATE,AMT,DISC,DISC_AMT, TAX,TAX_AMT,ADV,ADV_AMT,NET_AMT, DT_DESC, " +
                         " CL.GROUP_NAME AS COLORNAME,SL.GROUP_NAME AS SIZENAME,M.GRADE, WAREHOUSE,DEL_DATE,DUE_DATE, DUE_DAYS,VEH,BCODE,PERIOD_ID, M.ADD_USER_ID,M.ADD_DATE," +
                         " M.ADD_COMPUTER_NAME,M.ADD_IP_ADDRESS, M.EDIT_USER_ID,M.EDIT_DATE,M.EDIT_COMPUTER_NAME, " +
                         " M.EDIT_IP_ADDRESS, M.ADD_POSTALCODE,M.EDIT_POSTALCODE,M.MENU_ID,M.DLT,CHK,PICK_ID,PICK_ID_D " +
                         $" FROM {table} M LEFT OUTER JOIN TBL_BARCODE BG ON BG.CODE = M.ITEM_CODE " +
                         " LEFT OUTER JOIN TBL_SIZE SL ON SL.GROUP_CODE = BG.SIZE LEFT OUTER JOIN TBL_COLOR CL ON CL.GROUP_CODE = BG.COLOR" +
                         " WHERE  M.DLT = 'T' AND TRAN_ID = '" + code + "' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "'" +
                         " ORDER BY DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = 0,
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToInt32(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_AMT = Convert.ToString(reader["DISC_AMT"]),
                                TAX = Convert.ToString(reader["TAX"]),
                                TAX_AMT = Convert.ToString(reader["TAX_AMT"]),
                                ADV = Convert.ToString(reader["ADV"]),
                                ADV_AMT = Convert.ToString(reader["ADV_AMT"]),
                                NET_AMT = Convert.ToString(reader["NET_AMT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                COLOR = reader["COLOR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COLOR"]),
                                SIZE = reader["SIZE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE"]),
                                GRADE = Convert.ToInt32(reader["GRADE"]),
                                WAREHOUSE = Convert.ToInt32(reader["WAREHOUSE"]),
                                DEL_DATE = Convert.ToString(reader["DEL_DATE"]),
                                DUE_DATE = Convert.ToString(reader["DUE_DATE"]),
                                DUE_DAYS = Convert.ToString(reader["DUE_DAYS"]),
                                VEH = Convert.ToString(reader["VEH"]),
                                CHK = Convert.ToString(reader["CHK"]),
                                CHK1 = Convert.ToString(reader["CHK"]) == "1" ? true : false,
                                PICK_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                PICK_ID_D = Convert.ToInt32(reader["DT_CODE"])

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

        public MyHttpResponseMessage GetDeliveryOrderDetailByCode(int code, Common common)
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
                        string query = "SELECT TRAN_ID, DT_CODE, ITEM_CODE," +
                            " QTY, UNIT, QTY2, BAL_QTY, DIS_DATE, DIS_TIME, " +
                            " DT_DESC, WAREHOUSE, VEHICLE, BCODE, PERIOD_ID," +
                            " ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS," +
                            " EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS," +
                            " ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT, CHK, PICK_ID " +
                            $" FROM {table} " +
                            " WHERE DLT = 'T' AND TRAN_ID = '" + code + "' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "'" +
                            " ORDER BY DT_CODE DESC";
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
                                WAREHOUSE = Convert.ToInt32(reader["WAREHOUSE"]),
                                VEHICLE = Convert.ToString(reader["VEHICLE"]),
                                DIS_DATE = reader["DIS_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DIS_DATE"]).ToString("dd-MM-yyyy"),
                                CHK = Convert.ToString(reader["CHK"]),
                                DIS_TIME = Convert.ToString(reader["DIS_TIME"]),
                                CHK1 = Convert.ToString(reader["CHK"]) == "1" ? true : false,
                                PICK_ID = Convert.ToInt32(reader["PICK_ID"]),
                                //PICK_ID_D = Convert.ToInt32(reader["PICK_ID_D"])
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

        public MyHttpResponseMessage GetDeliveryOrderDetailByItem(int code, int qty, Common common)
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
                        string query = "SELECT B.CODE, B.SRATE, B.COLOR, B.SIZE, IT.ITEM_ID, IT.SALE_RATE FROM TBL_BARCODE B " +
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
                                ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
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
                                ADV = "",
                                ADV_AMT = "",
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

        public MyHttpResponseMessage Save(CustomDeliveryOrder modelRecord, Common common)
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
                    var periodInfo = _periodRepository.GetPeriodById(Convert.ToInt32(period));
                    string startDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
                    string endDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
                    var menuID = common.MenuID;
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
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlTransaction transaction = connection.BeginTransaction();
                            SqlCommand command = connection.CreateCommand();
                            command.Transaction = transaction;
                            try
                            {
                                string maxIdQuery1 = "SELECT SACT_CODE FROM TBL_PARTY_TYPES WHERE PARTY_CODE = '" + modelRecord.Master.SCODE + "'";
                                using (SqlConnection connectionNew = new SqlConnection(new SQLService().getconnstring()))
                                {
                                    SqlCommand commandNew1 = new SqlCommand(maxIdQuery1, connectionNew);
                                    connectionNew.Open();
                                    object result1 = commandNew1.ExecuteScalar();
                                    modelRecord.Master.SACODE = Convert.ToInt32(result1);
                                }
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
                                            "(TRAN_ID,V_DATE,VOUCHER_NO,PARTY_CODE,ACT_CODE," +
                                            "REF,REMARKS,SCODE,SACODE,DEL_CODE,DEL_ACODE,DRIVER_NAME,BATCH_NO,BCODE,PERIOD_ID,BTYPE," +
                                            "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                            "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                                            "EDIT_POSTALCODE,ASTATUS,MENU_ID,DLT)" +
                                            "VALUES" +
                                            "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.PARTY_CODE + "','" + modelRecord.Master.ACT_CODE + "'," +
                                            "'" + modelRecord.Master.REF + "','" + modelRecord.Master.REMARKS + "','" + modelRecord.Master.SCODE + "','" + modelRecord.Master.SACODE + "'," +
                                            "'" + modelRecord.Master.DEL_CODE + "','" + modelRecord.Master.DEL_ACODE + "','" + modelRecord.Master.DRIVER_NAME + "','" + modelRecord.Master.BATCH_NO + "','" + branch + "','" + period + "','" + modelRecord.Master.BTYPE + "'," +
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
                                                    REF = '" + modelRecord.Master.REF + @"',
                                                    REMARKS = '" + modelRecord.Master.REMARKS + @"',
                                                    SCODE = '" + modelRecord.Master.SCODE + @"',
                                                    SACODE = '" + modelRecord.Master.SACODE + @"',
                                                    DEL_CODE = '" + modelRecord.Master.DEL_CODE + @"',
                                                    DEL_ACODE = '" + modelRecord.Master.DEL_ACODE + @"',
                                                    BTYPE = '" + modelRecord.Master.BTYPE + @"',
                                                    DRIVER_NAME = '" + modelRecord.Master.DRIVER_NAME + @"',
                                                    BATCH_NO = '" + modelRecord.Master.BATCH_NO + @"',
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

                                StringBuilder insertQueryBuilder = new StringBuilder();
                                StringBuilder updateQueryBuilder = new StringBuilder();

                                bool allowInserts = true;
                                bool hasInserts = false;
                                bool hasUpdates = false;
                                int detailCode = GenerateNextDetailId(common, command);
                                foreach (var item in modelRecord.Detail.ToList())
                                {
                                    try
                                    {
                                        if (item.DT_CODE == null || item.DT_CODE == 0)
                                        {
                                            detailCode++;
                                            if (detailCode > 0)
                                            {
                                                if (!hasInserts)
                                                {
                                                    hasInserts = true;
                                                }

                                                insertQueryBuilder.AppendLine(
                                                    $"INSERT INTO {detailTable} (TRAN_ID, DT_CODE, ITEM_CODE, QTY, UNIT, QTY2, BAL_QTY, DT_DESC, WAREHOUSE, DIS_DATE, DIS_TIME, VEHICLE, BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT, CHK, PICK_ID) VALUES " +
                                                    $"('{modelRecord.Master.TRAN_ID}', '{detailCode}', '{item.ITEM_CODE}', '{item.QTY}', " +
                                                    $"'{item.UNIT}', '{item.QTY2}', '{item.BAL_QTY}', '{item.DT_DESC}', '{item.WAREHOUSE}', " +
                                                    $"'{item.DIS_DATE}', '{item.DIS_TIME}', '{item.VEHICLE}', '{branch}', '{period}', '{username}', " +
                                                    $"'{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', '{username}', " +
                                                    $"'{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', " +
                                                    $"'{Postal}', '{Postal}', '{menuID}', 'T', '{item.CHK}', '{item.PICK_ID}');");
                                            }
                                            else
                                            {
                                                isDetailAdded = false;
                                            }
                                        }
                                        else
                                        {
                                            if (!hasUpdates)
                                            {
                                                hasUpdates = true;
                                            }

                                            updateQueryBuilder.AppendLine(
                                                $"UPDATE {detailTable} SET " +
                                                $"ITEM_CODE = '{item.ITEM_CODE}', " +
                                                $"QTY = '{item.QTY}', " +
                                                $"UNIT = '{item.UNIT}', " +
                                                $"QTY2 = '{item.QTY2}', " +
                                                $"BAL_QTY = '{item.BAL_QTY}', " +
                                                $"DT_DESC = '{item.DT_DESC}', " +
                                                $"DIS_DATE = '{item.DIS_DATE}', " +
                                                $"DIS_TIME = '{item.DIS_TIME}', " +
                                                $"WAREHOUSE = '{item.WAREHOUSE}', " +
                                                $"VEHICLE = '{item.VEHICLE}', " +
                                                $"CHK = '{item.CHK}', EDIT_USER_ID = '{username}', " +
                                                $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                $"EDIT_COMPUTER_NAME = '{Computer}', EDIT_IP_ADDRESS = '{Ip}', " +
                                                $"EDIT_POSTALCODE = '{Postal}', DLT = 'T' " +
                                                $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND DT_CODE = '{item.DT_CODE}' " +
                                                $"AND BCODE = '{branch}' AND PERIOD_ID = '{period}';");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        isDetailAdded = false;
                                    }
                                }
                                if (hasInserts)
                                {
                                    command.CommandText = insertQueryBuilder.ToString();
                                    command.ExecuteNonQuery();
                                }
                                if (hasUpdates)
                                {
                                    command.CommandText = updateQueryBuilder.ToString();
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

                DeliveryOrder deliveryOrder = new DeliveryOrder();
                List<DeliveryOrderDetail> deliveryOrderDetailList = new List<DeliveryOrderDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        deliveryOrder = new DeliveryOrder
                        {
                            V_DATE = Convert.ToDateTime(reader["V_DATE"]),
                            PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            SCODE = Convert.ToInt32(reader["SCODE"]),
                            SACODE = Convert.ToInt32(reader["SACODE"]),
                            REF = Convert.ToString(reader["REF"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            BATCH_NO = Convert.ToString(reader["BATCH_NO"]),
                            DEL_CODE = Convert.ToInt32(reader["DEL_CODE"]),
                            DEL_ACODE = Convert.ToInt32(reader["DEL_ACODE"]),
                            BTYPE = Convert.ToString(reader["BTYPE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            DRIVER_NAME = Convert.ToString(reader["DRIVER_NAME"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new DeliveryOrderDetail
                        {
                            WAREHOUSE = Convert.ToInt32(detail_Reader["WAREHOUSE"]),
                            ITEM_CODE = Convert.ToInt32(detail_Reader["ITEM_CODE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                            UNIT = Convert.ToInt32(detail_Reader["UNIT"]),
                            QTY2 = Convert.ToDouble(detail_Reader["QTY2"]),
                            BAL_QTY = Convert.ToDouble(detail_Reader["BAL_QTY"]),
                            VEHICLE = Convert.ToString(detail_Reader["VEHICLE"]),
                            DIS_DATE = Convert.ToDateTime(detail_Reader["DIS_DATE"]),
                            DIS_TIME = Convert.ToString(detail_Reader["DIS_TIME"]),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            CHK = detail_Reader["CHK"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["CHK"]),
                            PICK_ID = Convert.ToInt32(detail_Reader["PICK_ID"]),
                            PICK_ID_D = detail_Reader["PICK_ID_D"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["PICK_ID_D"]),
                        };
                        deliveryOrderDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customRequisition = new CustomDeliveryOrder
                {
                    Master = deliveryOrder,
                    Detail = deliveryOrderDetailList
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

        public MyHttpResponseMessage DeleteDeliveryOrderDetailByCode(int code, Common common)
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

        public MyHttpResponseMessage GetDataForReport(DeliveryOrderRDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            DeliveryOrderRDLCReport masterData = new DeliveryOrderRDLCReport();
            CustomDeliveryOrderForPrintReport reportData = new CustomDeliveryOrderForPrintReport();
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
                if (menuDetails.MD_ID == 52)
                {
                    topQuery = @$"SELECT A.VOUCHER_NO, PT.PARTY_NAME, A.V_DATE, A.REMARKS AS COMMENT, A.REF, A.BATCH_NO, A.DRIVER_NAME, A.EDIT_USER_ID AS USER_NAME, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' else 'Inactive' END AS ASTATUS 
                                FROM {table} A 
                                LEFT OUTER JOIN TBL_PARTY_TYPES PT 
                                ON PT.PARTY_CODE = A.PARTY_CODE AND PT.ACT_CODE = A.ACT_CODE 
                                Where  a.BCODE =  '{common.Branch}' And a.PERIOD_ID =  '{common.Period}'   AND A.DLT = 'T' AND A.TRAN_ID = '{modelRecord.TRAN_ID}'";

                    query = @$"SELECT IT.ITEM_NAME, W.DESCR, A.DT_DESC AS DESCRIPTION, U.GROUP_NAME AS UNIT, A.QTY, A.QTY2, A.BAL_QTY, A.VEHICLE  
                            FROM {detailTable} A 
                            LEFT OUTER JOIN TBL_ITEMSMASTER IT
                            ON IT.ITEM_CODE = A.ITEM_CODE  
                            LEFT OUTER JOIN TBL_WAREHOUSE W
                            ON W.CODE = A.WAREHOUSE     
                            LEFT OUTER JOIN TBL_UNIT U
                            ON U.GROUP_CODE = A.UNIT   
                            WHERE A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' AND A.DLT = 'T' AND A.TRAN_ID = '{modelRecord.TRAN_ID}'";
                }

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(topQuery, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        masterData.INVOICE_NUMBER = Convert.ToString(reader["VOUCHER_NO"]);
                        masterData.DRIVER_NAME = Convert.ToString(reader["DRIVER_NAME"]);
                        masterData.DELIVERYMAN = Convert.ToString(reader["DRIVER_NAME"]);
                        masterData.PARTY = Convert.ToString(reader["PARTY_NAME"]);
                        masterData.SALESMAN = Convert.ToString(reader["DRIVER_NAME"]);
                        masterData.BATCH_NO = Convert.ToString(reader["BATCH_NO"]);
                        masterData.REFERENCE = Convert.ToString(reader["REF"]);
                        masterData.DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                        masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                        masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                        masterData.SIG1 = $"{menuDetails.MENU_SIG1}";
                        masterData.SIG2 = $"{menuDetails.MENU_SIG2}";
                        masterData.SIG3 = $"{menuDetails.MENU_SIG3}";
                        masterData.SIG4 = $"{menuDetails.MENU_SIG4}";
                        masterData.COMMENT = Convert.ToString(reader["COMMENT"]);
                        masterData.USER = reader["USER_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["USER_NAME"]);
                    }
                    reader.Close();
                }

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (menuDetails.MD_ID == 52)
                    {
                        while (reader.Read())
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["Item"] = Convert.ToString(reader["ITEM_NAME"]);
                            dataRow["Qty"] = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]);
                            dataRow["Description"] = Convert.ToString(reader["DESCRIPTION"]);
                            dataRow["Unit"] = Convert.ToString(reader["UNIT"]);
                            dataRow["Qty2"] = reader["QTY2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY2"]);
                            dataRow["BalQty"] = reader["BAL_QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BAL_QTY"]);
                            dataRow["Warehouse"] = Convert.ToString(reader["DESCR"]);
                            dataRow["Vehicle"] = Convert.ToString(reader["VEHICLE"]);
                            dataTable.Rows.Add(dataRow);
                        }
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