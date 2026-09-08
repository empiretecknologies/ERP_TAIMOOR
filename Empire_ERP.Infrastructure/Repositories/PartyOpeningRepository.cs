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

namespace Empire_ERP.Infrastructure.Repositories
{
    public class PartyOpeningRepository : IPartyOpeningRepository
	{
        public IMenuRepository _menuRepository { get; set; }

        public PartyOpeningRepository(IMenuRepository menuRepository)
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
				int? pType = 0;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.PICK_TABLE_MASTER;
					pType = menu.PTYPE;
				}

				if (!String.IsNullOrWhiteSpace(table) && pType > 0)
				{
					List<object> jsonDataResult = new List<object>();
					using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
					{
                        string query = @$"SELECT PT.PARTY_CODE, PT.PARTY_TYPE_CODE, PT.PARTY_NAME, 
						PT.PARTY_SHORT_NAME, PT.ACT_CODE, CH.ACT_NAME, CT.GROUP_NAME AS CATEGORY, 
						PT.CONTACT_PERSON, PT.CELL, PT.PAYMENT_TERMS, PT.CREDIT_LIMIT, PT.S_CODE, 
						SM.PARTY_NAME AS SALESPERSON, PT.SACT_CODE, SMCH.ACT_NAME AS SALES_ACT_NAME, PT.CAT_CODE,
						CASE WHEN MAX(OP.DC_TYPE) = 'C' THEN 'Credit' 
						WHEN MAX(OP.DC_TYPE) = 'D' THEN  'Debit'  
						WHEN MAX(OP.DC_TYPE) IS NULL THEN  CASE WHEN 3 = {pType} THEN 'Credit' else 'Debit' End 
						End AS DC_TYPE,
						CASE WHEN OP.ASTATUS = 'Y' THEN 'Active' Else 'Inactive' End AS ASTATUS,
						ABS(ISNULL((SELECT SUM(TAMT) FROM TBL_PARTY_TYPES_OP WHERE PARTY_CODE = OP.PARTY_CODE AND DLT = 'T' AND ACT_CODE = OP.ACT_CODE AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}
						AND DC_TYPE= 'D'),0)-
						ISNULL((SELECT SUM(TAMT) FROM TBL_PARTY_TYPES_OP WHERE PARTY_CODE = OP.PARTY_CODE AND DLT = 'T' AND ACT_CODE = OP.ACT_CODE AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}
						AND DC_TYPE= 'C'),0)) AS TAMT
						FROM TBL_PARTY_TYPES PT 
						LEFT OUTER JOIN TBL_CHART CH ON CH.ACT_CODE = PT.ACT_CODE 
						LEFT OUTER JOIN TBL_CATEGORY CT ON CT.GROUP_CODE = PT.CAT_CODE 
						LEFT OUTER JOIN TBL_PARTY_TYPES SM ON SM.PARTY_CODE = PT.S_CODE AND SM.ACT_CODE = PT.SACT_CODE 
						LEFT OUTER JOIN TBL_CHART SMCH ON SMCH.ACT_CODE = PT.SACT_CODE
						LEFT OUTER JOIN TBL_PARTY_TYPES_OP OP 
						ON PT.PARTY_CODE = OP.PARTY_CODE AND PT.ACT_CODE = OP.ACT_CODE AND  OP.BCODE = {common.Branch} AND OP.PERIOD_ID = {common.Period}
						WHERE PT.DLT = 'T' AND PT.ASTATUS = 'Y' AND PT.PARTY_TYPE_CODE = '{pType}'
						GROUP BY 
						PT.PARTY_CODE, PT.PARTY_TYPE_CODE, PT.PARTY_NAME, 
						PT.PARTY_SHORT_NAME, PT.ACT_CODE, CH.ACT_NAME, CT.GROUP_NAME , 
						PT.CONTACT_PERSON, PT.CELL, PT.PAYMENT_TERMS, PT.CREDIT_LIMIT, PT.S_CODE, 
						SM.PARTY_NAME , PT.SACT_CODE, SMCH.ACT_NAME , PT.CAT_CODE,OP.ASTATUS,OP.PARTY_CODE,OP.ACT_CODE,OP.BCODE,OP.PERIOD_ID ORDER BY PT.PARTY_NAME";
                        //AND OP.DLT = 'T'
                        SqlCommand command = new SqlCommand(query, connection);
						connection.Open();
						SqlDataReader reader = command.ExecuteReader();
						while (reader.Read())
						{
							var row = new
							{
								ID = Convert.ToString(reader["PARTY_CODE"]),
								PARTY_TYPE_CODE = Convert.ToString(reader["PARTY_TYPE_CODE"]),
								PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]),
								PARTY_SHORT_NAME = Convert.ToString(reader["PARTY_SHORT_NAME"]),
								ACT_CODE = Convert.ToString(reader["ACT_CODE"]),
								ACT_NAME = Convert.ToString(reader["ACT_NAME"]),
								GROUP_NAME = Convert.ToString(reader["PARTY_TYPE_CODE"]),
								DC_TYPE = Convert.ToString(reader["DC_TYPE"]),
								AMOUNT = Convert.ToString(reader["TAMT"]),
								ASTATUS = Convert.ToString(reader["ASTATUS"]),
								CAT_CODE = Convert.ToString(reader["CAT_CODE"]),
								CATEGORY = Convert.ToString(reader["CATEGORY"]),
								CONTACT_PERSON = Convert.ToString(reader["CONTACT_PERSON"]),
								CELL = Convert.ToString(reader["CELL"]),
								PAYMENT_TERMS = Convert.ToString(reader["PAYMENT_TERMS"]),
								CREDIT_LIMIT = Convert.ToString(reader["CREDIT_LIMIT"]),
								S_CODE = Convert.ToString(reader["S_CODE"]),
								SALESPERSON = Convert.ToString(reader["SALESPERSON"]),
								SACT_CODE = Convert.ToString(reader["SACT_CODE"]),
								SALES_ACT_NAME = Convert.ToString(reader["SALES_ACT_NAME"])
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

