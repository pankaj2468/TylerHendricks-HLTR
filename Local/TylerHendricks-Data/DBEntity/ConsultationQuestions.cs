using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class ConsultationQuestions
    {
        public ConsultationQuestions()
        {
            ConsultationQuestionsMappingNextQuestionNavigation = new HashSet<ConsultationQuestionsMapping>();
            ConsultationQuestionsMappingQuestion = new HashSet<ConsultationQuestionsMapping>();
        }

        public int Id { get; set; }
        public string QuestionDescription { get; set; }
        public bool? IsPopup { get; set; }
        public bool IsRecordDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int ConsultationCategoryId { get; set; }
        public bool IsStart { get; set; }
        public bool IsEndConsultation { get; set; }

        public virtual ICollection<ConsultationQuestionsMapping> ConsultationQuestionsMappingNextQuestionNavigation { get; set; }
        public virtual ICollection<ConsultationQuestionsMapping> ConsultationQuestionsMappingQuestion { get; set; }
    }
}
