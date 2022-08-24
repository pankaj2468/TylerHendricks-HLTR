using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class EmailCongiurationModel
    {
        public int Port { get; set; }
        public string FromMailSender { get; set; }
        public string FromMailPassword { get; set; }
        public string SmtpClient { get; set; }
        public string From { get; set; }
    }
}
