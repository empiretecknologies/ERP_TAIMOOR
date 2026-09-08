using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Cmp;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using System.Data;
using System.Text;
using static Azure.Core.HttpHeader;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class SaleTaxInvoiceController : BaseController
    {
        public ISaleTaxInvoiceService _saleTaxInvoiceService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public SaleTaxInvoiceController(ISaleTaxInvoiceService saleTaxInvoiceService, IMenuService menuService, IWebHostEnvironment hostingEnvironment, IBaseService baseService) : base(menuService, baseService)
        {
            _saleTaxInvoiceService = saleTaxInvoiceService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.DateTime = CommonService.GetDateTime("Pakistan Standard Time");
            var BranchID = HttpContext.Session.GetString("Branch");
            var CompanyID = HttpContext.Session.GetString("Company");
            //string nextId = "";
            //string formType = "";

            //string maxIdQuery = "SELECT PICK_TYPE, B_I FROM TBL_MENU_BUILDER WHERE DLT = 'T' AND ID = '" + common.MenuID + "'";
            //using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            //{
            //    SqlCommand command = new SqlCommand(maxIdQuery, connection);
            //    connection.Open();
            //    using (SqlDataReader reader = command.ExecuteReader())
            //    {
            //        if (reader.Read())
            //        {
            //            formType = reader.IsDBNull(0) ? "" : reader.GetString(0);
            //            nextId = reader.GetValue(1).ToString();
            //        }
            //    }
            //}

            ViewBag.Items = DropdownService.ItemMasterDropdownWithUnits();
            ViewBag.fbrType = DropdownService.FBRTypeDropdown();
            //ViewBag.Type = nextId;
            //ViewBag.FormType = formType;
            ViewBag.Units = DropdownService.UnitDropdown();
            ViewBag.PartyType = common.RoleType == "A"
                ? DropdownService.PartyTypeDropdownForInvoice(0, common.Branch, 0)
                : DropdownService.PartyTypeDropdownForInvoice(common.RoleID, common.Branch, common.ShowSelected);
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            ViewBag.Limit = CommonHelper.GetLimitByMenueID(common.MenuID);
            var response = _menuService.GetMenu(common.MenuID);
            if (response.msgType == 1)
            {
                ViewBag.DATA_CLEAR = ((Menu)response.data).DATA_CLEAR;
            }
            return View();
        }

        [HttpGet]
        public JsonResult GetParties()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = common.RoleType == "A"
                    ? DropdownService.PartyTypeDropdownForInvoice(0, common.Branch, 0)
                    : DropdownService.PartyTypeDropdownForInvoice(common.RoleID, common.Branch, common.ShowSelected);
                return Json(new { data = data, msgType = 1 });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        [HttpGet]
        public JsonResult GetItems()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = DropdownService.ItemMasterDropdown(common.RoleID, common.RoleType);
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        [HttpGet]
        public JsonResult GetUnits()
        {
            try
            {
                var data = DropdownService.UnitDropdown();
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        //[HttpGet]
        //public JsonResult GetColors()
        //{
        //    try
        //    {
        //        var data = DropdownService.ColorDropdown();
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}

        //[HttpGet]
        //public JsonResult GetSizes()
        //{
        //    try
        //    {
        //        var data = DropdownService.SizeDropdown();
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}

        //[HttpGet]
        //public JsonResult GetGrades()
        //{
        //    try
        //    {
        //        var data = DropdownService.GradeDropdown();
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(_catchMessage);
        //    }
        //}

        [HttpGet]
        public JsonResult GetQuickSearchData()
        {
            try
            {
                var data = _saleTaxInvoiceService.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        [HttpGet]
        public JsonResult GetSaleTaxInvoiceByCode(int code)
        {
            try
            {
                var data = _saleTaxInvoiceService.GetSaleTaxInvoiceByCode(code, CommonHelper.GetValues(HttpContext));
                var detailData = _saleTaxInvoiceService.GetSaleTaxInvoiceDetailByCode(code, CommonHelper.GetValues(HttpContext));

                var invoices = data.data as List<SaleTaxInvoice>;

                string fbrNo = invoices?.FirstOrDefault()?.FBR_NO;

                var qrCode = GenerateQrCode(fbrNo);

                return Json(new { Master = data, Detail = detailData, qrcode = qrCode });
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        [HttpPost]
        public JsonResult Save(CustomSaleTaxInvoice modelRecord)
        {
            try
            {
                var data = _saleTaxInvoiceService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        [HttpPost]
        public JsonResult Delete(int code)
        {
            try
            {
                var data = _saleTaxInvoiceService.Delete(code, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        [HttpPost]
        public JsonResult CopyRecord(CopyRecord record)
        {
            try
            {
                var data = _saleTaxInvoiceService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { msg = _catchMessage, msgType = 2 });
            }
        }

        [HttpPost]
        public JsonResult DeleteSaleTaxInvoiceDetailByCode(int code)
        {
            try
            {
                var data = _saleTaxInvoiceService.DeleteSaleTaxInvoiceDetailByCode(code, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(_catchMessage);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDataForApi(int code)
        {
            try
            {
                var masterResponse = _saleTaxInvoiceService
                                        .GetDataForApi(code, CommonHelper.GetValues(HttpContext));

                var masterList = masterResponse?.data as List<SaleTaxInvoice>;
                if (masterList == null || !masterList.Any())
                    return Json(new { msg = "No data found", msgType = 2 });

                var master = masterList.First();

                var detailData = _saleTaxInvoiceService
                                    .GetDetailDataForApi(code, CommonHelper.GetValues(HttpContext));

                var items = new List<FBRItemModel>();

                //foreach (var d in detailData)
                //{
                //    string rateValue = $"{d.TAX ?? 0}%";
                //    decimal valueSalesExcludingST = (int)(d.VALUE_SALES_EXCLUDING ?? 0);
                //    decimal salesTaxApplicable;

                //    if (d.S_NAME == "Exempt goods")
                //    {
                //        rateValue = "Exempt";
                //        valueSalesExcludingST = (int)(d.TOTAL_VALUES ?? 0);
                //    }
                //    else if (d.S_NAME == "Goods at zero-rate")
                //    {
                //        valueSalesExcludingST = (int)(d.TOTAL_VALUES ?? 0);
                //    }
                //    else
                //    {
                //        rateValue = "18%"; 
                //        valueSalesExcludingST = (int)(d.VALUE_SALES_EXCLUDING ?? 0);
                //        salesTaxApplicable = (int)(d.ST_APPLICABLE ?? 0);
                //    }

                //    items.Add(new FBRItemModel
                //        {
                //            hsCode = d.HS_CODE,
                //            productDescription = d.ITEM_NAME,
                //            rate = rateValue,
                //            uoM = d.UOM,
                //            quantity = d.QTY.HasValue ? (int?)d.QTY.Value : null,
                //            totalValues = d.TOTAL_VALUES,
                //            valueSalesExcludingST = valueSalesExcludingST,
                //            fixedNotifiedValueOrRetailPrice = d.FIXEDVALUE_RETAILPRICE.HasValue ? (int?)d.FIXEDVALUE_RETAILPRICE.Value : null,
                //            salesTaxApplicable = d.ST_APPLICABLE.HasValue ? (int?)d.ST_APPLICABLE.Value : null,
                //            salesTaxWithheldAtSource = 0,
                //            extraTax = 0,
                //            furtherTax = 0,
                //            sroScheduleNo = d.SRO_SCH_NO,
                //            fedPayable = 0,
                //            discount = 0,
                //            saleType = d.S_NAME,
                //            sroItemSerialNo = d.SERIAL_NO?.ToString()
                //        });
                //}
                //foreach (var d in detailData)
                //{
                //    string rateValue = "18%";

                //    decimal valueSalesExcludingST = (decimal)(d.VALUE_SALES_EXCLUDING ?? 0);
                //    decimal salesTaxApplicable = 0m;
                //    decimal totalValues = 0m;

                //    if (d.S_NAME == "Exempt goods")
                //    {
                //        rateValue = "Exempt";
                //        valueSalesExcludingST = (decimal)(d.TOTAL_VALUES ?? 0);
                //        salesTaxApplicable = 0m;
                //        totalValues = valueSalesExcludingST;
                //    }
                //    else if (d.S_NAME == "Goods at zero-rate")
                //    {
                //        rateValue = "0%";
                //        valueSalesExcludingST = (decimal)(d.TOTAL_VALUES ?? 0);
                //        salesTaxApplicable = 0m;
                //        totalValues = valueSalesExcludingST;
                //    }
                //    else
                //    {
                //        rateValue = "18%";
                //        salesTaxApplicable =
                //            Math.Round(valueSalesExcludingST *  18 / 100, 2);
                //        totalValues =
                //            Math.Round(valueSalesExcludingST + salesTaxApplicable, 2);
                //    }

                //    items.Add(new FBRItemModel
                //    {
                //        hsCode = d.HS_CODE,
                //        productDescription = d.ITEM_NAME,
                //        rate = rateValue,
                //        uoM = d.UOM,
                //        quantity = d.QTY.HasValue ? (int?)d.QTY.Value : null,
                //        totalValues = totalValues,
                //        valueSalesExcludingST = valueSalesExcludingST,
                //        fixedNotifiedValueOrRetailPrice = d.FIXEDVALUE_RETAILPRICE.HasValue
                //            ? (int?)d.FIXEDVALUE_RETAILPRICE.Value
                //            : null,
                //        salesTaxApplicable = salesTaxApplicable,
                //        salesTaxWithheldAtSource = 0,
                //        extraTax = 0,
                //        furtherTax = 0,
                //        sroScheduleNo = d.SRO_SCH_NO,
                //        fedPayable = 0,
                //        discount = 0,
                //        saleType = d.S_NAME,
                //        sroItemSerialNo = d.SERIAL_NO?.ToString()
                //    });
                //}
                //foreach (var d in detailData)
                //{
                //    string rateValue = "";
                //    decimal valueExcl = (decimal)(d.VALUE_SALES_EXCLUDING ?? 0);
                //    decimal salesTaxApplicable = 0m;
                //    decimal totalValues = valueExcl;

                //    if (d.S_NAME == "Exempt goods")
                //    {
                //        rateValue = "Exempt";

                //    }
                //    else if (d.S_NAME == "Goods at zero-rate")
                //    {
                //        rateValue = "0%";

                //    }else if(d.S_NAME == "Goods at standard rate (default)")
                //    {
                //        decimal fbrRate = 18;
                //        rateValue = $"{fbrRate}%";
                //        salesTaxApplicable = Math.Round(valueExcl * fbrRate / 100, 2);
                //        totalValues = Math.Round(valueExcl + salesTaxApplicable, 2);

                //    }

                //    items.Add(new FBRItemModel
                //    {
                //        hsCode = d.HS_CODE,
                //        productDescription = d.ITEM_NAME,
                //        rate = rateValue,
                //        uoM = d.UOM,
                //        quantity = d.QTY.HasValue ? (int)d.QTY.Value : 1,
                //        totalValues =totalValues,
                //        valueSalesExcludingST = valueExcl,
                //        salesTaxApplicable = salesTaxApplicable,
                //        fixedNotifiedValueOrRetailPrice = 0,
                //        salesTaxWithheldAtSource = 0,
                //        extraTax = 0,
                //        furtherTax = 0,
                //        fedPayable = 0,
                //        discount = 0,
                //        saleType = d.S_NAME,
                //        sroScheduleNo = d.SRO_SCH_NO ?? "",
                //        sroItemSerialNo = d.SERIAL_NO?.ToString() ?? ""
                //    });
                //}
                //var items = new List<FBRItemModel>();

                foreach (var d in detailData)
                {
                    decimal taxRate = d.TAX.HasValue ? (decimal)d.TAX.Value : 0m;
                    string rateValue = $"{taxRate}%";
                    // Convert nullable double to decimal safely
                    decimal valueExcl = d.VALUE_SALES_EXCLUDING.HasValue ? (decimal)d.VALUE_SALES_EXCLUDING.Value : 0m;
                    decimal salesTaxApplicable = 0m;
                    decimal totalValues = valueExcl;

                    // Determine tax rate label and calculate tax
                    if (d.S_NAME == "Exempt goods")
                    {
                        rateValue = "Exempt";
                    }
                    else if (d.S_NAME == "Goods at zero-rate")
                    {
                        rateValue = "0%";
                    }
                    else if (d.S_NAME == "Goods at standard rate (default)")
                    {
                      
                        salesTaxApplicable = Math.Round(valueExcl * taxRate / 100, 2);

                        // Calculate total including tax
                        totalValues = Math.Round(valueExcl + salesTaxApplicable, 2);
                    }

                    // Now you have:
                    // rateValue => string showing rate or "Exempt"
                    // salesTaxApplicable => calculated tax amount
                    // totalValues => value including tax

                    //Add item to FBR payload
                    items.Add(new FBRItemModel
                    {
                        hsCode = d.HS_CODE,
                        productDescription = d.ITEM_NAME,
                        rate = rateValue,
                        uoM = d.UOM,
                        quantity = d.QTY.HasValue ? (int)d.QTY.Value : 1, 
                        totalValues = totalValues,
                        valueSalesExcludingST = valueExcl,
                        fixedNotifiedValueOrRetailPrice = (int?)Math.Round(d.FIXEDVALUE_RETAILPRICE ?? 0, 0),
                        salesTaxApplicable = salesTaxApplicable,
                        salesTaxWithheldAtSource = 0m,
                        extraTax = 0m,
                        furtherTax = 0m,
                        fedPayable = 0m,
                        discount = 0m,
                        saleType = d.S_NAME,
                        sroScheduleNo = d.SRO_SCH_NO ?? "",
                        sroItemSerialNo = d.SERIAL_NO?.ToString() ?? "0"
                    });
                }



                var model = new FBRModel
                {
                    FBRUrlToken = new FBRUrlToken
                    {
                        Url = "https://gw.fbr.gov.pk/di_data/v1/di/postinvoicedata_sb",
                        Token = "331a9d2a-d1f3-3a39-85b9-3bc3e952b9c9"

                        
                    },
                    FBRPostModel = new FBRPostModel
                    {
                        invoiceType = "Sale Invoice",
                        invoiceDate = master.INVOICE_DATE,
                        sellerNTNCNIC = master.SELLER_NTN,
                        sellerBusinessName = master.SELLER_BNAME,
                        sellerProvince = master.SELLER_PROVINCE,
                        sellerAddress = master.SELLER_ADDRESS,
                        buyerNTNCNIC = master.BUYER_NTN,
                        buyerBusinessName = master.BUYER_BNAME,
                        buyerProvince = master.BUYER_PROVINCE,
                        buyerAddress = master.BUYER_ADDRESS,
                        buyerRegistrationType = master.BUYER_REG_TYPE,
                        invoiceRefNo="",
                        scenarioId = master.SCENARIO_ID,
                        items = items
                    }
                };

                var finalJson = JsonConvert.SerializeObject(model.FBRPostModel,Formatting.Indented);

                System.IO.File.WriteAllText(@"D:\fbr_request.json", finalJson);


                var fbrResult = await PostToFBR(model);

                dynamic fbrData = JsonConvert.DeserializeObject<dynamic>((fbrResult as ContentResult).Content);

                return Json(new
                {
                    msgType = 1, 
                    msg = "FBR response retrieved",
                    response = fbrData.response,
                    qrCode = fbrData.qrCode
                });
            }
            catch (Exception ex)
            {
                return Json(new { msg = ex.Message, msgType = 2 });
            }
        }




        //public JsonResult GetDataForApi(int code)
        //{
        //    try
        //    {
        //        var data = _saleTaxInvoiceService.GetDataForApi(code, CommonHelper.GetValues(HttpContext));
        //        var detailData = _saleTaxInvoiceService.GetDetailDataForApi(code,CommonHelper.GetValues(HttpContext));
        //        var abc = detailData;
        //        return Json(data);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        return Json(new { msg = _catchMessage, msgType = 2 });
        //    DeleteSaleTaxInvoiceDetailByCode
        //}

        [HttpPost]
        public async Task<IActionResult> PostToFBR([FromBody] FBRModel model)
        {
            try
            {
                var client = new RestClient(model.FBRUrlToken.Url);

                var request = new RestRequest();
                request.Method = Method.POST;
                request.AddHeader("Authorization", $"Bearer {model.FBRUrlToken.Token}");
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Cookie", "key=value; JSESSIONID=I-Q2LluP6Fk3_mV2QT9Aty52WqDxw6Wv2IvAxxz2.i01-irisdmz55; cookiesession1=678B28F284EBD23002739A5C50759553");
                request.AddJsonBody(model.FBRPostModel);
                var response = await client.ExecuteAsync(request);

                var responseObj = JsonConvert.DeserializeObject<FBRPostResponse>(response.Content);

                string qrText =
                    responseObj?.invoiceNumber
                    ?? responseObj?.validationResponse?.error
                    ?? "Unknown response";

                string qrCode = GenerateQrCode(qrText);

                return Content(JsonConvert.SerializeObject(new
                {
                    response = response.Content,
                    qrCode = qrCode
                }), "application/json");


                //return Content(response.Content, "application/json");
                //return Content(JsonConvert.SerializeObject(new
                //{
                //    response = response.Content,
                //    qrCode = GenerateQrCode(response.Content)
                //}), "application/json");

            }
            catch (Exception ex)
            {
                return Content("{\"error\":\"" + ex.Message + "\"}", "application/json");
            }
        }

        public static string GenerateQrCode(string text)
        {
            if (!string.IsNullOrEmpty(text))
            {
                var writer = new ZXing.BarcodeWriterPixelData
                {
                    Format = ZXing.BarcodeFormat.QR_CODE,
                    Options = new ZXing.Common.EncodingOptions
                    {
                        Height = 300,
                        Width = 300,
                        Margin = 2
                    }
                };

                var pixelData = writer.Write(text);

                using (var bitmap = new SkiaSharp.SKBitmap(new SkiaSharp.SKImageInfo(pixelData.Width, pixelData.Height)))
                {
                    // Pixel copy
                    var ptr = bitmap.GetPixels();
                    System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, ptr, pixelData.Pixels.Length);

                    using (var image = SkiaSharp.SKImage.FromBitmap(bitmap))
                    using (var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100))
                    {
                        return Convert.ToBase64String(data.ToArray());
                    }
                }
            }
            else
            {
                return string.Empty;
            }
        }

        [HttpGet]
        public JsonResult FBRApi_Status(string code, string apiResponce)
        {
            var data = _saleTaxInvoiceService.FBRApi_Status(code, apiResponce, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetReportTypes()
        {
            try
            {
                var data = _menuService.GetMenuDetails(CommonHelper.GetValues(HttpContext).MenuID);
                return Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage += "<br/>" + ex.InnerException.Message;
                }
                return Json(new { data = _catchMessage, msgType = 2 });
            }
        }

        [HttpPost]
        public JsonResult GetPrintReport(SaleTaxInvoiceRDLCReport model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var filePath = GenerateReport(model);
                if (!String.IsNullOrEmpty(filePath))
                {
                    response.data = filePath;
                    response.msg = "";
                    response.msgType = 1;
                }
                else
                {
                    response.msg = "Unable to generate report. Please try again later.";
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
            return Json(response);
        }

        public string GenerateQrCodeZXingToFile(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            string folder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\QR");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = $"{Guid.NewGuid()}.png";
            string fullPath = Path.Combine(folder, fileName);

            var writer = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 300,
                    Width = 300,
                    Margin = 2
                }
            };

            var pixelData = writer.Write(text);

            using (var bitmap = new SkiaSharp.SKBitmap(new SkiaSharp.SKImageInfo(pixelData.Width, pixelData.Height)))
            {
                var ptr = bitmap.GetPixels();
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, ptr, pixelData.Pixels.Length);

                using (var image = SkiaSharp.SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100))
                using (var stream = System.IO.File.OpenWrite(fullPath))
                {
                    data.SaveTo(stream);
                }
            }

            return $"/Client/QR/{fileName}";
        }
        private string GenerateReport(SaleTaxInvoiceRDLCReport model)
        {
            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0 && model.REPORT_NAME != null)
                {
                    Reports.Datasets.BarcodeReportDataset.SaleTaxInvoiceDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.SaleTaxInvoiceDataTable();
                    var responseMessage = _saleTaxInvoiceService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return "";
                    }
                    var reportData = (CustomSaleTaxInvoiceForPrintReport)responseMessage.data;

                    using (LocalReport report = new LocalReport())
                    {
                        var path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\{reportData.Master?.REPORT_NAME}.rdlc");
                        var stReader = new StreamReader(path);
                        string stringreader = stReader.ReadToEnd();
                        byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
                        MemoryStream stream = new MemoryStream(byteArray);
                        report.EnableExternalImages = true;
                        report.LoadReportDefinition(stream);
                        report.DataSources.Clear();

                        if (reportData.Master?.REPORT_NAME == "SaleTaxInvoice")
                        {
                            var companyLogoPath = Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}");
                            bool? showCompanyLogo = true;

                            if (!System.IO.File.Exists(companyLogoPath))
                            {
                                showCompanyLogo = false;
                            }

                            string qrRelativePath = GenerateQrCodeZXingToFile(reportData.Master?.FBR_NO);

                            string qrFullPath = Path.Combine(_hostingEnvironment.WebRootPath, qrRelativePath.TrimStart('/'));


                            ReportParameter parameter23 = new ReportParameter("InvoiceQR", new Uri(qrFullPath).AbsoluteUri);

                            ReportParameter parameter1 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
                            ReportParameter parameter2 = new ReportParameter("InvoiceNumber", reportData.Master?.INVOICE_NUMBER);
                            ReportParameter parameter3 = new ReportParameter("Date", reportData.Master?.DATE);
                            ReportParameter parameter4 = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
                            ReportParameter parameter5 = new ReportParameter("BType", reportData.Master?.BTYPE);

                            ReportParameter parameter6 = new ReportParameter("Party", reportData.Master?.PARTY_NAME);
                            ReportParameter parameter7 = new ReportParameter("PAddress", reportData.Master?.PADDRESS);
                            ReportParameter parameter8 = new ReportParameter("Tell", reportData.Master?.TELL);
                            ReportParameter parameter9 = new ReportParameter("PTNtn", reportData.Master?.PT_NTN);
                            ReportParameter parameter10 = new ReportParameter("CName", reportData.Master?.C_NAME);

                            ReportParameter parameter11 = new ReportParameter("BAddress", reportData.Master?.B_ADDRESS);
                            ReportParameter parameter12 = new ReportParameter("BTell", reportData.Master?.B_TEL);
                            ReportParameter parameter13 = new ReportParameter("BNTN", reportData.Master?.B_NTN);
                            ReportParameter parameter14 = new ReportParameter("STRN", reportData.Master?.STRN);
                            ReportParameter parameter15 = new ReportParameter("FBRNo", reportData.Master?.FBR_NO);

                            ReportParameter parameter16 = new ReportParameter("Sig1", reportData.Master?.SIG1);
                            ReportParameter parameter17 = new ReportParameter("Sig2", reportData.Master?.SIG2);
                            ReportParameter parameter18 = new ReportParameter("Sig3", reportData.Master?.SIG3);
                            ReportParameter parameter19 = new ReportParameter("Sig4", reportData.Master?.SIG4);
                            ReportParameter parameter20 = new ReportParameter("MenuTerms", reportData.Master?.MENU_TERMS);

                            ReportParameter parameter21 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
                            ReportParameter parameter22 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
                            ReportParameter parameter24 = new ReportParameter("FBRLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\FBRLogo.png")).AbsoluteUri);
                            ReportParameter parameter25 = new ReportParameter("BWeb", reportData.Master?.B_WEBSITE);
                            ReportParameter parameter26 = new ReportParameter("BEmail", reportData.Master?.EMAIL);

                            report.SetParameters(new ReportParameter[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9,
                            parameter10, parameter11, parameter12, parameter13, parameter14, parameter15, parameter16, parameter17, parameter18, parameter19, parameter20, parameter21,
                                parameter22, parameter23, parameter24, parameter25, parameter26 });
                        }

                        report.Refresh();
                        report.DataSources.Add(new ReportDataSource() { Name = "SaleTaxInvoice", Value = reportData.Detail });

                        byte[] file;
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\SaleTaxInvoice");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        if (report.IsReadyForRendering)
                        {
                            string input = reportData.Master?.INVOICE_NUMBER;
                            string[] parts = input.Split('/');
                            string prefix = string.Empty;
                            string voucherNumber = string.Empty;
                            if (parts.Length >= 3)
                            {
                                prefix = parts[1];
                                voucherNumber = parts[^1];
                            }
                            file = report.Render("PDF");
                            filePath = $"{prefix} - {voucherNumber}" + ".pdf";

                            stReader.Close();
                            stReader.Dispose();
                            stream.Flush();
                            stream.Close();
                            stream.Dispose();
                            report.Dispose();
                            string reportPath = Path.Combine(uploadsFolder, filePath);
                            System.IO.File.WriteAllBytes(reportPath, file);
                            filePath = $"/Client/SaleTaxInvoice/{filePath}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return filePath;
        }

    }
}
