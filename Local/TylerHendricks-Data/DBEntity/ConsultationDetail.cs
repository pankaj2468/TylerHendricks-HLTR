using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class ConsultationDetail
    {
        public int Id { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string Title { get; set; }
        public string Medication { get; set; }
        public int Refill { get; set; }
        public int RefillQuantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public bool IsHomeDelivery { get; set; }
        public bool IsDeleted { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ConsultationCategory ConsultationCategory { get; set; }
    }
}
