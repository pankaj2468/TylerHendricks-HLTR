using System;
using System.Collections.Generic;

namespace TylerHendricks_Data.DBEntity
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            AspNetUserRoles = new HashSet<AspNetUserRoles>();
            AspNetUserTokens = new HashSet<AspNetUserTokens>();
            Consultation = new HashSet<Consultation>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string NormalizedUserName { get; set; }
        public string Email { get; set; }
        public string NormalizedEmail { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string ConcurrencyStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public bool? RecordStatus { get; set; }
        public DateTime? AddedDate { get; set; }
        public bool? IsRecordDelete { get; set; }
        public int? ConsultationStateId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool? IsAnswerComplete { get; set; }
        public int StateId { get; set; }
        public DateTime? Dob { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Photo { get; set; }
        public string PhotoId { get; set; }
        public bool? RetakeRequestSelfie { get; set; }
        public bool? RetakeRequestPhotoId { get; set; }
        public bool? WeekChat { get; set; }
        public DateTime? WeekChatEndDate { get; set; }
        public DateTime? PhotoIdDate { get; set; }
        public DateTime? SelfieIdDate { get; set; }
        public int? TimeZone { get; set; }
        public int RowId { get; set; }
        public bool? IsDayLightSaving { get; set; }

        public virtual FacilityStates State { get; set; }
        public virtual ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual ICollection<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual ICollection<Consultation> Consultation { get; set; }
    }
}
