using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Net.Mail;
using static System.Net.Mime.MediaTypeNames;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class WeighBridgeRepository : IWeighBridgeRepository
    {
        public ICommonRepository _commonRepository { get; set; }
        public ILoginService _loginService { get; set; }
        public IUserService _userService { get; set; }
        public IMenuRepository _menuRepository { get; set; }

        public WeighBridgeRepository(ICommonRepository commonRepository, ILoginService loginService, IUserService userService, IMenuRepository menuRepository)
        {
            _commonRepository = commonRepository;
            _loginService = loginService;
            _userService = userService;
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetGrid(Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            var Menu = _menuRepository.GetMenu(common.MenuID);
            string? table = string.Empty;
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                table = menu.TABLE1;
            }
            string? databaseSize = string.Empty;
            List<object> data = new List<object>();
            try
            {
                string connectionString = new SQLService().getconnstring();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlTransaction transaction = connection.BeginTransaction();
                    SqlCommand command = connection.CreateCommand();
                    command.Transaction = transaction;
                    try
                    {
                        string query = @$"SELECT * FROM {table}";
                        command.CommandText = query;
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {

                            var row = new
                            {
                                SERIAL = Convert.ToString(reader["Serial"]),
                                VEHICLE = Convert.ToString(reader["Vehicle"]),
                                CUSTOMER = Convert.ToString(reader["Customer"]),
                                MATERIAL = Convert.ToString(reader["Material"]),
                                CONTAINER = Convert.ToString(reader["Container"]),
                                AVG = reader["AVG"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AVG"]),
                                QUANTITY = reader["Quantity"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Quantity"]),
                                PON = Convert.ToString(reader["PON"]),
                                MANUAL = Convert.ToString(reader["Manual"]),
                                MOUND = Convert.ToString(reader["Mound"]),
                                INDEX = reader["Index"] == DBNull.Value ? 0 : Convert.ToInt32(reader["Index"]),
                                LOT = Convert.ToString(reader["Lot"]),
                                PACKING = Convert.ToString(reader["Packing"]),
                                CREDIT = Convert.ToString(reader["Credit"]),
                                FTIME = reader["FTime"] == DBNull.Value ? null : Convert.ToString(reader["FTime"]),
                                STIME = reader["STime"] == DBNull.Value ? null : Convert.ToDateTime(reader["STime"]).ToString("HH:mm:ss"),
                                GROSS = reader["Gross"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Gross"]),
                                TARE = reader["Tare"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Tare"]),
                                NET = reader["Net"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Net"]),
                                RUPEES = reader["Rupees"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Rupees"]),
                                FDATE = reader["FDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["FDate"]).ToString("yyyy-MM-dd"),
                                SDATE = reader["SDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["SDate"]).ToString("yyyy-MM-dd"),
                                FOPERATOR = Convert.ToString(reader["FOperator"]),
                                SOPERATOR = Convert.ToString(reader["SOperator"]),
                                FW_WB = Convert.ToString(reader["FW_WB#"]),
                                SW_WB = Convert.ToString(reader["SW_WB#"]),
                                DRIVER = Convert.ToString(reader["Driver"]),
                                COMPLETE = reader["Complete"] == DBNull.Value ? false : Convert.ToBoolean(reader["Complete"])
                            };
                            data.Add(row);
                        }
                        reader.Close();


                        response.data = data;
                        response.msg = "";
                        response.msgType = 1;

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

        
    }
}