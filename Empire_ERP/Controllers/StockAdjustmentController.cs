using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Text;
using static Azure.Core.HttpHeader;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class StockAdjustmentController : BaseController
    {
        public IStockAdjustmentService _stockAdjustmentService { get; set; }
        public ICommonService _commonService { get; set; }
        public IBranchService _branchService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public StockAdjustmentController(IStockAdjustmentService stockAdjustmentService, IBranchService branchService, ICommonService commonService, IWebHostEnvironment hostingEnvironment, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {
            _stockAdjustmentService = stockAdjustmentService;
            _hostingEnvironment = hostingEnvironment;
            _commonService = commonService;
            _branchService = branchService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.DateTime = CommonService.GetDateTime("Pakistan Standard Time");
            var ID =  HttpContext.Session.GetString("Branch");
            var CompanyID = HttpContext.Session.GetString("Company");
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            string nextId = "";
            int compCond = 2;

            string maxIdQuery = "SELECT B_I FROM TBL_MENU_BUILDER WHERE DLT = 'T' AND ID = '" + common.MenuID + "'";
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                SqlCommand command = new SqlCommand(maxIdQuery, connection);
                connection.Open();
                object result = command.ExecuteScalar();
                nextId = Convert.ToString(result);
            }

            string compCondQuery = "SELECT CON_QTY FROM TBL_MENU_BUILDER WHERE DLT = 'T' AND ID = '" + common.MenuID + "'";
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                SqlCommand command = new SqlCommand(compCondQuery, connection);
                connection.Open();
                object result = command.ExecuteScalar();
                compCond = Convert.ToInt32(result);
            }

            if (nextId == "I")
            {
                ViewBag.Items = DropdownService.ItemMasterDropdownWithUnits();
            }
            else
            {
                ViewBag.Items = DropdownService.CustomBarcodeDropdownForStockTransfer();
            }

            ViewBag.ItemIds = common.RoleType == "A"
                ? DropdownService.ItemIdsDropdownForPurchaseBill(0, common.Branch, 0)
                : DropdownService.ItemIdsDropdownForPurchaseBill(common.RoleID, common.Branch, common.ShowSelected);

            ViewBag.Type = nextId;
            ViewBag.CompCond = compCond;
            ViewBag.PeriodTo = DropdownService.DescendingPeriodDropdown(Convert.ToInt32(ID));
            ViewBag.PeriodFrom = DropdownService.PeriodDropdown();
            ViewBag.Units = DropdownService.UnitDropdown();
            ViewBag.Colors = DropdownService.ColorDropdown();
            ViewBag.Sizes = DropdownService.SizeDropdown();
            ViewBag.Grades = DropdownService.GradeDropdown();
            ViewBag.Limit = CommonHelper.GetLimitByMenueID(common.MenuID);
            return View();
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
        public JsonResult GetStockAdjustmentDetailByItem(int code, int qty)
        {
            try
            {
                var data = _stockAdjustmentService.GetStockAdjustmentDetailByItem(code, qty, CommonHelper.GetValues(HttpContext));
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

        [HttpGet]
        public JsonResult GetColors()
        {
            try
            {
                var data = DropdownService.ColorDropdown();
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
        public JsonResult GetSizes()
        {
            try
            {
                var data = DropdownService.SizeDropdown();
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
        public JsonResult GetGrades()
        {
            try
            {
                var data = DropdownService.GradeDropdown();
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
        public JsonResult GetStockAdjustment()
        {
            try
            {
                var data = _stockAdjustmentService.QuickSearch(CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetStockAdjustmentByCode(int code)
        {
            try
            {
                var data = _stockAdjustmentService.GetStockAdjustmentByCode(code, CommonHelper.GetValues(HttpContext));
                var detailData = _stockAdjustmentService.GetStockAdjustmentDetailByCode(code, CommonHelper.GetValues(HttpContext));
                return Json(new { Master = data, Detail = detailData });
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
        public JsonResult GetStockAdjustmentDetailByCode(int code)
        {
            try
            {
                var data = _stockAdjustmentService.GetStockAdjustmentDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult Save(CustomStockAdjustment modelRecord)
        {
            try
            {
                var data = _stockAdjustmentService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
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
                var data = _stockAdjustmentService.Delete(code, CommonHelper.GetValues(HttpContext));
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
                var data = _stockAdjustmentService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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
        public JsonResult DeleteStockAdjustmentDetailByCode(int code)
        {
            try
            {
                var data = _stockAdjustmentService.DeleteStockAdjustmentDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public string GetItemId(int Id)
        {
            string Value = "";
            string maxIdQuery = "SELECT ITEM_ID FROM TBL_ITEMSMASTER WHERE ITEM_CODE = '"+ Id +"'";
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                SqlCommand command = new SqlCommand(maxIdQuery, connection);
                connection.Open();
                object result = command.ExecuteScalar();
                Value = Convert.ToString(result);
            }
            return Value;
        }

        [HttpPost]
        public JsonResult GetReport(List<StockAdjustmentStickerPrint> data)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var filePath = GenerateReport(data);
                if (!String.IsNullOrEmpty(filePath))
                {
                    response.data = filePath;
                    response.msg = "";
                    response.msgType = 1;
                }
                else
                {
                    response.msg = "Unable to generate stickers. Please try again later.";
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

        [HttpPost]
        public JsonResult GetPrintReport(RDLCReport model)
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

        private string GenerateReport(List<StockAdjustmentStickerPrint> stockData)
        {
            var filePath = "";
            try
            {
                if (stockData.Count > 0)
                {
                    var responseMessage = _stockAdjustmentService.GetDataForCartonSticker(stockData);
                    if (responseMessage.msgType != 1)
                    {
                        return "";
                    }
                    var fromBranchResponse = _branchService.GetBranchByCode(Convert.ToString(stockData.Select(s => s.FBCODE).FirstOrDefault()));
                    if (fromBranchResponse.msgType != 1)
                    {
                        return "";
                    }
                    var toBranchResponse = _branchService.GetBranchByCode(Convert.ToString(stockData.Select(s => s.TBCODE).FirstOrDefault()));
                    if (toBranchResponse.msgType != 1)
                    {
                        return "";
                    }
                    var fromBranch = (Branch)fromBranchResponse.data;
                    var toBranch = (Branch)toBranchResponse.data;
                    var voucherNo = stockData.Select(s => s.VOUCHER_NO).FirstOrDefault();
                    //var qrCodePathResponse = _commonService.GenerateQRCode($"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(toBranch.B_ADDRESS)}", _hostingEnvironment.WebRootPath);
                    var qrCodePathResponse = _commonService.GenerateQRCode($"{toBranch.B_WEBSITE}", _hostingEnvironment.WebRootPath);
                    if (qrCodePathResponse.msgType != 1)
                    {
                        return "";
                    }
                    var barCodePathResponse = _commonService.GenerateBarCode(voucherNo, _hostingEnvironment.WebRootPath);
                    if (barCodePathResponse.msgType != 1)
                    {
                        return "";
                    }
                    Reports.Datasets.BarcodeReportDataset.StockReportDataTable data = new Reports.Datasets.BarcodeReportDataset.StockReportDataTable();
                    using (LocalReport report = new LocalReport())
                    {
                        var path = Path.Combine(_hostingEnvironment.ContentRootPath, @"Reports\StockAdjustmentReportLabel.rdlc");
                        var stReader = new StreamReader(path);
                        string stringreader = stReader.ReadToEnd();
                        byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
                        MemoryStream stream = new MemoryStream(byteArray);

                        report.EnableExternalImages = true;
                        report.LoadReportDefinition(stream);

                        var reportData = (List<StockAdjustmentStickerPrint>)responseMessage.data;

                        DataRow dataRow = data.NewRow();
                        
                        var customSizes = reportData.Select(d => d.SIZES).GroupBy(d => d)
                                              .Select(g => $"{g.Key}/{g.Count()}");

                        string customSize = string.Join(", ", customSizes);
                        //dataRow["Name"] = "Shrit,item123,xzzxxz,Base Ball,Jeans,Chair,Spenser,test,RICE,zczxxzccxz,asddsasad,asdasd,sdasadsda,test,TENNIS BALL,dassdasda,sdsaddasdas";// String.Join(',', reportData.Select(d => d.ITEM_NAME).ToList().Distinct());
                        //dataRow["Size"] = "38,39,40,41,42,43,44,46,46,47,48,SMALL,YELLOW,RED,FIRST,SECOND,NEW SIZE,NEW SIZE 1";// String.Join(',', reportData.Select(d => d.SIZES).ToList().Distinct());
                        //dataRow["Color"] = "Yellow,Blue,Green,Red,Pink,Orange,Black,Grey,Grey,Light Grey,SANGRIA PURPLE,ELECTRIC VIOLET,FRENCH FUCHSIA";// String.Join(',', reportData.Select(d => d.COLORS).ToList().Distinct()); 
                        dataRow["CompanyName"] = reportData.Select(d => d.BLABEL).FirstOrDefault();
                        dataRow["Category"] = String.Join(", ", reportData.Select(d => d.CATEGORIES).ToList().Distinct());
                        dataRow["Name"] = String.Join(", ", reportData.Select(d => d.ITEM_NAME).ToList().Distinct());
                        dataRow["Size"] = customSize;//String.Join(", ", reportData.Select(d => d.SIZES).ToList().Distinct());
                        dataRow["Color"] = String.Join(", ", reportData.Select(d => d.COLORS).ToList().Distinct()); 
                        dataRow["CompanyName"] = reportData.Select(d => d.BLABEL).FirstOrDefault();
                        //dataRow["Category"] = "Distribution,Rice Traders,Exporter,Importer,Maaz"; // String.Join(',', reportData.Select(d => d.CATEGORIES).ToList().Distinct());
                        dataRow["Quantity"] = reportData.Select(d => d.QTY).FirstOrDefault();
                        dataRow["ToBranchName"] = toBranch.B_NAME;
                        dataRow["ToBranchAddress"] = toBranch.B_ADDRESS;
                        dataRow["ToBranchNumber"] = toBranch.B_TEL;
                        dataRow["FromBranchName"] = fromBranch.B_NAME;
                        dataRow["FromBranchAddress"] = fromBranch.B_ADDRESS;
                        dataRow["FromBranchNumber"] = fromBranch.B_TEL;
                        dataRow["ToBranchQRCodePath"] = qrCodePathResponse.data;
                        dataRow["VoucherNumber"] = voucherNo;
                        dataRow["BarcodePath"] = barCodePathResponse.data;
                        dataRow["SubCategory"] = String.Join(", ", reportData.Select(d => d.SUBCATEGORIES).ToList().Distinct());
                        dataRow["ItemId"] = String.Join(", ", reportData.Select(d => d.ITEM_ID).ToList().Distinct());
                        data.Rows.Add(dataRow);

                        System.Data.DataSet dataSet = new System.Data.DataSet();
                        dataSet.Tables.Add(data);

                        report.DataSources.Clear();
                        report.DataSources.Add(new ReportDataSource() { Name = "StockDataSet", Value = data });

                        byte[] file;
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\StockReport");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        if (report.IsReadyForRendering)
                        {
                            file = report.Render("PDF");
                            filePath = "StockReport_" + Guid.NewGuid().ToString("N").Substring(0, 15) + ".pdf";

                            stReader.Close();
                            stReader.Dispose();
                            stream.Flush();
                            stream.Close();
                            stream.Dispose();
                            report.Dispose();
                            string reportPath = Path.Combine(uploadsFolder, filePath);
                            System.IO.File.WriteAllBytes(reportPath, file);
                            filePath = $"/Client/StockReport/{filePath}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return filePath;
        }

        private string GenerateReport(RDLCReport model)
        {
            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
                {

                    Reports.Datasets.BarcodeReportDataset.StockTransferPrintReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.StockTransferPrintReportDataTable();
                    var responseMessage = _stockAdjustmentService.GetDataForPrintReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return "";
                    }
                    var reportData = (CustomStockAdjustmentForPrintReport)responseMessage.data;
                    Reports.Datasets.BarcodeReportDataset.StockReportDataTable data = new Reports.Datasets.BarcodeReportDataset.StockReportDataTable();
                    using (LocalReport report = new LocalReport())
                    {
                        var path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\{reportData.Master?.REPORT_NAME}.rdlc");
                        using (var stReader = new StreamReader(path))
                        {
                            string stringreader = stReader.ReadToEnd();
                            byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
                            using (var stream = new MemoryStream(byteArray))
                            {
                                report.EnableExternalImages = true;
                                report.LoadReportDefinition(stream);
                                report.DataSources.Clear();
                                var companyLogoPath = Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}");
                                bool? showCompanyLogo = true;
                                if (!System.IO.File.Exists(companyLogoPath))
                                {
                                    showCompanyLogo = false;
                                }
                                //ReportParameter parameter1 = new ReportParameter("TransferFrom", reportData.Master?.BRANCH_FROM_NAME);
                                //ReportParameter parameter2 = new ReportParameter("TransferFromAddress", reportData.Master?.BRANCH_FROM_ADDRESS);
                                ReportParameter parameter3 = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
                                ReportParameter parameter4 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
                                ReportParameter parameter5 = new ReportParameter("TransactionDate", reportData.Master?.V_DATE?.ToString("dd/MM/yyyy"));
                                ReportParameter parameter6 = new ReportParameter("VoucherNo", reportData.Master?.VOUCHER_NO);
                                ReportParameter parameter7 = new ReportParameter("Type", reportData.Master?.STOCK_TYPE);
                                ReportParameter parameter8 = new ReportParameter("Status", reportData.Master?.ASTATUS);
                                //ReportParameter parameter9 = new ReportParameter("TransferTo", reportData.Master?.BRANCH_TO_NAME);
                                //ReportParameter parameter10 = new ReportParameter("TransferToAddress", reportData.Master?.BRANCH_TO_ADDRESS);
                                ReportParameter parameter11 = new ReportParameter("Reference", reportData.Master?.REF);
                                ReportParameter parameter12 = new ReportParameter("Description", reportData.Master?.REMARKS);
                                ReportParameter parameter13 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS);
                                ReportParameter parameter14 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE);
                                ReportParameter parameter15 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
                                ReportParameter parameter16 = new ReportParameter("ShowSignature1", Convert.ToString(reportData.Master?.MENU_SIG1));
                                ReportParameter parameter17 = new ReportParameter("ShowSignature2", Convert.ToString(reportData.Master?.MENU_SIG2));
                                ReportParameter parameter18 = new ReportParameter("ShowSignature3", Convert.ToString(reportData.Master?.MENU_SIG3));
                                ReportParameter parameter19 = new ReportParameter("ShowSignature4", Convert.ToString(reportData.Master?.MENU_SIG4));
                                ReportParameter parameter20 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));

                                report.SetParameters(new ReportParameter[] {parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter11, parameter12, parameter13, parameter14, parameter15, parameter16, parameter17, parameter18, parameter19, parameter20 });
                                report.Refresh();
                                report.DataSources.Add(new ReportDataSource() { Name = "StockPrintReportDataSet", Value = reportData.Detail });
                                byte[] file;
                                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\StockReport");
                                if (!Directory.Exists(uploadsFolder))
                                {
                                    Directory.CreateDirectory(uploadsFolder);
                                }

                                if (report.IsReadyForRendering)
                                {
                                    string input = reportData.Master?.VOUCHER_NO;
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
                                    filePath = $"/Client/StockReport/{filePath}";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return filePath;
        }
        //private string GenerateReport(RDLCReport model)
        //{
        //    var filePath = "";
        //    try
        //    {
        //        if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
        //        {
        //            Reports.Datasets.BarcodeReportDataset.StockTransferPrintReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.StockTransferPrintReportDataTable();
        //            var responseMessage = _stockAdjustmentService.GetDataForPrintReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
        //            if (responseMessage.msgType != 1)
        //            {
        //                return "";
        //            }
        //            var reportData = (CustomStockAdjustmentForPrintReport)responseMessage.data;
        //            Reports.Datasets.BarcodeReportDataset.StockReportDataTable data = new Reports.Datasets.BarcodeReportDataset.StockReportDataTable();
        //            using (LocalReport report = new LocalReport())
        //            {
        //                var path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\{reportData.Master?.REPORT_NAME}.rdlc");
        //                var stReader = new StreamReader(path);
        //                string stringreader = stReader.ReadToEnd();
        //                byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
        //                MemoryStream stream = new MemoryStream(byteArray);
        //                report.EnableExternalImages = true;
        //                report.LoadReportDefinition(stream);
        //                report.DataSources.Clear();
        //                var companyLogoPath = Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}");
        //                bool? showCompanyLogo = true;
        //                if (!System.IO.File.Exists(companyLogoPath))
        //                {
        //                    showCompanyLogo = false;
        //                }
        //                ReportParameter parameter1 = new ReportParameter("TransferFrom", reportData.Master?.BRANCH_FROM_NAME);
        //                ReportParameter parameter2 = new ReportParameter("TransferFromAddress", reportData.Master?.BRANCH_FROM_ADDRESS);
        //                ReportParameter parameter3 = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
        //                ReportParameter parameter4 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
        //                ReportParameter parameter5 = new ReportParameter("TransactionDate", reportData.Master?.V_DATE?.ToString("dd/MM/yyyy"));
        //                ReportParameter parameter6 = new ReportParameter("VoucherNo", reportData.Master?.VOUCHER_NO);
        //                ReportParameter parameter7 = new ReportParameter("Type", reportData.Master?.STOCK_TYPE);
        //                ReportParameter parameter8 = new ReportParameter("Status", reportData.Master?.ASTATUS);
        //                ReportParameter parameter9 = new ReportParameter("TransferTo", reportData.Master?.BRANCH_TO_NAME);
        //                ReportParameter parameter10 = new ReportParameter("TransferToAddress", reportData.Master?.BRANCH_TO_ADDRESS);
        //                ReportParameter parameter11 = new ReportParameter("Reference", reportData.Master?.REF);
        //                ReportParameter parameter12 = new ReportParameter("Description", reportData.Master?.REMARKS);
        //                ReportParameter parameter13 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS);
        //                ReportParameter parameter14 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE);
        //                ReportParameter parameter15 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
        //                ReportParameter parameter16 = new ReportParameter("ShowSignature1", Convert.ToString(reportData.Master?.MENU_SIG1));
        //                ReportParameter parameter17 = new ReportParameter("ShowSignature2", Convert.ToString(reportData.Master?.MENU_SIG2));
        //                ReportParameter parameter18 = new ReportParameter("ShowSignature3", Convert.ToString(reportData.Master?.MENU_SIG3));
        //                ReportParameter parameter19 = new ReportParameter("ShowSignature4", Convert.ToString(reportData.Master?.MENU_SIG4));
        //                ReportParameter parameter20 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));

        //                report.SetParameters(new ReportParameter[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9, parameter10, parameter11, parameter12, parameter13, parameter14, parameter15, parameter16, parameter17, parameter18, parameter19, parameter20 });
        //                report.Refresh();
        //                report.DataSources.Add(new ReportDataSource() { Name = "StockPrintReportDataSet", Value = reportData.Detail });

        //                byte[] file;
        //                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\StockReport");
        //                if (!Directory.Exists(uploadsFolder))
        //                {
        //                    Directory.CreateDirectory(uploadsFolder);
        //                }

        //                if (report.IsReadyForRendering)
        //                {
        //                    file = report.Render("PDF");
        //                    filePath = "StockAdjustmentPrintReport_" + Guid.NewGuid().ToString("N").Substring(0, 15) + ".pdf";

        //                    stReader.Close();
        //                    stReader.Dispose();
        //                    stream.Flush();
        //                    stream.Close();
        //                    stream.Dispose();
        //                    report.Dispose();
        //                    string reportPath = Path.Combine(uploadsFolder, filePath);
        //                    System.IO.File.WriteAllBytes(reportPath, file);
        //                    filePath = $"/Client/StockReport/{filePath}";
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return filePath;
        //}

        [HttpPost]
        public JsonResult UpdatePrintStatus(string codes)
        {
            try
            {
                var data = _stockAdjustmentService.UpdatePrintStatus(codes, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetSodaBookFeedingDetailBySodaDate(DateTime? sodaDate, int branchTo)
        {
            try
            {
                var data = _stockAdjustmentService.GetSodaBookFeedingDetailBySodaDate(sodaDate, branchTo, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetBarcodeList()
        {
            try
            {
                var data = _stockAdjustmentService.GetBarcodeList();
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
    }
}
