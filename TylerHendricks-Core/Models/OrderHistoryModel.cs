using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class OrderHistoryModel
    {
        [Key]
        public string ConsultationId { get; set; }
        public string Request { get; set; }
        public string Status { get; set; }
        public string RequestedDate { get; set; }
        public string PrescribeDate { get; set; }
        public int Refills { get; set; }
        public decimal Payments { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyPhone { get; set; }
        public string PharmacyAddress { get; set; }
    }
}
