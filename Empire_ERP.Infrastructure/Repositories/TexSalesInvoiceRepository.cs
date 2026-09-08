using System.Data;
using System.Text;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using ZXing;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class TexSalesInvoiceRepository : ITexSalesInvoiceRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public ICommonService _commonService { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public IPeriodRepository _periodRepository { get; set; }

        public TexSalesInvoiceRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonRepository commonRepository, ICommonService commonService, IPeriodRepository periodRepository)
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
                string? pickMaster = string.Empty;
                string? pickDetail = string.Empty;
                string? search = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickMaster = menu.PICK_TABLE_MASTER;
                    pickDetail = menu.PICK_TABLE_DETAIL;
                    search = menu.SEARCH;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();

                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        if (search == "M")
                        {

                            //string query = $@"SELECT M.TRAN_ID AS TRAN_ID, M.TRAN_ID AS CODE,M.V_DATE,
                            //                       M.VOUCHER_NO, M.REF, M.REMARKS,
                            //                PT.PARTY_NAME, MR.CLIENT_PO, CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS 
                            //                FROM TBL_TSB_MASTER M 
                            //                LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE 
                            //                LEFT OUTER JOIN TBL_MPO_REG MR ON MR.CODE = M.CLIENT_PO
                            //                WHERE  M.DLT = 'T' AND M.BCODE = '1' AND M.PERIOD_ID = '1' ORDER BY M.TRAN_ID DESC";

                            //string quercy = $@"SELECT M.TRAN_ID AS TRAN_ID, M.TRAN_ID AS CODE,M.V_DATE,
                            //                    M.VOUCHER_NO, M.REF, M.REMARKS,
                            //                    PT.PARTY_NAME, MPO.CLIENT_PO,MPO.JOB_NO , CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS 
                            //                    FROM {table} M 
                            //                    LEFT OUTER JOIN {detailTable} D ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
                            //                    LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE 
                            //                    LEFT OUTER JOIN {pickDetail} MD ON MD.DT_CODE = D.PICK_ID_D AND D.BCODE = MD.BCODE AND D.PERIOD_ID = MD.PERIOD_ID
                            //                    LEFT OUTER JOIN {pickMaster} MM ON MM.TRAN_ID = MD.TRAN_ID AND MM.BCODE = MD.BCODE AND MM.PERIOD_ID = MD.PERIOD_ID
                            //                    LEFT OUTER JOIN TBL_MPO_MASTER MPO ON MPO.TRAN_ID = MM.JOB_NO
                            //                    WHERE  M.DLT = 'T' AND M.BCODE = '1' AND M.PERIOD_ID = '1' ORDER BY M.TRAN_ID DESC";


                            string query = $@"SELECT M.TRAN_ID, M.TRAN_ID AS CODE, CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS ,M.V_DATE,M.MD_ID,
                                            M.VOUCHER_NO, M.REF, PT.PARTY_NAME, M.DOC , M.REMARKS
                                            FROM TBL_TSB_MASTER M 
                                            LEFT OUTER JOIN {detailTable} D ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
                                            LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE 
                                            WHERE  M.DLT = 'T' AND M.BCODE = '1' AND M.PERIOD_ID = '1'
                                            GROUP BY M.TRAN_ID , M.ASTATUS, M.V_DATE, M.MD_ID, M.VOUCHER_NO, M.REF, PT.PARTY_NAME, M.DOC , M.REMARKS
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
                                    V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                    REF = Convert.ToString(reader["REF"]),
                                    REMARKS = Convert.ToString(reader["REMARKS"]),
                                    PARTY_NAME = reader["PARTY_NAME"],
                                    DOC = Convert.ToString(reader["DOC"]),
                                    //JOB_NO = Convert.ToString(reader["JOB_NO"]),
                                    ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                };
                                jsonDataResult.Add(row);
                            }
                            reader.Close();


                        }
                        else
                        {
                            var query = $@"SELECT M.TRAN_ID AS TRAN_ID, M.TRAN_ID AS CODE,M.V_DATE,M.VOUCHER_NO,M.BTYPE,M.COMM,M.PARTY_CODE,M.ACT_CODE,M.REF,D.DT_DESC,
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
                                            M.TRAN_ID  , M.TRAN_ID  ,M.V_DATE,M.VOUCHER_NO,M.BTYPE,M.COMM,M.PARTY_CODE,M.ACT_CODE,M.REF,D.DT_DESC,
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


        public MyHttpResponseMessage GetPurchaseBillByCode(int code, Common common)
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
                        string query = $@"SELECT * FROM {table} WHERE DLT = 'T' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}' AND TRAN_ID = '{code}'";
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
                                DOC = Convert.ToString(reader["DOC"]),
                                PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                                CLIENT_PO = reader["CLIENT_PO"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CLIENT_PO"]),
                                MD_ID = reader["MD_ID"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MD_ID"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                SUP_INVNO = Convert.ToString(reader["SUP_INVNO"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),

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

        public MyHttpResponseMessage GetPickDataBySupplier(int pCode, int actCode, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? detailTable = string.Empty;
                string? pickTable = string.Empty;
                string? pickDetailTable = string.Empty;
                string? pickType = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickTable = menu.PICK_TABLE_MASTER;
                    pickDetailTable = menu.PICK_TABLE_DETAIL;
                    pickType = menu.PICK_TYPE;
                }


                if (pickType == "MPO")
                {
                    if (!String.IsNullOrWhiteSpace(table))
                    {
                        List<object> jsonDataResult = new List<object>();
                        using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                        {

                            string query = $@"SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, P.PARTY_NAME AS CLIENT_NAME, S.PARTY_NAME AS SUPP_NAME, MPO.CLIENT_PO,
                                                MPO.CURR_CODE, MPO.JOB_NO, I.ITEM_NAME, U.GROUP_NAME AS UNIT_NAME, B.RATE, 
                                                SUM((ISNULL(B.QTY, 0))) AS QTY,
                                                SUM((ISNULL(B.AMT, 0))) AS AMT,
                                                MPO.COMM_AMT AS COMM_TYPE,
                                                MPO.COMM  AS COMM_RATE,
                                                SUM(ISNULL(MPO.COMM*B.AMT/100,0)-ISNULL(TSB.COMM_AMT,0)) AS COMM_AMT, 
                                                B.DT_CODE, MPO.PARTY_CODE, MPO.ACT_CODE, MPO.ITEM_CODE, MPO.SPARTY_CODE, MPO.SACT_CODE, B.DOC, MPO.UNIT,
                                                MPO.CRATE, C.DESCR, MB.ID AS MENU_ID, MB.MENU_PAGE, MB.MENU_PARENT_CODE 
                                                FROM {pickTable} A
                                                LEFT OUTER JOIN {pickDetailTable} B ON B.TRAN_ID = A.TRAN_ID AND B.BCODE = A.BCODE AND B.PERIOD_ID = A.PERIOD_ID
                                                LEFT OUTER JOIN {detailTable} TSB ON TSB.PICK_ID = B.DT_CODE AND TSB.BCODE = A.BCODE AND TSB.PERIOD_ID = A.PERIOD_ID
                                                LEFT OUTER JOIN TBL_MPO_MASTER MPO ON MPO.TRAN_ID = A.JOB_NO AND MPO.BCODE = A.BCODE AND MPO.PERIOD_ID = A.PERIOD_ID
                                                LEFT OUTER JOIN TBL_PARTY_TYPES P ON P.PARTY_CODE = MPO.PARTY_CODE AND P.ACT_CODE = MPO.ACT_CODE
                                                LEFT OUTER JOIN TBL_PARTY_TYPES S ON S.PARTY_CODE = A.SPARTY_CODE AND S.ACT_CODE = A.SACT_CODE
                                                LEFT OUTER JOIN TBL_UNIT U ON U.GROUP_CODE = MPO.UNIT
                                                LEFT OUTER JOIN TBL_ITEMSMASTER I ON I.ITEM_CODE = MPO.ITEM_CODE
                                                LEFT OUTER JOIN TBL_CURRENCY C ON C.CODE = MPO.CURR_CODE
                                                LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = A.MENU_ID 
                                                WHERE S.PARTY_CODE = {pCode} AND S.ACT_CODE = {actCode} AND A.BCODE = {common.Branch} And A.PERIOD_ID = {common.Period} AND A.DLT = 'T' AND B.DLT = 'T' AND A.ASTATUS = 'Y'
                                                GROUP BY A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, P.PARTY_NAME, S.PARTY_NAME , MPO.CLIENT_PO, MPO.CURR_CODE,
                                                MPO.JOB_NO, I.ITEM_NAME, U.GROUP_NAME, B.RATE,MPO.COMM_AMT , B.DT_CODE,MPO.COMM, MPO.PARTY_CODE, MPO.ACT_CODE, MPO.ITEM_CODE, MPO.SPARTY_CODE, MPO.SACT_CODE, B.DOC, MPO.UNIT,
                                                MPO.CRATE, C.DESCR, MB.ID, MB.MENU_PAGE, MB.MENU_PARENT_CODE 
                                                HAVING SUM(ISNULL(MPO.COMM*B.AMT/100,0)-ISNULL(TSB.COMM_AMT,0)) <> 0";



                            SqlCommand command = new SqlCommand(query, connection);
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                var row = new
                                {
                                    ID = Convert.ToString(reader["TRAN_ID"]),
                                    LB_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy"),
                                    VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? "" : Convert.ToString(reader["VOUCHER_NO"]),
                                    PARTY_NAME = reader["CLIENT_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["CLIENT_NAME"]),
                                    SUP_NAME = reader["SUPP_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["SUPP_NAME"]),
                                    CLIENT_PO = reader["CLIENT_PO"] == DBNull.Value ? "" : Convert.ToString(reader["CLIENT_PO"]),
                                    JOB_NO = reader["JOB_NO"] == DBNull.Value ? "" : Convert.ToString(reader["JOB_NO"]),
                                    ITEM_NAME = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    UNIT_NAME = reader["UNIT"] == DBNull.Value ? "" : Convert.ToString(reader["UNIT"]),
                                    RATE = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["RATE"]),
                                    QTY = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                    AMT = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["AMT"]),
                                    CRATE = reader["CRATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["CRATE"]),
                                    DESCR = reader["DESCR"] == DBNull.Value ? "" : Convert.ToString(reader["DESCR"]),
                                    COMM_TYPE = reader["COMM_TYPE"] == DBNull.Value ? "" : Convert.ToString(reader["COMM_TYPE"]),
                                    COMM_RATE = reader["COMM_RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["COMM_RATE"]),
                                    //COMM_AMT = reader["COMM_AMT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMM_AMT"]),
                                    PICK_ID_D = reader["DT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DT_CODE"]),
                                    PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                    ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                    SPARTY_CODE = reader["SPARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SPARTY_CODE"]),
                                    SACT_CODE = reader["SACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SACT_CODE"]),
                                    ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                    CURR_CODE = reader["CURR_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CURR_CODE"]),
                                    DOC = reader["DOC"] == DBNull.Value ? "" : Convert.ToString(reader["DOC"]),
                                    PARTY_DDL = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",
                                    UNIT = reader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UNIT"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),

                                    //PARTY_NAME = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
                                    //SUP_NAME = reader["SUP_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["SUP_NAME"]),
                                    //REF = reader["REF"] == DBNull.Value ? "" : Convert.ToString(reader["REF"]),
                                }
                            ;
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
                else
                {
                    response.data = "";
                    response.msg = "Pick data is not mapped. Please contact the administrator.";
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

        public MyHttpResponseMessage GetPurchaseBillPickDetailByCode(int code, Common common)
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
                         " FROM {table} M LEFT OUTER JOIN TBL_BARCODE BG ON BG.CODE = M.ITEM_CODE " +
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

        public MyHttpResponseMessage GetPurchaseBillDetailByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty;
                string? pickMaster = string.Empty, pickDetail = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickMaster = menu.PICK_TABLE_MASTER;
                    pickDetail = menu.PICK_TABLE_DETAIL;
                }

                if (!String.IsNullOrWhiteSpace(detailTable))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@" SELECT D.DT_CODE, D.ITEM_CODE, D.CLIENT_PO, D.PARTY_CODE, D.ACT_CODE, D.QTY, D.UNIT, D.RATE, D.AMT, D.NET_AMT, D.DT_DESC, D.PICK_ID_D, D.PICK_ID, D.COMM_TYPE,
                                            D.COMM_RATE, D.COMM_AMT, D.CURR_CODE, D.CRATE, MPOM.VOUCHER_NO, MPOM.TRAN_ID, MB.ID AS MENU_ID, MB.MENU_PAGE, MB.MENU_PARENT_CODE
                                             FROM {detailTable} D
                                             LEFT OUTER JOIN {pickDetail} MPOD ON MPOD.DT_CODE = D.PICK_ID_D
                                             LEFT OUTER JOIN {pickMaster} MPOM ON MPOD.TRAN_ID = MPOM.TRAN_ID
                                             LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = MPOD.MENU_ID
                                             WHERE D.DLT = 'T' AND D.TRAN_ID = '{code}' AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}' ORDER BY D.DT_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                ID = Convert.ToString(reader["TRAN_ID"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                PARTY_DDL = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = reader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UNIT"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                NET_AMT = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                PICK_ID_D = reader["PICK_ID_D"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PICK_ID_D"]),
                                PICK_ID = reader["PICK_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PICK_ID"]),
                                COMM_TYPE = Convert.ToString(reader["COMM_TYPE"]),
                                CLIENT_PO = reader["CLIENT_PO"] == DBNull.Value ? "" : Convert.ToString(reader["CLIENT_PO"]),
                                COMM_RATE = reader["COMM_RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["COMM_RATE"]),
                                COMM_AMT = reader["COMM_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["COMM_AMT"]),
                                CURR_CODE = reader["CURR_CODE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CURR_CODE"]),
                                CRATE = reader["CRATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CRATE"]),
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

        public MyHttpResponseMessage GetPurchaseBillDetailByItem(int code, int qty, Common common)
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

        public MyHttpResponseMessage Save(CustomTexSalesInvoice modelRecord, Common common)
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

                    if (stk_status == "Y")
                    {
                        Dictionary<int?, double?> currentItems = new Dictionary<int?, double?>();
                        Dictionary<int?, double?> previousItems = new Dictionary<int?, double?>();
                        Dictionary<int?, double?> stockBalance = new Dictionary<int?, double?>();
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
                                            "(TRAN_ID,V_DATE,VOUCHER_NO,PARTY_CODE,ACT_CODE,DOC," +
                                            "REF,CLIENT_PO,REMARKS,BCODE,PERIOD_ID,MD_ID,SUP_INVNO," +
                                            "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                            "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                                            "EDIT_POSTALCODE,ASTATUS,MENU_ID,DLT)" +
                                            "VALUES" +
                                            "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.PARTY_CODE + "','" + modelRecord.Master.ACT_CODE + "','" + modelRecord.Master.DOC + "'," +
                                            "'" + modelRecord.Master.REF + "','" + modelRecord.Master.CLIENT_PO + "','" + modelRecord.Master.REMARKS + "'," +
                                            "'" + branch + "','" + period + "','" + modelRecord.Master.MD_ID + "','" + modelRecord.Master.SUP_INVNO + "'," +
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
                                                    CLIENT_PO = '" + modelRecord.Master.CLIENT_PO + @"',
                                                    MD_ID = '" + modelRecord.Master.MD_ID + @"',
                                                    DOC = '" + modelRecord.Master.DOC + @"',
                                                    SUP_INVNO = '" + modelRecord.Master.SUP_INVNO + @"',
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
                                                    $"INSERT INTO {detailTable} (TRAN_ID, DT_CODE, ITEM_CODE, PARTY_CODE, ACT_CODE, QTY, UNIT, RATE, AMT, COMM_TYPE, COMM_RATE, COMM_AMT, CURR_CODE, CRATE," +
                                                    $"NET_AMT, DT_DESC, BCODE, PERIOD_ID, CLIENT_PO," +
                                                    $"ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, " +
                                                    $"EDIT_POSTALCODE, MENU_ID, DLT, PICK_ID, PICK_ID_D) VALUES " +

                                                    $"('{modelRecord.Master.TRAN_ID}','{detailCode}','{item.ITEM_CODE}','{item.PARTY_CODE}','{item.ACT_CODE}','{item.QTY}'," +
                                                    $"'{item.UNIT}', '{item.RATE}','{item.AMT}','{item.COMM_TYPE}','{item.COMM_RATE}','{item.COMM_AMT}','{item.CURR_CODE}','{item.CRATE}', " +
                                                    $"'{item.NET_AMT}', '{item.DT_DESC}','{branch}', '{period}', '{item.CLIENT_PO}', '{username}', " +
                                                    $"'{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', '{username}', " +
                                                    $"'{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', " +
                                                    $"'{Postal}', '{Postal}', '{menuID}', 'T', '{item.PICK_ID}', '{item.PICK_ID_D}');");
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
                                                $"ITEM_CODE = '{item.ITEM_CODE}', QTY = '{item.QTY}', PARTY_CODE = '{item.PARTY_CODE}', ACT_CODE = '{item.ACT_CODE}', UNIT = '{item.UNIT}', " +
                                                $"QTY2 = '{item.QTY2}', BAL_QTY = '{item.BAL_QTY}', COMM_TYPE = '{item.COMM_TYPE}', COMM_RATE = '{item.COMM_RATE}', COMM_AMT = '{item.COMM_AMT}', RATE = '{item.RATE}', " +
                                                $"AMT = '{item.AMT}', DISC = '{item.DISC}', DISC_AMT = '{item.DISC_AMT}', CURR_CODE = '{item.CURR_CODE}',  CRATE = '{item.CRATE}'," +
                                                $"TAX = '{item.TAX}', TAX_AMT = '{item.TAX_AMT}', CLIENT_PO = '{item.CLIENT_PO}',ADV = '{item.ADV}', ADV_AMT = '{item.ADV_AMT}', NET_AMT = '{item.NET_AMT}', " +
                                                $"DT_DESC = '{item.DT_DESC}', COLOR = '{item.COLOR}', SIZE = '{item.SIZE}', " +
                                                $"GRADE = '{item.GRADE}', WAREHOUSE = '{item.WAREHOUSE}', DEL_DATE = '{item.DEL_DATE}', " +
                                                $"DUE_DATE = '{item.DUE_DATE}', DUE_DAYS = '{item.DUE_DAYS}', VEH = '{item.VEH}', " +
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

                TexSalesInvoice texSalesInvoice = new TexSalesInvoice();
                List<TexSalesInvoiceDetail> texSalesInvoiceDetailList = new List<TexSalesInvoiceDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        texSalesInvoice = new TexSalesInvoice
                        {
                            V_DATE = Convert.ToDateTime(reader["V_DATE"]),
                            PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            SCODE = Convert.ToInt32(reader["SCODE"]),
                            SACODE = Convert.ToInt32(reader["SACODE"]),
                            COMM = Convert.ToDouble(reader["COMM"]),
                            REF = Convert.ToString(reader["REF"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            BTYPE = Convert.ToString(reader["BTYPE"]),
                            BCODE = Convert.ToInt32(reader["BCODE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            DISC = Convert.ToDouble(reader["DISC"]),
                            HS_CODE = Convert.ToString(reader["HS_CODE"]),
                            DOC = Convert.ToString(reader["DOC"]),
                            CURR_CODE = Convert.ToInt32(reader["CURR_CODE"]),
                            CRATE = Convert.ToDouble(reader["CRATE"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new TexSalesInvoiceDetail
                        {
                            ITEM_CODE = Convert.ToInt32(detail_Reader["ITEM_CODE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                            UNIT = Convert.ToInt32(detail_Reader["UNIT"]),
                            QTY2 = Convert.ToDouble(detail_Reader["QTY2"]),
                            BAL_QTY = Convert.ToDouble(detail_Reader["BAL_QTY"]),
                            RATE = Convert.ToDouble(detail_Reader["RATE"]),
                            AMT = Convert.ToDouble(detail_Reader["AMT"]),
                            DISC = Convert.ToDouble(detail_Reader["DISC"]),
                            DISC_AMT = Convert.ToDouble(detail_Reader["DISC_AMT"]),
                            TAX = Convert.ToDouble(detail_Reader["TAX"]),
                            TAX_AMT = Convert.ToDouble(detail_Reader["TAX_AMT"]),
                            NET_AMT = Convert.ToDouble(detail_Reader["NET_AMT"]),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            COLOR = Convert.ToInt32(detail_Reader["COLOR"]),
                            SIZE = Convert.ToInt32(detail_Reader["SIZE"]),
                            GRADE = Convert.ToInt32(detail_Reader["GRADE"]),
                            WAREHOUSE = Convert.ToInt32(detail_Reader["WAREHOUSE"]),
                            DEL_DATE = Convert.ToDateTime(detail_Reader["DEL_DATE"]),
                            DUE_DATE = Convert.ToDateTime(detail_Reader["DUE_DATE"]),
                            DUE_DAYS = Convert.ToInt32(detail_Reader["DUE_DAYS"]),
                            VEH = Convert.ToString(detail_Reader["VEH"]),
                            CHK = detail_Reader["CHK"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["CHK"]),
                            PICK_ID = Convert.ToInt32(detail_Reader["PICK_ID"]),
                            PICK_ID_D = detail_Reader["PICK_ID_D"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["PICK_ID_D"]),
                            ADV = Convert.ToDouble(detail_Reader["ADV"]),
                            ADV_AMT = Convert.ToDouble(detail_Reader["ADV_AMT"]),
                            PARTY_CODE = Convert.ToInt32(detail_Reader["PARTY_CODE"]),
                            ACT_CODE = Convert.ToInt32(detail_Reader["ACT_CODE"]),
                        };
                        texSalesInvoiceDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customRequisition = new CustomTexSalesInvoice
                {
                    Master = texSalesInvoice,
                    Detail = texSalesInvoiceDetailList
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

        public MyHttpResponseMessage DeletePurchaseBillDetailByCode(int code, Common common)
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

        public MyHttpResponseMessage GetDataForReport(TexSalesInvoiceRDLCReport modelRecord, DataTable inspectionServiceChargesDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            CustomMenuDetail menuDetails = new CustomMenuDetail();
            TexSalesInvoiceRDLCReport masterData = new TexSalesInvoiceRDLCReport();
            CustomTexSalesInvoiceForPrintReport reportData = new CustomTexSalesInvoiceForPrintReport();
            var Menu = _menuRepository.GetMenu(common.MenuID);
            string? table = string.Empty, detailTable = string.Empty;
            string? pickMaster = string.Empty, pickDetail = string.Empty;

            var menuResponse = _menuRepository.GetMenuDetails(common.MenuID);

            if (menuResponse.msgType != 1)
            {
                response.msgType = 2;
                response.msg = "Some thing went wrong !";
                return response;
            }

            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                table = menu.TABLE1;
                detailTable = menu.TABLE2;
                pickMaster = menu.PICK_TABLE_MASTER;
                pickDetail = menu.PICK_TABLE_DETAIL;
            }
            try
            {
                int mdId = 0;

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string checkQuery = $@"SELECT * FROM {table} WHERE TRAN_ID = {modelRecord.TRAN_ID}";

                    SqlCommand command = new SqlCommand(checkQuery, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        mdId = reader["MD_ID"] != DBNull.Value ? Convert.ToInt32(reader["MD_ID"]) : 0;
                    }
                    reader.Close();

                }

                
                var menuData = (List<CustomMenuDetail>)menuResponse.data;
                if (menuData.Count > 0)
                {
                    menuDetails = menuData.Where(m => m.MD_ID == mdId).FirstOrDefault();
                    if (menuDetails?.MD_ID <= 0)
                    {
                        response.msgType = 2;
                        response.msg = "Some thing went wrong !";
                        return response;
                    }
                }
                else
                {
                    response.msgType = 2;
                    response.msg = "Some thing went wrong !";
                    return response;
                }

                if ( mdId == modelRecord.MD_ID && mdId != 0 || modelRecord.MD_ID == 0 || modelRecord.MD_ID == null)
                {
                    string query = $@"SELECT CM.C_NAME AS COMPANY_NAME, BR.B_ADDRESS,BR.B_TEL,BR.B_GST,BR.B_NTN,
                                        A.PARTY_CODE, PT.PARTY_NAME, A.REF, C.DESCR2 AS CURR, C.SHORT_NAME AS CURR_SIG ,
                                        A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, A.SUP_INVNO, A.EDIT_USER_ID, AC.PARTY_NAME AS BUYER_NAME,
                                        A.REF,SUM(B.QTY) AS QTY, SUM(B.NET_AMT) AS AMT, I.ITEM_NAME, B.COMM_TYPE, B.COMM_RATE AS COMMISSION,
                                        SUM(B.COMM_AMT) AS COMMISSION_AMOUNT,MPO.JOB_NO, MPO.CLIENT_PO
                                        FROM {table} A
                                        LEFT OUTER JOIN {detailTable} B ON B.TRAN_ID = A.TRAN_ID AND B.BCODE = A.BCODE AND B.PERIOD_ID = A.PERIOD_ID
                                        LEFT OUTER JOIN TBL_PARTY_TYPES AC ON AC.PARTY_CODE = B.PARTY_CODE AND AC.ACT_CODE = B.ACT_CODE
                                        LEFT OUTER JOIN TBL_ITEMSMASTER I ON I.ITEM_CODE = B.ITEM_CODE
                                        LEFT OUTER JOIN {pickDetail} MD ON MD.DT_CODE = B.PICK_ID_D AND B.BCODE = MD.BCODE AND B.PERIOD_ID = MD.PERIOD_ID
										LEFT OUTER JOIN {pickMaster} MM ON MM.TRAN_ID = MD.TRAN_ID AND MM.BCODE = MD.BCODE AND MM.PERIOD_ID = MD.PERIOD_ID
                                        LEFT OUTER JOIN TBL_MPO_MASTER MPO ON MPO.TRAN_ID = MM.JOB_NO
                                        LEFT OUTER JOIN TBL_BRANCH BR ON BR.BCODE = A.BCODE
                                        LEFT OUTER JOIN TBL_COMPANY CM ON CM.CCODE = BR.CCODE
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = A.PARTY_CODE AND PT.ACT_CODE = A.ACT_CODE
                                        LEFT OUTER JOIN TBL_CURRENCY C ON C.CODE = B.CURR_CODE 
                                        WHERE A.TRAN_ID = {modelRecord.TRAN_ID} AND A.BCODE = {common.Branch} And A.PERIOD_ID = {common.Period} AND A.DLT = 'T' AND B.DLT = 'T' AND A.ASTATUS = 'Y'
                                        GROUP BY A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, A.SUP_INVNO, A.EDIT_USER_ID, AC.PARTY_NAME, A.REF, I.ITEM_NAME, B.COMM_TYPE, B.COMM_RATE,
                                        CM.C_NAME ,BR.B_ADDRESS,BR.B_TEL,BR.B_GST,BR.B_NTN,A.PARTY_CODE ,PT.PARTY_NAME, A.REF, C.DESCR2 , C.SHORT_NAME ,MPO.JOB_NO, MPO.CLIENT_PO";



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
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        bool firstRecord = true;

                        while (reader.Read())
                        {
                            if (firstRecord)
                            {
                                masterData.INVOICE_NUMBER = Convert.ToString(reader["VOUCHER_NO"]);
                                masterData.DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                                masterData.PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]);
                                masterData.REFERENCENO = Convert.ToString(reader["REF"]);
                                masterData.SUP_INVNO = reader["SUP_INVNO"] == DBNull.Value ? "" : Convert.ToString(reader["SUP_INVNO"]);
                                masterData.CLIENT_PO = Convert.ToString(reader["CLIENT_PO"]);
                                masterData.BRANCH_ADDRESS = Convert.ToString(reader["B_ADDRESS"]);
                                masterData.BRANCH_PHONE = Convert.ToString(reader["B_TEL"]);
                                masterData.CURR = Convert.ToString(reader["CURR"]);
                                masterData.CURR_SIG = Convert.ToString(reader["CURR_SIG"]);
                                masterData.EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]);

                                firstRecord = false;
                            }

                            DataRow dataRow = inspectionServiceChargesDetails.NewRow();
                            //dataRow["Desc"] = Convert.ToString(reader["DT_DESC"]);
                            dataRow["Item"] = Convert.ToString(reader["ITEM_NAME"]);
                            dataRow["JobNo"] = Convert.ToString(reader["JOB_NO"]);
                            dataRow["Buyer"] = Convert.ToString(reader["BUYER_NAME"]);
                            dataRow["ClientPO"] = reader["CLIENT_PO"] == DBNull.Value ? "" : Convert.ToString(reader["CLIENT_PO"]);
                            dataRow["Qty"] = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]);
                            dataRow["Amount"] = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);
                            dataRow["Comm"] = reader["COMMISSION"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["COMMISSION"]);
                            dataRow["CommAmt"] = reader["COMMISSION_AMOUNT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["COMMISSION_AMOUNT"]);
                            inspectionServiceChargesDetails.Rows.Add(dataRow);

                        }
                        reader.Close();
                    }

                    reportData.Master = masterData;
                    if (menuDetails.MD_ID == 54)
                    {
                        reportData.Detail = inspectionServiceChargesDetails;
                    }
                    response.data = reportData;
                    response.msg = "";
                    response.msgType = 1;
                }
                else
                {
                    if (mdId == 0)
                    {
                        response.data = null;
                        response.msg = "Update record with Report";
                        response.msgType = 2;
                    }
                    else
                    {
                        response.data = null;
                        response.msg = "Select valid Report";
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
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }
    }
}