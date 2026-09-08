using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class EmpPenalty
    {
        public List<PenaltyRecords>? Master { get; set; }
    }
    public class PenaltyRecords
    {
        public int? DT_CODE { get; set; }              
        public DateTime? date { get; set; }         
        public string? penalty { get; set; }           
        public string? reason { get; set; }                  
        public int? pAmt { get; set; }                  
        public string? Emp_ID { get; set; } 
        public string? DOC { get; set; } 
    }
}
