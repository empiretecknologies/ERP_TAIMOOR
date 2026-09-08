using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Data;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class ImportReportRepository : IImportReportRepository
    {
        public IMenuRepository _menuRepository { get; set; }

        public ImportReportRepository(IMenuRepository menuRepository)
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

        public MyHttpResponseMessage GetReportData(ImportReport report, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<CustomAccountingReport> jsonDataResult = new List<CustomAccountingReport>();

                var groupOrderMap = new Dictionary<string, int>
                    {
                        { "SALES", 1 },
                        { "SALES RETRUN", 2 },
                        { "PURCHASE", 3 },
                        { "Expenses", 4 }
                    };

                if (report.ReportID == 3 || report.ReportID == 4 || report.ReportID == 101 || report.ReportID == 119 || report.ReportID == 121 || report.ReportID == 103)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $"EXEC APROC '{report.ReportID}','{report.FromDate.Value.ToString("yyyy-MM-dd")}','{report.ToDate.Value.ToString("yyyy-MM-dd")}','{report.ControlCode}','{report.AccountCode}','{common.Branch}','{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            if (report.ReportID == 3)
                            {
                                row = new CustomAccountingReport
                                {
                                    VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    AccountCode = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                    TRAN_ID = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                                    AccountName = Convert.ToString(reader["ACCOUNT_NAME"]),
                                    AccountDescription = Convert.ToString(reader["ACT_DESC"]),
                                    Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CREDIT"])
                                };
                            }
                            else if (report.ReportID == 4 || report.ReportID == 119)
                            {
                                row = new CustomAccountingReport
                                {
                                    AccountCode = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                    AccountName = Convert.ToString(reader["ACT_NAME"]),
                                    Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                    ParentName = Convert.ToString(reader["PARENT_NAME"]),
                                    Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CREDIT"])
                                };
                            }
                            else if (report.ReportID == 121)
                            {
                                row = new CustomAccountingReport
                                {
                                    ParentName = Convert.ToString(reader["PARENT_NAME"]),
                                    Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                    AccountName = Convert.ToString(reader["ACT_NAME"]),
                                    VC_TYPE = reader["VC_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["VC_TYPE"])
                                };
                            }
                            else if (report.ReportID == 101)
                            {
                                row = new CustomAccountingReport
                                {
                                    chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                    LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                    TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                    VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                    chq = reader["CHQ"] == DBNull.Value ? "" : Convert.ToString(reader["CHQ"]),
                                    Amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]),
                                    bType = Convert.ToString(reader["B_TYPE"]),
                                    BankName = Convert.ToString(reader["BANK_NAME"]),
                                    desc = Convert.ToString(reader["DDESC"]),
                                };
                            }
                            else if (report.ReportID == 103)
                            {
                                row = new CustomAccountingReport
                                {
                                    AccountCode = Convert.ToInt32(reader["ACT_CODE"]),
                                    AccountName = Convert.ToString(reader["ACT_NAME"]),
                                    Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                    Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["CREDIT"]),
                                    ParentName = Convert.ToString(reader["PARENT_NAME"]),
                                    AccountNature = Convert.ToInt32(reader["ACT_NATURE"]),
                                    NatureName = Convert.ToString(reader["NATURE_NAME"]),
                                    GroupOrder = groupOrderMap.ContainsKey(Convert.ToString(reader["NATURE_NAME"]))
                                    ? groupOrderMap[Convert.ToString(reader["NATURE_NAME"])]
                                    : 99  // agar map me na mile toh last me show kare
                                };
                            }

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }

                    #region Calculate Balance Amount
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
                    #endregion
                }
                else if (report.ReportID == 37)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query;
                        query = $@"SELECT JV.V_DATE,JV.VOUCHER_NO,
                                CASE WHEN JV.PARTY_CODE = 0 THEN CH.ACT_NAME ELSE PT.PARTY_NAME END AS ACT_NAME,
                                JV.DEBIT,JV.CREDIT,JV.DT_DESC
                                from TBL_JV JV
                                LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE
                                LEFT OUTER JOIN TBL_CHART CH
                                ON CH.ACT_CODE = JV.ACT_CODE
                                WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T' AND JV.BCODE = 1 AND JV.PERIOD_ID = 1
                                AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                ORDER BY V_DATE , CREDIT";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport
                            {
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                Debit = Convert.ToDecimal(reader["DEBIT"]),
                                Credit = Convert.ToDecimal(reader["CREDIT"])
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 92)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $"EXEC APROC '{report.ReportID}','{report.FromDate.Value.ToString("yyyy-MM-dd")}','{report.ToDate.Value.ToString("yyyy-MM-dd")}','{report.ControlCode}','{report.AccountCode}','{common.Branch}','{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                AccountCode = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                ParentName = Convert.ToString(reader["PARENT_NAME"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 106)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE AS VOUCHER_DATE,JV.VOUCHER_NO,
                                        CASE WHEN JV.PARTY_CODE = 0 THEN CH.ACT_NAME ELSE PT.PARTY_NAME END AS ACT_NAME,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,BK.ACT_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_NO,JV.CHQ_DATE
                                        from TBL_CBRECEIPT JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART CH
                                        ON CH.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART BK
                                        ON BK.ACT_CODE = JV.BOOK_TYPE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                        AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 107)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE  AS VOUCHER_DATE,JV.VOUCHER_NO,
                                        CASE WHEN JV.PARTY_CODE = 0 THEN CH.ACT_NAME ELSE PT.PARTY_NAME END AS ACT_NAME,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,BK.ACT_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_DATE,JV.CHQ_NO
                                        from TBL_CBPAYMENT JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART CH
                                        ON CH.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART BK
                                        ON BK.ACT_CODE = JV.BOOK_TYPE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                         AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 108)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE AS VOUCHER_DATE,JV.VOUCHER_NO,
                                        CASE WHEN JV.PARTY_CODE = 0 THEN CH.ACT_NAME ELSE PT.PARTY_NAME END AS ACT_NAME,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,BK.ACT_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_DATE,JV.CHQ_NO
                                        from TBL_BBRECEIPT JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART CH
                                        ON CH.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART BK
                                        ON BK.ACT_CODE = JV.BOOK_TYPE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                        AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 109)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE AS VOUCHER_DATE,JV.VOUCHER_NO,
                                        CASE WHEN JV.PARTY_CODE = 0 THEN CH.ACT_NAME ELSE PT.PARTY_NAME END AS ACT_NAME,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,BK.ACT_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_DATE,JV.CHQ_NO
                                        from TBL_BBPAYMENT JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART CH
                                        ON CH.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART BK
                                        ON BK.ACT_CODE = JV.BOOK_TYPE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                        AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 110)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE AS VOUCHER_DATE ,JV.VOUCHER_NO,
                                        PT.PARTY_NAME AS ACT_NAME,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,BK.ACT_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_DATE,JV.CHQ_NO
                                        from TBL_PRECEIPT JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART BK
                                        ON BK.ACT_CODE = JV.BOOK_TYPE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                        AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 111)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE AS VOUCHER_DATE,JV.VOUCHER_NO,
                                        PT.PARTY_NAME AS ACT_NAME,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,BK.ACT_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_DATE,JV.CHQ_NO
                                        from TBL_PPAYMENT JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART BK
                                        ON BK.ACT_CODE = JV.BOOK_TYPE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                        AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 112)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE AS VOUCHER_DATE,JV.VOUCHER_NO,
                                        PT.PARTY_NAME AS ACT_NAME,
                                        JV.QTY,JV.RATE,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,BK.ACT_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_DATE,JV.CHQ_NO
                                        from TBL_PB_VOUCHER JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_CHART BK
                                        ON BK.ACT_CODE = JV.BOOK_TYPE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                        AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RATE"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 113)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE AS VOUCHER_DATE,JV.VOUCHER_NO,
                                        PT.PARTY_NAME AS ACT_NAME,
                                        JV.QTY,JV.RATE,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,BK.ACT_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_DATE,JV.CHQ_NO
                                        from TBL_DN_VOUCHER JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE

                                        LEFT OUTER JOIN TBL_CHART BK
                                        ON BK.ACT_CODE = JV.BOOK_TYPE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                        AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RATE"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 114)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE AS VOUCHER_DATE,JV.VOUCHER_NO,
                                        PT.PARTY_NAME AS ACT_NAME,
                                        JV.QTY,JV.RATE,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,BK.ACT_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_DATE,JV.CHQ_NO
                                        from TBL_SB_VOUCHER JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE

                                        LEFT OUTER JOIN TBL_CHART BK
                                        ON BK.ACT_CODE = JV.BOOK_TYPE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                        AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RATE"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 115)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE AS VOUCHER_DATE,JV.VOUCHER_NO,
                                        PT.PARTY_NAME AS ACT_NAME,
                                        JV.QTY,JV.RATE,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,BK.ACT_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_DATE,JV.CHQ_NO
                                        from TBL_CN_VOUCHER JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE

                                        LEFT OUTER JOIN TBL_CHART BK
                                        ON BK.ACT_CODE = JV.BOOK_TYPE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                        AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                Rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["RATE"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                else if (report.ReportID == 116)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = @$"select JV.V_DATE AS VOUCHER_DATE,JV.VOUCHER_NO,
                                        PT.PARTY_NAME AS ACT_NAME,
                                        CASE WHEN DC_TYPE = 'C' THEN JV.AMT WHEN DC_TYPE = 'D' THEN 0 END AS DEBIT,
                                        CASE WHEN DC_TYPE = 'D' THEN JV.AMT WHEN DC_TYPE = 'C' THEN 0 END AS CREDIT,
                                        JV.DT_DESC,PT.PARTY_NAME AS BOOK_tYPE,
                                        JV.MENU_ID,JV.TRAN_ID,MB.MENU_PARENT_CODE,MB.MENU_PAGE,JV.CHQ_NO,JV.CHQ_DATE
                                        from TBL_PPPV JV
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT
                                        ON PT.PARTY_CODE = JV.PARTY_CODE AND PT.ACT_CODE = JV.ACT_CODE
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PTT
                                        ON PT.PARTY_CODE = JV.TPARTY_CODE AND PT.ACT_CODE = JV.TACT_CODE
                                        LEFT OUTER JOIN TBL_MENU_BUILDER MB
                                        ON MB.ID = JV.MENU_ID
                                        WHERE JV.ASTATUS = 'Y' AND JV.DLT = 'T'
                                        AND JV.BCODE = {common.Branch} AND JV.PERIOD_ID = {common.Period}
                                        AND JV.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'
                                        ORDER BY V_DATE , CREDIT";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport();
                            row = new CustomAccountingReport
                            {
                                VoucherDate = reader["VOUCHER_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["VOUCHER_DATE"]).ToString("dd/MM/yyyy"),
                                chqDate = reader["CHQ_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["CHQ_DATE"]).ToString("dd/MM/yyyy"),
                                chqNo = Convert.ToString(reader["CHQ_NO"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                AccountDescription = Convert.ToString(reader["DT_DESC"]),
                                bType = Convert.ToString(reader["BOOK_tYPE"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT"]),
                                Credit = reader["CREDIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }
                }
                //else if (report.ReportID == 121)
                //{
                //    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                //    {
                //        string query = $"EXEC APROC '{report.ReportID}','{report.FromDate.Value.ToString("yyyy-MM-dd")}','{report.ToDate.Value.ToString("yyyy-MM-dd")}','{report.ControlCode}','{report.AccountCode}','{common.Branch}','{common.Period}'";
                //        SqlCommand command = new SqlCommand(query, connection);
                //        connection.Open();
                //        SqlDataReader reader = command.ExecuteReader();
                //        while (reader.Read())
                //        {
                //            var row = new CustomAccountingReport();
                //            row = new CustomAccountingReport
                //            {
                //                ParentName = Convert.ToString(reader["PARENT_NAME"]),
                //                AccountName = Convert.ToString(reader["ACT_NAME"]),
                //                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                //                VoucherType = reader["VC_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["VC_TYPE"]),
                //            };

                //            jsonDataResult.Add(row);
                //        }
                //        reader.Close();
                //    }
                //}
                else if (report.ReportID == 121)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $"EXEC APROC '{report.ReportID}','{report.FromDate.Value:yyyy-MM-dd}','{report.ToDate.Value:yyyy-MM-dd}','{report.ControlCode}','{report.AccountCode}','{common.Branch}','{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var row = new CustomAccountingReport
                            {
                                ParentName = Convert.ToString(reader["PARENT_NAME"]),
                                AccountName = Convert.ToString(reader["ACT_NAME"]),
                                Debit = reader["DEBIT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DEBIT"]),
                                VoucherType = reader["VC_TYPE"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["VC_TYPE"]),
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();
                    }

                    var groupedSums = jsonDataResult
                        .Where(x => x.VoucherType.HasValue)
                        .GroupBy(x => x.VoucherType.Value)
                        .ToDictionary(g => g.Key, g => g.Sum(x => x.Debit));

                    decimal sum1 = groupedSums.ContainsKey(1) ? groupedSums[1] ?? 0 : 0;
                    decimal sum2 = groupedSums.ContainsKey(2) ? groupedSums[2] ?? 0 : 0;
                    decimal sum3 = groupedSums.ContainsKey(3) ? groupedSums[3] ?? 0 : 0;
                    decimal sum4 = groupedSums.ContainsKey(4) ? groupedSums[4] ?? 0 : 0;

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

        public MyHttpResponseMessage GetDataForReport(ImportReportRDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            ImportReportRDLCReport masterData = new ImportReportRDLCReport();
            CustomImportReportForPrintReport reportData = new CustomImportReportForPrintReport();

            //var Menu = _menuRepository.GetMenu(common.MenuID);
            //string? table = string.Empty, detailTable = string.Empty;
            //if (Menu.data != null)
            //{
            //    var menu = (Menu)Menu.data;
            //    table = menu.TABLE1;
            //    detailTable = menu.TABLE2;
            //}

            try
            {
                string query = "";
                if (modelRecord.REPORTID == 131)
                {
                    query = @$"EXEC COST_CENTER {modelRecord.REPORTID},'{modelRecord.FROMDATE}','{modelRecord.TODATE}','{modelRecord.CONTROLCODE}','{modelRecord.SUBSIDIARYCODE}',{common.Branch},{common.Period}";
                }

                masterData.COMPANY_NAME = currentCompany.C_NAME;
                masterData.COMPANY_ADDRESS = currentCompany.C_ADDRESS;
                masterData.COMPANY_PHONE = currentCompany.C_TEL;
                masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                masterData.COMPANY_WATER = currentCompany.C_WATER;
                //masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                masterData.SIG1 = $"{menuDetails.MENU_SIG1}";
                masterData.SIG2 = $"{menuDetails.MENU_SIG2}";
                masterData.SIG3 = $"{menuDetails.MENU_SIG3}";
                masterData.SIG4 = $"{menuDetails.MENU_SIG4}";
                masterData.MENU_TERMS = $"{menuDetails.MENU_TERMS}";

                if (modelRecord.REPORTID == 131)
                {
                    //using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    //{
                    //    SqlCommand command = new SqlCommand(topQuery, connection);
                    //    connection.Open();
                    //    SqlDataReader reader = command.ExecuteReader();
                    //    while (reader.Read())
                    //    {
                    //        masterData.INVOICE_NUMBER = Convert.ToString(reader["VOUCHER_NO"]);
                    //        masterData.DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                    //        masterData.PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]);
                    //        masterData.ACT_GRCODE = Convert.ToString(reader["ACT_GRCODE"]);
                    //        masterData.BRANCH_ADDRESS = Convert.ToString(reader["B_ADDRESS"]);
                    //        masterData.BRANCH_PHONE = Convert.ToString(reader["B_TEL"]);
                    //        masterData.DISC = Convert.ToDecimal(reader["DISC"]);
                    //    }
                    //    reader.Close();
                    //}
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (modelRecord.REPORTID == 131)
                        {
                            while (reader.Read())
                            {
                                DataRow dataRow = dataTable.NewRow();
                                dataRow["BUYER_NAME"] = Convert.ToString(reader["BUYER_NAME"]);
                                dataRow["NTN"] = Convert.ToString(reader["NTN"]);
                                dataRow["ITEM_DETAIL"] = Convert.ToString(reader["ITEM_DETAIL"]);
                                dataRow["VOUCHER_NO"] = Convert.ToString(reader["VOUCHER_NO"]);
                                dataRow["PMENU_ID"] = Convert.ToString(reader["PMENU_ID"]);
                                dataRow["AMT"] = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);
                                dataRow["ACT_NAME"] = Convert.ToString(reader["ACT_NAME"]);
                                dataTable.Rows.Add(dataRow);
                            }
                        }
                        //else
                        //{
                        //    while (reader.Read())
                        //    {
                        //        DataRow dataRow = dataTable.NewRow();
                        //        dataRow["ItemName"] = Convert.ToString(reader["ITEM_NAME"]);
                        //        dataRow["Size"] = "";
                        //        dataRow["Qty"] = Convert.ToString(reader["QTY"]);
                        //        dataRow["Rate"] = 0;
                        //        dataRow["Amount"] = 0;
                        //        dataTable.Rows.Add(dataRow);
                        //    }
                        //}
                        reader.Close();
                    }
                }
                reportData.Master = masterData;
                reportData.Detail = dataTable;
                //if (menuDetails.MD_ID == 22)
                //{
                //    reportData.Detail = taxDataTable;
                //}
                //else if (menuDetails.MD_ID == 54)
                //{
                //    reportData.Detail = inspectionServiceChargesDetails;
                //}
                //else
                //{
                //    reportData.Detail = (menuDetails.MD_ID == 21 || menuDetails.MD_ID == 25) ? taxDataTable : dataTable;
                //}
                //=FormatNumber(FormatNumber(Fields!Rate.Value * (1 - Fields!Unit.Value / 100), 2) * Fields!Qty.Value, 0)
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