using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class UserChat
    {
        public int Id { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public DateTime SendingDate { get; set; }
        public DateTime? ReplyDate { get; set; }
        public string Message { get; set; }
        public string Reply { get; set; }
        public string SenderName { get; set; }
        public string ReceiverName { get; set; }
        public string Attachment { get; set; }
    }
}
