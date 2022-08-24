using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class ConsultationMedication
    {
        public ConsultationMedication()
        {
            PatientTempMedication = new HashSet<PatientTempMedication>();
        }

        public int Id { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string Medication { get; set; }
        public bool IsDeleted { get; set; }
        public int Priority { get; set; }
        public string AddedBy { get; set; }
        public DateTime? AddedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ConsultationCategory ConsultationCategory { get; set; }
        public virtual ICollection<PatientTempMedication> PatientTempMedication { get; set; }
    }
}
