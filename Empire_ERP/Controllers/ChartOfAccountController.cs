using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using Empire_ERP.Core.Services;
using Empire_ERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices.JavaScript;

namespace Empire_ERP.Controllers
{
    [CheckSession]
    [ExtractMenuCode]
    public class ChartOfAccountController : BaseController
    {
        public IChartOfAccountService _chartOfAccountService { get; set; }
        public IPeriodService _periodService { get; set; }
        public ChartOfAccountController(IChartOfAccountService chartOfAccountService, IMenuService menuService,IBaseService baseService, IPeriodService periodService) : base(menuService, baseService)
        {
            _chartOfAccountService = chartOfAccountService;
            _periodService = periodService;
        }

        public IActionResult Index()
        {
            ViewBag.Cheque = JsonConvert.SerializeObject(DropdownService.ChequeFormatDropdown());
            ViewBag.Currency = DropdownService.CurrencyDropdown();
            ViewBag.ChartType = DropdownService.GetChartTypeDropDown();
            ViewBag.Permissions = CommonHelper.GetValues(HttpContext).RoleType == "A"
                ? "Admin"
                : CommonHelper.GetPermissionByMenueID(CommonHelper.GetValues(HttpContext).RoleID, CommonHelper.GetValues(HttpContext).MenuID);
            return View();
        }



        [HttpGet]
        public JsonResult GetChartType()
        {
            try
            {
                var data = DropdownService.GetChartTypeDropDown();
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
        public JsonResult QuickSearch()
        {
            try
            {
                var data = _chartOfAccountService.QuickSearch(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public JsonResult GetChartOfAccounts()
        {
            var data = _chartOfAccountService.GetChartOfAccounts(CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpGet]
        public JsonResult GetAccountsForTreeView()
        {
            try
            {
                var data = _chartOfAccountService.GetAccountsForTreeView(CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       

        [HttpGet]
        public JsonResult ChartOfAccountByid(int id)
        {
            var data = _chartOfAccountService.GetChartOfAccountById(id, CommonHelper.GetValues(HttpContext));
            return Json(data);
        }

        [HttpPost]
        public JsonResult Save(ChartOfAccount model)
        {
            try
            {
                var data = _chartOfAccountService.Save(model, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            try
            {
                var common = CommonHelper.GetValues(HttpContext);
                var periodInfo = _periodService.GetPeriodById(Convert.ToInt32(common.Period));
                var startDate = ((Period)periodInfo.data).START_D.Value.ToString("yyyy-MM-dd");
                var endDate = ((Period)periodInfo.data).START_E.Value.ToString("yyyy-MM-dd");

                var data = _chartOfAccountService.Delete(id, startDate, endDate, CommonHelper.GetValues(HttpContext));
                return Json(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public JsonResult CopyRecord(CopyRecord record)
        {
            try
            {
                var data = _chartOfAccountService.CopyRecord(record, CommonHelper.GetValues(HttpContext));
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