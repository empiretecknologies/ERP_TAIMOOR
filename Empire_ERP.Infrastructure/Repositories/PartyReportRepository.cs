using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class PartyReportRepository : IPartyReportRepository
    {
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
        public async Task<MyHttpResponseMessage> GetReportData(PartyReport report, Common common)
        {

            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<CustomPartyReport> jsonDataResult = new List<CustomPartyReport>();
                var voucherMap = new Dictionary<string, CustomPartyReport>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $"EXEC PPROC '{report.ReportID}','{report.FromDate.Value.ToString("yyyy-MM-dd")}','{report.ToDate.Value.ToString("yyyy-MM-dd")}','{report.ControlCode}','{report.PartyCode}','{report.BRANCH}','{common.Period}','{report.Nature}','','','','','','','','{report.CAT_CODE}'";
                    string passCheck = "SELECT COUNT(*) FROM TBL_CHART WHERE ACT_CODE = '" + report.ControlCode + "' AND (PASS = '" + CommonService.EncryptString(report.Pass == null ? "" : report.Pass) + "' OR PASS is NULL OR PASS = '') AND DLT = 'T'";
                    SqlCommand command = new SqlCommand(passCheck, connection);
                    connection.Open();
                    if (report.ReportID == 38 || report.ReportID == 39)
                    {
                        command.CommandText = passCheck;
                        int count = (int)command.ExecuteScalar();
                        if (count == 0)
                        {
                            response.msgType = 2;
                            response.msg = "Wrong Password";
                            return response;
                        }
                    }
                    command.CommandTimeout = 120;
                    command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    //SqlDataReader reader = ExecuteStoredProcedure("PPROC", report.ReportID, report.FromDate, report.ToDate, report.ControlCode, report.PartyCode.ToString(), Convert.ToInt32(common.Branch), Convert.ToInt32(common.Period), report.RegionCode, report.Nature.ToString());
                    if (report.ReportID == 39)
                    {
                        while (reader.Read())
                        {
                            var row = new CustomPartyReport
                            {
                                VoucherDate = "",
                                VoucherNo = "",
                                AccountCode = 0,
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                PartyName = Convert.ToString(reader["PARTY_NAME"]),
                                AccountDescription = "",
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CREDIT"])
                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID == 11)
                    {
                        while (reader.Read())
                        {
                            var row = new CustomPartyReport
                            {
                                VoucherDate = "",
                                VoucherNo = "",
                                AccountCode = 0,
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                PartyName = Convert.ToString(reader["PARTY_NAME"]),
                                CATEGORY = Convert.ToString(reader["CATEGORY"]),
                                AccountDescription = "",
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CREDIT"]),
                                //BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID == 10)
                    {
                        while (reader.Read())
                        {
                            var row = new CustomPartyReport
                            {
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountCode = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                AccountName = Convert.ToString(reader["PARTY_NAME"]),
                                PartyName = "",
                                AccountDescription = Convert.ToString(reader["ACT_DESC"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CREDIT"]),
                                ChqNo = Convert.ToString(reader["CHQ_NO"]),
                                ChqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("yyyy-MM-dd"),
                                BName = reader["B_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["B_NAME"]),
                                BCODE = reader["BCODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BCODE"]),
                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID is 38 or 70)
                    {
                        while (reader.Read())
                        {
                            var row = new CustomPartyReport
                            {
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountCode = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                AccountName = Convert.ToString(reader["PARTY_NAME"]),
                                PartyName = "",
                                AccountDescription = Convert.ToString(reader["ACT_DESC"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CREDIT"]),
                                ChqNo = Convert.ToString(reader["CHQ_NO"]),
                                ChqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("yyyy-MM-dd"),

                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID is 58)
                    {
                        while (reader.Read())
                        {
                            var row = new CustomPartyReport
                            {
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountCode = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                AccountName = Convert.ToString(reader["PARTY_NAME"]),
                                PartyName = "",
                                AccountDescription = Convert.ToString(reader["ACT_DESC"]),
                                Debit = Convert.ToDecimal(reader["DEBIT"]),
                                Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]),
                                Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]),
                                Disc = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                Credit = Convert.ToDecimal(reader["CREDIT"])
                            };

                            if (voucherMap.ContainsKey(row.VoucherNo))
                            {
                                voucherMap[row.VoucherNo].Debit += row.Debit;
                                voucherMap[row.VoucherNo].Credit += row.Credit;
                                row.Debit = 0;
                                row.Credit = 0;
                                row.VoucherNo = "";
                                row.VoucherDate = "";
                                //row.AccountDescription = "";
                            }
                            voucherMap[row.VoucherNo] = row;
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID is 51 or 52)
                    {
                        while (reader.Read())
                        {
                            var row = new CustomPartyReport
                            {
                                PartyName = Convert.ToString(reader["Party_Name"]),
                                AccountName = Convert.ToString(reader["Act_Name"]),
                                CATEGORY = Convert.ToString(reader["CATEGORY"]),
                                VoucherDate = reader["B_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["B_DATE"]).ToString("yyyy-MM-dd"),
                                BillType = reader["DbCr"] == DBNull.Value ? "" : Convert.ToString(reader["DbCr"]) == "D" ? "Debit" : "Credit",
                                VoucherNo = Convert.ToString(reader["Bill_No"]),
                                DueDate = reader["DUE_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DUE_DATE"]).ToString("yyyy-MM-dd"),
                                Amount = reader["Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Amount"]),
                                DueAmount = reader["AfterAmount"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AfterAmount"]),
                                DueYear = reader["DueYear"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DueYear"]),
                                DueMonth = reader["DueMonth"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DueMonth"]),
                                DueDay = reader["DueDay"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DueDay"]),
                                Balance = reader["RunningAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RunningAmt"]),
                                LastDate = reader["L_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["L_DATE"]).ToString("yyyy-MM-dd"),
                                LastAmount = reader["L_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["L_AMT"]),
                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID is 97 or 98)
                    {

                        while (reader.Read())
                        {
                            var row = new CustomPartyReport
                            {
                                VoucherDate = reader["VDATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VDATE"]).ToString("yyyy-MM-dd"),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                PartyName = Convert.ToString(reader["PARTY_NAME"]),
                                Amount = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                            };
                            jsonDataResult.Add(row);
                        }
                    }

                    else if (report.ReportID is 99)
                    {
                        while (reader.Read())
                        {
                            var row = new CustomPartyReport
                            {
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("yyyy-MM-dd"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountCode = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                AccountName = Convert.ToString(reader["PARTY_NAME"]),
                                PartyName = "",
                                AccountDescription = Convert.ToString(reader["ACT_DESC"]),
                                Debit = Convert.ToDecimal(reader["DEBIT"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                Credit = Convert.ToDecimal(reader["CREDIT"]),
                                ChqNo = Convert.ToString(reader["CHQ_NO"]),
                                ChqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("yyyy-MM-dd"),

                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID is 102)
                    {
                        while (reader.Read())
                        {
                            var row = new CustomPartyReport
                            {
                                AccountNature = Convert.ToString(reader["ACT_NATURE"]),
                                PartyName = Convert.ToString(reader["PARTY_NAME"]),
                                AccountDescription = Convert.ToString(reader["ACT_DESC"]),
                                Amt = Convert.ToDecimal(reader["AMT"]),
                                ChqNo = Convert.ToString(reader["CHQ_NO"]),
                                ChqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("yyyy-MM-dd"),

                            };
                            jsonDataResult.Add(row);
                        }
                    }
                    else if (report.ReportID == 136)
                    {
                        while (reader.Read())
                        {
                            var row = new CustomPartyReport
                            {
                                ACT_NAME = Convert.ToString(reader["ACT_NAME"]),
                                PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]),
                                CATEGORY = Convert.ToString(reader["CATEGORY"]),
                                Aging_0_15 = reader["0-15"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["0-15"]),
                                Aging_16_30 = reader["16-30"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["16-30"]),
                                Aging_31_45 = reader["31-45"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["31-45"]),
                                Aging_46_60 = reader["46-60"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["46-60"]),
                                Aging_61_90 = reader["61-90"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["61-90"]),
                                Aging_91_120 = reader["91-120"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["91-120"]),
                                Aging_120_Plus = reader["120+"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["120+"]),
                                BalanceWithTotal = reader["BALANCE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["BALANCE"])

                            };
                            jsonDataResult.Add(row);
                        }
                    }

                    reader.Close();
                }

                #region Calculate Balance Amount
                if (report.ReportID is not 51 && report.ReportID is not 52 && report.ReportID is not 11 && report.ReportID is not 10)
                {
                    var distinctActCodes = jsonDataResult.Select(v => v.AccountCode).Distinct();

                    foreach (var accountCode in distinctActCodes)
                    {
                        var individualAccountVouchers = jsonDataResult.Where(v => v.AccountCode == accountCode).ToList();
                        decimal runningBalance = 0;

                        foreach (var voucher in individualAccountVouchers)
                        {
                            runningBalance += Convert.ToDecimal(voucher.Debit) - Convert.ToDecimal(voucher.Credit);
                            voucher.Balance = runningBalance;
                        }
                    }
                }
                else if (report.ReportID == 10)
                {
                    var distinctActCodes = jsonDataResult.Select(v => v.AccountCode).Distinct();

                    foreach (var accountCode in distinctActCodes)
                    {
                        var individualAccountVouchers = jsonDataResult.Where(v => v.AccountCode == accountCode).ToList();
                        decimal runningBalance = 0;

                        foreach (var voucher in individualAccountVouchers)
                        {
                            runningBalance += Convert.ToDecimal(voucher.Debit) - Convert.ToDecimal(voucher.Credit);
                            voucher.Balance2 = runningBalance;
                        }
                    }
                }

                #endregion
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

        public SqlDataReader ExecuteStoredProcedure(string storedProcedureName, int? ReportID, DateTime? FromDate, DateTime? ToDate, string? ControlCode, string? PartyCode, int? Branch, int? Period, string? RegionCode, string? Nature)
        {
            SqlConnection connection = new SqlConnection(new SQLService().getconnstring());
            SqlCommand command = new SqlCommand(storedProcedureName, connection);

            try
            {
                connection.Open();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 320;

                // Add parameters
                command.Parameters.AddWithValue("@REPORT_NO", ReportID);
                command.Parameters.AddWithValue("@FROM_DATE", FromDate.Value.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@TO_DATE", ToDate.Value.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@ACT_CODE", ControlCode ?? "");
                command.Parameters.AddWithValue("@PARTY_CODE", PartyCode);
                command.Parameters.AddWithValue("@BCODE", Branch);
                command.Parameters.AddWithValue("@PERIOD_ID", Period);
                command.Parameters.AddWithValue("@REGION", RegionCode ?? "");
                command.Parameters.AddWithValue("@NATURE", Nature ?? "");

                // Execute the reader with CommandBehavior.CloseConnection
                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                return reader;
            }
            catch (Exception)
            {
                // Ensure the connection is closed in case of an error
                connection.Dispose();
                throw; // Re-throw the exception
            }
        }
    }
}