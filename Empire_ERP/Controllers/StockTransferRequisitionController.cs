using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Empire_ERP.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Text;
using static Azure.Core.HttpHeader;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class StockTransferRequisitionController : BaseController
    {
        public IStockTransferRequisitionService _stockTransferService { get; set; }
        public ICommonService _commonService { get; set; }
        public IBranchService _branchService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public StockTransferRequisitionController(IStockTransferRequisitionService stockTransferService, IBranchService branchService, ICommonService commonService, IWebHostEnvironment hostingEnvironment, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {
            _stockTransferService = stockTransferService;
            _hostingEnvironment = hostingEnvironment;
            _commonService = commonService;
            _branchService = branchService;
        }

        public IActionResult Index()
        {
            ViewBag.DateTime = CommonService.GetDateTime("Pakistan Standard Time");
            var ID =  HttpContext.Session.GetString("Branch");
            ViewBag.Items = DropdownService.ItemMasterDropdownWithUnits();
            ViewBag.Groups = DropdownService.SubsidiaritiesItemGroups();
            ViewBag.PeriodTo = DropdownService.DescendingPeriodDropdown(Convert.ToInt32(ID));
            ViewBag.BranchTo = DropdownService.WithOutCurrentBrachDropdown(Convert.ToInt32(ID));
            ViewBag.BranchFrom = DropdownService.CurrentBrachDropdown(Convert.ToInt32(ID));
            ViewBag.PeriodFrom = DropdownService.PeriodDropdown();
            ViewBag.Units = DropdownService.UnitDropdown();
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
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
        public JsonResult GetStockTransferRequisition()
        {
            try
            {
                var data = _stockTransferService.QuickSearch(CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetStockTransferRequisitionByCode(int code)
        {
            try
            {
                var data = _stockTransferService.GetStockTransferRequisitionByCode(code, CommonHelper.GetValues(HttpContext));
                var detailData = _stockTransferService.GetStockTransferRequisitionDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetStockTransferRequisitionDetailByCode(int code)
        {
            try
            {
                var data = _stockTransferService.GetStockTransferRequisitionDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult Save(CustomStockTransferRequisition modelRecord)
        {
            try
            {
                var data = _stockTransferService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
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
                var data = _stockTransferService.Delete(code, CommonHelper.GetValues(HttpContext));
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
                var data = _stockTransferService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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
        public JsonResult DeleteStockTransferRequisitionDetailByCode(int code)
        {
            try
            {
                var data = _stockTransferService.DeleteStockTransferRequisitionDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetReport(List<StockTransferRequisitionStickerPrint> data)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                byte[] file = GenerateReport(data);
                if (file != null && file.Length > 0)
                {
                    string base64File = Convert.ToBase64String(file);
                    response.data = base64File;
                    response.msg = "Report generated successfully.";
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

        private byte[] GenerateReport(List<StockTransferRequisitionStickerPrint> stockData)
        {
            var filePath = "";
            try
            {
                if (stockData.Count > 0)
                {
                    var responseMessage = _stockTransferService.GetDataForCartonSticker(stockData);
                    if (responseMessage.msgType != 1)
                    {
                        return null;
                    }
                    var fromBranchResponse = _branchService.GetBranchByCode(Convert.ToString(stockData.Select(s => s.FBCODE).FirstOrDefault()));
                    if (fromBranchResponse.msgType != 1)
                    {
                        return null;
                    }
                    var toBranchResponse = _branchService.GetBranchByCode(Convert.ToString(stockData.Select(s => s.TBCODE).FirstOrDefault()));
                    if (toBranchResponse.msgType != 1)
                    {
                        return null;
                    }
                    var fromBranch = (Branch)fromBranchResponse.data;
                    var toBranch = (Branch)toBranchResponse.data;
                    var voucherNo = stockData.Select(s => s.VOUCHER_NO).FirstOrDefault();
                    //var qrCodePathResponse = _commonService.GenerateQRCode($"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(toBranch.B_ADDRESS)}", _hostingEnvironment.WebRootPath);
                    var qrCodePathResponse = _commonService.GenerateQRCode($"{toBranch.B_WEBSITE}", _hostingEnvironment.WebRootPath);
                    if (qrCodePathResponse.msgType != 1)
                    {
                        return null;
                    }
                    var barCodePathResponse = _commonService.GenerateBarCode(voucherNo, _hostingEnvironment.WebRootPath);
                    if (barCodePathResponse.msgType != 1)
                    {
                        return null;
                    }
                    Reports.Datasets.BarcodeReportDataset.StockReportDataTable data = new Reports.Datasets.BarcodeReportDataset.StockReportDataTable();
                    using (LocalReport report = new LocalReport())
                    {
                        var path = Path.Combine(_hostingEnvironment.ContentRootPath, @"Reports\StockTransferReportLabel.rdlc");
                        using (var stReader = new StreamReader(path))
                        {
                            string stringreader = stReader.ReadToEnd();
                            byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
                            using (var stream = new MemoryStream(byteArray))
                            {
                                report.EnableExternalImages = true;
                                report.LoadReportDefinition(stream);

                                var reportData = (List<StockTransferRequisitionStickerPrint>)responseMessage.data;

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

                                if (report.IsReadyForRendering)
                                {
                                    return report.Render("PDF");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        private string GenerateReport(RDLCReport model)
        {
            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
                {
                    Reports.Datasets.BarcodeReportDataset.StockTransferPrintReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.StockTransferPrintReportDataTable();
                    var responseMessage = _stockTransferService.GetDataForPrintReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return "";
                    }
                    var reportData = (CustomStockTransferRequisitionForPrintReport)responseMessage.data;
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
                                ReportParameter parameter1 = new ReportParameter("TransferFrom", reportData.Master?.BRANCH_FROM_NAME);
                                ReportParameter parameter2 = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
                                ReportParameter parameter3 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
                                ReportParameter parameter4 = new ReportParameter("TransactionDate", reportData.Master?.V_DATE?.ToString("dd/MM/yyyy"));
                                ReportParameter parameter5 = new ReportParameter("VoucherNo", reportData.Master?.VOUCHER_NO);
                                ReportParameter parameter6 = new ReportParameter("Status", reportData.Master?.ASTATUS);
                                ReportParameter parameter7 = new ReportParameter("TransferTo", reportData.Master?.BRANCH_TO_NAME);
                                ReportParameter parameter8 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS);
                                ReportParameter parameter9 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE);
                                ReportParameter parameter10 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
                                ReportParameter parameter11 = new ReportParameter("ShowSignature1", Convert.ToString(reportData.Master?.MENU_SIG1));
                                ReportParameter parameter12 = new ReportParameter("ShowSignature2", Convert.ToString(reportData.Master?.MENU_SIG2));
                                ReportParameter parameter13 = new ReportParameter("ShowSignature3", Convert.ToString(reportData.Master?.MENU_SIG3));
                                ReportParameter parameter14 = new ReportParameter("ShowSignature4", Convert.ToString(reportData.Master?.MENU_SIG4));
                                ReportParameter parameter15 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));

                                report.SetParameters(new ReportParameter[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9, parameter10, parameter11, parameter12, parameter13, parameter14, parameter15 });
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

        [HttpPost]
        public JsonResult UpdatePrintStatus(string codes)
        {
            try
            {
                var data = _stockTransferService.UpdatePrintStatus(codes, CommonHelper.GetValues(HttpContext));
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
    }
}
