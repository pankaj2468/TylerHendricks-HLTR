using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Core.ViewModel;
using TylerHendricks_Repo.Contracts;
using TylerHendricks_Utility;
using TylerHendricks_Web.Claim;
using TylerHendricks_Web.Filters;

using static TylerHendricks_Utility.Enums.Areas;

namespace TylerHendricks_Web.Areas.PatientPortal.Controllers
{
    [Area("PatientPortal")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        #region Private field initialization
        private readonly IRepositoryCollection _repCollection;
        private readonly IUsers _users;
        private readonly IEmailProvider _email;
        private readonly IUserService _userService;
        private readonly ILogger<HomeController> _logger;
        private readonly ResourcesConfig _resourcesConfig;
        private readonly EnvironmentalResource _environmentalResource;
        private readonly IWebHostEnvironment _hostingEnvironment;
        #endregion
        public HomeController(IRepositoryCollection repCollection, IUsers users, IUserService userService, IEmailProvider email
            , ILogger<HomeController> logger, ResourcesConfig resourcesConfig, EnvironmentalResource environmentalResource
            , IWebHostEnvironment hostingEnvironment)
        {
            _users = users;
            _repCollection = repCollection;
            _userService = userService;
            _email = email;
            _logger = logger;
            _resourcesConfig = resourcesConfig;
            _environmentalResource = environmentalResource;
            _hostingEnvironment = hostingEnvironment;
        }

       
        [HttpGet]
        [Route("")]
        [Route("consultation")]
        [Route("ed-consultation")]
        [Route("hair-loss-consultation")]
        [Route("refill-consultation")]
        [ServiceFilter(typeof(CustomLoginedUserAuthentication))]
        public async Task<IActionResult> Index()
        {
            try
            {
                var path = HttpContext.Request.Path.Value.ToLower();
                if (!string.IsNullOrEmpty(User.Identity.Name))
                {
                    var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                    if (sessionManager == null)
                    {
                        sessionManager = new SessionManager();
                    }
                    var userId = await _users.GetUserId(User.Identity.Name).ConfigureAwait(true);
                    var consultations = await _repCollection.Patients.GetConsultation(userId).ConfigureAwait(true);
                    var consultationHairLoss = consultations.Find(x => x.ConsultationCategoryId == (int)ConsultationType.HairLoss);
                    var consultationMedicalRefill = consultations.Find(x => x.ConsultationCategoryId == (int)ConsultationType.MedicationRefill);
                    var consultationErectileDysfunction = consultations.Find(x => x.ConsultationCategoryId == (int)ConsultationType.ErectileDysfunction);
                    if (path.Contains("ed"))
                    {
                        sessionManager.ConsultationCategoryId = consultationErectileDysfunction.ConsultationCategoryId;
                        if (consultationErectileDysfunction.IsCompleted && ((consultationErectileDysfunction.EnabledDate.HasValue && consultationErectileDysfunction.EnabledDate.Value > DateTime.UtcNow) || !consultationErectileDysfunction.EnabledDate.HasValue))
                        {
                            return View();
                        }
                        else if (consultationErectileDysfunction.IsStarted)
                        {
                            var consultationCurrent = await _repCollection.Patients.GetConsultation(userId, sessionManager.ConsultationCategoryId)
                                .ConfigureAwait(true);
                            sessionManager.ConsultationId = consultationCurrent.ConsultationId;
                            _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                            return RedirectToAction(nameof(Index), "Patient", new { area = PortalType.PatientPortal.ToString() });
                        }
                        else
                        {
                            sessionManager.ConsultationId = Comman.GenerateId();
                            await _repCollection.Patients.SetConsultation(userId, sessionManager.ConsultationId, sessionManager.ConsultationCategoryId)
                               .ConfigureAwait(true);
                            _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                            return RedirectToAction(nameof(SelectState), "Patient", new { area = PortalType.PatientPortal.ToString() });
                        }
                    }
                    else if (path.Contains("refill"))
                    {
                        sessionManager.ConsultationCategoryId = consultationMedicalRefill.ConsultationCategoryId;
                        if (consultationMedicalRefill.IsCompleted && ((consultationMedicalRefill.EnabledDate.HasValue && consultationMedicalRefill.EnabledDate.Value > DateTime.UtcNow) || !consultationMedicalRefill.EnabledDate.HasValue))
                        {
                            return View();
                        }
                        else if (consultationMedicalRefill.IsStarted)
                        {
                            var consultationCurrent = await _repCollection.Patients.GetConsultation(userId, sessionManager.ConsultationCategoryId)
                                .ConfigureAwait(true);
                            sessionManager.ConsultationId = consultationCurrent.ConsultationId;
                            _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                            return RedirectToAction(nameof(Index), "Patient", new { area = PortalType.PatientPortal.ToString() });
                        }
                        else
                        {
                            sessionManager.ConsultationId = Comman.GenerateId();
                            await _repCollection.Patients.SetConsultation(userId, sessionManager.ConsultationId, sessionManager.ConsultationCategoryId)
                               .ConfigureAwait(true);
                            _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                            return RedirectToAction(nameof(SelectState), "Patient", new { area = PortalType.PatientPortal.ToString() });
                        }
                    }
                    else if (path.Contains("hair"))
                    {
                        sessionManager.ConsultationCategoryId = consultationHairLoss.ConsultationCategoryId;
                        if (consultationHairLoss.IsCompleted && ((consultationHairLoss.EnabledDate.HasValue && consultationHairLoss.EnabledDate.Value > DateTime.UtcNow) || !consultationHairLoss.EnabledDate.HasValue))
                        {
                            return View();
                        }
                        else if (consultationHairLoss.IsStarted)
                        {
                            var consultationCurrent = await _repCollection.Patients.GetConsultation(userId, sessionManager.ConsultationCategoryId)
                                .ConfigureAwait(true);
                            sessionManager.ConsultationId = consultationCurrent.ConsultationId;
                            _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                            return RedirectToAction(nameof(Index), "Patient", new { area = PortalType.PatientPortal.ToString() });
                        }
                        else
                        {
                            sessionManager.ConsultationId = Comman.GenerateId();
                            await _repCollection.Patients.SetConsultation(userId, sessionManager.ConsultationId, sessionManager.ConsultationCategoryId)
                               .ConfigureAwait(true);
                            _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                            return RedirectToAction(nameof(SelectState), "Patient", new { area = PortalType.PatientPortal.ToString() });
                        }
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    var sessionManager = new SessionManager();
                    if (path.Contains("ed"))
                    {
                        sessionManager.ConsultationId = Comman.GenerateId();
                        sessionManager.ConsultationCategoryId = (int)ConsultationType.ErectileDysfunction;
                        _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                        return RedirectToAction(nameof(SelectState), "Home", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else if (path.Contains("refill"))
                    {
                        sessionManager.ConsultationId = Comman.GenerateId();
                        sessionManager.ConsultationCategoryId = (int)ConsultationType.MedicationRefill;
                        _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                        return RedirectToAction(nameof(SelectState), "Home", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else if (path.Contains("hair"))
                    {
                        sessionManager.ConsultationId = Comman.GenerateId();
                        sessionManager.ConsultationCategoryId = (int)ConsultationType.HairLoss;
                        _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                        return RedirectToAction(nameof(SelectState), "Home", new { area = PortalType.PatientPortal.ToString() });
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return View();
        }

        [HttpPost]
        [Route("consultation")]
        public async Task<IActionResult> Index(ConsultationView consultation)
        {
            try
            {
                var sessionManager = new SessionManager();
                if (!string.IsNullOrEmpty(User.Identity.Name))
                {
                    var userId = await _users.GetUserId(User.Identity.Name)
                          .ConfigureAwait(true);
                    if (consultation.IsStarted)
                    {
                        var consultationCurrent = await _repCollection.Patients.GetConsultation(userId, consultation.ConsultationCategoryId)
                            .ConfigureAwait(true);

                        sessionManager.ConsultationCategoryId = consultation.ConsultationCategoryId;
                        sessionManager.ConsultationId = consultationCurrent.ConsultationId;
                        _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                    }
                    else
                    {
                        sessionManager.ConsultationId = Comman.GenerateId();
                        sessionManager.ConsultationCategoryId = consultation.ConsultationCategoryId;
                        await _repCollection.Patients.SetConsultation(userId, sessionManager.ConsultationId, sessionManager.ConsultationCategoryId)
                           .ConfigureAwait(true);
                        _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                    }
                    return RedirectToAction(nameof(Index), "Patient", new { area = PortalType.PatientPortal.ToString() });
                }
                else
                {
                    sessionManager.ConsultationId = Comman.GenerateId();
                    sessionManager.ConsultationCategoryId = consultation.ConsultationCategoryId;
                    _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                    return RedirectToAction(nameof(SelectState), "Home", new { area = PortalType.PatientPortal.ToString() });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction(nameof(Index), "Home", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpGet]
        public IActionResult SelectState()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SelectState(SessionManager manager)
        {
            if (ModelState.IsValid && manager.StateId > 0)
            {
                try
                {
                    bool IsActive = await _repCollection.States.IsStateActive(manager.StateId).ConfigureAwait(true);
                    if (IsActive)
                    {
                        var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                        sessionManager.StateId = manager.StateId;
                        _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                        return RedirectToAction(nameof(StartConsultation), "Home", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else
                    {
                        return RedirectToAction(nameof(NotifyPatient), "Home", new { area = PortalType.PatientPortal.ToString(), manager.StateId });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    return RedirectToAction(nameof(Index), "Home", new { area = PortalType.PatientPortal.ToString() });
                }
            }
            else
            {
                ModelState.AddModelError("", _resourcesConfig.ErrorMessageForState);
                return View();
            }
        }

        [HttpGet]
        public IActionResult NotifyPatient(int StateId)
        {
            ViewBag.StateId = StateId;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NotifyPatient(Notify _notify)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    bool isRecordSvae = await _repCollection.States.SaveNotifyDetails(_notify).ConfigureAwait(true);
                    if (isRecordSvae)
                    {
                        ViewBag.isRecordSvae = isRecordSvae;
                    }
                    else
                    {
                        ViewBag.isRecordSvae = false;
                    }     
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    return RedirectToAction(nameof(NotifyPatient), "Home", new { area = PortalType.PatientPortal.ToString() });
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> StartConsultation()
        {
            try
            {
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                var firstQuestion = await _repCollection.Patients.GetFirstQuestion(sessionManager.ConsultationCategoryId).ConfigureAwait(true);
                var questions = await _repCollection.Patients.QuestionsDetail(string.Empty, firstQuestion).ConfigureAwait(true);
                return View(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction(nameof(Index), "Home", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> StartConsultation(Questions question)
        {
            try
            {
                if (question.Response.Value)
                {
                    var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                    var questionDetail = await _repCollection.Patients.QuestionsDetail(_userService.GetUserId(), question.NextQuestionId.Value, question.Response.Value).ConfigureAwait(true);
                    if (!string.IsNullOrEmpty(User.Identity.Name))
                    {
                        var result = await _repCollection.Patients.UpdateQuestionState(string.Empty, question.Id.ToString(), question.Answer, sessionManager.ConsultationId, (int)question.ConsultationCategoryId, question.Id.ToString()).ConfigureAwait(true);
                        return RedirectToAction(nameof(Signup));
                    }
                    else
                    {
                        var result = await _repCollection.Patients.UpdateQuestionState(string.Empty, question.Id.ToString(), question.Answer, sessionManager.ConsultationId, (int)question.ConsultationCategoryId, question.Id.ToString()).ConfigureAwait(true);
                        return RedirectToAction(nameof(Signup));
                    }
                }
                else
                {
                    return RedirectToAction(nameof(ExitConsultation));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction(nameof(Index), "Home", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExitConsultation()
        {
            var questions = await _repCollection.Patients.QuestionsDetail("", _resourcesConfig.ExistConsultationQuestionId, false).ConfigureAwait(true);
            return View(questions);
        }

        [HttpGet]
        [Route("patient-portal")]
        [ServiceFilter(typeof(CustomLoginedUserAuthentication))]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [Route("patient-portal")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                if (loginModel != null)
                {
                    loginModel.Role = RoleType.Patient.ToString();
                    LoginType loginType = await _users.SignIn(loginModel).ConfigureAwait(true);
                    if (loginType == LoginType.Success)
                    {
                        var userId = await _users.GetUserId(loginModel.Email).ConfigureAwait(true);
                        var status = await _repCollection.Patients.GetCompleteConsultationStatus(userId).ConfigureAwait(true);
                        if (status)
                        {
                            return RedirectToAction("OrderHistory", "Patient", new { area = PortalType.PatientPortal.ToString() });
                        }
                        else
                        {
                            return RedirectToAction(nameof(Index), "Home", new { area = PortalType.PatientPortal.ToString() });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", _resourcesConfig.ErrorMessageForEmail);
                        return View();
                    }
                }
                else
                {
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
        public IActionResult ForGotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForGotPassword(ForgotModel resetPassword)
        {
            ActionResult action;
            if (ModelState.IsValid)
            {
                if (resetPassword != null)
                {
                    resetPassword.Role = RoleType.Patient.ToString();
                    PasswordToken passToken = await _users.ResetPasswordToken(resetPassword).ConfigureAwait(true);
                    if (passToken != null)
                    {
                        var PassworfLink = Url.Action(nameof(ResetPassword), "Home", new { area = PortalType.PatientPortal.ToString(), TokenId = passToken.Id, passToken.Token }, Request.Scheme);
                        var htmlbody = await _repCollection.AmazonCustom.GetTemplate
                            ($"{_environmentalResource.AWSS3Credentials.EmailTemplateDirectory}/{EmailTemplate.ResetPassword}.html",
                            _environmentalResource.AWSS3Credentials.BucketName,
                            _environmentalResource.AWSS3Credentials.AccessKeyId,
                            _environmentalResource.AWSS3Credentials.SecretAccessKey
                            ).ConfigureAwait(true);
                        List<KeyValuePair<string, string>> placeHolders = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("{{ResetLink}}", PassworfLink) };
                        string body = Comman.UpdatePaceHolder(htmlbody, placeHolders);
                        _email.PlaintTextGmail(EmailTemplate.ResetPassword.ToString(), resetPassword.Email, _resourcesConfig.ForgotEmailSubject, body);
                        TempData["alert"] = AlertType.ForgotPassword;
                        return action = RedirectToAction(nameof(Login), "Home", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else
                    {
                        ModelState.AddModelError("", _resourcesConfig.ErrorMessageForUsername);
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("","");
                return action = RedirectToAction(nameof(ForGotPassword), "Home", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpGet]
        public IActionResult ResetPassword(string TokenId, string Token)
        {
            if (string.IsNullOrEmpty(TokenId) || string.IsNullOrEmpty(Token))
            {
                return RedirectToAction(nameof(ForGotPassword), "Home", new { area = PortalType.PatientPortal.ToString() });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel resetPassModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var IsSuccess = await _users.ResetPassword(resetPassModel).ConfigureAwait(true);
                    if (IsSuccess.Succeeded)
                    {
                        TempData["alert"] = AlertType.ResetPassword;
                        return RedirectToAction(nameof(Login), "Home", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else
                    {
                        return RedirectToAction(nameof(ResetPassword), "Home", new { area = PortalType.PatientPortal.ToString() });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                    return RedirectToAction(nameof(ForGotPassword), "Patient", new { area = PortalType.PatientPortal.ToString() });
                }
            }
            else
            {
                return RedirectToAction(nameof(ResetPassword), "Patient", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signup(Signup _signup)
        {
            try
            {
                var _sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                if (_sessionManager != null)
                {
                    _signup.ConsultationCategoryId = _sessionManager.ConsultationCategoryId;
                    _signup.ConsultationId = _sessionManager.ConsultationId;
                    _signup.StateId = _sessionManager.StateId;
                    _signup.Role = RoleType.Patient.ToString();
                    _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), _sessionManager);
                }
                else
                {
                    return RedirectToAction(nameof(Index), "Home", new { area = PortalType.PatientPortal.ToString() });
                }
                if (ModelState.IsValid)
                {
                    var IsSignUpUser = await _users.SignUpUsers(_signup).ConfigureAwait(true);
                    if (IsSignUpUser.Succeeded)
                    {
                        await _repCollection.States.DeleteNotify(_signup.Email).ConfigureAwait(true);
                        return RedirectToAction(nameof(Index), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else
                    {
                        ModelState.AddModelError("", _resourcesConfig.ErrorMessageForUserExist);
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "");
                    return View();
                }
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Signup), "Home", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpGet]
        public IActionResult GetTermsAndPolicyContent(string DocType)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = true };
            try
            {
                string html = string.Empty;
                if (DocumentType.TermsAndConditions.ToString() == DocType)
                {
                    html = Comman.GetFileText(DocumentType.TermsAndConditions.ToString());
                }
                else if (DocumentType.PrivacyPolicy.ToString() == DocType)
                {
                    html = Comman.GetFileText(DocumentType.PrivacyPolicy.ToString());
                }
                ajaxResponse.Data = html;
                return Json(ajaxResponse);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message, ex);
                ajaxResponse.ErrorMessage = ex.Message;
                ajaxResponse.Succeeded = false;
                return Json(ajaxResponse);
            }
        }

        [HttpGet]
        public IActionResult SetSession()
        {
            try
            {
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("" + ex.Message);
            }
        }
    }
}
