using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class FacilityStates
    {
        public FacilityStates()
        {
            AspNetUsers = new HashSet<AspNetUsers>();
            NotifyPatients = new HashSet<NotifyPatients>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool? IsActive { get; set; }
        public string TimeZone { get; set; }
        public bool? IsRecordDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }

        public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
        public virtual ICollection<NotifyPatients> NotifyPatients { get; set; }
    }
}