        public MyHttpResponseMessage GetPartyOpeningDetails(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty, prefix = string.Empty, dcType = string.Empty;
                int? pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    pType = menu.PTYPE;
					prefix = menu.PERFIX;
					dcType = menu.DCTYPE;
                }

                if (!String.IsNullOrWhiteSpace(table) && pType > 0)
                {
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
                        string query = "SELECT PARTY_CODE,PARTY_TYPE_CODE,PARTY_NAME,ACT_CODE,PAYMENT_TERMS,S_CODE,SACT_CODE " +
									   " FROM TBL_PARTY_TYPES" +
									   $" WHERE PARTY_CODE = '{code}' AND PARTY_TYPE_CODE = '{pType}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = Convert.ToString(reader["PARTY_CODE"]),
                                PARTY_TYPE_CODE = Convert.ToString(reader["PARTY_TYPE_CODE"]),
                                PARTY_NAME = Convert.ToString(reader["PARTY_NAME"]),
                                ACT_CODE = Convert.ToString(reader["ACT_CODE"]),
                                PAYMENT_TERMS = Convert.ToString(reader["PAYMENT_TERMS"]),
                                S_CODE = Convert.ToString(reader["S_CODE"]),
                                Type = prefix,
                                SACT_CODE = Convert.ToString(reader["SACT_CODE"]),
                                DC_TYPE = dcType
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

        public MyHttpResponseMessage GetPartyOpeningDetailByCode(int code, Common common)
		{
			MyHttpResponseMessage response = new MyHttpResponseMessage();
			try
			{
				var Menu = _menuRepository.GetMenu(common.MenuID);
				string? table = string.Empty;
				int? pType = 0;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE1;
					pType = menu.PTYPE;
				}

				if (!String.IsNullOrWhiteSpace(table) && pType > 0)
				{
					List<object> jsonDataResult = new List<object>();
					using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
					{
						string query = "SELECT OP.OP_ID,OP.PARTY_CODE ,OP.CURR_CODE, OP.BTYPE," +
									   " CASE WHEN OP.ASTATUS = 'Y' THEN 'Active'" +
									   " WHEN OP.ASTATUS = 'N' then 'In-Active'" +
									   " ELSE ''" +
									   " END AS ASTATUS," +
									   " OP.BILL_NO,OP.BILL_DATE ," +
									   " CASE WHEN OP.DC_TYPE = 'D' THEN 'Debit'" +
									   " WHEN OP.DC_TYPE = 'C' then 'Credit'" +
									   " ELSE '' END AS DC_TYPE," +
									   " OP.AMOUNT,OP.STAX_AMT,OP.TAMT,OP.COMM_AMT,OP.TERMS ," +
									   " OP.SALES_CODE,SM.PARTY_NAME AS SALESPERSON,OP.SALESACT_CODE ,SMCH.ACT_NAME AS SALES_ACT_NAME," +
									   " OP.DDESC ," +
									   " ISNULL(CRR.DESCR, 0) AS CURR_NAME, ISNULL(OP.CRATE, 0)  AS CRATE," +
									   " OP.ADD_USER_ID,OP.ADD_DATE,OP.ADD_IP_ADDRESS,OP.EDIT_USER_ID,OP.EDIT_DATE,OP.EDIT_COMPUTER_NAME," +
									   " OP.EDIT_IP_ADDRESS,OP.ADD_POSTALCODE,OP.EDIT_POSTALCODE,OP.ADD_COMPUTER_NAME,OP.MENU_ID" +
									   $" FROM {table} OP" +
									   " LEFT OUTER JOIN TBL_PARTY_TYPES SM" +
									   " ON SM.PARTY_CODE = OP.SALES_CODE AND SM.ACT_CODE = OP.SALESACT_CODE" +
									   " LEFT OUTER JOIN TBL_CHART SMCH" +
									   " ON SMCH.ACT_CODE = OP.SALESACT_CODE" +
									   " LEFT OUTER JOIN TBL_CURRENCY CRR" +
									   " ON CRR.CODE = OP.CURR_CODE" +
									   " WHERE OP.DLT = 'T' AND OP.ASTATUS = 'Y'" +
									   " AND OP.PARTYTYPE_CODE = '" + pType + "' AND OP.PARTY_CODE = '" + code + "' AND OP.DLT = 'T' AND OP.BCODE = '" + common.Branch + "' AND OP.PERIOD_ID = '" + common.Period +"'";
						SqlCommand command = new SqlCommand(query, connection);
						connection.Open();
						SqlDataReader reader = command.ExecuteReader();
						while (reader.Read())
						{
							var row = new
							{
								ID = Convert.ToString(reader["OP_ID"]),
								PARTY_CODE = Convert.ToString(reader["PARTY_CODE"]),
								BTYPE = Convert.ToString(reader["BTYPE"]),
								ASTATUS = Convert.ToString(reader["ASTATUS"]),
								BILL_NO = Convert.ToString(reader["BILL_NO"]),
								BILL_DATE = Convert.ToString(reader["BILL_DATE"]),
								DC_TYPE = Convert.ToString(reader["DC_TYPE"]),
								AMOUNT = Convert.ToString(reader["AMOUNT"]),
								STAX_AMT = Convert.ToString(reader["STAX_AMT"]),
								TAMT = Convert.ToString(reader["TAMT"]),
								COMM_AMT = Convert.ToString(reader["COMM_AMT"]),
								TERMS = Convert.ToString(reader["TERMS"]),
								SALES_CODE = Convert.ToString(reader["SALES_CODE"]),
								CURR_CODE = Convert.ToString(reader["CURR_CODE"]),
                                CURR_NAME = Convert.ToString(reader["CURR_NAME"]),
                                SALESACT_CODE = Convert.ToString(reader["SALESACT_CODE"]),
								DDESC = Convert.ToString(reader["DDESC"]),
								CRATE = Convert.ToString(reader["CRATE"]),
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

		private int GenerateNextId(Common common)
        {
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
				string? table = string.Empty;
				int? pType = 0;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE1;
					pType = menu.PTYPE;
				}

				if (!String.IsNullOrWhiteSpace(table) && pType > 0)
				{
                    string maxIdQuery = $"SELECT ISNULL(MAX(OP_ID), 0) + 1 FROM {table} WHERE PARTYTYPE_CODE = '{pType}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'"; 
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

        public MyHttpResponseMessage Save(PartyOpening modelRecord, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                int? pType = 0;
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
                    pType = menu.PTYPE;
                }

                if (!String.IsNullOrWhiteSpace(table) && pType > 0)
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
                            if (modelRecord.OP_ID == null || modelRecord.OP_ID == 0)
                            {
								query = "INSERT INTO TBL_PARTY_TYPES_OP (OP_ID,BTYPE,BILL_NO,BILL_DATE," +
										"DC_TYPE,AMOUNT,STAX_AMT,TAMT,COMM_AMT," +
										"TERMS,SALES_CODE,SALESACT_CODE,DDESC," +
										"PARTY_CODE,ACT_CODE,PARTYTYPE_CODE,CURR_CODE," +
										"CRATE,ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME," +
										"ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME," +
										"EDIT_IP_ADDRESS,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS," +
                                        "MENU_ID,DLT,BCODE,PERIOD_ID)" +
										"VALUES" +
										"('" + GenerateNextId(common) + "', '" + modelRecord.BTYPE + "', '" + modelRecord.BILL_NO + "', '" + modelRecord.BILL_DATE + "'," +
										"'" + modelRecord.DC_TYPE + "', '" + modelRecord.AMOUNT + "', '" + modelRecord.STAX_AMT + "', '" + modelRecord.TAMT + "', '" + modelRecord.COMM_AMT + "'," +
										"'" + modelRecord.TERMS + "', '" + modelRecord.SALES_CODE + "', '" + modelRecord.SALESACT_CODE + "', '" + modelRecord.DDESC + "'," +
										"'" + modelRecord.PARTY_CODE + "', '" + modelRecord.ACT_CODE + "', '" + pType + "', '" + modelRecord.CURR_CODE + "'," +
										"'" + modelRecord.CRATE + "', '" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "'," +
										"'" + Ip + "', '" + username + "', '" + CommonService.GetDateTime("Pakistan Standard Time") + "', '" + Computer + "'," +
										"'" + Ip + "', '" + Postal + "', '" + Postal + "', '" + modelRecord.ASTATUS + "'," +
										"'" + common.MenuID + "', 'T', '" + common.Branch + "','"+ common.Period +"')";
								command.CommandText = query;
                                command.ExecuteNonQuery();
								transaction.Commit();
								response.data = modelRecord.OP_ID;
								response.msgType = 1;
								response.msg = "Record Added Successfully";
							}
                            else
                            {
								query = "UPDATE TBL_PARTY_TYPES_OP SET BTYPE = '" + modelRecord.BTYPE + @"',
											 BILL_NO = '" + modelRecord.BILL_NO + @"',
											 BILL_DATE = '" + modelRecord.BILL_DATE + @"',
											 DC_TYPE = '" + modelRecord.DC_TYPE + @"',
											 AMOUNT = '" + modelRecord.AMOUNT + @"',
											 STAX_AMT = '" + modelRecord.STAX_AMT + @"',
											 TAMT = '" + modelRecord.TAMT + @"',
											 COMM_AMT = '" + modelRecord.COMM_AMT + @"',
											 TERMS = '" + modelRecord.TERMS + @"',
											 SALES_CODE = '" + modelRecord.SALES_CODE + @"',
											 SALESACT_CODE = '" + modelRecord.SALESACT_CODE + @"',
											 DDESC = '" + modelRecord.DDESC + @"',
											 CURR_CODE = '" + modelRecord.CURR_CODE + @"',
											 CRATE = '" + modelRecord.CRATE + @"',
											 EDIT_USER_ID = '" + username + @"',
											 EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
											 EDIT_COMPUTER_NAME = '" + Computer + @"',
											 EDIT_IP_ADDRESS = '" + Ip + @"',
											 EDIT_POSTALCODE = '" + Postal + @"',
											 ASTATUS = '" + modelRecord.ASTATUS + @"'
											 WHERE OP_ID = '" + modelRecord.OP_ID + @"' AND BCODE = '"+ common.Branch +"' AND PERIOD_ID = '"+ common.Period +"' AND PARTYTYPE_CODE = '"+ pType +"'";
								command.CommandText = query;
								command.ExecuteNonQuery();

								transaction.Commit();
								response.data = modelRecord.OP_ID;
								response.msgType = 1;
								response.msg = "Record Updated Successfully";
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

        public MyHttpResponseMessage GetPartyOpeningByCode(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
				string? table = string.Empty;
				int? pType = 0;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE1;
					pType = menu.PTYPE;
				}

				if (!String.IsNullOrWhiteSpace(table) && pType > 0)
				{
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {
						string query = "SELECT OP.OP_ID,OP.PARTY_CODE , OP.CURR_CODE, OP.BTYPE," +
									   " OP.ASTATUS," +
									   " OP.BILL_NO,OP.BILL_DATE ," +
									   " OP.DC_TYPE," +
									   " OP.AMOUNT,OP.STAX_AMT,OP.TAMT,OP.COMM_AMT,OP.TERMS ," +
									   " OP.SALES_CODE,SM.PARTY_NAME AS SALESPERSON,OP.SALESACT_CODE ,SMCH.ACT_NAME AS SALES_ACT_NAME," +
									   " OP.DDESC ," +
									   " ISNULL(CRR.DESCR, 0) AS CURR_NAME, ISNULL(OP.CRATE, 0)  AS CRATE" +
									   $" FROM {table} OP" +
									   " LEFT OUTER JOIN TBL_PARTY_TYPES SM" +
									   " ON SM.PARTY_CODE = OP.SALES_CODE AND SM.ACT_CODE = OP.SALESACT_CODE" +
									   " LEFT OUTER JOIN TBL_CHART SMCH" +
									   " ON SMCH.ACT_CODE = OP.SALESACT_CODE" +
									   " LEFT OUTER JOIN TBL_CURRENCY CRR" +
									   " ON CRR.CODE = OP.CURR_CODE" +
									   " WHERE OP.DLT = 'T' AND OP.ASTATUS = 'Y'" +
									   $" AND OP.PARTYTYPE_CODE = '{pType}' AND OP.OP_ID = '{code}' AND OP.DLT = 'T' AND OP.BCODE = '{common.Branch}' AND OP.PERIOD_ID = '{common.Period}'";
						SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
							var jsonDataResult = new
							{
								ID = Convert.ToInt32(reader["OP_ID"]),
								PARTY_CODE = Convert.ToString(reader["PARTY_CODE"]),
								BTYPE = Convert.ToString(reader["BTYPE"]),
								ASTATUS = Convert.ToString(reader["ASTATUS"]),
								BILL_NO = Convert.ToString(reader["BILL_NO"]),
								BILL_DATE = reader["BILL_DATE"] == DBNull.Value ? null : Convert.ToDateTime(reader["BILL_DATE"]).ToString("yyyy-MM-dd"),
								DC_TYPE = Convert.ToString(reader["DC_TYPE"]),
								AMOUNT = Convert.ToString(reader["AMOUNT"]),
								CURR_CODE = Convert.ToString(reader["CURR_CODE"]),
								STAX_AMT = Convert.ToString(reader["STAX_AMT"]),
								TAMT = Convert.ToString(reader["TAMT"]),
								COMM_AMT = Convert.ToString(reader["COMM_AMT"]),
								TERMS = Convert.ToString(reader["TERMS"]),
								SALES_CODE = Convert.ToString(reader["SALES_CODE"]),
								SALESACT_CODE = Convert.ToString(reader["SALESACT_CODE"]),
								DDESC = Convert.ToString(reader["DDESC"]),
								CRATE = Convert.ToString(reader["CRATE"]),
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
				string? table = string.Empty;
				int? pType = 0;
				if (Menu.data != null)
				{
					var menu = (Menu)Menu.data;
					table = menu.TABLE1;
					pType = menu.PTYPE;
				}

				if(!String.IsNullOrWhiteSpace(table) && pType > 0)
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
							string query = $"UPDATE {table} SET DLT = 'F' WHERE OP_ID = '{code}' AND PARTYTYPE_CODE = '{pType}' AND BCODE = '{common.Branch}' AND PERIOD_ID = '{common.Period}'";
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