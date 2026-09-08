using System.Drawing;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using ZXing;
using ZXing.Common;

namespace Empire_ERP.Infrastructure.Repositories
{
    public class BarcodePrintRepository : IBarcodePrintRepository
    {
        public IMenuRepository _menuRepository { get; set; }

        public BarcodePrintRepository(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public MyHttpResponseMessage GetData(string sdate,string edate)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                List<object> jsonDataResult = new List<object>();
                using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
                {
                    string query = $@"EXEC STKPROC 143,'{sdate}','{edate}','','','',''";
                    //string query = @"SELECT IG.GROUP_NAME AS ITEM_GROUP, BG.ITEM_CODE,IM.ITEM_NAME, IM.REMARKS AS REMARKS,BG.BARCODE,SZ.GROUP_NAME AS SIZE_NAME,
                    //                CL.GROUP_NAME AS COLOR_NAME,SRATE,BG.WSALE,BG.RRATE , 1 AS QTY, ISNULL(BLABEL, 0) AS BLABEL, IC.GROUP_NAME AS CAT_CODE, IM.ITEM_ID
                    //                FROM TBL_BARCODE BG
                    //                LEFT OUTER JOIN TBL_ITEMSMASTER IM
                    //                ON IM.ITEM_CODE = BG.ITEM_CODE
                    //                LEFT OUTER JOIN TBL_SIZE SZ
                    //                ON SZ.GROUP_CODE = BG.SIZE
                    //                LEFT OUTER JOIN TBL_COLOR CL
                    //                ON CL.GROUP_CODE = BG.COLOR
                    //                LEFT OUTER JOIN TBL_ITEMSGROUP IG
                    //                ON IG.GROUP_CODE = IM.GROUP_CODE
                    //                LEFT OUTER JOIN TBL_CATEGORY IC
                    //                ON IC.GROUP_CODE = IM.CAT_CODE
                    //                WHERE IM.ASTATUS = 'Y' AND BG.ASTATUS = 'Y' AND  SZ.ASTATUS = 'Y' AND CL.ASTATUS = 'Y'
                    //                AND IM.DLT = 'T' AND BG.DLT = 'T' AND SZ.DLT = 'T' AND CL.DLT = 'T'";

                    SqlCommand command = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new BarcodePrint
                        {
                            ITEM_CODE = Convert.ToInt32(reader["ITEM_CODE"]),
                            ITEM_NAME = Convert.ToString(reader["ITEM_NAME"]),
                            ITEM_GROUP = Convert.ToString(reader["ITEM_GROUP"]),
                            REMARKS = Convert.ToString(reader["REMARKS"]),
                            BARCODE = Convert.ToString(reader["BARCODE"]),
                            BARCODE_TEXT = Convert.ToString(reader["BARCODE_TEXT"]),
                            SIZE_NAME = Convert.ToString(reader["SIZE_NAME"]),
                            COLOR_NAME = Convert.ToString(reader["COLOR_NAME"]),
                            SRATE = Convert.ToDouble(reader["SRATE"]),
                            WSALE = Convert.ToDouble(reader["WSALE"]),
                            RRATE = Convert.ToDouble(reader["RRATE"]),
                            QTY = Convert.ToInt32(reader["QTY"]),
                            ISSALE = false,
                            ISRETAIL = false,
                            ISWHOLESALE = false,
                            BLABEL = Convert.ToInt32(reader["BLABEL"]),
                            CAT_CODE = Convert.ToString(reader["CAT_CODE"]),
                            ITEM_ID = Convert.ToString(reader["ITEM_ID"]),
                        };
                        jsonDataResult.Add(row);
                    }

                    reader.Close();
                }

                response.data = jsonDataResult;
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

        public string GenerateBarcode(string content, string path)
        {
            var relativePath = string.Empty;
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.CODE_39,
                Options = new EncodingOptions { Height = 100, Width = 400, Margin = 0, PureBarcode = true }
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
                    // save to stream as PNG
                    path += @"/Client/ItemBarcodes/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var filename = $"{Guid.NewGuid().ToString("N") + "_" + content + ".png"}";
                    relativePath = $"/Client/ItemBarcodes/{filename}";
                    path += filename;
                    bitmap.Save(path, System.Drawing.Imaging.ImageFormat.Png);
                }
            }
            return new Uri(path).AbsoluteUri;// relativePath;

            //// Convert pixel data to byte array
            //byte[] bytes = pixelData.Pixels;

            //// Convert byte array to base64 string
            //string base64String = Convert.ToBase64String(bytes);
            //return base64String;
        }
    }
}