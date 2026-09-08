using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Diagnostics;
using System.Transactions;
using ZXing.QrCode.Internal;
using static System.Net.WebRequestMethods;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class SPartyReportRepository : ISPartyReportRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public SPartyReportRepository(IMenuRepository menuRepository)
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
        public MyHttpResponseMessage UpdateSodeBookFeedingReport(SodePartyReport modelrecord, Common common)
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

                List<object> jsonDataResult = new List<object>();

                string connectionString = new SQLService().getconnstring();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = connection.CreateCommand();
                    command.Transaction = transaction;
                    foreach (var item in modelrecord.Detail.ToList())
                    {
                        string query = $"UPDATE {detailTable} SET SCOMP = 1 WHERE DT_CODE = {item.dT_CODE}";
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
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
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }
        public MyHttpResponseMessage GetReportData(SPartyReport report, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty, pickTableMaster = string.Empty, pickTableDetail = string.Empty;
                decimal totalAmt = 0;
                decimal totalIQTY = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                    pickTableMaster = menu.PICK_TABLE_MASTER;
                    pickTableDetail = menu.PICK_TABLE_DETAIL;
                }
                var validReportIds = new HashSet<int?> { 13, 15, 17};
                if (validReportIds.Contains(report.ReportID))
                {
                    report.Item = report.Item = report.Item ?? 0;
                    report.BuyerPartyCode = report.BuyerPartyCode = report.BuyerPartyCode ?? 0;
                    report.BuyerRegionCode = report.BuyerRegionCode = report.BuyerRegionCode ?? 0;
                    report.SellerPartyCode = report.SellerPartyCode = report.SellerPartyCode ?? 0;
                    report.BuyerControlCode = report.BuyerControlCode = report.BuyerControlCode ?? "";
                    report.SellerControlCode = report.SellerControlCode = report.SellerControlCode ?? "";
                    report.SellerRegionCode = report.SellerRegionCode = report.SellerRegionCode ?? 0;
                    report.ArivalStatus = report.ArivalStatus = report.ArivalStatus ?? "";
                }

                switch (report.ReportID)
                {
                    case 50:
                        report.SbfType = "LOC";
                        break;
                    case 68:
                        report.SbfType = "IMP";
                        break;
                    case 69:
                        report.SbfType = "EXP";
                        break;
                }

                List<CustomSPartyReport> jsonDataResult = new List<CustomSPartyReport>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query;
                    if (report.ReportID == 15)
                    {
                        query = "SELECT CASE WHEN A.ARIVAL_STATUS = 'G' THEN 'Godown' ELSE CASE WHEN A.ARIVAL_STATUS = 'P' THEN 'Port' END END AS ARIVAL_STATUS," +
                                    $"A.V_DATE, A.REMARKS, A.VOUCHER_NO, PTB.PARTY_NAME AS BUYER_NAME, PTS.PARTY_NAME AS SELLER_NAME, IT.ITEM_NAME, B.QTY, U.GROUP_NAME AS UNIT FROM {table} A " +
                                    $"LEFT OUTER JOIN {detailTable} B ON A.TRAN_ID = B.TRAN_ID AND A.BCODE = B.BCODE AND A.PERIOD_ID = B.PERIOD_ID " +
                                    "LEFT OUTER JOIN TBL_PARTY_TYPES PTB ON PTB.PARTY_CODE = A.BUYER_CODE AND PTB.ACT_CODE = A.BACT_CODE " +
                                    "LEFT OUTER JOIN TBL_PARTY_TYPES PTS ON PTS.PARTY_CODE = A.SELLER_CODE AND PTS.ACT_CODE = A.SACT_CODE " +
                                    "LEFT OUTER JOIN TBL_ITEMSMASTER IT ON IT.ITEM_CODE = B.ITEM_CODE " +
                                    "LEFT OUTER JOIN TBL_UNIT U ON U.GROUP_CODE = B.UNIT " +
                                    "WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' " +
                                    $"AND A.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}' " +
                                    $"AND IT.ITEM_CODE = CASE WHEN {report.Item} <> 0 THEN {report.Item} ELSE IT.ITEM_CODE END " +
                                    $"AND A.ARIVAL_STATUS = CASE WHEN '{report.ArivalStatus}' <> '' THEN '{report.ArivalStatus}' ELSE A.ARIVAL_STATUS END " +
                                    $"AND PTB.PARTY_CODE = CASE WHEN {report.BuyerPartyCode} <> 0 THEN {report.BuyerPartyCode} ELSE PTB.PARTY_CODE END " +
                                    $"AND PTB.ACT_CODE = CASE WHEN '{report.BuyerControlCode}' <> '' THEN '{report.BuyerControlCode}' ELSE PTB.ACT_CODE END " +
                                    $"AND PTB.REGION = CASE WHEN {report.BuyerRegionCode} <> 0 THEN {report.BuyerRegionCode} ELSE PTB.REGION END " +
                                    $"AND A.SELLER_CODE = CASE WHEN {report.SellerPartyCode} <> 0 THEN {report.SellerPartyCode} ELSE A.SELLER_CODE END " +
                                    $"AND A.SACT_CODE = CASE WHEN '{report.SellerControlCode}' <> '' THEN '{report.SellerControlCode}' ELSE A.SACT_CODE END ORDER BY A.V_DATE DESC";
                        //+ $"AND PTS.REGION = CASE WHEN {report.SellerRegionCode} <> 0 AND A.SELLER_CODE <> 0 THEN {report.SellerRegionCode} ELSE PTS.REGION END";
                    }
                    //else if (report.ReportID == 14 || report.ReportID == 40 || report.ReportID == 50 || report.ReportID == 68 || report.ReportID == 69)
                    //{
                    //    query = "SELECT A.V_DATE, A.TRAN_ID, MNU.MENU_PAGE, MNU.MENU_PARENT_CODE, MNU.ID AS MENU_ID, A.REMARKS, A.VOUCHER_NO, PTB.PARTY_NAME AS BUYER_NAME, PTS.PARTY_NAME AS SELLER_NAME, PTBK.PARTY_NAME AS BROKER_NAME, " +
                    //                "CASE WHEN A.COND = 'CR' THEN CONVERT(NVARCHAR(50),A.CREDIT_DAYS)+' Days' ELSE CASE WHEN A.COND  = 'Cash' THEN A.COND ELSE CASE WHEN A.COND  = 'Adv' THEN 'Advance' ELSE CASE WHEN A.COND  = 'CRD' THEN CONVERT(NVARCHAR(50), A.DUE_DATE, 105) END END END END AS COND, " +
                    //                $"IT.ITEM_NAME, B.QTY, U.GROUP_NAME AS UNIT , B.RATE, CONVERT(NVARCHAR(50),B.RT_TYPE)+' Kg'as Rt_type, B.AMT FROM {table} A " +
                    //                $"LEFT OUTER JOIN {detailTable} B ON A.TRAN_ID = B.TRAN_ID AND A.BCODE = B.BCODE AND A.PERIOD_ID = B.PERIOD_ID " +
                    //                "LEFT OUTER JOIN TBL_PARTY_TYPES PTB ON PTB.PARTY_CODE = A.BUYER_CODE AND PTB.ACT_CODE = A.BACT_CODE " +
                    //                "LEFT OUTER JOIN TBL_PARTY_TYPES PTS ON PTS.PARTY_CODE = A.SELLER_CODE AND PTS.ACT_CODE = A.SACT_CODE " +
                    //                "LEFT OUTER JOIN TBL_PARTY_TYPES PTBK ON PTBK.PARTY_CODE = A.BROKER_CODE AND PTBK.ACT_CODE = A.BD_ACT_CODE " +
                    //                "LEFT OUTER JOIN TBL_ITEMSMASTER IT ON IT.ITEM_CODE = B.ITEM_CODE " +
                    //                "LEFT OUTER JOIN TBL_UNIT U ON U.GROUP_CODE = B.UNIT " +
                    //                "LEFT OUTER JOIN TBL_REGION RS ON RS.CODE = PTS.REGION " +
                    //                "LEFT OUTER JOIN TBL_REGION RB ON RB.CODE = PTB.REGION " +
                    //                "LEFT OUTER JOIN TBL_MENU_BUILDER MNU ON MNU.ID = A.MENU_ID  " +
                    //                "WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' " +
                    //                $"AND A.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}' " +
                    //                $"AND IT.ITEM_CODE = CASE WHEN {report.Item} <> 0 THEN {report.Item} ELSE IT.ITEM_CODE END " +
                    //                $"AND PTB.PARTY_CODE = CASE WHEN {report.BuyerPartyCode} <> 0 THEN {report.BuyerPartyCode} ELSE PTB.PARTY_CODE END " +
                    //                $"AND PTB.ACT_CODE = CASE WHEN '{report.BuyerControlCode}' <> '' THEN '{report.BuyerControlCode}' ELSE PTB.ACT_CODE END " +
                    //                $"AND PTS.PARTY_CODE = CASE WHEN {report.SellerPartyCode} <> 0 THEN {report.SellerPartyCode} ELSE PTS.PARTY_CODE END " +
                    //                $"AND PTS.ACT_CODE = CASE WHEN '{report.SellerControlCode}' <> '' THEN '{report.SellerControlCode}' ELSE PTS.ACT_CODE END " +
                    //                $"AND (A.SBF_TYPE = CASE WHEN '{report.SbfType}' <> '' THEN '{report.SbfType}' ELSE A.SBF_TYPE END OR ('{report.SbfType}' = '' AND (A.SBF_TYPE IS NULL OR A.SBF_TYPE = '')))" +
                    //                $"AND (RB.GR_CODE LIKE CASE WHEN  '{report.BuyerRegionGr}' <>'' THEN '' +'{report.BuyerRegionGr}' +'%' ELSE RB.GR_CODE END) " +
                    //                $"AND (RS.GR_CODE LIKE CASE WHEN  '{report.SellerRegionGr}' <>'' THEN '' +'{report.SellerRegionGr}' +'%' ELSE RS.GR_CODE END) " +
                    //                $"ORDER BY A.V_DATE DESC";
                    //    //$"AND PTB.REGION = CASE WHEN {report.BuyerRegionCode} <> 0 THEN {report.BuyerRegionCode} ELSE PTB.REGION END " +
                    //    //$"AND PTS.REGION = CASE WHEN {report.SellerRegionCode} <> 0 THEN {report.SellerRegionCode} ELSE PTS.REGION END ORDER BY A.V_DATE DESC";
                    //}
                    else if (report.ReportID == 17)
                    {
                        query = "CREATE TABLE #TempStockTable (V_DATE DATE, VOUCHER_NO NVARCHAR(50), PARTY_CODE NVARCHAR(50), PARTY_NAME NVARCHAR(200), ACT_CODE NVARCHAR(50), " +
                            "REGION NVARCHAR(50), ITEM_CODE NVARCHAR(50), ITEM_NAME NVARCHAR(200), STOCK_IN DECIMAL(18,2), STOCK_OUT DECIMAL(18,2), UNIT NVARCHAR(50), BCODE INT, PERIOD_ID INT,AMT_DEBIT FLOAT, AMT_CREDIT FLOAT);" +
                            "INSERT INTO #TempStockTable (V_DATE, VOUCHER_NO, PARTY_CODE,PARTY_NAME, ACT_CODE, REGION, ITEM_CODE, ITEM_NAME, STOCK_IN, STOCK_OUT, UNIT, BCODE, PERIOD_ID, AMT_DEBIT, AMT_CREDIT)" +
                            "SELECT M.V_DATE, M.VOUCHER_NO, PT.PARTY_CODE, PT.PARTY_NAME AS PARTY_NAME, PT.ACT_CODE, PT.REGION, IT.ITEM_CODE, IT.ITEM_NAME, D.QTY AS STOCK_IN, " +
                            "0.00 AS STOCK_OUT, U.GROUP_NAME AS UNIT, M.BCODE, M.PERIOD_ID, D.AMT AS AMT_DEBIT, 0 AS AMT_CREDIT " +
                            "FROM TBL_DF_MASTER M " +
                            "LEFT OUTER JOIN TBL_DF_DETAIL D ON D.TRAN_ID = M.TRAN_ID AND D.PERIOD_ID = M.PERIOD_ID AND D.BCODE = M.BCODE " +
                            "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = M.SELLER_CODE AND PT.ACT_CODE = M.SACT_CODE " +
                            "LEFT OUTER JOIN TBL_ITEMSMASTER IT ON IT.ITEM_CODE = D.ITEM_CODE " +
                            "LEFT OUTER JOIN TBL_UNIT U ON U.GROUP_CODE = D.UNIT WHERE M.DLT = 'T' AND D.DLT = 'T' " +
                            "UNION ALL " +
                            "SELECT M.V_DATE, M.VOUCHER_NO, PT.PARTY_CODE, PT.PARTY_NAME AS PARTY_NAME, PT.ACT_CODE, PT.REGION, IT.ITEM_CODE, " +
                            "IT.ITEM_NAME, 0.00 AS STOCK_IN, D.QTY AS STOCK_OUT, U.GROUP_NAME AS UNIT, M.BCODE, M.PERIOD_ID, 0 AS AMT_DEBIT, D.AMT AS AMT_CREDIT " +
                            "FROM TBL_DF_MASTER M " +
                            "LEFT OUTER JOIN TBL_DF_DETAIL D ON D.TRAN_ID = M.TRAN_ID AND D.PERIOD_ID = M.PERIOD_ID AND D.BCODE = M.BCODE " +
                            "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = M.BUYER_CODE AND PT.ACT_CODE = M.BACT_CODE " +
                            "LEFT OUTER JOIN TBL_ITEMSMASTER IT ON IT.ITEM_CODE = D.ITEM_CODE " +
                            "LEFT OUTER JOIN TBL_UNIT U ON U.GROUP_CODE = D.UNIT WHERE M.DLT = 'T' AND D.DLT = 'T'  ; " +
                            "SELECT V_DATE, VOUCHER_NO, PARTY_NAME, ITEM_NAME, SUM(STOCK_IN) AS TOTAL_STOCK_IN, SUM(STOCK_OUT) AS TOTAL_STOCK_OUT, UNIT, SUM(AMT_DEBIT) AS AMT_DEBIT, SUM(AMT_CREDIT) AS AMT_CREDIT " +
                            "FROM #TempStockTable " +
                            "WHERE " +
                            $"PARTY_CODE = CASE WHEN {report.BuyerPartyCode} <> 0 THEN {report.BuyerPartyCode} ELSE PARTY_CODE END " +
                            $"AND ACT_CODE = CASE WHEN '{report.BuyerControlCode}' <> '' THEN '{report.BuyerControlCode}' ELSE ACT_CODE END " +
                            $"AND REGION = CASE WHEN {report.BuyerRegionCode} <> 0 THEN {report.BuyerRegionCode} ELSE REGION END " +
                            $"AND ITEM_CODE = CASE WHEN {report.Item} <> 0 THEN {report.Item} ELSE ITEM_CODE END " +
                            $"AND V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}' AND BCODE = 1 AND PERIOD_ID = 1 " +
                            "GROUP BY V_DATE, VOUCHER_NO, PARTY_NAME, ACT_CODE, ITEM_NAME, UNIT; " +
                            "DROP TABLE #TempStockTable";
                    }
                    //else if (report.ReportID == 72)
                    //{
                    //    query = "SELECT SD.SCOMP,SM.V_DATE, SM.TRAN_ID, MNU.MENU_PAGE, MNU.MENU_PARENT_CODE, MNU.ID AS MENU_ID, SM.VOUCHER_NO, UN.GROUP_NAME AS UNIT_NAME, PTS.PARTY_NAME AS SELLER, " +
                    //        $"PTB.PARTY_NAME AS BUYER, IM.ITEM_NAME, (ISNULL((SELECT SUM(ISNULL(BAL_QTY,0)) FROM {detailTable} " +
                    //        "WHERE SD.DT_CODE = DT_CODE AND SD.PERIOD_ID = PERIOD_ID AND SD.BCODE = BCODE),0)) AS SQTY, " +
                    //        "ISNULL((SELECT SUM(ISNULL(BAL_QTY, 0)) FROM TBL_DF_DETAIL " +
                    //        "WHERE DFD.PICK_ID = PICK_ID AND DFD.PERIOD_ID = PERIOD_ID AND DFD.BCODE = BCODE),0) AS DQTY, " +
                    //        $"(ISNULL((SELECT SUM(ISNULL(BAL_QTY, 0)) FROM {detailTable} " +
                    //        "WHERE SD.DT_CODE = DT_CODE AND SD.PERIOD_ID = PERIOD_ID AND SD.BCODE = BCODE), 0) - ISNULL((SELECT SUM(ISNULL(BAL_QTY,0)) FROM TBL_DF_DETAIL" +
                    //        " WHERE DFD.PICK_ID = PICK_ID AND DFD.PERIOD_ID = PERIOD_ID AND DFD.BCODE = BCODE),0)) " +
                    //        $"AS BAL_QTY, SD.RATE,SD.DT_CODE FROM {table} SM LEFT OUTER JOIN {detailTable} SD " +
                    //        "ON SD.TRAN_ID = SM.TRAN_ID AND SD.BCODE = SM.BCODE AND SD.PERIOD_ID = SM.PERIOD_ID " +
                    //        "LEFT OUTER JOIN TBL_DF_DETAIL DFD ON SD.DT_CODE = DFD.PICK_ID AND SD.BCODE = DFD.BCODE AND SD.PERIOD_ID = DFD.PERIOD_ID AND DFD.DLT = 'T' " +
                    //        "LEFT OUTER JOIN TBL_MENU_BUILDER MNU ON MNU.ID = SM.MENU_ID  " +
                    //        "LEFT OUTER JOIN TBL_PARTY_TYPES PTS ON PTS.PARTY_CODE = SM.SELLER_CODE AND PTS.ACT_CODE = SM.SACT_CODE LEFT OUTER JOIN TBL_PARTY_TYPES PTB " +
                    //        "ON PTB.PARTY_CODE = SM.BUYER_CODE AND PTB.ACT_CODE = SM.BACT_CODE LEFT OUTER JOIN TBL_PARTY_TYPES PTBR " +
                    //        "ON PTBR.PARTY_CODE = SM.BROKER_CODE AND PTBR.ACT_CODE = SM.BD_ACT_CODE LEFT OUTER JOIN TBL_ITEMSMASTER IM " +
                    //        "ON IM.ITEM_CODE = SD.ITEM_CODE LEFT OUTER JOIN TBL_UNIT UN ON SD.UNIT = UN.GROUP_CODE WHERE SM.DLT = 'T' AND SM.ASTATUS = 'Y' AND SD.DLT = 'T' " +
                    //        $"AND SD.SCOMP = 0 AND SM.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}'" +
                    //        @$"AND IM.ITEM_CODE IN (
                    //                        SELECT M.ITEM_CODE
                    //                        FROM TBL_ROLE R 
                    //                        LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE 
                    //                        LEFT OUTER JOIN TBL_ITEMSGROUP IG ON M.GROUP_CODE = IG.GROUP_CODE 
                    //                        WHERE R.ROLE_TYPE = '{common.RoleType}'
                    //                          AND R.MODULE_ID = 4 
                    //                          AND IG.GROUP_TYPE = 'S' 
                    //                          AND IG.DLT = 'T' 
                    //                          AND IG.ASTATUS = 'Y'
                    //                          AND R.ROLE_ID = {common.RoleID}
                    //                        GROUP BY M.ITEM_CODE
                                        
                    //                        UNION ALL
                                        
                    //                        SELECT M.ITEM_CODE 
                    //                        FROM TBL_ITEMSMASTER M 
                    //                        WHERE '{common.RoleType}' = 'A'   
                    //                          AND M.DLT = 'T' 
                    //                          AND M.ASTATUS = 'Y'
                                        
                    //                        UNION ALL
                                        
                    //                        SELECT M.ITEM_CODE 
                    //                        FROM TBL_ITEMSMASTER M 
                    //                        WHERE '{common.RoleType}' = 'U'  
                    //                          AND M.DLT = 'T' 
                    //                          AND M.ASTATUS = 'Y' 
                    //                          AND (
                    //                            SELECT COUNT(R.MODULE_ID) 
                    //                            FROM TBL_ROLE R 
                    //                            LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE 
                    //                            LEFT OUTER JOIN TBL_ITEMSGROUP IG ON M.GROUP_CODE = IG.GROUP_CODE 
                    //                            WHERE R.ROLE_TYPE = '{common.RoleType}'
                    //                              AND R.MODULE_ID = 4 
                    //                              AND IG.GROUP_TYPE = 'S' 
                    //                              AND IG.DLT = 'T' 
                    //                              AND IG.ASTATUS = 'Y'
                    //                              AND R.ROLE_ID = {common.RoleID}
                    //                          ) < 1
                    //                    )" +
                    //        "GROUP BY SM.TRAN_ID, MNU.MENU_PAGE, MNU.MENU_PARENT_CODE, MNU.ID , SM.V_DATE, SM.VOUCHER_NO,SM.REF,SM.REMARKS, PTS.PARTY_NAME , PTB.PARTY_NAME, PTBR.PARTY_NAME " +
                    //        ",SD.ITEM_CODE, IM.ITEM_NAME, SD.RATE, SD.AMT, SD.DT_CODE, SD.PERIOD_ID, SD.BCODE, DFD.PICK_ID, DFD.BCODE, DFD.PERIOD_ID, SD.UNIT, SM.SELLER_CODE, SM.SACT_CODE, UN.GROUP_NAME, SM.BUYER_CODE, SM.BACT_CODE, SM.BROKER_CODE, SM.BD_ACT_CODE, SM.COND, SM.CREDIT_DAYS, SM.DUE_DATE, SD.RT_TYPE,UN.QTY,SD.SCOMP";
                    //}
                    else if (report.ReportID is 14 or 40 or 50 or 68 or 69 or 72 or 77 or 78 or 79 or 80 or 81 or 82 or 83 or 84 or 123 or 124)
                    {
                        query = $"EXEC SODA_SUMMARY {report.ReportID},'{report.FromDate.Value.ToString("yyyy-MM-dd")}','{report.ToDate.Value.ToString("yyyy-MM-dd")}','{common.Branch}','{common.Period}','{report.Item}','{report.SellerPartyCode}','{report.SellerControlCode}','{report.BuyerPartyCode}','{report.BuyerControlCode}','{table}','{detailTable}','{pickTableMaster}','{pickTableDetail}','{report.SbfType}'";
                    }
                    else
                    {
                        string insuranceCondition = report.Insurance ? "AND (B.INS IS NOT NULL AND B.INS <> 0) " : "";
                        query = "SELECT A.V_DATE, A.REMARKS, A.VOUCHER_NO, PTB.PARTY_NAME AS BUYER_NAME, PTS.PARTY_NAME AS SELLER_NAME, PTBr.PARTY_NAME AS BROKER_NAME, IT.ITEM_NAME, B.QTY, U.GROUP_NAME AS UNIT," +
                            $"B.INS AS INSURANCE " +
                            $"FROM {table} A " +
                            $"LEFT OUTER JOIN {detailTable} B ON A.TRAN_ID = B.TRAN_ID AND A.BCODE = B.BCODE AND A.PERIOD_ID = B.PERIOD_ID " +
                            "LEFT OUTER JOIN TBL_PARTY_TYPES PTB ON PTB.PARTY_CODE = A.BUYER_CODE AND PTB.ACT_CODE = A.BACT_CODE " +
                            "LEFT OUTER JOIN TBL_PARTY_TYPES PTS ON PTS.PARTY_CODE = A.SELLER_CODE AND PTS.ACT_CODE = A.SACT_CODE " +
                            "LEFT OUTER JOIN TBL_PARTY_TYPES PTBr ON PTBr.PARTY_CODE = A.BROKER_CODE AND PTBr.ACT_CODE = A.BD_ACT_CODE " +
                            "LEFT OUTER JOIN TBL_ITEMSMASTER IT ON IT.ITEM_CODE = B.ITEM_CODE " +
                            "LEFT OUTER JOIN TBL_UNIT U ON U.GROUP_CODE = B.UNIT " +
                            "WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' " +
                            $"AND A.V_DATE BETWEEN '{report.FromDate.Value.ToString("yyyy-MM-dd")}' AND '{report.ToDate.Value.ToString("yyyy-MM-dd")}' " +
                            $"AND IT.ITEM_CODE = CASE WHEN {report.Item} <> 0 THEN {report.Item} ELSE IT.ITEM_CODE END " +
                            $"AND PTB.PARTY_CODE = CASE WHEN {report.BuyerPartyCode} <> 0 THEN {report.BuyerPartyCode} ELSE PTB.PARTY_CODE END " +
                            $"AND PTB.ACT_CODE = CASE WHEN '{report.BuyerControlCode}' <> '' THEN '{report.BuyerControlCode}' ELSE PTB.ACT_CODE END " +
                            $"AND PTB.REGION = CASE WHEN {report.BuyerRegionCode} <> 0 THEN {report.BuyerRegionCode} ELSE PTB.REGION END " +
                            $"AND PTS.PARTY_CODE = CASE WHEN {report.SellerPartyCode} <> 0 THEN {report.SellerPartyCode} ELSE PTS.PARTY_CODE END " +
                            $"AND PTS.ACT_CODE = CASE WHEN '{report.SellerControlCode}' <> '' THEN '{report.SellerControlCode}' ELSE PTS.ACT_CODE END " +
                            $"AND PTS.REGION = CASE WHEN {report.SellerRegionCode} <> 0 THEN {report.SellerRegionCode} ELSE PTS.REGION END " +
                            $"{insuranceCondition} ORDER BY A.V_DATE DESC";
                    }
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (report.ReportID == 15)
                        {
                            var row = new CustomSPartyReport
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
                        //else if (report.ReportID == 14 || report.ReportID == 40 || report.ReportID == 50 || report.ReportID == 68 || report.ReportID == 69)
                        //{
                        //    var qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]);
                        //    var amt = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);
                        //    string avg = "0.00";

                        //    if (qty != 0)
                        //    {
                        //        avg = (amt / qty).ToString("0.00");
                        //    }

                        //    var row = new CustomSPartyReport
                        //    {
                        //        VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                        //        VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                        //        BuyerName = Convert.ToString(reader["BUYER_NAME"]),
                        //        BrokerName = Convert.ToString(reader["BROKER_NAME"]),
                        //        SellerName = Convert.ToString(reader["SELLER_NAME"]),
                        //        ItemName = Convert.ToString(reader["ITEM_NAME"]),
                        //        QTY = Convert.ToString(reader["QTY"]),
                        //        UNIT = Convert.ToString(reader["UNIT"]),
                        //        COND = Convert.ToString(reader["COND"]),
                        //        RATE = Convert.ToString(reader["RATE"]),
                        //        RT_TYPE = Convert.ToString(reader["Rt_type"]),
                        //        AMT = Convert.ToString(reader["AMT"]),
                        //        AVG = avg,
                        //        Remarks = Convert.ToString(reader["REMARKS"]),
                        //        LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                        //        TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                        //    };
                        //    jsonDataResult.Add(row);
                        //}
                        else if (report.ReportID == 17)
                        {
                            var row = new CustomSPartyReport
                            {
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                BuyerName = Convert.ToString(reader["PARTY_NAME"]),
                                ItemName = Convert.ToString(reader["ITEM_NAME"]),
                                TOTAL_STOCK_IN = Convert.ToString(reader["TOTAL_STOCK_IN"]),
                                TOTAL_STOCK_OUT = Convert.ToString(reader["TOTAL_STOCK_OUT"]),
                                AMT_CREDIT = Convert.ToString(reader["AMT_CREDIT"]),
                                AMT_DEBIT = Convert.ToString(reader["AMT_DEBIT"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        //else if (report.ReportID == 72)
                        //{
                        //    var row = new CustomSPartyReport
                        //    {
                        //        VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                        //        VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                        //        UNIT = Convert.ToString(reader["UNIT_NAME"]),
                        //        SellerName = Convert.ToString(reader["SELLER"]),
                        //        BuyerName = Convert.ToString(reader["BUYER"]),
                        //        ItemName = Convert.ToString(reader["ITEM_NAME"]),
                        //        SQTY = Convert.ToString(reader["SQTY"]),
                        //        DQTY = Convert.ToString(reader["DQTY"]),
                        //        QTY = Convert.ToString(reader["BAL_QTY"]),
                        //        RATE = Convert.ToString(reader["RATE"]),
                        //        DT_CODE = Convert.ToInt32(reader["DT_CODE"]),
                        //        LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                        //        TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                        //    };
                        //    jsonDataResult.Add(row);
                        //}
                        else if (report.ReportID is 14 or 40 or 50 or 68 or 69 or 72 or 77 or 78 or 79 or 80 or 81 or 82 or 83 or 84 or 123 or 124)
                        {
                            var Qty = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]);
                            var rate = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]);
                            var amt = Qty * rate;

                            string avg = "0.00";
                            //if (Qty != 0)
                            //{
                            //    avg = (rate * Qty).ToString("0.00");
                            //}

                            var row = new CustomSPartyReport
                            {
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                ItemName = Convert.ToString(reader["ITEM_NAME"]),
                                ItemNameWithAvg = Convert.ToString(reader["ITEM_NAME"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                RATE1 = Convert.ToString(reader["RATE1"]),
                                OQTY = Convert.ToString(reader["OQTY"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                IQTY = Convert.ToString(reader["IQTY"]),
                                BQTY = (Convert.ToDecimal(reader["OQTY"]) - Convert.ToDecimal(reader["IQTY"])).ToString(),
                                SellerName = Convert.ToString(reader["SELLER"]),
                                BuyerName = Convert.ToString(reader["BUYER"]),
                                LINK = "/" + Convert.ToString(reader["MENU_PAGE"]) + "?MOID=" + Convert.ToString(reader["MENU_PARENT_CODE"]) + "&Code=" + Convert.ToString(reader["MENU_ID"]),
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                DT_CODE = Convert.ToInt32(reader["DT_CODE"]),
                                COND = Convert.ToString(reader["COND"]),
                                DUE_DATE = reader["DUE_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["DUE_DATE"]).ToString("dd/MM/yyyy"),
                                CREDIT_DAYS = Convert.ToInt32(reader["CREDIT_DAYS"]),
                                BrokerName = Convert.ToString(reader["BROKER_NAME"]),
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
                                AVG = Convert.ToString(reader["AMT"]),
                                //AVG = Convert.ToString(amt),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                //AMT = Convert.ToString(amt),
                                AMT = Convert.ToString(reader["AMT"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        else
                        {
                            var row = new CustomSPartyReport
                            {
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                BuyerName = Convert.ToString(reader["BUYER_NAME"]),
                                BrokerName = Convert.ToString(reader["BROKER_NAME"]),
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