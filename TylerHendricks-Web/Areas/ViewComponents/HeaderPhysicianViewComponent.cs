using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TylerHendricks_Repo.Contracts;
using TylerHendricks_Web.Claim;

namespace TylerHendricks_Web.Areas.ViewComponents
{
    public class HeaderPhysicianViewComponent: ViewComponent
    {
        #region Initialize private field
        private readonly IRepositoryCollection _repoCollection;
        private readonly IUserService _userService;
        private readonly IUsers _userManager;
        #endregion
        public HeaderPhysicianViewComponent(IRepositoryCollection repoCollection, IUserService userService, IUsers userManager)
        {
            _repoCollection = repoCollection;
            _userService = userService;
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(bool StaticHeading, string HeadingText)
        {
            ViewBag.StaticHeading = StaticHeading;
            ViewBag.HeadingText = HeadingText;
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                ViewBag.UserName = await _userManager.GetName(User.Identity.Name).ConfigureAwait(true);
            }      
            return View();
        }
    }
}
