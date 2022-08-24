using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class PharmacyInformation
    {
        public int PharmacyId { get; set; }
        public string UserId { get; set; }
        public string ConsultationId { get; set; }
        public string PharmacyName { get; set; }
        public string PhoneNumber { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public string ZipCode { get; set; }
        public bool IsRecordDeleted { get; set; }
        public DateTime? AddedDate { get; set; }
        public string AddedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
    }
}
