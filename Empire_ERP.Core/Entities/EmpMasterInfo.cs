using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class EmpMasterInfo
    {
        public int? GROUP_CODE { get; set; }
        public string? START_D { get; set; }
        public string? END_D { get; set; }
        public string? CompanyName { get; set; }
        public string? DESC_S { get; set; }
        public string? DESC_E { get; set; }
        public int? InitialSalary { get; set; }
        public int? FinalSalary { get; set; }
        public string? CellNo { get; set; }
        public string? LREASON { get; set; }
        public string? WADD { get; set; }
        public string? MSubject { get; set; }
        public string? EYear { get; set; }
        public string? GBatch { get; set; }
        public string? Institute { get; set; }
        public string? GCGPA { get; set; }
        public string? EduDoc { get; set; }
        public int? EducationID { get; set; }
        public string? WDate { get; set; }
        public string? Title { get; set; }
        public string? WorkRemark { get; set; }
        public string? WorkDoc { get; set; }
        public int? BasicSalary { get; set; }
        public int? HouseRent { get; set; }
        public string? Utility { get; set; }
        public string? Cola { get; set; }
        public string? EFFDate { get; set; }
        public string? EmpRemark { get; set; }
        public string? EmpDoc { get; set; }
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
        public int? EMP_ID { get; set; }
        public string? DLT { get; set; }
        public double? QTY { get; set; }
    }
}
