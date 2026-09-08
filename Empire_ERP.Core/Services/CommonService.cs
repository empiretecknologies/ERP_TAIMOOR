using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZXing;
using ZXing.Common;
using System.Net.Http;
using System.Drawing;
using SkiaSharp;
using System.Reflection.Metadata;
using ZXing.QrCode;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Entities;
using System.IO.Compression;
using System.IO;

namespace Empire_ERP.Core.Services
{
    public class CommonService : ICommonService
    {
        public ICommonRepository _commonRepository { get; set; }
        public CommonService(ICommonRepository commonRepository)
        {
            _commonRepository = commonRepository;
    }
        public const string LoginUserSession = "LoginUserSession";
        public const string LoginUserCookie = "LoginUserCookie";
        public static string EncryptString(string plainText)
        {
            if (true)
            {

                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        plainText = Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
            return plainText;
        }
        public static string DecryptString(string cipherText)
        {
            if (true)
            {



                string EncryptionKey = "MAKV2SPBNI99212";
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes encryptor = Aes.Create())
                {
                    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        cipherText = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
            }


            return cipherText;
        }
        public static DataTable DataTableResponse(string msg, string data, int msgType)
        {

            DataTable dataTable = new DataTable("Response");

            // Define columns for the DataTable
            DataColumn columnID = new DataColumn("msg", typeof(string));
            DataColumn columnName = new DataColumn("msgType", typeof(int));
            DataColumn columnAge = new DataColumn("data", typeof(string));

            // Add the columns to the DataTable
            dataTable.Columns.Add(columnID);
            dataTable.Columns.Add(columnName);
            dataTable.Columns.Add(columnAge);

            // Add some rows to the DataTable
            DataRow row1 = dataTable.NewRow();
            row1["msgType"] = msgType;
            row1["msg"] = msg;
            row1["data"] = data;
            dataTable.Rows.Add(row1);
            return dataTable;
        }
        public static string GetMacAddress()
        {
            string macAddress = string.Empty;
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                string mac = nic.GetPhysicalAddress().ToString();
                {
                    if (macAddress == string.Empty)
                    {
                        macAddress = mac;
                    }
                    else
                    {
                        macAddress += mac != "" ? "-" + nic.GetPhysicalAddress().ToString() : string.Empty;
                    }
                }
            }
            return macAddress;
        }
        public static PhysicalAddress GetMacAddress1()
        {
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    return nic.GetPhysicalAddress();
                }
            }
            return null;
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "IP Address N/A";
            //throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        //public static string GetPostalCode()
        //{

        //    try
        //    {
        //        GeoCoordinateWatcher watcher = new GeoCoordinateWatcher();

        //        if (watcher.TryStart(false, TimeSpan.FromMilliseconds(1000)))
        //        {
        //            GeoCoordinate coord = watcher.Position.Location;

        //            if (coord.IsUnknown)
        //            {
        //                Console.WriteLine("Could not determine your location.");
        //            }
        //            else
        //            {
        //                string postalCode = GetPostalCode(coord.Latitude, coord.Longitude);
        //                Console.WriteLine($"Your postal code is: {postalCode}");
        //            }
        //        }
        //        else
        //        {
        //            Console.WriteLine("Location services are not available.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error: {ex.Message}");
        //    }

        //}
        public static string UpperCaseWords(string value)
        {
            char[] array = value.ToCharArray();
            // Handle the first letter in the string.
            if (array.Length >= 1)
            {
                if (char.IsLower(array[0]))
                {
                    array[0] = char.ToUpper(array[0]);
                }
            }
            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
        }
        public static string GetSettingContentByName(string _Key)
        {
            string ReturnContent = "#";
            //var SettingRecord = dbContext.TBL_SETTING.FirstOrDefault(o => o.Name.Equals(_Key));
            //if (SettingRecord != null)
            //{
            //    ReturnContent = SettingRecord.Contents;
            //}
            ReturnContent = "https://localhost:44326/";
            return ReturnContent;
        }
        public static string ConvertToWebURL(string _value)
        {
            if (!string.IsNullOrWhiteSpace(_value) && !_value.Equals("#"))
            {
                if (_value.IndexOf("www.") == -1 && _value.IndexOf("http") == -1 && _value.IndexOf("https") == -1)
                {
                    //if (dbContext == null)
                    // {
                    // dbContext = new EmpireContext();
                    // }
                    if (_value.ToLower().Equals("home") || _value.Equals("/"))
                    {
                        _value = GetSettingContentByName("Website URL") + _value;
                    }
                    else
                    {
                        _value = GetSettingContentByName("Website URL") + _value;
                    }
                }
            }
            else
            {
                _value = "#";
            }
            return _value;
        }

        public static string GetDateTime(string timeZoneId)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            return currentTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static string GetDate(string timeZoneId)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            DateTime currentTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
            return currentTime.ToString("yyyy-MM-dd");
        }

        #region "Page Name Constant Value Helper"
        public const string PageLogin = "login";
        public const string PageForgotPassword = "forgot-password";
        public const string PageResetPassword = "reset-password";
        public const string PageRegister = "register";
        public const string PageDashboard = "dashboard";
        public const string PageProfile = "profile";
        public const string PageLogout = "logout";
        public const string TimeZone = "Pakistan Standard Time";
        #endregion

        #region "Default Values Helper"
        public static int ParseInt(object value)
        {
            int parseVal;
            return ((value == null) || (value == DBNull.Value)) ? 0 : int.TryParse(value.ToString(), out parseVal) ? parseVal : 0;
        }
        public static decimal ParseDecimal(object value)
        {
            decimal parseVal;
            return ((value == null) || (value == DBNull.Value)) ? 0 : decimal.TryParse(value.ToString(), out parseVal) ? parseVal : 0;
        }
        public static double ParseDouble(object value)
        {
            double parseVal;
            return ((value == null) || (value == DBNull.Value)) ? 0 : double.TryParse(value.ToString(), out parseVal) ? parseVal : 0;
        }
        public static DateTime ParseDateTime(object value)
        {
            DateTime parseVal;
            return ((value == null) || (value == DBNull.Value)) ? new DateTime(1900, 1, 1) : DateTime.TryParse(value.ToString(), out parseVal) ? parseVal : new DateTime(1900, 1, 1);
        }
        public static string ParseString(object value)
        {
            return ((value == null) || (value == DBNull.Value)) ? string.Empty : value.ToString();
        }
        public static bool ParseBoolean(object value)
        {
            bool parseVal;
            return ((value == null) || (value == DBNull.Value)) ? false : bool.TryParse(value.ToString(), out parseVal) ? parseVal : false;
        }
        public static bool IsEmailAddressValid(string EmailAddress)
        {
            string pattern = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(EmailAddress);
        }
        public static bool IsPasswordValid(string password)
        {
            string pattern = @"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(password);
        }
        #endregion

        public MyHttpResponseMessage GenerateQRCode(string content, string path)
        {
            return _commonRepository.GenerateQRCode(content, path);
        }

        public MyHttpResponseMessage GenerateBarCode(string content, string path)
        {
            return _commonRepository.GenerateBarCode(content, path);
        }

        public bool CreateZipFromSpecificFile(string sourceFilePath, string destinationZipFilePath, string password)
        {
            return _commonRepository.CreateZipFromSpecificFile(sourceFilePath, destinationZipFilePath, password);
        }

        public string ToAccountingFormat(decimal value)
        {
            return _commonRepository.ToAccountingFormat(value);
        }
    }
}
