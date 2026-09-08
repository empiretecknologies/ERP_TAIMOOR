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
    public class EntityRepository : IEntityRepository
    {
        public MyHttpResponseMessage GetEntity(int menuid)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();

            List<KeyValuePair<int, string>> Dropdown = new List<KeyValuePair<int, string>>();
            using (SqlConnection db = new SqlConnection(new SQLService().getconnstring()))
            {
                string Qurey = "SELECT GROUP_CODE,GROUP_NAME FROM TBL_ENTITY WHERE DLT = 'T'";
                using (SqlCommand Commad = new SqlCommand(Qurey, db))
                {
                    db.Open();
                    using (SqlDataReader Reader = Commad.ExecuteReader())
                    {
                        if (Reader.HasRows)
                        {
                            while (Reader.Read())
                            {
                                int code = Reader.GetInt32(0);
                                string name = Reader.GetString(1);
                                Dropdown.Add(new KeyValuePair<int, string>(code, name));
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
