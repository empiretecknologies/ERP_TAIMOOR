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
    public class CommMapRepository :ICommMapRepository
    {

        public IBranchRepository _branchRepository { get; set; }
        public ICommonRepository _commonRepository { get; set; }
        public IMenuRepository _menuRepository { get; set; }
        public CommMapRepository(IBranchRepository branchRepository, ICommonRepository commonRepository, IMenuRepository menuRepository)
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
                                    P.PARTY_NAME AS SALESMAN_NAME
                                FROM 
                                    {table} AS S
                                INNER JOIN 
                                    TBL_PARTY_TYPES AS P 
                                    ON S.SALESMAN = P.PARTY_CODE
                                WHERE 
                                    P.PARTY_TYPE_CODE = 9 
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
                            SALESMAN_NAME = Convert.ToString(reader["SALESMAN_NAME"]),
            
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
                        foreach (var item in modelRecord)
                        {
                            try
                            {
                                if (item.GROUP_CODE == null || item.GROUP_CODE == 0) // New record
                                {
                                    IsNew = true;


                                    string checkQuery = $"SELECT COUNT(*) FROM {table} WHERE SALESMAN = '{item.SALESMAN}'";
                                    command.CommandText = checkQuery;
                                    int existingCounts = (int)command.ExecuteScalar();

                                    if (existingCounts > 0)
                                    {
                                        throw new Exception("Salesman Already Exist please Select Another One!");
                                    }

                                    string checkQuerys = $"SELECT COUNT(*) FROM {table} WHERE SALESMAN = '{item.SALESMAN}' AND ITEM_CODE = '{item.ITEM_CODE}' AND DLT = 'T'";
                                    command.CommandText = checkQuerys;
                                    int existingCount = (int)command.ExecuteScalar();

                                    if (existingCount > 0)
                                    {
                                        // Already exists, stop and show error
                                        throw new Exception("This item is already assigned to this salesman");
                                    }

                                    if (code == 0)
                                    {
                                        code = GenerateNextId(common, command, menu);
                                        if (code <= 0)
                                        {
                                            IsMasterAdded = false;
                                        }
                                    }

                                    //item.GROUP_CODE = GenerateNextDetailId(command, menu);
                                    if(dt_code == 0)
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
                                                "(GROUP_CODE, SALESMAN, SACT_CODE, ITEM_CODE, COMM_UNIT, COMM_VALUE, " +
                                                "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, " +
                                                "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, DLT,DT_CODE) " +
                                                $"VALUES " +
                                                $"('{code}', '{item.SALESMAN}', '{item.SACT_CODE}', '{item.ITEM_CODE}', " +
                                                $"'{item.COMM_UNIT}', '{item.COMM_VALUE}', " +
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
                                        IsDetailAdded = false;
                                    }
                                }
                                else
                                {
                                    //Get the existing record from the database

                                   //string getExistingQuery = $"SELECT SALESMAN, ITEM_CODE FROM {table} WHERE GROUP_CODE = '{item.GROUP_CODE}' AND DLT = 'T'";
                                   // command.CommandText = getExistingQuery;
                                   // SqlDataReader reader = command.ExecuteReader();

                                   // int? existingSalesman = null;
                                   // int? existingItemCode = null;
                                   // if (reader.Read())
                                   // {
                                   //     if (int.TryParse(reader["SALESMAN"].ToString(), out int salesmanValue))
                                   //         existingSalesman = salesmanValue;

                                   //     if (int.TryParse(reader["SALESMAN"].ToString(), out int itemValue))
                                   //         existingSalesman = itemValue;
                                   // }
                                   // reader.Close();


                                   // if (item.SALESMAN != existingSalesman || item.ITEM_CODE != existingItemCode)
                                   // {
                                   //     string checkQuery = $"SELECT COUNT(*) FROM {table} " +
                                   //                         $"WHERE SALESMAN = '{item.SALESMAN}' " +
                                   //                         $"AND ITEM_CODE = '{item.ITEM_CODE}' " +
                                   //                         $"AND GROUP_CODE <> '{item.GROUP_CODE}' " +
                                   //                         $"AND DLT = 'T'";
                                   //     command.CommandText = checkQuery;
                                   //     int existingCount = (int)command.ExecuteScalar();

                                   //     if (existingCount > 0)
                                   //     {
                                   //         throw new Exception("This item is already assigned to this salesman");
                                   //     }
                                   // }
                                   
                                    if (item.DT_CODE == null || item.DT_CODE == 0)
                                    {
                                        string checkQuerys = $"SELECT COUNT(*) FROM {table} WHERE SALESMAN = '{item.SALESMAN}' AND ITEM_CODE = '{item.ITEM_CODE}' AND DLT = 'T'";
                                        command.CommandText = checkQuerys;
                                        int existingCount = (int)command.ExecuteScalar();

                                        if (existingCount > 0)
                                        {
                                            throw new Exception("This item is already assigned to this salesman");
                                        }

                                        dt_code = GenerateNextDetailId(command, menu);
                                        query = $"INSERT INTO {table} " +
                                              "(GROUP_CODE, SALESMAN, SACT_CODE, ITEM_CODE, COMM_UNIT, COMM_VALUE, " +
                                              "ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, EDIT_DATE, " +
                                              "EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, DLT,DT_CODE) " +
                                              $"VALUES " +
                                              $"('{item.GROUP_CODE}', '{item.SALESMAN}', '{item.SACT_CODE}', '{item.ITEM_CODE}', " +
                                              $"'{item.COMM_UNIT}', '{item.COMM_VALUE}', " +
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
                                        //var query2 = $"UPDATE {table} SET DLT = 'F' " +
                                        //          $"WHERE GROUP_CODE = '{item.GROUP_CODE}' AND DT_CODE = '{item.DT_CODE}'";
                                        //command.CommandText = query2;
                                        //command.ExecuteNonQuery();

                                        //if(!isInserted)
                                        //{
                                        //    string checkQuery = $"SELECT COUNT(*) FROM {table} WHERE SALESMAN = '{item.SALESMAN}' AND ITEM_CODE = '{item.ITEM_CODE}' AND DLT = 'T'";
                                        //    command.CommandText = checkQuery;
                                        //    int existingCount = (int)command.ExecuteScalar();

                                        //    if (existingCount > 0)
                                        //    {
                                        //        // Already exists, stop and show error
                                        //        throw new Exception("This item is already assigned to this salesman");
                                        //    }
                                        //}
                                        
                                        // Proceed with update
                                        query = $"UPDATE {table} SET " +
                                                 $"SALESMAN = '{item.SALESMAN}', " +
                                                 //$"SACT_CODE = '{item.SACT_CODE}', " +
                                                 $"ITEM_CODE = '{item.ITEM_CODE}', " +
                                                 $"COMM_UNIT = '{item.COMM_UNIT}', " +
                                                 $"COMM_VALUE = '{item.COMM_VALUE}', " +
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
                    string query = "SELECT TOP 1 GROUP_CODE, SALESMAN, ITEM_CODE, SACT_CODE, COMM_UNIT, COMM_VALUE, " +
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
                            //V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                            COMM_UNIT = Convert.ToString(reader["COMM_UNIT"]),
                            SALESMAN = Convert.ToInt32(reader["SALESMAN"]),
                      
                            ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            SACT_CODE = Convert.ToInt32(reader["SACT_CODE"]),


                            COMM_VALUE = Convert.ToInt32(reader["COMM_VALUE"]),

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
                    //string query = "SELECT GROUP_CODE,DT_CODE, SACT_CODE, SALESMAN, COMM_VALUE, COMM_UNIT, " +
                    //               "ITEM_CODE" +
                    //               $"FROM {table} WHERE DLT = 'T' AND GROUP_CODE = '{code}'  " +
                    //               $" ORDER BY DT_CODE DESC";

                  

                    string query = @$"SELECT C.GROUP_CODE, C.ITEM_CODE, C.COMM_UNIT, C.COMM_VALUE,C.SACT_CODE,C.DT_CODE
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
                            COMM_UNIT = Convert.ToString(reader["COMM_UNIT"]),
                            ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                            COMM_VALUE = Convert.ToString(reader["COMM_VALUE"]),
                            SACT_CODE = Convert.ToString(reader["SACT_CODE"])
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
                    //string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new CommList
                        {
                            GROUP_CODE = 0,
                            //V_DATE = record.V_DATE,
                            SALESMAN = record.SALESMAN,
                            SACT_CODE = Convert.ToInt32(reader["SACT_CODE"]),
                            ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            COMM_UNIT = Convert.ToString(reader["COMM_UNIT"]),
                            COMM_VALUE = Convert.ToInt32(reader["COMM_VALUE"]),
                            DT_CODE = 0,
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
