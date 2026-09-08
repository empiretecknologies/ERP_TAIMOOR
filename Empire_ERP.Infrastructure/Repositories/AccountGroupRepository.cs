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
    public class AccountGroupRepository : IAccountGroupRepository
    {
        public MyHttpResponseMessage GetAccountGroups()
        {            
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            List<AccountGroup> accountGroups = new List<AccountGroup>();
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT A.CODE,A.DESCR, CASE WHEN C.DESCR IS NULL THEN '' ELSE C.DESCR END AS CONTROL_NAME" +
                    " FROM TBL_ACT_GROUP A" +
                    " LEFT OUTER JOIN TBL_ACT_GROUP B ON A.GR_CODE LIKE CONCAT('', B.GR_CODE, '%') AND B.ASTATUS <> 'Y'" +
                    " LEFT OUTER JOIN TBL_ACT_GROUP C ON C.CODE = A.PARENT_CODE" +
                    " WHERE A.DLT = 'T' AND A.GROUP_TYPE = 'S'  AND B.ASTATUS IS NULL" +
                    " GROUP BY A.CODE, A.DESCR,A.ASTATUS,A.GR_CODE,B.ASTATUS,C.DESCR";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    AccountGroup accountGroup = new AccountGroup();
                    accountGroup.CODE = Convert.ToInt32(reader["CODE"]);
                    accountGroup.DESCR = Convert.ToString(reader["DESCR"]);
                    accountGroup.GROUP_TYPE = Convert.ToString(reader["CONTROL_NAME"]);
                    accountGroups.Add(accountGroup);
                }
                reader.Close();
                response.data = accountGroups;
                response.msg = "";
                response.msgType = 1;
                return response;
            }
        }
    }
}