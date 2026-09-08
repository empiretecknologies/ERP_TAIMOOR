using Empire_ERP.Core.Entities;
using Empire_ERP.Core.Interfaces;

namespace Empire_ERP.Core.Services
{
    public class EmployeeService : IEmployeeService
    {
        public IEmployeeRepository _employeeRepository { get; set; }
        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public MyHttpResponseMessage Save(Employee employee, Common common)
        {
            return _employeeRepository.Save(employee, common);
        }

        public MyHttpResponseMessage Delete(int employeeCode, int actCode, Common common)
        {
            return _employeeRepository.Delete(employeeCode, actCode, common);
        }

        public MyHttpResponseMessage QuickSearchEmployee(Common common)
        {
            return _employeeRepository.QuickSearchEmployee(common);
        }

        public MyHttpResponseMessage GetEmployeeByEmployeeCode(int employeeCode, Common common)
        {
            return _employeeRepository.GetEmployeeByEmployeeCode(employeeCode, common);
        }

        public MyHttpResponseMessage GetEmployeeByEmployeeCode(int employeeCode)
        {
            return _employeeRepository.GetEmployeeByEmployeeCode(employeeCode);
        }
    }
}