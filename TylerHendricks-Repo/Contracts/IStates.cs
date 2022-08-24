using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;

namespace TylerHendricks_Repo.Contracts
{
    public interface IStates: IDisposable
    {
        Task<List<State>> GetStates();
        Task<bool> IsStateActive(int stateId);
        Task<bool> SaveNotifyDetails(Notify notify);
        Task<bool> DeleteNotify(string emailAddress);
        
    }
}
