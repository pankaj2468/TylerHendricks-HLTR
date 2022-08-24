using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Repo.Contracts;

namespace TylerHendricks_Repo.Services
{
    public class Common : Repository, ICommon
    {
        public async Task<int> GetConsultationCategory(string consultationId)
        {
            return await DBEntity.Consultation.Where(x => x.ConsultationId == consultationId)
                .Select(x => x.ConsultationCategoryId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(true);
        }
        public async Task<bool> GetRxRequestStatus(string userId, int consultationCategoryId)
        {
            try
            {
                var consultation = await DBEntity.Consultation
                    .Where(x => x.IsRecordDeleted == false && x.ConsultationCategoryId == consultationCategoryId && x.UserId == userId)
                    .OrderBy(x => x.Id)
                    .Select(x => x.RxRequest)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                if (consultation == null)
                {
                    return false;
                }
                else
                {
                    return (bool)consultation;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> GetConsultationChatStatus(string consultationId)
        {
            try
            {
                var consult = await DBEntity.Consultation
                    .Where(x => x.IsRecordDeleted == false && x.ConsultationId == consultationId)
                    .Select(x => new { x.UserId, x.ConsultationCategoryId })
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                var consultations = await DBEntity.Consultation
                    .Where(x => x.UserId == consult.UserId && x.ConsultationCategoryId == consult.ConsultationCategoryId && x.IsRecordDeleted == false)
                    .ToListAsync()
                    .ConfigureAwait(true);
                if (consultations.Count > 1)
                {
                    var firstConsultation = consultations.OrderBy(x => x.Id).FirstOrDefault();
                    var lastConsultation = consultations.OrderBy(x => x.Id).LastOrDefault();
                    if (firstConsultation.ConsultationId == consultationId)
                    {
                        return false;
                    }
                    else if (lastConsultation.ConsultationId == consultationId)
                    {
                        return true;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<string>> GetRetakeRxRequest(string userId, int consultationCategoryId)
        {
            try
            {
                return await DBEntity.Consultation
                    .Where(x => x.IsRecordDeleted == false && x.UserId == userId && x.RxRequest == true && x.ConsultationCategoryId == consultationCategoryId)
                    .OrderBy(x => x.Id)
                    .Select(x => x.ConsultationId)
                    .ToListAsync()
                    .ConfigureAwait(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<string> GetCurrentUserRoleByName(string name)
        {
            try
            {
                return DBEntity.AspNetUsers
                    .Join(DBEntity.AspNetUserRoles, u => u.Id, r => r.UserId, (u, r) => new { u, r })
                    .Join(DBEntity.AspNetRoles, ur => ur.r.RoleId, a => a.Id, (ur, a) => new { ur, a })
                    .Where(x => x.ur.u.UserName == name)
                    .Select(x => x.a.Name)
                    .ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<NotificationView> GetNotification(string templateName)
        {
            try
            {
                var notifications = await DBEntity.Notification
                    .Where(x => x.IsRecordDeleted == false)
                    .Select(x => new { x.IsActived, x.TemplateName })
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                return new NotificationView()
                {
                    Status = notifications.IsActived,
                    TemplateName = notifications.TemplateName
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            
        }
    }
}
