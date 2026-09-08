using Azure;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class ItemGroupRepository : IItemGroupRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public ItemGroupRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetItemGroupsForTreeView(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<ItemGroup> itemGroups = new List<ItemGroup>();
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT A.GR_CODE + '-' + A.GROUP_NAME AS GROUP_NAME, A.GROUP_CODE, A.PARENT_CODE " +
                                   "FROM " + table + " A WHERE A.DLT = 'T' " +
                                   "ORDER BY A.GROUP_CODE";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            ItemGroup itemGroup = new ItemGroup();
                            itemGroup.GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]);
                            itemGroup.GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]);
                            itemGroup.PARENT_CODE = Convert.ToString(reader["PARENT_CODE"]);
                            itemGroups.Add(itemGroup);
                        }
                        reader.Close();
                    }
                    response.data = itemGroups;
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

        public MyHttpResponseMessage QuickSearch(Common common)
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
                        string query = "SELECT A.KOT_PRINTER, A.GR_CODE + '-' + A.GROUP_NAME AS GROUP_NAME, A.GROUP_NAME AS GROUP_NAME2, A.IPIC, A.GROUP_CODE, B.GROUP_NAME AS PARENT_CODE " +
                                       "FROM " + table + " A " +
                                       "LEFT JOIN " + table + " B ON A.PARENT_CODE = B.GROUP_CODE " +
                                       "WHERE A.DLT = 'T' ORDER BY A.GROUP_CODE";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new ItemGroup
                            {
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                K_PRINTER = Convert.ToString(reader["KOT_PRINTER"]) == "1" ? "Y" : "N",
                                GROUP_NAME2 = Convert.ToString(reader["GROUP_NAME2"]),
                                PARENT_CODE = Convert.ToString(reader["PARENT_CODE"]),
                                IPIC = Convert.ToString(reader["IPIC"]),
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"])
                            };

                            jsonDataResult.Add(row);
                        }
                        reader.Close();

                        response.data = jsonDataResult;
                        response.msg = "";
                        response.msgType = 1;
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

        public MyHttpResponseMessage Save(ItemGroup modelRecord, Common common)
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
                    var Ip = common.IPAddress;
                    var Computer = common.ComputerName;
                    var Postal = common.PostalCode;
                    var userid = common.Username;
                    string connectionString = new SQLService().getconnstring();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlTransaction transaction = connection.BeginTransaction();
                        SqlCommand command = connection.CreateCommand();
                        command.Transaction = transaction;

                        try
                        {
                            string query = "";
                            string Duplicationquery = "";
                            var GrCode = "0" + GenerateGrCode(Convert.ToString(modelRecord.PARENT_CODE), common);
                            if (modelRecord.GROUP_CODE == 0)
                            {
                                var nextId = GenerateNextId(common);
                                query = "INSERT INTO " + table + " " +
                                            "(GROUP_CODE,IPIC,GROUP_NAME,GR_CODE,PARENT_CODE,STICKER,KOT_PRINTER,STICKER_PRINTER," +
                                            "GROUP_TYPE,ASETUP,ITEM_TYPE,SALES_TAX," +
                                            "ADD_USER_ID,ADD_DATE," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                            "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                            "ADD_COMPUTER_NAME,MENU_ID,DLT)" +
                                            "VALUES" +
                                            "('" + nextId + "','" + modelRecord.IPIC + "','" + modelRecord.GROUP_NAME + "','" + GrCode + "','" + modelRecord.PARENT_CODE + "','" + modelRecord.STICKER + "','" + modelRecord.K_PRINTER + "','" + modelRecord.STK_PRINTER + "'," +
                                            "'" + modelRecord.GROUP_TYPE + "','" + modelRecord.ASETUP + "','" + modelRecord.ITEM_TYPE + "','" + modelRecord.SALES_TAX + "'," +
                                            "'" + common.Username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Ip + "','" + common.Username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                            "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                            "'" + Computer + "','" + common.MenuID + "','T')";
                                //SqlCommand command = new SqlCommand(query, connection);
                                //command.ExecuteNonQuery();
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE GROUP_NAME = '" + modelRecord.GROUP_NAME + "' AND DLT = 'T'";
                                //SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
                                //int count = (int)CMD.ExecuteScalar();
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.data = nextId;
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
                                query = "UPDATE " + table + " SET GROUP_NAME = '" + modelRecord.GROUP_NAME + @"',
                                        GROUP_TYPE = '" + modelRecord.GROUP_TYPE + @"',
                                        STICKER = '" + modelRecord.STICKER + @"',
                                        KOT_PRINTER = '" + modelRecord.K_PRINTER + @"',
                                        STICKER_PRINTER = '" + modelRecord.STK_PRINTER + @"',
                                        ASETUP = '" + modelRecord.ASETUP + @"',
                                        ITEM_TYPE = '" + modelRecord.ITEM_TYPE + @"',
                                        SALES_TAX = '" + modelRecord.SALES_TAX + @"',
                                        IPIC = '" + modelRecord.IPIC + @"',
                                        EDIT_USER_ID = '" + common.Username + @"',
                                        EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                        EDIT_COMPUTER_NAME = '" + Computer + @"',
                                        EDIT_IP_ADDRESS = '" + Ip + @"',
                                        EDIT_POSTALCODE = '" + Postal + @"',
                                        ASTATUS = '" + modelRecord.ASTATUS + @"'
                                        WHERE GROUP_CODE = '" + modelRecord.GROUP_CODE + "' AND MENU_ID = '" + common.MenuID + "'";
                                //SqlCommand command = new SqlCommand(query, connection);
                                //command.ExecuteNonQuery();
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE GROUP_NAME = '" + modelRecord.GROUP_NAME + "' AND DLT = 'T'";
                                //SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
                                //int count = (int)CMD.ExecuteScalar();
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count == 1)
                                {
                                    transaction.Commit();
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
                ItemGroup itemGroup = new ItemGroup();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE GROUP_CODE = {record.TRAN_ID} AND DLT = 'T'";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        itemGroup = new ItemGroup
                        {
                            GROUP_CODE = 0,
                            GROUP_NAME = record.PARTY_NAME,
                            GROUP_TYPE = Convert.ToString(reader["GROUP_TYPE"]),
                            ASETUP = Convert.ToInt32(reader["ASETUP"]),
                            ITEM_TYPE = Convert.ToInt32(reader["ITEM_TYPE"]),
                            SALES_TAX = Convert.ToInt32(reader["SALES_TAX"]),
                            PARENT_CODE = Convert.ToString(reader["PARENT_CODE"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            IPIC = Convert.ToString(reader["IPIC"]),
                        };
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(itemGroup, common);
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

        public string GenerateNextId(Common common)
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
                    string maxIdQuery = "SELECT ISNULL(MAX(GROUP_CODE), 0) + 1 FROM " + table;
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(maxIdQuery, connection);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        int nextId = Convert.ToInt32(result);
                        return Convert.ToString(nextId);
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public string GenerateGrCode(string ParentId, Common common)
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
                    string maxIdQuery = "SELECT CASE WHEN (select max(convert(bigint,GR_CODE))+1 from " + table + " where DLT = 'T' )   IS NULL THEN '01' " +
                                    "WHEN(SELECT COUNT(PARENT_CODE)FROM " + table + " where PARENT_CODE= '" + ParentId + "' AND DLT = 'T' ) = 0 THEN '0'+CONVERT(NVARCHAR(100)," +
                                    "(select (max(CONVERT(BIGINT,(GR_CODE)+'01'))) as act_groupCode from " + table + "  where GROUP_CODE= '" + ParentId + "' AND DLT = 'T') )" +
                                    " ELSE '0'+CONVERT(NVARCHAR(100),(select (max(CONVERT(BIGINT,(GR_CODE+1)))) as act_groupCode" +
                                    " from " + table + " where PARENT_CODE= '" + ParentId + "' AND DLT = 'T' ) ) END As ACCOUNT_GROUP_CODE";
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(maxIdQuery, connection);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        return Convert.ToString(Convert.ToInt64(result));
                    }
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public MyHttpResponseMessage GetItemGroupById(int id, Common common)
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
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT STICKER_PRINTER, KOT_PRINTER,STICKER, GROUP_CODE,GROUP_NAME,GROUP_TYPE,IPIC,ASETUP,ITEM_TYPE,SALES_TAX,GR_CODE,PARENT_CODE,ADD_USER_ID," +
                                       "ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID," +
                                       "EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,ADD_POSTALCODE," +
                                       "EDIT_POSTALCODE,ASTATUS,MENU_ID " +
                                       "FROM " + table + " " +
                                       "WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' AND GROUP_CODE = @Id";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Id", id);                        
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var itemGroup = new ItemGroup
                            {
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                                K_PRINTER = Convert.ToString(reader["KOT_PRINTER"]),
                                STICKER = Convert.ToString(reader["STICKER"]),
                                STK_PRINTER = Convert.ToString(reader["STICKER_PRINTER"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                GROUP_TYPE = Convert.ToString(reader["GROUP_TYPE"]),
                                ASETUP = Convert.ToInt32(reader["ASETUP"]),
                                ITEM_TYPE = Convert.ToInt32(reader["ITEM_TYPE"]),
                                SALES_TAX = Convert.ToInt32(reader["SALES_TAX"]),
                                GR_CODE = Convert.ToString(reader["GR_CODE"]),
                                IPIC = Convert.ToString(reader["IPIC"]),
                                PARENT_CODE = Convert.ToString(reader["PARENT_CODE"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"])
                            };
                            response.msg = "";
                            response.msgType = 1;
                            response.data = itemGroup;
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

        public MyHttpResponseMessage Delete(int id, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
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
                bool childExists = false;
                if (!String.IsNullOrWhiteSpace(table))
                {
                    if (id == 0)
                    {
                        response.msg = "ID is not in numeric format";
                        response.msgType = 2;
                    }
                    else
                    {
                        string checkQuerry = new SQLService().getconnstring();
                        using (SqlConnection connection = new SqlConnection(checkQuerry))
                        {
                            connection.Open();
                            string query = "SELECT GROUP_NAME FROM " + table + " WHERE DLT = 'T' AND PARENT_CODE = @ParentCode";

                            using (SqlCommand command = new SqlCommand(query, connection))
                            {
                                command.Parameters.AddWithValue("@ParentCode", id);

                                using (SqlDataReader reader = command.ExecuteReader())
                                {
                                    List<string> descriptions = new List<string>();
                                    while (reader.Read())
                                    {
                                        descriptions.Add(reader["GROUP_NAME"].ToString());
                                    }

                                    if (descriptions.Count > 0)
                                    {
                                        childExists = true;
                                        response.msgType = 2;
                                        response.msg = "The following child exist and must be deleted first:       " +
                                                       string.Join(", ", descriptions);
                                    }
                                }
                            }
                        }

                        if (!childExists)
                        {
                            string connectionString = new SQLService().getconnstring();
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                string query = "UPDATE " + table + " SET DLT = 'F' WHERE GROUP_CODE = '" + id + "'";
                                SqlCommand command = new SqlCommand(query, connection);
                                command.ExecuteNonQuery();
                                response.msg = "Record Deleted Successfully";
                                response.msgType = 1;
                            }
                        }
                    }
                }
                else
                {
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
    }
}