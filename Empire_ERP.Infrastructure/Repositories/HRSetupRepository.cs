using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class HRSetupRepository : IHRSetupRepository
    {
        public ICommonRepository _commonRepository { get; set; }
        public IPeriodRepository _periodRepository { get; set; }
        public IMenuRepository _menuRepository { get; set; }

        public HRSetupRepository(IMenuRepository menuRepository, ICommonRepository commonRepository, IPeriodRepository periodRepository)
        {
            _menuRepository = menuRepository;
            _commonRepository = commonRepository;
            _periodRepository = periodRepository;
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
                    string maxIdQuery = "SELECT ISNULL(MAX(EMP_CODE), 0) + 1 FROM " + table;
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

        public MyHttpResponseMessage Save(HRSetup modelRecord, Common common)
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
                }

                if (!String.IsNullOrWhiteSpace(table))
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
                            string query = "";
                            string IsInsert = "";

                            foreach (var item in modelRecord.Holiday.ToList())
                            {
                                if (item.GROUP_CODE == 0)
                                {
                                    item.GROUP_CODE = Convert.ToInt32(GenerateNextId(common, pType));
                                    //query = $@"INSERT INTO {table} 
                                    //        ([EMP_CODE],[EMP_ID],[MACHINE_CODE],[ENAME],[DEP_ID],[FATHER_NAME],[DESIG],
                                    //        [GENDER],[BCODE],[SHIFT_T],[EMP_TYPE],[CELL_NO],[EMAIL],[REG],[CHILD],
                                    //        [PAY_MODE],[JOIN_DATE],[PARM_DATE],[SALARY_HOLD],[CNIC],[FAMILY_NUM],[BANK_ACC],
                                    //        [CNIC_IDATE],[CNIC_EDATE],[BANK_NAME],[NTN_NO],[FILE_NO],[EMP_ADD],[NATION],[POB],
                                    //        [DOB],[MSTATUS],[MSTAFF],[RSTATUS],[THUMB],[SIGNA],[EMP_IMG],[OT],[EMP_CAST],
                                    //        [EDUCTION],[VEH_NUMBER],[LTYPE],[LNUMBER],[L_IDATE],[L_EDATE],[LEFT_DATE],[REASON_L],
                                    //        [ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME],[ADD_IP_ADDRESS],[ADD_POSTALCODE],[ASTATUS],[DLT]) 
                                    //        VALUES 
                                    //        ('{modelRecord.EMP_CODE}', '{modelRecord.EMP_ID}', '{modelRecord.MACHINE_CODE}', '{modelRecord.ENAME}', '{modelRecord.DEP_ID}', '{modelRecord.FATHER_NAME}', '{modelRecord.DESIG}',
                                    //        '{modelRecord.GENDER}', '{modelRecord.BCODE}', '{modelRecord.SHIFT_T}', '{modelRecord.EMP_TYPE}', '{modelRecord.CELL_NO}', '{modelRecord.EMAIL}', '{modelRecord.REG}', '{modelRecord.CHILD}', 
                                    //        '{modelRecord.PAY_MODE}', '{modelRecord.JOIN_DATE}', '{modelRecord.PARM_DATE}', '{modelRecord.SALARY_HOLD}', '{modelRecord.CNIC}', '{modelRecord.FAMILY_NUM}', '{modelRecord.BANK_ACC}', 
                                    //        '{modelRecord.CNIC_IDATE}', '{modelRecord.CNIC_EDATE}', '{modelRecord.BANK_NAME}', '{modelRecord.NTN_NO}', '{modelRecord.FILE_NO}', '{modelRecord.EMP_ADD}', '{modelRecord.NATION}', '{modelRecord.POB}', 
                                    //        '{modelRecord.DOB}', '{modelRecord.MSTATUS}', '{modelRecord.MSTAFF}', '{modelRecord.RSTATUS}', '{modelRecord.THUMB}', '{modelRecord.SIGNA}', '{modelRecord.EMP_IMG}', '{modelRecord.OT}', '{modelRecord.EMP_CAST}', 
                                    //        '{modelRecord.EDUCTION}', '{modelRecord.VEH_NUMBER}', '{modelRecord.LTYPE}', '{modelRecord.LNUMBER}', '{modelRecord.L_IDATE}', '{modelRecord.L_EDATE}', '{modelRecord.LEFT_DATE}', '{modelRecord.REASON_L}', 
                                    //        '{username}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', '{Ip}', '{Postal}', '{modelRecord.ASTATUS}', 'T')";

                                    command.CommandText = query;
                                    command.ExecuteNonQuery();


                                }
                                else
                                {
                                    //query = $@"UPDATE {table} SET 
                                    //           [EMP_ID] = '{modelRecord.EMP_ID}'
                                    //          ,[MACHINE_CODE] = '{modelRecord.MACHINE_CODE}'
                                    //          ,[ENAME] = '{modelRecord.ENAME}'
                                    //          ,[DEP_ID] = '{modelRecord.DEP_ID}'
                                    //          ,[FATHER_NAME] = '{modelRecord.FATHER_NAME}'
                                    //          ,[DESIG] = '{modelRecord.DESIG}'
                                    //          ,[GENDER] = '{modelRecord.GENDER}'
                                    //          ,[BCODE] = '{modelRecord.BCODE}'
                                    //          ,[SHIFT_T] = '{modelRecord.SHIFT_T}'
                                    //          ,[EMP_TYPE] = '{modelRecord.EMP_TYPE}'
                                    //          ,[CELL_NO] = '{modelRecord.CELL_NO}'
                                    //          ,[EMAIL] = '{modelRecord.EMAIL}'
                                    //          ,[REG] = '{modelRecord.REG}'
                                    //          ,[CHILD] = '{modelRecord.CHILD}'
                                    //          ,[PAY_MODE] = '{modelRecord.PAY_MODE}'
                                    //          ,[JOIN_DATE] = '{modelRecord.JOIN_DATE}'
                                    //          ,[PARM_DATE] = '{modelRecord.PARM_DATE}'
                                    //          ,[SALARY_HOLD] = '{modelRecord.SALARY_HOLD}'
                                    //          ,[CNIC] = '{modelRecord.CNIC}'
                                    //          ,[FAMILY_NUM] = '{modelRecord.FAMILY_NUM}'
                                    //          ,[BANK_ACC] = '{modelRecord.BANK_ACC}'
                                    //          ,[CNIC_IDATE] = '{modelRecord.CNIC_IDATE}'
                                    //          ,[CNIC_EDATE] = '{modelRecord.CNIC_EDATE}'
                                    //          ,[BANK_NAME] = '{modelRecord.BANK_NAME}'
                                    //          ,[NTN_NO] = '{modelRecord.NTN_NO}'
                                    //          ,[FILE_NO] = '{modelRecord.FILE_NO}'
                                    //          ,[EMP_ADD] = '{modelRecord.EMP_ADD}'
                                    //          ,[NATION] = '{modelRecord.NATION}'
                                    //          ,[POB] = '{modelRecord.POB}'
                                    //          ,[DOB] = '{modelRecord.DOB}'
                                    //          ,[MSTATUS] = '{modelRecord.MSTATUS}'
                                    //          ,[MSTAFF] = '{modelRecord.MSTAFF}'
                                    //          ,[RSTATUS] = '{modelRecord.RSTATUS}'
                                    //          ,[THUMB] = '{modelRecord.THUMB}'
                                    //          ,[SIGNA] = '{modelRecord.SIGNA}'
                                    //          ,[EMP_IMG] = '{modelRecord.EMP_IMG}'
                                    //          ,[OT] = '{modelRecord.OT}'
                                    //          ,[EMP_CAST] = '{modelRecord.EMP_CAST}'
                                    //          ,[EDUCTION] = '{modelRecord.EDUCTION}'
                                    //          ,[VEH_NUMBER] = '{modelRecord.VEH_NUMBER}'
                                    //          ,[LTYPE] = '{modelRecord.LTYPE}'
                                    //          ,[LNUMBER] = '{modelRecord.LNUMBER}'
                                    //          ,[L_IDATE] = '{modelRecord.L_IDATE}'
                                    //          ,[L_EDATE] = '{modelRecord.L_EDATE}'
                                    //          ,[LEFT_DATE] = '{modelRecord.LEFT_DATE}'
                                    //          ,[REASON_L] = '{modelRecord.REASON_L}'
                                    //          ,[EDIT_USER_ID] = '{username}'
                                    //          ,[EDIT_DATE] = '{CommonService.GetDateTime("Pakistan Standard Time")}'
                                    //          ,[EDIT_COMPUTER_NAME] = '{Computer}'
                                    //          ,[EDIT_IP_ADDRESS] = '{Ip}'
                                    //          ,[EDIT_POSTALCODE] = '{Postal}'
                                    //          ,[ASTATUS] = '{modelRecord.ASTATUS}'
                                    //     WHERE EMP_CODE = {modelRecord.EMP_CODE}";

                                    command.CommandText = query;
                                    command.ExecuteNonQuery();

                                }
                            }
                            transaction.Commit();
                            response.msgType = 1;
                            response.msg = IsInsert = true ? "Record Added Successfully" : "Record Updated Successfully";
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

        public MyHttpResponseMessage Delete(int Code, int actCode, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
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
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    if (Code == 0)
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
                            string query = $@"UPDATE {table} SET DLT = 'F' WHERE EMP_CODE = {Code}";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteScalar();
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

        public MyHttpResponseMessage QuickSearchHRSetup(Common common)
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

                if (!String.IsNullOrWhiteSpace(table))
                {
                    List<object> jsonDataResult = new List<object>();

                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"SELECT EM.EMP_CODE,EM.MACHINE_CODE,EM.EMP_IMG ,
                                      EM.EMP_ID,EM.ENAME,EM.FATHER_NAME,AG.DESCR AS DEP_NAME,SFT.GROUP_NAME AS SHIFT_T,
                                      EM.EMAIL,EM.CELL_NO,R.GROUP_NAME AS REG,EM.JOIN_DATE,EM.PARM_DATE,
                                      CASE WHEN EM.SALARY_HOLD = 'Y' THEN 'Freeze' Else 'Active' End AS SALARY_HOLD,
                                      CASE WHEN EM.MSTATUS = 'Y' THEN 'Management' else 'Non-Management' end AS MSTATUS
                                      FROM {table} EM
                                      LEFT OUTER JOIN TBL_ACT_GROUP AG
                                      ON AG.CODE = EM.DEP_ID
                                      LEFT OUTER JOIN TBL_SHIFT SFT
                                      ON SFT.GROUP_CODE = EM.SHIFT_T
                                      LEFT OUTER JOIN TBL_RELIGION R
                                      ON R.GROUP_CODE = EM.REG
                                      WHERE EM.DLT = 'T'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = reader["EMP_CODE"]
                                ,
                                EMP_ID = reader["EMP_ID"].ToString()
                                ,
                                MACHINE_CODE = reader["MACHINE_CODE"].ToString()
                                ,
                                ENAME = reader["ENAME"].ToString()
                                ,
                                DEP_ID = reader["DEP_NAME"].ToString()
                                ,
                                FATHER_NAME = reader["FATHER_NAME"].ToString()
                                ,
                                SHIFT_T = reader["SHIFT_T"].ToString()
                                ,
                                CELL_NO = reader["CELL_NO"].ToString()
                                ,
                                EMAIL = reader["EMAIL"].ToString()
                                ,
                                REG = reader["REG"].ToString()
                                ,
                                JOIN_DATE = reader["JOIN_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["JOIN_DATE"]).ToString("yyyy-MM-dd")
                                ,
                                PARM_DATE = reader["PARM_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["PARM_DATE"]).ToString("yyyy-MM-dd")
                                ,
                                SALARY_HOLD = reader["SALARY_HOLD"].ToString()
                                ,
                                MSTATUS = reader["MSTATUS"].ToString()
                                ,
                                EMP_IMG = reader["EMP_IMG"].ToString()

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

        public MyHttpResponseMessage GetHolidayRecords(Common common)
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
                }

                List<object> jsonDataResult = new List<object>();

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $@"SELECT GROUP_CODE , ASTATUS , GROUP_NAME FROM TBL_HR_HOLIDAYS EM
                                      WHERE EM.DLT = 'T'";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var row = new
                        {
                            GROUP_CODE = reader["GROUP_CODE"]
                            ,
                            ASTATUS = reader["ASTATUS"]
                            ,
                            GROUPNAME = reader["GROUPNAME"].ToString()

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

        public MyHttpResponseMessage GetHRSetupByHRSetupCode(int code, Common common)
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
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"SELECT [EMP_CODE], [EMP_ID], [MACHINE_CODE], [ENAME], [DEP_ID], [FATHER_NAME], [DESIG], [GENDER], [BCODE], [SHIFT_T]
                                    , [EMP_TYPE], [CELL_NO], [EMAIL], [REG], [CHILD], [PAY_MODE], [JOIN_DATE], [PARM_DATE], [SALARY_HOLD]
                                    , [CNIC], [FAMILY_NUM], [BANK_ACC], [CNIC_IDATE], [CNIC_EDATE], [BANK_NAME], [NTN_NO], [FILE_NO], [EMP_ADD]
                                    , [NATION], [POB], [DOB], [MSTATUS], [MSTAFF], [RSTATUS], [THUMB], [SIGNA], [EMP_IMG], [OT], [EMP_CAST]
                                    , [EDUCTION], [VEH_NUMBER], [LTYPE], [LNUMBER], [L_IDATE], [L_EDATE], [LEFT_DATE], [REASON_L], [ADD_USER_ID]
                                    , [ADD_DATE], [ADD_COMPUTER_NAME], [ADD_IP_ADDRESS], [EDIT_USER_ID], [EDIT_DATE], [EDIT_COMPUTER_NAME]
                                    , [EDIT_IP_ADDRESS], [ADD_POSTALCODE], [EDIT_POSTALCODE], [ASTATUS], [MENU_ID], [DLT] 
                                      FROM {table} WHERE EMP_CODE = {code} AND DLT = 'T'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new
                            {
                                EMP_CODE = reader["EMP_CODE"]
                                ,
                                EMP_ID = Convert.ToString(reader["EMP_ID"])
                                ,
                                MACHINE_CODE = Convert.ToString(reader["MACHINE_CODE"])
                                ,
                                ENAME = Convert.ToString(reader["ENAME"])
                                ,
                                DEP_ID = Convert.ToInt32(reader["DEP_ID"])
                                ,
                                FATHER_NAME = Convert.ToString(reader["FATHER_NAME"])
                                ,
                                DESIG = Convert.ToInt32(reader["DESIG"])
                                ,
                                GENDER = Convert.ToString(reader["GENDER"])
                                ,
                                BCODE = Convert.ToInt32(reader["BCODE"])
                                ,
                                SHIFT_T = Convert.ToInt32(reader["SHIFT_T"])
                                ,
                                EMP_TYPE = Convert.ToString(reader["EMP_TYPE"])
                                ,
                                CELL_NO = Convert.ToString(reader["CELL_NO"])
                                ,
                                EMAIL = Convert.ToString(reader["EMAIL"])
                                ,
                                REG = Convert.ToInt32(reader["REG"])
                                ,
                                CHILD = Convert.ToString(reader["CHILD"])
                                ,
                                PAY_MODE = Convert.ToString(reader["PAY_MODE"])
                                ,
                                JOIN_DATE = reader["JOIN_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["JOIN_DATE"]).ToString("yyyy-MM-dd")
                                ,
                                PARM_DATE = reader["PARM_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["PARM_DATE"]).ToString("yyyy-MM-dd")
                                ,
                                SALARY_HOLD = Convert.ToString(reader["SALARY_HOLD"])
                                ,
                                CNIC = Convert.ToString(reader["CNIC"])
                                ,
                                FAMILY_NUM = Convert.ToString(reader["FAMILY_NUM"])
                                ,
                                BANK_ACC = Convert.ToString(reader["BANK_ACC"])
                                ,
                                CNIC_IDATE = reader["CNIC_IDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["CNIC_IDATE"]).ToString("yyyy-MM-dd")
                                ,
                                CNIC_EDATE = reader["CNIC_EDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["CNIC_EDATE"]).ToString("yyyy-MM-dd")
                                ,
                                BANK_NAME = Convert.ToString(reader["BANK_NAME"])
                                ,
                                NTN_NO = Convert.ToString(reader["NTN_NO"])
                                ,
                                FILE_NO = Convert.ToString(reader["FILE_NO"])
                                ,
                                EMP_ADD = Convert.ToString(reader["EMP_ADD"])
                                ,
                                NATION = Convert.ToString(reader["NATION"])
                                ,
                                POB = Convert.ToString(reader["POB"])
                                ,
                                DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd")
                                ,
                                MSTATUS = Convert.ToString(reader["MSTATUS"])
                                ,
                                MSTAFF = Convert.ToString(reader["MSTAFF"])
                                ,
                                RSTATUS = Convert.ToString(reader["RSTATUS"])
                                ,
                                THUMB = Convert.ToString(reader["THUMB"])
                                ,
                                SIGNA = Convert.ToString(reader["SIGNA"])
                                ,
                                EMP_IMG = Convert.ToString(reader["EMP_IMG"])
                                ,
                                OT = Convert.ToString(reader["OT"])
                                ,
                                EMP_CAST = Convert.ToString(reader["EMP_CAST"])
                                ,
                                EDUCTION = Convert.ToInt32(reader["EDUCTION"])
                                ,
                                VEH_NUMBER = Convert.ToString(reader["VEH_NUMBER"])
                                ,
                                LTYPE = Convert.ToString(reader["LTYPE"])
                                ,
                                LNUMBER = Convert.ToString(reader["LNUMBER"])
                                ,
                                L_IDATE = reader["L_IDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["L_IDATE"]).ToString("yyyy-MM-dd")
                                ,
                                L_EDATE = reader["L_EDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["L_EDATE"]).ToString("yyyy-MM-dd")
                                ,
                                LEFT_DATE = reader["LEFT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["LEFT_DATE"]).ToString("yyyy-MM-dd")
                                ,
                                REASON_L = Convert.ToString(reader["REASON_L"])
                                ,
                                ASTATUS = Convert.ToString(reader["ASTATUS"])
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

        public MyHttpResponseMessage GetHRSetupByHRSetupCode(int partyCode)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "SELECT PARTY_CODE,PARTY_TYPE_CODE,PARTY_NAME,PARTY_SHORT_NAME,ACT_CODE,CAT_CODE,PADDRESS," +
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