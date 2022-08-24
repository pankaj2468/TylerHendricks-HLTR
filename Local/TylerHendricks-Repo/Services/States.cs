using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Data.DBEntity;
using TylerHendricks_Repo.Contracts;

namespace TylerHendricks_Repo.Services
{
    public class States : Repository, IStates
    {
        public async Task<List<State>> GetStates()
        {
            try
            {
                List<State> lsState = new List<State>();
                var states = await (from st in DBEntity.FacilityStates
                                    where st.IsRecordDeleted == false
                                    select new
                                    {
                                        Id = st.Id,
                                        StateName = st.Name,
                                        Status = st.IsActive
                                    }).ToListAsync().ConfigureAwait(true);

                foreach (var item in states)
                {
                    lsState.Add(new State()
                    {
                        Id = item.Id,
                        StateName = item.StateName,
                        Status = item.Status == null ? false : (bool)item.Status
                    });
                }
                return lsState;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> IsStateActive(int stateId)
        {
            try
            {
                var isActive = await (from st in DBEntity.FacilityStates
                                      where st.Id == stateId
                                      select new { IsActive = st.IsActive }).SingleAsync().ConfigureAwait(true);

                return (isActive.IsActive != null) ? isActive.IsActive.Value : false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SaveNotifyDetails(Notify notify)
        {
            using var transaction = DBEntity.Database.BeginTransaction();
            try
            {
                DBEntity.NotifyPatients.Add(new NotifyPatients()
                {
                    Email = notify.Email,
                    StateId = notify.StateId,
                    AddedDate = DateTime.UtcNow,
                    AddedBy = notify.Email,
                    IsRecordDelete = false,
                });

                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                return false;
            }
        }

        public async Task<bool> DeleteNotify(string emailAddress)
        {
            using var transaction = DBEntity.Database.BeginTransaction();
            try
            {
                var records = await (from fcState in DBEntity.NotifyPatients
                                     where fcState.Email == emailAddress
                                     select fcState).ToListAsync().ConfigureAwait(true);
              
                records.ForEach(x => { x.IsRecordDelete = true; });
                DBEntity.NotifyPatients.UpdateRange(records);
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transaction.Commit();
                return true;
            }
            catch(Exception)
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}
