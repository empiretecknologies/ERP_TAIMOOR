using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Diagnostics;
using System.Transactions;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class PurchaseBillReportRepository : IPurchaseBillReportRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public PurchaseBillReportRepository(IMenuRepository menuRepository)
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
        public MyHttpResponseMessage GetReportData(PurchaseBillReport report, Common common)
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
                report.Item = report.Item = report.Item ?? 0;
                report.BuyerPartyCode = report.BuyerPartyCode = report.BuyerPartyCode ?? 0;
                report.BuyerRegionCode = report.BuyerRegionCode = report.BuyerRegionCode ?? 0;
                report.SellerPartyCode = report.SellerPartyCode = report.SellerPartyCode ?? 0;
                report.BuyerControlCode = report.BuyerControlCode = report.BuyerControlCode ?? "";
                report.SellerControlCode = report.SellerControlCode = report.SellerControlCode ?? "";
                report.SellerRegionCode = report.SellerRegionCode = report.SellerRegionCode ?? 0;
                report.ArivalStatus = report.ArivalStatus = report.ArivalStatus ?? "";
                report.Astatus = report.Astatus = report.Astatus ?? "";
                List<CustomPurchaseBillReport> jsonDataResult = new List<CustomPurchaseBillReport>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "SELECT M.TRAN_ID, M.V_DATE, M.VOUCHER_NO , M.REF, PT.PARTY_NAME , M.ACT_CODE, M.REMARKS, IT.ITEM_NAME, D.QTY, U.GROUP_NAME AS UNIT, D.QTY2, D.BAL_QTY, D.RATE, D.AMT, D.DISC, " +
                            $"D.DISC_AMT, D.TAX , D.TAX_AMT, D.NET_AMT,M.MENU_ID, MB.MENU_PAGE, MB.MENU_PARENT_CODE FROM {table} M " +
                            $"LEFT OUTER JOIN {detailTable} D ON D.TRAN_ID = M.TRAN_ID AND D.BCODE = M.BCODE AND D.PERIOD_ID = M.PERIOD_ID " +
                            $"LEFT OUTER JOIN TBL_ITEMSMASTER IT ON IT.ITEM_CODE = D.ITEM_CODE " +
                            $"LEFT OUTER JOIN TBL_UNIT U ON U.GROUP_CODE = D.UNIT " +
                            $"LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE  " +
                            $"LEFT OUTER JOIN TBL_MENU_BUILDER MB ON M.MENU_ID = MB.ID "+
                            $"WHERE D.DLT = 'T' AND M.DLT = 'T' " +
                            $"AND M.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}' " +
                            $"AND M.ASTATUS = CASE WHEN '{report.Astatus}' <> '' THEN '{report.Astatus}' ELSE M.ASTATUS END " +
                            $"AND PT.PARTY_CODE = CASE WHEN {report.BuyerPartyCode} <> 0 THEN {report.BuyerPartyCode} ELSE PT.PARTY_CODE END " +
                            $"AND PT.ACT_CODE = CASE WHEN '{report.BuyerControlCode}' <> '' THEN '{report.BuyerControlCode}' ELSE PT.ACT_CODE END " +
                            $"AND PT.REGION = CASE WHEN {report.BuyerRegionCode} <> 0 THEN {report.BuyerRegionCode} ELSE PT.REGION END " +
                            $"AND IT.ITEM_CODE = CASE WHEN {report.Item} <> 0 THEN {report.Item} ELSE IT.ITEM_CODE END ";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (report.ReportID == 16 || report.ReportID == 104 || report.ReportID == 105)
                        {
                            var row = new CustomPurchaseBillReport
                            {
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                REF = Convert.ToString(reader["REF"]),
                                BuyerName = Convert.ToString(reader["PARTY_NAME"]),
                                Remarks = Convert.ToString(reader["REMARKS"]),
                                ItemName = Convert.ToString(reader["ITEM_NAME"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_AMT = Convert.ToString(reader["DISC_AMT"]),
                                TAX = Convert.ToString(reader["TAX"]),
                                TAX_AMT = Convert.ToString(reader["TAX_AMT"]),
                                NET_AMT = Convert.ToString(reader["NET_AMT"]),
                                MTRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),

                            };
                            jsonDataResult.Add(row);
                        }
                        else if (report.ReportID == 15)
                        {
                            var row = new CustomPurchaseBillReport
                            {
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                ArivalStatus = Convert.ToString(reader["ARIVAL_STATUS"]),
                                BuyerName = Convert.ToString(reader["BUYER_NAME"]),
                                SellerName = Convert.ToString(reader["SELLER_NAME"]),
                                ItemName = Convert.ToString(reader["ITEM_NAME"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                Remarks = Convert.ToString(reader["REMARKS"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        else if (report.ReportID == 14)
                        {
                            var row = new CustomPurchaseBillReport
                            {
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                BuyerName = Convert.ToString(reader["BUYER_NAME"]),
                                BrokerName = Convert.ToString(reader["BROKER_NAME"]),
                                SellerName = Convert.ToString(reader["SELLER_NAME"]),
                                ItemName = Convert.ToString(reader["ITEM_NAME"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                COND = Convert.ToString(reader["COND"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                RT_TYPE = Convert.ToString(reader["Rt_type"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                Remarks = Convert.ToString(reader["REMARKS"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        else
                        {
                            var row = new CustomPurchaseBillReport
                            {
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                BuyerName = Convert.ToString(reader["BUYER_NAME"]),
                                //BrokerName = Convert.ToString(reader["BROKER_NAME"]),
                                SellerName = Convert.ToString(reader["SELLER_NAME"]),
                                ItemName = Convert.ToString(reader["ITEM_NAME"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                INS = Convert.ToString(reader["INSURANCE"]),
                                //COND = Convert.ToString(reader["COND"]),
                                //RATE = Convert.ToString(reader["RATE"]),
                                //RT_TYPE = Convert.ToString(reader["Rt_type"]),
                                //AMT = Convert.ToString(reader["AMT"]),
                                Remarks = Convert.ToString(reader["REMARKS"]),
                            };
                            jsonDataResult.Add(row);
                        }
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
    }
}