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
    public class AdminHeaderViewComponent : ViewComponent
    {
        #region Initialize private field
        private readonly IRepositoryCollection _repoCollection;
        private readonly IUserService _userService;
        private readonly IUsers _userManager;

        #endregion
        public AdminHeaderViewComponent(IRepositoryCollection repoCollection, IUserService userService, IUsers userManager)
        {
            _repoCollection = repoCollection;
            _userService = userService;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(bool StaticHeading, string HeadingText)
        {
            ViewBag.StaticHeading = StaticHeading;
            if (StaticHeading)
            {
                ViewBag.HeadingText = HeadingText;
            }
            ViewBag.userName = User.Identity.Name;
            var userId = _userService.GetUserId();
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                ViewBag.ConsultationStatus = await _repoCollection.Patients.UserConsultationStatus(userId).ConfigureAwait(true);
                if (ViewBag.ConsultationStatus)
                {
                    ViewBag.userName = await _userManager.GetFirstName(User.Identity.Name).ConfigureAwait(true);
                }
            }
            return View();
        }
    }
}
