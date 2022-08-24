using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TylerHendricks_Repo.Contracts;

namespace TylerHendricks_Web.Areas.LogException.Controllers
{
    [Area("LogException")]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;
        private readonly IUsers _userManager;
        public ErrorController(ILogger<ErrorController> logger, IUsers userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            ViewBag.StatusCode = statusCode.ToString().ToCharArray();
            switch (statusCode)
            {
                case 404:
                    return RedirectPermanent("https://www.hendrxhealth.com/404");
                case 401:
                    return View("ResorceNotFound");
            }
            return View("ResorceNotFound");
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("Error")]
        public IActionResult Error()
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            _logger.LogError("Global-Error-Message : "+exception.Error.Message, "StackTrace :" + exception.Error.StackTrace, "Source :" + exception.Error.Source, "Path :" + exception.Path);
            _userManager.SignOut();
            return View();
        }
    }
}
