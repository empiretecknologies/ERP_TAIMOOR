using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class Attendance
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Time { get; set; }
        public string? EmpId { get; set; }
        public int? Employee { get; set; }
    }
}
