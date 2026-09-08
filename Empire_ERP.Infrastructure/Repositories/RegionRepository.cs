using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class RegionRepository : IRegionRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public RegionRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetRegionsDropDown(int menuid)
        {

            MyHttpResponseMessage response = new MyHttpResponseMessage();

            List<dynamic> Dropdown = new List<dynamic>();
            using (SqlConnection db = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @"SELECT A.CODE,
                                A.DESCR,C.DESCR AS CONTROL_NAME
                                FROM TBL_REGION A
                                LEFT OUTER   JOIN TBL_REGION B
                                ON A.GR_CODE LIKE CONCAT('', B.GR_CODE ,'%') AND B.ASTATUS <> 'Y' 
                                LEFT OUTER   JOIN TBL_REGION C
                                ON C.CODE = A.PARENT_CODE
                                WHERE A.DLT =  'T' AND A.GROUP_TYPE = 'S'
                                AND B.ASTATUS  IS NULL
                                GROUP BY 
                                A.CODE,
                                A.DESCR,A.ASTATUS,A.GR_CODE,B.ASTATUS,C.DESCR";
                using (SqlCommand Commad = new SqlCommand(query, db))
                {
                    db.Open();
                    using (SqlDataReader Reader = Commad.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                int code = Convert.ToInt32(Reader["CODE"]);
                                string name = Convert.ToString(Reader["DESCR"]);
                                string controlName = Convert.ToString(Reader["CONTROL_NAME"]);
                                Dropdown.Add(new { key = code, value = name, name = controlName });
                            }
                        }
                    }
                }
            }

            response.data = Dropdown;
            response.msg = "";
            response.msgType = 1;
            return response;
        }

        public MyHttpResponseMessage GetRegions(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<Region> regions = new List<Region>();
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
                        string query = $"SELECT A.CODE, A.DESCR, CASE WHEN  C.DESCR IS NULL THEN '' ELSE C.DESCR END AS CONRTOL_NAME " +
                            $"FROM {table} A " +
                            $"LEFT OUTER JOIN {table} B ON A.GR_CODE LIKE CONCAT('', B.GR_CODE, '%') AND B.ASTATUS<> 'Y'" +
                            $"LEFT OUTER JOIN {table} C ON C.CODE = A.PARENT_CODE " +
                            $"WHERE A.DLT = 'T' AND A.GROUP_TYPE = 'C' AND B.ASTATUS IS NULL " +
                            $"GROUP BY A.CODE, A.DESCR, A.ASTATUS, A.GR_CODE, B.ASTATUS, C.DESCR";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            Region region = new Region();
                            region.CODE = Convert.ToInt32(reader["CODE"]);
                            region.DESCR = Convert.ToString(reader["DESCR"]);
                            region.ACT_SNAME = Convert.ToString(reader["CONRTOL_NAME"]);
                            regions.Add(region);
                        }
                        reader.Close();
                    }

                    response.data = regions;
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

        public MyHttpResponseMessage GetAccountsForTreeView(Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<Region> regions = new List<Region>();
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
                        string query = "SELECT A.GR_CODE + '-' + A.DESCR AS DESCR, A.CODE, A.PARENT_CODE " +
                                       "FROM " + table + " A WHERE A.DLT = 'T' " +
                                       "ORDER BY A.GR_CODE";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            Region region = new Region();
                            string actName = Convert.ToString(reader["DESCR"]);
                            int actCode = Convert.ToInt32(reader["CODE"]);
                            int actParentCode = Convert.ToInt32(reader["PARENT_CODE"]);

                            region.DESCR = actName;
                            region.CODE = actCode;
                            region.PARENT_CODE = actParentCode;
                            regions.Add(region);
                        }
                        reader.Close();
                    }
                    response.data = regions;
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

        public MyHttpResponseMessage QuickSearch(Core.Entities.Common common)
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
                        string query = @"SELECT A.CODE,A.DESCR, A.GR_CODE, A.DEP_TYPE, " +
                            " CASE WHEN A.GROUP_TYPE = 'C' THEN 'Control' WHEN A.GROUP_TYPE = 'S' THEN 'Subsidiary' END AS ACCOUNT_TYPE, " +
                            " CASE WHEN B.DESCR IS NULL THEN '' ELSE B.DESCR END  AS PARENT_NAME, " +
                            " CASE WHEN A.ASTATUS = 'Y' THEN 'Active' else 'In-Active' end As ACT_STATUS " +
                            " FROM " + table + " A " +
                            " LEFT OUTER JOIN " + table + " B" +
                            " ON B.CODE = A.PARENT_CODE" +
                            " WHERE A.DLT = 'T'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = reader["CODE"],
                                DESCR = reader["DESCR"].ToString(),
                                ACCOUNT_TYPE = reader["ACCOUNT_TYPE"].ToString(),
                                PARENT_NAME = reader["PARENT_NAME"].ToString(),
                                ACT_GR_CODE = reader["GR_CODE"].ToString(),
                                ACT_STATUS = reader["ACT_STATUS"].ToString(),
                                DEP_TYPE = reader["DEP_TYPE"].ToString()
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

        public MyHttpResponseMessage Save(Region modelRecord, Core.Entities.Common common)
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
                            string depTypeDuplicationquery = "";
                            if (modelRecord.CODE == 0)
                            {
                                var Parent = modelRecord.PARENT_CODE;
                                var GrCode = "0" + GenerateGrCode(Convert.ToString(Parent), common);

                                query = "INSERT INTO " + table + " " +
                                        "([CODE],[DESCR],[GROUP_TYPE],[DEP_TYPE]," +
                                        "[PARENT_CODE],[GR_CODE],[ADDRESS],[ADD_USER_ID],[ADD_DATE],[ADD_COMPUTER_NAME]" +
                                        ",[ADD_IP_ADDRESS],[ADD_POSTALCODE]" +
                                        ",[EDIT_USER_ID],[EDIT_DATE],[EDIT_COMPUTER_NAME]" +
                                        ",[EDIT_IP_ADDRESS],[EDIT_POSTALCODE],[MENU_ID]" +
                                        ",[ASTATUS],[DLT])" +
                                        "VALUES" +
                                        "('" + GenerateNextId(common) + "','" + modelRecord.DESCR + "','" + modelRecord.GROUP_TYPE + "','" + modelRecord.DEP_TYPE + "'," +
                                        "'" + modelRecord.PARENT_CODE + "','" + GrCode + "','" + modelRecord.ADDRESS + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                        "'" + Ip + "','" + Postal + "','" + userid + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + Postal + "','" + common.MenuID + "','" + modelRecord.ASTATUS + "','T')";

                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE DESCR = '" + modelRecord.DESCR + "' AND DLT = 'T'";
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();

                                depTypeDuplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE DEP_TYPE = '" + modelRecord.DEP_TYPE + "' AND DLT = 'T'";
                                command.CommandText = depTypeDuplicationquery;
                                int depCount = (int)command.ExecuteScalar();

                                if (count == 1 && depCount == 1)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.msg = "Record Added Successfully";
                                }
                                else if(count != 1)
                                {
                                    transaction.Rollback();
                                    response.msgType = 2;
                                    response.msg = "Name Already Exist !....";
                                }
                                else if (depCount != 1)
                                {
                                    transaction.Rollback();
                                    response.msgType = 2;
                                    response.msg = "Department Type Already Exist !....";
                                }
                            }
                            else
                            {
                                query = "UPDATE " + table + " SET [DESCR] = '" + modelRecord.DESCR + @"',
		                                                [GROUP_TYPE] = '" + modelRecord.GROUP_TYPE + @"',
		                                                [PARENT_CODE] = '" + modelRecord.PARENT_CODE + @"',
		                                                [DEP_TYPE] = '" + modelRecord.DEP_TYPE + @"',
		                                                [ADDRESS] = '" + modelRecord.ADDRESS + @"',
		                                                [EDIT_USER_ID] = '" + userid + @"',
		                                                [EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
		                                                [EDIT_COMPUTER_NAME] = '" + Computer + @"',
		                                                [EDIT_IP_ADDRESS] = '" + Ip + @"',
		                                                [MENU_ID] = '" + common.MenuID + @"',
		                                                [EDIT_POSTALCODE] = '" + Postal + @"',
		                                                [ASTATUS] = '" + modelRecord.ASTATUS + @"',
		                                                [DLT] = 'T'
                                                    WHERE[CODE] = '" + modelRecord.CODE + @"'";

                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE DESCR = '" + modelRecord.DESCR + "' AND DLT = 'T'";
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();

                                depTypeDuplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE DEP_TYPE = '" + modelRecord.DEP_TYPE + "' AND DLT = 'T'";
                                command.CommandText = depTypeDuplicationquery;
                                int depCount = (int)command.ExecuteScalar();

                                if (count == 1 && depCount == 1)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.msg = "Record Updated Successfully";
                                }
                                else if (count != 1)
                                {
                                    transaction.Rollback();
                                    response.msgType = 2;
                                    response.msg = "Name Already Exist !....";
                                }
                                else if (depCount != 1)
                                {
                                    transaction.Rollback();
                                    response.msgType = 2;
                                    response.msg = "Department Type Already Exist !....";
                                }

                                //Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE DESCR = '" + modelRecord.DESCR + "' AND DLT = 'T'";
                                //command.CommandText = Duplicationquery;
                                //int count = (int)command.ExecuteScalar();
                                //if (count == 1)
                                //{
                                //    transaction.Commit();
                                //    response.msgType = 1;
                                //    response.msg = "Record Updated Successfully";
                                //}
                                //else
                                //{
                                //    transaction.Rollback();
                                //    response.msgType = 2;
                                //    response.msg = "Name Already Exist !....";
                                //}
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

        public string GenerateNextId(Core.Entities.Common common)
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
                    string maxIdQuery = "SELECT ISNULL(MAX(CODE), 0) + 1 FROM " + table + "";
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

        public string GenerateGrCode(string ParentId, Core.Entities.Common common)
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

                    string maxIdQuery = "SELECT CASE WHEN (select max(convert(bigint,GR_CODE))+1 "+
                                        "from "+table+" "+
                                        "where DLT = 'T' ) IS NULL THEN '01' WHEN(SELECT COUNT(PARENT_CODE) "+
                                        "FROM "+table+" "+
                                        "where PARENT_CODE = '"+ParentId+"' AND DLT = 'T') = 0 THEN '0' + CONVERT(NVARCHAR(100), (select(max(CONVERT(BIGINT, (GR_CODE) + '01'))) as act_groupCode "+
                                        "from "+table+" " +
                                        "where CODE = '"+ParentId+"' AND DLT = 'T') ) ELSE '0' + CONVERT(NVARCHAR(100), (select(max(CONVERT(BIGINT, (GR_CODE + 1)))) as act_groupCode "+
                                        "from "+table+" "+
                                        "where PARENT_CODE = '"+ParentId+"' AND DLT = 'T') ) END As ACCOUNT_GROUP_CODE";

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

        public MyHttpResponseMessage GetRegionById(int id, Core.Entities.Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                object json = null;
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
                        string query = "SELECT CODE, DESCR, GROUP_TYPE, DEP_TYPE, PARENT_CODE, " +
                            "GR_CODE, ADD_USER_ID, ADD_DATE, ADDRESS, ADD_COMPUTER_NAME, ADD_IP_ADDRESS, " +
                            "EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS, MENU_ID, ADD_POSTALCODE, " +
                            "EDIT_POSTALCODE, ASTATUS, DLT FROM " + table + " WHERE CODE = @Id";

                        SqlCommand command = new SqlCommand(query, connection);
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new
                            {
                                ID = reader["CODE"],
                                DESCR = reader["DESCR"],
                                ACT_TYPE = reader["GROUP_TYPE"],
                                PARENT_CODE = reader["PARENT_CODE"],
                                DEP_TYPE = reader["DEP_TYPE"],
                                ADDRESS = reader["ADDRESS"] == DBNull.Value ? "" : Convert.ToString(reader["ADDRESS"]),
                                ACT_GR_CODE = reader["GR_CODE"],
                                ASTATUS = reader["ASTATUS"],
                                ADD_USER_ID = reader["ADD_USER_ID"],
                                ADD_DATE = reader["ADD_DATE"],
                                ADD_COMPUTER_NAME = reader["ADD_COMPUTER_NAME"],
                                ADD_IP_ADDRESS = reader["ADD_IP_ADDRESS"],
                                EDIT_USER_ID = reader["EDIT_USER_ID"],
                                EDIT_DATE = reader["EDIT_DATE"],
                                EDIT_COMPUTER_NAME = reader["EDIT_COMPUTER_NAME"],
                                EDIT_IP_ADDRESS = reader["EDIT_IP_ADDRESS"],
                                MENU_ID = reader["MENU_ID"],
                                ADD_POSTALCODE = reader["ADD_POSTALCODE"],
                                EDIT_POSTALCODE = reader["EDIT_POSTALCODE"]
                            };
                            json = jsonDataResult;
                        }
                        reader.Close();
                    }

                    response.msg = "";
                    response.msgType = 1;
                    response.data = json;
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

        public MyHttpResponseMessage Delete(int id, Core.Entities.Common common)
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
                            string query = "UPDATE " + table + " SET DLT = 'F' WHERE CODE = '" + id + @"'";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.ExecuteNonQuery();
                            if (query != null)
                            {
                                response.msg = "Record Deleted Successfully";
                                response.msgType = 1;
                            }
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