using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
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
    public class PurchaseOrderController : BaseController
    {
        public IPurchaseOrderService _purchaseOrderService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IPeriodService _periodService { get; set; }

        public PurchaseOrderController(IPeriodService periodService, IPurchaseOrderService purchaseOrderService, IMenuService menuService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _purchaseOrderService = purchaseOrderService;
            _hostingEnvironment = hostingEnvironment;
            _periodService = periodService;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var Menu = _menuService.GetMenu(common.MenuID);
            string? itemType = string.Empty;
            string? dcType = string.Empty;
            string? typeForm = string.Empty;
            int? pType = 0;
            
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                itemType = menu.ITEM_TYPE;
                dcType = menu.DCTYPE;
                typeForm = menu.PICK_TYPE;
                pType = menu.PTYPE;

            }
                int compCond = 2;
            var period = common.Period;
            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(period));

            var StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");

            var EndDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
            string compCondQuery = "SELECT CON_QTY FROM TBL_MENU_BUILDER WHERE DLT = 'T' AND ID = '" + common.MenuID + "'";
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                SqlCommand command = new SqlCommand(compCondQuery, connection);
                connection.Open();
                object result = command.ExecuteScalar();
                compCond = Convert.ToInt32(result);
            }
            ViewBag.CompCond = compCond;
            ViewBag.Permissions = common.RoleType == "A" ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            ViewBag.EmployeeName = HttpContext.Session.GetString("Name");
            ViewBag.DateTime = CommonService.GetDateTime("Pakistan Standard Time");
            //ViewBag.Items = DropdownService.ItemMasterDropdown();
            ViewBag.Items = DropdownService.ItemMasterDropdownWithPrice(common.RoleID, common.RoleType, dcType, itemType );
            ViewBag.Units = DropdownService.UnitDropdown();
            ViewBag.Colors = DropdownService.ColorDropdown();
            ViewBag.Sizes = DropdownService.SizeDropdown();
            ViewBag.Grades = DropdownService.GradeDropdown();
            ViewBag.Warehouses = DropdownService.WareHouseDropdown();
            ViewBag.Limit = CommonHelper.GetLimitByMenueID(common.MenuID);
            ViewBag.typeForm = typeForm;
            //ViewBag.typeForm = dcType;
            ViewBag.Warehouse = DropdownService.WareHouseDropdownWthSubWithGr();
            //ViewBag.PartyType = DropdownService.PartyTypeWithpType(common.RoleID, common.RoleType, pType);
            ViewBag.PartyType = DropdownService.PartyTypeWithpType(StartDate, EndDate, "", "", common);

            //ViewBag.Parties = DropdownService.CustomPartyTypeDropdownWithAccountCode(common.RoleID, common.RoleType);
            var response = _menuService.GetMenu(common.MenuID);
            if (response.msgType == 1)
            {
                ViewBag.DATA_CLEAR = ((Menu)response.data).DATA_CLEAR;
            }
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
        public MyHttpResponseMessage GetSalesmanByParty()
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            string nextId = "";
            string maxIdQuery = "SELECT SACT_CODE FROM TBL_PARTY_TYPES WHERE  PARTY_CODE = '0'";
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                SqlCommand command = new SqlCommand(maxIdQuery, connection);
                connection.Open();
                object result = command.ExecuteScalar();
                nextId = Convert.ToString(result);
            }
            var companies = FetchSalesman(nextId);
            List<PartyTypes> companyList = new List<PartyTypes>();
            if (companies.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow Row in companies.Tables[0].Rows)
                {
                    PartyTypes company = new PartyTypes();
                    company.PARTY_CODE = Convert.ToInt32(Row["PARTY_CODE"]);
                    company.PARTY_NAME = Convert.ToString(Row["PARTY_NAME"]);
                    companyList.Add(company);
                }
            }
            response.data = companies;
            response.msg = "";
            response.msgType = 1;
            return response;
        }

        public DataSet FetchSalesman(string Id)
        {
            string query = string.Empty;
            if (Id == "")
            {
                query = "SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 9 AND DLT = 'T' AND ASTATUS = 'Y'";
            }
            else
            {
                query = "SELECT PARTY_CODE,PARTY_NAME FROM TBL_PARTY_TYPES WHERE PARTY_TYPE_CODE = 9 AND DLT = 'T' AND ASTATUS = 'Y' AND ACT_CODE = '" + Id + "'";
            }
            DataSet data = SqlHelper.ExecuteDataset(new SQLService().getconnstring(), CommandType.Text, query);
            return data;
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
        public JsonResult GetWarehouses()
        {
            try
            {
                var data = DropdownService.WareHouseDropdownWthSubWithGr();
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
        public JsonResult GetDepartments()
        {
            try
            {
                var data = DropdownService.DepartmentDropdownWithControlName();
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
        public JsonResult GetSalesmans()
        {
            try
            {

                var data = DropdownService.GetSalesmans();
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
        public JsonResult GetParties()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var Menu = _menuService.GetMenu(common.MenuID);
            int? pType = 0; 
            if (Menu.data != null)
            {
                var menu = (Menu)Menu.data;
                pType = menu.PTYPE;
            }

            try
            {
                
                var data = DropdownService.CustomPartyTypeDropdownWithAccountCodeAndPType(common.RoleID, common.RoleType, pType);
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
        public JsonResult GetPurchaseOrders()
        {
            try
            {
                var data = _purchaseOrderService.QuickSearch(CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPurchaseOrderByCode(int code)
        {
            try
            {
                var data = _purchaseOrderService.GetPurchaseOrderByCode(code, CommonHelper.GetValues(HttpContext));
                var detailData = _purchaseOrderService.GetPurchaseOrderDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPurchaseOrderDetailByCode(int code)
        {
            try
            {
                var data = _purchaseOrderService.GetPurchaseOrderDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult Save(CustomPurchaseOrder modelRecord)
        {
            try
            {
                var data = _purchaseOrderService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPickDataByParty(int partyCode, int actCode)
        {
            JsonResult result;
            try
            {
                MyHttpResponseMessage data = _purchaseOrderService.GetPickDataByParty(partyCode, actCode, CommonHelper.GetValues(base.HttpContext));
                result = Json(data);
            }
            catch (Exception ex)
            {
                string _catchMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    _catchMessage = _catchMessage + "<br/>" + ex.InnerException.Message;
                }
                result = Json(new
                {
                    msg = _catchMessage,
                    msgType = 2
                });
            }
            return result;
        }

        [HttpPost]
        public JsonResult Delete(int code)
        {
            try
            {
                var data = _purchaseOrderService.Delete(code, CommonHelper.GetValues(HttpContext));
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
                var data = _purchaseOrderService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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
        public JsonResult DeletePurchaseOrderDetailByCode(int code)
        {
            try
            {
                var data = _purchaseOrderService.DeletePurchaseOrderDetailByCode(code, CommonHelper.GetValues(HttpContext));
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

        [HttpPost]
        public JsonResult GetPrintReport(PurchaseOrderRDLCReport model)
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

        //private string GenerateReport(PurchaseOrderRDLCReport model)
        //{
        //    var filePath = "";
        //    try
        //    {
        //        if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0)
        //        {
        //            Reports.Datasets.BarcodeReportDataset.PurchaseOrderDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.PurchaseOrderDataTable();
        //            var responseMessage = _purchaseOrderService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
        //            if (responseMessage.msgType != 1)
        //            {
        //                return "";
        //            }
        //            var reportData = (CustomPurchaseOrderForPrintReport)responseMessage.data;

        //            using (LocalReport report = new LocalReport())
        //            {
        //                var path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\{reportData.Master?.REPORT_NAME}.rdlc");
        //                using (var stReader = new StreamReader(path))
        //                {
        //                    string stringreader = stReader.ReadToEnd();
        //                    byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
        //                    using (var stream = new MemoryStream(byteArray))
        //                    {
        //                        report.EnableExternalImages = true;
        //                        report.LoadReportDefinition(stream);
        //                        report.DataSources.Clear();

        //                        ReportParameter parameter1 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
        //                        ReportParameter parameter2 = new ReportParameter("InvoiceNumber", reportData.Master?.INVOICE_NUMBER);
        //                        ReportParameter parameter3 = new ReportParameter("Date", reportData.Master?.DATE);
        //                        ReportParameter parameter4 = new ReportParameter("Comment", reportData.Master?.COMMENT);
        //                        ReportParameter parameter5 = new ReportParameter("Status", reportData.Master?.STATUS);
        //                        ReportParameter parameter6 = new ReportParameter("Sig1", reportData.Master?.SIG1);
        //                        ReportParameter parameter7 = new ReportParameter("Sig2", reportData.Master?.SIG2);
        //                        ReportParameter parameter8 = new ReportParameter("Sig3", reportData.Master?.SIG3);
        //                        ReportParameter parameter9 = new ReportParameter("Sig4", reportData.Master?.SIG4);
        //                        ReportParameter parameter10 = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
        //                        ReportParameter parameter11 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS);
        //                        ReportParameter parameter12 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE);
        //                        ReportParameter parameter13 = new ReportParameter("User", reportData.Master?.USER);

        //                        report.SetParameters(new ReportParameter[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9, parameter10, parameter11, parameter12, parameter13 });
        //                        report.Refresh();
        //                        if (reportData.Detail == null)
        //                        {
        //                            throw new Exception("Report data source is empty or null.");
        //                        }

        //                        report.DataSources.Add(new ReportDataSource() { Name = "PurchaseOrderReport", Value = reportData.Detail });

        //                        byte[] file;
        //                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\PurchaseOrder");
        //                        if (!Directory.Exists(uploadsFolder))
        //                        {
        //                            Directory.CreateDirectory(uploadsFolder);
        //                        }

        //                        if (report.IsReadyForRendering)
        //                        {
        //                            string partyName = reportData.Detail.Rows[0]["Party"].ToString();
        //                            string input = reportData.Detail.Rows[0]["Voucher"].ToString();
        //                            string[] parts = input.Split('/');
        //                            string prefix = string.Empty;
        //                            string voucherNumber = string.Empty;
        //                            if (parts.Length >= 3)
        //                            {
        //                                prefix = parts[1];
        //                                voucherNumber = parts[^1];
        //                            }
        //                            file = report.Render("PDF");
        //                            filePath = $"{prefix} - {partyName.Replace("/", "-")} - {voucherNumber}" + ".pdf";

        //                            stReader.Close();
        //                            stReader.Dispose();
        //                            stream.Flush();
        //                            stream.Close();
        //                            stream.Dispose();
        //                            report.Dispose();
        //                            string reportPath = Path.Combine(uploadsFolder, filePath);
        //                            System.IO.File.WriteAllBytes(reportPath, file);
        //                            filePath = $"/Client/PurchaseOrder/{filePath}";
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return filePath;
        //}

        private string GenerateReport(PurchaseOrderRDLCReport model)
        {

            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0 && model.MD_ID > 0 && model.REPORT_NAME != null)
                {
                    Reports.Datasets.BarcodeReportDataset.PurchaseOrderDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.PurchaseOrderDataTable();
                    var responseMessage = _purchaseOrderService.GetDataForReport(model, reportDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return "";
                    }
                    var reportData = (CustomPurchaseOrderForPrintReport)responseMessage.data;

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

                        if (reportData.Master?.REPORT_NAME == "PurchaseOrder" || reportData.Master?.REPORT_NAME == "PurchaseBill")
                        {
                            var companyLogoPath = Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}");
                            var rightCompanyLogoPath = Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.RCOMPANY_LOGO}");
                            bool? showCompanyLogo = true;

                            if (!System.IO.File.Exists(companyLogoPath) || !System.IO.File.Exists(rightCompanyLogoPath))
                            {
                                showCompanyLogo = false;
                            }

                            ReportParameter parameter1 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
                            ReportParameter parameter2 = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
                            ReportParameter parameter3 = new ReportParameter("BranchName", reportData.Master?.B_NAME);
                            ReportParameter parameter4 = new ReportParameter("BranchTerms", reportData.Master?.B_TERMS);
                            ReportParameter parameter5 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS);
                            ReportParameter parameter6 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE);
                            ReportParameter parameter7 = new ReportParameter("BranchWebsite", reportData.Master?.B_WEBSITE);
                            ReportParameter parameter8 = new ReportParameter("BranchEmail", reportData.Master?.EMAIL);
                            ReportParameter parameter9 = new ReportParameter("BranchGST", reportData.Master?.B_GST);
                            ReportParameter parameter10 = new ReportParameter("BranchNTN", reportData.Master?.B_NTN);
                            ReportParameter parameter11 = new ReportParameter("Sig1", reportData.Master?.SIG1);
                            ReportParameter parameter12 = new ReportParameter("Sig2", reportData.Master?.SIG2);
                            ReportParameter parameter13 = new ReportParameter("Sig3", reportData.Master?.SIG3);
                            ReportParameter parameter14 = new ReportParameter("Sig4", reportData.Master?.SIG4);
                            ReportParameter parameter15 = new ReportParameter("MenuTerms", reportData.Master?.MENU_TERMS);
                            ReportParameter parameter16 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
                            ReportParameter parameter17 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
                            ReportParameter parameter30 = new ReportParameter("RCompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.RCOMPANY_LOGO}")).AbsoluteUri);
                            ReportParameter parameter18 = new ReportParameter("InvoiceNumber", reportData.Master?.INVOICE_NUMBER);
                            ReportParameter parameter19 = new ReportParameter("Date", reportData.Master?.DATE);
                            ReportParameter parameter20 = new ReportParameter("DelDate", reportData.Master?.DEL_DATE);
                            ReportParameter parameter21 = new ReportParameter("User", reportData.Master?.USER);
                            ReportParameter parameter22 = new ReportParameter("Status", reportData.Master?.STATUS);
                            ReportParameter parameter23 = new ReportParameter("Party", reportData.Master?.PARTY_NAME);
                            ReportParameter parameter24 = new ReportParameter("OrderType", reportData.Master?.ORDER_TYPE);
                            ReportParameter parameter25 = new ReportParameter("Terms", reportData.Master?.TERMS);
                            ReportParameter parameter26 = new ReportParameter("Ref", reportData.Master?.REF);
                            ReportParameter parameter27 = new ReportParameter("Comment", reportData.Master?.COMMENT);
                            ReportParameter parameter28 = new ReportParameter("PAddress", reportData.Master?.PADDRESS);
                            ReportParameter parameter29 = new ReportParameter("PNTN", reportData.Master?.PNTN);
                            report.SetParameters(new ReportParameter[] { parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9,
                            parameter10, parameter11, parameter12, parameter13, parameter14, parameter15, parameter16, parameter17,parameter18, parameter19, parameter20, parameter21,
                            parameter22, parameter23, parameter24, parameter25,parameter26,parameter27,parameter30, parameter28, parameter29

                            });
                        }


                        report.Refresh();
                        report.DataSources.Add(new ReportDataSource() { Name = "PurchaseOrder", Value = reportData.Detail });

                        byte[] file;
                        string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\PurchaseOrder");
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
                            filePath = $"/Client/PurchaseOrder/{filePath}";
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
