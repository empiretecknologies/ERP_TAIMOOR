using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using SkiaSharp;
using System.Drawing.Drawing2D;
using System.Text;
using System.Text.Json.Nodes;
using ZXing.QrCode.Internal;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class ItemMasterRepository : IItemMasterRepository
    {
        public ICommonRepository _commonRepository { get; set; }
        public IMenuRepository _menuRepository { get; set; }
        public IPeriodRepository _periodRepository { get; set; }

        public ItemMasterRepository(IMenuRepository menuRepository, ICommonRepository commonRepository, IPeriodRepository periodRepository)
        {
            _menuRepository = menuRepository;
            _commonRepository = commonRepository;
            _periodRepository = periodRepository;
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
                if (Menu.data != null)
                {
                    var menu = (Menu)Menu.data;
                    table = menu.TABLE1;
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
                            //string query1 = @$"IF NOT EXISTS(SELECT 1 FROM TBL_BARCODE WHERE DLT = 'T' AND ASTATUS = 'Y' AND ITEM_CODE = {code})
                            //                BEGIN
                            //                    UPDATE {table} SET DLT = 'F' WHERE ITEM_CODE = {code};
                            //                    SELECT 1;
                            //                END
                            //                ELSE
                            //                BEGIN
                            //                    SELECT 2;
                            //                END";

                            string query = $@"IF NOT EXISTS (SELECT 1 FROM TBL_BARCODE WHERE DLT = 'T' AND ASTATUS = 'Y' AND ITEM_CODE = {code})
                                                BEGIN UPDATE {table} SET DLT = 'F' WHERE ITEM_CODE = {code};
                                                UPDATE TBL_ITEM_ATT SET DLT = 'F' WHERE ITEM_CODE = {code};
                                                SELECT 1;
                                                END
                                                ELSE
                                                BEGIN
                                                    SELECT 2;
                                                END
                                                ";
                            SqlCommand command = new SqlCommand(query, connection);
                            int result = (int)command.ExecuteScalar();
                            connection.Close();
                            response.msgType = result;
                            if (response.msgType == 1)
                                response.msg = "Record Deleted Successfully!";
                            else
                                response.msg = "Please Delete Barcodes First!";
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

        public MyHttpResponseMessage AttributeDelete(int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string connectionString = new SQLService().getconnstring();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE TBL_ITEM_ATT SET DLT = 'F' WHERE CODE = '{code}'";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.ExecuteNonQuery();
                    response.msgType = 1;
                    response.msg = "Record Deleted Successfully";
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
                    string maxIdQuery = "SELECT ISNULL(MAX(ITEM_CODE), 0) + 1 FROM " + table;
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

        public MyHttpResponseMessage Save(ItemMaster modelRecord, Common common)
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
                            if (modelRecord.BITYPE == 2 && string.IsNullOrEmpty(modelRecord.BARCODE))
                            {
                                modelRecord.BARCODE = GenerateAutoBarCodeForItem(common, command);
                            }
                            string query = "";
                            string Duplicationquery = "";
                            string barcodeCheckQuery = "";
                            if (modelRecord.ITEM_CODE == null || modelRecord.ITEM_CODE == 0)
                            {
                                if (!string.IsNullOrEmpty(modelRecord.BARCODE))
                                {
                                    barcodeCheckQuery = "SELECT COUNT(*) FROM " + table +
                                                              " WHERE BARCODE = '" + modelRecord.BARCODE + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
                                    command.CommandText = barcodeCheckQuery;
                                    int barcodeCount = (int)command.ExecuteScalar();

                                    if (barcodeCount > 0)
                                    {
                                        transaction.Rollback();
                                        response.msg = "Barcode already exists!";
                                        response.msgType = 2;
                                        return response;
                                    }
                                }

                                var newItemCode = GenerateNextId(common);
                                query = "INSERT INTO " + table + " " +
                                        "(ITEM_CODE,ITEM_ID,COLOR,SIZE,ITEM_NAME,ITEM_SHORT_NAME,BARCODE_TYPE,BARCODE,WEIGHT," +
                                        "REMARKS,GROUP_CODE,IUNIT_CODE,PACK,PUNIT_CODE,SALE_RATE," +
                                        "PURCHASE_RATE,SALESTAX,ITAX_STATUS,ITEM_MAX,ITEM_MINI,IPIC," +
                                        "ASTATUS,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
                                        "EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                        "ADD_POSTALCODE,EDIT_POSTALCODE,MENU_ID," +
                                        "ADD_USER_ID,DLT,GRADE,ITEM_TYPE, CAT_CODE, SUB_CAT_CODE,HS_CODE)" +
                                        "VALUES" +
                                        "('" + newItemCode + "','" + modelRecord.ITEM_ID + "','" + modelRecord.COLOR + "','" + modelRecord.SIZE + "','" + modelRecord.ITEM_NAME + "','" + modelRecord.ITEM_SHORT_NAME + "','" + modelRecord.BITYPE + "','" + modelRecord.BARCODE + "','" + modelRecord.WEIGHT + "'," +
                                        "'" + modelRecord.REMARKS + "','" + modelRecord.GROUP_CODE + "','" + modelRecord.IUNIT_CODE + "','" + modelRecord.PACK + "','" + modelRecord.PUNIT_CODE + "','" + modelRecord.SALE_RATE + "'," +
                                        "'" + modelRecord.PURCHASE_RATE + "','" + modelRecord.SALESTAX + "','" + modelRecord.ITAX_STATUS + "','" + modelRecord.ITEM_MAX + "','" + modelRecord.ITEM_MIN + "','" + modelRecord.IPIC + "'," +
                                        "'" + modelRecord.ASTATUS + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                        "'" + common.Username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "'," +
                                        "'" + Postal + "','" + Postal + "','" + common.MenuID + "'," +
                                        "'" + common.Username + "','T','" + modelRecord.GRADE + "','" + modelRecord.ITEM_TYPE + "','" + modelRecord.CAT_CODE + "','" + modelRecord.SUB_CAT_CODE + "','" + modelRecord.HS_CODE + "')";
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE ITEM_NAME = '" + modelRecord.ITEM_NAME + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();

                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.data = newItemCode; 
                                    response.copyItemCode = newItemCode;
                                    response.msgType = 1;
                                    response.msg = "Record Added Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msg = "Name Already Exist !....";
                                    response.msgType = 2;
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(modelRecord.BARCODE))
                                {
                                    barcodeCheckQuery = "SELECT COUNT(*) FROM " + table +
                                                               " WHERE BARCODE = '" + modelRecord.BARCODE +
                                                               "' AND MENU_ID = '" + common.MenuID +
                                                               "' AND ITEM_CODE != '" + modelRecord.ITEM_CODE +
                                                               "' AND DLT = 'T'";
                                    command.CommandText = barcodeCheckQuery;
                                    int barcodeCount = (int)command.ExecuteScalar();

                                    if (barcodeCount > 0)
                                    {
                                        transaction.Rollback();
                                        response.msg = "Barcode already exists!";
                                        response.msgType = 2;
                                        return response;
                                    }
                                }

                                query = "UPDATE " + table + " SET ITEM_ID = '" + modelRecord.ITEM_ID + @"',
                                            ITEM_NAME = '" + modelRecord.ITEM_NAME + @"',
                                            WEIGHT = '" + modelRecord.WEIGHT + @"',
                                            ITEM_SHORT_NAME = '" + modelRecord.ITEM_SHORT_NAME + @"',
                                            BARCODE_TYPE = '" + modelRecord.BITYPE + @"',
                                            BARCODE = '" + modelRecord.BARCODE + @"',
                                            REMARKS = '" + modelRecord.REMARKS + @"',
                                            COLOR = '" + modelRecord.COLOR + @"',
                                            SIZE = '" + modelRecord.SIZE + @"',
                                            GROUP_CODE = '" + modelRecord.GROUP_CODE + @"',
                                            IUNIT_CODE = '" + modelRecord.IUNIT_CODE + @"',
                                            PACK = '" + modelRecord.PACK + @"',
                                            HS_CODE = '" + modelRecord.HS_CODE + @"',
                                            PUNIT_CODE = '" + modelRecord.PUNIT_CODE + @"',
                                            SALE_RATE = '" + modelRecord.SALE_RATE + @"',
                                            PURCHASE_RATE = '" + modelRecord.PURCHASE_RATE + @"',
                                            SALESTAX = '" + modelRecord.SALESTAX + @"',
                                            ITAX_STATUS = '" + modelRecord.ITAX_STATUS + @"',
                                            ITEM_MAX = '" + modelRecord.ITEM_MAX + @"',
                                            ITEM_MINI = '" + modelRecord.ITEM_MIN + @"',
                                            IPIC = '" + modelRecord.IPIC + @"',
                                            ASTATUS = '" + modelRecord.ASTATUS + @"',
                                            EDIT_USER_ID = '" + common.Username + @"',
                                            EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                            EDIT_COMPUTER_NAME = '" + Computer + @"',
                                            EDIT_IP_ADDRESS = '" + Ip + @"',
                                            EDIT_POSTALCODE = '" + Postal + @"',
                                            GRADE = '" + modelRecord.GRADE + @"',
                                            ITEM_TYPE = '" + modelRecord.ITEM_TYPE + @"',
                                            CAT_CODE = '" + modelRecord.CAT_CODE + @"',
                                            SUB_CAT_CODE = '" + modelRecord.SUB_CAT_CODE + @"'
                                            WHERE ITEM_CODE = '" + modelRecord.ITEM_CODE + @"'";
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE ITEM_NAME = '" + modelRecord.ITEM_NAME + "' AND MENU_ID = '" + common.MenuID + "' AND DLT = 'T'";
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();

                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.data = modelRecord.ITEM_CODE;
                                    response.msgType = 1;
                                    response.msg = "Record Updated Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    response.msg = "Name Already Exist !....";
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

        public MyHttpResponseMessage CopyRecordOld(CopyRecord record, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string? table = menu.TABLE1;
                string? table2 = menu.TABLE2;
                string connectionString = new SQLService().getconnstring();
                ItemMaster itemMaster = new ItemMaster();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE ITEM_CODE = {record.TRAN_ID} AND DLT = 'T'";
                    //string detailQuery = $@"SELECT * FROM {table2} WHERE TRAN_ID = {record.TRAN_ID} AND DLT = 'T' AND BCODE = {common.Branch} AND PERIOD_ID = {common.Period}";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        itemMaster = new ItemMaster
                        {
                            ITEM_CODE = 0,
                            ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                            ITEM_NAME = record.ITEM_NAME,
                            ITEM_SHORT_NAME = Convert.ToString(reader["ITEM_SHORT_NAME"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            HS_CODE = Convert.ToString(reader["HS_CODE"]),
                            GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                            IUNIT_CODE = Convert.ToInt32(reader["IUNIT_CODE"]),
                            PACK = Convert.ToString(reader["PACK"]),
                            PUNIT_CODE = Convert.ToInt32(reader["PUNIT_CODE"]),
                            SALE_RATE = Convert.ToDouble(reader["SALE_RATE"]),
                            WEIGHT = Convert.ToDouble(reader["WEIGHT"]),
                            PURCHASE_RATE = Convert.ToDouble(reader["PURCHASE_RATE"]),
                            SALESTAX = Convert.ToDouble(reader["SALESTAX"]),
                            ITAX_STATUS = Convert.ToInt32(reader["ITAX_STATUS"]),
                            ITEM_MAX = Convert.ToDouble(reader["ITEM_MAX"]),
                            ITEM_MIN = Convert.ToDouble(reader["ITEM_MINI"]),
                            IPIC = Convert.ToString(reader["IPIC"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            GRADE = Convert.ToInt32(reader["GRADE"]),
                            ITEM_TYPE = Convert.ToString(reader["ITEM_TYPE"]),
                            CAT_CODE = Convert.ToInt32(reader["CAT_CODE"]),
                            SUB_CAT_CODE = Convert.ToInt32(reader["SUB_CAT_CODE"]),
                        };
                    }
                    reader.Close();
                    connection.Close();
                }
                response = this.Save(itemMaster, common);
                if (response.msgType == 1)
                {
                    response.msg = "Record Copied Successfully";
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

        public MyHttpResponseMessage CopyRecord(CopyRecord record, Common common, Menu menu)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                string? table = menu.TABLE1;
                string? table2 = menu.TABLE2;
                string connectionString = new SQLService().getconnstring();

                ItemMaster itemMaster = new ItemMaster();
                List<Barcode> barCodeList = new List<Barcode>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = $@"SELECT * FROM {table} WHERE ITEM_CODE = {record.TRAN_ID} AND DLT = 'T'";
                    string detailQuery = $@"SELECT * FROM {table2} WHERE ITEM_CODE = {record.TRAN_ID} AND DLT = 'T' ";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        itemMaster = new ItemMaster
                        {
                            ITEM_CODE = 0,
                            ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                            ITEM_NAME = record.ITEM_NAME,
                            ITEM_SHORT_NAME = Convert.ToString(reader["ITEM_SHORT_NAME"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            GROUP_CODE = Convert.ToInt32(reader["GROUP_CODE"]),
                            COLOR = Convert.ToInt32(reader["COLOR"]),
                            SIZE = Convert.ToInt32(reader["SIZE"]),
                            IUNIT_CODE = Convert.ToInt32(reader["IUNIT_CODE"]),
                            PACK = Convert.ToString(reader["PACK"]),
                            PUNIT_CODE = Convert.ToInt32(reader["PUNIT_CODE"]),
                            SALE_RATE = Convert.ToDouble(reader["SALE_RATE"]),
                            PURCHASE_RATE = Convert.ToDouble(reader["PURCHASE_RATE"]),
                            SALESTAX = Convert.ToDouble(reader["SALESTAX"]),
                            ITAX_STATUS = Convert.ToInt32(reader["ITAX_STATUS"]),
                            ITEM_MAX = Convert.ToDouble(reader["ITEM_MAX"]),
                            ITEM_MIN = Convert.ToDouble(reader["ITEM_MINI"]),
                            IPIC = Convert.ToString(reader["IPIC"]),
                            ASTATUS = Convert.ToString(reader["ASTATUS"]),
                            GRADE = Convert.ToInt32(reader["GRADE"]),
                            ITEM_TYPE = Convert.ToString(reader["ITEM_TYPE"]),
                            CAT_CODE = Convert.ToInt32(reader["CAT_CODE"]),
                            SUB_CAT_CODE = Convert.ToInt32(reader["SUB_CAT_CODE"]),
                        };
                    }

                    reader.Close();

                    SqlCommand barcode_Command = new SqlCommand(detailQuery, connection);
                    SqlDataReader barcode_Reader = barcode_Command.ExecuteReader();
                    while (barcode_Reader.Read())
                    {
                        var barCodeType = Convert.ToInt32(barcode_Reader["BARCODE_TYPE"]);
                        var barCodeValue = (barCodeType == 1) ? Convert.ToString(barcode_Reader["BARCODE"]) : "0";
                        var row = new Barcode
                        {
                            CODE = 0,
                            BARCODE_TYPE = barCodeType,
                            BARCODE = barCodeValue,
                            COLOR = Convert.ToInt32(barcode_Reader["COLOR"]),
                            SIZE = Convert.ToInt32(barcode_Reader["SIZE"]),
                            PRATE = Convert.ToDouble(barcode_Reader["PRATE"]),
                            SRATE = Convert.ToDouble(barcode_Reader["SRATE"]),
                            WSALE = Convert.ToDouble(barcode_Reader["WSALE"]),
                            RRATE = Convert.ToDouble(barcode_Reader["RRATE"]),
                            DRATE = Convert.ToDouble(barcode_Reader["DRATE"]),
                            ITEM_CODE = Convert.ToInt32(barcode_Reader["ITEM_CODE"]),
                            ASTATUS = Convert.ToString(barcode_Reader["ASTATUS"]),
                            BLABEL = Convert.ToInt32(barcode_Reader["BLABEL"]),
                        };
                        barCodeList.Add(row);
                    }

                    barcode_Reader.Close();
                    connection.Close();
                }

                response = this.Save(itemMaster, common);
                //int? newItemCode = response.copyItemCode;

                if (response.msgType == 1)
                {
                    response.msg = "Record Copied Successfully";
                    //var barcodeResponse = this.SaveCopiedBarcodeInfo(newItemCode,barCodeList, common);

                    //if (barcodeResponse.msgType == 1)
                    //{
                    //    response.msg = "Record Copied Successfully";
                    //}
                    //else
                    //{
                    //    response.msgType = 2;
                    //    response.msg = "Item saved but barcodes failed: " + barcodeResponse.msg;
                    //}
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

        //public MyHttpResponseMessage QuickSearch(Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty, detailTable = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //            detailTable = menu.TABLE2;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table) && !String.IsNullOrWhiteSpace(detailTable))
        //        {
        //            List<object> jsonDataResult = new List<object>();
        //            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //            {
        //                //string query = "SELECT A.ITEM_CODE,A.ITEM_ID,A.ITEM_NAME,A.ITEM_SHORT_NAME,A.REMARKS,A.GROUP_CODE,A.IUNIT_CODE,A.PACK,A.PUNIT_CODE,A.SALE_RATE," +
        //                //        "A.PURCHASE_RATE,A.SALESTAX,A.ITAX_STATUS,A.ITEM_MAX,A.ITEM_MINI,A.IPIC,A.ADD_DATE,A.ADD_COMPUTER_NAME,A.ADD_IP_ADDRESS,A.EDIT_USER_ID,A.EDIT_DATE," +
        //                //        "A.EDIT_COMPUTER_NAME,A.EDIT_IP_ADDRESS,A.ADD_POSTALCODE,A.EDIT_POSTALCODE,A.MENU_ID,A.ADD_USER_ID,A.GRADE, " +
        //                //        "CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, " +
        //                //        "CASE WHEN A.ITEM_TYPE = 'A' THEN 'Active' WHEN A.ITEM_TYPE = 'N' THEN 'Non Active' WHEN A.ITEM_TYPE = 'P' THEN 'Packing Material' WHEN A.ITEM_TYPE = 'F' THEN 'Finish Goods' ELSE '' END AS ITEM_TYPE, " +
        //                //        "B.GROUP_NAME AS CAT_CODE, C.GROUP_NAME AS SUB_CAT_CODE  " +
        //                //        "FROM " + table + " A " +
        //                //        "LEFT JOIN TBL_CATEGORY B ON A.CAT_CODE = B.GROUP_CODE " +
        //                //        "LEFT JOIN TBL_SUB_CATEGORY C ON A.SUB_CAT_CODE = C.GROUP_CODE " +
        //                //        "WHERE A.DLT = 'T' ORDER BY A.ITEM_CODE DESC";

        //                string query = $@"SELECT A.ITEM_CODE,A.ITEM_ID,A.ITEM_NAME,A.ITEM_SHORT_NAME,A.REMARKS,A.GROUP_CODE,A.IUNIT_CODE,A.PACK,A.PUNIT_CODE,A.SALE_RATE,A.PURCHASE_RATE,
        //                                A.SALESTAX,A.ITAX_STATUS,A.ITEM_MAX,A.ITEM_MINI,A.IPIC,A.ADD_DATE,A.ADD_COMPUTER_NAME,A.ADD_IP_ADDRESS,A.EDIT_USER_ID,A.EDIT_DATE,A.EDIT_COMPUTER_NAME,
        //                                A.EDIT_IP_ADDRESS,A.ADD_POSTALCODE,A.EDIT_POSTALCODE,A.MENU_ID,A.ADD_USER_ID,A.GRADE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS 
        //                                ASTATUS, CASE WHEN A.ITEM_TYPE = 'A' THEN 'Active' WHEN A.ITEM_TYPE = 'N' THEN 'Non Active' WHEN A.ITEM_TYPE = 'P' THEN 'Packing Material' WHEN 
        //                                A.ITEM_TYPE = 'F' THEN 'Finish Goods' ELSE '' END AS ITEM_TYPE, B.GROUP_NAME AS CAT_CODE, C.GROUP_NAME AS SUB_CAT_CODE, F.GROUP_NAME AS FABRIC,
        //                                S.GROUP_NAME AS SEASON, G.GROUP_NAME AS BRAND, ST.GROUP_NAME AS STYLE
        //                                FROM {table} A 
        //                                LEFT JOIN TBL_CATEGORY B ON A.CAT_CODE = B.GROUP_CODE 
        //                                LEFT JOIN TBL_SUB_CATEGORY C ON A.SUB_CAT_CODE = C.GROUP_CODE 
        //                                LEFT JOIN TBL_FABRIC F ON A.FABRIC = F.GROUP_CODE 
        //                                LEFT JOIN TBL_SEASON S ON A.SEASON = S.GROUP_CODE 
        //                                LEFT JOIN TBL_GRADE G ON A.GRADE = G.GROUP_CODE 
        //                                LEFT JOIN TBL_STYLE ST ON A.STYLE = ST.GROUP_CODE 
        //                                WHERE A.DLT = 'T' ORDER BY A.ITEM_CODE DESC";

        //                SqlCommand command = new SqlCommand(query, connection);
        //                connection.Open();
        //                SqlDataReader reader = command.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    var row = new
        //                    {
        //                        ID = reader["ITEM_CODE"],
        //                        ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
        //                        ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
        //                        ITEM_SHORT_NAME = Convert.ToString(reader["ITEM_SHORT_NAME"]),
        //                        REMARKS = Convert.ToString(reader["REMARKS"]),
        //                        GROUP_CODE = Convert.ToString(reader["GROUP_CODE"]),
        //                        IUNIT_CODE = Convert.ToString(reader["IUNIT_CODE"]),
        //                        PACK = Convert.ToString(reader["PACK"]),
        //                        PUNIT_CODE = Convert.ToString(reader["PUNIT_CODE"]),
        //                        SALE_RATE = Convert.ToString(reader["SALE_RATE"]),
        //                        PURCHASE_RATE = Convert.ToString(reader["PURCHASE_RATE"]),
        //                        SALESTAX = Convert.ToString(reader["SALESTAX"]),
        //                        ITAX_STATUS = Convert.ToString(reader["ITAX_STATUS"]),
        //                        ITEM_MAX = Convert.ToString(reader["ITEM_MAX"]),
        //                        ITEM_MINI = Convert.ToString(reader["ITEM_MINI"]),
        //                        IPIC = Convert.ToString(reader["IPIC"]),
        //                        ADD_DATE = Convert.ToString(reader["ADD_DATE"]),
        //                        ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
        //                        ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
        //                        EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
        //                        EDIT_DATE = Convert.ToString(reader["EDIT_DATE"]),
        //                        EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
        //                        EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
        //                        ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
        //                        EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
        //                        ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
        //                        GRADE = Convert.ToString(reader["GRADE"]),
        //                        ASTATUS = Convert.ToString(reader["ASTATUS"]),
        //                        ITEM_TYPE = Convert.ToString(reader["ITEM_TYPE"]),
        //                        CAT_CODE = Convert.ToString(reader["CAT_CODE"]),
        //                        SUB_CAT_CODE = Convert.ToString(reader["SUB_CAT_CODE"]),
        //                        FABRIC = Convert.ToString(reader["FABRIC"]),
        //                        SEASON = Convert.ToString(reader["SEASON"]),
        //                        BRAND = Convert.ToString(reader["BRAND"]),
        //                        STYLE = Convert.ToString(reader["STYLE"])
        //                    };
        //                    jsonDataResult.Add(row);

        //                }
        //                reader.Close();
        //            }

        //            response.data = jsonDataResult;
        //            response.msg = "";
        //            response.msgType = 1;
        //        }
        //        else
        //        {
        //            response.data = "";
        //            response.msg = "Something went wrong! please try again later.";
        //            response.msgType = 2;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        response.msg = _catchMessage;
        //        response.msgType = 2;
        //    }
        //    return response;
        //}

        public MyHttpResponseMessage QuickSearch(Common common)
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
                    List<object> jsonDataResult = new List<object>();
                    using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                    {

                        string query = $@"SELECT  A.ITEM_CODE,A.ITEM_ID,A.ITEM_NAME,A.ITEM_SHORT_NAME,IG.GROUP_NAME,U.GROUP_NAME AS UNIT,PU.GROUP_NAME AS PUNIT,A.HS_CODE,
                                A.PACK,A.REMARKS,A.GROUP_CODE,A.SALE_RATE,A.PURCHASE_RATE,A.SALESTAX,A.WEIGHT,
                                CASE WHEN A.ITAX_STATUS = '0' THEN 'Exempt' WHEN A.ITAX_STATUS = '1' THEN 'Taxable' ELSE '' END AS ITAX_STATUS,
                                A.ITEM_MAX,A.ITEM_MINI,A.IPIC,A.ADD_DATE,A.ADD_COMPUTER_NAME,A.ADD_IP_ADDRESS,A.EDIT_USER_ID,A.EDIT_DATE,A.EDIT_COMPUTER_NAME,
                                A.EDIT_IP_ADDRESS,A.ADD_POSTALCODE,A.EDIT_POSTALCODE,A.MENU_ID,A.ADD_USER_ID,A.GRADE, CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS 
                                ASTATUS, CASE WHEN A.ITEM_TYPE = 'A' THEN 'Active' WHEN A.ITEM_TYPE = 'N' THEN 'Non Active' WHEN A.ITEM_TYPE = 'P' THEN 'Packing Material' WHEN 
                                A.ITEM_TYPE = 'F' THEN 'Finish Goods' ELSE '' END AS ITEM_TYPE, B.GROUP_NAME AS CAT_CODE, C.GROUP_NAME AS SUB_CAT_CODE, F.GROUP_NAME AS FABRIC,
                                S.GROUP_NAME AS SEASON, G.GROUP_NAME AS BRAND, ST.GROUP_NAME AS STYLE
                                FROM {table} A 
                                LEFT JOIN TBL_CATEGORY B ON A.CAT_CODE = B.GROUP_CODE 
                                LEFT JOIN TBL_SUB_CATEGORY C ON A.SUB_CAT_CODE = C.GROUP_CODE 
                                LEFT JOIN TBL_FABRIC F ON A.FABRIC = F.GROUP_CODE 
                                LEFT JOIN TBL_SEASON S ON A.SEASON = S.GROUP_CODE 
                                LEFT JOIN TBL_GRADE G ON A.GRADE = G.GROUP_CODE 
                                LEFT JOIN TBL_STYLE ST ON A.STYLE = ST.GROUP_CODE 
                                LEFT JOIN TBL_ITEMSGROUP IG ON A.GROUP_CODE = IG.GROUP_CODE
                                LEFT JOIN TBL_UNIT U ON A.IUNIT_CODE = U.GROUP_CODE
                                LEFT JOIN TBL_UNIT PU ON A.PUNIT_CODE = PU.GROUP_CODE
                                WHERE A.DLT = 'T' ORDER BY A.ITEM_CODE DESC";

                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = reader["ITEM_CODE"],
                                ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                                ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                                ITEM_SHORT_NAME = Convert.ToString(reader["ITEM_SHORT_NAME"]),
                                GROUP_NAME = Convert.ToString(reader["GROUP_NAME"]),
                                UNIT = Convert.ToString(reader["UNIT"]),
                                PUNIT = Convert.ToString(reader["PUNIT"]),
                                HS_CODE = Convert.ToString(reader["HS_CODE"]),
                                PACK = Convert.ToString(reader["PACK"]),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                SALE_RATE = Convert.ToString(reader["SALE_RATE"]),
                                PURCHASE_RATE = Convert.ToString(reader["PURCHASE_RATE"]),
                                SALESTAX = Convert.ToString(reader["SALESTAX"]),
                                TAX_TYPE = Convert.ToString(reader["ITAX_STATUS"]),
                                WEIGHT = Convert.ToString(reader["WEIGHT"]),
                                ITEM_MAX = Convert.ToString(reader["ITEM_MAX"]),
                                ITEM_MINI = Convert.ToString(reader["ITEM_MINI"]),
                                IPIC = Convert.ToString(reader["IPIC"]),
                                ITEM_TYPE = Convert.ToString(reader["ITEM_TYPE"]),
                                ADD_DATE = Convert.ToString(reader["ADD_DATE"]),
                                ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = Convert.ToString(reader["EDIT_DATE"]),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                GRADE = Convert.ToString(reader["GRADE"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                CAT_CODE = Convert.ToString(reader["CAT_CODE"]),
                                SUB_CAT_CODE = Convert.ToString(reader["SUB_CAT_CODE"]),
                                FABRIC = Convert.ToString(reader["FABRIC"]),
                                SEASON = Convert.ToString(reader["SEASON"]),
                                BRAND = Convert.ToString(reader["BRAND"]),
                                STYLE = Convert.ToString(reader["STYLE"])
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

        //public MyHttpResponseMessage QuickSearch(Common common, int skip, int take, string filter = null, string group = null)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            List<object> jsonDataResult = new List<object>();
        //            int totalCount = 0;

        //            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
        //            {
        //                string filterCondition = string.Empty;
        //                if (!string.IsNullOrEmpty(filter))
        //                {
        //                    JsonNode jsonNode = JsonNode.Parse(filter);
        //                    JsonArray jsonArray = jsonNode.AsArray();
        //                    if (jsonArray.Count >= 15)
        //                    {
        //                        jsonArray.RemoveAt(1);
        //                        jsonArray.RemoveAt(0);
        //                    }
        //                    filterCondition = _commonRepository.BuildFilterCondition(jsonArray);
        //                    filterCondition = filterCondition
        //                        .Replace("suB_CAT_CODE", "C.GROUP_NAME")
        //                        .Replace("caT_CODE", "B.GROUP_NAME")
        //                        .Replace("iteM_TYPE", "A.ITEM_TYPE")
        //                        .Replace("astatus", "A.ASTATUS")
        //                        .Replace("A.ITEM_TYPE LIKE '%Active%'", "A.ITEM_TYPE LIKE '%A%'")
        //                        .Replace("A.ITEM_TYPE LIKE '%Non Active%'", "A.ITEM_TYPE LIKE '%N%'")
        //                        .Replace("A.ITEM_TYPE LIKE '%Packing Material%'", "A.ITEM_TYPE LIKE '%P%'")
        //                        .Replace("A.ITEM_TYPE LIKE '%Finish Goods%'", "A.ITEM_TYPE LIKE '%F%'")
        //                        .Replace("A.ASTATUS LIKE '%Active%'", "A.ASTATUS LIKE '%Y%'")
        //                        .Replace("A.ASTATUS LIKE '%In-Active%'", "A.ASTATUS LIKE '%N%'");
        //                }

        //                string Qurey = "";

        //                if (string.IsNullOrEmpty(group))
        //                {
        //                    Qurey = "SELECT COUNT(*) OVER() AS COUNT, A.ITEM_CODE,A.ITEM_ID,A.ITEM_NAME,A.ITEM_SHORT_NAME,A.REMARKS,A.GROUP_CODE,A.IUNIT_CODE,A.PACK,A.PUNIT_CODE,A.SALE_RATE," +
        //                               "A.PURCHASE_RATE,A.SALESTAX,A.ITAX_STATUS,A.ITEM_MAX,A.ITEM_MINI,A.IPIC,A.ADD_DATE,A.ADD_COMPUTER_NAME,A.ADD_IP_ADDRESS,A.EDIT_USER_ID,A.EDIT_DATE," +
        //                               "A.EDIT_COMPUTER_NAME,A.EDIT_IP_ADDRESS,A.ADD_POSTALCODE,A.EDIT_POSTALCODE,A.MENU_ID,A.ADD_USER_ID,A.GRADE, " +
        //                               "CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, " +
        //                               "CASE WHEN A.ITEM_TYPE = 'A' THEN 'Active' WHEN A.ITEM_TYPE = 'N' THEN 'Non Active' WHEN A.ITEM_TYPE = 'P' THEN 'Packing Material' WHEN A.ITEM_TYPE = 'F' THEN 'Finish Goods' ELSE '' END AS ITEM_TYPE, " +
        //                               "B.GROUP_NAME AS CAT_CODE, C.GROUP_NAME AS SUB_CAT_CODE  " +
        //                               "FROM " + table + " A " +
        //                               "LEFT JOIN TBL_CATEGORY B ON A.CAT_CODE = B.GROUP_CODE " +
        //                               "LEFT JOIN TBL_SUB_CATEGORY C ON A.SUB_CAT_CODE = C.GROUP_CODE " +
        //                               "WHERE A.DLT = 'T' " + filterCondition + " ORDER BY A.ITEM_CODE DESC OFFSET " + skip + " ROWS FETCH NEXT " + take + " ROWS ONLY";
        //                }
        //                else
        //                {
        //                    Qurey = "SELECT COUNT(*) OVER() AS COUNT, A.ITEM_CODE,A.ITEM_ID,A.ITEM_NAME,A.ITEM_SHORT_NAME,A.REMARKS,A.GROUP_CODE,A.IUNIT_CODE,A.PACK,A.PUNIT_CODE,A.SALE_RATE," +
        //                               "A.PURCHASE_RATE,A.SALESTAX,A.ITAX_STATUS,A.ITEM_MAX,A.ITEM_MINI,A.IPIC,A.ADD_DATE,A.ADD_COMPUTER_NAME,A.ADD_IP_ADDRESS,A.EDIT_USER_ID,A.EDIT_DATE," +
        //                               "A.EDIT_COMPUTER_NAME,A.EDIT_IP_ADDRESS,A.ADD_POSTALCODE,A.EDIT_POSTALCODE,A.MENU_ID,A.ADD_USER_ID,A.GRADE, " +
        //                               "CASE WHEN A.ASTATUS = 'Y' THEN 'Active' ELSE 'In-Active' END AS ASTATUS, " +
        //                               "CASE WHEN A.ITEM_TYPE = 'A' THEN 'Active' WHEN A.ITEM_TYPE = 'N' THEN 'Non Active' WHEN A.ITEM_TYPE = 'P' THEN 'Packing Material' WHEN A.ITEM_TYPE = 'F' THEN 'Finish Goods' ELSE '' END AS ITEM_TYPE, " +
        //                               "B.GROUP_NAME AS CAT_CODE, C.GROUP_NAME AS SUB_CAT_CODE  " +
        //                               "FROM " + table + " A " +
        //                               "LEFT JOIN TBL_CATEGORY B ON A.CAT_CODE = B.GROUP_CODE " +
        //                               "LEFT JOIN TBL_SUB_CATEGORY C ON A.SUB_CAT_CODE = C.GROUP_CODE " +
        //                               "WHERE A.DLT = 'T' " + filterCondition + " ORDER BY A.ITEM_CODE DESC";
        //                }

        //                SqlCommand countCommand = new SqlCommand(Qurey, connection);
        //                connection.Open();
        //                SqlDataReader readerCommand = countCommand.ExecuteReader();
        //                if (readerCommand.Read())
        //                {
        //                    totalCount = readerCommand.GetInt32(readerCommand.GetOrdinal("COUNT"));
        //                }
        //                readerCommand.Close();
        //                connection.Close();

        //                SqlCommand command = new SqlCommand(Qurey, connection);
        //                connection.Open();
        //                SqlDataReader reader = command.ExecuteReader();
        //                while (reader.Read())
        //                {
        //                    var row = new
        //                    {
        //                        ID = reader["ITEM_CODE"],
        //                        ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
        //                        ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
        //                        ITEM_SHORT_NAME = Convert.ToString(reader["ITEM_SHORT_NAME"]),
        //                        REMARKS = Convert.ToString(reader["REMARKS"]),
        //                        GROUP_CODE = Convert.ToString(reader["GROUP_CODE"]),
        //                        IUNIT_CODE = Convert.ToString(reader["IUNIT_CODE"]),
        //                        PACK = Convert.ToString(reader["PACK"]),
        //                        PUNIT_CODE = Convert.ToString(reader["PUNIT_CODE"]),
        //                        SALE_RATE = Convert.ToString(reader["SALE_RATE"]),
        //                        PURCHASE_RATE = Convert.ToString(reader["PURCHASE_RATE"]),
        //                        SALESTAX = Convert.ToString(reader["SALESTAX"]),
        //                        ITAX_STATUS = Convert.ToString(reader["ITAX_STATUS"]),
        //                        ITEM_MAX = Convert.ToString(reader["ITEM_MAX"]),
        //                        ITEM_MINI = Convert.ToString(reader["ITEM_MINI"]),
        //                        IPIC = Convert.ToString(reader["IPIC"]),
        //                        ADD_DATE = Convert.ToString(reader["ADD_DATE"]),
        //                        ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
        //                        ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
        //                        EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
        //                        EDIT_DATE = Convert.ToString(reader["EDIT_DATE"]),
        //                        EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
        //                        EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
        //                        ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
        //                        EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
        //                        ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
        //                        GRADE = Convert.ToString(reader["GRADE"]),
        //                        ASTATUS = Convert.ToString(reader["ASTATUS"]),
        //                        ITEM_TYPE = Convert.ToString(reader["ITEM_TYPE"]),
        //                        CAT_CODE = Convert.ToString(reader["CAT_CODE"]),
        //                        SUB_CAT_CODE = Convert.ToString(reader["SUB_CAT_CODE"])
        //                    };
        //                    jsonDataResult.Add(row);
        //                }

        //                reader.Close();
        //            }

        //            if (!string.IsNullOrEmpty(group))
        //            {

        //                JsonNode groupNode = JsonNode.Parse(group);
        //                JsonArray groupArray = groupNode.AsArray();
        //                var groupSelectors = groupArray.Select(g => g["selector"].ToString());
        //                string groupByClause = string.Join("", groupSelectors.Select(selector => selector));

        //                var groupedData = new List<object>();
        //                var grouped = jsonDataResult.GroupBy(d => _commonRepository.GetPropertyValue(d, groupByClause.ToUpper())).Select(g => new
        //                {
        //                    key = g.Key,
        //                    items = g.ToList()
        //                });
        //                groupedData.AddRange(grouped);
        //                response.data = new
        //                {
        //                    data = groupedData,
        //                    totalCount = totalCount,
        //                };
        //            }
        //            else
        //            {
        //                response.data = new
        //                {
        //                    data = jsonDataResult,
        //                    totalCount = totalCount
        //                };
        //            }
        //            response.msg = "";
        //            response.msgType = 1;
        //        }
        //        else
        //        {
        //            response.data = "";
        //            response.msg = "Something went wrong! please try again later.";
        //            response.msgType = 2;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        response.msg = _catchMessage;
        //        response.msgType = 2;
        //    }
        //    return response;
        //}

        public MyHttpResponseMessage GetItemMasterByCode(int code, Common common)
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
                        //string query = "SELECT ITEM_CODE,ITEM_ID,ITEM_NAME,ITEM_SHORT_NAME,REMARKS,GROUP_CODE,IUNIT_CODE,PACK,PUNIT_CODE,SALE_RATE," +
                        //               "PURCHASE_RATE,SALESTAX,ITAX_STATUS,ITEM_MAX,ITEM_MINI,IPIC,GRADE,ASTATUS,ITEM_TYPE, CAT_CODE, SUB_CAT_CODE " +
                        //               "FROM " + table + " WHERE DLT = 'T' AND ITEM_CODE = '" + code + "'";

                        //string query = $@"SELECT IT.ITEM_CODE,IT.ITEM_ID,IT.ITEM_NAME,IT.ITEM_SHORT_NAME,IT.REMARKS,IT.GROUP_CODE,IT.IUNIT_CODE,IT.PACK,IT.PUNIT_CODE,IT.SALE_RATE,
                        //                IT.PURCHASE_RATE,IT.SALESTAX,IT.ITAX_STATUS,
                        //                IT.ITEM_MAX,IT.ITEM_MINI,IT.IPIC,IT.GRADE,IT.ASTATUS,IT.ITEM_TYPE, IT.CAT_CODE, IT.SUB_CAT_CODE ,
                        //                AI.CODE AS ATT_CODE, AI.CAT_CODE AS ATT_CAT_CODE, AI.SCAT_CODE , AI.FAB_CODE , AI.SEASON_CODE , AI.BRAND_CODE , AI.STY_CODE 
                        //                FROM {table} IT
                        //                LEFT OUTER JOIN TBL_ITEM_ATT AI ON AI.ITEM_CODE = IT.ITEM_CODE AND AI.DLT = 'T'
                        //                WHERE IT.DLT = 'T' AND IT.ITEM_CODE = '{code}'";

                        string query = $@"SELECT IT.WEIGHT,IT.BARCODE_TYPE,IT.BARCODE,IT.COLOR,IT.SIZE, IT.HS_CODE, IT.ITEM_CODE,IT.ITEM_ID,IT.ITEM_NAME,IT.ITEM_SHORT_NAME,IT.REMARKS,IT.GROUP_CODE,IT.IUNIT_CODE,IT.PACK,IT.PUNIT_CODE,IT.SALE_RATE,
                                        IT.PURCHASE_RATE,IT.SALESTAX,IT.ITAX_STATUS,
                                        IT.ITEM_MAX,IT.ITEM_MINI,IT.IPIC,IT.GRADE,IT.ASTATUS,IT.ITEM_TYPE,
										CAT_CODE, SUB_CAT_CODE, GRADE, FABRIC, SEASON, STYLE
                                        FROM {table} IT
                                        WHERE IT.DLT = 'T' AND IT.ITEM_CODE = '{code}'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new ItemMaster
                            {
                                WEIGHT = reader["WEIGHT"] == DBNull.Value ? 0 : Convert.ToDouble(reader["WEIGHT"]),
                                HS_CODE = Convert.ToString(reader["HS_CODE"]),
                                ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                                ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                                ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                                ITEM_SHORT_NAME = Convert.ToString(reader["ITEM_SHORT_NAME"]),
                                BARCODE = Convert.ToString(reader["BARCODE"] ?? ""),
                                REMARKS = Convert.ToString(reader["REMARKS"]),
                                GROUP_CODE = reader["GROUP_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GROUP_CODE"]),
                                BITYPE = reader["BARCODE_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BARCODE_TYPE"]),
                                COLOR = reader["COLOR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COLOR"]),
                                SIZE = reader["SIZE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE"]),
                                IUNIT_CODE = reader["IUNIT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["IUNIT_CODE"]),
                                PACK = Convert.ToString(reader["PACK"]),
                                PUNIT_CODE = reader["PUNIT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["PUNIT_CODE"]),
                                SALE_RATE = reader["SALE_RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["SALE_RATE"]),
                                PURCHASE_RATE = reader["PURCHASE_RATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["PURCHASE_RATE"]),
                                SALESTAX = reader["SALESTAX"] == DBNull.Value ? 0 : Convert.ToDouble(reader["SALESTAX"]),
                                ITAX_STATUS = reader["ITAX_STATUS"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITAX_STATUS"]),
                                ITEM_MAX = reader["ITEM_MAX"] == DBNull.Value ? 0 : Convert.ToDouble(reader["ITEM_MAX"]),
                                ITEM_MIN = reader["ITEM_MINI"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_MINI"]),
                                IPIC = Convert.ToString(reader["IPIC"]),
                                GRADE = reader["GRADE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GRADE"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                ITEM_TYPE = Convert.ToString(reader["ITEM_TYPE"]),
                                CAT_CODE = Convert.ToInt32(reader["CAT_CODE"]),
                                SUB_CAT_CODE = reader["SUB_CAT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SUB_CAT_CODE"]),
                            };

                            var itemAttributeResult = new ItemAttribute
                            {
                                //CODE = reader["ATT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ATT_CODE"]),
                                CAT_CODE = reader["CAT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CAT_CODE"]),
                                SUB_CAT_CODE = reader["SUB_CAT_CODE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SUB_CAT_CODE"]),
                                FABRIC = reader["FABRIC"] == DBNull.Value ? 0 : Convert.ToInt32(reader["FABRIC"]),
                                SEASON = reader["SEASON"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SEASON"]),
                                BRAND = reader["GRADE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["GRADE"]),
                                STYLE = reader["STYLE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["STYLE"]),
                            };
                            response.msgType = 1;
                            response.data = jsonDataResult;
                            response.data2 = itemAttributeResult;
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

        public MyHttpResponseMessage GetBarcodeInfoByCode(int code, Common common)
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
                        string Qurey = "SELECT A.CODE,A.BARCODE,A.BARCODE_TYPE,B.GROUP_NAME AS COLOR,C.GROUP_NAME AS SIZE,A.PRATE," +
                                       "A.SRATE,A.WSALE,A.RRATE,A.DRATE,A.ITEM_CODE,A.ADD_DATE,A.ADD_COMPUTER_NAME,A.ADD_IP_ADDRESS,A.EDIT_USER_ID," +
                                       "A.EDIT_DATE,A.EDIT_COMPUTER_NAME,A.EDIT_IP_ADDRESS,A.MENU_ID,A.ADD_POSTALCODE," +
                                       "A.EDIT_POSTALCODE,A.ASTATUS,A.ADD_USER_ID,BL.GROUP_NAME AS BLABEL " +
                                       "FROM " + table + " A " +
                                       "LEFT JOIN TBL_COLOR B ON A.COLOR = B.GROUP_CODE " +
                                       "LEFT JOIN TBL_SIZE C ON A.SIZE = C.GROUP_CODE " +
                                       "LEFT JOIN TBL_BLABEL BL ON A.BLABEL = BL.GROUP_CODE " +
                                       "WHERE A.DLT = 'T' AND A.ITEM_CODE = '" + code + "' ORDER BY A.CODE DESC";
                        SqlCommand command = new SqlCommand(Qurey, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var row = new
                            {
                                ID = Convert.ToInt32(reader["CODE"]),
                                BARCODE = Convert.ToString(reader["BARCODE"]),
                                ITEM_CODE = Convert.ToString(reader["ITEM_CODE"]),
                                BARCODE_TYPE = Convert.ToString(reader["BARCODE_TYPE"]),
                                COLOR = Convert.ToString(reader["COLOR"]),
                                SIZE = Convert.ToString(reader["SIZE"]),
                                PRATE = Convert.ToString(reader["PRATE"]),
                                SRATE = Convert.ToString(reader["SRATE"]),
                                WSALE = Convert.ToString(reader["WSALE"]),
                                RRATE = Convert.ToString(reader["RRATE"]),
                                DRATE = Convert.ToString(reader["DRATE"]),
                                ASTATUS = Convert.ToString(reader["ASTATUS"]),
                                ADD_DATE = Convert.ToString(reader["ADD_DATE"]),
                                ADD_COMPUTER_NAME = Convert.ToString(reader["ADD_COMPUTER_NAME"]),
                                ADD_IP_ADDRESS = Convert.ToString(reader["ADD_IP_ADDRESS"]),
                                EDIT_USER_ID = Convert.ToString(reader["EDIT_USER_ID"]),
                                EDIT_DATE = Convert.ToString(reader["EDIT_DATE"]),
                                EDIT_COMPUTER_NAME = Convert.ToString(reader["EDIT_COMPUTER_NAME"]),
                                EDIT_IP_ADDRESS = Convert.ToString(reader["EDIT_IP_ADDRESS"]),
                                ADD_POSTALCODE = Convert.ToString(reader["ADD_POSTALCODE"]),
                                EDIT_POSTALCODE = Convert.ToString(reader["EDIT_POSTALCODE"]),
                                ADD_USER_ID = Convert.ToString(reader["ADD_USER_ID"]),
                                BLABEL = Convert.ToString(reader["BLABEL"]),
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

        public MyHttpResponseMessage SaveBarcodeInfo(Barcode modelRecord, Common common)
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
                            if (modelRecord.CODE == null || modelRecord.CODE == 0)
                            {
                                if (modelRecord.BARCODE_TYPE == 2)
                                {
                                    char[] separators = { ',' };
                                    var colors = modelRecord.SELECTEDCOLORS.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                    var sizes = modelRecord.SELECTEDSIZES.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                                    var isSuccess = false;
                                    string color = string.Empty, size = string.Empty;
                                    foreach (var ColorRecord in colors)
                                    {
                                        foreach (var SizeRecord in sizes)
                                        {
                                            var code = GenerateNextDetailId(modelRecord.ITEM_CODE, common, command);
                                            var Barcode = GenerateAutoBarCode(common, command, modelRecord.BLABEL, code);
                                            query = "INSERT INTO  " + table + " " +
                                                    "(CODE,BARCODE,BARCODE_TYPE,COLOR," +
                                                    "SIZE,PRATE,SRATE,WSALE,RRATE,DRATE,ITEM_CODE," +
                                                    "ADD_DATE,ADD_COMPUTER_NAME," +
                                                    "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                                    "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                                    "MENU_ID,ADD_POSTALCODE,EDIT_POSTALCODE," +
                                                    "ASTATUS,ADD_USER_ID,DLT,BLABEL)" +
                                                    "VALUES" +
                                                    "('" + code + "','" + Barcode + "','" + modelRecord.BARCODE_TYPE + "','" + ColorRecord + "'," +
                                                    "'" + SizeRecord + "','" + modelRecord.PRATE + "','" + modelRecord.SRATE + "','" + modelRecord.WSALE + "'," +
                                                    "'" + modelRecord.RRATE + "','" + modelRecord.DRATE + "','" + modelRecord.ITEM_CODE + "'," +
                                                    "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                                    "'" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                                    "'" + Computer + "','" + Ip + "'," +
                                                    "'" + common.MenuID + "','" + Postal + "','" + Postal + "'," +
                                                    "'Y','" + username + "','T','" + modelRecord.BLABEL + "')";
                                            command.CommandText = query;
                                            command.ExecuteNonQuery();

                                            Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE COLOR = '" + ColorRecord + "' AND SIZE = '" + SizeRecord + "' AND ITEM_CODE = '" + modelRecord.ITEM_CODE + "' AND DLT = 'T'";
                                            command.CommandText = Duplicationquery;
                                            int count = (int)command.ExecuteScalar();
                                            if (count == 1)
                                            {
                                                isSuccess = true;
                                                response.data = modelRecord.ITEM_CODE;
                                                response.msgType = 1;
                                                response.msg = "Record Added Successfully";
                                            }
                                            else
                                            {
                                                isSuccess = false;
                                                color = ColorRecord;
                                                size = SizeRecord;
                                                break;
                                                response.msg = $"Color {ColorRecord} and Size {SizeRecord} Already Exist !....";
                                                response.msgType = 2;
                                            }
                                        }
                                        if (!isSuccess)
                                        {
                                            break;
                                        }
                                    }
                                    if (isSuccess)
                                    {
                                        transaction.Commit();
                                        response.data = modelRecord.ITEM_CODE;
                                        response.msgType = 1;
                                        response.msg = "Record Added Successfully";
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        //response.msg = $"Color {color} and Size {size} Already Exist !....";
                                        response.msg = $"Color and Size Already Exist !....";
                                        response.msgType = 2;
                                    }
                                }
                                else
                                {
                                    //var Barcode = GenerateAutoBarCode(common, command);
                                    query = "INSERT INTO  " + table + " " +
                                            "(CODE,BARCODE,BARCODE_TYPE,COLOR,SIZE," +
                                            "PRATE,SRATE,WSALE,RRATE,DRATE,ITEM_CODE," +
                                            "ADD_DATE,ADD_COMPUTER_NAME," +
                                            "ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE," +
                                            "EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                            "MENU_ID,ADD_POSTALCODE,EDIT_POSTALCODE," +
                                            "ASTATUS,ADD_USER_ID,DLT,BLABEL)" +
                                            "VALUES" +
                                            "('" + GenerateNextDetailId(modelRecord.ITEM_CODE, common, command) + "','" + modelRecord.BARCODE + "','" + modelRecord.BARCODE_TYPE + "','" + modelRecord.COLOR + "','" + modelRecord.SIZE + "'," +
                                            "'" + modelRecord.PRATE + "','" + modelRecord.SRATE + "','" + modelRecord.WSALE + "'," +
                                            "'" + modelRecord.RRATE + "','" + modelRecord.DRATE + "','" + modelRecord.ITEM_CODE + "'," +
                                            "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "'," +
                                            "'" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Computer + "','" + Ip + "'," +
                                            "'" + common.MenuID + "','" + Postal + "','" + Postal + "'," +
                                            "'Y','" + username + "','T','" + modelRecord.BLABEL + "')";
                                    command.CommandText = query;
                                    command.ExecuteNonQuery();

                                    Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE COLOR = '" + modelRecord.COLOR + "' AND SIZE = '" + modelRecord.SIZE + "' AND ITEM_CODE = '" + modelRecord.ITEM_CODE + "' AND DLT = 'T'";
                                    command.CommandText = Duplicationquery;
                                    int count = (int)command.ExecuteScalar();
                                    if (count == 1)
                                    {
                                        transaction.Commit();
                                        response.data = modelRecord.ITEM_CODE;
                                        response.msgType = 1;
                                        response.msg = "Record Added Successfully";
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        //response.msg = $"Color {modelRecord.COLOR} and Size {modelRecord.SIZE} Already Exist !....";
                                        response.msg = $"Color and Size Already Exist !....";
                                        response.msgType = 2;
                                    }
                                }
                            }
                            else
                            {
                                query = "UPDATE " + table + " SET BARCODE = '" + modelRecord.BARCODE + @"',
                                        BARCODE_TYPE = '" + modelRecord.BARCODE_TYPE + @"',
                                        PRATE = '" + modelRecord.PRATE + @"',
                                        SRATE = '" + modelRecord.SRATE + @"',
                                        WSALE = '" + modelRecord.WSALE + @"',
                                        RRATE = '" + modelRecord.RRATE + @"',
                                        DRATE = '" + modelRecord.DRATE + @"',
                                        EDIT_USER_ID = '" + username + @"',
                                        EDIT_DATE = '" + CommonService.GetDateTime("Pakistan Standard Time") + @"',
                                        EDIT_COMPUTER_NAME = '" + Computer + @"',
                                        EDIT_IP_ADDRESS = '" + Ip + @"',
                                        EDIT_POSTALCODE = '" + Postal + @"',
                                        ASTATUS = 'Y',
                                        BLABEL = '" + modelRecord.BLABEL + @"'
                                        WHERE CODE = '" + modelRecord.CODE + @"' AND ITEM_CODE = '" + modelRecord.ITEM_CODE + @"'";
                                command.CommandText = query;
                                command.ExecuteNonQuery();

                                Duplicationquery = "SELECT COUNT(*) FROM " + table + " WHERE COLOR = '" + modelRecord.COLOR + "' AND SIZE = '" + modelRecord.SIZE + "' AND ITEM_CODE = '" + modelRecord.ITEM_CODE + "' AND DLT = 'T'";
                                command.CommandText = Duplicationquery;
                                int count = (int)command.ExecuteScalar();

                                if (count == 1)
                                {
                                    transaction.Commit();
                                    response.msgType = 1;
                                    response.msg = "Record Updated Successfully";
                                }
                                else
                                {
                                    transaction.Rollback();
                                    //response.msg = $"Color {modelRecord.COLOR} and Size {modelRecord.SIZE} Already Exist !....";
                                    response.msg = $"Color and Size Already Exist !....";
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

        public MyHttpResponseMessage SaveAttributeInfo(ItemAttribute modelRecord, Common common)
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
                    //int code = 0;
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
                            if (modelRecord.CODE == 0 || modelRecord.CODE is null)
                            {

                                //code = GenerateNextAttributeId(common, command);
                                //modelRecord.CODE = code;

                                //query = "INSERT INTO TBL_ITEM_ATT " +
                                //                "(CODE,ITEM_CODE,CAT_CODE,SCAT_CODE,FAB_CODE,SEASON_CODE,BRAND_CODE,STY_CODE,ADD_USER_ID,ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS," +
                                //                "ADD_POSTALCODE,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS,EDIT_POSTALCODE,MENU_ID,DLT) " +
                                //                "VALUES " +
                                //                "('" + modelRecord.CODE + "','"
                                //                + modelRecord.ITEM_CODE + "','"
                                //                + modelRecord.CAT_CODE + "','"
                                //                + modelRecord.SUB_CAT_CODE + "','"
                                //                + modelRecord.FABRIC + "','"
                                //                + modelRecord.SEASON + "','"
                                //                + modelRecord.BRAND + "','"
                                //                + modelRecord.STYLE + "','"
                                //                + userid + "','"
                                //                + CommonService.GetDateTime("Pakistan Standard Time") + "','"
                                //                + Computer + "','"
                                //                + Ip + "','"
                                //                + Postal + "','"
                                //                + userid + "','"
                                //                + CommonService.GetDateTime("Pakistan Standard Time") + "','"
                                //                + Computer + "','"
                                //                + Ip + "','"
                                //                + Postal + "','"
                                //                + common.MenuID + "','T')";


                                //command.CommandText = query;
                                //command.ExecuteNonQuery();
                                //transaction.Commit();

                                //response.data = code;
                                response.msgType = 2;
                                response.msg = "Something went wrong! please try again later.";

                            }
                            else
                            {
                                query = "UPDATE "+ table +" SET " +

                                        "[CAT_CODE] = '" + modelRecord.CAT_CODE + "', " +
                                        "[SUB_CAT_CODE] = '" + modelRecord.SUB_CAT_CODE + "', " +
                                        "[FABRIC] = '" + modelRecord.FABRIC + "', " +
                                        "[SEASON] = '" + modelRecord.SEASON + "', " +
                                        "[GRADE] = '" + modelRecord.BRAND + "', " +
                                        "[STYLE] = '" + modelRecord.STYLE + "', " +

                                        "[EDIT_USER_ID] = '" + userid + "', " +
                                        "[EDIT_DATE] = '" + CommonService.GetDateTime("Pakistan Standard Time") + "', " +
                                        "[EDIT_COMPUTER_NAME] = '" + Computer + "', " +
                                        "[EDIT_IP_ADDRESS] = '" + Ip + "', " +
                                        "[EDIT_POSTALCODE] = '" + Postal + "', " +
                                        "[MENU_ID] = '" + common.MenuID + "'" +
                                        "WHERE [ITEM_CODE] = '" + modelRecord.CODE + "'";

                                command.CommandText = query;
                                command.ExecuteNonQuery();
                                transaction.Commit();
                                //response.data = modelRecord.CODE;
                                response.msgType = 1;
                                response.msg = "Attribute Record Added Successfully";
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

        public MyHttpResponseMessage SaveCopiedBarcodeInfo(int? newItemCode, List<Barcode> modelRecord, Common common)
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

                if (!string.IsNullOrWhiteSpace(table))
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

                        bool isSuccess = false;

                        try
                        {
                            foreach (var singleRecord in modelRecord)
                            {
                                string query = "";

                                if (singleRecord.BARCODE_TYPE == 2)
                                {
                                    var code = GenerateNextDetailId(newItemCode, common, command);
                                    var Barcode = GenerateAutoBarCode(common, command, singleRecord.BLABEL, code);

                                    query = "INSERT INTO " + table + " (CODE,BARCODE,BARCODE_TYPE,COLOR,SIZE,PRATE,SRATE,WSALE,RRATE,DRATE,ITEM_CODE," +
                                            "ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                            "MENU_ID,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS,ADD_USER_ID,DLT,BLABEL) VALUES " +
                                            "('" + code + "','" + Barcode + "','" + singleRecord.BARCODE_TYPE + "','" + singleRecord.COLOR + "','" + singleRecord.SIZE + "'," +
                                            "'" + singleRecord.PRATE + "','" + singleRecord.SRATE + "','" + singleRecord.WSALE + "','" + singleRecord.RRATE + "','" + singleRecord.DRATE + "','" + newItemCode + "'," +
                                            "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Computer + "','" + Ip + "','" + common.MenuID + "','" + Postal + "','" + Postal + "','Y','" + username + "','T','" + singleRecord.BLABEL + "')";
                                }
                                else
                                {
                                    query = "INSERT INTO " + table + " (CODE,BARCODE,BARCODE_TYPE,COLOR,SIZE,PRATE,SRATE,WSALE,RRATE,DRATE,ITEM_CODE," +
                                            "ADD_DATE,ADD_COMPUTER_NAME,ADD_IP_ADDRESS,EDIT_USER_ID,EDIT_DATE,EDIT_COMPUTER_NAME,EDIT_IP_ADDRESS," +
                                            "MENU_ID,ADD_POSTALCODE,EDIT_POSTALCODE,ASTATUS,ADD_USER_ID,DLT,BLABEL) VALUES " +
                                            "('" + GenerateNextDetailId(singleRecord.ITEM_CODE, common, command) + "','" + singleRecord.BARCODE + "','" + singleRecord.BARCODE_TYPE + "','" + singleRecord.COLOR + "','" + singleRecord.SIZE + "'," +
                                            "'" + singleRecord.PRATE + "','" + singleRecord.SRATE + "','" + singleRecord.WSALE + "','" + singleRecord.RRATE + "','" + singleRecord.DRATE + "','" + newItemCode + "'," +
                                            "'" + CommonService.GetDateTime("Pakistan Standard Time") + "','" + Computer + "','" + Ip + "','" + username + "','" + CommonService.GetDateTime("Pakistan Standard Time") + "'," +
                                            "'" + Computer + "','" + Ip + "','" + common.MenuID + "','" + Postal + "','" + Postal + "','Y','" + username + "','T','" + singleRecord.BLABEL + "')";
                                }

                                command.CommandText = query;
                                int rowsAffected = command.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    isSuccess = true;
                                }
                            }

                            if (isSuccess)
                            {
                                transaction.Commit();
                                response.msgType = 1;
                                response.msg = "Data saved successfully.";
                            }
                            else
                            {
                                transaction.Rollback();
                                response.msgType = 2;
                                response.msg = "No records were inserted.";
                            }
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            response.msgType = 2;
                            response.msg = "Error: " + ex.Message + (ex.InnerException != null ? "<br/>" + ex.InnerException.Message : "");
                        }
                    }
                }
                else
                {
                    response.msg = "Something went wrong! Please try again later.";
                    response.msgType = 2;
                }
            }
            catch (Exception ex)
            {
                response.msgType = 2;
                response.msg = "Error: " + ex.Message + (ex.InnerException != null ? "<br/>" + ex.InnerException.Message : "");
            }
            return response;
        }

        private string? GenerateAutoBarCode(Common common, SqlCommand command, int? groupcode, string? code)
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
                    //string maxIdQuery = @$"SELECT 
                    //                        CONCAT(
                    //                            (SELECT MAX(BARCODE) FROM TBL_BLABEL WHERE GROUP_CODE = {groupcode} AND DLT = 'T'), 
                    //                            RIGHT(CAST(CAST(RIGHT((SELECT TOP(1) BARCODE FROM TBL_BARCODE ORDER BY CODE DESC), 8) AS INT) + 1 AS VARCHAR), 6)
                    //                        ) AS BARCODE";
                    //string maxIdQuery = "SELECT " +
                    //                    " (SELECT MAX(BARCODE) FROM TBL_BLABEL WHERE GROUP_CODE = " + code + " AND DLT = 'T')" +
                    //                    " +RIGHT('00000000000'+CONVERT(NVARCHAR(100),ISNULL(MAX(CODE),0)+1),6) AS BARCODE FROM " + table +
                    //                    " WHERE  DLT = 'T'";// AND ITEM_CODE = '" + code+"'";

                    string maxIdQuery = @$"SELECT TM.VOUCHER_LEN FROM TBL_MENU_BUILDER TM WHERE ID = {common.MenuID}";
                    command.CommandText = maxIdQuery;
                    object result = command.ExecuteScalar();

                    int voucherlenght = result != null ? Convert.ToInt32(result) : 0;

                    string formattedVoucher = (code ?? "0").PadLeft(voucherlenght, '0');

                    string Query = @$"SELECT MAX(BARCODE) FROM TBL_BLABEL WHERE GROUP_CODE = {groupcode} AND DLT = 'T'";
                    command.CommandText = Query;
                    object result2 = command.ExecuteScalar();

                    string barcode = result2 != null ? result2.ToString() : "";

                    string finalBarcode = barcode + formattedVoucher;

                    return Convert.ToString(finalBarcode);
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        //private int GenerateNextAttributeId(Common common, SqlCommand command)
        //{
        //    try
        //    {
        //        var Menu = _menuRepository.GetMenu(common.MenuID);
        //        string? table = string.Empty;
        //        if (Menu.data != null)
        //        {
        //            var menu = (Menu)Menu.data;
        //            table = menu.TABLE1;
        //        }

        //        if (!String.IsNullOrWhiteSpace(table))
        //        {
        //            string maxIdQuery = $"SELECT ISNULL(MAX(CODE), 0) + 1 FROM TBL_ITEM_ATT ";
        //            command.CommandText = maxIdQuery;
        //            object result = command.ExecuteScalar();
        //            return Convert.ToInt32(result);
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return 0;
        //}

        private string? GenerateNextDetailId(int? code, Common common, SqlCommand command)
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
                    string maxIdQuery = "SELECT ISNULL(MAX(CODE), 0) + 1 FROM " + table;// + " WHERE ITEM_CODE = '" + code + "'";
                    command.CommandText = maxIdQuery;
                    object result = command.ExecuteScalar();
                    return Convert.ToString(result);
                }
            }
            catch (Exception ex)
            {

            }
            return string.Empty;
        }

        public MyHttpResponseMessage GetBarcodeInfoByBarcodeId(int barCodeId, int code, Common common)
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
                        string query = "SELECT CODE,BARCODE,BARCODE_TYPE,COLOR,SIZE,PRATE," +
                                       "SRATE,WSALE,RRATE,DRATE,GRADE,ITEM_CODE,ISNULL(BLABEL,0) AS BLABEL " +
                                       "FROM " + table + " " +
                                       "WHERE DLT = 'T' AND CODE = '" + barCodeId + "' AND ITEM_CODE = '" + code + "'";
                        SqlCommand command = new SqlCommand(query, connection);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            var jsonDataResult = new Barcode
                            {
                                CODE = Convert.ToInt32(reader["CODE"]),
                                BARCODE = Convert.ToString(reader["BARCODE"]),
                                ITEM_CODE = reader["COLOR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ITEM_CODE"]),
                                COLOR = reader["COLOR"] == DBNull.Value ? 0 : Convert.ToInt32(reader["COLOR"]),
                                SIZE = reader["SIZE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["SIZE"]),
                                BARCODE_TYPE = reader["BARCODE_TYPE"] == DBNull.Value ? 0 : Convert.ToInt32(reader["BARCODE_TYPE"]),
                                PRATE = reader["PRATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["PRATE"]),
                                SRATE = reader["SRATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["SRATE"]),
                                WSALE = reader["WSALE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["WSALE"]),
                                RRATE = reader["RRATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["RRATE"]),
                                DRATE = reader["DRATE"] == DBNull.Value ? 0 : Convert.ToDouble(reader["DRATE"]),
                                BLABEL = Convert.ToInt32(reader["BLABEL"]),
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

        public MyHttpResponseMessage DeleteBarcodeInfo(int barCodeId, int code, Common common)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Data not found in our records";
            try
            {
                var Menu = _menuRepository.GetMenu(common.MenuID);
                string? table = string.Empty;
                string query = string.Empty;
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

                        var periodInfo = _periodRepository.GetPeriodById(Convert.ToInt32(common.Period));
                        string StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
                        string EndDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
                        query = $@"EXEC STKPROC 26,'{StartDate}','{EndDate}',1,1,'',''";
                        bool hasStock = false;
                        using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                        {
                            SqlCommand command = new SqlCommand(query, connection);
                            connection.Open();
                            SqlDataReader reader = command.ExecuteReader();
                            if (reader.HasRows)
                            {
                                hasStock = true;
                            }
                        }

                        if (!hasStock)
                        {
                            string connectionString = new SQLService().getconnstring();
                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {
                                connection.Open();
                                query = "UPDATE " + table + " SET DLT = 'F' WHERE CODE = '" + barCodeId + "' AND ITEM_CODE = '" + code + "' AND MENU_ID = '" + common.MenuID + "'";
                                SqlCommand command = new SqlCommand(query, connection);
                                command.ExecuteNonQuery();
                                response.msgType = 1;
                                response.msg = "Record Deleted Successfully";
                            }
                        }
                        else
                        {
                            response.msgType = 2;
                            response.msg = "Barcode Already In Use!";
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
        private string? GenerateAutoBarCodeForItem(Common common, SqlCommand command)
        {
            try
            {
                string maxIdQuery = @$"SELECT TM.VOUCHER_LEN FROM TBL_MENU_BUILDER TM WHERE ID = {common.MenuID}";
                command.CommandText = maxIdQuery;
                object result = command.ExecuteScalar();
                int voucherLength = result != null ? Convert.ToInt32(result) : 0;
                string queryLastBarcode = @$"SELECT MAX(CAST(BARCODE AS BIGINT)) FROM TBL_ITEMSMASTER 
                                             WHERE  DLT = 'T'";
                //string queryLastBarcode = @$"SELECT MAX(CAST(BARCODE AS BIGINT)) FROM TBL_BLABEL 
                //                     WHERE GROUP_CODE = {groupcode} AND DLT = 'T'";
                command.CommandText = queryLastBarcode;
                object result2 = command.ExecuteScalar();

                long lastBarcode = result2 != DBNull.Value && result2 != null ? Convert.ToInt64(result2) : 0;
                long nextBarcode = lastBarcode + 1;
                string finalBarcode = nextBarcode.ToString().PadLeft(voucherLength, '0');

                return finalBarcode;
            }
            catch (Exception ex)
            {
            }

            return string.Empty;
        }
    }
}
