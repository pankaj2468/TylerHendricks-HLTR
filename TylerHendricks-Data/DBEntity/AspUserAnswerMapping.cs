using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class AspUserAnswerMapping
    {
        public int Id { get; set; }
        public string ConsultationId { get; set; }
        public string UserId { get; set; }
        public string ConsultationQuestionsMappingId { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string Answer { get; set; }
        public bool? IsAction { get; set; }
        public bool IsCheckOut { get; set; }
        public bool IsActive { get; set; }
        public bool IsRecordDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
