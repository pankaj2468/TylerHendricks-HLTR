using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;

namespace TylerHendricks_Repo.Contracts
{
    public interface ICommon : IDisposable
    {
        Task<int> GetConsultationCategory(string consultationId);
        Task<bool> GetRxRequestStatus(string userId, int consultationCategoryId);
        Task<bool> GetConsultationChatStatus(string consultationId);
        Task<List<string>> GetRetakeRxRequest(string userId, int consultationCategoryId);
        List<string> GetCurrentUserRoleByName(string name);
        Task<NotificationView> GetNotification(string templateName);
    }
}
