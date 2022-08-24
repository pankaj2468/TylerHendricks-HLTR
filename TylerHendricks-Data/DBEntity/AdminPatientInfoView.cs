using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class AdminPatientInfoView
    {
        public string UserId { get; set; }
        public string ConsultationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PatientPhone { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyAddress1 { get; set; }
        public string PharmacyAddress2 { get; set; }
        public string PharmacyCity { get; set; }
        public string PharmacyState { get; set; }
        public string PharmacyZip { get; set; }
        public string PharmacyPhone { get; set; }
        public DateTime? RequestedDate { get; set; }
        public bool? IsMdtoolbox { get; set; }
        public bool IsCompleted { get; set; }
        public string Email { get; set; }
        public DateTime? AddedDate { get; set; }
    }
}
