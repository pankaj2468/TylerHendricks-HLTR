using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Data.DBEntity;
using static TylerHendricks_Utility.Enums.Areas;

namespace TylerHendricks_Repo.Contracts
{
    public interface IUsers
    {
        Task<CrudResult> SignUpUsers(Signup userDetail);
        Task<CrudResult> SignOut();
        Task<LoginType> SignIn(LoginModel login);
        Task<PasswordToken> ResetPasswordToken(ForgotModel resetPassword);
        Task<IdentityResult> ResetPassword(ResetPasswordModel restPassModel);
        Task<List<string>> GetCurrentUserRoleByUserId(string userId);
        Task<List<string>> GetCurrentUserRoleByEmail(string email);
        Task<List<string>> GetCurrentUserRoleByName(string name);
        Task<AspNetUsers> GetUserDetails(string UserId);
        Task<IdentityResult> ChangePassword(ChangedPassword changePassModel, string userId);
        Task<int> GetStateId(string userId);
        Task<string> GetUserId(string email);
        Task<string> GetName(string username);
        Task<string> GetFirstName(string username);
        Task<string> GetEmail(string userId);
        Task<CrudResult> VerifyOTP(string userId, string OTP, string email);
        Task<CrudResult> SavePhotoIdImage(string selfie, string photoId, string userId);
        Task<CrudResult> SaveRetakePhotoId(string fileName, string userId);
        Task<CrudResult> SaveRetakeSelfie(string fileName, string userId);
        Task<CrudResult> UpdateWeekChatStatus(string userId, DateTime paymentDate);
        Task<CrudResult> GotoPrevious(string userId);
        Task<CrudResult> RetakePhotoIdRequest(string patientId, string userId, string consultationId, string message);
        Task<CrudResult> RetakeSelfieRequest(string patientId, string userId, string consultationId, string message);
        Task<CrudResult> RetakeMedicineImageRequest(string patientId, string userId, string consultationId, string message);
        Task<CrudResult> AccountStatus(string patientId);
        Task<CrudResult> UpdateAccountInfo(FinishAccountSetupModel finishAccountSetupModel, string userId);
        Task<CrudResult> UpdatePatientInfo(UpdatePatientView patientView);
        Task<TimeZoneConfig> GetTimeZone(string userId);
    }
}
