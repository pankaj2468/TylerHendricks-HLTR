using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class ConsultationModel
    {
        public int ConsultationId { get; set; }
        public string ConsultationName { get; set; }
        public DateTime ConsultationSubmissionDate { get; set; }
        public string RequestedRx { get; set; }
        public string Status { get; set; }
        public bool Active { get; set; }
    }
}
