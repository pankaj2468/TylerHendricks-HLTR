using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class ConsultationCategoryDetail
    {
        public ConsultationCategoryDetail()
        {
            PatientTempDetail = new HashSet<PatientTempDetail>();
        }

        public int Id { get; set; }
        public int ConsultationCategoryId { get; set; }
        public int Refill { get; set; }
        public int RefillDay { get; set; }
        public decimal MedicationRate { get; set; }
        public bool IsHomeDelivery { get; set; }
        public bool IsDeleted { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ConsultationCategory ConsultationCategory { get; set; }
        public virtual ICollection<PatientTempDetail> PatientTempDetail { get; set; }
    }
}
