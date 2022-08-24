using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Data.DBEntity;
using TylerHendricks_Repo.Contracts;
using TylerHendricks_Repo.IdentityContext;
using static TylerHendricks_Utility.Enums.Areas;

namespace TylerHendricks_Repo.Services
{
    public class Users : Repository, IUsers
    {
        #region Private readonly field
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        #endregion
        public Users(UserManager<ApplicationUser> userManger, SignInManager<ApplicationUser> signInManager)
        {
            _userManger = userManger;
            _signInManager = signInManager;
        }
        
        public async Task<CrudResult> SignOut()
        {
            await _signInManager.SignOutAsync().ConfigureAwait(true);
            return new CrudResult() { ErroCode = "No Erro", ErrorMassage = "N/A", Failed = false, Succeeded = true };
        }
        public async Task<CrudResult> SignUpUsers(Signup userDetail)
        {
            using var transaction = DBEntity.Database.BeginTransaction();
            try
            {
                if (userDetail != null)
                {
                    var user = new ApplicationUser()
                    {
                        FirstName = "n/a",
                        LastName = "n/a",
                        Email = userDetail.Email,
                        UserName = userDetail.Email,
                        PhoneNumber = "000-000-0000",
                        RecordStatus = true,
                        IsRecordDelete = false,
                        StateId = userDetail.StateId,
                        ConsultationStateId = userDetail.StateId,
                        IsAnswerComplete = false,
                        AddedDate = DateTime.UtcNow,
                        TimeZone = userDetail.TimezoneOffSet,
                        IsDayLightSaving = false
                    };
                    var isUserAdd = await _userManger.CreateAsync(user, userDetail.ConfirmPassword).ConfigureAwait(true);
                    if (isUserAdd.Succeeded)
                    {
                        IdentityResult IsRoleAdd = await _userManger.AddToRoleAsync(user, userDetail.Role).ConfigureAwait(true);
                        var currentUser = await _userManger.FindByEmailAsync(userDetail.Email).ConfigureAwait(true);
                        var entityEntry = await DBEntity.Consultation.AddAsync(new Consultation()
                        {
                            AddedBy = currentUser.Id,
                            AddedDate = DateTime.UtcNow,
                            ConsultationCategoryId = userDetail.ConsultationCategoryId,
                            ConsultationId = userDetail.ConsultationId,
                            IsCompleted = false,
                            IsStarted = true,
                            IsRecordDeleted = false,
                            UserId = currentUser.Id
                        }).ConfigureAwait(true);

                        var prevQuestion = await (from p in DBEntity.AspUserAnswerMapping
                                                  where p.IsRecordDeleted == false && p.IsActive == true && p.ConsultationId == userDetail.ConsultationId
                                                  && p.ConsultationCategoryId == userDetail.ConsultationCategoryId
                                                  select p).ToListAsync().ConfigureAwait(true);

                        prevQuestion.ForEach(x => { x.UserId = currentUser.Id; });

                        foreach (var item in prevQuestion)
                        {
                            DBEntity.AspUserAnswerMapping.Update(item).Property(x=>x.Id).IsModified=false;
                        }

                        DBEntity.SaveChanges();

                        transaction.Commit();

                        if (IsRoleAdd.Succeeded)
                        {

                            SignInResult IsSign = await _signInManager.PasswordSignInAsync(userDetail.Email, userDetail.ConfirmPassword, false, false).ConfigureAwait(true);
                            return (IsSign.Succeeded) ? new CrudResult() { ErroCode = "no Error", ErrorMassage = "no Error", Failed = false, Succeeded = true }
                            : new CrudResult() { ErroCode = "Error in SignIn", ErrorMassage = "Error in SignIn", Failed = true, Succeeded = false };
                        }
                        else
                        {
                            string message = string.Empty;
                            string Code = string.Empty;
                            foreach (var item in IsRoleAdd.Errors)
                            {
                                Code = item.Code;
                                message = item.Description;
                            }
                            return new CrudResult() { ErroCode = Code, ErrorMassage = message, Failed = true, Succeeded = false };
                        }
                    }
                    else
                    {
                        string message = string.Empty;
                        string Code = string.Empty;
                        foreach (var item in isUserAdd.Errors)
                        {
                            Code = item.Code;
                            message = item.Description;
                        }
                        return new CrudResult() { ErroCode = Code, ErrorMassage = message, Failed = true, Succeeded = false };
                    }
                }
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = true, Succeeded = false };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new CrudResult() { ErroCode = ex.Message, ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<LoginType> SignIn(LoginModel login)
        {
            if (login != null)
            {
                var user = await _userManger.FindByEmailAsync(login.Email).ConfigureAwait(true);
                if (user != null)
                {
                    var roleType = await GetCurrentUserRoleByEmail(login.Email).ConfigureAwait(true);
                    if (roleType.Contains(login.Role))
                    {
                        if (user.RecordStatus == true && user.IsRecordDelete == false)
                        {
                            var passwordOK = await _userManger.CheckPasswordAsync(user, login.Password).ConfigureAwait(true);
                            if (!passwordOK)
                            {
                                return LoginType.WrongPassword;
                            }
                            else
                            {
                                var signInOk = await _signInManager.PasswordSignInAsync(login.Email, login.Password, false, false).ConfigureAwait(true);
                                if (signInOk.Succeeded)
                                {
                                    user.TimeZone = login.TimezoneOffSet;
                                    user.IsDayLightSaving = Convert.ToBoolean(string.IsNullOrEmpty(login.IsDayLightSaving) ? "false": login.IsDayLightSaving);
                                    await _userManger.UpdateAsync(user).ConfigureAwait(true);
                                    return LoginType.Success;
                                }
                                else
                                {
                                    return LoginType.WrongPassword;
                                }
                            }
                        }
                        else
                        {
                            return LoginType.NotExists;
                        }
                    }
                    else
                    {
                        return LoginType.NotExists;
                    }
                }
                else
                {
                    return LoginType.NotExists;
                }
            }
            return LoginType.NotExists;
        }
        public async Task<PasswordToken> ResetPasswordToken(ForgotModel resetPassword)
        {
            if (resetPassword != null)
            {
                var user = await _userManger.FindByEmailAsync(resetPassword.Email).ConfigureAwait(true);
                if (user != null)
                {
                    var roleType = await GetCurrentUserRoleByEmail(resetPassword.Email).ConfigureAwait(true);
                    if (roleType.Contains(resetPassword.Role))
                    {
                        var token = await _userManger.GeneratePasswordResetTokenAsync(user).ConfigureAwait(true);
                        return new PasswordToken() { Id = user.Id, Token = token };
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            return null;
        }
        public async Task<IdentityResult> ResetPassword(ResetPasswordModel restPassModel)
        {
            IdentityResult identityResult=new IdentityResult();
            if (restPassModel != null)
            {
                var user = await _userManger.FindByIdAsync(restPassModel.TokenId).ConfigureAwait(true);
                identityResult= await _userManger.ResetPasswordAsync(user, restPassModel.Token, restPassModel.ConfirmChnagePass).ConfigureAwait(true);
            }
            return identityResult;
        }
        public async Task<List<string>> GetCurrentUserRoleByUserId(string userId)
        {
            var roleList = new List<string>();
            var currentUser = await _userManger.FindByIdAsync(userId).ConfigureAwait(true);
            if (await _userManger.IsInRoleAsync(currentUser, RoleType.Admin.ToString()).ConfigureAwait(true))
            {
                roleList.Add(RoleType.Admin.ToString());
            }
            if (await _userManger.IsInRoleAsync(currentUser, RoleType.Patient.ToString()).ConfigureAwait(true))
            {
                roleList.Add(RoleType.Patient.ToString());
            }
            if (await _userManger.IsInRoleAsync(currentUser, RoleType.Physician.ToString()).ConfigureAwait(true))
            {
                roleList.Add(RoleType.Physician.ToString());
            }
            return roleList;
        }
        public async Task<List<string>> GetCurrentUserRoleByEmail(string email)
        {
            var roleList = new List<string>();
            var currentUser = await _userManger.FindByEmailAsync(email).ConfigureAwait(true);
            if (await _userManger.IsInRoleAsync(currentUser, RoleType.Admin.ToString()).ConfigureAwait(true))
            {
                roleList.Add(RoleType.Admin.ToString());
            }
            if (await _userManger.IsInRoleAsync(currentUser, RoleType.Patient.ToString()).ConfigureAwait(true))
            {
                roleList.Add(RoleType.Patient.ToString());
            }
            if (await _userManger.IsInRoleAsync(currentUser, RoleType.Physician.ToString()).ConfigureAwait(true))
            {
                roleList.Add(RoleType.Physician.ToString());
            }
            return roleList;
        }
        public async Task<List<string>> GetCurrentUserRoleByName(string name)
        {
            var roleList = new List<string>();
            var currentUser = await _userManger.FindByNameAsync(name).ConfigureAwait(true);
            if (await _userManger.IsInRoleAsync(currentUser, RoleType.Admin.ToString()).ConfigureAwait(true))
            {
                roleList.Add(RoleType.Admin.ToString());
            }
            if (await _userManger.IsInRoleAsync(currentUser, RoleType.Patient.ToString()).ConfigureAwait(true))
            {
                roleList.Add(RoleType.Patient.ToString());
            }
            if (await _userManger.IsInRoleAsync(currentUser, RoleType.Physician.ToString()).ConfigureAwait(true))
            {
                roleList.Add(RoleType.Physician.ToString());
            }
            return roleList;
        }
        public async Task<AspNetUsers> GetUserDetails(string userId)
        {
            var _user = await (from u in DBEntity.AspNetUsers
                               where u.IsRecordDelete == false && u.Id == userId
                               select u).FirstOrDefaultAsync().ConfigureAwait(true);
            return _user;
        }
        public async Task<IdentityResult> ChangePassword(ChangedPassword changePassModel, string userId)
        {
            IdentityResult identityResult = new IdentityResult();
            if (changePassModel != null)
            {
                var user = await _userManger.FindByIdAsync(userId).ConfigureAwait(true);
                return await _userManger.ChangePasswordAsync(user, changePassModel.CurrentPassword, changePassModel.ConfirmChangePassword).ConfigureAwait(true);
            }
            return identityResult;
        }
        public async Task<int> GetStateId(string userId)
        {
            int stateid = await
                (from a in DBEntity.AspNetUsers where a.IsRecordDelete == false && a.Id == userId select a.StateId).FirstOrDefaultAsync().ConfigureAwait(true);
            return stateid;
        }
        public async Task<string> GetUserId(string email)
        {
            return await (from u in DBEntity.AspNetUsers where u.Email == email select u.Id).FirstOrDefaultAsync().ConfigureAwait(true);
        }
        public async Task<string> GetName(string username)
        {
            return await (from u in DBEntity.AspNetUsers
                          where u.IsRecordDelete == false && u.UserName == username
                          select u.FirstName + " " + u.LastName).FirstOrDefaultAsync().ConfigureAwait(true);
        }
        public async Task<string> GetFirstName(string username)
        {
            return await (from u in DBEntity.AspNetUsers
                          where u.IsRecordDelete == false && u.UserName == username
                          select u.FirstName).FirstOrDefaultAsync().ConfigureAwait(true);
        }
        public async Task<string> GetEmail(string userId)
        {
            var _user = await (from u in DBEntity.AspNetUsers where u.Id == userId select u.Email).FirstOrDefaultAsync().ConfigureAwait(true);
            return _user;
        }
        public async Task<CrudResult> VerifyOTP(string userId, string OTP, string email)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var prevRecord = await (from s in DBEntity.OtpLog where s.IsRecordDeleted == false && s.Otp == OTP && s.UserId == userId && s.IsUsed == false && s.ValidDate >= DateTime.UtcNow select s).FirstOrDefaultAsync().ConfigureAwait(true);
                if (prevRecord != null)
                {
                    prevRecord.IsUsed = true;
                    prevRecord.ModifiedDate = DateTime.UtcNow;
                    DBEntity.OtpLog.Update(prevRecord).Property(x => x.Id).IsModified = false;
                    var user = await _userManger.FindByIdAsync(userId).ConfigureAwait(true);
                    if (user != null)
                    {
                        await _userManger.SetEmailAsync(user, email).ConfigureAwait(true);
                        await _userManger.SetUserNameAsync(user, email).ConfigureAwait(true);
                    }
                    await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                    transcation.Commit();
                    return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
                }
                else
                {
                    transcation.Rollback();
                    return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = true, Succeeded = false };
                }
            }
            catch (Exception ex)
            {
                transcation.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> SavePhotoIdImage(string selfie,string photoId, string userId)
        {
            try
            {
                var user = await _userManger.FindByIdAsync(userId).ConfigureAwait(true);
                user.Photo = selfie;
                user.PhotoId = photoId;
                user.SelfieIdDate = DateTime.UtcNow;
                user.PhotoIdDate = DateTime.UtcNow;
                var identityResult = await _userManger.UpdateAsync(user).ConfigureAwait(true);
                if (identityResult.Succeeded)
                {
                    return new CrudResult() { ErroCode = "success", ErrorMassage = "success", Failed = false, Succeeded = true };
                }
                else
                {
                    return new CrudResult() { ErroCode = "error", ErrorMassage = "error", Failed = true, Succeeded = false };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> SaveRetakePhotoId(string fileName, string userId)
        {
            try
            {
                var user = await _userManger.FindByIdAsync(userId).ConfigureAwait(true);
                user.RetakeRequestPhotoId = false;
                user.PhotoIdDate = DateTime.UtcNow;
                user.PhotoId = fileName;
                var identityResult = await _userManger.UpdateAsync(user).ConfigureAwait(true);
                if (identityResult.Succeeded)
                {
                    return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
                }
                else
                {
                    return new CrudResult() { ErroCode = "", ErrorMassage = "failure", Failed = true, Succeeded = false };
                }
            }
            catch (Exception ex)
            {
                return new CrudResult() { InnerErrorMessage = ex.InnerException.Message, ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> SaveRetakeSelfie(string fileName, string userId)
        {
            try
            {
                var user = await _userManger.FindByIdAsync(userId).ConfigureAwait(true);
                user.RetakeRequestSelfie = false;
                user.SelfieIdDate = DateTime.UtcNow;
                user.Photo = fileName;
                var identityResult = await _userManger.UpdateAsync(user).ConfigureAwait(true);
                if (identityResult.Succeeded)
                {
                    return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
                }
                else
                {
                    return new CrudResult() { ErroCode = "", ErrorMassage = "failure", Failed = true, Succeeded = false };
                }
            }
            catch (Exception ex)
            {
                return new CrudResult() { InnerErrorMessage = ex.InnerException.Message, ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> UpdateWeekChatStatus(string userId, DateTime paymentDate)
        {
            try
            {
                var user = await _userManger.FindByIdAsync(userId).ConfigureAwait(true);
                user.WeekChat = true;
                user.WeekChatEndDate = paymentDate.AddDays(7);
                var identityResult = await _userManger.UpdateAsync(user).ConfigureAwait(true);
                if (identityResult.Succeeded)
                {
                    return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
                }
                else
                {
                    return new CrudResult() { ErroCode = "", ErrorMassage = "failure", Failed = true, Succeeded = false };
                }
            }
            catch (Exception ex)
            {
                return new CrudResult() { InnerErrorMessage = ex.InnerException.Message, ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> GotoPrevious(string userId)
        {
            try
            {
                var user = await _userManger.FindByIdAsync(userId).ConfigureAwait(true);
                user.Photo = string.Empty;
                user.PhotoId = string.Empty;
                var identityResult = await _userManger.UpdateAsync(user).ConfigureAwait(true);
                if (identityResult.Succeeded)
                {
                    return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
                }
                else
                {
                    return new CrudResult() { ErroCode = "", ErrorMassage = "failure", Failed = true, Succeeded = false };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> RetakePhotoIdRequest(string patientId, string userId, string consultationId, string message)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                int consultationCategoryId = 0;
                using (var Common = new Common())
                {
                    consultationCategoryId = await Common.GetConsultationCategory(consultationId).ConfigureAwait(true);
                };
                var user = await _userManger.FindByIdAsync(patientId).ConfigureAwait(true);
                user.RetakeRequestPhotoId = true;
                await _userManger.UpdateAsync(user).ConfigureAwait(true);
                DBEntity.PatientConsultation.Add(new PatientConsultation()
                {
                    AddedDate = DateTime.UtcNow,
                    IsRecordDeleted = false,
                    Message = message,
                    SenderId = userId,
                    ReceiverId = patientId,
                    ConsultationCategoryId = consultationCategoryId
                });
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception ex)
            {
                transcation.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> RetakeSelfieRequest(string patientId, string userId, string consultationId, string message)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                int consultationCategoryId = 0;
                using (var Common = new Common())
                {
                    consultationCategoryId = await Common.GetConsultationCategory(consultationId).ConfigureAwait(true);
                };
                var user = await _userManger.FindByIdAsync(patientId).ConfigureAwait(true);
                user.RetakeRequestSelfie = true;
                await _userManger.UpdateAsync(user).ConfigureAwait(true);

                DBEntity.PatientConsultation.Add(new PatientConsultation()
                {
                    AddedDate = DateTime.UtcNow,
                    IsRecordDeleted = false,
                    Message = message,
                    SenderId = userId,
                    ReceiverId = patientId,
                    ConsultationCategoryId= consultationCategoryId
                });

                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception ex)
            {
                transcation.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> RetakeMedicineImageRequest(string patientId, string userId,string consultationId, string message)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                int consultationCategoryId = 0;
                using (var Common = new Common())
                {
                    consultationCategoryId = await Common.GetConsultationCategory(consultationId).ConfigureAwait(true);
                };
                var consultation = await DBEntity.Consultation.Where(x => x.ConsultationId == consultationId).FirstOrDefaultAsync().ConfigureAwait(true);
                consultation.RxRequest = true;
                DBEntity.Consultation.Update(consultation).Property(x => x.Id).IsModified = false;
                DBEntity.PatientConsultation.Add(new PatientConsultation()
                {
                    AddedDate = DateTime.UtcNow,
                    IsRecordDeleted = false,
                    Message = message,
                    SenderId = userId,
                    ReceiverId = patientId,
                    ConsultationCategoryId= consultationCategoryId,
                });
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception ex)
            {
                transcation.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> AccountStatus(string patientId)
        {
            try
            {
                var user = await _userManger.FindByIdAsync(patientId).ConfigureAwait(true);
                if (user.RecordStatus == true)
                {
                    user.RecordStatus = false;
                }
                else
                {
                    user.RecordStatus = true;
                }
                await _userManger.UpdateAsync(user).ConfigureAwait(true);
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> UpdateAccountInfo(FinishAccountSetupModel finishAccountSetupModel, string userId)
        {
            try
            {
                if (finishAccountSetupModel != null)
                {
                    var user = await _userManger.FindByIdAsync(userId).ConfigureAwait(true);
                    user.FirstName = finishAccountSetupModel.FirstName;
                    user.LastName = finishAccountSetupModel.LastName;
                    user.AddressLine1 = finishAccountSetupModel.AddressLine1;
                    user.AddressLine2 = finishAccountSetupModel.AddressLine2;
                    user.City = finishAccountSetupModel.City;
                    user.StateId = finishAccountSetupModel.State;
                    user.Dob = finishAccountSetupModel.DateOfBirth;
                    user.PhoneNumber = finishAccountSetupModel.PhoneNumber;
                    user.ZipCode = finishAccountSetupModel.ZipCode;
                    var identityResult = await _userManger.UpdateAsync(user).ConfigureAwait(true);
                    if (identityResult.Succeeded)
                    {
                        return new CrudResult() { ErroCode = "success", ErrorMassage = "success", Failed = false, Succeeded = true };
                    }
                    else
                    {
                        return new CrudResult() { ErroCode = "error", ErrorMassage = "error", Failed = true, Succeeded = false };
                    }
                }
                else
                {
                    return new CrudResult() { ErroCode = "error", ErrorMassage = "error", Failed = true, Succeeded = false };
                }  
            }
            catch (Exception)
            { 
                throw; 
            }
        }
        public async Task<CrudResult> UpdatePatientInfo(UpdatePatientView patientView)
        {
            if (patientView == null)
            {
                return new CrudResult() { ErroCode = "", ErrorMassage = "Parameters is null", Failed = true, Succeeded = false };
            }
            try
            {
                var User = await _userManger.FindByIdAsync(patientView.Id).ConfigureAwait(true);
                if (User != null)
                {
                    User.City = patientView.City;
                    User.StateId = patientView.StateId;
                    User.ZipCode = patientView.ZipCode;
                    User.FirstName = patientView.FirstName;
                    User.LastName = patientView.LastName;
                    User.PhoneNumber = patientView.PhoneNumber;
                    User.Dob = patientView.Dob;
                    await _userManger.UpdateAsync(User).ConfigureAwait(true);
                    return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
                }
                else
                {
                    return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = true, Succeeded = false };
                }
            }
            catch (Exception ex)
            {
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<TimeZoneConfig> GetTimeZone(string userId)
        {
            try
            {
                var appUser = await DBEntity.AspNetUsers.Where(x => x.Id == userId).Select(x => new { OffSet = x.TimeZone, IsDayLightSaving = x.IsDayLightSaving }).FirstOrDefaultAsync().ConfigureAwait(true);
                if (appUser == null)
                {
                    return new TimeZoneConfig() { IsDayLightSaving = false, OffSet = -330 };
                }
                else
                {
                    return new TimeZoneConfig()
                    {
                        IsDayLightSaving = appUser.IsDayLightSaving,
                        OffSet = (int)appUser.OffSet
                    };
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
