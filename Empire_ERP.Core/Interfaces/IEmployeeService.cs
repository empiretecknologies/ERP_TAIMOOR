using Empire_ERP.Core.Entities;

namespace Empire_ERP.Core.Interfaces
{
    public interface IEmployeeService
    {
        MyHttpResponseMessage Delete(int code, int actCode, Common common);
        MyHttpResponseMessage Save(Employee employee, Common common);
        MyHttpResponseMessage QuickSearchEmployee(Common common);
        MyHttpResponseMessage GetEmployeeByEmployeeCode(int code, Common common);
        MyHttpResponseMessage GetEmployeeByEmployeeCode(int code);
    }
}