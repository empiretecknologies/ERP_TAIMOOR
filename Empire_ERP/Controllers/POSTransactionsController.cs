using DinkToPdf;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.NETCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Ocsp;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using TheArtOfDev.HtmlRenderer.WinForms;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using static Azure.Core.HttpHeader;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class POSTransactionsController : BaseController
    {
        public IPOSTransactionService _posTransactionService { get; set; }
        public IBranchService _branchService { get; set; }
        public ICompanyService _companyService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ICompositeViewEngine _viewEngine;
        private readonly IHubContext<OrderHub> _hubContext;
        private readonly IWebHostEnvironment _env;
        public POSTransactionsController(IMenuService menuService, IBranchService branchService, ICompanyService companyService, IPOSTransactionService posTransactionService, IWebHostEnvironment hostingEnvironment, ICompositeViewEngine viewEngine, IHubContext<OrderHub> hubContext, IWebHostEnvironment env, IBaseService baseService) : base(menuService, baseService)
        {
            _posTransactionService = posTransactionService;
            _branchService = branchService;
            _companyService = companyService;
            _hostingEnvironment = hostingEnvironment;
            _viewEngine = viewEngine;
            _hubContext = hubContext;
            _env = env;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            string nextId = "";
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);

            var Menu = _menuService.GetMenu(common.MenuID);
            int? pType = 0;
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                pType = menu.PTYPE;
            }

            string maxIdQuery = "SELECT B_I FROM TBL_MENU_BUILDER WHERE DLT = 'T' AND ID = '" + common.MenuID + "'";
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                SqlCommand command = new SqlCommand(maxIdQuery, connection);
                connection.Open();
                object result = command.ExecuteScalar();
                nextId = Convert.ToString(result);
            }
            //if (nextId == "I")
            //{
            //    ViewBag.Barcodes = DropdownService.ItemMasterDropdown(common.RoleID, common.RoleType);
            //    ViewBag.BarcodeTextBoxVisible = false;
            //}
            //else
            {
                ViewBag.BarcodeTextBoxVisible = true;
                ViewBag.Barcodes = DropdownService.BarcodesWithRateAndUnits();
            }
            ViewBag.Type = nextId;
            ViewBag.Users = DropdownService.UsersForPOS();
            //ViewBag.Units = DropdownService.UnitDropdownWithQuantity();
            Branch branch = (Branch)_branchService.GetBranchByCode(common.Branch).data;
            var GroupData = _posTransactionService.GetItemsGroup(CommonHelper.GetValues(HttpContext));
            ViewBag.ItemsGroup = GroupData.data;
            ViewBag.MapData = _posTransactionService.GetMapData(CommonHelper.GetValues(HttpContext));
            ViewBag.Units = DropdownService.UnitDropdownWithQuantity();
            //ViewBag.UserRights = GetUserRights();
            ViewBag.MapTable = DropdownService.MapTable(common.Branch);
            ViewBag.DeleteRow = DropdownService.DeleteRow();
            ViewBag.Commision = DropdownService.GetSalesmanCommision();
            ViewBag.BankAccount = DropdownService.BanksNameDropdown(pType, CommonHelper.GetValues(HttpContext));
            ViewBag.TablesData = DropdownService.GetAllTablesData();
            ViewBag.Branch_RT_TYPE = branch.RT_TYPE;
            return View();
        }
        [HttpGet]
        public JsonResult GetSyncData(int code)
        {
            try
            {
                var data = _posTransactionService.GetSyncData(CommonHelper.GetValues(HttpContext));
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
        [HttpGet]
        public JsonResult GetUserRights()
        {
            try
            {
                var data = DropdownService.POSUserRights(CommonHelper.GetValues(HttpContext).Username);
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
        [HttpPost]
        public async Task<IActionResult> SendOrder([FromBody] object order)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveOrder", order);
            return Ok();
        }
        [HttpGet]
        public JsonResult GetItemsMasterByGroup(int groupId)
        {
            var data = _posTransactionService.GetItemsMasterByGroup(groupId, CommonHelper.GetValues(HttpContext)).data;
            return Json(data);
        }
        [HttpGet]
        public JsonResult SRBApi_Status(string voucherno, string invoiceId)
        {
            var data = _posTransactionService.SRBApi_Status(voucherno, invoiceId, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }
        [HttpGet]
        public JsonResult GetItemsMasterByCode(int itemId, string barcode, int Qty, int TranId)
        {
            var data = _posTransactionService.GetItemsMasterByCode(itemId, barcode, Qty, TranId, CommonHelper.GetValues(HttpContext)).data;
            return Json(data);
        }
        [HttpGet]
        public JsonResult GetReturn()
        {
            try
            {
                var data = DropdownService.ReturnData();
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
        public JsonResult GetDeleterow()
        {
            try
            {
                var data = DropdownService.DeleteRow();
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
        public JsonResult GetAdvance()
        {
            try
            {
                var data = DropdownService.AdvanceRow();
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
        public JsonResult GetPOSRecords()
        {
            try
            {
                var data = DropdownService.UsersForPOS();
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
        public JsonResult GetPOSCustomerName()
        {
            try
            {
                var data = DropdownService.GetPOSCustomerName();
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
        public JsonResult GetCardDisc(string CardNum)
        {
            try
            {
                var data = DropdownService.CardDiscRecords(CardNum);
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
        public JsonResult GetBookTypes()
        {
            try
            {
                //var data = DropdownService.GetBookTypesForPOS();
                var common = CommonHelper.GetValues(HttpContext);
                var data = common.RoleType == "A"
                    ? DropdownService.GetBookTypesForPOS(0, common.Branch, 0)
                    : DropdownService.GetBookTypesForPOS(common.RoleID, common.Branch, common.ShowSelected);
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
        public JsonResult GetAllDiscount()
        {
            var data = _posTransactionService.GetAllDiscount(CommonHelper.GetValues(HttpContext)).data;
            return Json(data);

        }
        [HttpGet]
        public JsonResult GetAllWaiter()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var data = _posTransactionService.GetAllWaiter(common).data;
            return Json(data);

        }
        [HttpGet]
        public JsonResult GetAllTables(string Tran_Id)
        {
            var common = CommonHelper.GetValues(HttpContext);
            var data = _posTransactionService.GetAllTables(common, Tran_Id).data;
            return Json(data);

        }
        [HttpGet]
        public JsonResult ExpenseRecord()
        {
            var data = _posTransactionService.ExpenseRecord(CommonHelper.GetValues(HttpContext)).data;
            return Json(data);

        }
        [HttpGet]
        public JsonResult GetDueDate(int? partycode, int? actcode)
        {
            var data = DropdownService.GetDueDate(partycode, actcode);
            return Json(data);
        }
        [HttpPost]
        public JsonResult ExpenseRecordSave(ExpensePOSTransaction modelRecord)
        {
            var data = _posTransactionService.ExpenseRecordSave(modelRecord, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }
        [HttpGet]
        public JsonResult GetDynamicIcon()
        {
            var data = _posTransactionService.GetDynamicIcon().data;
            return Json(data);

        }
        public JsonResult GetBanksName()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var Menu = _menuService.GetMenu(common.MenuID);
            int? pType = 0;
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                pType = menu.PTYPE;
            }
            var data = DropdownService.BanksNameDropdown(pType, common);
            return Json(data);

        }
        public JsonResult GetSalesmanName()
        {
            var data = DropdownService.SalesmanNameDropdown(CommonHelper.GetValues(HttpContext));
            return Json(data);

        }
        public JsonResult GetPartyName()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var data = common.RoleType == "A"
                ? DropdownService.GetPartyName(0, common.RoleType)
                : DropdownService.GetPartyName(common.RoleID, common.RoleType);

            //var data = DropdownService.GetPartyName(common.RoleID, common.Branch, common.ShowSelected);
            return Json(data);

        }
        public JsonResult GetAcountName()
        {
            var data = DropdownService.AcountNameDropdown();
            return Json(data);

        }
        [HttpGet]
        public IActionResult GenerateQRCode(string invoiceNumber)
        {
            if (invoiceNumber != null)
            {
                invoiceNumber = invoiceNumber.Trim();
                var qrCodeImage = Generate(invoiceNumber);

                return File(qrCodeImage, "image/png");
            }
            else
            {
                return null;
            }

        }
        [HttpGet]
        public JsonResult GetPOSTransactions()
        {
            try
            {
                var data = _posTransactionService.QuickSearch(CommonHelper.GetValues(HttpContext));
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
        [HttpGet]
        public JsonResult GetPayQuickSearch()
        {
            try
            {
                var data = _posTransactionService.GetPayQuickSearch(CommonHelper.GetValues(HttpContext));
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
        [HttpGet]
        public JsonResult AdvanceBookingRecords()
        {
            try
            {
                var data = _posTransactionService.AdvanceBookingRecords(CommonHelper.GetValues(HttpContext));
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
        [HttpGet]
        public JsonResult GetCustomerHistory(string CstNumber)
        {
            try
            {
                var data = _posTransactionService.GetCustomerHistory(CommonHelper.GetValues(HttpContext), CstNumber);
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
        [HttpGet]
        public JsonResult GetMapData()
        {
            try
            {
                var data = _posTransactionService.GetMapData(CommonHelper.GetValues(HttpContext));
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
        public JsonResult UpdateTablesAndWaiter(CustomPOSTransaction modelRecord)
        {
            try
            {
                var data = _posTransactionService.UpdateTablesAndWaiter(modelRecord, CommonHelper.GetValues(HttpContext));

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
        public JsonResult Save(CustomPOSTransaction modelRecord)
        {
            try
            {
                var data = _posTransactionService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
                if (modelRecord.Master.BILL_STATUS == "K" || modelRecord.Master.BILL_STATUS == "P")
                {
                    try
                    {
                        string slipHtml = string.Empty;
                        Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable();
                        var model = new POSPrint_Model();
                        model.tranId = data.SaveData.code;
                        model.BillStatus = modelRecord.Master.BILL_STATUS;
                        model.DT_Code = data.SaveData.dt_codes;
                        var responseMessage = _posTransactionService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                        if (responseMessage.msgType != 1)
                        {
                            return Json("");
                        }
                        var reportData = (CustomMenuDetail)responseMessage.data;
                        var slip = (SlipViewModel)responseMessage.viewModel;
                        string projectOutputPath = AppDomain.CurrentDomain.BaseDirectory;
                        string slipFolder = Path.Combine(projectOutputPath, "Slips");
                        string pdfFullPath = string.Empty;
                        string StickerPath = string.Empty;
                        if (!Directory.Exists(slipFolder))
                        {
                            Directory.CreateDirectory(slipFolder);
                        }
                        if(modelRecord.Master.BILL_STATUS == "P" && modelRecord.Master.PWINDOW == 0)
                        {
                            
                            var viewModel = responseMessage.viewModel as SlipViewModel;
                            string pdfFileName = viewModel != null && !string.IsNullOrEmpty(viewModel.ReferenceNumber)
                                ? $"PAY_{model.tranId}_{viewModel.ReferenceNumber}.pdf"
                                : $"PAY_{model.tranId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";


                            pdfFullPath = Path.Combine(slipFolder, pdfFileName);

                            PdfService.GeneratePayWiseSlips(slip, pdfFullPath, _env.WebRootPath, modelRecord.Master.PRINT_COPIES);
                            var pdfFolder = Path.GetDirectoryName(pdfFullPath);
                            //if (Directory.Exists(pdfFolder))
                                //Directory.Delete(pdfFolder, true);
                        }
                        else
                        {
                            slipHtml = RenderPartialViewToString(reportData.REPORT_NAME == "STICKER" ? "POS_KOT" : reportData.REPORT_NAME, responseMessage.viewModel);
                            
                        }
                        data.PWindow = slip.PWindow;
                        data.SlipHtml = slipHtml;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
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
        public JsonResult PrintModal(POSPrint_Model modelRecord)
        {
            try
            {
                Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable();
                var responseMessage = _posTransactionService.GetDataForReport(modelRecord, reportDetails, CommonHelper.GetValues(HttpContext));
                if (responseMessage.msgType != 1)
                {
                    return Json("");
                }
                var reportData = (CustomMenuDetail)responseMessage.data;

                string slipHtml = RenderPartialViewToString(reportData.REPORT_NAME, responseMessage.viewModel);

                responseMessage.SlipHtml = slipHtml;

                return Json(responseMessage);
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
        private string RenderPartialViewToString(string viewName, object model)
        {
            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"View '{viewName}' not found.");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                viewResult.View.RenderAsync(viewContext).Wait();
                return sw.GetStringBuilder().ToString();
            }
        }
        [HttpGet]
        public JsonResult GetPOSTransactionByCode(int code, string voucher)
        {
            try
            {
                var data = _posTransactionService.GetPOSTransactionByCode(code, voucher, CommonHelper.GetValues(HttpContext));
                if (voucher != "SRBForm")
                {
                    var detailData = _posTransactionService.GetPOSTransactionDetailByCode(data.tranId, CommonHelper.GetValues(HttpContext));
                    return Json(new { Master = data, Detail = detailData });
                }
                return Json(new { Master = data });
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
        public IActionResult PrintExpenses([FromBody] List<ExpensePrint> expenses)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var slipHtml = string.Empty;
                if (expenses == null || expenses.Count == 0)
                {
                    return BadRequest("No expense data received.");
                }

                //Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.InvoiceReportDataTable();
                var responseMessage = _posTransactionService.ExpensePrintReport(expenses, CommonHelper.GetValues(HttpContext));
                if (responseMessage.msgType != 1)
                {
                    return BadRequest("No expense data received.");
                }

                ViewData.Model = responseMessage.viewModel;

                using (var sw = new StringWriter())
                {
                    // Use GetView to locate the view using a custom path
                    var viewResult = _viewEngine.FindView(ControllerContext, "ExpenseReceipt", false);

                    if (viewResult.View == null)
                    {
                        throw new ArgumentNullException($"View 'ExpenseReceipt' not found.");
                    }

                    var viewContext = new ViewContext(
                        ControllerContext,
                        viewResult.View,
                        ViewData,
                        TempData,
                        sw,
                        new HtmlHelperOptions()
                    );

                    viewResult.View.RenderAsync(viewContext).Wait();
                    slipHtml = sw.GetStringBuilder().ToString();


                    string thermalText = ConvertHtmlToThermalText(slipHtml);

                    // Direct print the formatted text
                    PrintTextDirect(thermalText);
                }

                if (!string.IsNullOrEmpty(slipHtml))
                {
                    response.SlipHtml = slipHtml;
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
        private string ConvertHtmlToThermalText(string html)
        {
            // Remove all HTML tags except line breaks
            string plainText = Regex.Replace(html, @"<br\s*/?>", Environment.NewLine);
            plainText = Regex.Replace(plainText, @"<[^>]+>", string.Empty);
            plainText = WebUtility.HtmlDecode(plainText);

            // Process tables if present
            if (html.Contains("<table"))
            {
                return FormatHtmlTablesForThermal(html);
            }

            return plainText;
        }
        private string FormatHtmlTablesForThermal(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            StringBuilder thermalText = new StringBuilder();

            foreach (HtmlNode table in doc.DocumentNode.SelectNodes("//table"))
            {
                foreach (HtmlNode row in table.SelectNodes(".//tr"))
                {
                    StringBuilder line = new StringBuilder();
                    foreach (HtmlNode cell in row.SelectNodes(".//td|.//th"))
                    {
                        string cellText = WebUtility.HtmlDecode(cell.InnerText.Trim());
                        line.Append(cellText.PadRight(20)); // Fixed width columns
                    }
                    thermalText.AppendLine(line.ToString());
                }
                thermalText.AppendLine(new string('-', 40)); // Table separator
            }

            return thermalText.ToString();
        }
        private void PrintTextDirect(string textToPrint)
        {
            PrintDocument p = new PrintDocument();
            p.PrinterSettings.PrinterName = "80mm Series Printer";

            // Set 80mm paper size
            p.DefaultPageSettings.PaperSize = new PaperSize("Custom", 280, 0);
            p.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

            p.PrintPage += (sender, e) =>
            {
                Font font = new Font("Courier New", 9);
                e.Graphics.DrawString(
                    textToPrint,
                    font,
                    Brushes.Black,
                    new RectangleF(0, 0, p.DefaultPageSettings.PaperSize.Width, p.DefaultPageSettings.PaperSize.Height),
                    new StringFormat
                    {
                        LineAlignment = StringAlignment.Near,
                        Alignment = StringAlignment.Near,
                        Trimming = StringTrimming.None,
                        FormatFlags = StringFormatFlags.NoWrap
                    }
                );
            };

            //p.Print();
        }
        [HttpGet]
        public JsonResult GetExpenseByCode(int code)
        {
            try
            {
                var data = _posTransactionService.GetExpenseByCode(code, CommonHelper.GetValues(HttpContext));
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
        [HttpGet]
        public JsonResult GetBarcodeList(int ItemId)
        {
            try
            {
                var data = _posTransactionService.GetBarcodeList(ItemId, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetAllBarcodeList()
        {
            try
            {
                var data = _posTransactionService.GetAllBarcodeList(CommonHelper.GetValues(HttpContext));
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
        [HttpGet]
        public JsonResult GetPOSTransactionDetailByCode(int code)
        {
            try
            {
                var data = _posTransactionService.GetPOSTransactionDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult Delete(int code)
        {
            try
            {
                var data = _posTransactionService.Delete(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult DeleteExpenseRow(int code)
        {
            try
            {
                var data = _posTransactionService.DeleteExpenseRow(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult DeletePOSTransactionDetailByCode(CustomPOSTransaction modelRecord, int code, int ItemId, string dtcode)
        {
            try
            {
                var data = _posTransactionService.DeletePOSTransactionDetailByCode(modelRecord, code, ItemId, dtcode, CommonHelper.GetValues(HttpContext));
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
        [HttpGet]
        public JsonResult GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate)
        {
            try
            {
                var data = _posTransactionService.GetSodaBookFeedingDetailBySodaDate(sodaDate, CommonHelper.GetValues(HttpContext));
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
        [HttpGet]
        public JsonResult GetExpenseType()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var data = DropdownService.ExpenseType(common);
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
        public IActionResult PostToSRB([FromBody] SRBPostModel model)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var json = System.Text.Json.JsonSerializer.Serialize(model);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = client.PostAsync(model.SrbUrl, content).GetAwaiter().GetResult();

                    string responseText = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                    SrbResponse apiResponse = JsonConvert.DeserializeObject<SrbResponse>(responseText);

                    if (apiResponse != null && apiResponse.resCode == "00")
                    {
                        _posTransactionService.SRBApi_Status(
                            model.BillVoucher,
                            apiResponse.srbInvoceId,
                            CommonHelper.GetValues(HttpContext)
                        );
                    }
                    else
                    {
                        _posTransactionService.SRBApi_Status(
                            model.BillVoucher,
                            apiResponse.err,
                            CommonHelper.GetValues(HttpContext)
                        );
                    }

                    return Content(responseText, "application/json");
                }


            }
            catch (Exception ex)
            {
                return Content("{\"resCode\":\"99\",\"err\":\"" + ex.Message + "\"}", "application/json");
            }
        }
        public byte[] Generate(string text)
        {
            // Define QR code encoding options
            var options = new EncodingOptions
            {
                Height = 300,
                Width = 300,
                Margin = 1
            };

            // Create a barcode writer
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };

            // Generate QR code pixel data
            var pixelData = barcodeWriter.Write(text);

            // Convert pixel data to a PNG image
            using (var ms = new MemoryStream())
            {
                using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height))
                {
                    for (int y = 0; y < pixelData.Height; y++)
                    {
                        for (int x = 0; x < pixelData.Width; x++)
                        {
                            var color = pixelData.Pixels[(y * pixelData.Width + x) * 4];
                            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(color, color, color));
                        }
                    }

                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }

                return ms.ToArray();
            }
        }
        public IActionResult GenerateWiFiQRCode(string ssid, string password, string encryption = "WPA")
        {
            string encodedSsid = Uri.EscapeDataString(ssid);
            string encodedPassword = Uri.EscapeDataString(password);
            // WiFi connection string (WPA/WEP/None)
            string wifiConfig = $"WIFI:S:{encodedSsid};T:{encryption};P:{encodedPassword};H:false;";

            // QR Code settings
            var options = new QrCodeEncodingOptions
            {
                Height = 300,
                Width = 300,
                Margin = 1,
                CharacterSet = "UTF-8"
            };

            // Generate QR Code
            var barcodeWriter = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = options
            };

            var pixelData = barcodeWriter.Write(wifiConfig); // Pass WiFi config here

            // Convert pixel data to PNG image
            using (var ms = new MemoryStream())
            {
                using (var bitmap = new System.Drawing.Bitmap(pixelData.Width, pixelData.Height))
                {
                    for (int y = 0; y < pixelData.Height; y++)
                    {
                        for (int x = 0; x < pixelData.Width; x++)
                        {
                            var color = pixelData.Pixels[(y * pixelData.Width + x) * 4];
                            bitmap.SetPixel(x, y, System.Drawing.Color.FromArgb(color, color, color));
                        }
                    }

                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                }

                return File(ms.ToArray(), "image/png");
            }
        }
        [HttpPost]
        public async Task<JsonResult> SendWhatsapp([FromBody] List<POSBookingItemDto> mobileData)
        {

            var common = CommonHelper.GetValues(HttpContext);
            var msgtype = true;
            var msgRes = "";
            var stactTrace = "";
            string apiUrl = "";
            var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
            var currentCompany = (Company)currentCompanyResponse.data;
            var currentBranchResponse = _branchService.GetBranchByCode(common.Branch);
            var currentBranch = (Branch)currentBranchResponse.data;

            using (HttpClient client = new HttpClient())
            {
                foreach (var msg in mobileData)
                {
                    if (msg.WhatsappUrl != "")
                    {
                        try
                        {
                            var data = _posTransactionService.GetPOSTransactionByCode(msg.traN_ID, "undefined", CommonHelper.GetValues(HttpContext));
                            var json = JsonConvert.SerializeObject(data.data);
                            var dataList = JsonConvert.DeserializeObject<List<POSBookingSummaryModel>>(json);
                            if (dataList.Count > 0)
                            {
                                var row = dataList[0];
                                decimal netTotal = row.NET_TOTAL;
                                //decimal taxRate = row.CASHTAX == 0 ? row.BANKTAX : row.CASHTAX;
                                decimal taxRate = row.CASHTAX != 0 ? row.CASHTAX : row.BANKTAX != 0 ? row.BANKTAX : row.PARTYTAX;
                                decimal advance = row.ADVANCE;
                                decimal advanceBank = row.ADVBANK;
                                string cname = row.CNAME;
                                int totalAmount = Convert.ToInt32(netTotal * (1 + taxRate / 100));
                                int Amount = totalAmount;
                                int balanceAmount = Convert.ToInt32(totalAmount - advance - advanceBank);
                                // Prepare message
                                string formattedNumber = string.Empty;
                                if (msg.mobile != "" && msg.mobile != null)
                                    formattedNumber = msg.mobile.StartsWith("0") ? "92" + msg.mobile.Substring(1) : msg.mobile;
                                formattedNumber = formattedNumber.Replace("-", "");
                                if (Amount != 0)
                                {
                                    string personalizedMessage = msg.message
                                       .Replace("[Customer Name]", cname)
                                       .Replace("[InvoiceNo]", row.VOUCHER_NO)
                                       .Replace("[InvoiceDate]", Convert.ToDateTime(row.MSG_DATE).ToString("dd/MM/yyyy"))
                                       .Replace("[DeliveryDate]", Convert.ToDateTime(row.MSGDELDATE).ToString("dd/MM/yyyy"))
                                       .Replace("[TotalAmount]", Amount.ToString() + "/=")
                                       .Replace("[ReturnAmount]", Math.Abs(row.RETURN_AMT).ToString() + "/=")
                                       .Replace("[AdvanceAmount]", (advance + advanceBank).ToString() + "/=")
                                       .Replace("[DueAmount]", (Amount - advance - advanceBank).ToString() + "/=")
                                       .Replace("[PartyPaidAmount]", row.PARTY + "/=")
                                       .Replace("[PartyDueAmount]", (Amount - row.PARTY).ToString() + "/=")
                                       .Replace("[BalanceAmount]", balanceAmount.ToString() + "/=")
                                       .Replace("[CompanyName]", currentCompany.C_NAME)
                                       .Replace("[Number]", currentBranch.B_TEL)
                                       .Replace("[Email]", currentBranch.EMAIL)
                                       .Replace("[PaymentMode]", row.PAY_TYPE)
                                       .Replace("[BranchName]", currentBranch.B_NAME);

                                    personalizedMessage = personalizedMessage.Replace("\r\n", "\n").Replace("\r", "\n");

                                    // Encode and send
                                    string encodedMessage = Uri.EscapeDataString(personalizedMessage);
                                    apiUrl = $"{msg.WhatsappUrl}?to={formattedNumber}&api={msg.WhatsappToken}&message={encodedMessage}";

                                    try
                                    {
                                        var response = await client.GetAsync(apiUrl);
                                        string result = await response.Content.ReadAsStringAsync();
                                    }
                                    catch (Exception ex)
                                    {
                                        msgRes = $"{ex.Message} Stactrace : {ex.StackTrace}";
                                        msgtype = false;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            msgRes = $"{ex.Message} Stactrace : {ex.StackTrace}";
                            msgtype = false;
                        }
                    }
                    else
                    {
                        msgtype = false;
                    }
                }
            }
            return Json(new { success = msgtype, url = apiUrl, msg = msgRes });
        }
    }

}
