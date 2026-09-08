using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class CommList
    {

        public int? GROUP_CODE { get; set; }
        public int? DT_CODE { get; set; }
        public int? SALESMAN { get; set; }
        public int? PARTY { get; set; }

        public int? SACT_CODE { get; set; }
        public int? ACT_CODE { get; set; }
        public int? ITEM_CODE { get; set; }
        public string? RATE { get; set; }
        public string? DISC { get; set; }
        public string? ADV { get; set; }

        public string? COMM_UNIT { get; set; }
        public int? COMM_VALUE { get; set; }
        public string? ASTATUS { get; set; }

    }
}
