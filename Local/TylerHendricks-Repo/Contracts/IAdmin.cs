using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;

namespace TylerHendricks_Repo.Contracts
{
    public interface IAdmin: IDisposable
    {
        Task<List<ServiceState>> GetStates();
        Task<CrudResult> ChangeStateStatus(int stateId);
        Task<List<AdminPatientDataView>> GetPatients(TimeZoneConfig timeZone, string SortColumn, string SortColumnDirection, string SearchValue, int PageSize, int Start, int Skip, int FilterMode, string StartDate, string EndDate);
        Task<List<InitialRegister>> GetPatients(TimeZoneConfig timeZone, string SortColumn, string SortColumnDirection, string SearchValue, int PageSize, int Start, int Skip, string StartDate ,string EndDate);
        Task<CrudResult> ChangeMDToolBoxStatus(string consultationId);
        Task<List<ConsultationCategoryView>> GetConsultations();
        Task<CrudResult> ChangeConsultationStatus(int id);
        Task<List<NotificationView>> GetNotifications();
        Task<CrudResult> ChangeNotificationStatus(int id);
    }
}
