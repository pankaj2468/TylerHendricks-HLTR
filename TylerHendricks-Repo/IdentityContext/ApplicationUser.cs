using Microsoft.AspNetCore.Identity;
using System;

namespace TylerHendricks_Repo.IdentityContext
{
    public class ApplicationUser : IdentityUser
    {
        public bool? RecordStatus { set; get; }
        public bool? IsRecordDelete { set; get; }
        public string FirstName { set; get; }
        public string LastName { set; get; }
        public int StateId { set; get; }
        public bool IsAnswerComplete { set; get; }
        public string Photo { set; get; }
        public string PhotoId { set; get; }
        public DateTime? SelfieIdDate { get; set; }
        public DateTime? PhotoIdDate { get; set; }
        public bool? RetakeRequestPhotoId { get; set; }
        public bool? RetakeRequestSelfie { get; set; }
        public bool? WeekChat { get; set; }
        public DateTime? WeekChatEndDate { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public DateTime? Dob { get; set; }
        public string ZipCode { get; set; }
        public DateTime? AddedDate { get; set; }
        public int? ConsultationStateId { get; set; }
        public int? TimeZone { get; set; }
        public bool? IsDayLightSaving { get; set; }
    }
}
