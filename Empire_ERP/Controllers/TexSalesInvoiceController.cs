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
    public class TexSalesInvoiceController : BaseController
    {
        public ITexSalesInvoiceService _texSalesInvoiceService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public TexSalesInvoiceController(ITexSalesInvoiceService texSalesInvoiceService, IMenuService menuService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _texSalesInvoiceService = texSalesInvoiceService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            ViewBag.DateTime = CommonService.GetDateTime("Pakistan Standard Time");
            var BranchID = HttpContext.Session.GetString("Branch");
            var CompanyID = HttpContext.Session.GetString("Company");
            string nextId = "";
            string formType = "";
            int compCond = 2;

            string maxIdQuery = "SELECT PICK_TYPE, B_I FROM TBL_MENU_BUILDER WHERE DLT = 'T' AND ID = '" + common.MenuID + "'";
            using (SqlConnection connection = new SqlConnection(new SQLService().getconnstring()))
            {
                SqlCommand command = new SqlCommand(maxIdQuery, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Indexes: 0 = MENU_NAME, 1 = B_I
                        //formType = reader.GetString(0);
                        formType = reader.IsDBNull(0) ? "" : reader.GetString(0);
                        nextId = reader.GetValue(1).ToString(); // Use GetValue to be safe for non-string types
                    }
                }
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
                ViewBag.Items = DropdownService.ItemMasterDropdown(common.RoleID, common.RoleType);
            }
            else
            {
                //ViewBag.Items = DropdownService.OnlyBarcodeDropdown();
                ViewBag.Items = DropdownService.CustomBarcodeDropdownForStockTransfer();
            }
            ViewBag.Type = nextId;
            ViewBag.FormType = formType;
            ViewBag.CompCond = compCond;
            ViewBag.Units = DropdownService.UnitDropdown();
            ViewBag.Colors = DropdownService.ColorDropdown();
            ViewBag.Sizes = DropdownService.SizeDropdown();
            ViewBag.Grades = DropdownService.GradeDropdown();
            ViewBag.ClientPO = DropdownService.ClientPODropdown();
            ViewBag.Currencies = DropdownService.CurrencyDropdownWithControlName();
            ViewBag.ReportTypes = GetReportTypes();
            ViewBag.PartyType = common.RoleType == "A"
                ? DropdownService.PartyTypeDropdownForInvoice(0, common.Branch, 0)
                : DropdownService.PartyTypeDropdownForInvoice(common.RoleID, common.Branch, common.ShowSelected);
            ViewBag.ItemIds = common.RoleType == "A"
                ? DropdownService.ItemIdsDropdownForPurchaseBill(0, common.Branch, 0)
                : DropdownService.ItemIdsDropdownForPurchaseBill(common.RoleID, common.Branch, common.ShowSelected);
            ViewBag.Salesman = common.RoleType == "A"
                ? DropdownService.SalesmanDropdown(0, common.Branch, 0)
                : DropdownService.SalesmanDropdown(common.RoleID, common.Branch, common.ShowSelected);
            ViewBag.Permissions = common.RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(common.RoleID, common.MenuID);
            ViewBag.Limit = CommonHelper.GetLimitByMenueID(common.MenuID);
            ViewBag.Parties = DropdownService.CustomPartyTypeDropdownWithAccountCode(common.RoleID, common.RoleType);
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
                var data = DropdownService.WareHouseDropdown();
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
        public JsonResult QuickSearch()
        {
            try
            {
                var data = _texSalesInvoiceService.QuickSearch(CommonHelper.GetValues(HttpContext));
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

        //[HttpPost]
        //public JsonResult GetPurchaseBill(int skip = 0, int take = 12, string sort = null, string filter = null, string group = null)
        //{
        //    try
        //    {
        //        var data = _texSalesInvoiceService.QuickSearch(CommonHelper.GetValues(HttpContext), skip, take, filter, group, sort);
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
        public JsonResult GetPurchaseBillByCode(int code)
        {
            try
            {
                var data = _texSalesInvoiceService.GetPurchaseBillByCode(code, CommonHelper.GetValues(HttpContext));
                var detailData = _texSalesInvoiceService.GetPurchaseBillDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPurchaseBillDetailByCode(int code)
        {
            try
            {
                var data = _texSalesInvoiceService.GetPurchaseBillDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPurchaseBillPickDetailByCode(int code)
        {
            try
            {
                var data = _texSalesInvoiceService.GetPurchaseBillPickDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPurchaseBillDetailByItem(int code, int qty)
        {
            try
            {
                var data = _texSalesInvoiceService.GetPurchaseBillDetailByItem(code, qty, CommonHelper.GetValues(HttpContext));
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
        public JsonResult Save(CustomTexSalesInvoice modelRecord)
        {
            try
            {
                var data = _texSalesInvoiceService.Save(modelRecord, CommonHelper.GetValues(HttpContext));
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
                var data = _texSalesInvoiceService.Delete(code, CommonHelper.GetValues(HttpContext));
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
                var data = _texSalesInvoiceService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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
        public JsonResult DeletePurchaseBillDetailByCode(int code)
        {
            try
            {
                var data = _texSalesInvoiceService.DeletePurchaseBillDetailByCode(code, CommonHelper.GetValues(HttpContext));
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
        public MyHttpResponseMessage GetSalesmanByParty(int Id)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            string nextId = "";
            string maxIdQuery = "SELECT SACT_CODE FROM TBL_PARTY_TYPES WHERE  PARTY_CODE = '" + Id + "'";
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
        public JsonResult GetPrintReport(TexSalesInvoiceRDLCReport model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();
            try
            {
                var responseReport = GenerateReport(model);

                if (responseReport.msgType == 1)
                {
                    response.data = responseReport.data;
                    response.msg = "";
                    response.msgType = 1;
                }
                else
                {
                    response.msg = responseReport.msg;
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


        private MyHttpResponseMessage GenerateReport(TexSalesInvoiceRDLCReport model)
        {
            MyHttpResponseMessage response = new MyHttpResponseMessage();

            var filePath = "";
            try
            {
                if (model != null && model.TRAN_ID > 0)
                {
                    Reports.Datasets.BarcodeReportDataset.InspectionServiceChargesReportDataTable inspectionServiceChargesDetails = new Reports.Datasets.BarcodeReportDataset.InspectionServiceChargesReportDataTable();
                    var responseMessage = _texSalesInvoiceService.GetDataForReport(model, inspectionServiceChargesDetails, CommonHelper.GetValues(HttpContext));
                    if (responseMessage.msgType != 1)
                    {
                        return responseMessage;
                    }
                    var reportData = (CustomTexSalesInvoiceForPrintReport)responseMessage.data;

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

                                ReportParameter parameter = new ReportParameter("CompanyName", reportData.Master?.COMPANY_NAME);
                                ReportParameter parameter1 = new ReportParameter("CompanyAddress", reportData.Master?.COMPANY_ADDRESS);
                                ReportParameter parameter2 = new ReportParameter("CompanyPhone", reportData.Master?.COMPANY_PHONE);
                                ReportParameter parameter3 = new ReportParameter("BranchAddress", reportData.Master?.BRANCH_ADDRESS);
                                ReportParameter parameter4 = new ReportParameter("BranchPhone", reportData.Master?.BRANCH_PHONE);
                                ReportParameter parameter24 = new ReportParameter("Discount", Convert.ToString(reportData.Master?.DISC));
                                ReportParameter parameter5 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
                                ReportParameter parameter6 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
                                ReportParameter parameter7 = new ReportParameter("Header", reportData.Master?.HEADER_NAME);
                                ReportParameter parameter8 = new ReportParameter("InvoiceNumber", reportData.Master?.INVOICE_NUMBER);
                                ReportParameter parameter9 = new ReportParameter("Date", reportData.Master?.DATE);
                                ReportParameter parameter10 = new ReportParameter("PartyName", reportData.Master?.PARTY_NAME);
                                ReportParameter parameter11 = new ReportParameter("ActGrCode", reportData.Master?.ACT_GRCODE);
                                ReportParameter parameter12 = new ReportParameter("Sig1", reportData.Master?.SIG1);
                                ReportParameter parameter13 = new ReportParameter("Sig2", reportData.Master?.SIG2);
                                ReportParameter parameter14 = new ReportParameter("Sig3", reportData.Master?.SIG3);
                                ReportParameter parameter15 = new ReportParameter("Sig4", reportData.Master?.SIG4);
                                ReportParameter parameter16 = new ReportParameter("CompanyWater", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_WATER}")).AbsoluteUri);
                                ReportParameter parameter17 = new ReportParameter("REFERENCENO", reportData.Master?.REFERENCENO);
                                ReportParameter parameter25 = new ReportParameter("ClientPO", reportData.Master?.CLIENT_PO);
                                ReportParameter parameter18 = new ReportParameter("TERM", reportData.Master?.TERM);
                                ReportParameter parameter19 = new ReportParameter("EDITUSERID", reportData.Master?.EDIT_USER_ID);
                                ReportParameter parameter20 = new ReportParameter("MENUTERMS", reportData.Master?.MENU_TERMS);
                                ReportParameter parameter21 = new ReportParameter("Curr", reportData.Master?.CURR);
                                ReportParameter parameter22 = new ReportParameter("CurrSig", reportData.Master?.CURR_SIG);
                                ReportParameter parameter23 = new ReportParameter("TranId", Convert.ToString(model.TRAN_ID));
                                ReportParameter parameter26 = new ReportParameter("SupInvNo", reportData.Master?.SUP_INVNO);


                                report.SetParameters(new ReportParameter[] { parameter, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8,
                                    parameter9, parameter10, parameter11, parameter12, parameter13, parameter14, parameter15, parameter16, parameter17, parameter18, parameter19, parameter20,
                                    parameter21, parameter22, parameter23, parameter25, parameter26 });
                                report.Refresh();
                                report.DataSources.Add(new ReportDataSource() { Name = "ServiceChargesReportDataSet", Value = reportData.Detail });


                                byte[] file;
                                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\PurchaseBillReport");
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
                                    filePath = $"{prefix} - {(reportData.Master?.PARTY_NAME).Replace(" / ", " - ")} - {voucherNumber}" + ".pdf";

                                    stReader.Close();
                                    stReader.Dispose();
                                    stream.Flush();
                                    stream.Close();
                                    stream.Dispose();
                                    report.Dispose();
                                    string reportPath = Path.Combine(uploadsFolder, filePath);
                                    System.IO.File.WriteAllBytes(reportPath, file);
                                    filePath = $"/Client/PurchaseBillReport/{filePath}";
                                    response.data = filePath;
                                    response.msgType = 1;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.msg = "Unable to generate report. Please try again later.";
                response.msgType = 2;
            }
            return response;
        }

        [HttpGet]
        public JsonResult GetPickDataBySupplier(int pCode, int actCode)
        {
            try
            {
                var data = _texSalesInvoiceService.GetPickDataBySupplier(pCode, actCode, CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetBarcodeList()
        {
            try
            {
                var data = _texSalesInvoiceService.GetBarcodeList();
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
