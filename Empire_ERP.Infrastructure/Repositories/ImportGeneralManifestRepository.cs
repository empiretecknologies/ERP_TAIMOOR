using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode.Internal;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class ImportGeneralManifestRepository : IImportGeneralManifestRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }
        public IPartyRepository _partyRepository { get; set; }
        public ImportGeneralManifestRepository(IMenuRepository menuRepository, IBranchRepository branchRepository, IPartyRepository partyRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
            _partyRepository = partyRepository;
        }

        public MyHttpResponseMessage QuickSearch(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT A.TRAN_ID, A.V_DATE, A.VOUCHER_NO, " +
                                       "S.PARTY_NAME AS SELLER_CODE, A.SACT_CODE, B.PARTY_NAME AS BUYER_CODE, A.BACT_CODE, " +
                                       "A.REF, A.REMARKS, CASE WHEN A.ARIVAL_STATUS = 'G' THEN 'Godown' ELSE 'Port' END AS ARIVAL_STATUS," +
                                       "A.ADD_USER_ID, A.ADD_DATE, A.ADD_COMPUTER_NAME, A.ADD_IP_ADDRESS, " +
                                       "A.EDIT_USER_ID, A.EDIT_DATE, A.EDIT_COMPUTER_NAME, A.EDIT_IP_ADDRESS, " +
                                       "A.ADD_POSTALCODE, A.EDIT_POSTALCODE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, " +
                                       "IM.ITEM_NAME AS ITEM_CODE, D.QTY, U.GROUP_NAME AS UNIT " +
                                       $"FROM {table} A " +
                                       "LEFT OUTER JOIN TBL_PARTY_TYPES S ON A.SELLER_CODE = S.PARTY_CODE AND S.ACT_CODE = A.SACT_CODE " +
                                       "LEFT OUTER JOIN TBL_PARTY_TYPES B ON A.BUYER_CODE = B.PARTY_CODE AND B.ACT_CODE = A.BACT_CODE " +
                                       "LEFT OUTER JOIN TBL_IGM_DETAIL D ON A.TRAN_ID = D.TRAN_ID " +
                                       "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON D.ITEM_CODE = IM.ITEM_CODE " +
                                       "LEFT OUTER JOIN TBL_UNIT U ON D.UNIT = U.GROUP_CODE " +
                                       $"WHERE A.DLT = 'T' AND A.BCODE = '{common.Branch}' AND A.PERIOD_ID = '{common.Period}' " +
                                       "ORDER BY A.TRAN_ID DESC, D.DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                ARIVAL_STATUS = Convert.ToString(reader["ARIVAL_STATUS"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                SELLER_CODE = Convert.ToString(reader["SELLER_CODE"]),
                                SACT_CODE = Convert.ToString(reader["SACT_CODE"]),
                                BUYER_CODE = Convert.ToString(reader["BUYER_CODE"]),
                                BACT_CODE = Convert.ToString(reader["BACT_CODE"]),
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
                                ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(TRAN_ID), 0) + 1 FROM {table} WHERE BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
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
                    string maxIdQuery = $"SELECT ISNULL(MAX(DT_CODE), 0) + 1 FROM {table}";
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

        public MyHttpResponseMessage Save(CustomImportGeneralManifest modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, detailTable = string.Empty;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    detailTable = menu.TABLE2;
                }

                List<CustomPartyType> partiesData = DropdownService.CustomPartyTypeDropdownWithAccountCode(common.RoleID, common.RoleType);

                if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable) && partiesData != null && partiesData.Count > 0)
                {
                    var ip = common.IPAddress;
                    var computer = common.ComputerName;
                    var postal = common.PostalCode;
                    var username = common.Username;
                    var branch = common.Branch;
                    var period = common.Period;
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
                            string query = "", detailQuery = "", voucherNo = string.Empty;
                            bool IsMasterAdded = true, IsNew = false;
                            int code = 0;
                            var sellerInformation = partiesData.ToList().Where(p => p.customizedKey == modelRecord.Master.SELLER_CODE).FirstOrDefault();
                            var buyerInformation = partiesData.ToList().Where(p => p.customizedKey == modelRecord.Master.BUYER_CODE).FirstOrDefault();

                            if (sellerInformation != null)
                            {
                                modelRecord.Master.SELLER_CODE = Convert.ToString(sellerInformation.key);
                                modelRecord.Master.SACT_CODE = sellerInformation.accountCode;
                            }

                            if (buyerInformation != null)
                            {
                                modelRecord.Master.BUYER_CODE = Convert.ToString(buyerInformation.key);
                                modelRecord.Master.BACT_CODE = buyerInformation.accountCode;
                            }

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

                                query = $"INSERT INTO {table}" +
                                        "(TRAN_ID, V_DATE, VOUCHER_NO, SELLER_CODE, " +
                                        "SACT_CODE, BUYER_CODE, BACT_CODE, REF, " +
                                        "REMARKS, ARIVAL_STATUS, " +
                                        "BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                        "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                        "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                        "ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID, " +
                                        "DLT)" +
                                        "VALUES" +
                                        "('" + code + "','" + modelRecord.Master.V_DATE + "','" + voucherNo + "','" + modelRecord.Master.SELLER_CODE + "'," +
                                        "'" + modelRecord.Master.SACT_CODE + "','" + modelRecord.Master.BUYER_CODE + "','" + modelRecord.Master.BACT_CODE + "','" + modelRecord.Master.REF + "'," +
                                        "'" + modelRecord.Master.REMARKS + "','" + modelRecord.Master.ARIVAL_STATUS + "'," +
                                        "'" + branch + "','" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                        "'" + computer + "','" + ip + "','" + username + "'," +
                                        "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + computer + "','" + ip + "'," +
                                        "'" + postal + "','" + postal + "','" + modelRecord.Master.ASTATUS + "','" + menuID + "','T')";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                query = $"UPDATE {table} SET " +
                                        $"V_DATE = '{modelRecord.Master.V_DATE}', " +
                                        $"SELLER_CODE = '{modelRecord.Master.SELLER_CODE}', " +
                                        $"SACT_CODE = '{modelRecord.Master.SACT_CODE}', " +
                                        $"BUYER_CODE = '{modelRecord.Master.BUYER_CODE}', " +
                                        $"BACT_CODE = '{modelRecord.Master.BACT_CODE}', " +
                                        $"REF = '{modelRecord.Master.REF}', " +
                                        $"REMARKS = '{modelRecord.Master.REMARKS}', " +
                                        $"ARIVAL_STATUS = '{modelRecord.Master.ARIVAL_STATUS}', " +
                                        $"EDIT_USER_ID = '{username}', " +
                                        $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                        $"EDIT_COMPUTER_NAME = '{computer}', " +
                                        $"EDIT_IP_ADDRESS = '{ip}', " +
                                        $"EDIT_POSTALCODE = '{postal}', " +
                                        $"ASTATUS = '{modelRecord.Master.ASTATUS}' " +
                                        $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                            }

                            var isDetailAdded = true;

                            if (modelRecord.Detail.Count > 0)
                            {
                                detailQuery = $"UPDATE {detailTable} SET DLT = 'F'" +
                                $" WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                                command.CommandText = detailQuery;
                                command.ExecuteNonQuery();
                            }
                            foreach (var item in modelRecord.Detail.ToList())
                            {
                                try
                                {
                                    if (item.DT_CODE == null || item.DT_CODE == 0)
                                    {
                                        int detailCode = GenerateNextDetailId(common, command);
                                        if (detailCode > 0)
                                        {
                                            detailQuery = $"INSERT INTO {detailTable}" +
                                                           "(TRAN_ID, DT_CODE, ITEM_CODE, QTY, " +
                                                           "UNIT, BCODE, PERIOD_ID, ADD_USER_ID, ADD_DATE, " +
                                                           "ADD_COMPUTER_NAME, ADD_IP_ADDRESS, EDIT_USER_ID, " +
                                                           "EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, " +
                                                           "ADD_POSTALCODE, EDIT_POSTALCODE, MENU_ID, DLT)" +
                                                           "VALUES" +
                                                           "('" + modelRecord.Master.TRAN_ID + "','" + detailCode + "','" + item.ITEM_CODE + "','" + item.QTY + "'," +
                                                           "'" + item.UNIT + "','" + branch + "'," +
                                                           "'" + period + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                                           "'" + computer + "','" + ip + "','" + username + "'," +
                                                           "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + computer + "','" + ip + "'," +
                                                           "'" + postal + "','" + postal + "','" + menuID + "','T')";
                                            command.CommandText = detailQuery;
                                            command.ExecuteNonQuery();
                                        }
                                        else
                                        {
                                            isDetailAdded = false;
                                        }
                                    }
                                    else
                                    {
                                        detailQuery = $"UPDATE {detailTable} SET " +
                                                      $"ITEM_CODE = '{item.ITEM_CODE}', " +
                                                      $"QTY = '{item.QTY}', " +
                                                      $"UNIT = '{item.UNIT}', " +
                                                      $"EDIT_USER_ID = '{username}', " +
                                                      $"EDIT_DATE = '{CommonService.GetDateTime("Pakistan Standard Time")}', " +
                                                      $"EDIT_COMPUTER_NAME = '{computer}', " +
                                                      $"EDIT_IP_ADDRESS = '{ip}', " +
                                                      $"EDIT_POSTALCODE = '{postal}', " +
                                                      $"DLT = 'T' " +
                                                      $"WHERE TRAN_ID = '{modelRecord.Master.TRAN_ID}' AND DT_CODE = '{item.DT_CODE}' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                                        command.CommandText = detailQuery;
                                        command.ExecuteNonQuery();
                                    }
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

        public MyHttpResponseMessage GetImportGeneralManifestByCode(int code, Common common)
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
                        string query = "SELECT TRAN_ID, V_DATE, VOUCHER_NO," +
                                       "SELLER_CODE, SACT_CODE, BUYER_CODE, BACT_CODE, REF, REMARKS, ASTATUS, ARIVAL_STATUS " +
                                       $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                ARIVAL_STATUS = Convert.ToString(reader["ARIVAL_STATUS"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                VOUCHER_NO = Convert.ToString(reader["VOUCHER_NO"]),
                                SELLER_CODE = $"{Convert.ToString(reader["SELLER_CODE"])}{Convert.ToString(reader["SACT_CODE"])}",
                                BUYER_CODE = $"{Convert.ToString(reader["BUYER_CODE"])}{Convert.ToString(reader["BACT_CODE"])}",
                                REF = Convert.ToString(reader["REF"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
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

        public MyHttpResponseMessage GetImportGeneralManifestDetailByCode(int code, Common common)
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
                        string query = "SELECT DT_CODE, ITEM_CODE, QTY," +
                                       "UNIT " +
                                       $"FROM {table} WHERE DLT = 'T' AND TRAN_ID = '{code}' AND BCODE = '{common.Branch}' " +
                                       $"AND PERIOD_ID = '{common.Period}' ORDER BY DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                DT_CODE = Convert.ToString(reader["DT_CODE"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToInt32(reader["UNIT"]),
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
                        if (!IsSodaDeliveryAvailable(code, true, common))
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
                        else
                        {
                            response.data = "";
                            response.msg = "Delivery is available against this soda. please delete the delivery first.";
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

        public MyHttpResponseMessage DeleteImportGeneralManifestDetailByCode(int code, Common common)
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
                        if (!IsSodaDeliveryAvailable(code, false, common))
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
                        else
                        {
                            response.data = "";
                            response.msg = "Delivery is available against this soda. please delete the delivery first.";
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

        public bool IsSodaDeliveryAvailable(int code, bool IsMaster, Common common)
        {
            bool IsAvailable = false;
            try
            {
                var branch = common.Branch;
                var period = common.Period;
                if (IsMaster)
                {
                    string connectionString = new SQLService().getconnstring();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = $"SELECT 1 FROM TBL_DF_DETAIL " +
                                       $"WHERE PICK_ID IN (SELECT DT_CODE FROM TBL_SBF_DETAIL WHERE TRAN_ID = '{code}' AND DLT = 'T') " +
                                       $"AND DLT = 'T' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            IsAvailable = true;
                        }
                    }
                }
                else
                {
                    string connectionString = new SQLService().getconnstring();
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = $"SELECT 1 FROM TBL_DF_DETAIL WHERE PICK_ID = '{code}' AND DLT = 'T' AND BCODE = '{branch}' AND PERIOD_ID = '{period}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        DataSet ds = new DataSet();
                        SqlDataAdapter da = new SqlDataAdapter(command);
                        da.Fill(ds);
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            IsAvailable = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsAvailable = true;
            }
            return IsAvailable;
        }

        private List<object> GetSodaBookDetails(string tranID, MyHttpResponseMessage menuData)
        {
            List<object> jsonDataResult = new List<object>();
            try
            {
                string? detailTable = string.Empty;
                if (menuData.data != null)
                {
                    var menu = (Menu)menuData.data;
                    detailTable = menu.TABLE2;
                }

                if (!String.IsNullOrWhiteSpace(detailTable))
                {
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT A.TRAN_ID, IM.ITEM_NAME AS ITEM_CODE, A.QTY," +
                                       "U.GROUP_NAME AS UNIT, QTY2, BAL_QTY, RATE, RTRIM(LTRIM(RT_TYPE)) + ' kg' as RT_TYPE, AMT, DT_DESC " +
                                       $"FROM {detailTable} A " +
                                       "LEFT OUTER JOIN TBL_ITEMSMASTER IM ON A.ITEM_CODE = IM.ITEM_CODE " +
                                       "LEFT OUTER JOIN TBL_UNIT U ON A.UNIT = U.GROUP_CODE " +
                                       $"WHERE A.DLT = 'T' AND A.TRAN_ID = '{tranID}' ORDER BY A.DT_CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                TRAN_ID = Convert.ToString(reader["TRAN_ID"]),
                                ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                                QTY = Convert.ToString(reader["QTY"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                QTY2 = Convert.ToString(reader["QTY2"]),
                                BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
                                RATE = Convert.ToString(reader["RATE"]),
                                RT_TYPE = Convert.ToString(reader["RT_TYPE"]),
                                AMT = Convert.ToString(reader["AMT"]),
                                DT_DESC = Convert.ToString(reader["DT_DESC"]),
                            };
                            jsonDataResult.Add(row);
                        }
                        reader.Close();
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
            return jsonDataResult;
        }
    }
}