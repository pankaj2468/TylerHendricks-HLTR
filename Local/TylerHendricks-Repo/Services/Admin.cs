using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Repo.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace TylerHendricks_Repo.Services
{
    public class Admin : Repository, IAdmin
    {
        public async Task<List<ServiceState>> GetStates()
        {
            try
            {
                var serviceList = new List<ServiceState>();
                var states = await DBEntity.FacilityStates.Where(x => x.IsRecordDeleted == false)
                    .Select(x => new
                    {
                        StateName = x.Name,
                        StateId = x.Id,
                        IsActive = x.IsActive
                    })
                    .ToListAsync().ConfigureAwait(true);

                foreach (var item in states)
                {
                    serviceList.Add(new ServiceState
                    {
                        IsActive = (bool)item.IsActive,
                        StateId = item.StateId,
                        StateName = item.StateName
                    });
                }
                return serviceList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> ChangeStateStatus(int stateId)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var state = await DBEntity.FacilityStates.Where(x => x.Id == stateId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                state.IsActive = state.IsActive == true ? false : true;
                DBEntity.FacilityStates.Update(state).Property(x => x.Id).IsModified = false;
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { Failed = false, Succeeded = true };
            }
            catch (Exception)
            {
                transcation.Rollback();
                throw;
            }
        }
        public async Task<List<AdminPatientDataView>> GetPatients(TimeZoneConfig timeZone, string SortColumn, string SortColumnDirection, string SearchValue, int PageSize, int Start, int Skip, int FilterMode,string StartDate,string EndDate)
        {
            try
            {
                var SortColumnParam = new SqlParameter("@SortColumn", SortColumn == null ? "" : SortColumn);
                var SortColumnDirectionParam = new SqlParameter("@SortColumnDirection", SortColumnDirection == null ? "" : SortColumnDirection);
                var SearchValueParam = new SqlParameter("@SearchValue", SearchValue);
                var PageSizeParam = new SqlParameter("@PageSize", PageSize);
                var StartParam = new SqlParameter("@Start", Start);
                var SkipParam = new SqlParameter("@Skip", Skip);
                var FilterModeParam = new SqlParameter("@FilterMode", FilterMode);
                var TimeZoneParam = new SqlParameter("@TimeZone", timeZone.OffSet * -1);
                var StartDateParam = new SqlParameter("@StartDate", StartDate == null ? "" : StartDate);
                var EndDateParam = new SqlParameter("@EndDate", EndDate == null ? "" : EndDate);
                var adminPatients = await DBEntity.ADMIN_PATIENT_RECORD.FromSqlRaw("EXEC [ADMIN_PATIENT_RECORD] @SortColumn,@SortColumnDirection,@SearchValue,@PageSize" +
                    ",@Start,@Skip,@FilterMode,@TimeZone,@StartDate,@EndDate", SortColumnParam, SortColumnDirectionParam, SearchValueParam, PageSizeParam, StartParam, SkipParam, FilterModeParam, TimeZoneParam,StartDateParam,EndDateParam)
                    .ToListAsync()
                    .ConfigureAwait(true);
                return adminPatients;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<InitialRegister>> GetPatients(TimeZoneConfig timeZone, string SortColumn, string SortColumnDirection, string SearchValue, int PageSize, int Start, int Skip, string StartDate, string EndDate)
        {
            try
            {
                var SortColumnParam = new SqlParameter("@SortColumn", SortColumn == null ? "" : SortColumn);
                var SortColumnDirectionParam = new SqlParameter("@SortColumnDirection", SortColumnDirection == null ? "" : SortColumnDirection);
                var SearchValueParam = new SqlParameter("@SearchValue", SearchValue);
                var PageSizeParam = new SqlParameter("@PageSize", PageSize);
                var StartParam = new SqlParameter("@Start", Start);
                var SkipParam = new SqlParameter("@Skip", Skip);
                var TimeZoneParam = new SqlParameter("@TimeZone", timeZone.OffSet * -1);
                var StartDateParam = new SqlParameter("@StartDate", StartDate == null ? "" : StartDate);
                var EndDateParam = new SqlParameter("@EndDate", EndDate == null ? "" : EndDate);
                var adminPatients = await DBEntity.GetInitialRegisters
                    .FromSqlRaw("EXEC AdminInitialRegistration @SortColumn,@SortColumnDirection,@SearchValue,@PageSize,@Start,@Skip,@TimeZone,@StartDate,@EndDate", SortColumnParam, SortColumnDirectionParam, SearchValueParam, PageSizeParam, StartParam, SkipParam, TimeZoneParam, StartDateParam, EndDateParam)
                    .ToListAsync()
                    .ConfigureAwait(true);
                return adminPatients;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<CrudResult> ChangeMDToolBoxStatus(string consultationId)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var consultation = await DBEntity.Consultation.Where(x => x.ConsultationId == consultationId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                consultation.IsMdtoolbox = consultation.IsMdtoolbox == null ? true : consultation.IsMdtoolbox == true ? false : true;
                DBEntity.Consultation.Update(consultation).Property(x => x.Id).IsModified = false;
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { Failed = false, Succeeded = true };
            }
            catch (Exception)
            {
                transcation.Rollback();
                throw;
            }
        }
        public async Task<List<ConsultationCategoryView>> GetConsultations()
        {
            try
            {
                var consultationCategories = await DBEntity.ConsultationCategory
                    .Where(x => x.IsRecordDeleted == false)
                    .ToListAsync()
                    .ConfigureAwait(true);
                var listConsultation = new List<ConsultationCategoryView>();
                foreach (var item in consultationCategories)
                {
                    listConsultation.Add(new ConsultationCategoryView()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Status = item.IsActived,
                    });
                }
                return listConsultation;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> ChangeConsultationStatus(int id)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var consultationCategory = await DBEntity.ConsultationCategory.Where(x => x.Id == id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                consultationCategory.IsActived = consultationCategory.IsActived == true ? false : true;
                DBEntity.ConsultationCategory.Update(consultationCategory).Property(x => x.Id).IsModified = false;
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { Failed = false, Succeeded = true };
            }
            catch (Exception)
            {
                transcation.Rollback();
                throw;
            }
        }
        public async Task<List<NotificationView>> GetNotifications()
        {
            try
            {
                var notifications = await DBEntity.Notification
                    .Where(x => x.IsRecordDeleted == false)
                    .Select(x => new
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Status = x.IsActived
                    })
                    .ToListAsync()
                    .ConfigureAwait(true);
                var listNotifications = new List<NotificationView>();
                foreach (var item in notifications)
                {
                    listNotifications.Add(new NotificationView()
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Status = item.Status,
                    });
                }
                return listNotifications;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> ChangeNotificationStatus(int id)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var notification = await DBEntity.Notification.Where(x => x.Id == id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                notification.IsActived = notification.IsActived == true ? false : true;
                DBEntity.Notification.Update(notification).Property(x => x.Id).IsModified = false;
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { Failed = false, Succeeded = true };
            }
            catch (Exception)
            {
                transcation.Rollback();
                throw;
            }
        }
    }
}
