using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using ZXing;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class PurchaseOrderRepository : IPurchaseOrderRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }

        public PurchaseOrderRepository(IMenuRepository menuRepository, IBranchRepository branchRepository)
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
                string? table = string.Empty, detailTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {

                        string query = $@"SELECT 
                    M.TRAN_ID AS TRAN_ID, 
                    M.TRAN_ID AS CODE,
                    M.V_DATE,
                    M.VOUCHER_NO,
                    M.BTYPE,
                    M.COMM,
                    M.PARTY_CODE,
                    M.ACT_CODE,
                    M.REF,
                    M.REMARKS,
                    M.SCODE,
                    M.SACODE,
                    M.RINV_NO,
                    M.RINV_DATE,
                    M.BCODE,
                    M.PERIOD_ID,
                    M.ADD_USER_ID,
                    M.ADD_DATE,
                    M.ADD_COMPUTER_NAME,
                    M.ADD_IP_ADDRESS,
                    M.EDIT_USER_ID,
                    M.EDIT_DATE,
                    M.EDIT_COMPUTER_NAME,
                    M.EDIT_IP_ADDRESS,
                    PT.PARTY_NAME,
                    M.ADD_POSTALCODE,
                    M.EDIT_POSTALCODE, 
                    CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS 
                  FROM {table} M 
                  LEFT OUTER JOIN TBL_PARTY_TYPES PT 
                    ON PT.PARTY_CODE = M.PARTY_CODE 
                    AND PT.ACT_CODE = M.ACT_CODE 
                  WHERE M.DLT = 'T' 
                    AND M.BCODE = '{common.Branch}' 
                    AND M.PERIOD_ID = '{common.Period}' 
                  ORDER BY M.TRAN_ID DESC";


                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                                CODE = reader["CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CODE"]),
                                ASTATUS = reader["ASTATUS"] == DBNull.Value ? "" : Convert.ToString(reader["ASTATUS"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? "" : Convert.ToString(reader["VOUCHER_NO"]),

                                PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                REF = reader["REF"] == DBNull.Value ? "" : Convert.ToString(reader["REF"]),
                                BTYPE = reader["BTYPE"] == DBNull.Value ? "" : Convert.ToString(reader["BTYPE"]),
                                REMARKS = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),

                                SCODE = reader["SCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SCODE"]),
                                SACODE = reader["SACODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SACODE"]),
                                ADD_USER_ID = reader["ADD_USER_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ADD_USER_ID"]),
                                ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ADD_COMPUTER_NAME"]),

                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"] == DBNull.Value ? "" : Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = reader["EDIT_USER_ID"] == DBNull.Value ? "" : Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"] == DBNull.Value ? "" : Convert.ToString(reader["EDIT_IP_ADDRESS"]),

                                PARTY_NAME = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"] == DBNull.Value ? "" : Convert.ToString(reader["ADD_POSTALCODE"]),
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"] == DBNull.Value ? "" : Convert.ToString(reader["EDIT_POSTALCODE"]),
                                COMM = reader["COMM"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMM"]),

                                RINV_NO = reader["RINV_NO"] == DBNull.Value ? "" : Convert.ToString(reader["RINV_NO"]),
                                RINV_DATE = reader["RINV_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["RINV_DATE"]).ToString("yyyy-MM-dd"),

                                //ID = Convert.ToString(reader["TRAN_ID"]),
                                //ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                //V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                //VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? null : reader["VOUCHER_NO"].ToString(),
                                //PARTY_NAME = reader["PARTY_NAME"] == DBNull.Value ? null : reader["PARTY_NAME"].ToString(),
                                //WAREHOUSE = reader["WAREHOUSE"] == DBNull.Value ? null : reader["WAREHOUSE"].ToString(),
                                //SALEMAN = reader["SALESMAN"] == DBNull.Value ? null : reader["SALESMAN"].ToString(),
                                //REF = reader["REF"] == DBNull.Value ? null : reader["REF"].ToString(),
                                //REF_DATE = reader["REF_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["REF_DATE"]).ToString("yyyy-MM-dd"),
                                //TERMS = reader["TERMS"] == DBNull.Value ? null : reader["TERMS"].ToString(),
                                //COMM = reader["COMM"] == DBNull.Value ? null : reader["COMM"].ToString(),
                                //CURRENCY = reader["CURRENCY"] == DBNull.Value ? null : reader["CURRENCY"].ToString(),
                                //CRATE = reader["CRATE"] == DBNull.Value ? null : reader["CRATE"].ToString(),
                                //REMARKS = reader["REMARKS"] == DBNull.Value ? null : reader["REMARKS"].ToString(),
                                //COMM_TYPE = reader["COMM_TYPE"] == DBNull.Value ? null : reader["COMM_TYPE"].ToString(),
                                //COMM_AMT = reader["COMM_AMT"] == DBNull.Value ? null : reader["COMM_AMT"].ToString(),
                                //ORDER_TYPE = reader["ORDER_TYPE"] == DBNull.Value ? null : reader["ORDER_TYPE"].ToString(),
                                //TRANSPORT_TYPE = reader["TRANSPORT_TYPE"] == DBNull.Value ? null : reader["TRANSPORT_TYPE"].ToString(),
                                //DEL_DATE = reader["DEL_DATE"] == DBNull.Value ? null : reader["DEL_DATE"].ToString(),
                                //EDIT_USER_ID = reader["EDIT_USER_ID"] == DBNull.Value ? null : reader["EDIT_USER_ID"].ToString(),
                                //EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
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

        public MyHttpResponseMessage GetPurchaseOrderByCode(int code, Common common)
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
                        string query = $@"SELECT 
                    TRAN_ID, V_DATE, VOUCHER_NO, BTYPE, DOC, COMM, 
                    COMM_AMT, COMM_VAL, DISC, TERMS, PARTY_CODE, ACT_CODE, 
                    CURR_CODE, CRATE, REF, DC_NO, RINV_NO, RINV_DATE, 
                    REMARKS, SCODE, SACODE, ASTATUS 
                  FROM {table} 
                  WHERE DLT = 'T' 
                    AND BCODE = '{common.Branch}' 
                    AND PERIOD_ID = '{common.Period}' 
                    AND TRAN_ID = '{code}'";

                        SqlCommand sqlCommand = new SqlCommand(query, connection);
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = reader["VOUCHER_NO"] == DBNull.Value ? "" : Convert.ToString(reader["VOUCHER_NO"]),
                                BTYPE = reader["BTYPE"] == DBNull.Value ? "" : Convert.ToString(reader["BTYPE"]),
                                DOC = reader["DOC"] == DBNull.Value ? "" : Convert.ToString(reader["DOC"]),
                                COMM = reader["COMM"] == DBNull.Value ? 0 : Convert.ToDouble(reader["COMM"]),
                                COMM_AMT = reader["COMM_AMT"] == DBNull.Value ? "" : Convert.ToString(reader["COMM_AMT"]),
                                COMM_VAL = reader["COMM_VAL"] == DBNull.Value ? 0 : Convert.ToDouble(reader["COMM_VAL"]),
                                DISC = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDouble(reader["DISC"]),
                                TERMS = reader["TERMS"] == DBNull.Value ? "" : Convert.ToString(reader["TERMS"]),
                                PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                CURR_CODE = reader["CURR_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CURR_CODE"]),
                                CRATE = reader["CRATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CRATE"]),
                                REF = reader["REF"] == DBNull.Value ? "" : Convert.ToString(reader["REF"]),
                                DC_NO = reader["DC_NO"] == DBNull.Value ? "" : Convert.ToString(reader["DC_NO"]),
                                RINV_NO = reader["RINV_NO"] == DBNull.Value ? "" : Convert.ToString(reader["RINV_NO"]),
                                RINV_DATE = reader["RINV_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["RINV_DATE"]).ToString("yyyy-MM-dd"),
                                REMARKS = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
                                SCODE = reader["SCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SCODE"]),
                                SACODE = reader["SACODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SACODE"]),
                                ASTATUS = reader["ASTATUS"] == DBNull.Value ? "" : Convert.ToString(reader["ASTATUS"]),

                                



                                // Dates

                                //ID = Convert.ToString(reader["TRAN_ID"]),
                                //ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                //V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                //VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                ////DEP = Convert.ToString(reader["DEP"]),
                                //REF = Convert.ToString(reader["REF"]),
                                //POD = Convert.ToString(reader["POD"]),
                                //TRANSPORT_TYPE = Convert.ToString(reader["TRANSPORT_TYPE"]),
                                //DOC = Convert.ToString(reader["DOC"]),
                                //REMARKS = Convert.ToString(reader["REMARKS"]),
                                //PARTY_CODE = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",
                                //ORDER_TYPE = Convert.ToString(reader["ORDER_TYPE"]),
                                //TERMS = Convert.ToString(reader["TERMS"]),
                                //SCODE = $"{Convert.ToString(reader["SCODE"])}{Convert.ToString(reader["SACODE"])}",
                                //COMM_AMT = Convert.ToString(reader["COMM_AMT"]),
                                //COMM = Convert.ToString(reader["COMM"]),
                                //COMM_TYPE = Convert.ToString(reader["COMM_TYPE"]),
                                //REF_DATE = reader["REF_DATE"] == DBNull.Value || Convert.ToDateTime(reader["REF_DATE"]) == new DateTime(1900, 1, 1) ? null : Convert.ToDateTime(reader["REF_DATE"]).ToString("yyyy-MM-dd"),
                                //CURR_CODE = Convert.ToString(reader["CURR_CODE"]),
                                //CRATE = Convert.ToString(reader["CRATE"]),
                                //WAREHOUSE = Convert.ToString(reader["WAREHOUSE"]),
                                //DEL_DATE = reader["DEL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DEL_DATE"]).ToString("yyyy-MM-dd"),

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

        public MyHttpResponseMessage GetPurchaseOrderDetailByCode(int code, Common common)
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
                        //string query = "SELECT DT_CODE, ITEM_CODE, QTY," +
                        //               "UNIT, QTY2, WEIGHT, BAL_QTY, DEL_DATE, DT_DESC," +
                        //               "COLOR, SIZE, GRADE, " +
                        //               "PRIOIRTY, REQ_TYPE," +
                        //               "BCODE, PERIOD_ID, CHK, RATE, AMT, DISC, DISC_AMT, TAX, TAX_AMT, NET_AMT " +
                        //               $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                        //               $"AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";

                        string query = $@"SELECT TRAN_ID, PARTY_CODE, ACT_CODE, PACK, TOTAL_PACK, DT_CODE, ITEM_CODE, HS_CODE, QTY, UNIT, QTY2, BAL_QTY, RATE, WEIGHT, PACK, TOTAL_PACK, AMT, DISC, DISC_AMT, TAX,
                                        TAX_AMT, ADV, ADV_AMT, NET_AMT, DT_DESC, COLOR, SIZE, GRADE, WAREHOUSE, DEL_DATE, DUE_DATE, DUE_DAYS, VEH, PICK_ID, PICK_ID_D 
                                          FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}' 
                                          ORDER BY DT_CODE DESC";

                        SqlCommand sqlCommand = new SqlCommand(query, connection);
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                // Integers / IDs
                                TRAN_ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                                ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                UNIT = reader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UNIT"]),
                                DT_CODE = reader["DT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DT_CODE"]),
                                DUE_DAYS = reader["DUE_DAYS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DUE_DAYS"]),

                                // Double / Numeric (Amounts & Qty)
                                QTY = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["QTY"]),
                                PACK = reader["PACK"] == DBNull.Value ? 0 : Convert.ToDouble(reader["PACK"]),
                                TOTAL_PACK = reader["TOTAL_PACK"] == DBNull.Value ? 0 : Convert.ToDouble(reader["TOTAL_PACK"]),
                                RATE = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["RATE"]),
                                AMT = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["AMT"]),
                                NET_AMT = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["NET_AMT"]),
                                WEIGHT = reader["WEIGHT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["WEIGHT"]),
                                DISC = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDouble(reader["DISC"]),
                                DISC_AMT = reader["DISC_AMT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["DISC_AMT"]),
                                TAX = reader["TAX"] == DBNull.Value ? 0 : Convert.ToDouble(reader["TAX"]),
                                TAX_AMT = reader["TAX_AMT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["TAX_AMT"]),
                                ADV = reader["ADV"] == DBNull.Value ? 0 : Convert.ToDouble(reader["ADV"]),
                                ADV_AMT = reader["ADV_AMT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["ADV_AMT"]),
                                BAL_QTY = reader["BAL_QTY"] == DBNull.Value ? 0 : Convert.ToDouble(reader["BAL_QTY"]),

                                // Strings
                                DT_DESC = reader["DT_DESC"] == DBNull.Value ? "" : Convert.ToString(reader["DT_DESC"]),
                                HS_CODE = reader["HS_CODE"] == DBNull.Value ? "" : Convert.ToString(reader["HS_CODE"]),
                                COLOR = reader["COLOR"] == DBNull.Value ? "" : Convert.ToString(reader["COLOR"]),
                                SIZE = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                GRADE = reader["GRADE"] == DBNull.Value ? "" : Convert.ToString(reader["GRADE"]),
                                WAREHOUSE = reader["WAREHOUSE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["WAREHOUSE"]),
                                VEH = reader["VEH"] == DBNull.Value ? "" : Convert.ToString(reader["VEH"]),

                                // Dates
                                DEL_DATE = reader["DEL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DEL_DATE"]).ToString("yyyy-MM-dd"),
                                DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),

                                // Special IDs
                                PICK_ID = reader["PICK_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PICK_ID"]),
                                PICK_ID_D = reader["PICK_ID_D"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PICK_ID_D"])

                                //DT_CODE = reader["DT_CODE"] == DBNull.Value ? "" : Convert.ToString(reader["DT_CODE"]),
                                //ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                //QTY = reader["QTY"] == DBNull.Value ? "" :  Convert.ToString(reader["QTY"]),
                                //UNIT = reader["UNIT"] == DBNull.Value ? 0 :  Convert.ToInt32(reader["UNIT"]),
                                //QTY2 = reader["QTY2"] == DBNull.Value ? "" : Convert.ToString(reader["QTY2"]),
                                //BAL_QTY = reader["BAL_QTY"] == DBNull.Value ? "" : Convert.ToString(reader["BAL_QTY"]),
                                //DT_DESC = reader["DT_DESC"] == DBNull.Value ? "" : Convert.ToString(reader["DT_DESC"]),
                                //COLOR = reader["COLOR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COLOR"]),
                                //SIZE = reader["SIZE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE"]),
                                //GRADE = reader["GRADE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GRADE"]),
                                //PRIORITY = reader["PRIOIRTY"] == DBNull.Value ? "" : Convert.ToString(reader["PRIOIRTY"]),
                                //REQ_TYPE = reader["REQ_TYPE"] == DBNull.Value ? "" : Convert.ToString(reader["REQ_TYPE"]),
                                //RATE = reader["RATE"] == DBNull.Value ? "" : Convert.ToString(reader["RATE"]),
                                //WEIGHT = reader["WEIGHT"] == DBNull.Value ? "" : Convert.ToString(reader["WEIGHT"]),
                                //AMT = reader["AMT"] == DBNull.Value ? "" : Convert.ToString(reader["AMT"]),
                                //DISC = reader["DISC"] == DBNull.Value ? "" : Convert.ToString(reader["DISC"]),
                                //DISC_AMT = reader["DISC_AMT"] == DBNull.Value ? "" : Convert.ToString(reader["DISC_AMT"]),
                                //TAX = reader["TAX"] == DBNull.Value ? "" : Convert.ToString(reader["TAX"]),
                                //TAX_AMT = reader["TAX_AMT"] == DBNull.Value ? "" : Convert.ToString(reader["TAX_AMT"]),
                                //NET_AMT = reader["NET_AMT"] == DBNull.Value ? "" : Convert.ToString(reader["NET_AMT"]),
                                //CHK = reader["CHK"] == DBNull.Value ? "" : Convert.ToString(reader["CHK"]),
                                //POD = Convert.ToString(reader["POD"]),
                                //ORDER_NO = Convert.ToString(reader["ORDER_NO"]),
                                //PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                                //ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                                //SHIP_DATE = (reader["SHIP_DATE"] == DBNull.Value || Convert.ToDateTime(reader["SHIP_DATE"]) <= new DateTime(1900, 1, 2)) ? "" : Convert.ToDateTime(reader["SHIP_DATE"]).ToString("yyyy-MM-dd"),
                                //DEL_DATE = (reader["DEL_DATE"] == DBNull.Value || Convert.ToDateTime(reader["DEL_DATE"]) <= new DateTime(1900, 1, 2)) ? "" : Convert.ToDateTime(reader["DEL_DATE"]).ToString("yyyy-MM-dd"),
                                //CHK1 = Convert.ToString(reader["CHK"]) == "1" ? true : false,
                                //PARTY_DDL = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}",

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

        public MyHttpResponseMessage Save(CustomPurchaseOrder modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty;
                string pickDetail = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickDetail = menu.PICK_TABLE_DETAIL;
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
                            var salesmanInformation = partiesData.ToList().Where(p => p.customizedKey == modelRecord.Master.SCODE).FirstOrDefault();

                            if (partyInformation != null)
                            {
                                modelRecord.Master.PARTY_CODE = Convert.ToString(partyInformation.key);
                                modelRecord.Master.ACT_CODE = partyInformation.accountCode;
                            }

                            if (salesmanInformation != null)
                            {
                                modelRecord.Master.SCODE = Convert.ToString(salesmanInformation.key);
                                modelRecord.Master.SACODE = salesmanInformation.accountCode;
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
                                        "(TRAN_ID, V_DATE, VOUCHER_NO, PARTY_CODE, ACT_CODE, DOC, " +
                                        "TERMS, SCODE, SACODE, DC_NO, RINV_NO, RINV_DATE," +
                                        "COMM_AMT, COMM, COMM_VAL, REF, BTYPE, DISC, CURR_CODE, " +
                                        "CRATE, REMARKS, BCODE, PERIOD_ID," +
                                        "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME," +
                                        "ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE," +
                                        "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE," +
                                        "EDIT_POSTALCODE, ASTATUS, MENU_ID, DLT)" +
                                        "VALUES" +
                                        "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.PARTY_CODE + "','" + modelRecord.Master.ACT_CODE + "','" + modelRecord.Master.DOC + "'," +
                                        "'" + modelRecord.Master.TERMS + "','" + modelRecord.Master.SCODE + "','" + modelRecord.Master.SACODE  + "','" + modelRecord.Master.DC_NO + "','" + modelRecord.Master.RINV_NO + "','" + modelRecord.Master.RINV_DATE + "'," +
                                        "'" + modelRecord.Master.COMM_AMT + "','" + modelRecord.Master.COMM + "','" + modelRecord.Master.COMM_VAL + "','" + modelRecord.Master.REF + "','" + modelRecord.Master.BTYPE + "','" + modelRecord.Master.DISC + "','" + modelRecord.Master.CURR_CODE + "'," +
                                        "'" + modelRecord.Master.CRATE + "','" + modelRecord.Master.REMARKS + "','" + branch + "','" + period + "'," +
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
                                                DOC = '" + modelRecord.Master.DOC + @"',
                                                TERMS = '" + modelRecord.Master.TERMS + @"',
                                                DC_NO = '" + modelRecord.Master.DC_NO + @"',
                                                DISC = '" + modelRecord.Master.DISC + @"',
                                                RINV_NO = '" + modelRecord.Master.RINV_NO + @"',
                                                RINV_DATE = '" + modelRecord.Master.RINV_DATE + @"',
                                                SCODE = '" + modelRecord.Master.SCODE + @"',
                                                SACODE = '" + modelRecord.Master.SACODE + @"',
                                                COMM_AMT = '" + modelRecord.Master.COMM_AMT + @"',
                                                COMM_VAL = '" + modelRecord.Master.COMM_VAL + @"',
                                                COMM = '" + modelRecord.Master.COMM + @"',
                                                REF = '" + modelRecord.Master.REF + @"',
                                                CURR_CODE = '" + modelRecord.Master.CURR_CODE + @"',
                                                CRATE = '" + modelRecord.Master.CRATE + @"',
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
                                    if (item.PICK_ID_D != null && item.PICK_ID_D > 0)
                                    {
                                        string checkQtyQuery = $@"SELECT ISNULL(QTY, 0) FROM {pickDetail} WHERE DT_CODE = '{item.PICK_ID_D}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";

                                        command.CommandText = checkQtyQuery;
                                        object result = command.ExecuteScalar();
                                        double originalQty = result != null ? Convert.ToDouble(result) : 0;

                                        if (item.QTY > originalQty)
                                        {
                                            isDetailAdded = false;
                                            response.msg = $"You Cannot add qty more then the pick qty.";
                                            break; 
                                        }
                                    }

                                    if (item.DT_CODE == null || item.DT_CODE == 0)
                                    {
                                        int detailCode = GenerateNextDetailId(common, command);
                                        if (detailCode > 0)
                                        {
                                            detailQuery = $"INSERT INTO {detailTable}" +
                                                           "(TRAN_ID,DT_CODE,ITEM_CODE,QTY," +
                                                           "UNIT,WEIGHT,QTY2,BAL_QTY, PACK, TOTAL_PACK, DT_DESC," +
                                                           "COLOR,SIZE,GRADE,RATE, HS_CODE, AMT, DISC, DISC_AMT, TAX, TAX_AMT, ADV, ADV_AMT, NET_AMT, DEL_DATE, WAREHOUSE," +
                                                           "BCODE, DUE_DATE, DUE_DAYS, VEH," +
                                                           "PERIOD_ID,ADD_USER_ID,ADD_DATE," +
                                                           "ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                                                           "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                                           "ADD_POSTALCODE,EDIT_POSTALCODE," +
                                                           "MENU_ID,DLT, PICK_ID, PICK_ID_D, CHK)" +
                                                           "VALUES" +
                                                           "('" + modelRecord.Master.TRAN_ID + "','" + detailCode + "','" + item.ITEM_CODE + "','" + item.QTY + "'," +
                                                           "'" + item.UNIT + "','" + item.WEIGHT + "','" + item.QTY2 + "','" + item.BAL_QTY + "','" + item.PACK + "','" + item.TOTAL_PACK + "','" + item.DT_DESC + "'," +
                                                           "'" + item.COLOR + "','" + item.SIZE + "','" + item.GRADE + "','" + item.RATE + "','" + item.HS_CODE + "'," +
                                                           "'" + item.AMT + "','" + item.DISC + "','" + item.DISC_AMT + "','" + item.TAX + "','" + item.TAX_AMT + "','" + item.ADV + "','" + item.ADV_AMT + "','" + item.NET_AMT + "','" + item.DEL_DATE + "','" + item.WAREHOUSE + "'," +
                                                           "'" + branch + "','" + item.DUE_DATE + "','" + item.DUE_DAYS + "','" + item.VEH + "'," +
                                                           "'" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                                           "'" + Computer + "','" + Ip + "','" + username + "'," +
                                                           "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                                           "'" + Postal + "','" + Postal + "','" + menuID + "','T','" + item.PICK_ID + "','" + item.PICK_ID_D + "','" + item.CHK + "')";
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
                                                        QTY2 = '" + item.QTY2 + @"',
                                                        BAL_QTY = '" + item.BAL_QTY + @"',
                                                        PACK = '" + item.PACK + @"',
                                                        TOTAL_PACK = '" + item.TOTAL_PACK + @"',
                                                        UNIT = '" + item.UNIT + @"',
                                                        RATE = '" + item.RATE + @"',
                                                        AMT = '" + item.AMT + @"',
                                                        DISC = '"+ item.DISC + @"',
                                                        DISC_AMT = '"+ item.DISC_AMT + @"',
                                                        TAX = '"+ item.TAX + @"',
                                                        TAX_AMT = '"+ item.TAX_AMT + @"',
                                                        ADV = '" + item.ADV + @"',
                                                        ADV_AMT = '" + item.ADV_AMT + @"',
                                                        NET_AMT = '"+ item.NET_AMT + @"',
                                                        WAREHOUSE = '" + item.WAREHOUSE + @"',
                                                        DT_DESC = '" + item.DT_DESC + @"',
                                                        DUE_DATE = '" + item.DUE_DATE + @"',
                                                        DUE_DAYS = '" + item.DUE_DAYS + @"',
                                                        VEH = '" + item.VEH + @"',
                                                        COLOR = '" + item.COLOR + @"',
                                                        DEL_DATE = '" + item.DEL_DATE + @"',
                                                        SIZE = '" + item.SIZE + @"',
                                                        WEIGHT = '" + item.WEIGHT + @"',
                                                        GRADE = '" + item.GRADE + @"',
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

        public MyHttpResponseMessage GetPickDataByParty(int partyCode, int actCode, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                MyHttpResponseMessage Menu = this._menuRepository.GetMenu(common.MenuID);
                string table = string.Empty;
                string detailTable = string.Empty;
                string pickTable = string.Empty;
                string pickDetailTable = string.Empty;
                string pickType = string.Empty;
                if (Menu.data != null)
                {
                    Menu menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickTable = menu.PICK_TABLE_MASTER;
                    pickDetailTable = menu.PICK_TABLE_DETAIL;
                    pickType = menu.PICK_TYPE;
                }

                if (pickType == "MPO")
                {
                    if (!string.IsNullOrWhiteSpace(table))
                    {
                        List<object> jsonDataResult3 = new List<object>();
                        using (SqlConnection connection3 = new SqlConnection(new SQLService().getconnstring()))
                        {
                            string query = $@"SELECT 
                                                A.TRAN_ID, A.V_DATE AS LB_DATE, A.VOUCHER_NO, A.TERMS,
                                                A.PARTY_CODE, A.CURR_CODE, A.CRATE, I.HS_CODE, A.ACT_CODE, AC.PARTY_NAME AS PARTY_NAME, A.REF,
                                                A.BTYPE, A.DISC AS DISC_M, A.LOCAL_CHARGES, A.DOC, A.COMM_VAL, A.RINV_NO, A.RINV_DATE,


-- Issue carton
                    ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {pickDetailTable} WHERE DT_CODE = B.DT_CODE), 0) AS ISSUE_CRTN,
                    -- Received carton
                    ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {detailTable} D
                            LEFT OUTER JOIN {table} M ON D.TRAN_ID = M.TRAN_ID
                            WHERE D.PICK_ID_D = B.DT_CODE AND D.DLT = 'T' AND M.DLT = 'T'), 0) AS R_CRTN,
                    -- Balance carton
                    ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {pickDetailTable} WHERE DT_CODE = B.DT_CODE), 0) - 
                    ISNULL((SELECT SUM(ISNULL(QTY,0)) FROM {detailTable} D
                            LEFT OUTER JOIN {table} M ON D.TRAN_ID = M.TRAN_ID
                            WHERE D.PICK_ID_D = B.DT_CODE AND D.DLT = 'T' AND M.DLT = 'T'), 0) AS B_CRTN,


                                                -- Issue Qty
                                                ISNULL((SELECT SUM(ISNULL(TOTAL_PACK,0)) FROM {pickDetailTable} WHERE DT_CODE = B.DT_CODE), 0) AS ISSUE_QTY,
                                                -- Returned Qty
                                                ISNULL((SELECT SUM(ISNULL(TOTAL_PACK,0)) FROM {detailTable} D
                                                        LEFT OUTER JOIN {table} M ON D.TRAN_ID = M.TRAN_ID
                                                        WHERE D.PICK_ID_D = B.DT_CODE AND D.DLT = 'T' AND M.DLT = 'T'), 0) AS R_QTY,
                                                -- Balance Qty
                                                ISNULL((SELECT SUM(ISNULL(TOTAL_PACK,0)) FROM {pickDetailTable} WHERE DT_CODE = B.DT_CODE), 0) - 
                                                ISNULL((SELECT SUM(ISNULL(TOTAL_PACK,0)) FROM {detailTable} D
                                                        LEFT OUTER JOIN {table} M ON D.TRAN_ID = M.TRAN_ID
                                                        WHERE D.PICK_ID_D = B.DT_CODE AND D.DLT = 'T' AND M.DLT = 'T'), 0) AS B_QTY,
                                                U.GROUP_CODE AS UNIT, U.GROUP_NAME AS UNIT_NAME, B.RATE, B.AMT,
                                                B.PACK, B.QTY, B.WEIGHT, W.CODE AS WAREHOUSE_CODE, W.DESCR AS WAREHOUSE_NAME,
                                                B.DT_CODE, B.DISC, B.DISC_AMT, B.ADV, B.ADV_AMT, B.TAX, B.TAX_AMT, G.GROUP_CODE AS GRADE, G.GROUP_NAME AS GRADE_NAME, B.NET_AMT,
                                                CL.GROUP_NAME AS COLOR, SL.GROUP_NAME AS SIZE, A.COMM, A.COMM_AMT,
                                                CL.GROUP_CODE AS COLOR_ID, SL.GROUP_CODE AS SIZE_ID, A.SCODE, A.SACODE, B.ITEM_CODE, B.DEL_DATE, B.DUE_DATE, B.DUE_DAYS, I.ITEM_NAME,
                                                MB.MENU_PAGE, MB.MENU_PARENT_CODE, A.MENU_ID
                                            FROM {pickTable} A
                                            LEFT OUTER JOIN {pickDetailTable} B ON B.TRAN_ID = A.TRAN_ID AND B.DLT = 'T' AND B.BCODE = A.BCODE AND B.PERIOD_ID = A.PERIOD_ID
                                            LEFT OUTER JOIN {detailTable} VC ON VC.PICK_ID_D = B.DT_CODE AND VC.BCODE = B.BCODE AND VC.DLT = 'T' AND VC.PERIOD_ID = B.PERIOD_ID
                                            LEFT OUTER JOIN {table} SBM ON VC.TRAN_ID = SBM.TRAN_ID AND SBM.DLT = 'T' 
                                            LEFT OUTER JOIN TBL_PARTY_TYPES AC ON AC.PARTY_CODE = A.PARTY_CODE AND AC.ACT_CODE = A.ACT_CODE
                                            LEFT OUTER JOIN TBL_COLOR CL ON CL.GROUP_CODE = B.COLOR
                                            LEFT OUTER JOIN TBL_SIZE SL ON SL.GROUP_CODE = B.SIZE
                                            LEFT OUTER JOIN TBL_UNIT U ON U.GROUP_CODE = B.UNIT
                                            LEFT OUTER JOIN TBL_ITEMSMASTER I ON I.ITEM_CODE = B.ITEM_CODE
                                            LEFT OUTER JOIN TBL_WAREHOUSE W ON W.CODE = B.WAREHOUSE
                                            LEFT OUTER JOIN TBL_GRADE G ON G.GROUP_CODE = B.GRADE
                                            LEFT OUTER JOIN TBL_MENU_BUILDER MB ON MB.ID = A.MENU_ID
                                            WHERE A.BCODE = '{common.Branch}' 
                                              AND A.PERIOD_ID = '{common.Period}' 
                                              AND AC.PARTY_CODE = '{partyCode}' 
                                              AND AC.ACT_CODE = '{actCode}' 
                                              AND A.DLT = 'T' AND A.ASTATUS = 'Y' AND B.DLT = 'T'
                                            GROUP BY 
                                                A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, A.TERMS, A.PARTY_CODE, A.CURR_CODE, A.CRATE, I.HS_CODE,
                                                A.BTYPE, A.DISC, A.LOCAL_CHARGES, A.DOC, A.COMM_VAL, A.RINV_NO, A.RINV_DATE,
                                                A.ACT_CODE, AC.PARTY_NAME, A.REF, U.GROUP_CODE, U.GROUP_NAME, B.RATE, B.AMT,
                                                B.DT_CODE, B.DISC, B.DISC_AMT, B.ADV, B.ADV_AMT, B.TAX, B.TAX_AMT, G.GROUP_CODE, G.GROUP_NAME, B.NET_AMT,
                                                CL.GROUP_NAME, SL.GROUP_NAME, A.COMM, A.COMM_AMT,
                                                CL.GROUP_CODE, SL.GROUP_CODE, A.SCODE, A.SACODE, B.ITEM_CODE,
                                                B.PACK, B.QTY, B.WEIGHT, W.CODE, W.DESCR, 
                                                B.DEL_DATE, B.DUE_DATE, B.DUE_DAYS, I.ITEM_NAME, MB.MENU_PAGE, MB.MENU_PARENT_CODE, A.MENU_ID, VC.PICK_ID_D
                                            HAVING 
                                                ISNULL((SELECT SUM(ISNULL(TOTAL_PACK,0)) FROM {pickDetailTable} WHERE DT_CODE = B.DT_CODE), 0) - 
                                                ISNULL((SELECT SUM(ISNULL(TOTAL_PACK,0)) FROM {detailTable} D
                                                LEFT OUTER JOIN {table} M ON D.TRAN_ID = M.TRAN_ID
                                                WHERE D.PICK_ID_D = B.DT_CODE AND D.DLT = 'T' AND M.DLT = 'T'), 0) > 0";

                            SqlCommand sqlCommand3 = new SqlCommand(query, connection3);
                            connection3.Open();
                            SqlDataReader reader3 = sqlCommand3.ExecuteReader();
                            while (reader3.Read())
                            {
                                var row3 = new
                                {
                                    ID = Convert.ToString(reader3["TRAN_ID"]),
                                    LB_DATE = ((reader3["LB_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader3["LB_DATE"]).ToString("dd-MM-yyyy")),
                                    RINV_DATE = ((reader3["RINV_DATE"] == DBNull.Value || Convert.ToDateTime(reader3["RINV_DATE"]) == new DateTime(1900, 1, 1)) ? null : Convert.ToDateTime(reader3["RINV_DATE"]).ToString("yyyy-MM-dd")),
                                    DEL_DATE = ((reader3["DEL_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader3["DEL_DATE"]).ToString("yyyy-MM-dd")),
                                    DUE_DATE = ((reader3["DUE_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader3["DUE_DATE"]).ToString("yyyy-MM-dd")),
                                    VOUCHER_NO = ((reader3["VOUCHER_NO"] == DBNull.Value) ? "" : Convert.ToString(reader3["VOUCHER_NO"])),
                                    PARTY_CODE = ((reader3["PARTY_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["PARTY_CODE"])),
                                    SPARTY_CODE = ((reader3["SCODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["SCODE"])),
                                    ACT_CODE = ((reader3["ACT_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["ACT_CODE"])),
                                    SACT_CODE = ((reader3["SACODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["SACODE"])),
                                    PARTY_DDL = Convert.ToString(reader3["PARTY_CODE"]) + Convert.ToString(reader3["ACT_CODE"]),
                                    PARTY_NAME = ((reader3["PARTY_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader3["PARTY_NAME"])),
                                    REF = ((reader3["REF"] == DBNull.Value) ? "" : Convert.ToString(reader3["REF"])),
                                    RINV_NO = ((reader3["RINV_NO"] == DBNull.Value) ? "" : Convert.ToString(reader3["RINV_NO"])),
                                    HS_CODE = ((reader3["HS_CODE"] == DBNull.Value) ? "" : Convert.ToString(reader3["HS_CODE"])),
                                    IQTY = ((reader3["ISSUE_QTY"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["ISSUE_QTY"])),
                                    TOTAL_PACK = ((reader3["B_QTY"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["B_QTY"])),
                                    RQTY = ((reader3["R_QTY"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["R_QTY"])),
                                    UNIT = ((reader3["UNIT"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["UNIT"])),
                                    UNIT_NAME = ((reader3["UNIT_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader3["UNIT_NAME"])),
                                    RATE = ((reader3["RATE"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["RATE"])),
                                    LOCAL_CHARGES = ((reader3["LOCAL_CHARGES"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["LOCAL_CHARGES"])),
                                    DISC_M = ((reader3["DISC_M"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["DISC_M"])),
                                    COMM_VAL = ((reader3["COMM_VAL"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["COMM_VAL"])),
                                    DISC = ((reader3["DISC"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["DISC"])),
                                    DISC_AMT = ((reader3["DISC_AMT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["DISC_AMT"])),
                                    ADV = ((reader3["ADV"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["ADV"])),
                                    ADV_AMT = ((reader3["ADV_AMT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["ADV_AMT"])),
                                    TAX = ((reader3["TAX"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["TAX"])),
                                    TAX_AMT = ((reader3["TAX_AMT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["TAX_AMT"])),
                                    PACK = ((reader3["PACK"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["PACK"])),

                                    QTY = ((reader3["B_CRTN"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["B_CRTN"])),

                                    CURR_CODE = ((reader3["CURR_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["CURR_CODE"])),
                                    CRATE = ((reader3["CRATE"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["CRATE"])),
                                    DUE_DAYS = ((reader3["DUE_DAYS"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["DUE_DAYS"])),
                                    WEIGHT = ((reader3["WEIGHT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["WEIGHT"])),
                                    AMT = ((reader3["AMT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["AMT"])),
                                    COLOR_NAME = ((reader3["COLOR"] == DBNull.Value) ? "" : Convert.ToString(reader3["COLOR"])),
                                    SIZE_NAME = ((reader3["SIZE"] == DBNull.Value) ? "" : Convert.ToString(reader3["SIZE"])),
                                    DOC = ((reader3["DOC"] == DBNull.Value) ? "" : Convert.ToString(reader3["DOC"])),
                                    COLOR = ((reader3["COLOR_ID"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["COLOR_ID"])),
                                    SIZE = ((reader3["SIZE_ID"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["SIZE_ID"])),
                                    PICK_ID_D = ((reader3["DT_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["DT_CODE"])),
                                    ITEM_CODE = ((reader3["ITEM_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["ITEM_CODE"])),
                                    ITEM_NAME = ((reader3["ITEM_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader3["ITEM_NAME"])),
                                    WAREHOUSE = ((reader3["WAREHOUSE_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["WAREHOUSE_CODE"])),
                                    WAREHOUSE_NAME = ((reader3["WAREHOUSE_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader3["WAREHOUSE_NAME"])),
                                    TERMS = ((reader3["TERMS"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["TERMS"])),
                                    GRADE = ((reader3["GRADE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader3["GRADE"])),
                                    GRADE_NAME = ((reader3["GRADE_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader3["GRADE_NAME"])),
                                    COMM_AMT = ((reader3["COMM_AMT"] == DBNull.Value) ? "" : Convert.ToString(reader3["COMM_AMT"])),
                                    BTYPE = ((reader3["BTYPE"] == DBNull.Value) ? "" : Convert.ToString(reader3["BTYPE"])),
                                    COMM = ((reader3["COMM"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader3["COMM"])),
                                    LINK = string.Concat(new string[]
                                    {
                                        "/",
                                        Convert.ToString(reader3["MENU_PAGE"]),
                                        "?MOID=",
                                        Convert.ToString(reader3["MENU_PARENT_CODE"]),
                                        "&Code=",
                                        Convert.ToString(reader3["MENU_ID"])
                                    })
                                };
                                jsonDataResult3.Add(row3);
                            }
                            reader3.Close();
                        }

                        response.data = jsonDataResult3;
                        response.msg = "";
                        response.msgType = 1;
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
                    _catchMessage = _catchMessage + "<br/>" + ex.InnerException.Message;
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
				string? tableDetail = string.Empty;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE1;
                    tableDetail = menu.TABLE2;
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

                            string checkQuery = $@"SELECT D.DT_CODE
                                                        FROM TBL_PO_MASTER M
                                                        LEFT OUTER JOIN TBL_PO_DETAIL D ON M.TRAN_ID = D.TRAN_ID
                                                        WHERE M.TRAN_ID = 37 
                                                          AND M.BCODE = 1 
                                                          AND M.PERIOD_ID = 1
                                                          AND D.DT_CODE IN (
                                                                SELECT G.PICK_ID 
                                                                FROM TBL_GRN_DETAIL G
                                                                JOIN TBL_GRN_MASTER GM ON G.TRAN_ID = GM.TRAN_ID
                                                                WHERE GM.DLT <> 'F')";

                            SqlCommand checkCmd = new SqlCommand(checkQuery, connection);

                            int relatedCount = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (relatedCount > 0)
                            {
                                response.msgType = 2;
                                response.msg = "You cannot delete this entry, record is used in next form.";
                                return response;
                            }

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

                PurchaseOrder purchaseOrder = new PurchaseOrder();
                List<PurchaseOrderDetail> PurchaseOrderDetailList = new List<PurchaseOrderDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        purchaseOrder = new PurchaseOrder
                        {
                            V_DATE = record.V_DATE,
                            PARTY_CODE = Convert.ToString(reader["PARTY_CODE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            SCODE = Convert.ToString(reader["SCODE"]),
                            SACODE = Convert.ToInt32(reader["SACODE"]),
                            COMM = Convert.ToDouble(reader["COMM"]),
                            REF = Convert.ToString(reader["REF"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            BTYPE = Convert.ToString(reader["BTYPE"]),
                            BCODE = new int?(Convert.ToInt32(reader["BCODE"])),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            DISC = Convert.ToString(reader["DISC"]),
                            DOC = Convert.ToString(reader["DOC"]),
                            CURR_CODE = Convert.ToInt32(reader["CURR_CODE"]),
                            CRATE = Convert.ToDouble(reader["CRATE"]),
                            RINV_NO = Convert.ToString(reader["RINV_NO"]),
                            DC_NO = Convert.ToString(reader["DC_NO"]),
                            RINV_DATE = new DateTime?(Convert.ToDateTime(reader["RINV_DATE"])),
                            TERMS = Convert.ToInt32(reader["TERMS"]),
                            COMM_AMT = Convert.ToString(reader["COMM_AMT"]),



                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new PurchaseOrderDetail
                        {
                            ITEM_CODE = new int?(Convert.ToInt32(detail_Reader["ITEM_CODE"])),
                            QTY = new double?(Convert.ToDouble(detail_Reader["QTY"])),
                            UNIT = new int?(Convert.ToInt32(detail_Reader["UNIT"])),
                            QTY2 = new double?(Convert.ToDouble(detail_Reader["QTY2"])),
                            BAL_QTY = new double?(Convert.ToDouble(detail_Reader["BAL_QTY"])),
                            RATE = new double?(Convert.ToDouble(detail_Reader["RATE"])),
                            AMT = new double?(Convert.ToDouble(detail_Reader["AMT"])),
                            DISC = new double?(Convert.ToDouble(detail_Reader["DISC"])),
                            DISC_AMT = new double?(Convert.ToDouble(detail_Reader["DISC_AMT"])),
                            TAX = new double?(Convert.ToDouble(detail_Reader["TAX"])),
                            TAX_AMT = new double?(Convert.ToDouble(detail_Reader["TAX_AMT"])),
                            ADV = new double?(Convert.ToDouble(detail_Reader["ADV"])),
                            ADV_AMT = new double?(Convert.ToDouble(detail_Reader["ADV_AMT"])),
                            NET_AMT = new double?(Convert.ToDouble(detail_Reader["NET_AMT"])),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            COLOR = new int?(Convert.ToInt32(detail_Reader["COLOR"])),
                            SIZE = new int?(Convert.ToInt32(detail_Reader["SIZE"])),
                            GRADE = new int?(Convert.ToInt32(detail_Reader["GRADE"])),
                            WAREHOUSE = new int?(Convert.ToInt32(detail_Reader["WAREHOUSE"])),
                            DEL_DATE = new DateTime?(Convert.ToDateTime(detail_Reader["DEL_DATE"])),
                            DUE_DATE = new DateTime?(Convert.ToDateTime(detail_Reader["DUE_DATE"])),
                            DUE_DAYS = new int?(Convert.ToInt32(detail_Reader["DUE_DAYS"])),
                            VEH = Convert.ToString(detail_Reader["VEH"]),
                            CHK = new int?((detail_Reader["CHK"] == DBNull.Value) ? 0 : Convert.ToInt32(detail_Reader["CHK"])),
                            PICK_ID = new int?(Convert.ToInt32(detail_Reader["PICK_ID"])),
                            PICK_ID_D = new int?((detail_Reader["PICK_ID_D"] == DBNull.Value) ? 0 : Convert.ToInt32(detail_Reader["PICK_ID_D"])),
                            PARTY_CODE = detail_Reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["PARTY_CODE"]),
                            ACT_CODE = detail_Reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["ACT_CODE"]),
                            HS_CODE = detail_Reader["HS_CODE"] == DBNull.Value ? "" : Convert.ToString(detail_Reader["HS_CODE"]),
                            WEIGHT = detail_Reader["WEIGHT"] == DBNull.Value ? 0 : Convert.ToDouble(detail_Reader["WEIGHT"]),
                            PACK = detail_Reader["PACK"] == DBNull.Value ? 0 : Convert.ToDouble(detail_Reader["PACK"]),
                            TOTAL_PACK = detail_Reader["TOTAL_PACK"] == DBNull.Value ? 0 : Convert.ToDouble(detail_Reader["TOTAL_PACK"]),

                        };
                        PurchaseOrderDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customRequisition = new CustomPurchaseOrder
                {
                    Master = purchaseOrder,
                    Detail = PurchaseOrderDetailList
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


        public MyHttpResponseMessage DeletePurchaseOrderDetailByCode(int code, Common common)
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
                            //getting tran_id
                            string tranIdQuery = $@"SELECT isnull(COUNT(*),0) FROM TBL_GRN_DETAIL WHERE PICK_ID = {code} AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                            SqlCommand checkCmd = new SqlCommand(tranIdQuery, connection);
                            var count = Convert.ToInt32(checkCmd.ExecuteScalar());

                            if (count > 0)
                            {
                                response.msgType = 2;
                                response.msg = "You cannot delete this entry, record is used in next form.";
                                return response;
                            }


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

        //public MyHttpResponseMessage GetDataForReport(PurchaseOrderRDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    PurchaseOrderRDLCReport masterData = new PurchaseOrderRDLCReport();
        //    CustomPurchaseOrderForPrintReport reportData = new CustomPurchaseOrderForPrintReport();
        //    var Menu = _menuRepository.GetMenu(common.MenuID);
        //    string? table = string.Empty, detailTable = string.Empty;
        //    if (Menu.data != null)
        //    {
        //        var menu = (Menu)Menu.data;
        //        table = menu.TABLE1;
        //        detailTable = menu.TABLE2;
        //    }
        //    try
        //    {
        //        string topQuery = "", query = "";
        //        if (menuDetails.MD_ID == 32)
        //        {
        //            query = @$"SELECT 
        //                        M.V_DATE,m.VOUCHER_NO,PT.PARTY_NAME,M.ORDER_TYPE,CONVERT(NVARCHAR(20),M.TERMS)+' Days'  as Terms,
        //                        M.REF,D.DEL_DATE,M.REMARKS,
        //                        IM.ITEM_NAME,D.BAL_QTY,U.GROUP_NAME AS UNIT,D.RATE,D.AMT,D.DISC,D.DISC_AMT,D.TAX,D.TAX_AMT,D.NET_AMT, G.GROUP_NAME AS GRADE
        //                        FROM {table} M
        //                        LEFT OUTER JOIN {detailTable} D
        //                        ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID
        //                        LEFT OUTER JOIN TBL_ITEMSMASTER IM
        //                        ON IM.ITEM_CODE = D.ITEM_CODE
        //                        LEFT OUTER JOIN TBL_UNIT U
        //                        ON U.GROUP_CODE = D.UNIT
        //                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
        //                        ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE
        //                        LEFT OUTER JOIN TBL_CURRENCY CR
        //                        ON CR.CODE = M.CURR_CODE
        //                        LEFT OUTER JOIN TBL_GRADE G
        //                        ON G.GROUP_CODE = D.GRADE
        //                        WHERE M.TRAN_ID = {modelRecord.TRAN_ID} AND M.BCODE = {common.Branch} AND M.PERIOD_ID = {common.Period}
        //                        AND M.ASTATUS = 'Y' AND M.DLT = 'T' AND D.DLT = 'T'";
        //        }

        //        masterData.COMPANY_NAME = currentCompany.C_NAME;
        //        masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
        //        masterData.COMPANY_PHONE = currentCompany.C_TEL;
        //        masterData.COMPANY_LOGO = currentCompany.C_LOGO;
        //        masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
        //        masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
        //        masterData.SIG1 = $"{menuDetails.MENU_SIG1}";
        //        masterData.SIG2 = $"{menuDetails.MENU_SIG2}";
        //        masterData.SIG3 = $"{menuDetails.MENU_SIG3}";
        //        masterData.SIG4 = $"{menuDetails.MENU_SIG4}";
        //        masterData.MENU_TERMS = $"{menuDetails.MENU_TERMS}";

        //        using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //        {
        //            SqlCommand command = new SqlCommand(query, connection);
        //            connection.Open();
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                DataRow dataRow = dataTable.NewRow();
        //                dataRow["Item"] = Convert.ToString(reader["ITEM_NAME"]);
        //                dataRow["Unit"] = Convert.ToString(reader["UNIT"]);
        //                dataRow["Qty"] = reader["BAL_QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BAL_QTY"]);
        //                dataRow["Rate"] = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]);
        //                dataRow["Amt"] = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);
        //                dataRow["NetAmt"] = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]);
        //                dataRow["Tax"] = reader["TAX"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TAX"]);
        //                dataRow["TaxAmt"] = reader["TAX_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TAX_AMT"]);
        //                dataRow["Disc"] = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]);
        //                dataRow["DiscAmt"] = reader["DISC_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC_AMT"]);

        //                dataRow["Voucher"] = Convert.ToString(reader["VOUCHER_NO"]);
        //                dataRow["Date"] = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
        //                dataRow["Party"] = Convert.ToString(reader["PARTY_NAME"]);
        //                dataRow["OrderType"] = Convert.ToString(reader["ORDER_TYPE"]);
        //                dataRow["Terms"] = Convert.ToString(reader["Terms"]);
        //                dataRow["Grade"] = Convert.ToString(reader["GRADE"]);
        //                dataRow["Reference"] = Convert.ToString(reader["REF"]);
        //                dataRow["Remarks"] = Convert.ToString(reader["REMARKS"]);
        //                dataRow["DeliveryDate"] = reader["DEL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["DEL_DATE"]).ToString("dd-MM-yyyy");
        //                dataTable.Rows.Add(dataRow);
        //            }
        //            reader.Close();
        //        }

        //        reportData.Master = masterData;
        //        reportData.Detail = dataTable;
        //        response.data = reportData;
        //        response.msg = "";
        //        response.msgType = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        response.msg = _catchMessage;
        //        response.msgType = 2;
        //    }
        //    return response;
        //}

        public MyHttpResponseMessage GetDataForReport(PurchaseOrderRDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            PurchaseOrderRDLCReport masterData = new PurchaseOrderRDLCReport();
            CustomPurchaseOrderForPrintReport reportData = new CustomPurchaseOrderForPrintReport();
            var Menu = _menuRepository.GetMenu(common.MenuID);
            string? table = string.Empty, detailTable = string.Empty, dCType = string.Empty, pickMaster = string.Empty, pickDetail = string.Empty;
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                table = menu.TABLE1;
                detailTable = menu.TABLE2;
                pickMaster = menu.PICK_TABLE_MASTER;
                pickDetail = menu.PICK_TABLE_DETAIL;
                dCType = menu.DCTYPE;
            }
            try
            {
                string query = "";

                if (menuDetails.REPORT_NAME == "PurchaseOrder")
                {


                    //query = $@"EXEC PROC_PRINT '{menuDetails.REPORT_NAME}','{table}','{detailTable}','{pickMaster}','{pickDetail}','{common.Branch}','{common.Period}','{modelRecord.TRAN_ID}','{common.Username}',''";
                    query = $@"EXEC PROC_PRINT '{table}','{detailTable}','','','{common.Branch}','{common.Period}','{modelRecord.TRAN_ID}','','','{menuDetails.REPORT_NAME}'";

                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader5 = command.ExecuteReader();
                        if (reader5.Read())
                        {
                            masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                            masterData.RCOMPANY_LOGO = currentCompany.RC_LOGO;
                            masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                            masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                            masterData.COMPANY_NAME = ((reader5["C_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["C_NAME"]));
                            masterData.B_NAME = ((reader5["B_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_NAME"]));
                            masterData.B_TERMS = ((reader5["B_TERMS"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_TERMS"]));
                            masterData.COMPANY_ADDRESS = ((reader5["B_ADDRESS"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_ADDRESS"]));
                            masterData.COMPANY_PHONE = ((reader5["B_TEL"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_TEL"]));
                            masterData.B_WEBSITE = ((reader5["B_WEBSITE"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_WEBSITE"]));
                            masterData.EMAIL = ((reader5["EMAIL"] == DBNull.Value) ? "" : Convert.ToString(reader5["EMAIL"]));
                            masterData.B_GST = ((reader5["B_GST"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_GST"]));
                            masterData.B_NTN = ((reader5["B_NTN"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_NTN"]));
                            masterData.SIG1 = ((reader5["MENU_SIG1"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG1"]));
                            masterData.SIG2 = ((reader5["MENU_SIG2"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG2"]));
                            masterData.SIG3 = ((reader5["MENU_SIG3"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG3"]));
                            masterData.SIG4 = ((reader5["MENU_SIG4"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG4"]));
                            masterData.MENU_TERMS = ((reader5["MENU_TERMS"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_TERMS"]));
                            masterData.INVOICE_NUMBER = Convert.ToString(reader5["VOUCHER_NO"]);
                            masterData.DATE = ((reader5["V_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader5["V_DATE"]).ToString("dd-MM-yyyy"));
                            masterData.DEL_DATE = ((reader5["DEL_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader5["DEL_DATE"]).ToString("dd-MM-yyyy"));
                            masterData.USER = common.Username;
                            masterData.STATUS = Convert.ToString(reader5["ASTATUS"]);
                            masterData.BT_CUSTOMER = ((reader5["BT_CUSTOMER"] == DBNull.Value) ? "" : Convert.ToString(reader5["BT_CUSTOMER"]));
                            masterData.PARTY_NAME = ((reader5["PARTY_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["PARTY_NAME"]));
                            masterData.PADDRESS = ((reader5["PADDRESS"] == DBNull.Value) ? "" : Convert.ToString(reader5["PADDRESS"]));
                            masterData.REF = ((reader5["REF"] == DBNull.Value) ? "" : Convert.ToString(reader5["REF"]));
                            masterData.COMMENT = ((reader5["REMARKS"] == DBNull.Value) ? "" : Convert.ToString(reader5["REMARKS"]));
                            masterData.PNTN = ((reader5["PNTN"] == DBNull.Value) ? "" : Convert.ToString(reader5["PNTN"]));
                            masterData.RINV_NO = ((reader5["RINV_NO"] == DBNull.Value) ? "" : Convert.ToString(reader5["RINV_NO"]));
                        }
                        reader5.Close();
                    }
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader6 = command.ExecuteReader();
                        while (reader6.Read())
                        {
                            DataRow dataRow3 = dataTable.NewRow();
                            dataRow3["Item"] = ((reader6["ITEM"] == DBNull.Value) ? "" : Convert.ToString(reader6["ITEM"]));
                            dataRow3["Unit"] = ((reader6["UNIT"] == DBNull.Value) ? "" : Convert.ToString(reader6["UNIT"]));
                            dataRow3["Qty"] = ((reader6["QTY"] == DBNull.Value) ? "" : Convert.ToString(reader6["QTY"]));
                            dataRow3["Rate"] = ((reader6["RATE"] == DBNull.Value) ? "" : Convert.ToString(reader6["RATE"]));
                            dataRow3["Amt"] = ((reader6["AMT"] == DBNull.Value) ? "" : Convert.ToString(reader6["AMT"]));
                            dataRow3["NetAmt"] = ((reader6["NET_AMT"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["NET_AMT"]));
                            dataRow3["Tax"] = ((reader6["TAX"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["TAX"]));
                            dataRow3["TaxAmt"] = ((reader6["TAX_AMT"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["TAX_AMT"]));
                            dataRow3["Disc"] = ((reader6["DISC"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["DISC"]));
                            dataRow3["DiscAmt"] = ((reader6["DISC_AMT"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["DISC_AMT"]));
                            dataRow3["Pack"] = ((reader6["Pack"] == DBNull.Value) ? "" : Convert.ToString(reader6["PACK"]));
                            dataRow3["TotalPack"] = ((reader6["TOTAL_PACK"] == DBNull.Value) ? "" : Convert.ToString(reader6["TOTAL_PACK"]));
                            dataRow3["Weight"] = ((reader6["WEIGHT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader6["WEIGHT"]));
                            dataRow3["Adv"] = ((reader6["ADV"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["ADV"]));
                            dataRow3["AdvAmt"] = ((reader6["ADV_AMT"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["ADV_AMT"]));

                            dataRow3["Grade"] = ((reader6["GRADE"] == DBNull.Value) ? "" : Convert.ToString(reader6["GRADE"]));
                            dataRow3["Size"] = ((reader6["SIZE"] == DBNull.Value) ? "" : Convert.ToString(reader6["SIZE"]));
                            dataRow3["ExStax"] = ((reader6["EX_STAX"] == DBNull.Value) ? "" : Convert.ToString(reader6["EX_STAX"]));
                            dataTable.Rows.Add(dataRow3);
                        }
                        reader6.Close();
                    }
                }
                else if (menuDetails.REPORT_NAME == "PurchaseBill")
                {
                    query = $@"EXEC PROC_PRINT '{table}','{detailTable}','','','{common.Branch}','{common.Period}','{modelRecord.TRAN_ID}','','','{menuDetails.REPORT_NAME}'";

                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader5 = command.ExecuteReader();
                        if (reader5.Read())
                        {
                            masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                            masterData.RCOMPANY_LOGO = currentCompany.RC_LOGO;
                            masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                            masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                            masterData.COMPANY_NAME = ((reader5["C_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["C_NAME"]));
                            masterData.B_NAME = ((reader5["B_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_NAME"]));
                            masterData.B_TERMS = ((reader5["B_TERMS"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_TERMS"]));
                            masterData.COMPANY_ADDRESS = ((reader5["B_ADDRESS"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_ADDRESS"]));
                            masterData.COMPANY_PHONE = ((reader5["B_TEL"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_TEL"]));
                            masterData.B_WEBSITE = ((reader5["B_WEBSITE"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_WEBSITE"]));
                            masterData.EMAIL = ((reader5["EMAIL"] == DBNull.Value) ? "" : Convert.ToString(reader5["EMAIL"]));
                            masterData.B_GST = ((reader5["B_GST"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_GST"]));
                            masterData.B_NTN = ((reader5["B_NTN"] == DBNull.Value) ? "" : Convert.ToString(reader5["B_NTN"]));
                            masterData.SIG1 = ((reader5["MENU_SIG1"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG1"]));
                            masterData.SIG2 = ((reader5["MENU_SIG2"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG2"]));
                            masterData.SIG3 = ((reader5["MENU_SIG3"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG3"]));
                            masterData.SIG4 = ((reader5["MENU_SIG4"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_SIG4"]));
                            masterData.MENU_TERMS = ((reader5["MENU_TERMS"] == DBNull.Value) ? "" : Convert.ToString(reader5["MENU_TERMS"]));
                            masterData.INVOICE_NUMBER = Convert.ToString(reader5["VOUCHER_NO"]);
                            masterData.DATE = ((reader5["V_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader5["V_DATE"]).ToString("dd-MM-yyyy"));
                            masterData.DEL_DATE = ((reader5["DEL_DATE"] == DBNull.Value) ? null : Convert.ToDateTime(reader5["DEL_DATE"]).ToString("dd-MM-yyyy"));
                            masterData.USER = common.Username;
                            masterData.STATUS = Convert.ToString(reader5["ASTATUS"]);
                            masterData.BT_CUSTOMER = ((reader5["BT_CUSTOMER"] == DBNull.Value) ? "" : Convert.ToString(reader5["BT_CUSTOMER"]));
                            masterData.PARTY_NAME = ((reader5["PARTY_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader5["PARTY_NAME"]));
                            masterData.PADDRESS = ((reader5["PADDRESS"] == DBNull.Value) ? "" : Convert.ToString(reader5["PADDRESS"]));
                            masterData.REF = ((reader5["REF"] == DBNull.Value) ? "" : Convert.ToString(reader5["REF"]));
                            masterData.COMMENT = ((reader5["REMARKS"] == DBNull.Value) ? "" : Convert.ToString(reader5["REMARKS"]));
                            masterData.PNTN = ((reader5["PNTN"] == DBNull.Value) ? "" : Convert.ToString(reader5["PNTN"]));
                            masterData.RINV_NO = ((reader5["RINV_NO"] == DBNull.Value) ? "" : Convert.ToString(reader5["RINV_NO"]));
                        }
                        reader5.Close();
                    }
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader6 = command.ExecuteReader();
                        while (reader6.Read())
                        {
                            DataRow dataRow3 = dataTable.NewRow();
                            dataRow3["Item"] = ((reader6["ITEM"] == DBNull.Value) ? "" : Convert.ToString(reader6["ITEM"]));
                            dataRow3["Unit"] = ((reader6["UNIT"] == DBNull.Value) ? "" : Convert.ToString(reader6["UNIT"]));
                            dataRow3["Qty"] = ((reader6["QTY"] == DBNull.Value) ? "" : Convert.ToString(reader6["QTY"]));
                            dataRow3["Rate"] = ((reader6["RATE"] == DBNull.Value) ? "" : Convert.ToString(reader6["RATE"]));
                            dataRow3["Amt"] = ((reader6["AMT"] == DBNull.Value) ? "" : Convert.ToString(reader6["AMT"]));
                            dataRow3["NetAmt"] = ((reader6["NET_AMT"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["NET_AMT"]));
                            dataRow3["Tax"] = ((reader6["TAX"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["TAX"]));
                            dataRow3["TaxAmt"] = ((reader6["TAX_AMT"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["TAX_AMT"]));
                            dataRow3["Disc"] = ((reader6["DISC"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["DISC"]));
                            dataRow3["DiscAmt"] = ((reader6["DISC_AMT"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["DISC_AMT"]));
                            dataRow3["Pack"] = ((reader6["Pack"] == DBNull.Value) ? "" : Convert.ToString(reader6["PACK"]));
                            dataRow3["TotalPack"] = ((reader6["TOTAL_PACK"] == DBNull.Value) ? "" : Convert.ToString(reader6["TOTAL_PACK"]));
                            dataRow3["HsCode"] = ((reader6["HS_CODE"] == DBNull.Value) ? "" : Convert.ToString(reader6["HS_CODE"]));
                            dataRow3["Weight"] = ((reader6["WEIGHT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader6["WEIGHT"]));
                            dataRow3["Adv"] = ((reader6["ADV"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["ADV"]));
                            dataRow3["AdvAmt"] = ((reader6["ADV_AMT"] == DBNull.Value) ? 0m : Convert.ToDecimal(reader6["ADV_AMT"]));

                            dataRow3["Grade"] = ((reader6["GRADE"] == DBNull.Value) ? "" : Convert.ToString(reader6["GRADE"]));
                            dataRow3["Size"] = ((reader6["SIZE"] == DBNull.Value) ? "" : Convert.ToString(reader6["SIZE"]));
                            dataRow3["ExStax"] = ((reader6["EX_STAX"] == DBNull.Value) ? "" : Convert.ToString(reader6["EX_STAX"]));
                            dataRow3["PvoucherNo"] = ((reader6["PVOUCHER_NO"] == DBNull.Value) ? "" : Convert.ToString(reader6["PVOUCHER_NO"]));
                            dataRow3["PtranId"] = ((reader6["PTRAN_ID"] == DBNull.Value) ? "" : Convert.ToString(reader6["PTRAN_ID"]));
                            dataTable.Rows.Add(dataRow3);
                        }
                        reader6.Close();
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