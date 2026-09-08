using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Services;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IAttendanceMachineRepository
    {
        MyHttpResponseMessage GetAttendanceMachineData(Common common);
        MyHttpResponseMessage GetAllAttendance(Attendance attendance, Common common);

        MyHttpResponseMessage DeleteattendanceByTime(string Time, string Date , Common common);
        //MyHttpResponseMessage GetDataForReport(ClosingShop modelRecord, CustomMenuDetail menuDetails, Info currentLabel, Branch currentBranch, Company currentCompany, Common common);
    }
}