using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Drawing;
using ZXing;
using ZXing.QrCode.Internal;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class SalesInvoiceReportRepository : ISalesInvoiceReportRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public SalesInvoiceReportRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetReportTypes(int menuID, string roleType, int? roleId)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = "TBL_REPORT_TYPES";
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "";
                    if (roleType == "A")
                    {
                        query = $"SELECT * FROM {table} " +
                                   "WHERE 1 = 1 " +
                                   $"AND M_ID = '{menuID}' " +
                                   "AND DLT = 'T' AND ASTATUS = 'Y' " +
                                   "ORDER BY SNO";
                    }
                    else
                    {
                        query = $"SELECT * FROM {table} " +
                                   "WHERE 1 = 1 " +
                                   $"AND M_ID = '{menuID}' " +
                                   $"AND DLT = 'T' AND ASTATUS = 'Y' AND R_ID IN (SELECT RMENU_ID from TBL_ROLE WHERE ROLE_ID = {roleId} AND MODULE_ID = 2) " +
                                   "ORDER BY SNO";
                    }
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            SNO = Convert.ToInt32(reader["SNO"]),
                            REPORT_NAME = Convert.ToString(reader["REPORT_NAME"]),
                            R_ID = Convert.ToInt32(reader["R_ID"]),
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

        public MyHttpResponseMessage GetReportData(SalesInvoiceReport report, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table1 = string.Empty, table2 = string.Empty, table3 = string.Empty, table4 = string.Empty, b_i = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table1 = menu.TABLE1;
                    table2 = menu.TABLE2;
                    table3 = menu.PICK_TABLE_MASTER;
                    table4 = menu.PICK_TABLE_DETAIL;
                    b_i = menu.B_I;
                }
                List<CustomSalesInvoiceReport> jsonDataResult = new List<CustomSalesInvoiceReport>();
                List<dynamic> jsonDetailDataResult = new List<dynamic>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    if (report.ReportID < 24 || report.ReportID == 100 || report.ReportID == 140)
                    {
                        string query = $"EXEC SUMMARY_BARCODE '{report.ReportID}','{report.FromDate.Value.ToString("yyyy-MM-dd")}','{report.ToDate.Value.ToString("yyyy-MM-dd")}','{common.Branch}','{common.Period}','{report.Item}','{report.PartyCode}','{report.ActCode}','{report.Group}','{table1}','{table2}','{table3}','{table4}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (report.ReportID == 18 || report.ReportID == 19 || report.ReportID == 20)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                    PartyName = Convert.ToString(reader["PARTY_NAME"]),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    ItemId = Convert.ToString(reader["ITEM_ID"]),
                                    ItemName = Convert.ToString(reader["ITEM_NAME"]),
                                    Color = Convert.ToString(reader["COLOR"]),
                                    Size = Convert.ToString(reader["SIZE"]),
                                    Barcode = Convert.ToString(reader["BARCODE"]),
                                    Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                    Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    MDisc_Amt = reader["MDISC_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MDISC_AMT"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 21)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                    PartyName = Convert.ToString(reader["PARTY_NAME"]),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    Disc = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    ReturnAmt = reader["RETURN_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RETURN_AMT"]),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                    ReturnDisc = reader["RETURN_DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RETURN_DISC"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 22)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    PartyName = Convert.ToString(reader["PARTY_NAME"]),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 23)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    Category = Convert.ToString(reader["CATEGORY"]),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 100)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VDATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VDATE"]).ToString("yyyy-MM-dd"),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    Desc = Convert.ToString(reader["DISC"]),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                    ReturnDisc = reader["RETURN_DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RETURN_DISC"]),
                                    ReturnAmt = reader["RETURN_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RETURN_AMT"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        if (report.ReportID == 140)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                    PartyName = Convert.ToString(reader["PARTY_NAME"]),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    ItemId = Convert.ToString(reader["ITEM_ID"]),
                                    ItemName = Convert.ToString(reader["ITEM_NAME"]),
                                    Color = Convert.ToString(reader["COLOR"]),
                                    Size = Convert.ToString(reader["SIZE"]),
                                    Barcode = Convert.ToString(reader["BARCODE"]),
                                    Region = reader["REGION"] == DBNull.Value ? "" : Convert.ToString(reader["REGION"]),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                    Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                    Pack = reader["PACK"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PACK"]),
                                    TotalPack = reader["TOTAL_PACK"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL_PACK"]),
                                    Weight = reader["WEIGHT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WEIGHT"]),
                                    Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),

                                    Disc = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]),
                                    MDisc_Amt = reader["MDISC_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["MDISC_AMT"]),
                                    Tax = reader["TAX"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TAX"]),
                                    TaxAmt = reader["TAX_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TAX_AMT"]),
                                    ADV_TAX = reader["ADV_TAX"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ADV_TAX"]),
                                    ADV_TAX_AMT = reader["ADV_TAX_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ADV_TAX_AMT"]),
                                    NetAmt = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]),


                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        reader.Close();
                    }
                    else if (report.ReportID >= 45 && report.ReportID <= 67 || report.ReportID == 125 || report.ReportID == 132 || report.ReportID == 133 || report.ReportID == 134 || report.ReportID == 135 || report.ReportID == 137 || report.ReportID == 87 || report.ReportID == 94 || report.ReportID == 138 || report.ReportID == 141)
                    {
                        string query = $"EXEC SUMMARY_POS '{report.ReportID}','{report.FromDate.Value.ToString("yyyy-MM-dd")}','{report.ToDate.Value.ToString("yyyy-MM-dd")}','{report.Branch}','{common.Period}','{report.Item}','{report.PartyCode}','{report.ActCode}','{report.Group}','{report.Category}','{report.SubCategory}', '{report.Barcode}','{report.Size}','{report.Color}','{b_i}','{report.Salesman}','{report.SActCode}'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (report.ReportID == 45)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemName = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    Remarks = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    POSQty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                    Amt = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 46)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    POSQty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                    Amt = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 47)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    POSQty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                    Amt = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 48)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    GroupName = reader["GROUP_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["GROUP_NAME"]),
                                    POSQty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                    Amt = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID <= 63)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    ItemName = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    Remarks = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    Color = reader["COLOR"] == DBNull.Value ? "" : Convert.ToString(reader["COLOR"]),
                                    Size = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                    Barcode = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                    POSQty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                    Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    Disc = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]),
                                    MDisc_Amt = reader["DISC_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC_AMT"]),
                                    NetAmt = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 64)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    POSQty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                    totalSales = reader["TOTAL_SALES"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL_SALES"]),
                                    Cash = reader["CASH"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CASH"]),
                                    CardType = reader["CARD_TYPE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CARD_TYPE"]),
                                    Party = reader["PARTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PARTY"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 65)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    ItemName = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    Remarks = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    Color = reader["COLOR"] == DBNull.Value ? "" : Convert.ToString(reader["COLOR"]),
                                    Size = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                    Barcode = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                    POSQty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                    Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    Disc = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]),
                                    MDisc_Amt = reader["DISC_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC_AMT"]),
                                    NetAmt = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]),
                                    Stock = reader["STOCK"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 66)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    ItemName = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    Size = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                    //Barcode = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                    POSQty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                    Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    Disc = reader["DISC_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC_AMT"]),
                                    NetAmt = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]),
                                    PbRate = reader["PB_RATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PB_RATE"]),
                                    PAmt = reader["PAMT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PAMT"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 67)
                        {


                            while (reader.Read())
                            {
                                decimal total = reader["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL"]);
                                decimal discPercent = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]);
                                decimal discAmt = (total * discPercent) / 100;
                                decimal tax = reader["TAX"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TAX"]);

                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    Total = total,
                                    Disc = discPercent,
                                    MDisc_Amt = discAmt,
                                    NetAmt = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]),
                                    PartyTax = tax,
                                    TaxAmt = (total - discAmt) * tax / 100,
                                    BName = Convert.ToString(reader["B_NAME"]),
                                    PartyName = Convert.ToString(reader["PARTY_NAME"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 87)
                        {


                            while (reader.Read())
                            {

                                var row = new CustomSalesInvoiceReport
                                {
                                    Mobile = reader["CMOB"] == DBNull.Value ? "" : Convert.ToString(reader["CMOB"]),
                                    CName = reader["CNAME"] == DBNull.Value ? "" : Convert.ToString(reader["CNAME"]),
                                    CAdd = reader["CADD"] == DBNull.Value ? "" : Convert.ToString(reader["CADD"]),
                                    Recv = reader["RECV"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RECV"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 94)
                        {


                            while (reader.Read())
                            {

                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    Mobile = reader["CMOB"] == DBNull.Value ? "" : Convert.ToString(reader["CMOB"]),
                                    CName = reader["CNAME"] == DBNull.Value ? "" : Convert.ToString(reader["CNAME"]),
                                    CAdd = reader["CADD"] == DBNull.Value ? "" : Convert.ToString(reader["CADD"]),
                                    Recv = reader["RECV"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RECV"]),
                                    Mode = reader["BILL_MODE"] == DBNull.Value ? "" : Convert.ToString(reader["BILL_MODE"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 125)
                        {
                            while (reader.Read())
                            {
                                var discPer = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]);
                                var taxPer = reader["TAX"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TAX"]);
                                var total = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);
                                var discAmt = (total * discPer) / 100;
                                var taxAmt = (total - discAmt) * taxPer / 100;

                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    PartyName = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
                                    Remarks = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
                                    ItemName2 = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                    Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                    Disc = discPer,
                                    MDisc_Amt = discAmt,
                                    Tax = taxPer,
                                    //TaxAmt = taxAmt,
                                    Amt = total,
                                    BillMode = reader["BILL_MODE"] == DBNull.Value ? "" : Convert.ToString(reader["BILL_MODE"]),
                                    Branch = reader["BRANCH"] == DBNull.Value ? "" : Convert.ToString(reader["BRANCH"]),
                                    //InclTax = total + taxAmt,

                                    TaxAmt = Math.Round(taxAmt, 2),
                                    InclTax = Math.Round(total + taxAmt, 2),

                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 132 || report.ReportID == 133)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["DEL_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DEL_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    Total = reader["TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL"]),
                                    Tax = reader["TAX"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TAX"]),
                                    Recv = reader["RECV"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RECV"]),
                                    BillMode = reader["PAY_TYPE"] == DBNull.Value ? "" : Convert.ToString(reader["PAY_TYPE"]),
                                    SRB_INV = reader["SRB_INV"] == DBNull.Value ? "" : Convert.ToString(reader["SRB_INV"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 134)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["DEL_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DEL_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    CName = Convert.ToString(reader["CNAME"]),
                                    CMOB = Convert.ToString(reader["CMOB"]),
                                    Salesman = Convert.ToString(reader["SALESMAN"]),
                                    Recv = reader["RECV"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RECV"]),
                                    Percentage = reader["PERCENTAGE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PERCENTAGE"]),
                                    Commission = reader["COMMISSION"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["COMMISSION"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 135)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["DEL_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DEL_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    Recv = reader["RECV"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RECV"]),
                                    CName = Convert.ToString(reader["CNAME"]),
                                    CMOB = Convert.ToString(reader["CMOB"]),
                                    Salesman = Convert.ToString(reader["PARTY_NAME"]),
                                    Percentage = reader["PERCENTAGE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PERCENTAGE"]),
                                    Commission = reader["COMMISSION"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["COMMISSION"]),
                                    Remarks = Convert.ToString(reader["REMARKS"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 137)
                        {
                            while (reader.Read())
                            {

                                var total = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);

                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    //DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),

                                    DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DUE_DATE"]).Year == 1900 ? "" : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),

                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    PartyName = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
                                    Remarks = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
                                    ItemName2 = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    Amt = total,
                                    Branch = reader["BRANCH"] == DBNull.Value ? "" : Convert.ToString(reader["BRANCH"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 138)
                        {
                            while (reader.Read())
                            {
                                var discPer = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]);
                                var taxPer = reader["TAX"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TAX"]);
                                var total = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);
                                var discAmt = (total * discPer) / 100;
                                var taxAmt = (total - discAmt) * taxPer / 100;

                                var row = new CustomSalesInvoiceReport
                                {
                                    vDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    PartyName = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
                                    WAITER = reader["WAITER"] == DBNull.Value ? "" : Convert.ToString(reader["WAITER"]),
                                    TABLE = reader["TABLE"] == DBNull.Value ? "" : Convert.ToString(reader["TABLE"]),
                                    Remarks = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
                                    ItemName2 = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                    Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                    Disc = discPer,
                                    MDisc_Amt = discAmt,
                                    Tax = taxPer,
                                    //TaxAmt = taxAmt,
                                    Amt = total,
                                    BillMode = reader["BILL_MODE"] == DBNull.Value ? "" : Convert.ToString(reader["BILL_MODE"]),
                                    Branch = reader["BRANCH"] == DBNull.Value ? "" : Convert.ToString(reader["BRANCH"]),
                                    //InclTax = total + taxAmt,

                                    TaxAmt = Math.Round(taxAmt, 2),
                                    InclTax = Math.Round(total + taxAmt, 2),

                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 141)
                        {
                            while (reader.Read())
                            {
                                //var discPer = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]);
                                //var taxPer = reader["TAX"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TAX"]);
                                //var total = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);
                                //var discAmt = (total * discPer) / 100;
                                //var taxAmt = (total - discAmt) * taxPer / 100;

                                var row = new CustomSalesInvoiceReport
                                {
                                    MONTH = reader["Month"] == DBNull.Value ? "" : Convert.ToString(reader["Month"]),
                                    TOTAL_QTY = reader["Total_Qty"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Total_Qty"]),
                                    TOTAL_R_PRICE = reader["TOTAL_R_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL_R_PRICE"]),
                                    TOTAL_DISC = reader["TOTAL_DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL_DISC"]),

                                    TOTAL_PRICE = reader["TOTAL_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL_PRICE"]),
                                    TOTAL_WS_PRICE = reader["TOTAL_WS_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL_WS_PRICE"]),
                                    STKT_WS_PRICE = reader["STKT_WS_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STKT_WS_PRICE"]),
                                    TOTAL_EXP = reader["TOTAL_EXP"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL_EXP"]),
                                    FACTORY_TRANSFER = reader["FACTORY_TRANSFER"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["FACTORY_TRANSFER"]),
                                    CASH_SALE = reader["CASH_SALE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CASH_SALE"]),
                                    BANK_W_CH = reader["BANK_W_CH"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BANK_W_CH"]),
                                    NET_PROFIT1 = reader["NET_PROFIT1"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_PROFIT1"]),
                                    TOTAL_COST = reader["TOTAL_COST"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TOTAL_COST"]),
                                    NET_PROFIT2 = reader["NET_PROFIT2"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_PROFIT2"]),

                                    //vDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    //VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    //PartyName = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
                                    //WAITER = reader["WAITER"] == DBNull.Value ? "" : Convert.ToString(reader["WAITER"]),
                                    //TABLE = reader["TABLE"] == DBNull.Value ? "" : Convert.ToString(reader["TABLE"]),
                                    //Remarks = reader["REMARKS"] == DBNull.Value ? "" : Convert.ToString(reader["REMARKS"]),
                                    //ItemName2 = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    //Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                    //Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                    //Disc = discPer,
                                    //MDisc_Amt = discAmt,
                                    //Tax = taxPer,
                                    ////TaxAmt = taxAmt,
                                    //Amt = total,
                                    //BillMode = reader["BILL_MODE"] == DBNull.Value ? "" : Convert.ToString(reader["BILL_MODE"]),
                                    //Branch = reader["BRANCH"] == DBNull.Value ? "" : Convert.ToString(reader["BRANCH"]),
                                    ////InclTax = total + taxAmt,

                                    //TaxAmt = Math.Round(taxAmt, 2),
                                    //InclTax = Math.Round(total + taxAmt, 2),

                                };
                                jsonDataResult.Add(row);
                            }
                        }
                    }
                    else if (report.ReportID == 74)
                    {
                        string query = @$"SELECT A.V_DATE, A.VOUCHER_NO as VOUCHER_NO,IT.ITEM_NAME AS ITEM_NAME,
                                          SZ.GROUP_NAME AS SIZE, BG.BARCODE,SUM(B.BAL_QTY) AS QTY, BG.RRATE AS RATE ,SUM(B.BAL_QTY * BG.RRATE) AS AMT,
                                          BG.SRATE AS WRATE,SUM(B.BAL_QTY * BG.SRATE) AS WAMT ,
                                          CASE WHEN A.AM = 'I' THEN 'Stock In' Else 'Stock Out' End AS BTRANSFER,CL.GROUP_NAME AS COLOR
                                          FROM TBL_STKA_MASTER A
                                          LEFT OUTER JOIN TBL_STKA_DETAIL B
                                          ON A.TRAN_ID = B.TRAN_ID AND A.BCODE = B.BCODE AND A.PERIOD_ID = B.PERIOD_ID
                                          LEFT JOIN TBL_MENU_BUILDER MB ON MB.ID = A.MENU_ID
                                          LEFT JOIN TBL_BARCODE BG ON BG.CODE = B.ITEM_CODE
                                          LEFT JOIN TBL_ITEMSMASTER IT ON IT.ITEM_CODE = CASE WHEN MB.B_I = 'I' THEN B.ITEM_CODE ELSE BG.ITEM_CODE END
                                          LEFT OUTER JOIN TBL_CATEGORY CT
                                          ON CT.GROUP_CODE = IT.CAT_CODE
                                          LEFT OUTER JOIN TBL_SUB_CATEGORY SCT
                                          ON SCT.GROUP_CODE = IT.SUB_CAT_CODE
                                          LEFT OUTER JOIN TBL_SIZE SZ
                                          ON SZ.GROUP_CODE = BG.SIZE
                                          LEFT OUTER JOIN TBL_COLOR CL
                                          ON CL.GROUP_CODE = BG.COLOR
                                          LEFT OUTER JOIN TBL_ITEMSGROUP IG
                                          ON IG.GROUP_CODE = IT.GROUP_CODE
                                          WHERE
                                          V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                          AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}'
                                          AND ('{report.Item}' = '' OR IT.ITEM_CODE = '{report.Item}')
                                          AND ('{report.Group}' = '' OR IG.GROUP_CODE = '{report.Group}')
                                          AND ('{report.Category}' = '' OR CT.GROUP_CODE = '{report.Category}')
                                          AND ('{report.SubCategory}' = '' OR SCT.GROUP_CODE = '{report.SubCategory}')
                                          AND ('{report.Barcode}' = '' OR BG.CODE = '{report.Barcode}')
                                          AND ('{report.Size}' = '' OR SZ.GROUP_CODE = '{report.Size}')
                                          AND ('{report.Color}' = '' OR CL.GROUP_CODE = '{report.Color}')
                                          GROUP BY
                                          A.V_DATE, A.VOUCHER_NO ,IT.ITEM_NAME ,IT.REMARKS, CT.GROUP_NAME ,SCT.GROUP_NAME ,
                                          SZ.GROUP_NAME ,BG.RRATE ,BG.SRATE,BG.BARCODE,CL.GROUP_NAME,A.AM";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomSalesInvoiceReport
                            {
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                ItemId = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                Size = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                Barcode = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                Qty = Convert.ToInt32(reader["Qty"]),
                                Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                WRate = reader["WRATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["WRATE"]),
                                WAmt = reader["WAMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WAMT"]),
                                BTransfer = reader["BTRANSFER"] == DBNull.Value ? "" : Convert.ToString(reader["BTRANSFER"]),
                                Color = reader["COLOR"] == DBNull.Value ? "" : Convert.ToString(reader["COLOR"])
                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID == 75)
                    {
                        string query = @$"SELECT IT.ITEM_NAME AS ITEM_NAME,CT.GROUP_NAME AS CATEGORY,
                                          SUM(B.BAL_QTY) AS QTY ,SUM(B.BAL_QTY * BG.RRATE) AS AMT,
										SUM(B.BAL_QTY * BG.SRATE) AS WAMT,
                                          round(SUM(B.BAL_QTY) /
                                          CASE
                                          WHEN (SELECT COUNT(ITEM_CODE) FROM TBL_BARCODE WHERE ITEM_CODE = IT.ITEM_CODE AND DLT = 'T') = 0 THEN 1
                                          ELSE (SELECT COUNT(ITEM_CODE) FROM TBL_BARCODE WHERE ITEM_CODE = IT.ITEM_CODE AND DLT = 'T')
                                          END,2)
                                          AS CSET
                                          FROM TBL_STKA_MASTER A
                                          LEFT OUTER JOIN TBL_STKA_DETAIL B
                                          ON A.TRAN_ID = B.TRAN_ID AND A.BCODE = B.BCODE AND A.PERIOD_ID = B.PERIOD_ID
                                          LEFT JOIN TBL_MENU_BUILDER MB ON MB.ID = A.MENU_ID
                                          LEFT JOIN TBL_BARCODE BG ON BG.CODE = B.ITEM_CODE
                                          LEFT JOIN TBL_ITEMSMASTER IT ON IT.ITEM_CODE = CASE WHEN MB.B_I = 'I' THEN B.ITEM_CODE ELSE BG.ITEM_CODE END
                                          LEFT OUTER JOIN TBL_CATEGORY CT
                                          ON CT.GROUP_CODE = IT.CAT_CODE
                                          LEFT OUTER JOIN TBL_SUB_CATEGORY SCT
                                          ON SCT.GROUP_CODE = IT.SUB_CAT_CODE
                                          LEFT OUTER JOIN TBL_SIZE SZ
                                          ON SZ.GROUP_CODE = BG.SIZE
                                          LEFT OUTER JOIN TBL_COLOR CL
                                          ON CL.GROUP_CODE = BG.COLOR
                                          LEFT OUTER JOIN TBL_ITEMSGROUP IG
                                          ON IG.GROUP_CODE = IT.GROUP_CODE
                                          WHERE
                                          V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                          AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}'
                                          AND ('{report.Item}' = '' OR IT.ITEM_CODE = '{report.Item}')
                                          AND ('{report.Group}' = '' OR IG.GROUP_CODE = '{report.Group}')
                                          AND ('{report.Category}' = '' OR CT.GROUP_CODE = '{report.Category}')
                                          AND ('{report.SubCategory}' = '' OR SCT.GROUP_CODE = '{report.SubCategory}')
                                          AND ('{report.Barcode}' = '' OR BG.CODE = '{report.Barcode}')
                                          AND ('{report.Size}' = '' OR SZ.GROUP_CODE = '{report.Size}')
                                          AND ('{report.Color}' = '' OR CL.GROUP_CODE = '{report.Color}')
                                          AND A.AM = 'I'
                                          GROUP BY
                                          IT.ITEM_NAME ,IT.REMARKS, CT.GROUP_NAME ,SCT.GROUP_NAME ,
                                          A.AM,IT.ITEM_CODE";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomSalesInvoiceReport
                            {
                                ItemId = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                Qty = Convert.ToInt32(reader["Qty"]),
                                Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                WAmt = reader["WAMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WAMT"]),
                                CSET = reader["CSET"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CSET"])
                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    //else if(report.ReportID == 87)
                    //{
                    //    string query = @$"select TC.CMOB,ISNULL(MAX(TC.CNAME),0) AS CNAME,
                    //                    ISNULL(MAX(TC.CADD),0) AS CADD,ISNULL(SUM(PM.RECV),0) AS RECV
                    //                    from TBL_POS_MASTER PM
                    //                    LEFT JOIN TBL_POS_CUS TC ON TC.TRAN_ID = PM.TRAN_ID AND TC.BCODE = PM.BCODE AND TC.PERIOD_ID = PM.PERIOD_ID
                    //                    WHERE TC.CMOB <> '' AND PM.BILL_STATUS = 'P' AND PM.DLT = 'T'
                    //                    AND PM.V_DATE <= '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                    //                    AND TC.CMOB = CASE WHEN '{report.Contact}' = '' THEN TC.CMOB ELSE '{report.Contact}' END 
                    //                    GROUP BY TC.CMOB 
                    //                    ORDER BY ISNULL(SUM(PM.RECV),0)  DESC";
                    //    SqlCommand command = new SqlCommand(query, connection);
                    //    connection.Open();
                    //    SqlDataReader reader = command.ExecuteReader();
                    //    while (reader.Read())
                    //    {
                    //        var row = new CustomSalesInvoiceReport
                    //        {
                    //            Mobile = reader["CMOB"] == DBNull.Value ? "" : Convert.ToString(reader["CMOB"]),
                    //            CName = reader["CNAME"] == DBNull.Value ? "" : Convert.ToString(reader["CNAME"]),
                    //            CAdd = reader["CADD"] == DBNull.Value ? "" : Convert.ToString(reader["CADD"]),
                    //            Recv = reader["RECV"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RECV"]),
                    //        };
                    //        jsonDataResult.Add(row);
                    //    }
                    //}
                    //else if(report.ReportID == 94)
                    //{
                    //    string query = @$"select PM.V_DATE,PM.VOUCHER_NO,TC.CMOB,ISNULL((TC.CNAME),0) AS CNAME,
                    //                    ISNULL((TC.CADD),0) AS CADD,ISNULL((PM.RECV),0) AS RECV,
                    //                    CASE WHEN PM.CACT_CODE <> 0 THEN 'Cash'
                    //                    WHEN PM.BACT_CODE <> 0 THEN 'Card'
                    //                    WHEN PM.PARTY <> 0 THEN 'Credit'
                    //                    END AS BILL_MODE
                    //                    from TBL_POS_MASTER PM
                    //                    LEFT JOIN TBL_POS_CUS TC ON TC.TRAN_ID = PM.TRAN_ID AND TC.BCODE = PM.BCODE AND TC.PERIOD_ID = PM.PERIOD_ID
                    //                    WHERE TC.CMOB <> '' AND PM.BILL_STATUS = 'P' AND PM.DLT = 'T'
                    //                    AND PM.V_DATE <= '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                    //                    AND TC.CMOB = CASE WHEN '{report.Contact}' = '' THEN TC.CMOB ELSE '{report.Contact}' END 
                    //                    ORDER BY PM.V_DATE DESC";

                    //    SqlCommand command = new SqlCommand(query, connection);
                    //    connection.Open();
                    //    SqlDataReader reader = command.ExecuteReader();
                    //    while (reader.Read())
                    //    {
                    //        var row = new CustomSalesInvoiceReport
                    //        {
                    //            VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                    //            VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                    //            Mobile = reader["CMOB"] == DBNull.Value ? "" : Convert.ToString(reader["CMOB"]),
                    //            CName = reader["CNAME"] == DBNull.Value ? "" : Convert.ToString(reader["CNAME"]),
                    //            CAdd = reader["CADD"] == DBNull.Value ? "" : Convert.ToString(reader["CADD"]),
                    //            Recv = reader["RECV"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RECV"]),
                    //            Mode = reader["BILL_MODE"] == DBNull.Value ? "" : Convert.ToString(reader["BILL_MODE"]),
                    //        };
                    //        jsonDataResult.Add(row);
                    //    }
                    //}
                    else if (report.ReportID == 76)
                    {
                        string query = @$"SELECT BR1.B_NAME, CH.ACT_NAME,V_DATE,DESCR,AMOUNT FROM TBL_POS_EXP E
                                        LEFT OUTER JOIN TBL_CHART CH
                                        ON CH.ACT_CODE = E.ACT_CODE
                                        LEFT OUTER JOIN TBL_BRANCH BR1
                                        ON BR1.BCODE = E.BCODE
                                        WHERE CLOSING = 1 AND E.DLT = 'T'
                                        AND E.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        AND ('{report.Branch}' = '' OR E.BCODE = '{report.Branch}')";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomSalesInvoiceReport
                            {
                                BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                ActName = reader["ACT_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ACT_NAME"]),
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                Desc = Convert.ToString(reader["DESCR"]),
                                Amt = reader["AMOUNT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMOUNT"]),
                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID == 117)
                    {
                        string query = @$"SELECT PM.TRAN_ID, DEL_DATE AS V_DATE, VOUCHER_NO, TC.CMOB, TC.CNAME, TC.CADD, NET_TOTAL, DEL_CHARGES, ADVANCE, (NET_TOTAL - ADVANCE) AS REMAINING, BILL_MODE, PM.TRAN_ID, PM.BCODE, PM.PERIOD_ID  
                                        FROM TBL_POS_MASTER PM
										LEFT JOIN TBL_POS_CUS TC ON TC.TRAN_ID = PM.TRAN_ID AND TC.BCODE = PM.BCODE AND TC.PERIOD_ID = PM.PERIOD_ID
                                        WHERE 
                                        PM.DLT = 'T' AND
                                        COMPLETE = 0 AND
                                        DEL_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}' AND PM.BCODE = '{report.Branch}' AND PM.PERIOD_ID = '{common.Period}'
                                        --AND CMOB = ''
                                        ";

                        string detailQuerry = @$"select IM.ITEM_NAME, D.QTY, D.TRAN_ID from TBL_POS_DETAIL D
                                                                LEFT OUTER JOIN TBL_BARCODE BG
                                                                ON BG.CODE = D.ITEM_CODE
                                                                LEFT OUTER JOIN TBL_ITEMSMASTER IM
                                                                ON IM.ITEM_CODE =
                                                                CASE WHEN '{b_i}' = 'B' THEN BG.ITEM_CODE
                                                                WHEN '{b_i}' = 'I' THEN D.ITEM_CODE END
                                                                WHERE D.DLT = 'T' AND D.BCODE = '{report.Branch}' AND D.PERIOD_ID = '{common.Period}'";
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
                            var row = new CustomSalesInvoiceReport
                            {
                                TRAN_ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                Mobile = reader["CMOB"] == DBNull.Value ? "" : Convert.ToString(reader["CMOB"]),
                                CName = reader["CNAME"] == DBNull.Value ? "" : Convert.ToString(reader["CNAME"]),
                                CAdd = reader["CADD"] == DBNull.Value ? "" : Convert.ToString(reader["CADD"]),
                                NetAmt = reader["NET_TOTAL"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_TOTAL"]),
                                Advance = reader["ADVANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["ADVANCE"]),
                                DeliveryCharges = reader["DEL_CHARGES"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEL_CHARGES"]),
                                Total = reader["REMAINING"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["REMAINING"]),
                                Mode = reader["BILL_MODE"] == DBNull.Value ? "" : Convert.ToString(reader["BILL_MODE"]),
                                detail = jsonDetailDataResult.Where(x => x.tranId == Convert.ToInt32(reader["TRAN_ID"])).ToList()
                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID == 118)
                    {
                        string query = @$"Select TC.CMOB , MAX(TC.CNAME) AS CNAME, MAX(TC.CADD) AS CADD from TBL_POS_CUS TC
                                          Left Join TBL_POS_MASTER PM on Pm.TRAN_ID = Tc.TRAN_ID
                                          Left Join TBL_BRANCH TB on TB.BCODE = PM.BCODE 
                                          Where TC.CMOB <> '' And PM.CLOSING = 1 And PM.BILL_STATUS = 'P' And PM.DLT = 'T' GROUP BY CMOB";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomSalesInvoiceReport
                            {
                                //BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                CMOB = reader["CMOB"] == DBNull.Value ? "" : Convert.ToString(reader["CMOB"]),
                                CNAME = Convert.ToString(reader["CNAME"]),
                                CADD = Convert.ToString(reader["CADD"]),
                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else
                    {
                        string query = $"EXEC STKPROC '{report.ReportID}','{report.FromDate.Value.ToString("yyyy-MM-dd")}','{report.ToDate.Value.ToString("yyyy-MM-dd")}','{report.Branch}','{common.Period}','{report.Item}','{report.Group}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (report.ReportID == 24)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    ItemId = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    Color = reader["COLOR"] == DBNull.Value ? "" : Convert.ToString(reader["COLOR"]),
                                    Size = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                    Desc = Convert.ToString(reader["DDESC"]),
                                    Barcode = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                    Debit = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    Credit = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"])

                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 25)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemId = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    Color = reader["COLOR"] == DBNull.Value ? "" : Convert.ToString(reader["COLOR"]),
                                    Size = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                    Barcode = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                    Debit = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    Credit = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"])

                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 26)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemId = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Barcode = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                    Size = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                    Color = reader["COLOR"] == DBNull.Value ? "" : Convert.ToString(reader["COLOR"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BALANCE"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 27)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                    ItemId = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    Color = reader["COLOR"] == DBNull.Value ? "" : Convert.ToString(reader["COLOR"]),
                                    Size = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                    Desc = Convert.ToString(reader["DDESC"]),
                                    Barcode = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                    Debit = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    Credit = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 28)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    Debit = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    Credit = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 29)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BALANCE"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 30)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString(),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                    ItemId = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Desc = Convert.ToString(reader["DDESC"]),
                                    Debit = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    Credit = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"]),
                                    SizeSet = reader["SIZE_SET"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE_SET"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 31)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemId = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    SizeSet = reader["SIZE_SET"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE_SET"]),
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 32)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemId = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Debit = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    Credit = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"]),
                                    SizeSet = reader["SIZE_SET"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE_SET"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 33)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemId = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    PPrice = reader["PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PRICE"]),
                                    WPrice = reader["WHOLE_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WHOLE_PRICE"]),
                                    SPrice = reader["SHOP_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["SHOP_PRICE"]),
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 95)
                        {

                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    PPrice = reader["PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PRICE"]),
                                    WPrice = reader["WHOLE_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WHOLE_PRICE"]),
                                    SPrice = reader["SHOP_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["SHOP_PRICE"]),
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 34)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemId = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Barcode = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                    Size = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                    Color = reader["COLOR"] == DBNull.Value ? "" : Convert.ToString(reader["COLOR"]),
                                    Price = reader["PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["PRICE"]),
                                    WholePrice = reader["WHOLE_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WHOLE_PRICE"]),
                                    ShopPrice = reader["SHOP_PRICE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["SHOP_PRICE"]),
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"]),
                                    ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_CODE"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 35)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemId = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 41)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemId = reader["LOT"] == DBNull.Value ? "" : Convert.ToString(reader["LOT"]),
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"]),
                                    Unit = reader["UNIT"] == DBNull.Value ? "" : reader["UNIT"].ToString(),
                                    ItemName = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    PartyName = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 42)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    PartyName = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
                                    ItemId = reader["LOT"] == DBNull.Value ? "" : Convert.ToString(reader["LOT"]),
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"]),
                                    Unit = reader["UNIT"] == DBNull.Value ? "" : reader["UNIT"].ToString()
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 43)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemName = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    ItemId = reader["LOT"] == DBNull.Value ? "" : Convert.ToString(reader["LOT"]),
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"]),
                                    Unit = reader["UNIT"] == DBNull.Value ? "" : reader["UNIT"].ToString()
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 44)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    ItemName = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    PartyName = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]),
                                    ItemId = reader["LOT_NO"] == DBNull.Value ? "" : Convert.ToString(reader["LOT_NO"]),
                                    Desc = Convert.ToString(reader["DDESC"]),
                                    Debit = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    Credit = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"]),


                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 36)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    ItemId = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    Size = reader["SIZE"] == DBNull.Value ? "" : Convert.ToString(reader["SIZE"]),
                                    Barcode = reader["BARCODE"] == DBNull.Value ? "" : Convert.ToString(reader["BARCODE"]),
                                    Qty = reader["Qty"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Qty"]),
                                    Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    WRate = reader["WRATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["WRATE"]),
                                    WAmt = reader["WAMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["WAMT"]),
                                    BTransfer = reader["BTRANSFER"] == DBNull.Value ? "" : Convert.ToString(reader["BTRANSFER"]),
                                    Color = reader["COLOR"] == DBNull.Value ? "" : Convert.ToString(reader["COLOR"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 88)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    ItemName = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                    GroupName = reader["GROUP_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["GROUP_NAME"]),
                                    Desc = Convert.ToString(reader["DDESC"]),
                                    Debit = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    Credit = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"]),
                                    //Balance2 = (reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]))
                                    //        - (reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"])),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 89)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemName = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    Debit = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    Credit = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"]),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                    Balance2 = (reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]))
                                            - (reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"]))
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 90)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemName = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                    Balance = (reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"]))
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 91)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemName = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Balance = (reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"])),
                                    Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                    Amt = reader["AMOUNT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMOUNT"]),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 126)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    ItemName2 = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Warehouse = reader["WAREHOUSE"] == DBNull.Value ? "" : Convert.ToString(reader["WAREHOUSE"]),
                                    Desc = reader["DDESC"] == DBNull.Value ? "" : Convert.ToString(reader["DDESC"]),
                                    StockIn = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    StockOut = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                    //Balance2 = (reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]))
                                    //        - (reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"])),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"])
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 127)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemName2 = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Warehouse = reader["WAREHOUSE"] == DBNull.Value ? "" : Convert.ToString(reader["WAREHOUSE"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    StockIn = reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]),
                                    StockOut = reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"]),
                                    Balance2 = (reader["STOCK_IN"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_IN"]))
                                            - (reader["STOCK_OUT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["STOCK_OUT"]))
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 128)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemName2 = reader["ITEM_ID"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_ID"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    Balance = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"]),
                                    Warehouse = reader["WAREHOUSE"] == DBNull.Value ? "" : Convert.ToString(reader["WAREHOUSE"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 129)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                    Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                    Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CREDIT"]),
                                    BTransfer = reader["BTRANSFER"] == DBNull.Value ? "" : Convert.ToString(reader["BTRANSFER"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 130)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    TotalBalance = reader["BALANCE"] == DBNull.Value ? 0 : Math.Round(Convert.ToDecimal(reader["BALANCE"]), 0),
                                    BTransfer = reader["BTRANSFER"] == DBNull.Value ? "" : Convert.ToString(reader["BTRANSFER"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }
                        else if (report.ReportID == 145)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomSalesInvoiceReport
                                {
                                    ItemName = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]),
                                    Category = reader["CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["CATEGORY"]),
                                    SubCategory = reader["SUB_CATEGORY"] == DBNull.Value ? "" : Convert.ToString(reader["SUB_CATEGORY"]),
                                    Balance = (reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"])),
                                    totalSales = reader["DAYS_NO_SALE"] == DBNull.Value ? 0m : Convert.ToDecimal(reader["DAYS_NO_SALE"]),
                                    LastSaleDate = reader["LAST_SALE_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["LAST_SALE_DATE"]).ToString("yyyy-MM-dd"),
                                    BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                };
                                jsonDataResult.Add(row);
                            }
                        }

                        reader.Close();
                    }

                }

                CalculateBalanceAmount(jsonDataResult, report.ReportID);

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

        public static void CalculateBalanceAmount(IEnumerable<CustomSalesInvoiceReport> jsonDataResult, int? reportId)
        {
            if (reportId == 88)
            {
                var distinctActCodes = jsonDataResult.Select(v => v.ItemName).Distinct();

                foreach (var accountCode in distinctActCodes)
                {
                    var individualAccountVouchers = jsonDataResult.Where(v => v.ItemName == accountCode).ToList();
                    decimal runningBalance = 0;

                    foreach (var voucher in individualAccountVouchers)
                    {
                        runningBalance += Convert.ToDecimal(voucher.Debit) - Convert.ToDecimal(voucher.Credit);
                        voucher.Balance2 = runningBalance;
                    }
                }
            }
            if (reportId == 24)
            {
                var distinctActCodes = jsonDataResult.Select(v => v.Barcode).Distinct();

                foreach (var accountCode in distinctActCodes)
                {
                    var individualAccountVouchers = jsonDataResult.Where(v => v.Barcode == accountCode).ToList();
                    decimal runningBalance = 0;

                    foreach (var voucher in individualAccountVouchers)
                    {
                        runningBalance += Convert.ToDecimal(voucher.Debit) - Convert.ToDecimal(voucher.Credit);
                        voucher.Balance2 = runningBalance;
                    }
                }
            }
            if (reportId == 35 || reportId == 44)
            {
                var distinctActCodes = jsonDataResult.Select(v => v.Barcode).Distinct();

                foreach (var accountCode in distinctActCodes)
                {
                    var individualAccountVouchers = jsonDataResult.Where(v => v.Barcode == accountCode).ToList();
                    decimal runningBalance = 0;

                    foreach (var voucher in individualAccountVouchers)
                    {
                        runningBalance += Convert.ToDecimal(voucher.Debit) - Convert.ToDecimal(voucher.Credit);
                        voucher.Balance = runningBalance;
                    }
                }
            }
            if (reportId == 27)
            {
                var distinctActCodes = jsonDataResult.Select(v => v.Category).Distinct();

                foreach (var accountCode in distinctActCodes)
                {
                    var individualAccountVouchers = jsonDataResult.Where(v => v.Category == accountCode).ToList();
                    decimal runningBalance = 0;

                    foreach (var voucher in individualAccountVouchers)
                    {
                        runningBalance += Convert.ToDecimal(voucher.Debit) - Convert.ToDecimal(voucher.Credit);
                        voucher.Balance = runningBalance;
                    }
                }
            }
            if (reportId == 30)
            {
                var distinctActCodes = jsonDataResult.Select(v => v.ItemId).Distinct();

                foreach (var accountCode in distinctActCodes)
                {
                    var individualAccountVouchers = jsonDataResult.Where(v => v.ItemId == accountCode).ToList();
                    decimal runningBalance = 0;

                    foreach (var voucher in individualAccountVouchers)
                    {
                        runningBalance += Convert.ToDecimal(voucher.Debit) - Convert.ToDecimal(voucher.Credit);
                        voucher.Balance = runningBalance;
                    }
                }
            }
            if (reportId == 126)
            {
                var distinctItems = jsonDataResult.Select(v => v.ItemName2).Distinct();

                foreach (var item in distinctItems)
                {
                    var vouchers = jsonDataResult.Where(v => v.ItemName2 == item).ToList();
                    decimal runningBalance = 0;

                    foreach (var voucher in vouchers)
                    {
                        //runningBalance += voucher.StockIn - voucher.StockOut;
                        runningBalance += (voucher.StockIn ?? 0) - (voucher.StockOut ?? 0);
                        voucher.Balance2 = runningBalance;
                    }
                }
            }
            if (reportId == 129)
            {
                var distinctItems = jsonDataResult.Select(v => v.BTransfer).Distinct();

                foreach (var item in distinctItems)
                {
                    var vouchers = jsonDataResult.Where(v => v.BTransfer == item).ToList();
                    decimal runningBalance = 0;

                    foreach (var voucher in vouchers)
                    {
                        //runningBalance += voucher.StockIn - voucher.StockOut;
                        runningBalance += (voucher.Debit ?? 0) - (voucher.Credit ?? 0);
                        voucher.Balance2 = runningBalance;
                    }
                }
            }
        }


    }
}