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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode.Internal;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class ItemOpeningRepository : IItemOpeningRepository
	{
        public IMenuRepository _menuRepository { get; set; }

        public ItemOpeningRepository(IMenuRepository menuRepository)
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
					table = menu.PICK_TABLE_MASTER;
				}

				if (!String.IsNullOrWhiteSpace(table))
				{
					List<object> jsonDataResult = new List<object>();
					using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
					{
						string query = " SELECT PT.ITEM_CODE,PT.ITEM_NAME,PT.ITEM_SHORT_NAME,ITEM_ID,PT.GROUP_CODE,CH.GROUP_NAME," +
					                   " SUM(ISNULL(OP.QTY, 0)) AS QTY," +
					                   " SUM(ISNULL(OP.QTY2, 0)) AS QTY2," +
					                   " SUM(ISNULL(OP.BAL_QTY, 0)) AS BAL_QTY," +
					                   " CASE WHEN OP.ASTATUS = 'Y' THEN 'Active'" +
					                   " WHEN OP.ASTATUS = 'N' then 'In-Active'" +
					                   " ELSE ''" +
					                   " END AS ASTATUS,ISNULL(U.GROUP_NAME, '') AS UNIT, ISNULL(UP.GROUP_NAME, '')  AS UNIT_PACKING" +
					                   " FROM " + table + " PT" +
					                   " LEFT OUTER JOIN TBL_ITEMSGROUP CH" +
					                   " ON CH.GROUP_CODE = PT.GROUP_CODE" +
					                   " LEFT OUTER JOIN TBL_ITEM_OPENING OP" +
                                       " ON OP.ITEM_CODE = PT.ITEM_CODE AND PT.DLT = OP.DLT" +
					                   " LEFT OUTER JOIN TBL_UNIT U" +
					                   " ON U.GROUP_CODE = PT.IUNIT_CODE" +
					                   " LEFT OUTER JOIN TBL_UNIT UP" +
					                   " ON UP.GROUP_CODE = PT.PUNIT_CODE" +
					                   " WHERE PT.DLT = 'T' AND PT.ASTATUS = 'Y'" +
					                   " GROUP BY" +
					                   " PT.ITEM_CODE,PT.ITEM_NAME,PT.ITEM_SHORT_NAME,ITEM_ID,PT.GROUP_CODE,CH.GROUP_NAME," +
					                   " ISNULL(U.GROUP_NAME, '') ,ISNULL(UP.GROUP_NAME, ''),OP.ASTATUS";
						SqlCommand command = new SqlCommand(query, connection);
						connection.Open();
						SqlDataReader reader = command.ExecuteReader();
						while (reader.Read())
						{
							var row = new
							{
								ID = Convert.ToString(reader["ITEM_CODE"]),
								ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
								ITEM_SHORT_NAME = Convert.ToString(reader["ITEM_SHORT_NAME"]),
								ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
								GROUP_CODE = Convert.ToString(reader["GROUP_CODE"]),
								GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
								QTY = Convert.ToString(reader["QTY"]),
								QTY2 = Convert.ToString(reader["QTY2"]),
								BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
								ASTATUS = Convert.ToString(reader["ASTATUS"]),
								UNIT = Convert.ToString(reader["UNIT"]),
								UNIT_PACKING = Convert.ToString(reader["UNIT_PACKING"]),
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

		private int GenerateNextId(Common common)
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
                    string maxIdQuery = "SELECT ISNULL(MAX(OP_ID), 0) + 1 FROM " + table;
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        SqlCommand command = new SqlCommand(maxIdQuery, connection);
                        connection.Open();
                        object result = command.ExecuteScalar();
                        return Convert.ToInt32(result);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        public MyHttpResponseMessage Save(ItemOpening modelRecord, Common common)
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
                            string query = "";
                            string Duplicationquery = "";
                            if (modelRecord.OP_ID == null || modelRecord.OP_ID == 0)
                            {
                                //modelRecord.ITEM_CODE = GenerateNextId(common);
								query = "INSERT INTO " + table + "" +
								        " (OP_ID, ITEM_CODE, WAREHOUSE, RATE, REF," +
								        " BATCH, LOT, MFG_DATE, EXP_DATE," +
								        " QTY, UNIT, QTY2, BAL_QTY," +
								        " PACK_UNIT, COLOR, SIZE, GRADE," +
								        " ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS," +
								        " EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS," +
								        " ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID," +
                                        " DLT, CHK, BCODE, PERIOD_ID, BILL_DATE)" +
								        " VALUES" +
								        "('" + GenerateNextId(common) + "', '" + modelRecord.ITEM_CODE + "', '" + modelRecord.WAREHOUSE + "', '" + modelRecord.RATE + "', '" + modelRecord.REF + "'," +
								        "'" + modelRecord.BATCH + "', '" + modelRecord.LOT + "', '" + modelRecord.MFG_DATE + "', '" + modelRecord.EXP_DATE + "'," +
								        "'" + modelRecord.QTY + "', '" + modelRecord.UNIT + "', '" + modelRecord.QTY2 + "', '" + modelRecord.BAL_QTY + "'," +
								        "'" + modelRecord.PACK_UNIT + "', '" + modelRecord.COLOR + "', '" + modelRecord.SIZE + "', '" + modelRecord.GRADE + "'," +
								        "'" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "', '" + Ip + "'," +
								        "'" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "', '" + Ip + "'," +
								        "'" + Postal + "', '" + Postal + "', '" + modelRecord.ASTATUS + "', '" + common.MenuID + "'," +
								        "'T', '" + modelRecord.CHK + "', '" + common.Branch + "', '" + common.Period + "', '" + modelRecord.BILL_DATE + "')";
								command.CommandText = query; 
                                command.ExecuteNonQuery();
								transaction.Commit();
								response.data = modelRecord.OP_ID;
								response.msgType = 1;
								response.msg = "Record Added Successfully";
								//Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE ITEM_NAME = '" + modelRecord.ITEM_NAME + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
								//command.CommandText = Duplicationquery;
								//int count = (int)command.ExecuteScalar();

								//if (count == 1)
								//{

								//}
								//else
								//{
								//    transaction.Rollback();
								//    response.msg = "Name Already Exist !....";
								//    response.msgType = 2;
								//}
							}
                            else
                            {
								string query2 = "SELECT ISNULL(SUM(QTY),0) AS QTY FROM TBL_BARCODE_OP WHERE ITEM_OP_ID = '" + modelRecord.OP_ID + "' AND DLT = 'T'";
								command.CommandText = query2;
								object result1 = command.ExecuteScalar();
								int QTY = Convert.ToInt32(result1);
								var BalQty = modelRecord.BAL_QTY;
								if (BalQty >= QTY)
								{
									query = "UPDATE " + table + " SET RATE = '" + modelRecord.RATE + @"',
                                            REF = '" + modelRecord.REF + @"',
                                            BATCH = '" + modelRecord.BATCH + @"',
                                            LOT = '" + modelRecord.LOT + @"',
                                            MFG_DATE = '" + modelRecord.MFG_DATE + @"',
                                            EXP_DATE = '" + modelRecord.EXP_DATE + @"',
                                            BILL_DATE = '" + modelRecord.BILL_DATE + @"',
                                            QTY = '" + modelRecord.QTY + @"',
                                            UNIT = '" + modelRecord.UNIT + @"',
                                            QTY2 = '" + modelRecord.QTY2 + @"',
                                            BAL_QTY = '" + modelRecord.BAL_QTY + @"',
                                            PACK_UNIT = '" + modelRecord.PACK_UNIT + @"',
                                            COLOR = '" + modelRecord.COLOR + @"',
                                            SIZE = '" + modelRecord.SIZE + @"',
                                            GRADE = '" + modelRecord.GRADE + @"',
                                            WAREHOUSE = '" + modelRecord.WAREHOUSE + @"',
                                            EDIT_USER_ID = '" + username + @"',
                                            EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                            EDIT_COMPUTER_NAME = '" + Computer + @"',
                                            EDIT_IP_ADDRESS = '" + Ip + @"',
                                            EDIT_POSTALCODE = '" + Postal + @"',
                                            ASTATUS = '" + modelRecord.ASTATUS + @"'
                                            WHERE OP_ID = '" + modelRecord.OP_ID + @"'";
									command.CommandText = query;
									command.ExecuteNonQuery();

									transaction.Commit();
									response.data = modelRecord.OP_ID;
									response.msgType = 1;
									response.msg = "Record Updated Successfully";
								}
								else
								{
									transaction.Rollback();
									response.msg = "You can't decrease the quantity";
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
                response.msgType = 2;
                response.msg = _catchMessage;
            }
            return response;
        }

        public MyHttpResponseMessage GetItemOpeningByCode(int code, Common common)
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
						string query = "SELECT OP_ID,ITEM_CODE,RATE,REF," +
									   " BATCH,LOT,MFG_DATE,EXP_DATE,QTY,UNIT," +
									   " QTY2,BAL_QTY,PACK_UNIT,COLOR,SIZE,GRADE,WAREHOUSE," +
									   " ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
									   " EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                       " ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS,MENU_ID,BILL_DATE,CHK" +
									   " FROM " + table + " WHERE DLT = 'T' AND OP_ID = '" + code + "'";
						SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
							var jsonDataResult = new
							{
								ID = Convert.ToInt32(reader["OP_ID"]),
								ASTATUS = Convert.ToString(reader["ASTATUS"]),
								ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
								RATE = Convert.ToString(reader["RATE"]),
								REF = Convert.ToString(reader["REF"]),
								BATCH = Convert.ToString(reader["BATCH"]),
								LOT = Convert.ToString(reader["LOT"]),
                                MFG_DATE = (reader["MFG_DATE"] == DBNull.Value || Convert.ToDateTime(reader["MFG_DATE"]) <= new DateTime(1900, 1, 2)) ? "" : Convert.ToDateTime(reader["MFG_DATE"]).ToString("yyyy-MM-dd"),
                                EXP_DATE = (reader["EXP_DATE"] == DBNull.Value || Convert.ToDateTime(reader["EXP_DATE"]) <= new DateTime(1900, 1, 2)) ? "" : Convert.ToDateTime(reader["EXP_DATE"]).ToString("yyyy-MM-dd"),
                                BILL_DATE = (reader["BILL_DATE"] == DBNull.Value || Convert.ToDateTime(reader["BILL_DATE"]) <= new DateTime(1900, 1, 2)) ? "" : Convert.ToDateTime(reader["BILL_DATE"]).ToString("yyyy-MM-dd"),
                                QTY = Convert.ToString(reader["QTY"]),
								UNIT = Convert.ToString(reader["UNIT"]),
								QTY2 = Convert.ToString(reader["QTY2"]),
								BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
								PACK_UNIT = Convert.ToString(reader["PACK_UNIT"]),
								COLOR = Convert.ToString(reader["COLOR"]),
								SIZE = Convert.ToString(reader["SIZE"]),
								GRADE = Convert.ToString(reader["GRADE"]),
								WAREHOUSE = Convert.ToString(reader["WAREHOUSE"]),
								CHK = Convert.ToString(reader["CHK"])
							};
							response.msgType = 1;
                            response.data = jsonDataResult;
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

		public MyHttpResponseMessage Delete(int code, Common common)
		{
			MyHttpResponseMessage response = new MyHttpResponseMessage();
			response.msgType = 2;
			response.msg = "Data not found in our records";
			try
			{
				var Menu = _menuRepository.GetMenu(common.MenuID);
				string? table = string.Empty, detailTable = string.Empty;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE1;
					detailTable = menu.TABLE2;
				}

				if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
				{
					if (code == 0)
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
							string query1 = $"SELECT COUNT(*) FROM {detailTable} WHERE ITEM_OP_ID = '{code}' AND DLT = 'T'";
							SqlCommand command1 = new SqlCommand(query1, connection);
							int count = (int)command1.ExecuteScalar();
							if (count > 0)
							{
								response.msg = "You can't delete this record.";
								response.msgType = 2;
							}
							else
							{
								string query = $"UPDATE {table} SET DLT = 'F' WHERE OP_ID = '{code}'";
								SqlCommand command = new SqlCommand(query, connection);
								command.ExecuteNonQuery();
								response.msgType = 1;
								response.msg = "Record Deleted Successfully";
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

		public MyHttpResponseMessage GetItemOpeningDetailByCode(int code, Common common)
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
						string query = "SELECT W.DESCR,OP.OP_ID , " +
									   " CASE WHEN OP.ASTATUS = 'Y' THEN 'Active'" +
									   " WHEN OP.ASTATUS = 'N' then 'In-Active'" +
									   " ELSE ''" +
									   " END AS ASTATUS,SM.ITEM_CODE,SM.ITEM_NAME,CH.GROUP_NAME AS ITEM_GROUP," +
									   " OP.RATE ,OP.REF,OP.BATCH,OP.LOT,OP.MFG_DATE,OP.EXP_DATE,OP.QTY ," +
									   " U.GROUP_NAME AS UNIT ,QTY2 ,BAL_QTY ,UP.GROUP_NAME AS PACK_UNIT ," +
									   " CL.GROUP_CODE AS COLOR ,CL.GROUP_NAME AS COLOR_NAME," +
									   " SZ.GROUP_CODE AS SIZE,SZ.GROUP_NAME AS SIZE_NAME," +
									   " G.GROUP_CODE AS GRADE,G.GROUP_NAME AS GRADE_NAME," +
									   " OP.ADD_USER_ID,OP.ADD_DATE,OP.ADD_IP_ADDRESS,OP.EDIT_USER_ID,OP.EDIT_DATE,OP.EDIT_COMPUTER_NAME," +
									   " OP.EDIT_IP_ADDRESS,OP.ADD_POSTALCODE,OP.EDIT_POSTALCODE,OP.ADD_COMPUTER_NAME" +
									   " FROM " + table + " OP" +
									   " LEFT OUTER JOIN TBL_ITEMSMASTER SM" +
									   " ON SM.ITEM_CODE = OP.ITEM_CODE" +
									   " LEFT OUTER JOIN TBL_ITEMSGROUP CH" +
									   " ON CH.GROUP_CODE = SM.GROUP_CODE" +
									   " LEFT OUTER JOIN TBL_UNIT U" +
									   " ON U.GROUP_CODE = OP.UNIT" +
									   " LEFT OUTER JOIN TBL_UNIT UP" +
									   " ON UP.GROUP_CODE = OP.PACK_UNIT" +
									   " LEFT OUTER JOIN TBL_COLOR CL" +
									   " ON CL.GROUP_CODE = OP.COLOR" +
									   " LEFT OUTER JOIN TBL_SIZE SZ" +
									   " ON SZ.GROUP_CODE = OP.SIZE" +
                                       " LEFT OUTER JOIN TBL_WAREHOUSE W" +
									   " ON W.CODE = OP.WAREHOUSE" +
                                       " LEFT OUTER JOIN TBL_UNIT G" +
									   " ON G.GROUP_CODE = OP.GRADE" +
									   " WHERE OP.DLT = 'T' AND OP.ASTATUS = 'Y' AND OP.ITEM_CODE = '" + code + "'";
						SqlCommand command = new SqlCommand(query, connection);
						connection.Open();
						SqlDataReader reader = command.ExecuteReader();
						while (reader.Read())
						{
							var row = new
							{
								//ID = Convert.ToString(reader["ITEM_CODE"]),
								//ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
								////ITEM_SHORT_NAME = Convert.ToString(reader["ITEM_SHORT_NAME"]),
								//ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
								//GROUP_CODE = Convert.ToString(reader["GROUP_CODE"]),
								//GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
								//QTY = Convert.ToString(reader["QTY"]),
								//QTY2 = Convert.ToString(reader["QTY2"]),
								//BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
								//ASTATUS = Convert.ToString(reader["ASTATUS"]),
								//UNIT = Convert.ToString(reader["UNIT"]),
								//UNIT_PACKING = Convert.ToString(reader["UNIT_PACKING"]),
								WAREHOUSE = Convert.ToString(reader["DESCR"]),
								ID = Convert.ToString(reader["OP_ID"]),
								ASTATUS = Convert.ToString(reader["ASTATUS"]),
								ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
								ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
								ITEM_GROUP = Convert.ToString(reader["ITEM_GROUP"]),
								RATE = Convert.ToString(reader["RATE"]),
								REF = Convert.ToString(reader["REF"]),
								BATCH = Convert.ToString(reader["BATCH"]),
								LOT = Convert.ToString(reader["LOT"]),
                                MFG_DATE = (reader["MFG_DATE"] == DBNull.Value || Convert.ToDateTime(reader["MFG_DATE"]) <= new DateTime(1900, 1, 2)) ? "" : Convert.ToDateTime(reader["MFG_DATE"]).ToString("yyyy-MM-dd"),
                                EXP_DATE = (reader["EXP_DATE"] == DBNull.Value || Convert.ToDateTime(reader["EXP_DATE"]) <= new DateTime(1900, 1, 2)) ? "" : Convert.ToDateTime(reader["EXP_DATE"]).ToString("yyyy-MM-dd"),
                                QTY = Convert.ToString(reader["QTY"]),
								UNIT = Convert.ToString(reader["UNIT"]),
								QTY2 = Convert.ToString(reader["QTY2"]),
								BAL_QTY = Convert.ToString(reader["BAL_QTY"]),
								PACK_UNIT = Convert.ToString(reader["PACK_UNIT"]),
								COLOR = Convert.ToString(reader["COLOR"]),
								COLOR_NAME = Convert.ToString(reader["COLOR_NAME"]),
								SIZE = Convert.ToString(reader["SIZE"]),
								SIZE_NAME = Convert.ToString(reader["SIZE_NAME"]),
								GRADE = Convert.ToString(reader["GRADE"]),
								GRADE_NAME = Convert.ToString(reader["GRADE_NAME"]),
								ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
								ADD_DATE = Convert.ToString(reader["ADD_DATE"]),
								ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
								ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
								EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
								EDIT_DATE = Convert.ToString(reader["EDIT_DATE"]),
								EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
								EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
								ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
								EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
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

		public MyHttpResponseMessage GetBarcodeDetailByCode(int code, Common common)
		{
			MyHttpResponseMessage response = new MyHttpResponseMessage();
			try
			{
				var Menu = _menuRepository.GetMenu(common.MenuID);
				string? table = string.Empty;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE2;
				}

				if (!String.IsNullOrWhiteSpace(table))
				{
					List<object> jsonDataResult = new List<object>();
					using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
					{
						string query = "SELECT OP.OP_ID, " +
									   " CASE WHEN OP.ASTATUS = 'Y' THEN 'Active'" +
									   " WHEN OP.ASTATUS = 'N' then 'In-Active'" +
									   " ELSE ''" +
									   " END AS ASTATUS,SM.ITEM_CODE,SM.ITEM_NAME," +
									   " BR.CODE AS BARCODE_ID , BR.BARCODE ," +
									   " OP.QTY ,OP.RATE," +
									   " U.GROUP_NAME AS UNIT ," +
									   " CL.GROUP_CODE AS COLOR ,CL.GROUP_NAME AS COLOR_NAME," +
									   " SZ.GROUP_CODE AS SIZE,SZ.GROUP_NAME AS SIZE_NAME," +
									   " G.GROUP_CODE AS GRADE,G.GROUP_NAME AS GRADE_NAME" +
									   " FROM " + table + " OP" +
									   " LEFT OUTER JOIN TBL_BARCODE BR" +
									   " ON BR.CODE = OP.BARCODE" +
									   " LEFT OUTER JOIN TBL_ITEMSMASTER SM" +
									   " ON SM.ITEM_CODE = BR.ITEM_CODE" +
									   " LEFT OUTER JOIN TBL_ITEMSGROUP CH" +
									   " ON CH.GROUP_CODE = SM.GROUP_CODE" +
									   " LEFT OUTER JOIN TBL_UNIT U" +
									   " ON U.GROUP_CODE = OP.UNIT" +
									   " LEFT OUTER JOIN TBL_COLOR CL" +
									   " ON CL.GROUP_CODE = BR.COLOR" +
									   " LEFT OUTER JOIN TBL_SIZE SZ" +
									   " ON SZ.GROUP_CODE = BR.SIZE" +
									   " LEFT OUTER JOIN TBL_UNIT G" +
									   " ON G.GROUP_CODE = SM.GRADE" +
									   " WHERE OP.DLT = 'T' AND OP.ASTATUS = 'Y' AND OP.ITEM_OP_ID = '" + code + "' ORDER BY OP.OP_ID DESC";
						SqlCommand command = new SqlCommand(query, connection);
						connection.Open();
						SqlDataReader reader = command.ExecuteReader();
						while (reader.Read())
						{
							var row = new
							{
								ID = Convert.ToString(reader["OP_ID"]),
								ASTATUS = Convert.ToString(reader["ASTATUS"]),
								ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
								ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
								BARCODE_ID = Convert.ToString(reader["BARCODE_ID"]),
								BARCODE = Convert.ToString(reader["BARCODE"]),
								QTY = Convert.ToString(reader["QTY"]),
								RATE = Convert.ToString(reader["RATE"]),
								UNIT = Convert.ToString(reader["UNIT"]),
								COLOR_NAME = Convert.ToString(reader["COLOR_NAME"]),
								SIZE_NAME = Convert.ToString(reader["SIZE_NAME"]),
								GRADE_NAME = Convert.ToString(reader["GRADE_NAME"]),
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

		private int GenerateBarCodeNextId(Common common)
		{
			try
			{
				var Menu = _menuRepository.GetMenu(common.MenuID);
				string? table = string.Empty;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE2;
				}

				if (!String.IsNullOrWhiteSpace(table))
				{
					string maxIdQuery = "SELECT ISNULL(MAX(OP_ID), 0) + 1 FROM " + table;
					using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
					{
						SqlCommand command = new SqlCommand(maxIdQuery, connection);
						connection.Open();
						object result = command.ExecuteScalar();
						return Convert.ToInt32(result);
					}
				}
			}
			catch (Exception ex)
			{

			}
			return 0;
		}

		public MyHttpResponseMessage SaveBarcode(BarcodeOpening modelRecord, Common common)
		{
			MyHttpResponseMessage response = new MyHttpResponseMessage();
			try
			{
				var Menu = _menuRepository.GetMenu(common.MenuID);
				string? table = string.Empty, detailTable = string.Empty;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE1;
					detailTable = menu.TABLE2;
				}

				if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
				{
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
							string query = "SELECT BAL_QTY FROM TBL_ITEM_OPENING WHERE OP_ID = '" + modelRecord.ITEM_OP_ID + "' AND DLT = 'T'";
							command.CommandText = query;
							object result1 = command.ExecuteScalar();
							int BAL_QTY = Convert.ToInt32(result1);

							query = "SELECT SUM(QTY) AS QTY FROM TBL_BARCODE_OP WHERE ITEM_OP_ID = '" + modelRecord.ITEM_OP_ID + "' AND DLT = 'T'";
							command.CommandText = query;
							object result2 = command.ExecuteScalar();
							int barcodeQty = result2 != DBNull.Value ? Convert.ToInt32(result2) : 0;
							if (barcodeQty == 0)
							{
								int TOTAL_QTY = Convert.ToInt32(modelRecord.QTY);
								if (TOTAL_QTY <= BAL_QTY)
								{
									if (modelRecord.OP_ID == null || modelRecord.OP_ID == 0)
									{
										query = "INSERT INTO " + detailTable + "" +
												" (OP_ID, ITEM_OP_ID, RATE, BARCODE," +
												" QTY, UNIT," +
												" ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS," +
												" EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS," +
												" ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID," +
												" DLT, BCODE, PERIOD_ID)" +
												" VALUES" +
												"('" + GenerateBarCodeNextId(common) + "', '" + modelRecord.ITEM_OP_ID + "', '" + modelRecord.RATE + "', '" + modelRecord.BARCODE + "'," +
												"'" + modelRecord.QTY + "', '" + modelRecord.UNIT + "'," +
												"'" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "', '" + Ip + "'," +
												"'" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "', '" + Ip + "'," +
												"'" + Postal + "', '" + Postal + "', 'Y','" + common.MenuID + "'," +
												"'T', '" + common.Branch + "', '" + common.Period + "')";
										command.CommandText = query;
										command.ExecuteNonQuery();
										transaction.Commit();
										response.msgType = 1;
										response.msg = "Record Added Successfully";
									}
									else
									{
										query = "UPDATE " + detailTable + " SET RATE = '" + modelRecord.RATE + @"',
												BARCODE = '" + modelRecord.BARCODE + @"',
												QTY = '" + modelRecord.QTY + @"',
												UNIT = '" + modelRecord.UNIT + @"',
												EDIT_USER_ID = '" + username + @"',
												EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
												EDIT_COMPUTER_NAME = '" + Computer + @"',
												EDIT_IP_ADDRESS = '" + Ip + @"',
												EDIT_POSTALCODE = '" + Postal + @"',
												ASTATUS = '" + modelRecord.ASTATUS + @"'
												WHERE OP_ID = '" + modelRecord.OP_ID + @"'";
										command.CommandText = query;
										command.ExecuteNonQuery();
										transaction.Commit();
										response.msgType = 1;
										response.msg = "Record Updated Successfully";
									}
								}
								else
								{
									transaction.Rollback();
									response.msg = "Not enough quantity available or invalid input balance quantity.";
									response.msgType = 2;
								}
							}
							else
							{
								int Qty = Convert.ToInt32(modelRecord.QTY);
								int TOTAL_QTY = Qty + barcodeQty;
								if (TOTAL_QTY <= BAL_QTY)
								{
									if (modelRecord.OP_ID == null || modelRecord.OP_ID == 0)
									{
										query = "INSERT INTO " + detailTable + "" +
												" (OP_ID, ITEM_OP_ID, RATE, BARCODE," +
												" QTY, UNIT," +
												" ADD_USER_ID, ADD_DATE, ADD_COMPUTER_NAME, ADD_IP_ADDRESS," +
												" EDIT_USER_ID, EDIT_DATE, EDIT_COMPUTER_NAME, EDIT_IP_ADDRESS," +
												" ADD_POSTALCODE, EDIT_POSTALCODE, ASTATUS, MENU_ID," +
												" DLT, BCODE , PERIOD_ID)" +
												" VALUES" +
												"('" + GenerateBarCodeNextId(common) + "', '" + modelRecord.ITEM_OP_ID + "', '" + modelRecord.RATE + "', '" + modelRecord.BARCODE + "'," +
												"'" + modelRecord.QTY + "', '" + modelRecord.UNIT + "'," +
												"'" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "', '" + Ip + "'," +
												"'" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "', '" + Ip + "'," +
												"'" + Postal + "', '" + Postal + "', 'Y','" + common.MenuID + "'," +
                                                "'T' , '" + common.Branch + "' , '" + common.Period + "')";
										command.CommandText = query;
										command.ExecuteNonQuery();
										transaction.Commit();
										response.msgType = 1;
										response.msg = "Record Added Successfully";
									}
									else
									{
										query = "UPDATE " + detailTable + " SET RATE = '" + modelRecord.RATE + @"',
												BARCODE = '" + modelRecord.BARCODE + @"',
												QTY = '" + modelRecord.QTY + @"',
												UNIT = '" + modelRecord.UNIT + @"',
												EDIT_USER_ID = '" + username + @"',
												EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
												EDIT_COMPUTER_NAME = '" + Computer + @"',
												EDIT_IP_ADDRESS = '" + Ip + @"',
												EDIT_POSTALCODE = '" + Postal + @"',
												ASTATUS = '" + modelRecord.ASTATUS + @"'
												WHERE OP_ID = '" + modelRecord.OP_ID + @"'";
										command.CommandText = query;
										command.ExecuteNonQuery();
										transaction.Commit();
										response.msgType = 1;
										response.msg = "Record Updated Successfully";
									}
								}
								else
								{
									transaction.Rollback();
									response.msgType = 2;
									response.msg = "Not enough quantity available or invalid input balance quantity.";
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
				response.msgType = 2;
				response.msg = _catchMessage;
			}
			return response;
		}

		public MyHttpResponseMessage GetBarcodeByCode(int code, Common common)
		{
			MyHttpResponseMessage response = new MyHttpResponseMessage();
			try
			{
				var Menu = _menuRepository.GetMenu(common.MenuID);
				string? table = string.Empty;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE2;
				}

				if (!String.IsNullOrWhiteSpace(table))
				{
					using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
					{
						string query = " SELECT OP_ID,ITEM_OP_ID,BARCODE," +
									   " QTY,RATE,UNIT,ASTATUS" +
									   " FROM " + table + " WHERE DLT = 'T' AND OP_ID = '" + code + "'";
						SqlCommand command = new SqlCommand(query, connection);
						connection.Open();
						SqlDataReader reader = command.ExecuteReader();
						if (reader.Read())
						{
							var jsonDataResult = new
							{
								ID = Convert.ToInt32(reader["OP_ID"]),
								ASTATUS = Convert.ToString(reader["ASTATUS"]),
								ITEM_OP_ID = Convert.ToString(reader["ITEM_OP_ID"]),
								RATE = Convert.ToString(reader["RATE"]),
								QTY = Convert.ToString(reader["QTY"]),
								UNIT = Convert.ToString(reader["UNIT"]),
								BARCODE = Convert.ToString(reader["BARCODE"])
							};
							response.msgType = 1;
							response.data = jsonDataResult;
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

		public MyHttpResponseMessage DeleteBarcode(int code, Common common)
		{
			MyHttpResponseMessage response = new MyHttpResponseMessage();
			response.msgType = 2;
			response.msg = "Data not found in our records";
			try
			{
				var Menu = _menuRepository.GetMenu(common.MenuID);
				string? table = string.Empty, detailTable = string.Empty;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE2;
				}

				if (!String.IsNullOrWhiteSpace(table))
				{
					if (code == 0)
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
							string query = $"UPDATE {table} SET DLT = 'F' WHERE OP_ID = '{code}'";
							SqlCommand command = new SqlCommand(query, connection);
							command.ExecuteNonQuery();
							response.msgType = 1;
							response.msg = "Record Deleted Successfully";
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