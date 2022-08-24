using System;
using System.Collections.Generic;
using TylerHendricks_Repo.Contracts;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TylerHendricks_Core.Models;
using TylerHendricks_Data.DBEntity;
using TylerHendricks_Core.ViewModel;
using TylerHendricks_Utility;
using static TylerHendricks_Utility.Enums.Areas;
using Microsoft.Data.SqlClient;

namespace TylerHendricks_Repo.Services
{
    public class Patient : Repository, IPatient
    {
        public string getName()
        {
            return null;
        }
        public async Task<int> TotalQuestions(int consultationCategoryId)
        {
            try
            {
                var count = await (from question in DBEntity.ConsultationQuestions
                                   where question.IsRecordDeleted == false && question.ConsultationCategoryId == consultationCategoryId && question.IsPopup == false
                                   select new { Id = question.Id }).ToListAsync().ConfigureAwait(true);
                return count.Count;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> AttemptQuestions(string userId, int consultationCategoryId, string consultationId)
        {
            try
            {
                int returnValue = 1;
                var count = await (from a in DBEntity.AspUserAnswerMapping
                                   where a.UserId == userId && a.ConsultationCategoryId == consultationCategoryId && a.ConsultationId == consultationId
                                   && a.IsRecordDeleted == false
                                   select new
                                   {
                                       Questions = a.ConsultationQuestionsMappingId
                                   }).ToListAsync().ConfigureAwait(true);

                if (count != null)
                {
                    returnValue = count.Count;
                }
                else
                {
                    returnValue = 1;
                }
                return returnValue;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Questions> QuestionsDetail(string userId = "", int QuestionId = 0, bool QuestionResponse = true)
        {
            try
            {
                var question = await (from questMap in DBEntity.ConsultationQuestionsMapping
                                      join ques in DBEntity.ConsultationQuestions
                                      on questMap.Questionid equals ques.Id
                                      where questMap.Questionid == QuestionId
                                      && questMap.IsRecordDeleted == false && questMap.IsActive == true
                                      orderby questMap.Id
                                      select new
                                      {
                                          Id = questMap.Id,
                                          Description = ques.QuestionDescription,
                                          Response = questMap.Response,
                                          NextQuestionId = questMap.NextQuestion,
                                          PreviousQuestionId = questMap.Questionid,
                                          ModalPopupId = questMap.ModalPopupId,
                                          IsNote = questMap.IsNote,
                                          ConsultationCategoryId = ques.ConsultationCategoryId
                                      }).ToListAsync().ConfigureAwait(true);

                if (question != null)
                {
                    string photo = string.Empty;
                    if (question[0].NextQuestionId == 35 && question[0].ConsultationCategoryId == 1)
                    {
                        photo = (from u in DBEntity.AspNetUsers where u.IsRecordDelete == false && u.Id == userId select u.Photo).FirstOrDefault();
                    }
                    return new Questions()
                    {
                        Id = question[0].Id,
                        Description = question[0].Description,
                        NextQuestionId = question[0].NextQuestionId,
                        PreviousQuestionId = question[0].PreviousQuestionId,
                        Response = question[0].Response,
                        NextQuestionId1 = question.Count > 1 ? question[1].NextQuestionId : null,
                        ModalPopupId = question[0].ModalPopupId,
                        ModalPopupId1 = question.Count > 1 ? question[1].ModalPopupId : null,
                        PopText = await GetPopupText(question[0].ModalPopupId).ConfigureAwait(true),
                        PopText1 = await GetPopupText(question.Count > 1 ? question[1].ModalPopupId : 0).ConfigureAwait(true),
                        IsNote = question[0].IsNote,
                        ConsultationCategoryId = (int)question[0].ConsultationCategoryId,
                        Answer = photo
                    };
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
        public async Task<Questions> CurrentQuestion(string userId, int consultationCategoryId, string consultationId)
        {
            try
            {
                var lastQuestion = await (from a in DBEntity.AspUserAnswerMapping
                                          where a.ConsultationCategoryId == consultationCategoryId && a.ConsultationId == consultationId
                                          && a.UserId == userId && a.IsRecordDeleted == false && a.IsActive == true
                                          && (a.IsAction == true || !string.IsNullOrEmpty(a.Answer))
                                          orderby a.Id descending
                                          select new
                                          {
                                              Questions = a.ConsultationQuestionsMappingId,
                                              Answer = a.Answer == null ? "" : a.Answer
                                          }).FirstOrDefaultAsync().ConfigureAwait(true);

                if (lastQuestion != null)
                {
                    int lastId = Convert.ToInt32(lastQuestion.Questions);
                    var questionId = await (from cm in DBEntity.ConsultationQuestionsMapping
                                            where cm.IsRecordDeleted == false && cm.IsActive == true && cm.Id == lastId
                                            select cm.Questionid)
                        .FirstOrDefaultAsync().ConfigureAwait(true);
                    var NextQuestion = await (from cm in DBEntity.ConsultationQuestionsMapping
                                              where cm.IsRecordDeleted == false && cm.IsActive == true && cm.Questionid == questionId
                                              && cm.Response == Convert.ToBoolean(lastQuestion.Answer.ToLower().Trim().Contains("no") ? "false" : "true")
                                              select cm.NextQuestion).FirstOrDefaultAsync().ConfigureAwait(true);

                    var question = await (from questMap in DBEntity.ConsultationQuestionsMapping
                                          join ques in DBEntity.ConsultationQuestions
                                          on questMap.Questionid equals ques.Id
                                          where questMap.Questionid == NextQuestion &&
                                          questMap.IsRecordDeleted == false && questMap.IsActive == true
                                          orderby questMap.Id
                                          select new
                                          {
                                              Id = questMap.Id,
                                              Description = ques.QuestionDescription,
                                              Response = questMap.Response,
                                              NextQuestionId = questMap.NextQuestion,
                                              PreviousQuestionId = questMap.Questionid,
                                              ModalPopupId = questMap.ModalPopupId,
                                              IsNote = questMap.IsNote,
                                              ConsultationCategoryId = ques.ConsultationCategoryId
                                          }).ToListAsync().ConfigureAwait(true);

                    if (question != null)
                    {
                        string photo = string.Empty;
                        if (question[0].NextQuestionId == (int)ConsultationLastQuestion.ErectileDysfunction && question[0].ConsultationCategoryId == (int)ConsultationType.ErectileDysfunction)
                        {
                            photo = (from u in DBEntity.AspNetUsers where u.IsRecordDelete == false && u.Id == userId select u.Photo).FirstOrDefault();
                        }
                        else if (question[0].NextQuestionId == (int)ConsultationLastQuestion.MedicationRefill && question[0].ConsultationCategoryId == (int)ConsultationType.MedicationRefill)
                        {
                            photo = (from u in DBEntity.AspNetUsers where u.IsRecordDelete == false && u.Id == userId select u.Photo).FirstOrDefault();
                        }
                        else if (question[0].NextQuestionId == (int)ConsultationLastQuestion.HairLoss && question[0].ConsultationCategoryId == (int)ConsultationType.HairLoss)
                        {
                            photo = (from u in DBEntity.AspNetUsers where u.IsRecordDelete == false && u.Id == userId select u.Photo).FirstOrDefault();
                        }
                        return new Questions()
                        {
                            Id = question[0].Id,
                            Description = question[0].Description,
                            NextQuestionId = question[0].NextQuestionId,
                            PreviousQuestionId = question[0].PreviousQuestionId,
                            Response = question[0].Response,
                            NextQuestionId1 = question.Count > 1 ? question[1].NextQuestionId : null,
                            ModalPopupId = question[0].ModalPopupId,
                            ModalPopupId1 = question.Count > 1 ? question[1].ModalPopupId : null,
                            PopText = await GetPopupText(question[0].ModalPopupId).ConfigureAwait(true),
                            PopText1 = await GetPopupText(question.Count > 1 ? question[1].ModalPopupId : 0).ConfigureAwait(true),
                            IsNote = question[0].IsNote,
                            ConsultationCategoryId = (int)question[0].ConsultationCategoryId,
                            Answer = photo
                        };
                    }
                }
                else
                {
                    var question = await (from questMap in DBEntity.ConsultationQuestionsMapping
                                          join ques in DBEntity.ConsultationQuestions
                                          on questMap.Questionid equals ques.Id
                                          where questMap.IsRecordDeleted == false
                                          && questMap.IsActive == true
                                          && ques.IsRecordDeleted == false
                                          && ques.IsStart == true
                                          && ques.ConsultationCategoryId == consultationCategoryId
                                          orderby questMap.Id
                                          select new
                                          {
                                              Id = questMap.Id,
                                              Description = ques.QuestionDescription,
                                              Response = questMap.Response,
                                              NextQuestionId = questMap.NextQuestion,
                                              PreviousQuestionId = questMap.Questionid,
                                              ModalPopupId = questMap.ModalPopupId,
                                              IsNote = questMap.IsNote,
                                              ConsultationCategoryId = ques.ConsultationCategoryId
                                          }).ToListAsync().ConfigureAwait(true);

                    if (question != null)
                    {
                        return new Questions()
                        {
                            Id = question[0].Id,
                            Description = question[0].Description,
                            NextQuestionId = question[0].NextQuestionId,
                            PreviousQuestionId = question[0].PreviousQuestionId,
                            Response = question[0].Response,
                            NextQuestionId1 = question.Count > 1 ? question[1].NextQuestionId : null,
                            ModalPopupId = question[0].ModalPopupId,
                            ModalPopupId1 = question.Count > 1 ? question[1].ModalPopupId : null,
                            PopText = await GetPopupText(question[0].ModalPopupId).ConfigureAwait(true),
                            PopText1 = await GetPopupText(question.Count > 1 ? question[1].ModalPopupId : 0).ConfigureAwait(true),
                            IsNote = question[0].IsNote,
                            ConsultationCategoryId = (int)question[0].ConsultationCategoryId,
                            Answer = string.Empty
                        };
                    }
                }
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> UpdateQuestionState(string userId, string previousId, string answer, string consultationId, int consultationCategoryId
            , string consultationMappingdId)
        {
            using var transaction = DBEntity.Database.BeginTransaction();
            try
            {
                var existQuery = await DBEntity.AspUserAnswerMapping.Where(x => x.IsRecordDeleted == false && x.UserId == userId && x.ConsultationId == consultationId &&
                 x.ConsultationCategoryId == consultationCategoryId && x.ConsultationQuestionsMappingId == consultationMappingdId).FirstOrDefaultAsync().ConfigureAwait(true);

                if (existQuery == null)
                {
                    DBEntity.AspUserAnswerMapping.Add(new AspUserAnswerMapping()
                    {
                        Answer = answer,
                        UserId = userId,
                        ConsultationId = consultationId,
                        ConsultationQuestionsMappingId = previousId,
                        IsRecordDeleted = false,
                        IsCheckOut = false,
                        IsActive = true,
                        ConsultationCategoryId = consultationCategoryId,
                        AddedBy = userId,
                        AddedDate = DateTime.UtcNow,
                        ModifiedBy = userId,
                        ModifiedDate = DateTime.UtcNow,
                        IsAction = true
                    });
                    await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                }
                transaction.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<CrudResult> AddConsulationCategory(string userId, int consultationCategoryId)
        {

            var question = await (from an in DBEntity.AspUserAnswerMapping
                                  where an.UserId == userId && an.ConsultationCategoryId == consultationCategoryId && an.IsRecordDeleted == false
                                  select new
                                  {
                                      ConsultationQuestionsMappingId = an.ConsultationQuestionsMappingId
                                  }).ToListAsync().ConfigureAwait(true);
            if (question != null)
            {
                using var transcation = DBEntity.Database.BeginTransaction();
                try
                {
                    DBEntity.AspUserAnswerMapping.Add(new AspUserAnswerMapping()
                    {
                        UserId = userId,
                        ConsultationId = DateTime.UtcNow.Ticks.ToString() + Guid.NewGuid().ToString("N").Substring(1, 5),
                        ConsultationCategoryId = consultationCategoryId,
                        IsCheckOut = false,
                        IsRecordDeleted = false,
                        ConsultationQuestionsMappingId = consultationCategoryId == 1 ? "3" : "22",
                        AddedDate = DateTime.UtcNow,
                        AddedBy = userId,
                        ModifiedBy = userId,
                        ModifiedDate = DateTime.UtcNow
                    });
                    DBEntity.SaveChanges();
                    transcation.Commit();
                    return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
                }
                catch (Exception ex)
                {
                    transcation.Rollback();
                    return new CrudResult() { ErroCode = ex.Message, ErrorMassage = ex.Message, Failed = true, Succeeded = false };
                }
            }
            return null;
        }
        public async Task<string> GetPopupText(int? QuestionId)
        {
            try
            {
                var qa = await (from q in DBEntity.ConsultationQuestions
                                where q.IsRecordDeleted == false && q.Id == QuestionId
                                select q.QuestionDescription).FirstOrDefaultAsync().ConfigureAwait(true);
                return qa == null ? "" : qa;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetAnswer(string userId, int consultationCategoryId, string questionMappingId)
        {
            try
            {
                var question = await (from ques in DBEntity.AspUserAnswerMapping
                                      where ques.IsRecordDeleted == false
                                     && ques.ConsultationCategoryId == consultationCategoryId && ques.ConsultationQuestionsMappingId == questionMappingId
                                     && ques.UserId == userId
                                      orderby ques.ModifiedDate descending
                                      select ques.Answer).FirstOrDefaultAsync().ConfigureAwait(true);
                return question == null ? "" : question;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> SaveShippingDetail(QuestionNaireViewModel question)
        {
            using var transaction = DBEntity.Database.BeginTransaction();
            try
            {
                var consultation = await DBEntity.Consultation
                    .Where(x => x.ConsultationId == question.ConsultationId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                consultation.IsHomeDelivery = question.IsMedicationDelivery == null ? false : question.IsMedicationDelivery;
                consultation.IsRecordDeleted = false;
                DBEntity.Consultation.Update(consultation).Property(x => x.Id).IsModified = false;
                var mediStatus = await CheckMedi(question.UserId, question.ConsultationId, question.ConsultationCategoryId).ConfigureAwait(true);
                int Status = mediStatus == 1 ? 2 : 1;
                var shipping = await (from s in DBEntity.ShippingInformation
                                      where s.ConsultationId == question.ConsultationId
                                      select s).FirstOrDefaultAsync().ConfigureAwait(true);
                if (shipping != null)
                {
                    shipping.OrderStatusTypeId = 1;
                    shipping.PaymentStatus = "Pending";
                    shipping.ProductName = question.chooseYourMedicationModel.MedicationName;
                    shipping.ProductUnit = question.chooseYourMedicationModel.MedicationUnit;
                    shipping.ProductQuantity = question.chooseYourMedicationModel.MedicationQuantity;
                    shipping.ProductPrice = question.chooseYourMedicationModel.MedicationPrice;
                    shipping.ProductDescription = question.chooseYourMedicationModel.Description;
                    shipping.Refills = question.chooseYourMedicationModel.Refills;
                    shipping.IsRecordDeleted = false;
                    shipping.ConsultationId = question.ConsultationId;
                    shipping.AddedDate = DateTime.UtcNow;
                    shipping.AddedBy = question.UserId;
                    shipping.ModifiedBy = question.UserId;
                    shipping.ModifiedDate = DateTime.UtcNow;
                    shipping.Status = Status;
                    DBEntity.ShippingInformation.Update(shipping).Property(x => x.Id).IsModified = false;
                }
                else
                {
                    await DBEntity.ShippingInformation.AddAsync(new ShippingInformation()
                    {
                        OrderStatusTypeId = 1,
                        PaymentStatus = "Pending",
                        ProductName = question.chooseYourMedicationModel.MedicationName,
                        ProductUnit = question.chooseYourMedicationModel.MedicationUnit,
                        ProductQuantity = question.chooseYourMedicationModel.MedicationQuantity,
                        ProductPrice = question.chooseYourMedicationModel.MedicationPrice,
                        ProductDescription = question.chooseYourMedicationModel.Description,
                        Refills = question.chooseYourMedicationModel.Refills,
                        IsRecordDeleted = false,
                        ConsultationId = question.ConsultationId,
                        AddedDate = DateTime.UtcNow,
                        AddedBy = question.UserId,
                        ModifiedBy = question.UserId,
                        ModifiedDate = DateTime.UtcNow,
                        UserId = question.UserId,
                        ConsultationCategoryId = question.ConsultationCategoryId,
                        Status = Status
                    }).ConfigureAwait(true);
                }
                if (question.IsMedicationDelivery == true)
                {
                    var pharmacy = await (from p in DBEntity.PharmacyInformation where p.IsRecordDeleted == false && p.ConsultationId == question.ConsultationId select p).FirstOrDefaultAsync().ConfigureAwait(true);
                    if (pharmacy != null)
                    {
                        pharmacy.IsRecordDeleted = true;
                        DBEntity.PharmacyInformation.Update(pharmacy).Property(x => x.PharmacyId).IsModified = false;
                    }
                }
                DBEntity.SaveChanges();
                transaction.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<List<MedicineForm>> GetMedicineForms()
        {
            try
            {
                return await (from q in DBEntity.MedicineForm where q.IsRecordDeleted == false select q).ToListAsync().ConfigureAwait(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<MedicineUnit>> GetMedicineUnit()
        {
            try
            {
                return await (from q in DBEntity.MedicineUnit where q.IsRecordDeleted == false select q).ToListAsync().ConfigureAwait(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<MedicineFrequency>> GetMedicineFrequency()
        {
            try
            {
                return await (from q in DBEntity.MedicineFrequency where q.IsRecordDeleted == false select q).ToListAsync().ConfigureAwait(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> AddMedicineDose(IEnumerable<MedicineDoseModal> modals, string userId, int consultationCategoryId, string consultationId, bool IsMedicine)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                if (modals != null)
                {
                    var userMedications = await DBEntity.UserMedication.Where(x => x.IsRecordDeleted == false && x.ConsultationId == consultationId && x.IsMedicine == IsMedicine)
                        .ToListAsync().ConfigureAwait(true);
                    if (userMedications.Count > 0)
                    {
                        userMedications.ForEach(x => { x.IsRecordDeleted = true; });
                        foreach (var item in userMedications)
                        {
                            DBEntity.UserMedication.Update(item).Property(x => x.Id).IsModified = false;
                        }
                    }
                    foreach (var m in modals)
                    {
                        DBEntity.UserMedication.Add(new UserMedication()
                        {
                            DrugName = m.DrugName,
                            Dose = m.Dose,
                            IsRecordDeleted = false,
                            AddedBy = userId,
                            AddedDate = DateTime.UtcNow,
                            MedicalCondition = m.MedicalCondition,
                            ConsultationId = consultationId,
                            ConsultationCategoryId = consultationCategoryId,
                            MedicationFormId = m.MedicationFormId,
                            MedicationFrequencyId = m.MedicationFrequencyId,
                            MedicationUnitId = m.MedicationUnitId,
                            ModifiedBy = userId,
                            ModifiedDate = DateTime.UtcNow,
                            UserId = userId,
                            IsMedicine = IsMedicine
                        });
                    }
                    await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                }
                transcation.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception ex)
            {
                transcation.Rollback();
                return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
            }
        }
        public async Task<CrudResult> SetPreviousQuestion(string userId, int consultationCategoryId, string consultationId, bool isBack)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                if (isBack)
                {
                    var LastQuestion = await (from q in DBEntity.AspUserAnswerMapping
                                              where q.IsRecordDeleted == false && q.UserId == userId && q.ConsultationId == consultationId
                                              && q.ConsultationCategoryId == consultationCategoryId
                                              orderby q.Id descending
                                              select q).FirstOrDefaultAsync().ConfigureAwait(true);

                    LastQuestion.IsRecordDeleted = true;
                    LastQuestion.ModifiedBy = userId;
                    LastQuestion.ModifiedDate = DateTime.UtcNow;
                    DBEntity.AspUserAnswerMapping.Update(LastQuestion).Property(x => x.Id).IsModified = false;
                }
                else
                {
                    var updateRecord = await (from q in DBEntity.AspUserAnswerMapping
                                              where q.IsRecordDeleted == false && q.UserId == userId && q.ConsultationId == consultationId
                                              && q.ConsultationCategoryId == consultationCategoryId
                                              orderby q.Id descending
                                              select q).FirstOrDefaultAsync().ConfigureAwait(true);
                    updateRecord.ModifiedBy = userId;
                    updateRecord.ModifiedDate = DateTime.UtcNow;
                    updateRecord.IsRecordDeleted = true;
                    DBEntity.AspUserAnswerMapping.Update(updateRecord).Property(x => x.Id).IsModified = false;
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
        public async Task<List<Medication>> GetMedication(int medicationCategoryId)
        {
            try
            {
                var lstmedication = new List<Medication>();
                var medication = await (from md in DBEntity.Medication
                                        where md.IsRecordDeleted == false && md.MedicationCatgoryId == medicationCategoryId
                                        select new
                                        {
                                            Id = md.Id,
                                            Name = md.Name,
                                        }).ToListAsync().ConfigureAwait(true);
                foreach (var item in medication)
                {
                    lstmedication.Add(new Medication()
                    {
                        Id = item.Id,
                        Name = item.Name
                    });

                }
                return lstmedication;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<MedicationCategory>> GetMedicationCategories()
        {
            try
            {
                var medicationCategories = new List<MedicationCategory>();
                var medcategory = await (from mcat in DBEntity.MedicationCategory
                                         where mcat.IsRecordDeleted == false
                                         select new
                                         {
                                             Id = mcat.Id,
                                             Name = mcat.Name,
                                         }).ToListAsync().ConfigureAwait(true);
                foreach (var item in medcategory)
                {
                    medicationCategories.Add(new MedicationCategory()
                    {
                        Id = item.Id,
                        Name = item.Name
                    });
                }
                return medicationCategories;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> SaveMedicineImage(List<string> fileName, string userId, string consultationId)
        {
            using (var transcation = DBEntity.Database.BeginTransaction())
            {
                try
                {
                    var uploadQuery = await (from img in DBEntity.UserMedicineImage
                                             where img.IsRecordDeleted == false && img.UserId == userId && img.ConsultationId == consultationId
                                             select img
                                            ).FirstOrDefaultAsync().ConfigureAwait(true);

                    if (uploadQuery == null)
                    {
                        if (fileName != null)
                        {
                            if (fileName.Count == 3)
                            {
                                DBEntity.UserMedicineImage.Add(new UserMedicineImage()
                                {
                                    ConsultationId = consultationId,
                                    AddedBy = userId,
                                    AddedDate = DateTime.UtcNow,
                                    IsRecordDeleted = false,
                                    UserId = userId,
                                    MedicineFile1 = fileName[0],
                                    MedicineFile2 = fileName[1],
                                    MedicineFile3 = fileName[2]
                                });
                            }
                            else if (fileName.Count == 2)
                            {
                                DBEntity.UserMedicineImage.Add(new UserMedicineImage()
                                {
                                    ConsultationId = consultationId,
                                    AddedBy = userId,
                                    AddedDate = DateTime.UtcNow,
                                    IsRecordDeleted = false,
                                    UserId = userId,
                                    MedicineFile1 = fileName[0],
                                    MedicineFile2 = fileName[1],
                                });
                            }
                            else if (fileName.Count == 1)
                            {
                                DBEntity.UserMedicineImage.Add(new UserMedicineImage()
                                {
                                    ConsultationId = consultationId,
                                    AddedBy = userId,
                                    AddedDate = DateTime.UtcNow,
                                    IsRecordDeleted = false,
                                    UserId = userId,
                                    MedicineFile1 = fileName[0],
                                });
                            }
                        }
                    }
                    else
                    {
                        if (fileName.Count == 3)
                        {
                            uploadQuery.MedicineFile1 = fileName[0];
                            uploadQuery.MedicineFile2 = fileName[1];
                            uploadQuery.MedicineFile3 = fileName[2];
                            uploadQuery.ModifiedBy = userId;
                            uploadQuery.ModifiedDate = DateTime.UtcNow;
                            DBEntity.UserMedicineImage.Update(uploadQuery).Property(x => x.Id).IsModified = false;
                        }
                        else if (fileName.Count == 2)
                        {
                            uploadQuery.MedicineFile1 = fileName[0];
                            uploadQuery.MedicineFile2 = fileName[1];
                            uploadQuery.ModifiedBy = userId;
                            uploadQuery.ModifiedDate = DateTime.UtcNow;
                            DBEntity.UserMedicineImage.Update(uploadQuery).Property(x => x.Id).IsModified = false;
                        }
                        else if (fileName.Count == 1)
                        {
                            uploadQuery.MedicineFile1 = fileName[0];
                            uploadQuery.ModifiedBy = userId;
                            uploadQuery.ModifiedDate = DateTime.UtcNow;
                            DBEntity.UserMedicineImage.Update(uploadQuery).Property(x => x.Id).IsModified = false;
                        }
                    }
                    var consultation = await DBEntity.Consultation.Where(x => x.ConsultationId == consultationId && x.IsRecordDeleted == false)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(true);
                    consultation.RxRequestUpdateDate = DateTime.UtcNow;
                    consultation.RxRequest = false;
                    DBEntity.Consultation.Update(consultation).Property(x => x.Id).IsModified = false;
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
        }
        public async Task<CrudResult> SaveRetakeMedicine(List<string> fileName, string userId, int consultationCategoryId)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var consultationIds = new List<string>();
                using (var common = new Common())
                {
                    consultationIds = await common.GetRetakeRxRequest(userId, consultationCategoryId).ConfigureAwait(true);
                }
                var userMedicines = await (from img in DBEntity.UserMedicineImage
                                         where img.IsRecordDeleted == false && img.UserId == userId && consultationIds.Contains(img.ConsultationId)
                                         select img).ToListAsync().ConfigureAwait(true);
                if (fileName != null)
                {
                    if (userMedicines.Count == 0)
                    {
                        foreach (var item in consultationIds)
                        {
                            if (fileName.Count == 3)
                            {
                                DBEntity.UserMedicineImage.Add(new UserMedicineImage()
                                {
                                    ConsultationId = item,
                                    AddedBy = userId,
                                    AddedDate = DateTime.UtcNow,
                                    IsRecordDeleted = false,
                                    UserId = userId,
                                    MedicineFile1 = fileName[0],
                                    MedicineFile2 = fileName[1],
                                    MedicineFile3 = fileName[2]
                                });
                            }
                            else if (fileName.Count == 2)
                            {
                                DBEntity.UserMedicineImage.Add(new UserMedicineImage()
                                {
                                    ConsultationId = item,
                                    AddedBy = userId,
                                    AddedDate = DateTime.UtcNow,
                                    IsRecordDeleted = false,
                                    UserId = userId,
                                    MedicineFile1 = fileName[0],
                                    MedicineFile2 = fileName[1],
                                });
                            }
                            else if (fileName.Count == 1)
                            {
                                DBEntity.UserMedicineImage.Add(new UserMedicineImage()
                                {
                                    ConsultationId = item,
                                    AddedBy = userId,
                                    AddedDate = DateTime.UtcNow,
                                    IsRecordDeleted = false,
                                    UserId = userId,
                                    MedicineFile1 = fileName[0],
                                });
                            }
                        }
                    }
                    else
                    {
                        foreach (var userMedicine in userMedicines)
                        {
                            if (fileName.Count == 3)
                            {
                                userMedicine.MedicineFile1 = fileName[0];
                                userMedicine.MedicineFile2 = fileName[1];
                                userMedicine.MedicineFile3 = fileName[2];
                                userMedicine.ModifiedBy = userId;
                                userMedicine.ModifiedDate = DateTime.UtcNow;
                                DBEntity.UserMedicineImage.Update(userMedicine).Property(x => x.Id).IsModified = false;
                            }
                            else if (fileName.Count == 2)
                            {
                                userMedicine.MedicineFile1 = fileName[0];
                                userMedicine.MedicineFile2 = fileName[1];
                                userMedicine.ModifiedBy = userId;
                                userMedicine.ModifiedDate = DateTime.UtcNow;
                                DBEntity.UserMedicineImage.Update(userMedicine).Property(x => x.Id).IsModified = false;
                            }
                            else if (fileName.Count == 1)
                            {
                                userMedicine.MedicineFile1 = fileName[0];
                                userMedicine.ModifiedBy = userId;
                                userMedicine.ModifiedDate = DateTime.UtcNow;
                                DBEntity.UserMedicineImage.Update(userMedicine).Property(x => x.Id).IsModified = false;
                            }
                        }
                    }
                }

                var consultations = await DBEntity.Consultation
                    .Where(x => x.IsRecordDeleted == false && consultationIds.Contains(x.ConsultationId) && x.ConsultationCategoryId == consultationCategoryId)
                    .ToListAsync()
                    .ConfigureAwait(true);

                foreach (var item in consultations)
                {
                    item.RxRequest = false;
                    item.RxRequestUpdateDate = DateTime.UtcNow;
                    DBEntity.Consultation.Update(item).Property(x => x.Id).IsModified = false;
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
        public async Task<CrudResult> SavePaymentDetails(string userId, int consultationCategoryId, string txnId, string consultationId)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var shippingInformation = await DBEntity.ShippingInformation
                    .Where(x => x.IsRecordDeleted == false && x.ConsultationId == consultationId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);

                shippingInformation.TxnId = txnId;
                shippingInformation.PaymentStatus = "capture";
                shippingInformation.RequestedDate = DateTime.UtcNow;
                shippingInformation.ModifiedDate = DateTime.UtcNow;
                shippingInformation.ModifiedBy = userId;

                DBEntity.ShippingInformation.Update(shippingInformation).Property(x => x.Id).IsModified = false;

                var consultation = await DBEntity.Consultation
                    .Where(x => x.ConsultationId == consultationId && x.IsRecordDeleted == false)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);

                consultation.IsCompleted = true;
                consultation.IsStarted = false;

                DBEntity.Consultation.Update(consultation).Property(x => x.Id).IsModified = false;

                var aspUsers = await DBEntity.AspUserAnswerMapping
                    .Where(x => x.IsRecordDeleted == false && x.ConsultationId == consultationId)
                    .ToListAsync()
                    .ConfigureAwait(true);
                aspUsers.ForEach(x => { x.IsCheckOut = true; });

                foreach (var item in aspUsers)
                {
                    DBEntity.AspUserAnswerMapping.Update(item).Property(x => x.Id).IsModified = false;
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
        public async Task<CrudResult> SavePaymentDetails(int userRowId,string invoiceNumber)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var shippingInformation = await DBEntity.ShippingInformation
                    .Where(x => x.IsRecordDeleted == false && x.OrderId == invoiceNumber)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                if (shippingInformation.PaymentStatus.ToLower() != "capture")
                {
                    var userId = await DBEntity.AspNetUsers
                    .Where(x => x.RowId == userRowId)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);

                    shippingInformation.TxnId = invoiceNumber;
                    shippingInformation.PaymentStatus = "capture";
                    shippingInformation.RequestedDate = DateTime.UtcNow;
                    shippingInformation.ModifiedDate = DateTime.UtcNow;
                    shippingInformation.ModifiedBy = userId;

                    var consultation = await DBEntity.Consultation
                        .Where(x => x.ConsultationId == shippingInformation.ConsultationId && x.IsRecordDeleted == false)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(true);

                    consultation.IsCompleted = true;
                    consultation.IsStarted = false;

                    var aspUsers = await DBEntity.AspUserAnswerMapping
                        .Where(x => x.IsRecordDeleted == false && x.ConsultationId == shippingInformation.ConsultationId)
                        .ToListAsync()
                        .ConfigureAwait(true);
                    aspUsers.ForEach(x => { x.IsCheckOut = true; });
                    foreach (var item in aspUsers)
                    {
                       
                        DBEntity.AspUserAnswerMapping.Update(item).Property(x => x.Id).IsModified = false;
                    }
                    DBEntity.Consultation.Update(consultation).Property(x => x.Id).IsModified = false;
                    DBEntity.ShippingInformation.Update(shippingInformation).Property(x => x.Id).IsModified = false;
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
        public async Task<ChooseYourMedicationModel> GetSelectedMedication(string consultationId)
        {
            try
            {
                var query = await (from a in DBEntity.UserMedication
                                   join u in DBEntity.MedicineUnit on a.MedicationUnitId equals u.Id
                                   where a.IsRecordDeleted == false && a.ConsultationId == consultationId && a.IsMedicine == false
                                   orderby a.Id descending
                                   select new { DrugName = a.DrugName, Dose = a.Dose, Unit = u.Name }).FirstOrDefaultAsync().ConfigureAwait(true);
                if (query != null)
                {
                    return new ChooseYourMedicationModel()
                    {
                        MedicationName = (query.DrugName+" "+ query.Dose +" "+ query.Unit).Trim(),
                    };
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
        public async Task<bool> ConsultationComplete(string consultationId)
        {
            try
            {
                var aspUserAnswer = await DBEntity.Consultation
                    .Where(x => x.ConsultationId == consultationId && x.IsRecordDeleted == false)
                    .Select(x=>x.IsCompleted)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                if (aspUserAnswer == null)
                {
                    return false;
                }
                return aspUserAnswer;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<OrderHistoryModel>> GetOrderHistory(string userId,TimeZoneConfig zoneConfig)
        {
            try
            {
                var UserIdParam = new SqlParameter("@USERID", userId);
                var TimeZoneParam = new SqlParameter("@TIMEZONE", zoneConfig.OffSet * -1);
                var returnModel = await DBEntity.PATIENT_REQUEST_HISTORY
                    .FromSqlRaw("EXEC PATIENT_REQUEST_HISTORY @USERID,@TIMEZONE", UserIdParam, TimeZoneParam)
                    .ToListAsync()
                    .ConfigureAwait(true);
                return returnModel;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> GetFirstQuestion(int ConsultationCategoryId)
        {
            try
            {
                return await (from c in DBEntity.ConsultationQuestions
                              where c.IsRecordDeleted == false && c.IsStart == true && c.ConsultationCategoryId == ConsultationCategoryId
                              select c.Id).FirstOrDefaultAsync().ConfigureAwait(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<MessagesViewModel> GetMessageDetail(string UserId, int ConsultationCategoryId, TimeZoneConfig TimeZone)
        {
            try
            {
                var messaging = await (from c in DBEntity.PatientConsultation
                                       join a in DBEntity.AspNetUsers on c.SenderId equals a.Id
                                       join ar in DBEntity.AspNetUsers on c.ReceiverId equals ar.Id
                                       where c.IsRecordDeleted == false && (c.SenderId == UserId || c.ReceiverId == UserId)
                                       && c.ConsultationCategoryId == ConsultationCategoryId
                                       orderby c.Id descending
                                       select new
                                       {
                                           Id = c.Id,
                                           SenderId = c.SenderId,
                                           ReceiverId = c.ReceiverId,
                                           SenderName = a.FirstName + " " + a.LastName,
                                           ReceiverName = ar.FirstName + " " + ar.LastName,
                                           SendingDate = Comman.GetUserDateByTimeZone(c.AddedDate, TimeZone.OffSet, TimeZone.IsDayLightSaving),
                                           ReplyDate = Comman.GetUserDateByTimeZone(c.ModifiedDate == null ? DateTime.UtcNow : c.ModifiedDate.Value, TimeZone.OffSet, TimeZone.IsDayLightSaving),
                                           ReplyMessage = c.Message,
                                           Attachment = c.Attachment,
                                           SendingMessage = c.Reply,
                                           ConsultationCategoryId = c.ConsultationCategoryId,
                                       }).ToListAsync().ConfigureAwait(true);

                var returnmodel = new List<UserChat>();
                var providerId = messaging.Where(x => x.SenderId != UserId).Select(x => x.SenderId).FirstOrDefault();
                if (providerId == null)
                {
                    providerId = messaging.Where(x => x.ReceiverId != UserId).Select(x => x.ReceiverId).FirstOrDefault();
                }
                foreach (var item in messaging)
                {
                    returnmodel.Add(new UserChat()
                    {
                        Id = item.Id,
                        SenderId = item.SenderId,
                        ReceiverId = item.ReceiverId,
                        SenderName = item.SenderName,
                        ReceiverName = item.ReceiverName,
                        SendingDate = item.SendingDate,
                        ReplyDate = item.ReplyDate,
                        Message = item.ReplyMessage,
                        Reply = item.SendingMessage,
                        Attachment = item.Attachment,
                        ConsultationCategoryId = item.ConsultationCategoryId,
                    });
                }

                var messageview = new MessagesViewModel();
                var userData = await (from c in DBEntity.AspNetUsers
                                      where c.IsRecordDelete == false && c.Id == UserId
                                      select new
                                      {
                                          IsRequestedPhotoId = c.RetakeRequestPhotoId,
                                          IsRequestedSelfie = c.RetakeRequestSelfie,
                                          WeekChat = c.WeekChat,
                                          WeekDate = c.WeekChatEndDate
                                      }).FirstOrDefaultAsync().ConfigureAwait(true);
                using (var common = new Common())
                {
                    messageview.IsRequestedMedication = await common.GetRxRequestStatus(UserId, ConsultationCategoryId).ConfigureAwait(true);
                };
                messageview.userChats = returnmodel;
                messageview.IsRequestedPhotoId = userData.IsRequestedPhotoId;
                messageview.IsRequestedSelfie = userData.IsRequestedSelfie;
                messageview.StartChat = false;
                messageview.ChatPayment = (userData.WeekDate > DateTime.UtcNow && userData.WeekChat == true) ? true : false;
                var providerLastMessageDateTime = await DBEntity.PatientConsultation
                    .Where(x => x.IsRecordDeleted == false && x.SenderId == providerId && x.ReceiverId == UserId && x.ConsultationCategoryId == ConsultationCategoryId)
                    .OrderBy(x => x.Id)
                    .Select(x => x.AddedDate)
                    .LastOrDefaultAsync()
                    .ConfigureAwait(true);
                double lastMessageByProviderHours = 100;
                if (providerLastMessageDateTime != null)
                {
                    lastMessageByProviderHours = DateTime.UtcNow.Subtract(providerLastMessageDateTime).TotalHours;
                }
                if (lastMessageByProviderHours < 72)
                {
                    messageview.StartChat = true;
                }
                else if (userData.WeekChat == true && userData.WeekDate > DateTime.UtcNow)
                {
                    messageview.StartChat = true;
                }
                return messageview;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<int> GetStateId(string UserId)
        {
            try
            {
                return await (from a in DBEntity.AspNetUsers where a.IsRecordDelete == false && a.Id == UserId select a.StateId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> GetProviderDetail(int StateId)
        {
            try
            {
                return await (from pro in DBEntity.AspNetUsers
                              join r in DBEntity.AspNetUserRoles on pro.Id equals r.UserId
                              join ar in DBEntity.AspNetRoles on r.RoleId equals ar.Id
                              where ar.NormalizedName == "PHYSICIAN"
                              select pro.Id).FirstOrDefaultAsync().ConfigureAwait(true);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> SaveMessages(UserChat userChat)
        {
            try
            {
                if (userChat != null)
                {
                    var consultation = await (from c in DBEntity.PatientConsultation
                                      where c.IsRecordDeleted == false
                                      && (c.SenderId == userChat.SenderId || c.ReceiverId == userChat.SenderId)
                                      orderby c.Id descending
                                      select c).FirstOrDefaultAsync().ConfigureAwait(true);
                    if (consultation != null)
                    {
                        if (string.IsNullOrEmpty(consultation.Reply) && consultation.ReceiverId == userChat.SenderId)
                        {
                            consultation.Reply = userChat.Message;
                            consultation.Attachment = userChat.Attachment;
                            consultation.ModifiedDate = DateTime.UtcNow;
                            DBEntity.PatientConsultation.Update(consultation).Property(x => x.Id).IsModified = false;
                        }
                        else
                        {
                            DBEntity.PatientConsultation.Add(new PatientConsultation()
                            {
                                SenderId = userChat.SenderId,
                                ReceiverId = userChat.ReceiverId,
                                AddedDate = DateTime.UtcNow,
                                Message = userChat.Message,
                                Attachment = userChat.Attachment,
                                IsRecordDeleted = false,
                                ConsultationCategoryId = userChat.ConsultationCategoryId
                            });
                        }
                    }
                    else
                    {
                        DBEntity.PatientConsultation.Add(new PatientConsultation()
                        {
                            SenderId = userChat.SenderId,
                            ReceiverId = userChat.ReceiverId,
                            AddedDate = DateTime.UtcNow,
                            Message = userChat.Message.Replace("'", "`"),
                            IsRecordDeleted = false
                        });
                    }
                }
                await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> SavePayment(PaymentInfo payment)
        {
            using (var transcation = DBEntity.Database.BeginTransaction())
            {
                try
                {
                    if (payment != null)
                    {
                        DBEntity.PaymentDetails.Add(new PaymentDetails()
                        {
                            UserId = payment.UserId,
                            InvoiceNumber = payment.TxnID,
                            PaymentDate = payment.PaymentDate,
                            Amount = payment.Amount,
                            PaymentType = payment.PaymentType,
                            AddedBy = payment.UserId,
                            AddedDate = DateTime.UtcNow,
                            Status = payment.Status
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
        }
        public async Task<int> CheckMedi(string UserId, string ConsultationId, int ConsultationCategoryId)
        {
            try
            {
                var resultset = await DBEntity.CheckMedi.FromSqlInterpolated($"select dbo.CheckMedi({UserId},{ConsultationId},{ConsultationCategoryId}) AS Result").FirstOrDefaultAsync().ConfigureAwait(true);
                return resultset.Result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> UpdatePatientOrderStatus(string UserId, int StatusId, int ConsultationCategoryId)
        {
            using (var transcation = DBEntity.Database.BeginTransaction())
            {
                try
                {
                    var consultations = await DBEntity.Consultation
                        .Where(x => x.ConsultationCategoryId == ConsultationCategoryId && x.IsRecordDeleted == false && x.IsCompleted==true && x.UserId== UserId)
                        .OrderByDescending(x => x.AddedDate)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(true);

                    var shipping = await (from s in DBEntity.ShippingInformation
                                          where s.IsRecordDeleted == false && s.UserId == UserId && s.ConsultationId == consultations.ConsultationId
                                          orderby s.Id descending
                                          select s)
                                    .FirstOrDefaultAsync().ConfigureAwait(true);
                    shipping.Status = StatusId;
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
        }
        public async Task<bool> VerifyEmail(string Email)
        {
            try
            {
                var list = await (from u in DBEntity.AspNetUsers where u.IsRecordDelete == false && u.Email == Email select u.Email).ToListAsync().ConfigureAwait(true);
                if (list.Count > 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> SendOTP(string UserId, string Email)
        {
            using (var transcation = DBEntity.Database.BeginTransaction())
            {
                string verificationOTP = Comman.GenerateOTP(6);
                try
                {
                    var prevRecord = await (from s in DBEntity.OtpLog where s.IsRecordDeleted == false select s).FirstOrDefaultAsync().ConfigureAwait(true);
                    if (prevRecord != null)
                    {
                        prevRecord.IsRecordDeleted = true;
                        prevRecord.ModifiedDate = DateTime.UtcNow;
                        DBEntity.OtpLog.Update(prevRecord).Property(x => x.Id).IsModified = false;
                    }

                    DBEntity.OtpLog.Add(new OtpLog()
                    {
                        AddedDate = DateTime.UtcNow,
                        IsRecordDeleted = false,
                        IsUsed = false,
                        Otp = verificationOTP,
                        UserId = UserId,
                        ValidDate = DateTime.UtcNow.AddMinutes(1440),
                        ValidTimeInMinute = 1440
                    });
                    await DBEntity.SaveChangesAsync().ConfigureAwait(true);
                    transcation.Commit();
                    return new CrudResult() { ErroCode = "", ErrorMassage = verificationOTP, Failed = false, Succeeded = true };
                }
                catch (Exception ex)
                {
                    transcation.Rollback();
                    return new CrudResult() { ErroCode = "", ErrorMassage = ex.Message, Failed = true, Succeeded = false };
                }
            }
        }
        public async Task<bool> QuestionCountIsOne(string userId,int consultationCategoryId)
        {
            try
            {
                var query = await (from a in DBEntity.AspUserAnswerMapping where a.IsRecordDeleted == false 
                                   && a.UserId == userId 
                                   && a.ConsultationCategoryId == consultationCategoryId select a.Id)
                    .ToListAsync()
                    .ConfigureAwait(true);
                if (query.Count > 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> SetConsultation(string userId, string consultationId, int consulationCategoryId)
        {
            using var transcation = DBEntity.Database.BeginTransaction();
            try
            {
                var consultation = await DBEntity.Consultation.Where(x => x.IsCompleted == false 
                && x.IsRecordDeleted == false 
                && x.UserId == userId 
                && x.ConsultationCategoryId == consulationCategoryId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);

                if (consultation == null)
                {
                    DBEntity.Consultation.Add(new Consultation()
                    {
                        AddedBy = userId,
                        AddedDate = DateTime.UtcNow,
                        ConsultationCategoryId = consulationCategoryId,
                        ConsultationId = consultationId,
                        IsCompleted = false,
                        IsStarted = true,
                        UserId = userId,
                        IsRecordDeleted = false,
                    });
                }
                DBEntity.SaveChanges();
                transcation.Commit();
                return new CrudResult() { ErroCode = "", ErrorMassage = "", Failed = false, Succeeded = true };
            }
            catch (Exception)
            {
                transcation.Rollback();
                throw;
            }
        }
        public async Task<List<ConsultationCategoryView>> GetConsultation()
        {
            try
            {
                var listConsultation = await DBEntity.ConsultationCategory
                    .Where(x => x.IsRecordDeleted == false)
                    .ToListAsync()
                    .ConfigureAwait(true);
                var returnList = new List<ConsultationCategoryView>();
                foreach (var item in listConsultation)
                {
                    returnList.Add(new ConsultationCategoryView()
                    {
                        Id = item.Id,
                        Name = item.FullName,
                        Status = item.IsActived
                    });
                }
                return returnList;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<ConsultationView>> GetConsultation(string userId)
        {
            try
            {
                var consultationViews = new List<ConsultationView>();
                var consultations = await DBEntity.ConsultationCategory.Where(x => x.IsRecordDeleted == false).ToListAsync().ConfigureAwait(true);
                var consultationResult = await DBEntity.Consultation
                    .Where(x => x.UserId == userId && x.IsRecordDeleted == false)
                    .OrderBy(x => x.Id)
                    .ToListAsync()
                    .ConfigureAwait(true);
                var joinQueryResult = (from c in consultations
                                       join cr in consultationResult on c.Id equals cr.ConsultationCategoryId into cc
                                       from cr in cc.DefaultIfEmpty()
                                       select new
                                       {
                                           ConsultationCategoryId = c.Id,
                                           IsStarted = cr == null ? false : cr.IsStarted,
                                           IsCompleted = cr == null ? false : cr.IsCompleted,
                                           EnabledDate = cr == null ? null : cr.NextRefillDate,
                                           Name = c.FullName,
                                           IsActive = c.IsActived
                                       }).ToList();

                foreach (var item in joinQueryResult)
                {
                    consultationViews.Add(new ConsultationView()
                    {
                        ConsultationCategoryId = item.ConsultationCategoryId,
                        IsStarted = item.IsStarted,
                        IsCompleted = item.IsCompleted,
                        EnabledDate = item.EnabledDate,
                        Name = item.Name,
                        Status = item.IsActive
                    });
                }
                return consultationViews;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<Consultation> GetConsultation(string userId, int consultationCategoryId)
        {
            try
            {
                var consultationResult = await DBEntity.Consultation
                    .Where(x => x.UserId == userId && x.ConsultationCategoryId == consultationCategoryId && x.IsRecordDeleted == false)
                    .OrderBy(x => x.Id)
                    .LastOrDefaultAsync()
                    .ConfigureAwait(true);

                return consultationResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<ConsultationView>> GetRecentConsultation(string userId)
        {
            try
            {
                var consultations = new List<ConsultationView>();
                var recentConsultationCategoryId = await DBEntity.PatientConsultation
                    .Where(x => x.IsRecordDeleted == false && (x.SenderId == userId || x.ReceiverId == userId))
                    .OrderByDescending(x => x.Id)
                    .Select(x => x.ConsultationCategoryId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                var consultationList = await DBEntity.ConsultationCategory
                    .Join(DBEntity.Consultation, cc => cc.Id, c => c.ConsultationCategoryId, (cc, c) => new { cc, c })
                    .Where(x => x.c.IsRecordDeleted == false && x.cc.IsRecordDeleted == false && x.c.UserId == userId
                     && x.c.IsCompleted == true)
                    .Select(x => new
                    {
                        ConsultationCategoryId = x.c.ConsultationCategoryId,
                        ConsultationCategoryName = x.cc.FullName
                    })
                    .ToListAsync()
                    .ConfigureAwait(true);
                if (recentConsultationCategoryId != 0)
                {
                    var recentRecord = consultationList.Where(x => x.ConsultationCategoryId == recentConsultationCategoryId).FirstOrDefault();
                    consultations.Add(new ConsultationView
                    {
                        ConsultationCategoryId = recentRecord.ConsultationCategoryId,
                        Name = recentRecord.ConsultationCategoryName
                    });
                    consultationList.Remove(recentRecord);
                }
                foreach (var item in consultationList)
                {
                    consultations.Add(new ConsultationView
                    {
                        ConsultationCategoryId = item.ConsultationCategoryId,
                        Name = item.ConsultationCategoryName
                    });
                }
                return consultations;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<bool> UserConsultationStatus(string userId)
        {
            try
            {
                var result = await DBEntity.Consultation
                    .Where(x => x.IsRecordDeleted == false && x.UserId == userId &&
                    ((x.ConsultationCategoryId == (int)ConsultationType.ErectileDysfunction && x.IsCompleted==true) ||
                     (x.ConsultationCategoryId == (int)ConsultationType.MedicationRefill && x.IsCompleted == true) ||
                     (x.ConsultationCategoryId == (int)ConsultationType.HairLoss && x.IsCompleted == true)))
                    .ToListAsync()
                    .ConfigureAwait(true);
                if (result.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> GetCompleteConsultationStatus(string userId)
        {
            try
            {
                var consultations = await DBEntity.Consultation
                   .Where(x => x.IsRecordDeleted == false && x.UserId == userId && x.IsCompleted == false)
                   .Select(x=>x.ConsultationId)
                   .ToListAsync()
                   .ConfigureAwait(true);

                if (consultations.Count > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CrudResult> SetOrderIdConsultation(string orderId,string consultationId)
        {
            using var transaction = DBEntity.Database.BeginTransaction();
            try
            {
                var information = await DBEntity.ShippingInformation
                    .Where(x => x.IsRecordDeleted == false && x.ConsultationId == consultationId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                information.OrderId = orderId;
                DBEntity.ShippingInformation.Update(information).Property(x => x.Id).IsModified = false;
                DBEntity.SaveChanges();
                transaction.Commit();
                return new CrudResult() { ErrorMassage = string.Empty, Succeeded = true, Failed = false };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new CrudResult() { ErrorMassage = ex.Message, Succeeded = false, Failed = true };
            }
        }
        public async Task<CrudResult> SetOrderIdChat(string orderId, string userId)
        {
            using var transaction = DBEntity.Database.BeginTransaction();
            try
            {
                var payment = await DBEntity.PaymentDetails
                    .Where(x => x.UserId == userId)
                    .OrderBy(x=>x.Id)
                    .LastOrDefaultAsync()
                    .ConfigureAwait(true);
                payment.InvoiceNumber = orderId;
                DBEntity.PaymentDetails.Update(payment).Property(x => x.Id).IsModified = false;
                DBEntity.SaveChanges();
                transaction.Commit();
                return new CrudResult() { ErrorMassage = string.Empty, Succeeded = true, Failed = false };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new CrudResult() { ErrorMassage = ex.Message, Succeeded = false, Failed = true };
            }
        }
        public async Task<List<ConsultationCustomMedication>> GetMedications(int consultationCategoryId)
        {
            try
            {
                var returnlist = new List<ConsultationCustomMedication>();
                var result = await DBEntity.ConsultationMedication
                    .Where(x => x.IsDeleted == false && x.ConsultationCategoryId == consultationCategoryId)
                    .Select(x => new
                    {
                        MedicationId = x.Id,
                        MedicationName = x.Medication,
                        Priority =x.Priority
                    })
                    .ToListAsync()
                    .ConfigureAwait(true);
                foreach(var item in result)
                {
                    returnlist.Add(new ConsultationCustomMedication()
                    {
                        MedicationId = item.MedicationId,
                        MedicationName = item.MedicationName,
                        Priority = item.Priority
                    });
                }
                return returnlist;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<ConsultationCategoryDetailView>> GetDetailViews(int consultationCategoryId, bool IsHomeDelivery)
        {
            try
            {
                var returnlist = new List<ConsultationCategoryDetailView>();
                var result = await DBEntity.ConsultationCategoryDetail
                    .Where(x => x.IsDeleted == false && x.ConsultationCategoryId == consultationCategoryId && x.IsHomeDelivery == IsHomeDelivery)
                    .Select(x => new
                    {
                        x.Id,
                        x.Refill,
                        x.RefillDay,
                        x.MedicationRate,
                        x.IsHomeDelivery,
                        x.ConsultationCategoryId
                    })
                    .ToListAsync()
                    .ConfigureAwait(true);
                foreach (var item in result)
                {
                    returnlist.Add(new ConsultationCategoryDetailView()
                    {
                        Id = item.Id,
                        Refill = item.Refill,
                        RefillDay = item.RefillDay,
                        MedicationRate = item.MedicationRate,
                        IsHomeDelivery = item.IsHomeDelivery,
                        ConsultationCategoryId = item.ConsultationCategoryId
                    });
                }
                return returnlist;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> IsConsultationMedicationExist(int? medicationId)
        {
            try
            {
                var result = await DBEntity.ConsultationMedication.Where(x => x.Id == medicationId)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                if (result == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> IsConsultationDetailExist(int? detailId)
        {
            try
            {
                var result = await DBEntity.ConsultationCategoryDetail.Where(x => x.Id == detailId)
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                if (result == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<CrudResult> SaveTempConsultationMedication(string consultationId,string userId, int medicationId)
        {
            using var transaction = DBEntity.Database.BeginTransaction();
            try
            {
                var tempMedication = await DBEntity.PatientTempMedication
                    .Where(x => x.ConsultationId == consultationId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                if (tempMedication == null)
                {
                    await DBEntity.PatientTempMedication.AddAsync(new PatientTempMedication()
                    {
                        AddedBy = userId,
                        AddedDate = DateTime.UtcNow,
                        ConsultationId = consultationId,
                        ConsultationMedicationId = medicationId,
                        IsRecordDeleted = false,
                    }).ConfigureAwait(true);
                }
                else
                {
                    tempMedication.ConsultationMedicationId = medicationId;
                    tempMedication.ModifiedBy = userId;
                    tempMedication.ModifiedDate = DateTime.UtcNow;
                    tempMedication.IsRecordDeleted = false;
                    DBEntity.PatientTempMedication.Update(tempMedication).Property(x => x.Id).IsModified = false;
                }
                DBEntity.SaveChanges();
                transaction.Commit();
                return new CrudResult() { ErrorMassage = string.Empty, Succeeded = true, Failed = false };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new CrudResult() { ErrorMassage = ex.Message, Succeeded = false, Failed = true };
            }
        }
        public async Task<CrudResult> SaveTempConsultationDetail(string consultationId, string userId, int detailId)
        {
            using var transaction = DBEntity.Database.BeginTransaction();
            try
            {
                var tempDetail = await DBEntity.PatientTempDetail
                    .Where(x => x.ConsultationId == consultationId)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(true);
                if (tempDetail == null)
                {
                    await DBEntity.PatientTempDetail.AddAsync(new PatientTempDetail()
                    {
                        AddedBy = userId,
                        AddedDate = DateTime.UtcNow,
                        ConsultationId = consultationId,
                        ConsultationDetailId = detailId,
                        IsRecordDeleted = false,
                    }).ConfigureAwait(true);
                }
                else
                {
                    tempDetail.ConsultationDetailId = detailId;
                    tempDetail.ModifiedBy = userId;
                    tempDetail.ModifiedDate = DateTime.UtcNow;
                    tempDetail.IsRecordDeleted = false;
                    DBEntity.PatientTempDetail.Update(tempDetail).Property(x => x.Id).IsModified = false;
                }
                DBEntity.SaveChanges();
                transaction.Commit();
                return new CrudResult() { ErrorMassage = string.Empty, Succeeded = true, Failed = false };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new CrudResult() { ErrorMassage = ex.Message, Succeeded = false, Failed = true };
            }
        }
        public async Task<CrudResult> SavePharmacyDetail(PharmacyInformationModel model,string consultationId,string userId)
        {
            var transaction = DBEntity.Database.BeginTransaction();
            try
            {
                var pharmacy = await DBEntity.PharmacyInformation.Where(x=>x.ConsultationId ==consultationId).FirstOrDefaultAsync().ConfigureAwait(true);
                if (pharmacy == null)
                {
                    await DBEntity.PharmacyInformation.AddAsync(new PharmacyInformation()
                    {
                        AddedBy = userId,
                        AddedDate = DateTime.UtcNow,
                        AddressLine1 = model.AddressLine1,
                        AddressLine2 = model.AddressLine2,
                        City = model.City,
                        ConsultationId = consultationId,
                        IsRecordDeleted = false,
                        ModifiedBy = userId,
                        ModifiedDate = DateTime.UtcNow,
                        PharmacyName = model.PharmacyName,
                        PhoneNumber = model.PhoneNumber,
                        StateId = model.State,
                        UserId = userId,
                        ZipCode = model.ZipCode
                    }).ConfigureAwait(true);
                }
                else
                {
                    pharmacy.AddressLine1 = model.AddressLine1;
                    pharmacy.AddressLine2 = model.AddressLine2;
                    pharmacy.City = model.City;
                    pharmacy.ModifiedBy = userId;
                    pharmacy.ModifiedDate = DateTime.UtcNow;
                    pharmacy.PharmacyName = model.PharmacyName;
                    pharmacy.PhoneNumber = model.PhoneNumber;
                    pharmacy.StateId = model.State;
                    pharmacy.ZipCode = model.ZipCode;
                    pharmacy.IsRecordDeleted = false;
                    DBEntity.PharmacyInformation.Update(pharmacy).Property(x => x.PharmacyId).IsModified = false;
                }
                DBEntity.SaveChanges();
                transaction.Commit();
                return new CrudResult() { ErrorMassage = string.Empty, Succeeded = true, Failed = false };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return new CrudResult() { ErrorMassage = ex.Message, Succeeded = false, Failed = true };
            }
        }
    }
}
