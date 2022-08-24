using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Repo.Contracts;
using TylerHendricks_Web.Claim;
using static TylerHendricks_Utility.Enums.Areas;
using static TylerHendricks_Utility.Comman;
using System.Web;
using TylerHendricks_Core.ViewModel;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using TylerHendricks_Utility;
using Newtonsoft.Json;
using TylerHendricks_Web.Filters;

namespace TylerHendricks_Web.Areas.PhysicianPortal.Controllers
{
    [Area("PhysicianPortal")]
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
            , EnvironmentalResource environmentalResource)
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

        [HttpGet]
        [AllowAnonymous]
        [Route("provider-portal")]
        [ServiceFilter(typeof(CustomLoginedUserAuthentication))]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("provider-portal")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            if (ModelState.IsValid)
            {
                loginModel.Role = RoleType.Physician.ToString();
                LoginType loginType = await _userManager.SignIn(loginModel).ConfigureAwait(true);
                if (loginType == LoginType.Success)
                {
                    return RedirectToAction(nameof(PhysicianDashboard), "Home", new { area = PortalType.PhysicianPortal.ToString() });
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
        [Route("provider-portal/ForGotPassword")]
        public IActionResult ForGotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("provider-portal/ForGotPassword")]
        public async Task<IActionResult> ForGotPassword(ForgotModel resetPassword)
        {
            if (ModelState.IsValid)
            {
                if (resetPassword == null)
                {
                    return RedirectToAction(nameof(ForGotPassword));
                }
                resetPassword.Role = RoleType.Physician.ToString();
                PasswordToken passToken = await _userManager.ResetPasswordToken(resetPassword).ConfigureAwait(true);
                if (passToken != null)
                {
                    var PassworfLink = Url.Action(nameof(ResetPassword), "Home", new { area = PortalType.PhysicianPortal.ToString(), TokenId = passToken.Id, Token = passToken.Token }, Request.Scheme);
                    var htmlbody = await _repCollection.AmazonCustom.GetTemplate
                       (
                       $"{_environmentalResource.AWSS3Credentials.EmailTemplateDirectory}/{EmailTemplate.ResetPassword.ToString()}.html",
                       _environmentalResource.AWSS3Credentials.BucketName,
                       _environmentalResource.AWSS3Credentials.AccessKeyId,
                       _environmentalResource.AWSS3Credentials.SecretAccessKey
                       )
                       .ConfigureAwait(true);
                    List<KeyValuePair<string, string>> placeHolders = new List<KeyValuePair<string, string>>() { new KeyValuePair<string, string>("{{ResetLink}}", PassworfLink) };
                    string body = Comman.UpdatePaceHolder(htmlbody, placeHolders);
                    _email.PlaintTextGmail(EmailTemplate.ResetPassword.ToString(), resetPassword.Email, _resourcesConfig.ResetPasswordEmailSubject, body);
                    TempData["alert"] = AlertType.ForgotPassword;
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    ModelState.AddModelError("", _resourcesConfig.ErrorMessageForUsername);
                    return View();
                }
            }
            else
            {
                return RedirectToAction(nameof(ForGotPassword));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string TokenId, string Token)
        {
            if (string.IsNullOrEmpty(TokenId) || string.IsNullOrEmpty(Token))
            {
                return RedirectToAction(nameof(ForGotPassword), "Home", new { area = PortalType.PhysicianPortal.ToString() });
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
        [Authorize(Roles = "Physician")]
        public IActionResult Profile()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _userManager.SignOut().ConfigureAwait(true);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize(Roles = "Physician")]
        [Route("provider-portal/dashboard")]
        public IActionResult PhysicianDashboard()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> GetPatientRecords(int RecordType)
        {
            var tableLoad = new DataTableLoad();
            var load = tableLoad.LoadData(HttpContext);
            try
            {
                var stateId = await _userManager.GetStateId(_userService.GetUserId()).ConfigureAwait(true);
                var timeZoneConfig = await _userManager.GetTimeZone(_userService.GetUserId()).ConfigureAwait(true);
                var patients = await _repCollection.Physician.GetInformationModels(RecordType, stateId, timeZoneConfig, load.SortColumn, load.SortColumnDirection, load.SearchValue, load.PageSize, load.Start, load.Skip).ConfigureAwait(true);
                var recordTotal = (patients != null) ? patients.Count > 0 ? patients[0].TotalRows : 0 : 0;
                if (patients == null || recordTotal == 0)
                {
                    return Json(new { draw = load.Draw, recordsFiltered = 0, recordsTotal = 0, data = new List<ProviderDashboard>() });
                }
                if (load.PageSize == -1)
                {
                    load.PageSize = recordTotal;
                }
                return Json(new { draw = load.Draw, recordsFiltered = recordTotal, recordsTotal = recordTotal, data = patients });
            }
            catch (Exception)
            {
                return Json(new { draw = load.Draw, recordsFiltered = 0, recordsTotal = 0, data = new List<ProviderDashboard>() });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> GetNotifyStates()
        {
            var tableLoad = new DataTableLoad();
            var load = tableLoad.LoadData(HttpContext);
            try
            {
                var patients = await _repCollection.Physician.GetNotify().ConfigureAwait(true);
                if (patients != null)
                {
                    if (!string.IsNullOrEmpty(load.SearchValue) && !string.IsNullOrEmpty(load.SortColumnDirection))
                    {
                        if (load.SearchValue.ToLower() == "email" && load.SortColumnDirection.ToLower() == "asc")
                        {
                            patients = patients.AsQueryable().OrderBy(x => x.Email).ToList();
                        }
                        else
                        {
                            patients = patients.AsQueryable().OrderByDescending(x => x.Email).ToList();
                        }
                        if (load.SearchValue.ToLower() == "statecode" && load.SortColumnDirection.ToLower() == "asc")
                        {
                            patients = patients.AsQueryable().OrderBy(x => x.StateCode).ToList();
                        }
                        else
                        {
                            patients = patients.AsQueryable().OrderByDescending(x => x.StateCode).ToList();
                        }
                    }
                }
                if (!string.IsNullOrEmpty(load.SearchValue))
                {
                    patients = patients.Where(
                       m => m.Email.ToLower().Contains(load.SearchValue.ToLower()) ||
                       m.StateCode.ToLower().Contains(load.SearchValue.ToLower())
                      ).ToList();
                }
                var recordsTotal = (patients != null) ? patients.Count() : 0;
                if (patients == null || recordsTotal == 0)
                {
                    return Json(new { draw = load.Draw, recordsFiltered = 0, recordsTotal = 0, data = new List<Notify>() });
                }
                if (load.PageSize == -1)
                {
                    load.PageSize = recordsTotal;
                }
                return Json(new { draw = load.Draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = patients });
            }
            catch (Exception)
            {
                return Json(new { draw = load.Draw, recordsFiltered = 0, recordsTotal = 0, data = new List<Notify>() });
            }
        }


        #region [Patient Chart]

        [HttpGet]
        [Authorize(Roles = "Physician")]
        [Route("provider-portal/patient-chart")]
        public async Task<IActionResult> PatientChart()
        {
            try
            {
                var patient = new PatientChartViewModel();
                if (Request.Query.Count > 0)
                {
                    var query = Request.Query.First().Key;
                    var cyperText = HttpUtility.UrlDecode(query);
                    var consultationIdPlainText = Decrypt(cyperText).Split('=')[1];
                    var StateId = await _userManager.GetStateId(_userService.GetUserId()).ConfigureAwait(true);
                    var TimeZoneoffset =  await _userManager.GetTimeZone(_userService.GetUserId()).ConfigureAwait(true);
                    var resultSet = await _repCollection.Physician.GetPatientByConsultationId(StateId, consultationIdPlainText).ConfigureAwait(true);
                    patient = await _repCollection.Physician.GetPatientChart(resultSet.Status, StateId, TimeZoneoffset, resultSet.RowNo, resultSet.PatientId, resultSet.ConsultationId).ConfigureAwait(true);
                    patient.RecordType = resultSet.Status;
                    patient.TotalRecord = resultSet.TotalRows;
                }
                return View(patient);
            }
            catch (Exception ex)
            {
                var exceptionType = ex.GetType().Name;
                await _userManager.SignOut().ConfigureAwait(true);
                return RedirectToAction(nameof(Login), "Home", new { area = PortalType.PhysicianPortal.ToString() });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Physician")]
        public async Task<IActionResult> GetPatientChart(string ConsultationId)
        {
            try
            {
                var patient = new PatientChartViewModel();
                var StateId = await _userManager.GetStateId(_userService.GetUserId()).ConfigureAwait(true);
                var TimeZoneoffset = await _userManager.GetTimeZone(_userService.GetUserId()).ConfigureAwait(true);
                var result = await _repCollection.Physician.GetPatientByConsultationId(StateId, ConsultationId).ConfigureAwait(true);
                if (result != null)
                {
                    patient = await _repCollection.Physician.GetPatientChart(result.Status, StateId, TimeZoneoffset, result.RowNo, result.PatientId, result.ConsultationId).ConfigureAwait(true);
                    patient.RecordType = result.Status;
                    patient.TotalRecord = result.TotalRows;
                }
                return View("_PatientChart", patient);
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(PhysicianDashboard), "Home", new { area = PortalType.PhysicianPortal.ToString() });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Physician")]
        public async Task<IActionResult> GetPatientChartByRowNo(int recordType, int rowNo)
        {
            try
            {
                var patient = new PatientChartViewModel();
                var StateId = await _userManager.GetStateId(_userService.GetUserId()).ConfigureAwait(true);
                var TimeZoneoffset = await _userManager.GetTimeZone(_userService.GetUserId()).ConfigureAwait(true);
                var result = await _repCollection.Physician.GetPatientByRowNo(StateId, rowNo, recordType).ConfigureAwait(true);
                if (result != null)
                {
                    patient = await _repCollection.Physician.GetPatientChart(result.Status, StateId, TimeZoneoffset, result.RowNo, result.PatientId, result.ConsultationId).ConfigureAwait(true);
                    patient.RecordType = result.Status;
                    patient.TotalRecord = result.TotalRows;
                }
                return View("_PatientChart", patient);
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(PhysicianDashboard), "Home", new { area = PortalType.PhysicianPortal.ToString() });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> RetakeRequests(string PatientId, string ConsultationId, string Message, string Action)
        {
            try
            {
                var result = new CrudResult();
                var userId = _userService.GetUserId();
                if (Action == "SE")
                {
                    result = await _userManager.RetakeSelfieRequest(PatientId, userId, ConsultationId, Message).ConfigureAwait(true);
                }
                else if (Action == "PI")
                {
                    result = await _userManager.RetakePhotoIdRequest(PatientId, userId, ConsultationId, Message).ConfigureAwait(true);
                }
                else if (Action == "MI")
                {
                    result = await _userManager.RetakeMedicineImageRequest(PatientId, userId, ConsultationId, Message).ConfigureAwait(true);
                }
                else
                {
                    result = await _repCollection.Physician.SaveChat(PatientId, userId, ConsultationId, Message).ConfigureAwait(true);
                }
                if (result.Succeeded)
                {
                    var emailId = await _userManager.GetEmail(PatientId).ConfigureAwait(true);
                    var htmlbody = await _repCollection.AmazonCustom.GetTemplate
                          (
                         $"{_environmentalResource.AWSS3Credentials.EmailTemplateDirectory}/{EmailTemplate.NewMessage.ToString()}.html",
                          _environmentalResource.AWSS3Credentials.BucketName,
                          _environmentalResource.AWSS3Credentials.AccessKeyId,
                          _environmentalResource.AWSS3Credentials.SecretAccessKey
                          )
                          .ConfigureAwait(true);
                    _email.PlaintTextGmail(EmailTemplate.NewMessage.ToString(), emailId, _resourcesConfig.MessageEmailSubject, htmlbody);
                    var resultset = await _repCollection.Physician.UpdatePatientOrderStatus(PatientId, ConsultationId, 3).ConfigureAwait(true);
                    if (resultset.Succeeded)
                    {
                        string queryString = HttpUtility.UrlEncode(Encrypt("c=" + ConsultationId));
                        return Json("{\"status\":true,\"url\":\"" + queryString + "\"}");
                    }
                    return Json("{\"status\":false,\"url\":\"\"}");
                }
                else
                {
                    return Json("{\"status\":false,\"url\":\"\"}");
                }
            }
            catch (Exception)
            {
                return Json("{\"status\":false,\"url\":\"\"}");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> ChanageAccountStatus(string PatientId)
        {
            try
            {
                var result = await _userManager.AccountStatus(PatientId).ConfigureAwait(true);
                if (result.Succeeded)
                {
                    return Json("success");
                }
                else
                {
                    return Json("error");
                }
            }
            catch (Exception)
            {
                return Json("error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> ChangeOrderStatus(string PatientId, string ConsultationId, int StatusId)
        {
            try
            {
                var result = await _repCollection.Physician.UpdateOrderStatus(ConsultationId, _userService.GetUserId(), StatusId).ConfigureAwait(true);
                if (result.Succeeded)
                {
                    var Email = await _userManager.GetEmail(PatientId).ConfigureAwait(true);
                    if (StatusId == 4)
                    {
                       // var htmlbody = await _repCollection.AmazonCustom.GetTemplate
                       // (
                       //$"{_environmentalResource.AWSS3Credentials.EmailTemplateDirectory}/{EmailTemplate.PatientPrescribed.ToString()}.html",
                       // _environmentalResource.AWSS3Credentials.BucketName,
                       // _environmentalResource.AWSS3Credentials.AccessKeyId,
                       // _environmentalResource.AWSS3Credentials.SecretAccessKey
                       // )
                       // .ConfigureAwait(true);
                       // _email.PlaintTextGmail(EmailTemplate.PatientPrescribed.ToString(), Email, _resourcesConfig.PrescribedEmailSubject, htmlbody);
                    }
                    else if (StatusId == 5)
                    {
                      //  var htmlbody = await _repCollection.AmazonCustom.GetTemplate
                      // (
                      //$"{_environmentalResource.AWSS3Credentials.EmailTemplateDirectory}/{EmailTemplate.PatientDenied.ToString()}.html",
                      // _environmentalResource.AWSS3Credentials.BucketName,
                      // _environmentalResource.AWSS3Credentials.AccessKeyId,
                      // _environmentalResource.AWSS3Credentials.SecretAccessKey
                      // )
                      // .ConfigureAwait(true);
                      //  _email.PlaintTextGmail(EmailTemplate.PatientDenied.ToString(), Email, _resourcesConfig.ConsultationUpdateEmailSubject, htmlbody);
                    }
                    string queryString = HttpUtility.UrlEncode(Encrypt("c=" + ConsultationId));
                    return Json("{\"status\":true,\"url\":\"" + queryString + "\"}");
                }
                else
                {
                    return Json("{\"status\":false,\"url\":\"\"}");
                }
            }
            catch (Exception)
            {
                return Json("{\"status\":false,\"url\":\"\"}"); ;
            }
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> UploadNote()
        {
            var files = Request.Form.Files;
            string path = Path.Combine(_hostingEnvironment.WebRootPath, "Upload");
            IList<string> fileList = new List<string>();
            var userid = files[0].Name;
            var fileExtension = Path.GetExtension(files[0].FileName);
            var fileName = _userService.GetUserId() + "_" + Guid.NewGuid().ToString("N").Substring(1, 8) + "_note" + fileExtension;
            string fileUploadedPath = Path.Combine(path, fileName);
            if (System.IO.File.Exists(fileUploadedPath))
            {
                System.IO.File.Delete(fileUploadedPath);
            }
            using(FileStream stream =new FileStream(fileUploadedPath,FileMode.Create))
            {
                files[0].CopyTo(stream);
            };
            fileList.Add("/Upload/" + fileName);
            CrudResult  result= await _repCollection.Physician.SaveNote(_userService.GetUserId(), userid, fileList[0]).ConfigureAwait(true);
            if (result.Succeeded)
            {
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> ChangePatientOrderStatus(string PatientId, int StatusId, string ConsultationId)
        {
            var result = await _repCollection.Physician.UpdatePatientOrderStatus(PatientId, ConsultationId, StatusId).ConfigureAwait(true);
            if (result.Succeeded)
            {
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> SaveNotesText(string PatientId, string Notes)
        {
            var result = await _repCollection.Physician.SaveNote(_userService.GetUserId(), PatientId, Notes).ConfigureAwait(true);
            if (result.Succeeded)
            {
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> DeleteNote(int Id)
        {
            var result =  await _repCollection.Physician.DeleteNotes(Id).ConfigureAwait(true);
            if (result.Succeeded)
            {
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> PopulatePatientData(string patientId)
        {
            var PatientData = await _repCollection.Physician.GetPatientDetails(patientId).ConfigureAwait(true);
            var ReturnString = JsonConvert.SerializeObject(PatientData);
            return Json(ReturnString);
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> UpdatePatient(PatientChartViewModel viewModel)
        {
            if (viewModel == null)
            {
                return Json("error");
            }
            var Result = await _userManager.UpdatePatientInfo(viewModel.UpdatePatientView).ConfigureAwait(true);
            if (Result.Succeeded)
            {
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> PopulatePharmacyData(string consultationId)
        {
            var PharmacyData = await _repCollection.Physician.GetPharmacyDetails(consultationId).ConfigureAwait(true);
            var ReturnString = JsonConvert.SerializeObject(PharmacyData);
            return Json(ReturnString);
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> UpdatePharmacy(PatientChartViewModel viewModel)
        {
            if (viewModel == null)
            {
                return Json("error");
            }
            var Result = await _repCollection.Physician.UpdatePharmacyInfo(viewModel.UpdatePharmacyView).ConfigureAwait(true);
            if (Result.Succeeded)
            {
                return Json("success");
            }
            else
            {
                return Json("error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> PopulateMedicineData(string consultationId)
        {
            var returnResult = new CrudResult() { Failed = false, Succeeded = true };
            try
            {
                var medicines = await _repCollection.Physician.GetMedicineDetails(consultationId).ConfigureAwait(true);
                returnResult.Data= JsonConvert.SerializeObject(medicines);
                return Json(returnResult);
            }
            catch (Exception ex)
            {
                returnResult.Failed = true; 
                returnResult.Succeeded = false;
                returnResult.ErrorMassage = ex.Message; 
                returnResult.InnerErrorMessage = ex.InnerException.Message;
                return Json(returnResult);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        public async Task<JsonResult> UpdateMedicine(List<MedicineView> MedicineList,string patientId,string consultationId)
        {
            var returnResult = new CrudResult() { Failed = false, Succeeded = true };
            if (MedicineList == null)
            {
                returnResult.Failed = true;
                returnResult.Succeeded = false;
                returnResult.ErrorMassage = "Parameter is null";
                return Json(returnResult);
            }
            try 
            {
                var Result = await _repCollection.Physician.UpdateMedicineInfo(MedicineList, patientId, _userService.GetUserId(), consultationId).ConfigureAwait(true);
                returnResult.Failed = false;
                returnResult.Succeeded = true;
                return Json(returnResult);
            }
            catch(Exception ex)
            {
                returnResult.Failed = true;
                returnResult.Succeeded = false;
                returnResult.ErrorMassage = ex.Message;
                return Json(returnResult);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Physician")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> RecordMoveToAllPatient(string consultationId)
        {
            var ajaxResponse = new AjaxResponse() { Succeeded = true };
            try
            {
                var result = await _repCollection.Physician.MoveToAllPatient(consultationId).ConfigureAwait(true);
                if (result)
                {
                    ajaxResponse.Data = "success";
                    ajaxResponse.Succeeded = true;
                }
                else
                {
                    ajaxResponse.Data = "failure";
                    ajaxResponse.Succeeded = false;
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
        [HttpGet]
        [Authorize(Roles = "Physician")]
        public IActionResult ED()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Physician")]
        public IActionResult Hairloss()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Physician")]
        public IActionResult MR()
        {
            return View();
        }
    }
}
