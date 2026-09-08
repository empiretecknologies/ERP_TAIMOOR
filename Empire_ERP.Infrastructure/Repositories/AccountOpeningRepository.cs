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
    public class AccountOpeningRepository : IAccountOpeningRepository
    {
        public IMenuRepository _menuRepository { get; set; }

        public AccountOpeningRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetAccountOpenings(Common common)
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
                        //string query = " SELECT CASE WHEN OP.OP_ID IS NULL THEN 0 ELSE OP.OP_ID END OP_ID ,A.ACT_CODE,A.ACT_GR_CODE, AC.ACT_NAME AS CONTROL_NAME,A.ACT_NAME," +
                        //			   " CASE WHEN OP.DEBIT IS NULL THEN 0.00 ELSE OP.DEBIT END AS DEBIT," +
                        //                                 " CASE WHEN OP.CREDIT IS NULL THEN 0.00 ELSE OP.CREDIT END AS CREDIT, CASE WHEN OP.ASTATUS IS NULL THEN 'Y' ELSE OP.ASTATUS END ASTATUS, A.ACT_TYPE" +
                        //			   " FROM TBL_CHART A" +
                        //                                 " LEFT OUTER JOIN "+table+" OP" +
                        //			   " ON OP.ACT_CODE = A.ACT_CODE" +
                        //			   " LEFT OUTER JOIN TBL_CHART AC" +
                        //			   " ON A.ACT_PARENT_CODE = AC.ACT_CODE" +
                        //			   " WHERE A.ACT_TYPE = 'S' AND A.DLT = 'T' AND A.ASTATUS = 'Y'" +
                        //			   " ORDER BY A.ACT_GR_CODE";
                        string query = @"SELECT CASE WHEN OP.OP_ID IS NULL THEN 0 ELSE OP.OP_ID END OP_ID ,A.ACT_CODE,A.ACT_GR_CODE, 
                                        AC.ACT_NAME AS CONTROL_NAME,A.ACT_NAME, CASE WHEN OP.DEBIT IS NULL THEN 0.00 ELSE OP.DEBIT END AS DEBIT,
                                        CASE WHEN OP.CREDIT IS NULL THEN 0.00 ELSE OP.CREDIT END AS CREDIT, 
                                        CASE WHEN OP.ASTATUS IS NULL THEN 'Y' ELSE OP.ASTATUS END ASTATUS,A.ACT_TYPE
                                        FROM TBL_CHART A 
                                        LEFT OUTER JOIN TBL_ACT_OP OP
                                        ON OP.ACT_CODE = A.ACT_CODE AND  BCODE = '" + common.Branch + @"' AND PERIOD_ID = '" + common.Period + @"'
                                        LEFT OUTER JOIN TBL_CHART AC 
                                        ON A.ACT_PARENT_CODE = AC.ACT_CODE 
                                        WHERE A.ACT_TYPE = 'S' AND A.DLT = 'T' AND A.ASTATUS = 'Y' 
                                        AND A.ACT_NATURE NOT IN (3,4,9,10,13)
                                        UNION ALL
                                        SELECT CASE WHEN MAX(OP.OP_ID) IS NULL THEN 0 ELSE MAX(OP.OP_ID) END OP_ID ,AC.ACT_CODE,AC.ACT_GR_CODE, 
                                        AC1.ACT_NAME AS CONTROL_NAME,AC.ACT_NAME, 
                                        CASE
                                        WHEN ISNULL((SELECT SUM(TAMT) FROM TBL_PARTY_TYPES_OP WHERE DC_TYPE = 'D'
                                        AND  BCODE = '" + common.Branch + @"' AND PERIOD_ID = '" + common.Period + @"' AND ACT_CODE = OP.ACT_CODE
                                        GROUP BY DC_TYPE),0)  < 0
                                        THEN
                                        0.00
                                        Else
                                        ISNULL((SELECT SUM(TAMT) FROM TBL_PARTY_TYPES_OP WHERE DC_TYPE = 'D'
                                        AND  BCODE = '" + common.Branch + @"' AND PERIOD_ID = '" + common.Period + @"' AND ACT_CODE = OP.ACT_CODE
                                        GROUP BY DC_TYPE),0)
                                        End
                                        AS DEBIT ,
                                        CASE
                                        WHEN ISNULL((SELECT SUM(TAMT) FROM TBL_PARTY_TYPES_OP WHERE DC_TYPE = 'C'
                                        AND  BCODE = '" + common.Branch + @"' AND PERIOD_ID = '" + common.Period + @"' AND ACT_CODE = OP.ACT_CODE
                                        GROUP BY DC_TYPE),0)  < 0
                                        THEN
                                        0.00
                                        Else
                                        ISNULL((SELECT SUM(TAMT) FROM TBL_PARTY_TYPES_OP WHERE DC_TYPE = 'C'
                                        AND  BCODE = '" + common.Branch + @"' AND PERIOD_ID = '" + common.Period + @"' AND ACT_CODE = OP.ACT_CODE
                                        GROUP BY DC_TYPE),0)
                                        END AS CREDIT,
                                        CASE WHEN OP.ASTATUS IS NULL THEN 'Y' ELSE OP.ASTATUS END ASTATUS, 'C' AS ACT_TYPE
                                        FROM TBL_CHART AC
                                        LEFT OUTER JOIN TBL_PARTY_TYPES_OP OP 
                                        ON OP.ACT_CODE = AC.ACT_CODE AND OP.BCODE = '" + common.Branch + @"' AND OP.PERIOD_ID = '" + common.Period + @"'
                                        LEFT OUTER JOIN TBL_CHART AC1 
                                        ON AC.ACT_PARENT_CODE = AC1.ACT_CODE 
                                        WHERE AC.DLT = 'T' AND AC.ASTATUS = 'Y' AND AC.ACT_TYPE= 'S'
                                        AND AC.ACT_NATURE  IN (3,4,9,10,13)
                                        GROUP BY 
                                        AC.ACT_CODE,AC.ACT_GR_CODE,OP.ASTATUS ,AC.ACT_NAME,OP.ACT_CODE,AC1.ACT_NAME
                                        ORDER BY A.ACT_GR_CODE";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                OP_ID = Convert.ToString(reader["OP_ID"]),
                                ACT_CODE = Convert.ToString(reader["ACT_CODE"]),
                                ACT_GR_CODE = Convert.ToString(reader["ACT_GR_CODE"]),
                                CONTROL_NAME = Convert.ToString(reader["CONTROL_NAME"]),
                                ACT_NAME = Convert.ToString(reader["ACT_NAME"]),
                                DEBIT = Convert.ToString(reader["DEBIT"]),
                                CREDIT = Convert.ToString(reader["CREDIT"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                ACT_TYPE = Convert.ToString(reader["ACT_TYPE"]),
                                //ACT_TYPE = Convert.ToString(reader["ACT_NAME"]) == "GENERATOR" ? "C" : Convert.ToString(reader["ACT_TYPE"]),
                                //ADD_DATE = Convert.ToString(reader["ADD_DATE"]),
                                //ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                //ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                //EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                //EDIT_DATE = Convert.ToString(reader["EDIT_DATE"]),
                                //EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                //EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                //ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                //EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                //ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
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

        public MyHttpResponseMessage Save(List<AccountOpening> accountOpenings, Common common)
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

                var permissions = common.RoleType == "A"
                ? "Admin"
                : GetPermissionByMenueID(common.RoleID, common.MenuID);

                if (!String.IsNullOrWhiteSpace(table))
                {
                    if (accountOpenings.Count == 0)
                    {
                        response.data = "";
                        response.msg = "Something went wrong! please try again later.";
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
                            foreach (var modelRecord in accountOpenings)
                            {
                                string query = "";
                                if (modelRecord.OP_ID == null || modelRecord.OP_ID == 0)
                                {
                                    if(common.RoleType == "A" || permissions.R_ADD)
                                    {
                                        query = $"INSERT INTO {table} (OP_ID,BTYPE,DEBIT,CREDIT," +
                                                "ACT_CODE,ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
                                                "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                                "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS,MENU_ID,BCODE,PERIOD_ID)" +
                                                "VALUES" +
                                                "('" + GenerateNextId(common, command) + "', '" + prefix + "', '" + modelRecord.DEBIT + "', '" + modelRecord.CREDIT + "'," +
                                                "'" + modelRecord.ACT_CODE + "', '" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "'," +
                                                "'" + Ip + "', '" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "'," +
                                                "'" + Ip + "', '" + Postal + "', '" + Postal + "', '" + modelRecord.ASTATUS + "'," +
                                                "'" + common.MenuID + "','" + common.Branch + "','" + common.Period + "')";
                                        command.CommandText = query;
                                        command.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        response.data = "";
                                        response.msg = "You are not allowed to add new records.";
                                        response.msgType = 2;
                                        return response;
                                    }
                                }
                                else
                                {
                                    if (common.RoleType == "A" || permissions.R_EDIT)
                                    {
                                        query = $"UPDATE {table} SET DEBIT = '" + modelRecord.DEBIT + @"',
											 CREDIT = '" + modelRecord.CREDIT + @"',
											 ACT_CODE = '" + modelRecord.ACT_CODE + @"',
											 EDIT_USER_ID = '" + username + @"',
											 EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
											 EDIT_COMPUTER_NAME = '" + Computer + @"',
											 EDIT_IP_ADDRESS = '" + Ip + @"',
											 EDIT_POSTALCODE = '" + Postal + @"',
											 ASTATUS = '" + modelRecord.ASTATUS + @"'
											 WHERE OP_ID = '" + modelRecord.OP_ID + @"' AND BCODE = '" + common.Branch + "' AND PERIOD_ID = '" + common.Period + "'";
                                        command.CommandText = query;
                                        command.ExecuteNonQuery();
                                    }
                                    else
                                    {
                                        response.data = "";
                                        response.msg = "You are not allowed to edit records.";
                                        response.msgType = 2;
                                        return response;
                                    }
                                }
                            }

                            transaction.Commit();
                            response.msgType = 1;
                            response.msg = "Records Updated Successfully";
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