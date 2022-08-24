using System;
using System.Collections.Generic;
using TylerHendricks_Repo.Contracts;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using Microsoft.Data.SqlClient;
using TylerHendricks_Core.ViewModel;
using static TylerHendricks_Utility.Comman;
using TylerHendricks_Data.DBEntity;
using TylerHendricks_Utility;
using System.Text.RegularExpressions;
using static TylerHendricks_Utility.Enums.Areas;
using System.Web;
using System.Data;
using System.Reflection;

namespace TylerHendricks_Repo.Services
{
    public class Physician : Repository, IPhysician
    {
        public async Task<List<ProviderDashboard>> GetInformationModels(int recordType, int stateId, TimeZoneConfig timeZone, string sortColumn,
            string sortColumnDirection, string searchValue, int pageSize, int start, int skip)
        {
            try
            {
                var returnModel = new List<ProviderDashboard>();
                var RecordTypeParam = new SqlParameter("@RecordType", recordType);
                var StateIdParam = new SqlParameter("@StateID", stateId);
                var TimeZoneParam = new SqlParameter("@TimeZone", timeZone.OffSet * -1);
                var SortColunmParam = new SqlParameter("@SortColumn", string.IsNullOrEmpty(sortColumn) == true ? "" : sortColumn);
                var SortColoumnDirectionParam = new SqlParameter("@SortColumnDirection", string.IsNullOrEmpty(sortColumnDirection) == true ? "" : sortColumnDirection);
                var SearchValueParam = new SqlParameter("@SearchValue", string.IsNullOrEmpty(searchValue) == true ? "" : searchValue);
                var PageSizeParam = new SqlParameter("@PageSize", pageSize);
                var StartParam = new SqlParameter("@Start", start);
                var SkipParam = new SqlParameter("@Skip", skip);
                var resultSet = await DBEntity.Sp_PatientRecords.FromSqlRaw
                    ("EXEC Sp_PatientRecords @RecordType,@StateID,@TimeZone,@SortColumn,@SortColumnDirection,@SearchValue,@PageSize,@Start,@Skip"
                    , RecordTypeParam, StateIdParam, TimeZoneParam, SortColunmParam, SortColoumnDirectionParam, SearchValueParam, PageSizeParam, StartParam, SkipParam).ToListAsync().ConfigureAwait(true);
                if (resultSet != null)
                {
                    foreach (var item in resultSet)
                    {
                        returnModel.Add(new ProviderDashboard()
                        {
                            ConsultationCategoryId = item.ConsultationCategoryId,
                            ConsultationCateory = item.ConsultationCateory,
                            ConsultationId=item.ConsultationId,
                            DOB = item.DOB,
                            Name = item.Name,
                            PatientId = item.PatientId,
                            Refills = item.Refills,
                            RequestedRx = item.RequestedRx,
                            StateCode = item.StateCode,
                            Status = item.Status,
                            Submitted = item.Submitted,
                            TabStatus = item.TabStatus,
                            TotalRows = item.TotalRows,
                            WaitInQuene = item.WaitInQuene,
                            RowNo = item.RowNo,
                            RowId = item.RowId,
                            QueryString= HttpUtility.UrlEncode(Encrypt("c=" + item.ConsultationId))
                        });
                    }
                }
                return returnModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<PatientChartViewModel> GetPatientChart(int recordType, int stateId, TimeZoneConfig timezone, int rowNo, string userId, string consultationId)
        {
            try
            {
                var StateIdParam = new SqlParameter("@StateId", stateId);
                var RecordTypeParam = new SqlParameter("@RecordType", recordType);
                var RowNoParam = new SqlParameter("@RowNo", rowNo);
                var TimeZoneParam = new SqlParameter("@TimeZone", timezone.OffSet * -1);
                var resultSet = await DBEntity.SP_PATIENT_CHART
                    .FromSqlRaw("EXEC SP_PATIENT_CHART @StateId,@RecordType,@RowNo,@TimeZone", StateIdParam, RecordTypeParam, RowNoParam, TimeZoneParam)
                    .ToListAsync()
                    .ConfigureAwait(true);
                int consultationCategoryId = 0;
                using (var common = new Common())
                {
                    consultationCategoryId = await common.GetConsultationCategory(consultationId).ConfigureAwait(true);
                }
                var prevMedicine = await (from m in DBEntity.UserMedication
                                          join mf in DBEntity.MedicineForm on m.MedicationFormId equals mf.Id
                                          join mfr in DBEntity.MedicineFrequency on m.MedicationFrequencyId equals mfr.Id
                                          join mu in DBEntity.MedicineUnit on m.MedicationUnitId equals mu.Id
                                          where m.IsRecordDeleted == false && m.ConsultationId == resultSet[0].ConsultationId
                                          select m.DrugName + " " + m.Dose + " " + mu.Name + " " + mf.Name + " " + mfr.Name
                                          ).ToListAsync().ConfigureAwait(true);
                var userChat = await (from a in DBEntity.AspNetUsers
                                      join p in DBEntity.PatientConsultation on a.Id equals p.SenderId
                                      join an in DBEntity.AspNetUsers on p.ReceiverId equals an.Id
                                      where (p.SenderId == resultSet[0].UserId || p.ReceiverId == resultSet[0].UserId)
                                      && p.ConsultationCategoryId == consultationCategoryId
                                      select new
                                      {
                                          SenderId = p.SenderId,
                                          ReceiverId = p.ReceiverId,
                                          SendingDate = p.AddedDate,
                                          ReplyDate = p.ModifiedDate,
                                          Message = p.Message,
                                          Reply = p.Reply,
                                          SenderName = a.FirstName + " " + a.LastName,
                                          ReceiverName = an.FirstName + " " + an.LastName,
                                          Attachment = p.Attachment
                                      }).ToListAsync().ConfigureAwait(true);

                var questionHistory = await (from a in DBEntity.AspUserAnswerMapping
                                             join c in DBEntity.ConsultationQuestionsMapping on Convert.ToInt32(a.ConsultationQuestionsMappingId) equals c.Id
                                             join cq in DBEntity.ConsultationQuestions on c.Questionid equals cq.Id
                                             where a.IsRecordDeleted == false && a.ConsultationId == resultSet[0].ConsultationId
                                             && a.Answer.ToUpper() != "NEXT"
                                             select new
                                             {
                                                 ConsultationId = a.ConsultationId,
                                                 Question = cq.QuestionDescription,
                                                 Answer = a.Answer,
                                             }).ToListAsync().ConfigureAwait(true);

                var questionNaire = await (from c in DBEntity.Consultation
                                           join s in DBEntity.ShippingInformation on c.ConsultationId equals s.ConsultationId
                                           join o in DBEntity.OrderStatusType on s.OrderStatusTypeId equals o.OrderStatusTypeId
                                           where s.IsRecordDeleted == false && c.IsRecordDeleted == false
                                           && s.UserId == resultSet[0].UserId
                                           select new
                                           {
                                               SubmissionDate = GetUserDateByTimeZone(s.RequestedDate.Value, timezone.OffSet, timezone.IsDayLightSaving).ToString("MMM dd, yyyy hh:mm tt"),
                                               RequestRx = s.ProductName,
                                               Status = o.OrderStatusType1,
                                               Link = HttpUtility.UrlEncode(Encrypt("c=" + c.ConsultationId)),
                                           }).ToListAsync().ConfigureAwait(true);

                var notes = await (from q in DBEntity.PhysicianNote
                                   where q.IsRecordDeleted == false && q.PatientId == resultSet[0].UserId
                                   select new
                                   {
                                       Id = q.Id,
                                       Filename = q.FilePath,
                                       IsFile = q.IsFile,
                                       DateAdd = GetUserDateByTimeZone(q.AddedDate.Value, timezone.OffSet, timezone.IsDayLightSaving).ToString("MMM dd, yyyy AT hh:mm tt")
                                   }).ToListAsync().ConfigureAwait(true);

                IList<Notes> _notes = new List<Notes>();
                if (notes != null)
                {
                    foreach (var item in notes)
                    {
                        _notes.Add(new Notes()
                        {
                            Id = item.Id,
                            Filename = item.Filename,
                            IsFile = item.IsFile,
                            DateAdd = item.DateAdd
                        });
                    }
                }

                IList<UserChat> userChats = new List<UserChat>();
                if (userChat != null)
                {
                    foreach (var item in userChat)
                    {
                        userChats.Add(new UserChat()
                        {
                            Message = item.Message,
                            ReceiverId = item.ReceiverId,
                            ReceiverName = item.ReceiverName,
                            Reply = item.Reply,
                            ReplyDate = Comman.GetUserDateByTimeZone(item.ReplyDate == null ? DateTime.UtcNow : item.ReplyDate.Value, timezone.OffSet, timezone.IsDayLightSaving),
                            SenderId = item.SenderId,
                            SenderName = item.SenderName,
                            SendingDate = Comman.GetUserDateByTimeZone(item.SendingDate, timezone.OffSet, timezone.IsDayLightSaving),
                            Attachment = item.Attachment
                        });
                    }
                }

                IList<ConsultationList> consultationLists = new List<ConsultationList>();
                foreach (var item in questionNaire)
                {
                    consultationLists.Add(new ConsultationList()
                    {
                        RequestRx = item.RequestRx,
                        Status = item.Status,
                        SubmissionDate = item.SubmissionDate,
                        Link = item.Link
                    });
                }

                List<string> medicationPictures = new List<string>();
                if (!string.IsNullOrEmpty(resultSet[0].MedicineFile1) && !string.IsNullOrEmpty(resultSet[0].MedicineFile2) && !string.IsNullOrEmpty(resultSet[0].MedicineFile3))
                {
                    medicationPictures.Add(resultSet[0].MedicineFile1);
                    medicationPictures.Add(resultSet[0].MedicineFile2);
                    medicationPictures.Add(resultSet[0].MedicineFile3);
                }
                else if (!string.IsNullOrEmpty(resultSet[0].MedicineFile1) && !string.IsNullOrEmpty(resultSet[0].MedicineFile2))
                {
                    medicationPictures.Add(resultSet[0].MedicineFile1);
                    medicationPictures.Add(resultSet[0].MedicineFile2);
                }
                else if (!string.IsNullOrEmpty(resultSet[0].MedicineFile1))
                {
                    medicationPictures.Add(resultSet[0].MedicineFile1);
                }

                IList<QuestionnaireHistory> questionnaires = new List<QuestionnaireHistory>();

                if (questionHistory != null)
                {
                    foreach (var item in questionHistory)
                    {
                        questionnaires.Add(new QuestionnaireHistory()
                        {
                            Answer = item.Answer,
                            ConsultationId = item.ConsultationId,
                            Question = item.Question
                        });
                    }
                }

                bool isChatBox = true;
                using (var common = new Common())
                {
                    isChatBox = await common.GetConsultationChatStatus(consultationId).ConfigureAwait(true);
                }

                return new PatientChartViewModel()
                {
                    RowNo = resultSet[0].RowNo,
                    LastOrder = resultSet[0].LastOrder,
                    LastProvider = resultSet[0].LastProvider,
                    LastRx = resultSet[0].LastRx,
                    MedicationPictures = medicationPictures,
                    Medications = prevMedicine,
                    PaitentAccount = resultSet[0].RecordStatus,
                    PatientAge = resultSet[0].Age,
                    PatientDob = resultSet[0].DOB,
                    PatientEmail = resultSet[0].Email,
                    PatientId = resultSet[0].UserId,
                    PatientName = resultSet[0].Name,
                    PatientPhoneNumber = resultSet[0].PhoneNumber,
                    PharmacyAddressLine1 = resultSet[0].Address,
                    PharmacyCity = resultSet[0].PharmacyCity,
                    PharmacyName = resultSet[0].PharmacyName,
                    PharmacyPhone = resultSet[0].PharmacyPhoneNumber,
                    PharmacyState = resultSet[0].PharmacyState,
                    PharmacyZipCode = resultSet[0].PharmacyZipCode,
                    PhotoId = resultSet[0].PhotoId,
                    PhotoIdUploadDate = resultSet[0].PhotoIdDate == null ? Comman.GetUserDateByTimeZone(DateTime.UtcNow, timezone.OffSet, timezone.IsDayLightSaving).ToString("MMM dd, yyyy AT hh:mm tt") : resultSet[0].PhotoIdDate.Value.ToString("MMM dd, yyyy AT hh:mm tt"),
                    RequestedRx = resultSet[0].RequestedRx,
                    Selfie = resultSet[0].Selfie,
                    StateCode = resultSet[0].StateCode,
                    Submitted = resultSet[0].Submitted,
                    Status = resultSet[0].Status,
                    MedicationUploadDate = resultSet[0].MedicationIdDate == null ? Comman.GetUserDateByTimeZone(DateTime.UtcNow, timezone.OffSet, timezone.IsDayLightSaving).ToString("MMM dd, yyyy AT hh:mm tt") : resultSet[0].MedicationIdDate.Value.ToString("MMM dd, yyyy AT hh:mm tt"),
                    SelfieUploadDate = resultSet[0].SelfieIdDate == null ? Comman.GetUserDateByTimeZone(DateTime.UtcNow, timezone.OffSet, timezone.IsDayLightSaving).ToString("MMM dd, yyyy AT hh:mm tt") : resultSet[0].SelfieIdDate.Value.ToString("MMM dd, yyyy AT hh:mm tt"),
                    ConsultationLists = consultationLists,
                    UserChats = userChats,
                    QuestionnaireHistories = questionnaires,
                    Notes = _notes,
                    RowId = resultSet[0].RowId,
                    Refills = resultSet[0].Refills,
                    HomeAddress = resultSet[0].HomeAddress,
                    ConsultationId = resultSet[0].ConsultationId,
                    IsChatEnabled = isChatBox
                };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<Notify>> GetNotify()
        {
            try
            {
                var list = new List<Notify>();
                var query = await DBEntity.NotifyPatients.Join(DBEntity.FacilityStates, d => d.StateId, s => s.Id, (d, s) => new { d, s })
                    .Where(x => x.d.IsRecordDelete == false)
                    .Select(a => new { Email = a.d.Email, StateCode = a.s.Code }).ToListAsync().ConfigureAwait(true);
                if (query != null)
                {
                    for (int i = 0; i < query.Count; i++)
                    {
                        list.Add(new Notify()
                        {
                            Id = i + 1,
                            Email = query[i].Email,
                            StateCode = query[i].StateCode
                        });
                    }
                }
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<PatientChartViewModel> GetPatient(string userId)
        {
           
            try
            {
                var patientChart = new PatientChartViewModel();
                var _userDetails = await (from c in DBEntity.AspNetUsers
                                          join fs in DBEntity.FacilityStates on c.StateId equals fs.Id
                                          join s in DBEntity.ShippingInformation on c.Id equals s.UserId
                                          join p in DBEntity.PharmacyInformation on s.ConsultationId equals p.ConsultationId
                                          join st in DBEntity.FacilityStates on p.StateId equals st.Id
                                          where c.IsRecordDelete == false && c.Id == userId && s.IsRecordDeleted == false
                                          && p.IsRecordDeleted == false
                                          orderby s.Id descending
                                          select new
                                          {
                                              PatientId = c.Id,
                                              Name = $"{c.FirstName} {c.LastName}",
                                              Email = c.Email,
                                              Phone = c.PhoneNumber,
                                              Dob = c.Dob == null ? "" : c.Dob.Value.ToString("MM/dd/yyyy"),
                                              Age = CalculateYourAge(c.Dob == null ? DateTime.UtcNow : Convert.ToDateTime(c.Dob)),
                                              StateCode = fs.Code,
                                              Subscribed = s.RequestedDate.Value.ToString("MMM dd, yyyy at mm:hh tt"),
                                              PatientAccount = c.RecordStatus,
                                              PharmacyName = p.PharmacyName,
                                              PharmacyAddressLine1 = p.AddressLine1,
                                              PharmacyAddressLine2 = p.AddressLine2,
                                              PharmacyCity = p.City,
                                              PharmacyState = st.Code,
                                              PharmacyZipCode = p.ZipCode,
                                              PharmacyPhone = p.PhoneNumber,
                                              PhotoId = c.PhotoId,
                                              Selfie = c.Photo
                                          }).FirstOrDefaultAsync().ConfigureAwait(true);

                patientChart.PatientId = _userDetails.PatientId;
                patientChart.PatientName = _userDetails.Name;
                patientChart.PatientEmail = _userDetails.Email;
                patientChart.PatientPhoneNumber = _userDetails.Phone;
                patientChart.PatientDob = _userDetails.Dob;
                patientChart.PatientAge = Convert.ToInt32(_userDetails.Age);
                patientChart.StateCode = _userDetails.StateCode;
                patientChart.Submitted = _userDetails.Subscribed;
                patientChart.LastOrder = "Not applicable";
                patientChart.LastProvider = "Not applicable";
                patientChart.LastRx = "Not applicable";
                patientChart.PaitentAccount = Convert.ToBoolean(_userDetails.PatientAccount);
                patientChart.PharmacyName = _userDetails.PharmacyName;
                patientChart.PharmacyAddressLine1 = _userDetails.PharmacyAddressLine1;
                patientChart.PharmacyAddressLine2 = _userDetails.PharmacyAddressLine2;
                patientChart.PharmacyCity = _userDetails.PharmacyCity;
                patientChart.PharmacyPhone = _userDetails.PharmacyPhone;
                patientChart.PharmacyZipCode = _userDetails.PharmacyZipCode;
                patientChart.PhotoId = _userDetails.PhotoId;
                patientChart.Selfie = _userDetails.Selfie;
                return patientChart;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> SaveChat(string patientId, string physicianId,string consultationId, string message)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                int consultationCategoryId = 0;
                using (var Common = new Common())
                {
                    consultationCategoryId = await Common.GetConsultationCategory(consultationId).ConfigureAwait(true);
                };
                var chat = await DBEntity.PatientConsultation
                    .Where(x => x.IsRecordDeleted == false && (x.SenderId == patientId || x.ReceiverId == patientId) 
                    && x.ConsultationCategoryId == consultationCategoryId)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                if (chat != null)
                {
                    if (string.IsNullOrEmpty(chat.Reply) && chat.ReceiverId == physicianId)
                    {
                        chat.ModifiedDate = DateTime.UtcNow;
                        chat.Reply = message;
                        DBEntity.PatientConsultation.Update(chat).Property(x => x.Id).IsModified = false;
                    }
                    else
                    {
                        DBEntity.PatientConsultation.Add(new PatientConsultation()
                        {
                            AddedDate = DateTime.UtcNow,
                            IsRecordDeleted = false,
                            Message = message,
                            SenderId = physicianId,
                            ReceiverId = patientId,
                            ConsultationCategoryId = consultationCategoryId,
                        });
                    }
                }
                else
                {
                    DBEntity.PatientConsultation.Add(new PatientConsultation()
                    {
                        AddedDate = DateTime.UtcNow,
                        IsRecordDeleted = false,
                        Message = message,
                        SenderId = physicianId,
                        ReceiverId = patientId,
                        ConsultationCategoryId = consultationCategoryId,
                    });
                }
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception ex)
            {
                transcation.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> UpdateOrderStatus(string consultationId, string userId, int statusId)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var shipping = await DBEntity.ShippingInformation
                    .Where(x => x.ConsultationId == consultationId && x.IsRecordDeleted == false)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);

                switch (statusId)
                {
                    case 4:
                        shipping.OrderStatusTypeId = 2;
                        break;
                    case 5:
                        shipping.OrderStatusTypeId = 3;
                        break;
                    default:
                        shipping.OrderStatusTypeId = shipping.OrderStatusTypeId;
                        break;
                }
                  
                if (statusId == 4)
                {
                    shipping.PrescribedDate = DateTime.UtcNow;
                    var consultation = await DBEntity.Consultation
                        .Where(x => x.ConsultationId == consultationId && x.IsRecordDeleted == false)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(true);
                    int days = 0;
                    string extraDays = shipping.ProductUnit.Replace(" ", "").Trim();
                    if (extraDays.All(char.IsDigit))
                    {
                        days = Convert.ToInt32(Regex.Match(shipping.ProductUnit, @"\d+").Value);
                    }
                    if (consultation.ConsultationCategoryId == (int)ConsultationType.ErectileDysfunction)
                    {
                        consultation.NextRefillDate = DateTime.UtcNow.AddDays(((int)shipping.Refills * 30) + days);
                    }
                    else if (consultation.ConsultationCategoryId == (int)ConsultationType.MedicationRefill)
                    {
                        consultation.NextRefillDate = DateTime.UtcNow.AddDays(((int)shipping.Refills * 30) + days);
                    }
                    else if (consultation.ConsultationCategoryId == (int)ConsultationType.HairLoss)
                    {
                        consultation.NextRefillDate = DateTime.UtcNow.AddDays(((int)shipping.Refills * 90) + days);
                    }
                    DBEntity.Entry(consultation).Property(x => x.Id).IsModified = false;
                }
                shipping.Status = statusId;
                shipping.ModifiedDate = DateTime.UtcNow;
                shipping.ModifiedBy = userId;
                DBEntity.ShippingInformation.Update(shipping).Property(x => x.Id).IsModified = false;

                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception ex)
            {
                transcation.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> SaveNote(string userId, string patientId, string filePath)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                DBEntity.PhysicianNote.Add(new PhysicianNote()
                {
                    AddedBy = userId,
                    AddedDate = DateTime.UtcNow,
                    FilePath = filePath,
                    IsFile = filePath.StartsWith("/") == true ? true : false,
                    IsRecordDeleted = false,
                    PatientId = patientId,
                    PhysicianId = userId,
                });
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception ex)
            {
                transcation.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> UpdatePatientOrderStatus(string patientId,string consultationId, int statusId)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var shipping = await (from s in DBEntity.ShippingInformation
                                        where s.IsRecordDeleted == false && s.UserId == patientId && s.ConsultationId== consultationId
                                        select s)
                                .FirstOrDefaultAsync().ConfigureAwait(true);
                shipping.Status = statusId;
                DBEntity.ShippingInformation.Update(shipping).Property(x => x.Id).IsModified = false;
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception ex)
            {
                transcation.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> DeleteNotes(int id)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var physicianNote = await (from u in DBEntity.PhysicianNote where u.Id == id select u).FirstOrDefaultAsync().ConfigureAwait(true);
                DBEntity.PhysicianNote.Remove(physicianNote);
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                transcation.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch(Exception ex)
            {
                transcation.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<MoveRecordModel> FindRowNumber(string patientId, string consultationId, int status, int stateId)
        {
            try
            {
                var StateIdParam = new SqlParameter("@StateID", stateId);
                var RecordTypeParam = new SqlParameter("@RecordType", status);
                var UserIdParam = new SqlParameter("@UserId", patientId);
                var ConsultationIdParam = new SqlParameter("@ConsultationId", consultationId);
                var resultSet = await DBEntity.SP_GETRowNumber.FromSqlRaw("exec SP_GETRowNumber @RecordType,@StateID,@UserId,@ConsultationId"
                    , RecordTypeParam, StateIdParam, UserIdParam, ConsultationIdParam).ToListAsync().ConfigureAwait(true);
                return resultSet[0];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<UpdatePatientView> GetPatientDetails(string patientId)
        {
            try
            {
                var updatePatientView = new UpdatePatientView();
                var Patient = await DBEntity.AspNetUsers.Where(x => x.Id == patientId)
                    .Select(x => new
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        PhoneNumber = x.PhoneNumber,
                        ZipCode = x.ZipCode,
                        City = x.City,
                        StateId = x.StateId,
                        Dob = x.Dob
                    }).FirstOrDefaultAsync().ConfigureAwait(true);

                if (Patient != null)
                {
                    updatePatientView.FirstName = Patient.FirstName;
                    updatePatientView.LastName = Patient.LastName;
                    updatePatientView.PhoneNumber = Patient.PhoneNumber;
                    updatePatientView.StateId = Patient.StateId;
                    updatePatientView.City = Patient.City;
                    updatePatientView.ZipCode = Patient.ZipCode;
                    updatePatientView.DobToLocal = Patient.Dob.Value == null ? "" : Patient.Dob.Value.ToString("yyyy-MM-dd");
                    return updatePatientView;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<UpdatePharmacyView> GetPharmacyDetails(string consultationId)
        {
            try
            {
                var updatePharmacyView = new UpdatePharmacyView();
                var Pharmacy = await DBEntity.PharmacyInformation
                    .Where(x => x.IsRecordDeleted == false && x.ConsultationId == consultationId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);

                if (Pharmacy != null)
                {
                    updatePharmacyView.PharmacyId = Pharmacy.PharmacyId;
                    updatePharmacyView.PharmacyName = Pharmacy.PharmacyName;
                    updatePharmacyView.PhoneNumber = Pharmacy.PhoneNumber;
                    updatePharmacyView.StateId = Pharmacy.StateId;
                    updatePharmacyView.City = Pharmacy.City;
                    updatePharmacyView.ZipCode = Pharmacy.ZipCode;
                    updatePharmacyView.AddressLine1 = Pharmacy.AddressLine1;
                    updatePharmacyView.AddressLine2 = Pharmacy.AddressLine2;
                    return updatePharmacyView;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> UpdatePharmacyInfo(UpdatePharmacyView pharmacyView)
        {
            if (pharmacyView == null)
            {
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = true, Succeeded = false };
            }
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var pharmacy = await DBEntity.PharmacyInformation
                    .Where(x => x.PharmacyId == pharmacyView.PharmacyId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                if (pharmacy != null)
                {
                    pharmacy.AddressLine1 = pharmacyView.AddressLine1;
                    pharmacy.AddressLine2 = pharmacyView.AddressLine2;
                    pharmacy.City = pharmacyView.City;
                    pharmacy.StateId = pharmacyView.StateId;
                    pharmacy.ZipCode = pharmacyView.ZipCode;
                    pharmacy.PharmacyName = pharmacyView.PharmacyName;
                    pharmacy.PhoneNumber = pharmacyView.PhoneNumber;
                    DBEntity.PharmacyInformation.Update(pharmacy).Property(x => x.PharmacyId).IsModified = false;
                    await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                }
                await transcation.CommitAsync().ConfigureAwait(true);
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception ex)
            {
                await transcation.RollbackAsync().ConfigureAwait(true);
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<List<MedicineView>> GetMedicineDetails(string consultationId)
        {
            try
            {
                var medicines = new List<MedicineView>();
                var medicineList = await DBEntity.UserMedication.Where(x => x.IsRecordDeleted == false && x.ConsultationId == consultationId)
                    .Select(x => new
                    {
                        DrugName = x.DrugName,
                        Dose = x.Dose,
                        UnitId = x.MedicationUnitId,
                        FrequencyId = x.MedicationFrequencyId,
                        FormId = x.MedicationFormId,
                        MedicalCondition = x.MedicalCondition,
                    }).ToListAsync().ConfigureAwait(true);

                foreach (var item in medicineList)
                {
                    medicines.Add(new MedicineView()
                    {
                        Dose = item.Dose,
                        DrugName = item.DrugName,
                        FormId = item.FormId,
                        FrequencyId = item.FrequencyId,
                        MedicalCondition = item.MedicalCondition,
                        UnitId = item.UnitId
                    });
                }
                return medicines;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> UpdateMedicineInfo(List<MedicineView> medicineViews, string patientId,string userId,string consultationId)
        {
            if (medicineViews == null)
            {
                throw new ArgumentException();
            }
            var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                int consultationCategoryId = 0;
                using (var Common = new Common())
                {
                    consultationCategoryId = await Common.GetConsultationCategory(consultationId).ConfigureAwait(true);
                };
                var medicine = await DBEntity.UserMedication
                    .Where(x => x.ConsultationId == consultationId && x.IsRecordDeleted == false)
                    .ToListAsync()
                    .ConfigureAwait(true);
                if (medicine.Count > 0)
                {
                    medicine.ForEach(x => { x.IsRecordDeleted = true; x.ModifiedBy = userId; x.ModifiedDate = DateTime.UtcNow; });
                    foreach (var item in medicine)
                    {
                        DBEntity.UserMedication.Update(item).Property(x => x.Id).IsModified = false;
                    }
                }
                foreach (var item in medicineViews)
                {
                    DBEntity.UserMedication.Add(new UserMedication
                    {
                        AddedBy = userId,
                        AddedDate = DateTime.UtcNow,
                        ConsultationCategoryId = consultationCategoryId,
                        ConsultationId = consultationId,
                        Dose = item.Dose,
                        DrugName = item.DrugName,
                        IsRecordDeleted = false,
                        MedicalCondition = item.MedicalCondition,
                        MedicationFormId = item.FormId,
                        MedicationFrequencyId = item.FrequencyId,
                        MedicationUnitId = item.UnitId,
                        UserId = patientId
                    });
                }
                DBEntity.SaveChanges();
                transcation.Commit();
                return new CrudResult() { Failed=false, Succeeded=true };
            }
            catch (Exception)
            {
                transcation.Rollback();
                throw;
            }
        }
        public async Task<MoveRecordModel> GetPatientByConsultationId(int stateId,string consultationId)
        {
            try
            {
                var stateIdParam = new SqlParameter("@StateID", stateId);
                var consultationIdParam = new SqlParameter("@ConsultationId", consultationId);
                var result = await DBEntity.GET_PATIENT.FromSqlRaw("EXEC GET_PATIENT @StateID,@ConsultationId", stateIdParam, consultationIdParam)
                    .ToListAsync()
                    .ConfigureAwait(true);
                return result[0];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<MoveRecordModel> GetPatientByRowNo(int stateId, int rowNo, int recordType)
        {
            try
            {
                var stateIdParam = new SqlParameter("@StateID", stateId);
                var rowNoParam = new SqlParameter("@RowNo", rowNo);
                var recordTypeParam = new SqlParameter("@RecordType", recordType);
                var result = await DBEntity.GET_PATIENT_ROWNO.FromSqlRaw("EXEC GET_PATIENT_ROWNO @StateID,@RowNo,@RecordType", stateIdParam, rowNoParam, recordTypeParam)
                    .ToListAsync()
                    .ConfigureAwait(true);
                return result[0];
            }
            catch (Exception)
            {
                throw;
            }
        }
        public DataTable ToDataTable<T>(List<T> listItems)
        {
            try
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties
                PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    //Setting column names as Property names
                    dataTable.Columns.Add(property.Name);
                }
                foreach (T item in listItems)
                {
                    var values = new object[properties.Length];
                    for (int i = 0; i < properties.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = properties[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                return dataTable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> MoveToAllPatient(string consultationId) 
        {
            try
            {
                var consultation = await DBEntity.ShippingInformation
                    .Where(x => x.ConsultationId == consultationId && x.IsRecordDeleted == false)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                consultation.Status = 4;
                DBEntity.ShippingInformation.Update(consultation).Property(x => x.Id).IsModified = false;
                int resultCount = await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                if (resultCount > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}