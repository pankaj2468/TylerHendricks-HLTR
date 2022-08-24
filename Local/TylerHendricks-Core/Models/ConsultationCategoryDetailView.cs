using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class ConsultationCategoryDetailView
    {
        public int Id { get; set; }
        public int ConsultationCategoryId { get; set; }
        public int Refill { get; set; }
        public int RefillDay { get; set; }
        public decimal MedicationRate{ get; set; }
        public bool IsHomeDelivery { get; set; }
    }
}
