using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class EmailSendingLog
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public bool? IsRecordDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}
