using System;

namespace TylerHendricks_Core.Models
{
    public class ConsultationView
    {
        public int ConsultationCategoryId { get; set; }
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsStarted { get; set; }
        public bool Status { get; set; }
        public DateTime? EnabledDate { get; set; }
        public int TimeZone { get; set; }
    }
}
