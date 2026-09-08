using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class CopyRecord
    {
        public int TRAN_ID { get; set; }
        public DateTime V_DATE { get; set; }
        public string ACT_NAME { get; set; }
        public string BATCH_NAME { get; set; }
        public string PARTY_NAME{ get; set; }
        public string PARTY_CODE { get; set; }
        public int FINISH_ITEM { get; set; }
        public int PROCESS { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string ITEM_NAME { get; set; }

        public string CLIENT_PO { get; set; }
    }
}