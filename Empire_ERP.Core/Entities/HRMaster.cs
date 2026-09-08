using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class HRMaster
    {
        public int? GROUP_CODE { get; set; }
        public string? GROUP_NAME { get; set; }
        public string? LEAVES { get; set; }
        public string? TIMEIN { get; set; }
        public string? TIMEOUT { get; set; }
        public string? HOURS { get; set; }
        public string? GTIMEIN { get; set; }
        public string? GTIMEOUT { get; set; }
        public string? BTIMEIN { get; set; }
        public string? BTIMEOUT { get; set; }
        public string? NIGHTSHIFT { get; set; }
        public string? Driver { get; set; }
        public string? Vehicle { get; set; }
        public string? Capacity { get; set; }
        public string? tRoute { get; set; }
        public string? BSALARY { get; set; }
        public string? GSALARY { get; set; }
        public string? AnnualPct { get; set; }
        public string? Annualamt { get; set; }
        public string? MaximumSalary { get; set; }
        public string? TableName { get; set; }
        public int? MasterId { get; set; }
        public string? ADD_USER_ID { get; set; }
        public DateTime? ADD_DATE { get; set; }
        public string? ADD_COMPUTER_NAME { get; set; }
        public string? ADD_IP_ADDRESS { get; set; }
        public string? EDIT_USER_ID { get; set; }
        public DateTime? EDIT_DATE { get; set; }
        public string? EDIT_COMPUTER_NAME { get; set; }
        public string? EDIT_IP_ADDRESS { get; set; }
        public string? ADD_POSTALCODE { get; set; }
        public string? EDIT_POSTALCODE { get; set; }
        public string? ASTATUS { get; set; }
        public int? MENU_ID { get; set; }
        public string? DLT { get; set; }
        public double? QTY { get; set; }
    }
}
