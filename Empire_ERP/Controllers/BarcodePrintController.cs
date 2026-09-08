using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection.Metadata;
using System.Text;
using static Azure.Core.HttpHeader;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class BarcodePrintController : BaseController
    {
        public IBarcodePrintService _barcodePrintService { get; set; }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ICompanyService _companyService { get; set; }
        public IPeriodService _periodService { get; set; }
        public BarcodePrintController(IPeriodService periodService,IBarcodePrintService barcodePrintService, ICompanyService companyService, IWebHostEnvironment hostingEnvironment, IMenuService menuService,IBaseService baseService) : base(menuService,baseService)
        {
            _barcodePrintService = barcodePrintService;
            _hostingEnvironment = hostingEnvironment;
            _companyService = companyService;
            _periodService = periodService;
        }

        public IActionResult Index()
        {
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            return View();
        }

        [HttpGet]
        public JsonResult GetBarcodePrintData()
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
                ViewBag.currentBranch = common.Branch;
                var SDATE = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
                var EDATE = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");
                var data = _barcodePrintService.GetData(SDATE, EDATE);
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
        public JsonResult GetBarcodeReport(BarcodePrintForRDLC data)
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
                    response.msg = "Unable to generate barcodes. Please try again later.";
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

        private byte[] GenerateReport(BarcodePrintForRDLC barcodeData)
        {
            var filePath = "";
            try
            {
                //var request = HttpContext.Request;
                //var domain = $"{request.Scheme}://{request.Host}/";
                if (barcodeData.data.Count > 0 && barcodeData.MD_ID > 0) 
                {
                    //var common = CommonHelper.GetValues(HttpContext);
                    var barcodeLabels = DropdownService.BarcodeLabelDropdown();
                    if (barcodeLabels.Count == 0)
                    {
                        return null;
                    }
                    Reports.Datasets.BarcodeReportDataset.BarcodeReportDataTable data = new Reports.Datasets.BarcodeReportDataset.BarcodeReportDataTable();
                    using (LocalReport report = new LocalReport())
                    {
                        var path = Path.Combine(_hostingEnvironment.ContentRootPath, @$"Reports\{barcodeData.REPORT_NAME}.rdlc");
                        using (var stReader = new StreamReader(path))
                        {
                            string stringreader = stReader.ReadToEnd();
                            byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
                            using (var stream = new MemoryStream(byteArray))
                            {

                                report.EnableExternalImages = true;
                                report.LoadReportDefinition(stream);

                                foreach (var item in barcodeData.data)
                                {
                                    try
                                    {
                                        var barcodePath = _barcodePrintService.GenerateBarcode(item.BARCODE, _hostingEnvironment.WebRootPath);
                                        if (!String.IsNullOrEmpty(barcodePath))
                                        {
                                            var barcodeLabel = barcodeLabels.Where(x => x.Key == item.BLABEL).FirstOrDefault();
                                            if (barcodeLabel.Key <= 0 && String.IsNullOrEmpty(barcodeLabel.Value))
                                            {
                                                return null;
                                            }
                                            if (barcodeData.MD_ID is 23 or 33 or 40 or 59 or 60)
                                            {
                                                for (int i = 0; i < item.QTY; i++)
                                                {
                                                    DataRow dataRow = data.NewRow();
                                                    dataRow["Name"] = item.ITEM_NAME;
                                                    dataRow["Size"] = item.SIZE_NAME;
                                                    dataRow["Barcode"] = item.BARCODE;
                                                    dataRow["Color"] = item.COLOR_NAME;
                                                    dataRow["Rate"] = item.ISSALE.HasValue && item.ISSALE.Value ? item.SRATE : item.ISWHOLESALE.HasValue && item.ISWHOLESALE.Value ? item.WSALE : item.ISRETAIL.HasValue && item.ISRETAIL.Value ? item.RRATE : "";
                                                    dataRow["Currency"] = "Rs";
                                                    dataRow["CompanyName"] = barcodeLabel.Value;
                                                    dataRow["Group"] = item.ITEM_GROUP;
                                                    dataRow["BarcodePath"] = barcodePath;
                                                    dataRow["Category"] = item.CAT_CODE;
                                                    dataRow["Remarks"] = item.REMARKS;
                                                    data.Rows.Add(dataRow);
                                                }
                                            }
                                            else if (barcodeData.MD_ID == 24)
                                            {
                                                for (int i = 0; i < item.QTY; i++)
                                                {
                                                    DataRow dataRow = data.NewRow();
                                                    dataRow["Name"] = item.ITEM_NAME;
                                                    dataRow["Size"] = item.SIZE_NAME;
                                                    dataRow["Barcode"] = item.BARCODE;
                                                    dataRow["Color"] = item.COLOR_NAME;
                                                    dataRow["Rate"] = item.ISSALE.HasValue && item.ISSALE.Value ? item.SRATE : item.ISWHOLESALE.HasValue && item.ISWHOLESALE.Value ? item.WSALE : item.ISRETAIL.HasValue && item.ISRETAIL.Value ? item.RRATE : "";
                                                    dataRow["Currency"] = "Rs";
                                                    dataRow["CompanyName"] = barcodeLabel.Value;
                                                    dataRow["Group"] = item.ITEM_GROUP;
                                                    dataRow["BarcodePath"] = barcodePath;
                                                    dataRow["Category"] = item.COLOR_NAME;
                                                    dataRow["Remarks"] = item.REMARKS;
                                                    data.Rows.Add(dataRow);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            return null;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        return null;
                                    }
                                }

                                System.Data.DataSet dataSet = new System.Data.DataSet();
                                dataSet.Tables.Add(data);

                                ReportDataSource reportData = new ReportDataSource();
                                reportData.Name = "ReportDataSet";
                                reportData.Value = data;

                                report.DataSources.Clear();
                                report.DataSources.Add(reportData);

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

        //private async Task<string> PrepareReport(List<CommissionMaster> master, List<CommissionDetail> child, string exportType)
        //{
        //    var filePath = "";
        //    try
        //    {
        //        using (LocalReport report = new LocalReport())
        //        {
        //            var path = Path.Combine(webHostEnvironment.WebRootPath, @"Reports\Report\PayrollReport.rdlc");
        //            var stReader = new StreamReader(path);
        //            //var stReader = new StreamReader(@".\Reports\Report\PayrollReport.rdlc");
        //            string stringreader = stReader.ReadToEnd();
        //            byte[] byteArray = Encoding.UTF8.GetBytes(stringreader);
        //            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
        //            MemoryStream stream = new MemoryStream(byteArray);

        //            //report.Refresh();
        //            report.LoadReportDefinition(stream);
        //            report.EnableExternalImages = true;
        //            var dsmems = report.GetDataSourceNames();
        //            report.DataSources.Add(new ReportDataSource(dsmems[0], pmList));
        //            report.DataSources.Add(new ReportDataSource(dsmems[1], pmListC));

        //            report.SetParameters(parameters);
        //            var prms = report.GetParameters();
        //            byte[] file;

        //            string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "PayrollReports");
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }
        //            var tt = report.IsReadyForRendering;
        //            if (exportType == "Excel")
        //            {
        //                file = report.Render("EXCELOPENXML");
        //                //file = report.Render("WORDOPENXML");
        //                filePath = "PayrollReport_" + Guid.NewGuid().ToString("N").Substring(0, 15) + ".xlsx";

        //                stReader.Close();
        //                stReader.Dispose();
        //                stream.Flush();
        //                stream.Close();
        //                stream.Dispose();
        //                report.Dispose();
        //                string reportPath = Path.Combine(uploadsFolder, filePath);
        //                System.IO.File.WriteAllBytes(reportPath, file);
        //                ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.Commercial;
        //                using (var package = new ExcelPackage(new FileInfo(reportPath)))
        //                {
        //                    if (package.Workbook.Worksheets.Count == 2)
        //                    {
        //                        var worksheet = package.Workbook.Worksheets[0];
        //                        worksheet.Name = "Compensation Summary Report";
        //                        var worksheet1 = package.Workbook.Worksheets[1];
        //                        worksheet1.Name = "Compensation Detail Report";
        //                        package.Save();
        //                    }
        //                }

        //                filePath = "PayrollReports/" + filePath;

        //            }
        //            else
        //            {
        //                file = report.Render("PDF");
        //                filePath = "PayrollReport_" + Guid.NewGuid().ToString("N").Substring(0, 15) + ".pdf";

        //                stReader.Close();
        //                stReader.Dispose();
        //                stream.Flush();
        //                stream.Close();
        //                stream.Dispose();
        //                report.Dispose();
        //                string reportPath = Path.Combine(uploadsFolder, filePath);
        //                System.IO.File.WriteAllBytes(reportPath, file);
        //                filePath = "PayrollReports/" + filePath;
        //            }
        //            //report.Refresh();
        //            //report.Dispose();


        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        var result = await common.AddErrorLogs(ex);
        //        throw;
        //    }


        //    return filePath;
        //}

        //private void Btn_Print_Click(object sender, EventArgs e)
        //{
        //    #region Using RDLC Report

        //    if (CheckData())
        //    {
        //        Datasets.BarcodeReportDataset.BarcodeReportDataTable data = new Datasets.BarcodeReportDataset.BarcodeReportDataTable();
        //        foreach (Panel panel in Pn_Tasks.Controls)
        //        {
        //            var cbItems = panel.Controls.Find("Cb_Items", true).FirstOrDefault();
        //            if (cbItems != null)
        //            {
        //                GunaComboBox ItemComboBox = (GunaComboBox)cbItems;
        //                var item = (Item)ItemComboBox.SelectedItem;

        //                var itemCount = panel.Controls.Find("Txt_Count", true).FirstOrDefault();
        //                var Txt_ItemCount = (GunaTextBox)itemCount;
        //                item = ItemService.Instance.GetItemById(item.Id);
        //                if (item.Id > 0)
        //                {
        //                    //Datasets.BarcodeReportDataset.BarcodeReportDataTable data = new Datasets.BarcodeReportDataset.BarcodeReportDataTable();
        //                    var barcode = ItemService.Instance.GenerateBarCode(item);
        //                    for (int i = 1; i <= Convert.ToInt32(Txt_ItemCount.Text); i++)
        //                    {
        //                        DataRow dataRow = data.NewRow();
        //                        dataRow["Name"] = item.Name;
        //                        dataRow["Rate"] = $"Rs. {item.Rate}";
        //                        dataRow["Barcode"] = barcode;
        //                        dataRow["MfgDate"] = item.ManufacturedDate.Value.ToString("MMM-yy");
        //                        dataRow["ExpDate"] = item.ExpireDate.Value.ToString("MMM-yy");
        //                        dataRow["CompanyName"] = Convert.ToString(ConfigurationManager.AppSettings["CompanyName"]).ToUpper();
        //                        data.Rows.Add(dataRow);
        //                    }
        //                }
        //            }
        //        }

        //        DataSet dataSet = new DataSet();
        //        dataSet.Tables.Add(data);

        //        ReportDataSource reportData = new ReportDataSource();
        //        reportData.Name = "ReportDataSet";
        //        reportData.Value = data;

        //        string reportPath = "Reports\\BarcodeReport.rdlc";
        //        PageSettings pageSettings = new PageSettings();
        //        pageSettings.PaperSize = new PaperSize("Custom", Convert.ToInt32(4 * 100), Convert.ToInt32(1 * 100));
        //        pageSettings.Margins.Left = 6;
        //        pageSettings.Margins.Right = 0;
        //        reportViewer.SetPageSettings(pageSettings);
        //        reportViewer.LocalReport.DataSources.Clear();
        //        reportViewer.LocalReport.DataSources.Add(reportData);
        //        reportViewer.LocalReport.ReportPath = reportPath;
        //        reportViewer.LocalReport.EnableExternalImages = true;
        //        reportViewer.RefreshReport();
        //        reportViewer.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
        //        reportViewer.Visible = true;

        //        this.Size = new Size(997, 536);

        //        vScrollHelper = new Guna.UI.Lib.ScrollBar.PanelScrollHelper(Pn_Tasks, GunaVScrollBar1, true);
        //        hScrollHelper = new Guna.UI.Lib.ScrollBar.PanelScrollHelper(Pn_Tasks, GunaHScrollBar1, true);

        //        vScrollHelper.UpdateScrollBar();
        //        hScrollHelper.UpdateScrollBar();

        //        this.StartPosition = FormStartPosition.Manual;
        //        this.Location = new System.Drawing.Point((Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
        //                                                  (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Please correct data of red fields!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    #endregion
        //}
    }
}
