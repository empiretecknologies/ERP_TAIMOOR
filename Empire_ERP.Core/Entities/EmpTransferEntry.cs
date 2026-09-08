using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class EmpTransferEntry
    {
        public List<TransferEntryRecords>? Master { get; set; }
    }
    public class TransferEntryRecords
    {             
        public int? GROUP_CODE { get; set; }
        public int? EMP_ID { get; set; }
        public DateTime? Date { get; set; }
        public int? BRANCH_FROM { get; set; } 
        public int? BRANCH_TO { get; set; } 
        public string? REPORT_TO { get; set; } 
        public string? DOC { get; set; } 
        public string? ASTATUS { get; set; } 
    }
}
