using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class Consultation
    {
        public Consultation()
        {
            PatientTempDetail = new HashSet<PatientTempDetail>();
            PatientTempMedication = new HashSet<PatientTempMedication>();
        }

        public int Id { get; set; }
        public string ConsultationId { get; set; }
        public string UserId { get; set; }
        public int ConsultationCategoryId { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? NextRefillDate { get; set; }
        public bool? IsMdtoolbox { get; set; }
        public bool IsRecordDeleted { get; set; }
        public bool? IsEmailSend { get; set; }
        public bool? RxRequest { get; set; }
        public DateTime? RxRequestUpdateDate { get; set; }
        public bool? IsHomeDelivery { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ConsultationCategory ConsultationCategory { get; set; }
        public virtual AspNetUsers User { get; set; }
        public virtual ICollection<PatientTempDetail> PatientTempDetail { get; set; }
        public virtual ICollection<PatientTempMedication> PatientTempMedication { get; set; }
    }
}
