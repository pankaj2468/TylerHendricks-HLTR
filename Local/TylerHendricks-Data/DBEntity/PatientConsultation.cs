using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class PatientConsultation
    {
        public int Id { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string SenderId { get; set; }
        public string Message { get; set; }
        public string ReceiverId { get; set; }
        public string Reply { get; set; }
        public string Attachment { get; set; }
        public bool IsRecordDeleted { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ConsultationCategory ConsultationCategory { get; set; }
    }
}
