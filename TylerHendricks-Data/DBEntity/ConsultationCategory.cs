using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class ConsultationCategory
    {
        public ConsultationCategory()
        {
            Consultation = new HashSet<Consultation>();
            ConsultationCategoryDetail = new HashSet<ConsultationCategoryDetail>();
            ConsultationDetail = new HashSet<ConsultationDetail>();
            ConsultationMedication = new HashSet<ConsultationMedication>();
            PatientConsultation = new HashSet<PatientConsultation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool IsActived { get; set; }
        public bool IsRecordDeleted { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<Consultation> Consultation { get; set; }
        public virtual ICollection<ConsultationCategoryDetail> ConsultationCategoryDetail { get; set; }
        public virtual ICollection<ConsultationDetail> ConsultationDetail { get; set; }
        public virtual ICollection<ConsultationMedication> ConsultationMedication { get; set; }
        public virtual ICollection<PatientConsultation> PatientConsultation { get; set; }
    }
}
