using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class MPORegistrationRepository : IMPORegistrationRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public MPORegistrationRepository(IMenuRepository menuRepository)
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
                        //string query = "SELECT L.CODE,PT.PARTY_NAME,L.LOT_NO,L.V_DATE,L.ADD_USER_ID,L.ADD_DATE,L.ADD_COMPUTER_NAME,L.ADD_IP_ADDRESS,L.EDIT_USER_ID," +
                        //               "L.EDIT_DATE,L.EDIT_COMPUTER_NAME,L.EDIT_IP_ADDRESS,L.ADD_POSTALCODE,L.EDIT_POSTALCODE," +
                        //               "CASE WHEN L.ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS " +
                        //               "FROM " + table + " L " +
                        //               "LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = L.PARTY_CODE AND PT.ACT_CODE = L.ACT_CODE " +
                        //               "WHERE L.MENU_ID = '" + common.MenuID + "' AND L.DLT = 'T' ORDER BY L.CODE DESC";

                        string query = $@"SELECT L.CODE, L.V_DATE, L.CLIENT_PO, PT.PARTY_NAME, F.GROUP_NAME AS FABRIC, G.GROUP_NAME AS GSM,
                                        CASE WHEN L.ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ASTATUS 
                                        FROM {table} L 
                                        LEFT OUTER JOIN TBL_PARTY_TYPES PT ON PT.PARTY_CODE = L.PARTY_CODE AND PT.ACT_CODE = L.ACT_CODE 
                                        LEFT OUTER JOIN TBL_FABRIC F ON F.GROUP_CODE = L.FABRIC
                                        LEFT OUTER JOIN TBL_GSM G ON G.GROUP_CODE = L.GSM
                                        WHERE L.MENU_ID = '{common.MenuID}' AND L.DLT = 'T' ORDER BY L.CODE DESC";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                CODE = Convert.ToInt32(reader["CODE"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                CLIENT_PO = Convert.ToString(reader["CLIENT_PO"]),
                                PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]),
                                FABRIC = Convert.ToString(reader["FABRIC"]),
                                GSM = Convert.ToString(reader["GSM"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                //ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                //ADD_DATE = reader["ADD_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["ADD_DATE"]).ToString("yyyy-MM-dd"),
                                //ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                //ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                //EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                //EDIT_DATE = reader["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["EDIT_DATE"]).ToString("yyyy-MM-dd"),
                                //EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                //EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                //ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                //EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
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

        public MyHttpResponseMessage Save(MPORegistration modelRecord, Common common)
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
                            if (modelRecord.CODE == 0)
                            {
                                var newCode = GenerateNextId(common);
                                query = "INSERT INTO " + table + " " +
                                            "(CODE,CLIENT_PO,V_DATE,PARTY_CODE,ACT_CODE, FABRIC, GSM, ADD_USER_ID,ADD_DATE," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
                                            "EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                            "ADD_COMPUTER_NAME,MENU_ID,DLT)" +
                                            "VALUES" +
                                            "('" + newCode + "','" + modelRecord.CLIENT_PO + "','" + modelRecord.V_DATE + "','" + modelRecord.PARTY_CODE + "','" + modelRecord.ACT_CODE + "'," +
                                            "'" + modelRecord.FABRIC + "','" + modelRecord.GSM + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Ip + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                            "'" + Ip + "','" + Postal + "','" + Postal + "','" + modelRecord.ASTATUS + "'," +
                                            "'" + Computer + "','" + common.MenuID + "','T')";
                                //SqlCommand command = new SqlCommand(query, connection);
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE CLIENT_PO = '" + modelRecord.CLIENT_PO + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
                                //SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();
                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.data = newCode;
                                    response.msg = "Record Added Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msg = "PO# Already Exist !....";
                                    response.msgType = 2;
                                }
                            }
                            else
                            {
                                query = "UPDATE " + table + " " +
                                    "SET CLIENT_PO = '" + modelRecord.CLIENT_PO + @"',
                                            V_DATE = '" + modelRecord.V_DATE + @"',
                                            PARTY_CODE = '" + modelRecord.PARTY_CODE + @"',
                                            ACT_CODE = '" + modelRecord.ACT_CODE + @"',
                                            FABRIC = '" + modelRecord.FABRIC + @"',
                                            GSM = '" + modelRecord.GSM + @"',
                                            EDIT_USER_ID = '" + userid + @"',
                                            EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                            EDIT_COMPUTER_NAME = '" + Computer + @"',
                                            EDIT_IP_ADDRESS = '" + Ip + @"',
                                            EDIT_POSTALCODE = '" + Postal + @"',
                                            ASTATUS = '" + modelRecord.ASTATUS + @"'
                                            WHERE CODE = '" + modelRecord.CODE + "' AND MENU_ID = '" + common.MenuID + "'";
                                //SqlCommand command = new SqlCommand(query, connection);
                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                
                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE CLIENT_PO = '" + modelRecord.CLIENT_PO + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
                                //SqlCommand CMD = new SqlCommand(Duplicationquery, connection);
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
                                    response.msg = "PO# Already Exist !....";
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
                MPORegistration mpoRegistration = new MPORegistration();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE CODE = {record.TRAN_ID} AND DLT = 'T'";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        mpoRegistration = new MPORegistration
                        {
                            CODE = 0,
                            V_DATE = record.V_DATE,
                            CLIENT_PO = record.CLIENT_PO,
                            PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                            ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                            FABRIC = Convert.ToInt32(reader["FABRIC"]),
                            GSM = Convert.ToInt32(reader["GSM"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                        };
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(mpoRegistration, common);
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
                    string maxIdQuery = "SELECT ISNULL(MAX(CODE), 0) + 1 FROM " + table;
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

        public MyHttpResponseMessage GetMPORegistrationById(int id, Common common)
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
                        string query = "SELECT CODE, CLIENT_PO, V_DATE, PARTY_CODE, ACT_CODE, ASTATUS, FABRIC, GSM " +
                                       "FROM " + table + " " +
                                       "WHERE MENU_ID = '" + common.MenuID + "' AND DLT = 'T' AND CODE = '" + id + "'";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var setupSubType = new
                            {
                                CODE = Convert.ToInt32(reader["CODE"]),
                                CLIENT_PO = Convert.ToString(reader["CLIENT_PO"]),
                                V_DATE = reader["V_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["V_DATE"]).ToString("yyyy-MM-dd"),
                                PARTY_CODE = Convert.ToInt32(reader["PARTY_CODE"]),
                                ACT_CODE = Convert.ToInt32(reader["ACT_CODE"]),
                                FABRIC = Convert.ToInt32(reader["FABRIC"]),
                                GSM = Convert.ToInt32(reader["GSM"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"])
                            };

                            response.msg = "";
                            response.msgType = 1;
                            response.data = setupSubType;
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

                if (!String.IsNullOrWhiteSpace(table))
                {
                    if (id == 0)
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE CODE = '" + id + "'";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            response.msg = "Record Deleted Successfully";
                            response.msgType = 1;
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