using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
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
using ZXing.QrCode.Internal;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class HawlaRepository : IHawlaRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }

        public HawlaRepository(IMenuRepository menuRepository, IBranchRepository branchRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
        }

        public MyHttpResponseMessage GetHawlas(int Branch, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table1 = string.Empty;
                string? table2 = string.Empty;
                string? pickMaster = string.Empty;
                string? pickDetail = string.Empty;

                var menu = (Menu)Menu.data;
                table1 = menu.TABLE1;
                table2 = menu.TABLE2;
                pickMaster = menu.PICK_TABLE_MASTER;
                pickDetail = menu.PICK_TABLE_DETAIL;

                if (!String.IsNullOrWhiteSpace(table1))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = $@"EXEC [PROC_PICK_DATA] '{menu.MENU_PAGE}', '{table1}','{table2}','{pickMaster}','{pickDetail}',{Branch},'','',''";

                        //string query = $@"select TJV.TRAN_ID AS TJV_TRANID, EP.V_DATE, EP.VOUCHER_NO, BK.ACT_NAME AS BOOK_TYPE,
                        //                CH.ACT_NAME, EP.DESCR, EP.AMOUNT, TJV.PICK_ID, EP.TRAN_ID, EP.BCODE, EP.PERIOD_ID,'POSE' AS PAGE_TYPE
                        //                from TBL_POS_EXP EP 
                        //                LEFT OUTER JOIN TBL_TJV TJV
                        //                ON TJV.PICK_ID = EP.TRAN_ID AND TJV.RBCODE = EP.BCODE AND TJV.RPERIOD_ID =    EP.PERIOD_ID
                        //                LEFT OUTER JOIN TBL_CHART BK
                        //                ON BK.ACT_CODE = EP.BOOK_TYPE 
                        //                LEFT OUTER JOIN TBL_CHART CH
                        //                ON CH.ACT_CODE = EP.ACT_CODE
                        //                WHERE EP.DLT = 'T' AND CLOSING = 1 
                        //                AND EP.BCODE = {Branch} 
                        //                AND CH.ACT_NATURE = 25
                              
                        //                UNION ALL
                        //                select TJV.TRAN_ID AS TJV_TRANID, EP.V_DATE, EP.VOUCHER_NO, BK.ACT_NAME AS BOOK_TYPE,
                        //                'Point Of Sales' AS ACT_NAME, 'Point Of Sales'  AS DESCR, EP.BANK, TJV.PICK_ID, EP.TRAN_ID, EP.BCODE, EP.PERIOD_ID,
                        //                'POS_BANK' AS PAGE_TYPE
                        //                from TBL_POS_MASTER EP 
                        //                LEFT OUTER JOIN TBL_TJV TJV
                        //                ON TJV.PICK_ID = EP.TRAN_ID AND TJV.RBCODE = EP.BCODE AND TJV.RPERIOD_ID =    EP.PERIOD_ID 
                        //                LEFT OUTER JOIN TBL_CHART BK
                        //                ON BK.ACT_CODE = EP.BACT_CODE 
                        //                LEFT OUTER JOIN TBL_CHART CH
                        //                ON CH.ACT_CODE = EP.ACT_CODE
                        //                WHERE EP.DLT = 'T' AND CLOSING = 1 AND BILL_STATUS = 'P' 
                        //                AND EP.BCODE = {Branch} AND bk.ACT_NATURE = 2


                        //                UNION ALL
                        //                select TJV.TRAN_ID AS TJV_TRANID, EP.V_DATE, EP.VOUCHER_NO, BK.ACT_NAME AS BOOK_TYPE,
                        //                'Point Of Sales' AS ACT_NAME, 'Point Of Sales'  AS DESCR, ROUND(EP.BANK*EP.BCHARGES/100,2)   , TJV.PICK_ID, EP.TRAN_ID, EP.BCODE, EP.PERIOD_ID,
                        //                'POS_BANK' AS PAGE_TYPE
                        //                from TBL_POS_MASTER EP 
                        //                LEFT OUTER JOIN TBL_TJV TJV
                        //                ON TJV.PICK_ID = EP.TRAN_ID AND TJV.RBCODE = EP.BCODE AND TJV.RPERIOD_ID = EP.PERIOD_ID 
                        //                LEFT OUTER JOIN TBL_CHART BK
                        //                ON BK.ACT_CODE = EP.BACT_CODE 
                        //                LEFT OUTER JOIN TBL_CHART CH
                        //                ON CH.ACT_CODE = EP.ACT_CODE
                        //                WHERE EP.DLT = 'T' AND CLOSING = 1 AND BILL_STATUS = 'P' 
                        //                AND EP.BCODE = {Branch} AND bk.ACT_NATURE = 2
                        //                AND EP.BCHARGES <> 0
                        //                ORDER BY EP.V_DATE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        var listWithPickId = new List<object>();
                        var listWithoutPickId = new List<object>();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                VoucherDate = reader["V_DATE"] == DBNull.Value ? "" : Convert.ToDateTime(reader["V_DATE"]).ToString("dd/MM/yyyy"),
                                VoucherNo = Convert.ToString(reader["VOUCHER_NO"]),
                                BookType = Convert.ToString(reader["BOOK_TYPE"]),
                                ActName = Convert.ToString(reader["ACT_NAME"]),
                                Descr = Convert.ToString(reader["DESCR"]),
                                BCODE = Convert.ToString(reader["BCODE"]),
                                PAGE_TYPE = Convert.ToString(reader["PAGE_TYPE"]),
                                PERIOD_ID = Convert.ToString(reader["PERIOD_ID"]),
                                Amount = reader["AMOUNT"] == DBNull.Value ? 0 : Convert.ToInt32(reader["AMOUNT"]),
                                PickId = reader["PICK_ID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["PICK_ID"]),
                                TranId = reader["TRAN_ID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TRAN_ID"]),
                                TJV_TRANID = reader["TJV_TRANID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TJV_TRANID"]),

                            };
                            if (row.PickId.HasValue && row.PickId.Value > 0)
                            {
                                listWithPickId.Add(row);
                            }
                            else
                            {
                                listWithoutPickId.Add(row);
                            }
                          

                        }
                  
                        reader.Close();
                        jsonDataResult.Add(new
                        {
                            WithPickId = listWithPickId,
                            WithoutPickId = listWithoutPickId
                        });


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

        public MyHttpResponseMessage Save(Hawla modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, prefix = string.Empty, query = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    prefix = menu.PERFIX;
                }

                if (!String.IsNullOrWhiteSpace(table))
                {
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
                            if (modelRecord.TJV_TRANID == null || modelRecord.TJV_TRANID == 0)
                            {
                                var code = GenerateNextId(common);
                                var voucherNo = GenerateVoucherNo(common, Convert.ToInt32(code), CommonService.GetDateTime("Pakistan Standard Time"));
                                query = @$"INSERT INTO {table} (
                                            [TRAN_ID], [V_DATE], [VOUCHER_NO], [DDESC], [DEBIT_AC], [CREDIT_AC], [BCODE], [PERIOD_ID], [PAGE_TYPE],
                                            [ADD_USER_ID], [ADD_DATE], [ADD_COMPUTER_NAME], [ADD_IP_ADDRESS], [EDIT_USER_ID],
                                            [EDIT_DATE], [EDIT_COMPUTER_NAME], [EDIT_IP_ADDRESS], [ADD_POSTALCODE],
                                            [EDIT_POSTALCODE], [MENU_ID], [DLT], [PICK_ID], [RBCODE], [RPERIOD_ID]
                                        )
                                        VALUES (
                                            '{Convert.ToInt32(code)}',
                                            '{DateTime.Now:yyyy-MM-dd}',
                                            '{voucherNo}',
                                            '{modelRecord.REMARKS}',
                                            '{modelRecord.DebitAccount}',
                                            '{modelRecord.CreditAccount}',
                                            '{common.Branch}',
                                            '{common.Period}',
                                            '{modelRecord.PAGE_TYPE}',
                                            '{username}',
                                            '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                            '{Computer}',
                                            '{common.IPAddress}',
                                            '{username}',
                                            '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                            '{Computer}',
                                            '{common.IPAddress}',
                                            '{Postal}',
                                            '{Postal}',
                                            '{common.MenuID}',
                                            'T',
                                            '{modelRecord.PickId}',
                                            '{modelRecord.RBCODE}',
                                            '{modelRecord.RPERIOD_ID}');
                                ";

                                
                            }
                            else
                            {
                                query = $@"UPDATE {table} SET
                                            [DEBIT_AC] = {modelRecord.DebitAccount}, 
                                            [CREDIT_AC] = {modelRecord.CreditAccount},
                                            [DDESC] = '{modelRecord.REMARKS}',
                                            [BCODE] = {common.Branch},
                                            [PERIOD_ID] = {common.Period},
                                            [EDIT_USER_ID] = '{username}',
                                            [EDIT_DATE] = '{CommonService.GetDateTime("Pakistan Standard Time")}',
                                            [EDIT_COMPUTER_NAME] = '{Computer}',
                                            [EDIT_IP_ADDRESS] = '{common.IPAddress}',
                                            [EDIT_POSTALCODE] = '{Postal}'
                                            WHERE [TRAN_ID] = '{modelRecord.TJV_TRANID}'

                                ";
                            }

                            command.CommandText = query;
                            command.ExecuteNonQuery();

                            transaction.Commit();
                            response.msgType = 1;
                            response.msg = modelRecord.TJV_TRANID == null || modelRecord.TJV_TRANID == 0 ? "Record Added Successfully" : "Record Updated Successfully";

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
                    string maxIdQuery = "SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM " + table;
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

        public MyHttpResponseMessage GetDataForReport(int amount , int branch, CustomMenuDetail menuDetails, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            POSTransactionReport masterData = new POSTransactionReport();
            CustomPOSTransactionForPrintReport reportData = new CustomPOSTransactionForPrintReport();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                var menu = (Menu)Menu.data;
                string connectionString = new SQLService().getconnstring();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = connection.CreateCommand();
                    command.Transaction = transaction;
                }

                var branchData = DropdownService.BranchDataDropdown();
                Branch FromBranch = branchData.FirstOrDefault(b => b.BCODE == Convert.ToInt32(common.Branch));
                Branch ToBranch = branchData.FirstOrDefault(b => b.BCODE == branch);

                var viewModel = new HawlaViewModel
                {
                    Amount = amount.ToString("N0"),
                    ToBranch = ToBranch.B_NAME,
                    ToBranchAddress = ToBranch.B_ADDRESS,
                    FromBranch = FromBranch.B_NAME,
                    FromBranchAddress = FromBranch.B_ADDRESS,
                    ToBranchPhone = ToBranch.B_TEL,
                    FromBranchPhone = FromBranch.B_TEL
                };

                response.viewModel = viewModel;
                response.data = menuDetails;
                //response.voucherNo = voucherNo;
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

        public MyHttpResponseMessage GetTJVRecord(int code, Common common)
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
                        string query = $"SELECT * FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DEBIT_AC = reader["DEBIT_AC"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DEBIT_AC"]),
                                CREDIT_AC = reader["CREDIT_AC"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CREDIT_AC"]),
                                DDESC = Convert.ToString(reader["DDESC"]),
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
    }
}