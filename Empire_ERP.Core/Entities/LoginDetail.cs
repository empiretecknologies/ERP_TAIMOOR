using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class LoginDetail
    {
        public class CompanyModel
        {
            public int CCODE { get; set; }
            public string C_NAME { get; set; }
        }

        public class BranchModel
        {
            public int BCODE { get; set; }
            public string B_NAME { get; set; }
        }

        public class PeriodModel
        {
            public int PID { get; set; }
            public string DESCR { get; set; }
        }

    }
}
