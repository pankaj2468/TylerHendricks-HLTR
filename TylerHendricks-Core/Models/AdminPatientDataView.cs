using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TylerHendricks_Utility;

namespace TylerHendricks_Core.Models
{
    public class AdminPatientDataView
    {
        public int TotalRecords { get; set; }
        public string UserId { get; set; }
        [Key]
        public string ConsultationId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string RegisteredDate { get; set; }
        public string SubmittedDate { get; set; }
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
        public bool? IsMDToolbox { get; set; }
        public decimal ProductPrice { get; set; }

        public string FormatDobString
        {
            get 
            { 
                return DateOfBirth.To_Mdyyyy_WithSlash();
            }
        }
    }
}
