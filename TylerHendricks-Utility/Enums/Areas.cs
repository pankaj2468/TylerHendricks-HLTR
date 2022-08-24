using System;
using System.Collections.Generic;
using System.Text;

namespace TylerHendricks_Utility.Enums
{
    public static class Areas
    {
        public enum PortalType
        {
            PatientPortal = 0,
            PhysicianPortal = 1,
            LogException = 2,
            AdminPortal = 3,
        }
        public enum AlertType
        {
            ForgotPassword = 1,
            ResetPassword = 2
        }
        public enum DocumentType
        {
            PrivacyPolicy = 1,
            TermsAndConditions = 2
        }
        public enum MedicationType
        {
            ChooseMedication = 1
        }
        public enum LoginType
        {
            WrongPassword = 1,
            NotExists = 2,
            Success = 3
        }
        public enum RoleType
        {
            Admin = 1,
            Patient = 2,
            Physician = 3
        }
        public enum RecordType
        {
            ReadyToPrescribe = 1,
            FurtherReviewNeeded = 2,
            PendingResponse = 3,
            AllPatients = 4,
            Denied = 5,
            PotientialsDuplicates = 6,
            Responded = 7,
        }

        public enum EmailTemplate
        {
            Confirmation = 1,
            ResetPassword = 2,
            LeftConsultation = 3,
            NewMessage = 4,
            OTP = 5,
            PatientDenied = 6,
            PatientPrescribed = 7
        }
        public enum ConsultationType
        {
            ErectileDysfunction = 1,
            MedicationRefill = 2,
            HairLoss = 3
        }
        public enum PhotoType
        {
            Selfie = 1,
            PhotoId = 2,
            Medicine = 3
        }
        public enum ConsultationLastQuestion
        {
            ErectileDysfunction = 35,
            MedicationRefill = 42,
            HairLoss = 59
        }
        public enum SessionKey
        {
            SessionManager = 1,
            QuestionNaire = 2,
            TimeZone=3
        }
        public enum CurrencyType
        {
            USD = 1
        }
        public enum ControllerType
        {
            Home = 0,
            Patient = 1,
        }
    }
}
