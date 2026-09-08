using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;
using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.Metrics;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Transactions;
using System.Xml.Linq;
using ZXing;
using ZXing.OneD;
using ZXing.QrCode.Internal;
using static System.Net.Mime.MediaTypeNames;
using Table = iText.Layout.Element.Table;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class POSTransactionRepository : IPOSTransactionRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public ICommonService _commonService { get; set; }
        public IPartyRepository _partyRepository { get; set; }
        public IPeriodRepository _periodRepository { get; set; }
        public POSTransactionRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, IPartyRepository partyRepository, ICommonService commonService, IPeriodRepository periodRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _partyRepository = partyRepository;
            _commonService = commonService;
            _periodRepository = periodRepository;
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
                        string query = $@"SELECT A.TRAN_ID , A.V_DATE, A.VOUCHER_NO, tc.CNAME, tc.CMOB, A.INV_STATUS, A.TOTAL, A.DISC, A.NET_TOTAL, A.BCODE, A.PERIOD_ID, A.ADD_USER_ID,
                                        A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, A.MENU_ID, A.DLT , A.DEL , A.DEL_CHARGES
                                        , A.DISC_AMT ,ps.GROUP_NAME As BILL_STATUS , A.BILL_STATUS As Status , A.CASH , A.CACT_CODE, A.BANK, A.BACT_CODE , A.PARTY , A.PARTY_CODE , A.ACT_CODE , A.RECV , tc.CADD  , A.COMPLETE 
                                        , w.GROUP_NAME As Waiter , t.GROUP_NAME As [Table] FROM {table} A 
                                        LEFT JOIN TBL_POS_STATUS ps On ps.GROUP_CODE = a.INV_STATUS
                                        LEFT JOIN TBL_POS_CUS tc On tc.TRAN_ID = a.TRAN_ID AND tc.BCODE = a.BCODE And tc.PERIOD_ID = a.PERIOD_ID
                                        Left Outer join TBL_WAITER w on w.GROUP_CODE = a.WAITER
                                        Left Outer Join TBL_TABLE t on t.GROUP_CODE = a.[TABLE]
                                        WHERE A.DLT = 'T' AND A.BILL_STATUS IN('K','H') AND A.BCODE = {common.Branch} AND A.PERIOD_ID = {common.Period} ORDER BY A.TRAN_ID DESC";


                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                CNAME = Convert.ToString(reader["CNAME"]),
                                CMOB = Convert.ToString(reader["CMOB"]),
                                INV_STATUS = Convert.ToString(reader["INV_STATUS"]),
                                TOTAL = Convert.ToString(reader["TOTAL"]),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_AMT = Convert.ToString(reader["DISC_AMT"]),
                                NET_TOTAL = Convert.ToString(reader["NET_TOTAL"]),
                                CASH = Convert.ToString(reader["CASH"]),
                                RECV = Convert.ToString(reader["RECV"]),
                                BCODE = Convert.ToString(reader["BCODE"]),
                                BILL_STATUS = reader["BILL_STATUS"],
                                Status = reader["Status"],
                                WAITER = reader["WAITER"],
                                TABLE = reader["TABLE"],
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
        public MyHttpResponseMessage GetPendingRecords(DateTime FromDate, DateTime ToDate, Common common)
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

                if (true)
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"SELECT A.TRAN_ID , A.V_DATE, A.VOUCHER_NO, CASE WHEN A.PARTY_CODE = 0 THEN
                                        tc.CNAME
                                        ELSE PT.PARTY_NAME END AS CNAME, A.TOTAL, A.DISC, A.NET_TOTAL, A.BCODE, A.PERIOD_ID, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME,
                                        A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, A.MENU_ID, A.DLT , A.DEL , A.DEL_CHARGES
                                        , A.DISC_AMT,ps.GROUP_NAME As BILL_STATUS , A.CASH , A.CACT_CODE, A.BANK, A.BACT_CODE , A.PARTY , A.PARTY_CODE , A.ACT_CODE ,
                                            CASE 
                                            WHEN A.RECV = 0 AND ADVANCE = 0 THEN ADV_BANK
                                            WHEN A.RECV = 0 AND ADV_BANK = 0 THEN ADVANCE
                                            WHEN A.RECV = 0 THEN 
                                                CASE 
                                                    WHEN ADVANCE <> 0 THEN ADVANCE
                                                    ELSE ADV_BANK 
                                                END
                                            ELSE A.RECV
                                        END AS RECV, tc.CADD  , A.COMPLETE 
                                        , w.GROUP_NAME As Waiter , t.GROUP_NAME As [Table] ,
                                        CASE WHEN A.CACT_CODE <> 0 AND A.BACT_CODE = 0 AND A.PARTY_CODE = 0 THEN 
                                        'Cash' 
                                        WHEN A.BACT_CODE <> 0 AND A.CACT_CODE = 0 AND A.PARTY_CODE = 0 THEN
                                        'Bank'
                                        WHEN A.PARTY_CODE <> 0 AND A.CACT_CODE = 0 AND A.BACT_CODE = 0  THEN
                                        BILL_MODE
                                        ELSE
                                        'Split'
                                        END AS BILL_MODE
                                        ,CASE WHEN A.CLOSING = 0 THEN 'Opened' ELSE 'Closed' END As CLOSING
                                        ,A.SRB_INV
                                        FROM TBL_POS_MASTER A 
                                        LEFT JOIN TBL_POS_STATUS ps On ps.GROUP_CODE = a.INV_STATUS
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = A.PARTY_CODE AND PT.ACT_CODE = A.ACT_CODE
                                        LEFT JOIN TBL_POS_CUS tc On tc.TRAN_ID = a.TRAN_ID AND tc.BCODE = a.BCODE AND tc.PERIOD_ID = a.PERIOD_ID
                                        Left Outer join TBL_WAITER w on w.GROUP_CODE = a.WAITER
                                        Left Outer Join TBL_TABLE t on t.GROUP_CODE = a.[TABLE] 
                                        WHERE A.DLT = 'T' AND A.BILL_STATUS = 'P' AND A.BCODE = {common.Branch} AND A.PERIOD_ID = {common.Period} 
                                        and (SRB_INV  LIKE '%[^0-9]%'  
                                        OR SRB_INV IS NULL           
                                        OR LTRIM(RTRIM(SRB_INV)) = '' )
										AND A.TOTAL != 0
                                        AND V_DATE BETWEEN '{FromDate.ToString("yyyy-MM-dd")}' AND '{ToDate.ToString("yyyy-MM-dd")}' ORDER BY A.TRAN_ID DESC";


                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                CNAME = Convert.ToString(reader["CNAME"]),
                                //INV_STATUS = Convert.ToString(reader["INV_STATUS"]),
                                TOTAL = Convert.ToString(reader["TOTAL"]),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_AMT = Convert.ToString(reader["DISC_AMT"]),
                                NET_TOTAL = Convert.ToString(reader["NET_TOTAL"]),
                                CASH = Convert.ToString(reader["CASH"]),
                                COMPLETE = Convert.ToString(reader["COMPLETE"]),
                                RECV = Convert.ToString(reader["RECV"]),
                                BCODE = Convert.ToString(reader["BCODE"]),
                                BILL_STATUS = Convert.ToString(reader["BILL_STATUS"]),
                                WAITER = Convert.ToString(reader["WAITER"]),
                                TABLE = Convert.ToString(reader["TABLE"]),
                                BILLMODE = Convert.ToString(reader["BILL_MODE"]),
                                CLOSING = Convert.ToString(reader["CLOSING"]),
                                SRBINV = Convert.ToString(reader["SRB_INV"]),
                                ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
                                ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"])
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
        public MyHttpResponseMessage GetPayQuickSearch(Common common)
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
                        string query = $@"SELECT A.TRAN_ID , A.V_DATE, A.VOUCHER_NO, CASE WHEN A.PARTY_CODE = 0 THEN
                                        tc.CNAME
                                        ELSE PT.PARTY_NAME END AS CNAME,
                                        CASE WHEN A.PARTY_CODE = 0 THEN
                                        tc.CMOB
                                        ELSE PT.CELL END AS CMOB , A.INV_STATUS, A.TOTAL, A.DISC, A.NET_TOTAL, A.BCODE, A.PERIOD_ID, A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME,
                                        A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, A.MENU_ID, A.DLT , A.DEL , A.DEL_CHARGES
                                        , A.DISC_AMT,ps.GROUP_NAME As BILL_STATUS , A.CASH , A.CACT_CODE, A.BANK, A.BACT_CODE , A.PARTY , A.PARTY_CODE , A.ACT_CODE ,
                                         CASE 
                                        WHEN A.RECV = 0 THEN ADVANCE + ADV_BANK
                                        ELSE A.RECV
                                    END AS RECV, tc.CADD  , A.COMPLETE 
                                    , w.GROUP_NAME As Waiter , t.GROUP_NAME As [Table] , CASE WHEN PAY_TYPE = 'PARTY' THEN A.BILL_MODE ELSE PAY_TYPE END AS BILL_MODE
                                    ,CASE WHEN A.CLOSING = 0 THEN 'Opened' ELSE 'Closed' END As CLOSING,A.REMARKS 
                                        FROM {table} A 
                                        LEFT JOIN TBL_POS_STATUS ps On ps.GROUP_CODE = a.INV_STATUS
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = A.PARTY_CODE AND PT.ACT_CODE = A.ACT_CODE
                                        LEFT JOIN TBL_POS_CUS tc On tc.TRAN_ID = a.TRAN_ID AND tc.BCODE = a.BCODE AND tc.PERIOD_ID = a.PERIOD_ID
                                        Left Outer join TBL_WAITER w on w.GROUP_CODE = a.WAITER
                                        Left Outer Join TBL_TABLE t on t.GROUP_CODE = a.[TABLE] 
                                        WHERE A.DLT = 'T' AND A.BILL_STATUS = 'P' AND A.BCODE = {common.Branch} AND A.PERIOD_ID = {common.Period} ORDER BY A.TRAN_ID DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                CNAME = Convert.ToString(reader["CNAME"]),
                                CMOB = Convert.ToString(reader["CMOB"]),
                                INV_STATUS = Convert.ToString(reader["INV_STATUS"]),
                                TOTAL = Convert.ToString(reader["TOTAL"]),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_AMT = Convert.ToString(reader["DISC_AMT"]),
                                NET_TOTAL = Convert.ToString(reader["NET_TOTAL"]),
                                CASH = Convert.ToString(reader["CASH"]),
                                COMPLETE = Convert.ToString(reader["COMPLETE"]),
                                RECV = Convert.ToString(reader["RECV"]),
                                BCODE = Convert.ToString(reader["BCODE"]),
                                BILL_STATUS = Convert.ToString(reader["BILL_STATUS"]),
                                WAITER = Convert.ToString(reader["WAITER"]),
                                TABLE = Convert.ToString(reader["TABLE"]),
                                BILLMODE = Convert.ToString(reader["BILL_MODE"]),
                                CLOSING = Convert.ToString(reader["CLOSING"]),
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
        public MyHttpResponseMessage AdvanceBookingRecords(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            List<dynamic> jsonDetailDataResult = new List<dynamic>();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty, b_i = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    b_i = menu.B_I;
                }

                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = @$"SELECT PM.TRAN_ID, DEL_DATE AS V_DATE, VOUCHER_NO, TC.CMOB, TC.CNAME, TC.CADD, NET_TOTAL, DEL_CHARGES, ADVANCE, ADV_BANK, (NET_TOTAL - ADVANCE) AS REMAINING, PAY_TYPE, PM.TRAN_ID, PM.BCODE, PM.PERIOD_ID , CASH_TAX 
                                        FROM TBL_POS_MASTER PM
										LEFT JOIN TBL_POS_CUS TC ON TC.TRAN_ID = PM.TRAN_ID AND TC.BCODE = PM.BCODE AND TC.PERIOD_ID = PM.PERIOD_ID
                                        WHERE 
                                        PM.DLT = 'T' AND
                                        PM.BILL_STATUS = 'P' AND
                                        PM.INV_STATUS = 3 AND                   
                                        COMPLETE = 0 AND 
                                        PM.PAY_TYPE = 'Advance'
                                        AND TC.CMOB != ''
                                        ";

                    string detailQuerry = @$"select IM.ITEM_NAME, D.QTY, D.TRAN_ID from TBL_POS_DETAIL D
                                                                LEFT OUTER JOIN TBL_BARCODE BG
                                                                ON BG.CODE = D.ITEM_CODE
                                                                LEFT OUTER JOIN TBL_ITEMSMASTER IM
                                                                ON IM.ITEM_CODE =
                                                                CASE WHEN '{b_i}' = 'B' THEN BG.ITEM_CODE
                                                                WHEN '{b_i}' = 'I' THEN D.ITEM_CODE END
                                                                WHERE D.DLT = 'T'";
                    SqlCommand detailCommand = new SqlCommand(detailQuerry, connection);
                    connection.Open();
                    SqlDataReader detailReader = detailCommand.ExecuteReader();
                    while (detailReader.Read())
                    {
                        int tranId = detailReader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(detailReader["TRAN_ID"]);
                        int qty = detailReader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(detailReader["QTY"]);
                        string itemName = Convert.ToString(detailReader["ITEM_NAME"]);

                        jsonDetailDataResult.Add(new { tranId = tranId, qty = qty, itemName = itemName });
                    }
                    connection.Close();

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var advCash = reader["ADVANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ADVANCE"]);
                        var advBank = reader["ADV_BANK"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ADV_BANK"]);
                        var NetAmt = reader["NET_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_TOTAL"]);
                        var Tax = reader["CASH_TAX"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CASH_TAX"]);
                        var NetAmtWithTax = Math.Round(NetAmt + (NetAmt * Tax) / 100, 0);
                        var row = new CustomSalesInvoiceReport
                        {
                            TRAN_ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                            VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                            VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                            Mobile = reader["CMOB"] == DBNull.Value ? "" : Convert.ToString(reader["CMOB"]),
                            CName = reader["CNAME"] == DBNull.Value ? "" : Convert.ToString(reader["CNAME"]),
                            CAdd = reader["CADD"] == DBNull.Value ? "" : Convert.ToString(reader["CADD"]),
                            NetAmt = NetAmtWithTax,
                            Advance = advCash + advBank,
                            DeliveryCharges = reader["DEL_CHARGES"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEL_CHARGES"]),
                            Total = NetAmtWithTax - (advCash + advBank),
                            Mode = reader["PAY_TYPE"] == DBNull.Value ? "" : Convert.ToString(reader["PAY_TYPE"]),
                            detail = jsonDetailDataResult.Where(x => x.tranId == Convert.ToInt32(reader["TRAN_ID"])).ToList()
                        };
                        jsonDataResult.Add(row);
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
        public MyHttpResponseMessage GetCustomerHistory(Common common, string CstNumber)
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

                //string nextId = "";
                //string maxIdQuery = "SELECT B_I FROM TBL_MENU_BUILDER WHERE DLT = 'T' AND ID = '" + common.MenuID + "'";
                //using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                //{
                //    SqlCommand command = new SqlCommand(maxIdQuery, connection);
                //    connection.Open();
                //    object result = command.ExecuteScalar();
                //    nextId = Convert.ToString(result);
                //}

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"SELECT A.TRAN_ID , A.V_DATE, A.VOUCHER_NO,
                                        CASE WHEN A.PARTY_CODE = 0 THEN
                                        tc.CNAME
                                        ELSE PT.PARTY_NAME END AS CNAME,
                                        CASE WHEN A.PARTY_CODE = 0 THEN
                                        tc.CMOB
                                        ELSE PT.CELL END AS CMOB,
                                        CASE WHEN A.PARTY_CODE = 0 THEN
                                        tc.CADD
                                        ELSE PT.PADDRESS END AS CADD,
                                        a.RECV
                                        FROM {table} A 
                                        LEFT JOIN TBL_POS_STATUS ps On ps.GROUP_CODE = a.INV_STATUS
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = A.PARTY_CODE AND PT.ACT_CODE = A.ACT_CODE
                                        LEFT JOIN TBL_POS_CUS tc On tc.TRAN_ID = a.TRAN_ID AND tc.BCODE = a.BCODE AND tc.PERIOD_ID = a.PERIOD_ID
                                        Left Outer join TBL_WAITER w on w.GROUP_CODE = a.WAITER
                                        Left Outer Join TBL_TABLE t on t.GROUP_CODE = a.[TABLE] 
                                        WHERE A.DLT = 'T' 
                                        AND A.BILL_STATUS = 'P' 
                                        AND (PT.CELL LIKE '%' + '{CstNumber}' + '%'
                                        OR TC.CMOB LIKE '%' + '{CstNumber}' + '%')
                                        ORDER BY A.V_DATE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                CNAME = Convert.ToString(reader["CNAME"]),
                                CMOB = Convert.ToString(reader["CMOB"]),
                                CADD = Convert.ToString(reader["CADD"]),
                                RECV = Convert.ToDouble(reader["RECV"]),
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
        public MyHttpResponseMessage GetMapData(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = "TBL_POS_MAP";
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $"SELECT * FROM {table} WHERE ASTATUS = 'Y' AND DLT = 'T' AND BCODE = {common.Branch}";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                CASH_ACT = Convert.ToInt32(reader["CASH_ACT"]),
                                CASH_TAX = Convert.ToInt32(reader["CASH_TAX"]),
                                BANK_ACT = Convert.ToInt32(reader["BANK_ACT"]),
                                PARTY_TAX = Convert.ToInt32(reader["PARTY_TAX"]),
                                BANK_TAX = Convert.ToInt32(reader["BANK_TAX"]),
                                BCHARGES = Convert.ToDecimal(reader["BCHARGES"]),
                                TAX_STATUS = Convert.ToString(reader["TAX_STATUS"]),
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
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                MENU_ID = Convert.ToString(reader["MENU_ID"]),
                                DLT = Convert.ToString(reader["DLT"]),
                                RCASH = Convert.ToString(reader["R_CASH"]),
                                RCARD = Convert.ToString(reader["R_CARD"]),
                                RPARTY = Convert.ToString(reader["R_PARTY"]),
                                RSPLIT = Convert.ToString(reader["R_SPLIT"]),
                                RADVANCE = Convert.ToString(reader["R_ADVANCE"]),
                                SRBNAME = Convert.ToString(reader["SRB_NAME"]),
                                SRBNTN = Convert.ToString(reader["SRB_NTN"]),
                                POSUSER = Convert.ToString(reader["POS_USER"]),
                                POSPASS = Convert.ToString(reader["POS_PASS"]),
                                SRBID = Convert.ToString(reader["SRB_ID"]),
                                SRBSTATUS = Convert.ToString(reader["SRB_STATUS"]),
                                SRBURL = Convert.ToString(reader["SRB_URL"]),
                                GROUPIMG = Convert.ToString(reader["GROUP_IMAGE"]),
                                ITEMIMG = Convert.ToString(reader["ITEM_IMAGE"]),
                                WAITERIMG = Convert.ToString(reader["WAITER_IMAGE"]),
                                TABLEIMG = Convert.ToString(reader["TABLE_IMAGE"]),
                                ADVANCEBTN = Convert.ToString(reader["ADVANCE_BTN"]),
                                KOTBTN = Convert.ToString(reader["KOT_BTN"]),
                                SALESMANREQ = Convert.ToString(reader["SALESMAN_REQ"]),
                                WHATSAPPURL = Convert.ToString(reader["WHATSAPP_URL"]),
                                WHATSAPPTOKEN = Convert.ToString(reader["WHATSAPP_TOKEN"]),
                                WHATSAPPMSG = Convert.ToString(reader["WHATSAPP_MSG"]),
                                WHTMSG_ADV = Convert.ToString(reader["WHT_MSG_ADV"]),
                                WHTMSG_RETURN = Convert.ToString(reader["WHT_MSG_RETURN"]),
                                WHATSAPPCCMSG = Convert.ToString(reader["WHATSAPP_CC_MSG"]),
                                WHT_ADVCOM = Convert.ToString(reader["WHT_ADV_COM"]),
                                WHT_PARTYMSG = Convert.ToString(reader["WHT_PARTY_MSG"]),
                                //RATE = (reader["RATE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["RATE"]),
                                SER_CHARGES = (reader["SER_CHARGES"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["SER_CHARGES"]),
                                PWINDOW = (reader["P_WINDOW"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["P_WINDOW"]),
                                PAY_COPIES = (reader["PAY_COPIES"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["PAY_COPIES"]),
                                PRE_COPIES = (reader["PRE_COPIES"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["PRE_COPIES"]),
                                DUP_COPIES = (reader["DUP_COPIES"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["DUP_COPIES"]),
                                PAY_MODAL = (reader["PAY_MODAL"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["PAY_MODAL"]),
                                DUP_MODAL = (reader["DUP_MODAL"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["DUP_MODAL"]),
                                PRE_MODAL = (reader["PRE_MODAL"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["PRE_MODAL"]),


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
        public MyHttpResponseMessage GetSyncData(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = "TBL_POS_MASTER";
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "EXEC [dbo].[SP_SYNC_ALL_TABLES]";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.CommandTimeout = 60;
                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                    response.data = "";
                    response.msg = "Data Sync Successfully";
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
        public MyHttpResponseMessage SRBApi_Status(string voucherno, string invoiceId, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string query = string.Empty;
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
                        if (invoiceId == "")
                        {
                            query = $"Update {table} set API_STATUS = 'Y' , SRB_INV = 'Unknown Error' , SRB_VN = '' WHERE VOUCHER_NO = '{voucherno}' AND DLT = 'T' AND BCODE = {common.Branch}";
                        }
                        else
                        {
                            query = $"Update {table} set API_STATUS = 'Y' , SRB_INV = '{invoiceId}' WHERE VOUCHER_NO = '{voucherno}' AND DLT = 'T' AND BCODE = {common.Branch}";
                        }

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        command.ExecuteNonQuery();
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
        private decimal GenerateNextId(Common common, SqlCommand command, bool Expense = false)
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

                if (!String.IsNullOrWhiteSpace(table) && !Expense)
                {
                    string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM {table} WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                    command.CommandText = maxIdQuery;
                    object result = command.ExecuteScalar();
                    return Convert.ToInt64(result);
                }
                else
                {
                    string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM TBL_POS_EXP WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                    command.CommandText = maxIdQuery;
                    object result = command.ExecuteScalar();
                    return Convert.ToInt64(result);
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }
        private string GenerateVoucherNo(Common common, decimal codes, SqlCommand command, string vDate, bool Expense = false)
        {
            try
            {
                if (!Expense)
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

                    var code = GetClosingRecord(common, command);

                    if (!String.IsNullOrWhiteSpace(shortName) && !String.IsNullOrWhiteSpace(prefix) && voucherLength > 0)
                    {
                        if (code == 0)
                        {
                            code = 1;
                        }
                        else
                        {
                            code++;
                        }
                        //string paddedVoucherValue = "0".ToString().PadLeft(voucherLength - 1, '0') + code;
                        string paddedVoucherValue = code.ToString().PadLeft(voucherLength, '0');
                        return $"{prefix}/{paddedVoucherValue}";
                    }
                }
                else
                {
                    var code = GetClosingRecord(common, command, Expense);
                    if (code == 0)
                    {
                        code = 1;
                    }
                    else
                    {
                        code++;
                    }
                    return $"00{code}";
                }

            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }
        private string GenerateSRBVoucherNo(Common common, SqlCommand command, string vDate, bool Expense = false)
        {
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? prefix = string.Empty, shortName = string.Empty, table = string.Empty;
                int voucherLength = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    voucherLength = Convert.ToInt32(menu.VOUCHER_LEN);
                    prefix = menu.PERFIX;
                    table = menu.TABLE1;
                }

                string maxIdQuery = $"SELECT COUNT(SRB_VN) FROM {table} WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}' AND SRB_VN != ''";
                command.CommandText = maxIdQuery;
                object result = command.ExecuteScalar();
                var code = Convert.ToInt32(result);

                var branchData = _branchRepository.GetBranchByCode(common.Branch);
                if (branchData.data != null)
                {
                    var branch = (Branch)branchData.data;
                    shortName = branch.B_SHORT_NAME;
                }

                if (!String.IsNullOrWhiteSpace(shortName) && !String.IsNullOrWhiteSpace(prefix) && voucherLength > 0)
                {
                    if (code == 0)
                    {
                        code = 1;
                    }
                    else
                    {
                        code++;
                    }
                    //string paddedVoucherValue = "0".ToString().PadLeft(voucherLength - 1, '0') + code;
                    string paddedVoucherValue = code.ToString().PadLeft(voucherLength, '0');
                    return $"{prefix}/{paddedVoucherValue}";
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }
        private int GetClosingRecord(Common common, SqlCommand command, bool Expense = false)
        {
            if (!Expense)
            {
                string countQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) FROM TBL_POS_MASTER WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                command.CommandText = countQuery;
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);

            }
            else
            {
                string countQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM TBL_POS_EXP WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                command.CommandText = countQuery;
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
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
        public MyHttpResponseMessage UpdateTablesAndWaiter(CustomPOSTransaction modelRecord, Common common)
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
                        string query = $"Update {table} set [TABLE] = '{modelRecord.Master.TABLE}' , WAITER = '{modelRecord.Master.WAITER}' WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' ";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        command.ExecuteNonQuery();

                    }

                    response.data = jsonDataResult;
                    response.msg = "Table and waiter updated successfully.";
                    response.msgType = 1;
                }
                else
                {
                    response.data = "";
                    response.msg = "Something went wrong! Table or waiter not updated. Please try again later.";
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
        public MyHttpResponseMessage Save(CustomPOSTransaction modelRecord, CustomMenuDetail menuDetails, Common common, Company currentCompany, Branch currentBranch, Info currentLabel)
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
                    var ip = common.IPAddress;
                    var computer = common.ComputerName;
                    var postal = common.PostalCode;
                    var username = common.Username;
                    var branch = common.Branch;
                    var period = common.Period;
                    var menuID = common.MenuID;
                    int? pickId = 0;
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
                                    currentItems.Add(item.ITEM_CODE, item.QTY);
                                }
                                else
                                {
                                    currentItems[item.ITEM_CODE] += item.QTY;
                                }
                                if (!stockBalance.ContainsKey(item.ITEM_CODE))
                                {
                                    stockBalance.Add(item.ITEM_CODE, GetAvailableStock(item.ITEM_CODE, period, common));
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
                                string query = "", detailQuery = "", CustomerInfoQuery = "", voucherNo = string.Empty, srbVoucherNo = string.Empty;
                                bool IsMasterAdded = true, IsNew = false;
                                decimal code = 0;
                                //string formattedDate = DateTime.ParseExact(modelRecord.Master.V_DATE, "d-M-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                                string formattedDate = CommonService.GetDate("Pakistan Standard Time");
                                string DelDate = modelRecord.Master.DEL_DATE == null ? formattedDate : modelRecord.Master.DEL_DATE;
                                decimal itemDiscount = modelRecord.Detail?.Sum(i => (decimal?)i.DISC_AMT) ?? 0;
                                var Net_Total = Convert.ToDecimal(modelRecord.Master.NET_TOTAL) - Math.Round(Convert.ToDecimal(itemDiscount));
                                string DueDate = modelRecord.Master.DUE_DATE != null
                                                    ? Convert.ToDateTime(modelRecord.Master.DUE_DATE).ToString("yyyy-MM-dd")
                                                    : "";
                                if (modelRecord.Master.TRAN_ID == null || modelRecord.Master.TRAN_ID == 0 || modelRecord.Master.Return == true)
                                {
                                    IsNew = true;
                                    code = GenerateNextId(common, command);

                                    if (code > 0)
                                    {
                                        modelRecord.Master.TRAN_ID = code;
                                        voucherNo = GenerateVoucherNo(common, code, command, CommonService.GetDateTime("Pakistan Standard Time"));
                                        if(modelRecord.Master.BILL_STATUS == "P")
                                            srbVoucherNo = modelRecord.Master.SRBSTATUS == "Y" ? GenerateSRBVoucherNo(common, command, CommonService.GetDateTime("Pakistan Standard Time")) : "";
                                        if (String.IsNullOrWhiteSpace(voucherNo))
                                        {
                                            IsMasterAdded = false;
                                        }
                                    }
                                    else
                                    {
                                        IsMasterAdded = false;
                                    }
                                    CustomerInfoQuery = $@"INSERT INTO TBL_POS_CUS (TRAN_ID , CNAME , CMOB , CADD ,BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID,
                                            EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT) VALUES ('{code}' , '{modelRecord.Master.CNAME}' , '{modelRecord.Master.CMOB}' , '{modelRecord.Master.CADD}' , '{branch}', '{period}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                            '{computer}', '{ip}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                            '{computer}', '{ip}', '{postal}', '{postal}', '{menuID}', 'T')";

                                    command.CommandText = CustomerInfoQuery;
                                    command.ExecuteNonQuery();

                                    query = $"INSERT INTO {table} " +
                                            "(TRAN_ID, V_DATE, SRB_VN ,VOUCHER_NO, INV_STATUS, TOTAL, DISC, NET_TOTAL, " +
                                            "BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                            "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT, DEL, " +
                                            "DEL_CHARGES, DISC_AMT, BILL_STATUS, CASH, CACT_CODE, BANK, BACT_CODE, PARTY, PARTY_CODE, " +
                                            "ACT_CODE, RECV , BCHARGES , CLOSING , CASH_TAX , BANK_TAX , PARTY_TAX , SALESMAN , SACT_CODE, COMM, DUE_DATE, REMARKS, WAITER, [TABLE], BILL_MODE, SRB_INV ," +
                                            "ADVANCE , DEL_DATE , COMPLETE , ADV_BANK , CARD_NO , ADV_BOOK_TYPE , PAY_TYPE,SER_CHARGES,SETTLEMENT,SETTL_SIGN)" +
                                            "VALUES " +
                                            $"('{code}', '{formattedDate}', '{srbVoucherNo}' ,'{voucherNo}', " +
                                            $"'{modelRecord.Master.INV_STATUS}', '{modelRecord.Master.TOTAL}', '{modelRecord.Master.DISC}', '{Net_Total}', " +
                                            $"'{branch}', '{period}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                            $"'{computer}', '{ip}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                            $"'{computer}', '{ip}', '{postal}', '{postal}', '{menuID}', 'T', '{modelRecord.Master.DEL}', " +
                                            $"'{modelRecord.Master.DEL_CHARGES}', '{modelRecord.Master.DISC_AMT}', " +
                                            $"'{modelRecord.Master.BILL_STATUS}', '{modelRecord.Master.CASH}', '{modelRecord.Master.CACT_CODE}', '{modelRecord.Master.BANK}', " +
                                            $"'{modelRecord.Master.BACT_CODE}', '{modelRecord.Master.PARTY}', '{modelRecord.Master.PARTY_CODE}', '{modelRecord.Master.ACT_CODE}', " +
                                            $"'{modelRecord.Master.RECV}' , '{modelRecord.Master.BCHARGES}' , '0' , '{modelRecord.Master.CASH_TAX}' , '{modelRecord.Master.BANK_TAX}' , '{modelRecord.Master.PARTY_TAX}' , '{modelRecord.Master.SALESMAN}', " +
                                            $"'{modelRecord.Master.SACT_CODE}' , '{modelRecord.Master.COMMISION}' , '{DueDate}' , '{modelRecord.Master.REMARK}' , '{modelRecord.Master.WAITER}' , '{modelRecord.Master.TABLE}' , '{modelRecord.Master.BILLMODE}' , '{modelRecord.Master.SRBInvoiceId}' ," +
                                            $" '{modelRecord.Master.ADVANCE}' , '{DelDate}' , '{modelRecord.Master.COMPLETE}' , '{modelRecord.Master.ADV_BANK}' , '{modelRecord.Master.CARD_NO}' , '{modelRecord.Master.ADV_BOOK_TYPE}' , '{modelRecord.Master.PAY_TYPE}', '{modelRecord.Master.SER_CHARGES}','{modelRecord.Master.SETT}','{modelRecord.Master.SETTL_SIGN}')";

                                    command.CommandText = query;
                                    command.ExecuteNonQuery();


                                    if (modelRecord.Master.CARD_NO != "" && modelRecord.Master.CARD_NO != null)
                                    {
                                        double totalPoints = 0;
                                        double tenPercentValue = 0;
                                        if (modelRecord.Master.CardDiscValue != 0)
                                        {
                                            totalPoints = (modelRecord.Master.PREV_POINTS ?? 0) + (modelRecord.Master.CURR_POINTS ?? 0);

                                            if (modelRecord.Master.CardDiscValue > modelRecord.Master.DISC_AMT)
                                            {
                                                var difference = Convert.ToInt32(modelRecord.Master.CardDiscValue) - Convert.ToInt32(modelRecord.Master.DISC_AMT);
                                                totalPoints = difference;
                                            }
                                            else if (modelRecord.Master.CardDiscValue == totalPoints)
                                            {
                                                totalPoints = 0;
                                            }
                                        }
                                        else
                                        {
                                            totalPoints = (modelRecord.Master.PREV_POINTS ?? 0) + (modelRecord.Master.CURR_POINTS ?? 0);
                                        }
                                        string updatePointRateQuery = $@"
                                                                UPDATE TBL_POS_MS 
                                                                SET POINT_RATE = '{totalPoints}'
                                                                WHERE CARD_NO = '{modelRecord.Master.CARD_NO}'";

                                        command.CommandText = updatePointRateQuery;
                                        command.ExecuteNonQuery();
                                    }

                                }
                                else
                                {
                                    var isUpdate = 0;
                                    srbVoucherNo = modelRecord.Master.SRB_VN;
                                    if((srbVoucherNo == "" || srbVoucherNo == null) && modelRecord.Master.BILL_STATUS == "P")
                                        srbVoucherNo = modelRecord.Master.SRBSTATUS == "Y" ? GenerateSRBVoucherNo(common, command, CommonService.GetDateTime("Pakistan Standard Time")) : "";

                                    if (modelRecord.Master.Return == false)
                                    {
                                        CustomerInfoQuery = $@"SELECT * FROM TBL_POS_MASTER WhERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BILL_STATUS = 'P' AND DLT = 'T' AND COMPLETE = 1 AND BCODE = '{common.Branch}'";
                                        using (SqlConnection connections = new SqlConnection(new SQLService().getconnstring()))
                                        {
                                            SqlCommand commands = new SqlCommand(CustomerInfoQuery, connections);
                                            connections.Open();
                                            object result = commands.ExecuteScalar();
                                            isUpdate = Convert.ToInt32(result);
                                        }
                                    }

                                    if (isUpdate == 0)
                                    {
                                        if (modelRecord.Master.COMPLETE == 1)
                                            DelDate = modelRecord.Master.DEL_DATE == null ? formattedDate : DateTime.Now.ToString("yyyy-MM-dd");
                                        CustomerInfoQuery = $@"UPDATE TBL_POS_CUS SET CNAME = '{modelRecord.Master.CNAME}' , CMOB = '{modelRecord.Master.CMOB}' , CADD = '{modelRecord.Master.CADD}' WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                                        command.CommandText = CustomerInfoQuery;
                                        command.ExecuteNonQuery();

                                        var IsKotBill = 0;
                                        string maxIdQuery = $"SELECT Count(*) FROM {table} WITH (NOLOCK) WHERE DLT = 'T' AND TRAN_ID = {modelRecord.Master.TRAN_ID} AND BILL_STATUS = 'K'";
                                        using (SqlConnection connections = new SqlConnection(new SQLService().getconnstring()))
                                        {
                                            SqlCommand commands = new SqlCommand(maxIdQuery, connections);
                                            connections.Open();
                                            object result = commands.ExecuteScalar();
                                            IsKotBill = Convert.ToInt32(result);
                                        }

                                        voucherNo = modelRecord.Master.VOUCHER_NO;
                                        query = $"UPDATE {table} SET ";
                                        if (IsKotBill > 0)
                                            query += $"V_DATE = '{formattedDate}', ";
                                        query += $"INV_STATUS = '{modelRecord.Master.INV_STATUS}', " +
                                                $"TOTAL = '{modelRecord.Master.TOTAL}', " +
                                                $"SRB_VN = '{srbVoucherNo}', " +
                                                $"DISC = '{modelRecord.Master.DISC}', " +
                                                $"NET_TOTAL = '{Net_Total}', " +
                                                $"BCODE = '{branch}', " +
                                                $"PERIOD_ID = '{period}', " +
                                                $"EDIT_USER_ID = '{username}', " +
                                                $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                $"EDIT_COMPUTER_NAME = '{computer}', " +
                                                $"EDIT_IP_ADDRESS = '{ip}', " +
                                                $"EDIT_POSTALCODE = '{postal}', " +
                                                $"DEL = '{modelRecord.Master.DEL}', " +
                                                $"DEL_CHARGES = '{modelRecord.Master.DEL_CHARGES}', " +
                                                $"DISC_AMT = '{modelRecord.Master.DISC_AMT}', " +
                                                $"BILL_STATUS = '{modelRecord.Master.BILL_STATUS}', " +
                                                $"CASH = '{modelRecord.Master.CASH}', " +
                                                $"CACT_CODE = '{modelRecord.Master.CACT_CODE}', " +
                                                $"BANK = '{modelRecord.Master.BANK}', " +
                                                $"BACT_CODE = '{modelRecord.Master.BACT_CODE}', " +
                                                $"PARTY = '{modelRecord.Master.PARTY}', " +
                                                $"PARTY_CODE = '{modelRecord.Master.PARTY_CODE}', " +
                                                $"ACT_CODE = '{modelRecord.Master.ACT_CODE}', " +
                                                $"RECV = '{modelRecord.Master.RECV}', " +
                                                $"BCHARGES = '{modelRecord.Master.BCHARGES}', " +
                                                $"CASH_TAX = '{modelRecord.Master.CASH_TAX}', " +
                                                $"BANK_TAX = '{modelRecord.Master.BANK_TAX}', " +
                                                $"PARTY_TAX = '{modelRecord.Master.PARTY_TAX}', " +
                                                $"SALESMAN = '{modelRecord.Master.SALESMAN}', " +
                                                $"SACT_CODE = '{modelRecord.Master.SACT_CODE}', " +
                                                $"COMM = '{modelRecord.Master.COMMISION}', " +
                                                $"DUE_DATE = '{DueDate}', " +
                                                $"REMARKS = '{modelRecord.Master.REMARK}', " +
                                                $"WAITER = '{modelRecord.Master.WAITER}', " +
                                                $"[TABLE] = '{modelRecord.Master.TABLE}', " +
                                                $"BILL_MODE = '{modelRecord.Master.BILLMODE}', " +
                                                $"SRB_INV = '{modelRecord.Master.SRBInvoiceId}', " +
                                                $"ADVANCE = '{modelRecord.Master.ADVANCE}', " +
                                                $"ADV_BANK = '{modelRecord.Master.ADV_BANK}', " +
                                                $"DEL_DATE = '{DelDate}', " +
                                                $"COMPLETE = '{modelRecord.Master.COMPLETE}' ," +
                                                $"CARD_NO = '{modelRecord.Master.CARD_NO}' ," +
                                                $"PAY_TYPE = '{modelRecord.Master.PAY_TYPE}', " +
                                                $"SER_CHARGES = '{modelRecord.Master.SER_CHARGES}', " +
                                                $"SETTLEMENT = '{modelRecord.Master.SETT}', " +
                                                $"SETTL_SIGN = '{modelRecord.Master.SETTL_SIGN}' ";
                                        if (modelRecord.Master.RECV == 0)
                                        {
                                            query += $",ADV_BOOK_TYPE = '{modelRecord.Master.ADV_BOOK_TYPE}' ";
                                        }
                                        query += $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";

                                        command.CommandText = query;
                                        command.ExecuteNonQuery();

                                        if (modelRecord.Master.PAY_TYPE == "Advance")
                                        {
                                            var closingQuery = $@"UPDATE {table} SET CLOSING = 0 WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}' ";
                                            command.CommandText = closingQuery;
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        return new MyHttpResponseMessage()
                                        {
                                            data = "",
                                            SaveData = new SaveData
                                            {
                                                code = IsNew ? code : modelRecord.Master.TRAN_ID,
                                                voucherNo = IsNew ? voucherNo : modelRecord.Master.VOUCHER_NO,
                                                srbInvoiceDate = formattedDate,
                                                srbNoucherNo = modelRecord.Master.SRBSTATUS == "Y" ? srbVoucherNo : "",
                                                dt_codes = ""
                                            },
                                            msg = "This invoice has either been paid or marked as returned. Therefore, you cannot add items to it.",
                                            msgType = 2,
                                        };

                                    }

                                }

                                var isDetailAdded = false;
                                List<string> dt_code = new List<string>();

                                foreach (var item in modelRecord.Detail.ToList())
                                {
                                    try
                                    {
                                        if (item.DT_CODE == null || item.DT_CODE == 0)
                                        {
                                            int detailCode = GenerateNextDetailId(common, command);
                                            if (detailCode > 0)
                                            {
                                                if (item.RITEM == 1)
                                                {
                                                    pickId = item.PICK_ID;
                                                }

                                                if (modelRecord.Master.BILL_STATUS == "K")
                                                    dt_code.Add(Convert.ToString(detailCode));

                                                detailQuery = $"INSERT INTO {detailTable} " +
                                                              "(TRAN_ID, DT_CODE, ITEM_CODE, QTY, UNIT, RATE, AMT, DISC, NET_AMT, RITEM, COLOR,SIZE,TAX,TAX_AMT, " +
                                                              "BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                                              "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                                              "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                                              "ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT, DISC_AMT, PICK_ID , REMARKS) " +
                                                              "VALUES " +
                                                              "('" + modelRecord.Master.TRAN_ID + "', '" + detailCode + "', '" + item.ITEM_CODE + "', '" + item.QTY + "', " +
                                                              "'" + item.UNIT + "', '" + item.RATE + "', '" + item.AMT + "', '" + item.DISC + "', " +
                                                              "'" + item.NET_AMT + "', '" + item.RITEM + "','" + item.COLOR + "','" + item.SIZE + "','" + item.TAX + "','" + item.TAX_AMT + "', " +
                                                              "'" + branch + "', '" + period + "', '" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', " +
                                                              "'" + computer + "', '" + ip + "', '" + username + "', " +
                                                              "'" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + computer + "', '" + ip + "', " +
                                                              "'" + postal + "', '" + postal + "', '" + menuID + "', 'T', '" + item.DISC_AMT + "', '" + pickId + "' , '" + item.REMARKS + "')";
                                                command.CommandText = detailQuery;
                                                command.ExecuteNonQuery();

                                                isDetailAdded = true;
                                            }
                                            else
                                            {
                                                isDetailAdded = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            if (item.RITEM == 1)
                                            {
                                                pickId = item.PICK_ID;
                                            }

                                            detailQuery = $"UPDATE {detailTable} SET " +
                                                           $"QTY = '{item.QTY}', " +
                                                           $"UNIT = '{item.UNIT}', " +
                                                           $"RATE = '{item.RATE}', " +
                                                           $"AMT = '{item.AMT}', " +
                                                           $"DISC = '{item.DISC}', " +
                                                           $"NET_AMT = '{item.NET_AMT}', " +
                                                           $"RITEM = '{item.RITEM}', " +
                                                           $"SIZE = '{item.SIZE}', " +
                                                           $"COLOR = '{item.COLOR}', " +
                                                           $"EDIT_USER_ID = '{username}', " +
                                                           $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                           $"EDIT_COMPUTER_NAME = '{computer}', " +
                                                           $"EDIT_IP_ADDRESS = '{ip}', " +
                                                           $"EDIT_POSTALCODE = '{postal}', " +
                                                           $"DLT = 'T', " +
                                                           $"DISC_AMT = '{item.DISC_AMT}', " + // Added DISC_AMT update
                                                           $"PICK_ID = '{pickId}', " + // Added DISC_AMT update
                                                           $"REMARKS = '{item.REMARKS}' " + // Added DISC_AMT update
                                                                                            //$"ISKOTPRINT = '{item.ISKOTPRINT}' " + // Added DISC_AMT update
                                                           $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' " +
                                                           $"AND ITEM_CODE = '{item.ITEM_CODE}' " +
                                                           $"AND DT_CODE = '{item.DT_CODE}' " +
                                                           $"AND BCODE = '{branch}' " +
                                                           $"AND PERIOD_ID = '{period}'";
                                            //$"AND DT_CODE = '{item.DT_CODE}' " +
                                            command.CommandText = detailQuery;
                                            command.ExecuteNonQuery();

                                            isDetailAdded = true;
                                        }

                                    }
                                    catch (Exception)
                                    {
                                        isDetailAdded = false;
                                        break;
                                    }

                                }

                                if (IsMasterAdded && isDetailAdded)
                                {
                                    if (modelRecord.Master.BILL_STATUS == "H")
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
                                        transaction.Commit();

                                        string dt_codes = string.Join(",", dt_code);

                                        response.data = new
                                        {
                                            code = IsNew ? code : modelRecord.Master.TRAN_ID,
                                            voucherNo = IsNew ? voucherNo : modelRecord.Master.VOUCHER_NO,
                                            srbInvoiceDate = formattedDate,
                                            srbNoucherNo = modelRecord.Master.SRBSTATUS == "Y" ? srbVoucherNo : "",
                                        };
                                        response.SaveData = new SaveData
                                        {
                                            code = IsNew ? code : modelRecord.Master.TRAN_ID,
                                            voucherNo = IsNew ? voucherNo : modelRecord.Master.VOUCHER_NO,
                                            srbInvoiceDate = formattedDate,
                                            srbNoucherNo = modelRecord.Master.SRBSTATUS == "Y" ? srbVoucherNo : "",
                                            dt_codes = dt_codes
                                        };
                                        response.msgType = 1;
                                        response.msg = IsNew ? "Record Added Successfully" : "Record Updated Successfully";
                                    }

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
        public MyHttpResponseMessage GetPOSTransactionByCode(int code, string voucher, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            int tranId = 0;
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
                        string query = string.Empty;

                        //string query = "SELECT TRAN_ID , V_DATE, VOUCHER_NO, BOOK_TYPE, CNAME, CMOB, INV_STATUS, TOTAL, DISC, SETT, NET_TOTAL, CASH, CHANGE, RECV, BCODE, PERIOD_ID " +
                        //               $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                        //               $"AND PERIOD_ID = '{common.Period}'";
                        if (voucher != "undefined")
                        {
                            query = "SELECT a.TRAN_ID , V_DATE, VOUCHER_NO, SRB_VN, SALESMAN , SACT_CODE , COMM , tc.CNAME, tc.CMOB, tc.CADD , BILL_STATUS ,INV_STATUS, TOTAL, DISC, DISC_AMT , NET_TOTAL, CASH, BANK , PARTY , PARTY_CODE , a.BCODE, a.PERIOD_ID, DEL , DEL_CHARGES , ACT_CODE " +
                                ", REMARKS, WAITER, [TABLE], BILL_MODE, SRB_INV , ADVANCE , DEL_DATE , COMPLETE , ADV_BANK , CARD_NO , BILL_MODE , RECV , BACT_CODE , ADV_BOOK_TYPE , DUE_DATE , a.EDIT_DATE , a.EDIT_USER_ID , a.CASH_TAX , a.BANK_TAX , a.PARTY_TAX , a.PAY_TYPE , a.SER_CHARGES , a.SETTLEMENT , a.SETTL_SIGN " +
                                       $" FROM {table} a " +
                                       $"LEFT JOIN TBL_POS_CUS tc On tc.TRAN_ID = a.TRAN_ID AND tc.BCODE = '{common.Branch}' ANd tc.PERIOD_ID = '{common.Period}'" +
                                       $" WHERE a.DLT = 'T' " +
                                       $" AND a.BILL_STATUS = 'P' AND a.BCODE = '{common.Branch}' AND a.PERIOD_ID = '{common.Period}'";
                            if (voucher != "SRBForm")
                            {
                                query += $" AND a.VOUCHER_NO = '{voucher}'";
                            }
                            else
                            {
                                query += $" And a.TRAN_ID = '{code}'";
                            }
                        }
                        else
                        {
                            query = "SELECT a.TRAN_ID , V_DATE, VOUCHER_NO, SRB_VN, SALESMAN , SACT_CODE , COMM , tc.CNAME, tc.CMOB, tc.CADD , BILL_STATUS ,INV_STATUS, TOTAL, DISC, DISC_AMT , NET_TOTAL, CASH, BANK , PARTY , PARTY_CODE " +
                                ", a.BCODE, a.PERIOD_ID, DEL , DEL_CHARGES , ACT_CODE , REMARKS, WAITER, [TABLE], BILL_MODE, SRB_INV , ADVANCE , DEL_DATE , COMPLETE , ADV_BANK , CARD_NO , BILL_MODE , RECV , BACT_CODE , ADV_BOOK_TYPE , DUE_DATE , a.EDIT_DATE , a.EDIT_USER_ID  , a.CASH_TAX , a.BANK_TAX , a.PARTY_TAX , a.PAY_TYPE , a.SER_CHARGES ,a.SETTLEMENT , a.SETTL_SIGN " +
                                       $"FROM {table} a " +
                                       $"LEFT JOIN TBL_POS_CUS tc On tc.TRAN_ID = a.TRAN_ID AND tc.BCODE = '{common.Branch}' ANd tc.PERIOD_ID = '{common.Period}' " +
                                       $"WHERE a.DLT = 'T' AND a.TRAN_ID = '{code}' AND a.BCODE = '{common.Branch}' " +
                                       $"AND a.PERIOD_ID = '{common.Period}'";
                        }

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            tranId = Convert.ToInt32(reader["TRAN_ID"]);
                            var AddDate = Convert.ToDateTime(reader["EDIT_DATE"]).ToString("HH:mm:ss");
                            var Date = Convert.ToDateTime(reader["V_DATE"]).ToString($"dd-MM-yyyy {AddDate}");
                            var row = new
                            {
                                TRAN_ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                                V_DATE = Date,
                                MSG_DATE = reader["V_DATE"].ToString(),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                SRB_VN = Convert.ToString(reader["SRB_VN"]),
                                SALESMAN = reader["SALESMAN"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SALESMAN"]),
                                SACT_CODE = reader["SACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SACT_CODE"]),
                                BACT_CODE = reader["BACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BACT_CODE"]),
                                ADV_BOOKTYPE = reader["ADV_BOOK_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ADV_BOOK_TYPE"]),
                                COMMISION = reader["COMM"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMM"]),
                                CNAME = Convert.ToString(reader["CNAME"]),
                                CMOB = Convert.ToString(reader["CMOB"]),
                                CADD = Convert.ToString(reader["CADD"]),
                                INV_STATUS = Convert.ToString(reader["INV_STATUS"]),
                                BILL_STATUS = Convert.ToString(reader["BILL_STATUS"]),
                                TOTAL = reader["TOTAL"] == DBNull.Value ? 0 : Convert.ToSingle(reader["TOTAL"]),
                                DISC = Convert.ToSingle(reader["DISC"]),
                                DISC_AMT = reader["DISC_AMT"] == DBNull.Value ? 0 : Convert.ToSingle(reader["DISC_AMT"]),
                                NET_TOTAL = reader["NET_TOTAL"] == DBNull.Value ? 0 : Convert.ToSingle(reader["NET_TOTAL"]),
                                CASH = reader["CASH"] == DBNull.Value ? 0 : Convert.ToSingle(reader["CASH"]),
                                BANK = Convert.ToString(reader["BANK"]),
                                PARTY = Convert.ToString(reader["PARTY"]),
                                PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                BCODE = reader["BCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BCODE"]),
                                DEL = reader["DEL"] == DBNull.Value ? 0 : Convert.ToSingle(reader["DEL"]),
                                DEL_CHARGES = reader["DEL_CHARGES"] == DBNull.Value ? 0 : Convert.ToSingle(reader["DEL_CHARGES"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                WAITER = reader["WAITER"] == DBNull.Value ? 0 : Convert.ToInt32(reader["WAITER"]),
                                TABLE = reader["TABLE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TABLE"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                BILLMODE = Convert.ToString(reader["BILL_MODE"]),
                                SRBINV = Convert.ToString(reader["SRB_INV"]),
                                ADVANCE = Convert.ToString(reader["ADVANCE"]),
                                DELDATE = Convert.ToDateTime(reader["DEL_DATE"]).ToString("yyyy-MM-dd"),
                                MSGDELDATE = reader["DEL_DATE"].ToString(),
                                DUEDATE = Convert.ToString(reader["DUE_DATE"]),
                                CARDNO = Convert.ToString(reader["CARD_NO"]),
                                BILL_MODE = Convert.ToString(reader["BILL_MODE"]),
                                EDIT_USERID = Convert.ToString(reader["EDIT_USER_ID"]),
                                PAY_TYPE = Convert.ToString(reader["PAY_TYPE"]),
                                COMPLETE = reader["COMPLETE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMPLETE"]),
                                SER_CHARGES = reader["SER_CHARGES"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SER_CHARGES"]),
                                ADVBANK = reader["ADV_BANK"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ADV_BANK"]),
                                RECV = reader["RECV"] == DBNull.Value ? 0 : Convert.ToSingle(reader["RECV"]),
                                RETURN_AMT = reader["RECV"] == DBNull.Value ? 0 : Convert.ToSingle(reader["RECV"]),
                                CASHTAX = reader["CASH_TAX"] == DBNull.Value ? 0 : Convert.ToSingle(reader["CASH_TAX"]),
                                BANKTAX = reader["BANK_TAX"] == DBNull.Value ? 0 : Convert.ToSingle(reader["BANK_TAX"]),
                                PARTYTAX = reader["PARTY_TAX"] == DBNull.Value ? 0 : Convert.ToSingle(reader["PARTY_TAX"]),
                                SETTLE = reader["SETTLEMENT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SETTLEMENT"]),
                                SETTL_SIGN = Convert.ToString(reader["SETTL_SIGN"]),

                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }

                    response.data = jsonDataResult;
                    response.tranId = tranId;
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
        public MyHttpResponseMessage GetExpenseByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = "TBL_POS_EXP";
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        //string query = "SELECT TRAN_ID , V_DATE, VOUCHER_NO, BOOK_TYPE, CNAME, CMOB, INV_STATUS, TOTAL, DISC, SETT, NET_TOTAL, CASH, CHANGE, RECV, BCODE, PERIOD_ID " +
                        //               $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                        //               $"AND PERIOD_ID = '{common.Period}'";
                        string query = $"SELECT ACT_CODE , DESCR , Amount FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ACTCODE = Convert.ToString(reader["ACT_CODE"]),
                                DESCR = Convert.ToString(reader["DESCR"]),
                                Amount = Convert.ToString(reader["Amount"]),
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
        public MyHttpResponseMessage GetPOSTransactionDetailByCode(int? code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, b_i = string.Empty, stk_status = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE2;
                    b_i = menu.B_I;
                    stk_status = menu.STK_STATUS;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    var ip = common.IPAddress;
                    var computer = common.ComputerName;
                    var postal = common.PostalCode;
                    var username = common.Username;
                    var branch = common.Branch;
                    var period = common.Period;
                    var menuID = common.MenuID;
                    var count = 0;
                    int? pickId = 0;
                    bool isStockSufficient = true;
                    string InSufficientItem = "";
                    double InSufficientItemQty = 0;
                    string connectionString = new SQLService().getconnstring();
                    Dictionary<int?, double?> currentItems = new Dictionary<int?, double?>();
                    Dictionary<int?, double?> previousItems = new Dictionary<int?, double?>();
                    Dictionary<int?, double?> stockBalance = new Dictionary<int?, double?>();

                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string query = $@"SELECT PD.DT_CODE, PD.ITEM_CODE,PD.TAX_AMT, IT.ITEM_NAME , PD.QTY, PD.UNIT, UN.GROUP_NAME ,C.GROUP_NAME AS COLOR,C.GROUP_CODE AS COLOR_CODE,TS.GROUP_CODE AS SIZE_CODE,
                                        PD.RATE,PD.AMT, PD.DISC, PD.DISC_AMT , PD.NET_AMT, PD.RITEM, PD.BCODE,
                                        ISNULL(IG.SALES_TAX,0) AS TAX , IT.BARCODE , isnull(PDD.DT_CODE,0) AS PICK_ID , PD.REMARKS , PDD.TRAN_ID,
                                        TS.GROUP_NAME As SIZE
                                        FROM {table} PD
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = PD.MENU_ID
                                        LEFT OUTER JOIN TBL_BARCODE BG ON BG.CODE = CASE WHEN MB.B_I = 'B' THEN PD.ITEM_CODE END
                                        LEFT OUTER JOIN TBL_SIZE TS ON TS.GROUP_CODE = BG.SIZE
                                        LEFT OUTER JOIN TBL_COLOR C ON C.GROUP_CODE = BG.COLOR

                                        LEFT JOIN TBL_ITEMSMASTER IT on IT.ITEM_CODE = CASE WHEN MB.B_I = 'I' THEN PD.ITEM_CODE ELSE BG.ITEM_CODE END
                                        LEFT OUTER JOIN TBL_ITEMSGROUP IG ON IG.GROUP_CODE = IT.GROUP_CODE 
                                        LEFT OUTER JOIN TBL_POS_DETAIL PDD
                                        ON PDD.PICK_ID = PD.DT_CODE 
                                        LEFT JOIN TBL_UNIT UN on UN.GROUP_CODE = PD.UNIT  WHERE PD.DLT = 'T' AND PD.TRAN_ID = '{code}' AND PD.BCODE = '{common.Branch}' AND PD.PERIOD_ID = '{common.Period}'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),

                                VoucherNo = Convert.ToString(reader["PICK_ID"]) != "0"
                                ? $"POS/{(reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"])):D6}"
                                : "",

                                TRANID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),

                                LINK = "/POSTransactions?MOID=150&Code=151",

                                ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                SIZE_CODE = reader["SIZE_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE_CODE"]),
                                COLOR_CODE = reader["COLOR_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COLOR_CODE"]),

                                ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                                COLOR = Convert.ToString(reader["COLOR"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                SIZE = Convert.ToString(reader["SIZE"]),

                                UNIT = reader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UNIT"]),

                                UNIT_NAME = Convert.ToString(reader["GROUP_NAME"]),

                                RATE = Convert.ToString(reader["RATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                TAX_AMT = Convert.ToString(reader["TAX_AMT"]),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_AMT = Convert.ToString(reader["DISC_AMT"]),
                                NET_AMT = Convert.ToString(reader["NET_AMT"]),

                                BCODE = Convert.ToString(reader["BCODE"]),

                                RITEM = (reader["PICK_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PICK_ID"])) != 0 ? 1 : 0,

                                TAX = Convert.ToString(reader["TAX"]),

                                BARCODE = string.IsNullOrEmpty(Convert.ToString(reader["BARCODE"]))
                                ? string.Empty
                                : Convert.ToString(reader["BARCODE"]),

                                RITEM1 = (reader["PICK_ID"] == DBNull.Value ? "0" : Convert.ToString(reader["PICK_ID"])) != "0",

                                PICKID = reader["PICK_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PICK_ID"]),

                                REMARKS = Convert.ToString(reader["REMARKS"]),

                                STOCKSTATUS = stk_status
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                    if (stk_status == "Y")
                    {
                        if (b_i == "B")
                        {
                            foreach (dynamic item in jsonDataResult)
                            {
                                int itemCode = item.ITEM_CODE;
                                double qty = Convert.ToDouble(item.QTY ?? "0");

                                if (!currentItems.ContainsKey(itemCode))
                                {
                                    currentItems[itemCode] = qty;
                                }
                                else
                                {
                                    currentItems[itemCode] += qty;
                                }

                                if (!stockBalance.ContainsKey(itemCode))
                                {
                                    stockBalance[itemCode] = GetAvailableStock(itemCode, period, common);
                                }
                            }

                            if (code > 0)
                            {
                                previousItems = PreviousStockInBill(table, code, period, branch);

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

                            foreach (var items in currentItems)
                            {
                                double availableStock = stockBalance.ContainsKey(items.Key) ? stockBalance[items.Key] ?? 0 : 0;
                                double currentQty = items.Value ?? 0;

                                dynamic item = jsonDataResult[count];

                                // Naya object create kar ke StockAvailable field add karna
                                var updatedItem = new
                                {
                                    item.DT_CODE,
                                    item.ITEM_CODE,
                                    item.ITEM_NAME,
                                    item.COLOR_CODE,
                                    item.SIZE_CODE,
                                    item.QTY,
                                    item.UNIT,
                                    item.UNIT_NAME,
                                    item.RATE,
                                    item.AMT,
                                    item.DISC,
                                    item.DISC_AMT,
                                    item.NET_AMT,
                                    item.BCODE,
                                    item.RITEM,
                                    item.TAX,
                                    item.TAX_AMT,
                                    item.BARCODE,
                                    item.RITEM1,
                                    item.PICKID,
                                    item.REMARKS,
                                    item.STOCKSTATUS,
                                    item.SIZE,
                                    item.COLOR,

                                    //item.KOTPRINT,
                                    STOCKQTY = availableStock // New Field
                                };

                                // Replace jsonDataResult mein existing object with updated object
                                jsonDataResult[count] = updatedItem;



                                if (currentQty > availableStock)
                                {
                                    List<CustomKeyValuPair> barcodes = DropdownService.BarcodesKeyAndValue();
                                    var SelectedItem = barcodes.Where(b => b.key == items.Key).FirstOrDefault();
                                    InSufficientItem = SelectedItem.value;
                                    InSufficientItemQty = availableStock;
                                    isStockSufficient = false;
                                    break;
                                }
                                count++;
                            }

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
        public MyHttpResponseMessage DeleteExpenseRow(int code, Common common)
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
                    table = "TBL_POS_EXP";
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
        public MyHttpResponseMessage GetBarcodeList(int ItemId, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = "TBL_ITEMSMASTER";
                }
                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"
                                        SELECT B.ITEM_CODE , B.ITEM_CODE , B.BARCODE , C.GROUP_NAME AS COLOR , S.GROUP_NAME AS SIZE , B.SALE_RATE as SRATE  FROM {table} B
                                        LEFT JOIN TBL_COLOR C ON  C.GROUP_CODE = B.COLOR
                                        LEFT JOIN TBL_SIZE S ON S.GROUP_CODE = B.SIZE
                                        where B.ITEM_CODE = {ItemId} AND B.DLT =  'T' AND B.ASTATUS = 'Y'
                        ";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                CODE = Convert.ToInt32(reader["CODE"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                BARCODE = Convert.ToString(reader["BARCODE"]),
                                COLOR = Convert.ToString(reader["COLOR"]),
                                SIZE = Convert.ToString(reader["SIZE"]),
                                SRATE = Convert.ToInt32(reader["SRATE"]),
                                QUANTITY = Convert.ToInt32(1),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }

                    response.data = jsonDataResult;
                    response.msg = "";
                    response.msgType = 1;
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
            }
            return response;
        }
        public MyHttpResponseMessage GetAllBarcodeList(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? dcType = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    dcType = menu.DCTYPE;
                    table = "TBL_BARCODE";
                }
                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"
                                        SELECT IM.ITEM_NAME AS ITEM_ID, IM.ITEM_CODE AS ITEM_CODE, C.GROUP_CODE AS COLOR_ID, S.GROUP_CODE AS SIZE_ID,
                                        IM.ITEM_CODE AS BARCODE_CODE,S.GROUP_NAME AS SIZE, C.GROUP_NAME AS COLOR, IM.BARCODE, 
                                        IM.SALE_RATE AS RATE,ISNULL(DM.DISC, 0) As DISC
                                        FROM TBL_ITEMSMASTER IM
                                        LEFT OUTER JOIN TBL_BARCODE B
                                        ON IM.ITEM_CODE = B.ITEM_CODE
                                        LEFT OUTER JOIN TBL_SIZE S
                                        ON S.GROUP_CODE = B.SIZE
                                        LEFT OUTER JOIN TBL_COLOR C
                                        ON C.GROUP_CODE = B.COLOR
										LEFT OUTER JOIN TBL_IT_DISC_DETAIL DIS ON DIS.ITEM_CODE = IM.ITEM_CODE And DIS.BCODE = '{common.Branch}'
										LEFT OUTER JOIN TBL_IT_DISC_MASTER DM ON DM.CODE = DIS.CODE
                                        WHERE IM.DLT = 'T' AND IM.ASTATUS = 'Y' AND IM.ASTATUS = 'Y'
                                        ORDER BY IM.ITEM_NAME 
                        ";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                CODE = Convert.ToInt32(reader["BARCODE_CODE"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                ITEM_NAME = Convert.ToString(reader["ITEM_ID"]),
                                BARCODE = Convert.ToString(reader["BARCODE"]),
                                COLOR = Convert.ToString(reader["COLOR"]),
                                SIZE = Convert.ToString(reader["SIZE"]),
                                SRATE = Convert.ToInt32(reader["RATE"]),
                                DISC = Convert.ToInt32(reader["DISC"]),
                                QUANTITY = Convert.ToInt32(1),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }

                    response.data = jsonDataResult;
                    response.msg = "";
                    response.msgType = 1;
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
            }
            return response;
        }
        public MyHttpResponseMessage DeletePOSTransactionDetailByCode(CustomPOSTransaction modelRecord, int code, int ItemId, string dtcode, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string? table1 = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE2;
                    table1 = menu.TABLE1;
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
                            string query = string.Empty;
                            foreach (var detail in modelRecord.Detail)
                            {
                                if (detail.RATE.HasValue)
                                {
                                    query = $"UPDATE {table} SET DLT = 'F' " +
                                                  $"WHERE TRAN_ID = {code} AND ITEM_CODE = {ItemId} " +
                                                  $"AND DT_CODE = {dtcode}";

                                    // yahan query execute karni hai
                                }
                            }
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();

                            string updatequery = $"UPDATE {table1} SET " +
                                       $"INV_STATUS = '{modelRecord.Master.INV_STATUS}', " +
                                       $"TOTAL = '{modelRecord.Master.TOTAL}', " +
                                       $"DISC = '{modelRecord.Master.DISC}', " +
                                       $"NET_TOTAL = '{modelRecord.Master.NET_TOTAL}', " +
                                       $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                       $"DEL = '{modelRecord.Master.DEL}', " +
                                       $"DEL_CHARGES = '{modelRecord.Master.DEL_CHARGES}', " +
                                       $"DISC_AMT = '{modelRecord.Master.DISC_AMT}', " +
                                       $"BILL_STATUS = '{modelRecord.Master.BILL_STATUS}', " +
                                       $"CASH = '{modelRecord.Master.CASH}', " +
                                       $"CACT_CODE = '{modelRecord.Master.CACT_CODE}', " +
                                       $"BANK = '{modelRecord.Master.BANK}', " +
                                       $"BACT_CODE = '{modelRecord.Master.BACT_CODE}', " +
                                       $"PARTY = '{modelRecord.Master.PARTY}', " +
                                       $"PARTY_CODE = '{modelRecord.Master.PARTY_CODE}', " +
                                       $"ACT_CODE = '{modelRecord.Master.ACT_CODE}', " +
                                       $"RECV = '{modelRecord.Master.RECV}' " +
                                       $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";

                            command.CommandText = updatequery;
                            command.ExecuteNonQuery();


                            response.msgType = 1;
                            response.msg = "Item Deleted Successfully";
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
        public MyHttpResponseMessage GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, Common common)
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
                        string query = @$"SELECT SM.TRAN_ID , SM.V_DATE, SM.VOUCHER_NO, PTS.PARTY_NAME AS SELLER, PTB.PARTY_NAME AS BUYER
                                        ,SD.ITEM_CODE, IM.ITEM_NAME,
                                        ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {pickDetailTable}
                                        WHERE SD.DT_CODE = DT_CODE AND SD.PERIOD_ID = PERIOD_ID AND SD.BCODE = BCODE),0) AS SQTY,
                                        ISNULL((SELECT SUM(ISNULL(QTY, 0)) FROM {detailTable}
                                        WHERE DFD.PICK_ID = PICK_ID AND DFD.PERIOD_ID = PERIOD_ID AND DFD.BCODE = BCODE),0) AS DQTY,
                                        (ISNULL((SELECT SUM(ISNULL(BAL_QTY, 0)) FROM {pickDetailTable}
                                        WHERE SD.DT_CODE = DT_CODE AND SD.PERIOD_ID = PERIOD_ID AND SD.BCODE = BCODE), 0) -
                                        ISNULL((SELECT SUM(ISNULL(BAL_QTY,0)) FROM {detailTable}
                                        WHERE DFD.PICK_ID = PICK_ID AND DFD.PERIOD_ID = PERIOD_ID AND DFD.BCODE = BCODE),0))
                                        AS BAL_QTY, '' AS QTY,
                                        SD.RATE,SD.AMT,SD.DT_CODE as PICK_ID, SD.UNIT, SM.SELLER_CODE, SM.SACT_CODE, SM.BUYER_CODE, SM.BACT_CODE, SM.COND, SM.CREDIT_DAYS, SM.DUE_DATE, SD.RT_TYPE
                                        FROM {pickMasterTable} SM
                                        LEFT OUTER JOIN {pickDetailTable} SD
                                        ON SD.TRAN_ID = SM.TRAN_ID AND SD.BCODE = SM.BCODE AND SD.PERIOD_ID = SM.PERIOD_ID
                                        LEFT OUTER JOIN {detailTable} DFD
                                        ON SD.DT_CODE = DFD.PICK_ID AND SD.BCODE = DFD.BCODE AND SD.PERIOD_ID = DFD.PERIOD_ID
                                        AND DFD.DLT = 'T'
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PTS
                                        ON PTS.PARTY_CODE = SM.SELLER_CODE AND PTS.ACT_CODE = SM.SACT_CODE
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PTB
                                        ON PTB.PARTY_CODE = SM.BUYER_CODE AND PTB.ACT_CODE = SM.BACT_CODE
                                        LEFT OUTER JOIN TBL_ITEMSMASTER IM
                                        ON IM.ITEM_CODE = SD.ITEM_CODE
                                        WHERE SM.DLT = 'T' AND SM.ASTATUS = 'Y' AND SD.DLT = 'T'
                                        AND SM.V_DATE = '{sodaDate}' AND (DFD.SCOMP= 0 OR DFD.SCOMP IS NULL)
                                        GROUP BY
                                        SM.TRAN_ID , SM.V_DATE, SM.VOUCHER_NO, PTS.PARTY_NAME , PTB.PARTY_NAME
                                        ,SD.ITEM_CODE, IM.ITEM_NAME, SD.RATE, SD.AMT, SD.DT_CODE, SD.PERIOD_ID, SD.BCODE, DFD.PICK_ID,
                                        DFD.BCODE, DFD.PERIOD_ID, SD.UNIT, SM.SELLER_CODE, SM.SACT_CODE, SM.BUYER_CODE, SM.BACT_CODE, SM.COND, SM.CREDIT_DAYS, SM.DUE_DATE, SD.RT_TYPE";
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
                                SELLER = Convert.ToString(reader["SELLER"]),
                                BUYER = Convert.ToString(reader["BUYER"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                                QTY = Convert.ToString(reader["SQTY"]),
                                SQTY = Convert.ToString(reader["SQTY"]),
                                DQTY = Convert.ToString(reader["DQTY"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                PICK_ID = Convert.ToString(reader["PICK_ID"]),
                                SELLER_CODE = $"{Convert.ToString(reader["SELLER_CODE"])}{Convert.ToString(reader["SACT_CODE"])}",
                                BUYER_CODE = $"{Convert.ToString(reader["BUYER_CODE"])}{Convert.ToString(reader["BACT_CODE"])}",
                                UNIT = Convert.ToInt32(reader["UNIT"]),
                                COND = Convert.ToString(reader["COND"]),
                                CREDIT_DAYS = Convert.ToString(reader["CREDIT_DAYS"]),
                                DUE_DATE = Convert.ToString(reader["DUE_DATE"]),
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
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
        private List<object> GetDeliveryDetails(string tranID, MyHttpResponseMessage menuData)
        {
            List<object> jsonDataResult = new List<object>();
            try
            {
                string? detailTable = string.Empty;
                if (menuData.data != null)
                {
                    var menu = (Menu)menuData.data;
                    detailTable = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(detailTable))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT A.TRAN_ID, IM.ITEM_NAME AS ITEM_CODE, A.QTY," +
                                       "U.GROUP_NAME AS UNIT, QTY2, BAL_QTY, RATE, RTRIM(LTRIM(RT_TYPE)) + ' kg' as RT_TYPE, AMT, DT_DESC, A.TRUCK_NO, " +
                                       "A.CONT_NO, A.SCOMP, A.BR_AMOUNT_BUYER, A.BR_AMOUNT_SELLER, A.WT_AMOUNT_BUYER, A.WT_AMOUNT_SELLER, A.NET_AMT " +
                                       $"FROM {detailTable} A " +
                                       "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE " +
                                       "LEFT OUTER JOIN TBL_UNIT U ON A.UNIT = U.GROUP_CODE " +
                                       $"WHERE A.DLT = 'T' AND A.TRAN_ID = '{tranID}' ORDER BY A.DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                TRUCK_NO = Convert.ToString(reader["TRUCK_NO"]),
                                CONT_NO = Convert.ToString(reader["CONT_NO"]),
                                SCOMP = Convert.ToString(reader["SCOMP"]) == "1" ? "Yes" : "No",
                                BR_AMOUNT_BUYER = Convert.ToString(reader["BR_AMOUNT_BUYER"]),
                                BR_AMOUNT_SELLER = Convert.ToString(reader["BR_AMOUNT_SELLER"]),
                                WT_AMOUNT_BUYER = Convert.ToString(reader["WT_AMOUNT_BUYER"]),
                                WT_AMOUNT_SELLER = Convert.ToString(reader["WT_AMOUNT_SELLER"]),
                                NET_AMT = Convert.ToString(reader["NET_AMT"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
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
            }
            return jsonDataResult;
        }
        public MyHttpResponseMessage GetItemsGroup(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                var role = $"'{common.RoleType}'";
                var roleId = $"'{common.RoleID}'";
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
                        string query = $@"
                                       SELECT Distinct IG.GROUP_CODE,   IG.GROUP_NAME,  IG.IPIC FROM TBL_ROLE R LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE
                                         LEFT OUTER JOIN TBL_ITEMSGROUP IG ON M.GROUP_CODE = IG.GROUP_CODE WHERE R.ROLE_TYPE = {role}
                                             AND R.MODULE_ID = 4 
                                             AND M.ITEM_TYPE = 'F'
                                             AND IG.GROUP_TYPE = 'S' 
                                             AND IG.DLT = 'T' 
                                             AND IG.ASTATUS = 'Y'
                                             AND R.ROLE_ID = {roleId}
                                         GROUP BY 
                                             IG.GROUP_CODE, 
                                             IG.GROUP_NAME, 
                                             IG.IPIC

                        UNION ALL

                        SELECT Distinct IG.GROUP_CODE,   IG.GROUP_NAME,  IG.IPIC  FROM TBL_ITEMSGROUP IG LEFT OUTER JOIN TBL_ITEMSMASTER M ON M.GROUP_CODE = IG.GROUP_CODE WHERE 
                        {role} = 'A'  AND IG.DLT = 'T'  AND IG.ASTATUS = 'Y' AND  IG.GROUP_TYPE = 'S' AND M.ITEM_TYPE = 'F'

                        UNION ALL

                        SELECT Distinct IG.GROUP_CODE, IG.GROUP_NAME, IG.IPIC FROM TBL_ITEMSGROUP IG WHERE {role} = 'U'  AND IG.GROUP_TYPE = 'S'  AND IG.DLT = 'T'
                                                                 AND  (SELECT COUNT(R.MODULE_ID ) FROM TBL_ROLE R
                                                                LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE
                                                                 LEFT OUTER JOIN TBL_ITEMSGROUP IG ON M.GROUP_CODE = IG.GROUP_CODE WHERE R.ROLE_TYPE = 'U'
                                                                     AND R.MODULE_ID = 4 
                                                                     AND M.ITEM_TYPE = 'F'
                                                                     AND IG.GROUP_TYPE = 'S' 
                                                                     AND IG.DLT = 'T' 
                                                                     AND IG.ASTATUS = 'Y'
                                                                     AND R.ROLE_ID = {roleId}
                                                                )< 1
                                         
                        ";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                IPIC = Convert.ToString(reader["IPIC"])
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }

                    response.data = jsonDataResult;
                    response.msg = "";
                    response.msgType = 1;
                }
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
            }
            return response;
        }
        public MyHttpResponseMessage GetItemsMasterByGroup(int groupId, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string query = string.Empty;
                var role = $"'{common.RoleType}'";
                var roleId = $"'{common.RoleID}'";
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    if (groupId != 0)
                    {
                        query = $@"
                                  SELECT 
                                  M.ITEM_CODE , M.ITEM_ID , M.ITEM_NAME , M.IPIC , M.SALE_RATE FROM TBL_ROLE R LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE LEFT OUTER JOIN TBL_ITEMSGROUP IG
                                   ON M.GROUP_CODE = IG.GROUP_CODE WHERE R.ROLE_TYPE = 
                                  {role} 
  
                                      AND R.MODULE_ID = 4 
                                      AND IG.GROUP_TYPE = 'S' 
                                      AND IG.DLT = 'T' 
                                      AND IG.ASTATUS = 'Y'
                                      AND R.ROLE_ID =
	  
	                                {roleId}
	
                                      and M.GROUP_CODE = 
	                                {groupId} 
                                        and M.ITEM_TYPE = 'F'
	
	                                  GROUP BY M.ITEM_CODE , M.ITEM_ID , M.ITEM_NAME , M.IPIC , M.SALE_RATE

                                  UNION ALL

                                  SELECT M.ITEM_CODE , M.ITEM_ID , M.ITEM_NAME , M.IPIC , M.SALE_RATE FROM TBL_ITEMSMASTER M WHERE 
  
                                  {role}
  
                                  = 'A'  
                                   and M.GROUP_CODE = 
                                  {groupId}
                                    and M.ITEM_TYPE = 'F'
                                      AND M.DLT = 'T' 
                                      AND M.ASTATUS = 'Y'
                                UNION ALL
	                                    SELECT M.ITEM_CODE , M.ITEM_ID , M.ITEM_NAME , M.IPIC , M.SALE_RATE FROM TBL_ITEMSMASTER M WHERE 
  
                                {role}

                                  = 'U'  
                                   and M.GROUP_CODE = 
                                   {groupId}
                                    and M.ITEM_TYPE = 'F'
                                      AND M.DLT = 'T' 
                                      AND M.ASTATUS = 'Y' AND 
	                                   (SELECT 
                                 COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE LEFT OUTER JOIN TBL_ITEMSGROUP IG
                                   ON M.GROUP_CODE = IG.GROUP_CODE WHERE R.ROLE_TYPE = 
                                   {role} 
   
                                      AND R.MODULE_ID = 4 
                                      AND IG.GROUP_TYPE = 'S' 
                                      AND IG.DLT = 'T' 
                                      AND IG.ASTATUS = 'Y'
                                      AND R.ROLE_ID =
	  
	                                 {roleId}
	 
                                      and M.GROUP_CODE = 
	                                {groupId} 
	                                and M.ITEM_TYPE = 'F'
	                                  ) < 1
                    ";
                    }
                    else
                    {
                        query = $@"
                                   SELECT 
                                  M.ITEM_CODE , M.ITEM_ID , M.ITEM_NAME , M.IPIC , M.SALE_RATE FROM TBL_ROLE R LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE LEFT OUTER JOIN TBL_ITEMSGROUP IG
                                   ON M.GROUP_CODE = IG.GROUP_CODE WHERE R.ROLE_TYPE = 
                                  {role} 
  
                                      AND R.MODULE_ID = 4 
                                      AND IG.GROUP_TYPE = 'S' 
                                      AND IG.DLT = 'T' 
                                      AND IG.ASTATUS = 'Y'
                                      AND R.ROLE_ID =
	  
	                                {roleId}
                                        and M.ITEM_TYPE = 'F'
	
	                                  GROUP BY M.ITEM_CODE , M.ITEM_ID , M.ITEM_NAME , M.IPIC , M.SALE_RATE

                                  UNION ALL

                                  SELECT M.ITEM_CODE , M.ITEM_ID , M.ITEM_NAME , M.IPIC , M.SALE_RATE FROM TBL_ITEMSMASTER M WHERE 
  
                                  {role}
  
                                  = 'A'  
                                    and M.ITEM_TYPE = 'F'
                                      AND M.DLT = 'T' 
                                      AND M.ASTATUS = 'Y'
                                UNION ALL
	                                    SELECT M.ITEM_CODE , M.ITEM_ID , M.ITEM_NAME , M.IPIC , M.SALE_RATE FROM TBL_ITEMSMASTER M WHERE 
  
                                {role}

                                  = 'U'  
                                    and M.ITEM_TYPE = 'F'
                                      AND M.DLT = 'T' 
                                      AND M.ASTATUS = 'Y' AND 
	                                   (SELECT 
                                 COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE LEFT OUTER JOIN TBL_ITEMSGROUP IG
                                   ON M.GROUP_CODE = IG.GROUP_CODE WHERE R.ROLE_TYPE = 
                                   {role} 
   
                                      AND R.MODULE_ID = 4 
                                      AND IG.GROUP_TYPE = 'S' 
                                      AND IG.DLT = 'T' 
                                      AND IG.ASTATUS = 'Y'
                                      AND R.ROLE_ID =
	  
	                                 {roleId}

	                                and M.ITEM_TYPE = 'F'
	                                  ) < 1
                    ";
                    }


                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                            ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                            IPIC = Convert.ToString(reader["IPIC"]),
                            SALE_RATE = Convert.ToString(reader["SALE_RATE"])
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
            }
            return response;
        }
        public MyHttpResponseMessage GetItemsMasterByCode(int itemId, string barcode, int Qty, int TranId, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            bool isStockSufficient = true;
            string InSufficientItem = "";
            double InSufficientItemQty = 0;
            double availableStock = 0;
            Dictionary<int?, double?> currentItems = new Dictionary<int?, double?>();
            Dictionary<int?, double?> previousItems = new Dictionary<int?, double?>();
            Dictionary<int?, double?> stockBalance = new Dictionary<int?, double?>();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string detailTable = string.Empty, b_i = string.Empty, stk_status = string.Empty;
                var period = common.Period;
                var branch = common.Branch;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    detailTable = menu.TABLE2;
                    b_i = menu.B_I;
                    stk_status = menu.STK_STATUS;
                }

                //if (stk_status == "Y")
                {
                    if (true)
                    {
                        if (!currentItems.ContainsKey(itemId))
                        {
                            currentItems.Add(itemId, Qty);
                        }
                        else
                        {
                            currentItems[itemId] += Qty;
                        }

                        if (!stockBalance.ContainsKey(itemId))
                        {
                            stockBalance.Add(itemId, GetAvailableStock(itemId, period, common));
                        }

                        if (TranId > 0)
                        {
                            previousItems = PreviousStockInBill(detailTable, TranId, period, branch);

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
                            availableStock = stockBalance.ContainsKey(item.Key) ? stockBalance[item.Key] ?? 0 : 0;
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

                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = string.Empty;
                    var status = barcode == "" || barcode == null ? "I" : "B";

                    query = @$"DECLARE @TYPE NVARCHAR(2) = '{status}';
                            DECLARE @TDATE DATETIME = '{DateTime.Now.ToShortDateString()}';
                            DECLARE @ITEM_CODE INT = {itemId};
                            DECLARE @BCODE INT = {common.Branch};
                            SELECT 
                                IM.ITEM_CODE, 
                                IM.ITEM_ID,
                                IM.ITEM_NAME AS ITEM_NAME,
                                S.GROUP_NAME AS SIZE,
								C.GROUP_NAME AS COLOR,
								C.GROUP_CODE AS COLOR_CODE,
								S.GROUP_CODE AS SIZE_CODE,
                                CASE 
									WHEN @TYPE = 'B' THEN IM.SALE_RATE 
									WHEN @TYPE = 'I' THEN IM.SALE_RATE 
								END  AS SALE_RATE,
                                IU.GROUP_NAME, 
                                IU.GROUP_CODE,
                                ISNULL(IM.BARCODE,'') AS BARCODE, 
                                ISNULL(IG.SALES_TAX, 0) AS TAX,

	
	                            ISNULL((
	                            SELECT 
                               MAX(M.DISC) AS DISC
                            FROM 



                                TBL_IT_DISC_MASTER M
                            LEFT OUTER JOIN TBL_IT_DISC_DETAIL D 
                                ON D.CODE = M.CODE
	                            WHERE D.ITEM_CODE = 
	                            IM.ITEM_CODE
	                            AND
                                M.DLT = 'T' AND 
                                D.DLT = 'T' AND 
                                M.ASTATUS = 'Y' AND 
                                DISC_EXP = 0 AND 
                                @TDATE BETWEEN M.FDATE AND M.TDATE
	                            AND IM.ITEM_CODE = 
	                            CASE WHEN @TYPE = 'B' THEN @ITEM_CODE WHEN  @TYPE = 'I' THEN  @ITEM_CODE END 

	                            AND D.BCODE = @BCODE
	                            ),0)
	
	
	                            AS DISC
                            FROM 
                                TBL_ITEMSMASTER IM
                            LEFT JOIN TBL_UNIT IU ON IU.GROUP_CODE = IM.IUNIT_CODE
                            LEFT JOIN TBL_ITEMSGROUP IG ON IG.GROUP_CODE = IM.GROUP_CODE
                            --LEFT JOIN TBL_BARCODE BG ON 
                            --    @TYPE = 'B' AND IM.ITEM_CODE = I.ITEM_CODE AND BG.DLT = 'T'
                            LEFT JOIN TBL_SIZE S ON IM.SIZE = S.GROUP_CODE 
				            LEFT JOIN TBL_COLOR C ON IM.COLOR = C.GROUP_CODE 
                            OUTER APPLY (
								SELECT TOP 1 PD.RATE
								FROM TBL_POS_DETAIL PD
								WHERE PD.ITEM_CODE = IM.ITEM_CODE
								AND PD.RATE > 0   -- sirf positive rate chahiye
								ORDER BY PD.TRAN_ID DESC
							) PD
                            WHERE 
                                IM.DLT = 'T' 
                                AND IM.ASTATUS = 'Y' AND 'T' 
	                            = CASE WHEN  @TYPE = 'B' THEN  IM.DLT WHEN  @TYPE = 'I' THEN 'T' END
	                            AND @ITEM_CODE = CASE WHEN @TYPE = 'B' THEN IM.ITEM_CODE WHEN  @TYPE = 'I' THEN IM.ITEM_CODE END";


                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                            ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                            SALE_RATE = Convert.ToString(reader["SALE_RATE"]),
                            DISC = Convert.ToString(reader["DISC"]),
                            SIZE = Convert.ToString(reader["SIZE"]),
                            COLOR = Convert.ToString(reader["COLOR"]),
                            COLOR_CODE = reader["COLOR_CODE"] != DBNull.Value ? Convert.ToDecimal(reader["COLOR_CODE"]) : 0,
                            SIZE_CODE = reader["SIZE_CODE"] != DBNull.Value ? Convert.ToDecimal(reader["SIZE_CODE"]) : 0,
                            TAX = Convert.ToDecimal(reader["TAX"]),
                            BARCODE = Convert.ToString(reader["BARCODE"]),
                            UNITID = reader["GROUP_CODE"] != DBNull.Value ? Convert.ToDecimal(reader["GROUP_CODE"]) : 0,
                            UNIT = reader["GROUP_NAME"] != DBNull.Value ? Convert.ToString(reader["GROUP_NAME"]) : string.Empty,
                            STOCK = isStockSufficient ? $"HAS ONLY {availableStock}" : $"HAS ONLY {InSufficientItemQty}",
                            STOCKQTY =  availableStock,
                            STOCKSTATUS = stk_status
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
            }
            return response;
        }
        public MyHttpResponseMessage GetDataForReport(POSPrint_Model modelRecord, CustomMenuDetail menuDetails, Info currentLabel, Branch currentBranch, Company currentCompany, POSMapping currentMapping, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            POSTransactionReport masterData = new POSTransactionReport();
            CustomPOSTransactionForPrintReport reportData = new CustomPOSTransactionForPrintReport();
            List<POSMaster_Print> MasterResult = new List<POSMaster_Print>();
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

                if (modelRecord.tranId != 0 && modelRecord.tranId != null)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        var dt_Code = modelRecord.DT_Code == "" || modelRecord.DT_Code == null ? "0" : modelRecord.DT_Code;

                        // ====================== MASTER QUERY ======================
                        string query = $@"EXEC [PROC_PRINT] '{table}','{detailTable}','','','{common.Branch}','{common.Period}','{modelRecord.tranId}','{common.Username}','{dt_Code}','{menuDetails.REPORT_NAME}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        if (menuDetails.REPORT_NAME == "PointOfSale_2" || menuDetails.REPORT_NAME == "SaleReceipt")
                        {
                            while (reader.Read())
                            {
                                var row = new POSMaster_Print
                                {
                                    V_DATE = reader["V_DATE"] != DBNull.Value
                                      ? Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy")
                                      : "",
                                    DEL_DATE = reader["DEL_DATE"] != DBNull.Value
                                        ? Convert.ToDateTime(reader["DEL_DATE"]).ToString("dd-MM-yyyy")
                                        : "",
                                    VOUCHER_NO = reader["VOUCHER_NO"]?.ToString(),
                                    SETTLEMENT = reader["SETTLEMENT"] == DBNull.Value ? null : Convert.ToDouble(reader["SETTLEMENT"]),
                                    SER_CHARGES = reader["SER_CHARGES"] == DBNull.Value ? null : Convert.ToDouble(reader["SER_CHARGES"]),
                                    DEL_CHARGES = reader["DEL_CHARGES"] == DBNull.Value ? null : Convert.ToDouble(reader["DEL_CHARGES"]),
                                    DEL = reader["DEL"] == DBNull.Value ? null : Convert.ToDouble(reader["DEL"]),
                                    INV_STATUS = reader["INV_STATUS"]?.ToString(),
                                    TOTAL = reader["TOTAL"] == DBNull.Value ? null : Convert.ToDouble(reader["TOTAL"]),
                                    DISC_Per = reader["DISC_Per"] == DBNull.Value ? null : Convert.ToDouble(reader["DISC_Per"]),
                                    DISC_AMT = reader["DISC_AMT"] == DBNull.Value ? null : Convert.ToDouble(reader["DISC_AMT"]),
                                    NET_TOTAL = reader["NET_TOTAL"] == DBNull.Value ? null : Convert.ToDouble(reader["NET_TOTAL"]),
                                    CNAME = reader["CNAME"]?.ToString(),
                                    CMOB = reader["CMOB"]?.ToString(),
                                    Salesman = reader["SALESMAN"]?.ToString(),
                                    PAY_TYPE = reader["PAY_TYPE"]?.ToString(),
                                    CASH = reader["CASH"] == DBNull.Value ? null : Convert.ToDouble(reader["CASH"]),
                                    BANK = reader["BANK"] == DBNull.Value ? null : Convert.ToDouble(reader["BANK"]),
                                    PARTY = reader["PARTY"] == DBNull.Value ? null : Convert.ToDouble(reader["PARTY"]),
                                    CASH_TAX_VALUE = reader["CASH_TAX_VALUE"] == DBNull.Value ? null : Convert.ToDouble(reader["CASH_TAX_VALUE"]),
                                    BANK_TAX_VALUE = reader["BANK_TAX_VALUE"] == DBNull.Value ? null : Convert.ToDouble(reader["BANK_TAX_VALUE"]),
                                    PARTY_TAX_VALUE = reader["PARTY_TAX_VALUE"] == DBNull.Value ? null : Convert.ToDouble(reader["PARTY_TAX_VALUE"]),
                                    CASH_TAX = reader["CASH_TAX"] == DBNull.Value ? null : Convert.ToDouble(reader["CASH_TAX"]),
                                    BANK_TAX = reader["BANK_TAX"] == DBNull.Value ? null : Convert.ToDouble(reader["BANK_TAX"]),
                                    PARTY_TAX = reader["PARTY_TAX"] == DBNull.Value ? null : Convert.ToDouble(reader["PARTY_TAX"]),
                                    RECV = reader["RECV"] == DBNull.Value ? null : Convert.ToDouble(reader["RECV"]),
                                    CashBack = reader["CashBack"] == DBNull.Value ? null : Convert.ToDouble(reader["CashBack"]),
                                    ADVANCE = reader["ADVANCE"] == DBNull.Value ? null : Convert.ToDouble(reader["ADVANCE"]),
                                    ADV_BANK = reader["ADV_BANK"] == DBNull.Value ? null : Convert.ToDouble(reader["ADV_BANK"]),
                                    BALANCE = reader["BALANCE"] == DBNull.Value ? null : Convert.ToDouble(reader["BALANCE"]),
                                    TOTAL_ADVANCE = reader["TOTAL_ADVANCE"] == DBNull.Value ? null : Convert.ToDouble(reader["TOTAL_ADVANCE"]),
                                    Waiter = reader["Waiter"]?.ToString(),
                                    TABLE = reader["TABLE"]?.ToString(),
                                    BARCODE = reader["BARCODE"]?.ToString(),
                                    ITEM_NAME = reader["ITEM_NAME"]?.ToString(),
                                    QTY = reader["QTY"] == DBNull.Value ? null : Convert.ToDouble(reader["QTY"]),
                                    RATE = reader["RATE"] == DBNull.Value ? null : Convert.ToDouble(reader["RATE"]),
                                    NET_AMT = reader["NET_AMT"] == DBNull.Value ? null : Convert.ToDouble(reader["NET_AMT"]),
                                    DISC = reader["DISC"] == DBNull.Value ? null : Convert.ToDouble(reader["DISC"]),
                                    DETAIL_DISC_AMT = reader["DETAIL_DISC_AMT"] == DBNull.Value ? null : Convert.ToDouble(reader["DETAIL_DISC_AMT"]),
                                    REMARKS = reader["REMARKS"]?.ToString(),
                                    D_REMARKS = reader["D_REMARKS"]?.ToString(),
                                    USER_NAME = reader["USER_NAME"]?.ToString(),
                                    BILL_STATUS = reader["BILL_STATUS"]?.ToString(),
                                    BILL_MODE = reader["BILL_MODE"]?.ToString(),
                                    SRB_INV = reader["SRB_INV"]?.ToString(),
                                    SRB_VN = reader["SRB_VN"]?.ToString(),
                                    COMPLETE = reader["COMPLETE"] == DBNull.Value ? null : Convert.ToInt32(reader["COMPLETE"]),
                                    API_STATUS = reader["API_STATUS"]?.ToString(),
                                    GROSS_TOTAL = reader["GROSS_TOTAL"] == DBNull.Value ? 0 : Convert.ToDouble(reader["GROSS_TOTAL"]),
                                    INVOICE_VALUE = reader["INVOICE_VALUE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["INVOICE_VALUE"]),
                                    BShortName = reader["B_SHORT_NAME"]?.ToString(),
                                    BName = reader["BNAME"]?.ToString(),
                                    BTel = reader["B_TEL"]?.ToString(),
                                    BAddress = reader["B_ADDRESS"]?.ToString(),
                                    BNTN = reader["B_NTN"]?.ToString(),
                                    CPC_CODE = reader["CPC_CODE"]?.ToString(),
                                    MENU_TERMS = reader["MENU_TERMS"]?.ToString(),
                                    FB_LINK = reader["FB_LINK"]?.ToString(),
                                    INSTA_LINK = reader["INSTA_LINK"]?.ToString(),
                                    WEB_LINK = reader["WEB_LINK"]?.ToString(),
                                    TIKTOK_LINK = reader["TIKTOK_LINK"]?.ToString(),
                                    YOUTUBE_LINK = reader["YOUTUBE_LINK"]?.ToString(),
                                    WIFINAME = reader["WIFI_NAME"]?.ToString(),
                                    WIFIPASSWORD = reader["WIFI_PASSWORD"]?.ToString(),
                                    C_LOGO = reader["C_LOGO"]?.ToString(),
                                    PRINT_MODE_LABLE = reader["PRINT_MODE_LABLE"]?.ToString(),
                                    QR_CODE = reader["QR_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QR_CODE"]),
                                    P_WINDOW = reader["P_WINDOW"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P_WINDOW"]),
                                    P_PRINTER = reader["PAY_PRINTER"]?.ToString(),

                                };

                                MasterResult.Add(row);
                            }
                        }
                        else if (menuDetails.REPORT_NAME == "POS_KOT")
                        {
                            while (reader.Read())
                            {
                                var row = new POSMaster_Print
                                {

                                    V_DATE = reader["V_DATE"] != DBNull.Value
                                             ? Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy")
                                             : "",
                                    DEL_DATE = reader["DEL_DATE"] != DBNull.Value
                                               ? Convert.ToDateTime(reader["DEL_DATE"]).ToString("dd-MM-yyyy")
                                               : "",
                                    VOUCHER_NO = reader["VOUCHER_NO"]?.ToString(),

                                    Waiter = reader["Waiter"]?.ToString(),
                                    TABLE = reader["TABLE"]?.ToString(),
                                    ITEM_NAME = reader["ITEM_NAME"]?.ToString(),
                                    QTY = reader["QTY"] == DBNull.Value ? null : Convert.ToDouble(reader["QTY"]),
                                    D_REMARKS = reader["D_REMARKS"]?.ToString(),
                                    BILL_STATUS = reader["BILL_STATUS"]?.ToString(),
                                    USER_NAME = reader["USER_NAME"]?.ToString(),
                                    ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? null : Convert.ToInt32(reader["ITEM_CODE"]),
                                    STICKER_LOGO = reader["STICKER_LOGO"]?.ToString(),
                                    LOCATION_SNAME = reader["LOCATION_SNAME"]?.ToString(),
                                    INV_STATUS = reader["INV_STATUS"]?.ToString(),
                                    QR_CODE = reader["QR_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QR_CODE"]),
                                    P_WINDOW = reader["P_WINDOW"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P_WINDOW"]),
                                    KOT_PRINTER = reader["KOT_PRINTER"]?.ToString(),
                                    STICKER_PRINTER = reader["STICKER_PRINTER"]?.ToString(),
                                    JobName = reader["JOB_NAME"]?.ToString(),
                                    CNAME = reader["C_NAME"]?.ToString(),

                                };

                                MasterResult.Add(row);
                            }
                        }
                        //else if (menuDetails.REPORT_NAME == "STICKER")
                        //{
                        //    while (reader.Read())
                        //    {
                        //        var row = new POSMaster_Print
                        //        {

                        //            V_DATE = reader["V_DATE"] != DBNull.Value
                        //                     ? Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy")
                        //                     : "",
                        //            DEL_DATE = reader["DEL_DATE"] != DBNull.Value
                        //                       ? Convert.ToDateTime(reader["DEL_DATE"]).ToString("dd-MM-yyyy")
                        //                       : "",
                        //            VOUCHER_NO = reader["VOUCHER_NO"]?.ToString(),

                        //            Waiter = reader["Waiter"]?.ToString(),
                        //            TABLE = reader["TABLE"]?.ToString(),
                        //            ITEM_NAME = reader["ITEM_NAME"]?.ToString(),
                        //            QTY = reader["QTY"] == DBNull.Value ? null : Convert.ToDouble(reader["QTY"]),
                        //            D_REMARKS = reader["D_REMARKS"]?.ToString(),
                        //            BILL_STATUS = reader["BILL_STATUS"]?.ToString(),
                        //            USER_NAME = reader["USER_NAME"]?.ToString(),
                        //            ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? null : Convert.ToInt32(reader["ITEM_CODE"]),
                        //            STICKER_LOGO = reader["STICKER_LOGO"]?.ToString(),
                        //            LOCATION_SNAME = reader["LOCATION_SNAME"]?.ToString(),
                        //            INV_STATUS = reader["INV_STATUS"]?.ToString(),
                        //            BShortName = reader["B_SHORT_NAME"]?.ToString(),
                        //            BName = reader["BNAME"]?.ToString(),
                        //            BTel = reader["B_TEL"]?.ToString(),
                        //            BAddress = reader["B_ADDRESS"]?.ToString(),
                        //            BNTN = reader["B_NTN"]?.ToString(),
                        //            CPC_CODE = reader["CPC_CODE"]?.ToString(),
                        //            MENU_TERMS = reader["MENU_TERMS"]?.ToString(),
                        //            FB_LINK = reader["FB_LINK"]?.ToString(),
                        //            INSTA_LINK = reader["INSTA_LINK"]?.ToString(),
                        //            WEB_LINK = reader["WEB_LINK"]?.ToString(),
                        //            TIKTOK_LINK = reader["TIKTOK_LINK"]?.ToString(),
                        //            YOUTUBE_LINK = reader["YOUTUBE_LINK"]?.ToString(),
                        //            WIFINAME = reader["WIFI_NAME"]?.ToString(),
                        //            WIFIPASSWORD = reader["WIFI_PASSWORD"]?.ToString(),
                        //            C_LOGO = reader["C_LOGO"]?.ToString(),
                        //            QR_CODE = reader["QR_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QR_CODE"]),
                        //            P_WINDOW = reader["P_WINDOW"] == DBNull.Value ? 0 : Convert.ToInt32(reader["P_WINDOW"]),
                        //            KOT_PRINTER = reader["KOT_PRINTER"]?.ToString(),
                        //            STICKER_PRINTER = reader["STICKER_PRINTER"]?.ToString(),
                        //        };

                        //        MasterResult.Add(row);
                        //    }
                        //}

                    }
                }

                if (MasterResult != null && MasterResult.Count > 0)
                {
                    var record = MasterResult[0];
                    //double? totalValue = ;
                    var menu = (Menu)Menu.data;
                    double? TotalAmountTax = 0;
                    var TotalItemDiscount = MasterResult.Sum(x => x.DETAIL_DISC_AMT);
                    if (record.PAY_TYPE == "Bank")
                    {
                        TotalAmountTax = record.BANK_TAX_VALUE;
                    }
                    else if (record.PAY_TYPE == "Party")
                    {
                        TotalAmountTax = record.PARTY_TAX_VALUE;
                    }
                    else
                    {
                        TotalAmountTax = record.CASH_TAX_VALUE;
                    }
                    var viewModel = new SlipViewModel
                    {
                        Clogo = record.C_LOGO,
                        BName = record.BName,
                        BAddress = record.BAddress,
                        BPhone = record.BTel,
                        BNTN = record.BNTN,
                        CPC_CODE = record.CPC_CODE,
                        ReferenceNumber = record.VOUCHER_NO,
                        Date = record.V_DATE,
                        Time = DateTime.Now.ToShortTimeString(),
                        CustomerName = record.PartyName == "" || record.PartyName == null ? record.CNAME : record.PartyName,
                        ContactNumber = record.CellNo == "" || record.CellNo == null ? record.CMOB : record.CellNo,
                        Waitername = record.Waiter,
                        TableNum = record.TABLE,
                        SalesmanName = string.IsNullOrEmpty(record.Salesman) ? common.Username : record.Salesman,
                        UserName = common.Username,
                        RoundingDiscount = record.DISC_AMT,
                        DiscountPercentage = record.DISC_Per,
                        SER_CHARGES = record.SER_CHARGES,
                        Del_Charges = record.DEL_CHARGES,
                        Delivery = record.DEL,
                        TotalValue = record.NET_TOTAL,
                        Payments = record.NET_TOTAL,
                        CashPaid = record.CASH,
                        CashFormatted = string.Format("{0:N0}", record.CASH),
                        CashBack = record.CashBack,
                        Cash = record.CASH,
                        FName = currentLabel.C_NAME,
                        FTEL = currentLabel.TEL,
                        FWebsite = currentLabel.WEBSITE,
                        TAX_CASH = record.CASH_TAX,
                        BANK_TAX = record.BANK_TAX,
                        PARTY_TAX = record.PARTY_TAX,
                        TAXCASH_AMT = record.CASH_TAX_VALUE,
                        CASHTAX_AMT = record.CASH_TAX_VALUE,
                        BANKTAX_AMT = record.BANK_TAX_VALUE,
                        PARTYTAX_AMT = record.PARTY_TAX_VALUE,
                        //TotalAmountFormatted = string.Format("{0:N0}", Convert.ToInt32((record.NET_TOTAL - record.SETTLEMENT) + TotalAmountTax)),
                        TotalAmountFormatted = Convert.ToString(record.INVOICE_VALUE - TotalItemDiscount),
                        Bank = record.BANK,
                        BankFormatted = string.Format("{0:N0}", Convert.ToInt32(record.BANK)),
                        Party = record.PARTY,
                        PartyFormatted = string.Format("{0:N0}", Convert.ToInt32(record.PARTY)),
                        MENUTERMS = menu.MENU_TERMS,
                        SRBInvoiceId = record.SRB_INV,
                        Waiter = record.Waiter,
                        Table = record.TABLE,
                        Remarks = record.REMARKS,
                        billmode = record.BILL_MODE,
                        billStatus = record.BILL_STATUS,
                        Type = record.INV_STATUS,
                        DelDate = !string.IsNullOrWhiteSpace(record.DEL_DATE) ? record.DEL_DATE : "",
                        Advance = record.ADVANCE,
                        TotalAdvance = record.TOTAL_ADVANCE,
                        Balance = record.BALANCE,
                        Complete = record.COMPLETE,
                        FBLINK = record.FB_LINK,
                        INSTALINK = record.INSTA_LINK,
                        WEBLINK = record.WEB_LINK,
                        TIKTOKLINK = record.TIKTOK_LINK,
                        YOUTUBELINK = record.YOUTUBE_LINK,
                        WIFIPASS = record.WIFIPASSWORD,
                        WIFINAME = record.WIFINAME,
                        SRBSTATUS = record.API_STATUS,
                        ADVBANK = record.ADV_BANK,
                        PAYTYPE = record.PAY_TYPE,
                        Sett = record.SETTLEMENT,
                        GrossTotal = record.GROSS_TOTAL - TotalItemDiscount,
                        StickerLogo = record.STICKER_LOGO,
                        LocationShortName = record.LOCATION_SNAME,
                        QRCode = record.QR_CODE,
                        PWindow = record.P_WINDOW,
                        KotPrinter = record.KOT_PRINTER,
                        StickerPrinter = record.STICKER_PRINTER,
                        JobName = record.JobName,
                        PrintModeLabel = record.PRINT_MODE_LABLE,
                        P_PRINTER = record.P_PRINTER,


                        Items = MasterResult.Select(i => new SlipItemViewModel
                        {
                            ItemCode = i.ITEM_CODE,
                            Description = i.ITEM_NAME,
                            Remarks = i.REMARKS,
                            Quantity = i.QTY,
                            Price = i.RATE,
                            Discount = i.DISC,
                            Disc_AMt = i.DETAIL_DISC_AMT,
                            Amount = i.NET_AMT,
                            BARCODE = i.BARCODE,
                            JobName = i.JobName,
                            ItemTax = 0,
                            KotPrinter = i.KOT_PRINTER,
                            StickerPrinter = i.STICKER_PRINTER,
                            P_PRINTER = i.P_PRINTER,
                        }).ToList(),
                        itemDiscount = TotalItemDiscount
                    };
                    var totalQuantity = MasterResult.Sum(i => i.QTY);
                    var totalAmounts = MasterResult.Sum(i => i.NET_AMT);
                    var totalPrice = MasterResult.Sum(i => i.RATE * i.QTY);
                    var totalDiscount = MasterResult.Sum(i => i.DETAIL_DISC_AMT);
                    //var totalPrice = totalAmounts;
                    var ItemLenght = MasterResult.Count();

                    if (menuDetails.MD_ID == 28)
                    {
                        viewModel.TotalQuantity = totalQuantity;
                        viewModel.NoOfItems = ItemLenght;
                    }
                    if (menuDetails.MD_ID == 47 || menuDetails.MD_ID == 51)
                    {
                        viewModel.TotalQuantity = totalQuantity;
                        viewModel.NoOfItems = ItemLenght;
                        viewModel.Amount = totalAmounts;
                    }
                    else
                    {
                        viewModel.Items.Add(new SlipItemViewModel
                        {
                            Description = "# Of Items :",
                            Quantity = totalQuantity,
                            TotalPrice = "",
                            Disc_Per = "",
                            Discount = totalDiscount,
                            Price = totalPrice,
                            Amount = totalAmounts,
                            BARCODE = Convert.ToString(ItemLenght),
                        });
                    }
                    response.viewModel = viewModel;
                    response.data = menuDetails;
                    response.voucherNo = record.VOUCHER_NO;
                    response.msg = "";
                    response.msgType = 1;
                }
                else
                {
                    response.msg = "Record Not Saved";
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
        public MyHttpResponseMessage ExpensePrintReport(List<ExpensePrint> modelRecord, Info currentLabel, Branch currentBranch, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            POSTransactionReport masterData = new POSTransactionReport();
            CustomPOSTransactionForPrintReport reportData = new CustomPOSTransactionForPrintReport();
            try
            {
                var totalAmount = modelRecord.Sum(i => i.Amount);
                var viewModel = new SlipViewModel
                {
                    Clogo = currentCompany.C_LOGO,
                    BName = currentBranch.B_NAME,
                    BAddress = currentBranch.B_ADDRESS,
                    BPhone = currentBranch.B_TEL,
                    BNTN = currentBranch.B_NTN,
                    Date = DateTime.Now.ToString("dd-MM-yyyy"),
                    Time = DateTime.Now.ToShortTimeString(),
                    SalesmanName = common.Username,
                    FName = currentLabel.C_NAME,
                    FTEL = currentLabel.TEL,
                    FWebsite = currentLabel.WEBSITE,
                    TotalAmount = totalAmount,

                    Expense = modelRecord.Select(i => new ExpenseViewModel
                    {
                        Description = i.Descr,
                        Amount = i.Amount
                    }).ToList()



                };
                response.viewModel = viewModel;
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
        public MyHttpResponseMessage GetAllDiscount(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $"select CODE , DESCR , DISC from TBL_POS_DISC where BCODE = {common.Branch} AND DISC_EXP = 0 and DLT = 'T'  AND ASTATUS = 'Y' AND TDATE > '2023-11-20'";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            CODE = Convert.ToInt32(reader["CODE"]),
                            DESCR = Convert.ToString(reader["DESCR"]),
                            DISC = Convert.ToString(reader["DISC"])
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
            }
            return response;
        }
        public MyHttpResponseMessage GetAllWaiter(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $"SELECT GROUP_CODE , GROUP_NAME , GPIC FROM TBL_WAITER WHERE ASTATUS = 'Y' AND DLT = 'T'";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                            DESCR = Convert.ToString(reader["GROUP_NAME"]),
                            IPIC = Convert.ToString(reader["GPIC"])
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
            }
            return response;
        }
        public MyHttpResponseMessage GetAllTables(Common common, string Tran_Id)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                Tran_Id = Tran_Id == "undefined" ? "0" : Tran_Id;
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = @$"SELECT T.GROUP_CODE, 
                                               T.GROUP_NAME, 
                                               T.GPIC
                                        FROM TBL_TABLE T
                                        WHERE T.ASTATUS = 'Y' 
                                          AND T.DLT = 'T'
                                          AND NOT EXISTS (
                                                SELECT 1 
                                                FROM TBL_POS_MASTER M
                                                WHERE M.[TABLE] = T.GROUP_CODE 
                                                  AND M.COMPLETE = 0
                                                  AND M.DLT = 'T'
                                                  AND (
                                                       (M.BILL_STATUS <> 'P') 
                                                       AND M.TRAN_ID NOT IN ({Tran_Id}) 
                                                  )
                                                
                                          );";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                            DESCR = Convert.ToString(reader["GROUP_NAME"]),
                            IPIC = Convert.ToString(reader["GPIC"])
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
            }
            return response;
        }
        public MyHttpResponseMessage ExpenseRecord(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                //string Date = DateTime.Now.ToString("yyyy-MM-dd"); 
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $"  Select TRAN_ID , V_DATE ,Tc.ACT_NAME As ACT_CODE , VOUCHER_NO , DESCR , AMOUNT from TBL_POS_EXP Te\r\n Left Join TBL_CHART Tc on Tc.ACT_CODE = Te.ACT_CODE \r\n where Te.DLT = 'T' And BCODE = {common.Branch} AND PERIOD_ID = {common.Period} Order by TRAN_ID desc";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                            V_DATE = Convert.ToString(reader["V_DATE"]),
                            ACTCODE = Convert.ToString(reader["ACT_CODE"]),
                            DESCR = Convert.ToString(reader["DESCR"]),
                            VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                            AMOUNT = Convert.ToString(reader["AMOUNT"])
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
            }
            return response;
        }
        public MyHttpResponseMessage ExpenseRecordSave(ExpensePOSTransaction modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
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
                        bool Expense = true, IsNew = false;
                        decimal code = 0;
                        var ip = common.IPAddress;
                        var computer = common.ComputerName;
                        var postal = common.PostalCode;
                        var username = common.Username;
                        var branch = common.Branch;
                        var period = common.Period;
                        var menuID = common.MenuID;
                        string formattedDate = DateTime.ParseExact(modelRecord.Master.EXPDATE, "d-M-yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                        if (modelRecord.Master.TRAN_ID == null || modelRecord.Master.TRAN_ID == 0)
                        {
                            IsNew = true;
                            code = GenerateNextId(common, command, Expense);

                            if (code > 0)
                            {
                                modelRecord.Master.TRAN_ID = code;
                                voucherNo = GenerateVoucherNo(common, code, command, CommonService.GetDateTime("Pakistan Standard Time"), Expense);
                            }
                            //DateTime Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local);

                            query = $"INSERT INTO TBL_POS_EXP " +
                                    "(TRAN_ID, V_DATE, VOUCHER_NO, BOOK_TYPE, ACT_CODE, DESCR, AMOUNT, " +
                                    "BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                    "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT, CLOSING) " +
                                    "VALUES " +
                                    $"('{code}', '{formattedDate}', '{voucherNo}', '{modelRecord.Master.BOOK_TYPE}', '{modelRecord.Master.ACT_CODE}', " +
                                    $"'{modelRecord.Master.Descr}', '{modelRecord.Master.ExpAmount}', " +
                                    $"'{branch}', '{period}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                    $"'{computer}', '{ip}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                    $"'{computer}', '{ip}', '{postal}', '{postal}', '{menuID}', 'T', 0)";

                            command.CommandText = query;
                            command.ExecuteNonQuery();

                        }
                        else
                        {
                            query = $"UPDATE TBL_POS_EXP SET " +
                                    $"BOOK_TYPE = '{modelRecord.Master.BOOK_TYPE}', " +
                                    $"ACT_CODE = '{modelRecord.Master.ACT_CODE}', " +
                                    $"DESCR = '{modelRecord.Master.Descr}', " +
                                    $"AMOUNT = '{modelRecord.Master.ExpAmount}', " +
                                    $"BCODE = '{branch}', " +
                                    $"PERIOD_ID = '{period}', " +
                                    $"EDIT_USER_ID = '{username}', " +
                                    $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                    $"EDIT_COMPUTER_NAME = '{computer}', " +
                                    $"EDIT_IP_ADDRESS = '{ip}', " +
                                    $"EDIT_POSTALCODE = '{postal}' " +
                                    $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";

                            command.CommandText = query;
                            command.ExecuteNonQuery();


                        }
                        transaction.Commit();
                        response.data = "";
                        response.msgType = 1;
                        response.msg = IsNew ? "Record Added Successfully" : "Record Updated Successfully";
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
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }
        public MyHttpResponseMessage GetDynamicIcon()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $"select GROUP_CODE , GROUP_NAME , GPIC  from TBL_POS_STATUS where DLT = 'T'  AND ASTATUS = 'Y' order by GROUP_CODE";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                            GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                            GPIC = Convert.ToString(reader["GPIC"]),
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
            }
            return response;
        }
        public int GetAvailableStock(int? barcodeId, string period, Common common)
        {
            var periodInfo = _periodRepository.GetPeriodById(Convert.ToInt32(period));
            List<CustomPurchaseBillReport> jsonDataResult = new List<CustomPurchaseBillReport>();
            string StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
            string EndDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
            string query = $@"EXEC STKPROC 90,'{StartDate}','{EndDate}','{common.Branch}','{common.Period}','{barcodeId}',''";
            int stock = 0;
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    stock += reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BALANCE"]);
                }
            }
            return stock;
        }
        public Dictionary<int?, double?> PreviousStockInBill(string table, decimal? TRAN_ID, string period, string branch)
        {
            List<CurrentItemsInBill> jsonDataResult = new List<CurrentItemsInBill>();
            string query;
            Dictionary<int?, double?> stock = new();
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                query = $"SELECT TRAN_ID,DT_CODE,ITEM_CODE, QTY FROM {table} " +
                    " WHERE DLT = 'T' AND TRAN_ID = '" + TRAN_ID + "' AND BCODE = '" + branch + "' AND PERIOD_ID = '" + period + "'";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var row = new CurrentItemsInBill
                    {
                        ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                        QTY = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["QTY"])
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

    }
}