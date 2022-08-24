using System;
using System.Collections.Generic;
using System.Text;
using TylerHendricks_Utility;

namespace TylerHendricks_Core.Models
{
    public class PaymentInfo
    {
        public string UserId { get; set; }
        public string PaymentType { get; set; }
        public string TxnID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; }
        public string FormatedDateString
        {
            get
            {
                return PaymentDate.To_MMddyyyy_WithSlash();
            }
        }
    }
}
