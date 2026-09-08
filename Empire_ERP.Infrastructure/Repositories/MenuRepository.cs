using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        public MyHttpResponseMessage GetMenu()
        {
            MyHttpResponseMessage res = new MyHttpResponseMessage();
            try
            {
                var menuData = FetchMenuData();
                List<Menu> menuList = new List<Menu>();
                if (menuData.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in menuData.Tables[0].Rows)
                    {
                        Menu menu = new Menu();
                        menu.ID = Convert.ToInt32(Row["ID"]);
                        menu.MENU_NAME = Convert.ToString(Row["MENU_NAME"]);
                        menu.MENU_GRCODE = Convert.ToString(Row["MENU_GRCODE"]);
                        menu.MENU_TYPE = Convert.ToInt32(Row["MENU_TYPE"]);
                        menu.MENU_PARENT_CODE = Convert.ToInt32(Row["MENU_PARENT_CODE"]);
                        menu.MENU_PAGE = Convert.ToString(Row["MENU_PAGE"]);
                        menu.TABLE1 = Convert.ToString(Row["TABLE1"]);
                        menu.TABLE2 = Convert.ToString(Row["TABLE2"]);
                        menu.PERFIX = Convert.ToString(Row["PERFIX"]);
                        menu.PTYPE = Row["PTYPE"] == DBNull.Value ? null : Convert.ToInt32(Row["PTYPE"]);
                        menu.VOUCHER_LEN = Convert.ToString(Row["VOUCHER_LEN"]);
                        menu.PICK_TABLE_MASTER = Convert.ToString(Row["PICK_TABLE_MASTER"]);
                        menu.PICK_TABLE_DETAIL = Convert.ToString(Row["PICK_TABLE_DETAIL"]);
                        menu.DCTYPE = Convert.ToString(Row["DCTYPE"]);
                        menu.MENU_SIG1 = Convert.ToString(Row["MENU_SIG1"]);
                        menu.MENU_SIG2 = Convert.ToString(Row["MENU_SIG2"]);
                        menu.MENU_SIG3 = Convert.ToString(Row["MENU_SIG3"]);
                        menu.MENU_SIG4 = Convert.ToString(Row["MENU_SIG4"]);
                        menu.MENU_TERMS = Convert.ToString(Row["MENU_TERMS"]);
                        menu.ADD_USER_ID = Convert.ToString(Row["ADD_USER_ID"]);
                        menu.ADD_DATE = Convert.ToDateTime(Row["ADD_DATE"]);
                        menu.ADD_COMPUTER_NAME = Convert.ToString(Row["ADD_COMPUTER_NAME"]);
                        menu.ADD_IP_ADDRESS = Convert.ToString(Row["ADD_IP_ADDRESS"]);
                        menu.EDIT_USER_ID = Convert.ToString(Row["EDIT_USER_ID"]);
                        menu.EDIT_DATE = Row["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(Row["EDIT_DATE"]);
                        menu.EDIT_COMPUTER_NAME = Convert.ToString(Row["EDIT_COMPUTER_NAME"]);
                        menu.EDIT_IP_ADDRESS = Convert.ToString(Row["EDIT_IP_ADDRESS"]);
                        menu.MENU_ID = Convert.ToInt32(Row["MENU_ID"]);
                        menu.ADD_POSTALCODE = Convert.ToString(Row["ADD_POSTALCODE"]);
                        menu.EDIT_POSTALCODE = Convert.ToString(Row["EDIT_POSTALCODE"]);
                        menu.DLT = Convert.ToString(Row["DLT"]);
                        menu.MPIC = Convert.ToString(Row["MPIC"]);
                        menu.MTYPE = Convert.ToString(Row["MTYPE"]);
                        menu.ASTATUS = Convert.ToString(Row["ASTATUS"]);
                        menu.SEARCH = Convert.ToString(Row["SEARCH"]);
                        menu.B_I = Convert.ToString(Row["B_I"]);
                        menu.STK_STATUS = Convert.ToString(Row["STK_STATUS"]);
                        menu.PICK_TYPE = Convert.ToString(Row["PICK_TYPE"]);
                        menu.PICK_DATA = Row["PICK_DATA"] == DBNull.Value ? "" : Row["PICK_DATA"].ToString();
                        menu.DATA_CLEAR = Row["DATA_CLEAR"] != DBNull.Value ? Convert.ToInt32(Row["DATA_CLEAR"]) : 0;
                        menu.LIMIT = Row["D_LIMIT"] == DBNull.Value ? "" : Row["D_LIMIT"].ToString();
                        menu.ITEM_TYPE = Row["ITEM_TYPE"] == DBNull.Value ? "" : Row["ITEM_TYPE"].ToString();
                        menuList.Add(menu);
                    }
                }
                res.data = null;
                res.Menu = menuList;
                res.msg = "";
                res.msgType = 1;
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public MyHttpResponseMessage GetMenuByRole(int? id)
        {
            MyHttpResponseMessage res = new MyHttpResponseMessage();
            try
            {
                var menuData = FetchMenuDataByRole(id);
                List<Menu> menuList = new List<Menu>();
                if (menuData.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in menuData.Tables[0].Rows)
                    {
                        Menu menu = new Menu();
                        menu.ID = Convert.ToInt32(Row["ID"]);
                        menu.MENU_NAME = Convert.ToString(Row["MENU_NAME"]);
                        menu.MENU_GRCODE = Convert.ToString(Row["MENU_GRCODE"]);
                        menu.MENU_TYPE = Convert.ToInt32(Row["MENU_TYPE"]);
                        menu.MENU_PARENT_CODE = Convert.ToInt32(Row["MENU_PARENT_CODE"]);
                        menu.MENU_PAGE = Convert.ToString(Row["MENU_PAGE"]);
                        menu.TABLE1 = Convert.ToString(Row["TABLE1"]);
                        menu.TABLE2 = Convert.ToString(Row["TABLE2"]);
                        menu.PERFIX = Convert.ToString(Row["PERFIX"]);
                        menu.PTYPE = Row["PTYPE"] == DBNull.Value ? null : Convert.ToInt32(Row["PTYPE"]);
                        menu.VOUCHER_LEN = Convert.ToString(Row["VOUCHER_LEN"]);
                        menu.PICK_TABLE_MASTER = Convert.ToString(Row["PICK_TABLE_MASTER"]);
                        menu.PICK_TABLE_DETAIL = Convert.ToString(Row["PICK_TABLE_DETAIL"]);
                        menu.DCTYPE = Convert.ToString(Row["DCTYPE"]);
                        menu.MENU_SIG1 = Convert.ToString(Row["MENU_SIG1"]);
                        menu.MENU_SIG2 = Convert.ToString(Row["MENU_SIG2"]);
                        menu.MENU_SIG3 = Convert.ToString(Row["MENU_SIG3"]);
                        menu.MENU_SIG4 = Convert.ToString(Row["MENU_SIG4"]);
                        menu.MENU_TERMS = Convert.ToString(Row["MENU_TERMS"]);
                        menu.ADD_USER_ID = Convert.ToString(Row["ADD_USER_ID"]);
                        menu.ADD_DATE = Convert.ToDateTime(Row["ADD_DATE"]);
                        menu.ADD_COMPUTER_NAME = Convert.ToString(Row["ADD_COMPUTER_NAME"]);
                        menu.ADD_IP_ADDRESS = Convert.ToString(Row["ADD_IP_ADDRESS"]);
                        menu.EDIT_USER_ID = Convert.ToString(Row["EDIT_USER_ID"]);
                        menu.EDIT_DATE = Row["EDIT_DATE"] == DBNull.Value ? null : Convert.ToDateTime(Row["EDIT_DATE"]);
                        menu.EDIT_COMPUTER_NAME = Convert.ToString(Row["EDIT_COMPUTER_NAME"]);
                        menu.EDIT_IP_ADDRESS = Convert.ToString(Row["EDIT_IP_ADDRESS"]);
                        menu.MENU_ID = Convert.ToInt32(Row["MENU_ID"]);
                        menu.ADD_POSTALCODE = Convert.ToString(Row["ADD_POSTALCODE"]);
                        menu.EDIT_POSTALCODE = Convert.ToString(Row["EDIT_POSTALCODE"]);
                        menu.DLT = Convert.ToString(Row["DLT"]);
                        menu.MPIC = Convert.ToString(Row["MPIC"]);
                        menu.MTYPE = Convert.ToString(Row["MTYPE"]);
                        menu.ASTATUS = Convert.ToString(Row["ASTATUS"]);
                        menu.B_I = Convert.ToString(Row["B_I"]);
                        menu.STK_STATUS = Convert.ToString(Row["STK_STATUS"]);
                        menu.ITEM_TYPE = Row["ITEM_TYPE"] == DBNull.Value ? "" : Convert.ToString(Row["ITEM_TYPE"]);
                        menuList.Add(menu);
                    }
                }
                res.data = null;
                res.Menu = menuList;
                res.msg = "";
                res.msgType = 1;
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public DataSet FetchMenuData()
        {
            string query = "SELECT * from TBL_MENU_BUILDER";
            DataSet dts = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
            return dts;
        }

        public DataSet FetchMenuDataByRole(int? id)
        {
            string query = "";
            if (id is not null)
            {
                query = "SELECT * from TBL_MENU_BUILDER WHERE ID IN (SELECT RMENU_ID from TBL_ROLE WHERE ROLE_ID = " + id + " AND MODULE_ID = 1)";
            }
            else
            {
                query = "SELECT * from TBL_MENU_BUILDER";
            }
            DataSet dts = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
            return dts;
        }

        public MyHttpResponseMessage GetMenu(int id)
        {
            MyHttpResponseMessage res = new MyHttpResponseMessage();
            try
            {

                var menu = GetMenu().Menu.Where(x => x.ID == id).FirstOrDefault();
                res.data = menu;
                res.msg = "";
                res.msgType = 1;
                return res;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public MyHttpResponseMessage GetMenuCustomDetails(int menuID)
        {
            MyHttpResponseMessage res = new MyHttpResponseMessage();
            CustomMenuDetail menu = new CustomMenuDetail();
            try
            {
                string query = $"SELECT MB.MENU_SIG1,MB.MENU_SIG2,MB.MENU_SIG3,MB.MENU_SIG4,MBD.MD_NAME, MBD.REPORT_NAME " +
                               $"FROM TBL_MENU_BUILDER MB WITH (NOLOCK) " +
                               $"INNER JOIN TBL_MENU_BUILDER_DETAIL MBD WITH (NOLOCK) ON MB.ID = MBD.MMENU_ID " +
                               $"WHERE MB.ID = {menuID} AND MB.DLT = 'T' AND MB.ASTATUS = 'Y' AND MBD.DLT = 'T' AND MBD.ASTATUS = 'Y'";
                DataSet menuDetailData = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
                if (menuDetailData.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in menuDetailData.Tables[0].Rows)
                    {
                        menu.MD_NAME = Convert.ToString(Row["MD_NAME"]);
                        menu.REPORT_NAME = Convert.ToString(Row["REPORT_NAME"]);
                        menu.MENU_SIG1 = Convert.ToString(Row["MENU_SIG1"]);
                        menu.MENU_SIG2 = Convert.ToString(Row["MENU_SIG2"]);
                        menu.MENU_SIG3 = Convert.ToString(Row["MENU_SIG3"]);
                        menu.MENU_SIG4 = Convert.ToString(Row["MENU_SIG4"]);
                    }
                }
                res.data = menu;
                res.msg = "";
                res.msgType = 1;
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                res.msg = _catchMessage;
                res.msgType = 2;
            }
            return res;
        }

        public MyHttpResponseMessage GetMenuDetails(int menuID)
        {
            MyHttpResponseMessage res = new MyHttpResponseMessage();
            List<CustomMenuDetail> menuDetailsList = new List<CustomMenuDetail>();
            try
            {
                string query = $"SELECT D.MD_ID,D.MD_NAME,D.REPORT_NAME, M.MENU_SIG1, M.MENU_SIG2, M.MENU_SIG3, M.MENU_SIG4 , M.MENU_TERMS FROM TBL_MENU_BUILDER_DETAIL D  " +
                               $"LEFT OUTER JOIN TBL_MENU_BUILDER M ON M.ID = D.MMENU_ID " +
                               $"WHERE D.MMENU_ID = {menuID} AND D.ASTATUS = 'Y' AND D.DLT = 'T' ORDER BY D.SNO";
                //string query = $"SELECT MD_ID,MD_NAME,REPORT_NAME FROM TBL_MENU_BUILDER_DETAIL WITH (NOLOCK) " +
                //               $"WHERE MMENU_ID = {menuID} AND ASTATUS = 'Y' AND DLT = 'T'";
                DataSet menuDetailData = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
                if (menuDetailData.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow Row in menuDetailData.Tables[0].Rows)
                    {
                        CustomMenuDetail menu = new CustomMenuDetail();
                        menu.MD_NAME = Convert.ToString(Row["MD_NAME"]);
                        menu.MD_ID = Convert.ToInt32(Row["MD_ID"]);
                        menu.REPORT_NAME = Convert.ToString(Row["REPORT_NAME"]);
                        menu.MENU_SIG1 = Convert.ToString(Row["MENU_SIG1"]);
                        menu.MENU_SIG2 = Convert.ToString(Row["MENU_SIG2"]);
                        menu.MENU_SIG3 = Convert.ToString(Row["MENU_SIG3"]);
                        menu.MENU_SIG4 = Convert.ToString(Row["MENU_SIG4"]);
                        menu.MENU_TERMS = Convert.ToString(Row["MENU_TERMS"]);
                        menuDetailsList.Add(menu);
                    }
                }
                res.data = menuDetailsList;
                res.msg = "";
                res.msgType = 1;
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                res.msg = _catchMessage;
                res.msgType = 2;
            }
            return res;
        }
    }
}