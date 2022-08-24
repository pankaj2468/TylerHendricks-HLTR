using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class UsersAddress
    {
        public int AddressId { get; set; }
        public string ConsultationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Dob { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateId { get; set; }
        public bool? IsRecordDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
