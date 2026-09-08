using Azure;
using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Services
{
    public class AttendanceMachineService : IAttendanceMachineService
    {
        public IAttendanceMachineRepository _AttendanceMachineRepository { get; set; }
        public IMenuService _menuService { get; set; }
        public ICompanyService _companyService { get; set; }
        public IBranchService _branchService { get; set; }
        public ILoginService _loginService { get; set; }
        public AttendanceMachineService(IAttendanceMachineRepository attendanceMachineRepository, IMenuService menuService, ICompanyService companyService, IBranchService branchService, ILoginService loginService)
        {
            _AttendanceMachineRepository = attendanceMachineRepository;
            _menuService = menuService;
            _companyService = companyService;
            _branchService = branchService;
            _loginService = loginService;
        }

        public MyHttpResponseMessage GetAttendanceMachineData(Common common)
        {
            return _AttendanceMachineRepository.GetAttendanceMachineData(common);
        }
        public MyHttpResponseMessage GetAllAttendance(Attendance attendance, Common common)
        {
            return _AttendanceMachineRepository.GetAllAttendance(attendance, common);
        }
        public MyHttpResponseMessage DeleteattendanceByTime(string Time, string Date , Common common)
        {
            return _AttendanceMachineRepository.DeleteattendanceByTime(Time ,Date , common);
        }
        //public MyHttpResponseMessage GetDataForReport(ClosingShop modelRecord, DataTable dataTable, Common common)
        //{
        //    MyHttpResponseMessage response = new MyHttpResponseMessage();
        //    CustomMenuDetail menuDetail = new CustomMenuDetail();
        //    try
        //    {
        //        var menuResponse = _menuService.GetMenuDetails(common.MenuID);
        //        if (menuResponse.msgType != 1)
        //        {
        //            response.msgType = 2;
        //            return response;
        //        }
        //        var menuData = (List<CustomMenuDetail>)menuResponse.data;
        //        if (menuData.Count > 0)
        //        {
        //            if (modelRecord.Master.MD_ID != null)
        //            {
        //                menuDetail = menuData.Where(m => m.MD_ID == modelRecord.Master.MD_ID).FirstOrDefault();
        //            }
        //            else
        //            {
        //                menuDetail = menuData.Where(m => m.MD_ID == 29).FirstOrDefault();
        //            }

        //            if (menuDetail?.MD_ID <= 0)
        //            {
        //                response.msgType = 2;
        //                return response;
        //            }
        //        }
        //        else
        //        {
        //            response.msgType = 2;
        //            return response;
        //        }

        //        var currentCompanyResponse = _companyService.GetCompanyByCode(common.Company);
        //        if (currentCompanyResponse.msgType != 1)
        //        {
        //            response.msgType = 2;
        //            return response;
        //        }

        //        var currentCompany = (Company)currentCompanyResponse.data;

        //        var currentBranchResponse = _branchService.GetBranchByCode(common.Branch);
        //        if (currentBranchResponse.msgType != 1)
        //        {
        //            response.msgType = 2;
        //            return response;
        //        }

        //        var currentBranch = (Branch)currentBranchResponse.data;

        //        var currentLableResponse = _loginService.GetBackGroundAndLogo();
        //        var currentLabel = (Info)currentLableResponse;

        //        return _AttendanceMachineRepository.GetDataForReport(modelRecord, menuDetail, currentLabel, currentBranch, currentCompany, common);
        //    }
        //    catch (Exception ex)
        //    {
        //        string _catchMessage = ex.Message;
        //        if (ex.InnerException != null)
        //        {
        //            _catchMessage += "<br/>" + ex.InnerException.Message;
        //        }
        //        response.msgType = 2;
        //        response.msg = _catchMessage;
        //    }
        //    return response;
        //}

    }
}