using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class CustomerPricingpRepository :ICustomerPricingRepository
    {

        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public IMenuRepository _menuRepository { get; set; }
        public CustomerPricingpRepository(IBranchRepository branchRepository, ICommonRepository commonRepository, IMenuRepository menuRepository)
        {
            _branchRepository = branchRepository;
            _commonRepository = commonRepository;
            _menuRepository = menuRepository;
        }
     

        public MyHttpResponseMessage QuickSearch(MyHttpResponseMessage Menu, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = string.Empty;
                int? pType;
                var menu = (Menu)Menu.data;
                table = menu.TABLE1;
                pType = menu.PTYPE;
                List<object> jsonDataResult = new List<object>();

                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {

                    string query = $@"
                                SELECT Distinct 
                                    S.GROUP_CODE,
                                    P.PARTY_NAME,
                                    p.PARTY_CODE
                                FROM 
                                    {table} AS S
                                INNER JOIN 
                                    TBL_PARTY_TYPES AS P 
                                    ON S.PARTY_CODE = P.PARTY_CODE
                                WHERE 
                                    P.PARTY_TYPE_CODE = 4
                                    AND S.DLT = 'T' 
                                    AND S.ASTATUS = 'Y'
                                ORDER BY 
                                    S.GROUP_CODE DESC";



                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            GROUP_CODE = Convert.ToString(reader["GROUP_CODE"]),
                            PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]),
                            PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
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

     
        private int GenerateNextId(Common common, SqlCommand command, Menu menu)
        {
            try
            {
                string? table = menu.TABLE1;
                string maxIdQuery = $"SELECT ISNULL(MAX(GROUP_CODE), 0) + 1 FROM {table}";
                command.CommandText = maxIdQuery;
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

       
        private int GenerateNextDetailId(SqlCommand command, Menu menu)
        {
            try
            {
                string? table = menu.TABLE1;
                string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) + 1 FROM {table}";
                command.CommandText = maxIdQuery;
                object result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public MyHttpResponseMessage Save(List<CommList> modelRecord, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                var ip = common.IPAddress;
                var computerName = common.ComputerName;
                var postalCode = common.PostalCode;
                var userid = common.Username;
                var branch = common.Branch;
                var periodID = common.Period;
                var menuID = common.MenuID;
                string connectionString = new SQLService().getconnstring();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = connection.CreateCommand();
                    command.Transaction = transaction;

                    try
                    {
                        string query = "", voucherNo = string.Empty;
                        bool IsMasterAdded = true, IsDetailAdded = true, IsNew = false;
                        int code = 0 , dt_code = 0;
                        var isInserted = false;
                        int? lastParty = 0;
                        int? lastAct = 0;
                        foreach (var item in modelRecord)
                        {
                            isInserted = false;

                            try
                            {
                                if (item.GROUP_CODE == null || item.GROUP_CODE == 0)
                                {
                                    IsNew = true;
                                    if (!(item.PARTY == lastParty && item.ACT_CODE == lastAct))
                                    {
                                        string checkQuery = @$"SELECT COUNT(*) FROM {table} 
                                                             WHERE PARTY_CODE = '{item.PARTY}'
                                                             AND ACT_CODE = '{item.ACT_CODE}'
                                                             AND DLT = 'T'";
                                                 command.CommandText = checkQuery;
                                        int existingCounts = (int)command.ExecuteScalar();

                                        if (existingCounts > 0)
                                        {
                                            response.msg = "Party Already Exist please Select Another One!";
                                            response.msgType = 2;
                                            return response;
                                        }

                                        lastParty = item.PARTY;
                                        lastAct = item.ACT_CODE;
                                    }

                                    //string checkQuery = $"SELECT COUNT(*) FROM {table} WHERE PARTY_CODE = '{item.PARTY}' AND ACT_CODE = '{item.ACT_CODE}' AND DLT='T'";
                                    //command.CommandText = checkQuery;
                                    //int existingCounts = (int)command.ExecuteScalar();

                                    //if (existingCounts > 0)
                                    //{
                                    //    throw new Exception("Party Already Exist please Select Another One!");
                                    //}
                                    //string checkQuerys = $@"SELECT COUNT(*) FROM {table} WHERE PARTY_CODE = '{item.PARTY}' AND ITEM_CODE = '{item.ITEM_CODE}' AND DLT = 'T'
                                    //                    AND NOT (GROUP_CODE = '{item.GROUP_CODE}' AND DT_CODE = '{item.DT_CODE}')";

                                    string checkQuerys = $"SELECT COUNT(*) FROM {table} WHERE PARTY_CODE = '{item.PARTY}' AND ACT_CODE = '{item.ACT_CODE}' AND ITEM_CODE = '{item.ITEM_CODE}' AND DLT = 'T'";
                                    command.CommandText = checkQuerys;
                                    int existingCount = (int)command.ExecuteScalar();

                                    if (existingCount > 0)
                                    {
                                        response.msg = "This item is already assigned to this Party";
                                        response.msgType = 2;
                                        return response;
                                    }

                                    if (code == 0)
                                    {
                                        code = GenerateNextId(common, command, menu);
                                        item.GROUP_CODE = code;
                                        if (code <= 0)
                                        {
                                            IsMasterAdded = false;
                                        }
                                    }
                                    //code = GenerateNextId(common, command, menu);
                                    if (dt_code == 0)
                                    {
                                        dt_code = GenerateNextDetailId(command, menu);
                                    }
                                    else
                                    {
                                        dt_code++;
                                    }
                                        

                                    if (true)
                                    {
                                        query = $"INSERT INTO {table} " +
                                                "(GROUP_CODE, PARTY_CODE, ACT_CODE, ITEM_CODE, RATE, DISC, ADV, " +
                                                "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, " +
                                                "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, DLT,DT_CODE) " +
                                                $"VALUES " +
                                                $"('{code}', '{item.PARTY}', '{item.ACT_CODE}', '{item.ITEM_CODE}', " +
                                                $"'{item.RATE}','{item.DISC}','{item.ADV}'," +
                                                $"'{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                $"'{computerName}', '{ip}', " +
                                                $"'{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{computerName}', " +
                                                $"'{ip}', '{postalCode}', '{postalCode}', " +
                                                $"'{item.ASTATUS}', '{menuID}', 'T',{dt_code})";

                                        command.CommandText = query;
                                        command.ExecuteNonQuery();
                                    }
                                    else
                                    {
                           
                                    }
                                }
                                else
                                {
                                  
                                    if (item.DT_CODE == null || item.DT_CODE == 0)

                                    {

                                        //var query2 = $"UPDATE {table} SET DLT = 'F' " +
                                        //           $"WHERE GROUP_CODE = '{item.GROUP_CODE}' AND DT_CODE = '{item.DT_CODE}'";
                                        //command.CommandText = query2;
                                        //command.ExecuteNonQuery();


                                        //string checkQuerys = $@"SELECT COUNT(*) FROM {table} WHERE PARTY_CODE = '{item.PARTY}' AND ACT_CODE='{item.ACT_CODE}' AND ITEM_CODE = '{item.ITEM_CODE}' AND DLT = 'T'
                                        //                AND NOT (GROUP_CODE = '{item.GROUP_CODE}' )";
                                        string checkQuerys = @$"SELECT COUNT(*) FROM {table} WHERE PARTY_CODE = '{item.PARTY}' 
                                                             AND ACT_CODE = '{item.ACT_CODE}' AND ITEM_CODE = '{item.ITEM_CODE}' AND GROUP_CODE='{item.GROUP_CODE}' AND DLT = 'T'";
                                        command.CommandText = checkQuerys;
                                        int existingCount = (int)command.ExecuteScalar();

                                        if (existingCount > 0)
                                        {
                                            throw new Exception("This item is already assigned to this Party");
                                        }
                                        dt_code = GenerateNextDetailId(command, menu);
                                        query = $"INSERT INTO {table} " +
                                              "(GROUP_CODE, PARTY_CODE, ACT_CODE, ITEM_CODE, RATE, DISC, ADV, " +
                                              "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, " +
                                              "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, DLT,DT_CODE) " +
                                              $"VALUES " +
                                              $"('{item.GROUP_CODE}', '{item.PARTY}', '{item.SACT_CODE}', '{item.ITEM_CODE}', " +
                                              $"'{item.RATE}','{item.DISC}','{item.ADV}', " +
                                              $"'{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                              $"'{computerName}', '{ip}', " +
                                              $"'{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{computerName}', " +
                                              $"'{ip}', '{postalCode}', '{postalCode}', " +
                                              $"'{item.ASTATUS}', '{menuID}', 'T',{dt_code})";
                                        command.CommandText = query;
                                        command.ExecuteNonQuery();
                                        isInserted = true;
                                        
                                    }
                                    else
                                    {
                                        var query2 = $"UPDATE {table} SET DLT = 'F' " +
                                                     $"WHERE GROUP_CODE = '{item.GROUP_CODE}' AND DT_CODE = '{item.DT_CODE}'";
                                        command.CommandText = query2;
                                        command.ExecuteNonQuery();

                                        if (!isInserted)
                                        {
                                            string checkQuery = $@"SELECT COUNT(*) FROM {table} WHERE PARTY_CODE = '{item.PARTY}' AND ACT_CODE='{item.ACT_CODE}' AND ITEM_CODE = '{item.ITEM_CODE}' AND DLT = 'T'
                                                        AND NOT (GROUP_CODE = '{item.GROUP_CODE}' AND DT_CODE = '{item.DT_CODE}')";
                                            command.CommandText = checkQuery;
                                            int existingCount = (int)command.ExecuteScalar();

                                            if (existingCount > 0)
                                            {
                                                throw new Exception("This item is already assigned to this Party");
                                            }
                                        }

                                        query = $"UPDATE {table} SET " +
                                                 $"PARTY_CODE = '{item.PARTY}', " +
                                                 $"ACT_CODE = '{item.ACT_CODE}', " +
                                                 $"ITEM_CODE = '{item.ITEM_CODE}', " +
                                                 $"RATE = '{item.RATE}', " +
                                                 $"DISC = '{item.DISC}', " +
                                                 $"ADV = '{item.ADV}', " +
                                                 $"EDIT_USER_ID = '{userid}', " +
                                                 $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                 $"EDIT_COMPUTER_NAME = '{computerName}', " +
                                                 $"EDIT_IP_ADDRESS = '{ip}', " +
                                                 $"EDIT_POSTALCODE = '{postalCode}', " +
                                                 $"ASTATUS = '{item.ASTATUS}', " +
                                                 $"MENU_ID = '{menuID}', " +
                                                 $"DLT = 'T' " +
                                                 $"WHERE GROUP_CODE = '{item.GROUP_CODE}' AND DT_CODE = '{item.DT_CODE}'";

                                        command.CommandText = query;
                                        command.ExecuteNonQuery();
                                    }
                                        
                                }

                            }
                            catch (Exception ex)
                            {
                                IsDetailAdded = false;
                                response.msg = ex.Message;
                                response.msgType = 2;
                                transaction.Rollback();
                                return response;
                            }
                        }

                        if (IsMasterAdded && IsDetailAdded)
                        {
                            transaction.Commit();
                            response.data = new
                            {
                                code = IsNew ? code : 0,

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


        public MyHttpResponseMessage GetCommisionMapByCode(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = "SELECT TOP 1 GROUP_CODE, PARTY_CODE, ACT_CODE, ITEM_CODE ,RATE, DISC, ADV, " +
                        "ASTATUS " +  
                        "FROM " + table + " " +
                        "WHERE DLT = 'T' AND GROUP_CODE = '" + code + "' " +
                        "ORDER BY GROUP_CODE";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            GROUP_CODE = Convert.ToString(reader["GROUP_CODE"]),
                            PARTY = Convert.ToInt32(reader["PARTY_CODE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            DISC = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDouble(reader["DISC"]),
                            ADV = reader["ADV"] == DBNull.Value ? "" : Convert.ToString(reader["ADV"]),
                            RATE = Convert.ToString(reader["RATE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
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

        public MyHttpResponseMessage GetCommisionMapDetailsByCode(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = @$"SELECT C.GROUP_CODE, C.ITEM_CODE, C.RATE,C.DT_CODE, DISC, ADV
                                       FROM {table} C
                                       WHERE C.DLT = 'T' AND C.GROUP_CODE = '{code}'
                                       ORDER BY C.GROUP_CODE DESC";


                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            GROUP_CODE = Convert.ToString(reader["GROUP_CODE"]),
                            DT_CODE = reader["DT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DT_CODE"]),
                            RATE = Convert.ToString(reader["RATE"]),
                            ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                            DISC = reader["DISC"] == DBNull.Value ? 0 : Convert.ToDouble(reader["DISC"]),
                            ADV = reader["ADV"] == DBNull.Value ? "" : Convert.ToString(reader["ADV"]),
                            //COMM_VALUE = Convert.ToString(reader["COMM_VALUE"]),
                            //ACT_CODE = Convert.ToInt32(reader["ACT_CODE"])
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

        public MyHttpResponseMessage Delete(int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string? table = menu.TABLE1;
                string connectionString = new SQLService().getconnstring();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE {table} SET DLT = 'F' WHERE GROUP_CODE = '{code}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    response.msgType = 1;
                    response.msg = "Record Deleted Successfully";
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

        public MyHttpResponseMessage CopyRecord(CopyRecordSalesman record, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string? table = menu.TABLE1;
                string? table2 = menu.TABLE2;
                string connectionString = new SQLService().getconnstring();
                List<CommList> commList = new List<CommList>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE GROUP_CODE = {record.GROUP_CODE}  AND DLT = 'T'";
                   
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new CommList
                        {
                            //GROUP_CODE = 0,
                            //V_DATE = record.V_DATE,
                            PARTY = record.PARTY,
                            ACT_CODE = record.ACT_CODE,
                            ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            RATE = Convert.ToString(reader["RATE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"])
                        };
                        commList.Add(row);
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(commList, common, menu);
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

     
        public MyHttpResponseMessage DeleteCommisionMapDetailByCode(int gcode, int code, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string? table = menu.TABLE1;
                var branch = common.Branch;
                var period = common.Period;
                string connectionString = new SQLService().getconnstring();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    //// Step 1: Check if related data exists in TBL_CC_DETAIL
                    //string checkQuery = $"SELECT COUNT(*) FROM {table} WHERE GROUP_CODE = '{gcode}' AND DT_CODE = '{code}' AND DLT = 'T'";
                    //SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                    //int relatedCount = (int)checkCommand.ExecuteScalar();

                    //if (relatedCount > 0)
                    //{
                    //    response.msgType = 2;
                    //    response.msg = "Record cannot be deleted. Related data exists in Cost Center.";
                    //    return response;
                    //}

                    // Step 2: Perform soft delete
                    string query = $"UPDATE {table} SET DLT = 'F' " +
                                   $"WHERE GROUP_CODE = '{gcode}' AND DT_CODE = '{code}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();

                    response.msgType = 1;
                    response.msg = "Record Deleted Successfully";
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
