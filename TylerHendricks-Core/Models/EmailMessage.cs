using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class EmailMessage
    {
        public List<string> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
    }
}
