using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class Questions
    {
        public int? ConsultationCategoryId { get; set; }
        public int Id { get; set; }
        public string Description { get; set; }
        public bool? Response { get; set; }
        public int? NextQuestionId { get; set; }
        public int? PreviousQuestionId { get; set; }
        public int? NextQuestionId1 { get; set; }
        public  int ? ModalPopupId { get; set; }
        public int ? ModalPopupId1 { get; set; }
        public string PopText { get; set; }
        public string PopText1 { get; set; }
        public bool? IsNote { get; set; }
        public string Answer { get; set; }
    }
}

