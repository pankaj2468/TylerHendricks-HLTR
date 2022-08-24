using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class ConsultationConfig
    {
        public int ConsultationCategoryId { get; set; }
        public string ConsultationQuestionsMappingId { get; set; }
        public int FirstQuestionId { get; set; }
        public int LastQuestionId { get; set; }
    }
}