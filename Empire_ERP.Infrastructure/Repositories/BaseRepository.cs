using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXing;
using ZXing.QrCode.Internal;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class BaseRepository : IBaseRepository
    {
        public IMenuRepository _menuRepository { get; set; }
        public IBranchRepository _branchRepository { get; set; }

        public BaseRepository(IMenuRepository menuRepository, IBranchRepository branchRepository)
        {
            _menuRepository = menuRepository;
            _branchRepository = branchRepository;
        }

        public MyHttpResponseMessage UpdateSettings(Base modelRecord, Common common)
        {
            var Menu = _menuRepository.GetMenu(common.MenuID);
            int? MID = null;

            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                MID = menu.ID;
            }

            MyHttpResponseMessage response = new MyHttpResponseMessage();
            if (MID == null)
            {
                response.msgType = 2;
                response.msg = "Menu not found. Update aborted.";
                return response;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {

                    connection.Open();
                    string query = $@"UPDATE TBL_MENU_BUILDER SET 
                                DATA_CLEAR='{modelRecord.DATA_CLEAR}',
                                SEARCH='{modelRecord.SEARCH}',
                                D_LIMIT='{modelRecord.DATA_RLIMIT}'
                                WHERE ID = '{MID}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    response.msgType = 1;
                    response.msg = "Record Updated Successfully";


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

        public MyHttpResponseMessage GetApproval(string? userName, Common common)
        {

            //await Task.Delay(500);
            var Menu = _menuRepository.GetMenu(common.MenuID);
            int? MID = null;

            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                MID = menu.ID;
            }

            MyHttpResponseMessage response = new MyHttpResponseMessage();
            if (MID == null)
            {
                response.msgType = 2;
                response.msg = "Menu not found. Update aborted.";
                return response;
            }
            try
            {
                //List<object> jsonDataResult = new List<object>();
                string STATUS = string.Empty;
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {


                    string query = $@"select R.RMENU_ID from TBL_ROLE R 
                                      LEFT OUTER JOIN TBL_USER U ON U.ROLEID = R.ROLE_ID
                                      WHERE MODULE_ID = 5
                                      AND U.USERNAME = '{userName}'
                                      AND R.RMENU_ID ='{MID}' AND R.R_BCODE =1 AND R.DLT = 'T'";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        STATUS = "N";
                    }
                    else
                    {
                        STATUS = "Y";
                    }

                    //while (reader.Read())
                    //{
                    //    var row = new
                    //    {
                    //        MENUID = reader["RMENU_ID"] != DBNull.Value ? "Y" : "N",
                    //    };
                    //    jsonDataResult.Add(row);

                    //    reader.Close();
                    //}

                    response.data = STATUS;
                    response.msg = "";
                    response.msgType = 1;
                    return response;

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

            return  response;
        }




    }
}
