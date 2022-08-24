using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class PatientSqlChart
    {
        [Key]
        public long RowNo { get; set; }
        public int RowId { get; set; }
        public string UserId { get; set; }
        public string ConsultationId { get; set; }
        public int ConsultationCategoryId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DOB { get; set; }
        public int Age { get; set; }
        public string StateCode { get; set; }
        public int StateId { get; set; }
        public string Submitted { get; set; }
        public string LastOrder { get; set; }
        public string LastRx { get; set; }
        public string LastProvider { get; set; }
        public bool RecordStatus { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyPhoneNumber { get; set; }
        public string Address { get; set; }
        public string PharmacyZipCode { get; set; }
        public string PharmacyCity { get; set; }
        public string PharmacyState { get; set; }
        public string Selfie { get; set; }
        public string RequestedRx { get; set; }
        public string PhotoId { get; set; }
        public string MedicineFile1 { get; set; }
        public string MedicineFile2 { get; set; }
        public string MedicineFile3 { get; set; }
        public int OrderStatusTypeId { get; set; }
        public int Status { get; set; }
        public DateTime? PhotoIdDate { get; set; }
        public DateTime? SelfieIdDate { get; set; }
        public DateTime? MedicationIdDate { get; set; }
        public int Refills { get; set; }
        public string HomeAddress { get; set; }

      
    }
}
