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
using System.Xml.Linq;
using ZXing.QrCode.Internal;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class POSDiscountItemWiseRepository : IPOSDiscountItemWiseRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public POSDiscountItemWiseRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
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
                        string query = @$"SELECT CODE, DESCR, FDATE, TDATE, DISC, CASE WHEN DISC_EXP = '1' THEN 'Active' ELSE 'In-Active' END AS DISC_EXP
                                        , A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS
                                        , A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, A.ADD_POSTALCODE 
                                        , A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS
                                          FROM " + table + " A " +
                                       " WHERE A.DLT = 'T'" +
                                       " --AND BR.DLT = 'T' AND BR.ASTATUS = 'Y'" +
                                       " --AND IM.DLT = 'T' AND IM.ASTATUS = 'Y'" +
                                       " --AND IGR.DLT = 'T' AND IGR.ASTATUS = 'Y'\n" +
                                       " ORDER BY A.CODE DESC OPTION(FAST 50)";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = Convert.ToString(reader["CODE"]),
                                DESCR = Convert.ToString(reader["DESCR"]),
                                //B_NAME = Convert.ToString(reader["B_NAME"]),
                                FDATE = Convert.ToString(reader["FDATE"]),
                                TDATE = Convert.ToString(reader["TDATE"]),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_EXP = Convert.ToString(reader["DISC_EXP"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                ADD_DATE = Convert.ToString(reader["ADD_DATE"]),
                                ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = Convert.ToString(reader["EDIT_DATE"]),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();

                        response.data = jsonDataResult;
                        response.msg = "";
                        response.msgType = 1;
                        return response;
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
        public MyHttpResponseMessage detailGrid(Common common)
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
                        string query = @$"select IM.ITEM_CODE,IM.ITEM_NAME,IM.REMARKS,IG.GROUP_NAME,CT.GROUP_NAME AS CAT_NAME,SCT.GROUP_NAME AS SUB_CAT_NAME
                                        FROM TBL_ITEMSMASTER IM
                                        LEFT OUTER JOIN TBL_ITEMSGROUP IG
                                        ON IG.GROUP_CODE = IM.GROUP_CODE
                                        LEFT OUTER JOIN TBL_CATEGORY CT
                                        ON CT.GROUP_CODE = IM.CAT_CODE
                                        LEFT OUTER JOIN TBL_SUB_CATEGORY SCT
                                        ON SCT.GROUP_CODE = IM.SUB_CAT_CODE
                                        WHERE IM.DLT = 'T' AND IM.ASTATUS = 'Y'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = Convert.ToString(reader["ITEM_CODE"]),
                                ItemName = Convert.ToString(reader["ITEM_NAME"]),
                                Remark = Convert.ToString(reader["REMARKS"]),
                                GroupName = Convert.ToString(reader["GROUP_NAME"]),
                                CatName = Convert.ToString(reader["CAT_NAME"]),
                                SubCatName = Convert.ToString(reader["SUB_CAT_NAME"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();

                        response.data = jsonDataResult;
                        response.msg = "";
                        response.msgType = 1;
                        return response;
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

        public string GenerateNextId(Common common, SqlCommand command)
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
                    string query = "SELECT ISNULL(MAX(CODE), 0) + 1 FROM " + table + "";
                    command.CommandText = query;
                    object result = command.ExecuteScalar();
                    int nextId = Convert.ToInt32(result);
                    return Convert.ToString(nextId);
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

        public MyHttpResponseMessage Save(POSDiscountItemWise modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, table2 = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    table2 = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(table2))
                {
                    var Ip = common.IPAddress;
                    var Computer = common.ComputerName;
                    var Postal = common.PostalCode;
                    var userid = common.Username;
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
                            List<int> generatedCodes = new List<int>();
                            char[] separators = { ',' };

                            //var branches = modelRecord.Master.SELECTEDBRANCHES
                            //                .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                            //                .Select(b => b.Trim())
                            //                .ToList();

                            string query = "";
                            bool IsNew = false;
                            int branchCode = 0;
                            if (modelRecord.Master.CODE == null || modelRecord.Master.CODE == 0)
                            {
                                IsNew = true;
                                bool isSuccess = true, isItem = false;
                                string color = string.Empty, size = string.Empty;
                                //foreach (var branch in branches)
                                //{
                                try
                                {
                                    var code = GenerateNextId(common, command);
                                    int nextCode = Convert.ToInt32(code);
                                    generatedCodes.Add(nextCode);
                                    modelRecord.Master.CODE = nextCode;
                                    query = $@"INSERT INTO {table} (
                                                       [CODE], [DESCR], [FDATE], [TDATE], 
                                                       [DISC], [DISC_EXP], 
                                                       [ADD_USER_ID], [ADD_DATE], [ADD_COMPUTER_NAME], 
                                                       [ADD_IP_ADDRESS], [EDIT_USER_ID], [EDIT_DATE], 
                                                       [EDIT_COMPUTER_NAME], [EDIT_IP_ADDRESS], [ADD_POSTALCODE], 
                                                       [EDIT_POSTALCODE], [MENU_ID], [ASTATUS], [DLT])
                                                       VALUES (
                                                           '{code}', '{modelRecord.Master.DESCR}', '{modelRecord.Master.FDATE}', '{modelRecord.Master.TDATE}', 
                                                           '{modelRecord.Master.DISC}', '{modelRecord.Master.DISC_EXP}',
                                                           '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', 
                                                           '{Ip}', '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', 
                                                           '{Computer}', '{Ip}', '{Postal}', 
                                                           '{Postal}', {menuID}, '{modelRecord.Master.ASTATUS}', 'T'
                                                       );";
                                    command.CommandText = query;
                                    command.ExecuteNonQuery();
                                }
                                catch (Exception ex)
                                {
                                    isSuccess = false;
                                    response.msg = $"{ex.Message} !....";
                                }
                                //}

                                //if (isSuccess)
                                //{
                                //    transaction.Commit();
                                //    response.msgType = 1;
                                //    response.msg = "Record Added Successfully";
                                //}
                                //else
                                //{
                                //    transaction.Rollback();
                                //    response.msgType = 2;
                                //}
                            }
                            else
                            {
                                query = $@"UPDATE {table}
                                           SET 
                                            [DESCR] = '{modelRecord.Master.DESCR}',  
                                            [FDATE] = '{modelRecord.Master.FDATE}', 
                                            [TDATE] = '{modelRecord.Master.TDATE}', 
                                            [DISC] = '{modelRecord.Master.DISC}', 
                                            [DISC_EXP] = '{modelRecord.Master.DISC_EXP}',
                                            [EDIT_USER_ID] = '{userid}', 
                                            [EDIT_DATE] = '{CommonService.GetDateTime("Pakistan Standard Time")}', 
                                            [EDIT_COMPUTER_NAME] = '{Computer}', 
                                            [EDIT_IP_ADDRESS] = '{Ip}', 
                                            [EDIT_POSTALCODE] = '{Postal}', 
                                            [ASTATUS] = '{modelRecord.Master.ASTATUS}'
                                           WHERE [CODE] = '{modelRecord.Master.CODE}';";

                                command.CommandText = query;
                                command.ExecuteNonQuery();

                            }

                            if (modelRecord.Detail.Count > 0)
                            {
                                var detailQuery = $"UPDATE {table2} SET DLT = 'F'" +
                                $" WHERE CODE = '{modelRecord.Master.CODE}'";
                                command.CommandText = detailQuery;
                                command.ExecuteNonQuery();
                            }


                            if (modelRecord.Detail.Count > 0 && modelRecord.Detail != null)
                            {
                                foreach(var branch in modelRecord.Branch.ToList())
                                {
                                    foreach (var item in modelRecord.Detail.ToList())
                                    {
                                        if (item.DT_CODE == null || item.DT_CODE == 0)
                                        {
                                            //for (int i = 0; i < branches.Count; i++)
                                            //{
                                            //    if (IsNew)
                                            //    {
                                            //        var branch = branches[i];
                                            //        branchCode = generatedCodes[i];
                                            //    }
                                            //    else
                                            //    {
                                            //        branchCode = Convert.ToInt32(modelRecord.Master.CODE);
                                            //    }
                                            int detailCode = GenerateNextDetailId(common, command);// Har branch ka respective generated code

                                            query = $@"INSERT INTO {table2} (
                                                        [CODE], [DT_CODE], [BCODE], [ITEM_CODE],
                                                        [ADD_USER_ID], [ADD_DATE], [ADD_COMPUTER_NAME], 
                                                        [ADD_IP_ADDRESS], [EDIT_USER_ID], [EDIT_DATE], 
                                                        [EDIT_COMPUTER_NAME], [EDIT_IP_ADDRESS], [ADD_POSTALCODE], 
                                                        [EDIT_POSTALCODE], [MENU_ID], [DLT])
                                                        VALUES (
                                                            '{modelRecord.Master.CODE}', '{detailCode}', '{branch.key}' , '{item.Id}',
                                                            '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', '{Computer}', 
                                                            '{Ip}', '{userid}', '{CommonService.GetDateTime("Pakistan Standard Time")}', 
                                                            '{Computer}', '{Ip}', '{Postal}', 
                                                            '{Postal}', {menuID}, 'T'
                                                        );";

                                            command.CommandText = query;
                                            command.ExecuteNonQuery();
                                            //}
                                        }
                                        else
                                        {
                                            query = $@"UPDATE {table2} 
                                               SET 
                                                   [ITEM_CODE] = '{item.Id}',
                                                   [EDIT_USER_ID] = '{userid}',
                                                   [EDIT_DATE] = '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                                   [EDIT_COMPUTER_NAME] = '{Computer}',
                                                   [EDIT_IP_ADDRESS] = '{Ip}',
                                                   [EDIT_POSTALCODE] = '{Postal}',
                                                   [MENU_ID] = {menuID},
                                                   [DLT] = 'T'
                                               WHERE [CODE] = '{modelRecord.Master.CODE}' AND [DT_CODE] = '{item.DT_CODE}';";

                                            command.CommandText = query;
                                            command.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }

                            transaction.Commit();
                            response.msgType = 1;
                            response.msg = IsNew ? "Record Added Successfully" : "Record Updated Successfully";
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

        public MyHttpResponseMessage GetPOSDiscountItemWiseByCode(int code, Common common)
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
                        string query = @$"SELECT CODE, DESCR, FDATE, TDATE, DISC, DISC_EXP, ASTATUS
                                          FROM {table} WHERE DLT = 'T' AND CODE = '{code}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                CODE = Convert.ToInt32(reader["CODE"]),
                                DESCR = Convert.ToString(reader["DESCR"]),
                                //BCODE = Convert.ToInt32(reader["BCODE"]),
                                FDATE = reader["FDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["FDATE"]).ToString("yyyy-MM-dd"),
                                TDATE = reader["TDATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["TDATE"]).ToString("yyyy-MM-dd"),
                                DISC = Convert.ToString(reader["DISC"]),
                                DISC_EXP = Convert.ToString(reader["DISC_EXP"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"])
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();

                        response.data = jsonDataResult;
                        response.msg = "";
                        response.msgType = 1;
                        return response;
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
        public MyHttpResponseMessage GetPOSDiscountItemWiseDetailByCode(int code, Common common)
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
                        string query = @$"SELECT CODE, DT_CODE, ITEM_CODE, BCODE FROM {table} WHERE DLT = 'T' AND CODE = '{code}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                CODE = Convert.ToInt32(reader["CODE"]),
                                BCODE = Convert.ToInt32(reader["BCODE"]),
                                DTCODE = Convert.ToString(reader["DT_CODE"]),
                                ITEMCODE = Convert.ToString(reader["ITEM_CODE"])
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();

                        response.data = jsonDataResult;
                        response.msg = "";
                        response.msgType = 1;
                        return response;
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

        private int GenerateNextDetailId(Common common, SqlCommand command)
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0)+ 1  FROM {table}";
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
        public MyHttpResponseMessage Delete(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, table2 = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    table2 = menu.TABLE2;
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
                            string query = $"UPDATE {table} SET DLT = 'F' WHERE CODE = '{code}' " +
                                           $"UPDATE {table2} SET DLT = 'F' WHERE CODE = '{code}' ";
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
    }
}