using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Core.ViewModel;
using TylerHendricks_Repo.Contracts;
using TylerHendricks_Utility;
using TylerHendricks_Web.Claim;
using static TylerHendricks_Utility.Enums.Areas;

namespace TylerHendricks_Web.Areas.PatientPortal.Controllers
{
    [Area("PatientPortal")]
    [Authorize(Roles = "Patient")]
    public class PatientController : Controller
    {
        #region [Intialize Private Field]
        private readonly IRepositoryCollection _repCollection;
        private readonly IUsers _userManager;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEmailProvider _email;
        private readonly ResourcesConfig _resourcesConfig;
        private readonly ILogger<HomeController> _logger;
        private readonly EnvironmentalResource _environmentalResource;
        #endregion

        public PatientController(IUsers userManager, IRepositoryCollection repCollection, IUserService userService, IWebHostEnvironment hostingEnvironment
            ,IEmailProvider email, ResourcesConfig resourcesConfig, ILogger<HomeController> logger, EnvironmentalResource environmentalResource)
        {
            _userManager = userManager;
            _repCollection = repCollection;
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
            _email = email;
            _resourcesConfig = resourcesConfig;
            _logger = logger;
            _environmentalResource = environmentalResource;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                string userId = _userService.GetUserId();
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                bool isCompleted = await _repCollection.Patients.ConsultationComplete(sessionManager.ConsultationId).ConfigureAwait(true);
                if (isCompleted)
                {
                    return RedirectToAction(nameof(OrderHistory), "Patient", new { area = PortalType.PatientPortal.ToString() });
                }
                var question = await _repCollection.Patients.CurrentQuestion(userId, sessionManager.ConsultationCategoryId, sessionManager.ConsultationId).ConfigureAwait(true);
                if (sessionManager.ConsultationCategoryId == (int)ConsultationType.MedicationRefill && !string.IsNullOrEmpty(question.Answer))
                {
                    return RedirectToAction(nameof(PharmacyInformation), "Patient", new { area = PortalType.PatientPortal.ToString() });
                }
                else if (sessionManager.ConsultationCategoryId == (int)ConsultationType.HairLoss && !string.IsNullOrEmpty(question.Answer))
                {
                    return RedirectToAction(nameof(SelectDeliveryOption), "Patient", new { area = PortalType.PatientPortal.ToString() });
                }
                else if (sessionManager.ConsultationCategoryId == (int)ConsultationType.ErectileDysfunction && !string.IsNullOrEmpty(question.Answer))
                {
                    return RedirectToAction(nameof(ChooseYourMedication), "Patient", new { area = PortalType.PatientPortal.ToString() });
                }
                return View(question);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction(nameof(LogOut), "Patient", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(Questions question)
        {
            try
            {
                if (question != null)
                {
                    string userId = _userService.GetUserId();
                    var _sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                    var questionDetail = await _repCollection.Patients.QuestionsDetail(userId, question.NextQuestionId.Value, question.Response.Value).ConfigureAwait(true);
                    CrudResult result = await _repCollection.Patients.UpdateQuestionState(userId, question.Id.ToString(), question.Answer, _sessionManager.ConsultationId, _sessionManager.ConsultationCategoryId, questionDetail.Id.ToString()).ConfigureAwait(true);
                    if (question.NextQuestionId == question.PreviousQuestionId)
                    {
                        return RedirectToAction(nameof(Index), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                    return View(questionDetail);   
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction(nameof(LogOut), "Patient", new { area = PortalType.PatientPortal.ToString() });
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PreviousQuestion()
        {
            try
            {
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                CrudResult result = await _repCollection.Patients.SetPreviousQuestion(_userService.GetUserId(), sessionManager.ConsultationCategoryId, sessionManager.ConsultationId, false).ConfigureAwait(true);
                return RedirectToAction(nameof(Index), "Patient", new { area = PortalType.PatientPortal.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction(nameof(LogOut), "Patient", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpGet]
        public async Task<IActionResult> PreviousMove()
        {
            try
            {
                var result = await _userManager.GotoPrevious(_userService.GetUserId()).ConfigureAwait(true);
                if (result.Succeeded)
                {
                    var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                    var innerResult = await _repCollection.Patients.SetPreviousQuestion(_userService.GetUserId(), sessionManager.ConsultationCategoryId, sessionManager.ConsultationId, true).ConfigureAwait(true);
                }
                return RedirectToAction(nameof(Index), "Patient", new { area = PortalType.PatientPortal.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return RedirectToAction(nameof(LogOut), "Patient", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetMedicationDropdown()
        {
            var units = (await _repCollection.Patients.GetMedicineUnit().ConfigureAwait(true)).Select(x => new { x.Id, x.Name });
            var forms = (await _repCollection.Patients.GetMedicineForms().ConfigureAwait(true)).Select(x => new { x.Id, x.Name });
            var frequencies = (await _repCollection.Patients.GetMedicineFrequency().ConfigureAwait(true)).Select(x => new { x.Id, x.Name });
            string jsonUnits = JsonConvert.SerializeObject(units);
            string jsonForm = JsonConvert.SerializeObject(forms);
            string jsonFrequency = JsonConvert.SerializeObject(frequencies);
            return Json("{\"units\":" + jsonUnits + ",\"forms\":" + jsonForm + ",\"frequency\":" + jsonFrequency + "}");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            try
            {
                await _userManager.SignOut().ConfigureAwait(true);
                _userService.ClearSession();
                return RedirectToAction(nameof(HomeController.Login), "Home", new { area = PortalType.PatientPortal.ToString() });
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("patient-portal/profile")]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserDetails(_userService.GetUserId()).ConfigureAwait(true);
            return View(user);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangedPassword changePassModel)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = true };
            try
            {
                var result = await _userManager.ChangePassword(changePassModel, _userService.GetUserId()).ConfigureAwait(true);
                if (result.Succeeded)
                {
                    return Json(ajaxResponse);
                }
                else
                {
                    ajaxResponse.Succeeded = false;
                    return Json(ajaxResponse);
                }
            }
            catch (Exception ex)
            {
                ajaxResponse.Succeeded = false;
                ajaxResponse.ErrorMessage = ex.Message;
                return Json(ajaxResponse);
            }
        }

        [HttpGet]
        public IActionResult UpdateEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateEmail(UpdateEmailModel emailModel)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = true };
            try
            {
                if (!string.IsNullOrEmpty(emailModel.Email) && !string.IsNullOrEmpty(emailModel.OTP))
                {
                    var result = await _userManager.VerifyOTP(_userService.GetUserId(), emailModel.OTP, emailModel.Email).ConfigureAwait(true);
                    if (result.Succeeded)
                    {
                            emailModel.Error = new Dictionary<string, string>() {
                        { "Email", ""}
                      , { "OTP", ""}
                      , { "Status", "success"} };
                        emailModel.IsOTPValid = true;
                        await _userManager.SignOut().ConfigureAwait(true);
                    }
                    else
                    {
                        emailModel.Error = new Dictionary<string, string>() {
                        { "Email", ""}
                      , { "OTP", "Invalid OTP"}
                      , { "Status", ""} };
                    }
                }
                else if (!string.IsNullOrEmpty(emailModel.Email))
                {
                    var result = await _repCollection.Patients.VerifyEmail(emailModel.Email).ConfigureAwait(true);
                    if (result)
                    {
                        
                        emailModel.IsValidEmail = true;
                        var crudResult = await _repCollection.Patients.SendOTP(_userService.GetUserId(), emailModel.Email).ConfigureAwait(true);
                        if (crudResult.Succeeded)
                        {
                            var htmlbody = await _repCollection.AmazonCustom.GetTemplate
                          (
                          $"{_environmentalResource.AWSS3Credentials.EmailTemplateDirectory}/{EmailTemplate.OTP}.html",
                          _environmentalResource.AWSS3Credentials.BucketName,
                          _environmentalResource.AWSS3Credentials.AccessKeyId,
                          _environmentalResource.AWSS3Credentials.SecretAccessKey
                          )
                          .ConfigureAwait(true);
                            List<KeyValuePair<string, string>> placeHolders = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("{{ResetLink}}", crudResult.ErrorMassage) };
                            string body = Comman.UpdatePaceHolder(htmlbody, placeHolders);
                            _email.PlaintTextGmail(EmailTemplate.OTP.ToString(), emailModel.Email, _resourcesConfig.OTPEmailSubject, body);
                            emailModel.Error = new Dictionary<string, string>() {
                        { "Email", ""}
                      , { "OTP", ""}
                      , { "Status", ""} };
                        }
                    }
                    else
                    {
                        
                        emailModel.IsValidEmail = false;
                        emailModel.Error = new Dictionary<string, string>() {
                        { "Email", "Email is already in use"}
                      , { "OTP", ""}
                      , { "Status", ""}
                    };
                    }
                }
                ajaxResponse.Data = emailModel;
                return Json(ajaxResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                ajaxResponse.Succeeded = false;
                ajaxResponse.ErrorMessage = ex.Message;
                return Json(ajaxResponse);
            }
        }

        #region [After Question Common]




        //public JsonResult CheckUsernameAvailability(string Email)
        //{
        //    System.Threading.Thread.Sleep(200);
        //    var SearchData = _userManager.GetEmail.Where(x => x.GetEmail == Email)
        //        .FirstOrDefault();
        //    if (SearchData != null)
        //    {
        //        return Json(1);
        //    }
        //    else
        //    {
        //        return Json(0);
        //    }

        //}

        [HttpGet]
        public async Task<IActionResult> ChooseYourMedication()
        {
            try
            {
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                var medicineFlag = await _repCollection.Patients.GetAnswer(_userService.GetUserId(), sessionManager.ConsultationCategoryId, "3").ConfigureAwait(true);
                ViewBag.MediFlag = medicineFlag;
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChooseYourMedication(QuestionNaireStep question)
        {
            ActionResult action = null;
            try
            {
                if (ModelState.IsValid)
                {
                    if (await _repCollection.Patients.IsConsultationMedicationExist(question.Id).ConfigureAwait(true))
                    {
                        var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                        var saveResult = await _repCollection.Patients.SaveTempConsultationMedication(sessionManager.ConsultationId, _userService.GetUserId(), question.Id).ConfigureAwait(true);
                        if (saveResult.Succeeded)
                        {
                            var sessionModel = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
                            if (sessionModel == null)
                            {
                                sessionModel = new QuestionNaireViewModel() { IsMedicationDelivery = false, MedicationId = question.Id };
                            }
                            else
                            {
                                sessionModel.MedicationId = question.Id;
                                sessionModel.IsMedicationDelivery = false;
                            }
                            _userService.SetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString(), sessionModel);
                            action = RedirectToAction(nameof(SelectDeliveryOption), "Patient", new { area = PortalType.PatientPortal.ToString() });
                        }
                        else
                        {
                            action = RedirectToAction(nameof(ChooseYourMedication), "Patient", new { area = PortalType.PatientPortal.ToString() });
                        }
                    }
                    else
                    {
                        action = RedirectToAction(nameof(ChooseYourMedication), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                }
                else
                {
                    action = RedirectToAction(nameof(ChooseYourMedication), "Patient", new { area = PortalType.PatientPortal.ToString() });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
                action = RedirectToAction(nameof(ChooseYourMedication), "Patient", new { area = PortalType.PatientPortal.ToString() });
            }
            return action;
        }

        [HttpGet]
        public IActionResult SelectDeliveryOption()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SelectDeliveryOption(SelectDeliveryOptionView selectDelivery)
        {
            ActionResult action;
            try
            {
                if (ModelState.IsValid)
                {
                    var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                    var questionSession = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
                    if (sessionManager.ConsultationCategoryId == (int)ConsultationType.ErectileDysfunction)
                    {
                        questionSession.IsMedicationDelivery = selectDelivery.isHomeDelivery;
                        action = RedirectToAction(nameof(FinishAccountSetup), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else
                    {
                        questionSession = new QuestionNaireViewModel() { IsMedicationDelivery = selectDelivery.isHomeDelivery };
                        action = RedirectToAction(nameof(FinishAccountSetupHairLoss), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                    if (selectDelivery.isHomeDelivery == false)
                    {
                        action = RedirectToAction(nameof(PharmacyInformation), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                    _userService.SetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString(), questionSession);
                }
                else
                {
                    action = RedirectToAction(nameof(SelectDeliveryOption), "Patient", new { area = PortalType.PatientPortal.ToString() });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                action = RedirectToAction(nameof(SelectDeliveryOption), "Patient", new { area = PortalType.PatientPortal.ToString() }); ;
            }
            return action;
        }

        [HttpGet]
        public IActionResult PharmacyInformation()
        {
            var questionNaireSession = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
            if (questionNaireSession == null)
            {
                questionNaireSession = new QuestionNaireViewModel();
            }
            questionNaireSession.IsMedicationDelivery = false;
            _userService.SetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString(), questionNaireSession);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PharmacyInformation(PharmacyInformationModel pharmacy)
        {
            ActionResult action;
            try
            {
                if (ModelState.IsValid)
                {
                    var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                    var naireViewModel = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
                    if (sessionManager.ConsultationCategoryId == (int)ConsultationType.ErectileDysfunction)
                    {
                        naireViewModel.pharmacyInformationModel = pharmacy;
                        action = RedirectToAction(nameof(FinishAccountSetup), ControllerType.Patient.ToString(), new { area = PortalType.PatientPortal.ToString() });
                    }
                    else if (sessionManager.ConsultationCategoryId == (int)ConsultationType.MedicationRefill)
                    {
                        naireViewModel = new QuestionNaireViewModel() { pharmacyInformationModel = pharmacy };
                        action = RedirectToAction(nameof(FinishAccountSetup), ControllerType.Patient.ToString(), new { area = PortalType.PatientPortal.ToString() });
                    }
                    else
                    {
                        naireViewModel.pharmacyInformationModel = pharmacy;
                        action = RedirectToAction(nameof(FinishAccountSetupHairLoss), ControllerType.Patient.ToString(), new { area = PortalType.PatientPortal.ToString() });
                    }
                    var result = await _repCollection.Patients.SavePharmacyDetail(pharmacy, sessionManager.ConsultationId, _userService.GetUserId()).ConfigureAwait(true);
                    if (result.Succeeded)
                    {
                        _userService.SetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString(), naireViewModel);
                    }
                    else
                    {
                        action = RedirectToAction(nameof(PharmacyInformation), ControllerType.Patient.ToString(), new { area = PortalType.PatientPortal.ToString() });
                    }
                }
                else
                {
                    action = View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                action = RedirectToAction(nameof(PharmacyInformation), ControllerType.Patient.ToString(), new { area = PortalType.PatientPortal.ToString() });
            }
            return action;
        }

        [HttpGet]
        public async Task<IActionResult> FinishAccountSetup()
        {
            try
            {
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                var naireViewModel = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
                TempData["IsHomeDelivery"] = naireViewModel.IsMedicationDelivery;
                if (sessionManager.ConsultationCategoryId == (int)ConsultationType.MedicationRefill)
                {
                    var medicationModel = await _repCollection.Patients.GetSelectedMedication(sessionManager.ConsultationId).ConfigureAwait(true);
                    if (medicationModel != null)
                    {
                        var chooseMedicalmodel = new ChooseYourMedicationModel()
                        {
                            MedicationName = medicationModel.MedicationName,
                            MedicationUnit = "30"
                        };
                        naireViewModel.chooseYourMedicationModel = chooseMedicalmodel;
                    }
                }
                else
                {
                    var medications = await _repCollection.Patients.GetMedications(sessionManager.ConsultationCategoryId).ConfigureAwait(true);
                    var chooseMedicalmodel = new ChooseYourMedicationModel()
                    {
                        MedicationName = medications.Find(x => x.MedicationId == (int)naireViewModel.MedicationId).MedicationName,
                        MedicationUnit = "30"
                    };
                    naireViewModel.chooseYourMedicationModel = chooseMedicalmodel;
                }
                TempData["MedicationName"] = naireViewModel.chooseYourMedicationModel.MedicationName;
                _userService.SetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString(), naireViewModel);
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinishAccountSetup(AccountSetupED model)
        {
            ActionResult action;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model != null)
                    {
                        var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                        var consultationDetailResult = await _repCollection.Patients.SaveTempConsultationDetail(sessionManager.ConsultationId, _userService.GetUserId(), model.DetailId).ConfigureAwait(true);
                        if (consultationDetailResult.Succeeded)
                        {
                            var oldSessionModel = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
                            oldSessionModel.DetailId = model.DetailId;
                            oldSessionModel.finishAccountSetupModel = new FinishAccountSetupModel()
                            {
                                AddressLine1 = model.AddressLine1,
                                AddressLine2 = model.AddressLine2,
                                City = model.City,
                                DateOfBirth = model.DateOfBirth,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                PhoneNumber = model.PhoneNumber,
                                State = model.State,
                                ZipCode = model.ZipCode
                            };
                            oldSessionModel.UserId = _userService.GetUserId();
                            oldSessionModel.ConsultationCategoryId = sessionManager.ConsultationCategoryId;
                            oldSessionModel.ConsultationId = sessionManager.ConsultationId;
                            var consultationDetails = (await _repCollection.Patients
                                .GetDetailViews(oldSessionModel.ConsultationCategoryId, (bool)oldSessionModel.IsMedicationDelivery).ConfigureAwait(true))
                                .Where(x => x.Id == oldSessionModel.DetailId)
                                .FirstOrDefault();
                            oldSessionModel.chooseYourMedicationModel.Refills = consultationDetails.Refill;
                            if (consultationDetails.Refill == 0)
                            {
                                oldSessionModel.chooseYourMedicationModel.MedicationQuantity = $"";
                            }
                            else if (consultationDetails.Refill == 1)
                            {
                                oldSessionModel.chooseYourMedicationModel.MedicationQuantity = $" with {consultationDetails.Refill} refill";
                            }
                            else
                            {
                                oldSessionModel.chooseYourMedicationModel.MedicationQuantity = $" with {consultationDetails.Refill} refills";
                            }
                            var shippingResult = await _repCollection.Patients.SaveShippingDetail(oldSessionModel).ConfigureAwait(true);
                            if (shippingResult.Succeeded)
                            {
                                var userResult = await _userManager.UpdateAccountInfo(oldSessionModel.finishAccountSetupModel, _userService.GetUserId()).ConfigureAwait(true);
                                if (userResult.Succeeded)
                                {
                                    _userService.SetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString(), oldSessionModel);
                                    action = RedirectToAction(nameof(Payeezy), "Patient", new { area = PortalType.PatientPortal.ToString() });
                                }
                                else
                                {
                                    action = RedirectToAction(nameof(FinishAccountSetup), ControllerType.Patient.ToString(), new { area = PortalType.PatientPortal.ToString() });
                                }
                            }
                            else
                            {
                                action = RedirectToAction(nameof(FinishAccountSetup), ControllerType.Patient.ToString(), new { area = PortalType.PatientPortal.ToString() });
                            }
                        }
                        else
                        {
                            action = RedirectToAction(nameof(FinishAccountSetup), ControllerType.Patient.ToString(), new { area = PortalType.PatientPortal.ToString() });
                        }
                    }
                    else
                    {
                        action = RedirectToAction(nameof(FinishAccountSetup), ControllerType.Patient.ToString(), new { area = PortalType.PatientPortal.ToString() });
                    }
                }
                else
                {
                    TempData["IsHomeDelivery"] = TempData["IsHomeDelivery"];
                    TempData["MedicationName"] = TempData["MedicationName"];
                    action = View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                action = RedirectToAction(nameof(FinishAccountSetup), ControllerType.Patient.ToString(), new { area = PortalType.PatientPortal.ToString() });
            }
            return action;
        }

        [HttpGet]
        public IActionResult FinishAccountSetupHairLoss()
        {
            try
            {
                var naireViewModel = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
                ViewBag.IsHomeDelivery = naireViewModel.IsMedicationDelivery;
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinishAccountSetupHairLoss(FinishAccountSetupModel accountSetupModel)
        {
            try
            {
                var questionNairePrev = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
                if (ModelState.IsValid)
                {
                    if (accountSetupModel != null)
                    {
                        var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                        questionNairePrev.finishAccountSetupModel = accountSetupModel;
                        questionNairePrev.UserId = _userService.GetUserId();
                        questionNairePrev.ConsultationCategoryId = sessionManager.ConsultationCategoryId;
                        questionNairePrev.ConsultationId = sessionManager.ConsultationId;
                        _userService.SetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString(), questionNairePrev);
                        return RedirectToAction(nameof(ConsultationOptions), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else
                    {
                        ViewBag.IsHomeDelivery = questionNairePrev.IsMedicationDelivery;
                        return View();
                    }
                }
                else
                {
                    ViewBag.IsHomeDelivery = questionNairePrev.IsMedicationDelivery;
                    return View();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public IActionResult ConsultationOptions()
        {
            try
            {
                var questionNaire = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
                ViewBag.IsHomeDelivery = questionNaire.IsMedicationDelivery;
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConsultationOptions(int? MedicationId, int? MedicationDetailId)
        {
            try
            {
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                var prevModel = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
                var medicationIdStatus = await _repCollection.Patients.IsConsultationMedicationExist(MedicationId).ConfigureAwait(true);
                if (medicationIdStatus)
                {
                    var detailIdStatus = await _repCollection.Patients.IsConsultationDetailExist(MedicationDetailId).ConfigureAwait(true);
                    if (detailIdStatus)
                    {
                        prevModel.MedicationId = MedicationId;
                        prevModel.DetailId = MedicationDetailId;
                        prevModel.UserId = _userService.GetUserId();
                        prevModel.ConsultationCategoryId = sessionManager.ConsultationCategoryId;
                        prevModel.ConsultationId = sessionManager.ConsultationId;
                        var consultationDetails = (await _repCollection.Patients
                                .GetDetailViews(prevModel.ConsultationCategoryId, (bool)prevModel.IsMedicationDelivery).ConfigureAwait(true))
                                .Where(x => x.Id == prevModel.DetailId).FirstOrDefault();
                        var medications = await _repCollection.Patients.GetMedications(sessionManager.ConsultationCategoryId).ConfigureAwait(true);
                        prevModel.chooseYourMedicationModel = new ChooseYourMedicationModel()
                        {
                            MedicationName = medications.Find(x => x.MedicationId == prevModel.MedicationId).MedicationName,
                            MedicationUnit = "90",
                            Description = ConsultationDescription(consultationDetails.Refill, consultationDetails.ConsultationCategoryId, consultationDetails.IsHomeDelivery),
                            MedicationPrice = consultationDetails.MedicationRate,
                            Refills = consultationDetails.Refill
                        };
                        _userService.SetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString(), prevModel);
                        await _repCollection.Patients.SaveShippingDetail(prevModel).ConfigureAwait(true);
                        await _userManager.UpdateAccountInfo(prevModel.finishAccountSetupModel, _userService.GetUserId()).ConfigureAwait(true);
                        return RedirectToAction(nameof(Payeezy), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else
                    {
                        ViewBag.IsHomeDelivery = prevModel.IsMedicationDelivery;
                        return View();
                    }
                }
                else
                {
                    ViewBag.IsHomeDelivery = prevModel.IsMedicationDelivery;
                    return View();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
        #region [Action Page]

        [HttpPost]
        public async Task<IActionResult> MedicineDose(IEnumerable<MedicineDoseModal> medicines, bool isMedicine)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = false };
            try
            {
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                var crudResult = await _repCollection.Patients.AddMedicineDose(medicines, _userService.GetUserId(), sessionManager.ConsultationCategoryId, sessionManager.ConsultationId, isMedicine).ConfigureAwait(true);
                ajaxResponse.Succeeded = crudResult.Succeeded;
            }
            catch (Exception ex)
            {
                ajaxResponse.Succeeded = false;
                ajaxResponse.ErrorMessage = ex.Message;
            }
            return Json(ajaxResponse);
        }

        #endregion

        #region [Payment]

        [HttpGet]
        public async Task<IActionResult> Payeezy()
        {
            try
            {
                var model = new PayeezyView();
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                var shoppingModel = _userService.GetSeesionvalue<QuestionNaireViewModel>(SessionKey.QuestionNaire.ToString());
                var user = await _userManager.GetUserDetails(_userService.GetUserId()).ConfigureAwait(true);
                if (shoppingModel.chooseYourMedicationModel.MedicationPrice > 0)
                {
                    model.Login = _environmentalResource.PayeezyCredentials.PayeezyLogin;
                    model.Amount = "" + Math.Round(shoppingModel.chooseYourMedicationModel.MedicationPrice, 2);
                    model.SequenceNumber = "" + DateTime.UtcNow.Ticks;
                    model.TimeStamp = Comman.GetTimeStamp();
                    model.CurrencyCode = CurrencyType.USD.ToString();
                    model.Hash = Comman.GetHashedMessage(model.Login + "^" + model.SequenceNumber + "^" + model.TimeStamp + "^" + model.Amount + "^" + model.CurrencyCode, _environmentalResource.PayeezyCredentials.TranscationKey);
                    model.MerchantCookie1 = "" + sessionManager.ConsultationCategoryId;
                    model.MerchantCookie2 = string.Empty;
                    model.PaymentGateway = _environmentalResource.PayeezyCredentials.PayeezyUrl;
                    model.InvoiceNumber = "" + DateTime.UtcNow.ToString("ddMMyyHHmmss") + Comman.GenerateOTP(3);
                    model.OrderId = "" + DateTime.UtcNow.Ticks + Comman.GenerateOTP(6);
                    model.PatientId = "" + user.RowId;
                    model.FirstName = shoppingModel.finishAccountSetupModel.FirstName.Substring(0, shoppingModel.finishAccountSetupModel.FirstName.Length > 14 ? 14 : shoppingModel.finishAccountSetupModel.FirstName.Length);
                    model.LastName = shoppingModel.finishAccountSetupModel.LastName.Substring(0, shoppingModel.finishAccountSetupModel.LastName.Length > 14 ? 14 : shoppingModel.finishAccountSetupModel.LastName.Length);
                    model.SetProducts(new List<string>() { $"1<|>{shoppingModel.chooseYourMedicationModel.MedicationName}<|>{shoppingModel.chooseYourMedicationModel.MedicationName}<|>1<|>{shoppingModel.chooseYourMedicationModel.MedicationPrice}<|>YES" });
                    await _repCollection.Patients.SetOrderIdConsultation(model.InvoiceNumber, sessionManager.ConsultationId).ConfigureAwait(true);
                    return View(model);
                }
                else
                {
                    _logger.LogInformation("Payeezy payment amount is less than and equcal to zero", "Test");
                    return RedirectToAction(nameof(Index), ControllerType.Home.ToString(), new { area = PortalType.PatientPortal.ToString() });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }
    
        [HttpGet]
        public async Task<IActionResult> CheckOutforMessage()
        {
            var model = new PayeezyView();
            try
            {
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                var user = await _userManager.GetUserDetails(_userService.GetUserId()).ConfigureAwait(true);
                if (sessionManager == null)
                {
                    sessionManager = new SessionManager();
                }
                model.Login = _environmentalResource.PayeezyCredentials.PayeezyLogin;
                model.Amount = _resourcesConfig.WeekChatAmount;
                model.SequenceNumber = "" + DateTime.UtcNow.Ticks;
                model.TimeStamp = Comman.GetTimeStamp();
                model.CurrencyCode = CurrencyType.USD.ToString();
                model.Hash = Comman.GetHashedMessage(model.Login + "^" + model.SequenceNumber + "^" + model.TimeStamp + "^" + model.Amount + "^" + model.CurrencyCode, _environmentalResource.PayeezyCredentials.TranscationKey);
                model.MerchantCookie1 = "4";
                model.MerchantCookie2 = string.Empty;
                model.PaymentGateway = _environmentalResource.PayeezyCredentials.PayeezyUrl;
                model.SetProducts(new List<string>() { "1<|>Weekly Chat<|>Weekly Chat<|>1<|>" + _resourcesConfig.WeekChatAmount + "<|>YES" });
                model.FirstName = user.FirstName.Substring(0, user.FirstName.Length >14 ? 14: user.FirstName.Length);
                model.LastName = user.LastName.Substring(0, user.LastName.Length > 14 ? 14 : user.LastName.Length);
                model.InvoiceNumber = "" + DateTime.UtcNow.ToString("ddMMyyHHmmss") + Comman.GenerateOTP(3);
                model.OrderId = "" + DateTime.UtcNow.Ticks + Comman.GenerateOTP(6);
                model.PatientId = "" + user.RowId;
                sessionManager.MessagePayment = true;
                _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                await _repCollection.Patients.SetOrderIdChat(model.InvoiceNumber, _userService.GetUserId()).ConfigureAwait(true);
            }
            catch (Exception)
            {
                throw;
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult PayeezyResponse()
        {
            try
            {
                var forms = HttpContext.Request.Form;
                string transcationId = string.Empty;
                int redirectionKey = 0;
                if (forms["x_response_code"].ToString() == "1")
                {
                    transcationId = forms["x_trans_id"].ToString();
                    redirectionKey = Convert.ToInt32(forms["merchant_cookie_1"].ToString());
                }
                return RedirectToAction(nameof(PaymentResponse), "Patient", new { area = PortalType.PatientPortal.ToString(), status = redirectionKey, txnid = transcationId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
                return RedirectToAction(nameof(Index), "Home", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult PayeezyResponse(string status)
        {
            return RedirectToAction(nameof(PaymentResponse), "Patient", new { area = PortalType.PatientPortal.ToString(), status = 0, txnid = string.Empty });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentResponse(int? status, string txnid)
        {
            try
            {
                string userId = _userService.GetUserId();
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                if (!string.IsNullOrEmpty(txnid))
                {
                    if (status == 4)
                    {
                        var payments = new PaymentInfo()
                        {
                            TxnID = txnid,
                            Amount = 9,
                            PaymentType = "WeekChat",
                            Status = "capture",
                            UserId = _userService.GetUserId(),
                            PaymentDate = DateTime.UtcNow,
                        };
                        var result = await _repCollection.Patients.SavePayment(payments).ConfigureAwait(true);
                        if (result.Succeeded)
                        {
                            await _userManager.UpdateWeekChatStatus(_userService.GetUserId(), payments.PaymentDate).ConfigureAwait(true);
                            return RedirectToAction(nameof(Messages), "Patient", new { area = PortalType.PatientPortal.ToString() });
                        }
                    }
                    else
                    {
                       // var htmlbody = await _repCollection.AmazonCustom.GetTemplate
                       // (
                       //$"{_environmentalResource.AWSS3Credentials.EmailTemplateDirectory}/{ EmailTemplate.Confirmation}.html",
                       // _environmentalResource.AWSS3Credentials.BucketName,
                       // _environmentalResource.AWSS3Credentials.AccessKeyId,
                       // _environmentalResource.AWSS3Credentials.SecretAccessKey
                       // )
                       // .ConfigureAwait(true);
                       // _email.PlaintTextGmail(EmailTemplate.Confirmation.ToString(), User.Identity.Name, _resourcesConfig.ConfirmationEmailSubject, htmlbody);
                        await _repCollection.Patients.SavePaymentDetails(userId, sessionManager.ConsultationCategoryId, txnid, sessionManager.ConsultationId)
                            .ConfigureAwait(true);
                    }
                }
                else
                {
                    if (sessionManager.MessagePayment == true)
                    {
                        sessionManager.MessagePayment = false;
                        _userService.SetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString(), sessionManager);
                        return RedirectToAction(nameof(Messages), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else if (sessionManager.ConsultationCategoryId == (int)ConsultationType.HairLoss)
                    {
                        return RedirectToAction(nameof(ConsultationOptions), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                    else
                    {
                        return RedirectToAction(nameof(FinishAccountSetup), "Patient", new { area = PortalType.PatientPortal.ToString() });
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message,ex);
                return RedirectToAction(nameof(Index), "Home", new { area = PortalType.PatientPortal.ToString() });
            }
        }

        [HttpGet]
        public IActionResult PaymentFailure()
        {
            return View();
        }

        [HttpGet]
        public async Task<JsonResult> GetMedicationCategory()
        {
            var medi = (await _repCollection.Patients.GetMedicationCategories().ConfigureAwait(true)).Select(x => new { x.Id, x.Name }).ToList();
            var jsonstr = JsonConvert.SerializeObject(medi);
            return Json("" + jsonstr + "");
        }

        [HttpGet]
        public async Task<JsonResult> GetMedication(int medicationcatId)
        {
            var medi = (await _repCollection.Patients.GetMedication(medicationcatId).ConfigureAwait(true)).Select(x => new { x.Id, x.Name }).ToList();
            var jsonstr = JsonConvert.SerializeObject(medi);
            return Json("" + jsonstr + "");
        }

        #endregion

        #region [FileUpload]

        [HttpPost]
        public async Task<IActionResult> MedicineUpload()
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = true };
            try
            {
                var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
                var files = Request.Form.Files;
                string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload");
                List<string> list = new List<string>();
                for (int i = 0; i < files.Count; i++)
                {
                    //string filename = await _repCollection.AmazonCustom.UploadFileToS3(files[i], _environmentalResource.AWSS3Credentials.BucketName, _environmentalResource.AWSS3Credentials.AccessKeyId, _environmentalResource.AWSS3Credentials.SecretAccessKey, _environmentalResource.AWSS3Credentials.ImageDirectory).ConfigureAwait(true);
                    string filename =  Comman.UploadFile(files[i], path);
                    list.Add(filename);
                }
                var crudResult = await _repCollection.Patients.SaveMedicineImage(list, _userService.GetUserId(), sessionManager.ConsultationId).ConfigureAwait(true);
                if (crudResult.Succeeded)
                {
                    ajaxResponse.Succeeded = true;
                }
                else
                {
                    ajaxResponse.Succeeded = false;
                    ajaxResponse.ErrorMessage = crudResult.ErrorMassage;
                }
            }
            catch (Exception ex)
            {
                ajaxResponse.Succeeded = false;
                ajaxResponse.ErrorMessage = ex.Message;
            }
            return Json(ajaxResponse);
        }

        [HttpPost]
        public async Task<IActionResult> PhotoIdUpload()
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = true };
            try
            {
                var files = Request.Form.Files;
                string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload");
                string Selfie = string.Empty;
                string PhotoId = string.Empty;
                for (int i = 0; i < files.Count; i++)
                {
                    if (files[i].Name.Contains("selfie"))
                    {
                        //Selfie =  await _repCollection.AmazonCustom.UploadFileToS3(files[i], _environmentalResource.AWSS3Credentials.BucketName, _environmentalResource.AWSS3Credentials.AccessKeyId, _environmentalResource.AWSS3Credentials.SecretAccessKey, _environmentalResource.AWSS3Credentials.ImageDirectory).ConfigureAwait(true);
                        Selfie = Comman.UploadFile(files[i], path);
                    }
                    else if (files[i].Name.Contains("photo"))
                    {
                        //PhotoId = await _repCollection.AmazonCustom.UploadFileToS3(files[i], _environmentalResource.AWSS3Credentials.BucketName, _environmentalResource.AWSS3Credentials.AccessKeyId, _environmentalResource.AWSS3Credentials.SecretAccessKey, _environmentalResource.AWSS3Credentials.ImageDirectory).ConfigureAwait(true);
                        PhotoId = Comman.UploadFile(files[i], path);
                    }
                }
                var crudResult = await _userManager.SavePhotoIdImage(Selfie, PhotoId, _userService.GetUserId()).ConfigureAwait(true);
                if (crudResult.Succeeded)
                {
                    ajaxResponse.Succeeded = true;
                }
                else
                {
                    ajaxResponse.Succeeded = false;
                    ajaxResponse.ErrorMessage = crudResult.ErrorMassage;
                }
            }
            catch (Exception ex)
            {
                ajaxResponse.Succeeded = false;
                ajaxResponse.ErrorMessage = ex.Message;
            }
            return Json(ajaxResponse);
        }

        [HttpPost]
        public async Task<IActionResult> MessageUploadFile(int ConsultationCategoryId)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = true };
            try
            {
                var crudResult = new CrudResult();
                var user = new UserChat() { ConsultationCategoryId = ConsultationCategoryId };
                var file = Request.Form.Files[0];
                var type = Comman.ParseEnum<PhotoType>(file.Name.Split('-')[0]);
                List<string> list = new List<string>();
                foreach (var item in Request.Form.Files)
                {
                    string location = await _repCollection.AmazonCustom
                        .UploadFileToS3(item, _environmentalResource.AWSS3Credentials.BucketName, _environmentalResource.AWSS3Credentials.AccessKeyId, _environmentalResource.AWSS3Credentials.SecretAccessKey, _environmentalResource.AWSS3Credentials.ImageDirectory)
                        .ConfigureAwait(true);
                    list.Add(location);
                }
                if (type == PhotoType.Selfie)
                {
                    user.Message = "Selfie Photo Uploaded";
                    user.Attachment = list[0];
                    crudResult = await _userManager.SaveRetakeSelfie(list[0], _userService.GetUserId()).ConfigureAwait(true);
                }
                else if (type == PhotoType.PhotoId)
                {
                    user.Message = "Photo ID Uploaded";
                    user.Attachment = list[0];
                    crudResult = await _userManager.SaveRetakePhotoId(list[0], _userService.GetUserId()).ConfigureAwait(true);
                }
                else if (type == PhotoType.Medicine)
                {
                    user.Message = "Photo Uploaded";
                    user.Attachment = string.Join(",", list);
                    crudResult = await _repCollection.Patients.SaveRetakeMedicine(list, _userService.GetUserId(), ConsultationCategoryId).ConfigureAwait(true);
                }
                if (crudResult.Succeeded)
                {
                    var userId = _userService.GetUserId();
                    var stateId = await _userManager.GetStateId(userId).ConfigureAwait(true);
                    var providerid = await _repCollection.Patients.GetProviderDetail(stateId).ConfigureAwait(true);
                    user.SenderId = userId;
                    user.SendingDate = DateTime.UtcNow;
                    user.ReceiverId = providerid;
                    var result = await _repCollection.Patients.SaveMessages(user).ConfigureAwait(true);
                    if (result.Succeeded)
                    {
                        await _repCollection.Patients.UpdatePatientOrderStatus(userId, 7, ConsultationCategoryId).ConfigureAwait(true);
                        ajaxResponse.Succeeded = true;
                    }
                    else
                    {
                        ajaxResponse.Succeeded = false;
                        ajaxResponse.ErrorMessage = result.ErrorMassage;
                    }
                }
                else
                {
                    ajaxResponse.Succeeded = false;
                    ajaxResponse.ErrorMessage = crudResult.ErrorMassage;
                }
            }
            catch (Exception ex)
            {
                ajaxResponse.Succeeded = false;
                ajaxResponse.ErrorMessage = ex.Message;
            }
            return Json(ajaxResponse);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            var crudResult = new CrudResult();
            var sessionManager = _userService.GetSeesionvalue<SessionManager>(SessionKey.SessionManager.ToString());
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload");
            var file = Request.Form.Files[0];
            var type = file.Name.ToString();
            List<string> list = new List<string>();
            if (path != null)
            {
                if (type == "M")
                {
                    foreach (var item in Request.Form.Files)
                    {
                        var location = Comman.UploadFile(item, path);
                        list.Add(location);
                    }
                }
                else
                {
                    var location = Comman.UploadFile(file, path);
                    list.Add(location);
                }
            }
            if (type == "S")
            {
                crudResult = await _userManager.SaveRetakeSelfie(list[0], _userService.GetUserId()).ConfigureAwait(true);
            }
            else if (type == "I")
            {
                crudResult = await _userManager.SaveRetakePhotoId(list[0], _userService.GetUserId()).ConfigureAwait(true);
            }
            else if (type == "M")
            {
                crudResult = await _repCollection.Patients.SaveRetakeMedicine(list, _userService.GetUserId(), sessionManager.ConsultationCategoryId).ConfigureAwait(true);
            }
            if (crudResult.Succeeded)
            {
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }
        #endregion

        [HttpGet]
        [Route("patient-portal/requests")]
        public async Task<IActionResult> OrderHistory()
        {
            try
            {
                string userId = _userService.GetUserId();
                var timeZoneConfig = await _userManager.GetTimeZone(userId).ConfigureAwait(true);
                var orders = await _repCollection.Patients.GetOrderHistory(userId, timeZoneConfig).ConfigureAwait(true);
                return View(orders);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        [HttpGet]
        [Route("patient-portal/messages")]
        public async Task<IActionResult> Messages()
        {
            try
            {
                var timeZone = await _userManager.GetTimeZone(_userService.GetUserId()).ConfigureAwait(true);
                var consultation = (await _repCollection.Patients.GetRecentConsultation(_userService.GetUserId()).ConfigureAwait(true)).FirstOrDefault();
                var model = await _repCollection.Patients.GetMessageDetail(_userService.GetUserId(), consultation.ConsultationCategoryId, timeZone).ConfigureAwait(true);
                return View(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public async Task<PartialViewResult> BindMessage(int ConsultationCategoryId)
        {
            try
            {
                var timeZone = await _userManager.GetTimeZone(_userService.GetUserId()).ConfigureAwait(true);
                var model = await _repCollection.Patients.GetMessageDetail(_userService.GetUserId(), ConsultationCategoryId, timeZone).ConfigureAwait(true);
                return PartialView("_Messages", model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveChat(string message,int ConsultationCategoryId)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = true,ErrorMessage=string.Empty };
            try
            {
                var UserId = _userService.GetUserId();
                var StateId = await _userManager.GetStateId(UserId).ConfigureAwait(true);
                var providerid = await _repCollection.Patients.GetProviderDetail(StateId).ConfigureAwait(true);
                var user = new UserChat() { 
                    SenderId = UserId , 
                    Message = message, 
                    SendingDate = DateTime.UtcNow,
                    ReceiverId = providerid, 
                    ConsultationCategoryId = ConsultationCategoryId 
                };
                var result = await _repCollection.Patients.SaveMessages(user).ConfigureAwait(true);
                if (result.Succeeded)
                {
                    var crudresult = await _repCollection.Patients.UpdatePatientOrderStatus(UserId, 7,ConsultationCategoryId).ConfigureAwait(true);
                    if (!crudresult.Succeeded)
                    {
                        ajaxResponse.ErrorMessage = "Repository Error";
                        ajaxResponse.Succeeded = false;
                    }
                }
                else
                {
                    ajaxResponse.ErrorMessage = "Repository Error";
                    ajaxResponse.Succeeded = false;
                }
            }
            catch (Exception ex)
            {
                ajaxResponse.ErrorMessage = ex.Message;
                ajaxResponse.Succeeded = false;
            }
            return Json(ajaxResponse);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PayeezyRelayResponse()
        {
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using FileStream fileStream = new FileStream(path + "/payeezy.txt", FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            try
            {
                string ResponseCode = Request.Form["x_response_code"].ToString().Trim();  //  Case 1 = success , case 2 failed
                string ResponseText = Request.Form["x_response_reason_text"].ToString().Trim();
                string exact_ctr = Request.Form["exact_ctr"].ToString().Trim();
                string Issuer = Request.Form["exact_issname"].ToString().Trim();
                string ConfirmationNumber = Request.Form["exact_issconf"].ToString().Trim();
                string TrackUrl = "http://merchant.com/order_tracking/" + Request.Form["x_invoice_num"].ToString().Trim();
                string Response = "File Modified DateTime UTC="+DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt")+"&ResponseCode =" + ResponseCode + "&ResponseText=" + ResponseText + "&Exact_ctr=" + exact_ctr + "&Issuer=" + Issuer + "&ConfirmationNumber=" + ConfirmationNumber + "&TrackUrl=" + TrackUrl;
                streamWriter.WriteLine("" + DateTime.UtcNow.ToString("dd/MM/yyyy HHmm") + " " + Response);
                if (ResponseCode == "1")
                {
                    await _repCollection.Patients.SavePaymentDetails(Convert.ToInt32(Request.Form["x_cust_id"].ToString()), Request.Form["x_invoice_num"].ToString()).ConfigureAwait(true);
                }
            }
            catch (Exception ex)
            {
                streamWriter.WriteLine("" + DateTime.UtcNow.ToString("dd/MM/yyyy HHmm") + " " + ex.Message);
            }
            finally
            {
                streamWriter.Close();
                streamWriter.Dispose();
            }
            return RedirectToAction(nameof(Index), "Home", new { area = PortalType.PatientPortal.ToString() });
        }

        private static string ConsultationDescription(int refill, int categoryId, bool isHomeDelivery)
        {
            if (categoryId == 1 || categoryId == 2)
            {
                if (isHomeDelivery)
                {
                    return "Physician consultation + 1 prescription(" + ((refill + 1) * 30) + " tablets)";
                }
                else
                {
                    string returnString = "Physician consultation + 1 prescription (30 day supply)";
                    switch (refill)
                    {
                        case 0:
                            break;
                        case 1:
                            returnString += " + 1 refill (30 day supply)";
                            break;
                        case 2:
                            returnString += " + 2 refill (30 day supply each)";
                            break;
                    }
                    return returnString;
                }
            }
            else
            {
                if (isHomeDelivery)
                {
                    return "Physician consultation + 1 prescription(" + ((refill + 1) * 90) + " tablets)";
                }
                else
                {
                    string returnString = "Physician consultation + 1 prescription for a 3 month supply (90 tablets)";
                    switch (refill)
                    {
                        case 0:
                            break;
                        case 1:
                            returnString += " + 1 refill (90 tablets)";
                            break;
                        default:
                            returnString += " + " + refill + " refills (90 tablets each)";
                            break;
                    }
                    return returnString;
                }
            }
        }
    }
}
