using Empire_ERP.Core.Interfaces;
using Empire_ERP.Helpers;
using Empire_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Web.Administration;
using System.Configuration;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IMenuService IMenuService, IConfiguration configuration ,IBaseService baseService) : base(IMenuService, baseService)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public void StopApplication()
        {
            string siteName = _configuration["IISSettings:SiteName"];
            string appPath = _configuration["IISSettings:AppPath"];
            using (var serverManager = new ServerManager())
            {
                var site = serverManager.Sites[siteName];
                if (site != null)
                {
                    var application = site.Applications[appPath];
                    if (application != null)
                    {
                        var appPoolName = application.ApplicationPoolName;
                        var appPool = serverManager.ApplicationPools[appPoolName];
                        if (appPool != null && appPool.State == ObjectState.Started)
                        {
                            appPool.Stop();
                            Console.WriteLine($"Application '{appPath}' in site '{siteName}' stopped successfully.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Application '{appPath}' not found in site '{siteName}'.");
                    }
                }
                else
                {
                    Console.WriteLine($"Site '{siteName}' not found.");
                }
            }
        }

        public IActionResult Index()
        {
            if (String.IsNullOrWhiteSpace(HttpContext.Session.GetString("Company")))
            {
                return RedirectToAction("Details", "Login");
            }
        //    string basePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "client");

        //    // Static list of folders to delete
        //    List<string> folderNames = new List<string>
        //{
        ////    "DailyProduction",
        ////    "DeliveryFeedingReport",
        ////    "DeliveryFormat",
        ////    "DeliveryOrder",
        ////    "MaterialRequisition",
        ////    "MerchantPurchaseOrderDetail",
        ////    "MpoLayout",
        ////    "PurchaseBillReport",
        ////    "PurchaseOrder",
        ////    "PurchaseSaleFormatList",
        ////    "SaleTaxInvoice",
        //    "StockReport"
        //};

        //    List<string> results = new List<string>();

        //    foreach (var folderName in folderNames)
        //    {
        //        string folderPath = Path.Combine(basePath, folderName);

        //        if (Directory.Exists(folderPath))
        //        {
        //            try
        //            {
        //                Directory.Delete(folderPath, true); 
        //                results.Add($"Folder '{folderName}' deleted successfully.");
        //            }
        //            catch (Exception ex)
        //            {
        //                results.Add($"Error deleting '{folderName}': {ex.Message}");
        //            }
        //        }
        //        else
        //        {
        //            results.Add($"Folder '{folderName}' not found.");
        //        }
        //    }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}