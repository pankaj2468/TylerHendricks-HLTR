using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class NotifyPatients
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int? StateId { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public bool? IsRecordDelete { get; set; }

        public virtual FacilityStates State { get; set; }
    }
}
