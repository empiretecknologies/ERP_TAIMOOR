using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Empire_ERP.Core.Entities
{
    public class ExpensePOSTransaction
    {
        public ExpenseRecord? Master { get; set; }

    }
    public class ExpenseRecord 
    {
        public decimal? TRAN_ID { get; set; }
        public string? EXPDATE { get; set; }
        public int? ExpAmount { get; set; }
        public string? Descr { get; set; }
        public string? VOUCHER_NO { get; set; }
        public int? BOOK_TYPE { get; set; }
        public int? ACT_CODE { get; set; }
    }
    public class ExpensePrint 
    {
        public int? TRAN_ID { get; set; }
        public int? Amount { get; set; }
        public string? Descr { get; set; }
        public string? VOUCHER_NO { get; set; }
        public int? BOOK_TYPE { get; set; }
        public int? ACT_CODE { get; set; }
    }
}
