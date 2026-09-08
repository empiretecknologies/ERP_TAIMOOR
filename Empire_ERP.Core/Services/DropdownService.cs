using Empire_ERP.Core.Entities;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;
using Microsoft.Data.SqlClient;
using SkiaSharp;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Empire_ERP.Core.Services
{
    public static class DropdownService
    {
        #region Dropdowns
        public static List<KeyValuePair<int, string>> ChartParentDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.ACT_CODE, A.ACT_NAME FROM TBL_CHART A LEFT OUTER  JOIN TBL_CHART B ON A.ACT_GR_CODE LIKE CONCAT('', B.ACT_GR_CODE ,'%') AND B.ASTATUS <> 'Y' WHERE A.DLT =  'T' AND A.ACT_TYPE = 'C' AND B.ASTATUS  IS NULL GROUP BY A.ACT_CODE, A.ACT_NAME,A.ASTATUS,A.ACT_GR_CODE,B.ASTATUS";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> AccountGroupDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.CODE,A.DESCR FROM TBL_ACT_GROUP A" +
                    " LEFT OUTER   JOIN TBL_ACT_GROUP B" +
                    " ON A.GR_CODE LIKE CONCAT('', B.GR_CODE ,'%') AND B.ASTATUS <> 'Y' " +
                    " WHERE A.DLT =  'T' AND A.GROUP_TYPE = 'S' " +
                    " AND B.ASTATUS  IS NULL" +
                    " GROUP BY A.CODE," +
                    " A.DESCR,A.ASTATUS,A.GR_CODE,B.ASTATUS";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> AccountNatureDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE, GROUP_NAME FROM TBL_ACT_NATURE WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> ChequeFormatDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_CHQ_FORAMT WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> CurrencyDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE, DESCR FROM TBL_CURRENCY WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> PartyTypeDropdown(int? roleId, string? branchId, int? showSelected)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE NOT IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = "SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> SalesmanDropdown(int? roleId, string? branchId, int? showSelected)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 9 AND DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 9 AND DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE NOT IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = "SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 9 AND DLT = 'T' AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> DeliverymanDropdown(int? roleId, string? branchId, int? showSelected)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 13 AND DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6  AND ROLE_ID = {roleId}) AND ACT_CODE IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 13 AND DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE NOT IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = "SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 13 AND DLT = 'T' AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<CustomPartyType> CustomPartyTypeDropdownWithAccountCode(int? roleId, string? roleType)
        {
            List<CustomPartyType> dropdown = new List<CustomPartyType>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {


                string query = $@"DECLARE @ROLE_TYPE NVARCHAR(10);
                                DECLARE @ROLE_ID INT;

                                SET @ROLE_TYPE = '{roleType}';  
                                SET @ROLE_ID = {roleId};  
                                SELECT 
                                    M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM
                                FROM TBL_ROLE R 
                                LEFT OUTER JOIN TBL_PARTY_TYPES M 
                                    ON (
                                        (R.SHOW_SELECTED = 1 AND M.PARTY_CODE = R.RMENU_ID AND M.ACT_CODE = R.ACT_CODE)
                                        OR 
                                        (R.SHOW_SELECTED = 0 AND 
                                        M.PARTY_CODE NOT IN 
                                            (SELECT DISTINCT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = @ROLE_ID AND MODULE_ID = 6) 
                                        AND 
                                        M.ACT_CODE NOT IN 
                                            (SELECT DISTINCT ACT_CODE FROM TBL_ROLE WHERE ROLE_ID = @ROLE_ID AND MODULE_ID = 6)
                                        )
                                    ) 
                                WHERE R.ROLE_TYPE = @ROLE_TYPE
                                    AND R.MODULE_ID = 6 
                                    AND M.DLT = 'T' 
                                    AND M.ASTATUS = 'Y'
                                    AND R.ROLE_ID = @ROLE_ID
                                GROUP BY M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM


                                    UNION ALL

                                    SELECT    M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM
                                    FROM TBL_PARTY_TYPES M WHERE 

                                @ROLE_TYPE

                                    = 'A'  


                                        AND M.DLT = 'T' 
                                        AND M.ASTATUS = 'Y'
                                UNION ALL
                                        SELECT M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM
                                  FROM TBL_PARTY_TYPES M WHERE 

                                @ROLE_TYPE

                                    = 'U'  


                                        AND M.DLT = 'T' 
                                        AND M.ASTATUS = 'Y' AND 
                                        (SELECT 
                                    COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_PARTY_TYPES M ON R.RMENU_ID = M.PARTY_CODE  AND M.ACT_CODE = R.ACT_CODE
                                    WHERE R.ROLE_TYPE = 
                                @ROLE_TYPE

                                        AND R.MODULE_ID = 6 

                                        AND M.DLT = 'T' 
                                        AND M.ASTATUS = 'Y'
                                        AND R.ROLE_ID =

                                    @ROLE_ID


                                        ) < 1";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["PARTY_CODE"]);
                                string? name = Convert.ToString(reader["PARTY_NAME"]);
                                int accountCode = Convert.ToInt32(reader["ACT_CODE"]);
                                int paymentTerms = Convert.ToInt32(reader["PAYMENT_TERMS"]);
                                int commission = reader["COMM"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMM"]);
                                string? customizedKey = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}";
                                dropdown.Add(new CustomPartyType { key = code, customizedKey = customizedKey, value = name, accountCode = accountCode, paymentTerms = paymentTerms, commission = commission });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<CustomPartyType> GetSalesmans()
        {
            List<CustomPartyType> dropdown = new List<CustomPartyType>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT * FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 9";


                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["PARTY_CODE"]);
                                string? name = Convert.ToString(reader["PARTY_NAME"]);
                                int accountCode = Convert.ToInt32(reader["ACT_CODE"]);
                                int paymentTerms = Convert.ToInt32(reader["PAYMENT_TERMS"]);
                                int commission = reader["COMM"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMM"]);
                                string? customizedKey = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}";
                                dropdown.Add(new CustomPartyType { key = code, customizedKey = customizedKey, value = name, accountCode = accountCode, paymentTerms = paymentTerms, commission = commission });

                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<CustomPartyType> CustomPartyTypeDropdownWithAccountCodeAndPType(int? roleId, string? roleType, int? pType)
        {
            List<CustomPartyType> dropdown = new List<CustomPartyType>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"	
                    DECLARE @ROLE_TYPE NVARCHAR(10);
                    DECLARE @ROLE_ID INT;
					DECLARE @PTYPE INT;

                    SET @ROLE_TYPE = '{roleType}';  
                    SET @ROLE_ID = {roleId};  
					 SET @PTYPE = {pType};  
                    SELECT 
                        M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM
                    FROM TBL_ROLE R 
                    LEFT OUTER JOIN TBL_PARTY_TYPES M 
                        ON (
                            (R.SHOW_SELECTED = 1 AND M.PARTY_CODE = R.RMENU_ID AND M.ACT_CODE = R.ACT_CODE)
                            OR 
                            (R.SHOW_SELECTED = 0 AND 
                            M.PARTY_CODE NOT IN 
                                (SELECT DISTINCT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = @ROLE_ID AND MODULE_ID = 6) 
                            AND 
                            M.ACT_CODE NOT IN 
                                (SELECT DISTINCT ACT_CODE FROM TBL_ROLE WHERE ROLE_ID = @ROLE_ID AND MODULE_ID = 6)
                            )
                        ) 
                    WHERE R.ROLE_TYPE = @ROLE_TYPE
                        AND R.MODULE_ID = 6 
                        AND M.DLT = 'T' 
                        AND M.ASTATUS = 'Y'
                        AND R.ROLE_ID = @ROLE_ID
						AND M.PARTY_TYPE_CODE = CASE WHEN @PTYPE = 0 THEN  M.PARTY_TYPE_CODE ELSE  @PTYPE END 
                    GROUP BY M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM


                        UNION ALL

                        SELECT    M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM
                        FROM TBL_PARTY_TYPES M WHERE 
  
                    @ROLE_TYPE
  
                        = 'A'  

  
                            AND M.DLT = 'T' 
                            AND M.ASTATUS = 'Y'
								AND M.PARTY_TYPE_CODE = CASE WHEN @PTYPE = 0 THEN  M.PARTY_TYPE_CODE ELSE  @PTYPE END 
                    UNION ALL
                            SELECT M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM
                            FROM TBL_PARTY_TYPES M WHERE 
  
                    @ROLE_TYPE

                        = 'U'  
  
   
                            AND M.DLT = 'T' 
                            AND M.ASTATUS = 'Y' AND 
                            (SELECT 
                        COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_PARTY_TYPES M ON R.RMENU_ID = M.PARTY_CODE  AND M.ACT_CODE = R.ACT_CODE
                        WHERE R.ROLE_TYPE = 
                    @ROLE_TYPE
   
                            AND R.MODULE_ID = 6 

                            AND M.DLT = 'T' 
                            AND M.ASTATUS = 'Y'
                            AND R.ROLE_ID =
	  
                        @ROLE_ID
							AND M.PARTY_TYPE_CODE = CASE WHEN @PTYPE = 0 THEN  M.PARTY_TYPE_CODE ELSE  @PTYPE END 

	
                            ) < 1";


                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["PARTY_CODE"]);
                                string? name = Convert.ToString(reader["PARTY_NAME"]);
                                int accountCode = Convert.ToInt32(reader["ACT_CODE"]);
                                int paymentTerms = Convert.ToInt32(reader["PAYMENT_TERMS"]);
                                int commission = reader["COMM"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMM"]);
                                string? customizedKey = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}";
                                dropdown.Add(new CustomPartyType { key = code, customizedKey = customizedKey, value = name, accountCode = accountCode, paymentTerms = paymentTerms, commission = commission });

                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<CustomPartyType> CustomPartyTypeDropdownWithAccountCodeForGatePass(int? roleId, string? branchId, int? showSelected)
        {
            List<CustomPartyType> dropdown = new List<CustomPartyType>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT PARTY_CODE, PARTY_NAME, ACT_CODE, PAYMENT_TERMS, COMM FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' AND PARTY_TYPE_CODE = 10  AND PARTY_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = {roleId} AND MODULE_ID = 6 AND R_BCODE = {Convert.ToInt32(branchId)})  AND ACT_CODE IN (SELECT ACT_CODE FROM TBL_ROLE WHERE ROLE_ID = {roleId} AND MODULE_ID = 6 AND R_BCODE = {Convert.ToInt32(branchId)})";
                    else
                        query = $"SELECT PARTY_CODE, PARTY_NAME, ACT_CODE, PAYMENT_TERMS, COMM FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' AND PARTY_TYPE_CODE = 10  AND PARTY_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = {roleId} AND MODULE_ID = 6 AND R_BCODE = {Convert.ToInt32(branchId)})  AND ACT_CODE NOT IN (SELECT ACT_CODE FROM TBL_ROLE WHERE ROLE_ID = {roleId} AND MODULE_ID = 6 AND R_BCODE = {Convert.ToInt32(branchId)})";
                }
                else
                {
                    query = "SELECT PARTY_CODE, PARTY_NAME, ACT_CODE, PAYMENT_TERMS, COMM FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND PARTY_TYPE_CODE = 10 AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["PARTY_CODE"]);
                                string? name = Convert.ToString(reader["PARTY_NAME"]);
                                int accountCode = Convert.ToInt32(reader["ACT_CODE"]);
                                int paymentTerms = Convert.ToInt32(reader["PAYMENT_TERMS"]);
                                int commission = reader["COMM"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMM"]);
                                string? customizedKey = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}";
                                dropdown.Add(new CustomPartyType { key = code, customizedKey = customizedKey, value = name, accountCode = accountCode, paymentTerms = paymentTerms, commission = commission });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<CustomPartyType> PartyTypeWithAccountCodeDynamic(Common common)
        {
            List<CustomPartyType> dropdown = new List<CustomPartyType>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (common.RoleType != "A")
                {
                    if (common.ShowSelected == 1)
                        query = $"SELECT PARTY_CODE, PARTY_NAME, ACT_CODE, PAYMENT_TERMS, COMM " +
                            $"FROM TBL_PARTY_TYPES " +
                            $"WHERE DLT = 'T' AND " +
                            $"ASTATUS = 'Y' AND " +
                            $"((SELECT PTYPE FROM TBL_MENU_BUILDER WHERE ID = {common.MenuID}) = 0 OR PARTY_TYPE_CODE = (SELECT PTYPE FROM TBL_MENU_BUILDER WHERE ID = {common.MenuID})) AND " +
                            $"PARTY_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = {common.RoleID} AND MODULE_ID = 6 AND R_BCODE = {Convert.ToInt32(common.Branch)}) AND " +
                            $"ACT_CODE IN (SELECT ACT_CODE FROM TBL_ROLE WHERE ROLE_ID = {common.RoleID} AND MODULE_ID = 6 AND R_BCODE = {Convert.ToInt32(common.Branch)})";
                    else
                        query = $"SELECT PARTY_CODE, PARTY_NAME, ACT_CODE, PAYMENT_TERMS, COMM FROM TBL_PARTY_TYPES " +
                            $"WHERE DLT = 'T' AND " +
                            $"ASTATUS = 'Y' AND " +
                            $"((SELECT PTYPE FROM TBL_MENU_BUILDER WHERE ID = {common.MenuID}) = 0 OR PARTY_TYPE_CODE = (SELECT PTYPE FROM TBL_MENU_BUILDER WHERE ID = {common.MenuID})) AND " +
                            $"PARTY_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = {common.RoleID} AND MODULE_ID = 6 AND R_BCODE = {Convert.ToInt32(common.Branch)}) AND " +
                            $"ACT_CODE NOT IN (SELECT ACT_CODE FROM TBL_ROLE WHERE ROLE_ID = {common.RoleID} AND MODULE_ID = 6 AND R_BCODE = {Convert.ToInt32(common.Branch)})";
                }
                else
                {
                    query = $"SELECT PARTY_CODE, PARTY_NAME, ACT_CODE, PAYMENT_TERMS, COMM " +
                        $"FROM TBL_PARTY_TYPES " +
                        $"WHERE DLT = 'T' AND " +
                        //$"PARTY_TYPE_CODE = (SELECT PTYPE FROM TBL_MENU_BUILDER WHERE ID = {common.MenuID}) AND " +
                        $"((SELECT PTYPE FROM TBL_MENU_BUILDER WHERE ID = {common.MenuID}) = 0 OR PARTY_TYPE_CODE = (SELECT PTYPE FROM TBL_MENU_BUILDER WHERE ID = {common.MenuID})) AND " +
                        $"ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["PARTY_CODE"]);
                                string? name = Convert.ToString(reader["PARTY_NAME"]);
                                int accountCode = Convert.ToInt32(reader["ACT_CODE"]);
                                int paymentTerms = Convert.ToInt32(reader["PAYMENT_TERMS"]);
                                int commission = reader["COMM"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COMM"]);
                                string? customizedKey = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}";
                                dropdown.Add(new CustomPartyType { key = code, customizedKey = customizedKey, value = name, accountCode = accountCode, paymentTerms = paymentTerms, commission = commission });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> PartySalesmanDropdown(int? roleId, string? branchId)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    query = $"SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' AND PARTY_TYPE_CODE = '9' AND PARTY_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = "SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' AND PARTY_TYPE_CODE = '9'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> WithOutCurrentBrachDropdown(int Id)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT BCODE,B_NAME FROM TBL_BRANCH WHERE DLT = 'T' AND BCODE != '" + Id + "'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> CurrentBrachDropdown(int Id)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT BCODE,B_NAME FROM TBL_BRANCH WHERE DLT = 'T' AND BCODE = '" + Id + "'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> employeeDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT EMP_CODE,ENAME FROM TBL_EMP_REG WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> GetEmployeeData()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT EMP_CODE,ENAME,EMP_ID FROM TBL_EMP_REG WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["EMP_CODE"]);
                                string name = Convert.ToString(reader["ENAME"]);
                                string EmpId = Convert.ToString(reader["EMP_ID"]);
                                dropdown.Add(new { key = code, value = name, EmpId = EmpId });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> GetBranchWithPeriod()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT B.BCODE,B.B_NAME,P.PID FROM TBL_BRANCH B
                                    LEFT JOIN TBL_PERIOD P ON P.BCODE = B.BCODE
                                    WHERE B.DLT = 'T' AND B.ASTATUS = 'Y'";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string bCode = Convert.ToString(reader["BCODE"]);
                                string bName = Convert.ToString(reader["B_NAME"]);
                                string pid = Convert.ToString(reader["PID"]);
                                dropdown.Add(new { key = bCode, value = bName, pid = pid});
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> GetEmployees()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT E.EMP_CODE, E.EMP_ID, E.ENAME, DP.DESCR DEP, DG.GROUP_NAME DESIG, BR.B_NAME BRANCH, E.FATHER_NAME 
                                FROM TBL_EMP_REG E 
                                LEFT OUTER JOIN TBL_DESIGNATION DG ON DG.GROUP_CODE = E.DESIG
                                LEFT OUTER JOIN TBL_BRANCH BR ON BR.BCODE = E.BCODE
                                LEFT OUTER JOIN TBL_ACT_GROUP DP ON DP.CODE = E.DEP_ID
                                WHERE E.DLT = 'T' AND E.ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["EMP_CODE"]);
                                string empId = Convert.ToString(reader["EMP_ID"]);
                                string name = Convert.ToString(reader["ENAME"]);
                                string dep = Convert.ToString(reader["DEP"]);
                                string desig = Convert.ToString(reader["DESIG"]);
                                string branch = Convert.ToString(reader["BRANCH"]);
                                string father = Convert.ToString(reader["FATHER_NAME"]);
                                dropdown.Add(new { key = code, value = name, empId = empId, dep = dep, desig = desig, branch = branch, father = father });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> CurrencyDropdownWithControlName()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE,DESCR,DESCR2,RATE FROM TBL_CURRENCY WHERE ASTATUS = 'Y' AND DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string name = Convert.ToString(reader["DESCR"]);
                                string controlName = Convert.ToString(reader["DESCR2"]);
                                string rate = Convert.ToString(reader["RATE"]);
                                dropdown.Add(new { key = code, value = name, name = controlName, rate = rate });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> CategoryDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_CATEGORY WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> SubCategoryDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_SUB_CATEGORY WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> EntityDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_ENTITY WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> RegionDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE,DESCR FROM TBL_REGION WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> RegionDropdownOnSCondition()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE,DESCR FROM TBL_REGION WHERE GROUP_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> PermitDetails()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT IMPORT_PERMIT, SUM(TQTY - IQTY) AS BalanceQty FROM TBL_IP_DETAIL GROUP BY IMPORT_PERMIT";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string key = Convert.ToString(reader["IMPORT_PERMIT"]);
                                decimal value = reader["BalanceQty"] != DBNull.Value ? Convert.ToDecimal(reader["BalanceQty"].ToString()) : 0;
                                dropdown.Add(new { key = key, value = value, code = "abc" });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> ReligionDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE, GROUP_NAME FROM TBL_RELIGION WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> OfferLetterDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT OL.TRAN_ID, CAN.FULL_NAME FROM TBL_OFFER_LETTER OL
                                    LEFT OUTER JOIN TBL_INTERVIEW_SCH INTS ON INTS.TRAN_ID = OL.INT_ID
                                    LEFT OUTER JOIN TBL_CANDIDATES CAN ON CAN.TRAN_ID = INTS.CON_ID
                                    WHERE OL.DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> DesignationDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE, GROUP_NAME FROM TBL_DESIGNATION WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> DepartmentDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE, DESCR FROM TBL_ACT_GROUP WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> CostCenterDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE,DESCR FROM TBL_COST_CENTER WHERE GROUP_TYPE = 'S' AND ASTATUS = 'Y' AND DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> EmploymentTypeDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE, GROUP_NAME FROM TBL_EMP_TYPE WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> ShiftDropdown()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @$"SELECT GROUP_CODE, GROUP_NAME, CONVERT(VARCHAR(20), TIME_IN, 100) AS TIME_IN, 
                                CONVERT(VARCHAR(20), TIME_OUT, 100) AS TIME_OUT FROM TBL_SHIFT WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["GROUP_CODE"]);
                                string name = Convert.ToString(reader["GROUP_NAME"]);
                                //string timeIn = reader["TIME_IN"] == DBNull.Value ? null : ((TimeSpan)reader["TIME_IN"]).ToString(@"hh\:mm\:ss");
                                //string timeOut = reader["TIME_OUT"] == DBNull.Value ? null : ((TimeSpan)reader["TIME_OUT"]).ToString(@"hh\:mm\:ss");
                                string timeIn = reader["TIME_IN"] == DBNull.Value ? null : reader["TIME_IN"].ToString();
                                string timeOut = reader["TIME_OUT"] == DBNull.Value ? null : reader["TIME_OUT"].ToString();
                                dropdown.Add(new { code = code, name = name, timeIn = timeIn, timeOut = timeOut });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> EducationDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE, GROUP_NAME FROM TBL_EDUCATION WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> UnitDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_UNIT WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> JobDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT TRAN_ID,JOB_TITLE FROM TBL_JOB_TYPE WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> CandidateDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT TRAN_ID,FULL_NAME,EMAIL FROM TBL_CANDIDATES WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> InterviewsByEmpDropdown(int id)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @$"SELECT ISCH.TRAN_ID,CAN.FULL_NAME FROM TBL_INTERVIEW_SCH ISCH LEFT OUTER JOIN TBL_CANDIDATES CAN ON CAN.TRAN_ID = ISCH.CON_ID WHERE ISCH.EMP_ID LIKE '%{id}%' AND ISCH.DLT = 'T' AND ISCH.ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> InterviewsDropdown()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @$"SELECT INT_S.TRAN_ID,CAN.FULL_NAME AS CAN_NAME, JOB.JOB_TITLE, JOB.DEP_ID FROM TBL_INTERVIEW_FEEDBACK INT_F
                                    LEFT OUTER JOIN TBL_INTERVIEW_SCH INT_S ON INT_S.TRAN_ID = INT_F.INT_ID
                                    LEFT OUTER JOIN TBL_CANDIDATES CAN ON CAN.TRAN_ID = INT_S.CON_ID
                                    LEFT OUTER JOIN TBL_JOB_TYPE JOB ON JOB.TRAN_ID = CAN.JOB_ID
                                    WHERE INT_F.DLT = 'T' GROUP BY INT_S.TRAN_ID,CAN.FULL_NAME , JOB.JOB_TITLE, JOB.DEP_ID";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int TRAN_ID = Convert.ToInt32(reader["TRAN_ID"]);
                                string CAN_NAME = Convert.ToString(reader["CAN_NAME"]);
                                string JOB_TITLE = Convert.ToString(reader["JOB_TITLE"]);
                                int DEP_ID = Convert.ToInt32(reader["DEP_ID"]);
                                dropdown.Add(new { TRAN_ID = TRAN_ID, CAN_NAME = CAN_NAME, JOB_TITLE = JOB_TITLE, DEP_ID = DEP_ID });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> EmpDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT EMP_CODE,ENAME FROM TBL_EMP_REG WHERE DLT = 'T' AND ASTATUS = 'Y' AND MSTAFF = 'Y' ORDER BY 1 ASC";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> PayTermsDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_PAY_TERMS WHERE DLT = 'T' AND ASTATUS = 'Y' ORDER BY 1 ASC";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> BrandDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_GRADE WHERE DLT = 'T' AND ASTATUS = 'Y' ORDER BY 1 ASC";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> ColorDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_COLOR WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> SizeDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_SIZE WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> PortDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_PORT WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> SEntityDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_S_ENTITY WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> DistributionChannelDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_D_CHANNEL WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        //public static List<KeyValuePair<int, string>> GradeDropdown()
        //{
        //    List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
        //    using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
        //    {
        //        string query = "SELECT GROUP_CODE,GROUP_NAME,SIZE FROM TBL_GRADE WHERE DLT = 'T'";
        //        using (SqlCommand command = new SqlCommand(query, conn))
        //        {
        //            conn.Open();
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        int code = reader.GetInt32(0);
        //                        string name = reader.GetString(1);
        //                        string size = reader.GetString(2);
        //                        dropdown.Add(new KeyValuePair<int, string>(code, name));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return dropdown;
        //}

        public static List<dynamic> GradeDropdown()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME,SIZE FROM TBL_GRADE WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int? code = reader["GROUP_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GROUP_CODE"]);
                                string? name = reader["GROUP_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["GROUP_NAME"]);
                                int? size = reader["SIZE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE"]);
                                dropdown.Add(new { key = code, value = name, size = size, });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }

        //public static List<KeyValuePair<int, string>> ClientPODropdown()
        //{
        //    List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
        //    using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
        //    {
        //        string query = "SELECT CODE,CLIENT_PO FROM TBL_MPO_REG WHERE DLT = 'T'";
        //        using (SqlCommand command = new SqlCommand(query, conn))
        //        {
        //            conn.Open();
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        int code = reader.GetInt32(0);
        //                        string name = reader.GetString(1);
        //                        dropdown.Add(new KeyValuePair<int, string>(code, name));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return dropdown;
        //}

        public static List<dynamic> ClientPODropdown()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @$"SELECT CODE,CLIENT_PO, PARTY_CODE, ACT_CODE, FABRIC, GSM FROM TBL_MPO_REG WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string clientPo = Convert.ToString(reader["CLIENT_PO"]);
                                int partyCode = Convert.ToInt32(reader["PARTY_CODE"]);
                                int actCode = Convert.ToInt32(reader["ACT_CODE"]);
                                int fabric = Convert.ToInt32(reader["FABRIC"]);
                                int gsm = Convert.ToInt32(reader["GSM"]);
                                dropdown.Add(new { key = code, value = clientPo, partyCode = partyCode, actCode = actCode, fabric = fabric, gsm = gsm });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }

        public static List<dynamic> POJobsDropdown()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @$"SELECT MPO.TRAN_ID, MPO.CLIENT_PO, MPO.JOB_NO, P.PARTY_NAME FROM TBL_MPO_MASTER MPO
                                    LEFT OUTER JOIN TBL_PARTY_TYPES P ON P.PARTY_CODE = MPO.PARTY_CODE AND P.ACT_CODE = MPO.ACT_CODE
                                    WHERE MPO.DLT = 'T' AND MPO.ASTATUS = 'Y' ORDER BY TRAN_ID DESC ";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["TRAN_ID"]);
                                string clientPo = Convert.ToString(reader["CLIENT_PO"]);
                                string JobNo = Convert.ToString(reader["JOB_NO"]);
                                string PartyName = Convert.ToString(reader["PARTY_NAME"]);

                                dropdown.Add(new { key = code, value = JobNo, clientPo = clientPo, PartyName = PartyName });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }
        public static List<dynamic> GradeDropdownWithControlName()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @"SELECT GROUP_CODE,GROUP_NAME, GROUP_NAME AS CONTROL_NAME
                                FROM TBL_GRADE WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["GROUP_CODE"]);
                                string name = Convert.ToString(reader["GROUP_NAME"]);
                                string controlName = Convert.ToString(reader["CONTROL_NAME"]);
                                dropdown.Add(new { key = code, value = name, name = controlName });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> NatureOfBusinessDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_BUS_NATURE WHERE DLT = 'T' ANA ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> GetGrandData()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_GRADE WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> GetFabricData()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_FABRIC WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> GetGSMData()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_GSM WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> GetSeasonData()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_SEASON WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> GetStyleData()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_STYLE WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> MenuDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT ID, MENU_NAME FROM TBL_MENU_BUILDER WHERE DLT = 'T' AND ASTATUS = 'Y' AND MTYPE = 'S'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> MenuDropdownWithSno()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.ID, A.MENU_NAME, IsNULL((SELECT MAX(SNO)+1 FROM TBL_MENU_BUILDER_DETAIL WHERE MMENU_ID = A.ID " +
                    "AND DLT = 'T' AND ASTATUS = 'Y' AND MTYPE = 'S'), 1) AS SNO FROM TBL_MENU_BUILDER A WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' AND A.MTYPE = 'S'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ID"]);
                                string? name = Convert.ToString(reader["MENU_NAME"]);
                                int sno = Convert.ToInt32(reader["SNO"]);
                                dropdown.Add(new { key = code, value = name, sno = sno });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> MenuReortDropdownWithSno()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.ID, A.MENU_NAME, IsNULL((SELECT MAX(SNO)+1 FROM TBL_REPORT_TYPES WHERE M_ID = A.ID AND DLT = 'T' AND ASTATUS = 'Y' AND MTYPE = 'S'), 1) AS SNO " +
                    "FROM TBL_MENU_BUILDER A WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' AND A.MTYPE = 'S'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ID"]);
                                string? name = Convert.ToString(reader["MENU_NAME"]);
                                int sno = Convert.ToInt32(reader["SNO"]);
                                dropdown.Add(new { key = code, value = name, sno = sno });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> HRMasterDropdown(Common common)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"Select GROUP_CODE , GROUP_NAME , TABLE_NAME from TBL_MASTER_TABLES Where DLT = 'T' AND GMENU_ID = {common.MenuID}";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["GROUP_CODE"]);
                                string? name = Convert.ToString(reader["GROUP_NAME"]);
                                string? tname = Convert.ToString(reader["TABLE_NAME"]);
                                dropdown.Add(new { key = code, value = name, tname = tname });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> GetAllTablesData()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"Select GROUP_CODE , GROUP_NAME , GPIC From TBL_TABLE Where DLT = 'T' AND ASTATUS = 'Y' ";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["GROUP_CODE"]);
                                string? name = Convert.ToString(reader["GROUP_NAME"]);
                                dropdown.Add(new { code = code, name = name });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> GetPosUsers()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"select U_ID,USERNAME from TBL_USER where ROLE_TYPE = 'U' AND DLT ='T' AND ASTATUS='Y' ";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["U_ID"]);
                                string? name = Convert.ToString(reader["USERNAME"]);
                                dropdown.Add(new { code = code, name = name });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> BanksNameDropdown(int? pType, Common common)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @$" SELECT 
                                      CH.ACT_CODE,CH.ACT_NAME 
                                    FROM TBL_ROLE R 
                                    LEFT OUTER JOIN TBL_CHART CH

                                       ON (

                                          (R.SHOW_SELECTED = 1 AND CH.ACT_CODE = R.RMENU_ID)
                                          OR 
                                          (R.SHOW_SELECTED = 0 AND 
                                           CH.ACT_CODE NOT IN 
                                             (SELECT DISTINCT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = {common.RoleID} AND MODULE_ID = 3) 
      
                                          )
                                      ) 
                                      LEFT OUTER JOIN TBL_CHART PCH
                                    ON PCH.ACT_CODE = CH.ACT_PARENT_CODE
                                    WHERE R.ROLE_TYPE = '{common.RoleType}'
                                      AND R.MODULE_ID = 3 
                                      AND CH.DLT = 'T' 
                                      AND CH.ASTATUS = 'Y'
                                      AND R.ROLE_ID = {common.RoleID}
                                      AND CH.ACT_NATURE = 2 AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'

                                    GROUP BY CH.ACT_CODE,CH.ACT_NAME 


                                      UNION ALL

                                      SELECT     CH.ACT_CODE,CH.ACT_NAME 
                                      FROM TBL_CHART CH
                                      LEFT OUTER JOIN TBL_CHART PCH
                                    ON PCH.ACT_CODE = CH.ACT_PARENT_CODE

                                    WHERE 

                                   '{common.RoleType}' = 'A'  
                                    AND CH.ACT_NATURE = 2 AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'


                                    UNION ALL
                                            SELECT CH.ACT_CODE,CH.ACT_NAME 
		                                    FROM TBL_CHART CH 
                                      LEFT OUTER JOIN TBL_CHART PCH
                                    ON PCH.ACT_CODE = CH.ACT_PARENT_CODE
		                                    WHERE 
  
                                    '{common.RoleType}'

                                      = 'U'  
                                    AND CH.ACT_NATURE = 2 AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'
   
                                          AND CH.DLT = 'T' 
                                          AND CH.ASTATUS = 'Y' AND  
                                           (SELECT 
                                     COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_CHART M ON R.RMENU_ID = M.ACT_CODE 
                                     WHERE R.ROLE_TYPE = 
                                    '{common.RoleType}'
   
                                          AND R.MODULE_ID = 3 

                                          AND M.DLT = 'T' 
                                          AND M.ASTATUS = 'Y'
                                          AND R.ROLE_ID =
	  
                                      {common.RoleID}
                                        AND CH.ACT_NATURE = 2 AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'
	
                                          ) < 1 ";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> EducationRecords()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "Select GROUP_CODE , GROUP_NAME from TBL_EDUCATION where DLT = 'T' And ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }
        public static MyHttpResponseMessage GetEmplyeeGroup()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            List<Employee> employeeGroups = new List<Employee>();
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @"Select EMP_CODE , EMP_ID , ENAME , DS.GROUP_NAME from TBL_EMP_REG EP
                    Left Join TBL_DESIGNATION DS ON DS.GROUP_CODE = EP.DEP_ID " +
                    " WHERE EP.DLT = 'T' AND EP.ASTATUS = 'Y'";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Employee employeeGroup = new Employee();
                    employeeGroup.CODE = Convert.ToInt32(reader["EMP_CODE"]);
                    employeeGroup.EMP_ID = Convert.ToString(reader["EMP_ID"]);
                    employeeGroup.ENAME = Convert.ToString(reader["ENAME"]);
                    employeeGroup.DESIGNATION_NAME = Convert.ToString(reader["GROUP_NAME"]);
                    employeeGroups.Add(employeeGroup);
                }
                reader.Close();
                response.data = employeeGroups;
                response.msg = "";
                response.msgType = 1;
                return response;
            }
        }
        public static List<KeyValuePair<int, string>> AcountNameDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "Select ACT_CODE , ACT_NAME from TBL_CHART where ACT_NATURE = 1 And DLT = 'T' And ASTATUS = 'Y' and ACT_TYPE = 'S'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }
        public static List<dynamic> GetPartyName(int? roleId, string? roleType)
        {
            List<dynamic> dropdown = new List<dynamic>();
            var nextId = 0;
            string query = string.Empty;
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                query = $@"DECLARE @ROLE_TYPE NVARCHAR(10);
                                DECLARE @ROLE_ID INT;

                                SET @ROLE_TYPE = 'U';  
                                SET @ROLE_ID = 4;  
                                SELECT 
                                    M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM , M.CELL
                                FROM TBL_ROLE R 
                                LEFT OUTER JOIN TBL_PARTY_TYPES M 
                                    ON (
                                        (R.SHOW_SELECTED = 1 AND M.PARTY_CODE = R.RMENU_ID AND M.ACT_CODE = R.ACT_CODE)
                                        OR 
                                        (R.SHOW_SELECTED = 0 AND 
                                        M.PARTY_CODE NOT IN 
                                            (SELECT DISTINCT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = @ROLE_ID AND MODULE_ID = 6) 
                                        AND 
                                        M.ACT_CODE NOT IN 
                                            (SELECT DISTINCT ACT_CODE FROM TBL_ROLE WHERE ROLE_ID = @ROLE_ID AND MODULE_ID = 6)
                                        )
                                    ) 
                                WHERE R.ROLE_TYPE = @ROLE_TYPE
                                    AND R.MODULE_ID = 6 
                                    AND M.DLT = 'T' 
                                    AND M.ASTATUS = 'Y'
                                    AND R.ROLE_ID = @ROLE_ID
                                GROUP BY M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM , M.CELL


                                    UNION ALL

                                    SELECT    M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM, M.CELL
                                    FROM TBL_PARTY_TYPES M WHERE 
  
                                @ROLE_TYPE
  
                                    = 'A'  

  
                                        AND M.DLT = 'T' 
                                        AND M.ASTATUS = 'Y'
                                UNION ALL
                                        SELECT M.PARTY_CODE, M.PARTY_NAME, M.ACT_CODE, M.PAYMENT_TERMS, M.COMM , M.CELL
		                                FROM TBL_PARTY_TYPES M WHERE 
  
                                @ROLE_TYPE

                                    = 'U'  
  
   
                                        AND M.DLT = 'T' 
                                        AND M.ASTATUS = 'Y' AND 
                                        (SELECT 
                                    COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_PARTY_TYPES M ON R.RMENU_ID = M.PARTY_CODE  AND M.ACT_CODE = R.ACT_CODE
                                    WHERE R.ROLE_TYPE = 
                                @ROLE_TYPE
   
                                        AND R.MODULE_ID = 6 

                                        AND M.DLT = 'T' 
                                        AND M.ASTATUS = 'Y'
                                        AND R.ROLE_ID =
	  
                                    @ROLE_ID
                                        ) < 1";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int pcode = Convert.ToInt32(reader["PARTY_CODE"]);
                                int acode = Convert.ToInt32(reader["ACT_CODE"]);
                                string name = Convert.ToString(reader["PARTY_NAME"]);
                                string cell = Convert.ToString(reader["CELL"]);
                                dropdown.Add(new { code = acode, key = pcode, value = name, cell = cell });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }

        public static List<dynamic> GetBatchName(int itemCode)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT BATCH_NAME FROM TBL_BOM_MASTER WHERE ITEM_CODE = {itemCode} AND ASTATUS = 'Y' AND DLT = 'T' AND BATCH_NAME IS NOT NULL;";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string? batchName = reader["BATCH_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["BATCH_NAME"]);
                                dropdown.Add(new { batchName = batchName });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }


        public static List<dynamic> SalesmanNameDropdown(Common common)
        {
            List<dynamic> dropdown = new List<dynamic>();
            string query = string.Empty;
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                query = $"Select PARTY_CODE , ACT_CODE , PARTY_NAME from TBL_PARTY_TYPES where PARTY_TYPE_CODE = 9 AND  DLT = 'T' And ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int pcode = Convert.ToInt32(reader["PARTY_CODE"]);
                                int acode = Convert.ToInt32(reader["ACT_CODE"]);
                                string name = Convert.ToString(reader["PARTY_NAME"]);
                                dropdown.Add(new { code = acode, key = pcode, value = name });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }
        public static List<dynamic> LotDropdown()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT L.CODE, L.LOT_NO, L.PARTY_CODE, L.ACT_CODE from TBL_LOT L WHERE L.DLT = 'T' And L.ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string lot = Convert.ToString(reader["LOT_NO"]);
                                string? customizedKey = $"{Convert.ToString(reader["PARTY_CODE"])}{Convert.ToString(reader["ACT_CODE"])}";
                                dropdown.Add(new { key = code, value = lot, customizedKey = customizedKey });
                            }
                        }
                    }
                    conn.Close();
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> CompanyDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CCODE,C_NAME FROM TBL_COMPANY WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> ItemTypeDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_ITEM_TYPE WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> ItemGroupParentDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_ITEMSGROUP WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> ItemGroupParentDropdownWithControlName()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.GROUP_CODE, A.GROUP_NAME, B.GROUP_NAME AS CONTROL_NAME FROM TBL_ITEMSGROUP A " +
                               "LEFT JOIN TBL_ITEMSGROUP B ON A.PARENT_CODE = B.GROUP_CODE " +
                               "WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' AND A.GROUP_TYPE = 'C' ORDER BY A.GR_CODE";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["GROUP_CODE"]);
                                string name = Convert.ToString(reader["GROUP_NAME"]);
                                string controlName = Convert.ToString(reader["CONTROL_NAME"]);
                                dropdown.Add(new { key = code, value = name, name = controlName });
                            }
                        }
                    }
                }
            }

            return dropdown;
        }
        public static List<KeyValuePair<int, string>> BranchDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT BCODE,B_NAME FROM TBL_BRANCH WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> RelationDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE, GROUP_NAME FROM TBL_RELATION WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<Branch> BranchDataDropdown()
        {
            List<Branch> dropdown = new List<Branch>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT BCODE,B_NAME,B_ADDRESS,B_TEL FROM TBL_BRANCH WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dropdown.Add(new Branch
                            {
                                BCODE = Convert.ToInt32(reader["BCODE"]),
                                B_NAME = reader["B_NAME"].ToString(),
                                B_ADDRESS = reader["B_ADDRESS"].ToString(),
                                B_TEL = reader["B_TEL"].ToString()
                            });
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> PeriodDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT PID,DESCR FROM TBL_PERIOD WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> DescendingPeriodDropdown(int Id)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT PID,DESCR FROM TBL_PERIOD WHERE DLT = 'T' AND BCODE = '" + Id + "' ORDER BY PID DESC";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> AccountSetupDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_ACCOUNT_SETUP WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        //public static List<KeyValuePair<int, string>> ItemMasterDropdown()
        //{
        //    List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
        //    using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
        //    {
        //        string query = "SELECT ITEM_CODE,ITEM_NAME FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y'";
        //        using (SqlCommand command = new SqlCommand(query, conn))
        //        {
        //            conn.Open();
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        int code = reader.GetInt32(0);
        //                        string name = reader.GetString(1);
        //                        dropdown.Add(new KeyValuePair<int, string>(code, name));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return dropdown;
        //}

        public static List<KeyValuePair<int, string>> ItemMasterDropdown(int? roleId, string? roleType)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"DECLARE @ROLE_TYPE NVARCHAR(10);
                DECLARE @ROLE_ID INT;

                SET @ROLE_TYPE = '{roleType}';  
                SET @ROLE_ID = {roleId};  

                SELECT 
                  M.ITEM_CODE, M.ITEM_NAME,M.SALE_RATE,M.HS_CODE
                FROM TBL_ROLE R 
                LEFT OUTER JOIN TBL_ITEMSMASTER M 
                  ON (R.SHOW_SELECTED = 1 AND M.ITEM_CODE = R.RMENU_ID)
                  OR (R.SHOW_SELECTED = 0 AND M.ITEM_CODE NOT IN 
                      (SELECT DISTINCT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = @ROLE_ID AND MODULE_ID = 4)
                  )

                WHERE R.ROLE_TYPE = @ROLE_TYPE
                  AND R.MODULE_ID = 4 
                  AND M.DLT = 'T' 
                  AND M.ASTATUS = 'Y'
                  AND R.ROLE_ID = @ROLE_ID
                GROUP BY M.ITEM_CODE, M.ITEM_ID, M.ITEM_NAME, M.IPIC, M.SALE_RATE,M.HS_CODE


                  UNION ALL

                  SELECT  M.ITEM_CODE, M.ITEM_NAME,M.SALE_RATE,M.HS_CODE FROM TBL_ITEMSMASTER M WHERE 

                @ROLE_TYPE

                  = 'A'  


                      AND M.DLT = 'T' 
                      AND M.ASTATUS = 'Y'
                UNION ALL
                        SELECT  M.ITEM_CODE, M.ITEM_NAME,M.SALE_RATE,M.HS_CODE FROM TBL_ITEMSMASTER M WHERE 

                @ROLE_TYPE

                  = 'U'  


                      AND M.DLT = 'T' 
                      AND M.ASTATUS = 'Y' AND 
                       (SELECT 
                 COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_ITEMSMASTER M ON R.RMENU_ID = M.ITEM_CODE 
                 WHERE R.ROLE_TYPE = 
                @ROLE_TYPE

                      AND R.MODULE_ID = 4 

                      AND M.DLT = 'T' 
                      AND M.ASTATUS = 'Y'
                      AND R.ROLE_ID =

                  @ROLE_ID


                      ) < 1";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> ItemMasterDropdownNew(int? roleId, string? roleType, string? itemType, string? dcType)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {

                //      string query = $@"
                //                       DECLARE @ROLE_TYPE NVARCHAR(10);
                //                       DECLARE @ROLE_ID INT;
                //                       DECLARE @ITEM_TYPE NVARCHAR(5);
                //                       DECLARE @DC_TYPE NVARCHAR(5);

                //                       SET @ROLE_TYPE = '{roleType}';  
                //                       SET @ROLE_ID = {roleId};  
                //                       SET @ITEM_TYPE = '{itemType}';
                //                       SET @DC_TYPE = '{dcType}';

                //                       SELECT 
                //                           M.ITEM_CODE, 
                //                           M.ITEM_NAME,
                //                           CASE 
                //                               WHEN @DC_TYPE IN ('PB','PR') THEN M.PURCHASE_RATE
                //    WHEN @DC_TYPE IN ('SB','SR') THEN M.SALE_RATE
                //    ELSE 0 
                //END AS SALE_RATE,
                //                           M.SALESTAX,
                //                           M.IUNIT_CODE,
                //                           M.WEIGHT,
                //                           M.HS_CODE
                //                       FROM TBL_ROLE R 
                //                       LEFT OUTER JOIN TBL_ITEMSMASTER M 
                //                           ON (R.SHOW_SELECTED = 1 AND M.ITEM_CODE = R.RMENU_ID)
                //                           OR (
                //                               R.SHOW_SELECTED = 0 
                //                               AND M.ITEM_CODE NOT IN (
                //                                   SELECT DISTINCT RMENU_ID 
                //                                   FROM TBL_ROLE 
                //                                   WHERE ROLE_ID = @ROLE_ID 
                //                                     AND MODULE_ID = 4
                //                               )
                //                           )
                //                       WHERE 
                //                           R.ROLE_TYPE = @ROLE_TYPE
                //                           AND R.MODULE_ID = 4 
                //                           AND R.ROLE_ID = @ROLE_ID
                //                           AND M.DLT = 'T' 
                //                           AND M.ASTATUS = 'Y'
                //                           AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                //                       GROUP BY 
                //                           M.ITEM_CODE, 
                //                           M.ITEM_ID, 
                //                           M.ITEM_NAME, 
                //                           M.IPIC, 
                //                           M.SALE_RATE,
                //                           M.PURCHASE_RATE,
                //                           M.SALESTAX,
                //                           M.IUNIT_CODE,
                //                           M.WEIGHT,
                //                           M.HS_CODE

                //                       UNION ALL

                //                       SELECT  
                //                           M.ITEM_CODE, 
                //                           M.ITEM_NAME,
                //                           CASE 
                //                               WHEN @DC_TYPE IN ('PB','PR') THEN M.PURCHASE_RATE
                //    WHEN @DC_TYPE IN ('SB','SR') THEN M.SALE_RATE
                //    ELSE 0 
                //END AS SALE_RATE,
                //                           M.SALESTAX,
                //                           M.IUNIT_CODE,
                //                           M.WEIGHT,
                //                           M.HS_CODE
                //                       FROM TBL_ITEMSMASTER M 
                //                       WHERE 
                //                           @ROLE_TYPE = 'A'
                //                           AND M.DLT = 'T' 
                //                           AND M.ASTATUS = 'Y'
                //                           AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )

                //                       UNION ALL

                //                       SELECT  
                //                           M.ITEM_CODE, 
                //                           M.ITEM_NAME,
                //                           CASE 
                //                               WHEN @DC_TYPE IN ('PB','PR') THEN M.PURCHASE_RATE
                //    WHEN @DC_TYPE IN ('SB','SR') THEN M.SALE_RATE
                //    ELSE 0 
                //END AS SALE_RATE,
                //                           M.SALESTAX,
                //                           M.IUNIT_CODE,
                //                           M.WEIGHT,
                //                           M.HS_CODE
                //                       FROM TBL_ITEMSMASTER M 
                //                       WHERE 
                //                           @ROLE_TYPE = 'U'
                //                           AND M.DLT = 'T' 
                //                           AND M.ASTATUS = 'Y'
                //                           AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                //                           AND (
                //                               SELECT COUNT(R.MODULE_ID)
                //                               FROM TBL_ROLE R 
                //                               LEFT OUTER JOIN TBL_ITEMSMASTER MI 
                //                                   ON R.RMENU_ID = MI.ITEM_CODE
                //                               WHERE 
                //                                   R.ROLE_TYPE = @ROLE_TYPE
                //                                   AND R.MODULE_ID = 4 
                //                                   AND R.ROLE_ID = @ROLE_ID
                //                                   AND MI.DLT = 'T' 
                //                                   AND MI.ASTATUS = 'Y'
                //                                   AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                //                           ) < 1;";

                string query = $@"DECLARE @ROLE_TYPE NVARCHAR(10);
                                 DECLARE @ROLE_ID INT;
                                 DECLARE @ITEM_TYPE NVARCHAR(5);
                                 DECLARE @DC_TYPE NVARCHAR(5);

                                 SET @ROLE_TYPE = '{roleType}';  
                                 SET @ROLE_ID = {roleId};  
                                 SET @ITEM_TYPE = '{itemType}';
                                 SET @DC_TYPE = '{dcType}';

                                 SELECT 
                                     M.ITEM_CODE, 
                                     M.ITEM_NAME,
                                     CASE 
                                         WHEN @DC_TYPE IN ('PB','PR') THEN M.PURCHASE_RATE
									     WHEN @DC_TYPE IN ('SB','SR') THEN PM.RATE
									     ELSE 0 
									 END AS SALE_RATE,
                                     M.SALESTAX,
                                     M.IUNIT_CODE,
                                     M.WEIGHT,
                                     M.HS_CODE
                                 FROM TBL_ROLE R 
                                 LEFT OUTER JOIN TBL_ITEMSMASTER M 
                                     ON (R.SHOW_SELECTED = 1 AND M.ITEM_CODE = R.RMENU_ID)
                                     OR (
                                         R.SHOW_SELECTED = 0 
                                         AND M.ITEM_CODE NOT IN (
                                             SELECT DISTINCT RMENU_ID 
                                             FROM TBL_ROLE 
                                             WHERE ROLE_ID = @ROLE_ID 
                                               AND MODULE_ID = 4
                                         )
                                     )
								 LEFT OUTER JOIN TBL_PARTY_MAPPING PM ON PM.ITEM_CODE = M.ITEM_CODE
                                 WHERE 
                                     R.ROLE_TYPE = @ROLE_TYPE
                                     AND R.MODULE_ID = 4 
                                     AND R.ROLE_ID = @ROLE_ID
                                     AND M.DLT = 'T' 
                                     AND M.ASTATUS = 'Y'
                                     AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                                 GROUP BY 
                                     M.ITEM_CODE, 
                                     M.ITEM_ID, 
                                     M.ITEM_NAME, 
                                     M.IPIC, 
                                     M.SALE_RATE,
									 PM.RATE,
                                     M.PURCHASE_RATE,
                                     M.SALESTAX,
                                     M.IUNIT_CODE,
                                     M.WEIGHT,
                                     M.HS_CODE

                                 UNION ALL

                                 SELECT  
                                     M.ITEM_CODE, 
                                     M.ITEM_NAME,
                                     CASE 
                                         WHEN @DC_TYPE IN ('PB','PR') THEN M.PURCHASE_RATE
									     WHEN @DC_TYPE IN ('SB','SR') THEN PM.RATE
									     ELSE 0 
									 END AS SALE_RATE,
                                     M.SALESTAX,
                                     M.IUNIT_CODE,
                                     M.WEIGHT,
                                     M.HS_CODE
                                 FROM TBL_ITEMSMASTER M 
								 LEFT OUTER JOIN TBL_PARTY_MAPPING PM ON PM.ITEM_CODE = M.ITEM_CODE
                                 WHERE 
                                     @ROLE_TYPE = 'A'
                                     AND M.DLT = 'T' 
                                     AND M.ASTATUS = 'Y'
                                     AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )

                                 UNION ALL

                                 SELECT  
                                     M.ITEM_CODE, 
                                     M.ITEM_NAME,
                                     CASE 
                                         WHEN @DC_TYPE IN ('PB','PR') THEN M.PURCHASE_RATE
									     WHEN @DC_TYPE IN ('SB','SR') THEN PM.RATE
									     ELSE 0 
									 END AS SALE_RATE,
                                     M.SALESTAX,
                                     M.IUNIT_CODE,
                                     M.WEIGHT,
                                     M.HS_CODE
                                 FROM TBL_ITEMSMASTER M 
								 LEFT OUTER JOIN TBL_PARTY_MAPPING PM ON PM.ITEM_CODE = M.ITEM_CODE
                                 WHERE 
                                     @ROLE_TYPE = 'U'
                                     AND M.DLT = 'T' 
                                     AND M.ASTATUS = 'Y'
                                     AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                                     AND (
                                         SELECT COUNT(R.MODULE_ID)
                                         FROM TBL_ROLE R 
                                         LEFT OUTER JOIN TBL_ITEMSMASTER MI 
                                             ON R.RMENU_ID = MI.ITEM_CODE
                                         WHERE 
                                             R.ROLE_TYPE = @ROLE_TYPE
                                             AND R.MODULE_ID = 4 
                                             AND R.ROLE_ID = @ROLE_ID
                                             AND MI.DLT = 'T' 
                                             AND MI.ASTATUS = 'Y'
                                             AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                                     ) < 1;";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int? code = reader["ITEM_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]);
                                string? name = reader["ITEM_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_NAME"]);
                                double? rate = reader["SALE_RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["SALE_RATE"]);
                                double? saleTax = reader["SALESTAX"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SALESTAX"]);
                                int? unit = reader["IUNIT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IUNIT_CODE"]);
                                double? weight = reader["WEIGHT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["WEIGHT"]);
                                string? hsCode = reader["HS_CODE"] == DBNull.Value ? "" : Convert.ToString(reader["HS_CODE"]);

                                dropdown.Add(new
                                {
                                    key = code,
                                    value = name,
                                    rate = rate,
                                    saleTax = saleTax,
                                    unit = unit,
                                    weight = weight,
                                    hsCode = hsCode
                                }
                                );

                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> ItemMasterDropdownWithPrice(int? roleId, string? roleType, string? dcType, string? itemType)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {

                string query = $@"

                                DECLARE @ROLE_TYPE NVARCHAR(10);
                                DECLARE @ROLE_ID INT;
                                DECLARE @DC_TYPE NVARCHAR(5);
                                DECLARE @ITEM_TYPE NVARCHAR(5);

                                SET @ROLE_TYPE = '{roleType}';  
                                SET @ROLE_ID = {roleId};  
                                SET @DC_TYPE = '{dcType}';  
                                SET @ITEM_TYPE = '{itemType}';

                                SELECT 
                                    M.ITEM_CODE, 
                                    M.ITEM_NAME,
                                    M.HS_CODE,
	                                M.IUNIT_CODE,
                                    M.WEIGHT,
                                    M.PACK,
	                                M.SALESTAX,
                                    CASE 
                                        WHEN @DC_TYPE IN ('PO','PB','PR') THEN M.PURCHASE_RATE
                                        WHEN @DC_TYPE IN ('SO','SB','SR') THEN M.SALE_RATE
                                        ELSE 0 
                                    END AS SALE_RATE
                                FROM TBL_ROLE R 
                                LEFT OUTER JOIN TBL_ITEMSMASTER M 
                                    ON (R.SHOW_SELECTED = 1 AND M.ITEM_CODE = R.RMENU_ID)
                                    OR (
                                        R.SHOW_SELECTED = 0 
                                        AND M.ITEM_CODE NOT IN (
                                            SELECT DISTINCT RMENU_ID 
                                            FROM TBL_ROLE 
                                            WHERE ROLE_ID = @ROLE_ID 
                                              AND MODULE_ID = 4
                                        )
                                    )
                                WHERE 
                                    R.ROLE_TYPE = @ROLE_TYPE
                                    AND R.MODULE_ID = 4 
                                    AND R.ROLE_ID = @ROLE_ID
                                    AND M.DLT = 'T' 
                                    AND M.ASTATUS = 'Y'
                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                                GROUP BY 
                                    M.ITEM_CODE, 
                                    M.ITEM_ID, 
                                    M.ITEM_NAME, 
                                    M.IPIC, 
                                    M.SALE_RATE,
                                    M.PURCHASE_RATE,
                                    M.HS_CODE,
	                                M.IUNIT_CODE,
                                    M.WEIGHT,
                                    M.PACK,
	                                M.SALESTAX

                                UNION ALL

                                SELECT  
                                    M.ITEM_CODE, 
                                    M.ITEM_NAME,
                                    M.HS_CODE,
	                                M.IUNIT_CODE,
                                    M.WEIGHT,
                                    M.PACK,
	                                M.SALESTAX,
                                    CASE 
                                        WHEN @DC_TYPE IN ('PO','PB','PR') THEN M.PURCHASE_RATE
                                        WHEN @DC_TYPE IN ('SO','SB','SR') THEN M.SALE_RATE
                                        ELSE 0 
                                    END AS SALE_RATE
                                FROM TBL_ITEMSMASTER M 
                                WHERE 
                                    @ROLE_TYPE = 'A'
                                    AND M.DLT = 'T' 
                                    AND M.ASTATUS = 'Y'
                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )

                                UNION ALL

                                SELECT  
                                    M.ITEM_CODE, 
                                    M.ITEM_NAME,
                                    M.HS_CODE,
	                                M.IUNIT_CODE,
                                    M.WEIGHT,
                                    M.PACK,
	                                M.SALESTAX,
                                    CASE 
                                        WHEN @DC_TYPE IN ('PO','PB','PR') THEN M.PURCHASE_RATE
                                        WHEN @DC_TYPE IN ('SO','SB','SR') THEN M.SALE_RATE
                                        ELSE 0 
                                    END AS SALE_RATE
                                FROM TBL_ITEMSMASTER M 
                                WHERE 
                                    @ROLE_TYPE = 'U'
                                    AND M.DLT = 'T' 
                                    AND M.ASTATUS = 'Y'
                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                                    AND (
                                        SELECT COUNT(R.MODULE_ID)
                                        FROM TBL_ROLE R 
                                        LEFT OUTER JOIN TBL_ITEMSMASTER MI 
                                            ON R.RMENU_ID = MI.ITEM_CODE
                                        WHERE 
                                            R.ROLE_TYPE = @ROLE_TYPE
                                            AND R.MODULE_ID = 4 
                                            AND R.ROLE_ID = @ROLE_ID
                                            AND MI.DLT = 'T' 
                                            AND MI.ASTATUS = 'Y'
                                            AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                                    ) < 1;";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = (reader["ITEM_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["ITEM_CODE"]);
                                string name = (reader["ITEM_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader["ITEM_NAME"]);
                                string hscode = (reader["HS_CODE"] == DBNull.Value) ? "" : Convert.ToString(reader["HS_CODE"]);
                                int? unit = new int?((reader["IUNIT_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["IUNIT_CODE"]));
                                double tax = (reader["SALESTAX"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["SALESTAX"]);
                                double rate = (reader["SALE_RATE"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["SALE_RATE"]);
                                double weight = (reader["WEIGHT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["WEIGHT"]);
                                string pack = (reader["PACK"] == DBNull.Value) ? "" : Convert.ToString(reader["PACK"]);
                                dropdown.Add(new
                                {
                                    key = code,
                                    value = name,
                                    rate = rate,
                                    hscode = hscode,
                                    unit = unit,
                                    saleTax = tax,
                                    weight = weight,
                                    pack = pack
                                });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> ItemsBehalfOnParty(int? roleId, string roleType, string dcType, string itemType, string pCode, string aCode)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {

                string query = $@"
                                    DECLARE @ROLE_TYPE NVARCHAR(10) = '{roleType}';
                                    DECLARE @ROLE_ID   INT        = {roleId};
                                    DECLARE @DC_TYPE   NVARCHAR(5)= '{dcType}';
                                    DECLARE @ITEM_TYPE NVARCHAR(5)= '{itemType}';
                                    DECLARE @PARTY_CODE NVARCHAR(5)= '{pCode}'; 
                                    DECLARE @ACT_CODE   NVARCHAR(5)= '{aCode}';

                                    SELECT 
                                    M.ITEM_CODE, 
                                    M.ITEM_NAME,
                                    M.GROUP_CODE,
                                    M.HS_CODE,
                                    M.IUNIT_CODE,
                                    M.COLOR,
									M.SIZE,
                                    M.WEIGHT,
                                    M.PACK,
                                    M.SALESTAX,
                                    CASE 
                                    WHEN @DC_TYPE IN ('PO','PB','PR') THEN M.PURCHASE_RATE
                                    WHEN @DC_TYPE IN ('SO','SB','SR') THEN 
                                    ISNULL(PM.RATE,M.SALE_RATE)
                                    ELSE 0 
                                    END AS SALE_RATE,ISNULL(PM.PARTY_CODE,0) AS PARTY_CODE,ISNULL(PM.ACT_CODE,0) AS ACT_CODE,ISNULL(PM.DISC,0) AS DISC,ISNULL(PM.ADV,0) AS ADV
                                    FROM TBL_ROLE R 
                                    LEFT OUTER JOIN TBL_ITEMSMASTER M 
                                    ON (R.SHOW_SELECTED = 1 AND M.ITEM_CODE = R.RMENU_ID)
                                    OR (
                                    R.SHOW_SELECTED = 0 
                                    AND M.ITEM_CODE NOT IN (
                                    SELECT DISTINCT RMENU_ID 
                                    FROM TBL_ROLE 
                                    WHERE ROLE_ID = @ROLE_ID 
                                    AND MODULE_ID = 4
                                    )
                                    )
                                    LEFT OUTER JOIN TBL_PARTY_MAPPING PM
                                    ON PM.ITEM_CODE = M.ITEM_CODE AND PM.PARTY_CODE = @PARTY_CODE AND PM.ACT_CODE = @ACT_CODE AND PM.DLT = 'T'
                                    WHERE 
                                    R.ROLE_TYPE = @ROLE_TYPE
                                    AND R.MODULE_ID = 4 
                                    AND R.ROLE_ID = @ROLE_ID
                                    AND M.DLT = 'T' 
                                    AND M.ASTATUS = 'Y'
                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                                    GROUP BY 
                                    M.ITEM_CODE, 
                                    M.ITEM_ID, 
                                    M.ITEM_NAME, 
                                    M.GROUP_CODE,
                                    M.IPIC, 
                                    M.SALE_RATE,
                                    M.PURCHASE_RATE,
                                    M.HS_CODE,
                                    M.IUNIT_CODE,
                                    M.COLOR,
									M.SIZE,
                                    M.WEIGHT,
                                    M.PACK,
                                    M.SALESTAX,PM.PARTY_CODE,PM.ACT_CODE,PM.RATE,PM.DISC,PM.ADV

                                    UNION ALL

                                    SELECT  
                                    M.ITEM_CODE, 
                                    M.ITEM_NAME,
                                    M.GROUP_CODE,
                                    M.HS_CODE,
                                    M.IUNIT_CODE,
                                    M.COLOR,
									M.SIZE,
                                    M.WEIGHT,
                                    M.PACK,
                                    M.SALESTAX,
                                    CASE 
                                    WHEN @DC_TYPE IN ('PO','PB','PR') THEN M.PURCHASE_RATE
                                    WHEN @DC_TYPE IN ('SO','SB','SR') THEN ISNULL(PM.RATE,M.SALE_RATE)
                                    ELSE 0 
                                    END AS SALE_RATE,ISNULL(PM.PARTY_CODE,0) AS PARTY_CODE,ISNULL(PM.ACT_CODE,0) AS ACT_CODE,ISNULL(PM.DISC,0) AS DISC,ISNULL(PM.ADV,0) AS ADV
                                    FROM TBL_ITEMSMASTER M 
                                    LEFT OUTER JOIN TBL_PARTY_MAPPING PM
                                    ON PM.ITEM_CODE = M.ITEM_CODE AND PM.PARTY_CODE = @PARTY_CODE AND PM.ACT_CODE = @ACT_CODE AND PM.DLT = 'T'
                                    WHERE 
                                    @ROLE_TYPE = 'A'
                                    AND M.DLT = 'T' 
                                    AND M.ASTATUS = 'Y'
                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )

                                    UNION ALL
                                    SELECT  
                                    M.ITEM_CODE, 
                                    M.ITEM_NAME,
                                    M.GROUP_CODE,
                                    M.HS_CODE,
                                    M.IUNIT_CODE,
                                    M.COLOR,
									M.SIZE,
                                    M.WEIGHT,
                                    M.PACK,
                                    M.SALESTAX,
                                    CASE 
                                    WHEN @DC_TYPE IN ('PO','PB','PR') THEN M.PURCHASE_RATE
                                    WHEN @DC_TYPE IN ('SO','SB','SR') THEN ISNULL(PM.RATE,M.SALE_RATE)
                                    ELSE 0 
                                    END AS SALE_RATE,ISNULL(PM.PARTY_CODE,0) AS PARTY_CODE,ISNULL(PM.ACT_CODE,0) AS ACT_CODE,ISNULL(PM.DISC,0) AS DISC,ISNULL(PM.ADV,0) AS ADV
                                    FROM TBL_ITEMSMASTER M 
                                    LEFT OUTER JOIN TBL_PARTY_MAPPING PM
                                    ON PM.ITEM_CODE = M.ITEM_CODE AND PM.PARTY_CODE = @PARTY_CODE AND PM.ACT_CODE = @ACT_CODE AND PM.DLT = 'T'
                                    WHERE 
                                    @ROLE_TYPE = 'U'
                                    AND M.DLT = 'T' 
                                    AND M.ASTATUS = 'Y'
                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                                    AND (
                                    SELECT COUNT(R.MODULE_ID)
                                    FROM TBL_ROLE R 
                                    LEFT OUTER JOIN TBL_ITEMSMASTER MI 
                                    ON R.RMENU_ID = MI.ITEM_CODE
                                    WHERE 
                                    R.ROLE_TYPE = @ROLE_TYPE
                                    AND R.MODULE_ID = 4 
                                    AND R.ROLE_ID = @ROLE_ID
                                    AND MI.DLT = 'T' 
                                    AND MI.ASTATUS = 'Y'
                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )
                                    ) < 1
                                    ORDER BY GROUP_CODE ASC;";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = (reader["ITEM_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["ITEM_CODE"]);
                                string name = (reader["ITEM_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader["ITEM_NAME"]);
                                string hscode = (reader["HS_CODE"] == DBNull.Value) ? "" : Convert.ToString(reader["HS_CODE"]);
                                int? unit = new int?((reader["IUNIT_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["IUNIT_CODE"]));
                                int? color = new int?((reader["COLOR"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["COLOR"]));
                                int? size = new int?((reader["SIZE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["SIZE"]));
                                double weight = (reader["WEIGHT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["WEIGHT"]);
                                string pack = (reader["PACK"] == DBNull.Value) ? "" : Convert.ToString(reader["PACK"]);
                                double tax = (reader["SALESTAX"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["SALESTAX"]);
                                double rate = (reader["SALE_RATE"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["SALE_RATE"]);
                                int? partyCode = new int?((reader["PARTY_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["PARTY_CODE"]));
                                int? accountCode = new int?((reader["ACT_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["ACT_CODE"]));
                                double disc = (reader["DISC"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["DISC"]);
                                string adv = (reader["ADV"] == DBNull.Value) ? "" : Convert.ToString(reader["ADV"]);
                                dropdown.Add(new
                                {
                                    key = code,
                                    value = name,
                                    rate = rate,
                                    hscode = hscode,
                                    unit = unit,
                                    saleTax = tax,
                                    weight = weight,
                                    pack = pack,
                                    partyCode = partyCode,
                                    accountCode = accountCode,
                                    disc = disc,
                                    adv = adv,
                                    size = size,
                                    color = color
                                });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        //public static List<dynamic> ItemsBehalfOnParty(int? roleId, string roleType, string dcType, string itemType, string pCode, string aCode)
        //{
        //    List<object> dropdown = new List<object>();
        //    using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
        //    {
        //        DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7369, 6);
        //        defaultInterpolatedStringHandler.AppendLiteral("\r\n                                    DECLARE @ROLE_TYPE NVARCHAR(10) = '");
        //        defaultInterpolatedStringHandler.AppendFormatted(roleType);
        //        defaultInterpolatedStringHandler.AppendLiteral("';\r\n                                    DECLARE @ROLE_ID   INT        = ");
        //        defaultInterpolatedStringHandler.AppendFormatted<int?>(roleId);
        //        defaultInterpolatedStringHandler.AppendLiteral(";\r\n                                    DECLARE @DC_TYPE   NVARCHAR(5)= '");
        //        defaultInterpolatedStringHandler.AppendFormatted(dcType);
        //        defaultInterpolatedStringHandler.AppendLiteral("';\r\n                                    DECLARE @ITEM_TYPE NVARCHAR(5)= '");
        //        defaultInterpolatedStringHandler.AppendFormatted(itemType);
        //        defaultInterpolatedStringHandler.AppendLiteral("';\r\n                                    DECLARE @PARTY_CODE NVARCHAR(5)= '");
        //        defaultInterpolatedStringHandler.AppendFormatted(pCode);
        //        defaultInterpolatedStringHandler.AppendLiteral("'; \r\n                                    DECLARE @ACT_CODE   NVARCHAR(5)= '");
        //        defaultInterpolatedStringHandler.AppendFormatted(aCode);
        //        defaultInterpolatedStringHandler.AppendLiteral("';\r\n\r\n                                    SELECT \r\n                                    M.ITEM_CODE, \r\n                                    M.ITEM_NAME,\r\n                                    M.GROUP_CODE,\r\n                                    M.HS_CODE,\r\n                                    M.IUNIT_CODE,\r\n                                    M.WEIGHT,\r\n                                    M.PACK,\r\n                                    M.SALESTAX,\r\n                                    CASE \r\n                                    WHEN @DC_TYPE IN ('PO','PB','PR') THEN M.PURCHASE_RATE\r\n                                    WHEN @DC_TYPE IN ('SO','SB','SR') THEN \r\n                                    ISNULL(PM.RATE,M.SALE_RATE)\r\n                                    ELSE 0 \r\n                                    END AS SALE_RATE,ISNULL(PM.PARTY_CODE,0) AS PARTY_CODE,ISNULL(PM.ACT_CODE,0) AS ACT_CODE,ISNULL(PM.DISC,0) AS DISC,ISNULL(PM.ADV,0) AS ADV\r\n                                    FROM TBL_ROLE R \r\n                                    LEFT OUTER JOIN TBL_ITEMSMASTER M \r\n                                    ON (R.SHOW_SELECTED = 1 AND M.ITEM_CODE = R.RMENU_ID)\r\n                                    OR (\r\n                                    R.SHOW_SELECTED = 0 \r\n                                    AND M.ITEM_CODE NOT IN (\r\n                                    SELECT DISTINCT RMENU_ID \r\n                                    FROM TBL_ROLE \r\n                                    WHERE ROLE_ID = @ROLE_ID \r\n                                    AND MODULE_ID = 4\r\n                                    )\r\n                                    )\r\n                                    LEFT OUTER JOIN TBL_PARTY_MAPPING PM\r\n                                    ON PM.ITEM_CODE = M.ITEM_CODE AND PM.PARTY_CODE = @PARTY_CODE AND PM.ACT_CODE = @ACT_CODE AND PM.DLT = 'T'\r\n                                    WHERE \r\n                                    R.ROLE_TYPE = @ROLE_TYPE\r\n                                    AND R.MODULE_ID = 4 \r\n                                    AND R.ROLE_ID = @ROLE_ID\r\n                                    AND M.DLT = 'T' \r\n                                    AND M.ASTATUS = 'Y'\r\n                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )\r\n                                    GROUP BY \r\n                                    M.ITEM_CODE, \r\n                                    M.ITEM_ID, \r\n                                    M.ITEM_NAME, \r\n                                    M.GROUP_CODE,\r\n                                    M.IPIC, \r\n                                    M.SALE_RATE,\r\n                                    M.PURCHASE_RATE,\r\n                                    M.HS_CODE,\r\n                                    M.IUNIT_CODE,\r\n                                    M.WEIGHT,\r\n                                    M.PACK,\r\n                                    M.SALESTAX,PM.PARTY_CODE,PM.ACT_CODE,PM.RATE,PM.DISC,PM.ADV\r\n\r\n                                    UNION ALL\r\n\r\n                                    SELECT  \r\n                                    M.ITEM_CODE, \r\n                                    M.ITEM_NAME,\r\n                                    M.GROUP_CODE,\r\n                                    M.HS_CODE,\r\n                                    M.IUNIT_CODE,\r\n                                    M.WEIGHT,\r\n                                    M.PACK,\r\n                                    M.SALESTAX,\r\n                                    CASE \r\n                                    WHEN @DC_TYPE IN ('PO','PB','PR') THEN M.PURCHASE_RATE\r\n                                    WHEN @DC_TYPE IN ('SO','SB','SR') THEN ISNULL(PM.RATE,M.SALE_RATE)\r\n                                    ELSE 0 \r\n                                    END AS SALE_RATE,ISNULL(PM.PARTY_CODE,0) AS PARTY_CODE,ISNULL(PM.ACT_CODE,0) AS ACT_CODE,ISNULL(PM.DISC,0) AS DISC,ISNULL(PM.ADV,0) AS ADV\r\n                                    FROM TBL_ITEMSMASTER M \r\n                                    LEFT OUTER JOIN TBL_PARTY_MAPPING PM\r\n                                    ON PM.ITEM_CODE = M.ITEM_CODE AND PM.PARTY_CODE = @PARTY_CODE AND PM.ACT_CODE = @ACT_CODE AND PM.DLT = 'T'\r\n                                    WHERE \r\n                                    @ROLE_TYPE = 'A'\r\n                                    AND M.DLT = 'T' \r\n                                    AND M.ASTATUS = 'Y'\r\n                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )\r\n\r\n                                    UNION ALL\r\n                                    SELECT  \r\n                                    M.ITEM_CODE, \r\n                                    M.ITEM_NAME,\r\n                                    M.GROUP_CODE,\r\n                                    M.HS_CODE,\r\n                                    M.IUNIT_CODE,\r\n                                    M.WEIGHT,\r\n                                    M.PACK,\r\n                                    M.SALESTAX,\r\n                                    CASE \r\n                                    WHEN @DC_TYPE IN ('PO','PB','PR') THEN M.PURCHASE_RATE\r\n                                    WHEN @DC_TYPE IN ('SO','SB','SR') THEN ISNULL(PM.RATE,M.SALE_RATE)\r\n                                    ELSE 0 \r\n                                    END AS SALE_RATE,ISNULL(PM.PARTY_CODE,0) AS PARTY_CODE,ISNULL(PM.ACT_CODE,0) AS ACT_CODE,ISNULL(PM.DISC,0) AS DISC,ISNULL(PM.ADV,0) AS ADV\r\n                                    FROM TBL_ITEMSMASTER M \r\n                                    LEFT OUTER JOIN TBL_PARTY_MAPPING PM\r\n                                    ON PM.ITEM_CODE = M.ITEM_CODE AND PM.PARTY_CODE = @PARTY_CODE AND PM.ACT_CODE = @ACT_CODE AND PM.DLT = 'T'\r\n                                    WHERE \r\n                                    @ROLE_TYPE = 'U'\r\n                                    AND M.DLT = 'T' \r\n                                    AND M.ASTATUS = 'Y'\r\n                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )\r\n                                    AND (\r\n                                    SELECT COUNT(R.MODULE_ID)\r\n                                    FROM TBL_ROLE R \r\n                                    LEFT OUTER JOIN TBL_ITEMSMASTER MI \r\n                                    ON R.RMENU_ID = MI.ITEM_CODE\r\n                                    WHERE \r\n                                    R.ROLE_TYPE = @ROLE_TYPE\r\n                                    AND R.MODULE_ID = 4 \r\n                                    AND R.ROLE_ID = @ROLE_ID\r\n                                    AND MI.DLT = 'T' \r\n                                    AND MI.ASTATUS = 'Y'\r\n                                    AND ( @ITEM_TYPE IS NULL OR @ITEM_TYPE = '' OR M.ITEM_TYPE = @ITEM_TYPE )\r\n                                    ) < 1\r\n                                    ORDER BY GROUP_CODE ASC;");
        //        using (SqlCommand command = new SqlCommand(defaultInterpolatedStringHandler.ToStringAndClear(), conn))
        //        {
        //            conn.Open();
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        int code = (reader["ITEM_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["ITEM_CODE"]);
        //                        string name = (reader["ITEM_NAME"] == DBNull.Value) ? "" : Convert.ToString(reader["ITEM_NAME"]);
        //                        string hscode = (reader["HS_CODE"] == DBNull.Value) ? "" : Convert.ToString(reader["HS_CODE"]);
        //                        int? unit = new int?((reader["IUNIT_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["IUNIT_CODE"]));
        //                        double weight = (reader["WEIGHT"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["WEIGHT"]);
        //                        string pack = (reader["PACK"] == DBNull.Value) ? "" : Convert.ToString(reader["PACK"]);
        //                        double tax = (reader["SALESTAX"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["SALESTAX"]);
        //                        double rate = (reader["SALE_RATE"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["SALE_RATE"]);
        //                        int? partyCode = new int?((reader["PARTY_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["PARTY_CODE"]));
        //                        int? accountCode = new int?((reader["ACT_CODE"] == DBNull.Value) ? 0 : Convert.ToInt32(reader["ACT_CODE"]));
        //                        double disc = (reader["DISC"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["DISC"]);
        //                        double adv = (reader["ADV"] == DBNull.Value) ? 0.0 : Convert.ToDouble(reader["ADV"]);
        //                        dropdown.Add(new
        //                        {
        //                            key = code,
        //                            value = name,
        //                            rate = rate,
        //                            hscode = hscode,
        //                            unit = unit,
        //                            saleTax = tax,
        //                            weight = weight,
        //                            pack = pack,
        //                            partyCode = partyCode,
        //                            accountCode = accountCode,
        //                            disc = disc,
        //                            adv = adv
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return dropdown;
        //}

        public static List<KeyValuePair<int, string>> GetLeaveType()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT GROUP_CODE , GROUP_NAME FROM TBL_LEAVE_TYPE WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> ItemMasterDropdownWithUnits()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT ITEM_CODE,ITEM_NAME,IUNIT_CODE,PACK,HS_CODE,PURCHASE_RATE AS RATE FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int key = Convert.ToInt32(reader["ITEM_CODE"]);
                                string? name = Convert.ToString(reader["ITEM_NAME"]);
                                string? unitcode = Convert.ToString(reader["IUNIT_CODE"]);
                                string? rate = Convert.ToString(reader["RATE"]);
                                string? hscode = Convert.ToString(reader["HS_CODE"]);
                                string? pack = Convert.ToString(reader["PACK"]);

                                dropdown.Add(new { key = key, value = name, unit = unitcode, rate = rate, hscode = hscode, pack = pack });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> ItemMasterWithStkProc(string fromDate, string toDate, string Branch, string Period)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                //string query = "SELECT ITEM_CODE,ITEM_NAME,IUNIT_CODE,PACK,HS_CODE,PURCHASE_RATE AS RATE FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y'";
                string query = $@"EXEC STKPROC 142,'{fromDate}','{toDate}','{Branch}',{Period},'',''";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int key = Convert.ToInt32(reader["ITEM_CODE"]);
                                string? name = Convert.ToString(reader["ITEM_NAME"]);
                                string? unitcode = Convert.ToString(reader["IUNIT_CODE"]);
                                string? rate = Convert.ToString(reader["RATE"]);
                                string? hscode = Convert.ToString(reader["HS_CODE"]);
                                string? pack = Convert.ToString(reader["PACK"]);
                                double? balance = Convert.ToDouble(reader["BALANCE"]);

                                dropdown.Add(new { key = key, value = name, unit = unitcode, rate = rate, hscode = hscode, pack = pack, balance = balance });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> ItemBarcodeDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE,BARCODE FROM TBL_BARCODE WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> WareHouseDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE,DESCR FROM TBL_WAREHOUSE WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> WareHouseDropdownWthSubWithGr()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @$"SELECT A.CODE, A.DESCR,
                                CASE WHEN  C.DESCR IS NULL THEN '' ELSE C.DESCR END AS CONTROL_NAME ,A.GR_CODE 
                                FROM TBL_WAREHOUSE A 
                                LEFT OUTER JOIN TBL_WAREHOUSE B 
                                ON A.GR_CODE LIKE CONCAT('', B.GR_CODE, '%') AND B.ASTATUS <> 'Y' 
                                LEFT OUTER JOIN TBL_WAREHOUSE C ON C.CODE = A.PARENT_CODE 
                                WHERE A.DLT = 'T' AND B.ASTATUS IS NULL AND A.GROUP_TYPE = 'S'
                                GROUP BY A.CODE, A.DESCR,A.ASTATUS,A.GR_CODE,B.ASTATUS,C.DESCR";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["DESCR"]);
                                string? controlName = Convert.ToString(reader["CONTROL_NAME"]);
                                string? grcode = Convert.ToString(reader["GR_CODE"]);

                                dropdown.Add(new { key = code, value = name, name = controlName, grcode = grcode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> SubWareHouseDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE,DESCR FROM TBL_WAREHOUSE WHERE DLT = 'T' AND ASTATUS = 'Y' AND GROUP_TYPE = 'S'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> ExpenseType(Common common)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                //string query = "Select * from TBL_CHART where ACT_NATURE IN(25,27) AND ASTATUS = 'Y' AND DLT = 'T' AND ACT_TYPE = 'S'";
                string query = @$"SELECT 
                                      CH.ACT_CODE,CH.ACT_NAME 
                                    FROM TBL_ROLE R 
                                    LEFT OUTER JOIN TBL_CHART CH

                                       ON (

                                          (R.SHOW_SELECTED = 1 AND CH.ACT_CODE = R.RMENU_ID)
                                          OR 
                                          (R.SHOW_SELECTED = 0 AND 
                                           CH.ACT_CODE NOT IN 
                                             (SELECT DISTINCT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = {common.RoleID} AND MODULE_ID = 3) 
      
                                          )
                                      ) 
                                      LEFT OUTER JOIN TBL_CHART PCH
                                    ON PCH.ACT_CODE = CH.ACT_PARENT_CODE
                                    WHERE R.ROLE_TYPE = '{common.RoleType}'
                                      AND R.MODULE_ID = 3 
                                      AND CH.DLT = 'T' 
                                      AND CH.ASTATUS = 'Y'
                                      AND R.ROLE_ID = {common.RoleID}
                                      AND CH.ACT_NATURE IN(25,27) AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'

                                    GROUP BY CH.ACT_CODE,CH.ACT_NAME 


                                      UNION ALL

                                      SELECT     CH.ACT_CODE,CH.ACT_NAME 
                                      FROM TBL_CHART CH
                                      LEFT OUTER JOIN TBL_CHART PCH
                                    ON PCH.ACT_CODE = CH.ACT_PARENT_CODE

                                    WHERE 

                                   '{common.RoleType}' = 'A'  
                                    AND CH.ACT_NATURE IN(25,27) AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'


                                    UNION ALL
                                            SELECT CH.ACT_CODE,CH.ACT_NAME 
		                                    FROM TBL_CHART CH 
                                      LEFT OUTER JOIN TBL_CHART PCH
                                    ON PCH.ACT_CODE = CH.ACT_PARENT_CODE
		                                    WHERE 
  
                                    '{common.RoleType}'

                                      = 'U'  
                                    AND CH.ACT_NATURE IN(25,27) AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'
   
                                          AND CH.DLT = 'T' 
                                          AND CH.ASTATUS = 'Y' AND  
                                           (SELECT 
                                     COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_CHART M ON R.RMENU_ID = M.ACT_CODE 
                                     WHERE R.ROLE_TYPE = 
                                    '{common.RoleType}'
   
                                          AND R.MODULE_ID = 3 

                                          AND M.DLT = 'T' 
                                          AND M.ASTATUS = 'Y'
                                          AND R.ROLE_ID =
	  
                                      {common.RoleID}
                                        AND CH.ACT_NATURE IN(25,27) AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'
                                          ) < 1 ";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> GetCardType()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "Select GROUP_CODE , GROUP_NAME From TBL_CARD_TYPE where DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int key = reader.GetInt32(0);
                                string value = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(key, value));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> GetDueDate(int? Partycode, int? Actcode)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                if (Partycode != null && Actcode != null)
                {
                    string query = $"SELECT CONVERT(DATETIME,FORMAT(GETDATE(), 'yyyy') + '-' + FORMAT(GETDATE(), 'MM') + '-' + CAST(IIF(DAY(DUE_DATE) > DAY(EOMONTH(GETDATE())), DAY(EOMONTH(GETDATE())), DAY(DUE_DATE)) AS VARCHAR) ) AS DUE_DATE1 FROM TBL_POS_MASTER WHERE PARTY_CODE = {Partycode} AND ACT_CODE = {Actcode} AND DLT = 'T' AND BILL_STATUS = 'P' AND DUE_DATE = (SELECT MIN(DUE_DATE) FROM TBL_POS_MASTER WHERE PARTY_CODE = {Partycode} AND ACT_CODE = {Actcode} AND DLT = 'T' AND BILL_STATUS = 'P')";
                    using (SqlCommand command = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    // Assuming the single column is a date and converting it to string
                                    string date = reader.GetDateTime(0).ToString("dd-MMM-yyyy");
                                    dropdown.Add(new KeyValuePair<int, string>(0, date)); // Using 0 as the key since there's no other column
                                }
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> WareHouseDropdownForPurchaseOrder()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE,DESCR FROM TBL_WAREHOUSE WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string name = Convert.ToString(reader["DESCR"]);
                                dropdown.Add(new { key = code, value = name });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> WareHouseDropdownWthControlName()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.CODE, A.DESCR, B.DESCR AS CONTROL_NAME FROM TBL_WAREHOUSE A " +
                               "LEFT JOIN TBL_WAREHOUSE B ON A.PARENT_CODE = B.CODE " +
                               "WHERE A.DLT =  'T' AND A.ASTATUS = 'Y' ";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["DESCR"]);
                                string? controlName = Convert.ToString(reader["CONTROL_NAME"]);
                                dropdown.Add(new { key = code, value = name, name = controlName });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> WareHouseDropdownWthControlNameWithGr()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @$"SELECT A.CODE, A.DESCR,
                                CASE WHEN  C.DESCR IS NULL THEN '' ELSE C.DESCR END AS CONTROL_NAME ,A.GR_CODE 
                                FROM TBL_WAREHOUSE A 
                                LEFT OUTER JOIN TBL_WAREHOUSE B 
                                ON A.GR_CODE LIKE CONCAT('', B.GR_CODE, '%') AND B.ASTATUS <> 'Y' 
                                LEFT OUTER JOIN TBL_WAREHOUSE C ON C.CODE = A.PARENT_CODE 
                                WHERE A.DLT = 'T' AND B.ASTATUS IS NULL 
                                GROUP BY A.CODE, A.DESCR,A.ASTATUS,A.GR_CODE,B.ASTATUS,C.DESCR";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["DESCR"]);
                                string? controlName = Convert.ToString(reader["CONTROL_NAME"]);
                                //int GRcode = Convert.ToInt32(reader["GR_CODE"]);
                                string? GRcode = Convert.ToString(reader["GR_CODE"]);

                                dropdown.Add(new { key = code, value = name, name = controlName, grcode = GRcode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> WareHouseDropdownWthControlNameSub()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @$"SELECT A.CODE, A.DESCR,
                                CASE WHEN  C.DESCR IS NULL THEN '' ELSE C.DESCR END AS CONTROL_NAME ,A.GR_CODE 
                                FROM TBL_WAREHOUSE A 
                                LEFT OUTER JOIN TBL_WAREHOUSE B 
                                ON A.GR_CODE LIKE CONCAT('', B.GR_CODE, '%') AND B.ASTATUS <> 'Y' 
                                LEFT OUTER JOIN TBL_WAREHOUSE C ON C.CODE = A.PARENT_CODE 
                                WHERE A.DLT = 'T' AND A.GROUP_TYPE = 'S' AND B.ASTATUS IS NULL 
                                GROUP BY A.CODE, A.DESCR,A.ASTATUS,A.GR_CODE,B.ASTATUS,C.DESCR";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["DESCR"]);
                                string? controlName = Convert.ToString(reader["CONTROL_NAME"]);
                                //int GRcode = Convert.ToInt32(reader["GR_CODE"]);
                                string? GRcode = Convert.ToString(reader["GR_CODE"]);

                                dropdown.Add(new { key = code, value = name, name = controlName, grcode = GRcode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> GetSalesmanCommision()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT COMM , PARTY_CODE , ACT_CODE FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                decimal? comm = reader["COMM"] != DBNull.Value ? Convert.ToDecimal(reader["COMM"]) : 0;
                                int? partycode = reader["PARTY_CODE"] != DBNull.Value ? Convert.ToInt32(reader["PARTY_CODE"]) : 0;
                                int? actcode = reader["ACT_CODE"] != DBNull.Value ? Convert.ToInt32(reader["ACT_CODE"]) : 0;
                                dropdown.Add(new { comm = comm, partycode = partycode, actcode = actcode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> UsersForPOS()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @"SELECT distinct PM.TRAN_ID , PD.ITEM_CODE , PD.QTY, tc.CNAME, tc.CMOB , tc.CADD , BILL_STATUS , COMPLETE , Total , Net_Total , PD.RATE FROM TBL_POS_CUS tc
                                Left Join TBL_POS_DETAIL PD on PD.TRAN_ID = tc.TRAN_ID
                                Left JOIN TBL_POS_MASTER PM On tc.TRAN_ID = PM.TRAN_ID WHERE PM.DLT = 'T' AND PD.DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string? name = Convert.ToString(reader["CNAME"]);
                                string? number = Convert.ToString(reader["CMOB"]);
                                string? address = Convert.ToString(reader["CADD"]);
                                decimal? TranId = reader["TRAN_ID"] != DBNull.Value ? Convert.ToDecimal(reader["TRAN_ID"]) : 0;
                                string? billStatus = Convert.ToString(reader["BILL_STATUS"]);
                                string? Complete = Convert.ToString(reader["COMPLETE"]);
                                decimal? Total = reader["Total"] != DBNull.Value ? Convert.ToDecimal(reader["Total"]) : 0;
                                decimal? NetTotal = reader["Net_Total"] != DBNull.Value ? Convert.ToDecimal(reader["Net_Total"]) : 0;
                                decimal? ItemCode = reader["ITEM_CODE"] != DBNull.Value ? Convert.ToDecimal(reader["ITEM_CODE"]) : 0;
                                decimal? Qty = reader["QTY"] != DBNull.Value ? Convert.ToDecimal(reader["QTY"]) : 0;
                                decimal? Rate = reader["RATE"] != DBNull.Value ? Convert.ToDecimal(reader["RATE"]) : 0;
                                dropdown.Add(new { name = name, number = number, address = address, TranId = TranId, billStatus = billStatus, Complete = Complete, Total = Total, NetTotal = NetTotal, ItemCode = ItemCode, Qty = Qty, Rate = Rate });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> GetPOSCustomerName()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @"SELECT c.CMOB, c.TRAN_ID, c.CNAME
                                FROM TBL_POS_CUS c
                                INNER JOIN (
                                    SELECT CMOB, MAX(TRAN_ID) AS MaxTranId
                                    FROM TBL_POS_CUS
                                    GROUP BY CMOB
                                ) x ON c.CMOB = x.CMOB AND c.TRAN_ID = x.MaxTranId
                                ORDER BY c.TRAN_ID DESC;";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string? name = Convert.ToString(reader["CNAME"]);
                                string? number = Convert.ToString(reader["CMOB"]);
                                dropdown.Add(new { name = name, number = number });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> CardDiscRecords(string CardNum)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"Select Tran_Id , Disc , POINT_START_VALUE , POINT_RATE,MAX_POINTS_DISC, FIRST_NAME, LAST_NAME, PHONE_NO From TBL_POS_MS Where CARD_NO = '{CardNum}' AND DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string? id = Convert.ToString(reader["Tran_Id"]);
                                string? Disc = Convert.ToString(reader["Disc"]);
                                string? startValue = Convert.ToString(reader["POINT_START_VALUE"]);
                                string? pointRate = Convert.ToString(reader["POINT_RATE"]);
                                string? maxPoint = Convert.ToString(reader["MAX_POINTS_DISC"]);
                                string? fisrtName = Convert.ToString(reader["FIRST_NAME"]);
                                string? lastName = Convert.ToString(reader["LAST_NAME"]);
                                string? phoneNo = Convert.ToString(reader["PHONE_NO"]);
                                dropdown.Add(new { id = id, Disc = Disc, startValue = startValue, pointRate = pointRate, maxPoint = maxPoint, fisrtName = fisrtName, lastName = lastName, phoneNo = phoneNo });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> POSUserRights(string UserName)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT CODE, GROUP_NAME, RATE, D_DISC, M_DISC, B_RETURN, I_RETURN, SETT_F, SETT_T, SERVICE_CHARGES, CARD_DISCOUNT, EXPENSES, COMM, USERNAME FROM TBL_POS_USER WHERE USERNAME = '{UserName}' AND DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string? code = Convert.ToString(reader["CODE"]);
                                string? groupName = Convert.ToString(reader["GROUP_NAME"]);
                                int? rate = Convert.ToInt32(reader["RATE"]);
                                int? dDisc = Convert.ToInt32(reader["D_DISC"]);
                                int? mDisc = Convert.ToInt32(reader["M_DISC"]);
                                int? bReturn = Convert.ToInt32(reader["B_RETURN"]);
                                int? iReturn = Convert.ToInt32(reader["I_RETURN"]);
                                int? settF = Convert.ToInt32(reader["SETT_F"]);
                                int? settT = Convert.ToInt32(reader["SETT_T"]);
                                int? serviceCharges = Convert.ToInt32(reader["SERVICE_CHARGES"]);
                                int? cardDisc = Convert.ToInt32(reader["CARD_DISCOUNT"]);
                                int? exp = Convert.ToInt32(reader["EXPENSES"]);
                                int? comm = Convert.ToInt32(reader["COMM"]);
                                string? userName = Convert.ToString(reader["USERNAME"]);
                                dropdown.Add(new
                                {
                                    code = code,
                                    groupName = groupName,
                                    rate = rate,
                                    dDisc = dDisc,
                                    mDisc = mDisc,
                                    bReturn = bReturn,
                                    iReturn = iReturn,
                                    settF = settF,
                                    settT = settT,
                                    serviceCharges = serviceCharges,
                                    cardDisc = cardDisc,
                                    exp = exp,
                                    comm = comm,
                                    userName = userName
                                });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> BarcodesWithRateAndUnits()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.ITEM_CODE, A.BARCODE, A.ITEM_CODE , A.PURCHASE_RATE FROM TBL_ITEMSMASTER A " +
                               "WHERE A.DLT =  'T' AND A.ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int? key = Convert.ToInt32(reader["ITEM_CODE"]);
                                int? itemcode = Convert.ToInt32(reader["ITEM_CODE"]);
                                object barcode = null;

                                if (reader["BARCODE"] != DBNull.Value)
                                {
                                    string barcodeStr = reader["BARCODE"].ToString();

                                    if (int.TryParse(barcodeStr, out int barcodeInt))
                                    {
                                        barcode = barcodeInt;
                                    }
                                    else
                                    {
                                        barcode = barcodeStr;
                                    }
                                }
                                dropdown.Add(new { key = key, barcode = barcode, itemcode = itemcode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<CustomKeyValuPair> BarcodesKeyAndValue()
        {
            List<CustomKeyValuPair> dropdown = new List<CustomKeyValuPair>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.ITEM_CODE AS CODE, A.BARCODE, A.ITEM_CODE , A.PURCHASE_RATE FROM TBL_ITEMSMASTER A " +
                               "WHERE A.DLT =  'T' AND A.ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var row = new CustomKeyValuPair
                                {
                                    key = Convert.ToInt32(reader["CODE"]),
                                    value = Convert.ToString(reader["BARCODE"])
                                };

                                dropdown.Add(row);
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> MapTable(string? BCODE)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"Select CASH_ACT from TBL_POS_MAP where DLT = 'T' And ASTATUS = 'Y' And BCODE = {BCODE}";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int? CashAct = reader["CASH_ACT"] != DBNull.Value ? Convert.ToInt32(reader["CASH_ACT"]) : 0;
                                dropdown.Add(new { CashAct = CashAct, Payact = 0 });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> DeleteRow()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"Select ID.TRAN_ID , ITEM_CODE , IM.VOUCHER_NO , IM.BILL_STATUS from TBL_POS_DETAIL ID LEFT JOIN TBL_POS_MASTER IM ON ID.TRAN_ID = IM.TRAN_ID WHERE IM.DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int? tranId = Convert.ToInt32(reader["TRAN_ID"]);
                                int? itemcode = Convert.ToInt32(reader["ITEM_CODE"]);
                                string? voucher = Convert.ToString(reader["VOUCHER_NO"]);
                                string? bill = Convert.ToString(reader["BILL_STATUS"]);
                                dropdown.Add(new { tranId = tranId, itemcode = itemcode, voucher = voucher, bill = bill });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> ReturnData()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"Select ID.TRAN_ID , ITEM_CODE , IM.VOUCHER_NO , IM.BILL_STATUS from TBL_POS_DETAIL ID LEFT JOIN TBL_POS_MASTER IM ON ID.TRAN_ID = IM.TRAN_ID WHERE IM.DLT = 'T' And IM.COMPLETE = 1";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int? tranId = Convert.ToInt32(reader["TRAN_ID"]);
                                int? itemcode = Convert.ToInt32(reader["ITEM_CODE"]);
                                string? voucher = Convert.ToString(reader["VOUCHER_NO"]);
                                string? bill = Convert.ToString(reader["BILL_STATUS"]);
                                dropdown.Add(new { tranId = tranId, itemcode = itemcode, voucher = voucher, bill = bill });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> AdvanceRow()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"Select ID.TRAN_ID , ITEM_CODE , IM.VOUCHER_NO , IM.BILL_STATUS , IM.COMPLETE from TBL_POS_DETAIL ID LEFT JOIN TBL_POS_MASTER IM ON ID.TRAN_ID = IM.TRAN_ID WHERE IM.DLT = 'T' And BILL_STATUS = 'P' And IM.COMPLETE = 0 ";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int? tranId = Convert.ToInt32(reader["TRAN_ID"]);
                                int? itemcode = Convert.ToInt32(reader["ITEM_CODE"]);
                                string? voucher = Convert.ToString(reader["VOUCHER_NO"]);
                                string? bill = Convert.ToString(reader["BILL_STATUS"]);
                                int? complete = Convert.ToInt32(reader["COMPLETE"]);
                                dropdown.Add(new { tranId = tranId, itemcode = itemcode, voucher = voucher, bill = bill, complete = complete });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> RoleModuleDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_ROLE_MODULE WHERE DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> RoleDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT ROLE_ID,ROLE_NAME FROM TBL_ROLE WHERE DLT = 'T' AND ASTATUS = 'Y' GROUP BY ROLE_ID,ROLE_NAME";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> RoleDropdownByType(string type)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT ROLE_ID,ROLE_NAME FROM TBL_ROLE WHERE DLT = 'T' AND ASTATUS = 'Y' AND ROLE_TYPE = '" + type + "' GROUP BY ROLE_ID,ROLE_NAME";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> BarcodeDropdown(int itemCode)
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE,BARCODE FROM TBL_BARCODE WHERE ITEM_CODE = '" + itemCode + "' AND DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["BARCODE"]);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> DepartmentDropdownWithControlName()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.CODE,A.DESCR, CASE WHEN C.DESCR IS NULL THEN '' ELSE C.DESCR END AS CONTROL_NAME" +
                               " FROM TBL_ACT_GROUP A" +
                               " LEFT OUTER JOIN TBL_ACT_GROUP B ON A.GR_CODE LIKE CONCAT('', B.GR_CODE, '%') AND B.ASTATUS <> 'Y'" +
                               " LEFT OUTER JOIN TBL_ACT_GROUP C ON C.CODE = A.PARENT_CODE" +
                               " WHERE A.DLT = 'T' AND A.GROUP_TYPE = 'S'  AND B.ASTATUS IS NULL" +
                               " GROUP BY A.CODE, A.DESCR,A.ASTATUS,A.GR_CODE,B.ASTATUS,C.DESCR";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["DESCR"]);
                                string? controlName = Convert.ToString(reader["CONTROL_NAME"]);
                                dropdown.Add(new { key = code, value = name, name = controlName });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> OnlyBarcodeDropdown()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE, BARCODE, SRATE FROM TBL_BARCODE WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["BARCODE"]);
                                int? wsale = Convert.ToInt32(reader["SRATE"]);
                                dropdown.Add(new { key = code, value = name, wsale = wsale });
                                //dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> CustomBarcodeDropdownForStockTransfer()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.CODE,A.BARCODE,A.BLABEL,B.ITEM_NAME AS ITEM_ID, A.SRATE, A.COLOR, A.SIZE FROM TBL_BARCODE A " +
                               "INNER JOIN TBL_ITEMSMASTER B ON A.ITEM_CODE = B.ITEM_CODE " +
                               "WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' AND B.DLT = 'T' AND B.ASTATUS = 'Y'";


                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["BARCODE"]);
                                string? blabel = Convert.ToString(reader["BLABEL"]);
                                string? itemID = Convert.ToString(reader["ITEM_ID"]);
                                int? wsale = Convert.ToInt32(reader["SRATE"]);
                                int? color = Convert.ToInt32(reader["COLOR"]);
                                int? size = Convert.ToInt32(reader["SIZE"]);
                                dropdown.Add(new { key = code, value = name, blabel = blabel, itemID = itemID, wsale = wsale, color = color, size = size });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<dynamic> CustomBarcodeDropdownWithPurchaseRate(string? dcType)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                //string query = "SELECT A.CODE,A.BARCODE,A.BLABEL,B.ITEM_NAME AS ITEM_ID, A.SRATE, A.COLOR, A.SIZE FROM TBL_BARCODE A " +
                //               "INNER JOIN TBL_ITEMSMASTER B ON A.ITEM_CODE = B.ITEM_CODE " +
                //               "WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' AND B.DLT = 'T' AND B.ASTATUS = 'Y'";
                string query = $@"DECLARE @DC_TYPE VARCHAR(5);
                       SET @DC_TYPE = '{dcType}';
                       SELECT A.CODE,A.BARCODE,A.BLABEL,B.ITEM_NAME AS ITEM_ID, 
                       CASE WHEN @DC_TYPE IN ('PB','PR') THEN A.PRATE
                       WHEN @DC_TYPE IN ('SB','SR') THEN A.SRATE
                       ELSE 0 END AS SRATE , A.COLOR, A.SIZE
                       FROM TBL_BARCODE A 
                       INNER JOIN TBL_ITEMSMASTER B ON A.ITEM_CODE = B.ITEM_CODE 
                       WHERE A.DLT = 'T' AND A.ASTATUS = 'Y' AND B.DLT = 'T' AND B.ASTATUS = 'Y'";


                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["BARCODE"]);
                                string? blabel = Convert.ToString(reader["BLABEL"]);
                                string? itemID = Convert.ToString(reader["ITEM_ID"]);
                                int? wsale = Convert.ToInt32(reader["SRATE"]);
                                int? color = Convert.ToInt32(reader["COLOR"]);
                                int? size = Convert.ToInt32(reader["SIZE"]);
                                dropdown.Add(new { key = code, value = name, blabel = blabel, itemID = itemID, wsale = wsale, color = color, size = size });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> BarcodeLabelDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE, GROUP_NAME FROM TBL_BLABEL WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["GROUP_CODE"]);
                                string? name = Convert.ToString(reader["GROUP_NAME"]);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<string, string>> ItemMasterDropdownForPOS()
        {
            List<KeyValuePair<string, string>> dropdown = new List<KeyValuePair<string, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT ITEM_ID,ITEM_NAME FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y' AND ISNULL(ITEM_ID,'') <> ''";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string code = reader.GetString(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<string, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> SubsidiaritiesItemGroups()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_ITEMSGROUP WHERE DLT = 'T' AND ASTATUS = 'Y' AND GROUP_TYPE = 'S'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> RoleModules()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE, GROUP_NAME FROM TBL_ROLE_MODULE WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> FinishItemsDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT ITEM_CODE,ITEM_NAME FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y' AND ITEM_TYPE = 'F'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> ProcessesDropdown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE, GROUP_NAME FROM TBL_PROCESS WHERE ASTATUS = 'Y'	AND DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> RawItemsDropdown()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT ITEM_CODE, ITEM_NAME, ISNULL(IUNIT_CODE,0) AS IUNIT_CODE FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y' AND (ITEM_TYPE <> 'F' OR ITEM_TYPE IS NULL)";
                //string query = $@"EXEC STKPROC 143,'2025-01-01','2027-01-01',1,1,'','','','','','','','','','','',''";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                int unit = reader.GetInt32(2);
                                //double currentStock = Convert.ToDouble(reader.GetValue(3));
                                dropdown.Add(new { key = code, value = name, unit = unit });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<dynamic> GetCurrentStock(string sDate, string eDate, string Branch, string Period)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"EXEC STKPROC 71,'{sDate}','{eDate}','{Branch}',{Period},'',''";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int ItemId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                                string ItemName = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                                int Balance = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2));
                                //double currentStock = Convert.ToDouble(reader.GetValue(3));
                                dropdown.Add(new { ItemId = ItemId, ItemName = ItemName, Balance = Balance });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static object GetQuantityByUnit(int id)
        {
            object qty = 0;
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"SELECT ISNULL(QTY,0) AS QTY FROM TBL_UNIT WHERE GROUP_CODE = '{id}' AND DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                qty = reader.GetDouble(0);
                            }
                        }
                    }
                }
            }
            return qty;
        }

        public static List<dynamic> UnitDropdownWithQuantity()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE,GROUP_NAME,ISNULL(QTY,0) AS QTY FROM TBL_UNIT WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                double quantity = reader.GetDouble(2);
                                dropdown.Add(new { key = code, value = name, qty = quantity });
                            }
                        }
                    }
                }
            }
            return dropdown;


            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.CODE,A.DESCR, CASE WHEN C.DESCR IS NULL THEN '' ELSE C.DESCR END AS CONTROL_NAME" +
                               " FROM TBL_ACT_GROUP A" +
                               " LEFT OUTER JOIN TBL_ACT_GROUP B ON A.GR_CODE LIKE CONCAT('', B.GR_CODE, '%') AND B.ASTATUS <> 'Y'" +
                               " LEFT OUTER JOIN TBL_ACT_GROUP C ON C.CODE = A.PARENT_CODE" +
                               " WHERE A.DLT = 'T' AND A.GROUP_TYPE = 'S'  AND B.ASTATUS IS NULL" +
                               " GROUP BY A.CODE, A.DESCR,A.ASTATUS,A.GR_CODE,B.ASTATUS,C.DESCR";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["DESCR"]);
                                string? controlName = Convert.ToString(reader["CONTROL_NAME"]);

                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> GetDetailCharges()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT ACT_CODE, ACT_NAME FROM TBL_CHART WHERE ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static dynamic GetAccountsForCashReceiptVoucher(int? roleId, string? roleType)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"DECLARE @ROLE_TYPE NVARCHAR(10);  -- Adjust datatype if needed
                                DECLARE @ROLE_ID INT;  -- Adjust datatype if needed

                                -- Assign test values for debugging (remove or modify based on your use case)
                                SET @ROLE_TYPE = '{roleType}';  
                                SET @ROLE_ID = {roleId};  

                                
                                SELECT 
                                   CH.ACT_CODE,CH.ACT_NAME,PCH.ACT_NAME AS PARENT_cODE,0 AS PARTY_CODE,0 AS WHT
                                FROM TBL_ROLE R 
                                LEFT OUTER JOIN TBL_CHART CH

                                   ON (
                                      (R.SHOW_SELECTED = 1 AND CH.ACT_CODE = R.RMENU_ID)
                                      OR 
                                      (R.SHOW_SELECTED = 0 AND 
                                       CH.ACT_CODE NOT IN 
                                         (SELECT DISTINCT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = @ROLE_ID AND MODULE_ID = 3) 
      
                                      )
                                  ) 
                                  LEFT OUTER JOIN TBL_CHART PCH
                                ON PCH.ACT_CODE = CH.ACT_PARENT_CODE
                                WHERE R.ROLE_TYPE = @ROLE_TYPE
                                  AND R.MODULE_ID = 3 
                                  AND CH.DLT = 'T' 
                                  AND CH.ASTATUS = 'Y'
                                  AND R.ROLE_ID = @ROLE_ID
                                  AND CH.ACT_TYPE = 'S' AND CH.ACT_NATURE NOT IN (3,4,9,10,13) AND CH.ASTATUS = 'Y' AND CH.DLT = 'T'

                                GROUP BY CH.ACT_CODE,CH.ACT_NAME,PCH.ACT_NAME 


                                  UNION ALL

                                  SELECT      CH.ACT_CODE,CH.ACT_NAME,PCH.ACT_NAME AS PARENT_cODE,0 AS PARTY_CODE,0 AS WHT
                                  FROM TBL_CHART CH
                                  LEFT OUTER JOIN TBL_CHART PCH
                                ON PCH.ACT_CODE = CH.ACT_PARENT_CODE

                                WHERE 

                                @ROLE_TYPE = 'A'  
                                AND CH.DLT = 'T' 
                                AND CH.ASTATUS = 'Y'
                                AND CH.ACT_TYPE = 'S' AND CH.ACT_NATURE NOT IN (3,4,9,10,13) AND CH.ASTATUS = 'Y' AND CH.DLT = 'T'


                                UNION ALL
                                        SELECT  CH.ACT_CODE,CH.ACT_NAME,PCH.ACT_NAME AS PARENT_cODE,0 AS PARTY_CODE,0 AS WHT
		                                FROM TBL_CHART CH 
                                  LEFT OUTER JOIN TBL_CHART PCH
                                ON PCH.ACT_CODE = CH.ACT_PARENT_CODE
		                                WHERE 
  
                                @ROLE_TYPE

                                  = 'U'  
                                  AND CH.ACT_TYPE = 'S' AND CH.ACT_NATURE NOT IN (3,4,9,10,13) AND CH.ASTATUS = 'Y' AND CH.DLT = 'T'
   
                                      AND CH.DLT = 'T' 
                                      AND CH.ASTATUS = 'Y' AND  
                                       (SELECT 
                                 COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_CHART M ON R.RMENU_ID = M.ACT_CODE 
                                 WHERE R.ROLE_TYPE = 
                                @ROLE_TYPE
   
                                      AND R.MODULE_ID = 3 

                                      AND M.DLT = 'T' 
                                      AND M.ASTATUS = 'Y'
                                      AND R.ROLE_ID =
	  
                                  @ROLE_ID
                                   AND M.ACT_TYPE = 'S' AND M.ACT_NATURE NOT IN (3,4,9,10,13) AND CH.ASTATUS = 'Y' AND M.DLT = 'T'
	
                                      ) < 1


                                UNION ALL
                                ------------------START PARTY------------
                                SELECT 
                                  M.ACT_CODE,M.PARTY_NAME ,''  AS PARENT_cODE,M.PARTY_CODE, M.WHT
                                FROM TBL_ROLE R 
                                LEFT OUTER JOIN TBL_PARTY_TYPES M 
                                  ON (
                                      (R.SHOW_SELECTED = 1 AND M.PARTY_CODE = R.RMENU_ID AND M.ACT_CODE = R.ACT_CODE)
                                      OR 
                                      (R.SHOW_SELECTED = 0 AND 
                                       M.PARTY_CODE NOT IN 
                                         (SELECT DISTINCT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = @ROLE_ID AND MODULE_ID = 6) 
                                       AND 
                                       M.ACT_CODE NOT IN 
                                         (SELECT DISTINCT ACT_CODE FROM TBL_ROLE WHERE ROLE_ID = @ROLE_ID AND MODULE_ID = 6)
                                      )
                                  ) 


                                WHERE R.ROLE_TYPE = @ROLE_TYPE
                                  AND R.MODULE_ID = 6 
                                  AND M.DLT = 'T' 
                                  AND M.ASTATUS = 'Y'
                                  AND R.ROLE_ID = @ROLE_ID
                                GROUP BY  M.ACT_CODE,M.PARTY_NAME  ,M.PARTY_CODE, M.WHT


                                  UNION ALL

                                  SELECT   CH.ACT_CODE,M.PARTY_NAME ,CH.ACT_NAME AS PARENT_cODE,M.PARTY_CODE , M.WHT
                                  FROM TBL_PARTY_TYPES M
                                  LEFT OUTER JOIN TBL_CHART CH
                                  ON CH.ACT_CODE = M.ACT_CODE
                                  WHERE 
  
                                @ROLE_TYPE
  
                                  = 'A'  

  
                                      AND M.DLT = 'T' 
                                      AND M.ASTATUS = 'Y'


                                UNION ALL
                                        SELECT   CH.ACT_CODE,M.PARTY_NAME ,CH.ACT_NAME AS PARENT_cODE,M.PARTY_CODE , M.WHT
		                                FROM TBL_PARTY_TYPES M 
		                                  LEFT OUTER JOIN TBL_CHART CH
                                  ON CH.ACT_CODE = M.ACT_CODE
		                                WHERE 
  
                                @ROLE_TYPE

                                  = 'U'  
  
   
                                      AND M.DLT = 'T' 
                                      AND M.ASTATUS = 'Y' AND 
                                       (SELECT 
                                 COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_PARTY_TYPES M ON R.RMENU_ID = M.PARTY_CODE  AND M.ACT_CODE = R.ACT_CODE
                                 WHERE R.ROLE_TYPE = 
                                @ROLE_TYPE
   
                                      AND R.MODULE_ID = 6 

                                      AND M.DLT = 'T' 
                                      AND M.ASTATUS = 'Y'
                                      AND R.ROLE_ID =
	  
                                  @ROLE_ID

	
                                      ) < 1
";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ACT_CODE"]);
                                string? name = Convert.ToString(reader["ACT_NAME"]);
                                string? parentCode = Convert.ToString(reader["PARENT_CODE"]);
                                int partyCode = Convert.ToInt32(reader["PARTY_CODE"]);
                                double wht = Convert.ToDouble(reader["WHT"]);
                                string? customKey = $"{Convert.ToInt32(reader["ACT_CODE"])}0123456789{Convert.ToInt32(reader["PARTY_CODE"])}";
                                dropdown.Add(new { key = customKey, value = name, accountCode = code, parentCode = parentCode, partyCode = partyCode, wht = wht });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic GetBookTypesForReceiptVoucher(int? roleId, string? branchId, int? showSelected, Common common)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                //if (roleId > 0)
                //{
                //    if (showSelected == 1)
                //        query = $"SELECT ACT_CODE,ACT_NAME FROM TBL_CHART WHERE ACT_NATURE IN (1, 2) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y' AND ACT_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                //    else
                //        query = $"SELECT ACT_CODE,ACT_NAME FROM TBL_CHART WHERE ACT_NATURE IN (1, 2) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y' AND ACT_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                //}
                //else
                //{
                //    query = "SELECT ACT_CODE,ACT_NAME FROM TBL_CHART WHERE ACT_NATURE IN (1, 2) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                //}

                query = @$" SELECT 
                           CH.ACT_CODE,CH.ACT_NAME 
                        FROM TBL_ROLE R 
                        LEFT OUTER JOIN TBL_CHART CH

                           ON (
                              (R.SHOW_SELECTED = 1 AND CH.ACT_CODE = R.RMENU_ID)
                              OR 
                              (R.SHOW_SELECTED = 0 AND 
                               CH.ACT_CODE NOT IN 
                                 (SELECT DISTINCT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = {common.RoleID} AND MODULE_ID = 3) 
      
                              )
                          ) 
                          LEFT OUTER JOIN TBL_CHART PCH
                        ON PCH.ACT_CODE = CH.ACT_PARENT_CODE
                        WHERE R.ROLE_TYPE = '{common.RoleType}'
                          AND R.MODULE_ID = 3 
                          AND CH.DLT = 'T' 
                          AND CH.ASTATUS = 'Y'
                          AND R.ROLE_ID = {common.RoleID}
                          AND CH.ACT_NATURE IN (1, 2) AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'

                        GROUP BY CH.ACT_CODE,CH.ACT_NAME 


                          UNION ALL

                          SELECT     CH.ACT_CODE,CH.ACT_NAME 
                          FROM TBL_CHART CH
                          LEFT OUTER JOIN TBL_CHART PCH
                        ON PCH.ACT_CODE = CH.ACT_PARENT_CODE

                        WHERE 

                        '{common.RoleType}' = 'A'  
                        AND CH.DLT = 'T' 
                        AND CH.ASTATUS = 'Y'
                         AND CH.ACT_NATURE IN (1, 2) AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'


                        UNION ALL
                                SELECT CH.ACT_CODE,CH.ACT_NAME 
		                        FROM TBL_CHART CH 
                          LEFT OUTER JOIN TBL_CHART PCH
                        ON PCH.ACT_CODE = CH.ACT_PARENT_CODE
		                        WHERE 
  
                        '{common.RoleType}'

                          = 'U'  
                          AND CH.ACT_NATURE IN (1, 2) AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'
   
                              AND CH.DLT = 'T' 
                              AND CH.ASTATUS = 'Y' AND  
                               (SELECT 
                         COUNT(R.MODULE_ID) FROM TBL_ROLE R LEFT OUTER JOIN TBL_CHART M ON R.RMENU_ID = M.ACT_CODE 
                         WHERE R.ROLE_TYPE = 
                        '{common.RoleType}'
   
                              AND R.MODULE_ID = 3 

                              AND M.DLT = 'T' 
                              AND M.ASTATUS = 'Y'
                              AND R.ROLE_ID =
	  
                          {common.RoleID}
                            AND CH.ACT_NATURE IN (1, 2) AND CH.ACT_TYPE = 'S' AND CH.DLT = 'T' AND CH.ASTATUS = 'Y'
	
                              ) < 1 ";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ACT_CODE"]);
                                string? name = Convert.ToString(reader["ACT_NAME"]);
                                dropdown.Add(new { key = code, value = name });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic GetAllBookTypesForReceiptVoucher(int? roleId, string? branchId, int? showSelected)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT ACT_CODE,ACT_NAME FROM TBL_CHART WHERE ACT_NATURE NOT IN (3, 4) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y' AND ACT_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT ACT_CODE,ACT_NAME FROM TBL_CHART WHERE ACT_NATURE NOT IN (3, 4) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y' AND ACT_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = "SELECT ACT_CODE,ACT_NAME FROM TBL_CHART WHERE ACT_NATURE NOT IN (3, 4) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                }

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ACT_CODE"]);
                                string? name = Convert.ToString(reader["ACT_NAME"]);
                                dropdown.Add(new { key = code, value = name });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic GetAccountsForAccountingReport(bool IsTypeC, int? roleId, string? branchId, int? showSelected)
        {
            string type;
            if (IsTypeC)
            {
                type = "C";
            }
            else
            {
                type = "S";
            }
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE FROM TBL_CHART WHERE ACT_TYPE = '{type}' AND DLT = 'T' AND ACT_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE FROM TBL_CHART WHERE ACT_TYPE = '{type}' AND DLT = 'T' AND ACT_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE FROM TBL_CHART WHERE ACT_TYPE = '{type}' AND DLT = 'T'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ACT_CODE"]);
                                string? name = Convert.ToString(reader["ACT_NAME"]);
                                string? accountGRCode = Convert.ToString(reader["ACT_GR_CODE"]);
                                dropdown.Add(new { key = code, value = name, accountGRCode = accountGRCode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static List<KeyValuePair<int, string>> GetControlsForImportReport()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE, DESCR FROM TBL_COST_CENTER WHERE DLT = 'T' AND GROUP_TYPE = 'C'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = reader.GetInt32(0);
                                string name = reader.GetString(1);
                                dropdown.Add(new KeyValuePair<int, string>(code, name));
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic GetSubsidiaryForImportReport()
        {

            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT CODE, DESCR, PARENT_CODE FROM TBL_COST_CENTER WHERE DLT = 'T' AND GROUP_TYPE = 'S'";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["DESCR"]);
                                int? pCode = Convert.ToInt32(reader["PARENT_CODE"]);
                                dropdown.Add(new { key = code, value = name, pCode = pCode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        //public static List<KeyValuePair<int, string>> GetSubsidiaryForImportReport()
        //{
        //    List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
        //    using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
        //    {
        //        string query = "SELECT CODE, DESCR, PARENT_CODE FROM TBL_COST_CENTER WHERE DLT = 'T' AND GROUP_TYPE = 'S'";
        //        using (SqlCommand command = new SqlCommand(query, conn))
        //        {
        //            conn.Open();
        //            using (SqlDataReader reader = command.ExecuteReader())
        //            {
        //                if (reader.HasRows)
        //                {
        //                    while (reader.Read())
        //                    {
        //                        int code = reader.GetInt32(0);
        //                        string name = reader.GetString(1);
        //                        dropdown.Add(new KeyValuePair<int, string>(code, name));
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return dropdown;
        //}

        public static dynamic GetAccountsForPartyReport(int? roleId, string? branchId, int? showSelected)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE, PASS FROM TBL_CHART WHERE ACT_NATURE IN (3,4,9,10,13) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                    else
                        query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE, PASS FROM TBL_CHART WHERE ACT_NATURE IN (3,4,9,10,13) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y' AND ACT_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE, PASS FROM TBL_CHART WHERE ACT_NATURE IN (3,4,9,10,13) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ACT_CODE"]);
                                string? name = Convert.ToString(reader["ACT_NAME"]);
                                string? accountGRCode = Convert.ToString(reader["ACT_GR_CODE"]);
                                string? pass = Convert.ToString(reader["PASS"]);
                                dropdown.Add(new { key = code, value = name, accountGRCode = accountGRCode, pass = pass });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic GetAccountsForSalesmanReport(int? roleId, string? branchId, int? showSelected)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE, PASS FROM TBL_CHART WHERE ACT_NATURE IN (9) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y' AND ACT_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE, PASS FROM TBL_CHART WHERE ACT_NATURE IN (9) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y' AND ACT_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE, PASS FROM TBL_CHART WHERE ACT_NATURE IN (9) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ACT_CODE"]);
                                string? name = Convert.ToString(reader["ACT_NAME"]);
                                string? accountGRCode = Convert.ToString(reader["ACT_GR_CODE"]);
                                string? pass = Convert.ToString(reader["PASS"]);
                                dropdown.Add(new { key = code, value = name, accountGRCode = accountGRCode, pass = pass });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }


        public static dynamic GetBookTypesForPOS(int? roleId, string? branchId, int? showSelected)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE FROM TBL_CHART WHERE ACT_NATURE IN (1,2) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y' AND ACT_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE FROM TBL_CHART WHERE ACT_NATURE IN (1,2) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y' AND ACT_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 3 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = $"SELECT ACT_CODE,ACT_NAME,ACT_GR_CODE FROM TBL_CHART WHERE ACT_NATURE IN (1,2) AND ACT_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ACT_CODE"]);
                                string? name = Convert.ToString(reader["ACT_NAME"]);
                                string? accountGRCode = Convert.ToString(reader["ACT_GR_CODE"]);
                                dropdown.Add(new { key = code, value = name, accountGRCode = accountGRCode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic GetRegionsForPartyReport()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"SELECT CODE,DESCR,GR_CODE FROM TBL_REGION WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["CODE"]);
                                string? name = Convert.ToString(reader["DESCR"]);
                                string? grcode = Convert.ToString(reader["GR_CODE"]);
                                dropdown.Add(new { key = code, value = name, grcode = grcode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic GetItemGroupsForPartyReport()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"SELECT GROUP_CODE,GROUP_NAME FROM TBL_ITEMSGROUP WHERE GROUP_TYPE = 'S' AND DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["GROUP_CODE"]);
                                string? name = Convert.ToString(reader["GROUP_NAME"]);
                                dropdown.Add(new { key = code, value = name });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static dynamic GetBranchForStockReport()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $"SELECT BCODE , B_NAME FROM TBL_BRANCH WHERE ASTATUS = 'Y' AND DLT = 'T'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["BCODE"]);
                                string? name = Convert.ToString(reader["B_NAME"]);
                                dropdown.Add(new { key = code, value = name });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic GetItemMasterForPartyReport()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT ITEM_CODE, ITEM_NAME, GROUP_CODE, ITEM_ID FROM TBL_ITEMSMASTER WHERE  DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ITEM_CODE"]);
                                string? name = Convert.ToString(reader["ITEM_NAME"]);
                                int groupCode = Convert.ToInt32(reader["GROUP_CODE"]);
                                string itemId = Convert.ToString(reader["ITEM_ID"]);
                                dropdown.Add(new { key = code, value = name, groupCode = groupCode, itemId = itemId });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic GetItemIds()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT ITEM_NAME AS ITEM_ID FROM TBL_ITEMSMASTER WHERE  DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string? code = Convert.ToString(reader["ITEM_ID"]);
                                string? name = Convert.ToString(reader["ITEM_ID"]);
                                dropdown.Add(new { key = code, value = name });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic PartyTypeDropdownForPartyReport(int? roleId, string? branchId, int? showSelected)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT PARTY_CODE,PARTY_NAME,ACT_CODE, REGION, PARTY_TYPE_CODE FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT PARTY_CODE,PARTY_NAME,ACT_CODE, REGION, PARTY_TYPE_CODE FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE NOT IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = "SELECT PARTY_CODE,PARTY_NAME,ACT_CODE, REGION, PARTY_TYPE_CODE FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["PARTY_CODE"]);
                                string? name = Convert.ToString(reader["PARTY_NAME"]);
                                string? accountCode = Convert.ToString(reader["ACT_CODE"]);
                                string? regionCode = Convert.ToString(reader["REGION"]);
                                string? natureCode = Convert.ToString(reader["PARTY_TYPE_CODE"]);
                                string? customKey = $"{Convert.ToInt32(reader["PARTY_CODE"])}0123456789{Convert.ToInt32(reader["ACT_CODE"])}";
                                dropdown.Add(new { key = customKey, value = name, partyCode = code, accountCode = accountCode, regionCode = regionCode, natureCode = natureCode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic SalesmanDropdownForPOS(int? roleId, string? branchId, int? showSelected)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT PARTY_CODE,PARTY_NAME,ACT_CODE, REGION, PARTY_TYPE_CODE FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 9 AND DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT PARTY_CODE,PARTY_NAME,ACT_CODE, REGION, PARTY_TYPE_CODE FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 9 AND DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE NOT IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = "SELECT PARTY_CODE,PARTY_NAME,ACT_CODE, REGION, PARTY_TYPE_CODE FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 9 AND DLT = 'T' AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["PARTY_CODE"]);
                                string? name = Convert.ToString(reader["PARTY_NAME"]);
                                string? accountCode = Convert.ToString(reader["ACT_CODE"]);
                                string? regionCode = Convert.ToString(reader["REGION"]);
                                string? natureCode = Convert.ToString(reader["PARTY_TYPE_CODE"]);
                                string? customKey = $"{Convert.ToInt32(reader["PARTY_CODE"])}0123456789{Convert.ToInt32(reader["ACT_CODE"])}";
                                dropdown.Add(new { key = customKey, value = name, partyCode = code, accountCode = accountCode, regionCode = regionCode, natureCode = natureCode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic PartyTypeWithpType(string sDate, string eDate, string partyCode, string aCode, Common common)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"EXEC PPROC 139,'{sDate}','{eDate}','{aCode}','{partyCode}','{common.Branch}','{common.Period}','','','','','','','','',''";


                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["PARTY_CODE"]);
                                string? name = Convert.ToString(reader["PARTY_NAME"]);
                                string? balance = Convert.ToString(reader["BALANCE"]);
                                int accountCode = Convert.ToInt32(reader["ACT_CODE"]);
                                string? natureCode = Convert.ToString(reader["PARTY_TYPE_CODE"]);
                                //string? scode = Convert.ToString(reader["S_CODE"]);
                                //string? regionCode = Convert.ToString(reader["REGION"]);
                                //string? disc = Convert.ToString(reader["DISC"]);
                                string? customKey = $"{Convert.ToInt32(reader["PARTY_CODE"])}0123456789{Convert.ToInt32(reader["ACT_CODE"])}";
                                int paymentTerms = Convert.ToInt32(reader["PAYMENT_TERMS"]);
                                //string? sTax =  Convert.ToString(reader["SERVICE_TAX"]);

                                dropdown.Add(new { key = customKey, value = name, partyCode = code, accountCode = accountCode, paymentTerms = paymentTerms, balance = balance, natureCode = natureCode });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static dynamic PartyTypeDropdownForInvoice(int? roleId, string? branchId, int? showSelected)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT PARTY_CODE,PARTY_NAME,ACT_CODE,REGION,DISC,S_CODE,PAYMENT_TERMS FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT PARTY_CODE,PARTY_NAME,ACT_CODE,REGION,DISC,S_CODE,PAYMENT_TERMS FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y' AND PARTY_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE NOT IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = "SELECT PARTY_CODE,PARTY_NAME,ACT_CODE,REGION,DISC,S_CODE,PAYMENT_TERMS FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["PARTY_CODE"]);
                                string? name = Convert.ToString(reader["PARTY_NAME"]);
                                string? accountCode = Convert.ToString(reader["ACT_CODE"]);
                                string? scode = Convert.ToString(reader["S_CODE"]);
                                string? regionCode = Convert.ToString(reader["REGION"]);
                                string? disc = Convert.ToString(reader["DISC"]);
                                string? customKey = $"{Convert.ToInt32(reader["PARTY_CODE"])}0123456789{Convert.ToInt32(reader["ACT_CODE"])}";
                                int paymentTerms = Convert.ToInt32(reader["PAYMENT_TERMS"]);
                                dropdown.Add(new { key = customKey, value = name, partyCode = code, accountCode = accountCode, regionCode = regionCode, disc = disc, scode = scode, paymentTerms = paymentTerms });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic PartyTypeDropdownForLot(int? roleId, string? branchId, int? showSelected)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT PARTY_CODE,PARTY_NAME,ACT_CODE,REGION,DISC FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND PARTY_TYPE_CODE = 10 AND ASTATUS = 'Y' AND PARTY_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                    else
                        query = $"SELECT PARTY_CODE,PARTY_NAME,ACT_CODE,REGION,DISC FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND PARTY_TYPE_CODE = 10 AND ASTATUS = 'Y' AND PARTY_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId}) AND ACT_CODE NOT IN (SELECT ACT_CODE FROM TBL_ROLE WHERE R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 6 AND ROLE_ID = {roleId})";
                }
                else
                {
                    query = "SELECT PARTY_CODE,PARTY_NAME,ACT_CODE,REGION,DISC FROM TBL_PARTY_TYPES WHERE DLT = 'T' AND PARTY_TYPE_CODE = 10 AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["PARTY_CODE"]);
                                string? name = Convert.ToString(reader["PARTY_NAME"]);
                                string? accountCode = Convert.ToString(reader["ACT_CODE"]);
                                string? regionCode = Convert.ToString(reader["REGION"]);
                                string? disc = Convert.ToString(reader["DISC"]);
                                string? customKey = $"{Convert.ToInt32(reader["PARTY_CODE"])}0123456789{Convert.ToInt32(reader["ACT_CODE"])}";
                                dropdown.Add(new { key = customKey, value = name, partyCode = code, accountCode = accountCode, regionCode = regionCode, disc = disc });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        public static dynamic ItemIdsDropdownForPurchaseBill(int? roleId, string? branchId, int? showSelected)
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "";
                if (roleId > 0)
                {
                    if (showSelected == 1)
                        query = $"SELECT ITEM_CODE,ITEM_ID,ITEM_NAME,PACK FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y' AND ITEM_CODE IN (SELECT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = {roleId} AND R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 4)";
                    else
                        query = $"SELECT ITEM_CODE,ITEM_ID,ITEM_NAME,PACK FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y' AND ITEM_CODE NOT IN (SELECT RMENU_ID FROM TBL_ROLE WHERE ROLE_ID = {roleId} AND R_BCODE = {Convert.ToInt32(branchId)} AND MODULE_ID = 4)";
                }
                else
                {
                    query = "SELECT ITEM_CODE,ITEM_ID,ITEM_NAME,PACK FROM TBL_ITEMSMASTER WHERE DLT = 'T' AND ASTATUS = 'Y'";
                }
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int code = Convert.ToInt32(reader["ITEM_CODE"]);
                                string? pack = Convert.ToString(reader["PACK"]);
                                string? name = Convert.ToString(reader["ITEM_NAME"]);
                                string? itemId = Convert.ToString(reader["ITEM_NAME"]);
                                dropdown.Add(new { key = code, value = name, itemId = itemId, pack = pack });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }

        #endregion

        //public static Models.MetaClass.UserSession GetUserData(Controller _controller)
        //{
        //    UserSession UserRecord = null;
        //    if (IsUserLogin(_controller))
        //    {
        //        var jsonResult = JsonConvert.DeserializeObject(GetSession(_controller, LoginUserSession).ToString());
        //        UserRecord = JsonConvert.DeserializeObject<UserSession>(jsonResult.ToString());
        //    }
        //    return UserRecord;
        //}
        //public static object GetSession(Controller _controller, string _key)
        //{
        //    object ReturnObject = null;
        //    var SessionObject = _controller.HttpContext.Session.GetString(_key);
        //    if (SessionObject != null)
        //    {
        //        ReturnObject = SessionObject;
        //    }
        //    return ReturnObject;
        //}
        //public static bool IsUserLogin(Controller _controller)
        //{
        //    bool ReturnValue = false;
        //    if (GetSession(_controller, LoginUserSession) != null)
        //    {
        //        ReturnValue = true;
        //    }
        //    return ReturnValue;
        //}
        public static List<dynamic> FBRTypeDropdown()
        {
            List<dynamic> dropdown = new List<dynamic>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT CODE, S_NAME, ITEM_SNO, SCHEDULE_NO, SERIAL_NO 
                                    FROM TBL_FBR_TYPE WHERE DLT = 'T' AND ASTATUS = 'Y'";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                int key = Convert.ToInt32(reader["CODE"]);
                                string? name = reader["S_NAME"] == DBNull.Value ? "" : Convert.ToString(reader["S_NAME"]);
                                string? itemSno = reader["ITEM_SNO"] == DBNull.Value ? "" : Convert.ToString(reader["ITEM_SNO"]);
                                string? scheNo = reader["SCHEDULE_NO"] == DBNull.Value ? "" : Convert.ToString(reader["SCHEDULE_NO"]);
                                string? seriNo = reader["SERIAL_NO"] == DBNull.Value ? "" : Convert.ToString(reader["SERIAL_NO"]);
                                dropdown.Add(new
                                {
                                    key = key,
                                    value = name,
                                    item_Sno = itemSno,
                                    sche_No = scheNo,
                                    seri_No = seriNo
                                });
                            }
                        }
                    }
                }
            }
            return dropdown;
        }
        public static List<KeyValuePair<int, string>> GetChartTypeDropDown()
        {
            List<KeyValuePair<int, string>> dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection conn = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @"SELECT ID,TNAME FROM TBL_CHART_TYPE";

                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            string name = reader.GetString(1);
                            dropdown.Add(new KeyValuePair<int, string>(id, name));
                        }
                    }
                }
            }
            return dropdown;
        }
    }
    public class AjaxResponse
    {
        public bool Success { get; set; }
        public string Type { get; set; }
        public string FieldName { get; set; }
        public string Message { get; set; }
        public string TargetURL { get; set; }
        public string Data { get; set; }
        public int Id { get; set; }
    }

    #region EnumHelper

    public static class EnumStatus
    {
        public const string Active = "0";
        public const string InActive = "1";
    }
    public static class EnumDrCr
    {
        public const string Debit = "Debit";
        public const string Credit = "Credit";
    }
    public static class EnumProfle
    {
        public const string Supplier = "Supplier";
        public const string Customer = "Customer";
    }
    public static class EnumBatchNo
    {
        public const string CashPayment = "CashPayment";
        public const string CashReceived = "CashReceived";
        public const string BankPayment = "BankPayment";
        public const string BankReceived = "BankReceived";
    }
    public static class EnumMenuType
    {
        public const string Parent = "P";
        public const string Children = "C";
    }
    public static class EnumRole
    {
        public const string SuperAdministrator = "Super Administrator";
        public const string Administrator = "Administrator";
        public const string Employee = "Employee";
        public const string Leader = "Leader";
        public const string Vendor = "Vendor";
    }
    public static class EnumPageType
    {
        public const string Index = "Index";
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string View = "View";
        public const string Sorting = "Sorting";
        public const string Import = "Import";
    }
    public static class EnumJQueryResponseType
    {
        public const string DataOnly = "D";
        public const string MessageOnly = "M";
        public const string RedirectOnly = "T";
        public const string RefreshOnly = "R";
        public const string ReloadOnly = "RL";
        public const string MessageAndRedirect = "M-T";
        public const string MessageAndRedirectWithDelay = "M-TD";
        public const string MessageAndRefresh = "M-R";
        public const string MessageRefreshRedirect = "M-R-T";
        public const string MessageRefreshRedirectWithDelay = "M-R-TD";
        public const string RefreshAndRedirect = "R-T";
        public const string RefreshAndRedirectWithDelay = "R-TD";
        public const string RedirectWithDelay = "TD";
        public const string MessageAndReloadWithDelay = "M-RLD";
    }
    #endregion
}
