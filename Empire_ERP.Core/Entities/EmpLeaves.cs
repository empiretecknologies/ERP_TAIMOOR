using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class EmpLeaves
    {
        public List<LeavesRecords>? Master { get; set; }
    }
    public class LeavesRecords
    {
        public int? DT_CODE { get; set; }              
        public DateTime? Date { get; set; }         
        public int? LEAVE_TYPE { get; set; }        
        public DateTime? LFrom { get; set; }          
        public DateTime? LTo { get; set; }            
        public int? NOL { get; set; }                  
        public int? Emp_ID { get; set; }                  
        public string? PURPOSE { get; set; } 
        public string? DOC { get; set; } 
    }
}
