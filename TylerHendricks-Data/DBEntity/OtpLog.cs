using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class OtpLog
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Otp { get; set; }
        public bool? IsUsed { get; set; }
        public DateTime ValidDate { get; set; }
        public long ValidTimeInMinute { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsRecordDeleted { get; set; }
    }
}
