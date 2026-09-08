using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class SalesManRepository : ISalesManRepository
    {
        public MyHttpResponseMessage GetAllSalesMan(int menuid)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            List<dynamic> Dropdown = new List<dynamic>();
            using (SqlConnection db = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = @"SELECT A.PARTY_CODE,A.PARTY_NAME,
                                B.ACT_NAME AS CONTROL_NAME ,A.ACT_CODE
                                FROM TBL_PARTY_TYPES  A
                                LEFT OUTER   JOIN TBL_CHART B
                                ON A.ACT_CODE = B.ACT_CODE
                                WHERE A.DLT =  'T' AND A.ASTATUS = 'Y'
                                AND B.ASTATUS = 'Y' AND B.DLT = 'T' AND PARTY_TYPE_CODE = 9
                                ORDER BY B.ACT_cODE,A.PARTY_CODE";

                using (SqlCommand Commad = new SqlCommand(query, db))
                {
                    db.Open();
                    using (SqlDataReader Reader = Commad.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                int code = Convert.ToInt32(Reader["PARTY_CODE"]);
                                string name = Convert.ToString(Reader["PARTY_NAME"]);
                                string controlName = Convert.ToString(Reader["CONTROL_NAME"]);
                                int acode = Reader["ACT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(Reader["ACT_CODE"]);
                                Dropdown.Add(new { key = code, value = name, name = controlName,acode = acode });
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
    }
}
