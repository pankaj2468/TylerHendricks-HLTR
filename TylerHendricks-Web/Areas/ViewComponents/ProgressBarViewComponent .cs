using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Core.ViewModel;
using TylerHendricks_Repo.Contracts;
using TylerHendricks_Web.Claim;
using static TylerHendricks_Utility.Enums.Areas;

namespace TylerHendricks_Web.Areas.ViewComponents
{
    public class ProgressBarViewComponent : ViewComponent
    {
        #region Initialize private field
        private readonly IRepositoryCollection _repoCollection;
        private readonly IUserService _userService;
        #endregion

        public ProgressBarViewComponent(IRepositoryCollection repoCollection, IUserService userService)
        {
            _repoCollection = repoCollection;
            _userService = userService;
        }
        public async Task<IViewComponentResult> InvokeAsync(bool IsStaticBar = false, int QuestionId = 0)
        {
            var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
            double tottalQuestion = await _repoCollection.Patients.TotalQuestions(sessionManager.ConsultationCategoryId).ConfigureAwait(true);
            double TottalAttempt = 0;
            var userId = _userService.GetUserId();
            if (userId != null)
            {
                TottalAttempt = await _repoCollection.Patients.AttemptQuestions(userId, sessionManager.ConsultationCategoryId, sessionManager.ConsultationId).ConfigureAwait(true);
            }
            ViewBag.BarPercent = (TottalAttempt / tottalQuestion) * 100;
            ViewBag.QuestionId = QuestionId;
            return View();
        }
    }
}
