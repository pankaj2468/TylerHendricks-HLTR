using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TylerHendricks_Core.Models;
using TylerHendricks_Repo.Contracts;
using TylerHendricks_Utility;
using TylerHendricks_Web.Claim;
using TylerHendricks_Web.Filters;

using static TylerHendricks_Utility.Enums.Areas;

namespace TylerHendricks_Web.Areas.AdminPortal.Controllers
{
    [Area("AdminPortal")]
    public class HomeController : Controller
    {
        private readonly IUsers _userManager;
        private readonly IEmailProvider _email;
        private readonly IUserService _userService;
        private readonly IRepositoryCollection _repCollection;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<HomeController> _logger;
        private readonly ResourcesConfig _resourcesConfig;
        private readonly EnvironmentalResource _environmentalResource;

        public HomeController(IUsers userManager, IEmailProvider email, IRepositoryCollection repositoryCollection, IUserService
     userService, IWebHostEnvironment hostingEnvironment, ILogger<HomeController> logger, ResourcesConfig resourcesConfig
            ,EnvironmentalResource environmentalResource)
        {
            _userManager = userManager;
            _email = email;
            _repCollection = repositoryCollection;
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
            _resourcesConfig = resourcesConfig;
            _environmentalResource = environmentalResource;
        }

        #region [User Handler]
        [HttpGet]
        [AllowAnonymous]
        [Route("admin-portal")]
        [ServiceFilter(typeof(CustomLoginedUserAuthentication))]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("admin-portal")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                loginModel.Role = RoleType.Admin.ToString();
                LoginType loginType = await _userManager.SignIn(loginModel).ConfigureAwait(true);
                if (loginType == LoginType.Success)
                {
                    return RedirectToAction(nameof(PatientInfo), "Home", new { area = PortalType.AdminPortal.ToString() });
                }
                else if (loginType == LoginType.WrongPassword)
                {
                    ModelState.AddModelError("", _resourcesConfig.ErrorMessageForEmail);
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", _resourcesConfig.ErrorMessageForEmail);
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", _resourcesConfig.ErrorMessageForEmail);
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("admin-portal/ForGotPassword")]
        public IActionResult ForGotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("admin-portal/ForGotPassword")]
        public async Task<IActionResult> ForGotPassword(ForgotModel resetPassword)
        {
            ActionResult action;
            if (ModelState.IsValid)
            {
                resetPassword.Role = RoleType.Admin.ToString();
                PasswordToken passToken = await _userManager.ResetPasswordToken(resetPassword).ConfigureAwait(true);
                if (passToken != null)
                {
                    var PassworfLink = Url.Action(nameof(ResetPassword), "Home", new { area = PortalType.AdminPortal.ToString(), TokenId = passToken.Id, Token = passToken.Token }, Request.Scheme);
                    var htmlbody = await _repCollection.AmazonCustom.GetTemplate
                        ( $"{_environmentalResource.AWSS3Credentials.EmailTemplateDirectory}/{EmailTemplate.ResetPassword.ToString()}.html" 
                        ,_environmentalResource.AWSS3Credentials.BucketName
                        ,_environmentalResource.AWSS3Credentials.AccessKeyId
                        ,_environmentalResource.AWSS3Credentials.SecretAccessKey
                        ).ConfigureAwait(true);
                    List<KeyValuePair<string, string>> placeHolders = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("{{ResetLink}}", PassworfLink) };
                    string body = Comman.UpdatePaceHolder(htmlbody, placeHolders);
                    _email.PlaintTextGmail(EmailTemplate.ResetPassword.ToString(), resetPassword.Email, _resourcesConfig.ResetPasswordEmailSubject, body);
                    TempData["alert"] = AlertType.ForgotPassword;
                    return action = RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("", _resourcesConfig.ErrorMessageForUsername);
                    return View();
                }
            }
            else
            {
                return action = RedirectToAction(nameof(ForGotPassword));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string TokenId, string Token)
        {
            if (string.IsNullOrEmpty(TokenId) || string.IsNullOrEmpty(Token))
            {
                return RedirectToAction(nameof(ForGotPassword), "Home", new { area = PortalType.AdminPortal.ToString() });
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPassModel)
        {
            ActionResult action;
            if (ModelState.IsValid)
            {
                var IsSuccess = await _userManager.ResetPassword(resetPassModel).ConfigureAwait(true);
                TempData["alert"] = AlertType.ResetPassword;
                if (IsSuccess.Succeeded)
                {
                    action = RedirectToAction(nameof(Login));
                }
                else
                {
                    action = RedirectToAction(nameof(ResetPassword));
                }
            }
            else
            {
                action = RedirectToAction(nameof(ResetPassword));
            }
            return action;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> LogOut()
        {
            await _userManager.SignOut().ConfigureAwait(true);
            return RedirectToAction(nameof(Login), "Home", new { area = PortalType.AdminPortal.ToString() });
        }
        #endregion

        #region [State]

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> State()
        {
            try
            {
                var serviceStates = await _repCollection.Admin.GetStates().ConfigureAwait(true);
                return View(serviceStates);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> ActiveInActiveState(int stateId)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = false  };
            try
            {
                var result = await _repCollection.Admin.ChangeStateStatus(stateId).ConfigureAwait(true);
                if (result.Succeeded)
                {
                    ajaxResponse.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                ajaxResponse.Succeeded = false;
                ajaxResponse.ErrorMessage = ex.Message;
            }
            return Json(ajaxResponse);
        }

        #endregion

        #region [Notification]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Notification()
        {
            try
            {
                var notifications = await _repCollection.Admin.GetNotifications().ConfigureAwait(true);
                return View(notifications);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> ActiveInActiveNotification(int id)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = false };
            try
            {
                var result = await _repCollection.Admin.ChangeNotificationStatus(id).ConfigureAwait(true);
                if (result.Succeeded)
                {
                    ajaxResponse.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                ajaxResponse.Succeeded = false;
                ajaxResponse.ErrorMessage = ex.Message;
            }
            return Json(ajaxResponse);
        }
        #endregion

        #region [Consultation]
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Consultation()
        {
            try
            {
                var consultations =  await _repCollection.Admin.GetConsultations().ConfigureAwait(true);
                return View(consultations);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<JsonResult> ActiveInActiveConsultation(int id)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = false };
            try
            {
                var result = await _repCollection.Admin.ChangeConsultationStatus(id).ConfigureAwait(true);
                if (result.Succeeded)
                {
                    ajaxResponse.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                ajaxResponse.Succeeded = false;
                ajaxResponse.ErrorMessage = ex.Message;
            }
            return Json(ajaxResponse);
        }
        #endregion

        #region [DataBase]

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult PatientInfo()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetPatientRecords(int FilterMode,string StartDate,string EndDate)
        {
            try
            {
                var timeZone = await _userManager.GetTimeZone(_userService.GetUserId()).ConfigureAwait(true);
                var tableLoad = new DataTableLoad();
                var load = tableLoad.LoadData(HttpContext);       
                var adminPatients = await _repCollection.Admin.GetPatients(timeZone, load.SortColumn, load.SortColumnDirection, load.SearchValue, load.PageSize, load.Start, load.Skip, FilterMode, StartDate, EndDate).ConfigureAwait(true);
                if (adminPatients == null)
                {
                    return Json(new { draw = load.Draw, recordsFiltered = 0, recordsTotal = 0, data = new List<AdminPatientDataView>() });
                }
                else if (adminPatients.Count == 0)
                {
                    return Json(new { draw = load.Draw, recordsFiltered = 0, recordsTotal = 0, data = new List<AdminPatientDataView>() });
                }
                else
                {
                    return Json(new { draw = load.Draw, recordsFiltered = adminPatients[0].TotalRecords, recordsTotal = adminPatients[0].TotalRecords, data = adminPatients });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetPatientInitialRegister(string StartDate,string EndDate)
        {
            try
            {
                var timeZone = await _userManager.GetTimeZone(_userService.GetUserId()).ConfigureAwait(true);
                var tableLoad = new DataTableLoad();
                var load = tableLoad.LoadData(HttpContext);
                var adminPatients = await _repCollection.Admin.GetPatients(timeZone, load.SortColumn, load.SortColumnDirection, load.SearchValue, load.PageSize, load.Start, load.Skip, StartDate, EndDate).ConfigureAwait(true);
                if (adminPatients == null)
                {
                    return Json(new { draw = load.Draw, recordsFiltered = 0, recordsTotal = 0, data = new List<InitialRegister>() });
                }
                else if (adminPatients.Count == 0)
                {
                    return Json(new { draw = load.Draw, recordsFiltered = 0, recordsTotal = 0, data = new List<InitialRegister>() });
                }
                else
                {
                    return Json(new { draw = load.Draw, recordsFiltered = adminPatients[0].TotalRecords, recordsTotal = adminPatients[0].TotalRecords, data = adminPatients });
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ChangeMDToolBox(string consultationId)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = false };
            try
            {
                var result = await _repCollection.Admin.ChangeMDToolBoxStatus(consultationId).ConfigureAwait(true);
                if (result.Succeeded)
                {
                    ajaxResponse.Succeeded = true;
                }
            }
            catch (Exception ex)
            {
                ajaxResponse.Succeeded = false;
                ajaxResponse.ErrorMessage = ex.Message;
            }
            return Json(ajaxResponse);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PatientDataExcel(int FilterMode)
        {
            try
            {
                var timeZone = await _userManager.GetTimeZone(_userService.GetUserId()).ConfigureAwait(true);
                var adminPatients = await _repCollection.Admin.GetPatients(timeZone, string.Empty, string.Empty, string.Empty, 0, 0, 0, FilterMode,"","").ConfigureAwait(true);
                var dataTable = _repCollection.Physician.ToDataTable<AdminPatientDataView>(adminPatients);
                dataTable.Columns.Remove("TotalRecords");
                dataTable.Columns.Remove("UserId");
                dataTable.Columns.Remove("ConsultationId");
                dataTable.Columns.Remove("FormatDobString"); 
                dataTable.TableName = "Patient";
                string fileName = "database.xlsx";
                using XLWorkbook wb = new XLWorkbook();
                wb.Worksheets.Add(dataTable);
                using MemoryStream stream = new MemoryStream(); 
                wb.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PatientInitialRegister()
        {
            try
            {
                var timeZone = await _userManager.GetTimeZone(_userService.GetUserId()).ConfigureAwait(true);
                var adminPatients = await _repCollection.Admin.GetPatients(timeZone, string.Empty, string.Empty, string.Empty, 0, 0, 0, "", "").ConfigureAwait(true);
                var dataTable = _repCollection.Physician.ToDataTable<InitialRegister>(adminPatients);
                dataTable.Columns.Remove("TotalRecords");
                dataTable.Columns.Remove("UserId");
                dataTable.Columns.Remove("ConsultationId");
                dataTable.TableName = "InitialRegistration";
                string fileName = "InitialRegistration.xlsx";
                using XLWorkbook wb = new XLWorkbook();
                wb.Worksheets.Add(dataTable);
                using MemoryStream stream = new MemoryStream();
                wb.SaveAs(stream);
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
