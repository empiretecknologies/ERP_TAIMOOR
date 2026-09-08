using Azure;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class BinariesRepository : IBinariesRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public BinariesRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage SourceDatabaseDDL(Binaries model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string connectionString = "";

                if (model.S_AUTH == "SQL")
                {
                    connectionString = $"Server={model.S_SERVER_HOST};" +
                                       $"Initial Catalog=master;" +
                                       $"User ID={model.S_USERNAME};" +
                                       $"Password={model.S_PASS};" +
                                       $"Encrypt=False;";
                }
                else if (model.S_AUTH == "Windows")
                {
                    connectionString = $"Server={model.S_SERVER_HOST};" +
                                       $"Initial Catalog=master;" +
                                       $"Integrated Security=True;" +
                                       $"Encrypt=False;";
                }
                else
                {
                    response.data = "";
                    return response;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        // Step: Get database names
                        SqlCommand cmd = new SqlCommand("SELECT name FROM sys.databases WHERE database_id > 4", conn);
                        SqlDataReader reader = cmd.ExecuteReader();

                        List<string> dbList = new List<string>();
                        while (reader.Read())
                        {
                            dbList.Add(reader.GetString(0)); // database name
                        }
                        reader.Close();

                        conn.Close();

                        response.data = dbList;
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
            }
            return response;
        }

        public MyHttpResponseMessage DestinationDatabaseDDL(Binaries model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string connectionString = "";

                if (model.D_AUTH == "SQL")
                {
                    connectionString = $"Server={model.D_SERVER_HOST};" +
                                       $"Initial Catalog=master;" +
                                       $"User ID={model.D_USERNAME};" +
                                       $"Password={model.D_PASS};" +
                                       $"Encrypt=False;";
                }
                else if (model.S_AUTH == "Windows")
                {
                    connectionString = $"Server={model.D_SERVER_HOST};" +
                                       $"Initial Catalog=master;" +
                                       $"Integrated Security=True;" +
                                       $"Encrypt=False;";
                }
                else
                {
                    response.data = "";
                    return response;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == ConnectionState.Open)
                    {
                        // Step: Get database names
                        SqlCommand cmd = new SqlCommand("SELECT name FROM sys.databases WHERE database_id > 4", conn);
                        SqlDataReader reader = cmd.ExecuteReader();

                        List<string> dbList = new List<string>();
                        while (reader.Read())
                        {
                            dbList.Add(reader.GetString(0)); // database name
                        }
                        reader.Close();

                        conn.Close();

                        response.data = dbList;
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
            }
            return response;
        }

        //public MyHttpResponseMessage GetDatabaseObjects(Binaries model)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        string connectionString = "";

        //        if (model.S_AUTH == "SQL")
        //        {
        //            connectionString = $"Server={model.S_SERVER_HOST};Initial Catalog={model.S_DATABASE};User ID={model.S_USERNAME};Password={model.S_PASS};Encrypt=False;";
        //        }
        //        else if (model.S_AUTH == "Windows")
        //        {
        //            connectionString = $"Server={model.S_SERVER_HOST};Initial Catalog={model.S_DATABASE};Integrated Security=True;Encrypt=False;";
        //        }
        //        else
        //        {
        //            response.msg = "Invalid authentication type.";
        //            response.msgType = 2;
        //            return response;
        //        }

        //        var dbTree = new List<TreeNode>();

        //        using (SqlConnection conn = new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            // Tables
        //            var tableList = new List<string>();
        //            using (SqlCommand cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", conn))
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    tableList.Add(reader.GetString(0));
        //                }
        //            }

        //            dbTree.Add(new TreeNode
        //            {
        //                id = "tables",
        //                name = "Tables",
        //                children = tableList.Select(x => new TreeNode { id = "table_" + x, name = x }).ToList()
        //            });

        //            // Views
        //            var viewList = new List<string>();
        //            using (SqlCommand cmd = new SqlCommand("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS", conn))
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    viewList.Add(reader.GetString(0));
        //                }
        //            }

        //            dbTree.Add(new TreeNode
        //            {
        //                id = "views",
        //                name = "Views",
        //                children = viewList.Select(x => new TreeNode { id = "view_" + x, name = x }).ToList()
        //            });

        //            // Stored Procedures
        //            var spList = new List<string>();
        //            using (SqlCommand cmd = new SqlCommand("SELECT name FROM sys.procedures", conn))
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    spList.Add(reader.GetString(0));
        //                }
        //            }

        //            dbTree.Add(new TreeNode
        //            {
        //                id = "sps",
        //                name = "Stored Procedures",
        //                children = spList.Select(x => new TreeNode { id = "sp_" + x, name = x }).ToList()
        //            });

        //            response.data = dbTree;
        //            response.msg = "Connection Successfull.";
        //            response.msgType = 1;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        response.msg = ex.Message;
        //        response.msgType = 2;
        //    }

        //    return response;
        //}

        public MyHttpResponseMessage GetDatabaseObjects(Binaries model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                string connectionString = "";
                if (model.DB_TYPE == "source")
                {
                    if (model.S_AUTH == "SQL")
                    {
                        connectionString = $"Server={model.S_SERVER_HOST};Initial Catalog={model.S_DATABASE};User ID={model.S_USERNAME};Password={model.S_PASS};Encrypt=False;";
                    }
                    else if (model.S_AUTH == "Windows")
                    {
                        connectionString = $"Server={model.S_SERVER_HOST};Initial Catalog={model.S_DATABASE};Integrated Security=True;Encrypt=False;";
                    }
                    else
                    {
                        response.msg = "Invalid authentication type.";
                        response.msgType = 2;
                        return response;
                    }
                }
                else if(model.DB_TYPE == "destination")
                {
                    if (model.D_AUTH == "SQL")
                    {
                        connectionString = $"Server={model.D_SERVER_HOST};Initial Catalog={model.D_DATABASE};User ID={model.D_USERNAME};Password={model.D_PASS};Encrypt=False;";
                    }
                    else if (model.S_AUTH == "Windows")
                    {
                        connectionString = $"Server={model.D_SERVER_HOST};Initial Catalog={model.D_DATABASE};Integrated Security=True;Encrypt=False;";
                    }
                    else
                    {
                        response.msg = "Invalid authentication type.";
                        response.msgType = 2;
                        return response;
                    }
                }


                var dbTree = new List<TreeNode>();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Helper to read list from SQL
                    List<TreeNode> GetNodes(string query, string prefix)
                    {
                        var list = new List<TreeNode>();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new TreeNode
                                {
                                    id = $"{prefix}_{reader.GetString(0)}",
                                    name = reader.GetString(0)
                                });
                            }
                        }
                        return list;
                    }

                    dbTree.Add(new TreeNode { id = "tables", name = "Tables", children = GetNodes("SELECT name FROM sys.tables ORDER BY name", "tbl") });
                    dbTree.Add(new TreeNode { id = "views", name = "Views", children = GetNodes("SELECT name FROM sys.views ORDER BY name", "view") });
                    dbTree.Add(new TreeNode { id = "sps", name = "Stored Procedures", children = GetNodes("SELECT name FROM sys.procedures ORDER BY name", "sp") });
                    dbTree.Add(new TreeNode { id = "functions", name = "Functions", children = GetNodes("SELECT name FROM sys.objects WHERE type IN ('FN','IF','TF') ORDER BY name", "fn") });
                    dbTree.Add(new TreeNode { id = "triggers", name = "Triggers", children = GetNodes("SELECT name FROM sys.triggers WHERE parent_class_desc = 'OBJECT_OR_COLUMN' ORDER BY name", "trg") });
                    dbTree.Add(new TreeNode { id = "synonyms", name = "Synonyms", children = GetNodes("SELECT name FROM sys.synonyms ORDER BY name", "syn") });
                    dbTree.Add(new TreeNode { id = "assemblies", name = "Assemblies", children = GetNodes("SELECT name FROM sys.assemblies ORDER BY name", "asm") });
                    dbTree.Add(new TreeNode { id = "roles", name = "Roles", children = GetNodes("SELECT name FROM sys.database_principals WHERE type = 'R' ORDER BY name", "role") });
                    dbTree.Add(new TreeNode { id = "sequences", name = "Sequences", children = GetNodes("SELECT name FROM sys.sequences ORDER BY name", "seq") });

                    response.data = dbTree;
                    response.msg = "Connection successful.";
                    response.msgType = 1;
                }
            }
            catch (Exception ex)
            {
                response.msg = ex.Message;
                response.msgType = 2;
            }

            return response;
        }

        //public MyHttpResponseMessage GetMissingCreateScripts(MissingObjectsModel model)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        string connStr = model.IsSource ? BuildConn(model, "source") : BuildConn(model, "destination");

        //        List<string> scripts = new List<string>();

        //        using (SqlConnection conn = new SqlConnection(connStr))
        //        {
        //            conn.Open();

        //            foreach (var obj in model.MissingObjects)
        //            {
        //                string script = GetCreateScript(obj.Type, obj.Name, conn);
        //                if (!string.IsNullOrEmpty(script))
        //                    scripts.Add(script + "\nGO\n");
        //            }
        //        }

        //        response.data = scripts;
        //        response.msg = "Scripts generated";
        //        response.msgType = 1;
        //    }
        //    catch (Exception ex)
        //    {
        //        response.msg = ex.Message;
        //        response.msgType = 2;
        //    }

        //    return response;
        //}

        //private string GetCreateScript(string objectType, string objectName, SqlConnection conn)
        //{
        //    string query = "";
        //    string result = "";

        //    if (objectType == "tbl")
        //    {
        //        // Table ke liye SQL Server ka built in stored procedure
        //        query = $"EXEC sp_helptext 'dbo.{objectName}'";
        //        // NOTE: Basic version — aur advance chahen to INFORMATION_SCHEMA use karna padega
        //    }
        //    else
        //    {
        //        // Views, SPs, Functions
        //        query = $@"
        //    SELECT m.definition
        //    FROM sys.sql_modules m
        //    JOIN sys.objects o ON m.object_id = o.object_id
        //    WHERE o.name = '{objectName}'";
        //    }

        //    using (SqlCommand cmd = new SqlCommand(query, conn))
        //    using (SqlDataReader rd = cmd.ExecuteReader())
        //    {
        //        while (rd.Read())
        //        {
        //            result += rd.GetString(0) + "\n";
        //        }
        //    }

        //    return result;
        //}


        //public MyHttpResponseMessage GetWarehouseAccountsForTreeView(Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        List<Warehouse> warehouseAccounts = new List<Warehouse>();
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            List<object> jsonDataResult = new List<object>();
        //            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //            {
        //                string query = "SELECT GR_CODE + '-' + DESCR AS DESCR, CODE, PARENT_CODE " +
        //                           "FROM " + table + " WHERE DLT = 'T' " +
        //                           "ORDER BY CODE";

        //                SqlCommand command = new SqlCommand(query, connection);
        //                connection.Open();
        //                SqlDataReader reader = command.ExecuteReader();

        //                while (reader.Read())
        //                {
        //                    Warehouse warehouseAccount = new Warehouse();
        //                    warehouseAccount.DESCR = Convert.ToString(reader["DESCR"]);
        //                    warehouseAccount.CODE = Convert.ToInt32(reader["CODE"]);
        //                    warehouseAccount.PARENT_CODE = Convert.ToInt32(reader["PARENT_CODE"]);
        //                    warehouseAccounts.Add(warehouseAccount);
        //                }
        //                reader.Close();
        //            }
        //            response.data = warehouseAccounts;
        //            response.msg = "";
        //            response.msgType = 1;
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

        //public MyHttpResponseMessage QuickSearch(Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            List<object> jsonDataResult = new List<object>();
        //            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //            {
        //                string query = "SELECT A.CODE, A.DESCR, A.GROUP_TYPE, B.DESCR AS PARENT_CODE, A.CONTACT_PERSON, A.TELL, A.CELL, A.ADDR, A.BCODE, " +
        //                               "A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, " +
        //                               "A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE, A.EDIT_POSTALCODE, A.ASTATUS, A.GR_CODE " +
        //                               "FROM " + table + " A " + 
        //                               "LEFT JOIN " + table + " B ON A.PARENT_CODE = B.CODE " +
        //                               "WHERE A.DLT = 'T' " +
        //                               "ORDER BY A.CODE";
        //                SqlCommand command = new SqlCommand(query, connection);
        //                connection.Open();
        //                SqlDataReader reader = command.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    var warehouse = new 
        //                    {
        //                        CODE = reader["CODE"] as int?,
        //                        DESCR = reader["DESCR"] as string,
        //                        GROUP_TYPE = reader["GROUP_TYPE"] as string,
        //                        PARENT_CODE = reader["PARENT_CODE"] as string,
        //                        CONTACT_PERSON = reader["CONTACT_PERSON"] as string,
        //                        TELL = reader["TELL"] as string,
        //                        CELL = reader["CELL"] as string,
        //                        ADDR = reader["ADDR"] as string,
        //                        BCODE = reader["BCODE"] as int?,
        //                        ADD_USER_ID = reader["ADD_USER_ID"] as string,
        //                        ADD_DATE = reader["ADD_DATE"] as DateTime?,
        //                        ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"] as string,
        //                        ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"] as string,
        //                        EDIT_USER_ID = reader["EDIT_USER_ID"] as string,
        //                        EDIT_DATE = reader["EDIT_DATE"] as DateTime?,
        //                        EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"] as string,
        //                        EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"] as string,
        //                        ADD_POSTALCODE = reader["ADD_POSTALCODE"] as string,
        //                        EDIT_POSTALCODE = reader["EDIT_POSTALCODE"] as string,
        //                        ASTATUS = reader["ASTATUS"] as string,
        //                        GR_CODE = reader["GR_CODE"] as string,
        //                    };
        //                    jsonDataResult.Add(warehouse);
        //                }
        //                reader.Close();
        //                response.data = jsonDataResult;
        //                response.msg = "";
        //                response.msgType = 1;
        //            }
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

        //public MyHttpResponseMessage Save(Warehouse modelRecord, Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            var Ip = common.IPAddress;
        //            var Computer = common.ComputerName;
        //            var Postal = common.PostalCode;
        //            var username = common.Username;
        //            string connectionString = new SQLService().getconnstring();
        //            using (SqlConnection connection = new SqlConnection(connectionString))
        //            {
        //                connection.Open();
        //                SqlTransaction transaction = connection.BeginTransaction();
        //                SqlCommand command = connection.CreateCommand();
        //                command.Transaction = transaction;
        //                try
        //                {
        //                    string query = "";
        //                    string Duplicationquery = "";
        //                    if (modelRecord.CODE == null || modelRecord.CODE == 0)
        //                    {
        //                        var newCode = GenerateNextId(common);
        //                        var GrCode = "0" + GenerateGrCode(Convert.ToString(modelRecord.PARENT_CODE), common);

        //                        query = "INSERT INTO " + table + " " +
        //                                "(CODE,DESCR,GROUP_TYPE,PARENT_CODE,GR_CODE," +
        //                                "CONTACT_PERSON,TELL,CELL,ADDR,BCODE," +
        //                                "ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME" +
        //                                ",ADD_IP_ADDRESS,MENU_ID" +
        //                                ",ADD_POSTALCODE,ASTATUS,DLT," +
        //                                "EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
        //                                "EDIT_POSTALCODE) " +
        //                                "VALUES " +
        //                                "('" + newCode + "','" + modelRecord.DESCR + "','" + modelRecord.GROUP_TYPE + "','" + modelRecord.PARENT_CODE + "','" + GrCode + "'," +
        //                                "'" + modelRecord.CONTACT_PERSON + "','" + modelRecord.TELL + "','" + modelRecord.CELL + "','" + modelRecord.ADDR + "','" + common.Branch + "'," +
        //                                "'" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
        //                                "'" + Ip + "','" + common.MenuID + "','" + Postal + "','" + modelRecord.ASTATUS + "','T'," +
        //                                "'" + modelRecord.EDIT_USER_ID + "','" + modelRecord.EDIT_DATE + "','" + modelRecord.EDIT_COMPUTER_NAME + "','" + modelRecord.EDIT_IP_ADDRESS + "'," +
        //                                "'" + modelRecord.EDIT_POSTALCODE + "')";
        //                        command.CommandText = query;
        //                        command.ExecuteNonQuery();

        //                        Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE DESCR = '" + modelRecord.DESCR + "' AND DLT = 'T'";
        //                        command.CommandText = Duplicationquery;
        //                        int count = (int)command.ExecuteScalar();
        //                        if (count == 1)
        //                        {
        //                            transaction.Commit();
        //                            response.msgType = 1;
        //                            response.data = newCode;
        //                            response.msg = "Record Added Successfully";
        //                        }
        //                        else
        //                        {
        //                            transaction.Rollback();
        //                            response.msgType = 2; 
        //                            response.msg = "Name Already Exist !....";                                    
        //                        }
        //                    }
        //                    else
        //                    {
        //                        query = "UPDATE " + table + " SET DESCR = '" + modelRecord.DESCR + @"',
        //                                GROUP_TYPE = '" + modelRecord.GROUP_TYPE + @"',
        //                                CONTACT_PERSON = '" + modelRecord.CONTACT_PERSON + @"',
        //                                TELL = '" + modelRecord.TELL + @"',
        //                                CELL = '" + modelRecord.CELL + @"',
        //                                ADDR = '" + modelRecord.ADDR + @"',
        //                                EDIT_USER_ID = '" + username + @"',
        //                                EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
        //                                EDIT_COMPUTER_NAME = '" + Computer + @"',
        //                                EDIT_IP_ADDRESS = '" + Ip + @"',
        //                                EDIT_POSTALCODE = '" + Postal + @"',
        //                                ASTATUS = '" + modelRecord.ASTATUS + @"'
        //                        WHERE CODE = '" + modelRecord.CODE + @"'";
        //                        command.CommandText = query;
        //                        command.ExecuteNonQuery();

        //                        Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE DESCR = '" + modelRecord.DESCR + "' AND DLT = 'T'";
        //                        command.CommandText = Duplicationquery;
        //                        int count = (int)command.ExecuteScalar();
        //                        if (count == 1)
        //                        {
        //                            transaction.Commit();
        //                            response.msgType = 1;
        //                            response.msg = "Record Updated Successfully";
        //                        }
        //                        else
        //                        {
        //                            transaction.Rollback();
        //                            response.msgType = 2;
        //                            response.msg = "Name Already Exist !....";
        //                        }
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    transaction.Rollback();
        //                    string _catchMessage = ex.Message;
        //                    if (ex.InnerException != null)
        //                    {
        //                        _catchMessage += "<br/>" + ex.InnerException.Message;
        //                    }
        //                    response.msg = _catchMessage;
        //                    response.msgType = 2;
        //                }
        //            }
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

        //public MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    response.msgType = 2;
        //    response.msg = "Data not found in our records";
        //    try
        //    {
        //        string? table = menu.TABLE1;
        //        string? table2 = menu.TABLE2;
        //        string connectionString = new SQLService().getconnstring();
        //        Warehouse warehouse = new Warehouse();

        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();
        //            string query = $@"SELECT * FROM {table} WHERE CODE = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch}";
        //            //string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
        //            SqlCommand command = new SqlCommand(query, connection);
        //            SqlDataReader reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                warehouse = new Warehouse
        //                {
        //                    DESCR = record.ITEM_NAME,
        //                    GROUP_TYPE = Convert.ToString(reader["GROUP_TYPE"]),
        //                    PARENT_CODE = Convert.ToInt32(reader["PARENT_CODE"]),
        //                    CONTACT_PERSON = Convert.ToString(reader["CONTACT_PERSON"]),
        //                    TELL = Convert.ToString(reader["TELL"]),
        //                    CELL = Convert.ToString(reader["CELL"]),
        //                    ADDR = Convert.ToString(reader["ADDR"]),
        //                    ASTATUS = Convert.ToString(reader["ASTATUS"]),
        //                };
        //            }
        //            reader.Close();
        //            connection.Close();
        //        }
        //        response = this.Save(warehouse, common);
        //        if (response.msgType == 1)
        //        {
        //            response.msg = "Record Copied Successfully";
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

        //public string GenerateNextId(Common common)
        //{
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            string maxIdQuery = "SELECT ISNULL(MAX(CODE), 0) + 1 FROM " + table;
        //            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //            {
        //                SqlCommand command = new SqlCommand(maxIdQuery, connection);
        //                connection.Open();
        //                object result = command.ExecuteScalar();
        //                int nextId = Convert.ToInt32(result);
        //                return Convert.ToString(nextId);
        //            }
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return string.Empty;
        //    }
        //}

        //public string GenerateGrCode(string ParentId, Common common)
        //{
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            string grCodeQuery = "SELECT CASE WHEN (select max(convert(bigint,GR_CODE))+1 from " + table + " where DLT = 'T' )   IS NULL THEN '01' " +
        //                                "WHEN(SELECT COUNT(PARENT_CODE)FROM " + table + " where PARENT_CODE= '" + ParentId + "' AND DLT = 'T' ) = 0 THEN '0'+CONVERT(NVARCHAR(100)," +
        //                                "(select (max(CONVERT(BIGINT,(GR_CODE)+'01'))) as act_groupCode from " + table + "  where CODE= '" + ParentId + "' AND DLT = 'T') )" +
        //                                " ELSE '0'+CONVERT(NVARCHAR(100),(select (max(CONVERT(BIGINT,(GR_CODE+1)))) as act_groupCode" +
        //                                " from " + table + " where PARENT_CODE= '" + ParentId + "' AND DLT = 'T' ) ) END As ACCOUNT_GROUP_CODE";

        //            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //            {
        //                SqlCommand command = new SqlCommand(grCodeQuery, connection);
        //                connection.Open();
        //                object result = command.ExecuteScalar();
        //                return Convert.ToString(Convert.ToInt64(result));
        //            }
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return string.Empty;
        //    }
        //}

        //public MyHttpResponseMessage GetWarehouseAccountById(int code, Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        object json = null;
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //            {
        //                string query = "SELECT CODE, DESCR, GROUP_TYPE, PARENT_CODE,CONTACT_PERSON,TELL,CELL,ADDR,GR_CODE," +
        //                               "ASTATUS FROM " + table + " WHERE CODE = '" + code + "'";

        //                SqlCommand command = new SqlCommand(query, connection);
        //                connection.Open();
        //                SqlDataReader reader = command.ExecuteReader();
        //                if (reader.Read())
        //                {
        //                    Warehouse warehouse = new Warehouse
        //                    {
        //                        CODE = reader["CODE"] as int?,
        //                        DESCR = reader["DESCR"] as string,
        //                        GROUP_TYPE = reader["GROUP_TYPE"] as string,
        //                        PARENT_CODE = reader["PARENT_CODE"] as int?,
        //                        CONTACT_PERSON = reader["CONTACT_PERSON"] as string,
        //                        TELL = reader["TELL"] as string,
        //                        CELL = reader["CELL"] as string,
        //                        ADDR = reader["ADDR"] as string,
        //                        ASTATUS = reader["ASTATUS"] as string,
        //                        GR_CODE = reader["GR_CODE"] as string,
        //                    };
        //                    json = warehouse;
        //                }
        //                reader.Close();
        //            }

        //            response.msg = "";
        //            response.msgType = 1;
        //            response.data = json;
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

        //public MyHttpResponseMessage Delete(int code, Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    response.msg = "Data not found in our records";
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            if (code == 0)
        //            {
        //                response.msg = "ID is not in numeric format";
        //                response.msgType = 2;
        //            }
        //            else
        //            {
        //                string connectionString = new SQLService().getconnstring();
        //                using (SqlConnection connection = new SqlConnection(connectionString))
        //                {
        //                    connection.Open();
        //                    string query = "UPDATE " + table + " SET DLT = 'F' WHERE CODE = '" + code + @"'";
        //                    SqlCommand command = new SqlCommand(query, connection);
        //                    command.ExecuteNonQuery();
        //                    response.msg = "Record Deleted Successfully";
        //                    response.msgType = 1;
        //                }
        //            }
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
    }
}