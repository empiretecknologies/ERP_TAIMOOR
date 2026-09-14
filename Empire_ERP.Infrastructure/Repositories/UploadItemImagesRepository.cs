using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class UploadItemImagesRepository : IUploadItemImagesRepository
    {
        public IMenuRepository _menuRepository { get; set; }

        public UploadItemImagesRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetItemMaster(string? sDate,string? currentDate,Common common)
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
                        //string query = $@"SELECT ITEM_CODE, ITEM_NAME, IPIC, SALE_RATE FROM {table} WHERE DLT = 'T'";
                        string query = $@"EXEC STKPROC '144','{sDate}','{currentDate}','{common.Branch}','{common.Period}','',''";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        bool hasPartyCode = false;
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i).Equals("PARTY_CODE", StringComparison.OrdinalIgnoreCase))
                            {
                                hasPartyCode = true;
                                break;
                            }
                        }
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                                GROUP_CODE = Convert.ToInt32(reader["GROUP_cODE"]),
                                ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                                SALE_RATE = reader["SALE_RATE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SALE_RATE"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                //BALANCE = Convert.ToString(reader["BALANCE"]),
                                BALANCE = reader["BALANCE"] != DBNull.Value ? Convert.ToDecimal(reader["BALANCE"]) : 0m,
                                DOC = Convert.ToString(reader["IPIC"]),
                                PARTY_CODE = hasPartyCode && reader["PARTY_CODE"] != DBNull.Value ? Convert.ToString(reader["PARTY_CODE"]) : "",
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

        public MyHttpResponseMessage GetPartyBranches(int partyCode, int actCode)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $@"SELECT BRANCH_NAME, PARTY_CODE, ACT_CODE
                                      FROM TBL_PARTY_BRANCHES
                                      WHERE DLT = 'T'
                                      AND PARTY_CODE = '{partyCode}'
                                      AND ACT_CODE = '{actCode}'
                                      ORDER BY BRANCH_NAME";
                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new
                        {
                            branchName = Convert.ToString(reader["BRANCH_NAME"])
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

        private int GenerateNextId(Common common, SqlCommand command)
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(OP_ID), 0) + 1 FROM {table} WHERE BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "'";
                    command.CommandText = maxIdQuery;
                    object result = command.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        //public MyHttpResponseMessage Save(List<UploadItemImages> UploadItemImagess, Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty, prefix = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //            prefix = menu.PERFIX;
        //        }

        //        var permissions = common.RoleType == "A"
        //        ? "Admin"
        //        : GetPermissionByMenueID(common.RoleID, common.MenuID);

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            if (UploadItemImagess.Count == 0)
        //            {
        //                response.data = "";
        //                response.msg = "Something went wrong! please try again later.";
        //                response.msgType = 2;
        //                return response;
        //            }
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
        //                    foreach (var modelRecord in UploadItemImagess)
        //                    {
        //                        string query = "";
        //                        if (modelRecord.OP_ID == null || modelRecord.OP_ID == 0)
        //                        {
        //                            if(common.RoleType == "A" || permissions.R_ADD)
        //                            {
        //                                query = $"INSERT INTO {table} (OP_ID,BTYPE,DEBIT,CREDIT," +
        //                                        "ACT_CODE,ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
        //                                        "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
        //                                        "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS,MENU_ID,BCODE,PERIOD_ID)" +
        //                                        "VALUES" +
        //                                        "('" + GenerateNextId(common, command) + "', '" + prefix + "', '" + modelRecord.DEBIT + "', '" + modelRecord.CREDIT + "'," +
        //                                        "'" + modelRecord.ACT_CODE + "', '" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "'," +
        //                                        "'" + Ip + "', '" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "'," +
        //                                        "'" + Ip + "', '" + Postal + "', '" + Postal + "', '" + modelRecord.ASTATUS + "'," +
        //                                        "'" + common.MenuID + "','" + common.Branch + "','" + common.Period + "')";
        //                                command.CommandText = query;
        //                                command.ExecuteNonQuery();

        //                                query $@"UPDATE {table} SET IPIC = '{Item.DOC} WHERE ITEM_CODE = '{Item.ITEM_CODE}' '";
        //                            }
        //                            else
        //                            {
        //                                response.data = "";
        //                                response.msg = "You are not allowed to add new records.";
        //                                response.msgType = 2;
        //                                return response;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (common.RoleType == "A" || permissions.R_EDIT)
        //                            {
        //                                query = $"UPDATE {table} SET DEBIT = '" + modelRecord.DEBIT + @"',
        //			 CREDIT = '" + modelRecord.CREDIT + @"',
        //			 ACT_CODE = '" + modelRecord.ACT_CODE + @"',
        //			 EDIT_USER_ID = '" + username + @"',
        //			 EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
        //			 EDIT_COMPUTER_NAME = '" + Computer + @"',
        //			 EDIT_IP_ADDRESS = '" + Ip + @"',
        //			 EDIT_POSTALCODE = '" + Postal + @"',
        //			 ASTATUS = '" + modelRecord.ASTATUS + @"'
        //			 WHERE OP_ID = '" + modelRecord.OP_ID + @"' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "'";
        //                                command.CommandText = query;
        //                                command.ExecuteNonQuery();
        //                            }
        //                            else
        //                            {
        //                                response.data = "";
        //                                response.msg = "You are not allowed to edit records.";
        //                                response.msgType = 2;
        //                                return response;
        //                            }
        //                        }
        //                    }

        //                    transaction.Commit();
        //                    response.msgType = 1;
        //                    response.msg = "Records Updated Successfully";
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
        //        response.msgType = 2;
        //        response.msg = _catchMessage;
        //    }
        //    return response;
        //}

        public MyHttpResponseMessage Save(List<UploadItemImages> UploadItemImagess, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, prefix = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    prefix = menu.PERFIX;
                }


                if (!String.IsNullOrWhiteSpace(table))
                {
                    if (UploadItemImagess.Count == 0)
                    {
                        response.data = "";
                        response.msg = "No items selected to save.";
                        response.msgType = 2;
                        return response;
                    }

                        var Ip = common.IPAddress;
                        var Computer = common.ComputerName;
                        var Postal = common.PostalCode;
                        var username = common.Username;
                        string connectionString = new SQLService().getconnstring();

                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            SqlTransaction transaction = connection.BeginTransaction();
                            SqlCommand command = connection.CreateCommand();
                            command.Transaction = transaction;
                            try
                            {
                                foreach (var modelRecord in UploadItemImagess)
                                {
                                    string query = $@"UPDATE {table} SET 
                                            IPIC = '{modelRecord.DOC}',
                                            EDIT_USER_ID = '{username}',
                                            EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                            EDIT_COMPUTER_NAME = '{Computer}',
                                            EDIT_IP_ADDRESS = '{Ip}',
                                            EDIT_POSTALCODE = '{Postal}'
                                            WHERE ITEM_CODE = '{modelRecord.ITEM_CODE}'";

                                    command.CommandText = query;
                                    command.ExecuteNonQuery();
                                }

                                transaction.Commit();
                                response.msgType = 1;
                                response.msg = "Images Updated Successfully";
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
                    response.msg = "Something went wrong! table name not found.";
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

        public static dynamic GetPermissionByMenueID(int? roleId, int? menuId)
        {
            object json = null;
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT * FROM TBL_ROLE WHERE ROLE_ID = {roleId} AND RMENU_ID = {menuId} AND MODULE_ID = 1";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var jsonDataResult = new
                    {
                        ROLE_ID = reader["ROLE_ID"],
                        ROLE_NAME = reader["ROLE_NAME"],
                        ROLE_TYPE = reader["ROLE_TYPE"],
                        MODULE_ID = reader["MODULE_ID"],
                        DT_CODE = reader["DT_CODE"],
                        R_ADD = Convert.ToBoolean(reader["R_ADD"]),
                        R_EDIT = Convert.ToBoolean(reader["R_EDIT"]),
                        R_DLT = Convert.ToBoolean(reader["R_DLT"]),
                        R_VIEW = Convert.ToBoolean(reader["R_VIEW"]),
                        R_PRINT = Convert.ToBoolean(reader["R_PRINT"]),
                        R_COPY = Convert.ToBoolean(reader["R_COPY"]),
                        R_BCODE = reader["R_BCODE"],
                        RMENU_ID = reader["RMENU_ID"]
                    };
                    json = jsonDataResult;
                }
                reader.Close();
            }
            return json;
        }
    }
}