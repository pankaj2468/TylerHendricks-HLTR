using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class NotificationView
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Status { get; set; }
        public string TemplateName { get; set; } 
    }
}
