using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class ShippingInformation
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ConsultationId { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductUnit { get; set; }
        public string ProductQuantity { get; set; }
        public string ProductDescription { get; set; }
        public int Refills { get; set; }
        public decimal ProductPrice { get; set; }
        public int OrderStatusTypeId { get; set; }
        public int Status { get; set; }
        public string PaymentStatus { get; set; }
        public string OrderId { get; set; }
        public string TxnId { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? PrescribedDate { get; set; }
        public bool? IsRecordDeleted { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
