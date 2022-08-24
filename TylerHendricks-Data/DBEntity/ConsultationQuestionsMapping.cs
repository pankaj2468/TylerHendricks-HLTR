using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class ConsultationQuestionsMapping
    {
        public int Id { get; set; }
        public int? Questionid { get; set; }
        public bool? Response { get; set; }
        public int? NextQuestion { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsRecordDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool? IsNote { get; set; }
        public int? ModalPopupId { get; set; }

        public virtual ConsultationQuestions NextQuestionNavigation { get; set; }
        public virtual ConsultationQuestions Question { get; set; }
    }
}
