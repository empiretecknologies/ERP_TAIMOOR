using Empire_ERP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Interfaces
{
    public interface IAttendanceMachineService
    {
        MyHttpResponseMessage GetAttendanceMachineData(Common common);
        MyHttpResponseMessage GetAllAttendance(Attendance attendance, Common common);
        MyHttpResponseMessage DeleteattendanceByTime(string Time, string Date, Common common);

        //MyHttpResponseMessage GetDataForReport(ClosingShop modelRecord, DataTable details, Common common);
    }
}