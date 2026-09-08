using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Reflection.Metadata;
using System.Text;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class SaleTaxInvoiceRepository : ISaleTaxInvoiceRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public ICommonService _commonService { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public IPeriodRepository _periodRepository { get; set; }

        public SaleTaxInvoiceRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, ICommonRepository commonRepository, ICommonService commonService, IPeriodRepository periodRepository)
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
                            string query = $@"SELECT M.TRAN_ID AS TRAN_ID, M.TRAN_ID AS CODE,M.V_DATE,M.VOUCHER_NO,M.BTYPE,PT.PARTY_NAME,DOC,M.ACT_CODE,M.REF,M.REMARKS,
                                             CASE WHEN M.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS 
                                            FROM {table} M 
                                            LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = M.PARTY_CODE AND PT.ACT_CODE = M.ACT_CODE 
                                            WHERE  M.DLT = 'T' AND M.BCODE = '{common.Branch}' AND M.PERIOD_ID = '{common.Period}' ORDER BY M.TRAN_ID DESC";

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
                                    //PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                    ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                    REF = Convert.ToString(reader["REF"]),
                                    PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]),
                                    DOC = Convert.ToString(reader["DOC"]),
                                    BTYPE = Convert.ToString(reader["BTYPE"]),
                                    REMARKS = Convert.ToString(reader["REMARKS"]),
                                    
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


        public MyHttpResponseMessage GetSaleTaxInvoiceByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
             
            List<SaleTaxInvoice> saleTaxInvoice = new List<SaleTaxInvoice>();
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
                    //List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"SELECT * FROM {table} WHERE DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period} AND TRAN_ID = {code}";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new SaleTaxInvoice
                            {
                                TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]),
                                V_DATE_STRING = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                PARTY_CODE = reader["PARTY_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ACT_CODE"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                BTYPE = Convert.ToString(reader["BTYPE"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                FBR_NO = Convert.ToString(reader["FBR_NO"]),
                                DOC = Convert.ToString(reader["DOC"]),
                            };
                            saleTaxInvoice.Add(row);
                        }
                        reader.Close();
                    }

                    response.data = saleTaxInvoice;
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
        public MyHttpResponseMessage GetSaleTaxInvoiceDetailByCode(int code, Common common)
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

                        string query = $@"SELECT * FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' 
                                        AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                ITEM_CODE = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                QTY = reader["QTY"] == DBNull.Value ? 0 : Convert.ToInt32(reader["QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_AMT = Convert.ToString(reader["DISC_AMT"]),
                                TAX = Convert.ToString(reader["TAX"]),
                                TAX_AMT = Convert.ToString(reader["TAX_AMT"]),
                                NET_AMT = Convert.ToString(reader["NET_AMT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                                ITEM_SNO = Convert.ToString(reader["ITEM_SNO"]),
                                HS_CODE = Convert.ToString(reader["HS_CODE"]),
                                SCHEDULE_NO = Convert.ToString(reader["SCHEDULE_NO"]),
                                SERIAL_NO = Convert.ToString(reader["SERIAL_NO"]),
                                UNIT = reader["UNIT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["UNIT"]),
                                FBR_TYPE = reader["FBR_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["FBR_TYPE"]),

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

        private int GenerateNextId(Common common, SqlCommand command, string Commission = "")
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

                    //if (Commission == "")
                    //{
                        
                    //}
                    //else
                    //{
                    //    string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM TBL_COMM_GEN WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                    //    command.CommandText = maxIdQuery;
                    //    object result = command.ExecuteScalar();
                    //    return Convert.ToInt32(result);
                    //}

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

        private int GenerateNextDetailId(Common common, SqlCommand command, string Commission = "")
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
                    if (Commission == "")
                    {
                        string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) FROM {table}";
                        command.CommandText = maxIdQuery;
                        object result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                    else
                    {
                        string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) + 1 FROM TBL_COMM_GEN";
                        command.CommandText = maxIdQuery;
                        object result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public MyHttpResponseMessage Save(CustomSaleTaxInvoice modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
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
                    //stk_status = menu.STK_STATUS;
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
                    DataTable dt = new DataTable();
                    string connectionString = new SQLService().getconnstring();

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;
                        try
                        {

                            string query = "",  detailQuery = "", voucherNo = string.Empty;
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

                                query = $@"INSERT INTO {table}
                                        (TRAN_ID, V_DATE, VOUCHER_NO, PARTY_CODE, ACT_CODE, 
                                        REF, REMARKS, BTYPE, BCODE, PERIOD_ID, DOC, ADD_USER_ID, 
                                        ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE,
                                        EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE,
                                        EDIT_POSTALCODE,ASTATUS,MENU_ID,DLT)
                                        VALUES
                                        ('{code}', '{modelRecord.Master.V_DATE}', '{voucherNo}', '{modelRecord.Master.PARTY_CODE}', '{modelRecord.Master.ACT_CODE}', 
                                        '{modelRecord.Master.REF}', '{modelRecord.Master.REMARKS}', '{modelRecord.Master.BTYPE}', '{branch}', '{period}', '{modelRecord.Master.DOC}', '{username}',
                                        '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', 
                                        '{Computer}', '{Ip}', '{Postal}', '{Postal}', '{modelRecord.Master.ASTATUS}', '{menuID}', 'T')";

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
                                                    BTYPE = '" + modelRecord.Master.BTYPE + @"',
                                                    DOC = '" + modelRecord.Master.DOC + @"',

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
                            //if (modelRecord.Detail.Count > 0)
                            //{
                            //    detailQuery = $"UPDATE {detailTable} SET DLT = 'F'" +
                            //    $" WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                            //    command.CommandText = detailQuery;
                            //    command.ExecuteNonQuery();
                            //}

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
                                            detailQuery = $"INSERT INTO {detailTable} (TRAN_ID, DT_CODE, ITEM_CODE, UNIT, QTY, RATE, AMT, DISC, ITEM_SNO, HS_CODE, FBR_TYPE, SCHEDULE_NO, SERIAL_NO, DISC_AMT, TAX, TAX_AMT, NET_AMT, DT_DESC, BCODE, PERIOD_ID, " +
                                                $"ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT) VALUES " +
                                                $"('{modelRecord.Master.TRAN_ID}', '{detailCode}', '{item.ITEM_CODE}', '{item.UNIT}', '{item.QTY}', " +
                                                $"'{item.RATE}', '{item.AMT}', '{item.DISC}', '{item.ITEM_SNO}', '{item.HS_CODE}', '{item.FBR_TYPE}', '{item.SCHEDULE_NO}', '{item.SERIAL_NO}', " +
                                                $"'{item.DISC_AMT}', '{item.TAX}', '{item.TAX_AMT}', '{item.NET_AMT}', '{item.DT_DESC}', " +
                                                $"'{branch}', '{period}', '{username}', " +
                                                $"'{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', '{username}', " +
                                                $"'{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', " +
                                                $"'{Postal}', '{Postal}', '{menuID}', 'T');";
                                        }
                                        else
                                        {
                                            isDetailAdded = false;
                                        }
                                    }
                                    else
                                    {
                                        detailQuery = $"UPDATE {detailTable} SET " +
                                            $"ITEM_CODE = '{item.ITEM_CODE}', QTY = '{item.QTY}', UNIT = '{item.UNIT}', " +
                                            $"RATE = '{item.RATE}',  ITEM_SNO = '{item.ITEM_SNO}', HS_CODE = '{item.HS_CODE}', " +
                                            $"AMT = '{item.AMT}', DISC = '{item.DISC}', DISC_AMT = '{item.DISC_AMT}', " +
                                            $"TAX = '{item.TAX}', TAX_AMT = '{item.TAX_AMT}', NET_AMT = '{item.NET_AMT}', " +
                                            $"DT_DESC = '{item.DT_DESC}', FBR_TYPE = '{item.FBR_TYPE}', SCHEDULE_NO = '{item.SCHEDULE_NO}', SERIAL_NO = '{item.SERIAL_NO}', " +
                                            $"EDIT_USER_ID = '{username}', " +
                                            $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                            $"EDIT_COMPUTER_NAME = '{Computer}', EDIT_IP_ADDRESS = '{Ip}', " +
                                            $"EDIT_POSTALCODE = '{Postal}', DLT = 'T' " +
                                            $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND DT_CODE = '{item.DT_CODE}' " +
                                            $"AND BCODE = '{branch}' AND PERIOD_ID = '{period}';";
                                    }

                                    command.CommandText = detailQuery.ToString();
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception)
                                {
                                    isDetailAdded = false;
                                }
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

                SaleTaxInvoice saleTaxInvoice = new SaleTaxInvoice();
                List<SaleTaxInvoiceDetail> saleTaxInvoiceDetailList = new List<SaleTaxInvoiceDetail>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        saleTaxInvoice = new SaleTaxInvoice
                        {
                            V_DATE = Convert.ToDateTime(reader["V_DATE"]),
                            PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            //SCODE = Convert.ToInt32(reader["SCODE"]),
                            //SACODE = Convert.ToInt32(reader["SACODE"]),
                            //COMM = Convert.ToDouble(reader["COMM"]),
                            //REF = Convert.ToString(reader["REF"]),
                            //REMARKS = Convert.ToString(reader["REMARKS"]),
                            //BTYPE = Convert.ToString(reader["BTYPE"]),
                            //BCODE = Convert.ToInt32(reader["BCODE"]),
                            //ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            //DISC = Convert.ToDouble(reader["DISC"]),
                            //HS_CODE = Convert.ToString(reader["HS_CODE"]),
                            //DOC = Convert.ToString(reader["DOC"]),
                            //CURR_CODE = Convert.ToInt32(reader["CURR_CODE"]),
                            //CRATE = Convert.ToDouble(reader["CRATE"]),
                        };
                    }

                    reader.Close();

                    SqlCommand detail_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader detail_Reader = detail_Command.ExecuteReader();
                    while (detail_Reader.Read())
                    {
                        var row = new SaleTaxInvoiceDetail
                        {
                            ITEM_CODE = Convert.ToInt32(detail_Reader["ITEM_CODE"]),
                            QTY = Convert.ToDouble(detail_Reader["QTY"]),
                            UNIT = Convert.ToInt32(detail_Reader["UNIT"]),
                            RATE = Convert.ToDouble(detail_Reader["RATE"]),
                            AMT = Convert.ToDouble(detail_Reader["AMT"]),
                            DISC = Convert.ToDouble(detail_Reader["DISC"]),
                            DISC_AMT = Convert.ToDouble(detail_Reader["DISC_AMT"]),
                            TAX = Convert.ToDouble(detail_Reader["TAX"]),
                            TAX_AMT = Convert.ToDouble(detail_Reader["TAX_AMT"]),
                            NET_AMT = Convert.ToDouble(detail_Reader["NET_AMT"]),
                            DT_DESC = Convert.ToString(detail_Reader["DT_DESC"]),
                            //QTY2 = Convert.ToDouble(detail_Reader["QTY2"]),
                            //BAL_QTY = Convert.ToDouble(detail_Reader["BAL_QTY"]),
                            //COLOR = Convert.ToInt32(detail_Reader["COLOR"]),
                            //SIZE = Convert.ToInt32(detail_Reader["SIZE"]),
                            //GRADE = Convert.ToInt32(detail_Reader["GRADE"]),
                            //WAREHOUSE = Convert.ToInt32(detail_Reader["WAREHOUSE"]),
                            //DEL_DATE = Convert.ToDateTime(detail_Reader["DEL_DATE"]),
                            //DUE_DATE = Convert.ToDateTime(detail_Reader["DUE_DATE"]),
                            //DUE_DAYS = Convert.ToInt32(detail_Reader["DUE_DAYS"]),
                            //VEH = Convert.ToString(detail_Reader["VEH"]),
                            //CHK = detail_Reader["CHK"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["CHK"]),
                            //PICK_ID = Convert.ToInt32(detail_Reader["PICK_ID"]),
                            //PICK_ID_D = detail_Reader["PICK_ID_D"] == DBNull.Value ? 0 : Convert.ToInt32(detail_Reader["PICK_ID_D"]),
                            //ADV = Convert.ToDouble(detail_Reader["ADV"]),
                            //ADV_AMT = Convert.ToDouble(detail_Reader["ADV_AMT"]),
                            //PARTY_CODE = Convert.ToInt32(detail_Reader["PARTY_CODE"]),
                            //ACT_CODE = Convert.ToInt32(detail_Reader["ACT_CODE"]),
                        };
                        saleTaxInvoiceDetailList.Add(row);
                    }

                    detail_Reader.Close();
                    connection.Close();
                }

                var customRequisition = new CustomSaleTaxInvoice
                {
                    Master = saleTaxInvoice,
                    Detail = saleTaxInvoiceDetailList
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

        public MyHttpResponseMessage DeleteSaleTaxInvoiceDetailByCode(int code, Common common)
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

        public MyHttpResponseMessage GetDataForApi(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<SaleTaxInvoice> jsonDataResult = new List<SaleTaxInvoice>();
                var Menu = _menuRepository.GetMenu(common.MenuID);
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                }

             
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {

                    // D.HS_CODE,
                    //I.ITEM_NAME,
                    //                    D.RATE,
                    //                    U.GROUP_NAME AS UOM,
                    //                    D.QTY,
                    //                    D.AMT AS TOTAL_VALUES,
                    //                    (D.AMT - D.DISC_AMT) AS VALUE_SALES_EXCLUDING,
                    //                    D.AMT AS FIXEDVALUE_RETAILPRICE,
                    //                    (D.AMT - D.DISC_AMT) * D.TAX / 100 AS ST_APPLICABLE,
                    //                    D.SCHEDULE_NO AS SRO_SCH_NO,
                    //                    FBR.S_NAME,
                    //                    D.SERIAL_NO,
                    //                    D.TAX,

                    //CASE WHEN P.NTN IS NOT NULL AND P.NTN <> '' THEN P.NTN  WHEN P.CNIC IS NOT NULL AND P.CNIC <> '' THEN P.CNIC

                    string query = $@"SELECT 
                                        M.V_DATE AS INVOICE_DATE, 
                                        B.B_NTN AS SELLER_NTN, 
                                        C.C_NAME AS SELLER_BNAME,
                                        B.PROVINCE AS SELLER_PROVINCE, 
                                        B.B_ADDRESS AS SELLER_ADDRESS,
                                        CASE WHEN P.NTN IS NULL OR P.NTN = '' THEN P.CNIC WHEN P.CNIC IS NULL OR P.CNIC = '' THEN P.NTN END AS BUYER_NTN,
                                        ELSE NULL
                                        END AS BUYER_NTN,
                                        P.PARTY_NAME AS BUYER_BNAME,
                                        R.DESCR AS BUYER_PROVINCE,
                                        P.PADDRESS AS BUYER_ADDRESS,
                                        P.REG_STATUS AS BUYER_REG,
                                        D.ITEM_SNO AS SCENARIO_ID,
                                        P.REG_STATUS AS BUYER_REG_TYPE

                                        FROM TBL_FBR_SB_MASTER M
                                        LEFT OUTER JOIN TBL_FBR_SB_DETAIL D ON D.TRAN_ID = M.TRAN_ID
                                        LEFT OUTER JOIN TBL_BRANCH B ON B.BCODE = M.BCODE
                                        LEFT OUTER JOIN TBL_COMPANY C ON C.CCODE = B.CCODE
                                        LEFT OUTER JOIN TBL_PARTY_TYPES P ON P.PARTY_CODE = M.PARTY_CODE AND P.ACT_CODE = M.ACT_CODE
                                        LEFT OUTER JOIN TBL_REGION R ON R.CODE = P.REGION
                                        WHERE M.TRAN_ID = {code} AND M.BCODE = {common.Branch} AND M.PERIOD_ID = {common.Period} AND D.BCODE = {common.Branch} AND D.PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new SaleTaxInvoice
                        {
                            INVOICE_DATE = SafeDate(reader["INVOICE_DATE"]),
                            SELLER_NTN = SafeString(reader["SELLER_NTN"]),
                            SELLER_BNAME = SafeString(reader["SELLER_BNAME"]),
                            SELLER_PROVINCE = SafeString(reader["SELLER_PROVINCE"]),
                            SELLER_ADDRESS = SafeString(reader["SELLER_ADDRESS"]),
                            BUYER_NTN = SafeString(reader["BUYER_NTN"]),
                            BUYER_BNAME = SafeString(reader["BUYER_BNAME"]),
                            BUYER_PROVINCE = SafeString(reader["BUYER_PROVINCE"]),
                            BUYER_ADDRESS = SafeString(reader["BUYER_ADDRESS"]),
                            BUYER_REG = SafeString(reader["BUYER_REG"]),
                            SCENARIO_ID = SafeString(reader["SCENARIO_ID"]),
                            BUYER_REG_TYPE = SafeString(reader["BUYER_REG_TYPE"]),

                            //HS_CODE = SafeString(reader["HS_CODE"]),
                            //ITEM_NAME = SafeString(reader["ITEM_NAME"]),
                            //RATE = Math.Round(SafeDouble(reader["RATE"]), 2),
                            //QTY = Math.Round(SafeDouble(reader["QTY"]), 2),
                            //TOTAL_VALUES = Math.Round(SafeDouble(reader["TOTAL_VALUES"]), 2),
                            //VALUE_SALES_EXCLUDING = Math.Round(SafeDouble(reader["VALUE_SALES_EXCLUDING"]), 2),
                            //FIXEDVALUE_RETAILPRICE = Math.Round(SafeDouble(reader["FIXEDVALUE_RETAILPRICE"]), 2),
                            //TAX = Math.Round(SafeDouble(reader["TAX"]), 2),
                            //ST_APPLICABLE = Math.Round(SafeDouble(reader["ST_APPLICABLE"]), 2),
                            //SRO_SCH_NO = SafeString(reader["SRO_SCH_NO"]),
                            //UOM = SafeString(reader["UOM"]),
                            //S_NAME = SafeString(reader["S_NAME"]),
                            //SERIAL_NO = SafeInt(reader["SERIAL_NO"])
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

        public List<SaleTaxInvoiceDetail> GetDetailDataForApi(int code, Common common)
        {
            List<SaleTaxInvoiceDetail> detailList = new List<SaleTaxInvoiceDetail>();

            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? detailTable = string.Empty;

                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    detailTable = menu.TABLE2;
                }

                if (!string.IsNullOrWhiteSpace(detailTable))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {

                        string query = $@"SELECT D.HS_CODE,
                                            I.ITEM_NAME,
                                            D.RATE,
                                            U.GROUP_NAME AS UOM,
                                            D.QTY,
                                            D.AMT AS TOTAL_VALUES,
                                            (D.AMT - D.DISC_AMT) AS VALUE_SALES_EXCLUDING,
                                            D.AMT AS FIXEDVALUE_RETAILPRICE,
                                            (D.AMT - D.DISC_AMT) * D.TAX / 100 AS ST_APPLICABLE,
                                            D.SCHEDULE_NO AS SRO_SCH_NO,
                                            FBR.S_NAME,
                                            D.SERIAL_NO,
                                            D.TAX
                                            FROM {detailTable} AS D 
                                            LEFT OUTER JOIN TBL_ITEMSMASTER I ON I.ITEM_CODE = D.ITEM_CODE
                                            LEFT OUTER JOIN TBL_UNIT U ON U.GROUP_CODE = D.UNIT
                                            LEFT OUTER JOIN TBL_FBR_TYPE FBR ON FBR.CODE = D.FBR_TYPE WHERE D.DLT = 'T' AND D.TRAN_ID = '{code}' 
                                            AND D.BCODE = '{common.Branch}' AND D.PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            SaleTaxInvoiceDetail row = new SaleTaxInvoiceDetail
                            {
                                HS_CODE = SafeString(reader["HS_CODE"]),
                                ITEM_NAME = SafeString(reader["ITEM_NAME"]),
                                RATE = Math.Round(SafeDouble(reader["RATE"])),
                                UOM = SafeString(reader["UOM"]),
                                QTY = Math.Round(SafeDouble(reader["QTY"])),
                                TOTAL_VALUES = Math.Round(SafeDouble(reader["TOTAL_VALUES"])),
                                VALUE_SALES_EXCLUDING = Math.Round(SafeDouble(reader["VALUE_SALES_EXCLUDING"])),
                                FIXEDVALUE_RETAILPRICE = Math.Round(SafeDouble(reader["FIXEDVALUE_RETAILPRICE"])),
                                ST_APPLICABLE = Math.Round(SafeDouble(reader["ST_APPLICABLE"]),2),
                                SRO_SCH_NO = SafeString(reader["SRO_SCH_NO"]),
                                S_NAME = SafeString(reader["S_NAME"]),
                                SERIAL_NO = SafeInt(reader["SERIAL_NO"]),
                                TAX = Math.Round(SafeDouble(reader["TAX"])),

                            };

                            detailList.Add(row);
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception)
            {
                throw; // service / controller handle karega
            }

            return detailList;
        }


        public MyHttpResponseMessage FBRApi_Status(string code, string apiResponce, Common common)
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
                        string query = $"UPDATE {table} SET FBR_NO = '{apiResponce}' WHERE TRAN_ID = '{code}' AND DLT = 'T' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
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

        public MyHttpResponseMessage GetDataForReport(SaleTaxInvoiceRDLCReport modelRecord, DataTable dataTable, CustomMenuDetail menuDetails, Company currentCompany, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            SaleTaxInvoiceRDLCReport masterData = new SaleTaxInvoiceRDLCReport();
            CustomSaleTaxInvoiceForPrintReport reportData = new CustomSaleTaxInvoiceForPrintReport();
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

                if (menuDetails.REPORT_NAME == "SaleTaxInvoice")
                {

                   // query = $@"EXEC PROC_PRINT '{menuDetails.REPORT_NAME}','{table}','{detailTable}','{pickMaster}','{pickDetail}','{common.Branch}','{common.Period}','{modelRecord.TRAN_ID}','{common.Username}',''";

                    query = $@"EXEC PROC_PRINT '{table}','{detailTable}','','','{common.Branch}','{common.Period}','{modelRecord.TRAN_ID}','{common.Username}','','{menuDetails.REPORT_NAME}'";
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            masterData.HEADER_NAME = $"{menuDetails.MD_NAME}";
                            masterData.REPORT_NAME = $"{menuDetails.REPORT_NAME}";
                            masterData.INVOICE_NUMBER = Convert.ToString(reader["VOUCHER_NO"]);
                            masterData.DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("dd-MM-yyyy");
                            masterData.COMPANY_NAME = reader["C_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["C_NAME"]);
                            masterData.BTYPE = reader["BTYPE"] == DBNull.Value ? "" : Convert.ToString(reader["BTYPE"]);
                            masterData.PARTY_NAME = reader["PARTY_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["PARTY_NAME"]);
                            masterData.PADDRESS = reader["PADDRESS"] == DBNull.Value ? "" : Convert.ToString(reader["PADDRESS"]);
                            masterData.TELL = reader["TELL"] == DBNull.Value ? "" : Convert.ToString(reader["TELL"]);
                            masterData.PT_NTN = reader["PT_NTN"] == DBNull.Value ? "" : Convert.ToString(reader["PT_NTN"]);
                            masterData.C_NAME = reader["C_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["C_NAME"]);
                            masterData.COMPANY_LOGO = currentCompany.C_LOGO;
                            masterData.B_ADDRESS = reader["B_ADDRESS"] == DBNull.Value ? "" : Convert.ToString(reader["B_ADDRESS"]);
                            masterData.B_TEL = reader["B_TEL"] == DBNull.Value ? "" : Convert.ToString(reader["B_TEL"]);
                            masterData.B_NTN = reader["B_NTN"] == DBNull.Value ? "" : Convert.ToString(reader["B_NTN"]);
                            masterData.STRN = reader["STRN"] == DBNull.Value ? "" : Convert.ToString(reader["STRN"]); 
                            masterData.FBR_NO = reader["FBR_NO"] == DBNull.Value ? "" : Convert.ToString(reader["FBR_NO"]);
                            masterData.SIG1 = reader["MENU_SIG1"] == DBNull.Value ? "" : Convert.ToString(reader["MENU_SIG1"]);
                            masterData.SIG2 = reader["MENU_SIG2"] == DBNull.Value ? "" : Convert.ToString(reader["MENU_SIG2"]);
                            masterData.SIG3 = reader["MENU_SIG3"] == DBNull.Value ? "" : Convert.ToString(reader["MENU_SIG3"]);
                            masterData.SIG4 = reader["MENU_SIG4"] == DBNull.Value ? "" : Convert.ToString(reader["MENU_SIG4"]);
                            masterData.MENU_TERMS = reader["MENU_TERMS"] == DBNull.Value ? "" : Convert.ToString(reader["MENU_TERMS"]);
                            masterData.B_WEBSITE = reader["B_WEBSITE"] == DBNull.Value ? "" : Convert.ToString(reader["B_WEBSITE"]);
                            masterData.EMAIL = reader["EMAIL"] == DBNull.Value ? "" : Convert.ToString(reader["EMAIL"]);
                        }
                        reader.Close();
                    }
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            DataRow dataRow = dataTable.NewRow();
                            dataRow["ItemName"] = Convert.ToString(reader["ITEM_NAME"]);
                            dataRow["HSCode"] = Convert.ToString(reader["HS_CODE"]);
                            dataRow["Qty"] = reader["QTY"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["QTY"]);
                            dataRow["Rate"] = reader["RATE"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RATE"]);
                            dataRow["Amt"] = reader["AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AMT"]);
                            dataRow["Disc"] = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC"]);
                            dataRow["DiscAmt"] = reader["DISC_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["DISC_AMT"]);
                            dataRow["Tax"] = reader["TAX"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TAX"]);
                            dataRow["TaxAmt"] = reader["TAX_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TAX_AMT"]);
                            dataRow["NetAmt"] = reader["NET_AMT"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["NET_AMT"]);
                            
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

        private double SafeDouble(object value)
        {
            if (value == DBNull.Value || value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return 0.0;

            return Convert.ToDouble(value);
        }


        private int SafeInt(object value)
        {
            if (value == DBNull.Value || value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return 0;

            return Convert.ToInt32(value);
        }

        private string SafeString(object value)
        {
            if (value == DBNull.Value || value == null)
                return "";

            return value.ToString().Trim();
        }

        private string SafeDate(object value)
        {
            if (value == DBNull.Value || value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return "";

            return Convert.ToDateTime(value).ToString("yyyy-MM-dd");
        }



    }
}