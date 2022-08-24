using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Core.Models
{
    public class ResourcesConfig
    {
        public string ErrorMessageForEmail { get; set; }
        public string ErrorMessageForState { get; set; }
        public string ErrorMessageForUsername { get; set; }
        public string ErrorMessageForUserExist { get; set; }
        public string ForgotEmailSubject { get; set; }
        public string OTPEmailSubject { get; set; }
        public string ConfirmationEmailSubject { get; set; }
        public string PrescribedEmailSubject { get; set; }
        public string ConsultationUpdateEmailSubject { get; set; }
        public string ResetPasswordEmailSubject { get; set; }
        public string MessageEmailSubject { get; set; }
        public int ExistConsultationQuestionId { get; set; }
        public string WeekChatAmount { get; set; }
    }
}
