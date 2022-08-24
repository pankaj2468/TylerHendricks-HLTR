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
    public class HeaderViewComponent : ViewComponent
    {
        #region Initialize private field
        private readonly IRepositoryCollection _repoCollection;
        private readonly IUserService _userService;
        private readonly IUsers _userManager;

        #endregion
        public HeaderViewComponent(IRepositoryCollection repoCollection, IUserService userService, IUsers userManager)
        {
            _repoCollection = repoCollection;
            _userService = userService;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(bool ShowRightSection, bool ShowQuestions, bool StaticHeading, string HeadingText,int QuestionId=0)
        {
            var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
            ViewBag.StaticHeading = StaticHeading;
            ViewBag.QuestionId = QuestionId;
            if (StaticHeading)
            {
                ViewBag.HeadingText = HeadingText;
            }
            ViewBag.userName = User.Identity.Name;
            var userId = _userService.GetUserId();
            ViewBag.userId = userId;
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                ViewBag.ConsultationStatus = await _repoCollection.Patients.UserConsultationStatus(userId).ConfigureAwait(true);
                if (ViewBag.ConsultationStatus)
                {
                    ViewBag.userName = await _userManager.GetFirstName(User.Identity.Name).ConfigureAwait(true);
                }
            }
            if (sessionManager != null)
            {
                ViewBag.ConsultationCategoryId = sessionManager.ConsultationCategoryId;
            }
            else
            {
                ViewBag.ConsultationCategoryId = 0;
            }
            if (ShowQuestions)
            {
                ViewBag.TotalQuestion = await _repoCollection.Patients.TotalQuestions(sessionManager.ConsultationCategoryId).ConfigureAwait(true);
                if (userId != null)
                {
                    ViewBag.TottalAttempt = await _repoCollection.Patients.AttemptQuestions(userId, sessionManager.ConsultationCategoryId, sessionManager.ConsultationId).ConfigureAwait(true);
                }
                else
                {
                    ViewBag.TottalAttempt = 1;
                }
            }
            ViewBag.ShowRightSection = ShowRightSection;
            ViewBag.ShowQuestions = ShowQuestions;
            return View();
        }
    }
}
