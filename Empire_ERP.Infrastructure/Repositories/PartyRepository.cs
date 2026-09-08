using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class PartyRepository : IPartyRepository
    {
        public ICommonRepository _commonRepository { get; set; }
        public IPeriodRepository _periodRepository { get; set; }
        public IMenuRepository _menuRepository { get; set; }

        public PartyRepository(IMenuRepository menuRepository, ICommonRepository commonRepository, IPeriodRepository periodRepository)
        {
            _menuRepository = menuRepository;
            _commonRepository = commonRepository;
            _periodRepository = periodRepository;
        }

        public MyHttpResponseMessage GetChartOfAccounts(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                int? pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    pType = menu.PTYPE;
                }

                if (pType > 0)
                {
                    List<KeyValuePair<int, string>> dropdownData = new List<KeyValuePair<int, string>>();
                    using (SqlConnection db = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string Parent = "SELECT ACT_CODE,ACT_NAME FROM TBL_CHART WHERE ACT_NATURE = '" + pType + "' AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                        using (SqlCommand ParentCommand = new SqlCommand(Parent, db))
                        {
                            db.Open();
                            using (SqlDataReader ParentReader = ParentCommand.ExecuteReader())
                            {
                                if (ParentReader.HasRows)
                                {
                                    while (ParentReader.Read())
                                    {
                                        int code = ParentReader.GetInt32(0);
                                        string name = ParentReader.GetString(1);
                                        dropdownData.Add(new KeyValuePair<int, string>(code, name));
                                    }
                                }
                            }
                        }
                    }
                    response.data = dropdownData;
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

        private string GenerateNextId(Common common, int pTypeCode)
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
                    string maxIdQuery = "SELECT ISNULL(MAX(PARTY_CODE), 0) + 1 FROM " + table + " WHERE PARTY_TYPE_CODE = '" + pTypeCode + "'";
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(maxIdQuery, connection);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        int nextId = Convert.ToInt32(result);
                        return nextId.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return string.Empty;
        }
        
        public MyHttpResponseMessage Save(PartyTypes modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                int pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    pType = menu.PTYPE.Value;
                }

                if (!String.IsNullOrWhiteSpace(table) && pType > 0)
                {
                    var Ip = common.IPAddress;
                    var Computer = common.ComputerName;
                    var Postal = common.PostalCode;
                    string connectionString = new SQLService().getconnstring();
                    string username = common.Username;
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;
                        try
                        {
                            //string maxIdQuery = "SELECT ACT_CODE FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = '"+ pType +"' AND PARTY_CODE = '"+ modelRecord.S_CODE +"'";
                            //using (SqlConnection connectionNew = new SqlConnection(new SQLService().getconnstring()))
                            //{
                            //    SqlCommand commandNew = new SqlCommand(maxIdQuery, connectionNew);
                            //    connectionNew.Open();
                            //    object result = commandNew.ExecuteScalar();
                            //    modelRecord.SACT_CODE = Convert.ToInt32(result);
                            //}
                            string query = "";
                            string Duplicationquery = "";
                            if (modelRecord.PARTY_CODE == 0)
                            {
                                var partyCode = Convert.ToInt32(GenerateNextId(common, pType));
                                query = "INSERT INTO " + table + " " +
                                        "(PARTY_CODE,PARTY_TYPE_CODE,PARTY_NAME,PARTY_SHORT_NAME," +
                                        "ACT_CODE,CAT_CODE,PADDRESS,NTN,CNIC,CONTACT_PERSON," +
                                        "CELL,WB,TELL,EMAIL,WEBSITE,CNIC_EXP,REMARKS," +
                                        "PAYMENT_TERMS,CREDIT_LIMIT,S_CODE,SACT_CODE,GST," +
                                        "SERVICE_TAX,F_CODE,ENTITY,ASTATUS,ACCOUNT_NUM,TAX," +
                                        "BANK_NAME,BRANCH_NAME,BANK_ADDRESS,DOC_PIC,CNIC_PIC," +
                                        "TERMS_CONDITION,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
                                        "EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                        "ADD_POSTALCODE,EDIT_POSTALCODE,MENU_ID,REGION" +
                                        ",ADD_USER_ID,COMM,DISC,DLT,T_CAT,WHT,EXEMPT_DATE)" +
                                        "VALUES" +
                                        "('" + partyCode + "','" + pType + "','" + modelRecord.PARTY_NAME + "','" + modelRecord.PARTY_SHORT_NAME + "'," +
                                        "'" + modelRecord.ACT_CODE + "','" + modelRecord.CAT_CODE + "','" + modelRecord.PADDRESS + "','" + modelRecord.NTN + "','" + modelRecord.CNIC + "','" + modelRecord.CONTACT_PERSON + "'," +
                                        "'" + modelRecord.CELL + "','" + modelRecord.WB + "','" + modelRecord.TELL + "','" + modelRecord.EMAIL + "','" + modelRecord.WEBSITE + "','" + modelRecord.CNIC_EXP + "','" + modelRecord.REMARKS + "'," +
                                        "'" + modelRecord.PAYMENT_TERMS + "','" + modelRecord.CREDIT_LIMIT + "','" + modelRecord.S_CODE + "','"+ modelRecord.SACT_CODE +"','" + modelRecord.GST + "'," +
                                        "'" + modelRecord.SERVICE_TAX + "','" + modelRecord.F_CODE + "','" + modelRecord.ENTITY + "','" + modelRecord.ASTATUS + "','" + modelRecord.ACCOUNT_NUM + "','" + modelRecord.TAX + "'," +
                                        "'" + modelRecord.BANK_NAME + "','" + modelRecord.BRANCH_NAME + "','" + modelRecord.BANK_ADDRESS + "','" + modelRecord.DOC_PIC + "','" + modelRecord.CNIC_PIC + "'," +
                                        "'" + modelRecord.TERMS_CONDITION + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                        "'" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                        "'" + Postal + "','" + Postal + "','" + common.MenuID + "','" + modelRecord.REGION + "'," +
                                        "'" + username + "','" + modelRecord.COMM + "','" + modelRecord.DISC + "','T','" +modelRecord.T_CAT+ "','" +modelRecord.WHT+ "','" +modelRecord.EXEMPT_DATE+"')";
                                //SqlCommand command = new SqlCommand(query, connection);
                                //command.ExecuteNonQuery();

                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE PARTY_NAME = '" + modelRecord.PARTY_NAME + "' AND PARTY_TYPE_CODE = '" + pType + "' AND DLT = 'T'";
                                //SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
                                //int count = (int)CMD.ExecuteScalar();
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.data = partyCode;
                                    response.msgType = 1;
                                    response.msg = "Record Added Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msg = "Name Already Exist !....";
                                    response.msgType = 2;
                                }
                            }
                            else
                            {
                                query = $"UPDATE {table} SET PARTY_NAME = '" + modelRecord.PARTY_NAME + @"',
                                        PARTY_SHORT_NAME = '" + modelRecord.PARTY_SHORT_NAME + @"',
                                        ACT_CODE = '" + modelRecord.ACT_CODE + @"',
                                        CAT_CODE = '" + modelRecord.CAT_CODE + @"',
                                        PADDRESS = '" + modelRecord.PADDRESS + @"',
                                        NTN = '" + modelRecord.NTN + @"',
                                        CNIC = '" + modelRecord.CNIC + @"',
                                        CONTACT_PERSON = '" + modelRecord.CONTACT_PERSON + @"',
                                        CELL = '" + modelRecord.CELL + @"',
                                        WB = '" + modelRecord.WB + @"',
                                        TELL = '" + modelRecord.TELL + @"',
                                        TAX = '" + modelRecord.TAX + @"',
                                        EMAIL = '" + modelRecord.EMAIL + @"',
                                        WEBSITE = '" + modelRecord.WEBSITE + @"',
                                        CNIC_EXP = '" + modelRecord.CNIC_EXP + @"',
                                        REMARKS = '" + modelRecord.REMARKS + @"', 
                                        PAYMENT_TERMS = '" + modelRecord.PAYMENT_TERMS + @"',
                                        CREDIT_LIMIT = '" + modelRecord.CREDIT_LIMIT + @"',
                                        S_CODE = '" + modelRecord.S_CODE + @"',
                                        SACT_CODE = '"+ modelRecord.SACT_CODE + @"',
                                        GST = '" + modelRecord.GST + @"',
                                        SERVICE_TAX = '" + modelRecord.SERVICE_TAX + @"',
                                        F_CODE = '" + modelRecord.F_CODE + @"',
                                        ENTITY = '" + modelRecord.ENTITY + @"',
                                        ASTATUS = '" + modelRecord.ASTATUS + @"',
                                        ACCOUNT_NUM = '" + modelRecord.ACCOUNT_NUM + @"',
                                        BANK_NAME = '" + modelRecord.BANK_NAME + @"',
                                        BRANCH_NAME = '" + modelRecord.BRANCH_NAME + @"',
                                        BANK_ADDRESS = '" + modelRecord.BANK_ADDRESS + @"',
                                        DOC_PIC = '" + modelRecord.DOC_PIC + @"',
                                        CNIC_PIC = '" + modelRecord.CNIC_PIC + @"',
                                        TERMS_CONDITION = '" + modelRecord.TERMS_CONDITION + @"',
                                        EDIT_USER_ID = '" + username + @"',
                                        EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                        EDIT_COMPUTER_NAME = '" + Computer + @"',
                                        EDIT_IP_ADDRESS = '" + Ip + @"',
                                        EDIT_POSTALCODE = '" + Postal + @"',
                                        REGION = '" + modelRecord.REGION + @"',
                                        COMM = '" + modelRecord.COMM + @"',
                                        DISC = '" + modelRecord.DISC + @"',
                                        T_CAT = '" + modelRecord.T_CAT + @"',
                                        WHT = '" + modelRecord.WHT + @"', 
                                        EXEMPT_DATE = '" + modelRecord.EXEMPT_DATE + @"'
                                        WHERE PARTY_CODE = '" + modelRecord.PARTY_CODE + @"' AND PARTY_TYPE_CODE = '" + pType + @"' ";
                                //SqlCommand command = new SqlCommand(query, connection);
                                //command.ExecuteNonQuery();
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE PARTY_NAME = '" + modelRecord.PARTY_NAME + "' AND PARTY_TYPE_CODE = '" + pType + "' AND DLT = 'T'";
                                //SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
                                //int count = (int)CMD.ExecuteScalar();
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.data = modelRecord.PARTY_CODE;
                                    response.msgType = 1;
                                    response.msg = "Record Updated Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msg = "Name Already Exist !....";
                                    response.msgType = 2;
                                }
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
        
        public MyHttpResponseMessage Delete(int parytCode, int actCode, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            List<CustomPartyReport> jsonDataResult = new List<CustomPartyReport>();
            var periodInfo = _periodRepository.GetPeriodById(Convert.ToInt32(common.Period));
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                int pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    pType = menu.PTYPE.Value;
                }

                if (!String.IsNullOrWhiteSpace(table) && pType > 0)
                {
                    if (parytCode == 0)
                    {
                        response.msg = "ID is not in numeric format";
                        response.msgType = 2;
                    }
                    else
                    {
                        using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                        {
                            string query = $"EXEC PPROC '11','{((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd")}','{((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd")}','{actCode}','{parytCode}','{common.Branch}','{common.Period}','{null}','{pType}','','','','','','',''";
                            connection.Open();
                            SqlCommand command = new SqlCommand(query, connection);
                            SqlDataReader reader = command.ExecuteReader();
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
                                    Debit = Convert.ToDecimal(reader["DEBIT"]),
                                    Credit = Convert.ToDecimal(reader["CREDIT"])
                                };
                                jsonDataResult.Add(row);
                            }
                            reader.Close();
                        }

                        bool isExists = jsonDataResult.Count > 0;

                        if (!isExists)
                        {
                            string connectionString = new SQLService().getconnstring();
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                string query = "UPDATE " + table + " SET DLT = 'F' WHERE PARTY_CODE = @partyCode AND PARTY_TYPE_CODE = @pType";
                                SqlCommand command = new SqlCommand(query, connection);
                                command.Parameters.AddWithValue("@partyCode", parytCode);
                                command.Parameters.AddWithValue("@pType", pType);
                                command.ExecuteScalar();
                                response.msgType = 1;
                                response.msg = "Record Deleted Successfully";
                            }
                        }
                        else
                        {
                            response.msgType = 2;
                            response.msg = "Cannot delete, already in use";
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
                PartyTypes partyTypes = new PartyTypes();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE PARTY_CODE = {record.TRAN_ID} AND PARTY_TYPE_CODE = {record.PARTY_TYPE_CODE} AND DLT = 'T'";
                    //string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        partyTypes = new PartyTypes
                        {
                            PARTY_CODE = 0,
                            PARTY_NAME = record.PARTY_NAME,
                            PARTY_SHORT_NAME = Convert.ToString(reader["PARTY_SHORT_NAME"]),
                            T_CAT = Convert.ToString(reader["T_CAT"]),
                            WHT = Convert.ToString(reader["WHT"]),
                            EXEMPT_DATE = Convert.ToDateTime(reader["EXEMPT_DATE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            CAT_CODE = Convert.ToInt32(reader["CAT_CODE"]),
                            PADDRESS = Convert.ToString(reader["PADDRESS"]),
                            NTN = Convert.ToString(reader["NTN"]),
                            CNIC = Convert.ToString(reader["CNIC"]),
                            CONTACT_PERSON = Convert.ToString(reader["CONTACT_PERSON"]),
                            CELL = Convert.ToString(reader["CELL"]),
                            WB = Convert.ToString(reader["WB"]),
                            TELL = Convert.ToString(reader["TELL"]),
                            EMAIL = Convert.ToString(reader["EMAIL"]),
                            WEBSITE = Convert.ToString(reader["WEBSITE"]),
                            CNIC_EXP = Convert.ToDateTime(reader["CNIC_EXP"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            PAYMENT_TERMS = Convert.ToInt32(reader["PAYMENT_TERMS"]),
                            CREDIT_LIMIT = Convert.ToDouble(reader["CREDIT_LIMIT"]),
                            S_CODE = Convert.ToInt32(reader["S_CODE"]),
                            SACT_CODE = Convert.ToInt32(reader["SACT_CODE"]),
                            GST = Convert.ToString(reader["GST"]),
                            SERVICE_TAX = Convert.ToString(reader["SERVICE_TAX"]),
                            F_CODE = Convert.ToString(reader["F_CODE"]),
                            ENTITY = Convert.ToInt32(reader["ENTITY"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            ACCOUNT_NUM = Convert.ToString(reader["ACCOUNT_NUM"]),
                            BANK_NAME = Convert.ToString(reader["BANK_NAME"]),
                            BRANCH_NAME = Convert.ToString(reader["BRANCH_NAME"]),
                            BANK_ADDRESS = Convert.ToString(reader["BANK_ADDRESS"]),
                            DOC_PIC = Convert.ToString(reader["DOC_PIC"]),
                            CNIC_PIC = Convert.ToString(reader["CNIC_PIC"]),
                            TERMS_CONDITION = Convert.ToString(reader["TERMS_CONDITION"]),
                            REGION = Convert.ToInt32(reader["REGION"]),
                            COMM = Convert.ToDouble(reader["COMM"]),
                            DISC = Convert.ToDouble(reader["DISC"]),
                        };
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(partyTypes, common);
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

        public MyHttpResponseMessage QuickSearch(int partyCode, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                int pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    pType = menu.PTYPE.Value;
                }

                if (!String.IsNullOrWhiteSpace(table) && pType > 0)
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string Qurey = "SELECT PT.T_CAT,PT.EXEMPT_DATE,PT.WHT, PT.PARTY_CODE,PT.PARTY_NAME,PT.PARTY_SHORT_NAME,CH.ACT_NAME, " +
                            " CT.GROUP_NAME AS CATEGORY," +
                            " PT.PADDRESS,PT.NTN,PT.CNIC,PT.CONTACT_PERSON,PT.CELL,PT.WB,PT.TELL,PT.EMAIL,PT.WEBSITE,PT.CNIC_EXP,PT.REMARKS," +
                            " PT.PAYMENT_TERMS,PT.CREDIT_LIMIT,SM.PARTY_NAME AS SALESPERSON,SMCH.ACT_NAME AS SALES_ACT_NAME," +
                            " PT.GST,PT.SERVICE_TAX,PT.F_CODE," +
                            " CASE WHEN PT.F_CODE = 1 THEN 'Others'" +
                            " WHEN PT.F_CODE = 2 then 'Filer'" +
                            " WHEN PT.F_CODE = 3 then 'Non-Filer'" +
                            " End As F_NAME," +
                            " ET.GROUP_NAME AS ENTITY_NAME,PT.ASTATUS," +
                            " CASE WHEN PT.ASTATUS = 'Y' THEN 'Active'" +
                            " WHEN PT.ASTATUS = 'N' then 'In-Active'" +
                            " end as STATUS_NAME," +
                            " PT.ACCOUNT_NUM,PT.BANK_NAME,PT.BRANCH_NAME,PT.BANK_ADDRESS,PT.DOC_PIC,PT.CNIC_PIC,PT.TERMS_CONDITION," +
                            " PT.ADD_DATE,PT.ADD_USER_ID,PT.ADD_COMPUTER_NAME,PT.ADD_IP_ADDRESS," +
                            " PT.EDIT_DATE,PT.EDIT_USER_ID,PT.EDIT_COMPUTER_NAME,PT.EDIT_IP_ADDRESS," +
                            " PT.ADD_POSTALCODE,PT.EDIT_POSTALCODE,PT.MENU_ID,RG.DESCR AS REGION_NAME,PT.COMM" +
                            " FROM " + table + " PT" +
                            " LEFT OUTER JOIN TBL_CHART CH" +
                            " ON CH.ACT_CODE = PT.ACT_CODE" +
                            " LEFT OUTER JOIN TBL_CATEGORY CT" +
                            " ON CT.GROUP_CODE = PT.CAT_CODE" +
                            " LEFT OUTER JOIN TBL_PARTY_TYPES SM" +
                            " ON SM.PARTY_CODE = PT.S_CODE AND SM.ACT_CODE = PT.SACT_CODE" +
                            " LEFT OUTER JOIN TBL_CHART SMCH" +
                            " ON SMCH.ACT_CODE = PT.SACT_CODE" +
                            " LEFT OUTER JOIN TBL_ENTITY ET" +
                            " ON ET.GROUP_CODE = PT.ENTITY" +
                            " LEFT OUTER JOIN TBL_REGION RG" +
                            " ON RG.CODE = PT.REGION" +
                            " WHERE PT.PARTY_TYPE_CODE = '" + pType + "' AND PT.DLT = 'T' OPTION(FAST 50)";

                        SqlCommand command = new SqlCommand(Qurey, connection);

                        connection.Open();

                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var row = new
                            {
                                T_CAT = reader["T_CAT"].ToString(),
                                WHT = reader["WHT"].ToString(),
                                EXEMPT_DATE = reader["EXEMPT_DATE"].ToString(),
                                ID = reader["PARTY_CODE"],
                                PARTY_NAME = reader["PARTY_NAME"].ToString(),
                                PARTY_SHORT_NAME = reader["PARTY_SHORT_NAME"].ToString(),
                                ACT_NAME = reader["ACT_NAME"].ToString(),
                                CATEGORY = reader["CATEGORY"].ToString(),
                                PADDRESS = reader["PADDRESS"].ToString(),
                                NTN = reader["NTN"].ToString(),
                                CNIC = reader["CNIC"].ToString(),
                                CONTACT_PERSON = reader["CONTACT_PERSON"].ToString(),
                                CELL = reader["CELL"].ToString(),
                                WB = reader["WB"].ToString(),
                                TELL = reader["TELL"].ToString(),
                                EMAIL = reader["EMAIL"].ToString(),
                                WEBSITE = reader["WEBSITE"].ToString(),
                                CNIC_EXP = reader["CNIC_EXP"].ToString(),
                                REMARKS = reader["REMARKS"].ToString(),
                                PAYMENT_TERMS = reader["PAYMENT_TERMS"],
                                CREDIT_LIMIT = reader["CREDIT_LIMIT"],
                                SALESPERSON = reader["SALESPERSON"].ToString(),
                                SALES_ACT_NAME = reader["SALES_ACT_NAME"].ToString(),
                                GST = reader["GST"].ToString(),
                                SERVICE_TAX = reader["SERVICE_TAX"].ToString(),
                                F_NAME = reader["F_NAME"].ToString(),
                                ENTITY_NAME = reader["ENTITY_NAME"].ToString(),
                                STATUS_NAME = reader["STATUS_NAME"].ToString(),
                                ACCOUNT_NUM = reader["ACCOUNT_NUM"].ToString(),
                                BANK_NAME = reader["BANK_NAME"].ToString(),
                                BRANCH_NAME = reader["BRANCH_NAME"].ToString(),
                                TERMS_CONDITION = reader["TERMS_CONDITION"],
                                ADD_DATE = reader["ADD_DATE"],
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"],
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"],
                                EDIT_USER_ID = reader["EDIT_USER_ID"],
                                EDIT_DATE = reader["EDIT_DATE"],
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"],
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"],
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"],
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"],
                                REGION_NAME = reader["REGION_NAME"].ToString(),
                                ADD_USER_ID = reader["ADD_USER_ID"],
                                COMM = reader["COMM"].ToString()
                            };

                            jsonDataResult.Add(row);
                        }

                        reader.Close();
                    }

                    response.data = jsonDataResult;
                    response.msg = "";
                    response.msgType = 1;
                    return response;
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

        public MyHttpResponseMessage QuickSearchParty(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                int pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    pType = menu.PTYPE.Value;
                }

                if (!String.IsNullOrWhiteSpace(table) && pType > 0)
                {
                    List<object> jsonDataResult = new List<object>();

                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT PT.T_CAT,PT.WHT,PT.EXEMPT_DATE, PT.PARTY_CODE,PT.PARTY_TYPE_CODE,PT.PARTY_NAME,PT.PARTY_SHORT_NAME,CH.ACT_NAME, " +
                            " CT.GROUP_NAME AS CATEGORY," +
                            " PT.PADDRESS,PT.NTN,PT.CNIC,PT.CONTACT_PERSON,PT.CELL,PT.WB,PT.TELL,PT.EMAIL,PT.WEBSITE,PT.CNIC_EXP,PT.REMARKS," +
                            " PT.PAYMENT_TERMS,PT.CREDIT_LIMIT,SM.PARTY_NAME AS SALESPERSON,SMCH.ACT_NAME AS SALES_ACT_NAME," +
                            " PT.GST,PT.SERVICE_TAX,PT.F_CODE," +
                            " CASE WHEN PT.F_CODE = 1 THEN 'Others'" +
                            " WHEN PT.F_CODE = 2 then 'Filer'" +
                            " WHEN PT.F_CODE = 3 then 'Non-Filer'" +
                            " End As F_NAME," +
                            " ET.GROUP_NAME AS ENTITY_NAME,PT.ASTATUS," +
                            " CASE WHEN PT.ASTATUS = 'Y' THEN 'Active'" +
                            " WHEN PT.ASTATUS = 'N' then 'In-Active'" +
                            " end as STATUS_NAME," +
                            " PT.ACCOUNT_NUM,PT.BANK_NAME,PT.BRANCH_NAME,PT.BANK_ADDRESS,PT.DOC_PIC,PT.CNIC_PIC,PT.TERMS_CONDITION," +
                            " PT.ADD_DATE,PT.ADD_USER_ID,PT.ADD_COMPUTER_NAME,PT.ADD_IP_ADDRESS," +
                            " PT.EDIT_DATE,PT.EDIT_USER_ID,PT.EDIT_COMPUTER_NAME,PT.EDIT_IP_ADDRESS," +
                            " PT.ADD_POSTALCODE,PT.EDIT_POSTALCODE,PT.MENU_ID,RG.DESCR AS REGION_NAME,PT.COMM" +
                            " FROM " + table + " PT" +
                            " LEFT OUTER JOIN TBL_CHART CH" +
                            " ON CH.ACT_CODE = PT.ACT_CODE" +
                            " LEFT OUTER JOIN TBL_CATEGORY CT" +
                            " ON CT.GROUP_CODE = PT.CAT_CODE" +
                            " LEFT OUTER JOIN TBL_PARTY_TYPES SM" +
                            " ON SM.PARTY_CODE = PT.S_CODE AND SM.ACT_CODE = PT.SACT_CODE" +
                            " LEFT OUTER JOIN TBL_CHART SMCH" +
                            " ON SMCH.ACT_CODE = PT.SACT_CODE" +
                            " LEFT OUTER JOIN TBL_ENTITY ET" +
                            " ON ET.GROUP_CODE = PT.ENTITY" +
                            " LEFT OUTER JOIN TBL_REGION RG" +
                            " ON RG.CODE = PT.REGION" +
                            " WHERE PT.PARTY_TYPE_CODE = '" + pType + "' AND PT.DLT = 'T' ORDER BY PT.PARTY_CODE DESC";
                        
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var row = new
                            {
                                T_CAT = reader["T_CAT"].ToString(),
                                WHT = reader["WHT"].ToString(),
                                EXEMPT_DATE = reader["EXEMPT_DATE"].ToString(),
                                ID = reader["PARTY_CODE"],
                                PARTY_NAME = reader["PARTY_NAME"].ToString(),
                                PARTY_TYPE_CODE = reader["PARTY_TYPE_CODE"].ToString(),
                                PARTY_SHORT_NAME = reader["PARTY_SHORT_NAME"].ToString(),
                                ACT_NAME = reader["ACT_NAME"].ToString(),
                                CATEGORY = reader["CATEGORY"].ToString(),
                                PADDRESS = reader["PADDRESS"].ToString(),
                                NTN = reader["NTN"].ToString(),
                                CNIC = reader["CNIC"].ToString(),
                                CONTACT_PERSON = reader["CONTACT_PERSON"].ToString(),
                                CELL = reader["CELL"].ToString(),
                                WB = reader["WB"].ToString(),
                                TELL = reader["TELL"].ToString(),
                                EMAIL = reader["EMAIL"].ToString(),
                                WEBSITE = reader["WEBSITE"].ToString(),
                                CNIC_EXP = reader["CNIC_EXP"].ToString(),
                                REMARKS = reader["REMARKS"].ToString(),
                                PAYMENT_TERMS = reader["PAYMENT_TERMS"],
                                CREDIT_LIMIT = reader["CREDIT_LIMIT"],
                                SALESPERSON = reader["SALESPERSON"].ToString(),
                                SALES_ACT_NAME = reader["SALES_ACT_NAME"].ToString(),
                                GST = reader["GST"].ToString(),
                                SERVICE_TAX = reader["SERVICE_TAX"].ToString(),
                                F_NAME = reader["F_NAME"].ToString(),
                                ENTITY_NAME = reader["ENTITY_NAME"].ToString(),
                                STATUS_NAME = reader["STATUS_NAME"].ToString(),
                                ACCOUNT_NUM = reader["ACCOUNT_NUM"].ToString(),
                                BANK_NAME = reader["BANK_NAME"].ToString(),
                                BRANCH_NAME = reader["BRANCH_NAME"].ToString(),
                                TERMS_CONDITION = reader["TERMS_CONDITION"],
                                ADD_DATE = reader["ADD_DATE"],
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"],
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"],
                                EDIT_USER_ID = reader["EDIT_USER_ID"],
                                EDIT_DATE = reader["EDIT_DATE"],
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"],
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"],
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"],
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"],
                                REGION_NAME = reader["REGION_NAME"].ToString(),
                                ADD_USER_ID = reader["ADD_USER_ID"],
                                COMM = reader["COMM"].ToString()
                            };

                            jsonDataResult.Add(row);
                        }

                        reader.Close();
                    }

                    response.data = jsonDataResult;
                    response.msg = "";
                    response.msgType = 1;
                    return response;
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

        //public MyHttpResponseMessage QuickSearchLazyLoading(Common common, int skip, int take, string filter = null, string group = null)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        int pType = 0;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //            pType = menu.PTYPE.Value;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table) && pType > 0)
        //        {
        //            List<object> jsonDataResult = new List<object>();
        //            int totalCount = 0;

        //            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //            {
        //                string filterCondition = string.Empty;
        //                if (!string.IsNullOrEmpty(filter))
        //                {
        //                    JsonNode jsonNode = JsonNode.Parse(filter);
        //                    JsonArray jsonArray = jsonNode.AsArray();
        //                    if (jsonArray.Count >= 15)
        //                    {
        //                        jsonArray.RemoveAt(1);
        //                        jsonArray.RemoveAt(0);
        //                    }
        //                    filterCondition = _commonRepository.BuildFilterCondition(jsonArray);
        //                    filterCondition = filterCondition
        //                        .Replace("partY_NAME", "PT.partY_NAME")
        //                        .Replace("id", "PT.PARTY_CODE")
        //                        .Replace("partY_SHORT_NAME", "PT.partY_SHORT_NAME")
        //                        .Replace("paddress", "PT.paddress")
        //                        .Replace("comm", "PT.comm")
        //                        .Replace("category", "CT.GROUP_NAME")
        //                        .Replace("ntn", "PT.ntn")
        //                        .Replace("email", "PT.email")
        //                        .Replace("cnic", "PT.cnic")
        //                        .Replace("cell", "PT.cell")
        //                        .Replace("contacT_PERSON", "PT.contacT_PERSON")
        //                        .Replace("wb", "PT.wb")
        //                        .Replace("website", "PT.website")
        //                        .Replace("tell", "PT.tell")
        //                        .Replace("salesperson", "SM.PARTY_NAME")
        //                        .Replace("regioN_NAME", "RG.DESCR")
        //                        .Replace("gst", "PT.gst")
        //                        .Replace("servicE_TAX", "PT.servicE_TAX")
        //                        .Replace("entitY_NAME", "ET.GROUP_NAME")
        //                        .Replace("accounT_NUM", "PT.accounT_NUM")
        //                        .Replace("banK_NAME", "PT.banK_NAME")
        //                        .Replace("brancH_NAME", "PT.brancH_NAME")
        //                        .Replace("BANK_ADDRESS", "PT.BANK_ADDRESS")
        //                        .Replace("remarks", "PT.remarks")
        //                        .Replace("termS_CONDITION", "PT.termS_CONDITION")
        //                        .Replace("acT_NAME", "CH.ACT_NAME")
        //                        .Replace("f_NAME", "PT.F_CODE")
        //                        .Replace("statuS_NAME", "PT.ASTATUS");
        //                }

        //                string query = "SELECT COUNT(*) OVER() AS COUNT, PT.PARTY_CODE,PT.PARTY_NAME,PT.PARTY_SHORT_NAME,CH.ACT_NAME, " +
        //                                            " CT.GROUP_NAME AS CATEGORY," +
        //                                            " PT.PADDRESS,PT.NTN,PT.CNIC,PT.CONTACT_PERSON,PT.CELL,PT.WB,PT.TELL,PT.EMAIL,PT.WEBSITE,PT.CNIC_EXP,PT.REMARKS," +
        //                                            " PT.PAYMENT_TERMS,PT.CREDIT_LIMIT,SM.PARTY_NAME AS SALESPERSON,SMCH.ACT_NAME AS SALES_ACT_NAME," +
        //                                            " PT.GST,PT.SERVICE_TAX,PT.F_CODE," +
        //                                            " CASE WHEN PT.F_CODE = 1 THEN 'Others'" +
        //                                            " WHEN PT.F_CODE = 2 then 'Filer'" +
        //                                            " WHEN PT.F_CODE = 3 then 'Non-Filer'" +
        //                                            " End As F_NAME," +
        //                                            " ET.GROUP_NAME AS ENTITY_NAME,PT.ASTATUS," +
        //                                            " CASE WHEN PT.ASTATUS = 'Y' THEN 'Active'" +
        //                                            " WHEN PT.ASTATUS = 'N' then 'In-Active'" +
        //                                            " end as STATUS_NAME," +
        //                                            " PT.ACCOUNT_NUM,PT.BANK_NAME,PT.BRANCH_NAME,PT.BANK_ADDRESS,PT.DOC_PIC,PT.CNIC_PIC,PT.TERMS_CONDITION," +
        //                                            " PT.ADD_DATE,PT.ADD_USER_ID,PT.ADD_COMPUTER_NAME,PT.ADD_IP_ADDRESS," +
        //                                            " PT.EDIT_DATE,PT.EDIT_USER_ID,PT.EDIT_COMPUTER_NAME,PT.EDIT_IP_ADDRESS," +
        //                                            " PT.ADD_POSTALCODE,PT.EDIT_POSTALCODE,PT.MENU_ID,RG.DESCR AS REGION_NAME,PT.COMM" +
        //                                            " FROM " + table + " PT" +
        //                                            " LEFT OUTER JOIN TBL_CHART CH" +
        //                                            " ON CH.ACT_CODE = PT.ACT_CODE" +
        //                                            " LEFT OUTER JOIN TBL_CATEGORY CT" +
        //                                            " ON CT.GROUP_CODE = PT.CAT_CODE" +
        //                                            " LEFT OUTER JOIN TBL_PARTY_TYPES SM" +
        //                                            " ON SM.PARTY_CODE = PT.S_CODE AND SM.ACT_CODE = PT.SACT_CODE" +
        //                                            " LEFT OUTER JOIN TBL_CHART SMCH" +
        //                                            " ON SMCH.ACT_CODE = PT.SACT_CODE" +
        //                                            " LEFT OUTER JOIN TBL_ENTITY ET" +
        //                                            " ON ET.GROUP_CODE = PT.ENTITY" +
        //                                            " LEFT OUTER JOIN TBL_REGION RG" +
        //                                            " ON RG.CODE = PT.REGION" +
        //                                            " WHERE PT.PARTY_TYPE_CODE = '" + pType + "' AND PT.DLT = 'T' " + filterCondition + "";

        //                SqlCommand countCommand = new SqlCommand(query, connection);
        //                connection.Open();
        //                SqlDataReader readerCommand = countCommand.ExecuteReader();
        //                if (readerCommand.Read())
        //                {
        //                    totalCount = readerCommand.GetInt32(readerCommand.GetOrdinal("COUNT"));
        //                }
        //                readerCommand.Close();
        //                connection.Close();

        //                string Qurey = "";

        //                if (string.IsNullOrEmpty(group))
        //                {
        //                    Qurey = "SELECT PT.PARTY_CODE,PT.PARTY_NAME,PT.PARTY_SHORT_NAME,CH.ACT_NAME, " +
        //                    " CT.GROUP_NAME AS CATEGORY," +
        //                    " PT.PADDRESS,PT.NTN,PT.CNIC,PT.CONTACT_PERSON,PT.CELL,PT.WB,PT.TELL,PT.EMAIL,PT.WEBSITE,PT.CNIC_EXP,PT.REMARKS," +
        //                    " PT.PAYMENT_TERMS,PT.CREDIT_LIMIT,SM.PARTY_NAME AS SALESPERSON,SMCH.ACT_NAME AS SALES_ACT_NAME," +
        //                    " PT.GST,PT.SERVICE_TAX,PT.F_CODE," +
        //                    " CASE WHEN PT.F_CODE = 1 THEN 'Others'" +
        //                    " WHEN PT.F_CODE = 2 then 'Filer'" +
        //                    " WHEN PT.F_CODE = 3 then 'Non-Filer'" +
        //                    " End As F_NAME," +
        //                    " ET.GROUP_NAME AS ENTITY_NAME,PT.ASTATUS," +
        //                    " CASE WHEN PT.ASTATUS = 'Y' THEN 'Active'" +
        //                    " WHEN PT.ASTATUS = 'N' then 'In-Active'" +
        //                    " end as STATUS_NAME," +
        //                    " PT.ACCOUNT_NUM,PT.BANK_NAME,PT.BRANCH_NAME,PT.BANK_ADDRESS,PT.DOC_PIC,PT.CNIC_PIC,PT.TERMS_CONDITION," +
        //                    " PT.ADD_DATE,PT.ADD_USER_ID,PT.ADD_COMPUTER_NAME,PT.ADD_IP_ADDRESS," +
        //                    " PT.EDIT_DATE,PT.EDIT_USER_ID,PT.EDIT_COMPUTER_NAME,PT.EDIT_IP_ADDRESS," +
        //                    " PT.ADD_POSTALCODE,PT.EDIT_POSTALCODE,PT.MENU_ID,RG.DESCR AS REGION_NAME,PT.COMM" +
        //                    " FROM " + table + " PT" +
        //                    " LEFT OUTER JOIN TBL_CHART CH" +
        //                    " ON CH.ACT_CODE = PT.ACT_CODE" +
        //                    " LEFT OUTER JOIN TBL_CATEGORY CT" +
        //                    " ON CT.GROUP_CODE = PT.CAT_CODE" +
        //                    " LEFT OUTER JOIN TBL_PARTY_TYPES SM" +
        //                    " ON SM.PARTY_CODE = PT.S_CODE AND SM.ACT_CODE = PT.SACT_CODE" +
        //                    " LEFT OUTER JOIN TBL_CHART SMCH" +
        //                    " ON SMCH.ACT_CODE = PT.SACT_CODE" +
        //                    " LEFT OUTER JOIN TBL_ENTITY ET" +
        //                    " ON ET.GROUP_CODE = PT.ENTITY" +
        //                    " LEFT OUTER JOIN TBL_REGION RG" +
        //                    " ON RG.CODE = PT.REGION" +
        //                    " WHERE PT.PARTY_TYPE_CODE = '" + pType + "' AND PT.DLT = 'T' " + filterCondition + " ORDER BY PT.PARTY_CODE DESC OFFSET " + skip + " ROWS FETCH NEXT " + take + " ROWS ONLY";
        //                }else
        //                {
        //                    Qurey = "SELECT PT.PARTY_CODE,PT.PARTY_NAME,PT.PARTY_SHORT_NAME,CH.ACT_NAME, " +
        //                    " CT.GROUP_NAME AS CATEGORY," +
        //                    " PT.PADDRESS,PT.NTN,PT.CNIC,PT.CONTACT_PERSON,PT.CELL,PT.WB,PT.TELL,PT.EMAIL,PT.WEBSITE,PT.CNIC_EXP,PT.REMARKS," +
        //                    " PT.PAYMENT_TERMS,PT.CREDIT_LIMIT,SM.PARTY_NAME AS SALESPERSON,SMCH.ACT_NAME AS SALES_ACT_NAME," +
        //                    " PT.GST,PT.SERVICE_TAX,PT.F_CODE," +
        //                    " CASE WHEN PT.F_CODE = 1 THEN 'Others'" +
        //                    " WHEN PT.F_CODE = 2 then 'Filer'" +
        //                    " WHEN PT.F_CODE = 3 then 'Non-Filer'" +
        //                    " End As F_NAME," +
        //                    " ET.GROUP_NAME AS ENTITY_NAME,PT.ASTATUS," +
        //                    " CASE WHEN PT.ASTATUS = 'Y' THEN 'Active'" +
        //                    " WHEN PT.ASTATUS = 'N' then 'In-Active'" +
        //                    " end as STATUS_NAME," +
        //                    " PT.ACCOUNT_NUM,PT.BANK_NAME,PT.BRANCH_NAME,PT.BANK_ADDRESS,PT.DOC_PIC,PT.CNIC_PIC,PT.TERMS_CONDITION," +
        //                    " PT.ADD_DATE,PT.ADD_USER_ID,PT.ADD_COMPUTER_NAME,PT.ADD_IP_ADDRESS," +
        //                    " PT.EDIT_DATE,PT.EDIT_USER_ID,PT.EDIT_COMPUTER_NAME,PT.EDIT_IP_ADDRESS," +
        //                    " PT.ADD_POSTALCODE,PT.EDIT_POSTALCODE,PT.MENU_ID,RG.DESCR AS REGION_NAME,PT.COMM" +
        //                    " FROM " + table + " PT" +
        //                    " LEFT OUTER JOIN TBL_CHART CH" +
        //                    " ON CH.ACT_CODE = PT.ACT_CODE" +
        //                    " LEFT OUTER JOIN TBL_CATEGORY CT" +
        //                    " ON CT.GROUP_CODE = PT.CAT_CODE" +
        //                    " LEFT OUTER JOIN TBL_PARTY_TYPES SM" +
        //                    " ON SM.PARTY_CODE = PT.S_CODE AND SM.ACT_CODE = PT.SACT_CODE" +
        //                    " LEFT OUTER JOIN TBL_CHART SMCH" +
        //                    " ON SMCH.ACT_CODE = PT.SACT_CODE" +
        //                    " LEFT OUTER JOIN TBL_ENTITY ET" +
        //                    " ON ET.GROUP_CODE = PT.ENTITY" +
        //                    " LEFT OUTER JOIN TBL_REGION RG" +
        //                    " ON RG.CODE = PT.REGION" +
        //                    " WHERE PT.PARTY_TYPE_CODE = '" + pType + "' AND PT.DLT = 'T' " + filterCondition + " ORDER BY PT.PARTY_CODE DESC";
        //                }

        //                SqlCommand command = new SqlCommand(Qurey, connection);
        //                connection.Open();
        //                SqlDataReader reader = command.ExecuteReader();

        //                while (reader.Read())
        //                {
        //                    var row = new
        //                    {
        //                        ID = reader["PARTY_CODE"],
        //                        PARTY_NAME = reader["PARTY_NAME"].ToString(),
        //                        PARTY_SHORT_NAME = reader["PARTY_SHORT_NAME"].ToString(),
        //                        ACT_NAME = reader["ACT_NAME"].ToString(),
        //                        CATEGORY = reader["CATEGORY"].ToString(),
        //                        PADDRESS = reader["PADDRESS"].ToString(),
        //                        NTN = reader["NTN"].ToString(),
        //                        CNIC = reader["CNIC"].ToString(),
        //                        CONTACT_PERSON = reader["CONTACT_PERSON"].ToString(),
        //                        CELL = reader["CELL"].ToString(),
        //                        WB = reader["WB"].ToString(),
        //                        TELL = reader["TELL"].ToString(),
        //                        EMAIL = reader["EMAIL"].ToString(),
        //                        WEBSITE = reader["WEBSITE"].ToString(),
        //                        CNIC_EXP = reader["CNIC_EXP"].ToString(),
        //                        REMARKS = reader["REMARKS"].ToString(),
        //                        PAYMENT_TERMS = reader["PAYMENT_TERMS"],
        //                        CREDIT_LIMIT = reader["CREDIT_LIMIT"],
        //                        SALESPERSON = reader["SALESPERSON"].ToString(),
        //                        SALES_ACT_NAME = reader["SALES_ACT_NAME"].ToString(),
        //                        GST = reader["GST"].ToString(),
        //                        SERVICE_TAX = reader["SERVICE_TAX"].ToString(),
        //                        F_NAME = reader["F_NAME"].ToString(),
        //                        ENTITY_NAME = reader["ENTITY_NAME"].ToString(),
        //                        STATUS_NAME = reader["STATUS_NAME"].ToString(),
        //                        ACCOUNT_NUM = reader["ACCOUNT_NUM"].ToString(),
        //                        BANK_NAME = reader["BANK_NAME"].ToString(),
        //                        BRANCH_NAME = reader["BRANCH_NAME"].ToString(),
        //                        TERMS_CONDITION = reader["TERMS_CONDITION"],
        //                        ADD_DATE = reader["ADD_DATE"],
        //                        ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"],
        //                        ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"],
        //                        EDIT_USER_ID = reader["EDIT_USER_ID"],
        //                        EDIT_DATE = reader["EDIT_DATE"],
        //                        EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"],
        //                        EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"],
        //                        ADD_POSTALCODE = reader["ADD_POSTALCODE"],
        //                        EDIT_POSTALCODE = reader["EDIT_POSTALCODE"],
        //                        REGION_NAME = reader["REGION_NAME"].ToString(),
        //                        ADD_USER_ID = reader["ADD_USER_ID"],
        //                        COMM = reader["COMM"].ToString()
        //                    };

        //                    jsonDataResult.Add(row);
        //                }

        //                reader.Close();
        //            }

        //            if (!string.IsNullOrEmpty(group))
        //            {

        //                JsonNode groupNode = JsonNode.Parse(group);
        //                JsonArray groupArray = groupNode.AsArray();
        //                var groupSelectors = groupArray.Select(g => g["selector"].ToString());
        //                string groupByClause = string.Join("", groupSelectors.Select(selector => selector));

        //                var groupedData = new List<object>();
        //                var grouped = jsonDataResult.GroupBy(d => _commonRepository.GetPropertyValue(d, groupByClause.ToUpper())).Select(g => new
        //                {
        //                    key = g.Key,
        //                    items = g.ToList()
        //                });
        //                groupedData.AddRange(grouped);
        //                response.data = new
        //                {
        //                    data = groupedData,
        //                    totalCount = totalCount,
        //                };
        //            }
        //            else
        //            {
        //                response.data = new
        //                {
        //                    data = jsonDataResult,
        //                    totalCount = totalCount
        //                };
        //            }
        //            response.msg = "";
        //            response.msgType = 1;
        //            return response;
        //        }
        //        else
        //        {
        //            response.data = "";
        //            response.msg = "Something went wrong! please try again later.";
        //            response.msgType = 2;
        //        }
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

        public MyHttpResponseMessage GetPartyTypeByPartyCode(int partyCode, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                int pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    pType = menu.PTYPE.Value;
                }

                if (!String.IsNullOrWhiteSpace(table) && pType > 0)
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        //string quefry = "SELECT T_CAT,WHT,EXEMPT_DATE, PARTY_CODE,PARTY_TYPE_CODE,PARTY_NAME,PARTY_SHORT_NAME,ACT_CODE,CAT_CODE,PADDRESS," +
                        //        "NTN,CNIC,CONTACT_PERSON,CELL,WB,TELL,EMAIL,WEBSITE,CNIC_EXP,REMARKS,PAYMENT_TERMS,CREDIT_LIMIT," +
                        //        "S_CODE,SACT_CODE,GST,SERVICE_TAX,F_CODE,ENTITY,ASTATUS,ACCOUNT_NUM,TAX,BANK_NAME,BRANCH_NAME,BANK_ADDRESS," +
                        //        "TERMS_CONDITION,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                        //        "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,MENU_ID,REGION,ADD_USER_ID,COMM,DISC,DOC_PIC,CNIC_PIC" +
                        //        " FROM " + table + " WHERE PARTY_TYPE_CODE = '" + pType + "' AND DLT = 'T' AND PARTY_CODE = '" + partyCode + "'";


                        string query = $@"SELECT T_CAT, WHT, EXEMPT_DATE, PARTY_CODE, PARTY_TYPE_CODE, PARTY_NAME, PARTY_SHORT_NAME, ACT_CODE, CAT_CODE, PADDRESS, NTN, CNIC, 
                                              CONTACT_PERSON, CELL, WB, TELL, EMAIL, WEBSITE, CNIC_EXP, REMARKS, PAYMENT_TERMS, CREDIT_LIMIT, S_CODE, SACT_CODE, 
                                              GST, SERVICE_TAX, F_CODE, ENTITY, ASTATUS, ACCOUNT_NUM, TAX, BANK_NAME, BRANCH_NAME, BANK_ADDRESS, TERMS_CONDITION, 
                                              ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, 
                                              EDIT_POSTALCODE, MENU_ID, REGION, ADD_USER_ID, COMM, DISC, DOC_PIC, CNIC_PIC 
                                              FROM {table} 
                                              WHERE PARTY_TYPE_CODE = '{pType}' 
                                              AND DLT = 'T' 
                                              AND PARTY_CODE = '{partyCode}'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new
                            {
                                T_CAT = reader["T_CAT"].ToString(),
                                WHT = reader["WHT"].ToString(),
                                EXEMPT_DATE = reader["EXEMPT_DATE"].ToString(),
                                ID = reader["PARTY_CODE"],
                                DOC_PIC = reader["DOC_PIC"],
                                CNIC_PIC = reader["CNIC_PIC"],
                                PARTY_TYPE_CODE = reader["PARTY_TYPE_CODE"],
                                PARTY_NAME = reader["PARTY_NAME"],
                                PARTY_SHORT_NAME = reader["PARTY_SHORT_NAME"].ToString(),
                                ACT_CODE = reader["ACT_CODE"],
                                CAT_CODE = reader["CAT_CODE"],
                                PADDRESS = reader["PADDRESS"].ToString(),
                                NTN = reader["NTN"].ToString(),
                                CNIC = reader["CNIC"].ToString(),
                                CONTACT_PERSON = reader["CONTACT_PERSON"].ToString(),
                                CELL = reader["CELL"].ToString(),
                                WB = reader["WB"].ToString(),
                                TELL = reader["TELL"].ToString(),
                                EMAIL = reader["EMAIL"].ToString(),
                                WEBSITE = reader["WEBSITE"].ToString(),
                                //CNIC_EXP = reader["CNIC_EXP"].ToString(),
                                CNIC_EXP = reader["CNIC_EXP"] == DBNull.Value || Convert.ToDateTime(reader["CNIC_EXP"]) == new DateTime(1900, 1, 1) ? null : Convert.ToDateTime(reader["CNIC_EXP"]).ToString("yyyy-MM-dd"),
                                REMARKS = reader["REMARKS"].ToString(),
                                PAYMENT_TERMS = reader["PAYMENT_TERMS"],
                                CREDIT_LIMIT = reader["CREDIT_LIMIT"],
                                S_CODE = reader["S_CODE"],
                                SACT_CODE = reader["SACT_CODE"],
                                GST = reader["GST"].ToString(),
                                SERVICE_TAX = reader["SERVICE_TAX"].ToString(),
                                F_CODE = reader["F_CODE"],
                                ENTITY = reader["ENTITY"],
                                ASTATUS = reader["ASTATUS"],
                                ACCOUNT_NUM = reader["ACCOUNT_NUM"].ToString(),
                                TAX = reader["TAX"] == DBNull.Value ? 0 : Convert.ToDouble(reader["TAX"]),
                                BANK_NAME = reader["BANK_NAME"].ToString(),
                                BANK_ADDRESS = reader["BANK_ADDRESS"].ToString(),
                                BRANCH_NAME = reader["BRANCH_NAME"].ToString(),
                                TERMS_CONDITION = reader["TERMS_CONDITION"],
                                ADD_DATE = reader["ADD_DATE"],
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"],
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"],
                                EDIT_USER_ID = reader["EDIT_USER_ID"],
                                EDIT_DATE = reader["EDIT_DATE"],
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"],
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"],
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"],
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"],
                                REGION = reader["REGION"],
                                MENU_ID = reader["MENU_ID"],
                                ADD_USER_ID = reader["ADD_USER_ID"],
                                COMM = reader["COMM"].ToString(),
                                DISC = reader["DISC"].ToString()
                            };
                            response.msgType = 1;
                            response.data = jsonDataResult;
                        }
                        reader.Close();
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

        public MyHttpResponseMessage GetBranchesInfoByPartyCode(int partyCode, Common common)
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
                        string Qurey = "SELECT CODE,PARTY_CODE,BRANCH_NAME,CONTACT_PERSON,EMAIL,CEL," +
                            "TEL,PB_ADD,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                            "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,MENU_ID,ADD_POSTALCODE," +
                            "EDIT_POSTALCODE,ASTATUS,ACT_CODE,ADD_USER_ID " +
                            "FROM " + table + "" +
                            " WHERE DLT = 'T' AND PARTY_CODE = '" + partyCode + "'  ORDER BY CODE DESC";

                        SqlCommand command = new SqlCommand(Qurey, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = reader["CODE"],
                                PARTY_CODE = reader["PARTY_CODE"],
                                BRANCH_NAME = reader["BRANCH_NAME"].ToString(),
                                CONTACT_PERSON = reader["CONTACT_PERSON"].ToString(),
                                EMAIL = reader["EMAIL"].ToString(),
                                CEL = reader["CEL"].ToString(),
                                TEL = reader["TEL"].ToString(),
                                PB_ADD = reader["PB_ADD"].ToString(),
                                ADD_DATE = reader["ADD_DATE"].ToString(),
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"].ToString(),
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"].ToString(),
                                EDIT_USER_ID = reader["EDIT_USER_ID"].ToString(),
                                EDIT_DATE = reader["EDIT_DATE"].ToString(),
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"].ToString(),
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"].ToString(),
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"].ToString(),
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"].ToString(),
                                ADD_USER_ID = reader["ADD_USER_ID"].ToString()
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

        public MyHttpResponseMessage SaveBranchInfo(PartyTypeBranch modelRecord, Common common)
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
                    var Ip = common.IPAddress;
                    var Computer = common.ComputerName;
                    var Postal = common.PostalCode;
                    string username = common.Username;
                    int Code = modelRecord.CODE;

                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;
                        try
                        {
                            string query = "";
                            string Duplicationquery = "";
                            if (modelRecord.CODE == 0)
                            {
                                string branchinfoID = GenerateNextDetailId(common, modelRecord.PARTY_CODE.Value);

                                query = "INSERT INTO  " + table + " " +
                                        "(CODE,PARTY_CODE,BRANCH_NAME,CONTACT_PERSON," +
                                        "EMAIL,CEL,TEL,PB_ADD,ADD_DATE,ADD_COMPUTER_NAME," +
                                        "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                        "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                        "MENU_ID,ADD_POSTALCODE,EDIT_POSTALCODE," +
                                        "ASTATUS,ACT_CODE,ADD_USER_ID,DLT)" +
                                        "VALUES" +
                                        "('" + branchinfoID + "','" + modelRecord.PARTY_CODE + "','" + modelRecord.BRANCH_NAME + "','" + modelRecord.CONTACT_PERSON + "'," +
                                        "'" + modelRecord.EMAIL + "','" + modelRecord.CEL + "','" + modelRecord.TEL + "','" + modelRecord.PB_ADD + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                        "'" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                        "'" + Computer + "','" + Ip + "'," +
                                        "'" + common.MenuID + "','" + Postal + "','" + Postal + "'," +
                                        "'Y','" + modelRecord.ACT_CODE + "','" + username + "','T')";
                                //SqlCommand command = new SqlCommand(query, connection);
                                //command.ExecuteNonQuery();
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE BRANCH_NAME = '" + modelRecord.BRANCH_NAME + "' AND PARTY_CODE = '" + modelRecord.PARTY_CODE + "' AND DLT = 'T'";
                                //SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
                                //int count = (int)CMD.ExecuteScalar();
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.data = branchinfoID;
                                    response.msgType = 1;
                                    response.msg = "Successfully.";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msgType = 2;
                                    response.msg = "Name Already Exist !....";
                                }
                            }
                            else
                            {
                                query = "UPDATE " + table + " SET BRANCH_NAME = '" + modelRecord.BRANCH_NAME + @"',
                                            CONTACT_PERSON = '" + modelRecord.CONTACT_PERSON + @"',
                                            EMAIL = '" + modelRecord.EMAIL + @"',
                                            CEL = '" + modelRecord.CEL + @"',
                                            TEL = '" + modelRecord.TEL + @"',
                                            PB_ADD = '" + modelRecord.PB_ADD + @"',
                                            EDIT_USER_ID = '" + username + @"',
                                            EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                            EDIT_COMPUTER_NAME = '" + Computer + @"',
                                            EDIT_IP_ADDRESS = '" + Ip + @"',
                                            EDIT_POSTALCODE = '" + Postal + @"',
                                            ASTATUS = 'Y',
                                            ACT_CODE = '" + modelRecord.ACT_CODE + @"'
                                            WHERE CODE = '" + modelRecord.CODE + @"' AND PARTY_CODE = '" + modelRecord.PARTY_CODE + @"' ";
                                //SqlCommand command = new SqlCommand(query, connection);
                                //command.ExecuteNonQuery();
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE BRANCH_NAME = '" + modelRecord.BRANCH_NAME + "' AND PARTY_CODE = '" + modelRecord.PARTY_CODE + "' AND DLT = 'T'";
                                //SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
                                //int count = (int)CMD.ExecuteScalar();
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.data = modelRecord.CODE;
                                    response.msg = "Record Updated Successfully";
                                    response.msgType = 1;
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msgType = 2;
                                    response.msg = "Name Already Exist !....";
                                }
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
                response.msg = _catchMessage;
                response.msgType = 2;
            }
            return response;
        }

        private string GenerateNextDetailId(Common common, int partyCode)
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
                    string maxIdQuery = "SELECT ISNULL(MAX(CODE), 0) + 1 FROM " + table + " WHERE PARTY_CODE = '" + partyCode + "'";
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(maxIdQuery, connection);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        int nextId = Convert.ToInt32(result);
                        return nextId.ToString();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        public MyHttpResponseMessage GetBranchInfoByBranchId(int branchId, int partyCode, Common common)
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
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT CODE,PARTY_CODE,BRANCH_NAME,CONTACT_PERSON,EMAIL,CEL," +
                                "TEL,PB_ADD,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                                "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,MENU_ID,ADD_POSTALCODE," +
                                "EDIT_POSTALCODE,ASTATUS,ACT_CODE,ADD_USER_ID " +
                        "FROM " + table + "" +
                                " WHERE DLT = 'T' AND CODE = '" + branchId + "' AND PARTY_CODE = '" + partyCode + "'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new
                            {
                                ID = reader["CODE"],
                                PARTY_CODE = reader["PARTY_CODE"],
                                BRANCH_NAME = reader["BRANCH_NAME"].ToString(),
                                CONTACT_PERSON = reader["CONTACT_PERSON"].ToString(),
                                EMAIL = reader["EMAIL"].ToString(),
                                CEL = reader["CEL"].ToString(),
                                TEL = reader["TEL"].ToString(),
                                PB_ADD = reader["PB_ADD"].ToString(),
                                ADD_DATE = reader["ADD_DATE"].ToString(),
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"].ToString(),
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"].ToString(),
                                EDIT_USER_ID = reader["EDIT_USER_ID"].ToString(),
                                EDIT_DATE = reader["EDIT_DATE"].ToString(),
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"].ToString(),
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"].ToString(),
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"].ToString(),
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"].ToString(),
                                ADD_USER_ID = reader["ADD_USER_ID"].ToString()
                            };
                            response.data = jsonDataResult;
                        }
                        reader.Close();
                    }
                    
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

        public MyHttpResponseMessage DeleteBranchInfo(int branchId, int partyCode, Common common)
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
                    if (branchId == 0)
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE CODE = '" + branchId + @"' AND PARTY_CODE = '" + partyCode + "'";
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

        public MyHttpResponseMessage GetPartyTypeByPartyCode(int partyCode)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT T_CAT,WHT,EXEMPT_DATE, PARTY_CODE,PARTY_TYPE_CODE,PARTY_NAME,PARTY_SHORT_NAME,ACT_CODE,CAT_CODE,PADDRESS," +
                                "NTN,CNIC,CONTACT_PERSON,CELL,WB,TELL,EMAIL,WEBSITE,CNIC_EXP,REMARKS,PAYMENT_TERMS,CREDIT_LIMIT," +
                                "S_CODE,SACT_CODE,GST,SERVICE_TAX,F_CODE,ENTITY,ASTATUS,ACCOUNT_NUM,BANK_NAME,BRANCH_NAME,BANK_ADDRESS," +
                                "TERMS_CONDITION,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,MENU_ID,REGION,ADD_USER_ID,COMM,DOC_PIC,CNIC_PIC" +
                                " FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND PARTY_CODE = '" + partyCode + "'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new 
                            {
                                T_CAT = reader["T_CAT"].ToString(),
                                WHT = reader["WHT"].ToString(),
                                EXEMPT_DATE = reader["EXEMPT_DATE"].ToString(),
                                ID = reader["PARTY_CODE"],
                                DOC_PIC = reader["DOC_PIC"],
                                CNIC_PIC = reader["CNIC_PIC"],
                                PARTY_TYPE_CODE = reader["PARTY_TYPE_CODE"],
                                PARTY_NAME = reader["PARTY_NAME"],
                                PARTY_SHORT_NAME = reader["PARTY_SHORT_NAME"].ToString(),
                                ACT_CODE = reader["ACT_CODE"],
                                CAT_CODE = reader["CAT_CODE"],
                                PADDRESS = reader["PADDRESS"].ToString(),
                                NTN = reader["NTN"].ToString(),
                                CNIC = reader["CNIC"].ToString(),
                                CONTACT_PERSON = reader["CONTACT_PERSON"].ToString(),
                                CELL = reader["CELL"].ToString(),
                                WB = reader["WB"].ToString(),
                                TELL = reader["TELL"].ToString(),
                                EMAIL = reader["EMAIL"].ToString(),
                                WEBSITE = reader["WEBSITE"].ToString(),
                                CNIC_EXP = reader["CNIC_EXP"].ToString(),
                                REMARKS = reader["REMARKS"].ToString(),
                                PAYMENT_TERMS = reader["PAYMENT_TERMS"],
                                CREDIT_LIMIT = reader["CREDIT_LIMIT"],
                                S_CODE = reader["S_CODE"],
                                SACT_CODE = reader["SACT_CODE"],
                                GST = reader["GST"].ToString(),
                                SERVICE_TAX = reader["SERVICE_TAX"].ToString(),
                                F_CODE = reader["F_CODE"],
                                ENTITY = reader["ENTITY"],
                                ASTATUS = reader["ASTATUS"],
                                ACCOUNT_NUM = reader["ACCOUNT_NUM"].ToString(),
                                BANK_NAME = reader["BANK_NAME"].ToString(),
                                BANK_ADDRESS = reader["BANK_ADDRESS"].ToString(),
                                BRANCH_NAME = reader["BRANCH_NAME"].ToString(),
                                TERMS_CONDITION = reader["TERMS_CONDITION"],
                                ADD_DATE = reader["ADD_DATE"],
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"],
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"],
                                EDIT_USER_ID = reader["EDIT_USER_ID"],
                                EDIT_DATE = reader["EDIT_DATE"],
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"],
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"],
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"],
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"],
                                REGION = reader["REGION"],
                                MENU_ID = reader["MENU_ID"],
                                ADD_USER_ID = reader["ADD_USER_ID"],
                                COMM = reader["COMM"].ToString()
                            };
                            response.msgType = 1;
                            response.data = jsonDataResult;
                        }
                        reader.Close();
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