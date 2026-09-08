using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Ionic.Zip;
using System.Drawing;
using System.Text.Json.Nodes;
using ZXing;
using ZXing.Common;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class CommonRepository : ICommonRepository
    {
        public MyHttpResponseMessage GenerateQRCode(string content, string path)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {

                var relativePath = string.Empty;

                var barcodeWriter = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions { Height = 400, Width = 400, Margin = 0, PureBarcode = true }
                };


                var pixelData = barcodeWriter.Write(content);

                if (pixelData.Pixels.Length > 0)
                {
                    using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
                    using (var ms = new MemoryStream())
                    {
                        // lock the data area for fast access
                        var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
                           System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                        try
                        {
                            // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
                            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                               pixelData.Pixels.Length);
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }
                        // save to stream as PNG
                        path += @"/Client/ItemQRCodes/";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var filename = $"{Guid.NewGuid().ToString("N")}.png";
                        relativePath = $"/Client/ItemQRCodes/{filename}";
                        path += filename;
                        bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                response.data = new Uri(path).AbsoluteUri;
                response.msg = "";
                response.msgType = 1;
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
        public MyHttpResponseMessage GenerateBarCode(string content, string path)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var barcodeWriter = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.CODE_39,
                    Options = new EncodingOptions { Height = 50, Width = 50, Margin = 0, PureBarcode = true }
                };


                var pixelData = barcodeWriter.Write(content);

                // creating a bitmap from the raw pixel data; if only black and white colors are used it makes no difference
                // that the pixel data ist BGRA oriented and the bitmap is initialized with RGB
                // the System.Drawing.Bitmap class is provided by the CoreCompat.System.Drawing package
                if (pixelData.Pixels.Length > 0)
                {
                    using (var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
                    using (var ms = new MemoryStream())
                    {
                        // lock the data area for fast access
                        var bitmapData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, pixelData.Width, pixelData.Height),
                           System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                        try
                        {
                            // we assume that the row stride of the bitmap is aligned to 4 byte multiplied by the width of the image
                            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0,
                               pixelData.Pixels.Length);
                        }
                        finally
                        {
                            bitmap.UnlockBits(bitmapData);
                        }

                        path += @"/Client/ItemBarcodes/";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var filename = $"{Guid.NewGuid().ToString("N") + "_" + content.Replace("/", "").Replace("-", "") + ".png"}";
                        path += filename;
                        bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                    }
                }
                response.data = new Uri(path).AbsoluteUri;
                response.msg = "";
                response.msgType = 1;
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
        public bool CreateZipFromSpecificFile(string sourceFilePath, string destinationZipFilePath, string password)
        {
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.Password = password;
                    zip.AddFile(sourceFilePath, "");
                    zip.Save(destinationZipFilePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string ToAccountingFormat(decimal value)
        {
            return value >= 0 ? value.ToString("#,##0.00") : "(" + Math.Abs(value).ToString("#,##0.00") + ")";
        }

        public string BuildFilterCondition(JsonArray filterArray)
        {
            if (filterArray == null || filterArray.Count == 0)
                return string.Empty;

            if (filterArray[0] is JsonArray)
            {
                var firstFilter = (JsonArray)filterArray[0];
                string condition = BuildSingleCondition(firstFilter);

                for (int i = 1; i < filterArray.Count; i += 2)
                {
                    string logicalOperator = filterArray[i].ToString();
                    var nextFilter = (JsonArray)filterArray[i + 1];
                    condition += $" {logicalOperator.ToUpper()} " + BuildSingleCondition(nextFilter);
                }
                return $"AND ({condition})";
            }
            else
            {
                return $"AND {BuildSingleCondition(filterArray)}";
            }
        }

        public string BuildSingleCondition(JsonArray filter)
        {
            string field = filter[0].ToString();
            string operation = filter[1].ToString();
            string value = filter[2].ToString();

            switch (operation.ToLower())
            {
                case "contains":
                    return $"{field} LIKE '%{value}%'";
                case "startswith":
                    return $"{field} LIKE '{value}%'";
                case "endswith":
                    return $"{field} LIKE '%{value}'";
                case "=":
                    return $"{field} = '{value}'";
                case ">":
                    return $"{field} > '{value}'";
                case ">=":
                    return $"{field} >= '{value}'";
                case "<":
                    return $"{field} < '{value}'";
                case "<=":
                    return $"{field} <= '{value}'";
                // Add more cases as needed
                default:
                    return string.Empty;
            }
        }

        public object GetPropertyValue(dynamic obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo != null ? propertyInfo.GetValue(obj, null) : null;
        }
    }
}
