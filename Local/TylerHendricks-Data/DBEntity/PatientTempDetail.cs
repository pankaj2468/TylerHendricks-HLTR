using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class PatientTempDetail
    {
        public int Id { get; set; }
        public string ConsultationId { get; set; }
        public int ConsultationDetailId { get; set; }
        public bool IsRecordDeleted { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual Consultation Consultation { get; set; }
        public virtual ConsultationCategoryDetail ConsultationDetail { get; set; }
    }
}
