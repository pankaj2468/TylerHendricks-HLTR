using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class PaymentDetails
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ConsultationId { get; set; }
        public string PaymentType { get; set; }
        public string InvoiceNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Product { get; set; }
        public decimal Amount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public string Status { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
