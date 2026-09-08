using System.Text;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;

namespace Empire_ERP.Controllers
{
    [ExtractMenuCode]
    [CheckSession]
    public class ImportReportController : BaseController
    {
        public IPeriodService _periodService { get; set; }
        public IImportReportService _importReportService { get; set; }
        public ICompanyService _companyService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ImportReportController(IMenuService menuService, IPeriodService periodService, IImportReportService importReportService, ICompanyService companyService, IWebHostEnvironment hostingEnvironment,IBaseService baseService) : base(menuService,baseService)
        {
            _periodService = periodService;
            _importReportService = importReportService;
            _companyService = companyService;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var common = CommonHelper.GetValues(HttpContext);
            var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
            var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
            var currentCompany = (Company)currentCompanyResponse.data;
            ViewBag.CompanyName = currentCompany.C_NAME;
            ViewBag.Controls = DropdownService.GetControlsForImportReport();
            ViewBag.Subsidiary = DropdownService.GetSubsidiaryForImportReport();
            ViewBag.StartDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = ((Period)periodInfo.data).CLOSING == 1
               ? ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd")
               : DateTime.Now.ToString("yyyy-MM-dd");
            return View();
        }


        [HttpGet]
        public JsonResult GetReportTypes()
        {
            try
            {
                var data = _importReportService.GetReportTypes(CommonHelper.GetValues(HttpContext));
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
        public JsonResult GetPrintReport(ImportReportRDLCReport model)
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

        private string GenerateReport(ImportReportRDLCReport model)
        {
            var filePath = "";
            try
            {
                if (model != null && model.REPORTID > 0)
                {
                    Reports.Datasets.BarcodeReportDataset.ImportBillReportDataTable reportDetails = new Reports.Datasets.BarcodeReportDataset.ImportBillReportDataTable();
                    var responseMessage = _importReportService.GetDataForReport(model, reportDetails,CommonHelper.GetValues(HttpContext));
                    var path = "";
                    if (responseMessage.msgType != 1)
                    {
                        return "";
                    }
                    var reportData = (CustomImportReportForPrintReport)responseMessage.data;

                    using (LocalReport report = new LocalReport())
                    {
                        if (model.REPORTID == 131)
                        {
                            path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\ImportBill.rdlc");
                        }

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
                                ReportParameter parameter5 = new ReportParameter("CompanyLogo", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_LOGO}")).AbsoluteUri);
                                ReportParameter parameter6 = new ReportParameter("ShowCompanyLogo", Convert.ToString(showCompanyLogo));
                                ReportParameter parameter7 = new ReportParameter("Header", Convert.ToString(model.REPORT_NAME));
                                ReportParameter parameter8 = new ReportParameter("FromDate",model.FROMDATE.ToString("dd-MMM-yyyy"));
                                ReportParameter parameter9 = new ReportParameter("Date", reportData.Master?.DATE);
                                ReportParameter parameter12 = new ReportParameter("Sig1", reportData.Master?.SIG1);
                                ReportParameter parameter13 = new ReportParameter("Sig2", reportData.Master?.SIG2);
                                ReportParameter parameter14 = new ReportParameter("Sig3", reportData.Master?.SIG3);
                                ReportParameter parameter15 = new ReportParameter("Sig4", reportData.Master?.SIG4);
                                ReportParameter parameter16 = new ReportParameter("CompanyWater", new Uri(Path.Combine(_hostingEnvironment.WebRootPath, @$"Client\Company\{reportData.Master?.COMPANY_WATER}")).AbsoluteUri);
                                

                                report.SetParameters(new ReportParameter[] { parameter, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9, parameter12, parameter13, parameter14, parameter15, parameter16, });
                                report.Refresh();
                                report.DataSources.Add(new ReportDataSource() { Name = "ImportBillReportDataSet", Value = reportData.Detail });


                                byte[] file;
                                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, @"Client\PurchaseBillReport");
                                if (!Directory.Exists(uploadsFolder))
                                {
                                    Directory.CreateDirectory(uploadsFolder);
                                }

                                if (report.IsReadyForRendering)
                                {
                                    if (model.REPORTID == 131)
                                    {
                                        string input = reportData.Master?.COMPANY_NAME;
                                        string[] parts = input.Split('/');
                                        string prefix = string.Empty;
                                        file = report.Render("PDF");
                                        filePath = $"{prefix} - {(model.REPORT_NAME).Replace(" / ", " - ")}" + ".pdf";
                                    }
                                    else
                                    {
                                        file = report.Render("PDF");
                                        filePath = $"unknown.pdf";
                                    }

                                    stReader.Close();
                                    stReader.Dispose();
                                    stream.Flush();
                                    stream.Close();
                                    stream.Dispose();
                                    report.Dispose();
                                    string reportPath = Path.Combine(uploadsFolder, filePath);
                                    System.IO.File.WriteAllBytes(reportPath, file);
                                    filePath = $"/Client/PurchaseBillReport/{filePath}";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
            }
            return filePath;
        }
    }
}
