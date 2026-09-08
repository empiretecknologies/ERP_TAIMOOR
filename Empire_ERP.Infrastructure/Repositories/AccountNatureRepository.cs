using Azure;
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
    public class AccountNatureRepository : IAccountNatureRepository
    {
        public MyHttpResponseMessage GetAccountNature()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            List<AccountNature> accountNatures = new List<AccountNature>();
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = "SELECT GROUP_CODE, GROUP_NAME FROM TBL_ACT_NATURE WHERE ASTATUS = 'Y' AND DLT = 'T'";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    AccountNature accountNature = new AccountNature();
                    accountNature.GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]);
                    accountNature.GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]);
                    accountNatures.Add(accountNature);
                }
                reader.Close();
                response.data = accountNatures;
                response.msg = "";
                response.msgType = 1;
                return response;
            }
        }
    }
}