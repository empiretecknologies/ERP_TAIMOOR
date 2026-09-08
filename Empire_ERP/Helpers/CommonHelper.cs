using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Net;

namespace Empire_ERP.Helpers
{
    public class CommonHelper
    {
        public static void SetValues(HttpContext httpContext)
        {
            if (String.IsNullOrWhiteSpace(httpContext.Session.GetString("IPAddress")))
            {
                string ipAddress = GetIPAddress(httpContext);
                httpContext.Session.SetString("IPAddress", ipAddress);
            }

            if (String.IsNullOrWhiteSpace(httpContext.Session.GetString("ComputerName")))
            {
                var computerName = GetComputerName(httpContext);
                httpContext.Session.SetString("ComputerName", computerName);
            }
        }

        public static bool AreValuesReady(HttpContext httpContext)
        {
            var isReady = true;
            if (String.IsNullOrWhiteSpace(httpContext.Session.GetString("IPAddress")))
            {
                isReady = false;
            }

            if (String.IsNullOrWhiteSpace(httpContext.Session.GetString("ComputerName")))
            {
                isReady = false;
            }

            return isReady;
        }

        public static Common GetValues(HttpContext httpContext)
        {
            Common common = new Common();
            if (!AreValuesReady(httpContext))
            {
                if (String.IsNullOrWhiteSpace(httpContext.Session.GetString("IPAddress")))
                {
                    SetValues(httpContext);
                    common.IPAddress = httpContext.Session.GetString("IPAddress");
                }
                else
                {
                    common.IPAddress = httpContext.Session.GetString("IPAddress");
                }

                if (String.IsNullOrWhiteSpace(httpContext.Session.GetString("ComputerName")))
                {
                    SetValues(httpContext);
                    common.ComputerName = httpContext.Session.GetString("ComputerName");
                }
                else
                {
                    common.ComputerName = httpContext.Session.GetString("ComputerName");
                }

                if (!String.IsNullOrWhiteSpace(httpContext.Session.GetString("PostalCode")))
                {
                    common.PostalCode = httpContext.Session.GetString("PostalCode");
                }

                common.Username = httpContext.Session.GetString("Username");
                common.MenuID = CommonService.ParseInt(httpContext.Session.GetString("MenuID"));
                common.Branch = httpContext.Session.GetString("Branch");
                common.Period = httpContext.Session.GetString("Period");
                common.Company = CommonService.ParseInt(httpContext.Session.GetString("Company"));
                common.UserID = httpContext.Session.GetString("Id");
                common.RoleType = httpContext.Session.GetString("RoleType");
                common.RoleID = httpContext.Session.GetInt32("RoleId");
                common.ShowSelected = httpContext.Session.GetInt32("ShowSelected");
            }
            else
            {
                common.IPAddress = httpContext.Session.GetString("IPAddress");
                common.ComputerName = httpContext.Session.GetString("ComputerName");
                common.PostalCode = httpContext.Session.GetString("PostalCode");
                common.Username = httpContext.Session.GetString("Username");
                common.MenuID = CommonService.ParseInt(httpContext.Session.GetString("MenuID"));
                common.Branch = httpContext.Session.GetString("Branch");
                common.Period = httpContext.Session.GetString("Period");
                common.Company = CommonService.ParseInt(httpContext.Session.GetString("Company"));
                common.UserID = httpContext.Session.GetString("Id");
                common.RoleType = httpContext.Session.GetString("RoleType");
                common.RoleID = httpContext.Session.GetInt32("RoleId");
                common.ShowSelected = httpContext.Session.GetInt32("ShowSelected");
            }
            return common;
        }

        public static string GetIPAddress(HttpContext httpContext, bool lan = false)
        {
            var ipAddress = string.Empty;

            ipAddress = Convert.ToString(httpContext.Connection.RemoteIpAddress?.ToString());

            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = Convert.ToString(httpContext.Request.Headers["X-Forwarded-For"]);
            }

            if (string.IsNullOrEmpty(ipAddress) || ipAddress.Trim() == "::1")
            {
                lan = true;
                ipAddress = string.Empty;
            }

            if (lan)
            {
                if (string.IsNullOrEmpty(ipAddress))
                {
                    //This is for Local (LAN) Connected ID Address
                    string stringHostName = Dns.GetHostName();
                    //Get Ip Host Entry
                    IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                    System.Net.IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                    try
                    {
                        foreach (IPAddress ipAddressItem in arrIpAddress)
                        {
                            if (ipAddressItem.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                ipAddress = Convert.ToString(ipAddressItem);
                            }
                        }
                    }
                    catch
                    {

                        if (string.IsNullOrEmpty(ipAddress))
                            ipAddress = Convert.ToString(arrIpAddress[arrIpAddress.Length - 1]);
                        try
                        {
                            ipAddress = Convert.ToString(arrIpAddress[0]);
                        }
                        catch
                        {
                            try
                            {
                                arrIpAddress = Dns.GetHostAddresses(stringHostName);
                                ipAddress = Convert.ToString(arrIpAddress[0]);
                            }
                            catch
                            {
                                //local address
                                ipAddress = "127.0.0.1";
                            }
                        }
                    }
                }
            }

            return ipAddress;
        }

