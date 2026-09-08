using Empire_ERP.Core.Entities;
using System.Data;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Empire_ERP.Controllers
{
    //[CheckSession]
    //[ExtractMenuCode]
    public class FavoriteMenuController : BaseController
    {
        private readonly IMenuService _menuService;

        public FavoriteMenuController(IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {
            _menuService = menuService;
        }

        [HttpPost]
        public IActionResult AddFavorite(string menuCode, string customName, string shortcutKey, string menuUrl)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Failed to add favorite.";

            try
            {
                var user = CommonHelper.GetValues(HttpContext);
                string connectionString = new SQLService().getconnstring();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string checkMenuQuery = $@"SELECT COUNT(*) FROM TBL_FAVORITE_MENUS WHERE USER_ID = '{user.UserID}' AND MENU_CODE = '{menuCode}'";

                    SqlCommand checkMenuCmd = new SqlCommand(checkMenuQuery, con);
                    int menuExists = Convert.ToInt32(checkMenuCmd.ExecuteScalar());

                    if (menuExists > 0)
                    {
                        response.msgType = 2;
                        response.msg = "This menu is already added to your favorites!";
                        return Json(response);
                    }

                    if (!string.IsNullOrEmpty(shortcutKey))
                    {
                        string checkShortcutQuery = $@"SELECT COUNT(*) FROM TBL_FAVORITE_MENUS WHERE USER_ID = '{user.UserID}' AND SHORTCUT_KEY = '{shortcutKey}'";

                        SqlCommand checkShortcutCmd = new SqlCommand(checkShortcutQuery, con);
                        int shortcutExists = Convert.ToInt32(checkShortcutCmd.ExecuteScalar());

                        if (shortcutExists > 0)
                        {
                            response.msgType = 2;
                            response.msg = $"Shortcut '{shortcutKey}' is already assigned to another favorite!";
                            return Json(response);
                        }
                    }

                    if (!string.IsNullOrEmpty(customName))
                    {
                        string checkcustomNameQuery = $@"SELECT COUNT(*) FROM TBL_FAVORITE_MENUS WHERE USER_ID = '{user.UserID}' AND MENU_NAME = '{customName}'";

                        SqlCommand checkcustomNameCmd = new SqlCommand(checkcustomNameQuery, con);
                        int customNameExists = Convert.ToInt32(checkcustomNameCmd.ExecuteScalar());

                        if (customNameExists > 0)
                        {
                            response.msgType = 2;
                            response.msg = $"A favorite with the name '{customName}' already exists!";
                            return Json(response);
                        }
                    }

                    string insertQuery = $@"INSERT INTO TBL_FAVORITE_MENUS (USER_ID, MENU_CODE, MENU_NAME, SHORTCUT_KEY, MENU_URL, ADD_DATE) VALUES ('{user.UserID}', '{menuCode}', '{customName}', '{(string.IsNullOrEmpty(shortcutKey) ? "" : shortcutKey)}', '{menuUrl}', '{CommonService.GetDateTime("Pakistan Standard Time")}')";

                    SqlCommand cmd = new SqlCommand(insertQuery, con);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        response.msgType = 1;
                        response.msg = "Favorite added successfully!";
                    }
                }
            }
            catch (Exception ex)
            {
                response.msgType = 3;
                response.msg = "Error: " + ex.Message;
            }

            return Json(response);
        }

        [HttpGet]
        public IActionResult GetFavorites()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "No favorites found.";

            try
            {
                var user = CommonHelper.GetValues(HttpContext);
                string connectionString = new SQLService().getconnstring();
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    string query = $@"SELECT * FROM TBL_FAVORITE_MENUS WHERE USER_ID = '{user.UserID}' ORDER BY ADD_DATE DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.Fill(dt);
                    Console.WriteLine("Rows count: " + dt.Rows.Count);

                }

                response.msgType = 1;
                response.msg = "Favorites loaded successfully.";
                response.data = dt;
            }
            catch (Exception ex)
            {
                response.msgType = 3;
                response.msg = "Error: " + ex.Message;
            }

            return Json(response);
        }

        [HttpPost]
        public IActionResult DeleteFavorite([FromBody] dynamic data)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            response.msgType = 2;
            response.msg = "Failed to delete favorite.";

            try
            {
                var user = CommonHelper.GetValues(HttpContext);
                string connectionString = new SQLService().getconnstring();
                string menuCode = data.menuCode;

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string deleteQuery = $@"DELETE FROM TBL_FAVORITE_MENUS WHERE USER_ID = '{user.UserID}' AND MENU_CODE = '{menuCode}'";
                    SqlCommand cmd = new SqlCommand(deleteQuery, con);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        response.msgType = 1;
                        response.msg = "Favorite deleted successfully!";
                    }
                }
            }
            catch (Exception ex)
            {
                response.msgType = 3;
                response.msg = "Error: " + ex.Message;
            }

            return Json(response);
        }

        [HttpPost]
        public IActionResult UpdateSidebarState(int state)
        {
            var response = new MyHttpResponseMessage();
            try
            {
                var user = CommonHelper.GetValues(HttpContext);
                string connectionString = new SQLService().getconnstring();

                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string query = $@"UPDATE TBL_FAVORITE_MENUS SET IS_SIDEBAR_OPEN = {state} WHERE USER_ID = '{user.UserID}'";

                    SqlCommand cmd = new SqlCommand(query, con);
                    int rows = cmd.ExecuteNonQuery();

                    response.msgType = 1;
                    response.msg = "Sidebar state updated successfully!";
                }
            }
            catch (Exception ex)
            {
                response.msgType = 3;
                response.msg = "Error: " + ex.Message;
            }

            return Json(response);
        }



    }
}
