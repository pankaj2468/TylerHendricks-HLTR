using System.Collections.Generic;
using TylerHendricks_Core.Models;

namespace TylerHendricks_Core.ViewModel
{
    public class PatientChartViewModel
    {
        public long RowNo { get; set; }
        public int RowId { get; set; }
        public int TotalRecord { get; set; }
        public int RecordType { get; set; }
        public string PatientId { get; set; }
        public string ConsultationId { get; set; }
        public string PatientName { get; set; }
        public string PatientDob { get; set; }
        public string PatientEmail { get; set; }
        public string PatientPhoneNumber { get; set; }
        public int PatientAge { get; set; }
        public string Submitted { get; set; }
        public string StateCode { get; set; }
        public string LastOrder { get; set; }
        public string LastRx { get; set; }
        public string LastProvider { get; set; }
        public bool PaitentAccount { get; set; }
        public string PharmacyName { get; set; }
        public string PharmacyAddressLine1 { get; set; }
        public string PharmacyAddressLine2 { get; set; }
        public string PharmacyCity { get; set; }
        public string PharmacyState { get; set; }
        public string PharmacyPhone { get; set; }
        public string PharmacyZipCode { get; set; }
        public string RequestedRx { get; set; }
        public int Refills { get; set; }
        public int Status { get; set; }
        public IList<string> Medications { get; set; }
        public string PhotoId { get; set; }
        public string PhotoIdUploadDate { get; set; }
        public string Selfie { get; set; }
        public string SelfieUploadDate { get; set; }
        public IList<string> MedicationPictures { get; set; }
        public string MedicationUploadDate { get; set; }
        public string HomeAddress { get; set; }
        public bool IsChatEnabled { get; set; }
        public IList<QuestionnaireHistory> QuestionnaireHistories { get; set; }
        public IList<ConsultationList> ConsultationLists { get; set; }
        public IList<UserChat> UserChats { get; set; }
        public IList<Notes> Notes { get; set; }
        public UpdatePatientView UpdatePatientView { get; set; }
        public UpdatePharmacyView UpdatePharmacyView { get; set; }
        
    }
}