        public static string GetComputerName(HttpContext httpContext)
        {
            try
            {
                var ipAddress = GetIPAddress(httpContext);
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                var deviceId = $"{ipAddress}-{userAgent}";
                if (ipAddress != null)
                {
                    IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
                    return hostEntry?.HostName ?? string.Empty;
                }
            }
            catch (System.Exception)
            {
                // Handle DNS lookup failure
            }
            return string.Empty;
        }

        public static async Task<string> GetPostalCode(double latitude, double longitude)
        {
            try
            {
                HttpClient _httpClient = new HttpClient();
                var apiUrl = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}";
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/121.0.0.0 Safari/537.36");
                _httpClient.DefaultRequestHeaders.Add("Referer", "http://www.microsoft.com");
                var response = await _httpClient.GetAsync(apiUrl);        
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(content);

                    var address = data["address"];
                    var postalCode = address?["postcode"]?.ToString();

                    return postalCode;
                }
            }
            catch (System.Exception)
            {
                // Handle DNS lookup failure
            }
            return string.Empty;
        }

        private static dynamic GetCoordinates()
        {

            ////GeoCoordinate pin1 = new GeoCoordinate(lat, lng);
            ////GeoCoordinate pin2 = new GeoCoordinate(lat, lng);

            ////double distanceBetween = pin1.GetDistanceTo(pin2);
            ////GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();
            ////watcher.TryStart(false, TimeSpan.FromMilliseconds(10000));
            ////GeoCoordinate coord = watcher.Position.Location;
            ////if (coord != null)
            ////{
            ////    return new { Latitude = coord.Latitude, Longitude = coord.Longitude };
            ////}
            ////else
            ////{
            ////    return new { Latitude = 0, Longitude = 0 };
            ////}


            //GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();

            //watcher.PositionChanged += (sender, e) =>
            //{
            //    var coordinate = e.Position.Location;
            //    Console.WriteLine("Lat: {0}, Long: {1}", coordinate.Latitude,
            //        coordinate.Longitude);
            //    // Uncomment to get only one event.
            //    // watcher.Stop(); 
            //};

            //// Begin listening for location updates.
            //watcher.Start();
            return new { Latitude = 0, Longitude = 0 };
        }

        public static async Task<string> SetPostalCode(HttpContext httpContext, double latitude, double longitude)
        {
            string postalCode = string.Empty;
            if (String.IsNullOrWhiteSpace(httpContext.Session.GetString("PostalCode")))
            {
                postalCode = await GetPostalCode(latitude, longitude);
                httpContext.Session.SetString("PostalCode", postalCode);
            }
            return postalCode;
        }

        public static dynamic GetPermissionByMenueID(int? roleId, int? menuId)
        {
            object json = null;
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT * FROM TBL_ROLE WHERE ROLE_ID = {roleId} AND RMENU_ID = {menuId} AND MODULE_ID = 1";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var jsonDataResult = new
                    {
                        ROLE_ID = reader["ROLE_ID"],
                        ROLE_NAME = reader["ROLE_NAME"],
                        ROLE_TYPE = reader["ROLE_TYPE"],
                        MODULE_ID = reader["MODULE_ID"],
                        DT_CODE = reader["DT_CODE"],
                        R_ADD = Convert.ToBoolean(reader["R_ADD"]),
                        R_EDIT = Convert.ToBoolean(reader["R_EDIT"]),
                        R_DLT = Convert.ToBoolean(reader["R_DLT"]),
                        R_VIEW = Convert.ToBoolean(reader["R_VIEW"]),
                        R_PRINT = Convert.ToBoolean(reader["R_PRINT"]),
                        R_COPY = Convert.ToBoolean(reader["R_COPY"]),
                        R_BCODE = reader["R_BCODE"],
                        RMENU_ID = reader["RMENU_ID"]
                    };
                    json = jsonDataResult;
                }
                reader.Close();
            }
            return json;
        }

        public static int? GetLimitByMenueID(int? menuId)
        {
            int? limit = null;

            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                string query = $@"SELECT ISNULL(D_LIMIT, 0) AS LIMIT FROM TBL_MENU_BUILDER WHERE ID = {menuId}";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read() && reader["LIMIT"] != DBNull.Value)
                {
                    limit = Convert.ToInt32(reader["LIMIT"]);
                }

                reader.Close();
            }

            return limit;
        }

        public static string? GetCompanyName(int Id)
        {
            try
            {
                string maxIdQuery = "Select C_NAME from TBL_COMPANY WHERE CCODE = '" + Id + "' AND DLT = 'T'";
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(maxIdQuery, connection);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    var C_Name = Convert.ToString(result);
                    return C_Name;
                }
            }
            catch (System.Exception)
            {
            }
            return string.Empty;
        }
        public static string? GetBranch(string Id)
        {
            try
            {
                string maxIdQuery = "Select B_NAME from TBL_BRANCH WHERE DLT = 'T' AND BCODE = '" + Id + "'";
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(maxIdQuery, connection);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    var Branch = Convert.ToString(result);
                    return Branch;
                }
            }
            catch (System.Exception)
            {
            }
            return string.Empty;
        }
        public static string? GetPeriod(string Id)
        {
            try
            {
                string maxIdQuery = "Select DESCR from TBL_PERIOD WHERE DLT = 'T' AND PID = '" + Id + "'";
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    SqlCommand command = new SqlCommand(maxIdQuery, connection);
                    connection.Open();
                    object result = command.ExecuteScalar();
                    var Period = Convert.ToString(result);
                    return Period;
                }
            }
            catch (System.Exception)
            {
            }
            return string.Empty;
        }
    }
}

                